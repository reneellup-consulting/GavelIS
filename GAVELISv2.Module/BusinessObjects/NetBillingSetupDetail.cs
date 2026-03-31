using System;
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
    public enum TariffReferenceEnum
    {
        None = 0,
        TruckerPay = 1,
        RateAdjmt = 2,
        TrailerRate = 3,
        Insurance = 4,
        VatRate = 5,
        WhtRate = 6,
        Given = 20,
        Formula = 21
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save,"SetupId,Seq,Code")]
    public class NetBillingSetupDetail : BaseObject
    {
        private NetBillingCompSetup _SetupId;
        private int _Seq;
        private string _Code;
        private string _Caption;
        private TariffReferenceEnum _ReferenceData;
        private decimal _GivenVal;
        private string _Formula;
        private Account _DebitAccount;
        private Account _CreditAccount;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("NetBillSetupDetails")]
        public NetBillingCompSetup SetupId
        {
            get { return _SetupId; }
            set { SetPropertyValue("SetupId", ref _SetupId, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public int Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Caption
        {
            get { return _Caption; }
            set { SetPropertyValue("Caption", ref _Caption, value); }
        }
        public TariffReferenceEnum ReferenceData
        {
            get { return _ReferenceData; }
            set { SetPropertyValue("ReferenceData", ref _ReferenceData, value); }
        }
        public decimal GivenVal
        {
            get { return _GivenVal; }
            set { SetPropertyValue("GivenVal", ref _GivenVal, value); }
        }
        [Size(500)]
        public string Formula
        {
            get { return _Formula; }
            set { SetPropertyValue("Formula", ref _Formula, value); }
        }
        public Account DebitAccount
        {
            get { return _DebitAccount; }
            set { SetPropertyValue("DebitAccount", ref _DebitAccount, value); }
        }
        public Account CreditAccount
        {
            get { return _CreditAccount; }
            set { SetPropertyValue("CreditAccount", ref _CreditAccount, value); }
        }
        
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
        public NetBillingSetupDetail(Session session)
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
