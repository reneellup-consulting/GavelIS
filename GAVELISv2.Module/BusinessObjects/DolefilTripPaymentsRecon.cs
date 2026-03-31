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

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class DolefilTripPaymentsRecon : XPObject
    {
        #region Fields
        private Customer _TripCustomer;
        private DateTime _EntryDate = DateTime.Now;
        private DateTime _DocumentDate;
        private string _Description;
        private bool _Reconciled = false;
        private decimal _VatRate;
        private decimal _WhtRate;

        // TripCustomer
        public Customer TripCustomer
        {
            get { return _TripCustomer; }
            set { SetPropertyValue("TripCustomer", ref _TripCustomer, value);
            if (!IsLoading && !IsLoading && _TripCustomer != null)
            {
                if (_TripCustomer.TaxCode != null)
                {
                    VatRate = _TripCustomer.TaxCode.Rate;
                }
                if (_TripCustomer.WHTGroupCode != null)
                {
                    WhtRate = _TripCustomer.WHTGroupCode.WHTRate;
                }
            }
            }
        }
        // VatRate
        [Custom("DisplayFormat", "n")]
        public decimal VatRate
        {
            get { return _VatRate; }
            set { SetPropertyValue("VatRate", ref _VatRate, value); }
        }
        // WhtRate
        [Custom("DisplayFormat", "n")]
        public decimal WhtRate
        {
            get { return _WhtRate; }
            set { SetPropertyValue("WhtRate", ref _WhtRate, value); }
        }
        // EntryNo
        public string EntryNo
        {
            get
            {
                return Oid > 0 ? String.Format("ID:{0:D6}"
                    , Oid) : String.Empty;
            }
        }
        // EntryDate
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Doc Date")]
        public DateTime DocumentDate
        {
            get { return _DocumentDate; }
            set { SetPropertyValue("DocumentDate", ref _DocumentDate, value); }
        }
        [Size(200)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        // Reconciled
        [Custom("AllowEdit", "False")]
        public bool Reconciled
        {
            get { return _Reconciled; }
            set { SetPropertyValue("Reconciled", ref _Reconciled, value); }
        }
        #endregion

        #region Collections
        [Aggregated,
        Association("DolefilTripPaymentsRecon-Trips")]
        public XPCollection<DtpReconDetail> Trips
        {
            get
            {
                return
                    GetCollection<DtpReconDetail>("Trips");
            }
        }
        #endregion

        #region Calculated Details

        [Persistent("Underpayment")]
        private decimal? _Underpayment;
        [Persistent("_Underpayment")]
        public decimal? Underpayment
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Underpayment == null)
                    {
                        UpdateUnderpayment(false);
                    }
                }
                catch (Exception)
                {
                }
                return _Underpayment;
            }
        }
        public void UpdateUnderpayment(bool forceChangeEvent)
        {
            decimal? oldUnderpayment = _Underpayment;
            decimal tempUnderpayment = 0m;
            foreach (DtpReconDetail detail in Trips)
            {
                if (detail.Adjustment<0)
                {
                    tempUnderpayment += Math.Abs(detail.Adjustment);
                }
            }
            _Underpayment = tempUnderpayment;
            if (forceChangeEvent)
            {
                OnChanged("Total", Underpayment, _Underpayment);
            }
        }

        [Persistent("Overpayment")]
        private decimal? _Overpayment;
        [Persistent("_Overpayment")]
        public decimal? Overpayment
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Overpayment == null)
                    {
                        UpdateOverpayment(false);
                    }
                }
                catch (Exception)
                {
                }
                return _Overpayment;
            }
        }
        public void UpdateOverpayment(bool forceChangeEvent)
        {
            decimal? oldOverpayment = _Overpayment;
            decimal tempOverpayment = 0m;
            foreach (DtpReconDetail detail in Trips)
            {
                if (detail.Adjustment > 0)
                {
                    tempOverpayment += Math.Abs(detail.Adjustment);
                }
            }
            _Overpayment = tempOverpayment;
            if (forceChangeEvent)
            {
                OnChanged("Total", Overpayment, _Overpayment);
            }
        }

        [Persistent("Unpaid")]
        private decimal? _Unpaid;
        [Persistent("_Unpaid")]
        public decimal? Unpaid
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Unpaid == null)
                    {
                        UpdateUnpaid(false);
                    }
                }
                catch (Exception)
                {
                }
                return _Unpaid;
            }
        }
        public void UpdateUnpaid(bool forceChangeEvent)
        {
            decimal? oldUnpaid = _Unpaid;
            decimal tempUnpaid = 0m;
            foreach (DtpReconDetail detail in Trips)
            {
                if (detail.AmountPaid==0)
                {
                    tempUnpaid += detail.Amount;
                }
            }
            _Unpaid = tempUnpaid;
            if (forceChangeEvent)
            {
                OnChanged("Total", Unpaid, _Unpaid);
            }
        }

        [Persistent("Total")]
        private decimal? _Total;
        [Persistent("_Total")]
        public decimal? Total
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Total == null)
                    {
                        UpdateTotal(false);
                    }
                }
                catch (Exception)
                {
                }
                return _Total;
            }
        }
        public void UpdateTotal(bool forceChangeEvent)
        {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (DtpReconDetail detail in Trips)
            {
                tempTotal += detail.AmountPaid;
            }
            _Total = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Total", Total, _Total);
            }
        }

        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }

        private void Reset()
        {
            _Underpayment = null;
            _Overpayment = null;
            _Unpaid = null;
            _Total = null;
        }

        #endregion

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Total * VatRate/100")]
        public decimal VatAmount
        {
            get
            {
                object tempObject = EvaluateAlias("VatAmount");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Total + VatAmount")]
        public decimal GrossBilling
        {
            get
            {
                object tempObject = EvaluateAlias("GrossBilling");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Total * WhtRate/100")]
        public decimal Withheld
        {
            get
            {
                object tempObject = EvaluateAlias("Withheld");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("GrossBilling - Withheld")]
        public decimal NetBilling
        {
            get
            {
                object tempObject = EvaluateAlias("NetBilling");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        #region Captions

        // NOTE:
        [EditorAlias("LabelControlEditor")]
        public string NoteCaption { 
            get { 
                return "NOTE:"; 
            } 
        }
        // UNDERPAYMENT
        [EditorAlias("LabelControlEditor")]
        public string UnderpaymentCaption
        {
            get
            {
                return "UNDERPAYMENT:";
            }
        }
        // Underpayment Value
        [EditorAlias("LabelDecControlEditor")]
        public string UnderpaymentValue
        {
            get
            {
                if (Underpayment != null)
                {
                    return Underpayment.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // OVERPAYMENT
        [EditorAlias("LabelControlEditor")]
        public string OverpaymentCaption
        {
            get
            {
                return "OVERPAYMENT:";
            }
        }
        // Overpayment Value
        [EditorAlias("LabelDecControlEditor")]
        public string OverpaymentValue
        {
            get
            {
                if (Overpayment != null)
                {
                    return Overpayment.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // UNPAID
        [EditorAlias("LabelControlEditor")]
        public string UnpaidCaption
        {
            get
            {
                return "UNPAID:";
            }
        }
        // Unpaid Value
        [EditorAlias("LabelDecControlEditor")]
        public string UnpaidValue
        {
            get
            {
                if (Unpaid != null)
                {
                    return Unpaid.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // TOTAL
        [EditorAlias("LabelControlEditor")]
        public string TotalCaption
        {
            get
            {
                return "TOTAL:";
            }
        }
        // PHP
        [EditorAlias("LabelControlEditor")]
        public string PhpCaption
        {
            get
            {
                return "PHP:";
            }
        }
        // TotalValue
        [EditorAlias("LabelDecControlEditor")]
        public string TotalValue
        {
            get
            {
                if (Total != null)
                {
                    return Total.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // VATRATE
        [EditorAlias("LabelControlEditor")]
        public string VatRateCaption
        {
            get
            {
                return "ADD:   " + _VatRate.ToString("n2") + "% VAT:";
            }
        }
        // VAT Value
        [EditorAlias("LabelDecControlEditor")]
        public string VatValue
        {
            get
            {
                return VatAmount.ToString("n2");
            }
        }
        // GROSS BILLING
        [EditorAlias("LabelControlEditor")]
        public string GrossBillingCaption
        {
            get
            {
                return "GROSS BILLING:";
            }
        }
        // Gross Billing Value
        [EditorAlias("LabelDecControlEditor")]
        public string GrossBillingValue
        {
            get
            {
                return GrossBilling.ToString("n2");
            }
        }
        // WHTRATE
        [EditorAlias("LabelControlEditor")]
        public string WhtRateCaption
        {
            get
            {
                return "LESS:   " + _WhtRate.ToString("n2") + "% WITHHELD:";
            }
        }
        // WHT Value
        [EditorAlias("LabelDecControlEditor")]
        public string WhtValue
        {
            get
            {
                return Withheld.ToString("n2");
            }
        }
        // NET BILLING
        [EditorAlias("LabelControlEditor")]
        public string NetBillingCaption
        {
            get
            {
                return "NET BILLING:";
            }
        }
        // Net Billing Value
        [EditorAlias("LabelDecControlEditor")]
        public string NetBillingValue
        {
            get
            {
                return NetBilling.ToString("n2");
            }
        }
        #endregion

        #region Records Creation
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
        #endregion
        public DolefilTripPaymentsRecon(Session session)
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
            Customer tmp = Session.FindObject<Customer>(CriteriaOperator.Parse(
            "Contains([Name], 'DOLE PHILIPPINES TRIPS')"));
            this.TripCustomer = tmp != null ? tmp : null;
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
        protected override void OnSaving()
        {
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
