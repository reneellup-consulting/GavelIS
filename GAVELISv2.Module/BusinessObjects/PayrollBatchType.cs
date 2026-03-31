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
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class PayrollBatchType : BaseObject {
        private const string defaultDisplayFormat = "{Code}->{Description}";
        private string _Code;
        private string _Description;
        private Customer _Customer;
        private bool _IsHolcim = false;
        private string _EmployeeFilter;
        private string _RegistryFilter;
        private int _DaysCovered = 15;
        private bool _IncludeTax = true;
        private bool _IncludeLoans = true;
        private bool _IncludePremiums = true;
        private bool _IncludeOtherDed = true;
        private bool _TaggedFuelRequired = false;
        private decimal _CAforMiscExp;
        private int _MonthlyRegDays;
        private bool _Contractor = false;
        private decimal _OTRate;
        private decimal _DayOffOTRate;
        private decimal _HolidayOTRate;
        private decimal _NightDiffRate;
        private bool _OTMustBeApproved = true;
        private Account _SalariesAndWagesAccount;
        private Account _PayrollAdjustmentsAccount;
        private Account _RescuePayrollAccount;
        private Account _PayrollIncentives;
        private Account _DriversMiscExpenseAccount;
        private Account _AddlMiscExpenseAccount;
        private Account _KDsPayrollAccount;
        private Account _ShuntingPayrollAccount;
        private Account _AdvancesToEmpMiscExpAccount;
        private Account _AdvancesToEmpAddlMiscExpAccount;
        private Account _CashInBankAccount;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public Customer Customer {
            get { return _Customer; }
            set { SetPropertyValue("Customer", ref _Customer, value); }
        }

        public bool IsHolcim {
            get { return _IsHolcim; }
            set { SetPropertyValue<bool>("IsHolcim", ref _IsHolcim, value); }
        }

        [Size(500)]
        //[RuleRequiredField("", DefaultContexts.Save)]
        public string EmployeeFilter {
            get { return _EmployeeFilter; }
            set { SetPropertyValue("EmployeeFilter", ref _EmployeeFilter, value)
                ; }
        }

        [Size(500)]
        //[RuleRequiredField("", DefaultContexts.Save)]
        public string RegistryFilter
        {
            get { return _RegistryFilter; }
            set
            {
                SetPropertyValue("RegistryFilter", ref _RegistryFilter, value)
                    ;
            }
        }

        public int DaysCovered {
            get { return _DaysCovered; }
            set { SetPropertyValue("DaysCovered", ref _DaysCovered, value); }
        }

        public bool IncludeTax {
            get { return _IncludeTax; }
            set { SetPropertyValue("IncludeTax", ref _IncludeTax, value); }
        }

        public bool IncludeLoans {
            get { return _IncludeLoans; }
            set { SetPropertyValue("IncludeLoans", ref _IncludeLoans, value); }
        }

        public bool IncludePremiums {
            get { return _IncludePremiums; }
            set { SetPropertyValue("IncludePremiums", ref _IncludePremiums,
                value); }
        }

        public bool IncludeOtherDed {
            get { return _IncludeOtherDed; }
            set { SetPropertyValue("IncludeOtherDed", ref _IncludeOtherDed,
                value); }
        }

        public bool TaggedFuelRequired
        {
            get { return _TaggedFuelRequired; }
            set
            {
                SetPropertyValue("TaggedFuelRequired", ref _TaggedFuelRequired,
                    value);
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CAforMiscExp {
            get { return _CAforMiscExp; }
            set { SetPropertyValue("CAforMiscExp", ref _CAforMiscExp, value); }
        }

        public int MonthlyRegDays {
            get { return _MonthlyRegDays; }
            set { SetPropertyValue("MonthlyRegDays", ref _MonthlyRegDays, value)
                ; }
        }

        public bool Contractor {
            get { return _Contractor; }
            set { SetPropertyValue("Contractor", ref _Contractor, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal OTRate {
            get { return _OTRate; }
            set { SetPropertyValue("OTRate", ref _OTRate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal DayOffOTRate {
            get { return _DayOffOTRate; }
            set { SetPropertyValue("DayOffOTRate", ref _DayOffOTRate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal HolidayOTRate {
            get { return _HolidayOTRate; }
            set { SetPropertyValue("HolidayOTRate", ref _HolidayOTRate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal NightDiffRate {
            get { return _NightDiffRate; }
            set { SetPropertyValue("NightDiffRate", ref _NightDiffRate, value); }
        }

        public bool OTMustBeApproved {
            get { return _OTMustBeApproved; }
            set { SetPropertyValue("OTMustBeApproved", ref _OTMustBeApproved,
                value); }
        }

        public Account SalariesAndWagesAccount {
            get { return _SalariesAndWagesAccount; }
            set { SetPropertyValue<Account>("SalariesAndWagesAccount", ref _SalariesAndWagesAccount, value); }
        }

        public Account PayrollAdjustmentsAccount {
            get { return _PayrollAdjustmentsAccount; }
            set { SetPropertyValue<Account>("PayrollAdjustmentsAccount", ref _PayrollAdjustmentsAccount, value); }
        }

        public Account RescuePayrollAccount {
            get { return _RescuePayrollAccount; }
            set { SetPropertyValue<Account>("RescuePayrollAccount", ref _RescuePayrollAccount, value); }
        }

        public Account PayrollIncentives {
            get { return _PayrollIncentives; }
            set { SetPropertyValue<Account>("PayrollIncentives", ref _PayrollIncentives, value); }
        }

        public Account DriversMiscExpenseAccount {
            get { return _DriversMiscExpenseAccount; }
            set { SetPropertyValue<Account>("DriversMiscExpenseAccount", ref _DriversMiscExpenseAccount, value); }
        }

        public Account AddlMiscExpenseAccount {
            get { return _AddlMiscExpenseAccount; }
            set { SetPropertyValue<Account>("AddlMiscExpenseAccount", ref _AddlMiscExpenseAccount, value); }
        }

        public Account KDsPayrollAccount {
            get { return _KDsPayrollAccount; }
            set { SetPropertyValue<Account>("KDsPayrollAccount", ref _KDsPayrollAccount, value); }
        }

        public Account ShuntingPayrollAccount {
            get { return _ShuntingPayrollAccount; }
            set { SetPropertyValue<Account>("ShuntingPayrollAccount", ref _ShuntingPayrollAccount, value); }
        }

        public Account AdvancesToEmpMiscExpAccount {
            get { return _AdvancesToEmpMiscExpAccount; }
            set { SetPropertyValue<Account>("AdvancesToEmpMiscExpAccount", ref _AdvancesToEmpMiscExpAccount, value); }
        }

        public Account AdvancesToEmpAddlMiscExpAccount {
            get { return _AdvancesToEmpAddlMiscExpAccount; }
            set { SetPropertyValue<Account>("AdvancesToEmpAddlMiscExpAccount", ref _AdvancesToEmpAddlMiscExpAccount, value); }
        }

        public Account CashInBankAccount {
            get { return _CashInBankAccount; }
            set { SetPropertyValue<Account>("CashInBankAccount", ref _CashInBankAccount, value); }
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

        #region Display String

        public string DisplayName {
            get { return ObjectFormatter.Format(
                defaultDisplayFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        public PayrollBatchType(Session session)
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
