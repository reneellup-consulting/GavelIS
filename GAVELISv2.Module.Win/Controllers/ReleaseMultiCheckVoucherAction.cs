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
    public partial class ReleaseMultiCheckVoucherAction : ViewController {
        private MultiCheckVoucher multiCheckVoucher;
        private SimpleAction releaseMultiCheckAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public ReleaseMultiCheckVoucherAction() {
            this.TargetObjectType = typeof(MultiCheckVoucher);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Release", this.GetType().Name);
            this.releaseMultiCheckAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.releaseMultiCheckAction.Caption = "Release";
            this.releaseMultiCheckAction.Execute += new 
            SimpleActionExecuteEventHandler(
            ReleaseMultiCheckVoucherAction_Execute);
            this.releaseMultiCheckAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReleaseMultiCheckVoucherAction_Executed);
            this.releaseMultiCheckAction.ConfirmationMessage = 
            "Do you really want to release this check voucher?";
            UpdateActionState(false);
        }
        private void ReleaseMultiCheckVoucherAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            multiCheckVoucher = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as MultiCheckVoucher;
            switch (multiCheckVoucher.Payee.ContactType) {
                case ContactTypeEnum.Customer:
                    if (((Customer)multiCheckVoucher.Payee).Account == null) {
                        throw new ApplicationException(
                        "Accounts Receivable account must be specified in the chosen " 
                        + multiCheckVoucher.Payee.ContactType + " card");}
                    break;
                case ContactTypeEnum.Vendor:
                    if (((Vendor)multiCheckVoucher.Payee).Account == null) {
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
            _BgWorker.RunWorkerAsync(multiCheckVoucher);
        }
        private void ReleaseMultiCheckVoucherAction_Executed(object sender, 
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
            MultiCheckVoucher _multiCheckVoucher = (MultiCheckVoucher)e.Argument
            ;
            MultiCheckVoucher thismultiCheckVoucher = session.GetObjectByKey<
            MultiCheckVoucher>(_multiCheckVoucher.Oid);
            try {
                if (thismultiCheckVoucher.Payee == null) {throw new 
                    ApplicationException("Must specify a Payee to the check");}
                switch (thismultiCheckVoucher.Payee.ContactType) {
                    case ContactTypeEnum.Customer:
                        if (((Customer)thismultiCheckVoucher.Payee).Account == 
                        null) {throw new ApplicationException(
                            "Accounts Receivable account must be specified in the chosen " 
                            + thismultiCheckVoucher.Payee.ContactType + " card")
                            ;}
                        break;
                    case ContactTypeEnum.Vendor:
                        if (((Vendor)thismultiCheckVoucher.Payee).Account == 
                        null) {throw new ApplicationException(
                            "Accounts Payable account must be specified in the chosen vendor card"
                            );}
                        break;
                    default:
                        break;
                }
            } finally {
                #region Expense
                bool IsExpense = false;
                // Verify if Expense
                foreach (MCheckEffectiveDetails item in thismultiCheckVoucher.
                MCheckEffectiveDetails) {if (item.Expense) {IsExpense = true;}}
                // If Expense = true
                Account tmpExp = null;
                if (IsExpense) {
                    tmpExp = Company.GetInstance(session).TemporaryExpenseAcct;
                    if (tmpExp == null) {throw new ApplicationException(
                        "Temporary account was not provided in the company setup card"
                        );}
                }
                foreach (MCheckVoucherDetail item in thismultiCheckVoucher.
                MCheckVoucherDetails) {
                    // Create check for each
                    CheckPayment _chk = ReflectionHelper.CreateObject<
                    CheckPayment>(session);
                    _chk.EntryDate = item.EntryDate;
                    _chk.PostDated = item.PostDated;
                    _chk.CheckDate = item.CheckDate;
                    _chk.BankCashAccount = item.BankAccount;
                    _chk.PaymentMode = PaymentTypeEnum.Check;
                    _chk.CheckNo = item.CheckNo;
                    _chk.ReferenceNo = item.CheckVoucher.CheckVoucherNo;
                    _chk.Memo = item.CheckVoucher.Memo;
                    _chk.Comments = item.CheckVoucher.Comments;
                    _chk.PayToOrder = item.CheckVoucher.Payee;
                    _chk.ExpenseType = item.ExpenseType;
                    _chk.SubExpenseType = item.SubExpenseType != null ? item.SubExpenseType : null;
                    _chk.CheckAmount = item.Amount;
                    _chk.Status = CheckStatusEnum.Approved;
                    _chk.Save();

                    // Create the following entry
                    // If Expense = true Dr. Temporary Expense
                    // If Expense = false Dr. Accounts Payable
                    // Cr. Cash in bank
                    if (IsExpense) {
                        // If Expense = true Dr. Temporary Expense
                        GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde1.GenJournalID = _chk;
                        _gjde1.GenJournalID.Approved = true;
                        _gjde1.Account = tmpExp;
                        _gjde1.DebitAmount = Math.Abs(_chk.CheckAmount);
                        debitAmount = debitAmount + _gjde1.DebitAmount;
                        _gjde1.Description = "Payments Made" + " " + item.
                        CheckVoucher.CheckVoucherNo;
                        _gjde1.SubAccountNo = _chk.PayToOrder;
                        _gjde1.SubAccountType = _chk.PayToOrder.ContactType;
                        _gjde1.Approved = true;
                        _gjde1.Save();
                    } else {
                        // If Expense = false Dr. Accounts Payable
                        GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde1.GenJournalID = _chk;
                        _gjde1.GenJournalID.Approved = true;
                        switch (_chk.PayToOrder.ContactType) {
                            case ContactTypeEnum.Customer:
                                _gjde1.Account = ((Customer)_chk.PayToOrder).
                                Account;
                                break;
                            case ContactTypeEnum.Vendor:
                                _gjde1.Account = ((Vendor)_chk.PayToOrder).
                                Account;
                                break;
                            //case ContactTypeEnum.Payee:
                            //    break;
                            //case ContactTypeEnum.Employee:
                            //    break;
                            default:
                                break;
                        }
                        _gjde1.DebitAmount = Math.Abs(_chk.CheckAmount);
                        debitAmount = debitAmount + _gjde1.DebitAmount;
                        _gjde1.Description = "Payments Made" + " " + item.
                        CheckVoucher.CheckVoucherNo;
                        ;
                        _gjde1.SubAccountNo = _chk.PayToOrder;
                        _gjde1.SubAccountType = _chk.PayToOrder.ContactType;
                        _gjde1.Approved = true;
                        _gjde1.Save();
                    }
                    // Credit Cash in Bank
                    GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde2.GenJournalID = _chk;
                    _gjde2.GenJournalID.Approved = true;
                    _gjde2.Account = _chk.BankCashAccount;
                    _gjde2.CreditAmount = Math.Abs(_chk.CheckAmount);
                    creditAmount = creditAmount + _gjde2.CreditAmount;
                    _gjde2.Description = "Payments Made" + " " + item.
                    CheckVoucher.CheckVoucherNo;
                    _gjde2.SubAccountNo = _chk.PayToOrder;
                    _gjde2.SubAccountType = _chk.PayToOrder.ContactType;
                    _gjde2.ExpenseType = _chk.ExpenseType;
                    _gjde2.SubExpenseType = _chk.SubExpenseType != null ? _chk.SubExpenseType : null;

                    _gjde2.Approved = true;
                    _gjde2.Save();
                }
                JournalEntry _je = null;
                if (thismultiCheckVoucher.MCheckEffectiveDetails.Count > 0) {
                    _je = ReflectionHelper.CreateObject<JournalEntry>(session);
                    _je.EntryDate = thismultiCheckVoucher.EntryDate;
                    _je.ReferenceNo = thismultiCheckVoucher.CheckVoucherNo;
                    _je.Memo = thismultiCheckVoucher.Memo;
                    _je.Comments = thismultiCheckVoucher.Comments;
                    _je.Status = JournalEntryStatusEnum.Approved;
                    _je.Save();
                    foreach (MCheckEffectiveDetails item in 
                    thismultiCheckVoucher.MCheckEffectiveDetails) {
                        if (item.Expense) {
                            // Dr. Operating Expense
                            GenJournalDetail _gjde1 = ReflectionHelper.
                            CreateObject<GenJournalDetail>(session);
                            _gjde1.GenJournalID = _je;
                            _gjde1.GenJournalID.Approved = true;
                            _gjde1.Account = item.Account;
                            _gjde1.DebitAmount = Math.Abs(item.Amount);
                            debitAmount = debitAmount + _gjde1.DebitAmount;
                            _gjde1.Description = item.Description + (string.IsNullOrEmpty(item.Description) ? "Adjustment " : " Adjustment ") + item.
                            MCheckVoucherID.CheckVoucherNo;
                            _gjde1.SubAccountNo = thismultiCheckVoucher.Payee;
                            _gjde1.SubAccountType = thismultiCheckVoucher.Payee.
                            ContactType;
                            _gjde1.Approved = true;
                            _gjde1.Save();
                            // Cr. Temp Expense
                            GenJournalDetail _gjde2 = ReflectionHelper.
                            CreateObject<GenJournalDetail>(session);
                            _gjde2.GenJournalID = _je;
                            _gjde2.GenJournalID.Approved = true;
                            _gjde2.Account = tmpExp;
                            _gjde2.CreditAmount = Math.Abs(item.Amount);
                            creditAmount = creditAmount + _gjde2.CreditAmount;
                            _gjde2.Description = item.Description + (string.IsNullOrEmpty(item.Description) ? "Adjustment " : " Adjustment ") + item.
                            MCheckVoucherID.CheckVoucherNo;
                            _gjde2.SubAccountNo = thismultiCheckVoucher.Payee;
                            _gjde2.SubAccountType = thismultiCheckVoucher.Payee.
                            ContactType;
                            _gjde2.Approved = true;
                            _gjde2.Save();
                        } else {
                            // Dr. Account Payable
                            GenJournalDetail _gjde = ReflectionHelper.
                            CreateObject<GenJournalDetail>(session);
                            _gjde.GenJournalID = _je;
                            _gjde.GenJournalID.Approved = true;
                            switch (thismultiCheckVoucher.Payee.ContactType) {
                                case ContactTypeEnum.Customer:
                                    _gjde.Account = ((Customer)
                                    thismultiCheckVoucher.Payee).Account;
                                    break;
                                case ContactTypeEnum.Vendor:
                                    _gjde.Account = ((Vendor)
                                    thismultiCheckVoucher.Payee).Account;
                                    break;
                                //case ContactTypeEnum.Payee:
                                //    break;
                                //case ContactTypeEnum.Employee:
                                //    break;
                                default:
                                    break;
                            }
                            _gjde.DebitAmount = Math.Abs(item.Amount);
                            debitAmount = debitAmount + _gjde.DebitAmount;
                            _gjde.Description = item.Description + (string.IsNullOrEmpty(item.Description) ? "Adjustment " : " Adjustment ") + item.
                            MCheckVoucherID.CheckVoucherNo;
                            _gjde.SubAccountNo = thismultiCheckVoucher.Payee;
                            _gjde.SubAccountType = thismultiCheckVoucher.Payee.
                            ContactType;
                            _gjde.Approved = true;
                            _gjde.Save();
                            // Cr. Effective Accounts
                            GenJournalDetail _gjde1 = ReflectionHelper.
                            CreateObject<GenJournalDetail>(session);
                            _gjde1.GenJournalID = _je;
                            _gjde1.GenJournalID.Approved = true;
                            _gjde1.Account = item.Account;
                            _gjde1.CreditAmount = Math.Abs(item.Amount);
                            creditAmount = creditAmount + _gjde1.CreditAmount;
                            _gjde1.Description = item.Description + (string.IsNullOrEmpty(item.Description) ? "Adjustment " : " Adjustment ") + item.
                            MCheckVoucherID.CheckVoucherNo;
                            _gjde1.SubAccountNo = thismultiCheckVoucher.Payee;
                            _gjde1.SubAccountType = thismultiCheckVoucher.Payee.
                            ContactType;
                            _gjde1.Approved = true;
                            _gjde1.Save();
                        }
                    }
                }
                #endregion
                // If IsExpense is false create accounts payable entry based on the total as debit
                // Create entry for every non expense effective account as credit
                // Create checks for every check details line
                // --> Create cash in bank entry for every checks as credit
                thismultiCheckVoucher.Status = MultiCheckStatusEnum.Released;
                thismultiCheckVoucher.Save();
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
                    ObjectSpace.ReloadObject(multiCheckVoucher);
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { 
            releaseMultiCheckAction.Enabled.SetItemValue("Releasing Check", !
            inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
