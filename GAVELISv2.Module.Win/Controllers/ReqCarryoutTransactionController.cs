using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReqCarryoutTransactionController : ViewController
    {
        public ReqCarryoutTransactionController()
        {
            this.TargetObjectType = typeof(ReqCarryoutTransaction);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "RequisitionWorksheet_ReqCarryoutTransactions_ListView";
            this.ViewControlsCreated += new EventHandler(ReqCarryoutTransactionController_ViewControlsCreated);
        }

        void ReqCarryoutTransactionController_ViewControlsCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }

        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            ReqCarryoutTransaction rq = this.View.CurrentObject as ReqCarryoutTransaction;
            if (rq.SourceType.Code == "PO")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "PurchaseOrder_DetailView_ReadOnly", false,rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view; 
            }
            if (rq.SourceType.Code == "PF")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "PurchaseOrderFuel_DetailView_ReadOnly", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
            if (rq.SourceType.Code == "RC")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "Receipt_DetailView_ReadOnly", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
            if (rq.SourceType.Code == "TO")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "TransferOrder_DetailView_ReadOnly", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
            if (rq.SourceType.Code == "WO")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "WorkOrder_DetailView_ReadOnly2", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
            if (rq.SourceType.Code == "JO")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "JobOrder_DetailView_ReadOnly", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
            if (rq.SourceType.Code == "SO")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "SalesOrder_DetailView_ReadOnly", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
            if (rq.SourceType.Code == "ECS")
            {
                DetailView view = Application.CreateDetailView(ObjectSpace, "EmployeeChargeSlip_DetailView_ReadOnly", false, rq.TransactionId);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = view;
            }
        }
    }
}
