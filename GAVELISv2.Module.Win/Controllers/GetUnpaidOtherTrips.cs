using System;
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
    public partial class GetUnpaidOtherTrips : ViewController
    {
        private PopupWindowShowAction getUnpaidOtherTrips;
        private OtherTripStatement _OtherTripStatement;
        public GetUnpaidOtherTrips()
        {
            this.TargetObjectType = typeof(OtherTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "OtherTripStatement.GetUnpaidOtherTrips";
            this.getUnpaidOtherTrips = new PopupWindowShowAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.getUnpaidOtherTrips.Caption = "Get Unpaid Trips";
            this.getUnpaidOtherTrips.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            GetUnpaidOtherTrips_CustomizePopupWindowParams);
            this.getUnpaidOtherTrips.Execute += new 
            PopupWindowShowActionExecuteEventHandler(GetUnpaidOtherTrips_Execute
            );
        }
        private void GetUnpaidOtherTrips_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _OtherTripStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as OtherTripStatement;
            #region Old Alrgoritms
            ////_Receipt.Save();
            ////_Receipt.Session.CommitTransaction();
            //IObjectSpace objectSpace = Application.CreateObjectSpace();
            //String listViewId = Application.FindListViewId(typeof(OtherTrip)
            //);
            //CollectionSourceBase collectionSource = Application.
            //CreateCollectionSource(objectSpace, typeof(OtherTrip), 
            //listViewId);
            //if (_OtherTripStatement.Customer == null) {throw new 
            //    ApplicationException("Customer not specified");}
            //if (_OtherTripStatement.FromDate != DateTime.MinValue || 
            //_OtherTripStatement.ToDate != DateTime.MinValue) {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _OtherTripStatement.Customer.No + 
            //    "' And [EntryDate] Between(#" + _OtherTripStatement.
            //    FromDate.ToShortDateString() + "#, #" + _OtherTripStatement.
            //    ToDate.ToShortDateString() + "#)");} else {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _OtherTripStatement.Customer.No + "'");
            //}
            //e.View = Application.CreateListView(listViewId, collectionSource, 
            //true);
            #endregion
            #region New Algorithms
            if (_OtherTripStatement.Customer == null)
            {
                throw new
                    ApplicationException("Customer not specified");
            }
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            // [Customer.No] = 'TC00001' And [Entry Date] >= #2015-06-01# And [Entry Date] < #2015-06-16#
            DateTime todate = _OtherTripStatement.ToDate.AddDays(1);
            CriteriaOperator criteria = CriteriaOperator.Parse("[Status] = 'Invoiced' And [Customer.No] = ?",
                _OtherTripStatement.Customer.No, _OtherTripStatement.FromDate, new DateTime(todate.Year, todate.Month, todate.Day));
            XPCollection<OtherTrip> filtered = new XPCollection<OtherTrip>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("BillSeq", DevExpress.Xpo.DB.SortingDirection.Ascending));
            for (int i = 0; i < filtered.Count; i++)
            {
                object obj = filtered[i];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            // StanfilcoTrip_UnpaidTrips_Selector
            string viewId = "OtherTrip_ListView_Unpaid";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(OtherTrip), viewId);
            if (keysToShow1.Count > 0)
            {
                if (dCount > 2100)
                {
                    collectionSource1.Criteria["GKey"] = new InOperator("GKey", keysToShow1);
                }
                else
                {
                    collectionSource1.Criteria["N0.Oid"] = new InOperator(ObjectSpace.GetKeyPropertyName(View.ObjectTypeInfo.Type), keysToShow1);
                }
            }
            else
            {
                collectionSource1.Criteria["GKey"] = new InOperator("GKey", new ArrayList[] { });
            }
            #endregion
            e.View = Application.CreateListView(viewId, collectionSource1, true);
        }
        private void GetUnpaidOtherTrips_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (OtherTrip item in e.PopupWindow.View.SelectedObjects) {
                OtherTrip trip = _OtherTripStatement.Session.
                GetObjectByKey<OtherTrip>(item.Oid);
                OtherTripCharge tripCharge = new OtherTripCharge(_OtherTripStatement.Session)
                {
                    OtherTripStatementID = _OtherTripStatement,
                    SourceType = trip.SourceType,
                    SourceNo = trip.SourceNo,
                    SourceID = trip.Oid,
                    Seq = trip.BillSeq,
                    Date = trip.EntryDate,
                    TruckNo = trip.TruckNo,
                    TrailerNo = trip.TrailerNo,
                    TripNo = trip.TripNo,
                    Origin = trip.Origin,
                    Destination = trip.Destination,
                    TruckerPay = trip.TruckerPay,
                    TrailerRental = trip.TrailerRental,
                    Billing = trip.Billing,
                    VATAmount = trip.VATAmount,
                    GrossBilling = trip.GrossBilling,
                    WHTAmount = trip.WHTAmount,
                    NetBilling = trip.NetBilling,
                    Adjust = trip.OpenAmount == 0 ? trip.NetBilling : trip.OpenAmount,
                    Terms = trip.Terms ?? null,
                    OpenAmount = trip.OpenAmount == 0 ? trip.NetBilling : trip.OpenAmount,
                    Pay = true
                };
                tripCharge.Save();
            }
        }
    }
}
