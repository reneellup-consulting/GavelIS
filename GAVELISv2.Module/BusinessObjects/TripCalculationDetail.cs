using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    //public enum AccountSideEnum
    //{
    //    Debit,
    //    Credit
    //}
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [OptimisticLocking(false)]
    public class TripCalculationDetail : XPObject
    {
        private GenJournalHeader _GenJournalID;
        private int _Seq;
        private string _Caption;
        private decimal _Value;
        private Account _GlAccount;
        //private decimal _Amount;
        private Account _GlAccount2;
        //private decimal _Amount2;
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-TripCalculationDetails")]
        public GenJournalHeader GenJournalID
        {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Caption
        {
            get { return _Caption; }
            set { SetPropertyValue("Caption", ref _Caption, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Value
        {
            get { return _Value; }
            set { SetPropertyValue("Value", ref _Value, value); }
        }
        [Custom("AllowEdit", "False")]
        public Account GlAccount
        {
            get { return _GlAccount; }
            set { SetPropertyValue("GlAccount", ref _GlAccount, value); }
        }
        //[Custom("AllowEdit", "False")]
        //public decimal Amount
        //{
        //    get { return _Amount; }
        //    set { SetPropertyValue("Amount", ref _Amount, value); }
        //}
        [Custom("AllowEdit", "False")]
        public Account GlAccount2
        {
            get { return _GlAccount2; }
            set { SetPropertyValue("GlAccount2", ref _GlAccount2, value); }
        }
        //[Custom("AllowEdit", "False")]
        //public decimal Amount2
        //{
        //    get { return _Amount2; }
        //    set { SetPropertyValue("Amount2", ref _Amount2, value); }
        //}
        
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
        public TripCalculationDetail(Session session)
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
    }

}
