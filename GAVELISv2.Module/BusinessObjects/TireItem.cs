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
    public class TireItem : Item {
        private TireItemClassEnum _TireItemClass;
        private TireMake _Make;
        private TireSize _Size;
        private TireType _Type;
        public TireItemClassEnum TireItemClass {
            get { return _TireItemClass; }
            set
            {
                if (_TireItemClass == value)
                    return;
                _TireItemClass = value;
            }
        }

        public TireMake Make {
            get { return _Make; }
            set { SetPropertyValue<TireMake>("Make", ref _Make, value); }
        }

        public TireSize Size {
            get { return _Size; }
            set { SetPropertyValue<TireSize>("Size", ref _Size, value); }
        }
        [DisplayName("Thread")]
        public TireType Type {
            get { return _Type; }
            set { SetPropertyValue<TireType>("Type", ref _Type, value); }
        }

        public TireItem(Session session)
            : base(session) {
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
            ItemType = ItemTypeEnum.TireItem;
        }
    }
}
