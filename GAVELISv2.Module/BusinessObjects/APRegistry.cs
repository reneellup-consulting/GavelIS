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
    public class APRegistry : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private DateTime _Date;
        private Vendor _Vendor;
        private string _SourceDesc;
        private string _SourceNo;
        private string _DocNo;
        private decimal _Amount;
        private decimal _AmtPaid;
        private decimal _AmtRmn;
        private int _DaysOt;
        private decimal _ZT30Days;
        private decimal _T3T60Days;
        private decimal _T6T90Days;
        private decimal _GRT90Days;
        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader GenJournalID
        {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
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
        [Association("Vendor-APRegistry")]
        public Vendor Vendor {
            get { return _Vendor; }
            set { SetPropertyValue("Vendor", ref _Vendor, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Source")]
        public string SourceDesc {
            get { return _SourceDesc; }
            set { SetPropertyValue("SourceDesc", ref _SourceDesc, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string DocNo {
            get { return _DocNo; }
            set { SetPropertyValue("DocNo", ref _DocNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Paid")]
        public decimal AmtPaid {
            get { return _AmtPaid; }
            set { SetPropertyValue("AmtPaid", ref _AmtPaid, value);
            if (!IsLoading)
            {
                try
                {
                    _Vendor.UpdateBalance(true);
                }
                catch (Exception) { }
            }
            }
        }
        [PersistentAlias("Amount - AmtPaid")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Unpaid")]
        public decimal AmtRmn {
            get {
                object tempObject = EvaluateAlias("AmtRmn");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public int DaysOt {
            get {
                //=IF(A3<1, "",IF(A3>CURRDT,0,(A3-CURRDT)*-1))
                if (AmtRmn > 0) {
                    if (Date > DateTime.Now) {_DaysOt = 0;} else {
                        TimeSpan ts = Date - DateTime.Now;
                        _DaysOt = ts.Days * -1;
                    }
                } else {
                    _DaysOt = 0;
                }
                return _DaysOt;
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName("0-30 Days")]
        public decimal ZT30Days {
            get {
                //=IF(A3<CURRDT,IF(J3<=30,I3,0),)
                if (AmtRmn > 0) {
                    if (Date < DateTime.Now) {
                        if (DaysOt <= 30) {_ZT30Days = AmtRmn;} else {
                            _ZT30Days = 0;
                        }
                    }
                } else {
                    _ZT30Days = 0;
                }
                return _ZT30Days;
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName("30-60 Days")]
        public decimal T3T60Days {
            get {
                //=IF(A3>CURRDT,0,IF(AND(J3<=60,J3>30),I3,0))
                if (AmtRmn > 0) {
                    if (Date > DateTime.Now) {_T3T60Days = 0;} else {
                        if (DaysOt <= 60 && DaysOt > 30) {_T3T60Days = AmtRmn;
                        } else {
                            _T3T60Days = 0;
                        }
                    }
                } else {
                    _T3T60Days = 0;
                }
                return _T3T60Days;
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName("60-90 Days")]
        public decimal T6T90Days {
            get {
                //=IF(A3>CURRDT,0,IF(AND(J3<=90,J3>60),I3,0))
                if (AmtRmn > 0) {
                    if (Date > DateTime.Now) {_T6T90Days = 0;} else {
                        if (DaysOt <= 90 && _DaysOt > 60) {_T6T90Days = AmtRmn;
                        } else {
                            _T6T90Days = 0;
                        }
                    }
                } else {
                    _T6T90Days = 0;
                }
                return _T6T90Days;
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName(">90 Days")]
        public decimal GRT90Days {
            get {
                //=IF(A3>CURRDT,0,IF(J3>=90,I3,0))
                if (AmtRmn > 0) {
                    if (Date > DateTime.Now) {_GRT90Days = 0;} else {
                        if (DaysOt >= 90) {_GRT90Days = AmtRmn;} else {
                            _GRT90Days = 0;
                        }
                    }
                } else {
                    _GRT90Days = 0;
                }
                return _GRT90Days;
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
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
        public APRegistry(Session session): base(session) {
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
