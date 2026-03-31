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
    [OptimisticLocking(false)]
    [System.ComponentModel.DefaultProperty("Code")]
    public class TireDettachReason : XPObject {
        private string _Code;
        private string _Description;
        private bool _Recapped = false;
        private bool _Scrap = false;
        private bool _Sold = false;
        private bool _Lost = false;
        private bool _Branding = false;
        private bool _FirstBranding=false;
        private string _Suffix;
        private string _BYear;
        private string _LastNo;
        private int _BrandingLevel;
        private decimal _TreadDepth;
        private Vendor _Vendor;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue<string>("Code", ref _Code, value); }
        }

        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }

        public bool Recapped {
            get { return _Recapped; }
            set { SetPropertyValue<bool>("Recapped", ref _Recapped, value); }
        }

        public bool Scrap {
            get { return _Scrap; }
            set { SetPropertyValue<bool>("Scrap", ref _Scrap, value); }
        }

        public bool Sold {
            get { return _Sold; }
            set { SetPropertyValue<bool>("Sold", ref _Sold, value); }
        }

        public bool Lost {
            get { return _Lost; }
            set { SetPropertyValue<bool>("Lost", ref _Lost, value); }
        }

        public bool Branding {
            get { return _Branding; }
            set { SetPropertyValue<bool>("Branding", ref _Branding, value); }
        }

        public bool FirstBranding
        {
            get { return _FirstBranding; }
            set { SetPropertyValue<bool>("FirstBranding", ref _FirstBranding, value); }
        }

        public string Suffix
        {
            get { return _Suffix; }
            set { SetPropertyValue<string>("Suffix", ref _Suffix, value); }
        }

        public string BYear
        {
            get { return _BYear; }
            set { SetPropertyValue<string>("BYear", ref _BYear, value); }
        }

        public string LastNo
        {
            get { return _LastNo; }
            set { SetPropertyValue<string>("LastNo", ref _LastNo, value); }
        }

        public int BrandingLevel {
            get { return _BrandingLevel; }
            set { SetPropertyValue<int>("BrandingLevel", ref _BrandingLevel, value); }
        }

        public decimal TreadDepth {
            get { return _TreadDepth; }
            set { SetPropertyValue<decimal>("TreadDepth", ref _TreadDepth, value); }
        }

        public Vendor Vendor {
            get { return _Vendor; }
            set { SetPropertyValue<Vendor>("Vendor", ref _Vendor, value); }
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

        public TireDettachReason(Session session)
            : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();


            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }

            #endregion

        }

        protected override void OnSaving() {
            base.OnSaving();

            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
            }

            #endregion

        }
    }
}
