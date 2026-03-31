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
    public partial class ClearDolefilTripCharges : ViewController {
        private DolefilTripStatement dolefilTripStatement;
        private SimpleAction clearStanfilcoTripCharges;
        public ClearDolefilTripCharges() {
            this.TargetObjectType = typeof(DolefilTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearStanfilcoTripCharges = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearStanfilcoTripCharges.Caption = "Clear list";
            this.clearStanfilcoTripCharges.Execute += new 
            SimpleActionExecuteEventHandler(ClearDolefilTripCharges_Execute);
            this.clearStanfilcoTripCharges.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearDolefilTripCharges_Executed);
            this.clearStanfilcoTripCharges.ConfirmationMessage = 
            "Do you really want to clear the list of charges?";
        }
        private void ClearDolefilTripCharges_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            dolefilTripStatement = ((DevExpress.ExpressApp.DetailView)this.View)
            .CurrentObject as DolefilTripStatement;
            for (int i = dolefilTripStatement.DolefilTripCharges.Count - 1; i >= 
            0; i--) {dolefilTripStatement.DolefilTripCharges[i].Delete();}
        }
        private void ClearDolefilTripCharges_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
