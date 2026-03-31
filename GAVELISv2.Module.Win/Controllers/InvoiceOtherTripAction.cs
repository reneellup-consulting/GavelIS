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
    public partial class InvoiceOtherTripAction : ViewController {
        private OtherTrip otherTrip;
        private SimpleAction invoiceOtherTripAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public InvoiceOtherTripAction() {
            this.TargetObjectType = typeof(OtherTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Invoice", this.GetType().Name);
            this.invoiceOtherTripAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.invoiceOtherTripAction.Caption = "Invoice";
            this.invoiceOtherTripAction.Execute += new 
            SimpleActionExecuteEventHandler(InvoiceOtherTripAction_Execute);
            this.invoiceOtherTripAction.Executed += new EventHandler<
            ActionBaseEventArgs>(InvoiceOtherTripAction_Executed);
            this.invoiceOtherTripAction.ConfirmationMessage = 
            "Do you really want to invoice this trip?";
            UpdateActionState(false);
        }
        private void InvoiceOtherTripAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            otherTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as OtherTrip;
            ObjectSpace.CommitChanges();
            var count = 1;
            _FrmProgress = new ProgressForm("Invoicing trip...", count, 
            "Invoicing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(otherTrip);
            _FrmProgress.ShowDialog();
        }
        private void InvoiceOtherTripAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(otherTrip);
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
            OtherTrip _other = (OtherTrip)e.Argument;
            OtherTrip thisTrip = session.GetObjectByKey<OtherTrip>(_other.Oid);
            try {
                // Validate Customer Accounts Receivable
                if (thisTrip.Customer == null) {throw new ApplicationException(
                    "Must specify a customer");} else {
                    if (thisTrip.Customer.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Receivable account must be specified in the chosen customer card"
                        );}
                    // Validate Trucking Income Account <- In Customer Card
                    if (thisTrip.Customer.TruckingIncomeAcct == null) {throw new 
                        ApplicationException(
                        "Trucking Income account must be specified in the chosen customer card"
                        );}
                    // Validate Trailer Income Account <- In Customer Card
                    if (thisTrip.TrailerRental > 0 && thisTrip.Customer.
                    TrailerIncomeAcct == null) {throw new ApplicationException(
                        "Trailer Income account must be specified in the chosen customer card"
                        );}
                    // Validate WHT Expense Account <- In Customer Card
                    //if (thisTrip.Tariff.WHTInclusive && thisTrip.Customer.
                    //WHTGroupCode.WHTExpenseAcct == null) {throw new 
                    //    ApplicationException(
                    //    "A WHT Group Code with WHT Expense Account must be specified in the chosen customer card"
                    //    );}
                }
                // Validate Output Tax Account <- TaxCode
                if (thisTrip.VATAmount > 0 && thisTrip.Customer.TaxCode.Account 
                == null) {throw new ApplicationException(
                    "A Tax Code with Output Tax Account must be specified in the chosen tariff"
                    );}
                // Validate Government Taxes Account
                //if (thisTrip.VATAmount > 0 && thisTrip.Customer.TaxCode.
                //ExpenseAccount == null) {throw new ApplicationException(
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
                    _gjde1.GenJournalID = thisTrip;
                    _gjde1.GenJournalID.Approved = true;
                    _gjde1.Account = thisTrip.Customer.Account;
                    _gjde1.DebitAmount = Math.Abs(thisTrip.GrossBilling);
                    debitAmount = debitAmount + _gjde1.DebitAmount;
                    _gjde1.Description = "Other Trip Invoice for Customer";
                    _gjde1.SubAccountNo = thisTrip.Customer;
                    _gjde1.SubAccountType = thisTrip.Customer.ContactType;
                    _gjde1.Approved = true;
                    _gjde1.Save();
                    // Create Output Tax <- Debit
                    if (thisTrip.VATAmount > 0) {
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = thisTrip;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = thisTrip.Customer.TaxCode.Account;
                        _gjde2.CreditAmount = Math.Abs(thisTrip.VATAmount);
                        creditAmount = creditAmount + _gjde2.CreditAmount;
                        _gjde2.Description = "Other Trip Invoice for Customer";
                        _gjde2.SubAccountNo = thisTrip.Customer;
                        _gjde2.SubAccountType = thisTrip.Customer.ContactType;
                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    // Create Trucking - Stanfilco <- Credit
                    GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde3.GenJournalID = thisTrip;
                    _gjde3.GenJournalID.Approved = true;
                    _gjde3.Account = thisTrip.Customer.TruckingIncomeAcct;
                    _gjde3.CreditAmount = Math.Abs(thisTrip.TruckerPay);
                    creditAmount = creditAmount + _gjde3.CreditAmount;
                    _gjde3.Description = "Other Trip Invoice for Customer";
                    _gjde3.SubAccountNo = thisTrip.Customer;
                    _gjde3.SubAccountType = thisTrip.Customer.ContactType;
                    _gjde3.Approved = true;
                    _gjde3.Save();
                    if (thisTrip.TrailerRental > 0) {
                        // Create Income - Trailers <- Credit
                        GenJournalDetail _gjde4 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde4.GenJournalID = thisTrip;
                        _gjde4.GenJournalID.Approved = true;
                        _gjde4.Account = thisTrip.Customer.TrailerIncomeAcct;
                        _gjde4.CreditAmount = Math.Abs(thisTrip.TrailerRental);
                        creditAmount = creditAmount + _gjde4.CreditAmount;
                        _gjde4.Description = "Other Trip Invoice for Customer";
                        _gjde4.SubAccountNo = thisTrip.Customer;
                        _gjde4.SubAccountType = thisTrip.Customer.ContactType;
                        _gjde4.Approved = true;
                        _gjde4.Save();
                    }
                    //if (thisTrip.Tariff.WHTInclusive) {
                    //    // Create WHT Expense <- Credit
                    //    GenJournalDetail _gjde6 = ReflectionHelper.CreateObject<
                    //    GenJournalDetail>(session);
                    //    _gjde6.GenJournalID = thisTrip;
                    //    _gjde6.GenJournalID.Approved = true;
                    //    _gjde6.Account = thisTrip.Customer.WHTGroupCode.
                    //    WHTExpenseAcct;
                    //    _gjde6.CreditAmount = Math.Abs(thisTrip.WHTAmount);
                    //    _gjde6.Description = "Dolefil Trip Invoice for Customer"
                    //    ;
                    //    _gjde6.SubAccountNo = thisTrip.Customer;
                    //    _gjde6.SubAccountType = thisTrip.Customer.ContactType;
                    //    _gjde6.Approved = true;
                    //    _gjde6.Save();
                    //}
                    // Create Government Taxes <- Credit
                    //if (thisTrip.VATAmount > 0) {
                    //    GenJournalDetail _gjde7 = ReflectionHelper.CreateObject<
                    //    GenJournalDetail>(session);
                    //    _gjde7.GenJournalID = thisTrip;
                    //    _gjde7.GenJournalID.Approved = true;
                    //    _gjde7.Account = thisTrip.Customer.TaxCode.
                    //    ExpenseAccount;
                    //    _gjde7.CreditAmount = Math.Abs(thisTrip.VATAmount);
                    //    _gjde7.Description = "Other Trip Invoice for Customer";
                    //    _gjde7.SubAccountNo = thisTrip.Customer;
                    //    _gjde7.SubAccountType = thisTrip.Customer.ContactType;
                    //    _gjde7.Approved = true;
                    //    _gjde7.Save();
                    //}
                    thisTrip.Status = TripStatusEnum.Invoiced;
                    thisTrip.Save();
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
                "Invoicing trip operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "This trip has been successfully Invoiced");

                    ObjectSpace.ReloadObject(otherTrip);
                    ObjectSpace.Refresh();

                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { invoiceOtherTripAction
            .Enabled.SetItemValue("Invoicing trip", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
