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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TreeListController : ViewController
    {
        public TreeListController()
        {
            TargetViewType = ViewType.ListView;
            TargetObjectType = typeof(ITreeNode);
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            ListView view = (ListView)View;
            if (view.Editor.GetType() == typeof(TreeListEditor))
            {
                TreeListEditor listEditor = (TreeListEditor)view.Editor;
                TreeList treeList = listEditor.TreeList;
                treeList.OptionsView.ShowHorzLines = true;
                treeList.OptionsView.ShowIndicator = true;
                treeList.OptionsView.ShowIndentAsRowStyle = true;
            }
        }

    }
}
