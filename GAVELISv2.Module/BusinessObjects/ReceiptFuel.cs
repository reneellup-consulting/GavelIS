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
    public class ReceiptFuel : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private ReceiptFuelStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Vendor _Vendor;
        private string _VendorAddress;
        private string _ShipToAddress;
        private Terms _Terms;
        private string _InvoiceNo;
        private PurchaseOrderFuel _PurchaseOrderNo;
        private decimal _Adjusted;
        private GenJournalHeader _TripUsed;
        private FuelUsageClassEnum _FuelUsageClassification;
        private string _FuelUsageDescription;
        private bool _Tagged;

        public override DateTime EntryDate
        {
            get
            {
                return base.EntryDate;
            }
            set
            {
                base.EntryDate = value;
                if (!IsLoading)
                {
                    if (_TruckNo != null)
                    {
                        FuelUsageClassification = _TruckNo.FuelUsageClassification;
                        GetLastGasUp(_TruckNo);
                        //OdoRead = _TruckNo.LastOdoReading;
                        //PrevDate = _TruckNo.LastFuelDate;
                        //PrevOdoRead = _TruckNo.LastOdoFuelReading;
                        //NoOfLtrs = _TruckNo.LastFuelLiters;
                        //PrevTotalAmt = _TruckNo.LastFuelTotalAmt;
                        //try
                        //{
                        //    PrevPrice = _PrevTotalAmt / _NoOfLtrs;
                        //} catch (Exception)
                        //{
                        //}
                    }
                    else if (_GensetNo != null)
                    {
                        GetLastGasUp(_GensetNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        PrevOdoRead = 0;
                        PrevHrsRead = 0;
                        NoOfLtrs = 0;
                        PrevPrice = 0;
                        PrevTotalAmt = 0;
                    }
                }
            }
        }
        //[RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
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

        [Size(1000)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        public ReceiptFuelStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != ReceiptFuelStatusEnum.Current)
                    {
                        Approved =
                        true;
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

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("Customer-FuelPayables")]
        public Vendor Vendor {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null)
                {
                    VendorAddress = _Vendor.FullAddress;
                    Terms = _Vendor.Terms;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string VendorAddress {
            get { return _VendorAddress; }
            set { SetPropertyValue("VendorAddress", ref _VendorAddress, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string ShipToAddress {
            get { return _ShipToAddress; }
            set { SetPropertyValue("ShipToAddress", ref _ShipToAddress, value); }
        }

        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }

        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string InvoiceNo {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
        }

        public PurchaseOrderFuel PurchaseOrderNo {
            get { return _PurchaseOrderNo; }
            set { SetPropertyValue("PurchaseOrderNo", ref _PurchaseOrderNo,
                value); }
        }
        public PurchaseOrder PurchaseOrderNo2
        {
            get { return _PurchaseOrderNo2; }
            set
            {
                SetPropertyValue("PurchaseOrderNo2", ref _PurchaseOrderNo2,
                    value);
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set
            {
                SetPropertyValue("Adjusted", ref _Adjusted, value);
                if (!IsLoading)
                {
                    _Vendor.UpdateBalance(true);
                    APRegistry apr =
                    Session.FindObject<APRegistry>(CriteriaOperator
                    .Parse("[GenJournalID.SourceNo] = '" + SourceNo + "'"));
                    if (apr != null)
                    {
                        apr.AmtPaid = _Adjusted;
                        apr.Save();
                    }
                }
            }
        }

        private List<string> _Refs;
        [Custom("AllowEdit", "False")]
        public List<string> Refs
        {
            get { return _Refs; }
            set { SetPropertyValue<List<string>>("Refs", ref _Refs, value); }
        }

        public GenJournalHeader TripUsed {
            get { return _TripUsed; }
            set { SetPropertyValue("TripUsed", ref _TripUsed, value); }
        }

        public FuelUsageClassEnum FuelUsageClassification {
            get { return _FuelUsageClassification; }
            set { SetPropertyValue<FuelUsageClassEnum>("FuelUsageClassification", ref _FuelUsageClassification, value); }
        }

        public bool Tagged
        {
            get { return ReceiptFuelUsageDetails.Count>0; }
        }

        [Size(500)]
        [Custom("AllowEdit", "False")]
        public string FuelUsageDescription {
            get { return _FuelUsageDescription; }
            set { SetPropertyValue<string>("FuelUsageDescription", ref _FuelUsageDescription, value); }
        }

        public TripType TripType
        {
            get { return _TripType; }
            set { SetPropertyValue("TripType", ref _TripType, value); }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        #region Fuel Information

        // --> if TruckOrGenset = TruckOrGensetEnum.Truck then return TruckNo.SeriesNo
        // --> if TruckOrGenset = TruckOrGensetEnum.Genset then return GensetNo.SeriesNo
        // --> if TruckOrGenset = TruckOrGensetEnum.Other then return OtherVehicle.Description
        [NonPersistent]
        public string UnitNo
        {
            get
            {
                switch (_TruckOrGenset)
                {
                    case TruckOrGensetEnum.Truck:
                        return _TruckNo != null ? _TruckNo.SeriesNo : null;
                    case TruckOrGensetEnum.Genset:
                        return _GensetNo != null ? _GensetNo.SeriesNo : null;
                    case TruckOrGensetEnum.Other:
                        return _OtherVehicle != null ? _OtherVehicle.Description : null;
                    default:
                        return null;
                }
            }
        }

        // --> if TruckOrGenset = TruckOrGensetEnum.Truck then return OdoRead
        // --> if TruckOrGenset = TruckOrGensetEnum.Genset then return MtrRead
        // --> if TruckOrGenset = TruckOrGensetEnum.Other then return OthRead
        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Meter
        {
            get
            {
                switch (_TruckOrGenset)
                {
                    case TruckOrGensetEnum.Truck:
                        return _OdoRead;
                    case TruckOrGensetEnum.Genset:
                        return _MtrRead;
                    case TruckOrGensetEnum.Other:
                        return _OthRead;
                    default:
                        return 0m;
                }
            }
        }

        private TruckOrGensetEnum _TruckOrGenset;
        private FATruck _TruckNo;
        private decimal _OdoRead;
        private FAGeneratorSet _GensetNo;
        private FAOtherVehicle _OtherVehicle;
        private decimal _MtrRead;
        private Employee _Driver;
        [ImmediatePostData]
        public TruckOrGensetEnum TruckOrGenset {
            get { return _TruckOrGenset; }
            set
            {
                SetPropertyValue("TruckOrGenset", ref _TruckOrGenset, value);
                if (!IsLoading)
                {
                    switch (_TruckOrGenset)
                    {
                        case TruckOrGensetEnum.Truck:
                            GensetNo = null;
                            OtherNo = null;
                            OtherVehicle = null;
                            MtrRead = 0;
                            OthRead = 0;
                            break;
                        case TruckOrGensetEnum.Genset:
                            TruckNo = null;
                            OtherNo = null;
                            OtherVehicle = null;
                            OdoRead = 0;
                            OthRead = 0;
                            break;
                        case TruckOrGensetEnum.Other:
                            TruckNo = null;
                            GensetNo = null;
                            OdoRead = 0;
                            MtrRead = 0;
                            break;
                        default:
                            TruckNo = null;
                            GensetNo = null;
                            OtherNo = null;
                            OtherVehicle = null;
                            OdoRead = 0;
                            MtrRead = 0;
                            OthRead = 0;
                            break;
                    }
                    PrevDate = DateTime.MinValue;
                    PrevOdoRead = 0;
                    PrevHrsRead = 0;
                    PrevLife = 0m;
                    NoOfLtrs = 0;
                    PrevPrice = 0;
                    PrevTotalAmt = 0;
                }
            }
        }
        [DisplayName("DTR #s")]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string DtrNo
        {
            get {
                StringBuilder sbr = new StringBuilder();
                foreach (var item in ReceiptFuelUsageDetails)
                {
                    sbr.AppendFormat("{0},", item.TripNo.TripReferenceNo);
                }
                if (sbr.Length > 0)
                {
                    sbr.Remove(sbr.Length - 1, 1);
                }
                return sbr.ToString(); }
        }

        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string DtrNoForRpt
        {
            get
            {
                StringBuilder sbr = new StringBuilder();
                foreach (var item in ReceiptFuelUsageDetails)
                {
                    sbr.AppendFormat("{0},\n", item.TripNo.TripReferenceNo);
                }
                if (sbr.Length > 0)
                {
                    sbr.Remove(sbr.Length - 1, 1);
                }
                return sbr.ToString();
            }
        }

        private GenJournalHeader oldUseInTrip = null;
        [Custom("AllowEdit", "False")]
        public GenJournalHeader UseInTrip
        {
            get { return _UseInTrip; }
            set {
                oldUseInTrip = _UseInTrip;
                SetPropertyValue("UseInTrip", ref _UseInTrip, value);
                if (!IsSaving && !IsLoading)
                {
                    if (_UseInTrip != null)
                    {
                        FuelUsageDescription = _UseInTrip.FuelUsageDescriptionForReceipt;
                    }
                    else
                    {
                        FuelUsageDescription = string.Empty;
                    }
                }
            }
        }

        [Action(Caption = "Get Previous Reading", AutoCommit = true)]
        public void GetPreviousReading() {
            if (_TruckNo != null)
            {
                FuelUsageClassification = _TruckNo.FuelUsageClassification;
                //Driver = _TruckNo.Operator ?? null;
                GetLastGasUp(_TruckNo);
            }
            else
            {
                PrevDate = DateTime.MinValue;
                OdoRead = 0m;
                PrevOdoRead = 0m;
                PrevLife = 0m;
                NoOfLtrs = 0m;
                PrevPrice = 0m;
                PrevTotalAmt = 0m;
            }
        }
        public FATruck TruckNo {
            get { return _TruckNo; }
            set
            {
                SetPropertyValue("TruckNo", ref _TruckNo, value);
                if (!IsLoading)
                {
                    if (_TruckNo != null)
                    {
                        FuelUsageClassification = _TruckNo.FuelUsageClassification;
                        Driver = _TruckNo.Operator ?? null;
                        GetLastGasUp(_TruckNo);
                    } else
                    {
                        PrevDate = DateTime.MinValue;
                        OdoRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        public FAOtherVehicle OtherNo
        {
            get { return _OtherNo; }
            set
            {
                SetPropertyValue("OtherNo", ref _OtherNo, value);
                if (!IsLoading)
                {
                    if (_OtherNo != null)
                    {
                        FuelUsageClassification = _OtherNo.FuelUsageClassification;
                        Driver = _OtherNo.Operator ?? null;
                        GetLastGasUp(_OtherNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        OthRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoRead {
            get { return _OdoRead; }
            set { SetPropertyValue("OdoRead", ref _OdoRead, value); }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OthRead
        {
            get { return _OthRead; }
            set { SetPropertyValue("OthRead", ref _OthRead, value); }
        }
        #region New Previous Gasup Details
        private void GetLastGasUp(FixedAsset asset)
        {
            // Retreive selected fleet
            FixedAsset selFa = null;
            switch (_TruckOrGenset)
            {
                case TruckOrGensetEnum.Truck:
                    if (_TruckNo != null)
                    {
                        selFa = _TruckNo;
                    }
                    break;
                case TruckOrGensetEnum.Genset:
                    if (_GensetNo != null)
                    {
                        selFa = _GensetNo;
                    }
                    break;
                case TruckOrGensetEnum.NotApplicable:
                    break;
                case TruckOrGensetEnum.Other:
                    if (_OtherVehicle != null)
                    {
                        selFa = _OtherVehicle;
                    }
                    break;
                default:
                    break;
            }
            if (selFa != null)
            {
                //FuelOdoRegistry fuelLog = null;
                if (EntryDate != DateTime.MinValue)
                {
                    string seq = EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, 999999)
                       : string.Empty;
                    decimal toDecimal = Convert.ToDecimal(seq);
                    if (selFa.GetType() == typeof(FATruck))
                    {
                        FATruck faTruck = selFa as FATruck;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null && o.ReceiptId != this && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevDate = data.EntryDate;
                            OdoRead = data.Reading;
                            PrevOdoRead = data.Reading;
                            PrevLife = data.Life;
                            NoOfLtrs = data.ReceiptId != null ? data.ReceiptId.TotalQty.Value : 0m;
                            PrevPrice = data.ReceiptId != null ? data.ReceiptId.Price.Value : 0m;
                            PrevTotalAmt = data.ReceiptId != null ? data.ReceiptId.Total.Value : 0m;
                        }
                    }
                    else if (selFa.GetType() == typeof(FAGeneratorSet))
                    {
                        FAGeneratorSet faTruck = selFa as FAGeneratorSet;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null && o.ReceiptId != this && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevDate = data.EntryDate;
                            MtrRead = data.Reading;
                            PrevOdoRead = data.Reading;
                            PrevLife = data.Life;
                            NoOfLtrs = data.ReceiptId != null ? data.ReceiptId.TotalQty.Value : 0m;
                            PrevPrice = data.ReceiptId != null ? data.ReceiptId.Price.Value : 0m;
                            PrevTotalAmt = data.ReceiptId != null ? data.ReceiptId.Total.Value : 0m;
                        }
                    }
                    else if (selFa.GetType() == typeof(FAOtherVehicle))
                    {
                        FAOtherVehicle faTruck = selFa as FAOtherVehicle;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null && o.ReceiptId != this && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevDate = data.EntryDate;
                            OthRead = data.Reading;
                            PrevOdoRead = data.Reading;
                            PrevLife = data.Life;
                            NoOfLtrs = data.ReceiptId != null ? data.ReceiptId.TotalQty.Value : 0m;
                            PrevPrice = data.ReceiptId != null ? data.ReceiptId.Price.Value : 0m;
                            PrevTotalAmt = data.ReceiptId != null ? data.ReceiptId.Total.Value : 0m;
                        }
                    }
                }
            }
        }
        #endregion
        
        //private void GetLastReading(FixedAsset asset) {
        //    LastReadings rdngsFuel = new LastReadings();
        //    LastReadings rdngsLast = new LastReadings();
        //    rdngsFuel = asset.GetFuelLastReadingBeforeDate(EntryDate);
        //    rdngsLast = asset.GetLastReadingBeforeDate(EntryDate);
        //    if (rdngsFuel != null)
        //    {
        //        if (asset.GetType() != typeof(FAGeneratorSet))
        //        {
        //            OdoRead = rdngsLast != null ? rdngsLast.LastOdoRead : 0m; // ok
        //            PrevOdoRead = rdngsFuel.LastOdoFuelReading;
        //        }
        //        else if (asset.GetType() == typeof(FAGeneratorSet))
        //        {
        //            MtrRead = rdngsLast != null ? rdngsLast.LastOdoRead : 0m; // ok
        //            PrevHrsRead = rdngsFuel.LastOdoFuelReading;
        //        }
        //        PrevDate = rdngsFuel.LastFuelDate; // ok
        //        NoOfLtrs = rdngsFuel.LastFuelLiters; // ok
        //        PrevPrice = rdngsFuel.LastFuelCost; // ok
        //        PrevTotalAmt = rdngsFuel.LastFuelTotalAmt; // ok
        //    } else
        //    {
        //        OdoRead = asset.GetType() != typeof(FAGeneratorSet) ? rdngsLast != null ? rdngsLast.LastOdoRead : 0m : 0m;
        //        MtrRead = asset.GetType() == typeof(FAGeneratorSet) ? rdngsLast != null ? rdngsLast.LastOdoRead : 0m : 0m;
        //        PrevOdoRead = 0;
        //        PrevDate = DateTime.MinValue;
        //        PrevHrsRead = 0;
        //        NoOfLtrs = 0;
        //        PrevPrice = 0;
        //        PrevTotalAmt = 0;
        //    }
        //}

        public FAGeneratorSet GensetNo {
            get { return _GensetNo; }
            set
            {
                SetPropertyValue("GensetNo", ref _GensetNo, value);
                if (!IsLoading)
                {
                    if (_GensetNo != null)
                    {
                        FuelUsageClassification = _GensetNo.FuelUsageClassification;
                        Driver = _GensetNo.Operator ?? null;
                        GetLastGasUp(_GensetNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        MtrRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        public FAOtherVehicle OtherVehicle
        {
            get { return _OtherVehicle; }
            set
            {
                SetPropertyValue("OtherVehicle", ref _OtherVehicle, value);
                if (!IsLoading)
                {
                    if (_OtherVehicle != null)
                    {
                        FuelUsageClassification = _OtherVehicle.FuelUsageClassification;
                        Driver = _OtherVehicle.Operator ?? null;
                        GetLastGasUp(_OtherVehicle);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        OthRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal MtrRead {
            get { return _MtrRead; }
            set { SetPropertyValue("MtrRead", ref _MtrRead, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }

        #endregion

        #region Previous

        private DateTime _PrevDate;
        private decimal _PrevOdoRead;
        private decimal _PrevHrsRead;
        private decimal _NoOfLtrs;
        private decimal _PrevPrice;
        private decimal _PrevTotalAmt;
        [Custom("AllowEdit", "False")]
        public DateTime PrevDate {
            get { return _PrevDate; }
            set { SetPropertyValue("PrevDate", ref _PrevDate, value); }
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevOdoRead {
            get { return _PrevOdoRead; }
            set { SetPropertyValue("PrevOdoRead", ref _PrevOdoRead, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevLife
        {
            get { return _PrevLife; }
            set { SetPropertyValue("PrevLife", ref _PrevLife, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevHrsRead {
            get { return _PrevHrsRead; }
            set { SetPropertyValue("PrevHrsRead", ref _PrevHrsRead, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal NoOfLtrs {
            get { return _NoOfLtrs; }
            set { SetPropertyValue("NoOfLtrs", ref _NoOfLtrs, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal PrevPrice {
            get { return _PrevPrice; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal PrevTotalAmt {
            get { return _PrevTotalAmt; }
            set { SetPropertyValue("PrevTotalAmt", ref _PrevTotalAmt, value); }
        }

        #endregion

        #region Calculated Details

        [Persistent("Total")]
        private decimal? _Total;
        [Persistent("TotalQty")]
        private decimal? _TotalQty;
        [Persistent("Price")]
        private decimal? _Price;
        [PersistentAlias("_Total")]
        [Custom("DisplayFormat", "n")]
        public decimal? Total {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Total == null)
                    {
                        UpdateTotal(
                        false);
                    }
                } catch (Exception)
                {
                }
                return _Total;
            }
        }

        [PersistentAlias("_TotalQty")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalQty {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalQty == null)
                    {
                        UpdateTotalQty(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalQty;
            }
        }

        [PersistentAlias("_Price")]
        [Custom("DisplayFormat", "n")]
        public decimal? Price {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Price == null)
                    {
                        UpdatePrice(
                        false);
                    }
                } catch (Exception)
                {
                }
                return _Price;
            }
        }
        [PersistentAlias("Total / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfGrossPurch
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfGrossPurch");
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

        [PersistentAlias("Total / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfTaxablePurch
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfTaxablePurch");
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

        [PersistentAlias("Total / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfPurchOfSrvcs
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfPurchOfSrvcs");
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

        [PersistentAlias("AmtOfPurchOfSrvcs * (12m/100m)")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfInputTax
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfInputTax");
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
        public void UpdateTotal(bool forceChangeEvent) {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (ReceiptFuelDetail detail in ReceiptFuelDetails)
            {
                tempTotal
                += detail.Total;
            }
            _Total = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Total", Total, _Total);
            }
            ;
        }

        public void UpdateTotalQty(bool forceChangeEvent) {
            decimal? oldTotalQty = _TotalQty;
            decimal tempTotal = 0m;
            foreach (ReceiptFuelDetail detail in ReceiptFuelDetails)
            {
                tempTotal
                += detail.Quantity;
            }
            _TotalQty = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalQty", TotalQty, _TotalQty);
            }
            ;
        }

        [NonPersistent]
        public TariffFuelAllocation CodeNo
        {
            get
            {
                if (ReceiptFuelDetails != null && ReceiptFuelDetails.Count > 0)
                {
                    var firstDetail = ReceiptFuelDetails.OrderBy(d => d.Oid).FirstOrDefault();
                    if (firstDetail != null && firstDetail.PODetailID != null)
                    {
                        return firstDetail.PODetailID.CodeNo ?? null;
                    }
                }
                return null;
            }
        }

        [NonPersistent]
        public TripLocation Origin
        {
            get
            {
                if (ReceiptFuelDetails != null && ReceiptFuelDetails.Count > 0)
                {
                    var firstDetail = ReceiptFuelDetails.OrderBy(d => d.Oid).FirstOrDefault();
                    if (firstDetail != null && firstDetail.PODetailID != null)
                    {
                        return firstDetail.PODetailID.Origin ?? null;
                    }
                }
                return null;
            }
        }

        [NonPersistent]
        public TripLocation Destination
        {
            get
            {
                if (ReceiptFuelDetails != null && ReceiptFuelDetails.Count > 0)
                {
                    var firstDetail = ReceiptFuelDetails.OrderBy(d => d.Oid).FirstOrDefault();
                    if (firstDetail != null && firstDetail.PODetailID != null)
                    {
                        return firstDetail.PODetailID.Destination ?? null;
                    }
                }
                return null;
            }
        }

        [NonPersistent]
        public decimal Tad
        {
            get
            {
                if (ReceiptFuelDetails != null && ReceiptFuelDetails.Count > 0)
                {
                    var firstDetail = ReceiptFuelDetails.OrderBy(d => d.Oid).FirstOrDefault();
                    if (firstDetail != null && firstDetail.PODetailID != null)
                    {
                        return firstDetail.PODetailID.Tad;
                    }
                }
                return 0m;
            }
        }

        public void UpdatePrice(bool forceChangeEvent) {
            decimal? oldPrice = _Price;
            decimal tempTotal = 0m;
            foreach (ReceiptFuelDetail detail in ReceiptFuelDetails)
            {
                tempTotal
                += detail.Cost;
            }
            if (ReceiptFuelDetails.Count != 0)
            {
                _Price = tempTotal / ReceiptFuelDetails.Count;
            }
            else
            {
                _Price = 0m;
            }
            
            if (forceChangeEvent)
            {
                OnChanged("Price", Price, _Price);
            }
            ;
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount
        {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }

        #endregion

        [Aggregated,
        Association("ReceiptFuel-FuelUsageRegistrations")]
        public XPCollection<FuelUsageRegistry> FuelUsageRegistrations {
            get { return GetCollection<FuelUsageRegistry>("FuelUsageRegistrations"
                ); }
        }

        [Aggregated,
        Association("ReceiptFuelUsageDetails")]
        public XPCollection<ReceiptFuelUsageDetail> ReceiptFuelUsageDetails
        {
            get
            {
                return GetCollection<ReceiptFuelUsageDetail>("ReceiptFuelUsageDetails"
                    );
            }
        }

        [NonPersistent]
        public bool IsInFuelSoaDetail
        {
            get
            {
                if (IsLoading || IsSaving) return false;
                try
                {
                    CriteriaOperator criteria = CriteriaOperator.Parse("[Source.Oid] = ?", this.Oid);
                    FuelSoaDetail existingDetail = Session.FindObject<FuelSoaDetail>(criteria);
                    return existingDetail != null;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        #region Fuel Efficiency Monitor

        private bool _FEMLogged = false;
        [Custom("AllowEdit", "False")]
        public bool FEMLogged {
            get { return _FEMLogged; }
            set { SetPropertyValue("FEMLogged", ref _FEMLogged, value); }
        }

        #endregion

        #region Aging

        //private decimal _AmtPaid;
        private int _DaysOt;
        private decimal _ZT30Days;
        private decimal _T3T60Days;
        private decimal _T6T90Days;
        private decimal _GRT90Days;

        [PersistentAlias("Total - Adjusted")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Paid")]
        //[NonPersistent]
        public decimal AmtPaid {
            get
            {
                object tempObject = EvaluateAlias("AmtPaid");
                if (tempObject != null)
                {
                    if (_Adjusted == 0)
                    {
                        return 0;
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

        [PersistentAlias("Total - AmtPaid")]
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
                        if (DaysOt > 90)
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
        private decimal _PrevLife;
        private FAOtherVehicle _OtherNo;
        private decimal _OthRead;
        private PurchaseOrder _PurchaseOrderNo2;
        private string _DtrNo;
        private GenJournalHeader _UseInTrip;
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

        public string FuelTypes
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (ReceiptFuelDetails != null && ReceiptFuelDetails.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in ReceiptFuelDetails)
                    {
                        if (item.ItemNo != null && !strRefs.Contains(item.ItemNo.Code))
                        {
                            strRefs.Add(item.ItemNo.Code);
                            sb.AppendFormat("{0},", item.ItemNo.Code);
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
        [Aggregated,
        Association("ReceiptFuelOdoLogs")]
        public XPCollection<FuelOdoRegistry> ReceiptFuelOdoLogs
        {
            get
            {
                return GetCollection<FuelOdoRegistry>("ReceiptFuelOdoLogs"
                    );
            }
        }
        public ReceiptFuel(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "RFL"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "RFL"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "RFL"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ?
                source.GetNewNo() :
                null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            ShipToAddress = company.FullShipAddress;
            Memo = "Receipt of Fuel for Trucking Operations";
        }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
            base.TriggerObjectChanged(args);
        }
        private FuelRegister fgToDelete = null;
        private string _Remarks;
        private BusinessObjects.TripType _TripType;
        private decimal _OpenAmount;
        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Receipt transactions."
                );
            }
            if (_UseInTrip != null)
            {
                fgToDelete = _UseInTrip.FuelRegistrations.Where(o => o.SourceNo == this.SourceNo).FirstOrDefault();
            }
        }
        protected override void OnSaving()
        {
            //foreach (ReceiptFuelUsageDetail item in ReceiptFuelUsageDetails)
            //{
            //    FuelRegister freg = Session.FindObject<FuelRegister>(BinaryOperator.Parse("[TripNo]=?", item.TripNo));
            //    if (freg != null)
            //    {
            //        freg.Delete();
            //    }
            //    item.Delete();
            //}
            #region Register Fuel in Trip Fuel Register
            if (this.IsSaving && oldUseInTrip != _UseInTrip)
            {
                if (oldUseInTrip != null && _UseInTrip == null)
                {
                    // delete oldUseInTrip
                    FuelRegister freg = Session.FindObject<FuelRegister>(BinaryOperator.Parse("[SourceNo]=?", this.SourceNo));
                    if (freg != null)
                    {
                        freg.Delete();
                    }
                }
                else if (oldUseInTrip == null && _UseInTrip != null)
                {
                    // create _UseInTrip
                    FuelRegister freg = Session.FindObject<FuelRegister>(BinaryOperator.Parse("[TripID]=?", _UseInTrip));
                    if (freg == null)
                    {
                        freg = ReflectionHelper.CreateObject<FuelRegister>(Session);
                    }
                    freg.TripID = _UseInTrip;
                    freg.SourceID = this.Oid;
                    freg.SourceType = this.SourceType;
                    freg.SourceNo = this.SourceNo;
                    freg.TruckOrGenset = _TruckOrGenset;
                    freg.TruckNo = _TruckNo;
                    freg.GensetNo = _GensetNo;
                    freg.Driver = _Driver;
                    freg.Qty = ReceiptFuelDetails.Sum(o => o.Quantity);
                    freg.Total = _Total != null ? _Total.Value : 0;
                    freg.Save();
                }
                else
                {
                    // delete oldUseInTrip
                    FuelRegister freg1 = Session.FindObject<FuelRegister>(BinaryOperator.Parse("[SourceNo]=?", this.SourceNo));
                    if (freg1 != null)
                    {
                        freg1.Delete();
                    }
                    // create _UseInTrip
                    FuelRegister freg = Session.FindObject<FuelRegister>(BinaryOperator.Parse("[TripID]=?", _UseInTrip));
                    if (freg == null)
                    {
                        freg = ReflectionHelper.CreateObject<FuelRegister>(Session);
                    }
                    freg.TripID = _UseInTrip;
                    freg.SourceID = this.Oid;
                    freg.SourceType = this.SourceType;
                    freg.SourceNo = this.SourceNo;
                    freg.TruckOrGenset = _TruckOrGenset;
                    freg.TruckNo = _TruckNo;
                    freg.GensetNo = _GensetNo;
                    freg.Driver = _Driver;
                    freg.Total = _Total.Value;
                    freg.Save();
                }
            }
            #endregion

            #region Delete assiciated Fuel Register
            if (this.IsDeleted && fgToDelete != null)
            {
                // delete oldUseInTrip
                fgToDelete.Delete();
                fgToDelete.TripID.Save();
            }
            #endregion

            #region New Fuel Odo Logging
            FixedAsset thisFA = null;
            //bool logMeter = true;
            switch (_TruckOrGenset)
            {
                case TruckOrGensetEnum.Truck:
                    thisFA = _TruckNo != null ? Session.GetObjectByKey<FixedAsset>(_TruckNo.Oid) : null;
                    break;
                case TruckOrGensetEnum.Genset:
                    thisFA = _GensetNo != null ? Session.GetObjectByKey<FixedAsset>(_GensetNo.Oid) : null;
                    break;
                case TruckOrGensetEnum.NotApplicable:
                    break;
                case TruckOrGensetEnum.Other:
                    thisFA = _OtherVehicle != null ? Session.GetObjectByKey<FixedAsset>(_OtherVehicle.Oid) : null;
                    break;
                default:
                    break;
            }
            if (thisFA != null)
            {
                FuelOdoRegistry newLog = null;
                //thisReceipt
                if (ReceiptFuelOdoLogs.Count > 0)
                {
                    newLog = ReceiptFuelOdoLogs.FirstOrDefault();
                }
                else
                {
                    newLog = ReflectionHelper.CreateObject<FuelOdoRegistry>(Session);
                }
                newLog.LastReading = 0m;
                newLog.Difference = 0m;
                newLog.Fleet = thisFA;
                newLog.ReceiptId = this;
                newLog.EntryDate = EntryDate;
                newLog.LogType = MeterRegistryTypeEnum.Log;
                switch (_TruckOrGenset)
                {
                    case TruckOrGensetEnum.Truck:
                        newLog.Reading = _OdoRead;
                        break;
                    case TruckOrGensetEnum.Genset:
                        newLog.Reading = _MtrRead;
                        break;
                    case TruckOrGensetEnum.NotApplicable:
                        break;
                    case TruckOrGensetEnum.Other:
                        newLog.Reading = _OthRead;
                        break;
                    default:
                        break;
                }
                newLog.Save();
            }
            #endregion
            base.OnSaving();
        }

        [Action(Caption = "Register Odo", AutoCommit= true)]
        public void RegisterOdo()
        {
            #region New Fuel Odo Logging
            FixedAsset thisFA = null;
            //bool logMeter = true;
            switch (_TruckOrGenset)
            {
                case TruckOrGensetEnum.Truck:
                    thisFA = _TruckNo != null ? Session.GetObjectByKey<FixedAsset>(_TruckNo.Oid) : null;
                    break;
                case TruckOrGensetEnum.Genset:
                    thisFA = _GensetNo != null ? Session.GetObjectByKey<FixedAsset>(_GensetNo.Oid) : null;
                    break;
                case TruckOrGensetEnum.NotApplicable:
                    break;
                case TruckOrGensetEnum.Other:
                    thisFA = _OtherVehicle != null ? Session.GetObjectByKey<FixedAsset>(_OtherVehicle.Oid) : null;
                    break;
                default:
                    break;
            }
            if (thisFA != null)
            {
                FuelOdoRegistry newLog = null;
                //thisReceipt
                if (ReceiptFuelOdoLogs.Count > 0)
                {
                    newLog = ReceiptFuelOdoLogs.FirstOrDefault();
                }
                else
                {
                    newLog = ReflectionHelper.CreateObject<FuelOdoRegistry>(Session);
                }
                newLog.Fleet = thisFA;
                newLog.ReceiptId = this;
                newLog.EntryDate = EntryDate;
                newLog.LogType = MeterRegistryTypeEnum.Log;
                switch (_TruckOrGenset)
                {
                    case TruckOrGensetEnum.Truck:
                        newLog.Reading = _OdoRead;
                        break;
                    case TruckOrGensetEnum.Genset:
                        newLog.Reading = _MtrRead;
                        break;
                    case TruckOrGensetEnum.NotApplicable:
                        break;
                    case TruckOrGensetEnum.Other:
                        newLog.Reading = _OthRead;
                        break;
                    default:
                        break;
                }
                newLog.Save();
            }
            #endregion
            //if (_Status != ReceiptFuelStatusEnum.Current)
            //{
            //    #region New Fuel Odo Logging
            //    FixedAsset thisFA = null;
            //    //bool logMeter = true;
            //    switch (_TruckOrGenset)
            //    {
            //        case TruckOrGensetEnum.Truck:
            //            thisFA = _TruckNo != null ? Session.GetObjectByKey<FixedAsset>(_TruckNo.Oid) : null;
            //            break;
            //        case TruckOrGensetEnum.Genset:
            //            thisFA = _GensetNo != null ? Session.GetObjectByKey<FixedAsset>(_GensetNo.Oid) : null;
            //            break;
            //        case TruckOrGensetEnum.NotApplicable:
            //            break;
            //        default:
            //            break;
            //    }
            //    if (thisFA != null)
            //    {
            //        FuelOdoRegistry newLog = null;
            //        //thisReceipt
            //        if (ReceiptFuelOdoLogs.Count > 0)
            //        {
            //            newLog = ReceiptFuelOdoLogs.FirstOrDefault();
            //        }
            //        else
            //        {
            //            newLog = ReflectionHelper.CreateObject<FuelOdoRegistry>(Session);
            //        }
            //        newLog.Fleet = thisFA;
            //        newLog.ReceiptId = this;
            //        newLog.EntryDate = EntryDate;
            //        newLog.LogType = MeterRegistryTypeEnum.Log;
            //        switch (_TruckOrGenset)
            //        {
            //            case TruckOrGensetEnum.Truck:
            //                newLog.Reading = _OdoRead;
            //                break;
            //            case TruckOrGensetEnum.Genset:
            //                newLog.Reading = _MtrRead;
            //                break;
            //            case TruckOrGensetEnum.NotApplicable:
            //                break;
            //            default:
            //                break;
            //        }
            //        newLog.Save();
            //    }
            //    #endregion
            //}
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
            _Total = null;
            _TotalQty = null;
            _Price = null;
        }
    }
}
