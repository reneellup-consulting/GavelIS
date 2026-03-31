using System;
using System.ComponentModel;
using DevExpress.Data;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.ExpressApp.Editors;
using DevExpress.Utils;

namespace GAVELISv2.Module.Win
{
    public partial class ItemMovFreqAnalDetListViewController : ViewController<ListView>
    {
        private IModelColumn unboundModelColumn;
        private GridListEditor gridListEditor;
        private const string UnboundColumnCaption = "No";
        private const string UnboundColumnName = "UnboundNoColumn";
        public ItemMovFreqAnalDetListViewController()
        {
            this.TargetObjectType = typeof(IWithLineNumber);
            this.TargetViewType = ViewType.ListView;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            gridListEditor = View.Editor as GridListEditor;
            if (gridListEditor != null)
            {
                InitGridView();
                InitUnboundColumn();
            }
        }

        private void InitUnboundColumn()
        {
            unboundModelColumn = (((IModelColumns)gridListEditor.Model.Columns).GetNode(UnboundColumnName) as IModelColumn);
            if (unboundModelColumn == null)
            {
                unboundModelColumn = gridListEditor.Model.Columns.AddNode<IModelColumn>(UnboundColumnName);
                unboundModelColumn.PropertyName = UnboundColumnName;
                unboundModelColumn.Width = 10;
                unboundModelColumn.PropertyEditorType = typeof(DefaultPropertyEditor);
                for (int i = gridListEditor.Columns.Count - 1; i >= 0; i--)
                {
                    ColumnWrapper cw = gridListEditor.Columns[i];
                    if (cw.PropertyName == unboundModelColumn.PropertyName)
                    {
                        gridListEditor.RemoveColumn(cw);
                        break;
                    }
                }
                gridListEditor.AddColumn(unboundModelColumn);
                GridColumn noColumn = gridListEditor.GridView.Columns[unboundModelColumn.PropertyName];
                if (noColumn != null)
                {
                    noColumn.UnboundType = UnboundColumnType.String;
                    noColumn.Caption = UnboundColumnCaption;
                    noColumn.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;
                    noColumn.AppearanceCell.TextOptions.HAlignment = HorzAlignment.Far;
                    noColumn.VisibleIndex = 0;
                    noColumn.OptionsColumn.AllowEdit = false;
                    noColumn.OptionsColumn.AllowGroup = DefaultBoolean.False;
                    noColumn.OptionsColumn.AllowMove = false;
                    noColumn.OptionsColumn.AllowShowHide = false;
                    noColumn.OptionsColumn.AllowSize = false;
                    noColumn.OptionsColumn.AllowSort = DefaultBoolean.False;
                    noColumn.OptionsColumn.FixedWidth = true;
                    noColumn.OptionsColumn.ShowInCustomizationForm = false;
                    noColumn.OptionsFilter.AllowFilter = false;
                }
            }
        }

        private void InitGridView()
        {
            gridListEditor.GridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
        }

        private void GridView_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (e.Column.Caption == UnboundColumnCaption && e.RowHandle >= 0)
            {
                int c = e.RowHandle + 1;
                e.DisplayText = c.ToString() + ".";
            }
        }

        protected override void OnDeactivated()
        {
            if (gridListEditor != null && gridListEditor.GridView != null) {
                gridListEditor.GridView.CustomDrawCell -= GridView_CustomDrawCell;
            }
            base.OnDeactivated();
        }
    }
}
