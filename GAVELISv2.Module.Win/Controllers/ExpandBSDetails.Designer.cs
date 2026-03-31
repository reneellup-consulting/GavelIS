namespace GAVELISv2.Module.Win.Controllers
{
    partial class ExpandBSDetails
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
            this.ExpandDetailsBS = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ExpandDetailsBS
            // 
            this.ExpandDetailsBS.Caption = "Expand Details";
            this.ExpandDetailsBS.Category = "View";
            this.ExpandDetailsBS.ConfirmationMessage = null;
            this.ExpandDetailsBS.Id = "ExpandBSDetails";
            this.ExpandDetailsBS.ImageName = null;
            this.ExpandDetailsBS.Shortcut = "CtrlE";
            this.ExpandDetailsBS.Tag = null;
            this.ExpandDetailsBS.TargetObjectsCriteria = null;
            this.ExpandDetailsBS.TargetViewId = "BalanceSheetHeader_BalanceSheetDetails_ListView";
            this.ExpandDetailsBS.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.ExpandDetailsBS.ToolTip = null;
            this.ExpandDetailsBS.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.ExpandDetailsBS.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExpandAccounts_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ExpandDetailsBS;
    }
}
