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
    [System.ComponentModel.DefaultProperty("RefNo")]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "Employee,DedCode,EntryDate,Explanation,RefNo")]
    public class EmpOtherDed : XPObject {
        public string EntryNo {
            get { return Oid > 0 ? String.Format("ID:{0:D6}"
                , Oid) : String.Empty; }
        }

        private Employee _Employee;
        private OtherDeduction _DedCode;
        private DateTime _EntryDate;
        private EmployeeChargeSlip _ChargeSlipRef;
        private string _Explanation;
        private string _RefNo;
        private decimal _Amount;
        private bool _MonthlyCA = false;
        private decimal _Deduction;
        private DateTime _PayBegin;
        private DateTime _PayEnd;
        private decimal _Balance;
        private bool _Paid = false;
        [Association("Employee-OtherDeds")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public OtherDeduction DedCode {
            get { return _DedCode; }
            set { SetPropertyValue("DedCode", ref _DedCode, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        public EmployeeChargeSlip ChargeSlipRef {
            get { return _ChargeSlipRef; }
            set { SetPropertyValue<EmployeeChargeSlip>("ChargeSlipRef", ref _ChargeSlipRef, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(1000)]
        public string Explanation {
            get { return _Explanation; }
            set { SetPropertyValue("Explanation", ref _Explanation, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string RefNo {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }

        public decimal Amount {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
                //if (!IsLoading && _PayBegin == DateTime.MinValue)
                //{
                //    Balance =
                //    _Amount;
                //}
            }
        }
        public decimal PaidAmount
        {
            get { return _PaidAmount; }
            set
            {
                SetPropertyValue("PaidAmount", ref _PaidAmount, value);
                //if (!IsLoading && _PayBegin == DateTime.MinValue)
                //{
                //    Balance = _Amount - _PaidAmount;
                    
                //}
            }
        }
        public bool MonthlyCA {
            get { return _MonthlyCA; }
            set { SetPropertyValue("MonthlyCA", ref _MonthlyCA, value); }
        }

        public decimal Deduction {
            get { return _Deduction; }
            set { SetPropertyValue("Deduction", ref _Deduction, value); }
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

        [PersistentAlias("Amount - PaidAmount")]
        public decimal Balance {
            get
            {
                object tempObject = EvaluateAlias("Balance");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public bool Paid {
            get { return _Paid; }
            set { SetPropertyValue("Paid", ref _Paid, value); }
        }
        [Action(AutoCommit = true, Caption = "Mark as Paid")]
        public void MarkAsPaid()
        {
            Paid = true;
            PaidAmount = _Amount;
            //Balance = 0;
        }
        [Action(AutoCommit = true, Caption = "Unmark as Paid")]
        public void UnmarkAsPaid()
        {
            Paid = false;
            //Balance = _PaidAmount;
        }
        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private decimal _PaidAmount;
        //private decimal _PrevBalance;
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

        public EmpOtherDed(Session session)
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
