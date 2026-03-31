using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum SmallToolsAndEquipmentActivityTypeEnum
    {
        None,
        Loaned,
        [DisplayName("Sent for Repair")]
        Sent,
        Returned,
        Dispose,
        Sold,
        Lost,
        Reserve
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class SmallToolsAndEquipmentDetail : XPObject
    {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private SmallToolsAndEquipment _ParentId;
        private DateTime _EntryDate = DateTime.Now;
        private DateTime _ActivityDate = DateTime.Now;
        private SmallToolsAndEquipmentActivityTypeEnum _ActivityType;
        private Department _Department;
        private Employee _LoanedTo;
        private SmallToolsAndEquipmentCondition _Condition;
        private SmallToolsAndEquipmentStatusEnum _Status;
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
        [Association("SmallToolsAndEquipmentDetails")]
        public SmallToolsAndEquipment ParentId
        {
            get { return _ParentId; }
            set { SetPropertyValue("ParentId", ref _ParentId, value); }
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
        //[Custom("AllowEdit", "False")]
        public SmallToolsAndEquipmentActivityTypeEnum ActivityType
        {
            get { return _ActivityType; }
            set { SetPropertyValue("ActivityType", ref _ActivityType, value);
            if (!IsSaving && !IsLoading)
            {
                switch (_ActivityType)
                {
                    case SmallToolsAndEquipmentActivityTypeEnum.None:
                        Status = SmallToolsAndEquipmentStatusEnum.NoHistory;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Loaned:
                        Status = SmallToolsAndEquipmentStatusEnum.CheckedOut;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Sent:
                        Status = SmallToolsAndEquipmentStatusEnum.UnderRepair;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Returned:
                        Status = SmallToolsAndEquipmentStatusEnum.Available;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Dispose:
                        Status = SmallToolsAndEquipmentStatusEnum.ForDisposal;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Sold:
                        Status = SmallToolsAndEquipmentStatusEnum.Sold;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Lost:
                        Status = SmallToolsAndEquipmentStatusEnum.Lost;
                        break;
                    case SmallToolsAndEquipmentActivityTypeEnum.Reserve:
                        Status = SmallToolsAndEquipmentStatusEnum.Reserved;
                        break;
                    default:
                        break;
                }
            }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Department Department
        {
            get { return _Department; }
            set { SetPropertyValue("Department", ref _Department, value); }
        }
        [DisplayName("Checked By:")]
        public Employee LoanedTo
        {
            get { return _LoanedTo; }
            set { SetPropertyValue("LoanedTo", ref _LoanedTo, value); }
        }
        public SmallToolsAndEquipmentCondition Condition
        {
            get { return _Condition; }
            set { SetPropertyValue("Condition", ref _Condition, value); }
        }
        
        public SmallToolsAndEquipmentStatusEnum Status
        {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); }
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
        public PhysicalAdjustment AdjustmentDoc
        {
            get { return _AdjustmentDoc; }
            set { SetPropertyValue("AdjustmentDoc", ref _AdjustmentDoc, value); }
        }
        private bool _ReserveDone;
        [Custom("AllowEdit", "False")]
        public bool ReserveDone
        {
            get { return _ReserveDone; }
            set { SetPropertyValue("ReserveDone", ref _ReserveDone, value); }
        }
        private bool _WasReserved;
        [Custom("AllowEdit", "False")]
        public bool WasReserved
        {
            get { return _WasReserved; }
            set { SetPropertyValue("WasReserved", ref _WasReserved, value); }
        }
        private bool _CreateJobOrder = false;
        public bool CreateJobOrder
        {
            get { return _CreateJobOrder; }
            set { SetPropertyValue("CreateJobOrder", ref _CreateJobOrder, value); }
        }
        private SmallToolsAndEquipmentDetail _ReserveDetail;
        [Custom("AllowEdit", "False")]
        public SmallToolsAndEquipmentDetail ReserveDetail
        {
            get { return _ReserveDetail; }
            set { SetPropertyValue("ReserveDetail", ref _ReserveDetail, value); }
        }
        public SmallToolsAndEquipmentDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
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
