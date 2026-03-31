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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class PaySelectedBillAction : ViewController {
        private PayBill payBill;
        private SimpleAction paySelectedAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public PaySelectedBillAction() {
            this.TargetObjectType = typeof(PayBill);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.PaySelected", this.GetType().
            Name);
            this.paySelectedAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.paySelectedAction.Caption = "Pay Selected Bill";
            this.paySelectedAction.Execute += new 
            SimpleActionExecuteEventHandler(PaySelectedBillAction_Execute);
            this.paySelectedAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ApproveBillAction_Executed);
            this.paySelectedAction.ConfirmationMessage = 
            "Do you really want to pay these bills?";
            UpdateActionState(false);
        }
        private void PaySelectedBillAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            payBill = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PayBill;
            int creditSelected = payBill.Credits.Where(o => o.Select).Count();
            if (creditSelected > 1)
            {
                throw new ApplicationException("Multiple credits selection is no longer allowed. This is to track cheque payments application.");
            }
            if (Math.Round(payBill.Difference, 2) != 0) {throw new 
                ApplicationException(
                "The difference must be zero in order to pay selected bills");}
            payBill.Save();
            ObjectSpace.CommitChanges();
            _FrmProgress = new ProgressForm("Applying credits...", creditSelected,
            "Applying credits {0} of {1} ");
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(payBill);
            _FrmProgress.ShowDialog();
        }
        private void ApproveBillAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //System.Threading.Thread.Sleep(500);
            //for (int i = payBill.Charges.Count - 1; i >= 0; i--)
            //{
            //    payBill.
            //        Charges[i].Delete();
            //}
            //for (int i = payBill.Credits.Count - 1; i >= 0; i--)
            //{
            //    payBill.
            //        Credits[i].Delete();
            //}
            //payBill.CashBankAccount = null;
            //payBill.CheckNo = string.Empty;
            //payBill.Amount = 0;
            //payBill.Save();
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(payBill);
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            //int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            PayBill _payBill = (PayBill)e.Argument;
            PayBill thisPayBill = session.GetObjectByKey<PayBill>(_payBill.Oid);
            try {
                _message = string.Format("Process starting.", 0);
                _BgWorker.ReportProgress(0, _message);
                foreach (PayBillExistingCharge item in thisPayBill.Charges) {
                    if (item.Pay) {
                        if (item.SourceType.Code == "RC") {
                            Receipt receipt = null;
                            if ((item.Charges - item.Adjusted) < item.Adjust) {
                                throw new ApplicationException(
                                "You cannot pay more than you have to pay");}
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.Interest == 0) {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust) {
                                    receipt = session.GetObjectByKey<Receipt>(
                                    item.SourceID);
                                    receipt.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receipt.Status = ReceiptStatusEnum.Paid;
                                    receipt.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust) {
                                    receipt = session.GetObjectByKey<Receipt>(
                                    item.SourceID);
                                    receipt.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receipt.Status = ReceiptStatusEnum.
                                    PartiallyPaid;
                                    receipt.Save();
                                }
                            }
                            #endregion
                            #region Pay with discount or with interest
                            if (item.Discount > 0 || item.Interest > 0) {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust) {
                                    receipt = session.GetObjectByKey<Receipt>(
                                    item.SourceID);
                                    receipt.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receipt.Status = ReceiptStatusEnum.Paid;
                                    receipt.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust) {
                                    receipt = session.GetObjectByKey<Receipt>(
                                    item.SourceID);
                                    receipt.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receipt.Status = ReceiptStatusEnum.
                                    PartiallyPaid;
                                    receipt.Save();
                                }
                            }
                            if (item.Discount > 0) {
                                // Discount account and Interest account based on terms
                                if (item.Terms == null) {throw new 
                                    ApplicationException(
                                    "Discount not allowed if terms is not provided"
                                    );} else {
                                    if (item.Terms.DiscountReceivedAccount == 
                                    null) {throw new ApplicationException(
                                        "Discount received account must be specified in terms card"
                                        );}
                                }
                                if (thisPayBill.Vendor == null) {throw new 
                                    ApplicationException("Must specify a vendor"
                                    );} else {
                                    if (thisPayBill.Vendor.Account == null) {
                                        throw new ApplicationException(
                                        "Accounts Payable account must be specified in the chosen vendor card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Discount Received from Vendor #" + thisPayBill.
                                Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.DebitAmount = Math.Abs(item.Discount);
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Discount Received";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Credit Discount Received
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                DiscountReceivedAccount;
                                _gjde2.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde2.CreditAmount;
                                _gjde2.Description = "Discount Received";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            if (item.Interest > 0) {
                                if (item.Terms == null) {throw new 
                                    ApplicationException(
                                    "Interest not allowed if terms is not provided"
                                    );} else {
                                    if (item.Terms.InterestExpenseAccount == 
                                    null) {throw new ApplicationException(
                                        "Interest expense account must be specified in terms card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = "Interest paid to Vendor #" 
                                + thisPayBill.Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.CreditAmount = Math.Abs(item.Interest);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Aging on AP";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Debit Interest Expense
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                InterestExpenseAccount;
                                _gjde2.DebitAmount = Math.Abs(item.Interest);
                                debitAmount = debitAmount + _gjde2.DebitAmount;
                                _gjde2.Description = "Aging on AP";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }
                        #region Fuel Receipt
                        if (item.SourceType.Code == "RFL") {
                            ReceiptFuel receiptFuel = null;
                            if ((item.Charges - item.Adjusted) < item.Adjust) {
                                throw new ApplicationException(
                                "You cannot pay more than you have to pay");}
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.Interest == 0) {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust) {
                                    receiptFuel = session.GetObjectByKey<
                                    ReceiptFuel>(item.SourceID);
                                    receiptFuel.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receiptFuel.Status = ReceiptFuelStatusEnum.
                                    Paid;
                                    receiptFuel.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust) {
                                    receiptFuel = session.GetObjectByKey<
                                    ReceiptFuel>(item.SourceID);
                                    receiptFuel.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receiptFuel.Status = ReceiptFuelStatusEnum.
                                    PartiallyPaid;
                                    receiptFuel.Save();
                                }
                            }
                            #endregion
                            #region Pay with discount or with interest
                            if (item.Discount > 0 || item.Interest > 0) {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust) {
                                    receiptFuel = session.GetObjectByKey<
                                    ReceiptFuel>(item.SourceID);
                                    receiptFuel.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receiptFuel.Status = ReceiptFuelStatusEnum.
                                    Paid;
                                    receiptFuel.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust) {
                                    receiptFuel = session.GetObjectByKey<
                                    ReceiptFuel>(item.SourceID);
                                    receiptFuel.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    receiptFuel.Status = ReceiptFuelStatusEnum.
                                    PartiallyPaid;
                                    receiptFuel.Save();
                                }
                            }
                            if (item.Discount > 0) {
                                // Discount account and Interest account based on terms
                                if (item.Terms == null) {throw new 
                                    ApplicationException(
                                    "Discount not allowed if terms is not provided"
                                    );} else {
                                    if (item.Terms.DiscountReceivedAccount == 
                                    null) {throw new ApplicationException(
                                        "Discount received account must be specified in terms card"
                                        );}
                                }
                                if (thisPayBill.Vendor == null) {throw new 
                                    ApplicationException("Must specify a vendor"
                                    );} else {
                                    if (thisPayBill.Vendor.Account == null) {
                                        throw new ApplicationException(
                                        "Accounts Payable account must be specified in the chosen vendor card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Discount Received from Vendor #" + thisPayBill.
                                Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.DebitAmount = Math.Abs(item.Discount);
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Discount Received";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Credit Discount Received
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                DiscountReceivedAccount;
                                _gjde2.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde2.CreditAmount;
                                _gjde2.Description = "Discount Received";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            if (item.Interest > 0) {
                                if (item.Terms == null) {throw new 
                                    ApplicationException(
                                    "Interest not allowed if terms is not provided"
                                    );} else {
                                    if (item.Terms.InterestExpenseAccount == 
                                    null) {throw new ApplicationException(
                                        "Interest expense account must be specified in terms card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = "Interest paid to Vendor #" 
                                + thisPayBill.Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.CreditAmount = Math.Abs(item.Interest);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Aging on AP";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Debit Interest Expense
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                InterestExpenseAccount;
                                _gjde2.DebitAmount = Math.Abs(item.Interest);
                                debitAmount = debitAmount + _gjde2.DebitAmount;
                                _gjde2.Description = "Aging on AP";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }
                        #endregion
                        #region Bill

                        if (item.SourceType.Code == "BE")
                        {
                            Bill bill = null;
                            if ((item.Charges - item.Adjusted) < item.Adjust)
                            {
                                throw new ApplicationException(
                                "You cannot pay more than you have to pay");
                            }
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.Interest == 0)
                            {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust)
                                {
                                    bill = session.GetObjectByKey<Bill>(
                                    item.SourceID);
                                    bill.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    bill.Status = BillStatusEnum.Paid;
                                    bill.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust)
                                {
                                    bill = session.GetObjectByKey<Bill>(
                                    item.SourceID);
                                    bill.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    bill.Status = BillStatusEnum.
                                    PartiallyPaid;
                                    bill.Save();
                                }
                            }
                            #endregion
                            #region Pay with discount or with interest
                            if (item.Discount > 0 || item.Interest > 0)
                            {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust)
                                {
                                    bill = session.GetObjectByKey<Bill>(
                                    item.SourceID);
                                    bill.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    bill.Status = BillStatusEnum.Paid;
                                    bill.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust)
                                {
                                    bill = session.GetObjectByKey<Bill>(
                                    item.SourceID);
                                    bill.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    bill.Status = BillStatusEnum.
                                    PartiallyPaid;
                                    bill.Save();
                                }
                            }
                            if (item.Discount > 0)
                            {
                                // Discount account and Interest account based on terms
                                if (item.Terms == null)
                                {
                                    throw new
                                        ApplicationException(
                                        "Discount not allowed if terms is not provided"
                                        );
                                }
                                else
                                {
                                    if (item.Terms.DiscountReceivedAccount ==
                                    null)
                                    {
                                        throw new ApplicationException(
                                            "Discount received account must be specified in terms card"
                                            );
                                    }
                                }
                                if (thisPayBill.Vendor == null)
                                {
                                    throw new
                                        ApplicationException("Must specify a vendor"
                                        );
                                }
                                else
                                {
                                    if (thisPayBill.Vendor.Account == null)
                                    {
                                        throw new ApplicationException(
                                        "Accounts Payable account must be specified in the chosen vendor card"
                                        );
                                    }
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo =
                                "Discount Received from Vendor #" + thisPayBill.
                                Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.DebitAmount = Math.Abs(item.Discount);
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Discount Received";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Credit Discount Received
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                DiscountReceivedAccount;
                                _gjde2.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde2.CreditAmount;
                                _gjde2.Description = "Discount Received";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            if (item.Interest > 0)
                            {
                                if (item.Terms == null)
                                {
                                    throw new
                                        ApplicationException(
                                        "Interest not allowed if terms is not provided"
                                        );
                                }
                                else
                                {
                                    if (item.Terms.InterestExpenseAccount ==
                                    null)
                                    {
                                        throw new ApplicationException(
                                            "Interest expense account must be specified in terms card"
                                            );
                                    }
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = "Interest paid to Vendor #"
                                + thisPayBill.Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.CreditAmount = Math.Abs(item.Interest);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Aging on AP";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Debit Interest Expense
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                InterestExpenseAccount;
                                _gjde2.DebitAmount = Math.Abs(item.Interest);
                                debitAmount = debitAmount + _gjde2.DebitAmount;
                                _gjde2.Description = "Aging on AP";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }


                        #endregion

                        #region Job Orders

                        if (item.SourceType.Code == "JO")
                        {
                            JobOrder job = null;
                            if ((item.Charges - item.Adjusted) < item.Adjust)
                            {
                                throw new ApplicationException(
                                "You cannot pay more than you have to pay");
                            }
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.Interest == 0)
                            {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust)
                                {
                                    job = session.GetObjectByKey<JobOrder>(
                                    item.SourceID);
                                    job.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    job.Status = JobOrderStatusEnum.Paid;
                                    job.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust)
                                {
                                    job = session.GetObjectByKey<JobOrder>(
                                    item.SourceID);
                                    job.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    job.Status = JobOrderStatusEnum.
                                    PartiallyPaid;
                                    job.Save();
                                }
                            }
                            #endregion
                            #region Pay with discount or with interest
                            if (item.Discount > 0 || item.Interest > 0)
                            {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust)
                                {
                                    job = session.GetObjectByKey<JobOrder>(
                                    item.SourceID);
                                    job.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    job.Status = JobOrderStatusEnum.Paid;
                                    job.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust)
                                {
                                    job = session.GetObjectByKey<JobOrder>(
                                    item.SourceID);
                                    job.Adjusted = item.Adjust + item.
                                    Adjusted;
                                    job.Status = JobOrderStatusEnum.
                                    PartiallyPaid;
                                    job.Save();
                                }
                            }
                            if (item.Discount > 0)
                            {
                                // Discount account and Interest account based on terms
                                if (item.Terms == null)
                                {
                                    throw new
                                        ApplicationException(
                                        "Discount not allowed if terms is not provided"
                                        );
                                }
                                else
                                {
                                    if (item.Terms.DiscountReceivedAccount ==
                                    null)
                                    {
                                        throw new ApplicationException(
                                            "Discount received account must be specified in terms card"
                                            );
                                    }
                                }
                                if (thisPayBill.Vendor == null)
                                {
                                    throw new
                                        ApplicationException("Must specify a vendor"
                                        );
                                }
                                else
                                {
                                    if (thisPayBill.Vendor.Account == null)
                                    {
                                        throw new ApplicationException(
                                        "Accounts Payable account must be specified in the chosen vendor card"
                                        );
                                    }
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo =
                                "Discount Received from Vendor #" + thisPayBill.
                                Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.DebitAmount = Math.Abs(item.Discount);
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Discount Received";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Credit Discount Received
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                DiscountReceivedAccount;
                                _gjde2.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde2.CreditAmount;
                                _gjde2.Description = "Discount Received";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            if (item.Interest > 0)
                            {
                                if (item.Terms == null)
                                {
                                    throw new
                                        ApplicationException(
                                        "Interest not allowed if terms is not provided"
                                        );
                                }
                                else
                                {
                                    if (item.Terms.InterestExpenseAccount ==
                                    null)
                                    {
                                        throw new ApplicationException(
                                            "Interest expense account must be specified in terms card"
                                            );
                                    }
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisPayBill.Session);
                                journalEntry.EntryDate = thisPayBill.EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = "Interest paid to Vendor #"
                                + thisPayBill.Vendor.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisPayBill.Vendor.Account;
                                _gjde.CreditAmount = Math.Abs(item.Interest);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Aging on AP";
                                _gjde.SubAccountNo = thisPayBill.Vendor;
                                _gjde.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Debit Interest Expense
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = journalEntry;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = item.Terms.
                                InterestExpenseAccount;
                                _gjde2.DebitAmount = Math.Abs(item.Interest);
                                debitAmount = debitAmount + _gjde2.DebitAmount;
                                _gjde2.Description = "Aging on AP";
                                _gjde2.SubAccountNo = thisPayBill.Vendor;
                                _gjde2.SubAccountType = thisPayBill.Vendor.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }


                        #endregion

                        #region Checks from Vendor Refund
                        if (item.SourceType.Code == "CR") {
                            ReceivePayment receivePayment = null;
                            if ((item.Charges - item.Adjusted) < item.Adjust) {
                                throw new ApplicationException(
                                "You cannot pay more than you have to pay");}
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.Interest == 0) {
                                if ((item.Charges - item.Adjusted) == item.
                                Adjust) {
                                    receivePayment = session.GetObjectByKey<
                                    ReceivePayment>(item.SourceID);
                                    receivePayment.Adjusted = item.Adjust + item
                                    .Adjusted;
                                    receivePayment.Save();
                                }
                                if ((item.Charges - item.Adjusted) != item.
                                Adjust) {
                                    receivePayment = session.GetObjectByKey<
                                    ReceivePayment>(item.SourceID);
                                    receivePayment.Adjusted = item.Adjust + item
                                    .Adjusted;
                                    receivePayment.Save();
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                #region Process Credits
                foreach (PayBillExistingCredit item in thisPayBill.Credits) {
                    if (item.Select) {
                        _message = string.Format("Applying credit {0} succesfull.", 1);
                        _BgWorker.ReportProgress(1, _message);
                        if (item.SourceType.Code == "CR") {
                            if (item.OperationType.Code == "PY") {
                                CheckPayment checkPayment = session.
                                GetObjectByKey<CheckPayment>(item.SourceID);
                                checkPayment.Adjusted = item.AdjustNow + item.
                                Adjusted;
                                ApplyDocs(session, thisPayBill.EntryDate, item.SourceID, thisPayBill.Charges);
                                checkPayment.Save();
                            }
                            if (item.OperationType.Code == "CV") {
                                CheckVoucher checkVoucher = session.
                                GetObjectByKey<CheckVoucher>(item.SourceID);
                                checkVoucher.Adjusted = item.AdjustNow + item.
                                Adjusted;
                                ApplyDocs(session, thisPayBill.EntryDate, item.SourceID, thisPayBill.Charges);
                                checkVoucher.Save();
                            }
                        }
                        if (item.SourceType.Code == "DM") {
                            DebitMemo debitMemo = session.GetObjectByKey<
                            DebitMemo>(item.SourceID);
                            debitMemo.Adjusted = item.AdjustNow + item.Adjusted;
                            if (debitMemo.Total == debitMemo.Adjusted) {
                                debitMemo.Status = DebitMemoStatusEnum.Applied;} 
                            else {
                                debitMemo.Status = DebitMemoStatusEnum.
                                PartiallyApplied;
                            }
                            ApplyDocs(session, thisPayBill.EntryDate, item.SourceID, thisPayBill.Charges);
                            debitMemo.Save();
                        }
                    }
                }
                #endregion
                #region Process Checks
                #region If Pay what is left to pay with new check
                if (thisPayBill.Amount != 0) {
                    _message = string.Format("Applying credit {0} succesfull.", 1);
                    _BgWorker.ReportProgress(1, _message);
                    if (thisPayBill.Amount > 0) {
                        if (thisPayBill.CashBankAccount == null) {throw new 
                            ApplicationException(
                            "Must specify a bank or cash account");}
                        if (thisPayBill.Vendor == null) {throw new 
                            ApplicationException("Must specify a Vendor");}
                        if (thisPayBill.Vendor.Account == null) {throw new 
                            ApplicationException(
                            "Accounts Payable account must be specified in the chosen vendor card"
                            );}
                        //  --> Cash/Bank Account
                        //  --> CheckNo
                        //  --> ReferenceNo
                        //  --> Memo
                        //  --> PayToOrder
                        //  --> CheckAmount
                        //  --> Adjusted == Checkamount
                        CheckPayment checkPayment = ReflectionHelper.
                        CreateObject<CheckPayment>(thisPayBill.Session);
                        checkPayment.EntryDate = thisPayBill.EntryDate;
                        checkPayment.CheckDate = thisPayBill.CheckDate;
                        checkPayment.PostDated = thisPayBill.PostDated;
                        checkPayment.BankCashAccount = thisPayBill.
                        CashBankAccount;
                        checkPayment.CheckNo = thisPayBill.CheckNo;
                        checkPayment.ReferenceNo = string.Empty;
                        checkPayment.Memo = "Paid from Pay Bill Module";
                        checkPayment.PayToOrder = thisPayBill.Vendor;
                        checkPayment.CheckAmount = thisPayBill.Amount;
                        checkPayment.Adjusted = thisPayBill.Amount;
                        checkPayment.Save();
                        ApplyCheckPayment(session, thisPayBill.EntryDate, checkPayment, thisPayBill.Charges);
                        // Check Details
                        foreach (PayBillExistingCharge item in thisPayBill.
                        Charges) {
                            if (item.Pay) {
                                CheckPaymentDetail checkPaymentDet = 
                                ReflectionHelper.CreateObject<CheckPaymentDetail
                                >(thisPayBill.Session);
                                checkPaymentDet.GenJournalID = checkPayment;
                                checkPaymentDet.SourceType = item.SourceType;
                                checkPaymentDet.SourceNo = item.SourceNo;
                                checkPaymentDet.Date = thisPayBill.EntryDate;
                                checkPaymentDet.Transaction = item.Transaction;
                                checkPaymentDet.Charges = item.Charges;
                                checkPaymentDet.Adjust = item.Adjust;
                                checkPaymentDet.Terms = item.Terms;
                                checkPaymentDet.Discount = item.Discount;
                                checkPaymentDet.Interest = item.Interest;
                                checkPaymentDet.AmountPaid = (item.Adjust - item
                                .Discount) + item.Interest;
                                checkPaymentDet.Save();
                            }
                        }
                        // Debit Accounts Payable
                        GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde.GenJournalID = checkPayment;
                        _gjde.GenJournalID.Approved = true;
                        _gjde.Account = thisPayBill.Vendor.Account;
                        _gjde.DebitAmount = Math.Abs(thisPayBill.Amount);
                        debitAmount = debitAmount + _gjde.DebitAmount;
                        _gjde.Description = "Payments Made";
                        _gjde.SubAccountNo = thisPayBill.Vendor;
                        _gjde.SubAccountType = thisPayBill.Vendor.ContactType;
                        _gjde.Approved = true;
                        _gjde.Save();
                        // Credit Bank or Cash Account
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = checkPayment;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = checkPayment.BankCashAccount;
                        _gjde2.CreditAmount = Math.Abs(thisPayBill.Amount);
                        creditAmount = creditAmount + _gjde2.CreditAmount;
                        _gjde2.Description = "Payments Made";
                        _gjde2.SubAccountNo = thisPayBill.Vendor;
                        _gjde2.SubAccountType = thisPayBill.Vendor.ContactType;
                        _gjde2.ExpenseType = thisPayBill.ExpenseType;
                        _gjde2.SubExpenseType = thisPayBill.SubExpenseType;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                        checkPayment.Status = CheckStatusEnum.Approved;
                        checkPayment.BankCashAccount.LastCheckNo = checkPayment.
                        CheckNo;
                        checkPayment.BankCashAccount.Save();
                        checkPayment.Save();
                    } else {
                        throw new ApplicationException(
                        "Cannot allow negative value as a check amount");
                    }
                }
                #endregion
                #endregion
            } finally {
                if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                {
                    throw new
                        ApplicationException("Accounting entries not balance");
                }
                CommitUpdatingSession(session);
            }
        }

        private void ApplyDocs(UnitOfWork session, DateTime entryDate, int srcId, XPCollection<PayBillExistingCharge> xPCollection)
        {
            GenJournalHeader hdr = session.GetObjectByKey<GenJournalHeader>(srcId);
            foreach (var item in xPCollection)
            {
                string lineId = string.Empty;
                if (item.Pay)
                {
                    lineId = string.Format("{0}{1}", hdr.SourceNo, item.SourceNo);
                    GenJournalHeader gjh = session.FindObject<GenJournalHeader>(BinaryOperator.Parse(string.Format("[SourceNo] = '{0}'", item.SourceNo)));
                    PayableDocsApplied pdapps = ReflectionHelper.CreateObject<PayableDocsApplied>(session);
                    pdapps.GenJournalID = hdr;
                    pdapps.LineID = lineId;
                    pdapps.EntryDate = entryDate;
                    pdapps.SourceType = gjh.SourceType;
                    pdapps.Source = gjh;
                    pdapps.RefNo = item.RefNo;
                    pdapps.AppliedAmount = item.Adjust;
                    pdapps.Save();
                }
            }
        }

        private void ApplyCheckPayment(UnitOfWork session, DateTime entryDate, CheckPayment chkpmt, XPCollection<PayBillExistingCharge> xPCollection)
        {
            //GenJournalHeader hdr = session.GetObjectByKey<GenJournalHeader>(srcId);
            foreach (var item in xPCollection)
            {
                string lineId = string.Empty;
                if (item.Pay)
                {
                    lineId = string.Format("{0}{1}", chkpmt.SourceNo, item.SourceNo);
                    GenJournalHeader gjh = session.FindObject<GenJournalHeader>(BinaryOperator.Parse(string.Format("[SourceNo] = '{0}'", item.SourceNo)));
                    PayableDocsApplied pdapps = ReflectionHelper.CreateObject<PayableDocsApplied>(session);
                    pdapps.GenJournalID = chkpmt;
                    pdapps.LineID = lineId;
                    pdapps.EntryDate = entryDate;
                    pdapps.SourceType = gjh.SourceType;
                    pdapps.Source = gjh;
                    pdapps.RefNo = item.RefNo;
                    pdapps.AppliedAmount = item.Adjust;
                    pdapps.Save();
                }
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
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Pay selected bills operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("Bills has been successfully paid");
                    System.Threading.Thread.Sleep(500);
                    for (int i = payBill.Charges.Count - 1; i >= 0; i--) {
                        payBill.Charges[i].Delete();}
                    for (int i = payBill.Credits.Count - 1; i >= 0; i--) {
                        payBill.Credits[i].Delete();}
                    payBill.CashBankAccount = null;
                    payBill.CheckNo = string.Empty;
                    payBill.Amount = 0;
                    payBill.Save();
                    ObjectSpace.CommitChanges();
                    ObjectSpace.ReloadObject(payBill);
                    ObjectSpace.Refresh();
                    ((DevExpress.ExpressApp.DetailView)this.View).Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { paySelectedAction.
            Enabled.SetItemValue("Releasing Check", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
