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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReconcileStanTripStatement : ViewController
    {
        private StanfilcoTripStatement stanTripStatement;
        private SimpleAction reconcileStanTripStatement;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        public ReconcileStanTripStatement()
        {
            this.TargetObjectType = typeof(StanfilcoTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReconcileStatement", this.GetType
            ().Name);
            this.reconcileStanTripStatement = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.reconcileStanTripStatement.Caption = "Reconcile";
            this.reconcileStanTripStatement.Execute += new 
            SimpleActionExecuteEventHandler(ReconcileStanTripStatement_Execute);
            this.reconcileStanTripStatement.Executed += new EventHandler<
            ActionBaseEventArgs>(ReconcileStanTripStatement_Executed);
            this.reconcileStanTripStatement.ConfirmationMessage = 
            "Do you really want to reconcile the selected entries?";
            UpdateActionState(false);
        }
        private void ReconcileStanTripStatement_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            stanTripStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as StanfilcoTripStatement;
            if (Math.Round(stanTripStatement.Balance, 2) != 0) {throw new 
                ApplicationException(
                "The balance must be zero in order to pay selected trips");}
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(stanTripStatement);
        }
        private void ReconcileStanTripStatement_Executed(object sender, 
        ActionBaseEventArgs e) {
            System.Threading.Thread.Sleep(500);
            //for (int i = stanTripStatement.Charges.Count - 1; i >= 0; i--) {
            //    stanTripStatement.Charges[i].Delete();}
            //for (int i = stanTripStatement.Payments.Count - 1; i >= 0; i--) {
            //    stanTripStatement.Payments[i].Delete();}
            //stanTripStatement.Save();
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanTripStatement);
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
            StanfilcoTripStatement _stanTripStatement = (StanfilcoTripStatement)e.
            Argument;
            StanfilcoTripStatement thisStatement = session.GetObjectByKey<
            StanfilcoTripStatement>(_stanTripStatement.Oid);
            try {
                #region Process Charge Adjustment
                foreach (StanfilcoTripCharge item in thisStatement.StanfilcoTripCharges) {
                    //if (item.Pay) {
                        #region --> Process Invoice
                        if (item.SourceType.Code == "ST") {
                            StanfilcoTrip invoice = null;
                            if (item.Adjust > item.OpenAmount) {throw new 
                                ApplicationException(
                                "You cannot pay more than you have to pay");}
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.FinanceCharge == 0) {
                                if (item.Adjust == item.OpenAmount) {
                                    invoice = session.GetObjectByKey<StanfilcoTrip>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = TripStatusEnum.Paid;
                                    invoice.Marked = false;
                                    invoice.Save();
                                }
                                if (item.Adjust != item.OpenAmount) {
                                    invoice = session.GetObjectByKey<StanfilcoTrip>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = TripStatusEnum.
                                    PartiallyPaid;
                                    invoice.Marked = false;
                                    invoice.Save();
                                }
                            }
                            #endregion
                            #region Pay with discount or with finance charge
                            if (item.Discount > 0 || item.FinanceCharge > 0) {
                                if (item.Adjust == item.OpenAmount) {
                                    invoice = session.GetObjectByKey<StanfilcoTrip>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = TripStatusEnum.Paid;
                                    invoice.Marked = false;
                                    invoice.Save();
                                }
                                if (item.Adjust != item.OpenAmount) {
                                    invoice = session.GetObjectByKey<StanfilcoTrip>(
                                    item.SourceID);
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = TripStatusEnum.
                                    PartiallyPaid;
                                    invoice.Marked = false;
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
                                if (thisStatement.Customer == null) {throw 
                                    new ApplicationException(
                                    "Must specify a customer");} else {
                                    if (thisStatement.Customer.Account == 
                                    null) {throw new ApplicationException(
                                        "Accounts receivable account must be specified in the chosen vendor card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisStatement.
                                Session);
                                journalEntry.EntryDate = thisStatement.
                                EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Discount Given to Customer #" + 
                                thisStatement.Customer.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Receivable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisStatement.Customer.
                                Account;
                                _gjde.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Discount Given";
                                _gjde.SubAccountNo = thisStatement.Customer;
                                _gjde.SubAccountType = thisStatement.Customer
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
                                _gjde2.SubAccountNo = thisStatement.Customer;
                                _gjde2.SubAccountType = thisStatement.
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
                                CreateObject<JournalEntry>(thisStatement.
                                Session);
                                journalEntry.EntryDate = thisStatement.
                                EntryDate;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Finance Charge from Customer #" + 
                                thisStatement.Customer.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Receivable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisStatement.Customer.
                                Account;
                                _gjde.DebitAmount = Math.Abs(item.FinanceCharge)
                                ;
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Aging on AR";
                                _gjde.SubAccountNo = thisStatement.Customer;
                                _gjde.SubAccountType = thisStatement.Customer
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
                                _gjde2.SubAccountNo = thisStatement.Customer;
                                _gjde2.SubAccountType = thisStatement.
                                Customer.ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }
                        #endregion
                    //}
                }
                #endregion
                #region Process Payment Adjustment
                foreach (StanfilcoTripPayment item in thisStatement.StanfilcoTripPayments) 
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
                        }
                        #endregion
                    }
                }
                #endregion
                #region Adjustments
                TempAccountCollection2 accounts = new TempAccountCollection2();
                TempAccount2 tmpAccount;
                int[] inds = null;
                foreach (var item in thisStatement.StanfilcoTripStatementAdjustments)
                {
                    if (item.Account != null && item.Amount < 0)
                    {
                        inds = null;
                        inds = accounts.Find2("AaUniqueId", string.Format("DR{0}", item.Account.No));
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += Math.Abs(item.Amount);
                        }
                        else
                        {
                            tmpAccount = new TempAccount2();
                            tmpAccount.AaUniqueId = string.Format("DR{0}", item.Account.No);
                            tmpAccount.Account = item.Account;
                            tmpAccount.DebitAmount += Math.Abs(item.Amount); ;
                            accounts.Add(tmpAccount);
                        }
                    }
                    if (item.Account != null && item.Amount > 0)
                    {
                        inds = null;
                        inds = accounts.Find2("AaUniqueId", string.Format("CR{0}", item.Account.No));
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += Math.Abs(item.Amount);
                        }
                        else
                        {
                            tmpAccount = new TempAccount2();
                            tmpAccount.AaUniqueId = string.Format("CR{0}", item.Account.No);
                            tmpAccount.Account = item.Account;
                            tmpAccount.CreditAmount += Math.Abs(item.Amount); ;
                            accounts.Add(tmpAccount);
                        }
                    }
                }
                if (accounts.Count > 0)
                {
                    JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisStatement.
                                Session);
                    journalEntry.EntryDate = thisStatement.
                    EntryDate;
                    journalEntry.ReferenceNo = thisStatement.EntryNo;
                    journalEntry.Memo =
                    "Stanfilco Trip Statement Reconciliation #" +
                    thisStatement.EntryNo;
                    foreach (TempAccount2 item in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = journalEntry;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Stanfilco Statement Adjustments";
                        _gjd.SubAccountNo = thisStatement.Customer;
                        _gjd.SubAccountType = thisStatement.Customer.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                        //Console.WriteLine("{0} {1} {2}", item.AaUniqueId, item.DebitAmount, item.CreditAmount);
                    }
                    GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                    _gjde2.GenJournalID = journalEntry;
                    _gjde2.GenJournalID.Approved = true;
                    _gjde2.Account = thisStatement.Customer.Account;
                    _gjde2.CreditAmount = Math.Abs(debitAmount - creditAmount);
                    creditAmount = creditAmount + _gjde2.CreditAmount;
                    _gjde2.Description = "Stanfilco Statement Adjustments";
                    _gjde2.SubAccountNo = thisStatement.Customer;
                    _gjde2.SubAccountType = thisStatement.
                    Customer.ContactType;
                    _gjde2.Approved = true;
                    _gjde2.Save();
                    journalEntry.Status = JournalEntryStatusEnum.Approved;
                    journalEntry.Save();
                }
                
                #endregion
                thisStatement.Reconciled=true;
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

                    ObjectSpace.ReloadObject(stanTripStatement);
                    ObjectSpace.Refresh();

                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { reconcileStanTripStatement
            .Enabled.SetItemValue("Reconciling invoices", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
