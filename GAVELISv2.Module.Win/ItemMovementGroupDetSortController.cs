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
    public partial class ItemMovementGroupDetSortController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private ItemsMovementGroupDetail _ItmMovGroupDet;
        public ItemMovementGroupDetSortController()
        {
            this.TargetObjectType = typeof(ItemsMovementGroupDetail);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewNesting = Nesting.Nested;
        }

        void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            UpdateSorting();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateSorting();
            ((NestedFrame)Frame).ViewItem.View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
        }

        private void UpdateSorting()
        {
            if (Frame is NestedFrame)
            {
                ItemsMovementGroup _ItmMovGroup = ((NestedFrame)Frame).ViewItem.View.CurrentObject as ItemsMovementGroup;
                string sortStr = "NoOfInstance";

                if (_ItmMovGroup != null)
                {
                    switch (_ItmMovGroup.SortBy)
                    {
                        case SortByEnum.Instance:
                            sortStr = "NoOfInstance";
                            break;
                        case SortByEnum.Receipt:
                            sortStr = "Rcpt";
                            break;
                        case SortByEnum.Invoice:
                            sortStr = "Invc";
                            break;
                        case SortByEnum.WorkOrder:
                            sortStr = "Word";
                            break;
                        case SortByEnum.EmpChargeSlip:
                            sortStr = "Ecs";
                            break;
                        case SortByEnum.CreditMemo:
                            sortStr = "Cm";
                            break;
                        case SortByEnum.DebitMemo:
                            sortStr = "Dm";
                            break;
                        case SortByEnum.FuelRequest:
                            sortStr = "Fpr";
                            break;
                        default:
                            break;
                    }
                }

                GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
                if (listEditor != null)
                {
                    GridView gridView = listEditor.GridView;
                    if (gridView != null && gridView.Columns.Count > 0)
                    {
                        GridColumn gridColumn;
                        for (int i = 0; i < gridView.Columns.Count; i++)
                        {
                            gridColumn = gridView.Columns[i];
                            if (gridColumn.FieldName == sortStr)
                            {
                                gridColumn.SortOrder = ColumnSortOrder.Descending;
                            }
                            else
                            {
                                gridColumn.SortOrder = ColumnSortOrder.None;
                            }
                        }
                    }
                }
            }
        }
    }
}
