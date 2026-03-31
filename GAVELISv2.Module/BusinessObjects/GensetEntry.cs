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
using System.Linq;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class GensetEntry : GenJournalHeader {
        private TimeSpan _TotalTime;
        private TimeSpan _OtherTime;
        private TimeSpan _QueuingTime;
        private TimeSpan _TravelTime;
        private DateTime _ColdroomTime;
        private DateTime _ArrivalTime;
        private DateTime _DepartureTime;
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        [Persistent("ReferenceNo")]
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private GensetStatusEnum _Status;
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
        private FAGeneratorSet _GensetNo;
        [Persistent("RegularHrs")]
        private decimal? _RegularHrs;
        [Persistent("ColdRoomHrs")]
        private decimal? _ColdRoomHrs;
        [Persistent("OtherHrs")]
        private decimal? _OtherHrs;
        private decimal _TotalHrs;
        private decimal _RatePerHr;
        private decimal _Total;
        private decimal _VATRate;
        private decimal _VATAmount;
        private decimal _GrossBilling;
        private decimal _WHTRate;
        private decimal _WHTAmount;
        private decimal _NetBilling;
        private string _Remarks;
        private GensetReason _Remarks2;
        private decimal _MeterReading;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-GensetEntries")]
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
                            RatePerHr = ((StanfilcoTrip)_TripID).Tariff.
                            GensetRatePerHr;
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

        [NonPersistent]
        public Tariff ThisTariff {
            get
            {
                Tariff tarf = null;
                if (_TripID.GetType() == typeof(StanfilcoTrip))
                {
                    tarf = ((StanfilcoTrip)_TripID).Tariff;
                }
                return tarf;
            }
        }

        [Custom("AllowEdit", "False")]
        public string TripNo {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value); }
        }

        //public string ReferenceNo {
        //    get { return _ReferenceNo; }
        //    set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        //}
        [PersistentAlias("_ReferenceNo")]
        public string ReferenceNo
        {
            get {
                if (!IsLoading && !IsSaving)
                    UpdateReferenceNo(false);
                return _ReferenceNo; }
        }
        public void UpdateReferenceNo(bool forceChangeEvents)
        {
            string oldReferenceNo = _ReferenceNo;
            string tempReferenceNo = string.Empty;
            if (TripID.GetType() == typeof(StanfilcoTrip))
            {
                tempReferenceNo = ((StanfilcoTrip)TripID).DTRNo;
            }
            if (TripID.GetType() == typeof(DolefilTrip))
            {
                tempReferenceNo = ((DolefilTrip)TripID).DocumentNo;
            }
            if (TripID.GetType() == typeof(OtherTrip))
            {
                tempReferenceNo = ((OtherTrip)TripID).TripNo;
            }
            _ReferenceNo = tempReferenceNo;
            if (forceChangeEvents)
                OnChanged("ReferenceNo", oldReferenceNo, _ReferenceNo);
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

        public GensetStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != GensetStatusEnum.Current)
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
                        DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0
                        , 0, 0));
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

        [RuleRequiredField("", DefaultContexts.Save)]
        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public FAGeneratorSet GensetNo {
            get { return _GensetNo; }
            set
            {
                SetPropertyValue("GensetNo", ref _GensetNo, value);
                PopulatePreviousReading();
            }
        }

        [Aggregated,
        Association("GensetEntry-GensetUsageDetails")]
        public XPCollection<GensetUsageDetail> GensetUsageDetails {
            get { return
                GetCollection<GensetUsageDetail>("GensetUsageDetails"); }
        }
        [NonPersistent]
        public DateTime DepartureTime
        {
            get {
                var lastOrDefault = GensetUsageDetails.Where(o => o.Type == GensetUsageTypeEnum.Regular).LastOrDefault();
                if (lastOrDefault!=null)
                {
                    _DepartureTime = lastOrDefault.PlugIn;
                }
                return _DepartureTime; }
        }
        [NonPersistent]
        public DateTime ArrivalTime
        {
            get {
                var lastOrDefault = GensetUsageDetails.Where(o => o.Type == GensetUsageTypeEnum.Regular).LastOrDefault();
                if (lastOrDefault != null)
                {
                    _ArrivalTime = lastOrDefault.PlugOff;
                }
                return _ArrivalTime; }
        }
        [NonPersistent]
        public DateTime ColdroomTime
        {
            get {
                var lastOrDefault = GensetUsageDetails.Where(o => o.Type == GensetUsageTypeEnum.Coldroom).LastOrDefault();
                if (lastOrDefault != null)
                {
                    _ColdroomTime = lastOrDefault.PlugOff;
                }
                return _ColdroomTime; }
        }
        [NonPersistent]
        public string TravelTime
        {
            get {
                _TravelTime = TimeSpan.FromHours((Double)Math.Round(_RegularHrs!=null?_RegularHrs.Value:0m, 2, MidpointRounding.AwayFromZero));
                if (_TravelTime.Seconds > 30)
                {
                    TimeSpan ts;
                    if (_TravelTime.Days > 0)
                    {
                        ts = new TimeSpan(_TravelTime.Hours + (_TravelTime.Days * 24), _TravelTime.Minutes + 1, 0);
                    }
                    else
                    {
                        ts = new TimeSpan(_TravelTime.Hours, _TravelTime.Minutes + 1, 0);
                    }
                    _TravelTime = ts;
                }
                if (_TravelTime.Days > 0)
                {
                    //return _TravelTime.ToString("dd\\.hh\\:mm");
                    return string.Format("{0}:{1}", Math.Truncate(_TravelTime.TotalHours), _TravelTime.Minutes.ToString("D2"));
                }
                else
                {
                    return _TravelTime.ToString("hh\\:mm");
                }
            }
        }
        [NonPersistent]
        public string QueuingTime
        {
            get {
                _QueuingTime = TimeSpan.FromHours((Double)Math.Round(_ColdRoomHrs!=null?_ColdRoomHrs.Value:0m, 2, MidpointRounding.AwayFromZero));
                if (_QueuingTime.Seconds > 30)
                {
                    //TimeSpan ts = new TimeSpan(_QueuingTime.Hours, _QueuingTime.Minutes + 1, 0);
                    //_QueuingTime = ts;
                    TimeSpan ts;
                    if (_QueuingTime.Days > 0)
                    {
                        ts = new TimeSpan(_QueuingTime.Hours + (_QueuingTime.Days * 24), _QueuingTime.Minutes + 1, 0);
                    }
                    else
                    {
                        ts = new TimeSpan(_QueuingTime.Hours, _QueuingTime.Minutes + 1, 0);
                    }
                    _QueuingTime = ts;
                }
                if (_QueuingTime.Days > 0)
                {
                    //return _QueuingTime.ToString("dd\\.hh\\:mm");
                    return string.Format("{0}:{1}", Math.Truncate(_QueuingTime.TotalHours), _QueuingTime.Minutes.ToString("D2"));
                }
                else
                {
                    return _QueuingTime.ToString("hh\\:mm");
                }
            }
        }
        [NonPersistent]
        public string OtherTime
        {
            get {
                //_OtherTime = TimeSpan.FromHours((Double)Math.Round(_OtherHrs.Value, 2, MidpointRounding.AwayFromZero));
                //if (_OtherTime.Seconds > 30)
                //{
                //    TimeSpan ts = new TimeSpan(_OtherTime.Hours, _OtherTime.Minutes + 1, 0);
                //    _OtherTime = ts;
                //}
                _OtherTime = TimeSpan.FromHours((Double)Math.Round(_OtherHrs != null ? _OtherHrs.Value : 0m, 2, MidpointRounding.AwayFromZero));
                if (_OtherTime.Seconds > 30)
                {
                    TimeSpan ts;
                    if (_OtherTime.Days > 0)
                    {
                        ts = new TimeSpan(_OtherTime.Hours + (_OtherTime.Days * 24), _OtherTime.Minutes + 1, 0);
                    }
                    else
                    {
                        ts = new TimeSpan(_OtherTime.Hours, _OtherTime.Minutes + 1, 0);
                    }
                    _OtherTime = ts;
                }
                if (_OtherTime.Days > 0)
                {
                    //return _OtherTime.ToString("dd\\.hh\\:mm");
                    return string.Format("{0}:{1}", Math.Truncate(_OtherTime.TotalHours), _OtherTime.Minutes.ToString("D2"));
                }
                else
                {
                    return _OtherTime.ToString("hh\\:mm");
                }
            }
        }
        [NonPersistent]
        public string TotalTime
        {
            get {
                //_TotalTime = TimeSpan.FromHours((Double)Math.Round(TotalHrs, 2, MidpointRounding.AwayFromZero));
                //if (_TotalTime.Seconds > 30)
                //{
                //    TimeSpan ts = new TimeSpan(_TotalTime.Hours, _TotalTime.Minutes + 1, 0);
                //    _TotalTime = ts;
                //}
                _TotalTime = TimeSpan.FromHours((Double)Math.Round(TotalHrs, 2, MidpointRounding.AwayFromZero));
                if (_TotalTime.Seconds > 30)
                {
                    TimeSpan ts;
                    if (_TotalTime.Days > 0)
                    {
                        ts = new TimeSpan(_TotalTime.Hours + (_TotalTime.Days * 24), _TotalTime.Minutes + 1, 0);
                    }
                    else
                    {
                        ts = new TimeSpan(_TotalTime.Hours, _TotalTime.Minutes + 1, 0);
                    }
                    _TotalTime = ts;
                }
                if (_TotalTime.Days > 0)
                {
                    // return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    return string.Format("{0}:{1}", Math.Truncate(_TotalTime.TotalHours), _TotalTime.Minutes.ToString("D2"));
                    //return _TotalTime.ToString("dd\\.hh\\:mm");
                }
                else
                {
                    return _TotalTime.ToString("hh\\:mm");
                }
            }
        }
        #region Hours Computation
        private decimal _PreCooling;
        [Custom("DisplayFormat", "n")]
        public decimal PreCooling
        {
            get { return _PreCooling; }
            set { SetPropertyValue("PreCooling", ref _PreCooling, value); }
        }
        
        [PersistentAlias("_RegularHrs")]
        [Custom("DisplayFormat", "n")]
        public decimal? RegularHrs {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _RegularHrs == null)
                    {
                        UpdateRegularHrs(false);
                    }
                } catch (Exception)
                {
                }
                return _RegularHrs;
            }
        }

        [PersistentAlias("_ColdRoomHrs")]
        [Custom("DisplayFormat", "n")]
        public decimal? ColdRoomHrs {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ColdRoomHrs == null)
                    {
                        UpdateColdRoomHrs(false);
                    }
                } catch (Exception)
                {
                }
                return _ColdRoomHrs;
            }
        }

        [PersistentAlias("_OtherHrs")]
        [Custom("DisplayFormat", "n")]
        public decimal? OtherHrs {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _OtherHrs == null)
                    {
                        UpdateOtherHrs(false);
                    }
                } catch (Exception)
                {
                }
                return _OtherHrs;
            }
        }

        public void UpdateRegularHrs(bool forceChangeEvent)
        {
            decimal? oldRegularHrs = _RegularHrs;
            decimal tempTotal = 0m;
            foreach (GensetUsageDetail detail in GensetUsageDetails)
            {
                if (detail
                .Type == GensetUsageTypeEnum.Regular)
                {
                    tempTotal += detail.Total
                    ;
                }
            }
            _RegularHrs = tempTotal;
            if (forceChangeEvent)
            {
                if (oldRegularHrs != _RegularHrs)
                {
                    OnChanged("RegularHrs", RegularHrs,
    _RegularHrs);
                }
            }
        }

        public void UpdateColdRoomHrs(bool forceChangeEvent) {
            decimal? oldColdRoomHrs = _ColdRoomHrs;
            decimal tempTotal = 0m;
            foreach (GensetUsageDetail detail in GensetUsageDetails)
            {
                if (detail
                .Type == GensetUsageTypeEnum.Coldroom)
                {
                    tempTotal += detail.
                    Total;
                }
            }
            _ColdRoomHrs = tempTotal;
            if (forceChangeEvent)
            {
                if (oldColdRoomHrs != _ColdRoomHrs)
                {
                    OnChanged("ColdRoomHrs", ColdRoomHrs,
                    _ColdRoomHrs);
                }
            }
            ;
        }

        public void UpdateOtherHrs(bool forceChangeEvent) {
            decimal? oldOtherHrs = _OtherHrs;
            decimal tempTotal = 0m;
            foreach (GensetUsageDetail detail in GensetUsageDetails)
            {
                if (detail
                .Type == GensetUsageTypeEnum.Others)
                {
                    tempTotal += detail.Total;
                }
            }
            _OtherHrs = tempTotal;
            if (forceChangeEvent)
            {
                if (oldOtherHrs != _OtherHrs)
                {
                    OnChanged("OtherHrs", OtherHrs, _OtherHrs);
                }
            }
            ;
        }

        [PersistentAlias("RegularHrs + ColdRoomHrs + OtherHrs")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalHrs
        {
            get
            {
                object tempObject = EvaluateAlias("TotalHrs");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Action(Caption = "Register Hours", AutoCommit = true)]
        public void RegisterHours()
        {
            #region New Trip Odo Logging
            if (_GensetNo != null)
            {
                GensetHoursRegister newLog = null;
                //thisReceipt
                if (this.GensetEntryHoursLogs.Count > 0)
                {
                    newLog = this.GensetEntryHoursLogs.FirstOrDefault();
                }
                else
                {
                    newLog = ReflectionHelper.CreateObject<GensetHoursRegister>(this.Session);
                }
                newLog.Genset = _GensetNo;
                newLog.EntryReference = this;
                newLog.EntryDate = this.EntryDate;
                newLog.Reading = this.TotalHrs;
                newLog.Save();
            }
            #endregion
        }
        [Aggregated,
        Association("GensetEntryHoursLogs")]
        public XPCollection<GensetHoursRegister> GensetEntryHoursLogs
        {
            get
            {
                return GetCollection<GensetHoursRegister>("GensetEntryHoursLogs"
                    );
            }
        }
        //[PersistentAlias("PreCooling + RegularHrs + ColdRoomHrs + OtherHrs")]
        //[DisplayName("Total Time")]
        //[Custom("DisplayFormat", "n")]
        //public TimeSpan TotalSpanHrs
        //{
        //    get
        //    {
        //        object tempObject = EvaluateAlias("TotalSpanHrs");
        //        if (tempObject != null)
        //        {
        //            return (TimeSpan)tempObject;
        //        }
        //        else
        //        {
        //            return new TimeSpan();
        //        }
        //    }
        //}
        //[PersistentAlias("PreCooling + RegularHrs + ColdRoomHrs + OtherHrs")]
        //[Custom("DisplayFormat", "n")]
        //public decimal TotalHrs {
        //    get
        //    {
        //        object tempObject = EvaluateAlias("TotalHrs");
        //        if (tempObject != null)
        //        {

        //            //#region RegularHrs

        //            //int iRegW = (int)Math.Truncate(RegularHrs.Value);
        //            //int iRegF = (int)((RegularHrs.Value - iRegW) * 100);
        //            //TimeSpan tsReg = new TimeSpan(iRegW, iRegF, 0);

        //            //#endregion

        //            //#region ColdRoomHrs

        //            //int iTsW = (int)Math.Truncate(ColdRoomHrs.Value);
        //            //int iTsF = (int)((ColdRoomHrs.Value - iTsW) * 100);
        //            //TimeSpan tsCold = new TimeSpan(iTsW, iTsF, 0);

        //            //#endregion


        //            //#region OtherHrs

        //            //int iOtW = (int)Math.Truncate(OtherHrs.Value);
        //            //int iOtF = (int)((OtherHrs.Value - iOtW) * 100);
        //            //TimeSpan tsOthers = new TimeSpan(iOtW, iOtF, 0);

        //            //#endregion

        //            TimeSpan tsTot = tsReg + tsCold + tsOthers;
        //            //double retf = (double)tsTot.Minutes / 100;
        //            //double ret = ((double)tsTot.Days * 24) + (double)tsTot.Hours + retf;



        //            return (decimal)ret;
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //}

        #endregion

        [Custom("DisplayFormat", "n")]
        public decimal RatePerHr {
            get { return _RatePerHr; }
            set { SetPropertyValue("RatePerHr", ref _RatePerHr, value); }
        }

        [PersistentAlias("TotalHrs * RatePerHr")]
        [DisplayName("Billing")]
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

        [Custom("DisplayFormat", "n")]
        public decimal VATRate {
            get { return _VATRate; }
            set { SetPropertyValue("VATRate", ref _VATRate, value); }
        }

        [PersistentAlias("Total * (VATRate/100)")]
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

        [PersistentAlias("Total + VATAmount")]
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

        [PersistentAlias("Total * (WHTRate/100)")]
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

        public GensetReason Remarks2 {
            get { return _Remarks2; }
            set { SetPropertyValue("Remarks2", ref _Remarks2, value); }
        }

        public bool IsHustling
        {
            get {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip) && ((StanfilcoTrip)_TripID).Hustling)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
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
        public GensetEntry(Session session)
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
            "Code", "GS"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "GS"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "GS"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Memo = "Genset #" + SourceNo;
        }
        
        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Genset transactions."
                );
            }
            if (this.GensetEntryHoursLogs.Count > 0)
            {
                foreach (var item in GensetEntryHoursLogs)
                {
                    item.EntryReference = null;
                    item.Save();
                }
            }

        }

        #region Hour Meter Registration

        public override DateTime EntryDate {
            get { return base.EntryDate; }
            set
            {
                base.EntryDate = value;
                // Populate Previous Reading
                PopulatePreviousReading();
            }
        }

        private void PopulatePreviousReading() {
            if (!IsLoading)
            {
                if (_GensetNo != null)
                {
                    GetLastReading(_GensetNo);
                } else
                {
                    PrevReadingDate = DateTime.MinValue;
                    PrevMeterReading = 0m;
                    PrevNoOfLiters = 0m;
                }
            }
        }

        private void GetLastReading(FixedAsset asset)
        {
            LastReadings rdngsFuel = new LastReadings();
            LastReadings rdngsLast = new LastReadings();
            rdngsFuel = asset.GetFuelLastReadingBeforeDate(EntryDate);
            rdngsLast = asset.GetLastReadingBeforeDate(EntryDate);
            if (rdngsFuel != null)
            {
                MeterReading = rdngsLast != null ? rdngsLast.LastOdoRead : 0m; // ok
                PrevMeterReading = rdngsFuel.LastOdoFuelReading;
                PrevReadingDate = rdngsFuel.LastFuelDate; // ok
                PrevNoOfLiters = rdngsFuel.LastFuelLiters; // ok
            }
            else
            {
                MeterReading = rdngsLast != null ? rdngsLast.LastOdoRead : 0m; // ok
                PrevMeterReading = 0m;
                PrevReadingDate = DateTime.MinValue; // ok
                PrevNoOfLiters = 0m; // ok
            }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal MeterReading {
            get { return _MeterReading; }
            set { SetPropertyValue<decimal>("MeterReading", ref _MeterReading, value); }
        }

        private decimal _NoOfLiters;
        public decimal NoOfLiters {
            get { return _NoOfLiters; }
            set { SetPropertyValue<decimal>("NoOfLiters", ref _NoOfLiters, value); }
        }

        private DateTime _PrevReadingDate;
        [Custom("AllowEdit", "False")]
        public DateTime PrevReadingDate {
            get { return _PrevReadingDate; }
            set { SetPropertyValue<DateTime>("PrevReadingDate", ref _PrevReadingDate, value); }
        }

        private decimal _PrevMeterReading;
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevMeterReading {
            get { return _PrevMeterReading; }
            set { SetPropertyValue<decimal>("PrevMeterReading", ref _PrevMeterReading, value); }
        }

        private decimal _PrevNoOfLiters;
        [Custom("AllowEdit", "False")]
        public decimal PrevNoOfLiters {
            get { return _PrevNoOfLiters; }
            set { SetPropertyValue<decimal>("PrevNoOfLiters", ref _PrevNoOfLiters, value); }
        }

        private OdometerRegister _MeterRegistryID;
        [Custom("AllowEdit", "False")]
        public OdometerRegister MeterRegistryID {
            get { return _MeterRegistryID; }
            set { SetPropertyValue<OdometerRegister>("MeterRegistryID", ref _MeterRegistryID, value); }
        }

        protected override void OnSaving()
        {
            //this.AutoRegisterIncomeExpenseVer();
            //if (_GensetNo != null && _GensetNo.VehicleOdoRegisters.Count >= 1)
            //{
            //    if (_GensetNo.ValidOdoRegDate(EntryDate))
            //    {
            //        OdometerRegister odo = null;
                    
            //        if (this.Oid == -1)
            //        {
            //            odo = ReflectionHelper.CreateObject<OdometerRegister>(Session);
            //            MeterRegistryID = odo;
            //        }
            //        else if (MeterRegistryID!=null)
            //        {
            //            odo = Session.GetObjectByKey<OdometerRegister>(MeterRegistryID.Oid);
            //            if (IsDeleted)
            //            {
            //                MeterRegistryID = null;
            //                odo.Delete();
            //                odo = null;
            //            }
            //        }
            //        if (odo!=null)
            //        {
            //            odo.Fleet = _GensetNo;
            //            odo.MeterType = MeterEntryTypeEnum.Odometer;
            //            odo.LogType = MeterLogTypeEnum.Fuel;
            //            odo.EntryDate = EntryDate;
            //            odo.ReportedBy = _Driver ?? null;
            //            odo.Reference = TripNo;
            //            odo.Liters = _NoOfLiters;
            //            odo.LastFuelOdoReading = _PrevMeterReading;
            //            odo.Reading = _MeterReading;
            //            odo.Save();
            //        }
            //    }
            //    else
            //    {
            //        if (!TempSkipOdoReg)
            //        {
            //            XtraMessageBox.Show("Cannot register meter before the initial meter log date. Meter will not be registered.", "Before Initial Meter Log", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            //        }
            //    }
            //}
            //if (IsDeleted && this.GensetEntryHoursLogs.Count > 0)
            //{
            //    foreach (var item in GensetEntryHoursLogs)
            //    {
            //        item.EntryReference = null;
            //        item.Delete();
            //        //item.Save();
            //    }
            //}
            if (!IsDeleted)
            {
                RegisterHours();
            }
            base.OnSaving();
        }
     
        protected override void OnSaved() {
            //this.IsIncExpNeedUpdate = true;
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            //this.IsIncExpNeedUpdate = false;
            // Auto Correct Start
            long _SeqNo = GensetNo.GetFirstSequenceNo() - 1;
            bool correctNext = false;
            decimal lastReading = 0m;
            decimal lastFuelReading = 0m;
            _GensetNo.VehicleOdoRegisters.Sorting.Add(new SortProperty("EntryDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
            foreach (OdometerRegister item in _GensetNo.VehicleOdoRegisters)
            {
                _SeqNo++;
                OdometerRegister odr = Session.GetObjectByKey<OdometerRegister>(
                item.Oid);
                if (correctNext)
                {
                    LastReadings lstRrd = new LastReadings();
                    lstRrd = _GensetNo.GetLastReadingBeforeDate(odr.EntryDate);
                    //Difference = _Reading - lstRrd.LastOdoRead;
                    //Life = lstRrd.LastLife + _Difference;

                    LastReadings lstRrdFl = new LastReadings();
                    LastReadings lstRrdSrv = new LastReadings();
                    if (odr.LogType == MeterLogTypeEnum.Service && odr.PrevMaintenanceID != null)
                    {
                        lstRrdSrv = _GensetNo.GetServiceIdLastReadingBeforeDate(odr.EntryDate, odr.PrevMaintenanceID);
                    } else if (odr.LogType == MeterLogTypeEnum.Service && odr.PrevMaintenanceID == null)
                    {
                        lstRrdSrv = _GensetNo.GetServiceLastReadingBeforeDate(odr.EntryDate);
                    } else if (odr.LogType == MeterLogTypeEnum.Fuel)
                    {
                        lstRrdFl = _GensetNo.GetFuelLastReadingBeforeDate(odr.EntryDate);

                        //lstRrdFl = _GensetNo.GetFuelLastReadingBeforeDate(odr.EntryDate);
                        //Range = lstRrdFl.LastFuelLife != 0 ? Life - lstRrdFl.LastFuelLife : 0;
                        switch (odr.LogType)
                        {
                            case MeterLogTypeEnum.Initial:
                                break;
                            case MeterLogTypeEnum.Log:
                                odr.Difference = item.Reading - lstRrd.LastOdoRead;
                                odr.Life = lstRrd.LastLife + odr.Difference;
                                odr.Range = 0m;
                                break;
                            case MeterLogTypeEnum.Change:
                                odr.Difference = 0m;
                                odr.Life = lstRrd.LastLife;
                                odr.Range = 0m;
                                break;
                            case MeterLogTypeEnum.Correct:
                                break;
                            case MeterLogTypeEnum.Fuel:
                                odr.Difference = item.Reading - lstRrd.LastOdoRead;
                                odr.Life = lstRrd.LastLife + odr.Difference;
                                odr.Range = lstRrdFl.LastFuelLife != 0 ? odr.Life - lstRrdFl.LastFuelLife : 0;
                                break;
                            case MeterLogTypeEnum.Service:
                                odr.Difference = item.Reading - lstRrd.LastOdoRead;
                                odr.Life = lstRrd.LastLife + odr.Difference;
                                odr.ServiceRange = lstRrdSrv.LastServiceLife != 0 ? odr.Life - lstRrdSrv.LastServiceLife : 0;
                                odr.ServiceIdRange = lstRrdSrv.LastServiceIdLife != 0 ? odr.Life - lstRrdSrv.LastServiceIdLife : 0;
                                break;
                            case MeterLogTypeEnum.None:
                                break;
                            default:
                                break;
                        }
                        //odr.Difference = item.Reading - lastReading;
                        //odr.Range = odr.LogType == MeterLogTypeEnum.Fuel && lastFuelReading != 0 ? item.Reading - lastFuelReading : 0;
                        //odr.Life = lastReading + odr.Difference;
                        odr.Save();
                    }
                    lastReading = odr.Reading;
                    if (odr.LogType == MeterLogTypeEnum.Fuel)
                    {
                        lastFuelReading = odr.Reading;
                    }
                    if (odr.SeqNo != _SeqNo)
                    {
                        correctNext = true;
                        odr.SeqNo = _SeqNo;
                        odr.Corrected = true;
                        odr.Save();
                    }
                    if (odr.Difference < 0)
                    {
                        if (correctNext)
                        {
                            odr.Difference = Math.Abs(odr.Difference);
                            odr.Range = Math.Abs(odr.Range);
                        } else
                        {
                            if (!TempSkipOdoReg)
                            {
                                XtraMessageBox.Show("Negative Difference from previous reading detected!", "Negative Difference", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            }
                            // throw new UserFriendlyException("Negative Difference from previous reading detected!");
                        }
                    }
                }
                if (correctNext)
                {
                    GensetNo.SeqNo = _SeqNo;
                    Session.CommitTransaction();
                }
                // Auto Correct End
            }
            base.OnSaved();
        }
        
        #endregion
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            //this.IsIncExpNeedUpdate = true;
            Console.WriteLine(string.Format("{0} : {1} -> {2}", args.PropertyName, args.OldValue, args.NewValue));
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }

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

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _RegularHrs = null;
            _ColdRoomHrs = null;
            _OtherHrs = null;
        }
    }
}
