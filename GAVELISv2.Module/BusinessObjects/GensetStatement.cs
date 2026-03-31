using System;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class GensetStatement : XPObject {
        public string EntryNo { get { return Oid > 0 ? String.Format("ID:{0:D6}"
                , Oid) : String.Empty; } }
        // FromDate and ToDate are optional
        // If not specified user must select from GetUnpaidTripEntries
        private DateTime _EntryDate = DateTime.Now;
        private Customer _Customer;
        private int _Period;
        private int _Week;
        private DateTime _FromDate;
        private DateTime _ToDate;
        private bool _Reconciled = false;
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [ImmediatePostData]
        public Customer Customer {
            get { return _Customer; }
            set { SetPropertyValue("Customer", ref _Customer, value); }
        }
        public int Period {
            get { return _Period; }
            set { SetPropertyValue("Period", ref _Period, value); }
        }
        public int Week {
            get { return _Week; }
            set { SetPropertyValue("Week", ref _Week, value); }
        }
        public DateTime FromDate {
            get { return _FromDate; }
            set { SetPropertyValue("FromDate", ref _FromDate, value); }
        }
        public DateTime ToDate {
            get { return _ToDate; }
            set { SetPropertyValue("ToDate", ref _ToDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Reconciled {
            get { return _Reconciled; }
            set { SetPropertyValue("Reconciled", ref _Reconciled, value); }
        }
        #region Balance Computation
        [Persistent("TotalBilling")]
        private decimal? _TotalBilling;
        [Persistent("SelectedPayments")]
        private decimal? _SelectedPayments;
        [Persistent("DiscountAllowed")]
        private decimal? _DiscountAllowed;
        [Persistent("SelectedCharges")]
        private decimal? _SelectedCharges;
        [Persistent("FinCharges")]
        private decimal? _FinCharges;
        [PersistentAlias("_TotalBilling")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalBilling
        {
            get {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalBilling == null)
                    {
                        UpdateTotalBilling(false);
                    }
                }
                catch (Exception)
                {
                }
                return _TotalBilling; }
        }
        
        [PersistentAlias("_SelectedPayments")]
        [Custom("DisplayFormat", "n")]
        public decimal? SelectedPayments {
            get {
                try {
                    if (!IsLoading && !IsSaving && _SelectedPayments == null) {
                        UpdateSelectedPayments(false);}
                } catch (Exception) {
                }
                return _SelectedPayments;
            }
        }
        [PersistentAlias("_DiscountAllowed")]
        [DisplayName("(+)Discount Allowed")]
        [Custom("DisplayFormat", "n")]
        public decimal? DiscountAllowed {
            get {
                try {
                    if (!IsLoading && !IsSaving && _DiscountAllowed == null) {
                        UpdateDiscountAllowed(false);}
                } catch (Exception) {
                }
                return _DiscountAllowed;
            }
        }
        [PersistentAlias("_SelectedCharges")]
        [DisplayName("(-)Selected Charges")]
        [Custom("DisplayFormat", "n")]
        public decimal? SelectedCharges {
            get {
                try {
                    if (!IsLoading && !IsSaving && _SelectedCharges == null) {
                        UpdateSelectedCharges(false);}
                } catch (Exception) {
                }
                return _SelectedCharges;
            }
        }
        [PersistentAlias("_FinCharges")]
        [DisplayName("(-)Fin Charges")]
        [Custom("DisplayFormat", "n")]
        public decimal? FinCharges {
            get {
                try {
                    if (!IsLoading && !IsSaving && _FinCharges == null) {
                        UpdateFinCharges(false);}
                } catch (Exception) {
                }
                return _FinCharges;
            }
        }
        public void UpdateTotalBilling(bool forceChangeEvent)
        {
            decimal? oldTotalBilling = _TotalBilling;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges)
            {
                tempTotal += detail.Billing;
            }
            _TotalBilling = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalBilling",
                    TotalBilling, _TotalBilling);
            }
            ;
        }
        public void UpdateSelectedPayments(bool forceChangeEvent) {
            decimal? oldSelectedPayments = _SelectedPayments;
            decimal tempTotal = 0m;
            foreach (GensetPayment detail in GensetPayments) {tempTotal += 
                detail.Select ? detail.AdjustNow : 0;}
            _SelectedPayments = tempTotal;
            if (forceChangeEvent) {OnChanged("SelectedPayments", 
                SelectedPayments, _SelectedPayments);}
            ;
        }
        public void UpdateDiscountAllowed(bool forceChangeEvent) {
            decimal? oldDiscountAllowed = _DiscountAllowed;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges) {tempTotal += detail.
                Pay ? detail.Discount : 0;}
            _DiscountAllowed = tempTotal;
            if (forceChangeEvent) {OnChanged("DiscountAllowed", DiscountAllowed, 
                _DiscountAllowed);}
            ;
        }
        public void UpdateSelectedCharges(bool forceChangeEvent) {
            decimal? oldSelectedCharges = _SelectedCharges;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges) {tempTotal += detail.
                Pay ? detail.GrossBilling : 0;}
            _SelectedCharges = tempTotal;
            if (forceChangeEvent) {OnChanged("SelectedCharges", SelectedCharges, 
                _SelectedCharges);}
            ;
        }
        public void UpdateFinCharges(bool forceChangeEvent) {
            decimal? oldFinCharges = _FinCharges;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges) {tempTotal += detail.
                Pay ? detail.FinanceCharge : 0;}
            _FinCharges = tempTotal;
            if (forceChangeEvent) {OnChanged("FinCharges", FinCharges, 
                _FinCharges);}
            ;
        }
        [PersistentAlias(
        "((SelectedPayments + DiscountAllowed) - SelectedCharges) - FinCharges")
        ]
        [Custom("DisplayFormat", "n")]
        public decimal Balance {
            get {
                object tempObject = EvaluateAlias("Balance");
                if (tempObject != null) {return Math.Round((decimal)tempObject, 
                    2);} else {
                    return 0;
                }
            }
        }
        #endregion
        #region Billing Computation
        [Persistent("VatTotal")]
        private decimal? _VatTotal;
        [Persistent("GrossBilling")]
        private decimal? _GrossBilling;
        [Persistent("WHTTotal")]
        private decimal? _WHTTotal;
        private decimal _NETTotal;
        [PersistentAlias("_VatTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal? VatTotal
        {
            get {
                try
                {
                    if (!IsLoading && !IsSaving && _VatTotal == null)
                    {
                        UpdateVatTotal(false);
                    }
                }
                catch (Exception)
                {
                }
                return _VatTotal; }
        }
        
        [PersistentAlias("_GrossBilling")]
        [Custom("DisplayFormat", "n")]
        public decimal? GrossBilling {
            get {
                try {
                    if (!IsLoading && !IsSaving && _GrossBilling == null) {
                        UpdateGrossBilling(false);}
                } catch (Exception) {
                }
                return _GrossBilling;
            }
        }
        public void UpdateVatTotal(bool forceChangeEvent)
        {
            decimal? oldVatTotal = _VatTotal;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges)
            {
                tempTotal += detail.VATAmount;
            }
            _VatTotal = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("VatTotal", VatTotal,
                    _VatTotal);
            }
            ;
        }
        public void UpdateGrossBilling(bool forceChangeEvent) {
            decimal? oldGrossBilling = _GrossBilling;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges) {tempTotal += detail.
                GrossBilling;}
            _GrossBilling = tempTotal;
            if (forceChangeEvent) {OnChanged("GrossBilling", GrossBilling, 
                _GrossBilling);}
            ;
        }
        [PersistentAlias("_WHTTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal? WHTTotal {
            get {
                try {
                    if (!IsLoading && !IsSaving && _WHTTotal == null) {
                        UpdateWHTTotal(false);}
                } catch (Exception) {
                }
                return _WHTTotal;
            }
        }
        public void UpdateWHTTotal(bool forceChangeEvent) {
            decimal? oldWHTTotal = _WHTTotal;
            decimal tempTotal = 0m;
            foreach (GensetCharge detail in GensetCharges) {tempTotal += detail.
                WHTAmount;}
            _WHTTotal = tempTotal;
            if (forceChangeEvent) {OnChanged("WHTTotal", WHTTotal, _WHTTotal);}
            ;
        }
        [PersistentAlias("GrossBilling - WHTTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal NETTotal {
            get {
                object tempObject = EvaluateAlias("NETTotal");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        #endregion
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
        [Aggregated,
        Association("GensetStatement-GensetCharges")]
        public XPCollection<GensetCharge> GensetCharges { get { return 
                GetCollection<GensetCharge>("GensetCharges"); } }
        [Aggregated,
        Association("GensetStatement-GensetPayments")]
        public XPCollection<GensetPayment> GensetPayments { get { return 
                GetCollection<GensetPayment>("GensetPayments"); } }
        public GensetStatement(Session session): base(session) {
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
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _VatTotal = null;
            _TotalBilling = null;
            _SelectedPayments = null;
            _DiscountAllowed = null;
            _SelectedCharges = null;
            _FinCharges = null;
            _GrossBilling = null;
            _WHTTotal = null;
        }
        protected override void OnDeleting() { if (Reconciled) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already reconciled statement."
                );} }

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
