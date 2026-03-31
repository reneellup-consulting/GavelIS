using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
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
    public partial class DeleteSelectedWOController : ViewController
    {
        private SimpleAction deleteSelectedWOAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public DeleteSelectedWOController()
        {
            this.TargetObjectType = typeof(WorkOrder);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.DeleteSelectedWO", this.GetType().
            Name);
            this.deleteSelectedWOAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.deleteSelectedWOAction.Caption = "Delete Selected WO";
            this.deleteSelectedWOAction.Execute += new
            SimpleActionExecuteEventHandler(DeleteSelectedWOAction_Execute);
            this.deleteSelectedWOAction.Executed += new EventHandler<
            ActionBaseEventArgs>(DeleteSelectedWOAction_Executed);
            this.deleteSelectedWOAction.ConfirmationMessage =
            "Do you really want to delete selected transactions?";
            UpdateActionState(false);
        }
        private void DeleteSelectedWOAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no transactions selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList genHeaders = null;
            genHeaders = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = genHeaders.Count;
            _FrmProgress = new ProgressForm("Deleting transactions...", count,
            "Deleting transactions {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(genHeaders);
            _FrmProgress.ShowDialog();
        }
        private void DeleteSelectedWOAction_Executed(object sender,
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
            //DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                foreach (WorkOrder wo in trans)
                {
                    index++;
                    _message = string.Format("Deleting entry {0}.", index);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    WorkOrder oWo = session.GetObjectByKey<WorkOrder>(wo.Oid);
                    if (oWo.WorkOrderItemDetails.Count > 0 || wo.WorkOrderJobsDetails.Count > 0)
                    {
                        continue;
                    }
                    if (oWo.Status != WorkOrderStatusEnum.Current)
                    {
                        oWo.Status = WorkOrderStatusEnum.Current;
                    }

                    // Delete referenced ServiceOdoRegistry
                    session.ExecuteNonQuery(string.Format("delete ServiceOdoRegistry where WorkOrderId={0}", oWo.Oid));
                    // Delete referenced RequsitionWorksheet.LastCarrySource
                    session.ExecuteNonQuery(string.Format("update RequisitionWorksheet set LastCarrySource=NULL where LastCarrySource={0}", oWo.Oid));
                    // Delete referenced ARRegistry
                    session.ExecuteNonQuery(string.Format("delete ARRegistry where GenJournalID={0}", oWo.Oid));
                    // Delete referenced IncomeAndExpense02
                    session.ExecuteNonQuery(string.Format("delete IncomeAndExpense02 where SourceID={0}", oWo.Oid));

                    oWo.Save();
                    oWo.Delete();
                    CommitUpdatingSession(session);

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
                if (index == trans.Count)
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
                    "Deleting selected transactions has been cancelled", "Cancelled",
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
                    " has been successfully deleted");

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
            deleteSelectedWOAction.
                Enabled.SetItemValue("Receiving transactions", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
