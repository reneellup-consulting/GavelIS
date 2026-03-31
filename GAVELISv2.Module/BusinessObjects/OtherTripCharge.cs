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
    public class OtherTripCharge : XPObject {
        private OtherTripStatement _OtherTripStatementID;
        private int _Seq;
        private bool _Pay;
        private SourceType _SourceType;
        private string _SourceNo;
        private int _SourceID;
        private DateTime _Date;
        private FATruck _TruckNo;
        private FATrailer _TrailerNo;
        private string _TripNo;
        private TripLocation _Origin;
        private TripLocation _Destination;
        private decimal _TruckerPay;
        private decimal _TrailerRental;
        private decimal _Billing;
        private decimal _VATAmount;
        private decimal _GrossBilling;
        private decimal _WHTAmount;
        private decimal _NetBilling;
        private decimal _Adjust;
        private Terms _Terms;
        private decimal _Discount;
        private decimal _FinanceCharge;
        //private decimal _Total;
        private decimal _OpenAmount;
        [Association("OtherTripStatement-OtherTripCharges")]
        public OtherTripStatement OtherTripStatementID {
            get { return _OtherTripStatementID; }
            set {
                OtherTripStatement oldDocumentId = _OtherTripStatementID;
                SetPropertyValue("OtherTripStatementID", ref 
                _OtherTripStatementID, value);
                if (!IsLoading && !IsSaving && oldDocumentId != _OtherTripStatementID)
                {
                    oldDocumentId = oldDocumentId ?? _OtherTripStatementID;
                    try
                    {
                        oldDocumentId.UpdateDiscountAllowed(true);
                        oldDocumentId.UpdateFinCharges(true);
                        oldDocumentId.UpdateGrossBilling(true);
                        oldDocumentId.UpdateSelectedCharges(true);
                        oldDocumentId.UpdateSelectedPayments(true);
                        oldDocumentId.UpdateWHTTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        public int Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        
        [ImmediatePostData]
        public bool Pay {
            get { return _Pay; }
            set {
                SetPropertyValue("Pay", ref _Pay, value);
                if (!IsLoading) {
                    try {
                        _OtherTripStatementID.UpdateSelectedCharges(true);
                        _OtherTripStatementID.UpdateDiscountAllowed(true);
                        _OtherTripStatementID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
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
        public int SourceID {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
        public FATruck TruckNo {
            get { return _TruckNo; }
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public string TripNo {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public TripLocation Origin {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        [Custom("AllowEdit", "False")]
        public TripLocation Destination {
            get { return _Destination; }
            set { SetPropertyValue("Destination", ref _Destination, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TruckerPay {
            get { return _TruckerPay; }
            set { SetPropertyValue("TruckerPay", ref _TruckerPay, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TrailerRental {
            get { return _TrailerRental; }
            set { SetPropertyValue("TrailerRental", ref _TrailerRental, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Billing {
            get { return _Billing; }
            set { SetPropertyValue("Billing", ref _Billing, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal VATAmount {
            get { return _VATAmount; }
            set { SetPropertyValue("VATAmount", ref _VATAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal GrossBilling {
            get { return _GrossBilling; }
            set { SetPropertyValue("GrossBilling", ref _GrossBilling, value);
            if (!IsLoading)
            {
                try
                {
                    _OtherTripStatementID.UpdateGrossBilling(true);
                }
                catch (Exception) { }
            }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTAmount {
            get { return _WHTAmount; }
            set { SetPropertyValue("WHTAmount", ref _WHTAmount, value);
            if (!IsLoading)
            {
                try
                {
                    _OtherTripStatementID.UpdateWHTTotal(true);
                }
                catch (Exception) { }
            }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal NetBilling {
            get { return _NetBilling; }
            set { SetPropertyValue("NetBilling", ref _NetBilling, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Adjust {
            get { return _Adjust; }
            set {
                SetPropertyValue("Adjust", ref _Adjust, value);
                if (!IsLoading) {
                    try {
                        _OtherTripStatementID.UpdateSelectedCharges(true);
                        _OtherTripStatementID.UpdateDiscountAllowed(true);
                        _OtherTripStatementID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Discount {
            get { return _Discount; }
            set {
                SetPropertyValue("Discount", ref _Discount, value);
                if (!IsLoading) {
                    try {
                        _OtherTripStatementID.UpdateSelectedCharges(true);
                        _OtherTripStatementID.UpdateDiscountAllowed(true);
                        _OtherTripStatementID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal FinanceCharge {
            get { return _FinanceCharge; }
            set {
                SetPropertyValue("FinanceCharge", ref _FinanceCharge, value);
                if (!IsLoading) {
                    try {
                        _OtherTripStatementID.UpdateSelectedCharges(true);
                        _OtherTripStatementID.UpdateDiscountAllowed(true);
                        _OtherTripStatementID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        [PersistentAlias("(Adjust - Discount) + FinanceCharge")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
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
        public OtherTripCharge(Session session): base(session) {
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
