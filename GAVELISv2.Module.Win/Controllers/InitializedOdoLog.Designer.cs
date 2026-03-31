namespace GAVELISv2.Module.Win.Controllers
{
    partial class InitializedOdoLog
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
            this.InitOdoLog = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // InitOdoLog
            // 
            this.InitOdoLog.Caption = "Initialized Odo Log";
            this.InitOdoLog.ConfirmationMessage = null;
            this.InitOdoLog.Id = "d7f00f50-22a2-48fc-a196-35fa06208d91";
            this.InitOdoLog.ImageName = null;
            this.InitOdoLog.Shortcut = null;
            this.InitOdoLog.Tag = null;
            this.InitOdoLog.TargetObjectsCriteria = null;
            this.InitOdoLog.TargetObjectType = typeof(GAVELISv2.Module.BusinessObjects.FATruck);
            this.InitOdoLog.TargetViewId = null;
            this.InitOdoLog.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.InitOdoLog.ToolTip = null;
            this.InitOdoLog.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.InitOdoLog.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.InitOdoLog_Execute);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction InitOdoLog;
    }
}
