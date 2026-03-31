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
    public partial class ConfirmJobOrderAction : ViewController {
        private JobOrder jobOrder;
        private SimpleAction confirmJobOrderAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ConfirmJobOrderAction() {
            this.TargetObjectType = typeof(JobOrder);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Confirm", this.GetType().Name);
            this.confirmJobOrderAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.confirmJobOrderAction.Caption = "Confirm";
            this.confirmJobOrderAction.Execute += new 
            SimpleActionExecuteEventHandler(ConfirmJobOrderAction_Execute);
            this.confirmJobOrderAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ConfirmJobOrderAction_Executed);
            this.confirmJobOrderAction.ConfirmationMessage = 
            "Do you really want to confirm these jobs?";
            UpdateActionState(false);
        }
        private void ConfirmJobOrderAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            jobOrder = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as JobOrder;
            ObjectSpace.CommitChanges();
            if (jobOrder.JobOrderDetails.Count == 0) {
                XtraMessageBox.Show("There are no jobs to confirm", "Attention", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
                return;
            }
            var count = jobOrder.JobOrderDetails.Count;
            _FrmProgress = new ProgressForm("Confirming Jobs...", count, 
            "Confirming jobs {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(jobOrder);
            _FrmProgress.ShowDialog();
        }
        private void ConfirmJobOrderAction_Executed(object sender, 
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
            JobOrder _jobOrder = (JobOrder)e.Argument;
            //if (_receipt.Oid==-1)
            //{
            //    throw new
            //            ApplicationException(
            //            "Please save this transaction first before receiving"
            //            );
            //}
            JobOrder thisJobOrder = session.GetObjectByKey<JobOrder>(_jobOrder.
            Oid);
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            int partial = 0;
            try {
                // Validate Vendor Accounts Payable
                if (thisJobOrder.Vendor == null) {throw new ApplicationException
                    ("Must specify a vendor");} else {
                    if (thisJobOrder.Vendor.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );}
                }
                if (string.IsNullOrEmpty(thisJobOrder.InvoiceNo))
                {
                    throw new ApplicationException("Cannot proceed if Invoice No is not specified");
                }
                foreach (JobOrderDetail item in thisJobOrder.JobOrderDetails) {
                    //_icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    //>(session);
                    //_icj.GenJournalID = thisJobOrder;
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
                        item.PODetailID.skipAuto = true;
                        item.PODetailID.Received += Math.Abs(item.Quantity);
                        //if (!(item.PODetailID.Received >= item.PODetailID.Quantity)) { partial++; }
                        decimal dt = item.PODetailID.PurchaseInfo.PurchaseOrderDetails.Sum(o => o.RemainingQty);
                        if (dt == 0)
                        {
                            item.PODetailID.PurchaseInfo.Status = PurchaseOrderStatusEnum.Received;
                        }
                        else
                        {
                            item.PODetailID.PurchaseInfo.Status = PurchaseOrderStatusEnum.PartiallyReceived;
                        }
                    }
                    amount += item.Total;
                    int[] inds = null;
                    inds = accounts.Find("Account", item.ItemNo.COGSAccount);
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item
                        .Total) : 0;
                        tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(
                        item.Total) : 0;
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.COGSAccount;
                        tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(item
                        .Total) : 0;
                        tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(
                        item.Total) : 0;
                        accounts.Add(tmpAccount);
                    }
                    if (_BgWorker.CancellationPending) {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Confirming entry {0} succesfull.", 
                    thisJobOrder.JobOrderDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            } finally {
                if (index == thisJobOrder.JobOrderDetails.Count) {
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts) {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisJobOrder;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Receipt of Services";
                        _gjd.SubAccountNo = thisJobOrder.Vendor;
                        _gjd.SubAccountType = thisJobOrder.Vendor.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }
                    // If has discount
                    Account discountAcct = null;
                    if (thisJobOrder.Discount > 0) {
                        if (thisJobOrder.DiscountAcct == null) {
                            if (thisJobOrder.CompanyInfo.PurchaseDiscountAcct == 
                            null) {throw new ApplicationException(
                                "There is no default Purchase Discount account provided in the company information card." 
                                + 
                                " Else please provide discount account in the field provided."
                                );} else {
                                discountAcct = thisJobOrder.CompanyInfo.
                                PurchaseDiscountAcct;
                            }
                        } else {
                            discountAcct = thisJobOrder.DiscountAcct;
                        }
                        // Credit Purchase Discount Account
                        GenJournalDetail _gjde4 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde4.GenJournalID = thisJobOrder;
                        _gjde4.GenJournalID.Approved = true;
                        _gjde4.Account = discountAcct;
                        _gjde4.CreditAmount = Math.Abs(thisJobOrder.Discount);
                        _gjde4.Description = "Receipt of Services";
                        _gjde4.SubAccountNo = thisJobOrder.Vendor;
                        _gjde4.SubAccountType = thisJobOrder.Vendor.ContactType;
                        _gjde4.Approved = true;
                        debitAmount = debitAmount + _gjde4.DebitAmount;
                        creditAmount = creditAmount + _gjde4.CreditAmount;
                        _gjde4.Save();
                    }
                    // Create Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisJobOrder;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisJobOrder.Vendor.Account;
                    _gjde.CreditAmount = Math.Abs(thisJobOrder.Total.Value);
                    _gjde.Description = "Receipt of Services";
                    _gjde.SubAccountNo = thisJobOrder.Vendor;
                    _gjde.SubAccountType = thisJobOrder.Vendor.ContactType;
                    _gjde.Approved = true;
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Save();
                    thisJobOrder.Status = JobOrderStatusEnum.Confirmed;
                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                        ApplicationException("Accounting entries not balance");
                    }
                    // Update AP Registry
                    APRegistry _apreg = ReflectionHelper.CreateObject<APRegistry>(session);
                    if (thisJobOrder.PurchaseOrderNo != null)
                    {
                        foreach (PurchaseOrderDetail item in thisJobOrder.
                        PurchaseOrderNo.PurchaseOrderDetails)
                        {
                            if (!(item.
                                Received >= item.Quantity)) { partial++; }
                        }
                        if (partial > 0 && thisJobOrder.PurchaseOrderNo != null)
                        {
                            thisJobOrder.PurchaseOrderNo.Status =
                               PurchaseOrderStatusEnum.PartiallyReceived;
                        }
                        else
                        {
                            if (partial == 0 && thisJobOrder.PurchaseOrderNo !=
                            null)
                            {
                                thisJobOrder.PurchaseOrderNo.Status =
                                    PurchaseOrderStatusEnum.Received;
                            }
                        }
                    }
                    _apreg.GenJournalID = thisJobOrder;
                    _apreg.Date = thisJobOrder.EntryDate;
                    _apreg.Vendor = thisJobOrder.Vendor;
                    _apreg.SourceDesc = thisJobOrder.SourceType.Description;
                    _apreg.SourceNo = thisJobOrder.SourceNo;
                    _apreg.DocNo = thisJobOrder.InvoiceNo;
                    _apreg.Amount = thisJobOrder.Total.Value;
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
                "Confirming jobs operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully confirmed");
                    ObjectSpace.ReloadObject(jobOrder);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { confirmJobOrderAction.
            Enabled.SetItemValue("Confirming Jobs", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
