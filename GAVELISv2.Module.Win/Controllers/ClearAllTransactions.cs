using System;
using System.ComponentModel;
using System.Collections;
//using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ClearAllTransactions : ViewController
    {
        private SimpleAction clearAllTransactions;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ClearAllTransactions()
        {
            this.TargetObjectType = typeof(GenJournalHeader);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ClearAllTransactions", this.GetType().
            Name);
            this.clearAllTransactions = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.clearAllTransactions.Caption = "Delete Transactions";
            this.clearAllTransactions.Execute += new
            SimpleActionExecuteEventHandler(ClearAllTransactions_Execute);
            this.clearAllTransactions.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearAllTransactions_Executed);
            this.clearAllTransactions.ConfirmationMessage =
            "Before you continue deleting the selected transactions, make sure that you know what you are doing.\r\n" + 
            "Do you really want to delete selected transactions?";
            UpdateActionState(false);
        }
        private void ClearAllTransactions_Execute(object sender,
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
            _FrmProgress = new ProgressForm("Deleting transactions...", count,
            "Deleting transactions {0} of {1} ");
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
                GenJournalHeader hd = null;
                foreach (GenJournalHeader item in trans)
                {
                    bool delitm = false;
                    if (item.GetType()==typeof(JournalEntry))
                    {
                        throw new ApplicationException("Cannot delete Journal Entry transaction");
                    }
                    hd = session.GetObjectByKey<GenJournalHeader>(item.Oid);
                    hd.Approved=false;
                    foreach (object obj in session.CollectReferencingObjects(hd))
                    {
                        foreach (XPMemberInfo property in session.GetClassInfo(obj).PersistentProperties)
                        {
                            if (property.MemberType.IsAssignableFrom(hd.GetType()))
                            {
                                if (item.GetType() == typeof(Invoice))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(ARRegistry))
                                    {
                                        (obj as ARRegistry).Delete();
                                    }
                                    if (obj.GetType() == typeof(InvoiceDetail))
                                    {
                                        (obj as InvoiceDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(PaymentsApplied))
                                    {
                                        (obj as PaymentsApplied).Delete();
                                    }
                                    if (obj.GetType() == typeof(CreditMemo))
                                    {
                                        (obj as CreditMemo).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(CreditMemo))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(CreditMemoDetail))
                                    {
                                        (obj as CreditMemoDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(PurchaseOrder))
                                {
                                    if (obj.GetType() == typeof(PurchaseOrderDetail))
                                    {
                                        (obj as PurchaseOrderDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(RequisitionWorksheet))
                                    {
                                        (obj as RequisitionWorksheet).Delete();
                                    }
                                    if (obj.GetType() == typeof(ReqCarryoutTransaction))
                                    {
                                        (obj as ReqCarryoutTransaction).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType()==typeof(Receipt))
                                {
                                    if (obj.GetType()==typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(ReceiptDetail))
                                    {
                                        (obj as ReceiptDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(ReqCarryoutTransaction))
                                    {
                                        (obj as ReqCarryoutTransaction).Delete();
                                    }
                                    if (obj.GetType() == typeof(DebitMemo))
                                    {
                                        (obj as DebitMemo).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(APRegistry))
                                    {
                                        (obj as APRegistry).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType()==typeof(DebitMemo))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(DebitMemoDetail))
                                    {
                                        (obj as DebitMemoDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType()==typeof(Bill))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(BillDetail))
                                    {
                                        (obj as BillDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(APRegistry))
                                    {
                                        (obj as APRegistry).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(CheckVoucher))
                                {
                                    if (obj.GetType() == typeof(IncomeAndExpense))
                                    {
                                        (obj as IncomeAndExpense).Delete();
                                    }
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(CheckPayment))
                                {
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(ReceivePayment))
                                {
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(PhysicalAdjustment))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(PhysicalAdjustmentDetail))
                                    {
                                        (obj as PhysicalAdjustmentDetail).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(TransferOrder))
                                {
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(RequisitionWorksheet))
                                    {
                                        (obj as RequisitionWorksheet).Delete();
                                    }
                                    if (obj.GetType() == typeof(ReqCarryoutTransaction))
                                    {
                                        (obj as ReqCarryoutTransaction).Delete();
                                    }
                                    if (obj.GetType() == typeof(TransferOrderDetail))
                                    {
                                        (obj as TransferOrderDetail).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(StanfilcoTrip) || item.GetType() == typeof(DolefilTrip) || item.GetType() == typeof(OtherTrip))
                                {
                                    if (obj.GetType() == typeof(CargoRegistry))
                                    {
                                        (obj as CargoRegistry).Delete();
                                    }
                                    if (obj.GetType() == typeof(DriverRegistry))
                                    {
                                        (obj as DriverRegistry).Delete();
                                    }
                                    if (obj.GetType() == typeof(GensetEntry))
                                    {
                                        (obj as GensetEntry).Delete();
                                    }
                                    if (obj.GetType() == typeof(ShuntingEntry))
                                    {
                                        (obj as ShuntingEntry).Delete();
                                    }
                                    if (obj.GetType() == typeof(KDEntry))
                                    {
                                        (obj as KDEntry).Delete();
                                    }
                                    if (obj.GetType() == typeof(TruckRegistry))
                                    {
                                        (obj as TruckRegistry).Delete();
                                    }
                                    if (obj.GetType() == typeof(ReceiptFuel))
                                    {
                                        (obj as ReceiptFuel).Delete();
                                    }
                                    if (obj.GetType() == typeof(FuelRegister))
                                    {
                                        (obj as FuelRegister).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(TruckingOperationDetail))
                                    {
                                        (obj as TruckingOperationDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(TOTrip))
                                    {
                                        (obj as TOTrip).Delete();
                                    }
                                    if (obj.GetType() == typeof(TOTrailer))
                                    {
                                        (obj as TOTrailer).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(ShuntingEntry))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(ShuntingEntryDetail))
                                    {
                                        (obj as ShuntingEntryDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(TruckingOperationDetail))
                                    {
                                        (obj as TruckingOperationDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(TOShunting))
                                    {
                                        (obj as TOShunting).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(GensetEntry))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(GensetUsageDetail))
                                    {
                                        (obj as GensetUsageDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(KDEntry))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(ReceiptFuel))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(ReceiptFuelDetail))
                                    {
                                        (obj as ReceiptFuelDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(APRegistry))
                                    {
                                        (obj as APRegistry).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(JobOrder))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(JobOrderDetail))
                                    {
                                        (obj as JobOrderDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    if (obj.GetType() == typeof(APRegistry))
                                    {
                                        (obj as APRegistry).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(WorkOrder))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(InventoryControlJournal))
                                    {
                                        (obj as InventoryControlJournal).Delete();
                                    }
                                    if (obj.GetType() == typeof(ARRegistry))
                                    {
                                        (obj as ARRegistry).Delete();
                                    }
                                    if (obj.GetType() == typeof(WorkOrderItemDetail))
                                    {
                                        (obj as WorkOrderItemDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(EmployeeChargeSlip))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(EmployeeChargeSlipExpenseDetail))
                                    {
                                        (obj as EmployeeChargeSlipExpenseDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(IncomeAndExpense02))
                                    {
                                        (obj as IncomeAndExpense02).Delete();
                                    }
                                    delitm = true;
                                }
                                if (item.GetType() == typeof(DriverPayrollBatch))
                                {
                                    if (obj.GetType() == typeof(GenJournalDetail))
                                    {
                                        (obj as GenJournalDetail).Delete();
                                    }
                                    if (obj.GetType() == typeof(DriverPayrollTrip))
                                    {
                                        (obj as DriverPayrollTrip).Delete();
                                    }
                                    if (obj.GetType() == typeof(PayrollDeductionOther))
                                    {
                                        (obj as PayrollDeductionOther).Delete();
                                    }
                                    if (obj.GetType() == typeof(DriverPayroll))
                                    {
                                        (obj as DriverPayroll).Delete();
                                    }
                                    delitm = true;
                                }
                                if (delitm)
                                {
                                    property.SetValue(obj, null);
                                }
                            }
                        }
                    }
                    if (delitm)
                    {
                        hd.Delete();
                    }
                    else
                    {
                        throw new ApplicationException("Problems occured when trying to delete the transaction.\r\n" +
                        "Please review the dependencies to locate the problems");
                    }
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Deleting transaction {0} succesfull.",
                    trans.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            }
            finally
            {
                if (index == trans.Count)
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void ClearAllTransactions_Executed(object sender, ActionBaseEventArgs e)
        {
            //throw new NotImplementedException();
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
                    "Deleting transaction operation has been cancelled", "Cancelled",
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
                    //ObjectSpace.ReloadObject(invoice);
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
            clearAllTransactions.
                Enabled.SetItemValue("Deleting transactions", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
