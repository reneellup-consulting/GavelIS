using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.Persistent.Base;
using DevExpress.XtraPivotGrid;
using DevExpress.Xpo.DB;

using GAVELISv2.Module.Win;

using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ExpenseAnalyticPivotBufferDrilldownController : ViewController<ListView>
    {
        public ExpenseAnalyticPivotBufferDrilldownController()
        {
            TargetObjectType = typeof(ExpenseAnalyticPivotBuffer);
        }
        protected override void OnActivated()
        {
            base.OnActivated();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            if (View.Editor.GetType() == typeof(PivotGridListEditor))
            {
                if (View.ObjectTypeInfo.FullName == typeof(ExpenseAnalyticPivotBuffer).FullName)
                {
                    PivotGridListEditor listEditor = View.Editor as PivotGridListEditor;
                    PivotGridControl pivotGrid = listEditor.PivotGridControl;
                    pivotGrid.CellDoubleClick += new PivotCellEventHandler(pivotGrid_CellDoubleClick);
                }
            }
        }

        void pivotGrid_CellDoubleClick(object sender, PivotCellEventArgs e)
        {
            PivotDrillDownDataSource drillDown = e.CreateDrillDownDataSource();
            string str = e.DataField.Caption;
            string str2 = e.Item.RowFieldValueItem.DisplayText;
            ArrayList keysToShow1 = new ArrayList();
            ArrayList keysToShow2 = new ArrayList();
            int dCount = 0;
            for (int i = 0; i < drillDown.RowCount; i++)
            {
                ExpenseAnalyticPivotBuffer obj = drillDown[i][0] as ExpenseAnalyticPivotBuffer;
                if (View.Id == "ExpenseAnalyticPivotBuffer_ListView" && obj != null) // && ((ExpenseAnalyticPivotBuffer)obj).GMonth.ToString() == str)
                {
                    // select * from vExpenseAnalyticsBuffer where ExpenseType='2eff2179-c543-4937-9012-69d0f0336881' and SubExpenseType='861a12c0-50e1-41f6-b94b-5682f3c98f6c' and PaymentMode=0 and GMonth=1 and GYear=2022
                    string cmd1;
                    if (obj.SubCategory != null)
                    {
                        cmd1 = string.Format("select * from vExpenseAnalyticsBuffer where ExpenseType='{0}' and SubExpenseType='{1}' and PaymentMode={2} and GMonth={3} and GYear={4} and BufferId='{5}'",
                            obj.Category.Oid, obj.SubCategory.Oid, DrilldownEnumHelper.GetPaymentTypeNo(obj.PaymentType.ToString()), DrilldownEnumHelper.GetMonthNo(obj.Month.ToString()), obj.Year, obj.BufferId);
                    }
                    else
                    {
                        cmd1 = string.Format("select * from vExpenseAnalyticsBuffer where ExpenseType='{0}' and PaymentMode={1} and GMonth={2} and GYear={3} and BufferId='{4}'",
                            obj.Category.Oid, DrilldownEnumHelper.GetPaymentTypeNo(obj.PaymentType.ToString()), DrilldownEnumHelper.GetMonthNo(obj.Month.ToString()), obj.Year, obj.BufferId);
                    }
                    SelectedData data1 = ((ObjectSpace)this.ObjectSpace).Session.ExecuteQuery(cmd1);
                    foreach (var row in data1.ResultSet[0].Rows)
                    {
                        dCount++;
                        keysToShow1.Add(row.Values[0]);
                    }
                }
            }
            if (keysToShow1.Count > 0)
            {
                string viewId = "ExpenseAnalyticsBuffer_PivotDetails";
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ExpenseAnalyticsBuffer), viewId);
                if (dCount > 2100)
                {
                    collectionSource1.Criteria["GKey"] = new InOperator("GKey", keysToShow1);
                }
                else
                {
                    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(typeof(ExpenseAnalyticsBuffer)), keysToShow1);
                }
                ListView listView = Application.CreateListView(viewId, collectionSource1, true);
                ShowViewParameters svp = new ShowViewParameters(listView);
                svp.CreatedView.Caption = string.Format("{0}|{1}", str2, "Month Expense Analysis Details");
                svp.TargetWindow = TargetWindow.NewModalWindow;
                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
            }
        }
    }
}
