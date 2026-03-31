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
    public class Terms : BaseObject {
        private string _Code;
        private string _Description;
        private int _DaysToPay;
        private decimal _EarlyDiscount;
        private int _EarlyDaysToPay;
        private decimal _LateCharge;
        private int _LateDaysToPay;
        private Account _DiscountReceivedAccount;
        private Account _InterestExpenseAccount;
        private Account _DicountGivenAccount;
        private Account _InterestIncomeAccount;
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
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
        public int DaysToPay {
            get { return _DaysToPay; }
            set { SetPropertyValue("DaysToPay", ref _DaysToPay, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal EarlyDiscount {
            get { return _EarlyDiscount; }
            set { SetPropertyValue("EarlyDiscount", ref _EarlyDiscount, value); 
            }
        }
        public int EarlyDaysToPay {
            get { return _EarlyDaysToPay; }
            set { SetPropertyValue("EarlyDaysToPay", ref _EarlyDaysToPay, value)
                ; }
        }
        [Custom("DisplayFormat", "n")]
        public decimal LateCharge {
            get { return _LateCharge; }
            set { SetPropertyValue("LateCharge", ref _LateCharge, value); }
        }
        public int LateDaysToPay {
            get { return _LateDaysToPay; }
            set { SetPropertyValue("LateDaysToPay", ref _LateDaysToPay, value); 
            }
        }
        public Account DiscountReceivedAccount {
            get { return _DiscountReceivedAccount; }
            set { SetPropertyValue("DiscountReceivedAccount", ref 
                _DiscountReceivedAccount, value); }
        }
        public Account InterestExpenseAccount {
            get { return _InterestExpenseAccount; }
            set { SetPropertyValue("InterestExpenseAccount", ref 
                _InterestExpenseAccount, value); }
        }
        public Account DicountGivenAccount {
            get { return _DicountGivenAccount; }
            set { SetPropertyValue("DicountGivenAccount", ref 
                _DicountGivenAccount, value); }
        }
        public Account InterestIncomeAccount {
            get { return _InterestIncomeAccount; }
            set { SetPropertyValue("InterestIncomeAccount", ref 
                _InterestIncomeAccount, value); }
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
        public Terms(Session session): base(session) {
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
