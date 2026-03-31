namespace GAVELISv2.Module.Win.Controllers
{
    partial class ExpandAllAccounts
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
            this.ExpandAccounts = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ExpandAccounts
            // 
            this.ExpandAccounts.Caption = "Expand All Accounts";
            this.ExpandAccounts.Category = "View";
            this.ExpandAccounts.ConfirmationMessage = null;
            this.ExpandAccounts.Id = "ExpandAccounts";
            this.ExpandAccounts.ImageName = null;
            this.ExpandAccounts.Shortcut = "CtrlE";
            this.ExpandAccounts.Tag = null;
            this.ExpandAccounts.TargetObjectsCriteria = null;
            this.ExpandAccounts.TargetViewId = "Account_ListView_Tree";
            this.ExpandAccounts.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.ExpandAccounts.ToolTip = null;
            this.ExpandAccounts.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.ExpandAccounts.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExpandAccounts_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ExpandAccounts;
    }
}
