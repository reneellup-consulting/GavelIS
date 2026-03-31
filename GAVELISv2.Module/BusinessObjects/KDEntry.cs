using System;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.Text;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class KDEntry : GenJournalHeader {
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private KDStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Customer _Customer;
        private Terms _Terms;
        private DateTime _DueDate;
        private DateTime _DiscountDate;
        private decimal _DiscountRate;
        private decimal _OpenAmount;
        private FATruck _TruckNo;
        private Employee _Driver;
        private FATrailer _TrailerNo;
        private TripLocation _Area;
        private decimal _KmRun;
        private decimal _KDAmount;
        private bool _RentTrailer;
        private decimal _FuelSubsidy;
        private decimal _TrailerRental;
        private decimal _Billing;
        private decimal _VATRate;
        private decimal _VATAmount;
        private decimal _GrossBilling;
        private decimal _WHTRate;
        private decimal _WHTAmount;
        private decimal _NetBilling;
        private string _Remarks;
        private string _Remarks2;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-KDEntries")]
        public GenJournalHeader TripID {
            get { return _TripID; }
            set
            {
                SetPropertyValue("TripID", ref _TripID, value);
                if (!IsLoading && _TripID != null)
                {
                    if (_TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        if (((StanfilcoTrip)_TripID).Tariff != null)
                        {
                            Period = _TripID.Period;
                            Week = _TripID.Week;
                            VATRate = ((StanfilcoTrip)_TripID).Tariff.TaxCode !=
                            null ? ((StanfilcoTrip)_TripID).Tariff.TaxCode.Rate
                             : 0;
                            WHTRate = ((StanfilcoTrip)_TripID).Tariff.
                            WHTGroupCode != null ? ((StanfilcoTrip)_TripID).
                            Tariff.WHTGroupCode.WHTRate : 0;
                            KmRun = ((StanfilcoTrip)_TripID).Tariff.KDKmRun;
                            KDAmount = ((StanfilcoTrip)_TripID).Tariff.KDAmount;
                            FuelSubsidy = ((StanfilcoTrip)_TripID).Tariff.
                            KDFuelSub;
                        }
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string TripNo {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value); }
        }

        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }

        public KDStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != KDStatusEnum.Current)
                    {
                        Approved = true;
                    } else
                    {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Customer Customer {
            get { return _Customer; }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
                if (!IsLoading && _Customer != null)
                {
                    Terms = _Customer.Terms !=
                    null ? _Customer.Terms : null;
                }
            }
        }

        public Terms Terms {
            get { return _Terms; }
            set
            {
                SetPropertyValue("Terms", ref _Terms, value);
                if (!IsLoading && _Terms != null)
                {
                    if (_Terms.DaysToPay > 0)
                    {
                        DueDate = EntryDate.Add(new
                        TimeSpan(_Terms.DaysToPay, 0, 0, 0));
                        DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
                        EarlyDaysToPay, 0, 0, 0));
                        DiscountRate = _Terms.EarlyDiscount;
                    } else
                    {
                        DueDate = DateTime.MinValue;
                    }
                }
                if (!IsLoading && _Terms == null)
                {
                    DueDate = DateTime.MinValue;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DueDate {
            get { return _DueDate; }
            set { SetPropertyValue("DueDate", ref _DueDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DiscountDate {
            get { return _DiscountDate; }
            set { SetPropertyValue("DiscountDate", ref _DiscountDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal DiscountRate {
            get { return _DiscountRate; }
            set { SetPropertyValue("DiscountRate", ref _DiscountRate, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public FATruck TruckNo {
            get { return _TruckNo; }
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }

        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); }
        }

        public TripLocation Area {
            get { return _Area; }
            set { SetPropertyValue("Area", ref _Area, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal KmRun {
            get { return _KmRun; }
            set { SetPropertyValue("KmRun", ref _KmRun, value); }
        }

        [ImmediatePostData]
        public bool RentTrailer {
            get { return _RentTrailer; }
            set
            {
                SetPropertyValue("RentTrailer", ref _RentTrailer, value);
                if (!IsLoading)
                {
                    TrailerRental = 0;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal KDAmount {
            get { return _KDAmount; }
            set { SetPropertyValue("KDAmount", ref _KDAmount, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal FuelSubsidy {
            get { return _FuelSubsidy; }
            set { SetPropertyValue("FuelSubsidy", ref _FuelSubsidy, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal TrailerRental {
            get { return _TrailerRental; }
            set { SetPropertyValue("TrailerRental", ref _TrailerRental, value); 
            }
        }

        [PersistentAlias("(KDAmount + FuelSubsidy) - TrailerRental")]
        [Custom("DisplayFormat", "n")]
        public decimal Billing {
            get
            {
                object tempObject = EvaluateAlias("Billing");
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
        public decimal VATRate {
            get { return _VATRate; }
            set { SetPropertyValue("VATRate", ref _VATRate, value); }
        }

        [PersistentAlias("Billing * (VATRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal VATAmount {
            get
            {
                object tempObject = EvaluateAlias("VATAmount");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("Billing + VATAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal GrossBilling {
            get
            {
                object tempObject = EvaluateAlias("GrossBilling");
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
        public decimal WHTRate {
            get { return _WHTRate; }
            set { SetPropertyValue("WHTRate", ref _WHTRate, value); }
        }

        [PersistentAlias("Billing * (WHTRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTAmount {
            get
            {
                object tempObject = EvaluateAlias("WHTAmount");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("GrossBilling - WHTAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal NetBilling {
            get
            {
                object tempObject = EvaluateAlias("NetBilling");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }

        public string Remarks2 {
            get { return _Remarks2; }
            set { SetPropertyValue<string>("Remarks2", ref _Remarks2, value); }
        }

        #region Aging

        //private decimal _AmtPaid;
        private int _DaysOt;
        private decimal _ZT30Days;
        private decimal _T3T60Days;
        private decimal _T6T90Days;
        private decimal _GRT90Days;

        [PersistentAlias("NetBilling - OpenAmount")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Paid")]
        //[NonPersistent]
        public decimal AmtPaid {
            get
            {
                object tempObject = EvaluateAlias("AmtPaid");
                if (tempObject != null)
                {
                    if (_OpenAmount == 0)
                    {
                        return _NetBilling;
                    } else
                    {
                        return (decimal)tempObject;
                    }
                } else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("NetBilling - AmtPaid")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Unpaid")]
        public decimal AmtRmn {
            get
            {
                object tempObject = EvaluateAlias("AmtRmn");
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
        [NonPersistent]
        public int DaysOt {
            get
            {
                //=IF(A3<1, "",IF(A3>CURRDT,0,(A3-CURRDT)*-1))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _DaysOt = 0;
                    } else
                    {
                        TimeSpan ts = EntryDate - DateTime.Now;
                        _DaysOt = ts.Days * -1;
                    }
                } else
                {
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
            get
            {
                //=IF(A3<CURRDT,IF(J3<=30,I3,0),)
                if (AmtRmn > 0)
                {
                    if (EntryDate < DateTime.Now)
                    {
                        if (DaysOt <= 30)
                        {
                            _ZT30Days = AmtRmn;
                        } else
                        {
                            _ZT30Days = 0;
                        }
                    }
                } else
                {
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
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=60,J3>30),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _T3T60Days = 0;
                    } else
                    {
                        if (DaysOt <= 60 && DaysOt > 30)
                        {
                            _T3T60Days = AmtRmn;
                        } else
                        {
                            _T3T60Days = 0;
                        }
                    }
                } else
                {
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
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=90,J3>60),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _T6T90Days = 0;
                    } else
                    {
                        if (DaysOt <= 90 && _DaysOt > 60)
                        {
                            _T6T90Days = AmtRmn;
                        } else
                        {
                            _T6T90Days = 0;
                        }
                    }
                } else
                {
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
            get
            {
                //=IF(A3>CURRDT,0,IF(J3>=90,I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _GRT90Days = 0;
                    } else
                    {
                        if (DaysOt >= 90)
                        {
                            _GRT90Days = AmtRmn;
                        } else
                        {
                            _GRT90Days = 0;
                        }
                    }
                } else
                {
                    _GRT90Days = 0;
                }
                return _GRT90Days;
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
        public MonthsEnum Month {
            get
            {
                _Month = EntryDate.Month == 1 ? MonthsEnum.January : EntryDate.
                Month
                 == 2 ? MonthsEnum.February : EntryDate.Month == 3 ? MonthsEnum.
                March : EntryDate.Month == 4 ? MonthsEnum.April : EntryDate.
                Month ==
                5 ? MonthsEnum.May : EntryDate.Month == 6 ? MonthsEnum.June :
                EntryDate.Month == 7 ? MonthsEnum.July : EntryDate.Month == 8 ?
                MonthsEnum.August : EntryDate.Month == 9 ? MonthsEnum.September
                 : EntryDate.Month == 10 ? MonthsEnum.October : EntryDate.Month 
                == 11
                 ? MonthsEnum.November : EntryDate.Month == 12 ? MonthsEnum.
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
        [Custom("DisplayFormat", "d")]
        public int Year {
            get
            {
                return EntryDate.Year;
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
        private bool _Marked = false;
        private int _BillSeq;
        private StanfilcoTrip _DtrNo;
        [Custom("AllowEdit", "False")]
        public bool Marked
        {
            get { return _Marked; }
            set { SetPropertyValue("Marked", ref _Marked, value); }
        }

        [Action(AutoCommit = true, Caption = "Mark for Process")]
        public void MarkForProcess()
        {
            Marked = true;
        }
        [Action(AutoCommit = true, Caption = "Unmark for Process")]
        public void UnmarkForProcess()
        {
            Marked = false;
        }
        public int BillSeq
        {
            get { return _BillSeq; }
            set { SetPropertyValue("BillSeq", ref _BillSeq, value); }
        }
        [NonPersistent]
        public int GKey
        {
            get { return this.Oid; }
        }
        [NonPersistent]
        public StanfilcoTrip DtrNo
        {
            get
            {
                if (TripID.GetType() == typeof(StanfilcoTrip))
                {
                    _DtrNo = (StanfilcoTrip)TripID;
                }
                else
                {
                    _DtrNo = null;
                }
                return _DtrNo;
            }
        }
        public string Particulars
        {
            get
            {
                return string.Format("{0}@{1}", _Remarks, _Area!=null?_Area.Code:string.Empty);
            }
        }

        public KDEntry(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            RowID = Guid.NewGuid();
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "KD"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "KD"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "KD"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Memo = "Knockdown Boxes #" + SourceNo;
        }
        protected override void OnSaving()
        {
            //this.AutoRegisterIncomeExpenseVer();
            base.OnSaving();
        }
        protected override void OnSaved()
        {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }
        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved KD transactions."
                );
            }
        }

        protected override void TriggerObjectChanged(ObjectChangeEventArgs args) 
        {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
            base.TriggerObjectChanged(args);
            if (args.PropertyName == "EntryDate")
            {
                if (!IsLoading && _Terms != null)
                {
                    DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0, 0,
                    0));
                    DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
                    EarlyDaysToPay, 0, 0, 0));
                    DiscountRate = _Terms.EarlyDiscount;
                }
            }
        }
    }
}
