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
    public class ExpenseType : BaseObject {
        private const string defaultDisplayFormat = "{Code}->{Description}";
        private string _Code;
        private string _Description;
        private bool _Income = false;
        private bool _requireSubExp = true;
        private bool _NoBuffer = false;
        private bool _Trucking = false;
        private bool _CreateForVoucher;
        private bool _TripIncome = false;

        [NonPersistent]
        public decimal ZeroFill {
            get { return 0; }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        public bool Income {
            get { return _Income; }
            set { SetPropertyValue("Income", ref _Income, value); }
        }

        public bool requireSubExp {
            get { return _requireSubExp; }
            set { SetPropertyValue("requireSubExp", ref _requireSubExp, value); }
        }
        public bool CreateForVoucher
        {
            get { return _CreateForVoucher; }
            set { SetPropertyValue("CreateForVoucher", ref _CreateForVoucher, value); }
        }
        public bool NoBuffer
        {
            get { return _NoBuffer; }
            set { SetPropertyValue("NoBuffer", ref _NoBuffer, value); }
        }
        public bool Trucking
        {
            get { return _Trucking; }
            set { SetPropertyValue("Trucking", ref _Trucking, value); }
        }
        public bool TripIncome
        {
            get { return _TripIncome; }
            set { SetPropertyValue("TripIncome", ref _TripIncome, value); }
        }
        [Aggregated,
        Association("Expense-SubType")]
        public XPCollection<SubExpenseType> SubExpenseTypes {
            get { return
                GetCollection<SubExpenseType>("SubExpenseTypes"); }
        }

        [Association("Expense-IncomeAndExpenses")]
        public XPCollection<IncomeAndExpense> IncomeAndExpenses {
            get { return
                GetCollection<IncomeAndExpense>("IncomeAndExpenses"); }
        }

        [Association("Expense-IncomeAndExpenses02")]
        public XPCollection<IncomeAndExpense02> IncomeAndExpenses02
        {
            get
            {
                return
                    GetCollection<IncomeAndExpense02>("IncomeAndExpenses02");
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

        #region Display String

        public string DisplayName {
            get { return ObjectFormatter.Format(
                defaultDisplayFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        public ExpenseType(Session session)
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
    }
}
