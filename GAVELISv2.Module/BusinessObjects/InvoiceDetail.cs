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
    public class InvoiceDetail : XPObject, IToMovementCapable
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
        private decimal _BasePrice;
        private decimal _Price;
        private decimal _LineDiscPercent;
        private decimal _LineDiscount;
        private SalesTaxCode _Tax;
        //private decimal _Total;
        private decimal _Ordered;
        private decimal _Invoiced;
        private decimal _Returned;
        private SalesOrderDetail _SalesOrderDetailID;
        private Guid _SalesRegistryRowID;
        private CostCenter _CostCenter;
        private Requisition _RequisitionNo;
        private Employee _RequestedBy;
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
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value); }
        }

        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set { SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value); }
        }

        [NonPersistent]
        public Invoice InvoiceInfo
        {
            get { return (Invoice)_GenJournalID; }
        }

        [NonPersistent]
        public Company CompanyInfo
        {
            get { return Company.GetInstance(Session); }
        }

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-InvoiceDetails")]
        public GenJournalHeader GenJournalID
        {
            get { return _GenJournalID; }
            set
            {
                GenJournalHeader oldGenJournalID = _GenJournalID;
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && oldGenJournalID != _GenJournalID)
                {
                    oldGenJournalID = oldGenJournalID ?? _GenJournalID;
                    if (_GenJournalID != null && _GenJournalID.GetType() == typeof(Invoice) && ((Invoice)_GenJournalID).InvoiceType == InvoiceTypeEnum.
                    OrderSlip)
                    {
                        Tax = CompanyInfo.DefaultNonSalesTax != null ?
                        CompanyInfo.DefaultNonSalesTax : null;
                    }
                    else
                    {
                        if (_ItemNo != null)
                        {
                            Tax = _ItemNo.TaxCode != null ? _ItemNo.TaxCode :
                            null;
                        }
                    }
                    ((Invoice)oldGenJournalID).UpdateNet(true);
                    ((Invoice)oldGenJournalID).UpdateTotalTax(true);
                    ((Invoice)oldGenJournalID).UpdateGrossTotal(true);
               }
            }
        }

        public Item ItemNo
        {
            get { return _ItemNo; }
            set
            {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    Warehouse = _ItemNo.WarehouseLocation;
                    Description = !string.IsNullOrEmpty(_ItemNo.SalesDescription
                    ) ? _ItemNo.SalesDescription : _ItemNo.Description;
                    if (_GenJournalID != null && ((Invoice)_GenJournalID).
                    InvoiceType == InvoiceTypeEnum.
                    OrderSlip)
                    {
                        Tax = CompanyInfo.DefaultNonSalesTax != null ?
                        CompanyInfo.DefaultNonSalesTax : null;
                    }
                    else
                    {
                        Tax = _ItemNo.TaxCode != null ? _ItemNo.TaxCode : null;
                    }
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
                        Price = _ItemNo.SalesPrice;
                        BasePrice = _ItemNo.SalesPrice / _Factor;
                    }
                    //CurrentQtyBase = _ItemNo.GetWarehouseQtyBase(_Warehouse, this.GenJournalID.EntryDate) /
                    //_Factor;
                }
            }
        }

        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Warehouse Warehouse
        {
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
                    //GetWarehouseQtyBaseCorrected(_Warehouse, this.GenJournalID.Oid, this.GenJournalID.EntryDate, "IN", RequisitionNo) / _Factor;
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
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((Invoice)_GenJournalID).UpdateNet(true);
                        ((Invoice)_GenJournalID).UpdateTotalTax(true);
                        ((Invoice)_GenJournalID).UpdateGrossTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM
        {
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
        public decimal Factor
        {
            get { return _Factor; }
            set
            {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((Invoice)_GenJournalID).UpdateNet(true);
                        ((Invoice)_GenJournalID).UpdateTotalTax(true);
                        ((Invoice)_GenJournalID).UpdateGrossTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [PersistentAlias("Quantity * Factor")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseQTY
        {
            get
            {
                object tempObject = EvaluateAlias("BaseQTY");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public UnitOfMeasure BaseUOM
        {
            get { return _BaseUOM; }
            set { SetPropertyValue("BaseUOM", ref _BaseUOM, value); }
        }

        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public decimal Cost
        {
            get
            {
                if (_ItemNo != null && !string.IsNullOrEmpty(_RowID.ToString()))
                {
                    return _ItemNo.GetInvoiceCost(_RowID.ToString());
                }
                else
                    return 0;
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BasePrice
        {
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
        public decimal Price
        {
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
                        ((Invoice)_GenJournalID).UpdateNet(true);
                        ((Invoice)_GenJournalID).UpdateTotalTax(true);
                        ((Invoice)_GenJournalID).UpdateGrossTotal(true);
                    }
                    catch (Exception)
                    {
                    }
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
                    _LineDiscount = (_Quantity * _Price) * (_LineDiscPercent /
                    100);
                    OnChanged("LineDiscount");
                    try
                    {
                        ((Invoice)_GenJournalID).UpdateNet(true);
                        ((Invoice)_GenJournalID).UpdateTotalTax(true);
                        ((Invoice)_GenJournalID).UpdateGrossTotal(true);
                    }
                    catch (Exception)
                    {
                    }
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
                    _LineDiscPercent = (_LineDiscount / (_Quantity * _Price)) *
                    100;
                    OnChanged("LineDiscPercent");
                    try
                    {
                        ((Invoice)_GenJournalID).UpdateNet(true);
                        ((Invoice)_GenJournalID).UpdateTotalTax(true);
                        ((Invoice)_GenJournalID).UpdateGrossTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public SalesTaxCode Tax
        {
            get { return _Tax; }
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((Invoice)_GenJournalID).UpdateNet(true);
                        ((Invoice)_GenJournalID).UpdateTotalTax(true);
                        ((Invoice)_GenJournalID).UpdateGrossTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [PersistentAlias("(Quantity * Price) - LineDiscount")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Ordered
        {
            get { return _Ordered; }
            set { SetPropertyValue("Ordered", ref _Ordered, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Invoiced
        {
            get { return _Invoiced; }
            set
            {
                SetPropertyValue("Invoiced", ref _Invoiced, value);
                // Update IH Parts Sales Registry
                //if (!IsLoading && _GenJournalID!=null && ((Invoice)_GenJournalID).Status==InvoiceStatusEnum.Invoiced)
                //{
                //    if (_SalesRegistryRowID==null)
                //    {
                //        IHPartsSalesRegistry registry = new IHPartsSalesRegistry(this.Session);
                //        registry.Date = _GenJournalID.EntryDate;
                //        registry.Customer = ((Invoice)_GenJournalID).Customer;
                //        registry.Item = _ItemNo;
                //        registry.Quantity = _Invoiced;
                //        registry.UnitCost = _ItemNo.Cost;
                //        registry.UnitPrice = _Price;
                //        registry.Save();
                //    }
                //}
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Returned
        {
            get { return _Returned; }
            set
            {
                SetPropertyValue("Returned", ref _Returned, value);
                if (!IsLoading)
                {
                    if (_SalesRegistryRowID == null)
                    {
                        IHPartsSalesRegistry registry = this.Session.FindObject<
                        IHPartsSalesRegistry>(CriteriaOperator.Parse(
                        "[RowID] = '" + _SalesRegistryRowID + "'"));
                        registry.Quantity = _Quantity - _Returned;
                        registry.Save();
                    }
                }
                // Update IH Parts Sales Registry
                //if (!IsLoading && _GenJournalID != null && (((Invoice)_GenJournalID).Status == InvoiceStatusEnum.PartiallyReturned || ((Invoice)_GenJournalID).Status == InvoiceStatusEnum.Returned))
                //{
                //    if (_SalesRegistryRowID == null)
                //    {
                //        IHPartsSalesRegistry registry = this.Session.FindObject<IHPartsSalesRegistry>(CriteriaOperator.Parse("[RowID] = '"+ _SalesRegistryRowID +"'"));
                //        registry.Date = _GenJournalID.EntryDate;
                //        registry.Customer = ((Invoice)_GenJournalID).Customer;
                //        registry.Item = _ItemNo;
                //        registry.Quantity = _Returned;
                //        registry.UnitCost = _ItemNo.Cost;
                //        registry.UnitPrice = _Price;
                //        registry.Save();
                //    }
                //}

            }
        }

        [PersistentAlias("Quantity - Returned")]
        [Custom("DisplayFormat", "n")]
        public decimal AQty
        {
            get
            {
                object tempObject = EvaluateAlias("AQty");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("AQty * Cost")]
        [Custom("DisplayFormat", "n")]
        public decimal ATotalCost
        {
            get
            {
                object tempObject = EvaluateAlias("ATotalCost");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("AQty * Price")]
        [Custom("DisplayFormat", "n")]
        public decimal ARevenue
        {
            get
            {
                object tempObject = EvaluateAlias("ARevenue");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public SalesOrderDetail SalesOrderDetailID
        {
            get { return _SalesOrderDetailID; }
            set
            {
                SetPropertyValue("SalesOrderDetailID", ref _SalesOrderDetailID
                    , value);
            }
        }

        [Custom("AllowEdit", "False")]
        public Guid SalesRegistryRowID
        {
            get { return _SalesRegistryRowID; }
            set
            {
                SetPropertyValue("SalesRegistryRowID", ref _SalesRegistryRowID
                    , value);
            }
        }


        public decimal GetFactor()
        {
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
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = dBaseUOM.UOM;
                    Price = dBaseUOM.PricePerBaseUom;
                }
            }
            else
            {
                _UOM = _ItemNo.BaseUOM;
                Price = _ItemNo.SalesPrice;
            }
            //_Price = _BasePrice * res;
            return res;
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

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        [Aggregated,
        Association("InvoiceDetail-InvoiceDetailTrackingLines")]
        public XPCollection<InvoiceDetailTrackingLine>
        InvoiceDetailTrackingLines
        {
            get
            {
                return GetCollection<
                    InvoiceDetailTrackingLine>("InvoiceDetailTrackingLines");
            }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month
        {
            get
            {
                _Month = InvoiceInfo.EntryDate.Month == 1 ? MonthsEnum.January :
                InvoiceInfo.EntryDate.Month
                 == 2 ? MonthsEnum.February : InvoiceInfo.EntryDate.Month == 3 ?
                MonthsEnum.
                March : InvoiceInfo.EntryDate.Month == 4 ? MonthsEnum.April :
                InvoiceInfo.EntryDate.Month ==
                5 ? MonthsEnum.May : InvoiceInfo.EntryDate.Month == 6 ?
                MonthsEnum.June :
                InvoiceInfo.EntryDate.Month == 7 ? MonthsEnum.July : InvoiceInfo
                .EntryDate.Month == 8 ?
                MonthsEnum.August : InvoiceInfo.EntryDate.Month == 9 ?
                MonthsEnum.September
                 : InvoiceInfo.EntryDate.Month == 10 ? MonthsEnum.October :
                InvoiceInfo.EntryDate.Month == 11
                 ? MonthsEnum.November : InvoiceInfo.EntryDate.Month == 12 ?
                MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string Quarter
        {
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
        public int Year
        {
            get
            {
                return InvoiceInfo.EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string MonthSorter
        {
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

        #region Sell Battery
        private Battery _BatterySoldId;
        [Custom("AllowEdit", "False")]
        public Battery BatterySoldId
        {
            get { return _BatterySoldId; }
            set { SetPropertyValue("BatterySoldId", ref _BatterySoldId, value); }
        }
        private RevolvingPart _RevPartSoldId;
        [Custom("AllowEdit", "False")]
        public RevolvingPart RevPartSoldId
        {
            get { return _RevPartSoldId; }
            set { SetPropertyValue("RevPartSoldId", ref _RevPartSoldId, value); }
        }

        #endregion

        public InvoiceDetail(Session session)
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
        private int deletedOid;
        protected override void OnDeleting()
        {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
            }
            base.OnDeleting();
        }
        protected override void OnSaving()
        {
            if (IsDeleted)
            {
                //if (SalesOrderDetailID != null)
                //{
                //    SalesOrderDetailID.InvDetID = null;
                //    SalesOrderDetailID.Save();

                //    SalesOrderDetailID = null;
                //}
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

            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }

            #endregion

        }

        //protected override void OnLoaded() {
        //    base.OnLoaded();
        //    //_CurrentQtyBase = _ItemNo.
        //    //GetWarehouseQtyBase(_Warehouse) / _Factor;
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
