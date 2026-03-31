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
    public class Facility : BaseObject {
        private const string defaultDisplayFormat = "{Code}->{Description}";
        private string _Code;
        private string _Description;
        private Region _Region;
        private CostCenter _CostCenter;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
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
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("Region-Facilities")]
        public Region Region {
            get { return _Region; }
            set { SetPropertyValue("Region", ref _Region, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue("CostCenter", ref _CostCenter, value); }
        }
        [Association("Facility-Departments")]
        public XPCollection<Department> Departments { get { return GetCollection
                <Department>("Departments"); } }
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
        public string DisplayName { get { return ObjectFormatter.Format(
                defaultDisplayFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); } }
        #endregion
        public Facility(Session session): base(session) {
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
            //Session.OptimisticLockingReadBehavior = 
            //OptimisticLockingReadBehavior.ReloadObject;
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
