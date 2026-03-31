using System;
using System.ComponentModel;
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
    public partial class UntagFuelTripUsageController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private PopupWindowShowAction
        UntagFuelTripUsageAction;
        private PurchaseOrderFuel _PurchaseOrderFuel;
        public UntagFuelTripUsageController()
        {
            this.TargetObjectType = typeof(PurchaseOrderFuel);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "PurchaseOrderFuel.UntagFuelTripUsageC";
            this.UntagFuelTripUsageAction = new
            PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit)
            ;
            this.UntagFuelTripUsageAction.Caption = "Untag Trips";
            this.UntagFuelTripUsageAction.
            CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(UntagFuelTripUsageAction_CustomizePopupWindowParams);
            this.UntagFuelTripUsageAction.Execute += new PopupWindowShowActionExecuteEventHandler(UntagFuelTripUsageAction_Execute);
            this.UntagFuelTripUsageAction.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(UntagFuelTripUsageAction_ExecuteCompleted);
        }

        void UntagFuelTripUsageAction_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh(); // Refreshes the ObjectSpace
            View.Refresh();        // Refreshes the current view, if applicable
        }

        void UntagFuelTripUsageAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrderFuel purchaseOrderFuel = ObjectSpace.GetObject(_PurchaseOrderFuel);
            if (purchaseOrderFuel == null)
            {
                throw new UserFriendlyException("The Purchase Order Fuel record could not be found.");
            }

            foreach (PurchaseOrderFuelUsageDetail item in e.PopupWindow.View.SelectedObjects)
            {
                PurchaseOrderFuelUsageDetail _toDel = ObjectSpace.GetObject(item);
                _toDel.Delete();
            }
        }

        void UntagFuelTripUsageAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _PurchaseOrderFuel = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as PurchaseOrderFuel;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PurchaseOrderFuelUsageDetails_ListView_Untag";
            CollectionSourceBase collectionSource = Application.CreateCollectionSource(objectSpace, typeof(PurchaseOrderFuelUsageDetail), listViewId);
            string crit = string.Empty;
            // [Header ID.Source No] = 'ggg'
            crit = string.Format("[HeaderID.SourceNo] = '{0}'", _PurchaseOrderFuel.SourceNo);
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(crit);
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
