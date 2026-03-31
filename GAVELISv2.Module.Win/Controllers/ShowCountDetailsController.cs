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
    public partial class ShowCountDetailsController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction
        ShowCountDetailsAction;
        private ItemsMovementGroup _ItemsMovementGroup;
        public ShowCountDetailsController()
        {
            this.TargetObjectType = typeof(ItemsMovementGroup);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "ShowCountDetailsActionId";
            this.ShowCountDetailsAction = new
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.ShowCountDetailsAction.Caption = "Show Count Details";
            this.ShowCountDetailsAction.
            CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            ShowCountDetailsAction_CustomizePopupWindowParams
            );
            this.ShowCountDetailsAction.Execute += new
            PopupWindowShowActionExecuteEventHandler(
            ShowCountDetailsAction_Execute);
        }
        private void
        ShowCountDetailsAction_CustomizePopupWindowParams
        (object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ItemsMovementGroup = this.View.CurrentObject as
            ItemsMovementGroup;
            ObjectSpace.CommitChanges();
            ItemsMovementGroup thisItemsMovementGroup = _ObjectSpace
            .GetObjectByKey<ItemsMovementGroup>(_ItemsMovementGroup.
            Oid);
            e.View = Application.CreateDetailView(_ObjectSpace,
            "ItemsMovementGroup_ShowCountDetails", false,
            thisItemsMovementGroup);
        }
        private void ShowCountDetailsAction_Execute(
        object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(_ItemsMovementGroup);
            //ObjectSpace.Refresh();
        }
    }
}
