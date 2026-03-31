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
    public partial class ClearStanfilcoTripCharges : ViewController {
        private StanfilcoTripStatement stanfilcoTripStatement;
        private SimpleAction clearStanfilcoTripCharges;
        public ClearStanfilcoTripCharges() {
            this.TargetObjectType = typeof(StanfilcoTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearStanfilcoTripCharges = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearStanfilcoTripCharges.Caption = "Clear list";
            this.clearStanfilcoTripCharges.Execute += new 
            SimpleActionExecuteEventHandler(ClearStanfilcoTripCharges_Execute);
            this.clearStanfilcoTripCharges.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearStanfilcoTripCharges_Executed);
            this.clearStanfilcoTripCharges.ConfirmationMessage = 
            "Do you really want to clear the list of charges?";
        }
        private void ClearStanfilcoTripCharges_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            stanfilcoTripStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as StanfilcoTripStatement;
            for (int i = stanfilcoTripStatement.StanfilcoTripCharges.Count - 1; 
            i >= 0; i--) {stanfilcoTripStatement.StanfilcoTripCharges[i].Delete(
                );}
        }
        private void ClearStanfilcoTripCharges_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
