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
    public partial class TiresAccumulatedCostDrillDownController : ViewController<ListView>
    {
        public TiresAccumulatedCostDrillDownController()
        {
            TargetViewId = "TireItemsAccomulatedCost_Details_PivotView";
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
                if (View.ObjectTypeInfo.FullName == typeof(TireItemsAccomCostDetail).FullName)
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
            string tireItemClass = string.Empty;
            string tireType = string.Empty;
            string lineCondition = string.Empty;
            for (int i = 0; i < drillDown.RowCount; i++)
            {
                object obj = drillDown[i][0];
                TireItemsAccomCostDetail tiacd = obj as TireItemsAccomCostDetail;
                tireItemClass =  tiacd.TireItemClass.ToString();
                tireType = tiacd.TireType.ToString();
                lineCondition = tiacd.LineCondition != null ? tiacd.LineCondition : string.Empty;
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            string viewId = string.Empty;
            if (keysToShow1.Count > 0)
            {
                if (tireItemClass == "Tire" && tireType == "Tubeless" && lineCondition == "Brandnew")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView";
                }
                else if (tireItemClass == "Tire" && tireType == "Tubeless" && lineCondition == "Recap")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_TubelessRecap";
                }
                else if (tireItemClass == "Tire" && tireType == "Tubeless" && lineCondition == "Scrapped")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_TubelessScrap";
                }
                else if (tireItemClass == "Tire" && tireType == "TubeType" && lineCondition == "Brandnew")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_TubeTypeBrandner";
                }
                else if (tireItemClass == "Tire" && tireType == "TubeType" && lineCondition == "Recap")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_TubeTypeRecap";
                }
                else if (tireItemClass == "Tire" && tireType == "TubeType" && lineCondition == "Scrapped")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_TubeTypeScrap";
                }
                else if (tireItemClass == "Flap")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_Flaps";
                }
                else if (tireItemClass == "Tube")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_Tube";
                }
                else if (tireItemClass == "Rim")
                {
                    viewId = "TireItemsAccomulatedCost_Details_ListView_Rim";
                }
                CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), View.ObjectTypeInfo.Type, viewId);
                if (dCount > 2100)
                {
                    collectionSource1.Criteria["GKey"] = new InOperator("GKey", keysToShow1);
                }
                else
                {
                    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(View.ObjectTypeInfo.Type), keysToShow1);
                }
                ListView listView = Application.CreateListView(viewId, collectionSource1, true);
                ShowViewParameters svp = new ShowViewParameters(listView);
                svp.TargetWindow = TargetWindow.NewModalWindow;
                Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
            }
        }
    }
}
