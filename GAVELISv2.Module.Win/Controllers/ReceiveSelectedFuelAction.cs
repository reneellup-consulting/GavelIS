using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiveSelectedFuelAction : ViewController
    {
        private SimpleAction receiveFuelAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReceiveSelectedFuelAction()
        {
            this.TargetObjectType = typeof(ReceiptFuel);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ReceiveSelectedFuel", this.GetType().
            Name);
            this.receiveFuelAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.receiveFuelAction.Caption = "Receive Selected";
            this.receiveFuelAction.Execute += new
            SimpleActionExecuteEventHandler(ReceiveSelectedFuelAction_Execute);
            this.receiveFuelAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReceiveSelectedFuelAction_Executed);
            this.receiveFuelAction.ConfirmationMessage =
            "Do you really want to receive the selected transactions?";
            UpdateActionState(false);
        }
        private void ReceiveSelectedFuelAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no transactions selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList genHeaders = null;
            genHeaders = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = genHeaders.Count;
            _FrmProgress = new ProgressForm("Receiving transactions...", count,
            "Receiving transactions {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(genHeaders);
            _FrmProgress.ShowDialog();
        }
        private void ReceiveSelectedFuelAction_Executed(object sender,
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
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                foreach (ReceiptFuel item in trans)
                {
                    index++;

                    #region Algorithms here...

                    decimal creditAmount = 0;
                    decimal debitAmount = 0;
                    ReceiptFuel thisReceipt = session.GetObjectByKey<ReceiptFuel>(
                    item.Oid);

                    TempAccountCollection accounts = new TempAccountCollection();
                    TempAccount tmpAccount;
                    decimal amount = 0;
                    int partial = 0;

                    // Validate Vendor Accounts Payable
                    if (thisReceipt.Vendor == null)
                    {
                        throw new ApplicationException(
                        "Must specify a vendor");
                    }
                    else
                    {
                        if (thisReceipt.Vendor.Account == null)
                        {
                            throw new
                            ApplicationException(
                            "Accounts Payable account must be specified in the chosen vendor card"
                            );
                        }
                    }

                    int index2 = 0;
                    foreach (ReceiptFuelDetail iDet in thisReceipt.
                        ReceiptFuelDetails)
                    {
                        index2++;
                        // Save last direct cost to item card
                        iDet.ItemNo.Cost = iDet.BaseCost;
                        if (iDet.PODetailID != null)
                        {
                            iDet.PODetailID.Received += Math.Abs(iDet.Quantity);
                            //if (!(item.PODetailID.Received >= item.PODetailID.Quantity)) { partial++; }
                        }
                        amount += iDet.Total;
                        int[] inds = null;
                        inds = accounts.Find("Account", ((FuelItem)iDet.ItemNo).ExpenseAccount);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(iDet
                            .Total) : 0;
                            tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(
                            iDet.Total) : 0;
                        }
                        else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = ((FuelItem)iDet.ItemNo).ExpenseAccount;
                            tmpAccount.DebitAmount += item.Total > 0 ? Math.Abs(iDet
                            .Total) : 0;
                            tmpAccount.CreditAmount += item.Total < 0 ? Math.Abs(
                            iDet.Total) : 0;
                            accounts.Add(tmpAccount);
                        }
                    }

                    if (index2 == thisReceipt.ReceiptFuelDetails.Count)
                    {
                        // Create Expense Account Entry
                        foreach (TempAccount acc in accounts)
                        {
                            GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                            GenJournalDetail>(session);
                            _gjd.GenJournalID = thisReceipt;
                            _gjd.Account = acc.Account;
                            _gjd.DebitAmount = acc.DebitAmount;
                            debitAmount = debitAmount + _gjd.DebitAmount;
                            _gjd.CreditAmount = acc.CreditAmount;
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
                            foreach (PurchaseOrderDetail pDet in thisReceipt.
                            PurchaseOrderNo.PurchaseOrderDetails)
                            {
                                if (!(pDet.
                                Received >= pDet.Quantity))
                                {
                                    partial++;
                                }
                            }
                            if (partial > 0 && thisReceipt.PurchaseOrderNo != null)
                            {
                                thisReceipt.PurchaseOrderNo2.Status =
                                PurchaseOrderStatusEnum.PartiallyReceived;
                            }
                            else
                            {
                                if (partial == 0 && thisReceipt.PurchaseOrderNo !=
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
                    }

                    thisReceipt.Save();
                    CommitUpdatingSession(session);

                    #endregion
                    
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Receiving entry {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.Count)
                {
                    e.Result = index;
                }
                session.Dispose();
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
                    "Receiving selected transactions has been cancelled", "Cancelled",
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
                    " has been successfully received");

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
            receiveFuelAction.
                Enabled.SetItemValue("Receiving transactions", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
