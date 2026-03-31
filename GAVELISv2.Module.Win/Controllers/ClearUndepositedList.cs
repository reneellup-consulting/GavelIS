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
    public partial class ClearUndepositedList : ViewController {
        private Deposit deposit;
        private SimpleAction clearUndepositedList;
        public ClearUndepositedList() {
            this.TargetObjectType = typeof(Deposit);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ClearCharges", this.GetType().
            Name);
            this.clearUndepositedList = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearUndepositedList.Caption = "Clear list";
            this.clearUndepositedList.Execute += new 
            SimpleActionExecuteEventHandler(ClearUndepositedList_Execute);
            this.clearUndepositedList.Executed += new EventHandler<
            ActionBaseEventArgs>(ClearUndepositedList_Executed);
            this.clearUndepositedList.ConfirmationMessage = 
            "Do you really want to clear the list?";
        }
        private void ClearUndepositedList_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            deposit = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Deposit;
            for (int i = deposit.DepositDetails.Count - 1; i >= 0; i--) {deposit
                .DepositDetails[i].Delete();}
            deposit.UpdateCount(true);
            deposit.UpdateTotalDeposit(true);
        }
        private void ClearUndepositedList_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
