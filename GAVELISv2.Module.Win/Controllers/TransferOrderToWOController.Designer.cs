namespace GAVELISv2.Module.Win.Controllers
{
    partial class TransferOrderToWOController
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
            this.TransferOrderToWOAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // TransferOrderToWOAction
            // 
            this.TransferOrderToWOAction.AcceptButtonCaption = null;
            this.TransferOrderToWOAction.CancelButtonCaption = null;
            this.TransferOrderToWOAction.Caption = "Carryout to WO";
            this.TransferOrderToWOAction.Category = "RecordEdit";
            this.TransferOrderToWOAction.ConfirmationMessage = null;
            this.TransferOrderToWOAction.Id = "dd945078-2abc-47d0-8b3e-47bdedeaaca5";
            this.TransferOrderToWOAction.ImageName = null;
            this.TransferOrderToWOAction.Shortcut = null;
            this.TransferOrderToWOAction.Tag = null;
            this.TransferOrderToWOAction.TargetObjectsCriteria = null;
            this.TransferOrderToWOAction.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.TransferOrderDetail);
            this.TransferOrderToWOAction.TargetViewId = null;
            this.TransferOrderToWOAction.ToolTip = null;
            this.TransferOrderToWOAction.TypeOfView = null;
            this.TransferOrderToWOAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.TransferOrderToWOAction_CustomizePopupWindowParams);
            this.TransferOrderToWOAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.TransferOrderToWOAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction TransferOrderToWOAction;
    }
}
