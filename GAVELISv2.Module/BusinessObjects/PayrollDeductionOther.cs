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
    [NavigationItem(false)]
    public class PayrollDeductionOther : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private DateTime _DeductionDate;
        private DeductionType _DeductionType;
        private OtherDeduction _DeductionCode;
        private DateTime _AdvanceEntryDate;
        private string _Explanation;
        private string _RefNo;
        private bool _MonthlyCA = false;
        private decimal _Balance;
        private decimal _Amount;
        private bool _Posted = false;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("Payroll-DeductionOthers")]
        public GenJournalHeader PayrollBatchID {
            get { return _PayrollBatchID; }
            set { SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value)
                ; }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime DeductionDate {
            get { return _DeductionDate; }
            set { SetPropertyValue("DeductionDate", ref _DeductionDate, value); 
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DeductionType DeductionType {
            get { return _DeductionType; }
            set { SetPropertyValue("DeductionType", ref _DeductionType, value); 
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public OtherDeduction DeductionCode {
            get { return _DeductionCode; }
            set { SetPropertyValue("DeductionCode", ref _DeductionCode, value); 
            }
        }

        public DateTime AdvanceEntryDate {
            get { return _AdvanceEntryDate; }
            set { SetPropertyValue<DateTime>("AdvanceEntryDate", ref 
                _AdvanceEntryDate, value); }
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

        public bool MonthlyCA {
            get { return _MonthlyCA; }
            set { SetPropertyValue("MonthlyCA", ref _MonthlyCA, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal PrevBalance
        {
            get { return _PrevBalance; }
            set { SetPropertyValue("PrevBalance", ref _PrevBalance, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Balance {
            get { return _Balance; }
            set { SetPropertyValue("Balance", ref _Balance, value); }
        }

        public decimal Amount {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool Posted {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
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

        public PayrollDeductionOther(Session session)
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
            RowID = Guid.NewGuid();

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
        private decimal _PrevBalance;
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
