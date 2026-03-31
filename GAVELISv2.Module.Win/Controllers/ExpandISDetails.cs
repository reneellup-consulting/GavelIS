using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.TreeListEditors.Win;
using DevExpress.XtraTreeList;
using DevExpress.Persistent.Base.General;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class ExpandISDetails : ViewController {
        public ExpandISDetails() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void ExpandAccounts_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            ListView view = (ListView)View;
            TreeListEditor listEditor = (TreeListEditor)view.Editor;
            TreeList treeList = listEditor.TreeList;
            treeList.ExpandAll();
        }
    }
}
