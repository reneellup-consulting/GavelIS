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
    [NavigationItem(false)]
    public class ReceiptFuelDetail : XPObject, ISetIncomeExpense {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private Warehouse _Warehouse;
        private decimal _Ordered;
        private decimal _Received;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        //private decimal _BaseQTY;
        private decimal _BaseCost;
        private decimal _Cost;
        //private decimal _Total;
        private decimal _Returned;
        private POrderFuelDetail _PODetailID;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private decimal _LineDiscPercent;
        private decimal _LineDiscount;
        private PurchaseOrderDetail _PODetailID2;
        private Requisition _RequisitionNo;
        private BusinessObjects.CostCenter _CostCenter;
        private BusinessObjects.Facility _Facility;
        private BusinessObjects.Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        private Employee _RequestedBy;
        private GenJournalDetail _PettyCashID;
        private string _Remarks;

        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("GenJournalHeader-ReceiptFuelDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set {
                GenJournalHeader oldGenJournalID = _GenJournalID;
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && oldGenJournalID != _GenJournalID)
                {
                    oldGenJournalID = oldGenJournalID ?? _GenJournalID;
                    ((ReceiptFuel)oldGenJournalID).UpdateTotal(true);
                    ((ReceiptFuel)oldGenJournalID).UpdateTotalQty(true);
                    ((ReceiptFuel)oldGenJournalID).UpdatePrice(true);
                }
            }
        }
        [NonPersistent]
        public ReceiptFuel ReceiptInfo { get { return (ReceiptFuel)_GenJournalID
                ; } }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo {
            get { return _ItemNo; }
            set {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null) {
                    Warehouse = _ItemNo.WarehouseLocation != null ? _ItemNo.
                    WarehouseLocation : null;
                    Description = !string.IsNullOrEmpty(_ItemNo.
                    PurchaseDescription) ? _ItemNo.PurchaseDescription : _ItemNo
                    .Description;
                    UOM = _ItemNo.PurchaseUOM != null ? _ItemNo.PurchaseUOM : 
                    _ItemNo.BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    BaseCost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0) {foreach (UOMRelation 
                        item in _ItemNo.UOMRelations) {if (item.UOM == _UOM) {
                                Factor = item.Factor;}}}
                }
            }
        }
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse Warehouse {
            get { return _Warehouse; }
            set { SetPropertyValue("Warehouse", ref _Warehouse, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Ordered {
            get { return _Ordered; }
            set { SetPropertyValue("Ordered", ref _Ordered, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Received {
            get { return _Received; }
            set { SetPropertyValue("Received", ref _Received, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set {
                decimal oldValue = _Quantity;
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading && _FuelRegister != null && _Rfud != null && oldValue != value)
                {
                    try
                    {
                        // TODO: Update fuelRegister Qty with the value
                        FuelRegister.Qty = value;
                        // TODO: Update ReceiptFuelDetai fuelQty with the value
                        Rfud.FuelQty = value;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public FuelRegister FuelRegister
        {
            get { return _FuelRegister; }
            set { SetPropertyValue("FuelRegister", ref _FuelRegister, value); }
        }
        [Custom("AllowEdit", "False")]
        public ReceiptFuelUsageDetail Rfud
        {
            get { return _Rfud; }
            set { SetPropertyValue("Rfud", ref _Rfud, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set {
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null) {Factor = GetFactor();}
                if (!IsLoading) {
                    try {
                        ((ReceiptFuel)_GenJournalID).UpdateTotal(true);
                        ((ReceiptFuel)_GenJournalID).UpdateTotalQty(true);
                        ((ReceiptFuel)_GenJournalID).UpdatePrice(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading) {
                    try {
                        ((ReceiptFuel)_GenJournalID).UpdateTotal(true);
                        ((ReceiptFuel)_GenJournalID).UpdateTotalQty(true);
                        ((ReceiptFuel)_GenJournalID).UpdatePrice(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set { SetPropertyValue("BaseUOM", ref _BaseUOM, value); }
        }
        [PersistentAlias("Quantity * Factor")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseQTY {
            get {
                object tempObject = EvaluateAlias("BaseQTY");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseCost {
            get { return _BaseCost; }
            set {
                SetPropertyValue("BaseCost", ref _BaseCost, value);
                if (!IsLoading) {
                    _Cost = 0;
                    _Cost = _BaseCost * _Factor;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal LineDiscPercent
        {
            get { return _LineDiscPercent; }
            set
            {
                SetPropertyValue("LineDiscPercent", ref _LineDiscPercent, value)
                ;
                if (!IsLoading)
                {
                    LineDiscount = (_Quantity * _Cost) * (_LineDiscPercent /
                    100);
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal LineDiscount
        {
            get { return _LineDiscount; }
            set
            {
                SetPropertyValue("LineDiscount", ref _LineDiscount, value);
                if (!IsLoading)
                {
                    if (!IsLoading)
                    {
                        try
                        {
                            ((ReceiptFuel)_GenJournalID).UpdateTotal(true);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set {
                SetPropertyValue("Cost", ref _Cost, value);
                if (!IsLoading) {
                    _BaseCost = 0;
                    _BaseCost = _Cost / _Factor;
                }
                if (!IsLoading) {
                    try {
                        ((ReceiptFuel)_GenJournalID).UpdateTotal(true);
                        ((ReceiptFuel)_GenJournalID).UpdateTotalQty(true);
                        ((ReceiptFuel)_GenJournalID).UpdatePrice(true);
                    } catch (Exception) {}
                }
            }
        }
        [PersistentAlias("Quantity * Cost")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Returned {
            get { return _Returned; }
            set { SetPropertyValue("Returned", ref _Returned, value); }
        }
        [Custom("AllowEdit", "False")]
        public POrderFuelDetail PODetailID {
            get { return _PODetailID; }
            set { SetPropertyValue("PODetailID", ref _PODetailID, value); }
        }
        [Custom("AllowEdit", "False")]
        public PurchaseOrderDetail PODetailID2
        {
            get { return _PODetailID2; }
            set { SetPropertyValue("PODetailID2", ref _PODetailID2, value); }
        }
        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo
        {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter
        {
            get { return _CostCenter; }
            set
            {
                SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value);
                if (!IsLoading && !IsSaving && _CostCenter != null)
                {
                    Facility = _CostCenter.Facility ?? null;
                    Department = _CostCenter.Department ?? null;
                }
            }
        }

        public Facility Facility
        {
            get { return _Facility; }
            set
            {
                SetPropertyValue("Facility", ref _Facility, value);
            }
        }
        [DataSourceProperty("Facility.Departments")]
        public Department Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
                if (!IsLoading && !IsSaving && _Department != null)
                {
                    FacilityHead = _Department.DepartmentHead ?? null;
                    DepartmentInCharge = _Department.InCharge ?? null;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Employee FacilityHead
        {
            get { return _FacilityHead; }
            set { SetPropertyValue("FacilityHead", ref _FacilityHead, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee DepartmentInCharge
        {
            get { return _DepartmentInCharge; }
            set { SetPropertyValue("DepartmentInCharge", ref _DepartmentInCharge, value); }
        }

        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set { SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value); }
        }

        [Custom("AllowEdit", "False")]
        [NonCloneable]
        public GenJournalDetail PettyCashID
        {
            get { return _PettyCashID; }
            set { SetPropertyValue("PettyCashID", ref _PettyCashID, value); }
        }

        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get
            {
                return _SubExpenseType;
            }
            set
            {
                SetPropertyValue("SubExpenseType", ref _SubExpenseType, value);
            }
        }
        public decimal GetFactor() {
            bool found = false;
            decimal res = 1;
            if (_ItemNo.UOMRelations.Count > 0) {
                foreach (UOMRelation item in _ItemNo.UOMRelations) {
                    if (item.UOM == _UOM) {
                        found = true;
                        res = item.Factor;
                        break;
                    }
                }
                if (!found) {_UOM = _ItemNo.BaseUOM;}
            } else {
                _UOM = _ItemNo.BaseUOM;
            }
            _Cost = _BaseCost * res;
            return res;
        }
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
        public ReceiptFuelDetail(Session session): base(session) {
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
        private int deletedOid;
        protected override void OnDeleting()
        {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
            }
            base.OnDeleting();
        }
        protected override void OnSaving() {
            if (IsDeleted)
            {
                IncomeAndExpense02 incExp = null;
                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = {1}", deletedOid, this.Oid)));
                if (incExp != null)
                {
                    incExp.Delete();
                }
            }
            else
            {
                if (this.GenJournalID != null)
                {
                    this.GenJournalID.IsIncExpNeedUpdate = true;
                    this.GenJournalID.Save();
                }
            }
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
        private BusinessObjects.FuelRegister _FuelRegister;
        private ReceiptFuelUsageDetail _Rfud;
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
