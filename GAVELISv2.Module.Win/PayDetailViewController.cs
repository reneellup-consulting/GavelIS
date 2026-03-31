using System;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win
{
    public partial class PayDetailViewController : ViewController<ListView>
    {
        public PayDetailViewController()
        {
            this.TargetObjectType = typeof(StaffPayrollPayDetail);
            this.TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            View.ModelSaved += new EventHandler(View_InfoSynchronized);
        }
        protected override void OnDeactivated()
        {
            View.ModelSaved -= View_InfoSynchronized;
            base.OnDeactivated();
        }
        void View_InfoSynchronized(object sender, EventArgs e)
        {
            GridListEditor gridListEditor = View.Editor as GridListEditor;
            if (gridListEditor != null)
            {
                GridView gridView = gridListEditor.GridView;
                if (gridView != null)
                {
                    GridColumn gridColumn;
                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        gridColumn = gridView.Columns[i];
                        if (gridColumn.FieldName == "LineAmount")
                        {
                            gridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        }
                    }
                    gridView.OptionsView.ShowColumnHeaders = false;
                    gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
                    gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
                    gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    gridView.OptionsView.ShowFooter = false;
                    gridView.OptionsView.ShowIndicator = false;
                }
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            GridListEditor gridListEditor = View.Editor as GridListEditor;
            if (gridListEditor != null)
            {
                GridView gridView = gridListEditor.GridView;
                if (gridView != null)
                {
                    GridColumn gridColumn;
                    for (int i = 0; i < gridView.Columns.Count; i++)
                    {
                        gridColumn = gridView.Columns[i];
                        if (gridColumn.FieldName == "LineAmount")
                        {
                            gridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        }
                    }
                    gridView.OptionsView.ShowColumnHeaders = false;
                    gridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
                    gridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
                    gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                    gridView.OptionsView.ShowFooter = false;
                    gridView.OptionsView.ShowIndicator = false;
                }
            }
        }
    }
}
