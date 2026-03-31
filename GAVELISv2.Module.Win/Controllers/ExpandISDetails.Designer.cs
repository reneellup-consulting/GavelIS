namespace GAVELISv2.Module.Win.Controllers
{
    partial class ExpandISDetails
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
            this.ExpandDetailsIS = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // ExpandDetailsIS
            // 
            this.ExpandDetailsIS.Caption = "Expand Details";
            this.ExpandDetailsIS.Category = "View";
            this.ExpandDetailsIS.ConfirmationMessage = null;
            this.ExpandDetailsIS.Id = "ExpandISDetails";
            this.ExpandDetailsIS.ImageName = null;
            this.ExpandDetailsIS.Shortcut = "CtrlE";
            this.ExpandDetailsIS.Tag = null;
            this.ExpandDetailsIS.TargetObjectsCriteria = null;
            this.ExpandDetailsIS.TargetViewId = "IncomeStatementHeader_IncomeStatementDetails_ListView";
            this.ExpandDetailsIS.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.ExpandDetailsIS.ToolTip = null;
            this.ExpandDetailsIS.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.ExpandDetailsIS.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ExpandAccounts_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ExpandDetailsIS;
    }
}
