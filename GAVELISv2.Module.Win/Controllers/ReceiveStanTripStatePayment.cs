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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class ReceiveStanTripStatePayment : ViewController {
        private StanfilcoTripStatement stanTripStatement;
        private SimpleAction receiveStanTripStatePayment;
        private NewObjectViewController newController;
        private DetailView payBillDetailView;
        public ReceiveStanTripStatePayment() {
            this.TargetObjectType = typeof(StanfilcoTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReceivePayment", this.GetType()
            .Name);
            this.receiveStanTripStatePayment = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.receiveStanTripStatePayment.Caption = "Receive Payment";
            this.receiveStanTripStatePayment.Execute += new 
            SimpleActionExecuteEventHandler(ReceiveStanTripStatePayment_Execute)
            ;
            //this.receiveStanTripStatePayment.Executed += new EventHandler<
            //ActionBaseEventArgs>(ReceiveStanTripStatePayment_Executed);
        }
        private void ReceiveStanTripStatePayment_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            Frame workFrame = Application.CreateFrame(TemplateContext.
            ApplicationWindow);
            workFrame.SetView(Application.CreateListView(Application.
            CreateObjectSpace(), typeof(ReceivePayment), true));
            newController = workFrame.GetController<NewObjectViewController>();
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
        //private void ReceiveStanTripStatePayment_Executed(object sender,
        //ActionBaseEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}
        private ChoiceActionItem FindNewObjectItem(Type type) {
            foreach (ChoiceActionItem item in newController.NewObjectAction.
            Items) {if (item.Data == type) {return item;}}
            return null;
        }
        private void NewObjectAction_Executed(object sender, ActionBaseEventArgs 
        e) {
            payBillDetailView = e.ShowViewParameters.CreatedView as DetailView;
            //Cancel showing the default View by the NewObjectAction
            e.ShowViewParameters.CreatedView = null;
        }
    }
}
