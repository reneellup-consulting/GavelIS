using System;
using System.Linq;
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
using System.Collections.Generic;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class DolefilTrip : GenJournalHeader {
        #region Header
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private TripStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Customer _Customer;
        private Terms _Terms;
        private DateTime _DueDate;
        private DateTime _DiscountDate;
        private decimal _DiscountRate;
        private decimal _OpenAmount;
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
        public TripStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != TripStatusEnum.Current) {Approved = true;} 
                    else {
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
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
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
        #endregion
        #region GAVEL Operations
        private bool _Hub = false;
        private bool _Duvo = false;
        private bool _CagayanTrip = false;
        private FATruck _TruckNo;
        private Employee _Driver;
        private FATrailer _TrailerNo;
        private decimal _Distance;
        [Persistent("ActualRun")]
        private decimal? _ActualRun;
        [Persistent("Allowance")]
        private decimal? _Allowance;
        [Persistent("FuelConsumed")]
        private decimal? _FuelConsumed;
        [Persistent("FuelConsEntries")]
        private decimal? _FuelConsEntries;
        public bool Hub
        {
            get { return _Hub; }
            set { SetPropertyValue("Hub", ref _Hub, value); }
        }
        public bool Duvo
        {
            get { return _Duvo; }
            set { SetPropertyValue("Duvo", ref _Duvo, value); }
        }
        public bool CagayanTrip
        {
            get { return _CagayanTrip; }
            set { SetPropertyValue("CagayanTrip", ref _CagayanTrip, value); }
        }
        // Last truck that is registered in this trip
        [Custom("AllowEdit", "False")]
        //[RuleRequiredField("", DefaultContexts.Save)]
        [PersistentAlias("ThisTruck")]
        public FATruck TruckNo {
            get {
                object tempObject = EvaluateAlias("TruckNo");
                if (tempObject != null) {return (FATruck)tempObject;} else {
                    return null;
                }
            }
        }
        // Last drive that is registered in this trip
        [Custom("AllowEdit", "False")]
        //[RuleRequiredField("", DefaultContexts.Save)]
        [PersistentAlias("ThisDriver")]
        public Employee Driver {
            get {
                object tempObject = EvaluateAlias("Driver");
                if (tempObject != null) {return (Employee)tempObject;} else {
                    return null;
                }
            }
        }
        // Last genset that is registered in this trip
        [Custom("AllowEdit", "False")]
        [PersistentAlias("ThisGenset")]
        public FAGeneratorSet GensetNo {
            get {
                object tempObject = EvaluateAlias("GensetNo");
                if (tempObject != null) {return (FAGeneratorSet)tempObject;} 
                else {
                    return null;
                }
            }
        }
        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        public decimal Distance {
            get { return _Distance; }
            set { SetPropertyValue("Distance", ref _Distance, value); }
        }
        [PersistentAlias("_ActualRun")]
        [Custom("DisplayFormat", "n")]
        public decimal? ActualRun {
            get {
                try {
                    if (!IsLoading && !IsSaving && _ActualRun == null) {
                        UpdateActualRun(false);}
                } catch (Exception) {}
                return _ActualRun;
            }
        }
        [PersistentAlias("_Allowance")]
        [Custom("DisplayFormat", "n")]
        public decimal? Allowance {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Allowance == null) {
                        UpdateAllowance(false);}
                } catch (Exception) {}
                return _Allowance;
            }
        }
        [PersistentAlias("_FuelConsumed")]
        [DisplayName("Fuel Cons(Truck)")]
        [Custom("DisplayFormat", "n")]
        public decimal? FuelConsumed {
            get {
                try {
                    if (!IsLoading && !IsSaving && _FuelConsumed == null) {
                        UpdateFuelConsumed(false);}
                } catch (Exception) {}
                return _FuelConsumed;
            }
        }
        [DisplayName("Fuel Cons(Entries)")]
        [PersistentAlias("_FuelConsEntries")]
        [Custom("DisplayFormat", "n")]
        public decimal? FuelConsEntries {
            get {
                try {
                    if (!IsLoading && !IsSaving && _FuelConsEntries == null) {
                        UpdateFuelConsEntries(false);}
                } catch (Exception) {}
                return _FuelConsEntries;
            }
        }
        [Size(1000)]
        public string FuelReferences
        {
            get
            {
                StringBuilder sbRefs = new StringBuilder();
                foreach (FuelRegister detail in FuelRegistrations)
                {
                    if (detail.ReceiptFuelDetailID != null && detail.ReceiptFuelDetailID.ReceiptInfo != null)
                    {
                        string refNo = !string.IsNullOrEmpty(detail.ReceiptFuelDetailID.ReceiptInfo.InvoiceNo) 
                            ? detail.ReceiptFuelDetailID.ReceiptInfo.InvoiceNo : string.Empty;
                        sbRefs.Append(refNo + ",");
                    }
                }
                return !string.IsNullOrEmpty(sbRefs.ToString())? sbRefs.ToString().Remove(sbRefs.ToString().Length-1): string.Empty;
            }
        }
        #region Calculated Details 1
        public void UpdateActualRun(bool forceChangeEvent) {
            decimal? oldActualRun = _ActualRun;
            decimal tempTotal = 0m;
            foreach (DolefilTripDetail detail in DolefilTripDetails) {if (!
                detail.Category.Trailer) {tempTotal += detail.Distance;}}
            _ActualRun = tempTotal;
            if (forceChangeEvent) {if (oldActualRun != _ActualRun) {OnChanged(
                    "ActualRun", ActualRun, _ActualRun);}}
            ;
        }
        public void UpdateAllowance(bool forceChangeEvent) {
            decimal? oldAllowance = _Allowance;
            decimal tempTotal = 0m;
            foreach (DolefilTripDetail detail in DolefilTripDetails) {if (!
                detail.Category.Trailer) {tempTotal += detail.Allowance;}}
            _Allowance = tempTotal;
            if (forceChangeEvent) {if (oldAllowance != _Allowance) {OnChanged(
                    "Allowance", Allowance, _Allowance);}}
            ;
        }
        public void UpdateFuelConsumed(bool forceChangeEvent) {
            decimal? oldFuelConsumed = _FuelConsumed;
            decimal tempTotal = 0m;
            foreach (TruckRegistry detail in TruckRegistrations) {tempTotal += 
                detail.FuelConsumedMnl;}
            _FuelConsumed = tempTotal;
            if (forceChangeEvent) {if (oldFuelConsumed != _FuelConsumed) {
                    OnChanged("FuelConsumed", FuelConsumed, _FuelConsumed);}}
            ;
        }
        public void UpdateFuelConsEntries(bool forceChangeEvent) {
            decimal? oldFuelConsEntries = _FuelConsEntries;
            decimal tempTotal = 0m;
            foreach (FuelRegister detail in FuelRegistrations) {tempTotal += 
                detail.ReceiptFuelDetailID.ReceiptInfo.TruckOrGenset == 
                TruckOrGensetEnum.Truck ? detail.ReceiptFuelDetailID.Quantity : 
                0;}
            _FuelConsEntries = tempTotal;
            if (forceChangeEvent) {if (oldFuelConsEntries != _FuelConsEntries) {
                    OnChanged("FuelConsEntries", FuelConsEntries, 
                    _FuelConsEntries);}}
            ;
        }
        #endregion
        #endregion
        #region Statement
        private string _DocumentNo;
        private Tariff _Tariff;
        private TripLocation _Origin;
        private TripLocation _Destination;
        [Persistent("AmountTruck")]
        private decimal? _AmountTruck;
        [Persistent("TrailerRental")]
        private decimal? _TrailerRental;
        private decimal _Billing;
        private decimal _VATRate;
        private decimal _VATAmount;
        private decimal _GrossBilling;
        private bool _WHTInclusive;
        private decimal _WHTRate;
        private decimal _WHTAmount;
        private decimal _NetBilling;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string DocumentNo {
            get { return _DocumentNo; }
            set { SetPropertyValue("DocumentNo", ref _DocumentNo, value); }
        }
        [ImmediatePostData]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Tariff Tariff {
            get { return _Tariff; }
            set {
                SetPropertyValue("Tariff", ref _Tariff, value);
                if (!IsLoading && _Tariff != null) {
                    VATRate = _Tariff.TaxCode != null ? _Tariff.TaxCode.Rate : 0
                    ;
                    WHTInclusive = _Tariff.WHTInclusive;
                    WHTRate = _Tariff.WHTInclusive ? _Tariff.WHTGroupCode != 
                    null ? _Tariff.WHTGroupCode.WHTRate : 0 : 0;
                    Distance = _Tariff.Distance;
                    Origin = _Tariff.Origin;
                    Destination = _Tariff.Destination;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public TripLocation Origin {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        [Custom("AllowEdit", "False")]
        public TripLocation Destination {
            get { return _Destination; }
            set { SetPropertyValue("Destination", ref _Destination, value); }
        }
        [PersistentAlias("_AmountTruck")]
        [Custom("DisplayFormat", "n")]
        public decimal? AmountTruck {
            get {
                try {
                    if (!IsLoading && !IsSaving && _AmountTruck == null) {
                        UpdateAmountTruck(false);}
                } catch (Exception) {}
                return _AmountTruck;
            }
        }
        public void UpdateAmountTruck(bool forceChangeEvent) {
            decimal? oldAmountTruck = _AmountTruck;
            decimal tempTotal = 0m;
            foreach (DolefilTripDetail detail in DolefilTripDetails) {if (!
                detail.Category.Trailer) {tempTotal += detail.Amount;}}
            _AmountTruck = tempTotal;
            if (forceChangeEvent) {OnChanged("AmountTruck", AmountTruck, 
                _AmountTruck);}
            ;
        }
        [PersistentAlias("_TrailerRental")]
        [Custom("DisplayFormat", "n")]
        public decimal? TrailerRental {
            get {
                try {
                    if (!IsLoading && !IsSaving && _TrailerRental == null) {
                        UpdateTrailerRental(false);}
                } catch (Exception) {}
                return _TrailerRental;
            }
        }
        public void UpdateTrailerRental(bool forceChangeEvent) {
            decimal? oldTrailerRental = _TrailerRental;
            decimal tempTotal = 0m;
            foreach (DolefilTripDetail detail in DolefilTripDetails) {if (detail
                .Category.Trailer) {tempTotal += detail.Amount;}}
            _TrailerRental = tempTotal;
            if (forceChangeEvent) {OnChanged("TrailerRental", TrailerRental, 
                _TrailerRental);}
            ;
        }
        [PersistentAlias("AmountTruck + TrailerRental")]
        [Custom("DisplayFormat", "n")]
        public decimal Billing {
            get {
                object tempObject = EvaluateAlias("Billing");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal VATRate {
            get { return _VATRate; }
            set { SetPropertyValue("VATRate", ref _VATRate, value); }
        }
        [PersistentAlias("Billing * (VATRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal VATAmount {
            get {
                object tempObject = EvaluateAlias("VATAmount");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [PersistentAlias("Billing + VATAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal GrossBilling {
            get {
                object tempObject = EvaluateAlias("GrossBilling");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool WHTInclusive {
            get { return _WHTInclusive; }
            set { SetPropertyValue("WHTInclusive", ref _WHTInclusive, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTRate {
            get { return _WHTRate; }
            set { SetPropertyValue("WHTRate", ref _WHTRate, value); }
        }
        [PersistentAlias("Billing * (WHTRate/100)")]
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
        #endregion

        //[Aggregated,
        //Association("DolefilTrip-HaulCategorySets")]
        //public XPCollection<HaulCategorySet> HaulCategorySets
        //{
        //    get
        //    {
        //        return GetCollection<HaulCategorySet>(
        //          "HaulCategorySets");
        //    }
        //}

        #region Autopopulate
        [NonPersistent]
        private FATruck ThisTruck {
            get {
                FATruck truck = null;
                foreach (TruckRegistry item in TruckRegistrations) {truck = item
                    .TruckNo;}
                return truck;
            }
        }
        [NonPersistent]
        private Employee ThisDriver {
            get {
                Employee driver = null;
                foreach (DriverRegistry item in DriverRegistrations) {driver = 
                    item.Driver;}
                return driver;
            }
        }
        [NonPersistent]
        private FAGeneratorSet ThisGenset {
            get {
                FAGeneratorSet genset = null;
                foreach (GensetEntry item in GensetEntries) {genset = item.
                    GensetNo;}
                return genset;
            }
        }
        #endregion
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
                        return _NetBilling;
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

        #region Cargo Info
        public string CargoParticulars
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.CargoRegistrations != null && this.CargoRegistrations.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in this.CargoRegistrations)
                    {
                        if (item.Particular != null && !strRefs.Contains(item.Particular.Code))
                        {
                            strRefs.Add(item.Particular.Code);
                            decimal qty = this.CargoRegistrations.Where(o => o.Particular == item.Particular).Sum(o => o.Quantity);
                            sb.AppendFormat("{0} {1}(s) of {2},", qty.ToString("n2"), item.UOM.Code, item.Particular.Description);
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }
        #endregion

        public DolefilTrip(Session session): base(session) {
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "DF"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "DF"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "DF"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Customer tmp = Session.FindObject<Customer>(CriteriaOperator.Parse(
            "Contains([Name], 'DOLE PHILIPPINES TRIPS')"));
            this.Customer = tmp != null ? tmp : null;
            Memo = "Dolefil Trip #" + SourceNo;
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Dolefil trip."
                );} }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args) 
        {
            //this.IsIncExpNeedUpdate = true;
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            base.TriggerObjectChanged(args);
            if (args.PropertyName == "EntryDate") {
                if (!IsLoading && _Terms != null) {
                    DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0, 0, 
                    0));
                    DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
                    EarlyDaysToPay, 0, 0, 0));
                    DiscountRate = _Terms.EarlyDiscount;
                }
            }
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
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _AmountTruck = null;
            _TrailerRental = null;
            _ActualRun = null;
            _Allowance = null;
            _FuelConsumed = null;
            _FuelConsEntries = null;
        }
    }
}
