namespace GAVELISv2.Module.Win.Controllers
{
    partial class FillPersonResponsibleController
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
            this.FillPersonResponsibleAction = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // FillPersonResponsibleAction
            // 
            this.FillPersonResponsibleAction.ActionMeaning = DevExpress.ExpressApp.Actions.ActionMeaning.Accept;
            this.FillPersonResponsibleAction.Caption = "Update FA/Responsibility";
            this.FillPersonResponsibleAction.Category = "RecordEdit";
            this.FillPersonResponsibleAction.ConfirmationMessage = "Do you really want to update the selected items?";
            this.FillPersonResponsibleAction.Id = "96202245-5b48-47c3-b65d-fd8362804365";
            this.FillPersonResponsibleAction.ImageName = null;
            this.FillPersonResponsibleAction.Shortcut = null;
            this.FillPersonResponsibleAction.Tag = null;
            this.FillPersonResponsibleAction.TargetObjectsCriteria = null;
            this.FillPersonResponsibleAction.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.CostCenter);
            this.FillPersonResponsibleAction.TargetViewId = null;
            this.FillPersonResponsibleAction.ToolTip = null;
            this.FillPersonResponsibleAction.TypeOfView = null;
            this.FillPersonResponsibleAction.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.FillPersonResponsibleAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction FillPersonResponsibleAction;
    }
}
