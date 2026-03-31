using System;
using System.Linq;
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
    public class TruckRegistry : XPObject {
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        private string _ReferenceNo;
        private DateTime _Date = DateTime.Now;
        private FATruck _TruckNo;
        private Employee _Driver;
        private DateTime _StartOut;
        private DateTime _EndIn;
        private decimal _PreOdoRead;
        private decimal _PostOdoRead;
        //private decimal _KMRunOdo;
        private decimal _KMRunMnl;
        private decimal _PreFuelRead;
        private decimal _PostFuelRead;
        //private decimal _FuelConsumedGge;
        private decimal _FuelConsumedMnl;
        private string _Location;
        private string _Reason;
        private TruckRegistryStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-TruckRegistrations")]
        public GenJournalHeader TripID {
            get { return _TripID; }
            set {
                SetPropertyValue("TripID", ref _TripID, value);
                if (!IsLoading && _TripID != null) {
                    TripNo = _TripID.SourceNo;
                    Date = _TripID.EntryDate;
                    if (_TripID.GetType() == typeof(StanfilcoTrip)) {ReferenceNo 
                        = ((StanfilcoTrip)_TripID).DTRNo;}
                    if (_TripID.GetType() == typeof(DolefilTrip)) {ReferenceNo = 
                        ((DolefilTrip)_TripID).DocumentNo;}
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
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public FATruck TruckNo {
            get { return _TruckNo; }
            set {
                SetPropertyValue("TruckNo", ref _TruckNo, value);
                if (!IsLoading && _TruckNo != null) {Driver = _TruckNo.Operator 
                    != null ? _TruckNo.Operator : null;}
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor2")]
        public DateTime StartOut {
            get { return _StartOut; }
            set { SetPropertyValue("StartOut", ref _StartOut, value); }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor2")]
        public DateTime EndIn {
            get { return _EndIn; }
            set { SetPropertyValue("EndIn", ref _EndIn, value); }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PreOdoRead {
            get { return _PreOdoRead; }
            set { SetPropertyValue("PreOdoRead", ref _PreOdoRead, value); }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PostOdoRead {
            get { return _PostOdoRead; }
            set { SetPropertyValue("PostOdoRead", ref _PostOdoRead, value); }
        }
        [PersistentAlias("PostOdoRead - PreOdoRead")]
        //[EditorAlias("MeterTypePropertyEditor")]
        [Custom("DisplayFormat", "n")]
        public decimal KMRunOdo {
            get {
                object tempObject = EvaluateAlias("KMRunOdo");
                if (tempObject != null) {
                    if ((decimal)tempObject != 0) {KMRunMnl = (decimal)
                        tempObject > 0 ? (decimal)tempObject : 0;}
                    return (decimal)tempObject > 0 ? (decimal)tempObject : 0;
                } else {
                    return 0;
                }
            }
        }
        //[EditorAlias("MeterTypePropertyEditor")]
        [Custom("DisplayFormat", "n")]
        public decimal KMRunMnl {
            get { return _KMRunMnl; }
            set {
                SetPropertyValue("KMRunMnl", ref _KMRunMnl, value);
                if (!IsLoading) {
                    try {
                        if (_TripID.GetType() == typeof(StanfilcoTrip)) {((
                            StanfilcoTrip)_TripID).UpdateActualRun(true);}
                        if (_TripID.GetType() == typeof(DolefilTrip)) {((
                            DolefilTrip)_TripID).UpdateActualRun(true);}
                        if (_TripID.GetType() == typeof(OtherTrip)) {((OtherTrip
                            )_TripID).UpdateActualRun(true);}
                    } catch (Exception) {}
                }
            }
        }
        //[EditorAlias("MeterTypePropertyEditor")]
        [Custom("DisplayFormat", "n")]
        public decimal PreFuelRead {
            get { return _PreFuelRead; }
            set { SetPropertyValue("PreFuelRead", ref _PreFuelRead, value); }
        }
        //[EditorAlias("MeterTypePropertyEditor")]
        [Custom("DisplayFormat", "n")]
        public decimal PostFuelRead {
            get { return _PostFuelRead; }
            set { SetPropertyValue("PostFuelRead", ref _PostFuelRead, value); }
        }
        [PersistentAlias("PreFuelRead - PostFuelRead")]
        //[EditorAlias("MeterTypePropertyEditor")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelConsumedGge {
            get {
                object tempObject = EvaluateAlias("FuelConsumedGge");
                if (tempObject != null) {
                    if ((decimal)tempObject != 0) {FuelConsumedMnl = (decimal)
                        tempObject > 0 ? (decimal)tempObject : 0;}
                    return (decimal)tempObject > 0 ? (decimal)tempObject : 0;
                } else {
                    return 0;
                }
            }
        }
        //[EditorAlias("MeterTypePropertyEditor")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelConsumedMnl {
            get { return _FuelConsumedMnl; }
            set {
                SetPropertyValue("FuelConsumedMnl", ref _FuelConsumedMnl, value)
                ;
                if (!IsLoading) {
                    try {
                        if (_TripID.GetType() == typeof(StanfilcoTrip)) {((
                            StanfilcoTrip)_TripID).UpdateFuelConsumed(true);}
                        if (_TripID.GetType() == typeof(DolefilTrip)) {((
                            StanfilcoTrip)_TripID).UpdateFuelConsumed(true);}
                    } catch (Exception) {}
                }
            }
        }
        //[RuleRequiredField("", DefaultContexts.Save)]
        public string Location {
            get { return _Location; }
            set { SetPropertyValue("Location", ref _Location, value); }
        }
        //[RuleRequiredField("", DefaultContexts.Save)]
        public string Reason {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }
        public TruckRegistryStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
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
        [Aggregated,
        Association("TruckRegistryOdoLogs")]
        public XPCollection<TripOdoRegistry> TruckRegistryOdoLogs
        {
            get
            {
                return GetCollection<TripOdoRegistry>("TruckRegistryOdoLogs"
                    );
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
        public TruckRegistry(Session session): base(session) {
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
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }
        // 1707071201 Start..
        [DisplayName("Trailer")]
        [NonPersistent]
        public FATrailer AttachedTrailer
        {
            get {
                if (TripID != null)
                {
                    if (TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        StanfilcoTrip st = TripID as StanfilcoTrip;
                        if (st.TrailerNo != null)
                        {
                            return st.TrailerNo;
                        }
                    }
                }
                return null; }
        }
        [NonPersistent]
        public Tariff Tariff
        {
            get
            {
                if (TripID != null)
                {
                    if (TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        StanfilcoTrip st = TripID as StanfilcoTrip;
                        if (st.Tariff != null)
                        {
                            return st.Tariff;
                        }
                    }
                }
                return null;
            }
        }
        [DisplayName("TRF Dist.")]
        [NonPersistent]
        public decimal TariffDistance
        {
            get
            {
                if (TripID != null)
                {
                    if (TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        StanfilcoTrip st = TripID as StanfilcoTrip;
                        if (st.Tariff != null)
                        {
                            return st.Tariff.Distance;
                        }
                    }
                }
                return 0m;
            }
        }

        [NonPersistent]
        public bool Hustling
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip) && (_TripID as StanfilcoTrip).Hustling)
                {
                    return true;
                }
                return false;
            }
        }

        [NonPersistent]
        public bool Mpp
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip) && (_TripID as StanfilcoTrip).Mpp)
                {
                    return true;
                }
                return false;
            }
        }

        [NonPersistent]
        public bool Long
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip) && (_TripID as StanfilcoTrip).LongHaul)
                {
                    return true;
                }
                return false;
            }
        }

        // Hub
        [NonPersistent]
        public bool Hub
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip) && (_TripID as DolefilTrip).Hub)
                {
                    return true;
                }
                return false;
            }
        }
        // TADI
        [NonPersistent]
        [DisplayName("TADI")]
        public bool Tadi
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip) && (_TripID as DolefilTrip).Duvo)
                {
                    return true;
                }
                return false;
            }
        }
        // Cagayan Trips
        [NonPersistent]
        public bool CagayanTrip
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip) && (_TripID as DolefilTrip).CagayanTrip)
                {
                    return true;
                }
                return false;
            }
        }
        //[NonPersistent]
        //public string SortingSequence
        //{
        //    get
        //    {
        //        string _seq = No;
        //        //var sortedQuery = from FATruck a in e.PopupWindow.View.SelectedObjects orderby int.Parse(a.No.Substring(3).PadLeft(4,'0')) select a;
        //        try
        //        {
        //            switch (_FixedAssetClass)
        //            {
        //                case FixedAssetClassEnum.LandAndBuilding:
        //                    break;
        //                case FixedAssetClassEnum.Truck:
        //                    _seq = string.Format("48-{0}", No.Substring(3).PadLeft(4, '0'));
        //                    break;
        //                case FixedAssetClassEnum.Trailer:
        //                    _seq = string.Format("TRLR {0}", No.Substring(3).PadLeft(6, '0'));
        //                    break;
        //                case FixedAssetClassEnum.GeneratorSet:
        //                    _seq = string.Format("18-{0}", No.Substring(3).PadLeft(4, '0'));
        //                    break;
        //                case FixedAssetClassEnum.OtherVehicle:
        //                    break;
        //                case FixedAssetClassEnum.Other:
        //                    break;
        //                default:
        //                    break;
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        return _seq;
        //    }
        //}
        [NonPersistent]
        public string Unit
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                {
                    return string.Format("48-{0}", (_TripID as StanfilcoTrip).TruckNo.No.Substring(3).PadLeft(3, '0'));
                }
                else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                {
                    return string.Format("48-{0}", (_TripID as DolefilTrip).TruckNo.No.Substring(3).PadLeft(3, '0'));
                }
                else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                {
                    return string.Format("48-{0}", (_TripID as OtherTrip).TruckNo.No.Substring(3).PadLeft(3, '0'));
                }
                return string.Empty;
            }
        }

        [NonPersistent]
        public string DriverName
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripID as StanfilcoTrip).Driver != null ? (_TripID as StanfilcoTrip).Driver.Name : string.Empty;
                }
                else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                {
                    return (_TripID as DolefilTrip).Driver != null ? (_TripID as DolefilTrip).Driver.Name : string.Empty;
                }
                else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                {
                    return (_TripID as OtherTrip).Driver != null ? (_TripID as OtherTrip).Driver.Name : string.Empty;
                }
                return string.Empty;
            }
        }
        [NonPersistent]
        public decimal LongHaul
        {
            get
            {
                if (Long)
                {
                    if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        return (_TripID as StanfilcoTrip).TruckerPay;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                    {
                        return (_TripID as DolefilTrip).NetBilling;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                    {
                        return (_TripID as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }
        [NonPersistent]
        public int LongCount
        {
            get
            {
                if (Long)
                {
                    return 1;
                }
                return 0;
            }
        }
        [NonPersistent]
        public int MppCount
        {
            get
            {
                if (Mpp)
                {
                    return 1;
                }
                return 0;
            }
        }
        [NonPersistent]
        public decimal MppHaul
        {
            get
            {
                if (Mpp)
                {
                    if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        return (_TripID as StanfilcoTrip).TruckerPay;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                    {
                        return (_TripID as DolefilTrip).NetBilling;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                    {
                        return (_TripID as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }
        [NonPersistent]
        public decimal ShortHaul
        {
            get
            {
                if (Hustling)
                {
                    if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        return (_TripID as StanfilcoTrip).TruckerPay;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                    {
                        return (_TripID as DolefilTrip).NetBilling;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                    {
                        return (_TripID as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }
        [NonPersistent]
        public int ShortCount
        {
            get
            {
                if (Hustling)
                {
                    return 1;
                }
                return 0;
            }
        }

        // Hub
        [NonPersistent]
        public decimal HubHaul
        {
            get
            {
                if (Hub)
                {
                    if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        return (_TripID as StanfilcoTrip).TruckerPay;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                    {
                        return (_TripID as DolefilTrip).NetBilling;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                    {
                        return (_TripID as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }
        [NonPersistent]
        public int HubCount
        {
            get
            {
                if (Hub)
                {
                    return 1;
                }
                return 0;
            }
        }
        // Tadi
        [NonPersistent]
        public decimal TadiHaul
        {
            get
            {
                if (Tadi)
                {
                    if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        return (_TripID as StanfilcoTrip).TruckerPay;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                    {
                        return (_TripID as DolefilTrip).NetBilling;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                    {
                        return (_TripID as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }
        [NonPersistent]
        public int TadiCount
        {
            get
            {
                if (Tadi)
                {
                    return 1;
                }
                return 0;
            }
        }
        // Cagayan Trip
        [NonPersistent]
        public decimal CagayanHaul
        {
            get
            {
                if (CagayanTrip)
                {
                    if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        return (_TripID as StanfilcoTrip).TruckerPay;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                    {
                        return (_TripID as DolefilTrip).NetBilling;
                    }
                    else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                    {
                        return (_TripID as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }
        [NonPersistent]
        public int CagayanCount
        {
            get
            {
                if (CagayanTrip)
                {
                    return 1;
                }
                return 0;
            }
        }
        [NonPersistent]
        public decimal Income
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripID as StanfilcoTrip).TruckerPay;
                }
                else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                {
                    return (_TripID as DolefilTrip).NetBilling;
                }
                else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                {
                    return (_TripID as OtherTrip).TruckerPay;
                }
                return 0m;
            }
        }
        // 1707071201 End..

        [Action(Caption = "Register Odo", AutoCommit = true)]
        public void RegisterOdo()
        {
            #region New Trip Odo Logging
            if (_TruckNo != null)
            {
                TripOdoRegistry newLog = null;
                //thisReceipt
                if (this.TruckRegistryOdoLogs.Count > 0)
                {
                    newLog = this.TruckRegistryOdoLogs.FirstOrDefault();
                }
                else
                {
                    newLog = ReflectionHelper.CreateObject<TripOdoRegistry>(this.Session);
                }
                newLog.TruckRegistryId = this;
                newLog.Fleet = _TruckNo;
                newLog.EntryDate = this.TripID.EntryDate;
                newLog.LogType = MeterRegistryTypeEnum.Log;
                newLog.Reading = this.PreOdoRead;
                newLog.Save();
                Session.CommitTransaction();
            }
            #endregion
        }
        protected override void OnDeleting()
        {
            TripOdoRegistry odoToDel = Session.FindObject<TripOdoRegistry>(CriteriaOperator.Parse("[TruckRegistryId]=?", this.Oid));
            if (odoToDel != null)
            {
                odoToDel.Delete();
            }
        }
        protected override void OnSaving() {
            #region New Trip Odo Logging
            if (_TruckNo != null && !IsDeleted)
            {
                TripOdoRegistry newLog = null;
                //thisReceipt
                if (this.TruckRegistryOdoLogs.Count > 0)
                {
                    newLog = this.TruckRegistryOdoLogs.FirstOrDefault();
                }
                else
                {
                    newLog = ReflectionHelper.CreateObject<TripOdoRegistry>(this.Session);
                }
                newLog.TruckRegistryId = this;
                newLog.Fleet = _TruckNo;
                newLog.EntryDate = this.TripID.EntryDate;
                newLog.LogType = MeterRegistryTypeEnum.Log;
                newLog.Reading = this.PreOdoRead;
                newLog.Save();
            }
            #endregion
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
                _Month = _TripID.EntryDate.Month == 1 ? MonthsEnum.January : _TripID.EntryDate.Month
                 == 2 ? MonthsEnum.February : _TripID.EntryDate.Month == 3 ? MonthsEnum.
                March : _TripID.EntryDate.Month == 4 ? MonthsEnum.April : _TripID.EntryDate.Month ==
                5 ? MonthsEnum.May : _TripID.EntryDate.Month == 6 ? MonthsEnum.June :
                _TripID.EntryDate.Month == 7 ? MonthsEnum.July : _TripID.EntryDate.Month == 8 ?
                MonthsEnum.August : _TripID.EntryDate.Month == 9 ? MonthsEnum.September
                 : _TripID.EntryDate.Month == 10 ? MonthsEnum.October : _TripID.EntryDate.Month == 11
                 ? MonthsEnum.November : _TripID.EntryDate.Month == 12 ? MonthsEnum.
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
            get
            {
                return _TripID.EntryDate.Year;
                ;
            }
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


    }
}
