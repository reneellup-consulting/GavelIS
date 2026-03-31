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
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class ShuntingEntry : GenJournalHeader {
        private StanfilcoTrip _DtrNo;
        private decimal _Distance;
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private ShuntingStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Customer _Customer;
        private Terms _Terms;
        private DateTime _DueDate;
        private DateTime _DiscountDate;
        private decimal _DiscountRate;
        private decimal _OpenAmount;
        private TripLocation _ShuntingTo;
        private FATruck _TruckNo;
        private Employee _Driver;
        private FATrailer _TrailerNo;
        private FAGeneratorSet _GensetNo;
        //private decimal _PreOdoRead;
        //private decimal _PostOdoRead;
        //private decimal _KMRunOdo;
        [Persistent("KMRunMnl")]
        private decimal? _KMRunMnl;
        [Persistent("Excess")]
        private decimal? _Excess;
        [Persistent("Additional")]
        private decimal? _Additional;
        //private decimal _TotalKms;
        private decimal _RatePerKms;
        private decimal _FuelSubsidy;
        private decimal _ChassyRental;
        private decimal _Insurance;
        //private decimal _Total;
        private decimal _VATRate;
        //private decimal _VATAmount;
        //private decimal _GrossBilling;
        private decimal _WHTRate;
        //private decimal _WHTAmount;
        //private decimal _NetBilling;
        private string _Remarks;
        //private FuelReceipt _FuelReceiptNo;
        //private decimal _FuelQty;
        //private decimal _FuelCost;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-ShuntingEntries")]
        public GenJournalHeader TripID {
            get { return _TripID; }
            set {
                SetPropertyValue("TripID", ref _TripID, value);
                if (!IsLoading && _TripID != null) {
                    if (_TripID.GetType() == typeof(StanfilcoTrip)) {
                        if (((StanfilcoTrip)_TripID).Tariff != null) {
                            Period = _TripID.Period;
                            Week = _TripID.Week;
                            RatePerKms = ((StanfilcoTrip)_TripID).Tariff.
                            ShuntingRatePerKm;
                            VATRate = ((StanfilcoTrip)_TripID).Tariff.TaxCode != 
                            null ? ((StanfilcoTrip)_TripID).Tariff.TaxCode.Rate 
                            : 0;
                            WHTRate = ((StanfilcoTrip)_TripID).Tariff.
                            WHTGroupCode != null ? ((StanfilcoTrip)_TripID).
                            Tariff.WHTGroupCode.WHTRate : 0;
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
        public ShuntingStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != ShuntingStatusEnum.Current) {Approved = true;
                    } else {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null) {
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
            set {
                SetPropertyValue("Customer", ref _Customer, value);
                if (!IsLoading && _Customer != null) {Terms = _Customer.Terms != 
                    null ? _Customer.Terms : null;}
            }
        }
        public Terms Terms {
            get { return _Terms; }
            set {
                SetPropertyValue("Terms", ref _Terms, value);
                if (!IsLoading && _Terms != null) {
                    if (_Terms.DaysToPay > 0) {
                        DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0
                        , 0, 0));
                        DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
                        EarlyDaysToPay, 0, 0, 0));
                        DiscountRate = _Terms.EarlyDiscount;
                    } else {
                        DueDate = DateTime.MinValue;
                    }
                }
                if (!IsLoading && _Terms == null) {DueDate = DateTime.MinValue;}
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
        [Custom("DisplayFormat", "n")]
        public decimal DiscountRate {
            get { return _DiscountRate; }
            set { SetPropertyValue("DiscountRate", ref _DiscountRate, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }
        public TripLocation ShuntingTo {
            get { return _ShuntingTo; }
            set { SetPropertyValue("ShuntingTo", ref _ShuntingTo, value); }
        }
        //private decimal _NoOfKM;
        //public decimal NoOfKM
        //{
        //    get
        //    {
        //        return _NoOfKM;
        //    }
        //    set
        //    {
        //        if (_NoOfKM == value)
        //            return;
        //        _NoOfKM = value;
        //    }
        //}
        [Persistent("NoOfBoxes")]
        private decimal? _NoOfBoxes;
        [PersistentAlias("_NoOfBoxes")]
        public decimal? NoOfBoxes
        {
            get {
                try
                {
                    if (!IsLoading && !IsSaving && _NoOfBoxes == null)
                    {
                        UpdateNoOfBoxes(false);
                    }
                }
                catch (Exception) { }
                return _NoOfBoxes; }
        }
        
        [NonPersistent]
        public TripLocation ThisShuntingTo{
            get {
                TripLocation loc = null;
                foreach (ShuntingEntryDetail item in ShuntingEntryDetails)
                {
                    if (item.ShuntingTo!=null)
                    {
                        loc = item.ShuntingTo;
                        //_NoOfKM = item.Kms;
                    }
                }
                return loc; }
        }
        [NonPersistent]
        public Tariff ThisTariff {
            get {
                Tariff tarf = null;
                if (_TripID.GetType()==typeof(StanfilcoTrip))
                {
                    tarf = ((StanfilcoTrip)_TripID).Tariff;
                }
                return tarf; 
            }
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
        public FAGeneratorSet GensetNo {
            get { return _GensetNo; }
            set { SetPropertyValue("GensetNo", ref _GensetNo, value); }
        }
        [NonPersistent]
        public StanfilcoTrip DtrNo
        {
            get {
                if (TripID.GetType()==typeof(StanfilcoTrip))
                {
                    _DtrNo = (StanfilcoTrip)TripID;
                }
                else
                {
                    _DtrNo = null;
                }
                return _DtrNo; }
        }
        //public decimal PreOdoRead {
        //    get { return _PreOdoRead; }
        //    set { SetPropertyValue("PreOdoRead", ref _PreOdoRead, value); }
        //}
        //public decimal PostOdoRead {
        //    get { return _PostOdoRead; }
        //    set { SetPropertyValue("PostOdoRead", ref _PostOdoRead, value); }
        //}
        //[PersistentAlias("PreOdoRead - PostOdoRead")]
        //public decimal KMRunOdo {
        //    get {
        //        object tempObject = EvaluateAlias("KMRunOdo");
        //        if (tempObject != null) {
        //            if ((decimal)tempObject != 0) {KMRunMnl = (decimal)
        //                tempObject;}
        //            return (decimal)tempObject;
        //        } else {
        //            return 0;
        //        }
        //    }
        //}
        [PersistentAlias("_KMRunMnl")]
        [Custom("DisplayFormat", "n")]
        public decimal? KMRunMnl {
            get {
                try {
                    if (!IsLoading && !IsSaving && _KMRunMnl == null) {
                        UpdateKMRunMnl(false);}
                } catch (Exception) {}
                return _KMRunMnl;
            }
        }
        [PersistentAlias("_Excess")]
        [Custom("DisplayFormat", "n")]
        public decimal? Excess {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Excess == null) {
                        UpdateExcess(false);}
                } catch (Exception) {}
                return _Excess;
            }
        }
        [PersistentAlias("_Additional")]
        [Custom("DisplayFormat", "n")]
        public decimal? Additional {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Additional == null) {
                        UpdateAdditional(false);}
                } catch (Exception) {}
                return _Additional;
            }
        }
        public void UpdateKMRunMnl(bool forceChangeEvent) {
            decimal? oldKMRunMnl = _KMRunMnl;
            decimal tempTotal = 0m;
            foreach (ShuntingEntryDetail detail in ShuntingEntryDetails) {if (
                detail.Type == ShuntingTypeEnum.Shunting) {tempTotal += detail.
                    Kms;}}
            _KMRunMnl = tempTotal;
            if (forceChangeEvent) {
                OnChanged("KMRunMnl", KMRunMnl, _KMRunMnl);
                if (!IsLoading) { OnChanged("TotalKms"); }
            }
        }
        public void UpdateExcess(bool forceChangeEvent) {
            decimal? oldExcess = _Excess;
            decimal tempTotal = 0m;
            foreach (ShuntingEntryDetail detail in ShuntingEntryDetails) {if (
                detail.Type == ShuntingTypeEnum.Excess) {tempTotal += detail.Kms
                    ;}}
            _Excess = tempTotal;
            if (forceChangeEvent) {
                OnChanged("Excess", Excess, _Excess);
                if (!IsLoading) { OnChanged("TotalKms"); }
            }
        }
        public void UpdateAdditional(bool forceChangeEvent) {
            decimal? oldAdditional = _Additional;
            decimal tempTotal = 0m;
            foreach (ShuntingEntryDetail detail in ShuntingEntryDetails) {if (
                detail.Type == ShuntingTypeEnum.Additional) {tempTotal += detail
                    .Kms;}}
            _Additional = tempTotal;
            if (forceChangeEvent) {
                OnChanged("Additional", Additional, _Additional);
                if (!IsLoading) { OnChanged("TotalKms"); }
            }
        }
        public void UpdateNoOfBoxes(bool forceChangeEvent)
        {
            decimal? oldAdditional = _NoOfBoxes;
            decimal tempTotal = 0m;
            foreach (ShuntingEntryDetail detail in ShuntingEntryDetails)
            {
                tempTotal += detail
                        .NoOfBoxes;
            }
            _NoOfBoxes = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("NoOfBoxes", NoOfBoxes, _NoOfBoxes);
                if (!IsLoading) { OnChanged("NoOfBoxes"); }
            }
        }
        [PersistentAlias("KMRunMnl + Excess + Additional")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalKms {
            get {
                object tempObject = EvaluateAlias("TotalKms");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal RatePerKms {
            get { return _RatePerKms; }
            set { SetPropertyValue("RatePerKms", ref _RatePerKms, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal FuelSubsidy
        {
            get
            {
                return _FuelSubsidy;
            }
            set
            {
                SetPropertyValue("FuelSubsidy", ref _FuelSubsidy, value);
            }
        }
        private decimal _ChassyRate = 4m;
        public decimal ChassyRate
        {
            get { return _ChassyRate; }
            set { SetPropertyValue("ChassyRate", ref _ChassyRate, value); }
        }
        
        [PersistentAlias("TotalKms * ChassyRate")]
        [Custom("DisplayFormat", "n")]
        public decimal ChassyRental
        {
            get
            {
                object tempObject = EvaluateAlias("ChassyRental");
                if (TrailerNo == null && tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }
        private string _ChassyNo;
        public string ChassyNo
        {
            get { return string.IsNullOrEmpty(_ChassyNo)?TrailerNo!=null?TrailerNo.No:_ChassyNo:_ChassyNo; }
            set { SetPropertyValue("ChassyNo", ref _ChassyNo, value);
            }
        }
        
        private decimal _InsuranceRate = 0.15m;
        public decimal InsuranceRate
        {
            get { return _InsuranceRate; }
            set { SetPropertyValue("InsuranceRate", ref _InsuranceRate, value); }
        }

        [PersistentAlias("TotalKms * InsuranceRate")]
        [Custom("DisplayFormat", "n")]
        public decimal Insurance
        {
            get
            {
                object tempObject = EvaluateAlias("Insurance");
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
        
        [NonPersistent]
        public decimal Distance
        {
            get { return ((StanfilcoTrip)_TripID).Distance; }
        }
        [PersistentAlias("FuelSubsidy * TotalKms")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelSubComp
        {
            get
            {
                object tempObject = EvaluateAlias("FuelSubComp");
                if (_FuelSubsidy>0 && tempObject != null) { 
                    return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("(TotalKms * RatePerKms) + FuelSubComp")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Trucker Pay")]
        public decimal Total {
            get {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [PersistentAlias("Total - ChassyRental - Insurance")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Total")]
        public decimal Total1
        {
            get
            {
                object tempObject = EvaluateAlias("Total1");
                if (tempObject != null) { return (decimal)tempObject; }
                else
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
        [PersistentAlias("Total * (VATRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal VATAmount {
            get {
                object tempObject = EvaluateAlias("VATAmount");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [PersistentAlias("Total1 + VATAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal GrossBilling {
            get {
                object tempObject = EvaluateAlias("GrossBilling");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal WHTRate {
            get { return _WHTRate; }
            set { SetPropertyValue("WHTRate", ref _WHTRate, value); }
        }
        [PersistentAlias("Total1 * (WHTRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTAmount {
            get {
                object tempObject = EvaluateAlias("WHTAmount");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [PersistentAlias("GrossBilling - WHTAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal NetBilling {
            get {
                object tempObject = EvaluateAlias("NetBilling");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        [Aggregated,
        Association("ShuntingEntry-ShuntingEntryDetails")]
        public XPCollection<ShuntingEntryDetail> ShuntingEntryDetails { get { 
                return GetCollection<ShuntingEntryDetail>("ShuntingEntryDetails"
                ); } }
        public ShuntingEntry(Session session): base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
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
        public decimal AmtPaid
        {
            get
            {
                object tempObject = EvaluateAlias("AmtPaid");
                if (tempObject != null)
                {
                    if (_OpenAmount == 0)
                    {
                        return NetBilling;
                    }
                    else
                    {
                        return (decimal)tempObject;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("NetBilling - AmtPaid")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Unpaid")]
        public decimal AmtRmn
        {
            get
            {
                object tempObject = EvaluateAlias("AmtRmn");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public int DaysOt
        {
            get
            {
                //=IF(A3<1, "",IF(A3>CURRDT,0,(A3-CURRDT)*-1))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now) { _DaysOt = 0; }
                    else
                    {
                        TimeSpan ts = EntryDate - DateTime.Now;
                        _DaysOt = ts.Days * -1;
                    }
                }
                else
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
        public decimal ZT30Days
        {
            get
            {
                //=IF(A3<CURRDT,IF(J3<=30,I3,0),)
                if (AmtRmn > 0)
                {
                    if (EntryDate < DateTime.Now)
                    {
                        if (DaysOt <= 30) { _ZT30Days = AmtRmn; }
                        else
                        {
                            _ZT30Days = 0;
                        }
                    }
                }
                else
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
        public decimal T3T60Days
        {
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=60,J3>30),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now) { _T3T60Days = 0; }
                    else
                    {
                        if (DaysOt <= 60 && DaysOt > 30) { _T3T60Days = AmtRmn; }
                        else
                        {
                            _T3T60Days = 0;
                        }
                    }
                }
                else
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
        public decimal T6T90Days
        {
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=90,J3>60),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now) { _T6T90Days = 0; }
                    else
                    {
                        if (DaysOt <= 90 && _DaysOt > 60) { _T6T90Days = AmtRmn; }
                        else
                        {
                            _T6T90Days = 0;
                        }
                    }
                }
                else
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
        public decimal GRT90Days
        {
            get
            {
                //=IF(A3>CURRDT,0,IF(J3>=90,I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now) { _GRT90Days = 0; }
                    else
                    {
                        if (DaysOt >= 90) { _GRT90Days = AmtRmn; }
                        else
                        {
                            _GRT90Days = 0;
                        }
                    }
                }
                else
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
        public MonthsEnum Month
        {
            get
            {
                _Month = EntryDate.Month == 1 ? MonthsEnum.January : EntryDate.Month
                    == 2 ? MonthsEnum.February : EntryDate.Month == 3 ? MonthsEnum.
                    March : EntryDate.Month == 4 ? MonthsEnum.April : EntryDate.Month ==
                    5 ? MonthsEnum.May : EntryDate.Month == 6 ? MonthsEnum.June :
                    EntryDate.Month == 7 ? MonthsEnum.July : EntryDate.Month == 8 ?
                    MonthsEnum.August : EntryDate.Month == 9 ? MonthsEnum.September
                    : EntryDate.Month == 10 ? MonthsEnum.October : EntryDate.Month == 11
                    ? MonthsEnum.November : EntryDate.Month == 12 ? MonthsEnum.
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
            get { return EntryDate.Year; ; }
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
        private bool _Marked = false;
        private int _BillSeq;
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
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            //Session.OptimisticLockingReadBehavior =
            //OptimisticLockingReadBehavior.ReloadObject;
            RowID = Guid.NewGuid();
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "SH"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "SH"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "SH"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Memo = "Shunting #" + SourceNo;
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
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Shunting transactions."
                );} }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
            base.TriggerObjectChanged(args);
            if (!string.IsNullOrEmpty(args.PropertyName))
            {
                if (!IsLoading)
                {
                    if (args.PropertyName == "EntryDate")
                    {
                        if (_Terms != null)
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
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _KMRunMnl = null;
            _Additional = null;
            _Excess = null;
            _NoOfBoxes = null;
        }
    }
}
