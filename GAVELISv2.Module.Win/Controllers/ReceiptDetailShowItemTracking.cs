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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiptDetailShowItemTracking : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction
        ReceiptDetailShowItemTrackingAction;
        private ReceiptDetail _ReceiptDetail;
        public ReceiptDetailShowItemTracking()
        {
            this.TargetObjectType = typeof(ReceiptDetail);
            this.TargetViewType = ViewType.Any;
            string actionID = "ReceiptDetail.ShowItemTracking";
            this.ReceiptDetailShowItemTrackingAction = new 
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.ReceiptDetailShowItemTrackingAction.
            CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            ReceiptDetailShowItemTrackingAction_CustomizePopupWindowParams
            );
            this.ReceiptDetailShowItemTrackingAction.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            ReceiptDetailShowItemTrackingAction_Execute);
        }
        private void
        ReceiptDetailShowItemTrackingAction_CustomizePopupWindowParams
        (object sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReceiptDetail = this.View.CurrentObject as 
            ReceiptDetail;
            ObjectSpace.CommitChanges();
            ReceiptDetail thisReceiptDetail = _ObjectSpace
            .GetObjectByKey<ReceiptDetail>(_ReceiptDetail.
            Oid);
            e.View = Application.CreateDetailView(_ObjectSpace,
            "ReceiptDetail_TrackingLine", false,
            thisReceiptDetail);
            //e.View = Application.CreateListEditor(collectionSource,);
        }
        private void ReceiptDetailShowItemTrackingAction_Execute(
        object sender, PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_ReceiptDetail);
            ObjectSpace.Refresh();
        }
    }
}
