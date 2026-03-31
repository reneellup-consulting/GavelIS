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

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class EmployeeChargeSlipItemDetail : XPObject, IToMovementCapable
    {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private CostCenter _CostCenter;
        private Warehouse _Warehouse;
        private decimal _CurrentQtyBase;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        private decimal _Cost;
        private decimal _BasePrice;
        private decimal _Price;
        private decimal _LineDiscPercent;
        private decimal _LineDiscount;
        private SalesTaxCode _Tax;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("EmployeeChargeSlip-EmployeeChargeSlipItemDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue<GenJournalHeader>("GenJournalID", ref _GenJournalID, value); }
        }

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
                                Cost = item.CostPerBaseUom;
                            }
                        }
                    }
                    else
                    {
                        Price = _ItemNo.SalesPrice;
                        BasePrice = _ItemNo.SalesPrice / _Factor;
                        Cost = _ItemNo.Cost;
                    }
                    Tax = _ItemNo.TaxCode != null ? _ItemNo.TaxCode : null;
                    //Cost = _ItemNo.Cost;
                    //Price = _ItemNo.SalesPrice;
                    //BasePrice = _ItemNo.SalesPrice / _Factor;
                    //CurrentQtyBase = _ItemNo.GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate) /
                    //_Factor;
                }
            }
        }

        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value); }
        }
        [Custom("AllowEdit", "False")]
        public PhysicalAdjustment PhysicalAdjustmentID
        {
            get { return _PhysicalAdjustmentID; }
            set { SetPropertyValue<PhysicalAdjustment>("PhysicalAdjustmentID", ref _PhysicalAdjustmentID, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Warehouse Warehouse {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue<Warehouse>("Warehouse", ref _Warehouse, value);
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
        public decimal CurrentQtyBase {
            get {
                if (_ItemNo != null && _Warehouse != null)
                {
                    //        return _ItemNo.
                    //GetWarehouseQtyBaseCorrected(_Warehouse, this.GenJournalID.Oid, this.GenJournalID.EntryDate, "ECS", RequisitionNo) / _Factor;
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
                SetPropertyValue<decimal>("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    try
                    {
                        //((EmployeeChargeSlip)_GenJournalID).UpdateTotalItems(true);
                        ((EmployeeChargeSlip)_GenJournalID).UpdateNetOfItems(true);
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
                SetPropertyValue<UnitOfMeasure>("UOM", ref _UOM, value);
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
                SetPropertyValue<decimal>("Factor", ref _Factor, value);
                if (!IsLoading)
                {
                    try
                    {
                        //((EmployeeChargeSlip)_GenJournalID).UpdateTotalItems(true);
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
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue<decimal>("Cost", ref _Cost, value); }
        }

        [Custom("AllowEdit", "False")]
        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set { SetPropertyValue<UnitOfMeasure>("BaseUOM", ref _BaseUOM, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
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
                        ((EmployeeChargeSlip)_GenJournalID).UpdateNetOfItems(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateTotalTax(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateGrossTotal(true);
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
                SetPropertyValue<decimal>("LineDiscPercent", ref _LineDiscPercent, value);
                if (!IsLoading)
                {
                    _LineDiscount = (_Quantity * _Price) * (_LineDiscPercent /
                    100);
                    OnChanged("LineDiscount");
                    try
                    {
                        ((EmployeeChargeSlip)_GenJournalID).UpdateNetOfItems(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateTotalTax(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal LineDiscount {
            get { return _LineDiscount; }
            set
            {
                SetPropertyValue<decimal>("LineDiscount", ref _LineDiscount, value);
                if (!IsLoading)
                {
                    _LineDiscPercent = (_LineDiscount / (_Quantity * _Price)) *
                    100;
                    OnChanged("LineDiscPercent");
                    try
                    {
                        ((EmployeeChargeSlip)_GenJournalID).UpdateNetOfItems(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateTotalTax(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        public SalesTaxCode Tax {
            get { return _Tax; }
            set
            {
                SetPropertyValue<SalesTaxCode>("Tax", ref _Tax, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((EmployeeChargeSlip)_GenJournalID).UpdateNetOfItems(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateTotalTax(true);
                        //((EmployeeChargeSlip)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }
        [NonPersistent]
        public EmployeeChargeSlip EmployeeChargeSlipInfo
        {
            get { return (EmployeeChargeSlip)_GenJournalID; }
        }
        [PersistentAlias("(Quantity * Price) - LineDiscount")]
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
                        Price = item.PricePerBaseUom;
                        Cost = item.CostPerBaseUom;
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = dBaseUOM.UOM;
                    Price = dBaseUOM.PricePerBaseUom;
                    Cost = dBaseUOM.CostPerBaseUom;
                }
            } else
            {
                _UOM = _ItemNo.BaseUOM;
                Price = _ItemNo.SalesPrice;
                Cost = _ItemNo.Cost;
            }
            //_Price = _BasePrice * res;
            return res;
        }
        [Custom("AllowEdit", "False")]
        //[RuleUniqueValue("", DefaultContexts.Save)]
        public Guid RequestID
        {
            get { return _RequestID; }
            set { SetPropertyValue("RequestID", ref _RequestID, value); }
        }

        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo
        {
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
        private GenJournalHeader deletedId;
        private Item itemId;
        private Requisition requisitionId;
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
            }
            else if (obj != null && genjo != null && !delete)
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
                if (gjh.SourceType.Code == "ECS")
                {
                    rwct.Quantity = (obj as EmployeeChargeSlipItemDetail).Quantity;
                }
                rwct.Save();
            }
            req01.Save();
            session.CommitTransaction();
            //this.Session.Reload(req01);

            #endregion

        }

        public EmployeeChargeSlipItemDetail(Session session)
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

        #region Get Current User

        private SecurityUser _CurrentUser;
        private PhysicalAdjustment _PhysicalAdjustmentID;
        private Guid _RequestID;
        private Requisition _RequisitionNo;
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
