using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace GAVELISv2.Module.Win
{
    public partial class ProgressForm : DevExpress.XtraEditors.XtraForm {
        public event EventHandler CancelClick = null;
        public ProgressForm() { InitializeComponent(); }
        public ProgressForm(string caption, int recordCount, string message) {
            InitializeComponent();
            Text = caption;
            progress.Properties.Maximum = recordCount;
            _totalRecords = recordCount;
            _MessageTemplate = message;
        }
        public void ChangeRecordCount(int recordCount) {
            progress.Properties.Maximum = recordCount;
            _totalRecords = recordCount;
        }
        private readonly string _MessageTemplate = string.Empty;
        private int _totalRecords;
        private int _current;
        private void SetLabelText() { labelControl1.Text = string.IsNullOrEmpty(
            _MessageTemplate) ? string.Empty : string.Format(_MessageTemplate, 
            _current, _totalRecords); }
        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }
        private void btnCancel_Click(object sender, EventArgs e) {
            if (CancelClick == null) {return;}
            Close();
            CancelClick(sender, e);
        }
        public void DoProgress() { DoProgress(1); }
        public void DoProgress(int i) {
            _current += i;
            progress.Increment(i);
            SetLabelText();
        }
    }
}
