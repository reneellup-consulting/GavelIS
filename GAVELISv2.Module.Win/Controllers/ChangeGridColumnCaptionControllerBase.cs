using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ChangeGridColumnCaptionControllerBase : ViewController
    {
        public ChangeGridColumnCaptionControllerBase()
        {
            TargetObjectType = typeof(TruckIncomeExpenseDetail);
            TargetViewType = ViewType.ListView;
            TargetViewId = "TruckIncomeExpense_TruckIncomeExpenseDetails_ListView";
            TargetViewNesting = Nesting.Nested;
        }

        void View_CurrentObjectChanged(object sender, EventArgs e)
        {
            UpdateCaption();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateCaption();
            ((NestedFrame)Frame).ViewItem.View.CurrentObjectChanged += new EventHandler(View_CurrentObjectChanged);
        }
        private void UpdateCaption()
        {
            if (Frame is NestedFrame)
            {
                TruckIncomeExpense dt = ((NestedFrame)Frame).ViewItem.View.CurrentObject as TruckIncomeExpense;
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
                            if (gridColumn != null)
                            {
                                if (gridColumn.FieldName == "Expense01")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption01;
                                }
                                if (gridColumn.FieldName == "Expense02")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption02;
                                }
                                if (gridColumn.FieldName == "Expense03")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption03;
                                }
                                if (gridColumn.FieldName == "Expense04")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption04;
                                }
                                if (gridColumn.FieldName == "Expense05")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption05;
                                }
                                if (gridColumn.FieldName == "Expense06")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption06;
                                }
                                if (gridColumn.FieldName == "Expense07")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption07;
                                }
                                if (gridColumn.FieldName == "Expense08")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption08;
                                }
                                if (gridColumn.FieldName == "Expense09")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption09;
                                }
                                if (gridColumn.FieldName == "Expense10")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption10;
                                }
                                if (gridColumn.FieldName == "Expense11")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption11;
                                }
                                if (gridColumn.FieldName == "Expense12")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption12;
                                }
                                if (gridColumn.FieldName == "Expense13")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption13;
                                }
                                if (gridColumn.FieldName == "Expense14")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption14;
                                }
                                if (gridColumn.FieldName == "Expense15")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption15;
                                }
                                if (gridColumn.FieldName == "Expense16")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption16;
                                }
                                if (gridColumn.FieldName == "Expense17")
                                {
                                    gridColumn.Caption = dt.ExpenseCaption17;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
