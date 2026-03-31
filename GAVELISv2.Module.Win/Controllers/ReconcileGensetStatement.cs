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
    public partial class ReconcileGensetStatement : ViewController {
        private GensetStatement gensetStatement;
        private SimpleAction reconcileGensetStatement;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        public ReconcileGensetStatement() {
            this.TargetObjectType = typeof(GensetStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReconcileStatement", this.
            GetType().Name);
            this.reconcileGensetStatement = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.reconcileGensetStatement.Caption = "Reconcile";
            this.reconcileGensetStatement.Execute += new 
            SimpleActionExecuteEventHandler(ReconcileGensetStatement_Execute);
            this.reconcileGensetStatement.Executed += new EventHandler<
            ActionBaseEventArgs>(ReconcileGensetStatement_Executed);
            this.reconcileGensetStatement.ConfirmationMessage = 
            "Do you really want to reconcile the selected entries?";
            UpdateActionState(false);
        }
        private void ReconcileGensetStatement_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            gensetStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as GensetStatement;
            if (Math.Round(gensetStatement.Balance, 2) != 0) {throw new 
                ApplicationException(
                "The balance must be zero in order to pay selected trips");}
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(gensetStatement);
        }
        private void ReconcileGensetStatement_Executed(object sender, 
        ActionBaseEventArgs e) {
            System.Threading.Thread.Sleep(500);
            //for (int i = stanTripStatement.Charges.Count - 1; i >= 0; i--) {
            //    stanTripStatement.Charges[i].Delete();}
            //for (int i = stanTripStatement.Payments.Count - 1; i >= 0; i--) {
            //    stanTripStatement.Payments[i].Delete();}
            //stanTripStatement.Save();
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(gensetStatement);
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
            GensetStatement _gensetStatement = (GensetStatement)e.Argument;
            GensetStatement thisStatement = session.GetObjectByKey<
            GensetStatement>(_gensetStatement.Oid);
            try {
                #region Process Charge Adjustment
                foreach (GensetCharge item in thisStatement.GensetCharges) {
                    if (item.Pay) {
                        #region --> Process Invoice
                        if (item.SourceType.Code == "GS") {
                            GensetEntry invoice = null;
                            if (item.Adjust > item.OpenAmount) {throw new 
                                ApplicationException(
                                "You cannot pay more than you have to pay");}
                            #region With/Without adjustment only
                            if (item.Discount == 0 && item.FinanceCharge == 0) {
                                if (item.Adjust == item.OpenAmount) {
                                    invoice = session.GetObjectByKey<GensetEntry
                                    >(item.SourceID);
                                    invoice.TempSkipOdoReg = true;
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = GensetStatusEnum.Paid;
                                    invoice.Save();
                                }
                                if (item.Adjust != item.OpenAmount) {
                                    invoice = session.GetObjectByKey<GensetEntry
                                    >(item.SourceID);
                                    invoice.TempSkipOdoReg = true;
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = GensetStatusEnum.
                                    PartiallyPaid;
                                    invoice.Save();
                                }
                            }
                            #endregion
                            #region Pay with discount or with finance charge
                            if (item.Discount > 0 || item.FinanceCharge > 0) {
                                if (item.Adjust == item.OpenAmount) {
                                    invoice = session.GetObjectByKey<GensetEntry
                                    >(item.SourceID);
                                    invoice.TempSkipOdoReg = true;
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = GensetStatusEnum.Paid;
                                    invoice.Save();
                                }
                                if (item.Adjust != item.OpenAmount) {
                                    invoice = session.GetObjectByKey<GensetEntry
                                    >(item.SourceID);
                                    invoice.TempSkipOdoReg = true;
                                    invoice.OpenAmount = item.OpenAmount - item.
                                    Adjust;
                                    invoice.Status = GensetStatusEnum.
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
                                if (thisStatement.Customer == null) {throw new 
                                    ApplicationException(
                                    "Must specify a customer");} else {
                                    if (thisStatement.Customer.Account == null) 
                                    {throw new ApplicationException(
                                        "Accounts receivable account must be specified in the chosen vendor card"
                                        );}
                                }
                                // Create Journal Entry
                                JournalEntry journalEntry = ReflectionHelper.
                                CreateObject<JournalEntry>(thisStatement.Session
                                );
                                journalEntry.EntryDate = thisStatement.EntryDate
                                ;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Discount Given to Customer #" + thisStatement.
                                Customer.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Credit Accounts Receivable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisStatement.Customer.Account;
                                _gjde.CreditAmount = Math.Abs(item.Discount);
                                creditAmount = creditAmount + _gjde.CreditAmount;
                                _gjde.Description = "Discount Given";
                                _gjde.SubAccountNo = thisStatement.Customer;
                                _gjde.SubAccountType = thisStatement.Customer.
                                ContactType;
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
                                _gjde2.SubAccountType = thisStatement.Customer.
                                ContactType;
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
                                CreateObject<JournalEntry>(thisStatement.Session
                                );
                                journalEntry.EntryDate = thisStatement.EntryDate
                                ;
                                journalEntry.ReferenceNo = item.SourceNo;
                                journalEntry.Memo = 
                                "Finance Charge from Customer #" + thisStatement
                                .Customer.No;
                                journalEntry.Status = JournalEntryStatusEnum.
                                Approved;
                                journalEntry.Save();
                                // Debit Accounts Receivable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = journalEntry;
                                _gjde.GenJournalID.Approved = true;
                                _gjde.Account = thisStatement.Customer.Account;
                                _gjde.DebitAmount = Math.Abs(item.FinanceCharge)
                                ;
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = "Aging on AR";
                                _gjde.SubAccountNo = thisStatement.Customer;
                                _gjde.SubAccountType = thisStatement.Customer.
                                ContactType;
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
                                _gjde2.SubAccountType = thisStatement.Customer.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                #endregion
                #region Process Payment Adjustment
                foreach (GensetPayment item in thisStatement.GensetPayments) {
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
                thisStatement.Reconciled = true;
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
                    ObjectSpace.ReloadObject(gensetStatement);
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { 
            reconcileGensetStatement.Enabled.SetItemValue("Reconciling invoices"
            , !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
