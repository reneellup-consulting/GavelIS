using System;
using System.Linq;
using System.Windows.Forms;
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
    public partial class InvoiceInvoiceAction : ViewController {
        private Invoice invoice;
        private SimpleAction invoiceInvoiceAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public InvoiceInvoiceAction() {
            this.TargetObjectType = typeof(Invoice);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.Invoice", this.GetType().Name);
            this.invoiceInvoiceAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.invoiceInvoiceAction.Caption = "Invoice";
            this.invoiceInvoiceAction.Execute += new 
            SimpleActionExecuteEventHandler(InvoiceInvoiceAction_Execute);
            this.invoiceInvoiceAction.Executed += new EventHandler<
            ActionBaseEventArgs>(InvoiceInvoiceAction_Executed);
            this.invoiceInvoiceAction.ConfirmationMessage = 
            "Do you really want to invoice these entries?";
            UpdateActionState(false);
        }
        private void InvoiceInvoiceAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            //invoice = ((DevExpress.ExpressApp.DetailView)this.View).
            //CurrentObject as Invoice;
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.DetailView))
            {
                invoice = ((DevExpress.ExpressApp.DetailView)this.View).
                CurrentObject as Invoice;
            }
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.ListView))
            {
                invoice = this.View.CurrentObject as Invoice;
            }
            ObjectSpace.CommitChanges();
            if (invoice.InvoiceDetails.Count == 0) {
                XtraMessageBox.Show("There are no entries to invoice", 
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            if (invoice.CompanyInfo.AllowInsufficientCurrQty != true)
            {
                foreach (var item in invoice.InvoiceDetails)
                {
                    if (item.Quantity > item.CurrentQtyBase)
                    {
                        throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + item.ItemNo.No + "!");
                    }
                }
            }

            foreach (var item in invoice.InvoiceDetails)
            {
                if (item.Quantity > item.CurrentQtyBase)
                {
                    DialogResult dres = XtraMessageBox.Show("Warehouse is not sufficient to fullfil item " + item.ItemNo.No + ". Do you want to continue?", "Insufficient Qty",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dres == DialogResult.Cancel)
                    {
                        return;
                    }
                }

            }

            var count = invoice.InvoiceDetails.Count;
            _FrmProgress = new ProgressForm("Invoice entries...", count, 
            "Invoicing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(invoice);
            _FrmProgress.ShowDialog();
        }
        private void InvoiceInvoiceAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(invoice);
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
            Invoice _invoice = (Invoice)e.Argument;
            if (_invoice.InvoiceType == InvoiceTypeEnum.Cash)
            {
                decimal tPayAmount = 0;
                foreach (PaymentsApplied tpay in _invoice.PaymentsApplied)
                {
                    if (tpay.PaymentTenderedType == PaymentTenderedTypeEnum.Memo && tpay.Memo != null)
                    {
                        var memo = session.GetObjectByKey<CreditMemo>(tpay.Memo.Oid);
                        if (memo.OpenAmount == 0m)
                        {
                            memo.OpenAmount = tpay.Memo.GrossTotal.Value - tpay.Amount;
                        }
                        else
                        {
                            memo.OpenAmount -= tpay.Amount;
                        }
                        if (memo.OpenAmount < 0)
                        {
                            throw new UserFriendlyException("Cannot exceed memo open amount");
                        }
                        if (memo.OpenAmount == 0)
                        {
                            memo.Status =
                                CreditMemoStatusEnum.Applied;
                        }
                        else
                        {
                            memo.Status = CreditMemoStatusEnum.
                            PartiallyApplied;
                        }
                        memo.Save();
                    }
                    tPayAmount += tpay.Amount;
                }
                if (Math.Round(_invoice.GrossTotal.Value, 2) != Math.Round(tPayAmount, 2))
                {
                    throw new UserFriendlyException("Cash payment is not equal to gross total");
                }
            }
            //if (_receipt.Oid==-1)
            //{
            //    throw new
            //            ApplicationException(
            //            "Please save this transaction first before receiving"
            //            );
            //}
            Invoice thisInvoice = session.GetObjectByKey<Invoice>(_invoice.Oid);
            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            int partial = 0;
            try {
                // Validate Customer Accounts Receivable
                if (thisInvoice.Customer == null) {throw new 
                    ApplicationException("Must specify a customer");} else {
                    if (thisInvoice.Customer.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Receivable account must be specified in the chosen customer card"
                        );}
                }
                foreach (InvoiceDetail item in thisInvoice.InvoiceDetails) {
                    _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                    _icj.GenJournalID = thisInvoice;
                    //_icj.OutQty = Math.Abs(item.BaseQTY);
                    _icj.Warehouse = item.Warehouse;
                    _icj.ItemNo = item.ItemNo;
                    if (item.ItemNo.UOMRelations.Count > 0)
                    {
                        var dUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.UOM).FirstOrDefault();
                        var dBaseUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.ItemNo.BaseUOM2).FirstOrDefault();
                        var dStockUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.ItemNo.StockUOM).FirstOrDefault();
                        UOMRelation UOMr = session.GetObjectByKey<UOMRelation>(dUOM.Oid);
                        UOMRelation UOMSr = session.GetObjectByKey<UOMRelation>(dStockUOM.Oid);
                        if (dStockUOM.UOM == dUOM.UOM)
                        {
                            _icj.OutQty = item.Quantity;
                        }
                        else
                        {
                            _icj.OutQty = item.BaseQTY / dStockUOM.Factor;
                            //UOMr.CostPerBaseUom = //(item.Quantity * item.Cost) / _icj.InQTY;
                        }
                        UOMr.PricePerBaseUom = item.Price;
                        UOMSr.PricePerBaseUom = (item.Quantity * item.Price) / _icj.OutQty;
                        _icj.UOM = item.ItemNo.StockUOM;
                        _icj.Price = UOMSr.PricePerBaseUom;
                    }
                    else
                    {
                        _icj.OutQty = item.Quantity;
                        _icj.UOM = item.UOM;
                        _icj.Price = item.Price;
                        item.ItemNo.SalesPrice = item.Cost;
                    }
                    
                    _icj.RowID = item.RowID.ToString();
                    _icj.RequisitionNo = item.RequisitionNo != null ? item.RequisitionNo : null;
                    _icj.CostCenter = item.CostCenter != null ? item.CostCenter : null;
                    _icj.RequestedBy = item.RequestedBy != null ? item.RequestedBy : null;
                    _icj.Save();
                    if (item.SalesOrderDetailID != null) {
                        item.SalesOrderDetailID.Invoiced += Math.Abs(item.
                        Quantity);
                        //if (!(item.SalesOrderDetailID.Invoiced >= item.
                        //SalesOrderDetailID.Quantity)) {partial++;}
                    }
                    // Update IH Parts Sales Registry
                    IHPartsSalesRegistry registry = new IHPartsSalesRegistry(session);
                    registry.Date = thisInvoice.EntryDate;
                    registry.Customer = thisInvoice.Customer;
                    registry.Item = item.ItemNo;
                    registry.Quantity = Math.Abs(item.BaseQTY);
                    registry.UnitCost = item.ItemNo.Cost;
                    registry.UnitPrice = item.Price;
                    registry.Save();
                    item.SalesRegistryRowID = registry.RowID;

                    amount += item.Total;
                    int[] inds = null;
                    // Credit Sales Income
                    if (item.ItemNo.IncomeAccount == null) {throw new 
                        ApplicationException(
                        "Must specify an income account in " + item.ItemNo.No + 
                        " Item Card");}
                    inds = accounts.Find("Account", item.ItemNo.IncomeAccount);
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.CreditAmount += Math.Round(Math.Abs(item.Total / (1 + item.Tax.Rate / 100))+item.LineDiscount, 2);
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.IncomeAccount;
                        tmpAccount.CreditAmount += Math.Round(Math.Abs(item.Total / (1 + item.Tax.Rate / 100)) + item.LineDiscount, 2);
                        accounts.Add(tmpAccount);
                    }
                    // Sales Discount
                    if (item.LineDiscount > 0) {
                        if (item.CompanyInfo.SalesDiscountIHAcct == null) {throw 
                            new ApplicationException(
                            "Must specify a default sales discount for IH accounts in the company information card"
                            );}
                        inds = accounts.Find("Account", item.CompanyInfo.
                        SalesDiscountIHAcct);
                        if (inds != null && inds.Length > 0) {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += Math.Round(Math.Abs(item.
                            LineDiscount), 2);
                        } else {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = item.CompanyInfo.
                            SalesDiscountIHAcct;
                            tmpAccount.DebitAmount += Math.Round(Math.Abs(item.
                            LineDiscount), 2);
                            accounts.Add(tmpAccount);
                        }
                    }
                    if (item.Tax.Taxable) {
                        // Credit Output Tax
                        if (item.Tax.Account == null) {throw new 
                            ApplicationException(
                            "Must specify a Tax account in " + item.Tax.Code + 
                            " Sales Tax Code");}
                        inds = null;
                        inds = accounts.Find("Account", item.Tax.Account);
                        if (inds != null && inds.Length > 0) {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.CreditAmount += Math.Round(Math.Abs(item.Total - (item.Total / (1 + (item.Tax.Rate / 100)))), 2);
                        } else {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = item.Tax.Account;
                            tmpAccount.CreditAmount += Math.Round(Math.Abs(item.Total - (item.Total / (1 + (item.Tax.Rate / 100)))), 2);
                            accounts.Add(tmpAccount);
                        }
                        amount += Math.Round(Math.Abs(item.Total) * (item.Tax.Rate / 100), 2);
                        // Credit Tax Expense
                        //if (item.Tax.ExpenseAccount == null) {throw new 
                        //    ApplicationException(
                        //    "Must specify an Expense account in " + item.Tax.
                        //    Code + " Sales Tax Code");}
                        //inds = null;
                        //inds = accounts.Find("Account", item.Tax.ExpenseAccount)
                        //;
                        //if (inds != null && inds.Length > 0) {
                        //    tmpAccount = accounts[inds[0]];
                        //    tmpAccount.CreditAmount += Math.Abs(item.Total) * (
                        //    item.Tax.Rate / 100);
                        //} else {
                        //    tmpAccount = new TempAccount();
                        //    tmpAccount.Account = item.Tax.ExpenseAccount;
                        //    tmpAccount.CreditAmount += Math.Abs(item.Total) * (
                        //    item.Tax.Rate / 100);
                        //    accounts.Add(tmpAccount);
                        //}
                    }
                    // Credit Inventory Asset
                    if (item.ItemNo.InventoryAccount == null) {throw new 
                        ApplicationException(
                        "Must specify an Inventory account in " + item.ItemNo.No 
                        + " Item Card");}
                    inds = null;
                    inds = accounts.Find("Account", item.ItemNo.InventoryAccount
                    );
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.CreditAmount += Math.Round((item.ItemNo.Cost * item.
                        Factor) * item.Quantity, 2);
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.InventoryAccount;
                        tmpAccount.CreditAmount += Math.Round((item.ItemNo.Cost * item.
                        Factor) * item.Quantity, 2);
                        accounts.Add(tmpAccount);
                    }
                    // Debit Cost of Goods Sold
                    if (item.ItemNo.COGSAccount == null) {throw new 
                        ApplicationException("Must specify a COGS account in " + 
                        item.ItemNo.No + " Item Card");}
                    inds = null;
                    inds = accounts.Find("Account", item.ItemNo.COGSAccount);
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += Math.Round((item.ItemNo.Cost * item.
                        Factor) * item.Quantity, 2);
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.COGSAccount;
                        tmpAccount.DebitAmount += Math.Round((item.ItemNo.Cost * item.
                        Factor) * item.Quantity, 2);
                        accounts.Add(tmpAccount);
                    }
                    if (item.ItemNo.RequireSerial) {
                        if (item.InvoiceDetailTrackingLines.Count != Math.Abs(
                        item.BaseQTY)) {throw new ApplicationException(
                            "An item requires a serial no. Please specify serial nos according to quantity"
                            );}
                        foreach (InvoiceDetailTrackingLine iTrack in item.
                        InvoiceDetailTrackingLines) {
                            ItemTrackingEntry _ite = null;
                            // Find serial no then mark it as Sold (make sure that is available)
                            _ite = (ItemTrackingEntry)session.FindObject(typeof(
                            ItemTrackingEntry), CriteriaOperator.Parse(
                            "[ItemNo.No] = '" + item.ItemNo.No + 
                            "' And [SerialNo] = '" + iTrack.SerialNo + 
                            "' And [Warehouse.Code] = '" + item.Warehouse.Code + 
                            "' And [Status] = 'Available'"));
                            if (_ite != null) {
                                _ite.Status = SerialNoStatusEnum.Sold;
                                _ite.DestPerson = item.InvoiceInfo.Customer;
                                _ite.DestSource = item.InvoiceInfo.SourceType;
                                _ite.DestSourceNo = item.InvoiceInfo;
                                _ite.DestRowID = item.RowID.ToString();
                                _ite.Save();

                                // If scrapped tire
                                if (item.ItemNo.GetType() == typeof(TireItem))
                                {
                                    TireItem tri = item.ItemNo as TireItem;
                                    if (tri.TireItemClass== TireItemClassEnum.ScrappedTire)
                                    {
                                        XPCollection<TireForSaleDet> tfsc= new XPCollection<TireForSaleDet>(session);
                                        TireForSaleDet tfsd = tfsc.Where(o => o.SerialBrandingNo == _ite.SerialNo).LastOrDefault();
                                        if (tfsd != null)
                                        {
                                            TireServiceDetail2 ctsd2 = tfsd.TireNo.TireServiceDetails2.LastOrDefault();
                                            if (ctsd2 != null)
                                            {
                                                CloneHelper cnh = new CloneHelper(session);
                                                TireServiceDetail2 tsd2 = cnh.Clone<TireServiceDetail2>(ctsd2,true);
                                                tsd2.EntryDate = item.GenJournalID.EntryDate;
                                                tsd2.ActivityDate = item.GenJournalID.EntryDate;
                                                tsd2.TfsId = ctsd2.TfsId ?? null;
                                                tsd2.Reason = session.FindObject<TireDettachReason>(
                                                new BinaryOperator("Code", "SOLD"));
                                                tsd2.ReferenceNo = item.GenJournalID.SourceNo;
                                                tsd2.Remarks = string.Format("Sold to {0} on {1}", (item.GenJournalID as Invoice).Customer.Name, item.GenJournalID.EntryDate.ToShortDateString());
                                                tfsd.TireNo.TireServiceDetails2.BaseAdd(tsd2);
                                                tsd2.ActivityType = TireActivityTypeEnum.Disposed;
                                            }
                                        }
                                    }
                                }

                            } else {
                                throw new ApplicationException(
                                "Cannot find the serial #" + iTrack.SerialNo + 
                                " in the list of available serial nos");
                            }
                        }
                    }
                    if (_BgWorker.CancellationPending) {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    item.Save();
                    _message = string.Format("Invoicing entry {0} succesfull.", 
                    thisInvoice.InvoiceDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                } //..
            } finally {
                if (index == thisInvoice.InvoiceDetails.Count) {
                    // if Cash
                    if (thisInvoice.InvoiceType==InvoiceTypeEnum.Cash)
                    {
                        if (thisInvoice.RequireTenderPayment && thisInvoice.InvoiceType == InvoiceTypeEnum.Cash && Math.Round(thisInvoice.GrossTotal.Value, 2) != Math.Round(thisInvoice.CashPayment.Value, 2))
                        {
                            throw new UserFriendlyException("Cash/Check Payments must be equal to Gross Total.");
                        }
                        if (thisInvoice.CompanyInfo.UndepositedCollectionAcct == null)
                        {
                            throw
                                new ApplicationException(
                                "Must specify a default undeposited collection account in the company information card"
                                );
                        }
                        // Create Accounts Receivable
                        GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde.GenJournalID = thisInvoice;
                        _gjde.GenJournalID.Approved = true;
                        _gjde.Account = thisInvoice.CompanyInfo.UndepositedCollectionAcct;
                        _gjde.DebitAmount = Math.Round(Math.Abs(thisInvoice.GrossTotal.Value), 2);
                        _gjde.Description = "Invoice Entry for Customer " +
                        thisInvoice.ReferenceNo;
                        _gjde.SubAccountNo = thisInvoice.Customer;
                        _gjde.SubAccountType = thisInvoice.Customer.ContactType;
                        _gjde.Approved = true;
                        debitAmount = Math.Round(debitAmount + _gjde.DebitAmount, 2);
                        _gjde.Save();
                        thisInvoice.Status = InvoiceStatusEnum.Paid;

                    }
                    else
                    {
                        // Create Accounts Receivable
                        GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde.GenJournalID = thisInvoice;
                        _gjde.GenJournalID.Approved = true;
                        _gjde.Account = thisInvoice.Customer.Account;
                        _gjde.DebitAmount = Math.Round(Math.Abs(thisInvoice.GrossTotal.Value), 2);
                        _gjde.Description = "Invoice Entry for Customer " +
                        thisInvoice.ReferenceNo;
                        _gjde.SubAccountNo = thisInvoice.Customer;
                        _gjde.SubAccountType = thisInvoice.Customer.ContactType;
                        _gjde.Approved = true;
                        debitAmount = Math.Round(debitAmount + _gjde.DebitAmount, 2);
                        _gjde.Save();
                        thisInvoice.Status = InvoiceStatusEnum.Invoiced;

                    }
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts) {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisInvoice;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = Math.Round(item.DebitAmount, 2);
                        _gjd.CreditAmount = Math.Round(item.CreditAmount, 2);
                        _gjd.Description = "Invoice Entry for Customer " + 
                        thisInvoice.ReferenceNo;
                        _gjd.SubAccountNo = thisInvoice.Customer;
                        _gjd.SubAccountType = thisInvoice.Customer.ContactType;
                        _gjd.Approved = true;
                        debitAmount = Math.Round(debitAmount + _gjd.DebitAmount, 2);
                        creditAmount = Math.Round(creditAmount + _gjd.CreditAmount, 2);
                        _gjd.Save();
                    }
                    if (thisInvoice.SONumber != null) {
                        foreach (SalesOrderDetail item in thisInvoice.SONumber.
                        SalesOrderDetails) {
                            InvoiceDetail invd = thisInvoice.InvoiceDetails.Where(o => o.SalesOrderDetailID == item).FirstOrDefault();
                            if (invd != null && !(item.Invoiced >= invd.Quantity)) {partial++;}
                            //if (item.RemainingQty != 0)
                            //{
                                
                            //}
                            //if (!(item.Returned == item.Quantity))
                            //{
                            //    partial++;
                            //}
                            
                        }
                        if (partial > 0 && thisInvoice.SONumber != null) {
                            thisInvoice.SONumber.Status = SalesOrderStatusEnum.
                            PartiallyInvoiced;} else {
                            if (partial == 0 && thisInvoice.SONumber != null) {
                                thisInvoice.SONumber.Status = 
                                SalesOrderStatusEnum.Invoiced;}
                        }
                    }
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new 
                        ApplicationException("Accounting entries not balance");}

                    // Insert AR Registry code here
                    if (thisInvoice.InvoiceType!=InvoiceTypeEnum.Cash){
                        ARRegistry _arreg = ReflectionHelper.CreateObject<ARRegistry>(session);
                        _arreg.GenJournalID = thisInvoice;
                        _arreg.Date = thisInvoice.EntryDate;
                        _arreg.Customer = thisInvoice.Customer;
                        _arreg.SourceDesc = "IH Parts";
                        _arreg.SourceNo = thisInvoice.SourceNo;
                        _arreg.DocNo = thisInvoice.ReferenceNo;
                        _arreg.Amount = Math.Round(thisInvoice.GrossTotal.Value, 2);
                        _arreg.Save();

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
                "Invoicing entries operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully Invoiced");
                    ObjectSpace.ReloadObject(invoice);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { invoiceInvoiceAction.
            Enabled.SetItemValue("Invoicing entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
