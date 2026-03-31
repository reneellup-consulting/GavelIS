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
    public enum BatteryActivityTypeEnum
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
    public class BatteryServiceDetail : XPObject {
        private Guid _RowID;
        private Battery _BatteryNo;
        private DateTime _EntryDate;
        private DateTime _ActivityDate;
        private BatteryActivityTypeEnum _ActivityType;
        private FixedAsset _Unit;
        private BatteryCondition _Condition;
        private Warehouse _Location;
        private Employee _RequestedBy;
        private BatteryStatusEnum _Status;
        private GenJournalHeader _SoldRef;
        private string _Remarks;
        private string _TransferNotice;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("BatteryServiceDetails")]
        public Battery BatteryNo
        {
            get { return _BatteryNo; }
            set { SetPropertyValue("BatteryNo", ref _BatteryNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue<DateTime>("EntryDate", ref _EntryDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ActivityDate
        {
            get { return _ActivityDate; }
            set { SetPropertyValue("ActivityDate", ref _ActivityDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public BatteryActivityTypeEnum ActivityType
        {
            get { return _ActivityType; }
            set { SetPropertyValue("ActivityType", ref _ActivityType, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("UnitBatteryServiceDetails")]
        public FixedAsset Unit
        {
            get { return _Unit; }
            set { SetPropertyValue("Unit", ref _Unit, value); }
        }
        private BatteryDettachReason _Reason;
        [DisplayName("Dettach Reason")]
        public BatteryDettachReason Reason
        {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }
        
        public BatteryCondition Condition
        {
            get { return _Condition; }
            set { SetPropertyValue("Condition", ref _Condition, value); }
        }
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

        public BatteryStatusEnum Status
        {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); }
        }
        public GenJournalHeader SoldRef
        {
            get { return _SoldRef; }
            set { SetPropertyValue("SoldRef", ref _SoldRef, value); }
        }
        
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
        // Adjustment to receive disposed batt to inventory
        private PhysicalAdjustment _AdjustmentDoc;
        public PhysicalAdjustment AdjustmentDoc
        {
            get { return _AdjustmentDoc; }
            set { SetPropertyValue("AdjustmentDoc", ref _AdjustmentDoc, value); }
        }
        private bool _OpenAdjustmentDoc;
        public bool OpenAdjustmentDoc
        {
            get { return _OpenAdjustmentDoc; }
            set { SetPropertyValue("OpenAdjustmentDoc", ref _OpenAdjustmentDoc, value); }
        }
        //private decimal _Cost;
        //private CostCenter _ChargeTo;
        //public decimal Cost
        //{
        //    get { return _Cost; }
        //    set { SetPropertyValue("Cost", ref _Cost, value); }
        //}
        //[NonPersistent]
        //[Custom("AllowEdit", "False")]
        //public CostCenter ChargeTo
        //{
        //    get { return _ChargeTo; }
        //}
        
        public BatteryServiceDetail(Session session): base(session) {
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
