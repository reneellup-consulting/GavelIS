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
    public class ServiceItem : Item {
        private bool _ThirdParty = false;
        private Account _ExpenseAccount;
        private decimal _Rate;
        public bool ThirdParty {
            get { return _ThirdParty; }
            set { SetPropertyValue("ThirdParty", ref _ThirdParty, value); }
        }

        public Account ExpenseAccount {
            get { return _ExpenseAccount; }
            set { SetPropertyValue("ExpenseAccount", ref _ExpenseAccount, value)
                ; }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Rate {
            get { return _Rate; }
            set { SetPropertyValue("Rate", ref _Rate, value); }
        }
        public ServiceItem(Session session): base(session) {
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
            ItemType = ItemTypeEnum.ServiceItem;
        }
    }
}
