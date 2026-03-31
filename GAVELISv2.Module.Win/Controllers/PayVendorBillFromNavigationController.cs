using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Reports;
using GAVELISv2.Module.BusinessObjects;
using System.IO;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class PayVendorBillFromNavigationController : 
    ShowNavigationItemController {
        private NewObjectViewController newController;
        private DetailView payBillDetailView;
        private const string PayBillObjectItemId = "PayVendorBill";
        private const string CreatePaymentItemId = "CreatePayment";
        private const string ReceivePaymentItemId = "ReceivePayment";
        private const string InvoiceReconItemId = "InvoiceReconciliation";
        private const string attendanceCalculatorId = "AttendanceCalculator";
        //private const string PayBillObjectNavigationItemActiveKey = 
        //"PayVendorBillAllowed";
        //private const string PayBillObjectNavigationItemDefaultPath = 
        //    "Issues/" + PayBillObjectItemId;
        public PayVendorBillFromNavigationController() { TargetWindowType = 
            WindowType.Main; }
        protected override void InitializeItems() {
            base.InitializeItems();
            //ChoiceActionItem newNavigationItem = FindNewNavigationItem();
            //if(newNavigationItem != null)
            //    newNavigationItem.Active[PayBillObjectNavigationItemActiveKey] = 
            //        SecuritySystem.IsGranted(new ObjectAccessPermission(
            //        typeof(PayBill), ObjectAccess.Create));
        }
        protected override void ShowNavigationItem(
        SingleChoiceActionExecuteEventArgs e) {
            // Previewing Reports
            XafReport rep = new XafReport();
            if (e.SelectedChoiceActionItem != null)
            {
                if (e.SelectedChoiceActionItem.Id == "@0b91bbc7-e798-4a58-ad57-65e436b42f12")
                {
                    string path = Directory.GetCurrentDirectory() + @"\VendorAgingSummary.repx";
                    rep.LoadLayout(path);
                    rep.ObjectSpace = Application.CreateObjectSpace();
                    //rep.DataSource = PurchaseOrderDetails;
                    rep.ShowPreview();
                    return;
                }

                if (e.SelectedChoiceActionItem.Id == "@0c82e772-dac6-4c4d-ad15-a629fc202df5")
                {
                    string path = Directory.GetCurrentDirectory() + @"\VendorBalancetoDate.repx";
                    rep.LoadLayout(path);
                    rep.ObjectSpace = Application.CreateObjectSpace();
                    //rep.DataSource = PurchaseOrderDetails;
                    rep.ShowPreview();
                    return;
                }

            }
            if ((e.SelectedChoiceActionItem != null) && e.
            SelectedChoiceActionItem.Enabled.ResultValue && e.
            SelectedChoiceActionItem.Id == PayBillObjectItemId) {
                Frame workFrame = Application.CreateFrame(TemplateContext.
                ApplicationWindow);
                workFrame.SetView(Application.CreateListView(Application.
                CreateObjectSpace(), typeof(PayBill), true));
                newController = workFrame.GetController<NewObjectViewController>
                ();
                if (newController != null) {
                    ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                    PayBill));
                    if (newObjectItem != null) {
                        newController.NewObjectAction.Executed += 
                        NewObjectAction_Executed;
                        newController.NewObjectAction.DoExecute(newObjectItem);
                        newController.NewObjectAction.Executed -= 
                        NewObjectAction_Executed;
                        e.ShowViewParameters.TargetWindow = TargetWindow.Current;
                        e.ShowViewParameters.CreatedView = payBillDetailView;
                        //Cancel the default processing for this navigation item.
                        return;
                    }
                }
            }
            if ((e.SelectedChoiceActionItem != null) && e.
            SelectedChoiceActionItem.Enabled.ResultValue && e.
            SelectedChoiceActionItem.Id == CreatePaymentItemId) {
                Frame workFrame = Application.CreateFrame(TemplateContext.
                ApplicationWindow);
                workFrame.SetView(Application.CreateListView(Application.
                CreateObjectSpace(), typeof(CheckPayment), true));
                newController = workFrame.GetController<NewObjectViewController>
                ();
                if (newController != null) {
                    ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                    CheckPayment));
                    if (newObjectItem != null) {
                        newController.NewObjectAction.Executed += 
                        NewObjectAction_Executed;
                        newController.NewObjectAction.DoExecute(newObjectItem);
                        newController.NewObjectAction.Executed -= 
                        NewObjectAction_Executed;
                        e.ShowViewParameters.TargetWindow = TargetWindow.
                        NewModalWindow;
                        e.ShowViewParameters.CreatedView = payBillDetailView;
                        //Cancel the default processing for this navigation item.
                        return;
                    }
                }
            }
            if ((e.SelectedChoiceActionItem != null) && e.
            SelectedChoiceActionItem.Enabled.ResultValue && e.
            SelectedChoiceActionItem.Id == ReceivePaymentItemId) {
                Frame workFrame = Application.CreateFrame(TemplateContext.
                ApplicationWindow);
                workFrame.SetView(Application.CreateListView(Application.
                CreateObjectSpace(), typeof(ReceivePayment), true));
                newController = workFrame.GetController<NewObjectViewController>
                ();
                if (newController != null) {
                    ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                    ReceivePayment));
                    if (newObjectItem != null) {
                        newController.NewObjectAction.Executed += 
                        NewObjectAction_Executed;
                        newController.NewObjectAction.DoExecute(newObjectItem);
                        newController.NewObjectAction.Executed -= 
                        NewObjectAction_Executed;
                        e.ShowViewParameters.TargetWindow = TargetWindow.
                        NewModalWindow;
                        e.ShowViewParameters.CreatedView = payBillDetailView;
                        //Cancel the default processing for this navigation item.
                        return;
                    }
                }
            }
            if ((e.SelectedChoiceActionItem != null) && e.
            SelectedChoiceActionItem.Enabled.ResultValue && e.
            SelectedChoiceActionItem.Id == InvoiceReconItemId) {
                Frame workFrame = Application.CreateFrame(TemplateContext.
                ApplicationWindow);
                workFrame.SetView(Application.CreateListView(Application.
                CreateObjectSpace(), typeof(InvoiceReconciliation), true));
                newController = workFrame.GetController<NewObjectViewController>
                ();
                if (newController != null) {
                    ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                    InvoiceReconciliation));
                    if (newObjectItem != null) {
                        newController.NewObjectAction.Executed += 
                        NewObjectAction_Executed;
                        newController.NewObjectAction.DoExecute(newObjectItem);
                        newController.NewObjectAction.Executed -= 
                        NewObjectAction_Executed;
                        e.ShowViewParameters.TargetWindow = TargetWindow.Current;
                        e.ShowViewParameters.CreatedView = payBillDetailView;
                        //Cancel the default processing for this navigation item.
                        return;
                    }
                }
            }

            if ((e.SelectedChoiceActionItem != null) && e.
                SelectedChoiceActionItem.Enabled.ResultValue && e.
                SelectedChoiceActionItem.Id == "AdjustItemCostPrices_ListView")
            {
                Frame workFrame = Application.CreateFrame(TemplateContext.
                ApplicationWindow);
                workFrame.SetView(Application.CreateListView(Application.
                CreateObjectSpace(), typeof(AdjustItemCostPrices), true));
                newController = workFrame.GetController<NewObjectViewController>
                ();
                if (newController != null)
                {
                    ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                    AdjustItemCostPrices));
                    if (newObjectItem != null)
                    {
                        newController.NewObjectAction.Executed +=
                        NewObjectAction_Executed;
                        newController.NewObjectAction.DoExecute(newObjectItem);
                        newController.NewObjectAction.Executed -=
                        NewObjectAction_Executed;
                        e.ShowViewParameters.TargetWindow = TargetWindow.Current;
                        e.ShowViewParameters.CreatedView = payBillDetailView;
                        //Cancel the default processing for this navigation item.
                        return;
                    }
                }
            }

            //Continue the default processing for other navigation items.
            base.ShowNavigationItem(e);
        }
        private ChoiceActionItem FindNewObjectItem(Type type) {
            foreach (ChoiceActionItem item in newController.NewObjectAction.
            Items) {if (item.Data == type) {return item;}}
            return null;
        }
        //private ChoiceActionItem FindNewNavigationItem() {
        //    return ShowNavigationItemAction.FindItemByIdPath(PayBillObjectNavigationItemDefaultPath);
        //}
        private void NewObjectAction_Executed(object sender, ActionBaseEventArgs 
        e) {
            payBillDetailView = e.ShowViewParameters.CreatedView as DetailView;
            //Cancel showing the default View by the NewObjectAction
            e.ShowViewParameters.CreatedView = null;
        }
    }
}
