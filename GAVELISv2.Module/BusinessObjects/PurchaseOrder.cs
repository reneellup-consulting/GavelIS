using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.IO;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class PurchaseOrder : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private PurchaseOrderStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Vendor _Vendor;
        private string _VendorAddress;
        private string _ShipToAddress;
        private Terms _Terms;
        private ShipVia _ShipVia;
        private DateTime _ExpectedDate;
        private string _Remarks;
        private DateTime _ApprovedDate;
        private DateTime _DisapprovedDate;
        private DateTime _PendingDate;
        private bool _ManualPrinted = false;
        private bool _Printed = false;
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        private List<string> _Refs;
        [Custom("AllowEdit", "False")]
        public List<string> Refs
        {
            get { return _Refs; }
            set { SetPropertyValue<List<string>>("Refs", ref _Refs, value); }
        }
        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool IsReopened
        {
            get { return _IsReopened; }
            set { SetPropertyValue("Comments", ref _IsReopened, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("From OFRS")]
        public bool IsOnlineFrs
        {
            get { return _IsOnlineFrs; }
            set { SetPropertyValue("IsOnlineFrs", ref _IsOnlineFrs, value); }
        }
        public PurchaseOrderStatusEnum Status {
            get { return _Status; }
            set
            {
                PurchaseOrderStatusEnum oldStatus = _Status;
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    InventoryControlJournal _icj;
                    if (oldStatus == PurchaseOrderStatusEnum.Approved && value == PurchaseOrderStatusEnum.Current)
                    {
                        // UpdateAfterChanges();
                        IsReopened = true;
                        StringBuilder sb = new StringBuilder(_AfterReopenAlterations);
                       // string stamp = string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}", DateTime.Now.Year, DateTime.Now.Month,
                       //DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendFormat("{0}*** Reopened by {1}", stamp, user).AppendLine();
                        AfterReopenAlterations = sb.ToString();
                    }
                    if (_Status != PurchaseOrderStatusEnum.Current)
                    {
                        Approved =
                        true;
                        // Update Purchases and Sales Module
                        //if (_Status == PurchaseOrderStatusEnum.Approved)
                        //{
                        //    foreach (PurchaseOrderDetail pod in PurchaseOrderDetails)
                        //    {
                        //        _icj = ReflectionHelper.CreateObject<InventoryControlJournal>(this.Session);
                        //        _icj.GenJournalID = this;
                        //        _icj.InQTY = Math.Abs(pod.Quantity);
                        //        _icj.Warehouse = pod.ItemNo.WarehouseLocation;
                        //        _icj.ItemNo = pod.ItemNo;
                        //        _icj.Cost = pod.BaseCost;
                        //        _icj.UOM = pod.ItemNo.BaseUOM;
                        //        _icj.RowID = pod.RowID.ToString();
                        //        _icj.RequisitionNo = pod.RequisitionNo != null ? pod.RequisitionNo : null;
                        //        _icj.CostCenter = pod.CostCenter != null ? pod.CostCenter : null;
                        //        _icj.RequestedBy = pod.RequestedBy != null ? pod.RequestedBy : null;
                        //        _icj.Save();
                        //    }
                        //    Session.CommitTransaction();
                        //}
                    } else
                    {
                        Approved = false;
                        Printed = false;
                        // Update Purchases and Sales Module
                        // [Row ID] = '6876'
                        //foreach (PurchaseOrderDetail pod in PurchaseOrderDetails)
                        //{
                        //    _icj = this.Session.FindObject<InventoryControlJournal>(CriteriaOperator.Parse("[RowID] = '" + pod.RowID.ToString() + "'"));
                        //    if (_icj != null)
                        //    {
                        //        _icj.Delete();
                        //    }
                        //}
                        //Session.CommitTransaction();
                    }
                }
                switch (_Status)
                {
                    case PurchaseOrderStatusEnum.Current:
                        Approved = false;
                        Printed = false;
                        ManualPrinted = false;
                        break;
                    case PurchaseOrderStatusEnum.Approved:
                        Approved = true;
                        ApprovedDate = DateTime.Now;
                        break;
                    case PurchaseOrderStatusEnum.PartiallyReceived:
                        break;
                    case PurchaseOrderStatusEnum.Received:
                        break;
                    case PurchaseOrderStatusEnum.Disapproved:
                        Approved = false;
                        DisapprovedDate = DateTime.Now;
                        break;
                    case PurchaseOrderStatusEnum.Pending:
                        Approved = false;
                        PendingDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }
        public string ChargeTo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (PurchaseOrderDetails != null && PurchaseOrderDetails.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in PurchaseOrderDetails)
                    {
                        if (item.CostCenter != null && !strRefs.Contains(item.CostCenter.Code))
                        {
                            strRefs.Add(item.CostCenter.Code);
                            sb.AppendFormat("{0},", item.CostCenter.Code);
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }

        // Requestors
        public string Requestors
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (PurchaseOrderDetails != null && PurchaseOrderDetails.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in PurchaseOrderDetails)
                    {
                        if (item.RequestedBy != null && !strRefs.Contains(item.RequestedBy.Code))
                        {
                            strRefs.Add(item.RequestedBy.Code);
                            sb.AppendFormat("{0},", item.RequestedBy.Name);
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null)
                {
                    VendorAddress = _Vendor.FullAddress;
                    Terms = _Vendor.Terms;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string VendorAddress {
            get { return _VendorAddress; }
            set { SetPropertyValue("VendorAddress", ref _VendorAddress, value); }
        }

        [Size(500)]
        public string RejectionReason
        {
            get { return _RejectionReason; }
            set { SetPropertyValue("RejectionReason", ref _RejectionReason, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string ShipToAddress {
            get { return _ShipToAddress; }
            set { SetPropertyValue("ShipToAddress", ref _ShipToAddress, value); }
        }

        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }

        public ShipVia ShipVia {
            get { return _ShipVia; }
            set { SetPropertyValue("ShipVia", ref _ShipVia, value); }
        }

        public DateTime ExpectedDate {
            get { return _ExpectedDate; }
            set { SetPropertyValue("ExpectedDate", ref _ExpectedDate, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime ApprovedDate {
            get { return _ApprovedDate; }
            set { SetPropertyValue<DateTime>("ApprovedDate", ref _ApprovedDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DisapprovedDate {
            get { return _DisapprovedDate; }
            set { SetPropertyValue<DateTime>("DisapprovedDate", ref _DisapprovedDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime PendingDate {
            get { return _PendingDate; }
            set { SetPropertyValue<DateTime>("PendingDate", ref _PendingDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool ManualPrinted {
            get { return _ManualPrinted; }
            set { SetPropertyValue<bool>("ManualPrinted", ref _ManualPrinted, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Printed {
            get { return _Printed; }
            set { SetPropertyValue<bool>("Printed", ref _Printed, value); }
        }

        [Action(Caption = "Manual PO")]
        public void ManuallyPrinted(){
            if (_Printed || _ManualPrinted)
            {
                throw new ApplicationException("PO already printed");
            }
            Printed = true;
            ManualPrinted = true;
            if (!string.IsNullOrEmpty(Remarks))
            {
                Remarks = "Manually Printed PO#:" + Environment.NewLine + Remarks;
            }
            else
            {
                Remarks = "Manually Printed PO#:";
            }
            this.Session.CommitTransaction();
        }

        [Action(Caption = "Print PO")]
        public void PrintDocument() {
            if (!Approved)
            {
                throw new ApplicationException("Cannot print unapproved PO");
            }
            if (_Printed || _ManualPrinted)
            {
                throw new ApplicationException("PO already printed");
            }
            Printed = true;
            ManualPrinted = true;
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\PurchaseOrderReport.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            rep.DataSource = PurchaseOrderDetails;
            rep.ShowPreview();
        }

        #region Calculated Details

        [Persistent("Total")]
        private decimal? _Total;
        private string _AfterReopenAlterations;
        private bool _IsReopened = false;
        private bool _IsOnlineFrs;
        private string _RejectionReason;
        [Persistent("_Total")]
        public decimal? Total {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Total == null)
                    {
                        UpdateTotal(false);
                    }
                } catch (Exception)
                {
                }
                return _Total;
            }
        }

        public void UpdateTotal(bool forceChangeEvent) {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (PurchaseOrderDetail detail in PurchaseOrderDetails)
            {
                tempTotal += detail.Total;
            }
            _Total = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Total", Total, _Total);
            }
            ;
        }

        public void UpdateResync(bool value) {
            OnChanged("ReSynced", ReSynced, value);
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _Total = null;
        }

        #endregion
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        [DisplayName("After Re-opening Changes")]
        public string AfterReopenAlterations
        {
            get { return _AfterReopenAlterations; }
            set { SetPropertyValue("AfterReopenAlterations", ref _AfterReopenAlterations, value); }
        }
        public PurchaseOrder(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "PO"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PO"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "PO"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            ShipToAddress = company.FullShipAddress;
            Memo = "Purchase Order #" + SourceNo;
        }

        protected override void OnDeleting()
        {
            if (Approved)
            {
                throw new UserFriendlyException(
                "The system prohibits the deletion of already approved Purchase Order transactions."
                );
            }

            // --- NEW SYNC QUEUE BLOCK ---
            PoSyncDeletionsQueue syncQueue = new PoSyncDeletionsQueue(Session);
            syncQueue.POType = "General";
            syncQueue.RowType = "Parent";
            syncQueue.RowId = this.Oid;
            // ----------------------------

            base.OnDeleting();
        }

        protected override void OnSaving()
        {
            // If the record has been modified and was previously synced, flag for re-sync
            if (!IsLoading && !IsDeleted && IsSynced)
            {
                ReSynced = true;
            }
            base.OnSaving();
        }
    }
}
