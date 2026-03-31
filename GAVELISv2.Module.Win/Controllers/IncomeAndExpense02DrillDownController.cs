using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.XtraGrid;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.XtraGrid.Views.Grid;
using System.Drawing;
using DevExpress.ExpressApp.Win.Layout;
using DevExpress.ExpressApp.PivotGrid.Win;
using DevExpress.XtraPivotGrid;
using System.Collections;
using DevExpress.Data.Filtering;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class IncomeAndExpense02DrillDownController : ViewController
    {
        private XafPivotGridControl _PivotGrid;
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        public IncomeAndExpense02DrillDownController()
        {
            TargetObjectType = typeof(IncomeAndExpense02);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.ControlsCreated += new EventHandler(View_ControlsCreated);
            processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
        }

        void View_ControlsCreated(object sender, EventArgs e)
        {
            XafLayoutControl xlc= processCurrentObjectController.View.Control as XafLayoutControl;
            if (xlc != null)
            {
                foreach (var item in xlc.Controls)
                {
                    if (item.GetType() == typeof(XafPivotGridControl))
                    {
                        _PivotGrid = item as XafPivotGridControl;
                        if (_PivotGrid != null)
                        {
                            _PivotGrid.CellDoubleClick += new DevExpress.XtraPivotGrid.PivotCellEventHandler(_PivotGrid_CellDoubleClick);
                        }
                    }
                }
            }
        }

        void _PivotGrid_CellDoubleClick(object sender, DevExpress.XtraPivotGrid.PivotCellEventArgs e)
        {
            PivotDrillDownDataSource drillDown = e.CreateDrillDownDataSource();
            string str = e.DataField.Caption;
            string str2 = e.Item.RowFieldValueItem.DisplayText;
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            for (int i = 0; i < drillDown.RowCount; i++)
            {
                object obj = drillDown[i][0];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            if (keysToShow1.Count > 0)
            {
                string viewId = "IncomeAndExpense02_Drilled_ListView";
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
