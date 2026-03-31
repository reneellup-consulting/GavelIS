namespace GAVELISv2.Module.Win.Controllers
{
    partial class UpdateVendorBalance
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
            this.Update_VendorBalance = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // Update_VendorBalance
            // 
            this.Update_VendorBalance.Caption = "Update Vendor Balance";
            this.Update_VendorBalance.ConfirmationMessage = null;
            this.Update_VendorBalance.Id = "dc7b6a59-d058-4f6b-8411-038a18f6c304";
            this.Update_VendorBalance.ImageName = null;
            this.Update_VendorBalance.Shortcut = null;
            this.Update_VendorBalance.Tag = null;
            this.Update_VendorBalance.TargetObjectsCriteria = null;
            this.Update_VendorBalance.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.Vendor);
            this.Update_VendorBalance.TargetViewId = null;
            this.Update_VendorBalance.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.Update_VendorBalance.ToolTip = null;
            this.Update_VendorBalance.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.Update_VendorBalance.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.Update_VendorBalance_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction Update_VendorBalance;
    }
}
