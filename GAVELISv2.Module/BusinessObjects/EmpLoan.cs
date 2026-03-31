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
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class EmpLoan : XPObject
    {
        public string EntryNo { get { return Oid > 0 ? String.Format("ID:{0:D6}"
                , Oid) : String.Empty; } }
        private Employee _Employee;
        private EmployeeLoan _LoanCode;
        private DateTime _LoanDate;
        private string _RefNo;
        private decimal _LoanAmount;
        private decimal _Amortization;
        private DateTime _PayBegin;
        private DateTime _PayEnd;
        private decimal _LoanBalance;
        private bool _Paid = false;
        [Association("Employee-Loans")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public EmployeeLoan LoanCode {
            get { return _LoanCode; }
            set { SetPropertyValue("LoanCode", ref _LoanCode, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime LoanDate {
            get { return _LoanDate; }
            set { SetPropertyValue("LoanDate", ref _LoanDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string RefNo {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }
        public decimal LoanAmount {
            get { return _LoanAmount; }
            set {
                SetPropertyValue("LoanAmount", ref _LoanAmount, value);
                if (!IsLoading && _PayBegin == DateTime.MinValue) {LoanBalance = 
                    _LoanAmount;}
            }
        }
        public decimal Amortization {
            get { return _Amortization; }
            set { SetPropertyValue("Amortization", ref _Amortization, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime PayBegin {
            get { return _PayBegin; }
            set { SetPropertyValue("PayBegin", ref _PayBegin, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime PayEnd {
            get { return _PayEnd; }
            set { SetPropertyValue("PayEnd", ref _PayEnd, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal LoanBalance {
            get { return _LoanBalance; }
            set { SetPropertyValue("LoanBalance", ref _LoanBalance, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Paid {
            get { return _Paid; }
            set { SetPropertyValue("Paid", ref _Paid, value); }
        }
        [Action(AutoCommit = true, Caption = "Mark as Paid")]
        public void MarkAsPaid()
        {
            PayEnd = DateTime.Now;
            Paid = true;
        }
        [Action(AutoCommit = true, Caption = "Mark as Unpaid")]
        public void MarkAsUnpaid()
        {
            PayEnd = DateTime.MinValue;
            Paid = false;
        }

        public bool IsAllowedToModifyLoanBal
        {
            get
            {
                var data = _CurrentUser != null ? _CurrentUser.Roles.Where(o => o.Name == "ModifyEmpLoan").LastOrDefault() : null;
                if (data != null)
                {
                    return true;
                }
                return false;
            }
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
        public EmpLoan(Session session): base(session) {
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
