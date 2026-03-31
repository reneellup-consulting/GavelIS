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
    public partial class InvoiceStanfilcoTripAction : ViewController {
        private StanfilcoTrip stanfilcoTrip;
        private SimpleAction invoiceStanfilcoTripAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public InvoiceStanfilcoTripAction() {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Invoice", this.GetType().Name);
            this.invoiceStanfilcoTripAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.invoiceStanfilcoTripAction.Caption = "Invoice";
            this.invoiceStanfilcoTripAction.Execute += new 
            SimpleActionExecuteEventHandler(InvoiceStanfilcoTripAction_Execute);
            this.invoiceStanfilcoTripAction.Executed += new EventHandler<
            ActionBaseEventArgs>(InvoiceStanfilcoTripAction_Executed);
            this.invoiceStanfilcoTripAction.ConfirmationMessage = 
            "Do you really want to invoice this trip?";
            UpdateActionState(false);
        }
        private void InvoiceStanfilcoTripAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            stanfilcoTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as StanfilcoTrip;
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
            _BgWorker.RunWorkerAsync(stanfilcoTrip);
            _FrmProgress.ShowDialog();
        }
        private void InvoiceStanfilcoTripAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(stanfilcoTrip);
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
            StanfilcoTrip _stanfilco = (StanfilcoTrip)e.Argument;
            StanfilcoTrip thisTrip = session.GetObjectByKey<StanfilcoTrip>(
            _stanfilco.Oid);
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
                    if (thisTrip.RentTrailer && thisTrip.Customer.
                    TrailerIncomeAcct == null) {throw new ApplicationException(
                        "Trailer Income account must be specified in the chosen customer card"
                        );}
                    // Validate WHT Expense Account <- In Customer Card
                    if (thisTrip.Customer.WHTGroupCode.WHTExpenseAcct == null) {
                        throw new ApplicationException(
                        "A WHT Group Code with WHT Expense Account must be specified in the chosen customer card"
                        );}
                }
                // Validate Insurance Expense Account
                if (thisTrip.Tariff.InsuranceExpenseAcct == null) {throw new 
                    ApplicationException(
                    "An Insurance Expense account must be specified in the chosen tariff"
                    );}
                // Validate Output Tax Account <- TaxCode
                if (thisTrip.Tariff.TaxCode.Account == null) {throw new 
                    ApplicationException(
                    "A Tax Code with Output Tax Account must be specified in the chosen tariff"
                    );}
                // Validate Government Taxes Account
                //if (thisTrip.Tariff.TaxCode.ExpenseAccount == null) {throw new 
                //    ApplicationException(
                //    "A Tax Code with Expense Account must be specified in the chosen tariff"
                //    );}
                System.Threading.Thread.Sleep(20);
                _BgWorker.ReportProgress(1, _message);
                index++;
            } finally {
                if (index == 1)
                {
                    //#region Old algorithm
                    //// Create Accounts Receivable -> TruckerPaye + RatedAdjmt + TrailerRental + Insurance + WHTAmount <- Debit
                    //GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde1.GenJournalID = thisTrip;
                    //_gjde1.GenJournalID.Approved = true;
                    //_gjde1.Account = thisTrip.Customer.Account;
                    //_gjde1.DebitAmount = Math.Abs(thisTrip.NewNetBilling);
                    //debitAmount = debitAmount + _gjde1.DebitAmount;
                    //_gjde1.Description = "Stanfilco Trip Invoice for Customer";
                    //_gjde1.SubAccountNo = thisTrip.Customer;
                    //_gjde1.SubAccountType = thisTrip.Customer.ContactType;
                    //_gjde1.Approved = true;
                    //_gjde1.Save();
                    //Console.WriteLine(string.Format("Dr. NewNetBilling {0}", _gjde1.DebitAmount));
                    //// Create Output Tax <- Debit
                    //GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde2.GenJournalID = thisTrip;
                    //_gjde2.GenJournalID.Approved = true;
                    //_gjde2.Account = thisTrip.Tariff.TaxCode.Account;
                    //_gjde2.CreditAmount = Math.Abs(thisTrip.NewVatAmount);
                    //creditAmount = creditAmount + _gjde2.CreditAmount;
                    //_gjde2.Description = "Stanfilco Trip Invoice for Customer";
                    //_gjde2.SubAccountNo = thisTrip.Customer;
                    //_gjde2.SubAccountType = thisTrip.Customer.ContactType;
                    //_gjde2.Approved = true;
                    //_gjde2.Save();
                    //Console.WriteLine(string.Format("Cr. NewVatAmount {0}", _gjde2.CreditAmount));
                    //// Create Trucking - Stanfilco <- Credit
                    //GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde3.GenJournalID = thisTrip;
                    //_gjde3.GenJournalID.Approved = true;
                    //_gjde3.Account = thisTrip.Customer.TruckingIncomeAcct;
                    ////if (!thisTrip.RentTrailer)
                    ////{
                    ////    _gjde3.CreditAmount = Math.Abs((
                    ////        thisTrip.TruckerPay - thisTrip.Tariff.TrailerRental) +
                    ////        thisTrip.RateAdjmt);
                    ////}
                    ////else
                    ////{
                    ////    _gjde3.CreditAmount = Math.Abs((thisTrip.TruckerPay -
                    ////    thisTrip.TrailerRental) + thisTrip.RateAdjmt);
                    ////}
                    //_gjde3.CreditAmount = thisTrip.NewBilling;
                    //creditAmount = creditAmount + _gjde3.CreditAmount;
                    //_gjde3.Description = "Stanfilco Trip Invoice for Customer";
                    //_gjde3.SubAccountNo = thisTrip.Customer;
                    //_gjde3.SubAccountType = thisTrip.Customer.ContactType;
                    //_gjde3.Approved = true;
                    //_gjde3.Save();
                    //Console.WriteLine(string.Format("Cr. NewBilling {0}", _gjde3.CreditAmount));
                    //if (thisTrip.TrailerRental > 0m)
                    //{
                    //    // Create Income - Trailers <- Credit
                    //    GenJournalDetail _gjde4 = ReflectionHelper.CreateObject<
                    //    GenJournalDetail>(session);
                    //    _gjde4.GenJournalID = thisTrip;
                    //    _gjde4.GenJournalID.Approved = true;
                    //    _gjde4.Account = thisTrip.Customer.TrailerIncomeAcct;
                    //    _gjde4.CreditAmount = Math.Abs(thisTrip.TrailerRental);
                    //    creditAmount = creditAmount + _gjde4.CreditAmount;
                    //    _gjde4.Description =
                    //    "Stanfilco Trip Invoice for Customer";
                    //    _gjde4.SubAccountNo = thisTrip.Customer;
                    //    _gjde4.SubAccountType = thisTrip.Customer.ContactType;
                    //    _gjde4.Approved = true;
                    //    _gjde4.Save();
                    //    Console.WriteLine(string.Format("Cr. TrailerRental {0}", _gjde4.CreditAmount));
                    //}
                    //// Create Cargo Insurance Expense <- Credit
                    //GenJournalDetail _gjde5 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde5.GenJournalID = thisTrip;
                    //_gjde5.GenJournalID.Approved = true;
                    //_gjde5.Account = thisTrip.Tariff.InsuranceExpenseAcct;
                    //_gjde5.DebitAmount = Math.Abs(thisTrip.Insurance);
                    //debitAmount = debitAmount + _gjde5.DebitAmount;
                    //_gjde5.Description = "Stanfilco Trip Invoice for Customer";
                    //_gjde5.SubAccountNo = thisTrip.Customer;
                    //_gjde5.SubAccountType = thisTrip.Customer.ContactType;
                    //_gjde5.Approved = true;
                    //_gjde5.Save();
                    //Console.WriteLine(string.Format("Dr. Insurance {0}", _gjde5.DebitAmount));
                    //// Create WHT Expense <- Credit
                    //GenJournalDetail _gjde6 = ReflectionHelper.CreateObject<
                    //GenJournalDetail>(session);
                    //_gjde6.GenJournalID = thisTrip;
                    //_gjde6.GenJournalID.Approved = true;
                    //_gjde6.Account = thisTrip.Customer.WHTGroupCode.
                    //WHTExpenseAcct;
                    //_gjde6.DebitAmount = Math.Abs(thisTrip.WHTAmount);
                    //debitAmount = debitAmount + _gjde6.DebitAmount;
                    //_gjde6.Description = "Stanfilco Trip Invoice for Customer";
                    //_gjde6.SubAccountNo = thisTrip.Customer;
                    //_gjde6.SubAccountType = thisTrip.Customer.ContactType;
                    //_gjde6.Approved = true;
                    //_gjde6.Save();
                    //Console.WriteLine(string.Format("Dr. WHTAmount {0}", _gjde6.DebitAmount));
                    //// Create Government Taxes <- Credit
                    ////GenJournalDetail _gjde7 = ReflectionHelper.CreateObject<
                    ////GenJournalDetail>(session);
                    ////_gjde7.GenJournalID = thisTrip;
                    ////_gjde7.GenJournalID.Approved = true;
                    ////_gjde7.Account = thisTrip.Tariff.TaxCode.ExpenseAccount;
                    ////_gjde7.CreditAmount = Math.Abs(thisTrip.VATAmount);
                    ////_gjde7.Description = "Stanfilco Trip Invoice for Customer";
                    ////_gjde7.SubAccountNo = thisTrip.Customer;
                    ////_gjde7.SubAccountType = thisTrip.Customer.ContactType;
                    ////_gjde7.Approved = true;
                    ////_gjde7.Save();
                    //#endregion

                    #region New algorithm
                    TempAccountCollection2 accounts = new TempAccountCollection2();
                    TempAccount2 tmpAccount;
                    int[] inds = null;
                    foreach (var item in thisTrip.TripCalculationDetails)
                    {
                        if (item.GlAccount != null && item.Value > 0m)
                        {
                            //Console.WriteLine(string.Format("Dr. {0} {1:n2}", item.GlAccount.DisplayName, item.Value));
                            inds = null;
                            inds = accounts.Find2("AaUniqueId", string.Format("DR{0}", item.GlAccount.No));
                            if (inds != null && inds.Length > 0)
                            {
                                tmpAccount = accounts[inds[0]];
                                tmpAccount.DebitAmount += item.Value;
                            }
                            else
                            {
                                tmpAccount = new TempAccount2();
                                tmpAccount.AaUniqueId = string.Format("DR{0}", item.GlAccount.No);
                                tmpAccount.Account = item.GlAccount;
                                tmpAccount.DebitAmount += item.Value;
                                accounts.Add(tmpAccount);
                            }
                        }
                        if (item.GlAccount2 != null && item.Value > 0m)
                        {
                            //Console.WriteLine(string.Format("Cr. {0} {1:n2}", item.GlAccount2.DisplayName, item.Value));
                            inds = null;
                            inds = accounts.Find2("AaUniqueId", string.Format("CR{0}", item.GlAccount2.No));
                            if (inds != null && inds.Length > 0)
                            {
                                tmpAccount = accounts[inds[0]];
                                tmpAccount.CreditAmount += item.Value;
                            }
                            else
                            {
                                tmpAccount = new TempAccount2();
                                tmpAccount.AaUniqueId = string.Format("CR{0}", item.GlAccount2.No);
                                tmpAccount.Account = item.GlAccount2;
                                tmpAccount.CreditAmount += item.Value;
                                accounts.Add(tmpAccount);
                            }
                        }
                    }

                    foreach (TempAccount2 item in accounts){
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisTrip;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Stanfilco Trip Invoice for Customer";
                        _gjd.SubAccountNo = thisTrip.Customer;
                        _gjd.SubAccountType = thisTrip.Customer.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                        //Console.WriteLine("{0} {1} {2}", item.AaUniqueId, item.DebitAmount, item.CreditAmount);
                    }
                    #endregion
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
                    ObjectSpace.ReloadObject(stanfilcoTrip);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { 
            invoiceStanfilcoTripAction.Enabled.SetItemValue("Invoicing trip", !
            inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
