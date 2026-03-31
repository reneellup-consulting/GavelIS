using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using GAVELISv2.Module.Editors;

namespace GAVELISv2.Win.Controls
{
    public partial class BiometricDeviceForm : DevExpress.XtraEditors.XtraForm, IXpoSessionAwareControl
    {
        public BiometricDeviceForm()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            //Initializing a child control in a scenario when it is placed on a form and is not created by XAF.
            if (!DesignMode)
            {
                ((IXpoSessionAwareControl)this.deviceControl1).Session = ((IXpoSessionAwareControl)this).Session;
            }
        }

        Session IXpoSessionAwareControl.Session { get; set; }

    }
}