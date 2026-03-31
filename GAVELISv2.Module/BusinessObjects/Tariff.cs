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
    [System.ComponentModel.DefaultProperty("Code")]
    public class Tariff : BaseObject {
        private const string defaultFromToFormat = 
        "{Origin.Code} -> {Destination.Code}";
        private string _Code;
        private TripZone _Zone;
        private TripLocation _Origin;
        private TripLocation _Destination;
        private decimal _Distance;
        private TimeSpan _TurnAround;
        private decimal _Allowance;
        private decimal _Fuel;
        private decimal _TruckerPay;
        private decimal _RateAdjmt;
        private bool _TrailerRent;
        private decimal _TrailerRental;
        private decimal _Insurance;
        private decimal _Point;
        private bool _TaxInclusive = true;
        private SalesTaxCode _TaxCode;
        private bool _WHTInclusive = true;
        private WHTGroupCode _WHTGroupCode;
        private decimal _GensetRatePerHr;
        private decimal _ShuntingRatePerKm;
        private Account _InsuranceExpenseAcct;
        private decimal _KDKmRun;
        private decimal _KDAmount;
        private decimal _KDFuelSub;
        private decimal _TariffDistance;
        private decimal _TariffTruckerPay;
        private decimal _TariffFuelSubsidy;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }
        public TripZone Zone {
            get { return _Zone; }
            set { SetPropertyValue("Zone", ref _Zone, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public TripLocation Origin {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public TripLocation Destination {
            get { return _Destination; }
            set { SetPropertyValue("Destination", ref _Destination, value); }
        }
        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Distance")]
        public decimal Distance {
            get { return _Distance; }
            set { SetPropertyValue("Distance", ref _Distance, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal TariffDistance
        {
            get { return _TariffDistance; }
            set { SetPropertyValue("TariffDistance", ref _TariffDistance, value); }
        }
        public TimeSpan TurnAround {
            get { return _TurnAround; }
            set { SetPropertyValue("TurnAround", ref _TurnAround, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Allowance {
            get { return _Allowance; }
            set { SetPropertyValue("Allowance", ref _Allowance, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Fuel {
            get { return _Fuel; }
            set { SetPropertyValue("Fuel", ref _Fuel, value); }
        }
        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Trucker Pay")]
        public decimal TruckerPay {
            get { return _TruckerPay; }
            set { SetPropertyValue("TruckerPay", ref _TruckerPay, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal TariffTruckerPay
        {
            get { return _TariffTruckerPay; }
            set { SetPropertyValue("TariffTruckerPay", ref _TariffTruckerPay, value); }
        }
        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Fuel Subsidy")]
        public decimal RateAdjmt {
            get { return _RateAdjmt; }
            set { SetPropertyValue("RateAdjmt", ref _RateAdjmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal TariffFuelSubsidy
        {
            get { return _TariffFuelSubsidy; }
            set { SetPropertyValue("TariffFuelSubsidy", ref _TariffFuelSubsidy, value); }
        }
        [DisplayName("Our Trailer")]
        public bool TrailerRent {
            get { return _TrailerRent; }
            set { SetPropertyValue("TrailerRent", ref _TrailerRent, value); }
        }
        [Custom("DisplayFormat", "n")]
        [DisplayName("Trailer Rate")]
        public decimal TrailerRental {
            get { return _TrailerRental; }
            set { SetPropertyValue("TrailerRental", ref _TrailerRental, value); 
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Insurance {
            get { return _Insurance; }
            set { SetPropertyValue("Insurance", ref _Insurance, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Point {
            get { return _Point; }
            set { SetPropertyValue("Point", ref _Point, value); }
        }
        public bool TaxInclusive {
            get { return _TaxInclusive; }
            set { SetPropertyValue("TaxInclusive", ref _TaxInclusive, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public SalesTaxCode TaxCode {
            get { return _TaxCode; }
            set { SetPropertyValue("TaxCode", ref _TaxCode, value); }
        }
        public bool WHTInclusive {
            get { return _WHTInclusive; }
            set { SetPropertyValue("WHTInclusive", ref _WHTInclusive, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public WHTGroupCode WHTGroupCode {
            get { return _WHTGroupCode; }
            set { SetPropertyValue("WHTGroupCode", ref _WHTGroupCode, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal GensetRatePerHr {
            get { return _GensetRatePerHr; }
            set { SetPropertyValue("GensetRatePerHr", ref _GensetRatePerHr, 
                value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal ShuntingRatePerKm {
            get { return _ShuntingRatePerKm; }
            set { SetPropertyValue("ShuntingRatePerKm", ref _ShuntingRatePerKm, 
                value); }
        }
        public Account InsuranceExpenseAcct {
            get { return _InsuranceExpenseAcct; }
            set { SetPropertyValue("InsuranceExpenseAcct", ref 
                _InsuranceExpenseAcct, value); }
        }
        public decimal KDKmRun
        {
            get
            {
                return _KDKmRun;
            }
            set
            {
                SetPropertyValue("KDKmRun", ref _KDKmRun, value);
            }
        }
        public decimal KDAmount
        {
            get
            {
                return _KDAmount;
            }
            set
            {
                SetPropertyValue("KDAmount", ref _KDAmount, value);
            }
        }
        public decimal KDFuelSub
        {
            get
            {
                return _KDFuelSub;
            }
            set
            {
                SetPropertyValue("KDFuelSub", ref _KDFuelSub, value);
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal TruckerPayRate
        {
            get { return _TruckerPayRate; }
            set
            {
                SetPropertyValue("TruckerPayRate", ref _TruckerPayRate,
                    value);
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal FuelSubsidyRate
        {
            get { return _FuelSubsidyRate; }
            set
            {
                SetPropertyValue("FuelSubsidyRate", ref _FuelSubsidyRate,
                    value);
            }
        }

        [Action(AutoCommit = true, Caption = "Diactivate Adjustment", ConfirmationMessage = "Are you sure you want to deactivate the active rate?")]
        public void DeactivateAdjustment()
        {
            ActiveTariffRateAdjustment = null;
        }

        [Custom("AllowEdit", "False")]
        public TariffRateAdjustmentHistory ActiveTariffRateAdjustment
        {
            get { return _ActiveTariffRateAdjustment; }
            set { SetPropertyValue("ActiveTariffRateAdjustment", ref _ActiveTariffRateAdjustment, value); }
        }
        
        [Persistent]
        public string Route { get { return ObjectFormatter.Format(
                defaultFromToFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); } }
        [Aggregated,
        Association("Tariff-TariffDriversClassifiers")]
        public XPCollection<TariffDriversClassifier> TariffDriversClassifiers { 
            get { return GetCollection<TariffDriversClassifier>(
                "TariffDriversClassifiers"); } }

        [Aggregated,
Association("Tariff-TariffFuelAllocations")]
        public XPCollection<TariffFuelAllocation> TariffFuelAllocations
        {
            get
            {
                return GetCollection<TariffFuelAllocation>(
                    "TariffFuelAllocations");
            }
        }

        [Aggregated,
        Association("Tariff-TariffRateAdjustmentHistory")]
        public XPCollection<TariffRateAdjustmentHistory> TariffRateAdjustmentHistory
        {
            get
            {
                return GetCollection<TariffRateAdjustmentHistory>(
                    "TariffRateAdjustmentHistory");
            }
        }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private BusinessObjects.TariffRateAdjustmentHistory _ActiveTariffRateAdjustment;
        private decimal _TruckerPayRate;
        private decimal _FuelSubsidyRate;
        
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
        public Tariff(Session session): base(session) {
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
    }
}
