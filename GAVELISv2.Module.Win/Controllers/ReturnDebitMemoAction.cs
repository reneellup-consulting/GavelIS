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
    public partial class ReturnDebitMemoAction : ViewController {
        private DebitMemo debitMemo;
        private SimpleAction returnDebitMemoAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReturnDebitMemoAction() {
            this.TargetObjectType = typeof(DebitMemo);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Return", this.GetType().Name);
            this.returnDebitMemoAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.returnDebitMemoAction.Caption = "Return";
            this.returnDebitMemoAction.Execute += new 
            SimpleActionExecuteEventHandler(ReturnDebitMemoAction_Execute);
            this.returnDebitMemoAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReturnDebitMemoAction_Executed);
            this.returnDebitMemoAction.ConfirmationMessage = 
            "Do you really want to return these entries?";
            UpdateActionState(false);
        }
        private void ReturnDebitMemoAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            debitMemo = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DebitMemo;
            ObjectSpace.CommitChanges();
            if (debitMemo.DebitMemoDetails.Count == 0) {
                XtraMessageBox.Show("There are no entries to return", 
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            
            var count = debitMemo.DebitMemoDetails.Count;
            _FrmProgress = new ProgressForm("Returning entries...", count, 
            "Returning entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(debitMemo);
            _FrmProgress.ShowDialog();
        }
        private void ReturnDebitMemoAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(debitMemo);
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
            DebitMemo _debitMemo = (DebitMemo)e.Argument;
            DebitMemo thisDebitMemo = session.GetObjectByKey<DebitMemo>(
            _debitMemo.Oid);
            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            int partial = 0;
            try {
                // Validate Vendor Accounts Payable
                if (thisDebitMemo.Vendor == null) {throw new 
                    ApplicationException("Must specify a vendor");} else {
                    if (thisDebitMemo.Vendor.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );}
                }
                foreach (DebitMemoDetail item in thisDebitMemo.DebitMemoDetails) 
                {
                    // Validate Current WH Quantity against Quantity to Return
                    Requisition reqs = null;
                    if (item.ReceiptDetailID != null && item.ReceiptDetailID.RequisitionNo != null)
                    {
                        reqs = session.GetObjectByKey<Requisition>(item.ReceiptDetailID.RequisitionNo.Oid);
                    }
                    decimal bQty = 0m;
                    UOMRelation dStockUOM = null;
                    if (item.ItemNo.UOMRelations.Count > 0)
                    {
                        dStockUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.ItemNo.StockUOM).FirstOrDefault();
                        bQty = item.BaseQTY / dStockUOM.Factor;
                        //if (bQty > item.ItemNo.GetWarehouseQtyBaseCorrected(item.
                        //    Warehouse, thisDebitMemo.Oid, thisDebitMemo.EntryDate, "DM"))
                        //{
                        //    throw new ApplicationException(
                        //        "Insufficient qauntity on hand on the warehouse specified"
                        //        );
                        //}
                        if (bQty > item.ItemNo.GetWarehouseQtyBaseSimplified(item.
                            Warehouse, thisDebitMemo.EntryDate) && thisDebitMemo.CompanyInfo.AllowInsufficientCurrQty != true)
                        {
                            throw new ApplicationException(
                                "Insufficient qauntity on hand on the warehouse specified for item #" + item.ItemNo.No + "!"
                                );
                        }
                    }
                    else
                    {
                        Requisition req = null;
                        if (item.ReceiptDetailID != null && item.ReceiptDetailID.RequisitionNo != null)
                        {
                            req = item.ReceiptDetailID.RequisitionNo ?? null;
                        }
                        //var nn = item.ItemNo.GetWarehouseQtyBaseCorrected(item.
                        //Warehouse, thisDebitMemo.Oid, thisDebitMemo.EntryDate, "DM", req);

                        //if (item.BaseQTY > item.ItemNo.GetWarehouseQtyBaseCorrected(item.
                        //Warehouse, thisDebitMemo.Oid, thisDebitMemo.EntryDate, "DM", req))
                        //{
                        //    throw new ApplicationException(
                        //        "Insufficient qauntity on hand on the warehouse specified"
                        //        );
                        //}
                        if (item.BaseQTY > item.ItemNo.GetWarehouseQtyBaseSimplified(item.
                        Warehouse, thisDebitMemo.EntryDate) && thisDebitMemo.CompanyInfo.AllowInsufficientCurrQty != true)
                        {
                            throw new ApplicationException(
                                "Insufficient qauntity on hand on the warehouse specified for item #" + item.ItemNo.No + "!"
                                );
                        }

                    }
                    //if (item.BaseQTY > item.ItemNo.GetWarehouseQtyBase(item.
                    //Warehouse, item.GenJournalID.EntryDate))
                    //{
                    //    throw new ApplicationException(
                    //        "Insufficient qauntity on hand on the warehouse specified"
                    //        );
                    //}
                    // Validate Quantity against returning
                    if (item.Quantity > (item.Returning - item.Returned)) {throw 
                        new ApplicationException(
                        "Quantity to return cannot be greater than the returning quantity"
                        );}
                    _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                    _icj.GenJournalID = thisDebitMemo;
                    if (item.ItemNo.UOMRelations.Count > 0)
                    {
                        _icj.OutQty = Math.Abs(bQty);
                        _icj.UOM = dStockUOM.UOM;
                        _icj.Cost = (item.Quantity * item.Cost) / _icj.OutQty;
                    }
                    else
                    {
                        _icj.OutQty = Math.Abs(item.BaseQTY);
                        _icj.UOM = item.BaseUOM;
                        _icj.Cost = item.BaseCost;
                    }
                    _icj.Warehouse = item.Warehouse;
                    _icj.ItemNo = item.ItemNo;
                    _icj.RequisitionNo = reqs ?? null;
                    _icj.RowID = item.RowID.ToString();
                    _icj.Save();
                    if (item.ReceiptDetailID != null) {
                        item.ReceiptDetailID.Returned += Math.Abs(item.Quantity)
                        ;
                        //if (!(item.ReceiptDetailID.Returned == item.
                        //ReceiptDetailID.Quantity)) {partial++;}
                    }
                    amount += item.Total;
                    int[] inds = null;
                    inds = accounts.Find("Account", item.ItemNo.InventoryAccount
                    );
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        //tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item
                        //.Total) : 0;
                        tmpAccount.CreditAmount += Math.Abs(item.Total);
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.InventoryAccount;
                        //tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item
                        //.Total) : 0;
                        tmpAccount.CreditAmount += Math.Abs(item.Total);
                        accounts.Add(tmpAccount);
                    }
                    if (item.ItemNo.RequireSerial) {
                        if (item.DebitMemoDetailTrackingLines.Count != Math.Abs(
                        item.BaseQTY)) {throw new ApplicationException(
                            "An item requires a serial no. Please specify serial nos according to quantity"
                            );}
                        foreach (DebitMemoDetailTrackingLine iTrack in item.
                        DebitMemoDetailTrackingLines) {
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
                            "' And [Status] = 'Available'"));
                            if (_ite != null) {
                                _ite.Status = SerialNoStatusEnum.Removed;
                                _ite.Save();
                            } else {
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
                    thisDebitMemo.ReceiptDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            } finally {
                if (index == thisDebitMemo.DebitMemoDetails.Count) {
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts) {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisDebitMemo;
                        _gjd.Account = item.Account;
                        //_gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Description = "Return to Vendor";
                        _gjd.SubAccountNo = thisDebitMemo.Vendor;
                        _gjd.SubAccountType = thisDebitMemo.Vendor.ContactType;
                        _gjd.Approved = true;
                        _gjd.Save();
                    }
                    // Create Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisDebitMemo;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisDebitMemo.Vendor.Account;
                    _gjde.DebitAmount = Math.Abs(amount);
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    //_gjde.CreditAmount = amount > 0 ? Math.Abs(amount) : 0;
                    _gjde.Description = "Return to Vendor";
                    _gjde.SubAccountNo = thisDebitMemo.Vendor;
                    _gjde.SubAccountType = thisDebitMemo.Vendor.ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    thisDebitMemo.Status = DebitMemoStatusEnum.Returned;
                    foreach (ReceiptDetail item in thisDebitMemo.ReceiptNo.
                    ReceiptDetails) {if (!(item.Returned == item.Quantity)) {
                            partial++;}}
                    if (partial > 0 && thisDebitMemo.ReceiptNo != null && 
                    thisDebitMemo.ReceiptNo.Status != ReceiptStatusEnum.Paid) {
                        thisDebitMemo.ReceiptNo.Status = ReceiptStatusEnum.
                        PartiallyReturned;} else {
                        if (partial == 0 && thisDebitMemo.ReceiptNo != null) {
                            thisDebitMemo.ReceiptNo.Status = ReceiptStatusEnum.
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

                    ObjectSpace.ReloadObject(debitMemo);
                    ObjectSpace.Refresh();

                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { returnDebitMemoAction.
            Enabled.SetItemValue("Return entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
