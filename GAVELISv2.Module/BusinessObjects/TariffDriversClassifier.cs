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
    //[NavigationItem(false)]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, 
    "TariffID, DriverClass")]
    public class TariffDriversClassifier : BaseObject {
        private Tariff _TariffID;
        private DriverClassification _DriverClass;
        private decimal _BaseShare;
        private decimal _ShuntingShare;
        private decimal _KDShare;
        private decimal _ShareRate;
        [Association("Tariff-TariffDriversClassifiers")]
        public Tariff TariffID {
            get { return _TariffID; }
            set { SetPropertyValue("TariffID", ref _TariffID, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DriverClassification DriverClass {
            get { return _DriverClass; }
            set {
                SetPropertyValue("DriverClass", ref _DriverClass, value);
                if (!IsLoading && _DriverClass != null) {ShareRate = 
                    _DriverClass.ShareRate;}
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal BaseShare {
            get { return _BaseShare; }
            set { SetPropertyValue("BaseShare", ref _BaseShare, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal ShuntingShare {
            get { return _ShuntingShare; }
            set { SetPropertyValue("ShuntingShare", ref _ShuntingShare, value); 
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal KDShare {
            get { return _KDShare; }
            set { SetPropertyValue("KDShare", ref _KDShare, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal ShareRate {
            get { return _ShareRate; }
            set { SetPropertyValue("ShareRate", ref _ShareRate, value); }
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
        public TariffDriversClassifier(Session session): base(session) {
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
