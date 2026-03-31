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
    public partial class InvoiceDetailShowItemTracking : ViewController {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction InvoiceDetailShowItemTrackingAction;
        private InvoiceDetail _InvoiceDetail;
        public InvoiceDetailShowItemTracking() {
            this.TargetObjectType = typeof(InvoiceDetail);
            this.TargetViewType = ViewType.Any;
            string actionID = "InvoiceDetail.ShowItemTracking";
            this.InvoiceDetailShowItemTrackingAction = new PopupWindowShowAction
            (this, actionID, PredefinedCategory.RecordEdit);
            this.InvoiceDetailShowItemTrackingAction.CustomizePopupWindowParams 
            += new CustomizePopupWindowParamsEventHandler(
            InvoiceDetailShowItemTrackingAction_CustomizePopupWindowParams);
            this.InvoiceDetailShowItemTrackingAction.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            InvoiceDetailShowItemTrackingAction_Execute);
        }
        private void 
        InvoiceDetailShowItemTrackingAction_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _InvoiceDetail = this.View.CurrentObject as InvoiceDetail;
            ObjectSpace.CommitChanges();
            InvoiceDetail thisInvoiceDetail = _ObjectSpace.GetObjectByKey<
            InvoiceDetail>(_InvoiceDetail.Oid);
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "InvoiceDetail_TrackingLine", false, thisInvoiceDetail);
            //e.View = Application.CreateListEditor(collectionSource,);
        }
        private void InvoiceDetailShowItemTrackingAction_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_InvoiceDetail);
            ObjectSpace.Refresh();
        }
    }
}
