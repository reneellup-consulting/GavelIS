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
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class TransferOrderDetail : XPObject, IToMovementCapable
    {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private Warehouse _Warehouse;
        private decimal _CurrentQtyBase;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        //private decimal _BaseQTY;
        private UnitOfMeasure _BaseUOM;
        private decimal _Transferred;
        private Guid _RequestID;
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
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set
            {
                SetPropertyValue<ExpenseType>("ExpenseType", ref _ExpenseType, value);
                if (!IsLoading)
                {
                    SubExpenseType = null;
                }
            }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get { return _SubExpenseType; }
            set { SetPropertyValue<SubExpenseType>("SubExpenseType", ref _SubExpenseType, value); }
        }
        public Vendor Vendor
        {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && !IsSaving)
                {
                    Origin = _Vendor != null ? _Vendor.Origin != null ? _Vendor.Origin : null : null;
                }
            }
        }
        public PartsOrigin Origin
        {
            get { return _Origin; }
            set
            {
                SetPropertyValue("Origin", ref _Origin, value);
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
        [NonPersistent]
        public TransferOrder TransferOrderInfo {
            get { return (TransferOrder)
                _GenJournalID; }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-TransferOrderDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public Warehouse Warehouse {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
                //if (!IsLoading && _Warehouse != null)
                //{
                //    CurrentQtyBase = _ItemNo.
                //    GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate) / _Factor;
                //}
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
                    //GetWarehouseQtyBaseCorrected(_Warehouse, this.GenJournalID.Oid, this.GenJournalID.EntryDate, "TO", RequisitionNo) / _Factor;
                    return _ItemNo.GetWarehouseQtyBaseSimplified(_Warehouse, this.GenJournalID.EntryDate) / _Factor;
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
        public Item ItemNo {
            get { return _ItemNo; }
            set
            {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    Warehouse = ((TransferOrder)_GenJournalID).FromWarehouse;
                    Description = !string.IsNullOrEmpty(_ItemNo.SalesDescription
                    ) ? _ItemNo.SalesDescription : _ItemNo.Description;
                    //if (_GenJournalID!=null && ((Invoice)_GenJournalID).InvoiceType == InvoiceTypeEnum.
                    //OrderSlip) {Tax = CompanyInfo.DefaultNonSalesTax != null ? 
                    //    CompanyInfo.DefaultNonSalesTax : null;} else {
                    //    Tax = _ItemNo.TaxCode != null ? _ItemNo.TaxCode : null;
                    //}
                    UOM = _ItemNo.SellUOM != null ? _ItemNo.SellUOM : _ItemNo.
                    BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        BaseUOM = _ItemNo.BaseUOM2;
                        foreach (UOMRelation
                        item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _UOM)
                            {
                                Factor = item.Factor;
                            }
                        }
                    }
                    //CurrentQtyBase = _ItemNo.GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate) /
                    //_Factor;
                }
            }
        }

        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value); }
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
                //if (!IsLoading && _Warehouse != null)
                //{
                //    CurrentQtyBase = _ItemNo.
                //    GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate) / _Factor;
                //}
            }
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
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = dBaseUOM.UOM;
                }
            } else
            {
                _UOM = _ItemNo.BaseUOM;
            }
            return res;
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
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
        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set { SetPropertyValue("BaseUOM", ref _BaseUOM, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal Transferred {
            get { return _Transferred; }
            set { SetPropertyValue("Transferred", ref _Transferred, value); }
        }

        [Custom("AllowEdit", "False")]
        //[RuleUniqueValue("", DefaultContexts.Save)]
        public Guid RequestID {
            get { return _RequestID; }
            set { SetPropertyValue("RequestID", ref _RequestID, value); }
        }
        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
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
        Association("TransferOrderDetail-TransferOrderDetailTrackingLines")]
        public XPCollection<TransferOrderDetailTrackingLine>
        TransferOrderDetailTrackingLines {
            get { return GetCollection<
                TransferOrderDetailTrackingLine>(
                "TransferOrderDetailTrackingLines"); }
        }

        public TransferOrderDetail(Session session)
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

        protected override void OnSaving() {
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

        private void RegenerateCarryoutTrans()
        {
            if (_RequisitionNo != null && _RequestID != Guid.Empty)
            {
                //UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
                //var rws = session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _PODetailID.RequestID));
                if (deletedId != null)
                {
                    this.RegenerateCarryoutTrans(deletedId, this, itemId, requisitionId, true);
                }
                else
                {
                    this.RegenerateCarryoutTrans(this.GenJournalID, this, _ItemNo, _RequisitionNo);
                }
                //rws.Save();
                //session.CommitChanges();
            }
        }

        public void RegenerateCarryoutTrans(GenJournalHeader genjo, XPObject obj, Item item, Requisition req, bool delete = false)
        {
            #region Start Processing

            RequisitionWorksheet req01 = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _RequestID));
            if (req01 == null && !delete)
            {
                throw new ApplicationException("There is no valid Requisition Worksheet identified for this transaction line");
            }
            CriteriaOperator criteria = null;
            if (obj != null && req01 !=null && delete)
            {
                //req01.LastCarrySource = null;
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
            if (req01 != null)
            {
                req01.Save();
            }
            if (!delete)
            {
                this.Session.CommitTransaction();
            }
            //this.Session.Reload(req01);
            #endregion
        }

        private GenJournalHeader deletedId;
        private Item itemId;
        private Requisition requisitionId;
        private int deletedOid;
        protected override void OnDeleting()
        {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
                deletedId = this.GenJournalID;
                //RequisitionWorksheet req01 = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _RequestID));
                //if (req01 != null)
                //{
                //    req01.LastCarrySource = null;
                //}
            }
            itemId = this.ItemNo;
            requisitionId = this.RequisitionNo ?? null;
            base.OnDeleting();
        }

        public bool skipAuto = false;
        protected override void OnSaved()
        {
            base.OnSaved();
            if (!skipAuto)
            {
                this.RegenerateCarryoutTrans();
            }
        }

        protected override void OnDeleted()
        {
            base.OnDeleted();
            this.RegenerateCarryoutTrans();
        }

        //protected override void OnLoaded() {
        //    //if (!IsLoading)
        //    //{
        //    //    _CurrentQtyBase = _ItemNo.GetWarehouseQtyBase(_Warehouse) / _Factor;
        //    //}
        //    base.OnLoaded();
        //}

        #region Get Current User

        private SecurityUser _CurrentUser;
        private BusinessObjects.Vendor _Vendor;
        private PartsOrigin _Origin;
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
