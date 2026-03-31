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
    public partial class ReceivePaymentAction : ViewController {
        private ReceivePayment receivePayment;
        private SimpleAction receivePaymentAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public ReceivePaymentAction() {
            this.TargetObjectType = typeof(ReceivePayment);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Receive", this.GetType().Name);
            this.receivePaymentAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.receivePaymentAction.Caption = "Receive";
            this.receivePaymentAction.Execute += new 
            SimpleActionExecuteEventHandler(ReceivePaymentAction_Execute);
            this.receivePaymentAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReceivePaymentAction_Executed);
            this.receivePaymentAction.ConfirmationMessage = 
            "Do you really want to receive this payment?";
            UpdateActionState(false);
        }
        private void ReceivePaymentAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            receivePayment = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ReceivePayment;
            switch (receivePayment.ReceiveFrom.ContactType) {
                case ContactTypeEnum.Customer:
                    if (((Customer)receivePayment.ReceiveFrom).Account == null) 
                    {throw new ApplicationException(
                        "Accounts Receivable account must be specified in the chosen " 
                        + receivePayment.ReceiveFrom.ContactType + " card");}
                    break;
                case ContactTypeEnum.Vendor:
                    if (((Vendor)receivePayment.ReceiveFrom).Account == null) {
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
            _BgWorker.RunWorkerAsync(receivePayment);
        }
        private void ReceivePaymentAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(receivePayment);
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
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            ReceivePayment _receivePayment = (ReceivePayment)e.Argument;
            ReceivePayment thisReceivePayment = session.GetObjectByKey<
            ReceivePayment>(_receivePayment.Oid);
            try {
                if (thisReceivePayment.ReceiveFrom == null) {throw new 
                    ApplicationException("Must specify a Payee to the check");}
                switch (thisReceivePayment.ReceiveFrom.ContactType) {
                    case ContactTypeEnum.Customer:
                        if (((Customer)thisReceivePayment.ReceiveFrom).Account 
                        == null) {throw new ApplicationException(
                            "Accounts Receivable account must be specified in the chosen " 
                            + thisReceivePayment.ReceiveFrom.ContactType + 
                            " card");}
                        break;
                    case ContactTypeEnum.Vendor:
                        if (((Vendor)thisReceivePayment.ReceiveFrom).Account == 
                        null) {throw new ApplicationException(
                            "Accounts Payable account must be specified in the chosen vendor card"
                            );}
                        break;
                    default:
                        break;
                }
            } finally {
                if (thisReceivePayment.ReceiveFrom.ContactType == 
                ContactTypeEnum.Customer || thisReceivePayment.ReceiveFrom.
                ContactType == ContactTypeEnum.Vendor) {
                    // Debit Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisReceivePayment;
                    _gjde.GenJournalID.Approved = true;
                    switch (thisReceivePayment.ReceiveFrom.ContactType) {
                        case ContactTypeEnum.Customer:
                            _gjde.Account = ((Customer)thisReceivePayment.
                            ReceiveFrom).Account;
                            break;
                        case ContactTypeEnum.Vendor:
                            _gjde.Account = ((Vendor)thisReceivePayment.
                            ReceiveFrom).Account;
                            break;
                        //case ContactTypeEnum.Payee:
                        //    break;
                        //case ContactTypeEnum.Employee:
                        //    break;
                        default:
                            break;
                    }
                    _gjde.CreditAmount = Math.Abs(thisReceivePayment.CheckAmount
                    );
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Description = "Received Payment";
                    _gjde.SubAccountNo = thisReceivePayment.ReceiveFrom;
                    _gjde.SubAccountType = thisReceivePayment.ReceiveFrom.
                    ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    // Credit Bank or Cash Account
                    GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde2.GenJournalID = thisReceivePayment;
                    _gjde2.GenJournalID.Approved = true;
                    _gjde2.Account = thisReceivePayment.BankCashAccount;
                    _gjde2.DebitAmount = Math.Abs(thisReceivePayment.CheckAmount
                    );
                    debitAmount = debitAmount + _gjde2.DebitAmount;
                    _gjde2.Description = "Received Payment";
                    _gjde2.SubAccountNo = thisReceivePayment.ReceiveFrom;
                    _gjde2.SubAccountType = thisReceivePayment.ReceiveFrom.
                    ContactType;
                    _gjde2.ExpenseType = thisReceivePayment.IncomeType;
                    _gjde2.SubExpenseType = thisReceivePayment.SubIncomeType != null ? thisReceivePayment.SubIncomeType : null;
                    _gjde2.Approved = true;
                    _gjde2.Save();
                    thisReceivePayment.Status = PaymentStatusEnum.Approved;
                } else {
                    if (thisReceivePayment.GetFromAccount == null) {throw new 
                        ApplicationException("Must provide Account to get from")
                        ;}
                    // Debit Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisReceivePayment;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisReceivePayment.GetFromAccount;
                    _gjde.CreditAmount = Math.Abs(thisReceivePayment.CheckAmount
                    );
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Description = "Received Payment";
                    _gjde.SubAccountNo = thisReceivePayment.ReceiveFrom;
                    _gjde.SubAccountType = thisReceivePayment.ReceiveFrom.
                    ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    // Credit Bank or Cash Account
                    GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde2.GenJournalID = thisReceivePayment;
                    _gjde2.GenJournalID.Approved = true;
                    _gjde2.Account = thisReceivePayment.BankCashAccount;
                    _gjde2.DebitAmount = Math.Abs(thisReceivePayment.CheckAmount
                    );
                    debitAmount = debitAmount + _gjde2.DebitAmount;
                    _gjde2.Description = "Received Payment";
                    _gjde2.SubAccountNo = thisReceivePayment.ReceiveFrom;
                    _gjde2.SubAccountType = thisReceivePayment.ReceiveFrom.
                    ContactType;
                    _gjde2.ExpenseType = thisReceivePayment.IncomeType;
                    _gjde2.SubExpenseType = thisReceivePayment.SubIncomeType != null ? thisReceivePayment.SubIncomeType : null;
                    _gjde2.Approved = true;
                    _gjde2.Save();
                    thisReceivePayment.Status = PaymentStatusEnum.Approved;
                }
                e.Result = index;
                if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                {
                    throw new
                        ApplicationException("Accounting entries not balance");
                }
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
                "Releasing check operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("Check has been successfully released");

                    //ObjectSpace.ReloadObject(receivePayment);
                    ObjectSpace.Refresh();

                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { receivePaymentAction.
            Enabled.SetItemValue("Releasing Check", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
