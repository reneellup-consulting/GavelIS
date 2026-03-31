using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using System.Collections;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo;
using DevExpress.XtraEditors;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class IssueTireController : WinDetailViewController {
        private RequisitionWorksheet _Rw;
        public RwsTireDetail rwsTireDetail;
        private PopupWindowShowAction getReceivedTireAction;
        public PopupWindowShowAction issueTireAction;
        public PopupWindowShowAction issueFromStock;
        public SimpleAction removeIssueAction;
        public SimpleAction releaseIssueAction;
        private ProgressForm _FrmProgress;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        public IssueTireController() {
            this.TargetObjectType = typeof(RwsTireDetail);
            this.TargetViewType = ViewType.ListView;
            //this.TargetViewId = "RequisitionWorksheet_RwsTireDetails_ListView";
            this.TargetViewNesting = Nesting.Any;

            // Get Received Tires
            this.getReceivedTireAction = new PopupWindowShowAction(this, "GetReceivedTireAction", PredefinedCategory.RecordEdit);
            this.getReceivedTireAction.Caption = "Get Received Tires";
            this.getReceivedTireAction.SelectionDependencyType = SelectionDependencyType.Independent;
            this.getReceivedTireAction.TargetObjectsCriteria = "[ReleaseStatus] = 'Open'";
            this.getReceivedTireAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(getReceivedTireAction_CustomizePopupWindowParams);
            this.getReceivedTireAction.Execute += new PopupWindowShowActionExecuteEventHandler(getReceivedTireAction_Execute);

            // Issue from Receipt
            string actionID = "IssueTireActionId";
            this.issueTireAction = new PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit);
            this.issueTireAction.Caption = "Issue from Receipt";
            this.issueTireAction.TargetObjectsCriteria = "[ReleaseStatus] = 'Open'";
            this.issueTireAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(issueTireAction_CustomizePopupWindowParams);
            this.issueTireAction.Execute += new PopupWindowShowActionExecuteEventHandler(issueTireAction_Execute);

            // Issue from Stock
            string issueFromStockID = "IssueFromStockID";
            this.issueFromStock = new PopupWindowShowAction(this, issueFromStockID, PredefinedCategory.RecordEdit);
            this.issueFromStock.Caption = "Issue from Stock";
            this.issueFromStock.TargetObjectsCriteria = "[ReleaseStatus] = 'Open'";
            this.issueFromStock.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(issueFromStock_CustomizePopupWindowParams);
            this.issueFromStock.Execute += new PopupWindowShowActionExecuteEventHandler(issueFromStock_Execute);

            // Remove Issue
            this.removeIssueAction = new SimpleAction(this, "RemoveIssueActionId", PredefinedCategory.RecordEdit);
            this.removeIssueAction.Caption = "Remove Issue";
            this.removeIssueAction.TargetObjectsCriteria = "[ReleaseStatus] = 'Open'";
            this.removeIssueAction.Execute += new SimpleActionExecuteEventHandler(removeIssueAction_Execute);

            // Release Issue
            this.releaseIssueAction = new SimpleAction(this, "ReleaseIssueActionId", PredefinedCategory.RecordEdit);
            this.releaseIssueAction.Caption = "Release Issue";
            this.releaseIssueAction.TargetObjectsCriteria = "[ReleaseStatus] = 'Open'";
            this.releaseIssueAction.Execute += new SimpleActionExecuteEventHandler(releaseIssueAction_Execute);
        }

        void issueFromStock_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            ItemTrackingEntry ite = e.PopupWindow.View.CurrentObject as ItemTrackingEntry;
            RwsTireDetail rtd = e.CurrentObject as RwsTireDetail;
            if (rtd.Replacement != null)
            {
                XtraMessageBox.Show(
                "This tire request has already been issued. Remove previous issue first.", "Already Issued",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
                return;
            }
            ItemTrackingEntry sIte = rtd.Session.GetObjectByKey<ItemTrackingEntry>(ite.Oid);
            sIte.Status = SerialNoStatusEnum.Reserved;
            sIte.IssueToCC = rtd.ReqWorksheetId.CostCenter;
            sIte.OriginSourceId = sIte.IcjID.GenJournalID;
            sIte.RequisitionId = rtd.ReqWorksheetId.RequisitionInfo;
            sIte.ReqWorksheetId = rtd.ReqWorksheetId;
            rtd.IssueDate = DateTime.Now;
            rtd.FromWarehouse = sIte.Warehouse;
            rtd.InvControlId = sIte.IcjID ?? null;
            rtd.Replacement = sIte;
            rtd.SerialNo = sIte.SerialNo;
            rtd.NewTireItem = sIte.ItemNo as TireItem;
            rtd.InvoiceNo = sIte.IcjID.GenJournalID.GetType() == typeof(Receipt) ? (sIte.IcjID.GenJournalID as Receipt).InvoiceNo : string.Empty;
            rtd.Cost = sIte.IcjID.Cost;
            sIte.Save();
            rtd.Save();
            this.View.ObjectSpace.CommitChanges();
        }

        void issueFromStock_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            //PropertyCollectionSource collectionSource = ((DevExpress.ExpressApp.ListView)this.View).CollectionSource as PropertyCollectionSource;
            //_stocks = collectionSource.MasterObject as InventoryControlJournal;
            //rwsTireDetail = ((DevExpress.ExpressApp.ListView)this.View).
            //CurrentObject as RwsTireDetail;
            //RequisitionWorksheet rw = rwsTireDetail.Session.GetObjectByKey<RequisitionWorksheet>(rwsTireDetail.ReqWorksheetId.Oid);
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).Session.ObjectLayer);
            //XPCollection<InventoryControlJournal> _stocks = new XPCollection<InventoryControlJournal>(session);
            //ItemCategory tCat = ObjectSpace.FindObject<ItemCategory>(BinaryOperator.Parse("[Code]='004381'"));
            //var rwct = from trns in _stocks
            //           where trns.CategoryCode == "004381" && trns.SourceTypeCode == "RC"
            //           select trns;
            XPClassInfo objClass1 = session.GetClassInfo<InventoryControlJournal>(); ;
            CriteriaOperator criteria1 = CriteriaOperator.Parse("[ItemNo.Category.Code] = '004381' And [SourceTypeCode] = 'RC'");
            SortingCollection sorting1 = new SortingCollection(null);
            var rwct = session.GetObjects(objClass1, criteria1, sorting1, 0, false, false);
            if (rwct != null && rwct.Count > 0)
            {
                ArrayList keysToShow1 = new ArrayList();
                //InventoryControlJournal icj = null;
                XPClassInfo objClass = null;
                CriteriaOperator criteria = null;
                SortingCollection sorting = new SortingCollection(null);
                foreach (InventoryControlJournal cot in rwct)
                {
                    //Receipt rcpt = cot.TransactionId as Receipt;
                    // Filter ItemNo, SourceNo [Gen Journal ID.Source No] = ? And [Item No.Item Type] = 'Tire Item'
                    //icj = _Rw.Session.FindObject<InventoryControlJournal>(CriteriaOperator.Parse("[GenJournalID.SourceNo] = ? And [ItemNo.ItemType] = 'TireItem'", cot.TransactionId.SourceNo));
                    objClass = session.GetClassInfo<ItemTrackingEntry>();
                    criteria = CriteriaOperator.Parse(
                    string.Format("[IcjID.Oid]= '{0}' And [Status] = 'Available'", cot.Oid));
                    ICollection cols = session.GetObjects(objClass, criteria, sorting, 0, false, false);
                    if (cols != null && cols.Count > 0)
                    {
                        foreach (ItemTrackingEntry col in cols)
                        {
                            keysToShow1.Add(col.Oid);
                        }
                    }
                }
                //if (keysToShow1.Count > 0)
                //{
                //    string viewId = "ItemTrackingEntry_ListView_UnissuedTires";
                //    CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ItemTrackingEntry), viewId);
                //    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ItemTrackingEntry)), keysToShow1);
                //    e.View = Application.CreateListView(viewId, collectionSource1,
                //    true);
                //}
                string viewId = "ItemTrackingEntry_ListView_UnissuedTires";
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ItemTrackingEntry), viewId);
                collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ItemTrackingEntry)), keysToShow1);
                e.View = Application.CreateListView(viewId, collectionSource1,
                true);
            }
            else
            {
                throw new UserFriendlyException("Has no carried out receipt transanction");
            }
        }

        void getReceivedTireAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            int index = 0;
            _FrmProgress = new ProgressForm("Retreiving tire...", e.PopupWindow.View.SelectedObjects.Count,
            "Retreiving tire {0} of {1} ");
            _FrmProgress.btnCancel.Enabled = false;
            _FrmProgress.Show();
            foreach (ItemTrackingEntry item in e.PopupWindow.View.SelectedObjects)
            {
                index++;
                _FrmProgress.DoProgress();
                _FrmProgress.Refresh();
                //ItemTrackingEntry ite = e.PopupWindow.View.CurrentObject as ItemTrackingEntry;
                RwsTireDetail rtd = ReflectionHelper.CreateObject<RwsTireDetail>(((ObjectSpace)this.View.ObjectSpace).Session);
                rtd.ReqWorksheetId = _Rw;
                if (rtd.Replacement != null)
                {
                    XtraMessageBox.Show(
                    "This tire request has already been issued. Remove previous issue first.", "Already Issued",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
                    return;
                }
                ItemTrackingEntry sIte = rtd.Session.GetObjectByKey<ItemTrackingEntry>(item.Oid);
                sIte.Status = SerialNoStatusEnum.Reserved;
                sIte.IssueToCC = rtd.ReqWorksheetId.CostCenter;
                sIte.OriginSourceId = sIte.IcjID.GenJournalID;
                sIte.RequisitionId = rtd.ReqWorksheetId.RequisitionInfo;
                sIte.ReqWorksheetId = rtd.ReqWorksheetId;
                rtd.IssueDate = DateTime.Now;
                rtd.InvControlId = sIte.IcjID ?? null;
                // Get ReceiptLineId
                ReceiptDetail rcptd = rtd.Session.FindObject<ReceiptDetail>(BinaryOperator.Parse("[GenJournalID]=?",sIte.IcjID.GenJournalID));
                rtd.FromWarehouse = sIte.Warehouse;
                rtd.Replacement = sIte;
                rtd.SerialNo = sIte.SerialNo;
                rtd.NewTireItem = sIte.ItemNo as TireItem;
                rtd.InvoiceNo = sIte.IcjID.GenJournalID.GetType() == typeof(Receipt) ? (sIte.IcjID.GenJournalID as Receipt).InvoiceNo : string.Empty;
                rtd.Cost = sIte.IcjID.Cost;
                sIte.Save();
                rtd.Save();
            }
            _FrmProgress.Close();
            this.View.ObjectSpace.CommitChanges();
        }

        void getReceivedTireAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            PropertyCollectionSource collectionSource = ((DevExpress.ExpressApp.ListView)this.View).CollectionSource as PropertyCollectionSource;
            _Rw = collectionSource.MasterObject as RequisitionWorksheet;
            //rwsTireDetail = ((DevExpress.ExpressApp.ListView)this.View).
            //CurrentObject as RwsTireDetail;
            //RequisitionWorksheet rw = rwsTireDetail.Session.GetObjectByKey<RequisitionWorksheet>(rwsTireDetail.ReqWorksheetId.Oid);
            var rwct = from trns in _Rw.ReqCarryoutTransactions
                       where trns.SourceType.Code == "RC"
                       select trns;
            if (rwct != null && rwct.Count() > 0)
            {
                ArrayList keysToShow1 = new ArrayList();
                InventoryControlJournal icj = null;
                XPClassInfo objClass = null;
                CriteriaOperator criteria = null;
                SortingCollection sorting = new SortingCollection(null);
                foreach (ReqCarryoutTransaction cot in rwct)
                {
                    //Receipt rcpt = cot.TransactionId as Receipt;
                    // Filter ItemNo, SourceNo [Gen Journal ID.Source No] = ? And [Item No.Item Type] = 'Tire Item'
                    icj = _Rw.Session.FindObject<InventoryControlJournal>(CriteriaOperator.Parse("[GenJournalID.SourceNo] = ? And [ItemNo.ItemType] = 'TireItem'", cot.TransactionId.SourceNo));
                    objClass = _Rw.Session.GetClassInfo<ItemTrackingEntry>();
                    criteria = CriteriaOperator.Parse(
                    string.Format("[IcjID.Oid]= '{0}' And [Status] = 'Available'", icj.Oid));
                    ICollection cols = _Rw.Session.GetObjects(objClass, criteria, sorting, 0, false, false);
                    if (cols != null && cols.Count > 0)
                    {
                        foreach (ItemTrackingEntry col in cols)
                        {
                            keysToShow1.Add(col.Oid);
                        }
                    }
                }
                //if (keysToShow1.Count > 0)
                //{
                //    string viewId = "ItemTrackingEntry_ListView_UnissuedTires";
                //    CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ItemTrackingEntry), viewId);
                //    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ItemTrackingEntry)), keysToShow1);
                //    e.View = Application.CreateListView(viewId, collectionSource1,
                //    true);
                //}
                string viewId = "ItemTrackingEntry_ListView_UnissuedTires";
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ItemTrackingEntry), viewId);
                collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ItemTrackingEntry)), keysToShow1);
                e.View = Application.CreateListView(viewId, collectionSource1,
                true);
            }
            else
            {
                throw new UserFriendlyException("Has no carried out receipt transanction");
            }
        }

        #region Release Issue

        void releaseIssueAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no details selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            if (XtraMessageBox.Show("Do you really want to release the selected tire issue?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            IList dList = null;
            dList = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            int count = dList.Count;
            _FrmProgress = new ProgressForm("Releasing tire...", count,
            "Releasing tire {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(dList);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList dets = (IList)e.Argument;
            try
            {
                foreach (RwsTireDetail item in dets)
                {
                    index++;
                    _message = string.Format("Releasing tire {0} succesfull.",
                    dets.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    RwsTireDetail sRtd = session.GetObjectByKey<RwsTireDetail>(item.Oid);
                    if (sRtd != null && sRtd.TireIssueId == null)
                    {
                        Tire sTire = ReflectionHelper.CreateObject<Tire>(session);
                        sTire.TireReqId = sRtd;
                        sTire.TireItem = sRtd.NewTireItem;
                        //sTire.TireItemClass = sRtd.NewTireItem.TireItemClass;
                        //sTire.TireStatus = TirePhysicalStatusEnum.Available;
                        if (sRtd.Replacement.IcjID.GenJournalID.GetType() == typeof(Receipt))
                        {
                            sTire.PurchaseFrom = (sRtd.Replacement.IcjID.GenJournalID as Receipt).Vendor;
                            sTire.PurchaseDate = sRtd.Replacement.IcjID.GenJournalID.EntryDate;
                        }
                        sTire.SerialNo = sRtd.SerialNo;
                        sTire.InvoiceNo = sRtd.InvoiceNo;
                        sTire.Cost = sRtd.Cost;
                        sTire.Save();
                        sRtd.Replacement.Status = SerialNoStatusEnum.Used;
                        sRtd.TireIssueId = sTire;
                        sRtd.Save();
                    }
                    // Deduct Inventory
                    InventoryControlJournal _icj = session.FindObject<InventoryControlJournal>(
                                        new BinaryOperator("RowID", item.RowID.ToString()));
                    Requisition _req = session.GetObjectByKey<Requisition>(item.ReqWorksheetId.GenJournalID.Oid);
                    RwsTireDetail _ortwd = session.GetObjectByKey<RwsTireDetail>(item.Oid);
                    if (_icj != null)
                    {
                        _icj.GenJournalID = _req;
                        _icj.DateIssued = _ortwd.IssueDate;
                        _icj.OutQty = 1m;
                        _icj.Warehouse = _ortwd.FromWarehouse;
                        _icj.ItemNo = _ortwd.ReqWorksheetId.ItemNo;
                        _icj.Cost = _ortwd.ReqWorksheetId.Cost;
                        _icj.Price = 0m;
                        _icj.UOM = _ortwd.ReqWorksheetId.UOM;
                        _icj.RowID = _ortwd.RowID.ToString();
                        _icj.CostCenter = _ortwd.ReqWorksheetId.CostCenter ?? null;
                        _icj.RequestedBy = _ortwd.ReqWorksheetId.RequisitionInfo.RequestedBy ?? null;
                        _icj.Save();
                    }
                    else
                    {
                        _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                        _icj.GenJournalID = _req;
                        _icj.DateIssued = _ortwd.IssueDate;
                        _icj.OutQty = 1m;
                        _icj.Warehouse = _ortwd.FromWarehouse;
                        _icj.ItemNo = _ortwd.ReqWorksheetId.ItemNo;
                        _icj.Cost = _ortwd.ReqWorksheetId.Cost;
                        _icj.Price = 0m;
                        _icj.UOM = _ortwd.ReqWorksheetId.UOM;
                        _icj.RowID = _ortwd.RowID.ToString();
                        _icj.CostCenter = _ortwd.ReqWorksheetId.CostCenter ?? null;
                        _icj.RequestedBy = _ortwd.ReqWorksheetId.RequisitionInfo.RequestedBy ?? null;
                        _icj.Save();
                    }
                    // Offset Expense
                    //IncomeAndExpense02 incExp = null;
                    //IncomeAndExpense02 incExpOffset = null;
                    //incExp = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    System.Threading.Thread.Sleep(20);
                    //CommitUpdatingSession(session);
                }
            } finally
            {
                if (index == dets.Count)
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
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
                "Releasing tire has been cancelled", "Cancelled",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
            } else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);
                } else
                {
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully released");
                    ObjectSpace.Refresh();
                }
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        private void UpdateActionState(bool inProgress) {
            this.releaseIssueAction.Enabled.SetItemValue("Releasing tire", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

        #endregion

        void removeIssueAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            int index = 0;
            _FrmProgress = new ProgressForm("Removing issue...", e.SelectedObjects.Count,
            "Removing issue {0} of {1} ");
            _FrmProgress.btnCancel.Enabled = false;
            _FrmProgress.Show();
            foreach (RwsTireDetail item in e.SelectedObjects)
            {
                // Algorithms here....
                index++;
                _FrmProgress.DoProgress();
                _FrmProgress.Refresh();
                RwsTireDetail rtd = item as RwsTireDetail;
                if (rtd.TireIssueId != null)
                {
                    XtraMessageBox.Show(
                    "Cannot remove issuance because tire request was already released.", "Cannot Remove",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
                    return;
                }
                if (rtd.Replacement != null) // && XtraMessageBox.Show("Do you really want to remove this tire issued?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    rtd.Cost = 0m;
                    rtd.InvoiceNo = string.Empty;
                    rtd.NewTireItem = null;
                    rtd.SerialNo = string.Empty;
                    rtd.FromWarehouse = null;
                    rtd.InvControlId = null;
                    rtd.Replacement.ReqWorksheetId = null;
                    rtd.Replacement.RequisitionId = null;
                    rtd.Replacement.OriginSourceId = null;
                    rtd.Replacement.IssueToCC = null;
                    rtd.Replacement.Status = SerialNoStatusEnum.Available;
                    rtd.Replacement = null;
                    //rtd.Replacement.Save();
                    rtd.Save();
                }
            }
            _FrmProgress.Close();
            this.View.ObjectSpace.CommitChanges();
        }


        void issueTireAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e) {
            ItemTrackingEntry ite = e.PopupWindow.View.CurrentObject as ItemTrackingEntry;
            RwsTireDetail rtd = e.CurrentObject as RwsTireDetail;
            if (rtd.Replacement != null)
            {
                XtraMessageBox.Show(
                "This tire request has already been issued. Remove previous issue first.", "Already Issued",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
                return;
            }
            ItemTrackingEntry sIte = rtd.Session.GetObjectByKey<ItemTrackingEntry>(ite.Oid);
            sIte.Status = SerialNoStatusEnum.Reserved;
            sIte.IssueToCC = rtd.ReqWorksheetId.CostCenter;
            sIte.OriginSourceId = sIte.IcjID.GenJournalID;
            sIte.RequisitionId = rtd.ReqWorksheetId.RequisitionInfo;
            sIte.ReqWorksheetId = rtd.ReqWorksheetId;
            rtd.IssueDate = DateTime.Now;
            rtd.FromWarehouse = sIte.Warehouse;
            rtd.InvControlId = sIte.IcjID ?? null;
            rtd.Replacement = sIte;
            rtd.SerialNo = sIte.SerialNo;
            rtd.NewTireItem = sIte.ItemNo as TireItem;
            rtd.InvoiceNo = sIte.IcjID.GenJournalID.GetType() == typeof(Receipt) ? (sIte.IcjID.GenJournalID as Receipt).InvoiceNo : string.Empty;
            rtd.Cost = sIte.IcjID.Cost;
            sIte.Save();
            rtd.Save();
            this.View.ObjectSpace.CommitChanges();
            //rtd.Session.CommitTransaction();
        }

        void issueTireAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e) {
            rwsTireDetail = ((DevExpress.ExpressApp.ListView)this.View).
            CurrentObject as RwsTireDetail;
            RequisitionWorksheet rw = rwsTireDetail.Session.GetObjectByKey<RequisitionWorksheet>(rwsTireDetail.ReqWorksheetId.Oid);
            var rwct = from trns in rw.ReqCarryoutTransactions
                       where trns.SourceType.Code == "RC"
                       select trns;
            if (rwct != null && rwct.Count() > 0)
            {
                ArrayList keysToShow1 = new ArrayList();
                InventoryControlJournal icj = null;
                XPClassInfo objClass = null;
                CriteriaOperator criteria = null;
                SortingCollection sorting = new SortingCollection(null);
                foreach (ReqCarryoutTransaction cot in rwct)
                {
                    //Receipt rcpt = cot.TransactionId as Receipt;
                    // Filter ItemNo, SourceNo [Gen Journal ID.Source No] = ? And [Item No.Item Type] = 'Tire Item'
                    icj = rwsTireDetail.Session.FindObject<InventoryControlJournal>(CriteriaOperator.Parse("[GenJournalID.SourceNo] = ? And [ItemNo.ItemType] = 'TireItem'", cot.TransactionId.SourceNo));
                    objClass = rwsTireDetail.Session.GetClassInfo<ItemTrackingEntry>();
                    criteria = CriteriaOperator.Parse(
                    string.Format("[IcjID.Oid]= '{0}' And [Status] = 'Available'", icj.Oid));
                    ICollection cols = rwsTireDetail.Session.GetObjects(objClass, criteria, sorting, 0, false, false);
                    if (cols != null && cols.Count > 0)
                    {
                        foreach (ItemTrackingEntry col in cols)
                        {
                            keysToShow1.Add(col.Oid);
                        }
                    }
                }
                //if (keysToShow1.Count > 0)
                //{
                //    string viewId = "ItemTrackingEntry_ListView_UnissuedTires";
                //    CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ItemTrackingEntry), viewId);
                //    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ItemTrackingEntry)), keysToShow1);
                //    e.View = Application.CreateListView(viewId, collectionSource1,
                //    true);
                //}
                string viewId = "ItemTrackingEntry_ListView_UnissuedTires";
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ItemTrackingEntry), viewId);
                collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ItemTrackingEntry)), keysToShow1);
                e.View = Application.CreateListView(viewId, collectionSource1,
                true);
            }
            else
            {
                throw new UserFriendlyException("Has no carried out receipt transanction");
            }
        }
    }
}
