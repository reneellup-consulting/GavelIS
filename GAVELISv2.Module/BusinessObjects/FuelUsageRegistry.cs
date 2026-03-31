using System;
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
    [NavigationItem(false)]
    public class FuelUsageRegistry : XPObject {
        private Guid _RowID;
        private ReceiptFuel _ReceiptFuelID;
        private DateTime _Date = DateTime.Now;
        private Employee _Driver;
        private Tariff _Tariff;
        private TripLocation _Origin;
        private TripLocation _Destinations;
        private GenJournalHeader _TripNo;
        private string _ReferenceNo;
        private DateTime _StartOut;
        private decimal _StartOdoReading;
        private DateTime _EndIn;
        private decimal _EndOdoReading;
        private decimal _Distance;
        private DateTime _PlugIn;
        private TimeSpan _PlugInReading;
        private DateTime _PlugOff;
        private TimeSpan _PlugOffReading;
        private TimeSpan _TotalHours;
        private decimal _Income;
        private string _Remarks;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("ReceiptFuel-FuelUsageRegistrations")]
        public ReceiptFuel ReceiptFuelID {
            get { return _ReceiptFuelID; }
            set { SetPropertyValue("ReceiptFuelID", ref _ReceiptFuelID, value); 
            }
        }
        // Date cannot be before receipt date
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        public Tariff Tariff {
            get { return _Tariff; }
            set { SetPropertyValue("Tariff", ref _Tariff, value); }
        }
        public TripLocation Origin {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        public TripLocation Destinations {
            get { return _Destinations; }
            set { SetPropertyValue("Destinations", ref _Destinations, value); }
        }
        public GenJournalHeader TripNo {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value); }
        }
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        // Date time cannot be before receipt date time
        public DateTime StartOut {
            get { return _StartOut; }
            set { SetPropertyValue("StartOut", ref _StartOut, value); }
        }
        public decimal StartOdoReading {
            get { return _StartOdoReading; }
            set { SetPropertyValue("StartOdoReading", ref _StartOdoReading, 
                value); }
        }
        // Date time cannot be before receipt date time
        public DateTime EndIn {
            get { return _EndIn; }
            set { SetPropertyValue("EndIn", ref _EndIn, value); }
        }
        public decimal EndOdoReading {
            get { return _EndOdoReading; }
            set { SetPropertyValue("EndOdoReading", ref _EndOdoReading, value); 
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Distance {
            get { return _Distance; }
            set { SetPropertyValue("Distance", ref _Distance, value); }
        }
        // Date time cannot be before receipt date time
        public DateTime PlugIn {
            get { return _PlugIn; }
            set { SetPropertyValue("PlugIn", ref _PlugIn, value); }
        }
        public TimeSpan PlugInReading {
            get { return _PlugInReading; }
            set { SetPropertyValue("PlugInReading", ref _PlugInReading, value); 
            }
        }
        // Date time cannot be before receipt date time
        public DateTime PlugOff {
            get { return _PlugOff; }
            set { SetPropertyValue("PlugOff", ref _PlugOff, value); }
        }
        public TimeSpan PlugOffReading {
            get { return _PlugOffReading; }
            set { SetPropertyValue("PlugOffReading", ref _PlugOffReading, value)
                ; }
        }
        public TimeSpan TotalHours {
            get { return _TotalHours; }
            set { SetPropertyValue("TotalHours", ref _TotalHours, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Income {
            get { return _Income; }
            set { SetPropertyValue("Income", ref _Income, value); }
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
        public FuelUsageRegistry(Session session): base(session) {
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

    }
}
