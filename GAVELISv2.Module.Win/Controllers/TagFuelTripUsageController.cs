using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.Data.Filtering;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TagFuelTripUsageController : ViewController
    {
        private PopupWindowShowAction tagFuelTripUsage;
        private PurchaseOrderFuel _purchaseOrderFuel;
        public TagFuelTripUsageController()
        {
            this.TargetObjectType = typeof(PurchaseOrderFuel);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "PurchaseOrderFuel.TagFuelTripUsage";
            this.tagFuelTripUsage = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.tagFuelTripUsage.Caption = "Tag Trips";
            this.tagFuelTripUsage.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(tagFuelTripUsage_CustomizePopupWindowParams);
            this.tagFuelTripUsage.Execute += new PopupWindowShowActionExecuteEventHandler(tagFuelTripUsage_Execute);
            this.tagFuelTripUsage.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(tagFuelTripUsage_ExecuteCompleted);
        }

        void tagFuelTripUsage_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh(); // Refreshes the ObjectSpace
            View.Refresh();        // Refreshes the current view, if applicable
        }

        void tagFuelTripUsage_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrderFuel purchaseOrderFuel = ObjectSpace.GetObject(_purchaseOrderFuel);
            if (purchaseOrderFuel == null)
            {
                throw new UserFriendlyException("The Purchase Order Fuel record could not be found.");
            }

            foreach (CombinedTrips item in e.PopupWindow.View.SelectedObjects)
            {
                GenJournalHeader tripDataFromCombinedTrip = null;

                if (item != null)
                {
                    tripDataFromCombinedTrip = item.GenJournalID;
                }

                if (tripDataFromCombinedTrip == null)
                {
                    continue; // Skip to the next selected item
                }

                GenJournalHeader tripToLink = ObjectSpace.GetObject(tripDataFromCombinedTrip);

                if (tripToLink == null)
                {
                    continue;
                }

                // Check if this trip has already been added to this PurchaseOrderFuel
                bool alreadyExists = purchaseOrderFuel.PurchaseOrderFuelUsageDetails
                                        .Any(detail => detail.TripNo == tripToLink);

                if (alreadyExists)
                {
                    continue; // Skip this item as it's already linked
                }

                PurchaseOrderFuelUsageDetail newDetail = ObjectSpace.CreateObject<PurchaseOrderFuelUsageDetail>();
                newDetail.MarkToDelete = false;
                newDetail.RowID = Guid.NewGuid();
                newDetail.HeaderID = purchaseOrderFuel;
                newDetail.TripNo = tripToLink;
                newDetail.Save();
            }
        }

        void tagFuelTripUsage_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _purchaseOrderFuel = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as PurchaseOrderFuel;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "CombinedTrips_ListView_TagTrips";
            CollectionSourceBase collectionSource = Application.CreateCollectionSource(objectSpace, typeof(CombinedTrips), listViewId);
            string crit = string.Empty;
            if (_purchaseOrderFuel.Customer != null)
            {
                crit = string.Format("[Customer.Name] = '{0}'", _purchaseOrderFuel.Customer.Name);
            }
            collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(crit);

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}