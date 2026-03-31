using System;
using System.Linq;
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
using System.IO;
using System.Collections.Generic;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class FuelStatementOfAccount : XPObject
    {
        private DateTime _EntryDate = DateTime.Now;
        private string _StatementNo;
        private DateTime _StatementDate = DateTime.Now;
        private BusinessObjects.TripType _TripType;
        private BusinessObjects.Vendor _Vendor;
        private BusinessObjects.Customer _Customer;
        private bool _Reconciled = false;
        public string EntryNo
        {
            get
            {
                return Oid > 0 ? String.Format("ID:{0:D6}"
                    , Oid) : String.Empty;
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // TripType
        public TripType TripType
        {
            get { return _TripType; }
            set { SetPropertyValue("TripType", ref _TripType, value); }
        }
        // Vendor
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor
        {
            get { return _Vendor; }
            set { SetPropertyValue("Vendor", ref _Vendor, value); }
        }
        // Customer
        [RuleRequiredField("", DefaultContexts.Save)]
        public Customer Customer
        {
            get { return _Customer; }
            set { SetPropertyValue("Customer", ref _Customer, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string StatementNo
        {
            get { return _StatementNo; }
            set { SetPropertyValue("StatementNo", ref _StatementNo, value); }
        }
        public DateTime StatementDate
        {
            get { return _StatementDate; }
            set { SetPropertyValue("StatementDate", ref _StatementDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PeriodStart
        {
            get { return _PeriodStart; }
            set
            {
                SetPropertyValue("PeriodStart", ref _PeriodStart, value);
                if (!IsLoading && _PeriodStart != DateTime.MinValue)
                {
                    PeriodEnd = _PeriodStart.AddDays(
                            6);
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PeriodEnd
        {
            get { return _PeriodEnd; }
            set { SetPropertyValue("PeriodEnd", ref _PeriodEnd, value); }
        }
        [Aggregated,
        Association("FuelStatementOfAccount-Details")]
        public XPCollection<FuelSoaDetail> FuelStatementOfAccountDetails
        {
            get
            {
                return GetCollection<FuelSoaDetail>("FuelStatementOfAccountDetails"
                );
            }
        }
        // FuelStatementPayments
        [Aggregated,
        Association("FuelStatementOfAccount-FuelStatementPayments")]
        public XPCollection<FuelStatementPayment> FuelStatementPayments
        {
            get
            {
                return GetCollection<FuelStatementPayment>("FuelStatementPayments"
                );
            }
        }
        // FuelStatementAdjustments
        [Aggregated,
        Association("FuelStatementOfAccount-FuelStatementAdjustments")]
        public XPCollection<FuelStatementAdjustment> FuelStatementAdjustments
        {
            get
            {
                return GetCollection<FuelStatementAdjustment>("FuelStatementAdjustments"
                );
            }
        }

        // Total
        [Persistent("Total")]
        private decimal? _Total;
        private decimal _Discount;
        [PersistentAlias("_Total")]
        [Custom("DisplayFormat", "n")]
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
            foreach (FuelSoaDetail detail in FuelStatementOfAccountDetails)
            {
                tempTotal += detail.Adjust;
            }
            _Total = tempTotal;
            if (forceChangeEvent) { OnChanged("Total", Total, _Total); }
            ;
        }

        // Discount
        [Custom("DisplayFormat", "n")]
        public decimal Discount
        {
            get { return _Discount; }
            set { SetPropertyValue("Discount", ref _Discount, value); }
        }
        // AmountOayable
        [DisplayName("Amount Payable")]
        [PersistentAlias("Total - (Discount + Adjustments)")]
        [Custom("DisplayFormat", "n")]
        public decimal AmountOayable
        {
            get
            {
                object tempObject = EvaluateAlias("AmountOayable");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool Reconciled
        {
            get { return _Reconciled; }
            set { SetPropertyValue("Reconciled", ref _Reconciled, value); }
        }
        [Action(AutoCommit = true, Caption = "Reset Sequence", ConfirmationMessage = "Do you really want to reset the number sequence?")]
        public void ResetSequence()
        {
            int n = 1;
            foreach (FuelSoaDetail item in FuelStatementOfAccountDetails.OrderBy(o => o.Seq))
            {
                item.Seq = n++;
                item.Save();
            }
        }

        [Action(Caption = "Print SOA")]
        public void PrintDocument()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\FuelStatementofAccounts.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);

            // Create XPCollection with current object
            XPCollection<FuelStatementOfAccount> collection = new XPCollection<FuelStatementOfAccount>(Session);
            collection.Add(this);
            rep.DataSource = collection;
            rep.FilterString = string.Format("[EntryNo] = '{0}'", this.EntryNo);
            rep.ShowPreview();
        }

        [Action(Caption = "Print Per Type")]
        public void PrintDocument2()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\FuelReconciliationPerType.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);

            // Create XPCollection with current object
            XPCollection<FuelStatementOfAccount> collection = new XPCollection<FuelStatementOfAccount>(Session);
            collection.Add(this);
            rep.DataSource = collection;
            rep.FilterString = string.Format("[EntryNo] = '{0}'", this.EntryNo);
            rep.ShowPreview();
        }

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
        public FuelStatementOfAccount(Session session)
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
            "Contains([Name], 'STANFILCO TRIPS')"));
            this.Customer = tmp != null ? tmp : null;
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
        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }
        private void Reset()
        {
            _SelectedPayments = null;
            _SelectedCharges = null;
            _Total = null;
            _Adjustments = null;
        }
        protected override void OnDeleting()
        {
            if (Reconciled)
            {
                throw new
                    UserFriendlyException(
                    "The system prohibits the deletion of already reconciled statement."
                    );
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
        [Persistent("SelectedPayments")]
        private decimal? _SelectedPayments;
        private decimal? _Adjustments;
        private decimal? _SelectedCharges;
        private DateTime _PeriodStart;
        private DateTime _PeriodEnd;
        [Custom("DisplayFormat", "n")]
        public decimal? SelectedPayments
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _SelectedPayments == null)
                    {
                        UpdateSelectedPayments(false);
                    }
                }
                catch (Exception)
                {
                }
                return _SelectedPayments;
            }
        }
        public void UpdateSelectedPayments(bool forceChangeEvent)
        {
            decimal? oldSelectedPayments = _SelectedPayments;
            decimal tempTotal = 0m;
            foreach (FuelStatementPayment detail in FuelStatementPayments)
            {
                tempTotal += detail.Select ? detail.AdjustNow : 0;
            }
            _SelectedPayments = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("SelectedPayments",
                    SelectedPayments, _SelectedPayments);
            }
            ;
        }
        [PersistentAlias(
        "AmountOayable - SelectedPayments")
        ]
        [Custom("DisplayFormat", "n")]
        public decimal Balance
        {
            get
            {
                object tempObject = EvaluateAlias("Balance");
                if (tempObject != null)
                {
                    return Math.Round((decimal)tempObject,
                        2);
                }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("_Adjustments")]
        [Custom("DisplayFormat", "n")]
        public decimal? Adjustments
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Adjustments == null)
                    {
                        UpdateAdjustments(false);
                    }
                }
                catch (Exception)
                {
                }
                return _Adjustments;
            }
        }
        [PersistentAlias("_SelectedCharges")]
        [DisplayName("(-)Selected Charges")]
        [Custom("DisplayFormat", "n")]
        public decimal? SelectedCharges
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _SelectedCharges == null)
                    {
                        UpdateSelectedCharges(false);
                    }
                }
                catch (Exception)
                {
                }
                return _SelectedCharges;
            }
        }
        public void UpdateAdjustments(bool forceChangeEvent)
        {
            decimal? oldWAdjustments = _Adjustments;
            decimal tempTotal = 0m;
            foreach (FuelStatementAdjustment detail in FuelStatementAdjustments)
            {
                tempTotal += detail.Amount;
            }
            _Adjustments = tempTotal;
            if (forceChangeEvent) { OnChanged("Adjustments", Adjustments, _Adjustments); }
            ;
        }

        public void UpdateSelectedCharges(bool forceChangeEvent)
        {
            decimal? oldSelectedCharges = _SelectedCharges;
            decimal tempTotal = 0m;
            foreach (FuelSoaDetail detail in FuelStatementOfAccountDetails)
            {
                tempTotal += detail.Pay ? detail.Adjust : 0;
            }
            _SelectedCharges = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("SelectedCharges", SelectedCharges,
                    _SelectedCharges);
            }
            ;
        }

        private void GenerateStatementNo()
        {
            if (_PeriodStart != DateTime.MinValue && _PeriodEnd != DateTime.MinValue)
            {
                string startMonth = _PeriodStart.ToString("MMM").ToUpper();
                string endMonth = _PeriodEnd.ToString("MMM").ToUpper();
                string startDay = _PeriodStart.Day.ToString();
                string endDay = _PeriodEnd.Day.ToString();

                if (_PeriodStart.Year != _PeriodEnd.Year)
                {
                    // Different years - show both years
                    _StatementNo = string.Format("{0} {1}, {2} - {3} {4}, {5} (OVERALL)",
                        startMonth, startDay, _PeriodStart.Year,
                        endMonth, endDay, _PeriodEnd.Year);
                }
                else if (_PeriodStart.Month != _PeriodEnd.Month)
                {
                    // Same year, different months
                    _StatementNo = string.Format("{0} {1} - {2} {3}, {4} (OVERALL)",
                        startMonth, startDay, endMonth, endDay, _PeriodStart.Year);
                }
                else
                {
                    // Same month and year
                    _StatementNo = string.Format("{0} {1}-{2}, {3} (OVERALL)",
                        startMonth, startDay, endDay, _PeriodStart.Year);
                }

                OnChanged("StatementNo");
            }
        }

        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            if (!IsLoading && !IsSaving &&
                (propertyName == "PeriodStart" || propertyName == "PeriodEnd" || propertyName == "TripType"))
            {
                GenerateStatementNo();
            }
        }
    }
}
