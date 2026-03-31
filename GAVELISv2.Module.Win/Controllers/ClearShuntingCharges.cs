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
    public partial class ClearShuntingCharges : ViewController {
        private ShuntingStatement shuntingStatement;
        private SimpleAction clearShuntingCharges;
        public ClearShuntingCharges() {
            this.TargetObjectType = typeof(ShuntingStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearShuntingCharges = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearShuntingCharges.Caption = "Clear list";
            this.clearShuntingCharges.Execute += new 
            SimpleActionExecuteEventHandler(ClearOtherTripCharges_Execute);
            this.clearShuntingCharges.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearOtherTripCharges_Executed);
            this.clearShuntingCharges.ConfirmationMessage = 
            "Do you really want to clear the list of charges?";
        }
        private void ClearOtherTripCharges_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            shuntingStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ShuntingStatement;
            for (int i = shuntingStatement.ShuntingCharges.Count - 1; i >= 0; i
            --) {shuntingStatement.ShuntingCharges[i].Delete();}
        }
        private void ClearOtherTripCharges_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
