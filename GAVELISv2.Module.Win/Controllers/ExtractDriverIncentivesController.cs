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
    public partial class ExtractDriverIncentivesController : ViewController
    {
        private SimpleAction extractDriverIncentivesAction;
        private DriverPayrollBatch _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ExtractDriverIncentivesController()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.ExtractDriverIncentives";
            this.extractDriverIncentivesAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.extractDriverIncentivesAction.Caption = "Extract Incentives";
            this.extractDriverIncentivesAction.Execute += new
            SimpleActionExecuteEventHandler(
            ExtractDriverIncentivesAction_Execute);
        }
        private void ExtractDriverIncentivesAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch;

            ObjectSpace.CommitChanges();

            var distinctDrivers = _DriverPayrollBatch.DriverPayrollTrips.Select(o => o.Driver).Distinct();

            if (distinctDrivers.Count() == 0)
            {
                throw new UserFriendlyException("There are no drivers found");
            }

            _FrmProgress = new ProgressForm("Generating data...", distinctDrivers.Count(),
            "Drivers processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(distinctDrivers);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<Employee> _drivers = (IEnumerable<Employee>)e.Argument;
            try
            {
                foreach (var item in _drivers)
                {
                    index++;
                    Employee driver = session.GetObjectByKey<Employee>(item.Oid);
                    _message = string.Format("Processing {0} successful.",
                    driver.No);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    DriverPayrollBatch batch = session.GetObjectByKey<DriverPayrollBatch>(_DriverPayrollBatch.Oid);
                    foreach (EmpIncentives incentive in driver.EmpIncentives)
                    {
                        EmpIncentives iEnt = session.GetObjectByKey<EmpIncentives>(incentive.Oid);
                        PayrollAdjustment padj = null;
                        var data = _DriverPayrollBatch.PayrollAdjustments.Where(o => o.EmpIncentiveId.Oid == incentive.Oid).FirstOrDefault();
                        if (data == null)
                        {
                            padj = ReflectionHelper.CreateObject<PayrollAdjustment>(session);
                            padj.PayrollBatchID = batch;
                            padj.Employee = driver;
                            padj.EmpIncentiveId = iEnt;
                            padj.AdjustmentType = iEnt.AdjustmentType;
                            padj.Explanation = iEnt.Explanation;
                            padj.Amount = iEnt.Amount;
                            padj.Save();
                        }
                        else
                        {
                            padj = session.GetObjectByKey<PayrollAdjustment>(data.Oid);
                            padj.PayrollBatchID = batch;
                            padj.Employee = driver;
                            padj.EmpIncentiveId = iEnt;
                            padj.AdjustmentType = iEnt.AdjustmentType;
                            padj.Explanation = iEnt.Explanation;
                            padj.Amount = iEnt.Amount;
                            padj.Save();
                        }
                    }

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
                if (index == _drivers.Count())
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
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
                    "Extraction of incentives is cancelled.", "Cancelled",
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
                    "Incentives has been successfully extracted.");
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
