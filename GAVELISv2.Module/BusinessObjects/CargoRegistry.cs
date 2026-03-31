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
    public class CargoRegistry : XPObject {
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        private string _ReferenceNo;
        private DateTime _Date = DateTime.Now;
        private FATruck _TruckNo;
        private Employee _Driver;
        private DriverClassification _DriverClass;
        private Tariff _Tariff;
        private FATrailer _TrailerNo;
        private FAGeneratorSet _GensetNo;
        private string _ContainerNo;
        private CargoType _Particular;
        private UnitOfMeasure _UOM;
        private decimal _Quantity;
        private string _SealNo;
        private string _Remarks;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-CargoRegistrations")]
        public GenJournalHeader TripID {
            get { return _TripID; }
            set { SetPropertyValue("TripID", ref _TripID, value); }
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
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver {
            get { return _Driver; }
            set {
                SetPropertyValue("Driver", ref _Driver, value);
                if (!IsLoading && _Driver != null) {DriverClass = _Driver.
                    DriverClassification;}
            }
        }
        [Custom("AllowEdit", "False")]
        public DriverClassification DriverClass {
            get { return _DriverClass; }
            set { SetPropertyValue("DriverClass", ref _DriverClass, value); }
        }
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Tariff Tariff {
            get { return _Tariff; }
            set { SetPropertyValue("Tariff", ref _Tariff, value); }
        }
        //[RuleRequiredField("", DefaultContexts.Save)]
        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); }
        }
        public FAGeneratorSet GensetNo {
            get { return _GensetNo; }
            set { SetPropertyValue("GensetNo", ref _GensetNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string ContainerNo {
            get { return _ContainerNo; }
            set { SetPropertyValue("ContainerNo", ref _ContainerNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public CargoType Particular {
            get { return _Particular; }
            set { SetPropertyValue("Particular", ref _Particular, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set { SetPropertyValue("UOM", ref _UOM, value); }
        }
        [NonPersistent]
        public decimal Factor
        {
            get
            {
                if (_Particular != null && _Particular.UOMRelations.Count > 0 && _UOM != null)
                {
                    var data = _Particular.UOMRelations.Where(o => o.UOM == _UOM).FirstOrDefault();
                    if (data != null)
                    {
                        return data.Factor;
                    }
                }
                return 1m;
            }
        }
        [PersistentAlias("Quantity * Factor")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseQTY
        {
            get
            {
                object tempObject = EvaluateAlias("BaseQTY");
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
        public decimal SmallQTY
        {
            get
            {
                if (_Particular != null && _Particular.SmallUOM != null)
                {
                    var data = _Particular.UOMRelations.Where(o => o.UOM == _Particular.SmallUOM).FirstOrDefault();
                    if (data !=null)
                    {
                        return BaseQTY / data.Factor;
                    }
                    else
                    {
                        return BaseQTY;
                    }
                }
                return BaseQTY;
            }
        }
        [NonPersistent]
        public decimal BigQTY
        {
            get
            {
                if (_Particular != null && _Particular.BigUOM != null)
                {
                    var data = _Particular.UOMRelations.Where(o => o.UOM == _Particular.BigUOM).FirstOrDefault();
                    if (data != null)
                    {
                        return BaseQTY / data.Factor;
                    }
                    else
                    {
                        return BaseQTY;
                    }
                }
                return BaseQTY;
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value); }
        }
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string SealNo {
            get { return _SealNo; }
            set { SetPropertyValue("SealNo", ref _SealNo, value); }
        }
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
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
        public CargoRegistry(Session session): base(session) {
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
        protected override void OnSaving() {
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

        [NonPersistent]
        public string UnitSeq
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
        public int NoOfTrip
        {
            get
            {
                return 1;
            }
        }

        [NonPersistent]
        public decimal TruckerPay
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
        [NonPersistent]
        public decimal NetBilling
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripID as StanfilcoTrip).NetBilling;
                }
                else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip))
                {
                    return (_TripID as DolefilTrip).NetBilling;
                }
                else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip))
                {
                    return (_TripID as OtherTrip).NetBilling;
                }
                return 0m;
            }
        }
        [NonPersistent]
        public string Origin
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip) && (_TripID as StanfilcoTrip).Origin != null)
                {
                    return (_TripID as StanfilcoTrip).Origin.ToString();
                }
                else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip) && (_TripID as DolefilTrip).Origin != null)
                {
                    return (_TripID as DolefilTrip).Origin.ToString();
                }
                else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip) && (_TripID as OtherTrip).Origin != null)
                {
                    return (_TripID as OtherTrip).Origin.ToString();
                }
                return string.Empty;
            }
        }
        [NonPersistent]
        public string Destination
        {
            get
            {
                if (_TripID != null && _TripID.GetType() == typeof(StanfilcoTrip) && (_TripID as StanfilcoTrip).Destination != null)
                {
                    return (_TripID as StanfilcoTrip).Destination.ToString();
                }
                else if (_TripID != null && _TripID.GetType() == typeof(DolefilTrip) && (_TripID as DolefilTrip).Destination != null)
                {
                    return (_TripID as DolefilTrip).Destination.ToString();
                }
                else if (_TripID != null && _TripID.GetType() == typeof(OtherTrip) && (_TripID as OtherTrip).Destination != null)
                {
                    return (_TripID as OtherTrip).Destination.ToString();
                }
                return string.Empty;
            }
        }
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
                if (_TripID != null)
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
                else
                {
                    return MonthsEnum.None;
                }
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
                return _TripID != null ? _TripID.EntryDate.Year : 0;
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
