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
    [NavigationItem(false)]
    public class EmpTax : XPObject {
        public string EntryNo { get { return Oid > 0 ? String.Format("ID:{0:D6}"
                , Oid) : String.Empty; } }
        private Employee _Employee;
        private EmployeeTax _TaxCode;
        private int _Year;
        private decimal _TaxAmount;
        private decimal _Deduction;
        private DateTime _PayBegin;
        private DateTime _PayEnd;
        private decimal _TaxBalance;
        private bool _Paid = false;
        [Association("Employee-Taxes")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public EmployeeTax TaxCode {
            get { return _TaxCode; }
            set { SetPropertyValue("TaxCode", ref _TaxCode, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public int Year {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        public decimal TaxAmount {
            get { return _TaxAmount; }
            set {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
                if (!IsLoading && _PayBegin == DateTime.MinValue) {TaxBalance = 
                    _TaxAmount;}
            }
        }
        public decimal Deduction {
            get { return _Deduction; }
            set { SetPropertyValue("Amortization", ref _Deduction, value); }
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
        [Custom("AllowEdit", "False")]
        public decimal TaxBalance {
            get { return _TaxBalance; }
            set { SetPropertyValue("LoanBalance", ref _TaxBalance, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Paid {
            get { return _Paid; }
            set { SetPropertyValue("Paid", ref _Paid, value); }
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
        public EmpTax(Session session): base(session) {
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
