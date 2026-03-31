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
    public partial class NLSGeneratorChangeGridColController : ViewController
    {
        public NLSGeneratorChangeGridColController()
        {
            TargetObjectType = typeof(NLSGenerationDetail);
            TargetViewType = ViewType.ListView;
            //TargetViewId = "TruckIncomeExpense_TruckIncomeExpenseDetails_ListView";
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
                NonLocalSupplierReportGenerator dt = ((NestedFrame)Frame).ViewItem.View.CurrentObject as NonLocalSupplierReportGenerator;
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
                                if (gridColumn.FieldName == "ForVendor01")
                                {
                                    gridColumn.Caption = dt.VendorCaption01;
                                }
                                if (gridColumn.FieldName == "ForVendor02")
                                {
                                    gridColumn.Caption = dt.VendorCaption02;
                                }
                                if (gridColumn.FieldName == "ForVendor03")
                                {
                                    gridColumn.Caption = dt.VendorCaption03;
                                }
                                if (gridColumn.FieldName == "ForVendor04")
                                {
                                    gridColumn.Caption = dt.VendorCaption04;
                                }
                                if (gridColumn.FieldName == "ForVendor05")
                                {
                                    gridColumn.Caption = dt.VendorCaption05;
                                }
                                if (gridColumn.FieldName == "ForVendor06")
                                {
                                    gridColumn.Caption = dt.VendorCaption06;
                                }
                                if (gridColumn.FieldName == "ForVendor07")
                                {
                                    gridColumn.Caption = dt.VendorCaption07;
                                }
                                if (gridColumn.FieldName == "ForVendor08")
                                {
                                    gridColumn.Caption = dt.VendorCaption08;
                                }
                                if (gridColumn.FieldName == "ForVendor09")
                                {
                                    gridColumn.Caption = dt.VendorCaption09;
                                }
                                if (gridColumn.FieldName == "ForVendor10")
                                {
                                    gridColumn.Caption = dt.VendorCaption10;
                                }
                            }
                        }
                    }
                }
            }
        }

    }
}
