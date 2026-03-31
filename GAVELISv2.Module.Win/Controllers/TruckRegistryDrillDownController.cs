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
    public partial class TruckRegistryDrillDownController : ViewController<ListView>
    {
        public TruckRegistryDrillDownController()
        {
            TargetObjectType = typeof(TruckRegistry);
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
                if (View.ObjectTypeInfo.FullName == typeof(TruckRegistry).FullName)
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
                object obj = drillDown[i][0];
                if (str == "Short Count" && ((TruckRegistry)obj).ShortCount == 1)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                    continue;
                }
                else if (str == "Short Haul" && ((TruckRegistry)obj).ShortHaul != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                    continue;
                }
                else if (str == "Long Count" && ((TruckRegistry)obj).LongCount == 1)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                    continue;
                }
                else if (str == "Long Haul" && ((TruckRegistry)obj).LongHaul != 0)
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                    continue;
                }
                else if (str == "Trip No")
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                    continue;
                }
                else if (str == "Income")
                {
                    dCount++;
                    keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
                    continue;
                }
            }
            if (keysToShow1.Count > 0)
            {
                string viewId = "TruckRegistry_PivotDetails";
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
