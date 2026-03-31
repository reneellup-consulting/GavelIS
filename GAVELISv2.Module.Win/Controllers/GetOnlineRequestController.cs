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
using DevExpress.ExpressApp.Workflow;
using DevExpress.ExpressApp.Workflow.Xpo;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetOnlineRequestController : ViewController
    {
        private SimpleAction getOnlineRequestsAction;
        public GetOnlineRequestController()
        {
            this.TargetObjectType = typeof(OfrsBuffer);
            this.getOnlineRequestsAction = new SimpleAction(this, "GetOnlineRequestActionID",
            PredefinedCategory.RecordEdit);
            this.getOnlineRequestsAction.Caption = "Get Online Requests";
            this.getOnlineRequestsAction.Execute += new SimpleActionExecuteEventHandler(getOnlineRequestsAction_Execute);
        }

        void getOnlineRequestsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var criteriaOperator = CriteriaOperator.Parse("[Name]='Create PO from OFRS Buffer'");
            IWorkflowDefinition definition = View.ObjectSpace.FindObject<XpoWorkflowDefinition>(criteriaOperator);
            OfrsBuffer buff = this.View.CurrentObject as OfrsBuffer;
            if (definition != null && buff != null)
            {
                var request = new XpoStartWorkflowRequest(((ObjectSpace)ObjectSpace).Session);
                request.TargetObjectKey = buff.Oid;
                request.TargetWorkflowUniqueId = definition.GetUniqueId();
            }
        }
    }
}
