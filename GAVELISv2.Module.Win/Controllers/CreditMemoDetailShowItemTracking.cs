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
    public partial class CreditMemoDetailShowItemTracking : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction CreditMemoDetailShowItemTrackingAction;
        private CreditMemoDetail _CreditMemoDetail;
        public CreditMemoDetailShowItemTracking()
        {
            this.TargetObjectType = typeof(CreditMemoDetail);
            this.TargetViewType = ViewType.Any;
            string actionID = "CreditMemoDetail.ShowItemTracking";
            this.CreditMemoDetailShowItemTrackingAction = new 
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.CreditMemoDetailShowItemTrackingAction.
            CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreditMemoDetailShowItemTrackingAction_CustomizePopupWindowParams);
            this.CreditMemoDetailShowItemTrackingAction.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            CreditMemoDetailShowItemTrackingAction_Execute);
        }
        private void 
        CreditMemoDetailShowItemTrackingAction_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _CreditMemoDetail = this.View.CurrentObject as CreditMemoDetail;
            ObjectSpace.CommitChanges();
            CreditMemoDetail thisCreditMemoDetail = _ObjectSpace.GetObjectByKey<
            CreditMemoDetail>(_CreditMemoDetail.Oid);
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "CreditMemoDetail_TrackingLine", false, thisCreditMemoDetail);
            //e.View = Application.CreateListEditor(collectionSource,);
        }
        private void CreditMemoDetailShowItemTrackingAction_Execute(object sender
        , PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_CreditMemoDetail);
            ObjectSpace.Refresh();
        }
    }
}
