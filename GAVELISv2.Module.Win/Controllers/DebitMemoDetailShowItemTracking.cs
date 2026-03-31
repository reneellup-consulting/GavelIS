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
    public partial class DebitMemoDetailShowItemTracking : ViewController {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction DebitMemoDetailShowItemTrackingAction;
        private DebitMemoDetail _DebitMemoDetail;
        public DebitMemoDetailShowItemTracking() {
            this.TargetObjectType = typeof(DebitMemoDetail);
            this.TargetViewType = ViewType.Any;
            string actionID = "DebitMemoDetail.ShowItemTracking";
            this.DebitMemoDetailShowItemTrackingAction = new 
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.DebitMemoDetailShowItemTrackingAction.
            CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            DebitMemoDetailShowItemTrackingAction_CustomizePopupWindowParams);
            this.DebitMemoDetailShowItemTrackingAction.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            DebitMemoDetailShowItemTrackingAction_Execute);
        }
        private void 
        DebitMemoDetailShowItemTrackingAction_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _DebitMemoDetail = this.View.CurrentObject as DebitMemoDetail;
            ObjectSpace.CommitChanges();
            DebitMemoDetail thisDebitMemoDetail = _ObjectSpace.GetObjectByKey<
            DebitMemoDetail>(_DebitMemoDetail.Oid);
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "DebitMemoDetail_TrackingLine", false, thisDebitMemoDetail);
            //e.View = Application.CreateListEditor(collectionSource,);
        }
        private void DebitMemoDetailShowItemTrackingAction_Execute(object sender
        , PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_DebitMemoDetail);
            ObjectSpace.Refresh();
        }
    }
}
