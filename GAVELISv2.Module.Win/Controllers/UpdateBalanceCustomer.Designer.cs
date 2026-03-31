namespace GAVELISv2.Module.Win.Controllers
{
    partial class UpdateBalanceCustomer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.UpdateCustomerBalance = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // UpdateCustomerBalance
            // 
            this.UpdateCustomerBalance.Caption = "Update Customer Balance";
            this.UpdateCustomerBalance.Category = "RecordEdit";
            this.UpdateCustomerBalance.ConfirmationMessage = null;
            this.UpdateCustomerBalance.Id = "ed23d61c-5472-4f25-a597-af203d98f330";
            this.UpdateCustomerBalance.ImageName = null;
            this.UpdateCustomerBalance.Shortcut = null;
            this.UpdateCustomerBalance.Tag = null;
            this.UpdateCustomerBalance.TargetObjectsCriteria = null;
            this.UpdateCustomerBalance.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.Customer);
            this.UpdateCustomerBalance.TargetViewId = null;
            this.UpdateCustomerBalance.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.UpdateCustomerBalance.ToolTip = null;
            this.UpdateCustomerBalance.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.UpdateCustomerBalance.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.UpdateCustomerBalance_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction UpdateCustomerBalance;
    }
}
