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
    public class ChargeItem : Item {
        private AmountOrRateEnum _AmountOrRate;
        private decimal _Amount;
        private Account _ChargeAccount;
        public AmountOrRateEnum AmountOrRate {
            get { return _AmountOrRate; }
            set { SetPropertyValue("AmountOrRate", ref _AmountOrRate, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
        }
        public Account ChargeAccount {
            get { return _ChargeAccount; }
            set { SetPropertyValue("ChargeAccount", ref _ChargeAccount, value); 
            }
        }
        public ChargeItem(Session session): base(session) {
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
            ItemType = ItemTypeEnum.ChargeItem;
        }
    }
}
