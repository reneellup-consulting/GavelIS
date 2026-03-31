namespace GAVELISv2.Module.Win.Controllers
{
    partial class GenerateStaffPayroll
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                if (generateStaffPayroll != null)
                {
                    generateStaffPayroll.Dispose();
                    generateStaffPayroll = null;
                }
                if (_BgWorker != null)
                {
                    _BgWorker.Dispose();
                    _BgWorker = null;
                }
                if (_FrmProgress != null)
                {
                    _FrmProgress.Dispose();
                    _FrmProgress = null;
                }
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
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
