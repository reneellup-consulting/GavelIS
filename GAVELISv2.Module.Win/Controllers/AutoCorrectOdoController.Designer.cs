namespace GAVELISv2.Module.Win.Controllers
{
    partial class AutoCorrectOdoController
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
            this.autoCorrectOdoAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // autoCorrectOdoAction
            // 
            this.autoCorrectOdoAction.Caption = "Correct Meter Entries";
            this.autoCorrectOdoAction.Category = "RecordEdit";
            this.autoCorrectOdoAction.ConfirmationMessage = "Do you really want to correct meter entries?";
            this.autoCorrectOdoAction.Id = "AutoCorrectOdoAction";
            this.autoCorrectOdoAction.ImageName = null;
            this.autoCorrectOdoAction.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.autoCorrectOdoAction.Shortcut = null;
            this.autoCorrectOdoAction.Tag = null;
            this.autoCorrectOdoAction.TargetObjectsCriteria = null;
            this.autoCorrectOdoAction.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.FixedAsset);
            this.autoCorrectOdoAction.TargetViewId = null;
            this.autoCorrectOdoAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.autoCorrectOdoAction.ToolTip = null;
            this.autoCorrectOdoAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.autoCorrectOdoAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.autoCorrectOdo_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction autoCorrectOdoAction;
    }
}
