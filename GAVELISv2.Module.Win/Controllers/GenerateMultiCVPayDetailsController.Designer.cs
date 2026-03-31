namespace GAVELISv2.Module.Win.Controllers
{
    partial class GenerateMultiCVPayDetailsController
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
            this.GenerateMultiCVPayDetailsAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // GenerateMultiCVPayDetailsAction
            // 
            this.GenerateMultiCVPayDetailsAction.Caption = "Generate Pay Details";
            this.GenerateMultiCVPayDetailsAction.Category = "RecordEdit";
            this.GenerateMultiCVPayDetailsAction.ConfirmationMessage = null;
            this.GenerateMultiCVPayDetailsAction.Id = "GenerateMultiCVPayDetailsActionId";
            this.GenerateMultiCVPayDetailsAction.ImageName = null;
            this.GenerateMultiCVPayDetailsAction.Shortcut = null;
            this.GenerateMultiCVPayDetailsAction.Tag = null;
            this.GenerateMultiCVPayDetailsAction.TargetObjectsCriteria = null;
            this.GenerateMultiCVPayDetailsAction.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.MultiCheckVoucher);
            this.GenerateMultiCVPayDetailsAction.TargetViewId = null;
            this.GenerateMultiCVPayDetailsAction.TargetViewType = DevExpress.ExpressApp.ViewType.DetailView;
            this.GenerateMultiCVPayDetailsAction.ToolTip = null;
            this.GenerateMultiCVPayDetailsAction.TypeOfView = typeof(DevExpress.ExpressApp.DetailView);
            this.GenerateMultiCVPayDetailsAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.GenerateMultiCVPayDetailsAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction GenerateMultiCVPayDetailsAction;
    }
}
