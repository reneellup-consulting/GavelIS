namespace GAVELISv2.Module.Win.Controllers
{
    partial class CalculatePartialPayController
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
            this.CalculatePartialPayAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // CalculatePartialPayAction
            // 
            this.CalculatePartialPayAction.Caption = "Calculate Partial Pay";
            this.CalculatePartialPayAction.Category = "RecordEdit";
            this.CalculatePartialPayAction.ConfirmationMessage = null;
            this.CalculatePartialPayAction.Id = "CalculatePartialPayActionId";
            this.CalculatePartialPayAction.ImageName = null;
            this.CalculatePartialPayAction.Shortcut = null;
            this.CalculatePartialPayAction.Tag = null;
            this.CalculatePartialPayAction.TargetObjectsCriteria = null;
            this.CalculatePartialPayAction.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.DriverRegistry);
            this.CalculatePartialPayAction.TargetViewId = null;
            this.CalculatePartialPayAction.ToolTip = null;
            this.CalculatePartialPayAction.TypeOfView = null;
            this.CalculatePartialPayAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.CalculatePartialPayAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction CalculatePartialPayAction;
    }
}
