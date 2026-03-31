using System;
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
    public enum TireIssueReleasedStatus
    {
        [ImageName("State_Validation_Skipped")]
        Open,
        [ImageName("State_Validation_Valid")]
        Released
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [System.ComponentModel.DefaultProperty("RwsTireDetId")]
    public class RwsTireDetail : XPObject {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string RwsTireDetId
        {
            get { return string.Format("{0:RWST00000000}", Oid); }
        }
        private Guid _RowID;
        private RequisitionWorksheet _ReqWorksheetId;
        private DateTime _IssueDate =  DateTime.Now;
        private InventoryControlJournal _InvControlId;
        private Warehouse _FromWarehouse;
        private ExpenseType _ExpToReverse;
        private ExpenseType _SubExpToReverse;
        private decimal _OdoRead;
        private TireItem _OldTireItem;
        private string _RepTireBranding;
        private ItemTrackingEntry _Replacement;
        private string _SerialNo;
        private TireItem _NewTireItem;
        private string _InvoiceNo;
        private decimal _Cost;
        //private decimal _Discount;
        //private decimal _TotalCost;
        private Tire _TireIssueId;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("RwsTireDetails")]
        public RequisitionWorksheet ReqWorksheetId {
            get { return _ReqWorksheetId; }
            set
            {
                SetPropertyValue<RequisitionWorksheet>("ReqWorksheetId", ref _ReqWorksheetId, value);
                if (!IsLoading && !IsSaving && _ReqWorksheetId != null)
                {
                    OdoRead = _ReqWorksheetId.LastRtdOdo;
                    OldTireItem = _ReqWorksheetId.LastRtdItem;
                    RepTireBranding = _ReqWorksheetId.LastRtdBranding;
                }
            }
        }
        public DateTime IssueDate
        {
            get { return _IssueDate; }
            set { SetPropertyValue("IssueDate", ref _IssueDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public InventoryControlJournal InvControlId
        {
            get { return _InvControlId; }
            set { SetPropertyValue("InvControlId", ref _InvControlId, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public Warehouse FromWarehouse
        {
            get { return _FromWarehouse; }
            set { SetPropertyValue("FromWarehouse", ref _FromWarehouse, value); }
        }
        [Custom("AllowEdit", "False")]
        public ExpenseType ExpToReverse
        {
            get { return _ExpToReverse; }
            set { SetPropertyValue("ExpToReverse", ref _ExpToReverse, value); }
        }
        [Custom("AllowEdit", "False")]
        public ExpenseType SubExpToReverse
        {
            get { return _SubExpToReverse; }
            set { SetPropertyValue("SubExpToReverse", ref _SubExpToReverse, value); }
        }
        
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Odometer")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoRead {
            get { return _OdoRead; }
            set { SetPropertyValue<decimal>("OdoRead", ref _OdoRead, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        //[DisplayName("Truck/Trailer")]
        //public FixedAsset Fleet {
        //    get { return _Fleet; }
        //    set { SetPropertyValue<FixedAsset>("Fleet", ref _Fleet, value);
        //    }
        //}
        //[RuleRequiredField("", DefaultContexts.Save)]
        public TireItem OldTireItem {
            get { return _OldTireItem; }
            set { SetPropertyValue<TireItem>("OldTireItem", ref _OldTireItem, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Branding of Replaced")]
        public string RepTireBranding {
            get { return _RepTireBranding; }
            set { SetPropertyValue<string>("RepTireBranding", ref _RepTireBranding, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public ItemTrackingEntry Replacement {
            get { return _Replacement; }
            set { SetPropertyValue<ItemTrackingEntry>("Replacement", ref _Replacement, value); }
        }

        [Custom("AllowEdit", "False")]
        public string SerialNo {
            get { return _SerialNo; }
            set { SetPropertyValue<string>("SerialNo", ref _SerialNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public TireItem NewTireItem {
            get { return _NewTireItem; }
            set { SetPropertyValue<TireItem>("NewTireItem", ref _NewTireItem, value); }
        }

        [Custom("AllowEdit", "False")]
        public string InvoiceNo {
            get { return _InvoiceNo; }
            set { SetPropertyValue<string>("InvoiceNo", ref _InvoiceNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue<decimal>("Cost", ref _Cost, value); }
        }

        //[Custom("AllowEdit", "False")]
        //public decimal Discount {
        //    get { return _Discount; }
        //    set { SetPropertyValue<decimal>("Discount", ref _Discount, value); }
        //}

        //public decimal TotalCost {
        //    get { return _TotalCost; }
        //    set { SetPropertyValue<decimal>("TotalCost", ref _TotalCost, value); }
        //}

        [Custom("AllowEdit", "False")]
        public Tire TireIssueId {
            get { return _TireIssueId; }
            set { SetPropertyValue<Tire>("TireIssueId", ref _TireIssueId, value); }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public TireIssueReleasedStatus ReleaseStatus {
            get { return _TireIssueId != null ? TireIssueReleasedStatus.Released : TireIssueReleasedStatus.Open; }
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

        public RwsTireDetail(Session session)
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

        protected override void OnDeleting()
        {
            if (_TireIssueId != null)
            {
                throw new
                    UserFriendlyException(
                    "The system prohibits the deletion of already released tire."
                    );
            }
            if (_Replacement != null)
            {
                throw new
                    UserFriendlyException(
                    "The system prohibits the deletion of already issued tire."
                    );
            }

            base.OnDeleting();
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
                _Month = IssueDate.Month == 1 ? MonthsEnum.January : IssueDate.Month
                 == 2 ? MonthsEnum.February : IssueDate.Month == 3 ? MonthsEnum.
                March : IssueDate.Month == 4 ? MonthsEnum.April : IssueDate.Month ==
                5 ? MonthsEnum.May : IssueDate.Month == 6 ? MonthsEnum.June :
                IssueDate.Month == 7 ? MonthsEnum.July : IssueDate.Month == 8 ?
                MonthsEnum.August : IssueDate.Month == 9 ? MonthsEnum.September
                 : IssueDate.Month == 10 ? MonthsEnum.October : IssueDate.Month == 11
                 ? MonthsEnum.November : IssueDate.Month == 12 ? MonthsEnum.
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
        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get
            {
                return IssueDate.Year;
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

    }

}
