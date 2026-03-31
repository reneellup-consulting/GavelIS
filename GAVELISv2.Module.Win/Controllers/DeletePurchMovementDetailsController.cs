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
    public partial class DeletePurchMovementDetailsController : ViewController
    {
        private PurchasesMovementAnalysis reporter;
        private SimpleAction deletePurchMovementDetailsAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public DeletePurchMovementDetailsController()
        {
            this.TargetObjectType = typeof(PurchasesMovementAnalysis);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.DeletePurchMovementDetails", this.GetType().
            Name);
            this.deletePurchMovementDetailsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.deletePurchMovementDetailsAction.Caption = "Delete Details";
            this.deletePurchMovementDetailsAction.Execute += new
            SimpleActionExecuteEventHandler(deletePurchMovementDetailsAction_Execute);
            this.deletePurchMovementDetailsAction.Executed += new EventHandler<
            ActionBaseEventArgs>(deletePurchMovementDetailsAction_Executed);
            UpdateActionState(false);
        }
        private void deletePurchMovementDetailsAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            reporter = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PurchasesMovementAnalysis;
            //ObjectSpace.CommitChanges();
            if (reporter.PurchasesMovementBuffDetails.Count == 0)
            {
                throw new UserFriendlyException("There are no details to delete.");
            }
            int delCount = reporter.PurchasesMovementBuffSummaries.Count + reporter.PurchasesMovementBuffDetails.Count;
            _FrmProgress = new ProgressForm("Deleting details...", delCount,
            "Deleting detail {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(reporter);
            _FrmProgress.ShowDialog();
        }

        private void deletePurchMovementDetailsAction_Executed(object sender,
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
            PurchasesMovementAnalysis trans = (PurchasesMovementAnalysis)e.Argument;
            try
            {
                foreach (PurchasesMovementSummary item in trans.PurchasesMovementBuffSummaries)
                {
                    index++;
                    #region Algorithms here...

                    PurchasesMovementSummary pmb = session.GetObjectByKey<PurchasesMovementSummary>(item.Oid);
                    pmb.Delete();

                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Delete {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }

                foreach (PurchasesMovementBuffer item in trans.PurchasesMovementBuffDetails)
                {
                    index++;
                    #region Algorithms here...

                    PurchasesMovementBuffer pmb = session.GetObjectByKey<PurchasesMovementBuffer>(item.Oid);
                    pmb.Delete();

                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Delete {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.PurchasesMovementBuffSummaries.Count() + trans.PurchasesMovementBuffDetails.Count())
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
                    "Deleting details has been cancelled", "Cancelled",
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
            deletePurchMovementDetailsAction.
                Enabled.SetItemValue("Deleting details", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
