using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiveFuelStatementPayment : ViewController
    {
        private FuelStatementOfAccount fuelStatement;
        private SimpleAction receiveFuelStatementPayment;
        private NewObjectViewController newController;
        private DetailView payBillDetailView;
        public ReceiveFuelStatementPayment()
        {
            this.TargetObjectType = typeof(FuelStatementOfAccount);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReceivePayment", this.GetType()
            .Name);
            this.receiveFuelStatementPayment = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.receiveFuelStatementPayment.Caption = "Receive Payment";
            this.receiveFuelStatementPayment.Execute += new SimpleActionExecuteEventHandler(receiveFuelStatementPayment_Execute);
        }

        void receiveFuelStatementPayment_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            // Get the current fuel statement from the view
            fuelStatement = (FuelStatementOfAccount)View.CurrentObject;

            Frame workFrame = Application.CreateFrame(TemplateContext.
            ApplicationWindow);
            workFrame.SetView(Application.CreateListView(Application.
            CreateObjectSpace(), typeof(ReceivePayment), true));
            newController = workFrame.GetController<NewObjectViewController>();
            if (newController != null)
            {
                ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                ReceivePayment));
                if (newObjectItem != null)
                {
                    newController.NewObjectAction.Executed += new EventHandler<ActionBaseEventArgs>(NewObjectAction_Executed);
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

        void NewObjectAction_Executed(object sender, ActionBaseEventArgs e)
        {
            payBillDetailView = e.ShowViewParameters.CreatedView as DetailView;
            
            // TODO: Populate the newObjectItem.ReferenceNo with fuelStatement.StatementNo - COMPLETED
            if (payBillDetailView != null && payBillDetailView.CurrentObject is ReceivePayment)
            {
                object currentObject = payBillDetailView.CurrentObject;
                ReceivePayment receivePayment = (ReceivePayment)currentObject;
                // Set the reference number to the fuel statement number
                receivePayment.ReferenceNo = fuelStatement.StatementNo;

                Customer customerFromSameSession = receivePayment.Session.GetObjectByKey<Customer>(fuelStatement.Customer.Oid);

                receivePayment.ReceiveFrom = customerFromSameSession;
                receivePayment.CheckAmount = fuelStatement.SelectedCharges.Value;
                
                // Update the view to reflect the changes
                payBillDetailView.Refresh();
            }
            
            //Cancel showing the default View by the NewObjectAction
            e.ShowViewParameters.CreatedView = null;
        }

        private ChoiceActionItem FindNewObjectItem(Type type)
        {
            foreach (ChoiceActionItem item in newController.NewObjectAction.
            Items) { if (item.Data == type) { return item; } }
            return null;
        }
    }
}
