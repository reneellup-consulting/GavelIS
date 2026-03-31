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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ExtractCarryoutTransactionController : ViewController {
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private SimpleAction _ExtractCarryoutTransactionAction;
        public ExtractCarryoutTransactionController() {
            this.TargetObjectType = typeof(RequisitionWorksheet);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ExtractCarryoutTransactionActionId";
            this._ExtractCarryoutTransactionAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this._ExtractCarryoutTransactionAction.Caption = "Extract Carryout Transactions";
            this._ExtractCarryoutTransactionAction.Execute += new SimpleActionExecuteEventHandler(_ExtractCarryoutTransactionAction_Execute);
        }

        private void _ExtractCarryoutTransactionAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no Requisition Worksheet selected", "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList reqWsSelected = null;
            reqWsSelected = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            _FrmProgress = new ProgressForm("Extracting RWS transactions...", reqWsSelected.Count, "Extracting RWS transactions {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(reqWsSelected);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList argument = (IList)e.Argument;
            try
            {
                foreach (RequisitionWorksheet item in argument)
                {
                    index++;
                    _message = string.Format("Extracting OID#:{0}.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Start Processing

                    RequisitionWorksheet reqWs = session.GetObjectByKey<RequisitionWorksheet>(item.Oid);
                    // Delete all existing carryout transactions
                    for (int i = reqWs.ReqCarryoutTransactions.Count - 1; i >= 0; i--)
                    {
                        reqWs.ReqCarryoutTransactions[i].Delete();
                    }

                    CriteriaOperator criteria = null;
                    SortingCollection sorting = new SortingCollection(null);

                    // Get PO transactions
                    //var poDet = session.FindObject<PurchaseOrderDetail>(CriteriaOperator.Parse("[RequestID] = ?", item.RowID));
                    XPClassInfo poDetClassInfo = session.GetClassInfo<PurchaseOrderDetail>();
                    criteria = CriteriaOperator.Parse("[RequestID] = ?", item.RowID);
                    var poDet = session.GetObjects(poDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (PurchaseOrderDetail pdt in poDet)
                    {
                        if (pdt.PurchaseInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = pdt.PurchaseInfo ?? null;
                        rwct.SourceType = pdt.PurchaseInfo != null ? pdt.PurchaseInfo.SourceType : null;
                        rwct.LineNo = pdt.Oid;
                        rwct.Quantity = pdt.Quantity;
                        rwct.Save();
                    }
                    // Get Receipt transactions
                    XPClassInfo recDetClassInfo = session.GetClassInfo<ReceiptDetail>();
                    criteria = CriteriaOperator.Parse(string.Format("[RequisitionNo.Oid] = {0} And [ItemNo.Oid] = '{1}'", item.RequisitionInfo.Oid, item.ItemNo.Oid));
                    var recDet = session.GetObjects(recDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (ReceiptDetail rdt in recDet)
                    {
                        if (rdt.ReceiptInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = rdt.ReceiptInfo ?? null;
                        rwct.SourceType = rdt.ReceiptInfo != null ? rdt.ReceiptInfo.SourceType : null;
                        rwct.LineNo = rdt.Oid;
                        rwct.Quantity = rdt.Quantity;
                        rwct.Save();
                    }
                    // Get Transfer Order transactions
                    XPClassInfo toDetClassInfo = session.GetClassInfo<TransferOrderDetail>();
                    criteria = CriteriaOperator.Parse("[RequestID] = ?", item.RowID);
                    var toDet = session.GetObjects(toDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (TransferOrderDetail tdt in toDet)
                    {
                        if (tdt.TransferOrderInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = tdt.TransferOrderInfo ?? null;
                        rwct.SourceType = tdt.TransferOrderInfo != null ? tdt.TransferOrderInfo.SourceType : null;
                        rwct.LineNo = tdt.Oid;
                        rwct.Quantity = tdt.Quantity;
                        rwct.Save();
                    }
                    // Get SO transactions
                    XPClassInfo soDetClassInfo = session.GetClassInfo<SalesOrderDetail>();
                    criteria = CriteriaOperator.Parse("[RequestID] = ?", item.RowID);
                    var soDet = session.GetObjects(soDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (SalesOrderDetail sdt in soDet)
                    {
                        if (sdt.SalesOrderInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = sdt.SalesOrderInfo ?? null;
                        rwct.SourceType = sdt.SalesOrderInfo != null ? sdt.SalesOrderInfo.SourceType : null;
                        rwct.LineNo = sdt.Oid;
                        rwct.Quantity = sdt.Quantity;
                        rwct.Save();
                    }
                    // Get Job Order transactions
                    XPClassInfo joDetClassInfo = session.GetClassInfo<JobOrderDetail>();
                    criteria = CriteriaOperator.Parse("[RequestID] = ?", item.RowID);
                    var joDet = session.GetObjects(joDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (JobOrderDetail jdt in joDet)
                    {
                        if (jdt.JobOrderInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = jdt.JobOrderInfo ?? null;
                        rwct.SourceType = jdt.JobOrderInfo != null ? jdt.JobOrderInfo.SourceType : null;
                        rwct.LineNo = jdt.Oid;
                        rwct.Quantity = jdt.Quantity;
                        rwct.Save();
                    }
                    // Get WO transactions
                    XPClassInfo woDetClassInfo = session.GetClassInfo<WorkOrderItemDetail>();
                    criteria = CriteriaOperator.Parse("[RequestID] = ?", item.RowID);
                    var woDet = session.GetObjects(woDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (WorkOrderItemDetail wdt in woDet)
                    {
                        if (wdt.WorkOrderInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = wdt.WorkOrderInfo ?? null;
                        rwct.SourceType = wdt.WorkOrderInfo != null ? wdt.WorkOrderInfo.SourceType : null;
                        rwct.LineNo = wdt.Oid;
                        rwct.Quantity = wdt.Quantity;
                        rwct.Save();
                    }
                    // Get ECS transactions
                    XPClassInfo ecsItemDetClassInfo = session.GetClassInfo<EmployeeChargeSlipItemDetail>();
                    criteria = CriteriaOperator.Parse("[RequestID] = ?", item.RowID);
                    var ecsItemDet = session.GetObjects(ecsItemDetClassInfo, criteria, sorting, 0, false, true);
                    foreach (EmployeeChargeSlipItemDetail eit in ecsItemDet)
                    {
                        if (eit.EmployeeChargeSlipInfo == null)
                        {
                            continue;
                        }
                        ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                        rwct.ReqWorksheetId = reqWs;
                        rwct.TransactionId = eit.EmployeeChargeSlipInfo ?? null;
                        rwct.SourceType = eit.EmployeeChargeSlipInfo != null ? eit.EmployeeChargeSlipInfo.SourceType : null;
                        rwct.LineNo = eit.Oid;
                        rwct.Quantity = eit.Quantity;
                        rwct.Save();
                    }

                    // If Req. Tire Details not empty
                    if (item.RwsTireDetails.Count > 0)
                    {
                        foreach (var rwt in item.RwsTireDetails)
                        {
                            if (rwt.Replacement != null && rwt.Replacement.IcjID != null && rwt.Replacement.IcjID.SourceTypeCode == "RC" && rwt.Replacement.IcjID.GenJournalID != null)
                            {
                                Receipt rcpt = session.GetObjectByKey<Receipt>((rwt.Replacement.IcjID.GenJournalID as Receipt).Oid);
                                ReceiptDetail rcpdt = rcpt.ReceiptDetails.Where(o => o.ItemNo.No == rwt.Replacement.IcjID.ItemNo.No).LastOrDefault();
                                ReqCarryoutTransaction rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
                                rwct.ReqWorksheetId = reqWs;
                                rwct.TransactionId = rcpt ?? null;
                                rwct.SourceType = rcpt != null ? rcpt.SourceType : null;
                                rwct.LineNo = rcpdt != null ? rcpdt.Oid : 0;
                                rwct.Quantity = rcpdt.Quantity;
                                rwct.Save();
                            }
                        }
                    }
                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    System.Threading.Thread.Sleep(20);
                    CommitUpdatingSession(session);
                }
            }
            finally
            {
                if (index == argument.Count)
                {
                    e.Result = index;
                    //CommitUpdatingSession(session);
                }
                session.Dispose();
            }
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
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                , new SessionEventArgs(session));
            }
        }

        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) {
            if
            (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                SessionEventArgs(session));
            }
        }

        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e) {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                DoProgress(e.ProgressPercentage);
            }
        }

        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                "Extracting RWS transactions has been cancelled", "Cancelled",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
            } else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(_message + ": " + e.Error.Message,
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);
                } else
                {
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully Extracted");
                    ObjectSpace.Refresh();
                }
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        private void UpdateActionState(bool inProgress) {
            this._ExtractCarryoutTransactionAction.Enabled.SetItemValue("Extracting RWS transactions", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
