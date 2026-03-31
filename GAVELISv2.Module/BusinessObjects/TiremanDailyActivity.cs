using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [RuleCriteria("", DefaultContexts.Save, "[Finished] > [Started]")]
    public class TiremanDailyActivity : XPObject {
        private TiremanDaily _TmDailyId;
        private FixedAsset _Fleet;
        private DateTime _Reported;
        private DateTime _Started;
        private DateTime _Finished;
        private TimeSpan _Hours;
        private decimal _OdoReading;
        private Tire _DisTire;
        private TireMake _DisMake;
        private TireSize _DisSize;
        private decimal _DisPlyRating;
        private string _DisSerial;
        private string _DisBranding;
        private WheelPosition _DisWheelPos;
        private TreadStatus _DisTreadStatus;
        private TireDettachReason _DisCauseOfRemoval;
        private Tire _MntTire;
        private TireMake _MntMake;
        private TireSize _MntSize;
        private decimal _MntPlyRating;
        private string _MntSerial;
        private string _MntBranding;
        private WheelPosition _MntWheelPos;
        private TreadStatus _MntTreadStatus;
        private TiremanActivity _TiremanActivityId;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TiremanDailyActivities")]
        [DisplayName("TRD ID")]
        [Custom("AllowEdit", "False")]
        public TiremanDaily TmDailyId {
            get { return _TmDailyId; }
            set
            {
                SetPropertyValue<TiremanDaily>("TmDailyId", ref _TmDailyId, value);
                if (!IsLoading && !IsSaving && _TmDailyId != null)
                {
                    Reported = _TmDailyId.EntryDate;
                    Started = _TmDailyId.EntryDate;
                    Finished = _TmDailyId.EntryDate;
                }
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string EntryDate {
            get { return _TmDailyId != null ? _TmDailyId.EntryDate.ToShortDateString() : string.Empty; }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [DataSourceCriteria("[FixedAssetClass] In ('Truck', 'Trailer', 'OtherVehicle')")]
        public FixedAsset Fleet {
            get { return _Fleet; }
            set { SetPropertyValue<FixedAsset>("Fleet", ref _Fleet, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "hh:mm:ss tt")]
        [Custom("EditMask", "hh:mm:ss tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime Reported {
            get { return _Reported; }
            set { SetPropertyValue<DateTime>("Reported", ref _Reported, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "hh:mm:ss tt")]
        [Custom("EditMask", "hh:mm:ss tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime Started {
            get { return _Started; }
            set { SetPropertyValue<DateTime>("Started", ref _Started, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "hh:mm:ss tt")]
        [Custom("EditMask", "hh:mm:ss tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime Finished {
            get { return _Finished; }
            set { SetPropertyValue<DateTime>("Finished", ref _Finished, value); }
        }

        [PersistentAlias("Finished - Started")]
        public TimeSpan Hours {
            get
            {
                var tempObject = EvaluateAlias("Hours");
                if (tempObject != null)
                {
                    return (TimeSpan)tempObject;
                } else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoReading {
            get { return _OdoReading; }
            set { SetPropertyValue<decimal>("OdoReading", ref _OdoReading, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Tire DisTire {
            get { return _DisTire; }
            set
            {
                SetPropertyValue<Tire>("DisTire", ref _DisTire, value);
                if (!IsLoading && !IsSaving && _DisTire != null)
                {
                    DisMake = _DisTire.TireItem.Make;
                    DisSize = _DisTire.TireItem.Size;
                    DisSerial = _DisTire.SerialNo;
                    DisBranding = _DisTire.TireNo;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public TireMake DisMake {
            get { return _DisMake; }
            set { SetPropertyValue<TireMake>("DisMake", ref _DisMake, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public TireSize DisSize {
            get { return _DisSize; }
            set { SetPropertyValue<TireSize>("DisSize", ref _DisSize, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal DisPlyRating {
            get { return _DisPlyRating; }
            set { SetPropertyValue<decimal>("DisPlyRating", ref _DisPlyRating, value); }
        }

        [Custom("AllowEdit", "False")]
        public string DisSerial {
            get { return _DisSerial; }
            set { SetPropertyValue<string>("DisSerial", ref _DisSerial, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string DisBranding {
            get { return _DisBranding; }
            set { SetPropertyValue<string>("DisBranding", ref _DisBranding, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public WheelPosition DisWheelPos {
            get { return _DisWheelPos; }
            set { SetPropertyValue<WheelPosition>("DisWheelPos", ref _DisWheelPos, value); }
        }

        public TreadStatus DisTreadStatus {
            get { return _DisTreadStatus; }
            set { SetPropertyValue<TreadStatus>("DisTreadStatus", ref _DisTreadStatus, value); }
        }

        public TireDettachReason DisCauseOfRemoval {
            get { return _DisCauseOfRemoval; }
            set { SetPropertyValue<TireDettachReason>("DisCauseOfRemoval", ref _DisCauseOfRemoval, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Tire MntTire {
            get { return _MntTire; }
            set
            {
                SetPropertyValue<Tire>("MntTire", ref _MntTire, value);
                if (!IsLoading && !IsSaving && _MntTire != null)
                {
                    MntMake = _MntTire.TireItem.Make;
                    MntSize = _MntTire.TireItem.Size;
                    MntSerial = _MntTire.SerialNo;
                    MntBranding = _MntTire.TireNo;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public TireMake MntMake {
            get { return _MntMake; }
            set { SetPropertyValue<TireMake>("MntMake", ref _MntMake, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public TireSize MntSize {
            get { return _MntSize; }
            set { SetPropertyValue<TireSize>("MntSize", ref _MntSize, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal MntPlyRating {
            get { return _MntPlyRating; }
            set { SetPropertyValue<decimal>("MntPlyRating", ref _MntPlyRating, value); }
        }

        [Custom("AllowEdit", "False")]
        public string MntSerial {
            get { return _MntSerial; }
            set { SetPropertyValue<string>("MntSerial", ref _MntSerial, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public string MntBranding {
            get { return _MntBranding; }
            set { SetPropertyValue<string>("MntBranding", ref _MntBranding, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public WheelPosition MntWheelPos {
            get { return _MntWheelPos; }
            set { SetPropertyValue<WheelPosition>("MntWheelPos", ref _MntWheelPos, value); }
        }

        public TreadStatus MntTreadStatus {
            get { return _MntTreadStatus; }
            set { SetPropertyValue<TreadStatus>("MntTreadStatus", ref _MntTreadStatus, value); }
        }

        public TiremanActivity TiremanActivityId {
            get { return _TiremanActivityId; }
            set { SetPropertyValue<TiremanActivity>("TiremanActivityId", ref _TiremanActivityId, value); }
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

        public TiremanDailyActivity(Session session)
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

            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
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

            if (SecuritySystem.CurrentUser != null)
            {
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

    }

}
