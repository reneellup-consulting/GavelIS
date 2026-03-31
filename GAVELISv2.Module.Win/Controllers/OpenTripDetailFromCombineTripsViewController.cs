using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

using GAVELISv2.Module.BusinessObjects;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Xpo;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class OpenTripDetailFromCombineTripsViewController : ViewController<ListView>
    {
        private ListViewProcessCurrentObjectController processCurrentObjectController;
        public OpenTripDetailFromCombineTripsViewController()
        {
            this.TargetObjectType = typeof(CombinedTrips);
            this.TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();

            processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (processCurrentObjectController != null)
            {
                processCurrentObjectController.CustomProcessSelectedItem += new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
            }
        }

        void processCurrentObjectController_CustomProcessSelectedItem(object sender, CustomProcessListViewSelectedItemEventArgs e)
        {
            e.Handled = true;

            CombinedTrips selectedCombinedTrip = View.CurrentObject as CombinedTrips;

            if (selectedCombinedTrip != null && selectedCombinedTrip.GenJournalID != null) // Assuming your PurchaseOrderDetail has a 'Customer' property
            {
                Type type = selectedCombinedTrip.GenJournalID.GetType();
                
                IObjectSpace _ObjectSpace = Application.CreateObjectSpace();
                var detailToShow = _ObjectSpace.GetObject(selectedCombinedTrip.GenJournalID);
                if (detailToShow != null)
                {
                    DetailView detailView = Application.CreateDetailView(_ObjectSpace, detailToShow, true);
                    ShowViewParameters svp = new ShowViewParameters(detailView);
                    svp.TargetWindow = TargetWindow.NewModalWindow;
                    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
                }
            }
        }

        protected override void OnDeactivated()
        {
            processCurrentObjectController = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (processCurrentObjectController != null)
            {
                processCurrentObjectController.CustomProcessSelectedItem -= new EventHandler<CustomProcessListViewSelectedItemEventArgs>(processCurrentObjectController_CustomProcessSelectedItem);
            }
            base.OnDeactivated();
        }
    }
}
