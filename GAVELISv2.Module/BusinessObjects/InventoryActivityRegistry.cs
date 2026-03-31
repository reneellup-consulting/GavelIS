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
    public class InventoryActivityRegistry : XPObject {
        private Guid _RowID;
        private DateTime _OrderDate;
        private OrderTypeEnum _OrderType;
        private DateTime _ExpectedDate;
        private Contact _Partner;
        private string _SourceNo;
        private Item _Item;
        private ItemCategory _Category;
        private decimal _UnitPrice;
        private decimal _Quantity;
        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Persistent("Availability")]
        private decimal? _Availability;
        private decimal _OnHand;
        [Persistent("PendingPO")]
        private decimal? _PendingPO;
        [Persistent("PendingSO")]
        private decimal? _PendingSO;
        private bool _Calculated = false;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime OrderDate {
            get { return _OrderDate; }
            set {
                SetPropertyValue("OrderDate", ref _OrderDate, value);
                if (!IsLoading) {
                    Month = _OrderDate.Month == 1 ? MonthsEnum.January : 
                    _OrderDate.Month == 2 ? MonthsEnum.February : _OrderDate.
                    Month == 3 ? MonthsEnum.March : _OrderDate.Month == 4 ? 
                    MonthsEnum.April : _OrderDate.Month == 5 ? MonthsEnum.May : 
                    _OrderDate.Month == 6 ? MonthsEnum.June : _OrderDate.Month 
                    == 7 ? MonthsEnum.July : _OrderDate.Month == 8 ? MonthsEnum.
                    August : _OrderDate.Month == 9 ? MonthsEnum.September : 
                    _OrderDate.Month == 10 ? MonthsEnum.October : _OrderDate.
                    Month == 11 ? MonthsEnum.December : _OrderDate.Month == 12 ? 
                    MonthsEnum.December : MonthsEnum.None;
                    switch (_Month) {
                        case MonthsEnum.None:
                            break;
                        case MonthsEnum.January:
                            Quarter = "1st QTR";
                            break;
                        case MonthsEnum.February:
                            Quarter = "1st QTR";
                            break;
                        case MonthsEnum.March:
                            Quarter = "1st QTR";
                            break;
                        case MonthsEnum.April:
                            Quarter = "2nd QTR";
                            break;
                        case MonthsEnum.May:
                            Quarter = "2nd QTR";
                            break;
                        case MonthsEnum.June:
                            Quarter = "2nd QTR";
                            break;
                        case MonthsEnum.July:
                            Quarter = "3rd QTR";
                            break;
                        case MonthsEnum.August:
                            Quarter = "3rd QTR";
                            break;
                        case MonthsEnum.September:
                            Quarter = "3rd QTR";
                            break;
                        case MonthsEnum.October:
                            Quarter = "4th QTR";
                            break;
                        case MonthsEnum.November:
                            Quarter = "4th QTR";
                            break;
                        case MonthsEnum.December:
                            Quarter = "4th QTR";
                            break;
                        default:
                            break;
                    }
                    Year = OrderDate.Year;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public OrderTypeEnum OrderType {
            get { return _OrderType; }
            set { SetPropertyValue("OrderType", ref _OrderType, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ExpectedDate {
            get { return _ExpectedDate; }
            set { SetPropertyValue("ExpectedDate", ref _ExpectedDate, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Contact Partner {
            get { return _Partner; }
            set { SetPropertyValue("Partner", ref _Partner, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Item Item {
            get { return _Item; }
            set { SetPropertyValue("Item", ref _Item, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public ItemCategory Category {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal UnitPrice {
            get { return _UnitPrice; }
            set { SetPropertyValue("UnitPrice", ref _UnitPrice, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value); }
        }
        [PersistentAlias("UnitPrice * Quantity")]
        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get {
                object tempObject = EvaluateAlias("Amount");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public MonthsEnum Month {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Quarter {
            get { return _Quarter; }
            set { SetPropertyValue("Quarter", ref _Quarter, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public int Year {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public string MonthSorter {
            get {
                switch (_Month) {
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
        //[Custom("AllowEdit", "False")]
        //[NonPersistent]
        //public decimal Availability
        //{
        //    get
        //    {
        //        XPClassInfo regInfo = Session.GetClassInfo<InventoryActivityRegistry>();
        //        SortingCollection sort = new SortingCollection(null);
        //        sort.Add(new SortProperty("[OrderDate]", DevExpress.Xpo.DB.SortingDirection.Ascending));
        //        System.Collections.ICollection regsPO=Session.GetObjects(regInfo,
        //            CriteriaOperator.Parse("[Item.No] = '"+ _Item.No +"' And [OrderType] = 'Purchase' And [ExpectedDate] <= #"+ _ExpectedDate.ToShortDateString() +"#"),
        //            sort,0,false,false);
        //        System.Collections.ICollection regsSO = Session.GetObjects(regInfo,
        //            CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Sale' And [ExpectedDate] <= #" + _ExpectedDate.ToShortDateString() + "#"),
        //            sort, 0, false, false);
        //        decimal poTot = 0m;
        //        foreach (InventoryActivityRegistry po in regsPO)
        //        {
        //            poTot = poTot + po.Quantity;
        //        }
        //        decimal soTot = 0m;
        //        foreach (InventoryActivityRegistry so in regsSO)
        //        {
        //            soTot = soTot + so.Quantity;
        //        }
        //         _Availability = poTot - soTot;
        //        return _Availability;
        //    }
        //}
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public decimal OnHand
        {
            get
            {
                switch (_OrderType)
                {
                    case OrderTypeEnum.Purchase:
                        _OnHand = _Quantity;
                        break;
                    case OrderTypeEnum.Sale:
                        _OnHand = 0 - _Quantity;
                        break;
                    default:
                        break;
                }
                return _OnHand;
            }
        }
        //[Custom("AllowEdit", "False")]
        //public decimal PendingPO
        //{
        //    get
        //    {
        //        XPClassInfo regInfo = Session.GetClassInfo<InventoryActivityRegistry>();
        //        SortingCollection sort = new SortingCollection(null);
        //        sort.Add(new SortProperty("[OrderDate]", DevExpress.Xpo.DB.SortingDirection.Ascending));
        //        System.Collections.ICollection regsPO = Session.GetObjects(regInfo,
        //            CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Purchase' And [ExpectedDate] >= #" + DateTime.Now.ToShortDateString() + "#"),
        //            sort, 0, false, false);
        //        decimal poTot = 0m;
        //        foreach (InventoryActivityRegistry po in regsPO)
        //        {
        //            poTot = poTot + po.Quantity;
        //        }
        //        _PendingPO = poTot;
        //        return _PendingPO;
        //    }
        //}
        //[Custom("AllowEdit", "False")]
        //public decimal PendingSO
        //{
        //    get
        //    {
        //        XPClassInfo regInfo = Session.GetClassInfo<InventoryActivityRegistry>();
        //        SortingCollection sort = new SortingCollection(null);
        //        sort.Add(new SortProperty("[OrderDate]", DevExpress.Xpo.DB.SortingDirection.Ascending));
        //        System.Collections.ICollection regsSO = Session.GetObjects(regInfo,
        //            CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Sale' And [ExpectedDate] >= #" + DateTime.Now.ToShortDateString() + "#"),
        //            sort, 0, false, false);
        //        decimal soTot = 0m;
        //        foreach (InventoryActivityRegistry so in regsSO)
        //        {
        //            soTot = soTot + so.Quantity;
        //        }
        //        _PendingSO=soTot;
        //        return _PendingSO;
        //    }
        //}
        [PersistentAlias("_Availability")]
        [Custom("DisplayFormat", "n")]
        public decimal? Availability {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Availability == null) {
                        UpdateAvailability(false);}
                } catch (Exception) {
                }
                return _Availability;
            }
        }
        public void UpdateAvailability(bool forceChangeEvent)
        {
            decimal? oldValue = _Availability;
            decimal tempValue = 0m;
            XPClassInfo regInfo = Session.GetClassInfo<InventoryActivityRegistry>();
            SortingCollection sort = new SortingCollection(null);
            sort.Add(new SortProperty("[OrderDate]", DevExpress.Xpo.DB.SortingDirection.Ascending));
            System.Collections.ICollection regsPO = Session.GetObjects(regInfo,
                CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Purchase' And [ExpectedDate] <= #" + _ExpectedDate.ToShortDateString() + "#"),
                sort, 0, false, false);
            System.Collections.ICollection regsSO = Session.GetObjects(regInfo,
                CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Sale' And [ExpectedDate] <= #" + _ExpectedDate.ToShortDateString() + "#"),
                sort, 0, false, false);
            decimal poTot = 0m;
            foreach (InventoryActivityRegistry po in regsPO)
            {
                poTot = poTot + po.Quantity;
            }
            decimal soTot = 0m;
            foreach (InventoryActivityRegistry so in regsSO)
            {
                soTot = soTot + so.Quantity;
            }
            tempValue = poTot - soTot;

            _Availability = tempValue;
            if (forceChangeEvent) { OnChanged("Availability", Availability, _Availability); }
            ;
        }

        [PersistentAlias("_PendingPO")]
        [Custom("DisplayFormat", "n")]
        public decimal? PendingPO {
            get {
                try {
                    if (!IsLoading && !IsSaving && _PendingPO == null) {
                        UpdatePendingPO(false);}
                } catch (Exception) {
                }
                return _PendingPO;
            }
        }
        public void UpdatePendingPO(bool forceChangeEvent)
        {
            decimal? oldValue = _PendingPO;
            decimal tempValue = 0m;
            XPClassInfo regInfo = Session.GetClassInfo<InventoryActivityRegistry>();
            SortingCollection sort = new SortingCollection(null);
            sort.Add(new SortProperty("[OrderDate]", DevExpress.Xpo.DB.SortingDirection.Ascending));
            System.Collections.ICollection regsPO = Session.GetObjects(regInfo,
                CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Purchase' And [ExpectedDate] >= #" + DateTime.Now.ToShortDateString() + "#"),
                sort, 0, false, false);
            decimal poTot = 0m;
            foreach (InventoryActivityRegistry po in regsPO)
            {
                poTot = poTot + po.Quantity;
            }
            tempValue = poTot;

            _PendingPO = tempValue;
            if (forceChangeEvent) { OnChanged("PendingPO", PendingPO, _PendingPO); }
            ;
        }

        [PersistentAlias("_PendingSO")]
        [Custom("DisplayFormat", "n")]
        public decimal? PendingSO {
            get {
                try {
                    if (!IsLoading && !IsSaving && _PendingSO == null) {
                        UpdatePendingSO(false);}
                } catch (Exception) {
                }
                return _PendingSO;
            }
        }
        public void UpdatePendingSO(bool forceChangeEvent)
        {
            decimal? oldValue = _PendingSO;
            decimal tempValue = 0m;
            XPClassInfo regInfo = Session.GetClassInfo<InventoryActivityRegistry>();
            SortingCollection sort = new SortingCollection(null);
            sort.Add(new SortProperty("[OrderDate]", DevExpress.Xpo.DB.SortingDirection.Ascending));
            System.Collections.ICollection regsSO = Session.GetObjects(regInfo,
                CriteriaOperator.Parse("[Item.No] = '" + _Item.No + "' And [OrderType] = 'Sale' And [ExpectedDate] >= #" + DateTime.Now.ToShortDateString() + "#"),
                sort, 0, false, false);
            decimal soTot = 0m;
            foreach (InventoryActivityRegistry so in regsSO)
            {
                soTot = soTot + so.Quantity;
            }
            tempValue = soTot;

            _PendingSO = tempValue;
            if (forceChangeEvent) { OnChanged("PendingSO", PendingSO, _PendingSO); }
            ;
        }

        //[Custom("AllowEdit", "False")]
        public bool Calculated {
            get { return _Calculated; }
            set { SetPropertyValue("Calculated", ref _Calculated, value); }
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
        public InventoryActivityRegistry(Session session): base(session) {
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
        private void Calculate() {
            #region Availability
            _Availability = new decimal();
            #endregion
            #region OnHand
            _OnHand = new decimal();
            #endregion
            #region PendingPO
            _PendingPO = new decimal();
            #endregion
            #region PendingSO
            _PendingSO = new decimal();
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
