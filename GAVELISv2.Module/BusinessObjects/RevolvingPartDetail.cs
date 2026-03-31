using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    public enum RevolvingPartActivityTypeEnum
    {
        [DisplayName("NONE")]
        None,
        [DisplayName("ATTACHED TO")]
        Attach,
        [DisplayName("DETTACHED FROM")]
        Dettach,
        Dispose
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class RevolvingPartDetail : XPObject {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private RevolvingPart _RevolvingPartId;
        private DateTime _EntryDate;
        private DateTime _ActivityDate;
        private RevolvingPartActivityTypeEnum _ActivityType;
        private FixedAsset _Unit;
        private RevolvingPartsCondition _Condition;
        private Warehouse _Location;
        private Employee _RequestedBy;
        private RevolvingPartsStatusEnum _Status;
        private GenJournalHeader _SoldRef;
        private string _Remarks;
        private string _TransferNotice;
        private Vendor _PurchaseFrom;
        private DateTime _PurchaseDate;
        private string _SerialNo;
        private decimal _Cost;
        private WorkOrder _WorkOrderDoc;
        private JobOrder _JobOrderDoc;
        private Vendor _ServiceCenter;
        private PhysicalAdjustment _AdjustmentDoc;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("RevolvingPartsDetails")]
        public RevolvingPart RevolvingPartId
        {
            get { return _RevolvingPartId; }
            set { SetPropertyValue("RevolvingPartId", ref _RevolvingPartId, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ActivityDate
        {
            get { return _ActivityDate; }
            set { SetPropertyValue("ActivityDate", ref _ActivityDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public RevolvingPartActivityTypeEnum ActivityType
        {
            get { return _ActivityType; }
            set { SetPropertyValue("ActivityType", ref _ActivityType, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("RevolvingPartsServiceDetails")]
        public FixedAsset Unit
        {
            get { return _Unit; }
            set { SetPropertyValue("Unit", ref _Unit, value); }
        }
        public RevolvingPartsCondition Condition
        {
            get { return _Condition; }
            set { SetPropertyValue("Condition", ref _Condition, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse Location
        {
            get { return _Location; }
            set { SetPropertyValue("Location", ref _Location, value); }
        }
        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set { SetPropertyValue("RequestedBy", ref _RequestedBy, value); }
        }
        public RevolvingPartsStatusEnum Status
        {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); }
        }
        private RevolvingPartsActivityReason _Reason;
        public RevolvingPartsActivityReason Reason
        {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }
        
        public GenJournalHeader SoldRef
        {
            get { return _SoldRef; }
            set { SetPropertyValue("SoldRef", ref _SoldRef, value); }
        }
        [Size(500)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        [Custom("AllowEdit", "False")]
        public string TransferNotice
        {
            get { return _TransferNotice; }
            set { SetPropertyValue("TransferNotice", ref _TransferNotice, value); }
        }
        private bool _OpenAdjustmentDoc;
        [DisplayName("Open Document")]
        public bool OpenAdjustmentDoc
        {
            get { return _OpenAdjustmentDoc; }
            set { SetPropertyValue("OpenAdjustmentDoc", ref _OpenAdjustmentDoc, value); }
        }
        public Vendor PurchaseFrom
        {
            get { return _PurchaseFrom; }
            set { SetPropertyValue("PurchaseFrom", ref _PurchaseFrom, value); }
        }
        public DateTime PurchaseDate
        {
            get { return _PurchaseDate; }
            set { SetPropertyValue("PurchaseDate", ref _PurchaseDate, value); }
        }
        public string SerialNo
        {
            get { return _SerialNo; }
            set { SetPropertyValue("SerialNo", ref _SerialNo, value); }
        }
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        private bool _CreateJobOrder = false;
        public bool CreateJobOrder
        {
            get { return _CreateJobOrder; }
            set { SetPropertyValue("CreateJobOrder", ref _CreateJobOrder, value); }
        }
        
        public WorkOrder WorkOrderDoc
        {
            get { return _WorkOrderDoc; }
            set { SetPropertyValue("WorkOrderDoc", ref _WorkOrderDoc, value); }
        }
        public JobOrder JobOrderDoc
        {
            get { return _JobOrderDoc; }
            set { SetPropertyValue("JobOrderDoc", ref _JobOrderDoc, value); }
        }
        public Vendor ServiceCenter
        {
            get { return _ServiceCenter; }
            set { SetPropertyValue("ServiceCenter", ref _ServiceCenter, value); }
        }
        private Item _ScrappedItemNo;
        public Item ScrappedItemNo
        {
            get { return _ScrappedItemNo; }
            set { SetPropertyValue("ScrappedItemNo", ref _ScrappedItemNo, value); }
        }
        
        public PhysicalAdjustment AdjustmentDoc
        {
            get { return _AdjustmentDoc; }
            set { SetPropertyValue("AdjustmentDoc", ref _AdjustmentDoc, value); }
        }
        
        public RevolvingPartDetail(Session session): base(session) {
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
            RowID = Guid.NewGuid();
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                if (SecuritySystem.CurrentUser != null)
                {
                    _CurrentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                }
                return _CurrentUser;
            }
        }

        #endregion

    }
}
