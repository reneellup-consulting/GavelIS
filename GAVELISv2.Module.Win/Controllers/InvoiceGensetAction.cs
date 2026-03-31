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
    public partial class InvoiceGensetAction : ViewController {
        private GensetEntry gensetEntry;
        private SimpleAction invoiceGensetAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public InvoiceGensetAction() {
            this.TargetObjectType = typeof(GensetEntry);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Invoice", this.GetType().Name);
            this.invoiceGensetAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.invoiceGensetAction.Caption = "Invoice";
            this.invoiceGensetAction.Execute += new 
            SimpleActionExecuteEventHandler(InvoiceGensetAction_Execute);
            this.invoiceGensetAction.Executed += new EventHandler<
            ActionBaseEventArgs>(InvoiceGensetAction_Executed);
            this.invoiceGensetAction.ConfirmationMessage = 
            "Do you really want to invoice this Genset Entry?";
            UpdateActionState(false);
        }
        private void InvoiceGensetAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            gensetEntry = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as GensetEntry;
            ObjectSpace.CommitChanges();
            var count = 1;
            _FrmProgress = new ProgressForm("Invoicing Genset...", count, 
            "Invoicing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(gensetEntry);
            _FrmProgress.ShowDialog();
        }
        private void InvoiceGensetAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(gensetEntry);
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
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            GensetEntry _gensetEntry = (GensetEntry)e.Argument;
            GensetEntry thisEntry = session.GetObjectByKey<GensetEntry>(
            _gensetEntry.Oid);
            try {
                // Validate Customer Accounts Receivable
                if (thisEntry.Customer == null) {throw new ApplicationException(
                    "Must specify a customer");} else {
                    if (thisEntry.Customer.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Receivable account must be specified in the chosen customer card"
                        );}
                    // Validate Trucking Income Account <- In Customer Card
                    if (thisEntry.Customer.GensetIncomeAcct == null) {throw new 
                        ApplicationException(
                        "Genset Income account must be specified in the chosen customer card"
                        );}
                    // Validate WHT Expense Account <- In Customer Card
                    if (thisEntry.WHTAmount > 0 && thisEntry.Customer.
                    WHTGroupCode.WHTExpenseAcct == null) {throw new 
                        ApplicationException(
                        "A WHT Group Code with WHT Expense Account must be specified in the chosen customer card"
                        );}
                }
                // Validate Output Tax Account <- TaxCode
                if (thisEntry.TripID.GetType() == typeof(StanfilcoTrip) && ((
                StanfilcoTrip)thisEntry.TripID).Tariff.TaxCode.Account == null) 
                {throw new ApplicationException(
                    "A Tax Code with Output Tax Account must be specified in the chosen tariff"
                    );}
                if (thisEntry.TripID.GetType() == typeof(DolefilTrip) && ((
                DolefilTrip)thisEntry.TripID).Tariff.TaxCode.Account == null) {
                    throw new ApplicationException(
                    "A Tax Code with Output Tax Account must be specified in the chosen tariff"
                    );}
                if (thisEntry.TripID.GetType() == typeof(OtherTrip) && ((
                OtherTrip)thisEntry.TripID).Customer.TaxCode.Account == null) {
                    throw new ApplicationException(
                    "A Tax Code with Output Tax Account must be specified in the chosen Customer"
                    );}
                // Validate Government Taxes Account
                //if (thisEntry.TripID.GetType() == typeof(StanfilcoTrip) && ((
                //StanfilcoTrip)thisEntry.TripID).Tariff.TaxCode.ExpenseAccount == 
                //null) {throw new ApplicationException(
                //    "A Tax Code with Expense Account must be specified in the chosen tariff"
                //    );}
                //if (thisEntry.TripID.GetType() == typeof(DolefilTrip) && ((
                //DolefilTrip)thisEntry.TripID).Tariff.TaxCode.ExpenseAccount == 
                //null) {throw new ApplicationException(
                //    "A Tax Code with Expense Account must be specified in the chosen tariff"
                //    );}
                //if (thisEntry.TripID.GetType() == typeof(OtherTrip) && ((
                //OtherTrip)thisEntry.TripID).Customer.TaxCode.ExpenseAccount == 
                //null) {throw new ApplicationException(
                //    "A Tax Code with Expense Account must be specified in the chosen Customer"
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
                    _gjde1.DebitAmount = Math.Abs(thisEntry.NetBilling);
                    //Init validate trans balance
                    debitAmount = debitAmount + _gjde1.DebitAmount;
                    _gjde1.Description = "Genset Invoice for Customer";
                    _gjde1.SubAccountNo = thisEntry.Customer;
                    _gjde1.SubAccountType = thisEntry.Customer.ContactType;
                    _gjde1.Approved = true;
                    _gjde1.Save();
                    // Create Output Tax <- Debit
                    if (thisEntry.VATAmount != 0) {
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = thisEntry;
                        _gjde2.GenJournalID.Approved = true;
                        if (thisEntry.TripID.GetType() == typeof(StanfilcoTrip)) 
                        {_gjde2.Account = ((StanfilcoTrip)thisEntry.TripID).
                            Tariff.TaxCode.Account;}
                        if (thisEntry.TripID.GetType() == typeof(DolefilTrip)) {
                            _gjde2.Account = ((DolefilTrip)thisEntry.TripID).
                            Tariff.TaxCode.Account;}
                        if (thisEntry.TripID.GetType() == typeof(OtherTrip)) {
                            _gjde2.Account = ((OtherTrip)thisEntry.TripID).
                            Customer.TaxCode.Account;}
                        _gjde2.CreditAmount = Math.Abs(thisEntry.VATAmount);
                        //Init validate trans balance
                        creditAmount = creditAmount + _gjde2.CreditAmount;
                        _gjde2.Description = "Genset Invoice for Customer";
                        _gjde2.SubAccountNo = thisEntry.Customer;
                        _gjde2.SubAccountType = thisEntry.Customer.ContactType;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    // Create Genset - Stanfilco <- Credit
                    GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde3.GenJournalID = thisEntry;
                    _gjde3.GenJournalID.Approved = true;
                    _gjde3.Account = thisEntry.Customer.GensetIncomeAcct;
                    _gjde3.CreditAmount = Math.Abs(thisEntry.Total);
                    // Init validate trans balance
                    creditAmount = creditAmount + _gjde3.CreditAmount;
                    _gjde3.Description = "Genset Invoice for Customer";
                    _gjde3.SubAccountNo = thisEntry.Customer;
                    _gjde3.SubAccountType = thisEntry.Customer.ContactType;
                    _gjde3.Approved = true;
                    _gjde3.Save();
                    if (thisEntry.WHTAmount != 0) {
                        // Create WHT Expense <- Credit
                        GenJournalDetail _gjde6 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde6.GenJournalID = thisEntry;
                        _gjde6.GenJournalID.Approved = true;
                        _gjde6.Account = thisEntry.Customer.WHTGroupCode.
                        WHTExpenseAcct;
                        _gjde6.DebitAmount = Math.Abs(thisEntry.WHTAmount);
                        // Init validate trans balance
                        debitAmount = debitAmount + _gjde6.DebitAmount;
                        _gjde6.Description = "Genset Invoice for Customer";
                        _gjde6.SubAccountNo = thisEntry.Customer;
                        _gjde6.SubAccountType = thisEntry.Customer.ContactType;
                        _gjde6.Approved = true;
                        _gjde6.Save();
                    }
                    // Create Government Taxes <- Credit
                    //GenJournalDetail _gjde7 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde7.GenJournalID = thisEntry;
                    //_gjde7.GenJournalID.Approved = true;
                    //if (thisEntry.TripID.GetType() == typeof(StanfilcoTrip)) {
                    //    _gjde7.Account = ((StanfilcoTrip)thisEntry.TripID).
                    //    Tariff.TaxCode.ExpenseAccount;}
                    //if (thisEntry.TripID.GetType() == typeof(DolefilTrip)) {
                    //    _gjde7.Account = ((DolefilTrip)thisEntry.TripID).Tariff.
                    //    TaxCode.ExpenseAccount;}
                    //if (thisEntry.TripID.GetType() == typeof(OtherTrip)) {_gjde7
                    //    .Account = ((OtherTrip)thisEntry.TripID).Customer.
                    //    TaxCode.ExpenseAccount;}
                    //_gjde7.CreditAmount = Math.Abs(thisEntry.VATAmount);
                    //_gjde7.Description = "Genset Invoice for Customer";
                    //_gjde7.SubAccountNo = thisEntry.Customer;
                    //_gjde7.SubAccountType = thisEntry.Customer.ContactType;
                    //_gjde7.Approved = true;
                    //_gjde7.Save();
                    thisEntry.Status = GensetStatusEnum.Invoiced;
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
                "Invoicing genset operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "This genset has been successfully Invoiced");
                    //ObjectSpace.ReloadObject(gensetEntry);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { invoiceGensetAction.
            Enabled.SetItemValue("Invoicing genset", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
