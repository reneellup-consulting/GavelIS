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
using DevExpress.XtraGrid;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TruckIncomeExpenseDetailViewController : ViewController
    {
        public TruckIncomeExpenseDetailViewController()
        {
            this.TargetObjectType = typeof(TruckIncomeExpenseDetail);
            this.TargetViewType = ViewType.ListView;
            this.ViewControlsCreated += new EventHandler(TruckIncomeExpenseDetailViewController_ViewControlsCreated);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated += new EventHandler(View_ControlsCreated);
        }
        void View_ControlsCreated(object sender, EventArgs e)
        {
            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            if (listEditor != null)
            {
                GridView gridView = listEditor.GridView;
                if (gridView.Columns.Count > 0)
                {
                    GridColumn gridColumn;
                    //for (int i = 0; i < gridView.Columns.Count; i++)
                    //{
                    //    gridColumn = gridView.Columns[i];
                    //    if (gridColumn != null)
                    //    {
                    //        Console.WriteLine("{0} {1}", i, gridColumn.FieldName);
                    //    }
                    //}
                    for (int i = 0; i < 6; i++)
                    {
                        gridColumn = gridView.Columns[i];
                        if (gridColumn != null)
                        {
                            gridColumn.Fixed = FixedStyle.Left;
                        }
                    }
                    gridView.Columns[45].Fixed = FixedStyle.Right;
                    gridView.Columns[44].Fixed = FixedStyle.Right;
                    gridView.Columns[43].Fixed = FixedStyle.Right;
                    gridView.Columns[42].Fixed = FixedStyle.Right;
                }

            }
        }
        void TruckIncomeExpenseDetailViewController_ViewControlsCreated(object sender, EventArgs e)
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
