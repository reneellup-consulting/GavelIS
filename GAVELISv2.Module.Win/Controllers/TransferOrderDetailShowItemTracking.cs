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
    public partial class TransferOrderDetailShowItemTracking : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction
        transferOrderDetailShowItemTrackingAction;
        private TransferOrderDetail _TransferOrderDetail;

        public TransferOrderDetailShowItemTracking()
        {
            this.TargetObjectType = typeof(TransferOrderDetail);
            this.TargetViewType = ViewType.Any;
            string actionID = "TransferOrderDetail.ShowItemTracking";
            this.transferOrderDetailShowItemTrackingAction = new
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.transferOrderDetailShowItemTrackingAction.
            CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            TransferOrderDetailShowItemTrackingAction_CustomizePopupWindowParams
            );
            this.transferOrderDetailShowItemTrackingAction.Execute += new
            PopupWindowShowActionExecuteEventHandler(
            TransferOrderDetailShowItemTrackingAction_Execute);
        }
        private void
        TransferOrderDetailShowItemTrackingAction_CustomizePopupWindowParams(object
        sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _TransferOrderDetail = this.View.CurrentObject as
            TransferOrderDetail;
            ObjectSpace.CommitChanges();
            TransferOrderDetail thisTransferOrderDetail = _ObjectSpace
            .GetObjectByKey<TransferOrderDetail>(_TransferOrderDetail.
            Oid);
            e.View = Application.CreateDetailView(_ObjectSpace,
            "TransferOrderDetail_TrackingLine", false,
            thisTransferOrderDetail);
            //e.View = Application.CreateListEditor(collectionSource,);
        }
        private void TransferOrderDetailShowItemTrackingAction_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_TransferOrderDetail);
            ObjectSpace.Refresh();
        }
    }
}
