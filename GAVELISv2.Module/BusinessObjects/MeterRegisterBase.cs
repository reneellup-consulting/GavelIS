using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum MeterRegistryTypeEnum
    {
        Log,
        Change
    }
    public enum RegistryTypeEnum
    {
        Fuel,
        Service,
        Trip
    }
    [DefaultClassOptions]
    [NonPersistent]
    [FriendlyKeyProperty("Sequence")]
    public abstract class MeterRegisterBase : XPObject
    {
        private Guid _RowID;
        private string _Sequence;
        private MeterRegistryTypeEnum _LogType;
        private DateTime _EntryDate;
        private Employee _ReportedBy;
        private string _Reference;
        private string _Reason;
        private decimal _LastReading;
        private decimal _Reading;
        private decimal _Difference;
        private decimal _Life;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Sequence
        {
            get { return _Sequence; }
            set { SetPropertyValue("Sequence", ref _Sequence, value); }
        }
        private bool _ChangeOdo;
        public bool ChangeOdo
        {
            get { return _ChangeOdo; }
            set { SetPropertyValue("ChangeOdo", ref _ChangeOdo, value); }
        }

        [Custom("AllowEdit", "False")]
        public MeterRegistryTypeEnum LogType
        {
            get { return _LogType; }
            set { SetPropertyValue("LogType", ref _LogType, value); }
        }
        private int _RegRefId;
        [Custom("AllowEdit", "False")]
        [System.ComponentModel.DisplayName("Reg #")]
        public int RegRefId
        {
            get { return _RegRefId; }
            set { SetPropertyValue("RegRefId", ref _RegRefId, value); }
        }

        //[Custom("AllowEdit", "False")]
        //public virtual FixedAsset Fleet
        //{
        //    get { return _Fleet; }
        //    set { SetPropertyValue("Fleet", ref _Fleet, value); }
        //}
        //[Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set
            {
                SetPropertyValue("EntryDate", ref _EntryDate, value);
                //if (!IsLoading)
                //{
                //    Sequence = _EntryDate!=DateTime.MinValue? 
                //        string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}",_EntryDate.Year,_EntryDate.Month,
                //        _EntryDate.Day,_EntryDate.Hour,_EntryDate.Minute,_EntryDate.Second)
                //        :string.Empty;
                //    ProcessRead();
                //}
                if (!IsLoading)
                {
                    string seq = _EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", _EntryDate.Year, _EntryDate.Month,
                       _EntryDate.Day, _EntryDate.Hour, _EntryDate.Minute, _EntryDate.Second, RegRefId > 0 ? RegRefId : 0)
                       : string.Empty;
                    if (BaseFleet != null)
                    {
                        switch (BaseType)
                        {
                            case RegistryTypeEnum.Fuel:
                                var data = BaseFleet.FleetFuelOdoLogs.Where(o => o.Sequence == seq).LastOrDefault();
                                if (data == null)
                                {
                                    Sequence = seq;
                                }
                                else
                                {
                                    DateTime dt = _EntryDate.AddSeconds(1);
                                    Sequence = dt != DateTime.MinValue ?
                                   string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", dt.Year, dt.Month,
                                   dt.Day, dt.Hour, dt.Minute, dt.Second, RegRefId > 0 ? RegRefId : 0)
                                   : string.Empty;
                                }
                                break;
                            case RegistryTypeEnum.Service:
                                var data2 = BaseFleet.FleetServiceOdoLogs.Where(o => o.Sequence == seq).LastOrDefault();
                                if (data2 == null)
                                {
                                    Sequence = seq;
                                }
                                else
                                {
                                    DateTime dt = _EntryDate.AddSeconds(1);
                                    Sequence = dt != DateTime.MinValue ?
                                   string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", dt.Year, dt.Month,
                                   dt.Day, dt.Hour, dt.Minute, dt.Second, RegRefId > 0 ? RegRefId : 0)
                                   : string.Empty;
                                }
                                break;
                            case RegistryTypeEnum.Trip:
                                var data3 = BaseFleet.FleetTripeOdoLogs.Where(o => o.Sequence == seq).LastOrDefault();
                                if (data3 == null)
                                {
                                    Sequence = seq;
                                }
                                else
                                {
                                    DateTime dt = _EntryDate.AddSeconds(1);
                                    Sequence = dt != DateTime.MinValue ?
                                   string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", dt.Year, dt.Month,
                                   dt.Day, dt.Hour, dt.Minute, dt.Second, RegRefId > 0 ? RegRefId : 0)
                                   : string.Empty;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    ProcessRead();
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Employee ReportedBy
        {
            get { return _ReportedBy; }
            set { SetPropertyValue("ReportedBy", ref _ReportedBy, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Reference
        {
            get { return _Reference; }
            set { SetPropertyValue("Reference", ref _Reference, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Reason
        {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastReading
        {
            get { return _LastReading; }
            set { SetPropertyValue("LastReading", ref _LastReading, value); }
        }
        private FixedAsset _BaseFleet;
        [Custom("AllowEdit", "False")]
        public FixedAsset BaseFleet
        {
            get { return _BaseFleet; }
            set { SetPropertyValue("BaseFleet", ref _BaseFleet, value); }
        }
        private RegistryTypeEnum _BaseType;
        [Custom("AllowEdit", "False")]
        public RegistryTypeEnum BaseType
        {
            get { return _BaseType; }
            set { SetPropertyValue("BaseType", ref _BaseType, value); }
        }
        public void CallRead()
        {
            if (!IsLoading)
            {
                if (_Sequence == "20160501000000")
                {

                }
                decimal toDecimal = Convert.ToDecimal(_Sequence);
                switch (BaseType)
                {
                    case RegistryTypeEnum.Fuel:
                        XPCollection<FuelOdoRegistry> fuels = new XPCollection<FuelOdoRegistry>(Session);
                        var data1 = fuels.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (fuels.Count == 0)
                            break;
                        if (data1 == null)
                        {
                            Life = _Reading;
                            break;
                        }
                        LastReading = data1 != null ? data1.Reading : 0m;
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            Life = data1 != null ? data1.Life + _Difference : 0m;
                            // Test 1707061517
                            if (Life == 0m)
                            {
                                Life = _Reading;
                            }
                        }
                        if (_LogType == MeterRegistryTypeEnum.Change && data1.LogType == MeterRegistryTypeEnum.Log)
                        {
                            Difference = 0m;
                            Life = data1.Life;
                        }
                        if (_LogType == MeterRegistryTypeEnum.Change && data1.LogType == MeterRegistryTypeEnum.Change)
                        {
                            Difference = 0m;
                            Life = data1.Life;
                        }
                        if (data1.LogType == MeterRegistryTypeEnum.Change && data1.Reading != 0m)
                        {
                            Difference = _Reading - _LastReading;
                            Life = data1 != null ? data1.Life + _Difference : 0m;
                        }
                        var datab1 = fuels.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                        if (datab1 == null)
                        {
                            break;
                        }
                        datab1.LastReading = _Reading;
                        datab1.Difference = datab1.Reading - datab1.LastReading;
                        if (Life < Reading)
                        {
                            Life = _Reading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            datab1.Life = _Life + datab1.Difference;
                        }
                        break;
                    case RegistryTypeEnum.Service:
                        XPCollection<ServiceOdoRegistry> servs = new XPCollection<ServiceOdoRegistry>(Session);
                        var data2 = servs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (servs.Count == 0)
                            break;
                        if (data2 == null)
                        {
                            Life = _Reading;
                            break;
                        }
                        LastReading = data2 != null ? data2.Reading : 0m;
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            Life = data2 != null ? data2.Life + _Difference : 0m;
                            // Test 1707061517
                            if (Life == 0m)
                            {
                                Life = _Reading;
                            }
                        }
                        if (_LogType == MeterRegistryTypeEnum.Change && data2.LogType == MeterRegistryTypeEnum.Log)
                        {
                            Difference = 0m;
                            Life = data2.Life;
                        }
                        if (_LogType == MeterRegistryTypeEnum.Change && data2.LogType == MeterRegistryTypeEnum.Change)
                        {
                            Difference = 0m;
                            Life = data2.Life;
                        }
                        if (data2.LogType == MeterRegistryTypeEnum.Change && data2.Reading != 0m)
                        {
                            Difference = _Reading - _LastReading;
                            Life = data2 != null ? data2.Life + _Difference : 0m;
                        }
                        var datab2 = servs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                        if (datab2 == null)
                        {
                            break;
                        }
                        datab2.LastReading = _Reading;
                        datab2.Difference = datab2.Reading - datab2.LastReading;
                        if (Life < Reading)
                        {
                            Life = _Reading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            datab2.Life = _Life + datab2.Difference;
                        }
                        break;
                    case RegistryTypeEnum.Trip:
                        XPCollection<TripOdoRegistry> trips = new XPCollection<TripOdoRegistry>(Session);
                        var data3 = trips.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (trips.Count == 0)
                            break;
                        if (data3 == null)
                        {
                            Life = _Reading;
                            break;
                        }
                        LastReading = data3 != null ? data3.Reading : 0m;
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            Life = data3 != null ? data3.Life + _Difference : 0m;
                            // Test 1707061517
                            if (Life == 0m)
                            {
                                Life = _Reading;
                            }
                        }
                        if (_LogType == MeterRegistryTypeEnum.Change && data3.LogType == MeterRegistryTypeEnum.Log)
                        {
                            Difference = 0m;
                            Life = data3.Life;
                        }
                        if (_LogType == MeterRegistryTypeEnum.Change && data3.LogType == MeterRegistryTypeEnum.Change)
                        {
                            Difference = 0m;
                            Life = data3.Life;
                        }
                        if (data3.LogType == MeterRegistryTypeEnum.Change && data3.Reading != 0m)
                        {
                            Difference = _Reading - _LastReading;
                            Life = data3 != null ? data3.Life + _Difference : 0m;
                        }
                        var datab3 = trips.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                        if (datab3 == null)
                        {
                            break;
                        }
                        datab3.LastReading = _Reading;
                        datab3.Difference = datab3.Reading - datab3.LastReading;
                        if (Life < Reading)
                        {
                            Life = _Reading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            datab3.Life = _Life + datab3.Difference;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastLife
        {
            get
            {
                decimal toDecimal = Convert.ToDecimal(_Sequence);
                switch (BaseType)
                {
                    case RegistryTypeEnum.Fuel:
                        var data1 = _BaseFleet.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        return data1 != null ? data1.Life : 0m;
                    case RegistryTypeEnum.Service:
                        var data2 = _BaseFleet.FleetServiceOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        return data2 != null ? data2.Life : 0m;
                    case RegistryTypeEnum.Trip:
                        var data3 = _BaseFleet.FleetTripeOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        return data3 != null ? data3.Life : 0m;
                    default:
                        return 0m;
                }
            }
        }
        public void ProcessRead()
        {
            if (!IsLoading && _LogType != MeterRegistryTypeEnum.Change)
            {
                decimal toDecimal = Convert.ToDecimal(_Sequence);
                switch (BaseType)
                {
                    case RegistryTypeEnum.Fuel:
                        var data1 = _BaseFleet.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (_BaseFleet.FleetFuelOdoLogs.Count == 0)
                            Life = _Reading;
                        if (data1 == null && _BaseFleet.FleetFuelOdoLogs.Count == 1)
                        {
                            Life = _Reading;
                            break;
                        }
                        else if (data1 == null && _BaseFleet.FleetFuelOdoLogs.Count > 1)
                        {
                            Life = _Reading;
                            Difference = 0m;
                        }
                        LastReading = data1 != null ? data1.Reading : 0m;
                        if (_LastReading != 0)
                        {
                            Difference = _Reading - _LastReading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            Life = data1 != null ? data1.Life + _Difference : 0m;
                            // Test 1707061517
                            if (Life == 0m)
                            {
                                Life = _Reading;
                            }
                        }
                        var datab1 = _BaseFleet.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                        if (datab1 == null)
                        {
                            break;
                        }
                        datab1.LastReading = _Reading;
                        datab1.Difference = datab1.Reading - datab1.LastReading;
                        if (datab1 != null && Life < Reading)
                        {
                            Life = _Reading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            datab1.Life = _Life + datab1.Difference;
                        }
                        break;
                    case RegistryTypeEnum.Service:
                        if (_BaseFleet == null)
                        {
                            break;
                        }
                        var data2 = _BaseFleet.FleetServiceOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (_BaseFleet.FleetServiceOdoLogs.Count == 0)
                            Life = _Reading;
                        if (data2 == null && _BaseFleet.FleetServiceOdoLogs.Count == 1)
                        {
                            Life = _Reading;
                            break;
                        }
                        else if (data2 == null && _BaseFleet.FleetServiceOdoLogs.Count > 1)
                        {
                            Life = _Reading;
                            Difference = 0m;
                        }
                        LastReading = data2 != null ? data2.Reading : 0m;
                        if (_LastReading != 0)
                        {
                            Difference = _Reading - _LastReading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            Life = data2 != null ? data2.Life + _Difference : 0m;
                            // Test 1707061517
                            if (Life == 0m)
                            {
                                Life = _Reading;
                            }
                        }
                        var datab2 = _BaseFleet.FleetServiceOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                        if (datab2 == null)
                        {
                            break;
                        }
                        datab2.LastReading = _Reading;
                        datab2.Difference = datab2.Reading - datab2.LastReading;
                        if (datab2 != null && Life < Reading)
                        {
                            Life = _Reading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            datab2.Life = _Life + datab2.Difference;
                        }
                        break;
                    case RegistryTypeEnum.Trip:
                        var data3 = _BaseFleet.FleetTripeOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (_BaseFleet.FleetTripeOdoLogs.Count == 0)
                            Life = _Reading;
                        if (data3 == null && _BaseFleet.FleetTripeOdoLogs.Count == 1)
                        {
                            Life = _Reading;
                            break;
                        }
                        else if (data3 == null && _BaseFleet.FleetTripeOdoLogs.Count > 1)
                        {
                            Life = _Reading;
                            Difference = 0m;
                        }
                        LastReading = data3 != null ? data3.Reading : 0m;
                        if (_LastReading != 0)
                        {
                            Difference = _Reading - _LastReading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            Life = data3 != null ? data3.Life + _Difference : 0m;
                            // Test 1707061517
                            if (Life == 0m)
                            {
                                Life = _Reading;
                            }
                        }
                        var datab3 = _BaseFleet.FleetTripeOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                        if (datab3 == null)
                        {
                            break;
                        }
                        datab3.LastReading = _Reading;
                        datab3.Difference = datab3.Reading - datab3.LastReading;
                        if (datab3 != null && Life < Reading)
                        {
                            Life = _Reading;
                        }
                        if (_LogType != MeterRegistryTypeEnum.Change)
                        {
                            datab3.Life = _Life + datab3.Difference;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Reading
        {
            get { return _Reading; }
            set
            {
                SetPropertyValue("Reading", ref _Reading, value);
                ProcessRead();
                if (!IsLoading)
                {
                    FATrailer trailer = null;
                    TripOdoRegistry odoLog = null;
                    if (this.GetType() == typeof(TripOdoRegistry) && (this as TripOdoRegistry).Trailer != null)
                    {
                        trailer = (this as TripOdoRegistry).Trailer;
                        odoLog = this as TripOdoRegistry;
                    }
                    if (!IsLoading && this.BaseType == RegistryTypeEnum.Trip && trailer != null)
                    {
                        decimal toDecimal = Convert.ToDecimal(Sequence);
                        TripOdoRegistry data3 = null;
                        decimal lstMileage = 0m;
                        if (trailer.TraileRegistryTripLogs != null)
                        {
                            data3 = trailer.TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => o.Trailer == trailer && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                            lstMileage = data3 != null ? data3.TrlrMileage : 0m;
                            odoLog.TrlrMileage = lstMileage + odoLog.Distance;
                        }
                        else
                        {
                            odoLog.TrlrMileage = lstMileage + odoLog.Distance;
                        }
                    }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Difference
        {
            get { return _Difference; }
            set { SetPropertyValue("Difference", ref _Difference, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Life
        {
            get { return _Life; }
            set { SetPropertyValue("Life", ref _Life, value); }
        }
        [NonPersistent]
        public bool Negative
        {
            get { return _Difference < 0; }
        }
        [NonPersistent]
        public bool InvalidDiff
        {
            get
            {
                if (_LastReading == 0)
                {
                    return false;
                }
                decimal diff = _Reading - _LastReading;
                switch (_LogType)
                {
                    case MeterRegistryTypeEnum.Log:
                        if (diff != _Difference)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case MeterRegistryTypeEnum.Change:
                        return false;
                    default:
                        return false;
                }
            }
        }
        [NonPersistent]
        public string Descrepancy
        {
            get {
                string _Descrepancy = String.Empty;
                if (Negative)
                {
                    _Descrepancy = "Negative difference";
                }
                else if (UnderRead)
                {
                    _Descrepancy = "Decreasing life";
                }
                else if (InvalidDiff)
                {
                    _Descrepancy = "Invalid difference";
                }
                else if (Negative && UnderRead)
                {
                    _Descrepancy = "Negative and decreasing life";
                }
                else
                {
                    _Descrepancy = "None";
                }
                return _Descrepancy; }
        }

        [NonPersistent]
        public bool UnderRead { get 
        {
            decimal toDecimal = Convert.ToDecimal(_Sequence);
            switch (BaseType)
            {
                case RegistryTypeEnum.Fuel:
                    //XPCollection<FuelOdoRegistry> fuels = new XPCollection<FuelOdoRegistry>(Session);
                    var data1 = _BaseFleet.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        //if (fuels.Count == 0)
                        //    break;
                        if (data1 != null && data1.Life > _Life)
                        {
                            return true;
                        }
                    break;
                case RegistryTypeEnum.Service:
                    //XPCollection<ServiceOdoRegistry> servs = new XPCollection<ServiceOdoRegistry>(Session);
                        var data2 = _BaseFleet.FleetServiceOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        //if (servs.Count == 0)
                        //    break;
                        if (data2 != null && data2.Life > _Life)
                        {
                            return true;
                        }
                    break;
                case RegistryTypeEnum.Trip:
                    //XPCollection<TripOdoRegistry> trips = new XPCollection<TripOdoRegistry>(Session);
                    var data3 = _BaseFleet.FleetTripeOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        //if (trips.Count == 0)
                        //    break;
                        if (data3 != null && data3.Life > _Life)
                        {
                            return true;
                        }
                    break;
                default:
                    break;
            }
            return false; } }

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        public MeterRegisterBase(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            RowID = Guid.NewGuid();
            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }
        }
        private string delSequenceNo;
        protected override void OnDeleting()
        {
            delSequenceNo = this.Sequence;
            decimal toDecimal = Convert.ToDecimal(delSequenceNo);
            FuelOdoRegistry afterFuel = null;
            ServiceOdoRegistry afterService = null;
            TripOdoRegistry afterTrip = null;
            switch (BaseType)
            {
                case RegistryTypeEnum.Fuel:
                    afterFuel = _BaseFleet.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && o.LogType != MeterRegistryTypeEnum.Change && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                    break;
                case RegistryTypeEnum.Service:
                    afterService = _BaseFleet.FleetServiceOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && o.LogType != MeterRegistryTypeEnum.Change && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                    break;
                case RegistryTypeEnum.Trip:
                    afterTrip = _BaseFleet.FleetTripeOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && o.LogType != MeterRegistryTypeEnum.Change && Convert.ToDecimal(o.Sequence) > toDecimal).FirstOrDefault();
                    break;
                default:
                    break;
            }
            //if (afterFuel != null)
            //{
                decimal delSeqDecimal = Convert.ToDecimal(delSequenceNo);
                switch (BaseType)
                {
                    case RegistryTypeEnum.Fuel:
                        if (afterFuel == null)
                        {
                            break;
                        }
                        var data1 = _BaseFleet.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < delSeqDecimal).LastOrDefault();
                        if (data1 != null)
                        {
                            afterFuel.LastReading = data1.Reading;
                            afterFuel.Difference = afterFuel.Reading - afterFuel.LastReading;
                        }
                        else
                        {
                            afterFuel.LastReading = 0m;
                            afterFuel.Difference = 0m;
                        }
                        if (data1 != null)
                        {
                            if (afterFuel != null && data1.Life < data1.Reading)
                            {
                                data1.Life = data1.Reading;
                            }
                            if (_LogType != MeterRegistryTypeEnum.Change)
                            {
                                afterFuel.Life = data1.Life + afterFuel.Difference;
                            }
                        }       
                        break;
                    case RegistryTypeEnum.Service:
                        if (afterService == null)
                        {
                            break;
                        }
                        var data2 = _BaseFleet.FleetServiceOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < delSeqDecimal).LastOrDefault();
                        if (data2 != null)
                        {
                            afterService.LastReading = data2.Reading;
                            afterService.Difference = afterService.Reading - afterService.LastReading;
                        }
                        else
                        {
                            afterService.LastReading = 0m;
                            afterService.Difference = 0m;
                        }
                        if (data2 != null)
                        {
                            if (afterTrip != null && data2.Life < data2.Reading)
                            {
                                data2.Life = data2.Reading;
                            }
                            if (_LogType != MeterRegistryTypeEnum.Change)
                            {
                                afterService.Life = data2.Life + afterService.Difference;
                            }
                        }          
                        break;
                    case RegistryTypeEnum.Trip:
                        if (afterTrip == null)
                        {
                            break;
                        }
                        var data3 = _BaseFleet.FleetTripeOdoLogs.OrderBy(o => o.Sequence).Where(o => o.Fleet == _BaseFleet && Convert.ToDecimal(o.Sequence) < delSeqDecimal).LastOrDefault();
                        if (data3 != null)
                        {
                            afterTrip.LastReading = data3.Reading;
                            afterTrip.Difference = afterTrip.Reading - afterTrip.LastReading;
                        }
                        else
                        {
                            afterTrip.LastReading = 0m;
                            afterTrip.Difference = 0m;
                        }
                        if (data3 != null)
                        {
                            if (afterTrip != null && data3.Life < data3.Reading)
                            {
                                data3.Life = data3.Reading;
                            }
                            if (_LogType != MeterRegistryTypeEnum.Change)
                            {
                                afterTrip.Life = data3.Life + afterTrip.Difference;
                            }
                        }                        
                        break;
                    default:
                        break;
                }
            //}
            base.OnDeleting();
        }
        protected override void OnSaving()
        {
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
