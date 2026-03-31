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
    public partial class ReceiveFuelAction : ViewController
    {
        private ReceiptFuel receipt;
        private SimpleAction receiveFuelAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReceiveFuelAction()
        {
            this.TargetObjectType = typeof(ReceiptFuel);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReceiveFuel", this.GetType().
            Name);
            this.receiveFuelAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.receiveFuelAction.Caption = "Receive";
            this.receiveFuelAction.Execute += new
            SimpleActionExecuteEventHandler(ReceiveReceiptAction_Execute);
            this.receiveFuelAction.Executed += new EventHandler<
            ActionBaseEventArgs>(PhysicalAdjustmentApplyAction_Executed);
            this.receiveFuelAction.ConfirmationMessage =
            "Do you really want to receive these entries?";
            UpdateActionState(false);
        }
        private void ReceiveReceiptAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ReceiptFuel;
            ObjectSpace.CommitChanges();
            if (receipt.ReceiptFuelDetails.Count == 0)
            {
                XtraMessageBox.Show("There are no entries to receive",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = receipt.ReceiptFuelDetails.Count;
            _FrmProgress = new ProgressForm("Receiving entries...", count,
            "Receiving entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(receipt);
            _FrmProgress.ShowDialog();
        }
        private void PhysicalAdjustmentApplyAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //ObjectSpace.ReloadObject(receipt);
            //ObjectSpace.Refresh();
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            //DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            ReceiptFuel _receipt = (ReceiptFuel)e.Argument;
            ReceiptFuel thisReceipt = session.GetObjectByKey<ReceiptFuel>(
            _receipt.Oid);
            
            //InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            int partial = 0;
            try
            {
                // Validate Vendor Accounts Payable
                if (thisReceipt.Vendor == null)
                {
                    throw new ApplicationException(
                    "Must specify a vendor");
                } else
                {
                    if (thisReceipt.Vendor.Account == null)
                    {
                        throw new
                        ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );
                    }
                }
                foreach (ReceiptFuelDetail item in thisReceipt.
                ReceiptFuelDetails)
                {
                    //_icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    //>(session);
                    //_icj.GenJournalID = thisReceipt;
                    //_icj.InQTY = Math.Abs(item.BaseQTY);
                    //_icj.Warehouse = item.Warehouse;
                    //_icj.ItemNo = item.ItemNo;
                    //_icj.Cost = item.BaseCost;
                    //_icj.UOM = item.BaseUOM;
                    //_icj.RowID = item.RowID.ToString();
                    //_icj.Save();
                    // Save last direct cost to item card
                    item.ItemNo.Cost = item.BaseCost;
                    if (item.PODetailID != null)
                    {
                        item.PODetailID.Received += Math.Abs(item.Quantity);
                        item.Received = item.PODetailID.Received;
                        //if (!(item.PODetailID.Received >= item.PODetailID.Quantity)) { partial++; }
                    }
                    amount += item.Total;
                    int[] inds = null;
                    inds = accounts.Find("Account", ((FuelItem)item.ItemNo).ExpenseAccount);
                    if (inds != null && inds.Length > 0)
                    {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item
                        .Total) : 0;
                        tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(
                        item.Total) : 0;
                    } else
                    {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = ((FuelItem)item.ItemNo).ExpenseAccount;
                        tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item
                        .Total) : 0;
                        tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(
                        item.Total) : 0;
                        accounts.Add(tmpAccount);
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
            } finally
            {
                if (index == thisReceipt.ReceiptFuelDetails.Count)
                {
                    // Create Expense Account Entry
                    foreach (TempAccount item in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisReceipt;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Description =
                        "Receipt of Fuel for Trucking Operations";
                        _gjd.SubAccountNo = thisReceipt.Vendor;
                        _gjd.SubAccountType = thisReceipt.Vendor.ContactType;
                        _gjd.Approved = true;
                        _gjd.Save();
                    }
                    // Create Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisReceipt;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisReceipt.Vendor.Account;
                    _gjde.DebitAmount = amount < 0 ? Math.Abs(amount) : 0;
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    _gjde.CreditAmount = amount > 0 ? Math.Abs(amount) : 0;
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Description =
                    "Receipt of Fuel for Trucking Operations";
                    _gjde.SubAccountNo = thisReceipt.Vendor;
                    _gjde.SubAccountType = thisReceipt.Vendor.ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    thisReceipt.Status = ReceiptFuelStatusEnum.Received;
                    if (thisReceipt.PurchaseOrderNo != null)
                    {
                        foreach (POrderFuelDetail item in thisReceipt.
                        PurchaseOrderNo.POrderFuelDetails)
                        {
                            if (!(item.
                            Received >= item.Quantity))
                            {
                                partial++;
                            }
                        }
                        if (partial > 0 && thisReceipt.PurchaseOrderNo != null)
                        {
                            thisReceipt.PurchaseOrderNo.Status =
                            PurchaseOrderFuelStatusEnum.PartiallyReceived;
                        } else
                        {
                            if (partial == 0 && thisReceipt.PurchaseOrderNo !=
                            null)
                            {
                                thisReceipt.PurchaseOrderNo.Status =
                                PurchaseOrderFuelStatusEnum.Received;
                            }
                        }
                    }
                    if (thisReceipt.PurchaseOrderNo2 != null)
                    {
                        foreach (PurchaseOrderDetail item in thisReceipt.
                        PurchaseOrderNo.PurchaseOrderDetails)
                        {
                            if (!(item.
                            Received >= item.Quantity))
                            {
                                partial++;
                            }
                        }
                        if (partial > 0 && thisReceipt.PurchaseOrderNo2 != null)
                        {
                            thisReceipt.PurchaseOrderNo2.Status =
                            PurchaseOrderStatusEnum.PartiallyReceived;
                        }
                        else
                        {
                            if (partial == 0 && thisReceipt.PurchaseOrderNo2 !=
                            null)
                            {
                                thisReceipt.PurchaseOrderNo2.Status =
                                PurchaseOrderStatusEnum.Received;
                            }
                        }
                    }
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                        ApplicationException("Accounting entries not balance");
                    }
                    // Update AP Registry
                    APRegistry _apreg = ReflectionHelper.CreateObject<APRegistry>(session);
                    _apreg.GenJournalID = thisReceipt;
                    _apreg.Date = thisReceipt.EntryDate;
                    _apreg.Vendor = thisReceipt.Vendor;
                    _apreg.SourceDesc = thisReceipt.SourceType.Description;
                    _apreg.SourceNo = thisReceipt.SourceNo;
                    _apreg.DocNo = thisReceipt.InvoiceNo;
                    _apreg.Amount = thisReceipt.Total.Value;
                    _apreg.Save();
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
        private void UpdateActionState(bool inProgress) { receiveFuelAction.
            Enabled.SetItemValue("Receiving entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
