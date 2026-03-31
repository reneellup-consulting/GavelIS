using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class RemoveNotIncludedInDriverPayrollBatchController : ViewController
    {
        private DriverPayrollBatch2 driverPayrollBatch;
        private SimpleAction removeNotIncludedAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public RemoveNotIncludedInDriverPayrollBatchController()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.RemoveNotIncluded", this.GetType().Name);
            this.removeNotIncludedAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.removeNotIncludedAction.Caption = "Remove not Included";
            this.removeNotIncludedAction.Execute += new
            SimpleActionExecuteEventHandler(removeNotIncludedAction_Execute);
            this.removeNotIncludedAction.Executed += new EventHandler<
            ActionBaseEventArgs>(removeNotIncludedAction_Executed);
            this.removeNotIncludedAction.ConfirmationMessage =
            "Do you really want to remove some entries?";
            UpdateActionState(false);
        }

        private void removeNotIncludedAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            //receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            //CurrentObject as Receipt;
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.DetailView))
            {
                driverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
                CurrentObject as DriverPayrollBatch2;
            }
            ObjectSpace.CommitChanges();
            var notIncluded = driverPayrollBatch.DriverPayrolls2.Where(o => o.Include != true);
            if (notIncluded.Count() == 0)
            {
                XtraMessageBox.Show("There are no entries to remove",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = notIncluded.Count();
            _FrmProgress = new ProgressForm("Removing entries...", count,
            "Removing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(notIncluded);
            _FrmProgress.ShowDialog();
        }
        private void removeNotIncludedAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //ObjectSpace.ReloadObject(receipt);
            //ObjectSpace.Refresh();
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<DriverPayroll2> lines = (IEnumerable<DriverPayroll2>)e.Argument;
            try
            {
                foreach (DriverPayroll2 item in lines)
                {
                    index++;
                    #region Algorithms here...

                    DriverPayroll2 dp = session.GetObjectByKey<DriverPayroll2>(item.Oid);
                    dp.Delete();

                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Remove {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == lines.Count())
                {
                    e.Result = index;
                }
                session.Dispose();
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
                    "Removing entries has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully removed");
                    ObjectSpace.ReloadObject(driverPayrollBatch);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        private void UpdateActionState(bool inProgress)
        {
            removeNotIncludedAction.
                Enabled.SetItemValue("Removing entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
