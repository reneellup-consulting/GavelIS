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
    public partial class ClearOtherTripCharges : ViewController {
        private OtherTripStatement otherTripStatement;
        private SimpleAction clearStanfilcoTripCharges;
        public ClearOtherTripCharges() {
            this.TargetObjectType = typeof(OtherTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearStanfilcoTripCharges = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearStanfilcoTripCharges.Caption = "Clear list";
            this.clearStanfilcoTripCharges.Execute += new 
            SimpleActionExecuteEventHandler(ClearOtherTripCharges_Execute);
            this.clearStanfilcoTripCharges.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearOtherTripCharges_Executed);
            this.clearStanfilcoTripCharges.ConfirmationMessage = 
            "Do you really want to clear the list of charges?";
        }
        private void ClearOtherTripCharges_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            otherTripStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as OtherTripStatement;
            for (int i = otherTripStatement.OtherTripCharges.Count - 1; i >= 0; 
            i--) {otherTripStatement.OtherTripCharges[i].Delete();}
        }
        private void ClearOtherTripCharges_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
