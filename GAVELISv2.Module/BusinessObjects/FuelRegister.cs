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
    public class FuelRegister : XPObject {
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        private string _ReferenceNo;
        private DateTime _Date = DateTime.Now;
        private int _SourceID;
        private SourceType _SourceType;
        private string _SourceNo;
        private TruckOrGensetEnum _TruckOrGenset;
        private FATruck _TruckNo;
        private FAGeneratorSet _GensetNo;
        private Employee _Driver;
        private decimal _Total;
        private ReceiptFuelDetail _ReceiptFuelDetailID;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-FuelRegistrations")]
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
        [Custom("AllowEdit", "False")]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
        public int SourceID {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Qty
        {
            get { return _Qty; }
            set { SetPropertyValue("Qty", ref _Qty, value); }
        }
        [Custom("AllowEdit", "False")]
        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }
        [Custom("AllowEdit", "False")]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public TruckOrGensetEnum TruckOrGenset {
            get { return _TruckOrGenset; }
            set { SetPropertyValue("TruckOrGenset", ref _TruckOrGenset, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        public FATruck TruckNo {
            get { return _TruckNo; }
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public FAGeneratorSet GensetNo {
            get { return _GensetNo; }
            set { SetPropertyValue("GensetNo", ref _GensetNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get { return _Total; }
            set { SetPropertyValue("Total", ref _Total, value); }
        }
        [Custom("AllowEdit", "False")]
        public ReceiptFuelDetail ReceiptFuelDetailID {
            get { return _ReceiptFuelDetailID; }
            set { SetPropertyValue("ReceiptFuelDetailID", ref 
                _ReceiptFuelDetailID, value); }
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
        public FuelRegister(Session session): base(session) {
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
        protected override void OnDeleting()
        {
            base.OnDeleting();
            if (_ReceiptFuelDetailID != null && _ReceiptFuelDetailID.ReceiptInfo.TripUsed != null)
            {
                _ReceiptFuelDetailID.ReceiptInfo.TripUsed = null;
                _ReceiptFuelDetailID.ReceiptInfo.Save();
            }
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _Qty;
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
