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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class ReturnCreditMemoAction : ViewController {
        private CreditMemo creditMemo;
        private SimpleAction returncreditMemoAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReturnCreditMemoAction() {
            this.TargetObjectType = typeof(CreditMemo);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Return", this.GetType().Name);
            this.returncreditMemoAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.returncreditMemoAction.Caption = "Return";
            this.returncreditMemoAction.Execute += new 
            SimpleActionExecuteEventHandler(ReturnDebitMemoAction_Execute);
            this.returncreditMemoAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReturnDebitMemoAction_Executed);
            this.returncreditMemoAction.ConfirmationMessage = 
            "Do you really want to return these entries?";
            UpdateActionState(false);
        }
        private void ReturnDebitMemoAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            creditMemo = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as CreditMemo;
            ObjectSpace.CommitChanges();
            if (creditMemo.CreditMemoDetails.Count == 0) {
                XtraMessageBox.Show("There are no entries to return", 
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = creditMemo.CreditMemoDetails.Count;
            _FrmProgress = new ProgressForm("Returning entries...", count, 
            "Returning entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(creditMemo);
            _FrmProgress.ShowDialog();
        }
        private void ReturnDebitMemoAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(creditMemo);
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
            CreditMemo _creditMemo = (CreditMemo)e.Argument;
            CreditMemo thisCreditMemo = session.GetObjectByKey<CreditMemo>(
            _creditMemo.Oid);
            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            int partial = 0;
            try {
                // Validate Customer Accounts Payable
                if (thisCreditMemo.Customer == null) {throw new 
                    ApplicationException("Must specify a customer");} else {
                    if (thisCreditMemo.Customer.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Receivable account must be specified in the chosen customer card"
                        );}
                }
                foreach (CreditMemoDetail item in thisCreditMemo.
                CreditMemoDetails) {
                    // Validate Quantity against returning
                    if (item.Quantity > (item.Returning - item.Returned)) {throw 
                        new ApplicationException(
                        "Quantity to return cannot be greater than the returning quantity"
                        );}
                    _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                    _icj.GenJournalID = thisCreditMemo;
                    //_icj.InQTY = Math.Abs(item.BaseQTY);
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
                            _icj.InQTY = item.Quantity;
                        }
                        else
                        {
                            _icj.InQTY = item.BaseQTY / dStockUOM.Factor;
                        }
                        _icj.UOM = item.ItemNo.StockUOM;
                        _icj.Cost = (item.Quantity * item.ItemNo.GetInvoiceCost(item.InvoiceDetailID.RowID.ToString())) / item.BaseQTY;
                        decimal iprice = item.ItemNo.GetInvoicePrice(item.InvoiceDetailID.RowID.ToString());
                        _icj.Price = (item.Quantity * item.ItemNo.GetInvoicePrice(item.InvoiceDetailID.RowID.ToString())) / item.BaseQTY;
                    }
                    else
                    {
                        _icj.InQTY = item.Quantity;
                        _icj.UOM = item.UOM;
                        _icj.Cost = item.ItemNo.GetInvoiceCost(item.InvoiceDetailID.RowID.ToString());
                        _icj.Price = item.ItemNo.GetInvoicePrice(item.InvoiceDetailID.RowID.ToString());
                    }
                    //_icj.UOM = item.BaseUOM;
                    _icj.RowID = item.RowID.ToString();
                    _icj.Save();

                    if (item.InvoiceDetailID != null) {
                        item.InvoiceDetailID.Returned += Math.Abs(item.Quantity)
                        ;
                        //if (!(item.ReceiptDetailID.Returned == item.
                        //ReceiptDetailID.Quantity)) {partial++;}
                        IHPartsSalesRegistry registry = session.FindObject<IHPartsSalesRegistry>(CriteriaOperator.Parse("[RowID] = '" + item.InvoiceDetailID.SalesRegistryRowID + "'"));
                        registry.Quantity -= Math.Abs(item.Quantity);
                        registry.Save();
                    }
                    amount += item.Total;
                    int[] inds = null;
                    // Debit Sales Income
                    if (item.ItemNo.IncomeAccount == null) {throw new 
                        ApplicationException(
                        "Must specify an income account in " + item.ItemNo.No + 
                        " Item Card");}
                    inds = accounts.Find("Account", item.ItemNo.IncomeAccount);
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += Math.Abs(item.Total);
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.IncomeAccount;
                        tmpAccount.DebitAmount += Math.Abs(item.Total);
                        accounts.Add(tmpAccount);
                    }
                    if (item.Tax.Taxable) {
                        // Debit Output Tax
                        if (item.Tax.Account == null) {throw new 
                            ApplicationException(
                            "Must specify a Tax account in " + item.Tax.Code + 
                            " Sales Tax Code");}
                        inds = null;
                        inds = accounts.Find("Account", item.Tax.Account);
                        if (inds != null && inds.Length > 0) {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += Math.Abs(item.Total) * (
                            item.Tax.Rate / 100);
                        } else {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = item.Tax.Account;
                            tmpAccount.DebitAmount += Math.Abs(item.Total) * (
                            item.Tax.Rate / 100);
                            accounts.Add(tmpAccount);
                        }
                        amount += Math.Abs(item.Total) * (
                            item.Tax.Rate / 100);
                        // Debit Tax Expense
                        //if (item.Tax.ExpenseAccount == null) {throw new 
                        //    ApplicationException(
                        //    "Must specify an Expense account in " + item.Tax.
                        //    Code + " Sales Tax Code");}
                        //inds = null;
                        //inds = accounts.Find("Account", item.Tax.ExpenseAccount)
                        //;
                        //if (inds != null && inds.Length > 0) {
                        //    tmpAccount = accounts[inds[0]];
                        //    tmpAccount.DebitAmount += Math.Abs(item.Total) * (
                        //    item.Tax.Rate / 100);
                        //} else {
                        //    tmpAccount = new TempAccount();
                        //    tmpAccount.Account = item.Tax.ExpenseAccount;
                        //    tmpAccount.DebitAmount += Math.Abs(item.Total) * (
                        //    item.Tax.Rate / 100);
                        //    accounts.Add(tmpAccount);
                        //}
                    }
                    // Debit Inventory Asset
                    if (item.ItemNo.InventoryAccount == null) {throw new 
                        ApplicationException(
                        "Must specify an Inventory account in " + item.ItemNo.No 
                        + " Item Card");}
                    inds = null;
                    inds = accounts.Find("Account", item.ItemNo.InventoryAccount
                    );
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += (item.ItemNo.GetInvoiceCost(
                        item.InvoiceDetailID.RowID.ToString()) * item.Factor) * 
                        item.Quantity;
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.InventoryAccount;
                        tmpAccount.DebitAmount += (item.ItemNo.GetInvoiceCost(
                        item.InvoiceDetailID.RowID.ToString()) * item.Factor) * 
                        item.Quantity;
                        accounts.Add(tmpAccount);
                    }
                    // Credit Cost of Goods Sold
                    if (item.ItemNo.COGSAccount == null) {throw new 
                        ApplicationException("Must specify a COGS account in " + 
                        item.ItemNo.No + " Item Card");}
                    inds = null;
                    inds = accounts.Find("Account", item.ItemNo.COGSAccount);
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.CreditAmount += (item.ItemNo.GetInvoiceCost(
                        item.InvoiceDetailID.RowID.ToString()) * item.Factor) * 
                        item.Quantity;
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.COGSAccount;
                        tmpAccount.CreditAmount += (item.ItemNo.GetInvoiceCost(
                        item.InvoiceDetailID.RowID.ToString()) * item.Factor) * 
                        item.Quantity;
                        accounts.Add(tmpAccount);
                    }
                    if (item.ItemNo.RequireSerial)
                    {
                        if (item.CreditMemoDetailTrackingLines.Count != Math.Abs(
                        item.BaseQTY))
                        {
                            throw new ApplicationException(
                                "An item requires a serial no. Please specify serial nos according to quantity"
                                );
                        }
                        foreach (CreditMemoDetailTrackingLine iTrack in item.
                        CreditMemoDetailTrackingLines)
                        {
                            ItemTrackingEntry _ite = null;
                            //if (item.BaseQTY > 0) {
                            //    // Check if serial no exist and if its currently removed
                            //    // if found and currently marked as removed change the status to available
                            //    _ite = (ItemTrackingEntry)session.FindObject(
                            //    typeof(ItemTrackingEntry), CriteriaOperator.
                            //    Parse("[ItemNo.No] = '" + item.ItemNo.No + 
                            //    "' And [SerialNo] = '" + iTrack.SerialNo + 
                            //    "' And [Warehouse.Code] = '" + item.Warehouse.
                            //    Code + "' And [Status] = 'Removed'"));
                            //    if (_ite != null) {
                            //        _ite.Status = SerialNoStatusEnum.Available;
                            //        _ite.Save();
                            //    } else {
                            //        _ite = ReflectionHelper.CreateObject<
                            //        ItemTrackingEntry>(session);
                            //        _ite.IcjID = _icj;
                            //        _ite.ItemNo = item.ItemNo;
                            //        _ite.SerialNo = iTrack.SerialNo;
                            //        _ite.Warehouse = item.Warehouse;
                            //        _ite.Status = SerialNoStatusEnum.Available;
                            //        _ite.Save();
                            //    }
                            //} else {
                            // Find serial no then mark it as removed (make sure that is available)
                            _ite = (ItemTrackingEntry)session.FindObject(typeof(
                            ItemTrackingEntry), CriteriaOperator.Parse(
                            "[ItemNo.No] = '" + item.ItemNo.No +
                            "' And [SerialNo] = '" + iTrack.SerialNo +
                            "' And [Warehouse.Code] = '" + item.Warehouse.Code +
                            "' And [Status] = 'Sold'"));
                            if (_ite != null)
                            {
                                _ite.Status = SerialNoStatusEnum.Available;
                                _ite.DestPerson = null;
                                _ite.DestSource = null;
                                _ite.DestSourceNo = null;
                                _ite.DestRowID = string.Empty;
                                _ite.Save();
                            }
                            else
                            {
                                throw new ApplicationException(
                                "Cannot find the serial #" + iTrack.SerialNo +
                                " in the list of available serial nos");
                            }
                            //}
                        }
                    }
                    if (_BgWorker.CancellationPending) {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Returning entry {0} succesfull.", 
                    thisCreditMemo.ReceiptDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            } finally {
                if (index == thisCreditMemo.CreditMemoDetails.Count) {
                    // Create Accounts Receivable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisCreditMemo;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisCreditMemo.Customer.Account;
                    _gjde.CreditAmount = Math.Abs(amount);
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Description = "Return of Goods from Customer";
                    _gjde.SubAccountNo = thisCreditMemo.Customer;
                    _gjde.SubAccountType = thisCreditMemo.Customer.ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts) {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisCreditMemo;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Description = "Return of Goods from Customer";
                        _gjd.SubAccountNo = thisCreditMemo.Customer;
                        _gjd.SubAccountType = thisCreditMemo.Customer.
                        ContactType;
                        _gjd.Approved = true;
                        _gjd.Save();
                    }
                    thisCreditMemo.Status = CreditMemoStatusEnum.Returned;
                    foreach (InvoiceDetail item in thisCreditMemo.InvoiceNo.
                    InvoiceDetails) {if (!(item.Returned == item.Quantity)) {
                            partial++;}}
                    if (partial > 0 && thisCreditMemo.InvoiceNo != null) {
                        thisCreditMemo.InvoiceNo.Status = InvoiceStatusEnum.
                        PartiallyReturned;} else {
                        if (partial == 0 && thisCreditMemo.InvoiceNo != null) {
                            thisCreditMemo.InvoiceNo.Status = InvoiceStatusEnum.
                            Returned;}
                    }
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
                "Return entries operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully returned");

                    ObjectSpace.ReloadObject(creditMemo);
                    ObjectSpace.Refresh();

                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { returncreditMemoAction
            .Enabled.SetItemValue("Return entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
