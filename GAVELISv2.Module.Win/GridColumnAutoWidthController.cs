using System;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;

namespace GAVELISv2.Module.Win
{
    public partial class GridColumnAutoWidthController : ViewController<ListView> 
    {
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ModelSaved += View_InfoSynchronized;
        }
        protected override void OnDeactivated()
        {
            View.ModelSaved -= View_InfoSynchronized;
            base.OnDeactivated();
        }
        private void View_InfoSynchronized(object sender, EventArgs e)
        {
            IModelListViewExtender modelListView = View.Model as
            IModelListViewExtender;
            if (modelListView != null && modelListView.IsDisableColumnAutoWidth)
            {
                GridListEditor gridListEditor = View.Editor as GridListEditor;
                if (gridListEditor != null)
                {
                    GridView gridView = gridListEditor.GridView;
                    if (gridView != null)
                    {
                        gridView.OptionsView.ColumnAutoWidth = false;
                    }
                }
            }
            else
            {
                GridListEditor gridListEditor = View.Editor as GridListEditor;
                if (gridListEditor != null)
                {
                    GridView gridView = gridListEditor.GridView;
                    if (gridView != null)
                    {
                        gridView.OptionsView.ColumnAutoWidth = true;
                    }
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            IModelListViewExtender modelListView = View.Model as
            IModelListViewExtender;
            if (modelListView != null && modelListView.IsDisableColumnAutoWidth)
            {
                GridListEditor gridListEditor = View.Editor as GridListEditor;
                if (gridListEditor != null)
                {
                    GridView gridView = gridListEditor.GridView;
                    gridView.OptionsView.ColumnAutoWidth = false;
                }
            }
            else
            {
                GridListEditor gridListEditor = View.Editor as GridListEditor;
                if (gridListEditor != null)
                {
                    GridView gridView = gridListEditor.GridView;
                    gridView.OptionsView.ColumnAutoWidth = true;
                }
            }
        }
    }
}
