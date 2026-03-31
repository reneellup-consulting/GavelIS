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
    public partial class ReleaseCheckAction : ViewController {
        private CheckPayment checkPayment;
        private SimpleAction releaseCheckAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public ReleaseCheckAction() {
            this.TargetObjectType = typeof(CheckPayment);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Release", this.GetType().Name);
            this.releaseCheckAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.releaseCheckAction.Caption = "Release";
            this.releaseCheckAction.Execute += new 
            SimpleActionExecuteEventHandler(ApproveBillAction_Execute);
            this.releaseCheckAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ApproveBillAction_Executed);
            this.releaseCheckAction.ConfirmationMessage = 
            "Do you really want to release this check?";
            UpdateActionState(false);
        }
        private void ApproveBillAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            checkPayment = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as CheckPayment;
            switch (checkPayment.PayToOrder.ContactType) {
                case ContactTypeEnum.Customer:
                    if (((Customer)checkPayment.PayToOrder).Account == null) {
                        throw new ApplicationException(
                        "Accounts Receivable account must be specified in the chosen " 
                        + checkPayment.PayToOrder.ContactType + " card");}
                    break;
                case ContactTypeEnum.Vendor:
                    if (((Vendor)checkPayment.PayToOrder).Account == null) {
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
            _BgWorker.RunWorkerAsync(checkPayment);
        }
        private void ApproveBillAction_Executed(object sender, 
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
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            CheckPayment _checkPayment = (CheckPayment)e.Argument;
            CheckPayment thisCheckPayment = session.GetObjectByKey<CheckPayment>
            (_checkPayment.Oid);
            try {
                // Validate Vendor Accounts Payable
                //if (thisCheckPayment.Vendor == null) {throw new 
                //    ApplicationException("Must specify a vendor");} else {
                //    if (thisCheckPayment.Vendor.Account == null) {throw new 
                //        ApplicationException(
                //        "Accounts Payable account must be specified in the chosen vendor card"
                //        );}
                //}
                if (thisCheckPayment.PayToOrder == null) {throw new 
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
                switch (thisCheckPayment.PayToOrder.ContactType) {
                    case ContactTypeEnum.Customer:
                        if (((Customer)thisCheckPayment.PayToOrder).Account == 
                        null) {throw new ApplicationException(
                            "Accounts Receivable account must be specified in the chosen " 
                            + thisCheckPayment.PayToOrder.ContactType + " card")
                            ;}
                        break;
                    case ContactTypeEnum.Vendor:
                        if (((Vendor)thisCheckPayment.PayToOrder).Account == 
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
                if (thisCheckPayment.PayToOrder.ContactType == ContactTypeEnum.
                Customer || thisCheckPayment.PayToOrder.ContactType == 
                ContactTypeEnum.Vendor) {
                    // Debit Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisCheckPayment;
                    _gjde.GenJournalID.Approved = true;
                    //if (thisCheckPayment.PayToOrder.ContactType != 
                    //ContactTypeEnum.Vendor) {_gjde.Account = thisCheckPayment.
                    //    PayToOrder.NonTradeAccountsPayable;} else {
                    //    _gjde.Account = ((Vendor)thisCheckPayment.PayToOrder).
                    //    Account;
                    //}
                    switch (thisCheckPayment.PayToOrder.ContactType) {
                        case ContactTypeEnum.Customer:
                            _gjde.Account = ((Customer)thisCheckPayment.
                            PayToOrder).Account;
                            break;
                        case ContactTypeEnum.Vendor:
                            _gjde.Account = ((Vendor)thisCheckPayment.PayToOrder
                            ).Account;
                            break;
                        case ContactTypeEnum.Payee:
                            _gjde.Account = ((Payee)thisCheckPayment.PayToOrder
                            ).NonTradeAccountsPayable;
                            break;
                        case ContactTypeEnum.Employee:
                            _gjde.Account = ((Employee)thisCheckPayment.PayToOrder
                            ).NonTradeAccountsPayable;
                            break;
                        default:
                            break;
                    }
                    _gjde.DebitAmount = Math.Abs(thisCheckPayment.CheckAmount);
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    _gjde.Description = "Payments Made";
                    _gjde.SubAccountNo = thisCheckPayment.PayToOrder;
                    _gjde.SubAccountType = thisCheckPayment.PayToOrder.
                    ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    // Credit Bank or Cash Account
                    GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde2.GenJournalID = thisCheckPayment;
                    _gjde2.GenJournalID.Approved = true;
                    _gjde2.Account = thisCheckPayment.BankCashAccount;
                    _gjde2.CreditAmount = Math.Abs(thisCheckPayment.CheckAmount)
                    ;
                    creditAmount = creditAmount + _gjde2.CreditAmount;
                    _gjde2.Description = "Payments Made";
                    _gjde2.SubAccountNo = thisCheckPayment.PayToOrder;
                    _gjde2.SubAccountType = thisCheckPayment.PayToOrder.
                    ContactType;
                    _gjde2.ExpenseType=thisCheckPayment.ExpenseType;
                    _gjde2.SubExpenseType=thisCheckPayment.SubExpenseType!=null?thisCheckPayment.SubExpenseType:null;
                    _gjde2.Approved = true;
                    _gjde2.Save();
                    thisCheckPayment.Status = CheckStatusEnum.Approved;
                    thisCheckPayment.BankCashAccount.LastCheckNo = 
                    thisCheckPayment.CheckNo;
                    thisCheckPayment.BankCashAccount.Save();
                } else {
                    if (thisCheckPayment.CheckPayeeDetails.Count == 0) {throw 
                        new ApplicationException(
                        "Please provide effective accounts");}
                    decimal amount = 0;
                    foreach (CheckPayeeDetail item in thisCheckPayment.
                    CheckPayeeDetails) {
                        amount += item.Amount;
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = thisCheckPayment;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = item.Account;
                        _gjde2.DebitAmount = Math.Abs(item.Amount);
                        debitAmount = debitAmount + _gjde2.DebitAmount;
                        _gjde2.Description = "Payments Made";
                        _gjde2.Description2 = item.Date.ToShortDateString() +" - "+ item.Description;
                        _gjde2.SubAccountNo = thisCheckPayment.PayToOrder;
                        _gjde2.SubAccountType = thisCheckPayment.PayToOrder.
                        ContactType;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde3.GenJournalID = thisCheckPayment;
                    _gjde3.GenJournalID.Approved = true;
                    _gjde3.Account = thisCheckPayment.BankCashAccount;
                    _gjde3.CreditAmount = Math.Abs(amount);
                    creditAmount = creditAmount + _gjde3.CreditAmount;
                    _gjde3.Description = "Payments Made";
                    _gjde3.SubAccountNo = thisCheckPayment.PayToOrder;
                    _gjde3.SubAccountType = thisCheckPayment.PayToOrder.
                    ContactType;
                    _gjde3.Approved = true;
                    _gjde3.Save();
                    thisCheckPayment.CheckAmount = amount;
                    thisCheckPayment.Status = CheckStatusEnum.Approved;
                    thisCheckPayment.BankCashAccount.LastCheckNo = 
                    thisCheckPayment.CheckNo;
                    thisCheckPayment.BankCashAccount.Save();
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

                    ObjectSpace.ReloadObject(checkPayment);
                    ObjectSpace.Refresh();

                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { releaseCheckAction.
            Enabled.SetItemValue("Releasing Check", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
