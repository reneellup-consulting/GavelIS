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
    public partial class ForwardToCurrentDolePayrollController : ViewController
    {
        private SimpleAction forwardToCurrentDolePayrollAction;
        private DriverPayrollBatch3 driverPayrollBatch;
        private DolefilTripDetail _TripDetail;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ForwardToCurrentDolePayrollController()
        {
            this.TargetObjectType = typeof(DolefilTripDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ForwardToCurrentDolePayrollActionId";
            this.forwardToCurrentDolePayrollAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.forwardToCurrentDolePayrollAction.TargetObjectsCriteria = "[TripID.SourceType.Code] = 'DF'";
            this.forwardToCurrentDolePayrollAction.Caption = "Forward to DF Payroll";
            this.forwardToCurrentDolePayrollAction.Execute += new SimpleActionExecuteEventHandler(ForwardToCurrentDolePayrollAction_Execute);
        }

        void ForwardToCurrentDolePayrollAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            driverPayrollBatch = ObjectSpace.GetObjects<DriverPayrollBatch3>().LastOrDefault();
            if (driverPayrollBatch == null)
            {
                throw new UserFriendlyException("No driver payroll batch for Dole trips found in the system.");
            }
            else if (driverPayrollBatch.Status != PayrollBatchStatusEnum.Current)
            {
                throw new UserFriendlyException("The last driver payroll batch is not open for processing anymore.");
            }
            IList selectedObjects = this.View.SelectedObjects;
            if (selectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no lines selected.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Marking/Unmarking...", selectedObjects.Count,
                        "Line marked {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(selectedObjects);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                DriverPayroll3 dpr = null;
                DriverPayrollBatch3 dpb = session.GetObjectByKey<DriverPayrollBatch3>(driverPayrollBatch.Oid);
                foreach (DolefilTripDetail item in trans)
                {
                    index++;
                    _message = string.Format("Forwarding source {0} succesfull.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here

                    DolefilTripDetail dftd = session.GetObjectByKey<DolefilTripDetail>(item.Oid);
                    DriverRegistry odr = null;
                    if (dftd.TripID != null && dftd.TripID.DriverRegistrations.Count > 0)
                    {
                        odr = session.GetObjectByKey<DriverRegistry>(dftd.TripID.DriverRegistrations.Last().Oid);
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
                        odr.DolePayroll = true;
                        odr.Status = DriverRegistryStatusEnum.Processed;
                        odr.PayrollBatchID3 = dpb;
                        odr.Save();

                        dpr = dpb.DriverPayrolls3.Where(o => o.Employee == odr.Driver).FirstOrDefault();
                        if (dpr == null)
                        {
                            dpr = ReflectionHelper.CreateObject<DriverPayroll3>(session);
                            dpr.PayrollBatchID = dpb;
                        }

                        dpr.Employee = odr.Driver;

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

                    dpb.Processed = true;
                    dpb.Save();

                    #endregion

                    CommitUpdatingSession(session);

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
                if (index == trans.Count)
                {
                    e.Result = index;
                }
                session.Dispose();
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
                    "Forwarding is cancelled.", "Cancelled",
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
                    "Forwarding has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
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
