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
    public partial class AddDoleTripsForReconciliation : ViewController
    {
        private PopupWindowShowAction addTrips;
        private DolefilTripPaymentsRecon _doleTripPaymentRecon;

        public AddDoleTripsForReconciliation()
        {
            this.TargetObjectType = typeof(DolefilTripPaymentsRecon);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DolefilTripPaymentsRecon.AddTrips";
            this.addTrips = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.addTrips.Caption = "Add Trips";
            this.addTrips.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            AddTrips_CustomizePopupWindowParams);
            this.addTrips.Execute += new
            PopupWindowShowActionExecuteEventHandler(AddTrips_Execute
            );
            this.addTrips.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(AddTrips_ExecuteCompleted);
        }

        void AddTrips_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }

        private void AddTrips_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _doleTripPaymentRecon = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DolefilTripPaymentsRecon;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "DolefilTripDetail_ListView_AddTrips";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(DolefilTripDetail), listViewId)
            ;
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }

        private void AddTrips_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            DolefilTripPaymentsRecon dftpr = _doleTripPaymentRecon.Session.GetObjectByKey<DolefilTripPaymentsRecon>(
                _doleTripPaymentRecon.Oid);
            foreach (DolefilTripDetail item in e.PopupWindow.View.SelectedObjects)
            {
                #region Algorithm here...
                DolefilTripDetail dftd = _doleTripPaymentRecon.Session.GetObjectByKey<DolefilTripDetail>(
                    item.Oid);
                StringBuilder sb = new StringBuilder();
                bool hasError = false;
                sb.AppendFormat("Problems found in trip detail ID#{0}. ", dftd.Oid);
                //if (dftd.Tariff == null)
                //{
                //    hasError = true;
                //    sb.Append("Tariff is not specified and ");
                //}
                //if (dftd.Driver.DriverClassification == null)
                //{
                //    hasError = true;
                //    sb.AppendFormat("Driver {0} Driver Classification is not specified     ", dftd.Driver.Name);
                //}
                DtpReconDetail trpp = dftpr.Trips.Where(o => o.DetailId.Oid == item.Oid).FirstOrDefault();
                if (trpp != null)
                {
                    throw new ApplicationException(string.Format("Trip detail ID: {0} with doc.no: {1} was already added to this reconciliation.",trpp.DetailId.Oid, trpp.Trip.DocumentNo));
                }
                // Add to DtpReconDetail
                DtpReconDetail odtprp = ReflectionHelper.CreateObject<DtpReconDetail>(_doleTripPaymentRecon.Session);
                // HeaderId => DolefilTripPaymentsRecon
                odtprp.HeaderId = dftpr;
                // Trip => DolefilTrip
                odtprp.Trip = dftd.TripID as DolefilTrip;
                // DetailId => DolefilTripDetail
                odtprp.DetailId = dftd;
                // HaulDerscription => HaulCategory
                // HType => HaulType
                odtprp.Remarks = dftd.Remarks;
                odtprp.Save();

                // Update DolefilTripDetail
                dftd.ReconId = dftpr;
                dftd.Save();

                if (hasError)
                {
                    sb.Remove(sb.Length - 5, 5);
                    sb.Append(".");
                    throw new ApplicationException(sb.ToString());
                }
                #endregion
            }
        }
    }
}
