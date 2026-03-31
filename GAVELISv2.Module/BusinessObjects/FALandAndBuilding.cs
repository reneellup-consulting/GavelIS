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
    public class FALandAndBuilding : FixedAsset {
        private string _Reference;
        private RealPropertClassEnum _RealPropertyClass;
        private string _Size;
        public string Reference {
            get { return _Reference; }
            set { SetPropertyValue("Reference", ref _Reference, value); }
        }
        public RealPropertClassEnum RealPropertyClass {
            get { return _RealPropertyClass; }
            set { SetPropertyValue("RealPropertyClass", ref _RealPropertyClass, 
                value); }
        }
        public string Size {
            get { return _Size; }
            set { SetPropertyValue("Size", ref _Size, value); }
        }
        public FALandAndBuilding(Session session): base(session) {
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
            FixedAssetClass = FixedAssetClassEnum.LandAndBuilding;
        }
    }
}
