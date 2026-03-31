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
    public partial class ClearGensetCharges : ViewController {
        private GensetStatement gensetStatement;
        private SimpleAction clearGensetCharges;
        public ClearGensetCharges() {
            this.TargetObjectType = typeof(GensetStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearGensetCharges = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearGensetCharges.Caption = "Clear list";
            this.clearGensetCharges.Execute += new 
            SimpleActionExecuteEventHandler(ClearOtherTripCharges_Execute);
            this.clearGensetCharges.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearOtherTripCharges_Executed);
            this.clearGensetCharges.ConfirmationMessage = 
            "Do you really want to clear the list of charges?";
        }
        private void ClearOtherTripCharges_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            gensetStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as GensetStatement;
            for (int i = gensetStatement.GensetCharges.Count - 1; i >= 0; i--) {
                gensetStatement.GensetCharges[i].Delete();}
        }
        private void ClearOtherTripCharges_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
