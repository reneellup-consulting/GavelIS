namespace GAVELISv2.Win.Controls
{
    partial class BiometricDeviceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.deviceControl1 = new GAVELISv2.Win.Controls.DeviceControl();
            this.SuspendLayout();
            // 
            // deviceControl1
            // 
            this.deviceControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceControl1.Location = new System.Drawing.Point(0, 0);
            this.deviceControl1.Name = "deviceControl1";
            this.deviceControl1.Size = new System.Drawing.Size(1224, 651);
            this.deviceControl1.TabIndex = 0;
            // 
            // BiometricDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1224, 651);
            this.Controls.Add(this.deviceControl1);
            this.LookAndFeel.SkinName = "Sharp Plus";
            this.Name = "BiometricDeviceForm";
            this.Text = "Biometric Device";
            this.ResumeLayout(false);

        }

        #endregion

        private DeviceControl deviceControl1;
    }
}