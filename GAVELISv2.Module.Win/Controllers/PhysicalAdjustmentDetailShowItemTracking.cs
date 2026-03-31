using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class PhysicalAdjustmentDetailShowItemTracking : 
    ViewController {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction 
        physicalAdjustmentDetailShowItemTrackingAction;
        private PhysicalAdjustmentDetail _PhysicalAdjustmentDetail;
        public PhysicalAdjustmentDetailShowItemTracking() {
            this.TargetObjectType = typeof(PhysicalAdjustmentDetail);
            this.TargetViewType = ViewType.Any;
            string actionID = "PhysicalAdjustmentDetail.ShowItemTracking";
            this.physicalAdjustmentDetailShowItemTrackingAction = new 
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.physicalAdjustmentDetailShowItemTrackingAction.
            CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            PhysicalAdjustmentDetailShowItemTrackingAction_CustomizePopupWindowParams
            );
            this.physicalAdjustmentDetailShowItemTrackingAction.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            PhysicalAdjustmentDetailShowItemTrackingAction_Execute);
        }
        private void 
        PhysicalAdjustmentDetailShowItemTrackingAction_CustomizePopupWindowParams
        (object sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _PhysicalAdjustmentDetail = this.View.CurrentObject as 
            PhysicalAdjustmentDetail;
            ObjectSpace.CommitChanges();
            PhysicalAdjustmentDetail thisPhysicalAdjustmentDetail = _ObjectSpace
            .GetObjectByKey<PhysicalAdjustmentDetail>(_PhysicalAdjustmentDetail.
            Oid);
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "PhysicalAdjustmentDetail_TrackingLine", false, 
            thisPhysicalAdjustmentDetail);
            //e.View = Application.CreateListEditor(collectionSource,);
        }
        private void PhysicalAdjustmentDetailShowItemTrackingAction_Execute(
        object sender, PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_PhysicalAdjustmentDetail);
            ObjectSpace.Refresh();
        }
    }
}
