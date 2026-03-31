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
    public partial class ClearKDCharges : ViewController {
        private KDStatement kdStatement;
        private SimpleAction clearKDCharges;
        public ClearKDCharges() {
            this.TargetObjectType = typeof(KDStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearKDCharges = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearKDCharges.Caption = "Clear list";
            this.clearKDCharges.Execute += new SimpleActionExecuteEventHandler(
            ClearOtherTripCharges_Execute);
            this.clearKDCharges.Executed += new EventHandler<ActionBaseEventArgs
            >(ClearOtherTripCharges_Executed);
            this.clearKDCharges.ConfirmationMessage = 
            "Do you really want to clear the list of charges?";
        }
        private void ClearOtherTripCharges_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            kdStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as KDStatement;
            for (int i = kdStatement.KDCharges.Count - 1; i >= 0; i--) {
                kdStatement.KDCharges[i].Delete();}
        }
        private void ClearOtherTripCharges_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
