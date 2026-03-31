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
    [NavigationItem(false)]
    public class PhysicalAdjustmentDetailTrackingLine : ItemTrackingLine {
        private PhysicalAdjustmentDetail _PhysicalAdjustmentDetailID;
        [Association(
        "PhysicalAdjustmentDetail-PhysicalAdjustmentDetailTrackingLines")]
        public PhysicalAdjustmentDetail PhysicalAdjustmentDetailID {
            get { return _PhysicalAdjustmentDetailID; }
            set {
                SetPropertyValue("PhysicalAdjustmentDetailID", ref 
                _PhysicalAdjustmentDetailID, value);
                if (!IsLoading && _PhysicalAdjustmentDetailID != null) {
                    ItemNo = _PhysicalAdjustmentDetailID.ItemNo;
                    SourceRowID = _PhysicalAdjustmentDetailID.RowID;
                    Source = "Physical Adjustmentment Detail";
                }
            }
        }
        public PhysicalAdjustmentDetailTrackingLine(Session session): base(
            session) {
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
        }
    }
}
