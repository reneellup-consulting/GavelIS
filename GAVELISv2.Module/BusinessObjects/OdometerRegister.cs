using System;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class OdometerRegister : XPObject {
        private decimal _PreviousReading;
        private Guid _RowID;
        private FixedAsset _Fleet;
        private DateTime _EntryDate;
        private Employee _ReportedBy;
        private MeterLogTypeEnum _LogType;
        private MeterEntryTypeEnum _MeterType;
        private string _Reference;
        private string _Reason;
        private bool _Corrected = false;
        private decimal _LastFuelOdoReading;
        private decimal _Reading;
        private decimal _Difference;
        private decimal _Life;
        private decimal _Liters;
        private decimal _Cost;
        private decimal _Range;
        private decimal _ServiceRange;
        private decimal _ServiceIdRange;
        private decimal _ServiceCost;
        private decimal _LitersPerKm;
        private long _SeqNo;
        private PreventiveMaintenance _PrevMaintenanceID;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("Fleet-OdometerRegister")]
        public FixedAsset Fleet {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }

        void CallRead() {
            LastReadings lstRrd = new LastReadings();
            lstRrd = _Fleet.GetLastReadingBeforeDate(_EntryDate);
            //Difference = _Reading - lstRrd.LastOdoRead;
            //Life = lstRrd.LastLife + _Difference;

            LastReadings lstRrdSrv = new LastReadings();
            LastReadings lstRrdFl = new LastReadings();
            if (LogType == MeterLogTypeEnum.Service && PrevMaintenanceID != null)
            {
                lstRrdSrv = _Fleet.GetServiceIdLastReadingBeforeDate(EntryDate, PrevMaintenanceID);
            }
            else if (LogType == MeterLogTypeEnum.Service && PrevMaintenanceID == null)
            {
                lstRrdSrv = _Fleet.GetServiceLastReadingBeforeDate(EntryDate);
            }
            else if (LogType == MeterLogTypeEnum.Fuel)
            {
                lstRrdFl = _Fleet.GetFuelLastReadingBeforeDate(EntryDate);
            }
            //lstRrdFl = _Fleet.GetFuelLastReadingBeforeDate(_EntryDate);
            //Range = lstRrdFl.LastFuelLife != 0 ? Life - lstRrdFl.LastFuelLife : 0;
            switch (_LogType)
            {
                case MeterLogTypeEnum.Initial:
                    //if (_Fleet.InitialLife<=0)
                    //{
                    //    throw new UserFriendlyException(
                    //    "Must provide a valid initial life value");
                    //}
                    Difference = 0;
                    Fleet.InitialLife = _Reading;
                    Life = _Fleet.InitialLife;
                    Fleet.LastLife = _Fleet.InitialLife;
                    Fleet.LastOdoReading = _Reading;
                    break;
                case MeterLogTypeEnum.Log:
                    Difference = _Reading - (lstRrd != null ? lstRrd.LastOdoRead : 0);
                    Life = (lstRrd != null ? lstRrd.LastLife : 0) + Difference;
                    Range = 0m;
                    break;
                case MeterLogTypeEnum.Change:
                    Difference = 0m;
                    Life = lstRrd != null ? lstRrd.LastLife : 0;
                    Range = 0m;
                    //Life = _Fleet.LastLife;
                    //_Fleet.LastOdoReading = _Reading;
                    break;
                case MeterLogTypeEnum.Correct:
                    ////Difference = 0;
                    ////Life = _Reading;
                    ////_Fleet.LastLife = _Reading;
                    //Difference = _Reading - _Fleet.LastOdoReading;
                    //Life = _Fleet.LastLife + _Difference;
                    //_Fleet.LastLife = _Life;
                    //_Fleet.LastOdoReading = _Reading;
                    break;
                case MeterLogTypeEnum.Fuel:
                    Difference = Reading - (lstRrd != null ? lstRrd.LastOdoRead : 0);
                    Life = (lstRrd != null ? lstRrd.LastLife : 0) + Difference;
                    Range = lstRrdFl != null ? lstRrdFl.LastFuelLife != 0 ? Life - lstRrdFl.LastFuelLife : 0 : 0;
                    ////odo = ReflectionHelper.CreateObject<OdometerRegister>(session);
                    ////odo.Fleet = thisFA;
                    ////odo.MeterType = MeterEntryTypeEnum.Odometer;
                    ////odo.LogType = MeterLogTypeEnum.Fuel;
                    ////odo.EntryDate = thisReceipt.EntryDate;
                    ////odo.ReportedBy = thisReceipt.Driver != null ? thisReceipt.Driver : null;
                    ////odo.Reference = thisReceipt.InvoiceNo + "/" + thisReceipt.SourceNo;
                    ////odo.Liters = thisReceipt.TotalQty.Value;
                    ////odo.Cost = thisReceipt.Total.Value;
                    ////odo.LastFuelOdoReading = thisReceipt.PrevOdoRead;
                    ////odo.Reading = thisReceipt.OdoRead;

                    ////Difference = _Reading - _LastFuelOdoReading;
                    ////Range = _LastFuelOdoReading != 0 ? _Reading - _LastFuelOdoReading : 0;
                    ////Life = _LastFuelOdoReading + _Difference;
                    ////_Fleet.LastLife = _Life;
                    ////_Fleet.LastOdoReading = _Reading;
                    ////_Fleet.LastFuelLifeReading = _Life;
                    ////_Fleet.LastOdoFuelReading = _Reading;
                    //////_Fleet.LastFuelDate = _EntryDate;
                    ////_Fleet.LastFuelLiters = _Liters;
                    ////_Fleet.LastFuelTotalAmt = _Cost;

                    //// What if LastReading.LogType is Change
                    ////LastReadings lstRrd = new LastReadings();
                    ////lstRrd = _Fleet.GetLastReadingBeforeDate(_EntryDate);
                    //Difference = _Reading - lstRrd.LastOdoRead;
                    //Life = lstRrd.LastLife + _Difference;

                    ////LastReadings lstRrdFl = new LastReadings();
                    ////lstRrdFl = _Fleet.GetFuelLastReadingBeforeDate(_EntryDate);
                    //Range = lstRrdFl.LastFuelLife != 0 ? Life - lstRrdFl.LastFuelLife : 0;
                    ////_Fleet.LastLife = _Life;
                    ////_Fleet.LastOdoReading = _Reading;
                    ////_Fleet.LastFuelLifeReading = _Life;
                    ////_Fleet.LastOdoFuelReading = _Reading;
                    //////_Fleet.LastFuelDate = _EntryDate;
                    ////_Fleet.LastFuelLiters = _Liters;
                    ////_Fleet.LastFuelTotalAmt = _Cost;
                    break;
                case MeterLogTypeEnum.Service:
                    Difference = Reading - (lstRrd != null ? lstRrd.LastOdoRead : 0);
                    Life = (lstRrd != null ? lstRrd.LastLife : 0) + Difference;
                    ServiceRange = lstRrdSrv != null ? lstRrdSrv.LastServiceLife != 0 ? Life - lstRrdSrv.LastServiceLife : 0 : 0;
                    ServiceIdRange = lstRrdSrv != null ? lstRrdSrv.LastServiceIdLife != 0 ? Life - lstRrdSrv.LastServiceIdLife : 0 : 0;
                    //Difference = _Reading - _Fleet.LastOdoReading;
                    ////Range = _Reading - _Fleet.LastOdoFuelReading;
                    //Range = _Fleet.LastOdoServiceReading != 0 ? _Reading - _Fleet.LastOdoServiceReading : 0;
                    //Life = _Fleet.LastLife + _Difference;
                    //_Fleet.LastLife = _Life;
                    //_Fleet.LastOdoReading = _Reading;
                    //_Fleet.LastOdoServiceReading = _Reading;
                    //_Fleet.LastServiceLifeReading = _Life;
                    ////_Fleet.LastServiceDate = _EntryDate;
                    //_Fleet.LastServiceCost = _Cost;
                    break;
                default:
                    break;
            }
        }

        public DateTime EntryDate {
            get { return _EntryDate; }
            set
            {
                SetPropertyValue("EntryDate", ref _EntryDate, value);
                if (!IsLoading && _Fleet != null)
                {
                    switch (_LogType)
                    {
                        case MeterLogTypeEnum.Fuel:
                            _Fleet.LastFuelDate = _EntryDate;
                            break;
                        case MeterLogTypeEnum.Service:
                            _Fleet.LastServiceDate = _EntryDate;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public Employee ReportedBy {
            get { return _ReportedBy; }
            set { SetPropertyValue("ReportedBy", ref _ReportedBy, value); }
        }

        [Custom("AllowEdit", "False")]
        public MeterLogTypeEnum LogType {
            get { return _LogType; }
            set { SetPropertyValue("LogType", ref _LogType, value); }
        }

        public MeterEntryTypeEnum MeterType {
            get { return _MeterType; }
            set { SetPropertyValue("MeterType", ref _MeterType, value); }
        }

        public string Reference {
            get { return _Reference; }
            set { SetPropertyValue("Reference", ref _Reference, value); }
        }

        public string Reason {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }

        public bool Corrected {
            get { return _Corrected; }
            set { SetPropertyValue("Corrected", ref _Corrected, value); }
        }

        public decimal LastFuelOdoReading {
            get { return _LastFuelOdoReading; }
            set { SetPropertyValue<decimal>("LastFuelOdoReading", ref _LastFuelOdoReading, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Reading {
            get { return _Reading; }
            set
            {
                SetPropertyValue("Reading", ref _Reading, value);
                if (!IsLoading && _Fleet != null)
                {
                    CallRead();
                    //switch (_LogType) {
                    //    case MeterLogTypeEnum.Initial:
                    //        Difference = 0;
                    //        Life = _Reading;
                    //        _Fleet.LastLife = _Reading;
                    //        _Fleet.LastOdoReading = _Reading;
                    //        break;
                    //    case MeterLogTypeEnum.Log:
                    //        Difference = _Reading - _Fleet.LastOdoReading;
                    //        Life = _Fleet.LastLife + _Difference;
                    //        _Fleet.LastLife = _Life;
                    //        _Fleet.LastOdoReading = _Reading;
                    //        break;
                    //    case MeterLogTypeEnum.Change:
                    //        Difference = 0;
                    //        Life = _Fleet.LastLife;
                    //        _Fleet.LastOdoReading = _Reading;
                    //        break;
                    //    case MeterLogTypeEnum.Correct:
                    //        //Difference = 0;
                    //        //Life = _Reading;
                    //        //_Fleet.LastLife = _Reading;
                    //        Difference = _Reading - _Fleet.LastOdoReading;
                    //        Life = _Fleet.LastLife + _Difference;
                    //        _Fleet.LastLife = _Life;
                    //        _Fleet.LastOdoReading = _Reading;
                    //        break;
                    //    case MeterLogTypeEnum.Fuel:
                    //        Difference = _Reading - _Fleet.LastOdoReading;
                    //        Range = _Fleet.LastOdoFuelReading!=0? _Reading - _Fleet.LastOdoFuelReading:0;
                    //        Life = _Fleet.LastLife + _Difference;
                    //        _Fleet.LastLife = _Life;
                    //        _Fleet.LastOdoReading = _Reading;
                    //        _Fleet.LastFuelLifeReading = _Life;
                    //        _Fleet.LastOdoFuelReading = _Reading;
                    //        _Fleet.LastFuelDate = _EntryDate;
                    //        _Fleet.LastFuelLiters = _Liters;
                    //        _Fleet.LastFuelTotalAmt = _Cost;
                    //        break;
                    //    case MeterLogTypeEnum.Service:
                    //        Difference = _Reading - _Fleet.LastOdoReading;
                    //        //Range = _Reading - _Fleet.LastOdoFuelReading;
                    //        Range = _Fleet.LastOdoServiceReading != 0 ? _Reading - _Fleet.LastOdoServiceReading : 0;
                    //        Life = _Fleet.LastLife + _Difference;
                    //        _Fleet.LastLife = _Life;
                    //        _Fleet.LastOdoReading = _Reading;
                    //        _Fleet.LastOdoServiceReading = _Reading;
                    //        _Fleet.LastServiceLifeReading = _Life;
                    //        _Fleet.LastServiceDate = _EntryDate;
                    //        _Fleet.LastServiceCost = _Cost;
                    //        break;
                    //    default:
                    //        break;
                    //}
                    //if (_Difference<0)
                    //{
                    //    throw new UserFriendlyException(
                    //    "Entered reading value resulted to a negative difference value. Which cannot be permitted.");
                    //}
                }
            }
        }

        //[DisplayName("Range")]
        public decimal Difference {
            get { return _Difference; }
            set { SetPropertyValue("Difference", ref _Difference, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        [Custom("AllowEdit", "False")]
        public decimal Life {
            get { return _Life; }
            set { SetPropertyValue("Life", ref _Life, value); }
        }

        public decimal Liters {
            get { return _Liters; }
            set
            {
                SetPropertyValue("Liters", ref _Liters, value);
                if (!IsLoading && _Fleet != null)
                {
                    switch (_LogType)
                    {
                        case MeterLogTypeEnum.Fuel:
                            _Fleet.LastFuelLiters = _Liters;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public decimal Cost {
            get { return _Cost; }
            set
            {
                SetPropertyValue("Cost", ref _Cost, value);
                if (!IsLoading && _Fleet != null)
                {
                    switch (_LogType)
                    {
                        case MeterLogTypeEnum.Fuel:
                            _Fleet.LastFuelTotalAmt = _Cost;
                            break;
                        case MeterLogTypeEnum.Service:
                            _Fleet.LastServiceCost = _Cost;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public decimal Range {
            get { return _Range; }
            set { SetPropertyValue("Range", ref _Range, value); }
        }

        public decimal ServiceRange {
            get { return _ServiceRange; }
            set { SetPropertyValue("ServiceRange", ref _ServiceRange, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal ServiceIdRange {
            get { return _ServiceIdRange; }
            set { SetPropertyValue<decimal>("ServiceIdRange", ref _ServiceIdRange, value); }
        }

        public decimal ServiceCost {
            get { return _ServiceCost; }
            set { SetPropertyValue<decimal>("ServiceCost", ref _ServiceCost, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "f0")]
        [Custom("EditMask", "f0")]
        public long SeqNo {
            get { return _SeqNo; }
            set { SetPropertyValue<long>("SeqNo", ref _SeqNo, value); }
        }

        public PreventiveMaintenance PrevMaintenanceID {
            get { return _PrevMaintenanceID; }
            set { SetPropertyValue<PreventiveMaintenance>("PrevMaintenanceID", ref _PrevMaintenanceID, value); }
        }

        //public decimal Range {
        //    get { return _Range; }
        //    set { SetPropertyValue("Range", ref _Range, value); }
        //}
        [PersistentAlias("Liters/Range")]
        [Custom("DisplayFormat", "n")]
        public decimal LitersPerKm {
            get
            {
                try
                {
                    if (Range <= 0)
                    {
                        return 0;
                    }
                    object tempObject = EvaluateAlias("LitersPerKm");
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

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        [NonPersistent]
        public decimal PreviousReading { get; set; }

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

        public OdometerRegister(Session session)
            : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            RowID = Guid.NewGuid();
            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }
        }

        protected override void OnSaving() {
            if (Oid == -1)
            {
                _Fleet.SeqNo++;
                _SeqNo = _Fleet.SeqNo;
            }
            base.OnSaving();
            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
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

    }
}
