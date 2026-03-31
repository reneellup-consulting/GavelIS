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
    public partial class ReceiveReceiptAction : ViewController {
        private Receipt receipt;
        private SimpleAction receiveReceiptAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReceiveReceiptAction() {
            this.TargetObjectType = typeof(Receipt);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.Receive", this.GetType().Name);
            this.receiveReceiptAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.receiveReceiptAction.Caption = "Receive";
            this.receiveReceiptAction.Execute += new 
            SimpleActionExecuteEventHandler(ReceiveReceiptAction_Execute);
            this.receiveReceiptAction.Executed += new EventHandler<
            ActionBaseEventArgs>(PhysicalAdjustmentApplyAction_Executed);
            this.receiveReceiptAction.ConfirmationMessage = 
            "Do you really want to receive these entries?";
            UpdateActionState(false);
        }
        private void ReceiveReceiptAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            //receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            //CurrentObject as Receipt;
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.DetailView))
            {
                receipt = ((DevExpress.ExpressApp.DetailView)this.View).
                CurrentObject as Receipt;
            }
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.ListView))
            {
                receipt = this.View.CurrentObject as Receipt;
            }
            if (string.IsNullOrEmpty(receipt.InvoiceNo))
            {
                XtraMessageBox.Show("Must provide Invoice No. to continue.",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var nonCash = receipt.ReceiptDetails.Where(o => o.PettyCashID == null).FirstOrDefault();
            var isCash = receipt.ReceiptDetails.Where(o => o.PettyCashID != null).FirstOrDefault();
            if (nonCash != null && isCash != null)
            {
                throw new UserFriendlyException("Cannot combine non cash purchases from cash purchases");
            }
            ObjectSpace.CommitChanges();
            if (receipt.ReceiptDetails.Count == 0) {
                XtraMessageBox.Show("There are no entries to receive", 
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = receipt.ReceiptDetails.Count;
            _FrmProgress = new ProgressForm("Receiving entries...", count, 
            "Receiving entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(receipt);
            _FrmProgress.ShowDialog();
        }
        private void PhysicalAdjustmentApplyAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(receipt);
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
            Receipt _receipt = (Receipt)e.Argument;
            //if (_receipt.Oid==-1)
            //{
            //    throw new
            //            ApplicationException(
            //            "Please save this transaction first before receiving"
            //            );
            //}
            Receipt thisReceipt = session.GetObjectByKey<Receipt>(_receipt.Oid);
            InventoryControlJournal _icj = null;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            //int partial = 0;
            try {
                // Validate Vendor Accounts Payable
                if (thisReceipt.Vendor == null) {throw new ApplicationException(
                    "Must specify a vendor");} else {
                    if (thisReceipt.Vendor.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );}
                }
                foreach (ReceiptDetail item in thisReceipt.ReceiptDetails)
                {
                    if (!thisReceipt.TireTransaction)
                    {
                        if (item.ItemNo.ItemType != ItemTypeEnum.ChargeItem)
                        {
                            _icj = ReflectionHelper.CreateObject<InventoryControlJournal>(session);
                            _icj.GenJournalID = thisReceipt;
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
                                    //UOMr.CostPerBaseUom = //(item.Quantity * item.Cost) / _icj.InQTY;
                                }
                                UOMr.CostPerBaseUom = item.Cost;
                                UOMSr.CostPerBaseUom = (item.Quantity * item.Cost) / _icj.InQTY;
                                _icj.UOM = item.ItemNo.StockUOM;
                                _icj.Cost = UOMSr.CostPerBaseUom;
                            }
                            else
                            {
                                _icj.InQTY = item.Quantity;
                                _icj.UOM = item.UOM;
                                _icj.Cost = item.Cost;
                                item.ItemNo.Cost = item.Cost;
                            }
                            _icj.RowID = item.RowID.ToString();
                            _icj.RequisitionNo = item.RequisitionNo ?? null;
                            _icj.CostCenter = item.CostCenter ?? null;
                            _icj.RequestedBy = item.RequestedBy ?? null;
                            _icj.Save();
                        }
                        // Save last direct cost to item card
                        //item.ItemNo.Cost = item.BaseCost;
                        if (item.PODetailID != null)
                        {
                            item.PODetailID.skipAuto = true;
                            item.PODetailID.Received += Math.Abs(item.Quantity);
                            decimal dt = item.PODetailID.PurchaseInfo.PurchaseOrderDetails.Sum(o => o.RemainingQty);
                            if (dt==0)
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
                        amount += item.Total;
                        int[] inds = null;
                        if (item.ItemNo.InventoryAccount == null)
                        {
                            throw new ApplicationException("Inventory account must be specified in the chosen item card");
                        }
                        inds = accounts.Find("Account", item.ItemNo.InventoryAccount);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item.Total) : 0;
                            tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(item.Total) : 0;
                        } else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = item.ItemNo.InventoryAccount;
                            tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item.Total) : 0;
                            tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(item.Total) : 0;
                            accounts.Add(tmpAccount);
                        }
                        if (item.ItemNo.RequireSerial)
                        {
                            if (item.ReceiptDetailTrackingLines.Count != Math.Abs(item.BaseQTY))
                            {
                                throw new ApplicationException("An item requires a serial no. Please specify serial nos according to quantity");
                            }
                            foreach (ReceiptDetailTrackingLine iTrack in item.ReceiptDetailTrackingLines)
                            {
                                ItemTrackingEntry _ite = null;
                                if (item.BaseQTY > 0)
                                {
                                    // Check if serial no exist and if its currently removed
                                    // if found and currently marked as removed change the status to available
                                    _ite = (ItemTrackingEntry)session.FindObject(typeof(ItemTrackingEntry), CriteriaOperator.Parse(string.Format("[ItemNo.No] = '{0}' And [SerialNo] = '{1}' And [Warehouse.Code] = '{2}' And [Status] = 'Removed'", item.ItemNo.No, iTrack.SerialNo, item.Warehouse.Code)));
                                    if (_ite != null)
                                    {
                                        _ite.Status = SerialNoStatusEnum.Available;
                                        _ite.Save();
                                    } else
                                    {
                                        _ite = ReflectionHelper.CreateObject<ItemTrackingEntry>(session);
                                        _ite.IcjID = _icj;
                                        _ite.ItemNo = item.ItemNo;
                                        _ite.SerialNo = iTrack.SerialNo;
                                        _ite.Warehouse = item.Warehouse;
                                        _ite.Status = SerialNoStatusEnum.Available;
                                        _ite.Save();
                                    }
                                } else
                                {
                                    // Find serial no then mark it as removed (make sure that is available)
                                    _ite = (ItemTrackingEntry)session.FindObject(typeof(ItemTrackingEntry), CriteriaOperator.Parse(string.Format("[ItemNo.No] = '{0}' And [SerialNo] = '{1}' And [Warehouse.Code] = '{2}' And [Status] = 'Available'", item.ItemNo.No, iTrack.SerialNo, item.Warehouse.Code)));
                                    if (_ite != null)
                                    {
                                        _ite.Status = SerialNoStatusEnum.Removed;
                                        _ite.Save();
                                    } else
                                    {
                                        throw new ApplicationException(string.Format("Cannot find the serial #{0} in the list of available serial nos", iTrack.SerialNo));
                                    }
                                }
                            }
                        }
                    } else
                    {
                        // Start coding algorithms for retreaders items
                        if (item.TireToRetDetailId == null)
                        {
                            throw new ApplicationException(string.Format("Tire to Retreaders detail id does not exist in receipt line #{0}", item.Oid));
                        }
                        TireToRetDetail ttr = session.GetObjectByKey<TireToRetDetail>(item.TireToRetDetailId.Oid);
                        ttr.Completed = true;
                        Tire tr = ttr.TireNo;
                        CloneHelper cnh = new CloneHelper(session);
                        //CloneHelper cnh1 = new CloneHelper(session);
                        TireServiceDetail2 tsd1;
                        if (tr.LastDetail.Remarks.Contains("Received"))
                        {
                            tsd1 = ReflectionHelper.CreateObject<TireServiceDetail2>(session);
                            tr.TireServiceDetails2.Add(tsd1);
                            tsd1.BrandingNo = tr.LastDetail.BrandingNo;
                            tsd1.EntryDate = item.GenJournalID.EntryDate;
                            tsd1.ActivityDate = item.GenJournalID.EntryDate;
                            tsd1.ActivityType = TireActivityTypeEnum.Dettached;
                        }
                        else
                        {
                            tsd1 = tr.LastDetail;
                        }
                        
                        if (tr.LastDetail.ActivityType != TireActivityTypeEnum.Dettached)
                        {
                            throw new ApplicationException(string.Format("Last detail found is for the tire in receipt line #{0}\r is not tire activity type Dettached", item.Oid));
                        }
                        TireServiceDetail2 tsd2 = cnh.Clone<TireServiceDetail2>(tr.LastDetail, true);
                        tsd2.EntryDate = item.GenJournalID.EntryDate;
                        tsd2.ActivityDate = item.GenJournalID.EntryDate;
                        tsd2.TfsId = tr.LastDetail.TfsId ?? null;
                        // Must be linked to dettach reason as recapped with respect to recap type and vendor
                        StringBuilder sb;
                        if (item.Regrooved)
                        {
                            sb = new StringBuilder("RG");
                        }
                        else
                        {
                            sb = new StringBuilder("R");
                            tr.TireItemClass = TireItemClassEnum.RecappedTire;
                        }
                        if (!string.IsNullOrEmpty(thisReceipt.Vendor.Initial))
                        {
                            sb.AppendFormat("/{0}", thisReceipt.Vendor.Initial);
                        }
                        else
                        {
                            throw new ApplicationException("Vendor initials has not been set up");
                        }
                        if (!item.Regrooved && item.RecapType != null)
                        {
                            sb.AppendFormat("/{0}", item.RecapType.Code);
                        }
                        //else
                        //{
                        //    throw new ApplicationException(string.Format("Recap type was not specified in line #{0}", item.Oid));
                        //}
                        if (item.WithSR)
                        {
                            sb.Append(" W/SR");
                        }
                        tsd1.Reason = session.FindObject<TireDettachReason>(
                        new BinaryOperator("Code", sb.ToString()));
                        if (tsd1.Reason == null)
                        {
                            IObjectSpace createObjectSpace = Application.CreateObjectSpace();
                            //throw new ApplicationException(string.Format("Reason {0} does not exist for line #{1}", sb.ToString(), item.Oid));
                            //XtraMessageBox.Show(string.Format("Reason {0} does not exist for line #{1}. It will be automatically created.", sb.ToString(), item.Oid));
                            TireDettachReason ntsd1 = ReflectionHelper.CreateObject<TireDettachReason>(((ObjectSpace)createObjectSpace).Session);
                            ntsd1.Code = sb.ToString();
                            ntsd1.Description = "Enter description here...";
                            ntsd1.Recapped = !item.Regrooved;
                            ntsd1.Vendor = createObjectSpace.GetObjectByKey<Vendor>(item.ReceiptInfo.Vendor.Oid);
                            ntsd1.Save();
                            createObjectSpace.CommitChanges();
                            tsd1.Reason = session.FindObject<TireDettachReason>(
                        new BinaryOperator("Code", sb.ToString()));
                        }
                        tsd1.Remarks = tsd1.Reason.Description;
                        tsd2.Reason = session.FindObject<TireDettachReason>(
                        new BinaryOperator("Code", "USABLE"));
                        if (tsd2.Reason == null)
                        {
                            throw new ApplicationException("Reason USABLE has not yet been established");
                        }
                        tsd2.Vendor = thisReceipt.Vendor;
                        tsd2.Cost = item.Total;
                        tsd1.Cost = 0m;
                        tsd2.TreadDepth = item.TreadDepth;
                        tsd1.ReferenceNo = item.GenJournalID.SourceNo;
                        tsd2.ReferenceNo = item.GenJournalID.SourceNo;
                        tsd1.Remarks = string.Format("Sent to retreader on {0}", item.TireToRetDetailId.DocNo.EntryDate.ToShortDateString()); //string.Format("Sold to {0} on {1}", (item.GenJournalID as Invoice).Customer.Name, item.GenJournalID.EntryDate.ToShortDateString());
                        tsd2.Remarks = string.Format("Received {0} on {1}", sb.ToString(), thisReceipt.EntryDate.ToShortDateString());
                        tr.TireServiceDetails2.BaseAdd(tsd2);
                        tsd2.ActivityType = TireActivityTypeEnum.Dettached;
                        tsd2.Save();
                        tsd1.Save();
                        tr.Description = string.Format("{0} {1}", tr.TireItem.Description, sb.ToString());
                        tr.Save();
                        ttr.Save();

                        amount += item.Total;
                        int[] inds = null;
                        if (item.ItemNo.InventoryAccount == null)
                        {
                            throw new ApplicationException("Inventory account must be specified in the chosen item card");
                        }
                        inds = accounts.Find("Account", item.ItemNo.InventoryAccount);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item.Total) : 0;
                            tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(item.Total) : 0;
                        }
                        else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = item.ItemNo.InventoryAccount;
                            tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item.Total) : 0;
                            tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(item.Total) : 0;
                            accounts.Add(tmpAccount);
                        }
                        // End other than retreader
                    }
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Receiving entry {0} succesfull.",
                    thisReceipt.ReceiptDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            } finally {
                if (index == thisReceipt.ReceiptDetails.Count) {
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts) {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisReceipt;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Receipt of Goods " + thisReceipt.
                        InvoiceNo;
                        _gjd.SubAccountNo = thisReceipt.Vendor;
                        _gjd.SubAccountType = thisReceipt.Vendor.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }
                    // Create Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisReceipt;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisReceipt.Vendor.Account;
                    _gjde.DebitAmount = thisReceipt.Total.Value < 0 ? Math.Abs(
                    thisReceipt.Total.Value) : 0;
                    _gjde.CreditAmount = thisReceipt.Total.Value > 0 ? Math.Abs(
                    thisReceipt.Total.Value) : 0;
                    _gjde.Description = "Receipt of Goods " + thisReceipt.
                    InvoiceNo;
                    _gjde.SubAccountNo = thisReceipt.Vendor;
                    _gjde.SubAccountType = thisReceipt.Vendor.ContactType;
                    _gjde.Approved = true;
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Save();
                    // Create Purchase Discount
                    if (thisReceipt.Discount > 0) {
                        GenJournalDetail _gjde3 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde3.GenJournalID = thisReceipt;
                        _gjde3.GenJournalID.Approved = true;
                        _gjde3.Account = thisReceipt.DiscountAcct != null ? 
                        thisReceipt.DiscountAcct : thisReceipt.CompanyInfo.
                        PurchaseDiscountAcct;
                        _gjde3.DebitAmount = thisReceipt.Discount < 0 ? Math.Abs
                        (thisReceipt.Discount) : 0;
                        _gjde3.CreditAmount = thisReceipt.Discount > 0 ? Math.
                        Abs(thisReceipt.Discount) : 0;
                        _gjde3.Description = 
                        "Discount for the Receipt of Goods " + thisReceipt.
                        InvoiceNo;
                        _gjde3.SubAccountNo = thisReceipt.Vendor;
                        _gjde3.SubAccountType = thisReceipt.Vendor.ContactType;
                        _gjde3.Approved = true;
                        debitAmount = debitAmount + _gjde3.DebitAmount;
                        creditAmount = creditAmount + _gjde3.CreditAmount;
                        _gjde3.Save();
                    }
                    thisReceipt.Status = ReceiptStatusEnum.Received;
                    if (thisReceipt.InvoiceType == InvoiceTypeEnum.Cash)
                    {
                        thisReceipt.Adjusted = thisReceipt.Total.GetValueOrDefault(0m);
                        thisReceipt.Status = ReceiptStatusEnum.Paid;
                    }
                    //if (thisReceipt.PurchaseOrderNo != null)
                    //{
                    //    foreach (PurchaseOrderDetail item in thisReceipt.
                    //    PurchaseOrderNo.PurchaseOrderDetails)
                    //    {
                    //        if (!(item.
                    //            Received >= item.Quantity)) { partial++; }
                    //    }
                    //    if (partial > 0 && thisReceipt.PurchaseOrderNo != null)
                    //    {
                    //        thisReceipt.PurchaseOrderNo.Status =
                    //           PurchaseOrderStatusEnum.PartiallyReceived;
                    //    }
                    //    else
                    //    {
                    //        if (partial == 0 && thisReceipt.PurchaseOrderNo !=
                    //        null)
                    //        {
                    //            thisReceipt.PurchaseOrderNo.Status =
                    //                PurchaseOrderStatusEnum.Received;
                    //        }
                    //    }
                    //}
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new 
                        ApplicationException("Accounting entries not balance");}
                    // Update AP Registry
                    if (thisReceipt.InvoiceType != InvoiceTypeEnum.Cash)
                    {
                        APRegistry _apreg = ReflectionHelper.CreateObject<APRegistry>(session);
                        _apreg.GenJournalID = thisReceipt;
                        _apreg.Date = thisReceipt.EntryDate;
                        _apreg.Vendor = thisReceipt.Vendor;
                        _apreg.SourceDesc = thisReceipt.SourceType.Description;
                        _apreg.SourceNo = thisReceipt.SourceNo;
                        _apreg.DocNo = thisReceipt.InvoiceNo;
                        _apreg.Amount = thisReceipt.Total.Value;
                        _apreg.Save();

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
                "Receiving entries operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully received");
                    ObjectSpace.ReloadObject(receipt);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { receiveReceiptAction.
            Enabled.SetItemValue("Receiving entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
