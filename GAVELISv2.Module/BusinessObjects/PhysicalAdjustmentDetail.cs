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
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class PhysicalAdjustmentDetail : XPObject, IToMovementCapable {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private Warehouse _Warehouse;
        private decimal _CurrentQtyBase;
        private UnitOfMeasure _BaseUOM;
        //private decimal _CurrentQtyStock;
        private UnitOfMeasure _StockUOM;
        private decimal _Factor = 1;
        private decimal _ActualQtyBase;
        private decimal _ActualQtyStock;
        //private decimal _DifferenceBase;
        private decimal _Cost;
        //private decimal _LineDiffCost;
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("GenJournalHeader-PhysicalAdjustmentDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo {
            get { return _ItemNo; }
            set {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    Warehouse = _ItemNo.WarehouseLocation;
                    //CurrentQtyBase = _ItemNo.GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate);
                    BaseUOM = _ItemNo.BaseUOM;
                    StockUOM = _ItemNo.StockUOM;
                    ActualQtyBase = 0;
                    ActualQtyStock = 0;
                    Cost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        foreach (UOMRelation
                            item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _StockUOM
                                )
                            {
                                Factor = item.Factor;
                                Cost = item.CostPerBaseUom;
                            }
                        }
                    }
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Warehouse Warehouse {
            get { return _Warehouse; }
            set {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
                //if (!IsLoading && _Warehouse != null) {CurrentQtyBase = _ItemNo.
                //    GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate);}
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public decimal CurrentQtyBase
        {
            get
            {
                if (_ItemNo != null && _Warehouse != null)
                {
                    //return _ItemNo.
                    //GetWarehouseQtyBaseCorrected(_Warehouse, this.GenJournalID.Oid, this.GenJournalID.EntryDate, "PI") / _Factor;
                    return _ItemNo.GetWarehouseQtyBaseSimplified(_Warehouse, this.GenJournalID.EntryDate);
                }
                else
                {
                    return 0m;
                }
                //return _CurrentQtyBase; 
            }
            //set { SetPropertyValue<decimal>("CurrentQtyBase", ref _CurrentQtyBase, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set { SetPropertyValue("BaseUOM", ref _BaseUOM, value); }
        }
        [PersistentAlias("CurrentQtyBase / Factor")]
        public decimal CurrentQtyStock {
            get {
                object tempObject = EvaluateAlias("CurrentQtyStock");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure StockUOM {
            get { return _StockUOM; }
            set { SetPropertyValue("StockUOM", ref _StockUOM, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal Factor {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
        }
        [Custom("DisplayFormat", "n")]
        //[Custom("AllowEdit", "False")]
        public decimal ActualQtyBase {
            get { return _ActualQtyBase; }
            set {
                SetPropertyValue("ActualQtyBase", ref _ActualQtyBase, value);
                if (!IsLoading) {
                    _ActualQtyStock = 0;
                    _ActualQtyStock = _ActualQtyBase / _Factor;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        [DisplayName("Actual Count")]
        //[Custom("AllowEdit", "False")]
        public decimal ActualQtyStock {
            get { return _ActualQtyStock; }
            set {
                SetPropertyValue("ActualQtyStock", ref _ActualQtyStock, value);
                if (!IsLoading) {
                    _ActualQtyBase = 0;
                    _ActualQtyBase = _ActualQtyStock * Factor;
                }
            }
        }
        [PersistentAlias("ActualQtyStock * Cost")]
        [Custom("DisplayFormat", "n")]
        public decimal ActualValue
        {
            get
            {
                object tempObject = EvaluateAlias("ActualValue");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("ActualQtyBase - CurrentQtyBase")]
        [Custom("DisplayFormat", "n")]
        public decimal DifferenceBase
        {
            get
            {
                object tempObject = EvaluateAlias("DifferenceBase");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("ActualQtyStock - CurrentQtyStock")]
        [Custom("DisplayFormat", "n")]
        public decimal DifferenceStock {
            get {
                object tempObject = EvaluateAlias("DifferenceStock");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        [PersistentAlias("Cost * DifferenceBase")]
        [Custom("DisplayFormat", "n")]
        public decimal LineDiffCost {
            get {
                object tempObject = EvaluateAlias("LineDiffCost");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [PersistentAlias("Cost * DifferenceStock")]
        [Custom("DisplayFormat", "n")]
        public decimal ValueDiff {
            get {
                object tempObject = EvaluateAlias("ValueDiff");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        private string _BatteryRef;
        [DisplayName("Scrapped Ref.")]
        [Custom("AllowEdit", "False")]
        public string BatteryRef
        {
            get { return _BatteryRef; }
            set { SetPropertyValue("BatteryRef", ref _BatteryRef, value); }
        }
        
        [Aggregated,
        Association(
        "PhysicalAdjustmentDetail-PhysicalAdjustmentDetailTrackingLines")]
        public XPCollection<PhysicalAdjustmentDetailTrackingLine> 
        PhysicalAdjustmentDetailTrackingLines { get { return GetCollection<
                PhysicalAdjustmentDetailTrackingLine>(
                "PhysicalAdjustmentDetailTrackingLines"); } }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        #endregion
        public PhysicalAdjustmentDetail(Session session): base(session) {
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
            RowID = Guid.NewGuid();
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }
        protected override void OnSaving() {
            base.OnSaving();
            #region Saving Modified
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }
            #endregion
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
