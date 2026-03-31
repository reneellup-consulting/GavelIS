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
    public partial class InvoiceShuntingAction : ViewController {
        private ShuntingEntry shuntingEntry;
        private SimpleAction invoiceShuntingAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public InvoiceShuntingAction() {
            this.TargetObjectType = typeof(ShuntingEntry);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Invoice", this.GetType().Name);
            this.invoiceShuntingAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.invoiceShuntingAction.Caption = "Invoice";
            this.invoiceShuntingAction.Execute += new 
            SimpleActionExecuteEventHandler(InvoiceShuntingAction_Execute);
            this.invoiceShuntingAction.Executed += new EventHandler<
            ActionBaseEventArgs>(InvoiceShuntingAction_Executed);
            this.invoiceShuntingAction.ConfirmationMessage = 
            "Do you really want to invoice this Shunting Entry?";
            UpdateActionState(false);
        }
        private void InvoiceShuntingAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            shuntingEntry = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ShuntingEntry;
            ObjectSpace.CommitChanges();
            var count = 1;
            _FrmProgress = new ProgressForm("Invoicing Shunting...", count, 
            "Invoicing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(shuntingEntry);
            _FrmProgress.ShowDialog();
        }
        private void InvoiceShuntingAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(shuntingEntry);
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            ShuntingEntry _shuntingEntry = (ShuntingEntry)e.Argument;
            ShuntingEntry thisEntry = session.GetObjectByKey<ShuntingEntry>(
            _shuntingEntry.Oid);
            try {
                // Validate Customer Accounts Receivable
                if (thisEntry.Customer == null) {throw new ApplicationException(
                    "Must specify a customer");} else {
                    if (thisEntry.Customer.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Receivable account must be specified in the chosen customer card"
                        );}
                    // Validate Trucking Income Account <- In Customer Card
                    if (thisEntry.Customer.ShuntingIncomeAcct == null) {throw 
                        new ApplicationException(
                        "Shunting Income account must be specified in the chosen customer card"
                        );}
                    // Validate WHT Expense Account <- In Customer Card
                    if (thisEntry.Customer.WHTGroupCode.WHTExpenseAcct == null) 
                    {throw new ApplicationException(
                        "A WHT Group Code with WHT Expense Account must be specified in the chosen customer card"
                        );}
                }
                // Validate Output Tax Account <- TaxCode
                if (((StanfilcoTrip)thisEntry.TripID).Tariff.TaxCode.Account == 
                null) {throw new ApplicationException(
                    "A Tax Code with Output Tax Account must be specified in the chosen tariff"
                    );}
                // Validate Government Taxes Account
                //if (((StanfilcoTrip)thisEntry.TripID).Tariff.TaxCode.ExpenseAccount == null)
                //{
                //    throw new 
                //    ApplicationException(
                //    "A Tax Code with Expense Account must be specified in the chosen tariff"
                //    );}
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
            } finally {
                if (index == 1) {
                    // Create Accounts Receivable -> TruckerPaye + RatedAdjmt + TrailerRental + Insurance + WHTAmount <- Debit
                    GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde1.GenJournalID = thisEntry;
                    _gjde1.GenJournalID.Approved = true;
                    _gjde1.Account = thisEntry.Customer.Account;
                    _gjde1.DebitAmount = Math.Abs(thisEntry.GrossBilling);
                    debitAmount = debitAmount + _gjde1.DebitAmount;
                    _gjde1.Description = "Shunting Invoice for Customer";
                    _gjde1.SubAccountNo = thisEntry.Customer;
                    _gjde1.SubAccountType = thisEntry.Customer.ContactType;
                    _gjde1.Approved = true;
                    _gjde1.Save();
                    // Create Genset - Stanfilco <- Credit
                    GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde3.GenJournalID = thisEntry;
                    _gjde3.GenJournalID.Approved = true;
                    _gjde3.Account = thisEntry.Customer.ShuntingIncomeAcct;
                    _gjde3.CreditAmount = Math.Abs(thisEntry.GrossBilling);
                    creditAmount = creditAmount + _gjde3.CreditAmount;
                    _gjde3.Description = "Shunting Invoice for Customer";
                    _gjde3.SubAccountNo = thisEntry.Customer;
                    _gjde3.SubAccountType = thisEntry.Customer.ContactType;
                    _gjde3.Approved = true;
                    _gjde3.Save();
                    #region Old Algorithms
                    //// Create Output Tax <- Debit
                    //if (thisEntry.VATAmount != 0)
                    //{
                    //    GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    //    GenJournalDetail>(session);
                    //    _gjde2.GenJournalID = thisEntry;
                    //    _gjde2.GenJournalID.Approved = true;
                    //    _gjde2.Account = ((StanfilcoTrip)thisEntry.TripID).
                    //    Tariff.TaxCode.Account;
                    //    _gjde2.CreditAmount = Math.Abs(thisEntry.VATAmount);
                    //    creditAmount = creditAmount + _gjde2.CreditAmount;
                    //    _gjde2.Description = "Shunting Invoice for Customer";
                    //    _gjde2.SubAccountNo = thisEntry.Customer;
                    //    _gjde2.SubAccountType = thisEntry.Customer.ContactType;
                    //    _gjde2.Approved = true;
                    //    _gjde2.Save();
                    //}
                    //// Create Genset - Stanfilco <- Credit
                    //GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde3.GenJournalID = thisEntry;
                    //_gjde3.GenJournalID.Approved = true;
                    //_gjde3.Account = thisEntry.Customer.ShuntingIncomeAcct;
                    //_gjde3.CreditAmount = Math.Abs(thisEntry.Total);
                    //creditAmount = creditAmount + _gjde3.CreditAmount;
                    //_gjde3.Description = "Shunting Invoice for Customer";
                    //_gjde3.SubAccountNo = thisEntry.Customer;
                    //_gjde3.SubAccountType = thisEntry.Customer.ContactType;
                    //_gjde3.Approved = true;
                    //_gjde3.Save();
                    //// Create WHT Expense <- Credit
                    //if (thisEntry.WHTAmount != 0)
                    //{
                    //    GenJournalDetail _gjde6 = ReflectionHelper.CreateObject<
                    //    GenJournalDetail>(session);
                    //    _gjde6.GenJournalID = thisEntry;
                    //    _gjde6.GenJournalID.Approved = true;
                    //    _gjde6.Account = thisEntry.Customer.WHTGroupCode.
                    //    WHTExpenseAcct;
                    //    _gjde6.DebitAmount = Math.Abs(thisEntry.WHTAmount);
                    //    debitAmount = debitAmount + _gjde6.DebitAmount;
                    //    _gjde6.Description = "Shunting Invoice for Customer";
                    //    _gjde6.SubAccountNo = thisEntry.Customer;
                    //    _gjde6.SubAccountType = thisEntry.Customer.ContactType;
                    //    _gjde6.Approved = true;
                    //    _gjde6.Save();
                    //}
                    //// Create Government Taxes <- Credit
                    ////GenJournalDetail _gjde7 = ReflectionHelper.CreateObject<
                    ////GenJournalDetail>(session);
                    ////_gjde7.GenJournalID = thisEntry;
                    ////_gjde7.GenJournalID.Approved = true;
                    ////_gjde7.Account = ((StanfilcoTrip)thisEntry.TripID).Tariff.TaxCode.ExpenseAccount;
                    ////_gjde7.CreditAmount = Math.Abs(thisEntry.VATAmount);
                    ////_gjde7.Description = "Shunting Invoice for Customer";
                    ////_gjde7.SubAccountNo = thisEntry.Customer;
                    ////_gjde7.SubAccountType = thisEntry.Customer.ContactType;
                    ////_gjde7.Approved = true;
                    ////_gjde7.Save();
                    #endregion
                    thisEntry.Status = ShuntingStatusEnum.Invoiced;
                    thisEntry.Save();
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                            ApplicationException("Accounting entries not balance");
                    }
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void BgWorkerProgressChanged(object sender, 
        ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
                DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Invoicing shunting operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "This shunting has been successfully Invoiced");
                    ObjectSpace.ReloadObject(shuntingEntry);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { invoiceShuntingAction.
            Enabled.SetItemValue("Invoicing shunting", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
