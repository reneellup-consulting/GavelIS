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
    public class InvoiceReconciliation : XPObject {
        private DateTime _EntryDate = DateTime.Now;
        private Customer _Customer;
        [Persistent("SelectedPayments")]
        private decimal? _SelectedPayments;
        [Persistent("DiscountAllowed")]
        private decimal? _DiscountAllowed;
        [Persistent("SelectedCharges")]
        private decimal? _SelectedCharges;
        [Persistent("FinCharges")]
        private decimal? _FinCharges;
        [Persistent("TaxCharges")]
        private decimal? _TaxCharges;
        [Persistent("TaxPayments")]
        private decimal? _TaxPayments;
        [Persistent("Withheld")]
        private decimal? _Withheld;
        //private decimal _Balance;
        private decimal _RemainingTax;
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        [ImmediatePostData]
        public Customer Customer {
            get { return _Customer; }
            set { SetPropertyValue("Customer", ref _Customer, value); }
        }

        [PersistentAlias(
        "((SelectedPayments + DiscountAllowed + Withheld) - SelectedCharges) - FinCharges")
        ]
        [Custom("DisplayFormat", "n")]
        public decimal Balance {
            get
            {
                object tempObject = EvaluateAlias("Balance");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("TaxCharges - TaxPayments")]
        [Custom("DisplayFormat", "n")]
        public decimal RemainingTax {
            get
            {
                object tempObject = EvaluateAlias("RemainingTax");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        #region Calculated Details

        [PersistentAlias("_SelectedPayments")]
        [Custom("DisplayFormat", "n")]
        public decimal? SelectedPayments {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _SelectedPayments == null)
                    {
                        UpdateSelectedPayments(false);
                    }
                } catch (Exception)
                {
                }
                return _SelectedPayments;
            }
        }

        [PersistentAlias("_DiscountAllowed")]
        [DisplayName("(+)Discount Allowed")]
        [Custom("DisplayFormat", "n")]
        public decimal? DiscountAllowed {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _DiscountAllowed == null)
                    {
                        UpdateDiscountAllowed(false);
                    }
                } catch (Exception)
                {
                }
                return _DiscountAllowed;
            }
        }

        [PersistentAlias("_SelectedCharges")]
        [DisplayName("(-)Selected Charges")]
        [Custom("DisplayFormat", "n")]
        public decimal? SelectedCharges {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _SelectedCharges == null)
                    {
                        UpdateSelectedCharges(false);
                    }
                } catch (Exception)
                {
                }
                return _SelectedCharges;
            }
        }

        [PersistentAlias("_FinCharges")]
        [DisplayName("(-)Fin Charges")]
        [Custom("DisplayFormat", "n")]
        public decimal? FinCharges {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _FinCharges == null)
                    {
                        UpdateFinCharges(false);
                    }
                } catch (Exception)
                {
                }
                return _FinCharges;
            }
        }

        [PersistentAlias("_TaxCharges")]
        [Custom("DisplayFormat", "n")]
        public decimal? TaxCharges {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TaxCharges == null)
                    {
                        UpdateTaxCharges(false);
                    }
                } catch (Exception)
                {
                }
                return _TaxCharges;
            }
        }

        [PersistentAlias("_TaxPayments")]
        [Custom("DisplayFormat", "n")]
        public decimal? TaxPayments {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TaxPayments == null)
                    {
                        UpdateTaxPayments(false);
                    }
                } catch (Exception)
                {
                }
                return _TaxPayments;
            }
        }

        [PersistentAlias("_Withheld")]
        [Custom("DisplayFormat", "n")]
        public decimal? Withheld {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Withheld == null)
                    {
                        UpdateWithheld(false);
                    }
                } catch (Exception)
                {
                }
                return _Withheld;
            }
        }


        public void UpdateWithheld(bool forceChangeEvent)
        {
            decimal? oldSelectedWithheld = _Withheld;
            decimal tempTotal = 0m;
            foreach (InvoiceReconPayment detail in Payments)
            {
                tempTotal +=
                detail.Select ? detail.Withheld : 0;
            }
            _Withheld = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Withheld",
                oldSelectedWithheld, _Withheld);
            }
            ;
        }

        public void UpdateSelectedPayments(bool forceChangeEvent) {
            decimal? oldSelectedPayments = _SelectedPayments;
            decimal tempTotal = 0m;
            foreach (InvoiceReconPayment detail in Payments)
            {
                tempTotal +=
                detail.Select ? detail.AdjustNow : 0;
            }
            _SelectedPayments = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("SelectedPayments",
                oldSelectedPayments, _SelectedPayments);
            }
            ;
        }

        public void UpdateDiscountAllowed(bool forceChangeEvent) {
            decimal? oldDiscountAllowed = _DiscountAllowed;
            decimal tempTotal = 0m;
            foreach (InvoiceReconCharge detail in Charges)
            {
                tempTotal += detail.
                Pay ? detail.Discount : 0;
            }
            _DiscountAllowed = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("DiscountAllowed", oldDiscountAllowed,
                _DiscountAllowed);
            }
            ;
        }

        public void UpdateSelectedCharges(bool forceChangeEvent) {
            decimal? oldSelectedCharges = _SelectedCharges;
            decimal tempTotal = 0m;
            foreach (InvoiceReconCharge detail in Charges)
            {
                tempTotal += detail.
                Pay ? detail.Adjust : 0;
            }
            _SelectedCharges = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("SelectedCharges", oldSelectedCharges,
                _SelectedCharges);
            }
            ;
        }

        public void UpdateFinCharges(bool forceChangeEvent) {
            decimal? oldFinCharges = _FinCharges;
            decimal tempTotal = 0m;
            foreach (InvoiceReconCharge detail in Charges)
            {
                tempTotal += detail.
                Pay ? detail.FinanceCharge : 0;
            }
            _FinCharges = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("FinCharges", oldFinCharges,
                _FinCharges);
            }
            ;
        }

        public void UpdateTaxCharges(bool forceChangeEvent) {
            decimal? oldTaxCharges = _TaxCharges;
            decimal tempTotal = 0m;
            foreach (InvoiceReconCharge detail in Charges)
            {
                tempTotal += detail.
                Pay ? detail.Tax : 0;
            }
            _TaxCharges = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TaxCharges", oldTaxCharges,
                _TaxCharges);
            }
            ;
        }

        public void UpdateTaxPayments(bool forceChangeEvent) {
            decimal? oldTaxPayments = _TaxPayments;
            decimal tempTotal = 0m;
            foreach (InvoiceReconPayment detail in Payments)
            {
                tempTotal +=
                detail.Select ? detail.TaxPayment : 0;
            }
            _TaxPayments = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TaxPayments", oldTaxPayments,
                _TaxPayments);
            }
            ;
        }

        #endregion

        [Aggregated,
        Association("InvoiceRecon-Charges")]
        public XPCollection<InvoiceReconCharge> Charges {
            get { return
                GetCollection<InvoiceReconCharge>("Charges"); }
        }

        [Aggregated,
        Association("InvoiceRecon-Payments")]
        public XPCollection<InvoiceReconPayment> Payments {
            get { return
                GetCollection<InvoiceReconPayment>("Payments"); }
        }

        public InvoiceReconciliation(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _SelectedPayments = null;
            _DiscountAllowed = null;
            _SelectedCharges = null;
            _FinCharges = null;
            _TaxCharges = null;
            _TaxPayments = null;
            _Withheld = null;
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
