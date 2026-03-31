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
    public partial class PhysicalAdjustmentApplyAction : ViewController {
        private PhysicalAdjustment physicalAdjustment;
        private SimpleAction physicalAdjustmentApplyAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public PhysicalAdjustmentApplyAction() {
            this.TargetObjectType = typeof(PhysicalAdjustment);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Apply", this.GetType().Name);
            this.physicalAdjustmentApplyAction = new SimpleAction(this, actionID
            , PredefinedCategory.RecordEdit);
            this.physicalAdjustmentApplyAction.Caption = "Apply";
            this.physicalAdjustmentApplyAction.Execute += new 
            SimpleActionExecuteEventHandler(
            PhysicalAdjustmentApplyAction_Execute);
            this.physicalAdjustmentApplyAction.Executed += new EventHandler<
            ActionBaseEventArgs>(PhysicalAdjustmentApplyAction_Executed);
            this.physicalAdjustmentApplyAction.ConfirmationMessage = 
            "Do you really want to apply these entries?";
            UpdateActionState(false);
        }
        private void PhysicalAdjustmentApplyAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            physicalAdjustment = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PhysicalAdjustment;
            if (physicalAdjustment.PhysicalAdjustmentDetails.Count == 0) {
                XtraMessageBox.Show("There are no entries to apply", "Attention"
                , System.Windows.Forms.MessageBoxButtons.OK, System.Windows.
                Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = physicalAdjustment.PhysicalAdjustmentDetails.Count;
            _FrmProgress = new ProgressForm("Applying entries...", count, 
            "Processing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(physicalAdjustment);
            _FrmProgress.ShowDialog();
        }
        private void PhysicalAdjustmentApplyAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(physicalAdjustment);
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
            PhysicalAdjustment _physicalAdjustment = (PhysicalAdjustment)e.
            Argument;
            PhysicalAdjustment thisPhysicalAdjustment = session.GetObjectByKey<
            PhysicalAdjustment>(_physicalAdjustment.Oid);
            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            try {
                // Validate equity account
                if (thisPhysicalAdjustment.EquityAccount==null)
                {
                    throw new ApplicationException("Please specify a valid Equity Account");
                }
                foreach (PhysicalAdjustmentDetail item in thisPhysicalAdjustment
                .PhysicalAdjustmentDetails) {
                    _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                    _icj.GenJournalID = thisPhysicalAdjustment;
                    if (item.DifferenceStock > 0) {_icj.InQTY = Math.Abs(item.DifferenceStock);} else {
                        _icj.OutQty = Math.Abs(item.DifferenceStock);
                    }
                    _icj.Warehouse = item.Warehouse;
                    _icj.ItemNo = item.ItemNo;
                    _icj.Cost = item.Cost;
                    _icj.UOM = item.StockUOM;
                    _icj.RowID = item.RowID.ToString();
                    _icj.Save();
                    amount += item.ValueDiff;
                    int[] inds = null;
                    inds = accounts.Find("Account", item.ItemNo.InventoryAccount
                    );
                    if (inds != null && inds.Length > 0) {
                        tmpAccount = accounts[inds[0]];
                        tmpAccount.DebitAmount += item.ValueDiff > 0 ? Math.
                        Abs(item.ValueDiff) : 0;
                        tmpAccount.CreditAmount += item.ValueDiff < 0 ? Math.
                        Abs(item.ValueDiff) : 0;
                    } else {
                        tmpAccount = new TempAccount();
                        tmpAccount.Account = item.ItemNo.InventoryAccount;
                        tmpAccount.DebitAmount += item.ValueDiff > 0 ? Math.
                        Abs(item.ValueDiff) : 0;
                        tmpAccount.CreditAmount += item.ValueDiff < 0 ? Math.
                        Abs(item.ValueDiff) : 0;
                        accounts.Add(tmpAccount);
                    }
                    if (item.ItemNo.RequireSerial) {
                        if (item.PhysicalAdjustmentDetailTrackingLines.Count != 
                        Math.Abs(item.DifferenceBase)) {throw new 
                            ApplicationException(
                            "An item requires a serial no. Please specify serial nos according to quantity"
                            );}
                        foreach (PhysicalAdjustmentDetailTrackingLine iTrack in 
                        item.PhysicalAdjustmentDetailTrackingLines) {
                            ItemTrackingEntry _ite = null;
                            if (item.DifferenceBase > 0) {
                                // Check if serial no exist and if its currently removed
                                // if found and currently marked as removed change the status to available
                                _ite = (ItemTrackingEntry)session.FindObject(
                                typeof(ItemTrackingEntry), CriteriaOperator.
                                Parse("[ItemNo.No] = '" + item.ItemNo.No + 
                                "' And [SerialNo] = '" + iTrack.SerialNo + 
                                "' And [Warehouse.Code] = '" + item.Warehouse.
                                Code + "' And [Status] = 'Removed'"));
                                if (_ite != null) {
                                    _ite.Status = SerialNoStatusEnum.Available;
                                    _ite.Save();
                                } else {
                                    _ite = ReflectionHelper.CreateObject<
                                    ItemTrackingEntry>(session);
                                    _ite.IcjID = _icj;
                                    _ite.ItemNo = item.ItemNo;
                                    _ite.SerialNo = iTrack.SerialNo;
                                    _ite.Warehouse = item.Warehouse;
                                    _ite.Status = SerialNoStatusEnum.Available;
                                    _ite.Save();
                                }
                            } else {
                                // Find serial no then mark it as removed (make sure that is available)
                                _ite = (ItemTrackingEntry)session.FindObject(
                                typeof(ItemTrackingEntry), CriteriaOperator.
                                Parse("[ItemNo.No] = '" + item.ItemNo.No + 
                                "' And [SerialNo] = '" + iTrack.SerialNo + 
                                "' And [Warehouse.Code] = '" + item.Warehouse.
                                Code + "' And [Status] = 'Available'"));
                                if (_ite != null) {
                                    _ite.Status = SerialNoStatusEnum.Removed;
                                    _ite.Save();
                                } else {
                                    throw new ApplicationException(
                                    "Cannot find the serial #" + iTrack.SerialNo 
                                    + " in the list of available serial nos");
                                }
                            }
                        }
                    }
                    if (_BgWorker.CancellationPending) {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Applying entry {0} succesfull.", 
                    thisPhysicalAdjustment.PhysicalAdjustmentDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            } finally {
                if (index == thisPhysicalAdjustment.PhysicalAdjustmentDetails.
                Count) {
                    // Create Inventory Account Entry
                    foreach (TempAccount item in accounts) {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisPhysicalAdjustment;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Description = "Physical Adjustment";
                        _gjd.Approved = true;
                        _gjd.Save();
                    }
                    // Create Equity Account Entry
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = thisPhysicalAdjustment;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = thisPhysicalAdjustment.EquityAccount;
                    _gjde.DebitAmount = amount < 0 ? Math.Abs(amount) : 0;
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    _gjde.CreditAmount = amount > 0 ? Math.Abs(amount) : 0;
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Description = "Physical Adjustment";
                    _gjde.Approved = true;
                    _gjde.Save();
                    thisPhysicalAdjustment.Status = PhysicalAdjustmentStatusEnum
                    .Applied;
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
                "Apply entries operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully applied");

                    ObjectSpace.ReloadObject(physicalAdjustment);
                    ObjectSpace.Refresh();

                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { 
            physicalAdjustmentApplyAction.Enabled.SetItemValue(
            "Applying entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
