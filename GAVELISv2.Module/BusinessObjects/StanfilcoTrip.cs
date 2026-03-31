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
using NCalc;
using System.Text;
using System.Collections.Generic;
using System.Collections;

namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class StanfilcoTrip : GenJournalHeader {

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
        private decimal _GensetRate;
        private decimal _TariffDistance;
        private decimal _TariffTruckerPay;
        private decimal _TariffFuelSubsidy;
        private decimal _TariffNewBilling;
        private decimal _TariffNewVatAmount;
        private decimal _TariffNewGrossBilling;
        private decimal _TariffNewWhtAmount;
        private decimal _TariffNewNetBilling;
        private bool _Mpp;
        private bool _LongHaul;
        //[RuleUniqueValue("", DefaultContexts.Save)]
        //[RuleRequiredField("", DefaultContexts.Save)]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        public override DateTime EntryDate
        {
            get
            {
                return base.EntryDate;
            }
            set
            {
                base.EntryDate = value;
                GetGensetRate(value);
            }
        }
        public void GetGensetRate(DateTime entrydate) {
            if (!IsLoading && !IsSaving && _Customer != null)
            {
                XPClassInfo classInfo = Session.GetClassInfo<GensetRateRegister>();
                CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("[EffectiveDate] <= #{0}#", this.ExactEntryDate.ToString("yyyy-MM-dd")));
                SortingCollection sorting = new SortingCollection();
                sorting.Add(new SortProperty("EffectiveDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
                var data = Session.GetObjects(classInfo, criteria, sorting, 0, false, true);
                if (data != null && data.Count > 0)
                {
                    IEnumerable<GensetRateRegister> rates = data.Cast<GensetRateRegister>();
                    GensetRate = rates.LastOrDefault().Rate;
                }
            }
        }
        [Action(Caption="Verify Genset Rental",AutoCommit=true)]
        public void VerifyGensetRental() {
            GetGensetRate(EntryDate);
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
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != TripStatusEnum.Current)
                    {
                        Approved = true;
                    } else
                    {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    var currentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                    StatusBy = currentUser.UserName;
                    StatusDate = DateTime.Now;
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
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
                GetGensetRate(EntryDate);
                if (!IsLoading && _Customer != null)
                {
                    Terms = _Customer.Terms !=
                    null ?
                    _Customer.Terms :
                    null;
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
                        DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0, 0,
                        0));
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

        private bool _Hustling = false;
        private FATruck _TruckNo;
        private Employee _Driver;
        private FAGeneratorSet _GensetNo;
        private FATrailer _TrailerNo;
        private DateTime _Start;
        private bool _YardStop = false;
        private DateTime _InYard;
        private TimeSpan _YardTimeSpan;
        private DateTime _OutYard;
        private DateTime _InDestination;
        private TimeSpan _DestinationTimeSpan;
        private DateTime _OutDestination;
        private DateTime _Finished;
        private decimal _Distance;
        [Persistent("ActualRun")]
        private decimal? _ActualRun;
        private TimeSpan _TurnAround;
        private TimeSpan _TurnAroundActual;
        private decimal _Allowance;
        private decimal _FuelAllocation;
        [Persistent("FuelConsumed")]
        private decimal? _FuelConsumed;
        [Persistent("FuelConsEntries")]
        private decimal? _FuelConsEntries;
        [Persistent("FuelCostEntries")]
        private decimal? _FuelCostEntries;
        private decimal? _FuelShunting;
        private decimal _FuelVariance;
        private decimal _FuelVarEntries;
        public bool Hustling {
            get { return _Hustling; }
            set { SetPropertyValue("Hustling", ref _Hustling, value); }
        }
        public bool Mpp
        {
            get { return _Mpp; }
            set { SetPropertyValue("Mpp", ref _Mpp, value); }
        }
        public bool LongHaul
        {
            get { return _LongHaul; }
            set { SetPropertyValue("LongHaul", ref _LongHaul, value); }
        }
        // Last truck that is registered in this trip
        [Custom("AllowEdit", "False")]
        //[RuleRequiredField("", DefaultContexts.Save)]
        [PersistentAlias("ThisTruck")]
        public FATruck TruckNo {
            get
            {
                var tempObject = EvaluateAlias("TruckNo");
                if (tempObject != null)
                {
                    return (FATruck)tempObject;
                } else
                {
                    return null;
                }
            }
        }

        // Last drive that is registered in this trip
        [Custom("AllowEdit", "False")]
        //[RuleRequiredField("", DefaultContexts.Save)]
        [PersistentAlias("ThisDriver")]
        public Employee Driver {
            get
            {
                var tempObject = EvaluateAlias("Driver");
                if (tempObject != null)
                {
                    return (Employee)tempObject;
                } else
                {
                    return null;
                }
            }
        }

        // Last genset that is registered in this trip
        [Custom("AllowEdit", "False")]
        [PersistentAlias("ThisGenset")]
        public FAGeneratorSet GensetNo {
            get
            {
                var tempObject = EvaluateAlias("GensetNo");
                if (tempObject != null)
                {
                    return (FAGeneratorSet)tempObject;
                } else
                {
                    return null;
                }
            }
        }

        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime Start {
            get { return _Start; }
            set { SetPropertyValue("Start", ref _Start, value); }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime OutPcy
        {
            get { return _OutPcy; }
            set { SetPropertyValue("OutPcy", ref _OutPcy, value); }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime StartDT
        {
            get {
                if (_Start != DateTime.MinValue && _OutPcy != DateTime.MinValue)
                {
                    _StartDT = _Start;
                }
                else if (_Start != DateTime.MinValue && _OutPcy == DateTime.MinValue)
                {
                    _StartDT = _Start;
                }
                else if (_Start == DateTime.MinValue && _OutPcy != DateTime.MinValue)
                {
                    _StartDT = _OutPcy;
                }
                else
                {
                    _StartDT = DateTime.MinValue;
                }
                return _StartDT; }
        }

        [ImmediatePostData]
        public bool YardStop {
            get { return _YardStop; }
            set
            {
                SetPropertyValue("YardStop", ref _YardStop, value);
                if (!IsLoading)
                {
                    InYard = DateTime.MinValue;
                    OutYard = DateTime.MinValue;
                }
            }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime InYard {
            get { return _InYard; }
            set { SetPropertyValue("InYard", ref _InYard, value); }
        }

        //[Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        //[EditorAlias("CustomDateTimeEditor2")]
        [PersistentAlias("OutYard - InYard")]
        public TimeSpan YardTimeSpan {
            get
            {
                var tempObject = EvaluateAlias("YardTimeSpan");
                if (tempObject != null)
                {
                    return (TimeSpan)tempObject;
                } else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime OutYard {
            get { return _OutYard; }
            set { SetPropertyValue("OutYard", ref _OutYard, value); }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime InDestination {
            get { return _InDestination; }
            set { SetPropertyValue("InDestination", ref _InDestination, value); }
        }

        // AtRampForLoading
        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime AtRampForLoading
        {
            get { return _AtRampForLoading; }
            set { SetPropertyValue("AtRampForLoading", ref _AtRampForLoading, value); }
        }
        // AtStartLoading
        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime AtStartLoading
        {
            get { return _AtStartLoading; }
            set { SetPropertyValue("AtStartLoading", ref _AtStartLoading, value); }
        }
        // AtFinishedLoading
        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime AtFinishedLoading
        {
            get { return _AtFinishedLoading; }
            set { SetPropertyValue("AtFinishedLoading", ref _AtFinishedLoading, value); }
        }

        [PersistentAlias("OutDestination - InDestination")]
        public TimeSpan DestinationTimeSpan {
            get
            {
                var tempObject = EvaluateAlias("DestinationTimeSpan");
                if (tempObject != null)
                {
                    return (TimeSpan)tempObject;
                } else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime OutDestination {
            get { return _OutDestination; }
            set { SetPropertyValue("OutDestination", ref _OutDestination, value); }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime PostTrip
        {
            get { return _PostTrip; }
            set { SetPropertyValue("PostTrip", ref _PostTrip, value); }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime Finished {
            get { return _Finished; }
            set { SetPropertyValue("Finished", ref _Finished, value); }
        }

        

        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Distance")]
        public decimal Distance {
            get { return _Distance; }
            set { SetPropertyValue("Distance", ref _Distance, value);
                if (!IsLoading && _Tariff != null)
                {
                    // Update Tariff Distance with the new distance value
                    Tariff.Distance = value;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TariffDistance
        {
            get { return _TariffDistance; }
            set { SetPropertyValue("TariffDistance", ref _TariffDistance, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal TruckerPayRate
        {
            get { return _TruckerPayRate; }
            set
            {
                SetPropertyValue("TruckerPayRate", ref _TruckerPayRate,
                    value);
                if (!IsLoading)
                {
                    if (_Tariff != null)
                    {
                        Tariff.TruckerPayRate = value;
                    }
                    // Auto calculate TruckerPay = Distance * TruckerPayRate
                    TruckerPay = Distance * value;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal FuelSubsidyRate
        {
            get { return _FuelSubsidyRate; }
            set
            {
                SetPropertyValue("FuelSubsidyRate", ref _FuelSubsidyRate,
                    value);
                if (!IsLoading)
                {
                    if (_Tariff != null)
                    {
                        Tariff.FuelSubsidyRate = value;
                    }
                    // Auto calculate RateAdjmt = Distance * FuelSubsidyRate
                    RateAdjmt = Distance * value;
                }
            }
        }

        [PersistentAlias("_ActualRun")]
        [Custom("DisplayFormat", "n")]
        public decimal? ActualRun {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ActualRun == null)
                    {
                        UpdateActualRun(false);
                    }
                } catch (Exception)
                {
                }
                return _ActualRun;
            }
        }

        [Custom("AllowEdit", "False")]
        public TimeSpan TurnAround {
            get { return _TurnAround; }
            set { SetPropertyValue("TurnAround", ref _TurnAround, value); }
        }

        [PersistentAlias("Finished - StartDT")]
        public TimeSpan TurnAroundActual {
            get
            {
                var tempObject = EvaluateAlias("TurnAroundActual");
                if (tempObject != null)
                {
                    //return ((TimeSpan)tempObject).Subtract(YardTimeSpan);
                    return ((TimeSpan)tempObject).Subtract(YardTimeSpan);
                } else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        
        [Custom("DisplayFormat", "n")]
        [DisplayName("Genset Rental Days")]
        public int GensetDays
        {
            get
            {
                int days = TurnAroundActual.Days;
                int hours = TurnAroundActual.Hours;
                if (hours >= 4)
                {
                    return days + 1;
                }
                else
                {
                    return days;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        [DisplayName("Genset Rental Rate")]
        public decimal GensetRate
        {
            get { return _GensetRate; }
            set { SetPropertyValue("GensetRate", ref _GensetRate, value); }
        }
        [PersistentAlias("GensetDays * GensetRate")]
        public decimal GensetRentalAmount
        {
            get
            {
                var tempObject = EvaluateAlias("GensetRentalAmount");
                if (tempObject != null && ThisGenset != null && !_Hustling)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal GensetVatRate
        {
            get { return _GensetVatRate; }
            set { SetPropertyValue("GensetVatRate", ref _GensetVatRate, value); }
        }

        [PersistentAlias("GensetRentalAmount / (1 + GensetVatRate/100)")]
        public decimal GensetRentalAmountVatEx
        {
            get
            {
                var tempObject = EvaluateAlias("GensetRentalAmountVatEx");
                if (tempObject != null && ThisGenset != null && !_Hustling)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        [PersistentAlias("GensetRentalAmount - GensetRentalAmountVatEx")]
        public decimal GensetRentalVatAmount
        {
            get
            {
                var tempObject = EvaluateAlias("GensetRentalVatAmount");
                if (tempObject != null && ThisGenset != null && !_Hustling)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal GensetWHRate
        {
            get { return _GensetWHRate; }
            set { SetPropertyValue("GensetWHRate", ref _GensetWHRate, value); }
        }

        [PersistentAlias("GensetRentalAmountVatEx * (GensetWHRate/100)")]
        public decimal GensetRentalWhAmount
        {
            get
            {
                var tempObject = EvaluateAlias("GensetRentalWhAmount");
                if (tempObject != null && ThisGenset != null && !_Hustling)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        [PersistentAlias("GensetRentalAmount - GensetRentalWhAmount")]
        public decimal GensetNetBilling
        {
            get
            {
                var tempObject = EvaluateAlias("GensetNetBilling");
                if (tempObject != null && ThisGenset != null && !_Hustling)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool MigratedToDollar
        {
            get { return _MigratedToDollar; }
            set { SetPropertyValue("MigratedToDollar", ref _MigratedToDollar, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CurrencyRate
        {
            get
            {
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Metadata.XPClassInfo currencyRateClass;
            currencyRateClass = Session.GetClassInfo(typeof(CurrencyRate));
            criteria = CriteriaOperator.Parse(string.Format("[EntryDate] <= #{0}# And [Pair] = 'USD/PHP'",
                    EntryDate.ToString("yyyy-MM-dd")));
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("EntryDate", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            var data = Session.GetObjects(currencyRateClass, criteria, sortProps, 0, false, true).Cast<CurrencyRate>();
            if (data != null && data.Count() > 0)
                {
                    return (decimal)data.Last().Rate;
                }
                return 0;
            }
        }

        [PersistentAlias("GensetNetBilling * CurrencyRate")]
        [Custom("DisplayFormat", "n")]
        //[NonPersistent]
        public decimal GensetNetBillingLCY
        {
            get
            {
                object tempObject = EvaluateAlias("GensetNetBillingLCY");
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

        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal GensetNetBillingLCY
        //{
        //    get
        //    {
        //        var data = Session.FindObject<CurrencyRate>(
        //            BinaryOperator.Parse(string.Format("[EntryDate] <= #{0}# And [Pair] = 'USD/PHP'",
        //            EntryDate.ToString("yyyy-MM-dd"))));
        //        if (data != null)
        //        {
        //            return GensetNetBilling * (decimal)data.Rate;
        //        }
        //        return 0;
        //    }
        //}

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Allowance {
            get { return _Allowance; }
            set { SetPropertyValue("Allowance", ref _Allowance, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelAllocation {
            get { return _FuelAllocation; }
            set { SetPropertyValue("FuelAllocation", ref _FuelAllocation, value); }
        }

        [PersistentAlias("_FuelConsumed")]
        [DisplayName("Fuel Cons(Truck)")]
        [Custom("DisplayFormat", "n")]
        public decimal? FuelConsumed {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _FuelConsumed == null)
                    {
                        UpdateFuelConsumed(false);
                    }
                } catch (Exception)
                {
                }
                return _FuelConsumed;
            }
        }

        [DisplayName("Fuel Qty(Top Up)")]
        [PersistentAlias("_FuelConsEntries")]
        [Custom("DisplayFormat", "n")]
        public decimal? FuelConsEntries {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _FuelConsEntries == null)
                    {
                        UpdateFuelConsEntries(false);
                    }
                } catch (Exception)
                {
                }
                return _FuelConsEntries;
            }
        }

        [DisplayName("Fuel Cost(Top Up)")]
        [PersistentAlias("_FuelCostEntries")]
        [Custom("DisplayFormat", "n")]
        public decimal? FuelCostEntries
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _FuelCostEntries == null)
                    {
                        UpdateFuelCostEntries(false);
                    }
                }
                catch (Exception)
                {
                }
                return _FuelCostEntries;
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal? FuelShunting {
            get { return _FuelShunting; }
            set { SetPropertyValue("FuelShunting", ref _FuelShunting, value); }
        }

        [DisplayName("Fuel Var(Truck)")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelVariance {
            get { return _FuelVariance; }
            set { SetPropertyValue("FuelVariance", ref _FuelVariance, value); }
        }

        [DisplayName("Fuel Var(Entries)")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelVarEntries {
            get { return _FuelVarEntries; }
            set { SetPropertyValue("FuelVarEntries", ref _FuelVarEntries, value); }
        }

        #region Calculated Details 1

        public void UpdateActualRun(bool forceChangeEvent) {
            var oldActualRun = _ActualRun;
            var tempTotal = 0m;
            foreach (TruckRegistry detail in TruckRegistrations)
            {
                tempTotal +=
                detail.KMRunMnl;
            }
            _ActualRun = tempTotal;
            if (forceChangeEvent)
            {
                if (oldActualRun != _ActualRun)
                {
                    OnChanged(
                    "ActualRun", ActualRun, _ActualRun);
                }
            }
            ;
        }

        public void UpdateFuelConsumed(bool forceChangeEvent) {
            var oldFuelConsumed = _FuelConsumed;
            var tempTotal = 0m;
            foreach (TruckRegistry detail in TruckRegistrations)
            {
                tempTotal +=
                detail.FuelConsumedMnl;
            }
            _FuelConsumed = tempTotal;
            if (forceChangeEvent)
            {
                if (oldFuelConsumed != _FuelConsumed)
                {
                    OnChanged(
                    "FuelConsumed", FuelConsumed, _FuelConsumed);
                }
            }
            ;
        }

        public void UpdateFuelConsEntries(bool forceChangeEvent) {
            var oldFuelConsEntries = _FuelConsEntries;
            var tempTotal = 0m;
            foreach (FuelRegister detail in FuelRegistrations)
            {
                //tempTotal += detail.
                //ReceiptFuelDetailID.ReceiptInfo.TruckOrGenset == TruckOrGensetEnum.
                //Truck ?
                //detail.ReceiptFuelDetailID.Quantity :
                //0;
                tempTotal += detail.Qty;
            }
            _FuelConsEntries = tempTotal;
            if (forceChangeEvent)
            {
                if (oldFuelConsEntries != _FuelConsEntries)
                {
                    OnChanged("FuelConsEntries", FuelConsEntries, _FuelConsEntries);
                }
            }
            ;
        }

        public void UpdateFuelCostEntries(bool forceChangeEvent)
        {
            var oldFuelCostEntries = _FuelCostEntries;
            var tempTotal = 0m;
            foreach (FuelRegister detail in FuelRegistrations)
            {
                //tempTotal += detail.
                //ReceiptFuelDetailID.ReceiptInfo.TruckOrGenset == TruckOrGensetEnum.
                //Truck ?
                //detail.ReceiptFuelDetailID.Quantity :
                //0;
                tempTotal += detail.Total;
            }
            _FuelCostEntries = tempTotal;
            if (forceChangeEvent)
            {
                if (oldFuelCostEntries != _FuelCostEntries)
                {
                    OnChanged("FuelCostEntries", FuelCostEntries, _FuelCostEntries);
                }
            }
            ;
        }

        #endregion

        #endregion
        #region Mark/Unmarked for billing process
        private bool _Marked = false;
        private int _BillSeq;
        private int _BYear;
        [Custom("AllowEdit", "False")]
        public bool Marked
        {
            get { return _Marked; }
            set { SetPropertyValue("Marked", ref _Marked, value); }
        }
        public int BillSeq
        {
            get { return _BillSeq; }
            set { SetPropertyValue("BillSeq", ref _BillSeq, value); }
        }
        public int BYear
        {
            get { return _BYear; }
            set { SetPropertyValue("BYear", ref _BYear, value); }
        }
        
        [Action(AutoCommit = true, Caption = "Mark for Process")]
        public void MarkForProcess(){
            Marked = true;
        }
        [Action(AutoCommit = true, Caption = "Unmark for Process")]
        public void UnmarkForProcess(){
            Marked = false;
        }

        [NonPersistent]
        public int GKey
        {
            get { return this.Oid; }
        }
        #endregion

        #region Statement
        private decimal _NewBilling;
        private decimal _NewVatAmount;
        private decimal _NewGrossBilling;
        private decimal _NewWhtAmount;
        private decimal _NewNetBilling;
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Billing")]
        public decimal NewBilling
        {
            get { return _NewBilling; }
            set { SetPropertyValue("NewBilling", ref _NewBilling, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Tariff Billing")]
        public decimal TariffNewBilling
        {
            get { return _TariffNewBilling; }
            set { SetPropertyValue("TariffNewBilling", ref _TariffNewBilling, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 VAT Amount")]
        public decimal NewVatAmount
        {
            get { return _NewVatAmount; }
            set { SetPropertyValue("NewVatAmount", ref _NewVatAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Tariff VAT Amount")]
        public decimal TariffNewVatAmount
        {
            get { return _TariffNewVatAmount; }
            set { SetPropertyValue("TariffNewVatAmount", ref _TariffNewVatAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Gross Billing")]
        public decimal NewGrossBilling
        {
            get { return _NewGrossBilling; }
            set { SetPropertyValue("NewGrossBilling", ref _NewGrossBilling, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Tariff Gross Billing")]
        public decimal TariffNewGrossBilling
        {
            get { return _TariffNewGrossBilling; }
            set { SetPropertyValue("TariffNewGrossBilling", ref _TariffNewGrossBilling, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 WHT Amount")]
        public decimal NewWhtAmount
        {
            get { return _NewWhtAmount; }
            set { SetPropertyValue("NewWhtAmount", ref _NewWhtAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Tariff WHT Amount")]
        public decimal TariffNewWhtAmount
        {
            get { return _TariffNewWhtAmount; }
            set { SetPropertyValue("TariffNewWhtAmount", ref _TariffNewWhtAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Net Billing")]
        public decimal NewNetBilling
        {
            get { return _NewNetBilling; }
            set { SetPropertyValue("NewNetBilling", ref _NewNetBilling, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("01 Tariff Net Billing")]
        public decimal TariffNewNetBilling
        {
            get { return _TariffNewNetBilling; }
            set { SetPropertyValue("TariffNewNetBilling", ref _TariffNewNetBilling, value); }
        }
        
        // New Computation
        private NetBillingCompSetup _ComputationTemplate;
        public NetBillingCompSetup ComputationTemplate
        {
            get { return _ComputationTemplate; }
            set { SetPropertyValue("ComputationTemplate", ref _ComputationTemplate, value); }
        }
        [Action(AutoCommit=true, Caption="Calculate Billing")]
        public void CalculateStanfilcoNetBilling()
        {
            if (_ComputationTemplate!=null && _ComputationTemplate.NetBillSetupDetails.Count>0)
            {
                for (int i = TripCalculationDetails.Count - 1; i >= 0; i--)
                {
                    TripCalculationDetails[i].Delete();
                }
                for (int i = TariffTripCalculationDetails.Count - 1; i >= 0; i--)
                {
                    TariffTripCalculationDetails[i].Delete();
                }
                IOrderedEnumerable<NetBillingSetupDetail> orderBy = _ComputationTemplate.NetBillSetupDetails.OrderBy(o => o.Seq);
                foreach (var item in orderBy)
                {
                    TripCalculationDetail det = ReflectionHelper.CreateObject<TripCalculationDetail>(Session);
                    TariffTripCalculationDetail det1 = ReflectionHelper.CreateObject<TariffTripCalculationDetail>(Session);
                    det.GenJournalID = this;
                    det1.GenJournalID = this;
                    det.Seq = item.Seq;
                    det1.Seq = item.Seq;
                    det.Caption = item.Caption;
                    det1.Caption = item.Caption;
                    switch (item.ReferenceData)
                    {
                        case TariffReferenceEnum.None:
                            if (!string.IsNullOrEmpty(item.Formula))
                            {
                                det.Value = 0m;
                                det1.Value = 0m;
                            }
                            else
                            {
                                det.Value = item.GivenVal;
                                det1.Value = item.GivenVal;
                            }
                            break;
                        case TariffReferenceEnum.TruckerPay:
                            det.Value = TruckerPay;
                            det1.Value = TariffTruckerPay;
                            break;
                        case TariffReferenceEnum.RateAdjmt:
                            det.Value = RateAdjmt;
                            det1.Value = TariffFuelSubsidy;
                            break;
                        case TariffReferenceEnum.TrailerRate:
                            det.Value = TrailerRental;
                            det1.Value = TrailerRental;
                            break;
                        case TariffReferenceEnum.Insurance:
                            det.Value = Insurance;
                            det1.Value = Insurance;
                            break;
                        case TariffReferenceEnum.VatRate:
                            det.Value = VATRate;
                            det1.Value = VATRate;
                            break;
                        case TariffReferenceEnum.WhtRate:
                            det.Value = WHTRate;
                            det1.Value = WHTRate;
                            break;
                        case TariffReferenceEnum.Given:
                            det.Value = item.GivenVal;
                            det1.Value = item.GivenVal;
                            break;
                        case TariffReferenceEnum.Formula:
                            string[] strArr = item.Formula.Split(new char[] { '=' });
                            string trim01 = strArr[1].Trim();
                            string trim02 = trim01.Replace("(", string.Empty);
                            string trim03 = trim02.Replace(")", string.Empty);
                            string trim04 = trim03.Replace("+", string.Empty);
                            string trim05 = trim04.Replace("-", string.Empty);
                            string trim06 = trim05.Replace("*", string.Empty);
                            string trim07 = trim06.Replace("/100", string.Empty);
                            string[] split = trim07.Split();
                            string eval = trim01;
                            string eval1 = trim01;
                            NetBillingSetupDetail def = null;
                            foreach (var str in split)
                            {
                                if (!string.IsNullOrEmpty(str))
                                {
                                    def = _ComputationTemplate.NetBillSetupDetails.Where(o => o.Code == str.ToString()).FirstOrDefault();
                                    if (def != null)
                                    {
                                        TripCalculationDetail def2 = TripCalculationDetails.Where(o => o.Caption == def.Caption).FirstOrDefault();
                                        eval = eval.Replace(str, def2.Value.ToString());

                                        TariffTripCalculationDetail def1 = TariffTripCalculationDetails.Where(o => o.Caption == def.Caption).FirstOrDefault();
                                        eval1 = eval1.Replace(str, def1.Value.ToString());
                                    }
                                }
                            }
                            Expression e = new Expression(eval);
                            Expression e1 = new Expression(eval1);
                            det.Value = Convert.ToDecimal(e.Evaluate());
                            det1.Value = Convert.ToDecimal(e1.Evaluate());
                            if (item != null && item.Code == "GROSSPAY")
                            {
                                NewBilling = det.Value;
                                TariffNewBilling = det1.Value;
                            }
                            if (item != null && item.Code == "VATAMOUNT")
                            {
                                NewVatAmount = det.Value;
                                TariffNewVatAmount = det1.Value;
                            }
                            if (item != null && item.Code == "GROSSBILLING")
                            {
                                NewGrossBilling = det.Value;
                                TariffNewGrossBilling = det1.Value;
                            }
                            if (item != null && item.Code == "WHTAMOUNT")
                            {
                                NewWhtAmount = det.Value;
                                TariffNewWhtAmount = det1.Value;
                            }
                            if (item != null && item.Code == "NETBILLING")
                            {
                                NewNetBilling = det.Value;
                                TariffNewNetBilling = det1.Value;
                            }
                            break;
                        default:
                            det.Value = 0m;
                            det1.Value = 0m;
                            break;
                    }
                    if (item!=null && item.DebitAccount!=null)
                    {
                        det.GlAccount = item.DebitAccount;
                        det1.GlAccount = item.DebitAccount;
                    }
                    if (item != null && item.CreditAccount != null)
                    {
                        det.GlAccount2 = item.CreditAccount;
                        det1.GlAccount2 = item.CreditAccount;
                    }
                    det.Save();
                    det1.Save();
                }
                
                //Session.CommitTransaction();
            }
            else
            {
                throw new ApplicationException("Computation template has not been properly set up.");
            }
        }
        // Old Computation
        private string _DTRNo;
        private Tariff _Tariff;
        private TripLocation _Origin;
        private TripLocation _Destination;
        private decimal _TruckerPay;
        private decimal _RateAdjmt;
        private bool _RentTrailer;
        private decimal _TrailerRental;
        private decimal _Insurance;
        private decimal _Billing;
        private decimal _VATRate;
        private decimal _VATAmount;
        private decimal _GrossBilling;
        private decimal _WHTRate;
        private decimal _WHTAmount;
        private decimal _NetBilling;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string DTRNo {
            get { return _DTRNo; }
            set { SetPropertyValue("DTRNo", ref _DTRNo, value); }
        }

        [ImmediatePostData]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Tariff Tariff {
            get { return _Tariff; }
            set
            {
                SetPropertyValue("Tariff", ref _Tariff, value);
                if (!IsLoading && _Tariff != null)
                {
                    TruckerPay = _Tariff.TruckerPay;
                    RateAdjmt = _Tariff.RateAdjmt;
                    RentTrailer = _Tariff.TrailerRent;
                    Insurance = _Tariff.Insurance;
                    VATRate = _Tariff.TaxCode != null ?
                    _Tariff.TaxCode.Rate :
                    0;
                    WHTRate = _Tariff.WHTGroupCode != null ?
                    _Tariff.WHTGroupCode.WHTRate :
                    0;
                    Distance = _Tariff.Distance;
                    TurnAround = _Tariff.TurnAround;
                    Allowance = _Tariff.Allowance;
                    FuelAllocation = _Tariff.Fuel;
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

        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Trucker Pay")]
        public decimal TruckerPay {
            get { return _TruckerPay; }
            set { SetPropertyValue("TruckerPay", ref _TruckerPay, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal TariffTruckerPay
        {
            get { return _TariffTruckerPay; }
            set { SetPropertyValue("TariffTruckerPay", ref _TariffTruckerPay, value); }
        }

        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Fuel Subsidy")]
        public decimal RateAdjmt {
            get { return _RateAdjmt; }
            set { SetPropertyValue("RateAdjmt", ref _RateAdjmt, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal TariffFuelSubsidy
        {
            get { return _TariffFuelSubsidy; }
            set { SetPropertyValue("TariffFuelSubsidy", ref _TariffFuelSubsidy, value); }
        }

        [ImmediatePostData]
        //[Custom("AllowEdit", "False")]
        [DisplayName("Our Trailer")]
        public bool RentTrailer {
            get { return _RentTrailer; }
            set
            {
                SetPropertyValue("RentTrailer", ref _RentTrailer, value);
                //if (!IsLoading) {TrailerRental = _RentTrailer ? _Tariff.
                //    TrailerRental : 0;}
            }
        }

        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Trailer Rate")]
        public decimal TrailerRental {
            get { return _TrailerRental; }
            set { SetPropertyValue("TrailerRental", ref _TrailerRental, value); }
        }

        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Insurance {
            get { return _Insurance; }
            set { SetPropertyValue("Insurance", ref _Insurance, value); }
        }

        ////[PersistentAlias(
        ////"((TruckerPay + RateAdjmt) - TrailerRental) - Insurance")]
        //[PersistentAlias("(TruckerPay + RateAdjmt) - Insurance")]
        //[Custom("DisplayFormat", "n")]
        //public decimal Billing {
        //    get {
        //        object tempObject = EvaluateAlias("Billing");
        //        if (tempObject != null) {return (decimal)tempObject;} else {
        //            return 0;
        //        }
        //    }
        //}
        [Custom("DisplayFormat", "n")]
        [Persistent]
        public decimal Billing {
            get
            {
                if (_RentTrailer)
                {
                    _Billing = (_TruckerPay + _RateAdjmt) - _Insurance;
                } else
                {
                    _Billing = ((_TruckerPay + _RateAdjmt) - _TrailerRental) -
                    _Insurance;
                }
                return _Billing;
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
            get
            {
                var tempObject = EvaluateAlias("VATAmount");
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
                var tempObject = EvaluateAlias("GrossBilling");
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
        public decimal WHTRate {
            get { return _WHTRate; }
            set { SetPropertyValue("WHTRate", ref _WHTRate, value); }
        }

        [PersistentAlias("Billing * (WHTRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTAmount {
            get
            {
                var tempObject = EvaluateAlias("WHTAmount");
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
                var tempObject = EvaluateAlias("NetBilling");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Other Income

        private decimal? _IncomeShunting;
        private decimal? _IncomeGenset;
        private decimal? _IncomeKD;
        [Custom("DisplayFormat", "n")]
        public decimal? IncomeShunting {
            get { return _IncomeShunting; }
            set { SetPropertyValue("IncomeShunting", ref _IncomeShunting, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal? IncomeGenset {
            get { return _IncomeGenset; }
            set { SetPropertyValue("IncomeGenset", ref _IncomeGenset, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal? IncomeKD {
            get { return _IncomeKD; }
            set { SetPropertyValue("IncomeKD", ref _IncomeKD, value); }
        }

        #endregion

        #region Autopopulate

        [NonPersistent]
        private FATruck ThisTruck {
            get
            {
                FATruck truck = null;
                foreach (TruckRegistry item in TruckRegistrations)
                {
                    truck = item.TruckNo
                    ;
                }
                return truck;
            }
        }

        [NonPersistent]
        private Employee ThisDriver {
            get
            {
                Employee driver = null;
                foreach (DriverRegistry item in DriverRegistrations)
                {
                    driver = item.
                    Driver;
                }
                return driver;
            }
        }

        [NonPersistent]
        private FAGeneratorSet ThisGenset {
            get
            {
                FAGeneratorSet genset = null;
                foreach (GensetEntry item in GensetEntries)
                {
                    genset = item.GensetNo;
                }
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
        private DateTime _OutPcy;
        private DateTime _PostTrip;
        private DateTime _AtRampForLoading;
        private DateTime _AtStartLoading;
        private DateTime _AtFinishedLoading;
        private DateTime _StartDT;
        private decimal _GensetVatRate = 12m;
        private decimal _GensetWHRate = 5m;
        private decimal _GensetNetBillingLCY;
        private bool _MigratedToDollar = false;
        private decimal _TruckerPayRate;
        private decimal _FuelSubsidyRate;
        
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month {
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

        public string KdsParticulars
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (this.KDEntries != null && this.KDEntries.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in this.KDEntries)
                    {
                        if (!strRefs.Contains(item.Particulars))
                        {
                            strRefs.Add(item.Particulars);
                            sb.AppendFormat("{0},", item.Particulars);
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
                return !string.IsNullOrEmpty(sbRefs.ToString()) ? sbRefs.ToString().Remove(sbRefs.ToString().Length - 1) : string.Empty;
            }
        }
        public StanfilcoTrip(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator("Code", "ST")
            );
            OperationType = Session.FindObject<OperationType>(new BinaryOperator("Code",
            "ST"));
            var session = new UnitOfWork(Session.ObjectLayer);
            var source = session.FindObject<SourceType>(new BinaryOperator("Code", "ST")
            );
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ?
                source.GetNewNo() :
                null;
                source.Save();
                session.CommitChanges();
            }
            var tmp = Session.FindObject<Customer>(CriteriaOperator.Parse(
            "Contains([Name], 'STANFILCO TRIPS')"));
            Customer = tmp != null ?
            tmp :
            null;
            Memo = "Stanfilco Trip #" + SourceNo;
        }

        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Stanfilco trip.")
                ;
            }
        }

        protected override void OnSaving() {
            //this.AutoRegisterIncomeExpenseVer();
            base.OnSaving();
        }

        protected override void OnSaved() {
            AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }

        protected override void TriggerObjectChanged(ObjectChangeEventArgs args) {
            //this.IsIncExpNeedUpdate = true;
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                IsIncExpNeedUpdate = true;
            }
            base.TriggerObjectChanged(args);
            if (args.PropertyName == "EntryDate")
            {
                if (!IsLoading && _Terms != null)
                {
                    DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0, 0, 0));
                    DiscountDate = EntryDate.Add(new TimeSpan(_Terms.EarlyDaysToPay, 0,
                    0, 0));
                    DiscountRate = _Terms.EarlyDiscount;
                }
            }
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _ActualRun = null;
            _FuelConsumed = null;
            _FuelConsEntries = null;
            _FuelShunting = null;
            _IncomeShunting = null;
            _IncomeGenset = null;
            _IncomeKD = null;
        }
    }
}
