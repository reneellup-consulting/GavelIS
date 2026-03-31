using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ProcessDriverDolePayrollController : ViewController
    {
        private SimpleAction processDriverPayroll;
        private DriverPayrollBatch3 _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ProcessDriverDolePayrollController()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch3);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch3.ProcessDriverPayroll";
            this.processDriverPayroll = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.processDriverPayroll.TargetObjectsCriteria = "[Status] = 'Current'";
            this.processDriverPayroll.Caption = "Process Trips";
            this.processDriverPayroll.Execute += new
            SimpleActionExecuteEventHandler(
            processDriverPayroll_Execute);
        }

        private void processDriverPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as DriverPayrollBatch3;

            foreach (var item in _DriverPayrollBatch.DriverPayrolls3)
            {
                item.ResetZero();
                item.Save();
            }

            ObjectSpace.CommitChanges();

            IList<DriverRegistry> included;
            if (!string.IsNullOrEmpty(_DriverPayrollBatch.BatchType.RegistryFilter))
            {
                included = ObjectSpace.GetObjects<DriverRegistry>(CriteriaOperator.Parse(string.Format("{0} And Not [Status] In ('Paid') And [DolePayroll] = True", _DriverPayrollBatch.BatchType.RegistryFilter)));
            }
            else
            {
                included = ObjectSpace.GetObjects<DriverRegistry>(CriteriaOperator.Parse(string.Format("{0} And Not [Status] In ('Paid') And [DolePayroll] = True", "[TripID.SourceType.Code] == 'DF'")));
            }
            _FrmProgress = new ProgressForm("Processing data...", included.Count,
            "Data processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(included);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IOrderedEnumerable<DriverRegistry> _included = ((IList<DriverRegistry>)e.Argument).OrderBy(o=>o.Driver).OrderBy(o=>o.ReferenceNo);
            try
            {
                DriverPayrollBatch3 dpb = session.GetObjectByKey<DriverPayrollBatch3>(_DriverPayrollBatch.Oid);
                foreach (var item in _included)
                {
                    index++;
                    _message = string.Format("Processing {0} succesfull.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    DriverRegistry odr = session.GetObjectByKey<DriverRegistry>(item.Oid);
                    StringBuilder sb = new StringBuilder();
                    bool hasError = false;
                    sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", odr.Oid);
                    if (odr.Tariff == null)
                    {
                        hasError = true;
                        sb.Append("Tariff is not specified and ");
                    }
                    if (odr.Driver.DriverClassification == null)
                    {
                        hasError = true;
                        sb.AppendFormat("Driver {0} Driver Classification is not specified", odr.Driver.Name);
                    }
                    if (hasError)
                    {
                        sb.Remove(sb.Length - 5, 5);
                        sb.Append(".");
                        throw new ApplicationException(sb.ToString());
                    }

                    odr.Status = DriverRegistryStatusEnum.Processed;
                    odr.PayrollBatchID3 = dpb;
                    odr.Save();

                    DriverPayroll3 dpr = dpb.DriverPayrolls3.Where(o => o.Employee == odr.Driver).FirstOrDefault();
                    if (dpr == null)
                    {
                        dpr = ReflectionHelper.CreateObject<DriverPayroll3>(session);
                        dpr.PayrollBatchID = dpb;
                    }

                    dpr.Employee = odr.Driver;

                    #region Trip processing here...
                    foreach (DolefilTripDetail dftd in odr.TripID.DolefilTripDetails)
                    {
                        DriverPayrollTripLine3 dptl3 = dpr.DriverPayrollTripLines.Where(o => o.Driver == dpr.Employee && o.OriginDestination == dftd.CommRoute).FirstOrDefault();
                        if (dptl3 != null)
                        {
                            dptl3.Reprocessing = true;
                        }
                        if (dptl3 == null)
                        {
                            dptl3 = ReflectionHelper.CreateObject<DriverPayrollTripLine3>(session);
                            dptl3.DriverPayrollID = dpr;
                            dptl3.DriverRegistryId = odr;
                            dptl3.TripId = odr.TripID as DolefilTrip;
                            dptl3.Driver = odr.Driver;
                            dptl3.OriginDestination = dftd.CommRoute;
                            dptl3.Category = dftd.Category;
                            dptl3.Manual = false;
                        }
                        if (!dptl3.Altered)
                        {
                            dptl3.NoOfTrips += dftd.CommCount;
                            dptl3.Commission += dftd.Commission;
                        }
                        dptl3.Reprocessing = false;
                        dptl3.Save();
                        dftd.Dptl3Id = dptl3;
                        dftd.Dprl3Id = dpr;
                        dftd.DriverRegistryId = odr;
                        dftd.Save();
                    }

                    foreach (DriverPayrollTripLine3 dptl in dpr.DriverPayrollTripLines)
                    {
                        dptl.Reprocessing = true;
                        //if ((dptl.NoOfTrips - Math.Truncate(dptl.NoOfTrips)) == 0.50m)
                        //{
                        //    var gcomm = dptl.Commission / dptl.NoOfTrips;
                        //    // According to LVG, she allow .5 for a reason
                        //    //dptl.NoOfTrips = Math.Truncate(dptl.NoOfTrips) + 0.50m;
                        //    dptl.NoOfTrips = Math.Truncate(dptl.NoOfTrips);
                        //    dptl.Commission = gcomm * dptl.NoOfTrips;
                        //    dptl.Save();
                        //}
                        dptl.Reprocessing = false;
                        dptl.Save();
                    }

                    for (int i = dpr.DriverPayrollTripLines.Count - 1; i >= 0; i--)
                    {
                        if (dpr.DriverPayrollTripLines[i].NoOfTrips == 0)
                        {
                            dpr.DriverPayrollTripLines[i].Delete();
                        }
                    }
                    #endregion

                    dpb.Processed = true;
                    dpb.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }
            }
            finally
            {
                if (index == _included.Count<DriverRegistry>())
                {
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }

        private bool AlterValue(decimal p)
        {
            if (p == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private UnitOfWork CreateUpdatingSession()
        {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session)
        {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session)
        {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                    , new SessionEventArgs(session));
            }
        }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session)
        {
            if
                (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                    SessionEventArgs(session));
            }
        }

        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Generation of payroll data is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Payroll data has been successfully generated.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
