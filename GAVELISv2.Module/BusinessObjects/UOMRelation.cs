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
    public class UOMRelation : BaseObject {
        private Item _ItemNo;
        private UnitOfMeasure _UOM;
        private decimal _Factor;
        private decimal _CostPerBaseUom;
        private decimal _PricePerBaseUom;
        [Association("Item-UOMRelations")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Item ItemNo {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set { SetPropertyValue("UOM", ref _UOM, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
        }

        [Custom("DisplayFormat", "n")]
        [DisplayName("Cost/BaseUOM")]
        public decimal CostPerBaseUom
        {
            get { return _CostPerBaseUom; }
            set { SetPropertyValue("CostPerBaseUom", ref _CostPerBaseUom, value); }
        }

        [Custom("DisplayFormat", "n")]
        [DisplayName("Price/BaseUOM")]
        public decimal PricePerBaseUom
        {
            get { return _PricePerBaseUom; }
            set { SetPropertyValue("PricePerBaseUom", ref _PricePerBaseUom, value); }
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
        public UOMRelation(Session session): base(session) {
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
