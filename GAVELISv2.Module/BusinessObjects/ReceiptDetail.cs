using System;
using System.Linq;
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
    [System.ComponentModel.DefaultProperty("Oid")]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class ReceiptDetail : XPObject, ISetIncomeExpense {
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
        private decimal _LineDiscPercent;
        private decimal _LineDiscount;
        //private decimal _Total;
        private decimal _Returned;
        private PurchaseOrderDetail _PODetailID;
        private TireForSaleDet _TfsDetailId;
        private Requisition _RequisitionNo;
        private CostCenter _CostCenter;
        private Employee _RequestedBy;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private Facility _Facility;
        private Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value);
            if (!IsLoading && !IsSaving && _CostCenter != null)
            {
                Facility = _CostCenter.Facility ?? null;
                Department = _CostCenter.Department ?? null;
            }
            }
        }

        public Employee RequestedBy {
            get { return _RequestedBy; }
            set { SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value); }
        }
        //[Custom("AllowEdit", "False")]
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        //[Custom("AllowEdit", "False")]
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
        [Custom("AllowEdit", "False")]
        [NonCloneable]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("GenJournalHeader-ReceiptDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }

        [NonPersistent]
        public Receipt ReceiptInfo {
            get { return (Receipt)_GenJournalID; }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo {
            get { return _ItemNo; }
            set
            {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    Warehouse = _ItemNo.WarehouseLocation != null ? _ItemNo.
                    WarehouseLocation : null;
                    Description = !string.IsNullOrEmpty(_ItemNo.
                    PurchaseDescription) ? _ItemNo.PurchaseDescription : _ItemNo
                    .Description;
                    UOM = _ItemNo.PurchaseUOM != null ? _ItemNo.PurchaseUOM :
                    _ItemNo.BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    BaseCost = _ItemNo.Cost;
                    Cost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        var data = _ItemNo.UOMRelations.OrderBy(o => o.Factor).FirstOrDefault();
                        if (data != null)
                        {
                            BaseCost = data.CostPerBaseUom;
                            BaseUOM = data.UOM;
                        }
                        foreach (UOMRelation
                        item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _UOM)
                            {
                                Factor = item.Factor;
                                Cost = item.CostPerBaseUom;
                            }
                        }
                    }
                }
            }
        }

        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
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

        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading && _GenJournalID != null)
                {
                    try
                    {
                        ((Receipt)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set
            {
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null)
                {
                    Factor = GetFactor();
                }
                if (!IsLoading)
                {
                    try
                    {
                        ((Receipt)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set
            {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((Receipt)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
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
            get
            {
                object tempObject = EvaluateAlias("BaseQTY");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseCost {
            get { return _BaseCost; }
            set
            {
                SetPropertyValue("BaseCost", ref _BaseCost, value);
                //if (!IsLoading)
                //{
                //    _Cost = 0;
                //    _Cost = _BaseCost * _Factor;
                //}
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set
            {
                SetPropertyValue("Cost", ref _Cost, value);
                //if (!IsLoading)
                //{
                //    _BaseCost = 0;
                //    _BaseCost = _Cost / _Factor;
                //}
                if (!IsLoading)
                {
                    try
                    {
                        ((Receipt)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal LineDiscPercent {
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
        public decimal LineDiscount {
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
                            ((Receipt)_GenJournalID).UpdateTotal(true);
                        } catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        [PersistentAlias("(Quantity * Cost)  - LineDiscount")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
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
        public Requisition RequisitionNo {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }

        [Custom("AllowEdit", "False")]
        [NonCloneable]
        public PurchaseOrderDetail PODetailID {
            get { return _PODetailID; }
            set { SetPropertyValue("PODetailID", ref _PODetailID, value); }
        }

        // POFuelDetail
        [Custom("AllowEdit", "False")]
        [NonCloneable]
        public POrderFuelDetail POFuelDetail
        {
            get { return _POFuelDetail; }
            set { SetPropertyValue("POFuelDetail", ref _POFuelDetail, value); }
        }

        [Custom("AllowEdit", "False")]
        [NonCloneable]
        public GenJournalDetail PettyCashID
        {
            get { return _PettyCashID; }
            set { SetPropertyValue("PettyCashID", ref _PettyCashID, value); }
        }
        private TreadStatus _RecapType;
        public TreadStatus RecapType {
            get { return _RecapType; }
            set { SetPropertyValue<TreadStatus>("RecapType", ref _RecapType, value); }
        }

        private bool _WithSR = false;
        [DisplayName("SR")]
        public bool WithSR {
            get { return _WithSR; }
            set { SetPropertyValue<bool>("WithSR", ref _WithSR, value); }
        }

        private bool _Regrooved = false;
        public bool Regrooved {
            get { return _Regrooved; }
            set { SetPropertyValue<bool>("Regrooved", ref _Regrooved, value); }
        }

        private decimal _TreadDepth = 14m;
        public decimal TreadDepth
        {
            get { return _TreadDepth; }
            set { SetPropertyValue("TreadDepth", ref _TreadDepth, value); }
        }
        
        private TireToRetDetail _TireToRetDetailId;
        [Custom("AllowEdit", "False")]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public TireToRetDetail TireToRetDetailId {
            get { return _TireToRetDetailId; }
            set { SetPropertyValue<TireToRetDetail>("TireToRetDetailId", ref _TireToRetDetailId, value); }
        }

        [Custom("AllowEdit", "False")]
        public TireForSaleDet TfsDetailId {
            get { return _TfsDetailId; }
            set { SetPropertyValue<TireForSaleDet>("TfsDetailId", ref _TfsDetailId, value); }
        }

        public decimal GetFactor() {
            bool found = false;
            decimal res = 1;
            if (_ItemNo.UOMRelations.Count > 0)
            {
                var dBaseUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.BaseUOM2).FirstOrDefault();
                foreach (UOMRelation item in _ItemNo.UOMRelations)
                {
                    if (item.UOM == _UOM)
                    {
                        found = true;
                        res = item.Factor;
                        Cost = item.CostPerBaseUom;
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = dBaseUOM.UOM;
                    Cost = dBaseUOM.CostPerBaseUom;
                }
            }
            else
            {
                _UOM = _ItemNo.BaseUOM;
                Cost = _ItemNo.Cost;
            }
            //_Cost = _BaseCost * res;
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

        [Aggregated,
        Association("ReceiptDetail-ReceiptDetailTrackingLines")]
        public XPCollection<ReceiptDetailTrackingLine>
        ReceiptDetailTrackingLines {
            get { return GetCollection<
                ReceiptDetailTrackingLine>("ReceiptDetailTrackingLines"); }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month {
            get
            {
                _Month = ReceiptInfo.EntryDate.Month == 1 ? MonthsEnum.January : ReceiptInfo.EntryDate.Month
                 == 2 ? MonthsEnum.February : ReceiptInfo.EntryDate.Month == 3 ? MonthsEnum.
                March : ReceiptInfo.EntryDate.Month == 4 ? MonthsEnum.April : ReceiptInfo.EntryDate.Month ==
                5 ? MonthsEnum.May : ReceiptInfo.EntryDate.Month == 6 ? MonthsEnum.June :
                ReceiptInfo.EntryDate.Month == 7 ? MonthsEnum.July : ReceiptInfo.EntryDate.Month == 8 ?
                MonthsEnum.August : ReceiptInfo.EntryDate.Month == 9 ? MonthsEnum.September
                 : ReceiptInfo.EntryDate.Month == 10 ? MonthsEnum.October : ReceiptInfo.EntryDate.Month == 11
                 ? MonthsEnum.November : ReceiptInfo.EntryDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string Quarter {
            get
            {
                switch (Month)
                {
                    case MonthsEnum.None:
                        break;
                    case MonthsEnum.January:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.February:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.March:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.April:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.May:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.June:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.July:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.August:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.September:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.October:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.November:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.December:
                        _Quarter = "4th QTR";
                        break;
                    default:
                        break;
                }
                return _Quarter;
            }
        }

        [NonPersistent]
        public int Year {
            get
            {
                return ReceiptInfo.EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string MonthSorter {
            get
            {
                switch (Month)
                {
                    case MonthsEnum.None:
                        return "00 NONE";
                    case MonthsEnum.January:
                        return "01 JANUARY";
                    case MonthsEnum.February:
                        return "02 FEBRUARY";
                    case MonthsEnum.March:
                        return "03 MARCH";
                    case MonthsEnum.April:
                        return "04 APRIL";
                    case MonthsEnum.May:
                        return "05 MAY";
                    case MonthsEnum.June:
                        return "06 JUNE";
                    case MonthsEnum.July:
                        return "07 JULY";
                    case MonthsEnum.August:
                        return "08 AUGUST";
                    case MonthsEnum.September:
                        return "09 SEPTEMBER";
                    case MonthsEnum.October:
                        return "10 OCTOBER";
                    case MonthsEnum.November:
                        return "11 NOVEMBER";
                    case MonthsEnum.December:
                        return "12 DECEMBER";
                    default:
                        return "00 NONE";
                }
            }
        }

        #endregion

        public ReceiptDetail(Session session)
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
            //Session.OptimisticLockingReadBehavior = 
            //OptimisticLockingReadBehavior.ReloadObject;
            RowID = Guid.NewGuid();

            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }

            #endregion

        }

        private void RegenerateCarryoutTrans() {
            if (_RequisitionNo != null && _PODetailID.RequestID != Guid.Empty)
            {
                //UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
                //var rws = session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _PODetailID.RequestID));
                if (deletedId != null)
                {
                    this.RegenerateCarryoutTrans(deletedId, this, itemId, requisitionId, true);
                } else
                {
                    this.RegenerateCarryoutTrans(this.GenJournalID, this, _ItemNo, _RequisitionNo);
                }
                //rws.Save();
                //session.CommitChanges();
            }
        }

        public void RegenerateCarryoutTrans(GenJournalHeader genjo, XPObject obj, Item item, Requisition req, bool delete = false) {

            #region Start Processing

            RequisitionWorksheet req01 = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _PODetailID.RequestID));
            if (req01 == null)
            {
                throw new ApplicationException("There is no valid Requisition Worksheet identified for this transaction line");
            }
            CriteriaOperator criteria = null;
            if (obj != null && delete)
            {
                var data = (from rct in req01.ReqCarryoutTransactions
                            where rct.SourceType.Code == genjo.SourceType.Code && rct.LineNo == obj.Oid
                            select rct).FirstOrDefault();
                if (data != null)
                {
                    data.Delete();
                }
            }
            else if (obj != null && genjo != null && !delete)
            {
                GenJournalHeader gjh = this.Session.GetObjectByKey<GenJournalHeader>(genjo.Oid);
                criteria = CriteriaOperator.Parse(string.Format("[SourceType.Code] = '{0}' And [LineNo] = {1}", genjo.SourceType.Code, obj.Oid));
                ReqCarryoutTransaction rwct = this.Session.FindObject<ReqCarryoutTransaction>(criteria);
                if (rwct == null)
                {
                    rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(this.Session);
                }
                rwct.ReqWorksheetId = req01;
                rwct.TransactionId = gjh;
                rwct.SourceType = gjh.SourceType;
                rwct.LineNo = obj.Oid;
                if (gjh.SourceType.Code == "PO")
                {
                    rwct.Quantity = (obj as PurchaseOrderDetail).Quantity;
                }
                if (gjh.SourceType.Code == "RC")
                {
                    rwct.Quantity = (obj as ReceiptDetail).Quantity;
                }
                if (gjh.SourceType.Code == "TO")
                {
                    rwct.Quantity = (obj as TransferOrderDetail).Quantity;
                }
                if (gjh.SourceType.Code == "SO")
                {
                    rwct.Quantity = (obj as SalesOrderDetail).Quantity;
                }
                if (gjh.SourceType.Code == "JO")
                {
                    rwct.Quantity = (obj as JobOrderDetail).Quantity;
                }
                if (gjh.SourceType.Code == "WO")
                {
                    rwct.Quantity = (obj as WorkOrderItemDetail).Quantity;
                }
                rwct.Save();
            }
            req01.Save();
            this.Session.Save(this.Session.GetObjectsToSave(true));
            //this.Session.CommitTransaction();
            //this.Session.Reload(req01);

            #endregion

        }

        private GenJournalHeader deletedId;
        private Item itemId;
        private Requisition requisitionId;
        private int deletedOid;
        protected override void OnDeleting() {
            if (this.TireToRetDetailId!=null)
            {
                this.TireToRetDetailId.ReceiptDetailId = null;
                this.TireToRetDetailId.Save();
            }
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
                deletedId = this.GenJournalID;
            }
            //deletedOid = this.GenJournalID.Oid;
            //deletedId = this.GenJournalID;
            itemId = this.ItemNo;
            requisitionId = this.RequisitionNo ?? null;
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
            } else
            {
                if (this.GenJournalID != null)
                {
                    this.GenJournalID.IsIncExpNeedUpdate = true;
                    this.GenJournalID.Save();
                }
            }
            base.OnSaving();

            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }

            #endregion

        }

        public bool skipAuto = false;
        protected override void OnSaved() {
            base.OnSaved();
            this.RegenerateCarryoutTrans();
            //if (!skipAuto)
            //{
            //    this.RegenerateCarryoutTrans();
            //}
        }

        protected override void OnDeleted() {
            base.OnDeleted();
            this.RegenerateCarryoutTrans();
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private GenJournalDetail _PettyCashID;
        private POrderFuelDetail _POFuelDetail;
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
