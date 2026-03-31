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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ClearFuelStatementCharges : ViewController
    {
        private FuelStatementOfAccount fuelStatementOfAccount;
        private SimpleAction clearFuelStatementCharges;
        public ClearFuelStatementCharges()
        {
            this.TargetObjectType = typeof(FuelStatementOfAccount);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("ClearFuelStatementCharges", this.GetType().
            Name);
            this.clearFuelStatementCharges = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.clearFuelStatementCharges.Caption = "Clear list";
            this.clearFuelStatementCharges.Execute += new SimpleActionExecuteEventHandler(clearFuelStatementCharges_Execute);
            this.clearFuelStatementCharges.Executed += new EventHandler<ActionBaseEventArgs>(clearFuelStatementCharges_Executed);
            this.clearFuelStatementCharges.ConfirmationMessage =
            "Do you really want to clear the list of charges?";
        }

        void clearFuelStatementCharges_Executed(object sender, ActionBaseEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void clearFuelStatementCharges_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            fuelStatementOfAccount = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as FuelStatementOfAccount;
            for (int i = fuelStatementOfAccount.FuelStatementOfAccountDetails.Count - 1;
            i >= 0; i--)
            {
                fuelStatementOfAccount.FuelStatementOfAccountDetails[i].Delete(
                    );
            }
        }
    }
}
