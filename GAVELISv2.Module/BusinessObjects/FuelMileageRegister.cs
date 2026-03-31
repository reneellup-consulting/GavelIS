using System;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class FuelMileageRegister : XPObject {
        private Guid _RowID;
        private FixedAsset _Fleet;
        private DateTime _EntryDate;
        private Employee _ReportedBy;
        private MeterLogTypeEnum _LogType;
        private MeterEntryTypeEnum _MeterType;
        private string _Reference;
        private string _Reason;
        private bool _Corrected = false;
        private decimal _Reading;
        private decimal _Difference;
        private decimal _Life;
        private decimal _Liters;
        private decimal _Cost;
        //private decimal _Range;
        private decimal _LitersPerKm;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("Fleet-FuelMileageRegister")]
        public FixedAsset Fleet {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }

        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        public Employee ReportedBy {
            get { return _ReportedBy; }
            set { SetPropertyValue("ReportedBy", ref _ReportedBy, value); }
        }

        public MeterLogTypeEnum LogType {
            get { return _LogType; }
            set {
                SetPropertyValue("LogType", ref _LogType, value);
                //if (!IsLoading) {
                //    switch (_LogType) {
                //        case MeterLogTypeEnum.Initial:
                //            Difference = 0;
                //            Life = _Reading;
                //            break;
                //        case MeterLogTypeEnum.Log:
                //            break;
                //        case MeterLogTypeEnum.Fuel:
                //            break;
                //        case MeterLogTypeEnum.Service:
                //            break;
                //        default:
                //            break;
                //    }
                //}
            }
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

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Reading {
            get { return _Reading; }
            set {
                SetPropertyValue("Reading", ref _Reading, value);
                if (!IsLoading && _Fleet != null) {
                    switch (_LogType) {
                        case MeterLogTypeEnum.Initial:
                            Difference = 0;
                            Life = _Reading;
                            _Fleet.LastFuelLifeReading = _Reading;
                            _Fleet.LastOdoFuelReading = _Reading;
                            break;
                        case MeterLogTypeEnum.Log:
                            Difference = _Reading - _Fleet.LastOdoFuelReading;
                            Life = _Fleet.LastFuelLifeReading + _Difference;
                            _Fleet.LastFuelLifeReading = _Life;
                            _Fleet.LastOdoFuelReading = _Reading;
                            break;
                        case MeterLogTypeEnum.Change:
                            Difference = 0;
                            Life = _Fleet.LastFuelLifeReading;
                            _Fleet.LastOdoFuelReading = _Reading;
                            break;
                        case MeterLogTypeEnum.Correct:
                            Difference = 0;
                            Life = _Reading;
                            _Fleet.LastFuelLifeReading = _Reading;
                            break;
                        case MeterLogTypeEnum.Fuel:
                            break;
                        case MeterLogTypeEnum.Service:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        [DisplayName("Range")]
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
            set { SetPropertyValue("Liters", ref _Liters, value); }
        }

        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }

        //public decimal Range
        //{
        //    get { return _Range; }
        //    set { SetPropertyValue("Range", ref _Range, value); }
        //}
        [PersistentAlias("Liters/Difference")]
        [Custom("DisplayFormat", "n")]
        public decimal LitersPerKm {
            get {
                try {
                    var tempObject = EvaluateAlias("LitersPerKm");
                    if (tempObject != null) {return (decimal)tempObject;} else {
                        return 0;
                    }
                } catch(Exception) {
                    return 0;
                }
            }
        }

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

        public FuelMileageRegister(Session session): base(session) {
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
            RowID = Guid.NewGuid();
            if (SecuritySystem.CurrentUser != null) {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }
        }

        protected override void OnSaving() {
            base.OnSaving();
            if (SecuritySystem.CurrentUser != null) {
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
