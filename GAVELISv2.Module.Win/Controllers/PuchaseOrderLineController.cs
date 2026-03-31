using System;
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
    public partial class PuchaseOrderLineController : ViewController {
        private SimpleAction holdReleaseAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public PuchaseOrderLineController() {
            this.TargetObjectType = typeof(PurchaseOrderDetail);
            this.TargetViewType = ViewType.ListView;

            #region Hold/Release Line

            this.holdReleaseAction = new SimpleAction(this, "PoLine.HoldRelease", PredefinedCategory.RecordEdit);
            this.holdReleaseAction.Caption = "Hold/Release";
            this.holdReleaseAction.Execute += new SimpleActionExecuteEventHandler(holdReleaseAction_Execute);

            #endregion

        }

        void holdReleaseAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no P.O. line selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            IList lst = null;
            lst = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = lst.Count;
            _FrmProgress = new ProgressForm("Holding/Releasing line...", count,
            "Holding/Releasing line {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(lst);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                foreach (PurchaseOrderDetail item in trans)
                {
                    index++;
                    _message = string.Format("Holding/Releasing line {0} succesfull.",
                    trans.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    PurchaseOrderDetail obj = session.GetObjectByKey<PurchaseOrderDetail>(item.Oid);
                    obj.LineApprovalStatus = item.LineApprovalStatus == POLineStatusEnum.Hold ? POLineStatusEnum.Released : POLineStatusEnum.Hold;
                    obj.Save();
                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    System.Threading.Thread.Sleep(20);
                }
            }
            finally
            {
                if (index == trans.Count)
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
                    "Holding/Releasing line cancelled", "Cancelled",
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
                    " has been successfully hold/released");
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
            this.holdReleaseAction.Enabled.SetItemValue("Holding/Releasing lines", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
