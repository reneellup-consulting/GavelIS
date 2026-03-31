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
    public partial class ChangeOriginController : ViewController
    {
        private PopupWindowShowAction selectOrigin;
        private WorkOrderItemDetail _WoItemDetail;
        public ChangeOriginController()
        {
            this.TargetObjectType = typeof(WorkOrderItemDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = "WorkOrderItemDetail.ChangeOrigin";
            this.selectOrigin = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.selectOrigin.Caption = "Change Origin";
            this.selectOrigin.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            selectOrigin_CustomizePopupWindowParams);
            this.selectOrigin.Execute += new
            PopupWindowShowActionExecuteEventHandler(selectOrigin_Execute
            );
        }
        private void selectOrigin_CustomizePopupWindowParams(object
        sender, CustomizePopupWindowParamsEventArgs e)
        {
            _WoItemDetail = this.View.
            CurrentObject as WorkOrderItemDetail;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PartsOrigin_ListView";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(PartsOrigin),
            listViewId);
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void selectOrigin_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            PartsOrigin porigin = _WoItemDetail.Session.GetObjectByKey<PartsOrigin>(((
            PartsOrigin)e.PopupWindow.View.SelectedObjects[0]).Oid);
            _WoItemDetail.Origin = porigin;
        }
    }
}
