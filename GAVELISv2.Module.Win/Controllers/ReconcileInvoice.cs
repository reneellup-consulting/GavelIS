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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class ReconcileInvoice : ViewController {
        private InvoiceReconciliation invoiceRecon;
        private SimpleAction reconcileInvoiceAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        public ReconcileInvoice() {
            this.TargetObjectType = typeof(InvoiceReconciliation);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReconcileInvoice", this.GetType
            ().Name);
            this.reconcileInvoiceAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.reconcileInvoiceAction.Caption = "Reconcile Invoice";
            this.reconcileInvoiceAction.Execute += new 
            SimpleActionExecuteEventHandler(ReconcileInvoiceAction_Execute);
            this.reconcileInvoiceAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReconcileInvoiceAction_Executed);
            this.reconcileInvoiceAction.ConfirmationMessage = 
            "Do you really want to reconcile the selected entries?";
            UpdateActionState(false);
        }
        private void ReconcileInvoiceAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            invoiceRecon = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as InvoiceReconciliation;
            if (Math.Round(invoiceRecon.Balance, 2) != 0) {throw new 
                ApplicationException(
                "The balance must be zero in order to pay selected bills");}
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(invoiceRecon);
        }
        private void ReconcileInvoiceAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //System.Threading.Thread.Sleep(500);
            //for (int i = invoiceRecon.Charges.Count - 1; i >= 0; i--) {
            //    invoiceRecon.Charges[i].Delete();}
            //for (int i = invoiceRecon.Payments.Count - 1; i >= 0; i--) {
            //    invoiceRecon.Payments[i].Delete();}
            //invoiceRecon.Save();
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(invoiceRecon);
            //ObjectSpace.Refresh();
            //((DevExpress.ExpressApp.DetailView)this.View).Refresh();
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
            //int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            InvoiceReconciliation _invoiceRecon = (InvoiceReconciliation)e.
            Argument;
            InvoiceReconciliation thisInvoiceRecon = session.GetObjectByKey<
            InvoiceReconciliation>(_invoiceRecon.Oid);
            //TempAccountCollection accounts = new TempAccountCollection();
            //TempAccount tmpAccount;
            try {
                #region Process Charge Adjustment
                foreach (InvoiceReconCharge item in thisInvoiceRecon.Charges) {
                    if (item.Pay) {
                        #region --> Process Invoice
                        if (item.SourceType.Code == "IN") {
                            Invoice invoice = null;
                            if (item.Adjust > item.OpenAmount) {throw new 
                                ApplicationException(
                                "You cannot pay more than you have to pay");}
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.FinanceCharge == 0) {
                                if (item.Adjust == item.OpenAmount) {
                                    invoice = session.GetObjectByKey<Invoice>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = InvoiceStatusEnum.Paid;
                                    invoice.Save();
                                }
                                if (item.Adjust != item.OpenAmount) {
                                    invoice = session.GetObjectByKey<Invoice>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = InvoiceStatusEnum.
                                    PartiallyPaid;
                                    invoice.Save();
                                }
                            }
                            #endregion
                            #region If WHT Taxable
                            //if (item.WHTCode != null && item.WHTRate != 0) {
                            //    if (item.WHTCode.WHTExpenseAcct == null) {throw 
                            //        new ApplicationException(
                            //        "WHT Expense account is not specified in the WHT Code card"
                            //        );}
                            //    int[] inds = null;
                            //    inds = accounts.Find("Account", item.WHTCode.
                            //    WHTExpenseAcct);
                            //    if (inds != null && inds.Length > 0) {
                            //        tmpAccount = accounts[inds[0]];
                            //        tmpAccount.DebitAmount += Math.Abs(item.WHTAmount);
                            //    } else {
                            //        tmpAccount = new TempAccount();
                            //        tmpAccount.Account = item.WHTCode.
                            //        WHTExpenseAcct;
                            //        tmpAccount.DebitAmount += Math.Abs(item.WHTAmount);
                            //        accounts.Add(tmpAccount);
                            //    }
                            //    //// Create Journal Entry
                            //    //JournalEntry journalEntry = ReflectionHelper.
                            //    //CreateObject<JournalEntry>(thisInvoiceRecon.
                            //    //Session);
                            //    //journalEntry.EntryDate = thisInvoiceRecon.
                            //    //EntryDate;
                            //    //journalEntry.ReferenceNo = item.SourceNo;
                            //    //journalEntry.Memo =
                            //    //"WHT Expense Adjustment for #" +
                            //    //thisInvoiceRecon.Customer.No;
                            //    //journalEntry.Status = JournalEntryStatusEnum.
                            //    //Approved;
                            //    //journalEntry.Save();
                            //    //// Debit WHT Expense
                            //    //GenJournalDetail _gjde = ReflectionHelper.
                            //    //CreateObject<GenJournalDetail>(session);
                            //    //_gjde.GenJournalID = journalEntry;
                            //    //_gjde.GenJournalID.Approved = true;
                            //    //_gjde.Account = item.WHTCode.WHTExpenseAcct;
                            //    //_gjde.CreditAmount = Math.Abs(item.WHTAmount);
                            //    //_gjde.Description = "WHT Expense Adjustment";
                            //    //_gjde.SubAccountNo = thisInvoiceRecon.Customer;
                            //    //_gjde.SubAccountType = thisInvoiceRecon.Customer
                            //    //.ContactType;
                            //    //_gjde.Approved = true;
                            //    //_gjde.Save();
                            //    //// Credit Cash in Bank
                            //    //GenJournalDetail _gjde2 = ReflectionHelper.
                            //    //CreateObject<GenJournalDetail>(session);
                            //    //_gjde2.GenJournalID = journalEntry;
                            //    //_gjde2.GenJournalID.Approved = true;
                            //    //_gjde2.Account = item.Terms.DicountGivenAccount;
                            //    //_gjde2.DebitAmount = Math.Abs(item.WHTAmount);
                            //    //_gjde2.Description = "WHT Expense Adjustment";
                            //    //_gjde2.SubAccountNo = thisInvoiceRecon.Customer;
                            //    //_gjde2.SubAccountType = thisInvoiceRecon.
                            //    //Customer.ContactType;
                            //    //_gjde2.Approved = true;
                            //    //_gjde2.Save();
                            //}
                            #endregion
                            #region Pay with discount or with finance charge
                            if (item.Discount > 0 || item.FinanceCharge > 0) {
                                if (item.Adjust == item.OpenAmount) {
                                    invoice = session.GetObjectByKey<Invoice>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = InvoiceStatusEnum.Paid;
                                    invoice.Save();
                                }
                                if (item.Adjust != item.OpenAmount) {
                                    invoice = session.GetObjectByKey<Invoice>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = InvoiceStatusEnum.
                                    PartiallyPaid;
                                    invoice.Save();
                                }
                            }
                            #endregion
                            #region Process Discount
                            if (item.Discount > 0) {
                                // Discount account and Interest account based on terms
                                if (item.Terms == null) {throw new 
                                    ApplicationException(
                                    "Discount not allowed if terms is not provided"
                                    );} else {
                                    if (item.Terms.DicountGivenAccount == null) 
                                    {throw new ApplicationException(
                                        "Discount given account must be specified in terms card"
                                        );}
                                }
                                if (thisInvoiceRecon.Customer == null) {throw 
                                    new ApplicationException(
                                    "Must specify a customer");} else {
                                    if (thisInvoiceRecon.Customer.Account == 
                                    null) {throw new ApplicationException(
                                        "Accounts receivable account must be specified in the chosen vendor card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisInvoiceRecon.
                                Session);
                                journalEntry.EntryDate = thisInvoiceRecon.
                                EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Discount Given to Customer #" + 
                                thisInvoiceRecon.Customer.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Receivable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisInvoiceRecon.Customer.
                                Account;
                                _gjde.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Discount Given";
                                _gjde.SubAccountNo = thisInvoiceRecon.Customer;
                                _gjde.SubAccountType = thisInvoiceRecon.Customer
                                .ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Debit Discount Given
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.DicountGivenAccount;
                                _gjde2.DebitAmount = Math.Abs(item.Discount);
                                debitAmount = debitAmount + _gjde2.DebitAmount;
                                _gjde2.Description = "Discount Given";
                                _gjde2.SubAccountNo = thisInvoiceRecon.Customer;
                                _gjde2.SubAccountType = thisInvoiceRecon.
                                Customer.ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                            #region Process Finance Charge
                            if (item.FinanceCharge > 0) {
                                if (item.Terms == null) {throw new 
                                    ApplicationException(
                                    "Finance charge not allowed if terms is not provided"
                                    );} else {
                                    if (item.Terms.InterestIncomeAccount == null
                                    ) {throw new ApplicationException(
                                        "Interest income account must be specified in terms card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisInvoiceRecon.
                                Session);
                                journalEntry.EntryDate = thisInvoiceRecon.
                                EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Finance Charge from Customer #" + 
                                thisInvoiceRecon.Customer.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Receivable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisInvoiceRecon.Customer.
                                Account;
                                _gjde.DebitAmount = Math.Abs(item.FinanceCharge)
                                ;
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Aging on AR";
                                _gjde.SubAccountNo = thisInvoiceRecon.Customer;
                                _gjde.SubAccountType = thisInvoiceRecon.Customer
                                .ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Credit Interest Income
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                InterestIncomeAccount;
                                _gjde2.CreditAmount = Math.Abs(item.
                                FinanceCharge);
                                creditAmount = creditAmount + _gjde2.CreditAmount;
                                _gjde2.Description = "Aging on AR";
                                _gjde2.SubAccountNo = thisInvoiceRecon.Customer;
                                _gjde2.SubAccountType = thisInvoiceRecon.
                                Customer.ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }
                        #endregion
                        #region --> Process Payments Made to Customer
                        if (item.SourceType.Code == "CR") {
                            if (item.OperationType.Code == "PY") {
                                CheckPayment checkPayment = null;
                                if (item.Adjust < item.OpenAmount) {throw new 
                                    ApplicationException(
                                    "You cannot pay more than you have to pay");
                                }
                                #region With/Without adjustment only
                                if (item.Discount == 0 && item.FinanceCharge == 
                                0) {
                                    if (item.Adjust == item.OpenAmount) {
                                        checkPayment = session.GetObjectByKey<
                                        CheckPayment>(item.SourceID);
                                        checkPayment.OpenAmount = item.
                                        OpenAmount - item.Adjust;
                                        checkPayment.Adjusted = checkPayment.
                                        CheckAmount - checkPayment.OpenAmount;
                                        checkPayment.Save();
                                    }
                                    if (item.Adjust != item.OpenAmount) {
                                        checkPayment = session.GetObjectByKey<
                                        CheckPayment>(item.SourceID);
                                        checkPayment.OpenAmount = item.
                                        OpenAmount - item.Adjust;
                                        checkPayment.Adjusted = checkPayment.
                                        CheckAmount - checkPayment.OpenAmount;
                                        checkPayment.Save();
                                    }
                                }
                                #endregion
                            }
                            if (item.OperationType.Code == "CV") {
                                CheckVoucher checkVoucher = null;
                                if (item.Adjust < item.OpenAmount) {throw new 
                                    ApplicationException(
                                    "You cannot pay more than you have to pay");
                                }
                                #region With/Without adjustment only
                                if (item.Discount == 0 && item.FinanceCharge == 
                                0) {
                                    if (item.Adjust == item.OpenAmount) {
                                        checkVoucher = session.GetObjectByKey<
                                        CheckVoucher>(item.SourceID);
                                        checkVoucher.OpenAmount = item.
                                        OpenAmount - item.Adjust;
                                        checkVoucher.Adjusted = checkVoucher.
                                        CheckAmount.Value - checkVoucher.
                                        OpenAmount;
                                        checkVoucher.Save();
                                    }
                                    if (item.Adjust != item.OpenAmount) {
                                        checkVoucher = session.GetObjectByKey<
                                        CheckVoucher>(item.SourceID);
                                        checkVoucher.OpenAmount = item.
                                        OpenAmount - item.Adjust;
                                        checkVoucher.Adjusted = checkVoucher.
                                        CheckAmount.Value - checkVoucher.
                                        OpenAmount;
                                        checkVoucher.Save();
                                    }
                                }
                                #endregion
                            }
                        }
                        #endregion
                    }
                }
                #endregion
                #region Process Payment Adjustment
                foreach (InvoiceReconPayment item in thisInvoiceRecon.Payments) 
                {
                    if (item.Select) {
                        #region --> Process Payment Received
                        if (item.SourceType.Code == "CR") {
                            ReceivePayment receivePayment = session.
                            GetObjectByKey<ReceivePayment>(item.SourceID);
                            receivePayment.OpenAmount = item.OpenAmount - item.
                            AdjustNow;
                            receivePayment.Adjusted = receivePayment.CheckAmount 
                            - receivePayment.OpenAmount;
                            receivePayment.Save();
                            //if (accounts.Count > 0) {
                            //    // Create Journal Entry
                            //    JournalEntry journalEntry = ReflectionHelper.
                            //    CreateObject<JournalEntry>(thisInvoiceRecon.
                            //    Session);
                            //    journalEntry.EntryDate = thisInvoiceRecon.
                            //    EntryDate;
                            //    journalEntry.ReferenceNo = item.SourceNo;
                            //    journalEntry.Memo = 
                            //    "WHT Expense Adjustment for #" + 
                            //    thisInvoiceRecon.Customer.No;
                            //    journalEntry.Status = JournalEntryStatusEnum.
                            //    Approved;
                            //    journalEntry.Save();
                            //    foreach (TempAccount tmp in accounts) {
                            //        // Debit WHT Expense
                            //        GenJournalDetail _gjde = ReflectionHelper.
                            //        CreateObject<GenJournalDetail>(session);
                            //        _gjde.GenJournalID = journalEntry;
                            //        _gjde.GenJournalID.Approved = true;
                            //        _gjde.Account = tmp.Account;
                            //        _gjde.DebitAmount = Math.Abs(tmp.DebitAmount
                            //        );
                            //        _gjde.Description = "WHT Expense Adjustment"
                            //        ;
                            //        _gjde.SubAccountNo = thisInvoiceRecon.
                            //        Customer;
                            //        _gjde.SubAccountType = thisInvoiceRecon.
                            //        Customer.ContactType;
                            //        _gjde.Approved = true;
                            //        _gjde.Save();
                            //        // Credit Cash in Bank
                            //        GenJournalDetail _gjde2 = ReflectionHelper.
                            //        CreateObject<GenJournalDetail>(session);
                            //        _gjde2.GenJournalID = journalEntry;
                            //        _gjde2.GenJournalID.Approved = true;
                            //        _gjde2.Account = receivePayment.
                            //        BankCashAccount;
                            //        _gjde2.CreditAmount = Math.Abs(tmp.
                            //        DebitAmount);
                            //        _gjde2.Description = 
                            //        "WHT Expense Adjustment";
                            //        _gjde2.SubAccountNo = thisInvoiceRecon.
                            //        Customer;
                            //        _gjde2.SubAccountType = thisInvoiceRecon.
                            //        Customer.ContactType;
                            //        _gjde2.Approved = true;
                            //        _gjde2.Save();
                            //    }
                            //}
                        }
                        #endregion
                        #region --> Process Return of Goods from Customer
                        if (item.SourceType.Code == "CM") {
                            CreditMemo creditMemo = session.GetObjectByKey<
                            CreditMemo>(item.SourceID);
                            creditMemo.OpenAmount = item.OpenAmount - item.
                            AdjustNow;
                            if (creditMemo.OpenAmount == 0) {creditMemo.Status = 
                                CreditMemoStatusEnum.Applied;} else {
                                creditMemo.Status = CreditMemoStatusEnum.
                                PartiallyApplied;
                            }
                            creditMemo.Save();
                        }
                        #endregion
                    }
                }
                #endregion
                //#region Process Remaining Tax
                //if (thisInvoiceRecon.RemainingTax>0)
                //{
                //    JournalEntry journalEntry = ReflectionHelper.
                //    CreateObject<JournalEntry>(thisInvoiceRecon.
                //    Session);
                //    journalEntry.EntryDate = thisInvoiceRecon.
                //    EntryDate;
                //    journalEntry.ReferenceNo = null;
                //    journalEntry.Memo =
                //    "Sales Tax Receivable Adjustments for #" +
                //    thisInvoiceRecon.Customer.No;
                //    journalEntry.Status = JournalEntryStatusEnum.
                //    Approved;
                //    journalEntry.Save();
                //    // Debit Accounts Receivable
                //    GenJournalDetail _gjde = ReflectionHelper.
                //    CreateObject<GenJournalDetail>(session);
                //    _gjde.GenJournalID = journalEntry;
                //    _gjde.GenJournalID.Approved = true;
                //    _gjde.Account = thisInvoiceRecon.Customer.
                //    Account;
                //    _gjde.DebitAmount = Math.Abs(thisInvoiceRecon.RemainingTax);
                //    _gjde.Description = "Sales Tax Receivable Adjustment";
                //    _gjde.SubAccountNo = thisInvoiceRecon.Customer;
                //    _gjde.SubAccountType = thisInvoiceRecon.Customer
                //    .ContactType;
                //    _gjde.Approved = true;
                //    _gjde.Save();
                //    // Credit Output Tax
                //    GenJournalDetail _gjde2 = ReflectionHelper.
                //    CreateObject<GenJournalDetail>(session);
                //    _gjde2.GenJournalID = journalEntry;
                //    _gjde2.GenJournalID.Approved = true;
                //    _gjde2.Account = thisInvoiceRecon.Customer.TaxCode.Account;
                //    _gjde2.CreditAmount = Math.Abs(thisInvoiceRecon.RemainingTax);
                //    _gjde2.Description = "Sales Tax Receivable Adjustment";
                //    _gjde2.SubAccountNo = thisInvoiceRecon.Customer;
                //    _gjde2.SubAccountType = thisInvoiceRecon.
                //    Customer.ContactType;
                //    _gjde2.Approved = true;
                //    _gjde2.Save();
                //}
                //#endregion
            } finally {
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
                "Reconcile selected invoice operation has been cancelled", 
                "Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Invoices has been successfully reconciled");
                    System.Threading.Thread.Sleep(500);
                    for (int i = invoiceRecon.Charges.Count - 1; i >= 0; i--) {
                        invoiceRecon.Charges[i].Delete();}
                    for (int i = invoiceRecon.Payments.Count - 1; i >= 0; i--) {
                        invoiceRecon.Payments[i].Delete();}
                    invoiceRecon.Save();
                    ObjectSpace.CommitChanges();
                    ObjectSpace.ReloadObject(invoiceRecon);
                    ObjectSpace.Refresh();
                    ((DevExpress.ExpressApp.DetailView)this.View).Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { reconcileInvoiceAction
            .Enabled.SetItemValue("Reconciling invoices", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
