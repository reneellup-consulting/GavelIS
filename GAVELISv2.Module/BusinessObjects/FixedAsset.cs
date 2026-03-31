using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects {
    public enum FleetStatusEnum
    {
        [DisplayName("Commissioned")]
        InCommission,
        Decommissioned,
        [DisplayName("Under Repair")]
        UnderRepair,
        Impounded,
        Disassembled,
        Sold,
        Missing
    }

    public enum FleetTireStatusEnum
    {
        Complete,
        Incomplete,
        Undue,
        Unnecessary
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [OptimisticLocking(false)]
    public class FixedAsset : Item {
        private Employee _DriverOperator;
        private decimal _LastGasUpReading;
        private MeterLogTypeEnum _MeterLogType;
        private FixedAssetClassEnum _FixedAssetClass;
        private Account _AsssetAccount;
        private Account _DepreciationAccount;
        private AcquisitionState _AcquisitionState;
        private DateTime _PurchaseDate;
        private decimal _FACost;
        private string _FAVendor;
        private DateTime _SalesDate;
        private decimal _FASalesPrice;
        private decimal _FASalesExpense;
        private string _AssetDescription;
        private string _Location;
        private string _PONumber;
        private string _SerialNo;
        private string _WarrantyExpires;
        private string _Notes;
        private decimal _CostBasis;
        private decimal _AccumDepreciation;
        private decimal _BookValue;
        private string _EngineNo;
        private string _BrandModel;
        private string _PlateNo;
        private CostCenter _CostCenter;
        private decimal _InitialLife;
        private decimal _LastOdoReading;
        private decimal _LastLife;
        private decimal _LastOdoFuelReading;
        private decimal _LastFuelLifeReading;
        private decimal _LastOdoServiceReading;
        private decimal _LastServiceLifeReading;
        private DateTime _LastFuelDate;
        private decimal _LastFuelLiters;
        private decimal _LastFuelTotalAmt;
        private DateTime _LastServiceDate;
        private decimal _LastServiceCost;
        private decimal _FuelEfficiency;
        private FleetStatusEnum _FleetStatus;
        private FleetTireStatusEnum _FleetTireStatus;
        private TireSize _TireSize;
        private TireSize _LastTireSize;
        private long _SeqNo = 1000000000000000;
        private FuelUsageClassEnum _FuelUsageClassification;
        [NonPersistent]
        public string SortingSequence
        {
            get {
                string _seq = No;
                //var sortedQuery = from FATruck a in e.PopupWindow.View.SelectedObjects orderby int.Parse(a.No.Substring(3).PadLeft(4,'0')) select a;
                try
                {
                    switch (_FixedAssetClass)
                    {
                        case FixedAssetClassEnum.LandAndBuilding:
                            break;
                        case FixedAssetClassEnum.Truck:
                            _seq = string.Format("48-{0}", No.Substring(3).PadLeft(4, '0'));
                            break;
                        case FixedAssetClassEnum.Trailer:
                            _seq = string.Format("TRLR {0}", No.Substring(3).PadLeft(6, '0'));
                            break;
                        case FixedAssetClassEnum.GeneratorSet:
                            _seq = string.Format("18-{0}", No.Substring(3).PadLeft(4, '0'));
                            break;
                        case FixedAssetClassEnum.OtherVehicle:
                            break;
                        case FixedAssetClassEnum.Other:
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                }
                return _seq; }
        }
        
        [RuleRequiredField("", DefaultContexts.Save)]
        public FixedAssetClassEnum FixedAssetClass {
            get { return _FixedAssetClass; }
            set { SetPropertyValue("FixedAssetClass", ref _FixedAssetClass,
                value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account AsssetAccount {
            get { return _AsssetAccount; }
            set { SetPropertyValue("AsssetAccount", ref _AsssetAccount, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account DepreciationAccount {
            get { return _DepreciationAccount; }
            set { SetPropertyValue("DepreciationAccount", ref
                _DepreciationAccount, value); }
        }

        public AcquisitionState AcquisitionState {
            get { return _AcquisitionState; }
            set { SetPropertyValue("AcquisitionState", ref _AcquisitionState,
                value); }
        }

        public DateTime PurchaseDate {
            get { return _PurchaseDate; }
            set { SetPropertyValue("PurchaseDate", ref _PurchaseDate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal FACost {
            get { return _FACost; }
            set { SetPropertyValue("FACost", ref _FACost, value); }
        }

        public string FAVendor {
            get { return _FAVendor; }
            set { SetPropertyValue("FAVendor", ref _FAVendor, value); }
        }

        public DateTime SalesDate {
            get { return _SalesDate; }
            set { SetPropertyValue("SalesDate", ref _SalesDate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal FASalesPrice {
            get { return _FASalesPrice; }
            set { SetPropertyValue("FASalesPrice", ref _FASalesPrice, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal FASalesExpense {
            get { return _FASalesExpense; }
            set { SetPropertyValue("FASalesExpense", ref _FASalesExpense, value)
                ; }
        }

        [Size(500)]
        public string AssetDescription {
            get { return _AssetDescription; }
            set { SetPropertyValue("AssetDescription", ref _AssetDescription,
                value); }
        }

        public string EngineNo {
            get { return _EngineNo; }
            set { SetPropertyValue("EngineNo", ref _EngineNo, value); }
        }

        public string BrandModel {
            get { return _BrandModel; }
            set { SetPropertyValue("BrandModel", ref _BrandModel, value); }
        }

        public string PlateNo {
            get { return _PlateNo; }
            set { SetPropertyValue("PlateNo", ref _PlateNo, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue("CostCenter", ref _CostCenter, value); }
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal InitialLife {
            get { return _InitialLife; }
            set { SetPropertyValue("InitialLife", ref _InitialLife, value); }
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastOdoReading {
            get { return _LastOdoReading; }
            set { SetPropertyValue("LastOdoReading", ref _LastOdoReading, value)
                ; }
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastLife {
            get { return _LastLife; }
            set { SetPropertyValue("LastLife", ref _LastLife, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastOdoFuelReading {
            get { return _LastOdoFuelReading; }
            set { SetPropertyValue("LastOdoFuelReading", ref _LastOdoFuelReading
                , value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastFuelLifeReading {
            get { return _LastFuelLifeReading; }
            set { SetPropertyValue("LastFuelLifeReading", ref
                _LastFuelLifeReading, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastOdoServiceReading {
            get { return _LastOdoServiceReading; }
            set { SetPropertyValue("LastOdoServiceReading", ref
                _LastOdoServiceReading, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastServiceLifeReading {
            get { return _LastServiceLifeReading; }
            set { SetPropertyValue("LastServiceLifeReading", ref
                _LastServiceLifeReading, value); }
        }

        public DateTime LastFuelDate {
            get { return _LastFuelDate; }
            set { SetPropertyValue("LastFuelDate", ref _LastFuelDate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal LastFuelLiters {
            get { return _LastFuelLiters; }
            set { SetPropertyValue("LastFuelLiters", ref _LastFuelLiters, value)
                ; }
        }

        [PersistentAlias("LastFuelTotalAmt/LastFuelLiters")]
        [Custom("DisplayFormat", "n")]
        public decimal LastFuelPrice {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("LastFuelPrice");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    } else
                    {
                        return 0;
                    }
                } catch (Exception)
                {
                    return 0;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal LastFuelTotalAmt {
            get { return _LastFuelTotalAmt; }
            set { SetPropertyValue("LastFuelTotalAmt", ref _LastFuelTotalAmt,
                value); }
        }

        public DateTime LastServiceDate {
            get { return _LastServiceDate; }
            set { SetPropertyValue("LastServiceDate", ref _LastServiceDate,
                value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal LastServiceCost {
            get { return _LastServiceCost; }
            set { SetPropertyValue("LastServiceCost", ref _LastServiceCost,
                value); }
        }

        public Employee DriverOperator {
            get
            {
                switch (_FixedAssetClass)
                {
                    case FixedAssetClassEnum.LandAndBuilding:
                        _DriverOperator = null;
                        break;
                    case FixedAssetClassEnum.Truck:
                        if (this.GetType() != typeof(FATruck))
                        {
                            break;
                        }
                        _DriverOperator = ((FATruck)this).Operator ?? null;
                        break;
                    case FixedAssetClassEnum.Trailer:
                        if (this.GetType() != typeof(FATrailer))
                        {
                            break;
                        }
                        _DriverOperator = ((FATrailer)this).Operator ?? null;
                        break;
                    case FixedAssetClassEnum.GeneratorSet:
                        if (this.GetType() != typeof(FAGeneratorSet))
                        {
                            break;
                        }
                        _DriverOperator = ((FAGeneratorSet)this).Operator ?? null;
                        break;
                    case FixedAssetClassEnum.OtherVehicle:
                        if (this.GetType() != typeof(FAOtherVehicle))
                        {
                            break;
                        }
                        _DriverOperator = ((FAOtherVehicle)this).Operator ?? null;
                        break;
                    case FixedAssetClassEnum.Other:
                        _DriverOperator = null;
                        break;
                    default:
                        break;
                }
                return _DriverOperator;
            }
        }
        //[Custom("AllowEdit", "False")]
        public bool FuelOdoDescrepancy
        {
            get
            {
                FuelOdoRegistry lastOrDefault = FleetFuelOdoLogs.Where(o => o.Negative == true || o.UnderRead == true || o.InvalidDiff == true && o.Oid > 0).LastOrDefault();
                if (lastOrDefault != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        public bool ServiceOdoDescrepancy
        {
            get
            {
                ServiceOdoRegistry lastOrDefault = FleetServiceOdoLogs.Where(o => o.Negative == true || o.UnderRead == true || o.InvalidDiff == true && o.Oid > 0).LastOrDefault();
                if (lastOrDefault != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool TripOdoDescrepancy
        {
            get
            {
                TripOdoRegistry lastOrDefault = FleetTripeOdoLogs.Where(o => o.Negative == true || o.UnderRead == true || o.InvalidDiff == true && o.Oid > 0).LastOrDefault();
                if (lastOrDefault != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #region Odometer Registration Correction

        [NonPersistent]
        public MeterLogTypeEnum MeterLogType {
            get
            {
                try
                {
                    var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).LastOrDefault();
                    if (data != null)
                    {
                        return data.LogType;
                    }
                    else
                    {
                        return MeterLogTypeEnum.None;
                    }
                } catch (Exception)
                {
                    return MeterLogTypeEnum.None;
                }
            }
        }

        //[NonPersistent]
        //[EditorAlias("MeterTypePropertyEditor")]
        //public decimal PreviousReading { get; set; }

        public long GetFirstSequenceNo() {
            try
            {
                var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).FirstOrDefault();
                return data != null ? data.SeqNo : 0;
            } catch (Exception)
            {
                return 0L;
            }
        }


        public long GetFirstSequenceNoForGenset() {
            try
            {
                var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).LastOrDefault();
                if (data != null)
                {
                    return data.SeqNo;
                }
                else
                {
                    return 0L;
                }
            } catch (Exception)
            {
                return 0L;
            }
        }

        public decimal GetPreviousReading(long seqNo) {
            try
            {
                var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).Where(o => o.SeqNo < seqNo).LastOrDefault();
                if (data != null)
                {
                    return data.Reading;
                }
                else
                {
                    return 0m;
                }
            } catch (Exception)
            {
                return 0;
            }
        }

        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastReading {
            get
            {
                try
                {
                    var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).LastOrDefault();
                    if (data != null)
                    {
                        return data.Reading;
                    }
                    else
                    {
                        return 0m;
                    }
                } catch (Exception)
                {
                    return 0;
                }
            }
        }

        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal InitialLife01 {
            get
            {
                try
                {
                    var data = VehicleOdoRegisters.Where(o => o.LogType ==
                    MeterLogTypeEnum.Initial).FirstOrDefault();
                    return data != null ? data.Life : 0m;
                } catch (Exception)
                {
                    return 0;
                }
            }
        }

        //[NonPersistent]
        //[EditorAlias("MeterTypePropertyEditor")]
        //public decimal Mileage01 {
        //    get
        //    {
        //        try
        //        {
        //            var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).LastOrDefault();
        //            if (data != null)
        //            {
        //                return data.Life;
        //            }
        //            else
        //            {
        //                return 0m;
        //            }
        //        } catch (Exception)
        //        {
        //            return 0;
        //        }
        //    }
        //}
        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Mileage02
        {
            get
            {
                try
                {
                    switch (FixedAssetClass)
                    {
                        case FixedAssetClassEnum.LandAndBuilding:
                            return 0m;
                        case FixedAssetClassEnum.Truck:
                            var data = (this as FATruck).FleetFuelOdoLogs.OrderBy(o => o.Sequence).LastOrDefault();
                            if (data != null)
                            {
                                return data.Life;
                            }
                            else
                            {
                                return 0m;
                            }
                        case FixedAssetClassEnum.Trailer:
                            var data2 = (this as FATrailer).TraileRegistryTripLogs.OrderBy(o => o.Sequence).LastOrDefault();
                            if (data2 != null)
                            {
                                return data2.TrlrMileage;
                            }
                            else
                            {
                                return 0m;
                            }
                        case FixedAssetClassEnum.GeneratorSet:
                            var data3 = (this as FAGeneratorSet).GensetHoursLogs.OrderBy(o => o.Sequence).LastOrDefault();
                            if (data3 != null)
                            {
                                return data3.LifeHours;
                            }
                            else
                            {
                                return 0m;
                            }
                        case FixedAssetClassEnum.OtherVehicle:
                            var data4 = (this as FAOtherVehicle).FleetFuelOdoLogs.OrderBy(o => o.Sequence).LastOrDefault();
                            if (data4 != null)
                            {
                                return data4.Life;
                            }
                            else
                            {
                                return 0m;
                            }
                        case FixedAssetClassEnum.Other:
                            return 0m;
                        default:
                            return 0m;
                    }
                    //var data = VehicleOdoRegisters.OrderBy(o => o.SeqNo).LastOrDefault();
                    //if (data != null)
                    //{
                    //    return data.Life;
                    //}
                    //else
                    //{
                    //    return 0m;
                    //}
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }
        [NonPersistent]
        public DateTime LastGasUp {
            get
            {
                try
                {
                    var data = VehicleOdoRegisters.Where(o => o.LogType ==
                    MeterLogTypeEnum.Fuel).OrderBy(o => o.SeqNo).LastOrDefault();
                    if (data != null)
                    {
                        return data.EntryDate;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                } catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
        }

        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastGasUpLife {
            get
            {
                try
                {
                    var data = VehicleOdoRegisters.Where(o => o.LogType ==
                    MeterLogTypeEnum.Fuel).OrderBy(o => o.SeqNo).LastOrDefault();
                    if (data != null)
                    {
                        return data.Life;
                    }
                    else
                    {
                        return 0;
                    }
                } catch (Exception)
                {
                    return 0;
                }
            }
        }

        #endregion

        [Custom("DisplayFormat", "n")]
        public decimal FuelEfficiency {
            get
            {
                //XPQuery<OdometerRegister> oregs = Session.DefaultSession.Query<
                //OdometerRegister>();
                try
                {
                    decimal lpkm = 0;
                    var data = VehicleOdoRegisters.Where(o => o.LogType ==
                    MeterLogTypeEnum.Fuel);
                    if (data.Count() <= 0)
                    {
                        return 0m;
                    }
                    foreach (var item in data)
                    {
                        lpkm = lpkm + item.LitersPerKm;
                    }
                    _FuelEfficiency = lpkm / data.Count();
                } catch (Exception)
                {
                    return _FuelEfficiency;
                }
                return _FuelEfficiency;
            }
        }

        public FleetStatusEnum FleetStatus {
            get { return _FleetStatus; }
            set { SetPropertyValue<FleetStatusEnum>("FleetStatus", ref _FleetStatus, value); }
        }

        public TireSize TireSize {
            get { return _TireSize; }
            set { SetPropertyValue<TireSize>("TireSize", ref _TireSize, value); }
        }
        [Custom("AllowEdit", "False")]
        public TireSize LastTireSize {
            get
            {
                try
                {
                    var data = this.FleetTireServiceDetails.OrderBy(o => o.Seq).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.TireNo.TireItem.Size ?? null;
                    } else
                    {
                        return null;
                    }
                } catch (Exception)
                {
                    return null;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "f0")]
        [Custom("EditMask", "f0")]
        public long SeqNo {
            get { return _SeqNo; }
            set { SetPropertyValue<long>("SeqNo", ref _SeqNo, value); }
        }

        public FuelUsageClassEnum FuelUsageClassification {
            get { return _FuelUsageClassification; }
            set { SetPropertyValue<FuelUsageClassEnum>("FuelUsageClassification", ref _FuelUsageClassification, value); }
        }

        public string Location {
            get { return _Location; }
            set { SetPropertyValue("Location", ref _Location, value); }
        }

        public string PONumber {
            get { return _PONumber; }
            set { SetPropertyValue("PONumber", ref _PONumber, value); }
        }

        public string SerialNo {
            get { return _SerialNo; }
            set { SetPropertyValue("SerialNo", ref _SerialNo, value); }
        }

        public string WarrantyExpires {
            get { return _WarrantyExpires; }
            set { SetPropertyValue("WarrantyExpires", ref _WarrantyExpires,
                value); }
        }

        [Size(1000)]
        public string Notes {
            get { return _Notes; }
            set { SetPropertyValue("Notes", ref _Notes, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CostBasis {
            get { return _CostBasis; }
            set { SetPropertyValue("CostBasis", ref _CostBasis, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal AccumDepreciation {
            get { return _AccumDepreciation; }
            set { SetPropertyValue("AccumDepreciation", ref _AccumDepreciation,
                value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal BookValue {
            get { return _BookValue; }
            set { SetPropertyValue("BookValue", ref _BookValue, value); }
        }

        // Vehicle Odometer Register
        // Fuel Efficiency Register
        // Service & Maintenance Register
        [Aggregated,
        Association("Fleet-OdometerRegister")]
        [NonCloneable]
        public XPCollection<OdometerRegister> VehicleOdoRegisters {
            get { return
                GetCollection<OdometerRegister>("VehicleOdoRegisters"); }
        }

        [Aggregated,
        Association("Fleet-FuelMileageRegister")]
        [NonCloneable]
        public XPCollection<FuelMileageRegister> FuelMileageRegisters {
            get { return GetCollection<FuelMileageRegister>("FuelMileageRegisters"
                ); }
        }
        [Aggregated,
        Association("FleetFuelOdoLogs")]
        [NonCloneable]
        public XPCollection<FuelOdoRegistry> FleetFuelOdoLogs
        {
            get
            {
                return GetCollection<FuelOdoRegistry>("FleetFuelOdoLogs"
                    );
            }
        }
        [Aggregated,
        Association("FleetServiceOdoLogs")]
        [NonCloneable]
        public XPCollection<ServiceOdoRegistry> FleetServiceOdoLogs
        {
            get
            {
                return GetCollection<ServiceOdoRegistry>("FleetServiceOdoLogs"
                    );
            }
        }
        [Aggregated,
        Association("FleetTripOdoLogs")]
        [NonCloneable]
        public XPCollection<TripOdoRegistry> FleetTripeOdoLogs
        {
            get
            {
                return GetCollection<TripOdoRegistry>("FleetTripeOdoLogs"
                    );
            }
        }
        [Aggregated,
        Association("Fleet-PreventiveMaintenance")]
        [NonCloneable]
        public XPCollection<PreventiveMaintenance> PreventiveMaintenanceScheds {
            get { return GetCollection<PreventiveMaintenance>(
                "PreventiveMaintenanceScheds"); }
        }


        [Aggregated,
        Association("FleetTireServiceDetails")]
        [NonCloneable]
        public XPCollection<TireServiceDetail2> FleetTireServiceDetails {
            get { return GetCollection<TireServiceDetail2>(
                "FleetTireServiceDetails"); }
        }

        [Aggregated,
        Association("UnitBatteryServiceDetails")]
        [NonCloneable]
        public XPCollection<BatteryServiceDetail> UnitBatteryServiceDetails
        {
            get
            {
                return GetCollection<BatteryServiceDetail>(
                    "UnitBatteryServiceDetails");
            }
        }
        [Aggregated,
        Association("RevolvingPartsServiceDetails")]
        [NonCloneable]
        public XPCollection<RevolvingPartDetail> RevolvingPartsServiceDetails
        {
            get
            {
                return GetCollection<RevolvingPartDetail>(
                    "RevolvingPartsServiceDetails");
            }
        }

        [Aggregated, Association("FleetWoItems")]
        public XPCollection<WorkOrderItemDetail> FleetWoItems
        {
            get
            {
                return GetCollection<WorkOrderItemDetail>(
                    "FleetWoItems");
            }
        }

        // Genset Hourmeter Register
        // Genset Fuel Efficiency Register
        // Genset Service & Maintenance Register

        private int _NoOfTires;
        private int _ActualTiresAttached;
        private int _DeviceId;
        private string _UniqueId;
        private DateTime _LastPosUpdate;
        private int _LastPositionId;
        private string _LastAddress;
        private string _LastLongitude;
        private string _LastLatitude;
        private double _TrackMileage;
        private bool _Monitor;
        private bool _FmsUpdate;

        public int NoOfTires {
            get { return _NoOfTires; }
            set { SetPropertyValue<int>("NoOfTires", ref _NoOfTires, value); }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public int ActualTiresAttached {
            get
            {
                try
                {
                    var list = from trd in this.FleetTireServiceDetails
                               select trd;
                    int aCount = list.Where(o => o.ActivityType == TireActivityTypeEnum.Attached).Count();
                    int dCount = list.Where(o => new[] { TireActivityTypeEnum.Dettached, TireActivityTypeEnum.Disposed
                    }.Contains(o.ActivityType)).Where(o => o.WheelPos != null).Count();
                    int tmp = aCount - dCount;
                    _ActualTiresAttached = tmp > 0 ? tmp : 0;
                } catch (Exception)
                {
                    _ActualTiresAttached = 0;
                }
                return _ActualTiresAttached;
            }
        }
        //[RuleFromBoolProperty("",DefaultContexts.Save,CustomMessageTemplate = "Detected meter descrepancy. Please check the Fleet Meter Monitor")]
        //public bool HasMeterDescrepancy { get { 
        //    return false; } }
        public FleetTireStatusEnum FleetTireStatus {
            get
            {
                if (new[] { FixedAssetClassEnum.Truck, FixedAssetClassEnum.Trailer
                }.Contains(FixedAssetClass) && new[] { FleetStatusEnum.InCommission, FleetStatusEnum.Missing
                }.Contains(_FleetStatus))
                {
                    if (_NoOfTires > _ActualTiresAttached)
                    {
                        return FleetTireStatusEnum.Incomplete;
                    } else if (_NoOfTires < _ActualTiresAttached)
                    {
                        return FleetTireStatusEnum.Undue;
                    } else if (_NoOfTires == _ActualTiresAttached)
                    {
                        return FleetTireStatusEnum.Complete;
                    }
                }
                return FleetTireStatusEnum.Unnecessary;
            }
            //set { SetPropertyValue<FleetTireStatusEnum>("FleetTireStatus", ref _FleetTireStatus, value); }
        }

        public FixedAsset(Session session)
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
            ItemType = ItemTypeEnum.FixedAsset;
        }

        public bool ValidOdoRegDate(DateTime regDate) {
            try
            {
                OdometerRegister odo = (from r in VehicleOdoRegisters
                                        where (r.LogType == MeterLogTypeEnum.Initial)
                                        orderby r.EntryDate
                                        select r).FirstOrDefault();
                if (odo != null)
                {
                    if (regDate >= odo.EntryDate)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                } else
                {
                    return true;
                }
            } catch (Exception)
            {
                return true;
            }
        }

        //[NonPersistent]
        //public bool ValidOdoRegDate {
        //    get {
        //        try
        //        {
        //            OdometerRegister odo = (from r in VehicleOdoRegisters
        //                                    where (r.LogType == MeterLogTypeEnum.Initial)
        //                                    orderby r.EntryDate
        //                                    select r).First();
        //            if (odo!=null)
        //            {

        //            }
        //        }
        //        catch (Exception)
        //        {
        //            return false;
        //        }

        //        return false; }
        //}

        public LastReadings GetFuelLastReadingBeforeDate(DateTime beforeDate) {
            LastReadings lstrd = new LastReadings();
            try
            {
                OdometerRegister odo = (from r in VehicleOdoRegisters
                                        where (r.EntryDate < beforeDate && r.LogType == MeterLogTypeEnum.Fuel)
                                        orderby r.EntryDate
                                        select r).LastOrDefault();

                if (odo != null)
                {
                    lstrd.LastOdoRead = odo.Reading;
                    lstrd.LastLife = odo.Life;
                    lstrd.LastFuelDate = odo.EntryDate;
                    lstrd.LastFuelLife = odo.Life;
                    lstrd.LastOdoFuelReading = odo.Reading;
                    lstrd.LastFuelLiters = odo.Liters;
                    lstrd.LastFuelCost = odo.Cost;
                    lstrd.LastFuelTotalAmt = odo.Cost > 0 ? odo.Cost / odo.Liters : 0;
                    if (odo.LogType == MeterLogTypeEnum.Service)
                    {
                        lstrd.LastServiceID = odo.PrevMaintenanceID ?? null;
                        lstrd.LastServiceDate = odo.EntryDate;
                        lstrd.LastServiceRead = odo.Reading;
                        lstrd.LastServiceLife = odo.Life;
                        lstrd.LastServiceIdLife = odo.Life;
                    }
                    return lstrd;
                } else
                {
                    //lstrd.LastOdoRead = 0m;
                    //lstrd.LastLife = 0m;
                    //lstrd.LastFuelDate = DateTime.MinValue;
                    //lstrd.LastFuelLife = 0m;
                    //lstrd.LastOdoFuelReading = 0m;
                    //lstrd.LastFuelLiters = 0m;
                    //lstrd.LastFuelCost = 0m;
                    //lstrd.LastFuelTotalAmt = 0m;
                    return null;
                }
            } catch (Exception)
            {
                return null;
            }
        }

        public LastReadings GetServiceIdLastReadingBeforeDate(DateTime beforeDate, PreventiveMaintenance maint) {
            LastReadings lstrd = new LastReadings();
            try
            {
                OdometerRegister odo = (from r in VehicleOdoRegisters
                                        where (r.EntryDate < beforeDate && r.LogType == MeterLogTypeEnum.Service && r.PrevMaintenanceID == maint)
                                        orderby r.EntryDate
                                        select r).LastOrDefault();

                if (odo != null)
                {
                    lstrd.LastOdoRead = odo.Reading;
                    lstrd.LastLife = odo.Life;
                    lstrd.LastFuelDate = odo.EntryDate;
                    lstrd.LastFuelLife = odo.Life;
                    lstrd.LastOdoFuelReading = odo.Reading;
                    lstrd.LastFuelLiters = odo.Liters;
                    lstrd.LastFuelCost = odo.Cost;
                    lstrd.LastFuelTotalAmt = odo.Cost > 0 ? odo.Cost / odo.Liters : 0;
                    if (odo.LogType == MeterLogTypeEnum.Service)
                    {
                        lstrd.LastServiceID = odo.PrevMaintenanceID ?? null;
                        lstrd.LastServiceDate = odo.EntryDate;
                        lstrd.LastServiceRead = odo.Reading;
                        lstrd.LastServiceLife = odo.Life;
                        lstrd.LastServiceIdLife = odo.Life;
                    }
                    return lstrd;
                } else
                {
                    //lstrd.LastOdoRead = 0m;
                    //lstrd.LastLife = 0m;
                    //lstrd.LastFuelDate = DateTime.MinValue;
                    //lstrd.LastFuelLife = 0m;
                    //lstrd.LastOdoFuelReading = 0m;
                    //lstrd.LastFuelLiters = 0m;
                    //lstrd.LastFuelCost = 0m;
                    //lstrd.LastFuelTotalAmt = 0m;
                    return null;
                }
            } catch (Exception)
            {
                return null;
            }
        }

        public LastReadings GetServiceLastReadingBeforeDate(DateTime beforeDate) {
            LastReadings lstrd = new LastReadings();
            try
            {
                OdometerRegister odo = (from r in VehicleOdoRegisters
                                        where (r.EntryDate < beforeDate && r.LogType == MeterLogTypeEnum.Service)
                                        orderby r.EntryDate
                                        select r).LastOrDefault();

                if (odo != null)
                {
                    lstrd.LastOdoRead = odo.Reading;
                    lstrd.LastLife = odo.Life;
                    lstrd.LastFuelDate = odo.EntryDate;
                    lstrd.LastFuelLife = odo.Life;
                    lstrd.LastOdoFuelReading = odo.Reading;
                    lstrd.LastFuelLiters = odo.Liters;
                    lstrd.LastFuelCost = odo.Cost;
                    lstrd.LastFuelTotalAmt = odo.Cost > 0 ? odo.Cost / odo.Liters : 0;
                    if (odo.LogType == MeterLogTypeEnum.Service)
                    {
                        lstrd.LastServiceID = odo.PrevMaintenanceID ?? null;
                        lstrd.LastServiceDate = odo.EntryDate;
                        lstrd.LastServiceRead = odo.Reading;
                        lstrd.LastServiceLife = odo.Life;
                        lstrd.LastServiceIdLife = odo.Life;
                    }
                    return lstrd;
                } else
                {
                    //lstrd.LastOdoRead = 0m;
                    //lstrd.LastLife = 0m;
                    //lstrd.LastFuelDate = DateTime.MinValue;
                    //lstrd.LastFuelLife = 0m;
                    //lstrd.LastOdoFuelReading = 0m;
                    //lstrd.LastFuelLiters = 0m;
                    //lstrd.LastFuelCost = 0m;
                    //lstrd.LastFuelTotalAmt = 0m;
                    return null;
                }
            } catch (Exception)
            {
                return null;
            }
        }

        public LastReadings GetLastReadingBeforeDate(DateTime beforeDate) {
            LastReadings lstrd = new LastReadings();
            try
            {
                OdometerRegister odo = (from r in VehicleOdoRegisters
                                        where (r.EntryDate < beforeDate)
                                        orderby r.EntryDate
                                        select r).LastOrDefault();

                if (odo != null)
                {
                    lstrd.LastOdoRead = odo.Reading;
                    lstrd.LastLife = odo.Life;
                    lstrd.LastFuelDate = odo.EntryDate;
                    lstrd.LastFuelLife = odo.Life;
                    lstrd.LastOdoFuelReading = odo.Reading;
                    lstrd.LastFuelLiters = odo.Liters;
                    lstrd.LastFuelCost = odo.Cost;
                    lstrd.LastFuelTotalAmt = odo.Cost > 0 ? odo.Cost / odo.Liters : 0;
                    if (odo.LogType == MeterLogTypeEnum.Service)
                    {
                        lstrd.LastServiceID = odo.PrevMaintenanceID ?? null;
                        lstrd.LastServiceDate = odo.EntryDate;
                        lstrd.LastServiceRead = odo.Reading;
                        lstrd.LastServiceLife = odo.Life;
                        lstrd.LastServiceIdLife = odo.Life;
                    }
                    return lstrd;
                } else
                {
                    return null;
                }
            } catch (Exception)
            {
                return null;
            }
        }

        public LastReadings GetLastReading() {
            LastReadings lstrd = new LastReadings();
            try
            {
                OdometerRegister odo = (from r in VehicleOdoRegisters
                                        orderby r.EntryDate
                                        select r).LastOrDefault();

                if (odo != null)
                {
                    lstrd.LastOdoRead = odo.Reading;
                    lstrd.LastLife = odo.Life;
                    lstrd.LastFuelDate = odo.EntryDate;
                    lstrd.LastFuelLife = odo.Life;
                    lstrd.LastOdoFuelReading = odo.Reading;
                    lstrd.LastFuelLiters = odo.Liters;
                    lstrd.LastFuelCost = odo.Cost;
                    lstrd.LastFuelTotalAmt = odo.Cost > 0 ? odo.Cost / odo.Liters : 0;
                    if (odo.LogType == MeterLogTypeEnum.Service)
                    {
                        lstrd.LastServiceID = odo.PrevMaintenanceID ?? null;
                        lstrd.LastServiceDate = odo.EntryDate;
                        lstrd.LastServiceRead = odo.Reading;
                        lstrd.LastServiceLife = odo.Life;
                        lstrd.LastServiceIdLife = odo.Life;
                    }
                    return lstrd;
                } else
                {
                    return null;
                }
            } catch (Exception)
            {
                return null;
            }
        }

        #region Hyper FMS

        public bool Monitor
        {
            get { return _Monitor; }
            set { SetPropertyValue("Monitor", ref _Monitor, value); }
        }

        // DeviceId
        [Custom("AllowEdit", "False")]
        public int DeviceId
        {
            get { return _DeviceId; }
            set { SetPropertyValue("DeviceId", ref _DeviceId, value); }
        }
        // UniqueId
        [Custom("AllowEdit", "False")]
        public string UniqueId
        {
            get { return _UniqueId; }
            set { SetPropertyValue("UniqueId", ref _UniqueId, value); }
        }
        // LastPosUpdate
        [Custom("AllowEdit", "False")]
        public DateTime LastPosUpdate
        {
            get { return _LastPosUpdate; }
            set { SetPropertyValue("LastPosUpdate", ref _LastPosUpdate, value); }
        }
        // LastPositionId
        [Custom("AllowEdit", "False")]
        public int LastPositionId
        {
            get { return _LastPositionId; }
            set { SetPropertyValue("LastPositionId", ref _LastPositionId, value); }
        }

        [Custom("AllowEdit", "False")]
        [Size(500)]
        public string LastAddress
        {
            get { return _LastAddress; }
            set { SetPropertyValue("LastAddress", ref _LastAddress, value); }
        }

        [Custom("AllowEdit", "False")]
        public string LastLongitude
        {
            get { return _LastLongitude; }
            set { SetPropertyValue("LastLongitude", ref _LastLongitude, value); }
        }

        [Custom("AllowEdit", "False")]
        public string LastLatitude
        {
            get { return _LastLatitude; }
            set { SetPropertyValue("LastLatitude", ref _LastLatitude, value); }
        }

        [Custom("AllowEdit", "False")]
        public double TrackMileage
        {
            get { return _TrackMileage; }
            set { SetPropertyValue("TrackMileage", ref _TrackMileage, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool FmsUpdate
        {
            get { return _FmsUpdate; }
            set { SetPropertyValue("FmsUpdate", ref _FmsUpdate, value); }
        }
        #endregion
    }


    public class LastReadings
    {
        public decimal LastOdoRead { get; set; }
        public decimal LastLife { get; set; }
        public DateTime LastFuelDate { get; set; }
        public decimal LastOdoFuelReading { get; set; }
        public decimal LastFuelLife { get; set; }
        public decimal LastFuelLiters { get; set; }
        public decimal LastFuelCost { get; set; }
        public decimal LastFuelTotalAmt { get; set; }
        public PreventiveMaintenance LastServiceID { get; set; }
        public decimal LastServiceLife { get; set; }
        public decimal LastServiceIdLife { get; set; }
        public DateTime LastServiceDate { get; set; }
        public decimal LastServiceRead { get; set; }
    }

}
