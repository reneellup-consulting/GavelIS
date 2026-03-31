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
    public partial class InvoiceWorkOrderAction : ViewController
    {
        private WorkOrder workOrder;
        private SimpleAction invoiceWorkOrderAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public InvoiceWorkOrderAction()
        {
            this.TargetObjectType = typeof(WorkOrder);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Invoice", this.GetType().Name);
            this.invoiceWorkOrderAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.invoiceWorkOrderAction.Caption = "Invoice";
            this.invoiceWorkOrderAction.Execute += new
            SimpleActionExecuteEventHandler(InvoiceWorkOrderAction_Execute);
            this.invoiceWorkOrderAction.Executed += new EventHandler<
            ActionBaseEventArgs>(InvoiceWorkOrderAction_Executed);
            this.invoiceWorkOrderAction.ConfirmationMessage =
            "Do you really want to invoice these entries?";
            UpdateActionState(false);
        }
        private void InvoiceWorkOrderAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            workOrder = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as WorkOrder;
            ObjectSpace.CommitChanges();
            if (workOrder.WorkOrderItemDetails.Count == 0)
            {
                XtraMessageBox.Show("There are no entries to invoice",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = workOrder.WorkOrderItemDetails.Count;
            _FrmProgress = new ProgressForm("Invoice entries...", count,
            "Invoicing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(workOrder);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            WorkOrder _invoice = (WorkOrder)e.Argument;
            WorkOrder thisInvoice = session.GetObjectByKey<WorkOrder>(_invoice.Oid);
            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            //int partial = 0;
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

                foreach (WorkOrderItemDetail item in thisInvoice.WorkOrderItemDetails)
                {
                    item.skipAuto = true;

                    index++;
                    _message = string.Format("Invoicing entry {0} succesfull.",
                    thisInvoice.InvoiceDetails.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    _icj = session.FindObject<InventoryControlJournal>(
                    new BinaryOperator("RowID", item.RowID.ToString()));
                    if (_icj != null)
                    {
                        _icj.GenJournalID = thisInvoice;
                        _icj.DateIssued = item.DateIssued;
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
                            _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(item.ItemNo.StockUOM.Oid);
                            _icj.Price = UOMSr.PricePerBaseUom;
                        }
                        else
                        {
                            _icj.OutQty = item.Quantity;
                            _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(item.UOM.Oid);
                            _icj.Price = item.Price;
                            item.ItemNo.SalesPrice = item.Price;
                        }
                        //_icj.Cost = item.ItemNo.Cost;
                        //_icj.Price = item.Price;
                        //_icj.UOM = item.BaseUOM;
                        _icj.RowID = item.RowID.ToString();
                        _icj.CostCenter = item.CostCenter != null ? item.CostCenter : null;
                        _icj.RequestedBy = item.RequestedBy != null ? item.RequestedBy : null;
                        _icj.Save();
                    }
                    else
                    {
                        _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                        //_icj.GenJournalID = thisInvoice;
                        //_icj.DateIssued = item.DateIssued;
                        //_icj.OutQty = Math.Abs(item.BaseQTY);
                        //_icj.Warehouse = item.Warehouse;
                        //_icj.ItemNo = item.ItemNo;
                        //_icj.Cost = item.ItemNo.Cost;
                        //_icj.Price = item.Price;
                        //_icj.UOM = item.BaseUOM;
                        //_icj.RowID = item.RowID.ToString();
                        //_icj.CostCenter = item.CostCenter != null ? item.CostCenter : null;
                        //_icj.RequestedBy = item.RequestedBy != null ? item.RequestedBy : null;
                        //_icj.Save();
                        _icj.GenJournalID = thisInvoice;
                        _icj.DateIssued = item.DateIssued;
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
                            _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(item.ItemNo.StockUOM.Oid);
                            _icj.Price = UOMSr.PricePerBaseUom;
                        }
                        else
                        {
                            _icj.OutQty = item.Quantity;
                            _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(item.UOM.Oid);
                            _icj.Price = item.Price;
                            item.ItemNo.SalesPrice = item.Price;
                        }
                        //_icj.Cost = item.ItemNo.Cost;
                        //_icj.Price = item.Price;
                        //_icj.UOM = item.BaseUOM;
                        _icj.RowID = item.RowID.ToString();
                        _icj.CostCenter = item.CostCenter != null ? item.CostCenter : null;
                        _icj.RequestedBy = item.RequestedBy != null ? item.RequestedBy : null;
                        _icj.Save();
                    }

                    // Update IH Parts Sales Registry
                    IHPartsSalesRegistry registry = new IHPartsSalesRegistry(session);
                    registry.Date = thisInvoice.EntryDate;
                    registry.Customer = defaultCustomer;
                    registry.Item = item.ItemNo;
                    registry.Quantity = Math.Abs(item.BaseQTY);
                    registry.UnitCost = item.ItemNo.Cost;
                    registry.UnitPrice = item.Price;
                    registry.Save();
                    item.SalesRegistryRowID = registry.RowID;

                    amount += item.Total;
                    int[] inds = null;
                    // Credit Sales Income
                    if (item.ItemNo.IncomeAccount == null)
                    {
                        throw new
                            ApplicationException(
                            "Must specify an income account in " + item.ItemNo.No +
                            " Item Card");
                    }
                    inds = accounts.Find("Account", item.ItemNo.IncomeAccount);
                    if (inds != null && inds.Length > 0)
                    {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.CreditAmount += Math.Abs(item.Total / (1 + defaultCustomer.TaxCode.Rate / 100));
                    }
                    else
                    {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.IncomeAccount;
                        tmpAccount.CreditAmount += Math.Abs(item.Total / (1 + defaultCustomer.TaxCode.Rate / 100));
                        accounts.Add(tmpAccount);
                    }
                    if (true)
                    {
                        // Credit Output Tax
                        if (defaultCustomer.TaxCode.Account == null)
                        {
                            throw new
                                ApplicationException(
                                "Must specify a Tax account in " + defaultCustomer.TaxCode.Code +
                                " Sales Tax Code");
                        }
                        inds = null;
                        inds = accounts.Find("Account", defaultCustomer.TaxCode.Account);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.CreditAmount += Math.Abs(item.Total - (item.Total / (1 + (defaultCustomer.TaxCode.Rate / 100))));
                        }
                        else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = defaultCustomer.TaxCode.Account;
                            tmpAccount.CreditAmount += Math.Abs(item.Total - (item.Total / (1 + (defaultCustomer.TaxCode.Rate / 100))));
                            accounts.Add(tmpAccount);
                        }
                        amount += Math.Abs(item.Total) * (defaultCustomer.TaxCode.Rate / 100);
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
                        tmpAccount.CreditAmount += (item.ItemNo.Cost * item.
                        Factor) * item.Quantity;
                    }
                    else
                    {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.InventoryAccount;
                        tmpAccount.CreditAmount += (item.ItemNo.Cost * item.
                        Factor) * item.Quantity;
                        accounts.Add(tmpAccount);
                    }
                    // Debit Cost of Goods Sold
                    if (item.ItemNo.COGSAccount == null)
                    {
                        throw new
                            ApplicationException("Must specify a COGS account in " +
                            item.ItemNo.No + " Item Card");
                    }
                    inds = null;
                    inds = accounts.Find("Account", item.ItemNo.COGSAccount);
                    if (inds != null && inds.Length > 0)
                    {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += (item.ItemNo.Cost * item.
                        Factor) * item.Quantity;
                    }
                    else
                    {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.COGSAccount;
                        tmpAccount.DebitAmount += (item.ItemNo.Cost * item.
                        Factor) * item.Quantity;
                        accounts.Add(tmpAccount);
                    }
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
                if (index == thisInvoice.WorkOrderItemDetails.Count)
                {
                    // if Cash
                    if (false)
                    {
                        //if (thisInvoice.CompanyInfo.UndepositedCollectionAcct == null)
                        //{
                        //    throw
                        //        new ApplicationException(
                        //        "Must specify a default undeposited collection account in the company information card"
                        //        );
                        //}
                        //// Create Accounts Receivable
                        //GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        //GenJournalDetail>(session);
                        //_gjde.GenJournalID = thisInvoice;
                        //_gjde.GenJournalID.Approved = true;
                        //_gjde.Account = thisInvoice.CompanyInfo.UndepositedCollectionAcct;
                        //_gjde.DebitAmount = Math.Abs(thisInvoice.GrossTotal.Value);
                        //_gjde.Description = "Invoice Entry for Customer " +
                        //thisInvoice.ReferenceNo;
                        //_gjde.SubAccountNo = thisInvoice.Customer;
                        //_gjde.SubAccountType = thisInvoice.Customer.ContactType;
                        //_gjde.Approved = true;
                        //debitAmount = debitAmount + _gjde.DebitAmount;
                        //_gjde.Save();

                    }
                    else
                    {
                        // Create Accounts Receivable
                        GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde.GenJournalID = thisInvoice;
                        _gjde.GenJournalID.Approved = true;
                        _gjde.Account = defaultCustomer.Account;
                        _gjde.DebitAmount = Math.Abs(thisInvoice.TotalParts.Value);
                        _gjde.Description = "WO Invoice Entry for Customer " +
                        thisInvoice.ReferenceNo;
                        _gjde.SubAccountNo = defaultCustomer;
                        _gjde.SubAccountType = defaultCustomer.ContactType;
                        _gjde.Approved = true;
                        debitAmount = debitAmount + _gjde.DebitAmount;
                        _gjde.Save();

                    }
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisInvoice;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "WO Invoice Entry for Customer " +
                        thisInvoice.ReferenceNo;
                        _gjd.SubAccountNo = defaultCustomer;
                        _gjd.SubAccountType = defaultCustomer.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }
                    thisInvoice.Status = WorkOrderStatusEnum.Invoiced;
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                        ApplicationException("Accounting entries not balance");
                    }
                    // Insert AR Registry code here
                        ARRegistry _arreg = ReflectionHelper.CreateObject<ARRegistry>(session);
                        _arreg.GenJournalID = thisInvoice;
                        _arreg.Date = thisInvoice.EntryDate;
                        _arreg.Customer = defaultCustomer;
                        _arreg.SourceDesc = "Work Order";
                        _arreg.SourceNo = thisInvoice.SourceNo;
                        _arreg.DocNo = thisInvoice.ReferenceNo;
                        _arreg.Amount = thisInvoice.TotalParts.Value;
                        _arreg.Save();

                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void InvoiceWorkOrderAction_Executed(object sender, ActionBaseEventArgs
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
                    ObjectSpace.ReloadObject(workOrder);
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
            invoiceWorkOrderAction.
                Enabled.SetItemValue("Invoicing entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
