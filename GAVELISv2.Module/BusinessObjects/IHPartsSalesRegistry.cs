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
    public class IHPartsSalesRegistry : XPObject {
        private Guid _RowID;
        private DateTime _Date;
        private Customer _Customer;
        private Item _Item;
        private decimal _UnitCost;
        private decimal _UnitPrice;
        private decimal _Quantity;
        private decimal _PlanCustomer;
        private decimal _PlanItem;
        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        //[Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime Date {
            get { return _Date; }
            set {
                SetPropertyValue("Date", ref _Date, value);
                if (!IsLoading) {
                    Month = _Date.Month == 1 ? MonthsEnum.January : _Date.Month 
                    == 2 ? MonthsEnum.February : _Date.Month == 3 ? MonthsEnum.
                    March : _Date.Month == 4 ? MonthsEnum.April : _Date.Month == 
                    5 ? MonthsEnum.May : _Date.Month == 6 ? MonthsEnum.June : 
                    _Date.Month == 7 ? MonthsEnum.July : _Date.Month == 8 ? 
                    MonthsEnum.August : _Date.Month == 9 ? MonthsEnum.September 
                    : _Date.Month == 10 ? MonthsEnum.October : _Date.Month == 11 
                    ? MonthsEnum.December : _Date.Month == 12 ? MonthsEnum.
                    December : MonthsEnum.None;
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
                    Year = Date.Year;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Customer Customer {
            get { return _Customer; }
            set { SetPropertyValue("Customer", ref _Customer, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Item Item {
            get { return _Item; }
            set { SetPropertyValue("Item", ref _Item, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal UnitCost {
            get { return _UnitCost; }
            set { SetPropertyValue("UnitCost", ref _UnitCost, value); }
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
        public decimal PlanCustomer {
            get { return _PlanCustomer; }
            set { SetPropertyValue("PlanCustomer", ref _PlanCustomer, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal PlanItem {
            get { return _PlanItem; }
            set { SetPropertyValue("PlanItem", ref _PlanItem, value); }
        }
        [PersistentAlias("Amount - (UnitCost * Quantity)")]
        [Custom("DisplayFormat", "n")]
        public decimal Revenue {
            get {
                object tempObject = EvaluateAlias("Revenue");
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
        public IHPartsSalesRegistry(Session session): base(session) {
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
    }
}
