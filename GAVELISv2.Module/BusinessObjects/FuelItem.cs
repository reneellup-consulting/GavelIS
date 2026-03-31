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
    public class FuelItem : Item {
        private Account _ExpenseAccount;
        private bool _UpdateOnline;
        public override string No
        {
            get
            {
                return base.No;
            }
            set
            {
                base.No = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }
        public override string Description
        {
            get
            {
                return base.Description;
            }
            set
            {
                base.Description = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }
        public override decimal Cost
        {
            get
            {
                return base.Cost;
            }
            set
            {
                base.Cost = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account ExpenseAccount {
            get { return _ExpenseAccount; }
            set { SetPropertyValue("ExpenseAccount", ref _ExpenseAccount, value)
                ; }
        }
        [Custom("AllowEdit", "False")]
        public bool UpdateOnline
        {
            get { return _UpdateOnline; }
            set { SetPropertyValue("UpdateOnline", ref _UpdateOnline, value); }
        }
        public FuelItem(Session session): base(session) {
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
            ItemType = ItemTypeEnum.FuelItem;
        }
    }
}
