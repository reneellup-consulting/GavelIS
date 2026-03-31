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
    public partial class ApproveSelectedVoucherController : ViewController
    {
        private SimpleAction approveSelectedVoucherAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ApproveSelectedVoucherController()
        {
            this.TargetObjectType = typeof(MultiCheckVoucher);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ApproveSelectedVoucher", this.GetType().
            Name);
            this.approveSelectedVoucherAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.approveSelectedVoucherAction.Caption = "Approve Selected";
            this.approveSelectedVoucherAction.Execute += new
            SimpleActionExecuteEventHandler(ApproveSelectedVoucher_Execute);
            this.approveSelectedVoucherAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ApproveSelectedVoucher_Executed);
            this.approveSelectedVoucherAction.ConfirmationMessage =
            "Do you really want to approve the selected vouchers?";
            UpdateActionState(false);
        }
        private void ApproveSelectedVoucher_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no vouchers selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList genHeaders = null;
            genHeaders = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = genHeaders.Count;
            _FrmProgress = new ProgressForm("Approving vouchers...", count,
            "Approving vouchers {0} of {1} ");
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
        private void ApproveSelectedVoucher_Executed(object sender,
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
            IList trans = (IList)e.Argument;
            try
            {
                foreach (MultiCheckVoucher item in trans)
                {
                    index++;

                    #region Algorithms here...

                    MultiCheckVoucher thisVoucher = session.GetObjectByKey<MultiCheckVoucher>(
                    item.Oid);
                    if (thisVoucher.Status == MultiCheckStatusEnum.Approved)
                    {
                        throw new ApplicationException(string.Format("Voucher {0} was already approved!", thisVoucher.CheckVoucherNo));
                    }
                    thisVoucher.Status = MultiCheckStatusEnum.Approved;
                    thisVoucher.Save();
                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Approving voucher {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
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
                    "Approving selected vouchers has been cancelled", "Cancelled",
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
                    " has been successfully approved");

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
            approveSelectedVoucherAction.
                Enabled.SetItemValue("Approving vouchers", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
