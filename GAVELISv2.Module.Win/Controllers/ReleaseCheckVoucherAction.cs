using System;
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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class ReleaseCheckVoucherAction : ViewController {
        private CheckVoucher checkVoucher;
        private SimpleAction releaseCheckVoucherAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public ReleaseCheckVoucherAction() {
            this.TargetObjectType = typeof(CheckVoucher);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Release", this.GetType().Name);
            this.releaseCheckVoucherAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.releaseCheckVoucherAction.Caption = "Release";
            this.releaseCheckVoucherAction.Execute += new 
            SimpleActionExecuteEventHandler(ReleaseCheckVoucherAction_Execute);
            this.releaseCheckVoucherAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReleaseCheckVoucherAction_Executed);
            this.releaseCheckVoucherAction.ConfirmationMessage = 
            "Do you really want to release this check?";
            UpdateActionState(false);
        }
        private void ReleaseCheckVoucherAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            checkVoucher = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as CheckVoucher;
            switch (checkVoucher.PayToOrder.ContactType) {
                case ContactTypeEnum.Customer:
                    if (((Customer)checkVoucher.PayToOrder).Account == null) {
                        throw new ApplicationException(
                        "Accounts Receivable account must be specified in the chosen " 
                        + checkVoucher.PayToOrder.ContactType + " card");}
                    break;
                case ContactTypeEnum.Vendor:
                    if (((Vendor)checkVoucher.PayToOrder).Account == null) {
                        throw new ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );}
                    break;
                default:
                    break;
            }
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(checkVoucher);
        }
        private void ReleaseCheckVoucherAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(checkPayment);
            //ObjectSpace.Refresh();
        }
        private UnitOfWork CreateUpdatingSession() {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) { 
            if (UpdatingSessionCommitted != null) {UpdatingSessionCommitted(this
                , new SessionEventArgs(session));} }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) { if 
            (UpdatingSessionCreated != null) {UpdatingSessionCreated(this, new 
                SessionEventArgs(session));} }
        //private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            CheckVoucher _checkVoucher = (CheckVoucher)e.Argument;
            CheckVoucher thisCheckVoucher = session.GetObjectByKey<CheckVoucher>
            (_checkVoucher.Oid);
            try {
                // Validate Vendor Accounts Payable
                //if (thisCheckPayment.Vendor == null) {throw new 
                //    ApplicationException("Must specify a vendor");} else {
                //    if (thisCheckPayment.Vendor.Account == null) {throw new 
                //        ApplicationException(
                //        "Accounts Payable account must be specified in the chosen vendor card"
                //        );}
                //}
                if (thisCheckVoucher.PayToOrder == null) {throw new 
                    ApplicationException("Must specify a Payee to the check");}
                //if (thisCheckPayment.PayToOrder.ContactType == ContactTypeEnum.
                //Vendor) {if (((Vendor)thisCheckPayment.PayToOrder).Account == 
                //    null) {throw new ApplicationException(
                //        "Accounts Payable account must be specified in the chosen vendor card"
                //        );}}
                //if (thisCheckPayment.PayToOrder.ContactType != ContactTypeEnum.
                //Customer) {if (((Customer)thisCheckPayment.PayToOrder).Account 
                //    == null == null) {throw new ApplicationException(
                //        "Accounts Receivable account must be specified in the chosen " 
                //        + thisCheckPayment.PayToOrder.ContactType + " card");}}
                switch (thisCheckVoucher.PayToOrder.ContactType) {
                    case ContactTypeEnum.Customer:
                        if (((Customer)thisCheckVoucher.PayToOrder).Account == 
                        null) {throw new ApplicationException(
                            "Accounts Receivable account must be specified in the chosen " 
                            + thisCheckVoucher.PayToOrder.ContactType + " card")
                            ;}
                        break;
                    case ContactTypeEnum.Vendor:
                        if (((Vendor)thisCheckVoucher.PayToOrder).Account == 
                        null) {throw new ApplicationException(
                            "Accounts Payable account must be specified in the chosen vendor card"
                            );}
                        break;
                    default:
                        break;
                }
                //foreach (BillDetail item in thisCheckPayment.BillDetails) {
                //    GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                //    GenJournalDetail>(session);
                //    _gjd.GenJournalID = thisCheckPayment;
                //    _gjd.Account = item.Expense;
                //    _gjd.DebitAmount = item.Amount;
                //    _gjd.Description = "Business Expense";
                //    _gjd.ExpenseType = item.ExpenseType != null ? item.
                //    ExpenseType : null;
                //    _gjd.SubAccountNo = thisCheckPayment.Vendor;
                //    _gjd.SubAccountType = thisCheckPayment.Vendor.ContactType;
                //    _gjd.Approved = true;
                //    _gjd.Save();
                //    amount += item.Amount;
                //    if (_BgWorker.CancellationPending) {
                //        e.Cancel = true;
                //        session.Dispose();
                //        break;
                //    }
                //    _message = string.Format("Approving entry {0} succesfull.", 
                //    thisCheckPayment.ReceiptDetails.Count - 1);
                //    System.Threading.Thread.Sleep(20);
                //    _BgWorker.ReportProgress(1, _message);
                //    index++;
                //}
            } finally {
                thisCheckVoucher.Status = CheckStatusEnum.Approved;
                e.Result = index;
                CommitUpdatingSession(session);
            }
        }
        //private void BgWorkerProgressChanged(object sender, 
        //ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
        //        DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            //_FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Releasing check voucher operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("Check Voucher has been successfully released");
                    ObjectSpace.ReloadObject(checkVoucher);
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { 
            releaseCheckVoucherAction.Enabled.SetItemValue("Releasing Check Voucher", !
            inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
