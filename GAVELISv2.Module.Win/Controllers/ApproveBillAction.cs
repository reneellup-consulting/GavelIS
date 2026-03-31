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
    public partial class ApproveBillAction : ViewController {
        private Bill bill;
        private SimpleAction billAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ApproveBillAction() {
            this.TargetObjectType = typeof(Bill);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Approve", this.GetType().Name);
            this.billAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.billAction.Caption = "Approve";
            this.billAction.Execute += new SimpleActionExecuteEventHandler(
            ApproveBillAction_Execute);
            this.billAction.Executed += new EventHandler<ActionBaseEventArgs>(
            ApproveBillAction_Executed);
            this.billAction.ConfirmationMessage = 
            "Do you really want to approve these entries?";
            UpdateActionState(false);
        }
        private void ApproveBillAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            bill = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as Bill;
            ObjectSpace.CommitChanges();
            if (bill.BillDetails.Count == 0) {
                XtraMessageBox.Show("There are no entries to approve", 
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = bill.BillDetails.Count;
            _FrmProgress = new ProgressForm("Approving entries...", count, 
            "Approving entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(bill);
            _FrmProgress.ShowDialog();
        }
        private void ApproveBillAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(bill);
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
            UnitOfWork session = CreateUpdatingSession();
            Bill _bill = (Bill)e.Argument;
            Bill thisBill = session.GetObjectByKey<Bill>(_bill.Oid);
            decimal amount = 0;
            try {
                // Validate Vendor Accounts Payable
                if (thisBill.Vendor == null) {throw new ApplicationException(
                    "Must specify a vendor");} else {
                    if (thisBill.Vendor.Account == null) {throw new 
                        ApplicationException(
                        "Accounts Payable account must be specified in the chosen vendor card"
                        );}
                }
                foreach (BillDetail item in thisBill.BillDetails) {
                    GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjd.GenJournalID = thisBill;
                    _gjd.Account = item.Expense;
                    _gjd.DebitAmount = item.Amount;
                    _gjd.Description = "Business Expense";
                    _gjd.ExpenseType = item.ExpenseType != null ? item.
                    ExpenseType : null;
                    _gjd.SubAccountNo = thisBill.Vendor;
                    _gjd.SubAccountType = thisBill.Vendor.ContactType;
                    _gjd.Approved = true;
                    _gjd.Save();
                    amount += item.Amount;
                    if (_BgWorker.CancellationPending) {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Approving entry {0} succesfull.", 
                    thisBill.ReceiptDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            } finally {
                if (index == thisBill.BillDetails.Count) {
                    // Create Accounts Payable
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisBill;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisBill.Vendor.Account;
                    _gjde.CreditAmount = Math.Abs(amount);
                    _gjde.Description = "Business Expense";
                    _gjde.SubAccountNo = thisBill.Vendor;
                    _gjde.SubAccountType = thisBill.Vendor.ContactType;
                    _gjde.Approved = true;
                    _gjde.Save();
                    thisBill.Status = BillStatusEnum.Approved;
                    e.Result = index;
                    // Update AP Registry
                    APRegistry _apreg = ReflectionHelper.CreateObject<APRegistry>(session);
                    _apreg.GenJournalID = thisBill;
                    _apreg.Date = thisBill.EntryDate;
                    _apreg.Vendor = thisBill.Vendor;
                    _apreg.SourceDesc = thisBill.SourceType.Description;
                    _apreg.SourceNo = thisBill.SourceNo;
                    _apreg.DocNo = thisBill.SourceNo;
                    _apreg.Amount = thisBill.Total.Value;
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
                "Approving entries operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully approved");

                    ObjectSpace.ReloadObject(bill);
                    ObjectSpace.Refresh();

                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { billAction.Enabled.
            SetItemValue("Approving entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
