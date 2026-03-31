namespace GAVELISv2.Module.Win.Controllers
{
    partial class ReceiptLineToWOController
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
            this.receiptLineToWOAction = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            // 
            // receiptLineToWOAction
            // 
            this.receiptLineToWOAction.AcceptButtonCaption = null;
            this.receiptLineToWOAction.CancelButtonCaption = null;
            this.receiptLineToWOAction.Caption = "Carryout to WO";
            this.receiptLineToWOAction.Category = "RecordEdit";
            this.receiptLineToWOAction.ConfirmationMessage = null;
            this.receiptLineToWOAction.Id = "d744f141-833e-499c-bcb5-9d9d4c8a7d83";
            this.receiptLineToWOAction.ImageName = null;
            this.receiptLineToWOAction.Shortcut = null;
            this.receiptLineToWOAction.Tag = null;
            this.receiptLineToWOAction.TargetObjectsCriteria = null;
            this.receiptLineToWOAction.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.ReceiptDetail);
            this.receiptLineToWOAction.TargetViewId = null;
            this.receiptLineToWOAction.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.receiptLineToWOAction.ToolTip = null;
            this.receiptLineToWOAction.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.receiptLineToWOAction.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.receiptLineToWOAction_CustomizePopupWindowParams);
            this.receiptLineToWOAction.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.receiptLineToWOAction_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction receiptLineToWOAction;
    }
}
