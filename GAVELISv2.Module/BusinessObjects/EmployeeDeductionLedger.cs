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
    public class EmployeeDeductionLedger : XPObject {
        private DriverPayrollBatch _PayrollBatchID;
        private Employee _Employee;
        private EmpLoan _EmpLoanID;
        private EmpTax _EmpTaxID;
        private EmpOtherDed _EmpOtherDedID;
        private int _EntryNo;
        private DateTime _EntryDate;
        private DeductionType _DeductionType;
        private string _DeductionName;
        private string _RefNo;
        private MonthsEnum _Month;
        private decimal _Debit;
        private decimal _Credit;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DriverPayrollBatch PayrollBatchID {
            get { return _PayrollBatchID; }
            set { SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value)
                ; }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        //[Association("Employee-LoanLedger")]
        public EmpLoan EmpLoanID {
            get { return _EmpLoanID; }
            set { SetPropertyValue("EmpLoanID", ref _EmpLoanID, value); }
        }
        //[Association("Employee-TaxLedger")]
        public EmpTax EmpTaxID {
            get { return _EmpTaxID; }
            set { SetPropertyValue("EmpTaxID", ref _EmpTaxID, value); }
        }
        //[Association("Employee-OtherDedLedger")]
        public EmpOtherDed EmpOtherDedID {
            get { return _EmpOtherDedID; }
            set { SetPropertyValue("EmpOtherDedID", ref _EmpOtherDedID, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public int EntryNo {
            get { return _EntryNo; }
            set { SetPropertyValue("EntryNo", ref _EntryNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DeductionType DeductionType {
            get { return _DeductionType; }
            set { SetPropertyValue("DeductionType", ref _DeductionType, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string DeductionName {
            get { return _DeductionName; }
            set { SetPropertyValue("DeductionName", ref _DeductionName, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string RefNo {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public MonthsEnum Month {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public decimal Debit {
            get { return _Debit; }
            set { SetPropertyValue("Debit", ref _Debit, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public decimal Credit {
            get { return _Credit; }
            set { SetPropertyValue("Credit", ref _Credit, value); }
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
        public EmployeeDeductionLedger(Session session): base(session) {
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
