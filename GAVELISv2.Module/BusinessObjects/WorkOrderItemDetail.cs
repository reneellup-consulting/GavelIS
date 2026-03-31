using System;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class WorkOrderItemDetail : XPObject, IToMovementCapable, ISetIncomeExpense
    {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private DateTime _DateIssued;
        private Item _ItemNo;
        private string _Description;
        private Warehouse _Warehouse;
        private decimal _CurrentQtyBase;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        private decimal _BasePrice;
        private decimal _Price;
        private Guid _SalesRegistryRowID;
        private Guid _RequestID;
        private string _FWDNo;
        private PartsOrigin _Origin;
        private Requisition _RequisitionNo;
        private CostCenter _CostCenter;
        private Employee _RequestedBy;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private bool _IsFromReceipt = false;
        private ReceiptDetail _ReceiptDetailID;
        private TransferOrderDetail _TransOrdDetailID;
        private Facility _Facility;
        private Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set
            {
                SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value);
                if (!IsLoading && _CostCenter != null)
                {
                    RequestedBy = _CostCenter.PersonResponsible ?? null;
                    Fleet = _CostCenter.FixedAsset ?? null;
                }
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

        public ExpenseType ExpenseType {
            get { return _ExpenseType; }
            set { SetPropertyValue<ExpenseType>("ExpenseType", ref _ExpenseType, value);
            if (!IsLoading)
            {
                SubExpenseType = null;
            }
            }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType {
            get { return _SubExpenseType; }
            set { SetPropertyValue<SubExpenseType>("SubExpenseType", ref _SubExpenseType, value); }
        }

        public bool IsFromReceipt {
            get { return _IsFromReceipt; }
            set { SetPropertyValue<bool>("IsFromReceipt", ref _IsFromReceipt, value); }
        }

        public ReceiptDetail ReceiptDetailID {
            get { return _ReceiptDetailID; }
            set { SetPropertyValue<ReceiptDetail>("ReceiptDetailID", ref _ReceiptDetailID, value); }
        }

        public TransferOrderDetail TransOrdDetailID {
            get { return _TransOrdDetailID; }
            set { SetPropertyValue<TransferOrderDetail>("TransOrdDetailID", ref _TransOrdDetailID, value); }
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
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("WorkOrder-WorkOrderItemDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set
            {
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && _GenJournalID != null)
                {
                    this.ExpenseType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0070'"));
                    DateIssued = _GenJournalID.EntryDate;
                }
            }
        }
        // Richfarm
        public decimal Richfarm
        {
            get { return 0m; }
        }
        // Primer
        public decimal Primer
        {
            get { return 0m; }
        }
        // OtherSuppliers
        public decimal OtherSuppliers
        {
            get { return 0m; }
        }
        public DateTime DateIssued {
            get { return _DateIssued; }
            set { SetPropertyValue<DateTime>("DateIssued", ref _DateIssued, value); }
        }

        public DateTime ExactDateIssued
        {
            get { return new DateTime(_DateIssued.Date.Year, _DateIssued.Date.Month, _DateIssued.Date.Day, 0, 0, 0); }
        }
        [Association("FleetWoItems")]
        public FixedAsset Fleet
        {
            get { return _Fleet; }
            set { SetPropertyValue<FixedAsset>("Fleet", ref _Fleet, value); }
        }
        //[Action(Caption = "Populate Fleet", AutoCommit = true)]
        //public void PopFleet()
        //{
        //    if (_CostCenter != null && _CostCenter.FixedAsset != null)
        //    {
        //        Fleet = _CostCenter.FixedAsset;
        //    }
        //    else if (WorkOrderInfo != null && WorkOrderInfo.Fleet != null)
        //    {
        //        Fleet = WorkOrderInfo.Fleet;
        //    }
        //    else
        //    {
        //        Fleet = null;
        //    }
        //}
        public Item ItemNo {
            get { return _ItemNo; }
            set
            {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    Warehouse = _ItemNo.WarehouseLocation;
                    Description = _ItemNo.Description;
                    UOM = _ItemNo.SellUOM != null ? _ItemNo.SellUOM : _ItemNo.
                    BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        var data = _ItemNo.UOMRelations.OrderBy(o => o.Factor).FirstOrDefault();
                        if (data != null)
                        {
                            BasePrice = data.PricePerBaseUom;
                        }
                        foreach (UOMRelation
                        item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _UOM)
                            {
                                Factor = item.Factor;
                            }
                        }
                    }
                    else
                    {
                        Price = _ItemNo.Cost;
                        BasePrice = _ItemNo.Cost / _Factor;
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

        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
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
                if (_ItemNo != null && _Warehouse != null && _GenJournalID != null)
                {
                    //return _ItemNo.
                    //GetWarehouseQtyBaseCorrected(_Warehouse, this.GenJournalID.Oid, this.GenJournalID.EntryDate, "WO", RequisitionNo) / _Factor;
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

        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
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
                //if (!IsLoading && _Warehouse != null)
                //{
                //    CurrentQtyBase = _ItemNo.
                //    GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate) / _Factor;
                //}
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
                        ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
                    } catch (Exception)
                    {
                    }
                }
            }
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
        [Custom("DisplayFormat", "n")]
        [DisplayName("Base Cost")]
        public decimal BasePrice {
            get { return _BasePrice; }
            set
            {
                SetPropertyValue("BasePrice", ref _BasePrice, value);
                //if (!IsLoading)
                //{
                //    _Price = 0;
                //    _Price = _BasePrice * _Factor;
                //}
            }
        }

        [Custom("DisplayFormat", "n")]
        [DisplayName("Cost")]
        public decimal Price {
            get { return _Price; }
            set
            {
                SetPropertyValue("Price", ref _Price, value);
                //if (!IsLoading)
                //{
                //    _BasePrice = 0;
                //    _BasePrice = _Price / _Factor;
                //}
                if (!IsLoading)
                {
                    try
                    {
                        ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [PersistentAlias("Quantity * Price")]
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
                        Price = item.CostPerBaseUom;
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = dBaseUOM.UOM;
                    Price = dBaseUOM.CostPerBaseUom;
                }
            } else
            {
                _UOM = _ItemNo.BaseUOM;
                Price = _ItemNo.Cost;
            }
            //_Price = _BasePrice * res;
            return res;
        }

        [Custom("AllowEdit", "False")]
        public Guid SalesRegistryRowID {
            get { return _SalesRegistryRowID; }
            set { SetPropertyValue("SalesRegistryRowID", ref _SalesRegistryRowID, value); }
        }

        [Custom("AllowEdit", "False")]
        //[RuleUniqueValue("", DefaultContexts.Save)]
        public Guid RequestID {
            get { return _RequestID; }
            set { SetPropertyValue("RequestID", ref _RequestID, value); }
        }

        public string FWDNo {
            get { return _FWDNo; }
            set { SetPropertyValue<string>("FWDNo", ref _FWDNo, value); }
        }

        public PartsOrigin Origin {
            get { return _Origin; }
            set { SetPropertyValue<PartsOrigin>("Origin", ref _Origin, value); }
        }

        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }

        [NonPersistent]
        public WorkOrder WorkOrderInfo {
            get { return (WorkOrder)_GenJournalID; }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
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

        public WorkOrderItemDetail(Session session)
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
            if (_RequisitionNo != null && _RequestID != Guid.Empty)
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
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            RequisitionWorksheet req01 = session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _RequestID));
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
            } else if (obj != null && genjo != null && !delete)
            {
                GenJournalHeader gjh = session.GetObjectByKey<GenJournalHeader>(genjo.Oid);
                criteria = CriteriaOperator.Parse(string.Format("[SourceType.Code] = '{0}' And [LineNo] = {1}", genjo.SourceType.Code, obj.Oid));
                ReqCarryoutTransaction rwct = session.FindObject<ReqCarryoutTransaction>(criteria);
                if (rwct == null)
                {
                    rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(session);
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
            session.CommitTransaction();
            //this.Session.Reload(req01);

            #endregion

        }
        [Action(Caption = "Retrieve Vendor/Origin", ConfirmationMessage = "Do you really want to retrieve origin?", AutoCommit = true)]
        public void RetrieveVendorOrigin()
        {
            Vendor = _ReceiptDetailID != null ? _ReceiptDetailID.ReceiptInfo != null ? _ReceiptDetailID.ReceiptInfo.Vendor : null : null;
            Origin = _ReceiptDetailID != null ? _ReceiptDetailID.ReceiptInfo != null ? _ReceiptDetailID.ReceiptInfo.Vendor.Origin != null ? _ReceiptDetailID.ReceiptInfo.Vendor.Origin : null : null : null;
        }
        [Action(Caption = "Clear Origin", ConfirmationMessage = "Do you really want to remove the specified origin?", AutoCommit = true)]
        public void ClearOrigin() {
            Origin = null;
        }
        private GenJournalHeader deletedId;
        private Item itemId;
        private Requisition requisitionId;
        private int deletedOid;
        protected override void OnDeleting() {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
                deletedId = this.GenJournalID;
            }
            itemId = this.ItemNo;
            requisitionId = this.RequisitionNo ?? null;
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            InventoryControlJournal _icj = session.FindObject<InventoryControlJournal>(
                    new BinaryOperator("RowID", RowID.ToString()));
            if (_icj != null)
            {
                _icj.Delete();
            }
            session.CommitTransaction();
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
            if (!skipAuto)
            {
                this.RegenerateCarryoutTrans();
            }
            //Work Order must be seen on stockcard biskan wala pa na invoice ang WO
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            InventoryControlJournal _icj = session.FindObject<InventoryControlJournal>(
                    new BinaryOperator("RowID", RowID.ToString()));
            WorkOrder objectByKey = session.GetObjectByKey<WorkOrder>(this.WorkOrderInfo.Oid);
            //if (objectByKey.CompanyInfoHead.AllowInsufficientCurrQty != true)
            //{
            //    if (this._Quantity > this.CurrentQtyBase)
            //    {
            //        throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + _ItemNo.No + "!");
            //    }
            //}

            //if (this._Quantity > this.CurrentQtyBase)
            //{
            //    DialogResult dres = XtraMessageBox.Show("Warehouse is not sufficient to fullfil item " + _ItemNo.No + ". Do you want to continue?", "Insufficient Qty",
            //    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            //    if (dres == DialogResult.Cancel)
            //    {
            //        return;
            //    }
            //}

            if (_icj == null)
            {
                _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                _icj.GenJournalID = objectByKey;
                _icj.DateIssued = DateIssued;
                //_icj.OutQty = Math.Abs(BaseQTY);
                _icj.Warehouse = session.GetObjectByKey<Warehouse>(Warehouse.Oid);
                _icj.ItemNo = session.GetObjectByKey<Item>(ItemNo.Oid);
                if (_ItemNo.UOMRelations.Count > 0)
                {
                    var dUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _UOM).FirstOrDefault();
                    var dBaseUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.BaseUOM2).FirstOrDefault();
                    var dStockUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.StockUOM).FirstOrDefault();
                    UOMRelation UOMr = session.GetObjectByKey<UOMRelation>(dUOM.Oid);
                    UOMRelation UOMSr = session.GetObjectByKey<UOMRelation>(dStockUOM.Oid);
                    if (dStockUOM.UOM == dUOM.UOM)
                    {
                        _icj.OutQty = _Quantity;
                    }
                    else
                    {
                        _icj.OutQty = BaseQTY / dStockUOM.Factor;
                        //UOMr.CostPerBaseUom = //(item.Quantity * item.Cost) / _icj.InQTY;
                    }
                    UOMr.PricePerBaseUom = _Price;
                    UOMSr.PricePerBaseUom = (_Quantity * _Price) / _icj.OutQty;
                    _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(_ItemNo.StockUOM.Oid);
                    _icj.Price = UOMSr.PricePerBaseUom;
                }
                else
                {
                    _icj.OutQty = _Quantity;
                    _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(_UOM.Oid);
                    _icj.Price = _Price;
                    _ItemNo.SalesPrice = _Price;
                }
                //_icj.Cost = _icj.ItemNo.Cost;
                //_icj.Price = Price;
                //_icj.UOM = session.GetObjectByKey<UnitOfMeasure>(BaseUOM.Oid);
                _icj.RowID = RowID.ToString();
                _icj.RequisitionNo = _RequisitionNo != null ? session.GetObjectByKey<Requisition>(_RequisitionNo.Oid) : null;
                _icj.CostCenter = CostCenter!=null?session.GetObjectByKey<CostCenter>(CostCenter.Oid):null;
                _icj.RequestedBy = RequestedBy!=null?session.GetObjectByKey<Employee>(RequestedBy.Oid):null;
                _icj.Save();
            }
            else
            {
                _icj.GenJournalID = objectByKey;
                _icj.DateIssued = DateIssued;
                //_icj.OutQty = Math.Abs(BaseQTY);
                _icj.Warehouse = session.GetObjectByKey<Warehouse>(Warehouse.Oid);
                _icj.ItemNo = session.GetObjectByKey<Item>(ItemNo.Oid);
                if (_ItemNo.UOMRelations.Count > 0)
                {
                    var dUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _UOM).FirstOrDefault();
                    var dBaseUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.BaseUOM2).FirstOrDefault();
                    var dStockUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.StockUOM).FirstOrDefault();
                    UOMRelation UOMr = session.GetObjectByKey<UOMRelation>(dUOM.Oid);
                    UOMRelation UOMSr = session.GetObjectByKey<UOMRelation>(dStockUOM.Oid);
                    if (dStockUOM.UOM == dUOM.UOM)
                    {
                        _icj.OutQty = _Quantity;
                    }
                    else
                    {
                        _icj.OutQty = BaseQTY / dStockUOM.Factor;
                        //UOMr.CostPerBaseUom = //(item.Quantity * item.Cost) / _icj.InQTY;
                    }
                    UOMr.PricePerBaseUom = _Price;
                    UOMSr.PricePerBaseUom = (_Quantity * _Price) / _icj.OutQty;
                    _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(_ItemNo.StockUOM.Oid);
                    _icj.Price = UOMSr.PricePerBaseUom;
                }
                else
                {
                    _icj.OutQty = _Quantity;
                    _icj.UOM = session.GetObjectByKey<UnitOfMeasure>(_UOM.Oid);
                    _icj.Price = _Price;
                    _ItemNo.SalesPrice = _Price;
                }
                //_icj.Cost = _icj.ItemNo.Cost;
                //_icj.Price = Price;
                //_icj.UOM = session.GetObjectByKey<UnitOfMeasure>(BaseUOM.Oid);
                _icj.RowID = RowID.ToString();
                _icj.RequisitionNo = _RequisitionNo != null ? session.GetObjectByKey<Requisition>(_RequisitionNo.Oid) : null;
                _icj.CostCenter = CostCenter != null ? session.GetObjectByKey<CostCenter>(CostCenter.Oid) : null;
                _icj.RequestedBy = RequestedBy != null ? session.GetObjectByKey<Employee>(RequestedBy.Oid) : null;
                _icj.Save();
            }
            session.CommitTransaction();
        }
       
        protected override void OnDeleted() {
            base.OnDeleted();
            this.RegenerateCarryoutTrans();
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private BusinessObjects.Vendor _Vendor;
        private FixedAsset _Fleet;
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
