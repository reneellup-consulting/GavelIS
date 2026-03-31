using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TruckIncExpSelUnitsController : ViewController
    {
        private PopupWindowShowAction selectUnitsAction;
        private TruckIncomeExpense truckIncExp;
        public TruckIncExpSelUnitsController()
        {
            this.TargetObjectType = typeof(TruckIncomeExpense);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "TruckIncomeExpense.SelectUnits";
            this.selectUnitsAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.selectUnitsAction.Caption = "Select Units";
            this.selectUnitsAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(selectUnitsAction_CustomizePopupWindowParams);
            this.selectUnitsAction.Execute += new PopupWindowShowActionExecuteEventHandler(selectUnitsAction_Execute);
        }
        void selectUnitsAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            truckIncExp = ((DetailView)this.View).
            CurrentObject as TruckIncomeExpense;
            TruckIncomeExpense emp = ObjectSpace.GetObject(truckIncExp) as TruckIncomeExpense;
            var sortedQuery = from FixedAsset a in e.PopupWindow.View.SelectedObjects orderby a.SortingSequence select a;
            foreach (FixedAsset ttbl in sortedQuery)
            {
                //string substring = ttbl.No.Substring(3);
                //string padLeft = ttbl.No.Substring(3).PadLeft(4, '0');
                FixedAsset ottbl = ObjectSpace.GetObject<FixedAsset>(ttbl);
                TruckIncomeExpenseTruck ose;
                ose = emp.TruckIncomeExpenseTrucks.Where(o => o.Unit == ottbl).FirstOrDefault();
                if (ose != null)
                {
                    continue;
                }
                ose = ObjectSpace.CreateObject<TruckIncomeExpenseTruck>();
                ose.Unit = ottbl;
                ose.Seq = ottbl.SortingSequence;
                ose.Save();
                emp.TruckIncomeExpenseTrucks.Add(ose);
            }
            emp.Save();
            ObjectSpace.CommitChanges();
        }

        void selectUnitsAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            //String listViewId = Application.FindListViewId(typeof(FixedAsset));
            String listViewId = "Fleet_ListView";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(FixedAsset), listViewId)
            ;
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
