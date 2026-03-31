using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetOnlineRequisitionController : ViewController
    {
        private SimpleAction getOrsAction;
        public GetOnlineRequisitionController()
        {
            this.TargetObjectType = typeof(PurchaseOrder);
            this.TargetViewId = "PurchaseOrder_ListView_OnlineFrs";
            this.TargetViewType = ViewType.ListView;
            string actionID = "GetOrsActionId";
            this.getOrsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.getOrsAction.Caption = "Get Online Request";
            this.getOrsAction.Execute += new SimpleActionExecuteEventHandler(getOrsAction_Execute);
        }
        void getOrsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
