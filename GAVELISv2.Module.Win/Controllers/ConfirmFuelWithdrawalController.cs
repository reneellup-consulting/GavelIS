using System;
using System.Linq;
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
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ConfirmFuelWithdrawalController : ViewController
    {
        private FuelPumpRegister fuelPumpReg;
        private SimpleAction confirmAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ConfirmFuelWithdrawalController()
        {
            this.TargetObjectType = typeof(FuelPumpRegister);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Confirm", this.GetType().Name);
            this.confirmAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.confirmAction.Caption = "Confirm";
            this.confirmAction.Execute += new
            SimpleActionExecuteEventHandler(ConfirmAction_Execute);
            this.confirmAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ConfirmAction_Executed);
            this.confirmAction.ConfirmationMessage =
            "Do you really want to confirm these entries?";
            UpdateActionState(false);
        }
        private void ConfirmAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            fuelPumpReg = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FuelPumpRegister;
            if (fuelPumpReg.TransactionType== FuelPumpRegisterTypeEnum.ChargeToEmployee && string.IsNullOrEmpty(fuelPumpReg.ChargeSlipNo))
            {
                throw new ApplicationException("Must provide an ECS number");
            }
            ObjectSpace.CommitChanges();
            if (fuelPumpReg.FuelPumpRegisterDetails.Count == 0)
            {
                XtraMessageBox.Show("There are no entries to confirm",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = fuelPumpReg.WorkOrderItemDetails.Count;
            _FrmProgress = new ProgressForm("Confirm entries...", count,
            "Confirming entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(fuelPumpReg);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            FuelPumpRegister _reg = (FuelPumpRegister)e.Argument;
            FuelPumpRegister thisReg = session.GetObjectByKey<FuelPumpRegister>(_reg.Oid);
            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            int partial = 0;
            Customer defaultCustomer = session.FindObject<Customer>(CriteriaOperator.Parse("[No] = 'AC00001'"));
            try
            {
                // Validate Customer Accounts Receivable
                // Make GSC GAVEL TRUCKING default customer
                if (defaultCustomer == null)
                {
                    throw new
                        ApplicationException("Customer GSC GAVEL TRUCKING does not exist in the system");
                }
                else
                {
                    if (defaultCustomer.Account == null)
                    {
                        throw new
                            ApplicationException(
                            "Accounts Receivable account must be specified in the chosen customer card"
                            );
                    }
                }

                foreach (FuelPumpRegisterDetail item in thisReg.FuelPumpRegisterDetails)
                {
                    index++;
                    _message = string.Format("Confirming entry {0} succesfull.",
                    thisReg.FuelPumpRegisterDetails.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    _icj = session.FindObject<InventoryControlJournal>(
                    new BinaryOperator("RowID", item.RowID.ToString()));
                    if (_icj != null)
                    {
                        _icj.GenJournalID = thisReg;
                        _icj.DateIssued = thisReg.EntryDate;
                        _icj.OutQty = Math.Abs(item.BaseQTY);
                        _icj.Warehouse = item.Warehouse;
                        _icj.ItemNo = item.ItemNo;
                        _icj.Cost = item.Cost;
                        _icj.Price = item.Cost;
                        _icj.UOM = item.BaseUOM;
                        _icj.RowID = item.RowID.ToString();
                        _icj.CostCenter = thisReg.ChargeTo != null ? thisReg.ChargeTo : null;
                        _icj.RequestedBy = thisReg.Requestor != null ? thisReg.Requestor : null;
                        _icj.Save();
                    }
                    else
                    {
                        _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                        _icj.GenJournalID = thisReg;
                        _icj.DateIssued = thisReg.EntryDate;
                        _icj.OutQty = Math.Abs(item.BaseQTY);
                        _icj.Warehouse = item.Warehouse;
                        _icj.ItemNo = item.ItemNo;
                        _icj.Cost = item.Cost;
                        _icj.Price = item.Cost;
                        _icj.UOM = item.BaseUOM;
                        _icj.RowID = item.RowID.ToString();
                        _icj.CostCenter = thisReg.ChargeTo != null ? thisReg.ChargeTo : null;
                        _icj.RequestedBy = thisReg.Requestor != null ? thisReg.Requestor : null;
                        _icj.Save();
                    }
                    if (item.PODetailID != null)
                    {
                        item.PODetailID.skipAuto = true;
                        item.PODetailID.Received += Math.Abs(item.Quantity);
                        decimal dt = item.PODetailID.PurchaseInfo.PurchaseOrderDetails.Sum(o => o.RemainingQty);
                        if (dt == 0)
                        {
                            item.PODetailID.PurchaseInfo.Status = PurchaseOrderStatusEnum.Received;
                        }
                        else
                        {
                            item.PODetailID.PurchaseInfo.Status = PurchaseOrderStatusEnum.PartiallyReceived;
                        }

                        // Update Requisition Worksheet
                        //if (!string.IsNullOrEmpty(item.PODetailID.RequestID.ToString()))
                        //{
                        //    RequisitionWorksheet _rw = session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = '" + item.PODetailID.RequestID.ToString() + "'"));
                        //    _rw.Served = true;
                        //    _rw.ServedQTY = Math.Abs(item.Quantity);
                        //    _rw.RequisitionInfo.Status = RequisitionStatusEnum.
                        //    Served;
                        //    _rw.Save();
                        //}
                        //if (!(item.PODetailID.Received >= item.PODetailID.Quantity)) { partial++; }
                        // Disabled Start 010816 RL: Since Online PO approval is implemented ICJ PO registrations are bypassed
                        //_icj = session.FindObject<InventoryControlJournal>(CriteriaOperator.Parse("[RowID] = '" + item.PODetailID.RowID.ToString() + "'"));
                        //_icj.InQTY -= Math.Abs(item.BaseQTY);
                        //if (_icj.InQTY >= 0)
                        //{
                        //    _icj.Save();
                        //}
                        // Disabled End 010816 RL:
                    }
                    //// Update IH Parts Sales Registry
                    //IHPartsSalesRegistry registry = new IHPartsSalesRegistry(session);
                    //registry.Date = thisReg.EntryDate;
                    //registry.Customer = defaultCustomer;
                    //registry.Item = item.ItemNo;
                    //registry.Quantity = Math.Abs(item.BaseQTY);
                    //registry.UnitCost = item.ItemNo.Cost;
                    //registry.UnitPrice = item.Price;
                    //registry.Save();
                    //item.SalesRegistryRowID = registry.RowID;

                    amount += item.Total;
                    int[] inds = null;
                    // Credit Sales Income
                    //if (item.ItemNo.IncomeAccount == null)
                    //{
                    //    throw new
                    //        ApplicationException(
                    //        "Must specify an income account in " + item.ItemNo.No +
                    //        " Item Card");
                    //}
                    //inds = accounts.Find("Account", item.ItemNo.IncomeAccount);
                    //if (inds != null && inds.Length > 0)
                    //{
                    //    tmpAccount = accounts[inds[0]];
                    //    tmpAccount.CreditAmount += Math.Abs(item.Total / (1 + defaultCustomer.TaxCode.Rate / 100));
                    //}
                    //else
                    //{
                    //    tmpAccount = new TempAccount();
                    //    tmpAccount.Account = item.ItemNo.IncomeAccount;
                    //    tmpAccount.CreditAmount += Math.Abs(item.Total / (1 + defaultCustomer.TaxCode.Rate / 100));
                    //    accounts.Add(tmpAccount);
                    //}
                    //if (true)
                    //{
                    //    // Credit Output Tax
                    //    if (defaultCustomer.TaxCode.Account == null)
                    //    {
                    //        throw new
                    //            ApplicationException(
                    //            "Must specify a Tax account in " + defaultCustomer.TaxCode.Code +
                    //            " Sales Tax Code");
                    //    }
                    //    inds = null;
                    //    inds = accounts.Find("Account", defaultCustomer.TaxCode.Account);
                    //    if (inds != null && inds.Length > 0)
                    //    {
                    //        tmpAccount = accounts[inds[0]];
                    //        tmpAccount.CreditAmount += Math.Abs(item.Total - (item.Total / (1 + (defaultCustomer.TaxCode.Rate / 100))));
                    //    }
                    //    else
                    //    {
                    //        tmpAccount = new TempAccount();
                    //        tmpAccount.Account = defaultCustomer.TaxCode.Account;
                    //        tmpAccount.CreditAmount += Math.Abs(item.Total - (item.Total / (1 + (defaultCustomer.TaxCode.Rate / 100))));
                    //        accounts.Add(tmpAccount);
                    //    }
                    //    amount += Math.Abs(item.Total) * (defaultCustomer.TaxCode.Rate / 100);
                    //    // Credit Tax Expense
                    //    //if (item.Tax.ExpenseAccount == null) {throw new 
                    //    //    ApplicationException(
                    //    //    "Must specify an Expense account in " + item.Tax.
                    //    //    Code + " Sales Tax Code");}
                    //    //inds = null;
                    //    //inds = accounts.Find("Account", item.Tax.ExpenseAccount)
                    //    //;
                    //    //if (inds != null && inds.Length > 0) {
                    //    //    tmpAccount = accounts[inds[0]];
                    //    //    tmpAccount.CreditAmount += Math.Abs(item.Total) * (
                    //    //    item.Tax.Rate / 100);
                    //    //} else {
                    //    //    tmpAccount = new TempAccount();
                    //    //    tmpAccount.Account = item.Tax.ExpenseAccount;
                    //    //    tmpAccount.CreditAmount += Math.Abs(item.Total) * (
                    //    //    item.Tax.Rate / 100);
                    //    //    accounts.Add(tmpAccount);
                    //    //}
                    //}
                    // Credit Inventory Asset
                    if (item.ItemNo.InventoryAccount == null)
                    {
                        throw new
                            ApplicationException(
                            "Must specify an Inventory account in " + item.ItemNo.No
                            + " Item Card");
                    }
                    inds = null;
                    inds = accounts.Find("Account", item.ItemNo.InventoryAccount
                    );
                    if (inds != null && inds.Length > 0)
                    {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.CreditAmount += (item.Cost * item.
                        Factor) * item.Quantity;
                    }
                    else
                    {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.InventoryAccount;
                        tmpAccount.CreditAmount += (item.Cost * item.
                        Factor) * item.Quantity;
                        accounts.Add(tmpAccount);
                    }
                    // Debit Inventory
                    //if (item.ItemNo.InventoryAccount == null)
                    //{
                    //    throw new
                    //        ApplicationException("Must specify a Inventory account in " +
                    //        item.ItemNo.No + " Item Card");
                    //}
                    //inds = null;
                    //inds = accounts.Find("Account", item.ItemNo.InventoryAccount);
                    //if (inds != null && inds.Length > 0)
                    //{
                    //    tmpAccount = accounts[inds[0]];
                    //    tmpAccount.DebitAmount += item.Total;
                    //}
                    //else
                    //{
                    //    tmpAccount = new TempAccount();
                    //    tmpAccount.Account = item.ItemNo.InventoryAccount;
                    //    tmpAccount.DebitAmount += item.Total;
                    //    accounts.Add(tmpAccount);
                    //}
                    //if (item.ItemNo.RequireSerial)
                    //{
                    //    if (item.InvoiceDetailTrackingLines.Count != Math.Abs(
                    //    item.BaseQTY))
                    //    {
                    //        throw new ApplicationException(
                    //            "An item requires a serial no. Please specify serial nos according to quantity"
                    //            );
                    //    }
                    //    foreach (InvoiceDetailTrackingLine iTrack in item.
                    //    InvoiceDetailTrackingLines)
                    //    {
                    //        ItemTrackingEntry _ite = null;
                    //        // Find serial no then mark it as Sold (make sure that is available)
                    //        _ite = (ItemTrackingEntry)session.FindObject(typeof(
                    //        ItemTrackingEntry), CriteriaOperator.Parse(
                    //        "[ItemNo.No] = '" + item.ItemNo.No +
                    //        "' And [SerialNo] = '" + iTrack.SerialNo +
                    //        "' And [Warehouse.Code] = '" + item.Warehouse.Code +
                    //        "' And [Status] = 'Available'"));
                    //        if (_ite != null)
                    //        {
                    //            _ite.Status = SerialNoStatusEnum.Sold;
                    //            _ite.DestPerson = item.InvoiceInfo.Customer;
                    //            _ite.DestSource = item.InvoiceInfo.SourceType;
                    //            _ite.DestSourceNo = item.InvoiceInfo;
                    //            _ite.DestRowID = item.RowID.ToString();
                    //            _ite.Save();
                    //        }
                    //        else
                    //        {
                    //            throw new ApplicationException(
                    //            "Cannot find the serial #" + iTrack.SerialNo +
                    //            " in the list of available serial nos");
                    //        }
                    //    }
                    //}
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    item.Save();
                    //_message = string.Format("Invoicing entry {0} succesfull.",
                    //thisInvoice.InvoiceDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    //_BgWorker.ReportProgress(1, _message);
                    //index++;
                } //..

            }
            finally
            {
                if (index == thisReg.FuelPumpRegisterDetails.Count)
                {
                    //// if Cash
                    //if (false)
                    //{
                    //    //if (thisInvoice.CompanyInfo.UndepositedCollectionAcct == null)
                    //    //{
                    //    //    throw
                    //    //        new ApplicationException(
                    //    //        "Must specify a default undeposited collection account in the company information card"
                    //    //        );
                    //    //}
                    //    //// Create Accounts Receivable
                    //    //GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    //    //GenJournalDetail>(session);
                    //    //_gjde.GenJournalID = thisInvoice;
                    //    //_gjde.GenJournalID.Approved = true;
                    //    //_gjde.Account = thisInvoice.CompanyInfo.UndepositedCollectionAcct;
                    //    //_gjde.DebitAmount = Math.Abs(thisInvoice.GrossTotal.Value);
                    //    //_gjde.Description = "Invoice Entry for Customer " +
                    //    //thisInvoice.ReferenceNo;
                    //    //_gjde.SubAccountNo = thisInvoice.Customer;
                    //    //_gjde.SubAccountType = thisInvoice.Customer.ContactType;
                    //    //_gjde.Approved = true;
                    //    //debitAmount = debitAmount + _gjde.DebitAmount;
                    //    //_gjde.Save();

                    //}
                    //else
                    //{
                    //    // Create Accounts Receivable
                    //    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    //    GenJournalDetail>(session);
                    //    _gjde.GenJournalID = thisReg;
                    //    _gjde.GenJournalID.Approved = true;
                    //    _gjde.Account = defaultCustomer.Account;
                    //    _gjde.DebitAmount = Math.Abs(thisReg.TotalParts.Value);
                    //    _gjde.Description = "WO Invoice Entry for Customer " +
                    //    thisReg.ReferenceNo;
                    //    _gjde.SubAccountNo = defaultCustomer;
                    //    _gjde.SubAccountType = defaultCustomer.ContactType;
                    //    _gjde.Approved = true;
                    //    debitAmount = debitAmount + _gjde.DebitAmount;
                    //    _gjde.Save();

                    //}
                    // Create Debit Fuel Expense Account Entry
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisReg;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = defaultCustomer.Account;
                    _gjde.DebitAmount = Math.Abs(thisReg.Total.Value);
                    _gjde.Description = "Fuel Withdrawal Confirmation " +
                    thisReg.FuelWithdrawalRef;
                    _gjde.SubAccountNo = defaultCustomer;
                    _gjde.SubAccountType = defaultCustomer.ContactType;
                    _gjde.Approved = true;
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    _gjde.Save();
                    switch (thisReg.TransactionType)
                    {
                        case FuelPumpRegisterTypeEnum.ChargeToCompany:
                            break;
                        case FuelPumpRegisterTypeEnum.ChargeToEmployee:
                            // Autoamtically create employees chargeslip
                            EmployeeChargeSlip ecs = ReflectionHelper.CreateObject<EmployeeChargeSlip>(session);
                            ecs.EntryDate = thisReg.EntryDate;
                            StringBuilder sb = new StringBuilder();
                            sb.Append("FUEL REQUEST\r\n");
                            sb.AppendFormat("{0}\r\n", thisReg.Unit.DisplayName);
                            sb.AppendFormat("{0}/{1}/{2}\r\n", thisReg.PurchaseOrderRef.SourceNo, thisReg.FuelRequestRef, thisReg.FuelWithdrawalRef);
                            ecs.Memo = sb.ToString();
                            ecs.Employee = thisReg.Requestor;
                            switch (thisReg.Unit.FixedAssetClass)
	                            {
                                    case FixedAssetClassEnum.LandAndBuilding:
                                        break;
                                    case FixedAssetClassEnum.Truck:
                                        ecs.TypeOfCharge = TypeOfEmployeeChargesEnum.FuelChargeTractor;
                                        ecs.DeductionCode = session.FindObject<OtherDeduction>(CriteriaOperator.Parse("[Code] = '10-003'"));
                                        ecs.AdvancesAccount = session.FindObject<Account>(CriteriaOperator.Parse("[No] = '101912'"));
                                        break;
                                    case FixedAssetClassEnum.Trailer:
                                        break;
                                    case FixedAssetClassEnum.GeneratorSet:
                                        ecs.TypeOfCharge = TypeOfEmployeeChargesEnum.FuelChargeGenset;
                                        ecs.DeductionCode = session.FindObject<OtherDeduction>(CriteriaOperator.Parse("[Code] = '10-002'"));
                                        ecs.AdvancesAccount = session.FindObject<Account>(CriteriaOperator.Parse("[No] = '101911'"));
                                        break;
                                    case FixedAssetClassEnum.OtherVehicle:
                                        ecs.TypeOfCharge = TypeOfEmployeeChargesEnum.FuelCharge;
                                        ecs.DeductionCode = session.FindObject<OtherDeduction>(CriteriaOperator.Parse("[Code] = '10-001'"));
                                        ecs.AdvancesAccount = session.FindObject<Account>(CriteriaOperator.Parse("[No] = '101905'"));
                                        break;
                                    case FixedAssetClassEnum.Other:
                                        break;
                                    default:
                                        break;
	                            }
                            ecs.ReferenceNo = string.Format("{0}/{1}",thisReg.ChargeSlipNo, thisReg.SourceNo);
                            foreach (var item in thisReg.FuelPumpRegisterDetails)
                            {
                                EmployeeChargeSlipItemDetail ecsitm = ReflectionHelper.CreateObject<EmployeeChargeSlipItemDetail>(session);
                                ecsitm.GenJournalID = ecs;
                                ecsitm.ItemNo = item.ItemNo;
                                ecsitm.CostCenter = session.FindObject<CostCenter>(CriteriaOperator.Parse("[Code] = ?", thisReg.Requestor.No))??null;
                                ecsitm.Quantity = item.Quantity;
                                ecsitm.UOM = item.UOM;
                                ecsitm.Cost = item.Cost;
                                ecsitm.Price = item.Cost;
                                ecsitm.Save();
                            }
                            ecs.Save();
                            thisReg.ChargeSlipRef = ecs;
                            break;
                        default:
                            break;
                    }
                     
                    foreach (TempAccount item in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisReg;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Fuel Withdrawal Confirmation " +
                        thisReg.FuelWithdrawalRef;
                        _gjd.SubAccountNo = defaultCustomer;
                        _gjd.SubAccountType = defaultCustomer.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }
                    thisReg.Status = FuelPumpRegisterStatusEnum.Charged;
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                        ApplicationException("Accounting entries not balance");
                    }
                    // Insert AR Registry code here
                    //ARRegistry _arreg = ReflectionHelper.CreateObject<ARRegistry>(session);
                    //_arreg.GenJournalID = thisReg;
                    //_arreg.Date = thisReg.EntryDate;
                    //_arreg.Customer = defaultCustomer;
                    //_arreg.SourceDesc = "Work Order";
                    //_arreg.SourceNo = thisReg.SourceNo;
                    //_arreg.DocNo = thisReg.ReferenceNo;
                    //_arreg.Amount = thisReg.TotalParts.Value;
                    //_arreg.Save();

                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void ConfirmAction_Executed(object sender, ActionBaseEventArgs
        e)
        {
            //throw new NotImplementedException();
        }
        private UnitOfWork CreateUpdatingSession()
        {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session)
        {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session)
        {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                    , new SessionEventArgs(session));
            }
        }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session)
        {
            if
                (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                    SessionEventArgs(session));
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
        RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Invoicing entries operation has been cancelled", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully Invoiced");
                    ObjectSpace.ReloadObject(fuelPumpReg);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        private void UpdateActionState(bool inProgress)
        {
            confirmAction.
                Enabled.SetItemValue("Invoicing entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
