using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CashflowListViewController : ViewController
    {
        public CashflowListViewController()
        {
            this.TargetObjectType = typeof(CashflowSummary);
            this.TargetViewType = ViewType.ListView;
            this.ViewControlsCreated += new EventHandler(ListViewController_ViewControlsCreated);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            //View.ControlsCreated += new EventHandler(View_ControlsCreated);
        }

        //void View_ControlsCreated(object sender, EventArgs e)
        //{
        //    GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
        //    if (listEditor != null)
        //    {
        //        GridView gridView = listEditor.GridView;
        //        if (gridView.Columns.Count > 0)
        //        {
        //            GridColumn gridColumn;
        //            for (int i = 0; i < 16; i++)
        //            {
        //                gridColumn = gridView.Columns[i];
        //                if (gridColumn != null)
        //                {
        //                    gridColumn.Fixed = FixedStyle.Left;
        //                }
        //            }
        //        }
        //    }
        //}

        void ListViewController_ViewControlsCreated(object sender, EventArgs e)
        {
            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            if (listEditor != null)
            {
                GridView gridView = listEditor.GridView;
                gridView.OptionsView.ColumnAutoWidth = false;
            }
        }
    }
}
