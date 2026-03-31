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
    public partial class SelectAllUndeposited : ViewController {
        private Deposit deposit;
        private SimpleAction clearUndepositedList;
        public SelectAllUndeposited() {
            this.TargetObjectType = typeof(Deposit);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.SelectAll", this.GetType().Name
            );
            this.clearUndepositedList = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.clearUndepositedList.Caption = "Select All";
            this.clearUndepositedList.Execute += new 
            SimpleActionExecuteEventHandler(SelectAllUndeposited_Execute);
            this.clearUndepositedList.Executed += new EventHandler<
            ActionBaseEventArgs>(SelectAllUndeposited_Executed);
            //this.clearUndepositedList.ConfirmationMessage = 
            //"Do you really want to clear the list?";
        }
        private void SelectAllUndeposited_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            deposit = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Deposit;
            for (int i = deposit.DepositDetails.Count - 1; i >= 0; i--) {deposit
                .DepositDetails[i].Select = true;}
            deposit.UpdateCount(true);
            deposit.UpdateTotalDeposit(true);
        }
        private void SelectAllUndeposited_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(stanfilcoTripStatement);
            //ObjectSpace.Refresh();
        }
    }
}
