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
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class PartsUsagePivotDrillDownController : ViewController<ListView>
    {
        public PartsUsagePivotDrillDownController()
        {
            TargetObjectType = typeof(PartsPurchasesUsageDetail);
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
                if (View.ObjectTypeInfo.FullName == typeof(PartsPurchasesUsageDetail).FullName)
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
            string str = e.Item.ColumnFieldValueItem.DisplayText;
            string str2 = e.Item.RowFieldValueItem.DisplayText;
            ArrayList keysToShow1 = new ArrayList();
            ArrayList keysToShow2 = new ArrayList();
            int dCount = 0;
            //PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot
            //PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot_Usage
            for (int i = 0; i < drillDown.RowCount; i++)
            {
                object obj = drillDown[i][0];
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot" && str == "01 Parts Purchases" && obj != null && ((PartsPurchasesUsageDetail)obj).ItemNo.Description == str2 && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot" && str == "02 Parts Sold" && obj != null && ((PartsPurchasesUsageDetail)obj).ItemNo.Description == str2 && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot" && str == "03 Repairs & Maintenance" && obj != null && ((PartsPurchasesUsageDetail)obj).ItemNo.Description == str2 && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot" && str == "04 Charge to Employees" && obj != null && ((PartsPurchasesUsageDetail)obj).ItemNo.Description == str2 && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                //PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot_Usage

                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot_Usage" && str2 == "01 Parts Purchases" && obj != null && ((PartsPurchasesUsageDetail)obj).GMonth.ToString() == str && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot_Usage" && str2 == "02 Parts Sold" && obj != null && ((PartsPurchasesUsageDetail)obj).GMonth.ToString() == str && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot_Usage" && str2 == "03 Repairs & Maintenance" && obj != null && ((PartsPurchasesUsageDetail)obj).GMonth.ToString() == str && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
                if (View.Id == "PartsPurchasesUsageReporter_PartsUsageDetails_ListView_Pivot_Usage" && str2 == "04 Charge to Employees" && obj != null && ((PartsPurchasesUsageDetail)obj).GMonth.ToString() == str && ((PartsPurchasesUsageDetail)obj).Total != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                }
            }
            if (keysToShow1.Count > 0)
            {
                string viewId = "PartsUsageIcjDetails_PivotDetails";
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(PartsUsageIcjDetails), viewId);
                if (dCount > 2100)
                {
                    collectionSource1.Criteria["DetailID"] = new InOperator("DetailID", keysToShow1);
                }
                else
                {
                    collectionSource1.Criteria["N0.DetailID"] = new InOperator("DetailID", keysToShow1);
                }
                ListView listView = Application.CreateListView(viewId, collectionSource1, true);
                listView.Caption = string.Format("{0} - {1}", str2, str);
                ShowViewParameters svp = new ShowViewParameters(listView);
                svp.TargetWindow = TargetWindow.NewModalWindow;
                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
            }
        }
    }
}
