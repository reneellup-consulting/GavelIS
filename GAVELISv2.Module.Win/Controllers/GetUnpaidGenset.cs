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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class GetUnpaidGenset : ViewController {
        private PopupWindowShowAction getUnpaidGenset;
        private GensetStatement _GensetStatement;
        public GetUnpaidGenset() {
            this.TargetObjectType = typeof(GensetStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GensetStatement.GetUnpaidGenset";
            this.getUnpaidGenset = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getUnpaidGenset.Caption = "Get Unpaid Genset";
            this.getUnpaidGenset.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            GetUnpaidGenset_CustomizePopupWindowParams);
            this.getUnpaidGenset.Execute += new 
            PopupWindowShowActionExecuteEventHandler(GetUnpaidGenset_Execute);
        }
        private void GetUnpaidGenset_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _GensetStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as GensetStatement;
            #region Old Algorithms
            //            //_Receipt.Save();
            //            //_Receipt.Session.CommitTransaction();
            //            IObjectSpace objectSpace = Application.CreateObjectSpace();
            //            String listViewId = Application.FindListViewId(typeof(GensetEntry));
            //            CollectionSourceBase collectionSource = Application.
            //            CreateCollectionSource(objectSpace, typeof(GensetEntry), listViewId)
            //            ;
            //            if (_GensetStatement.Customer == null) {throw new 
            //                ApplicationException("Customer not specified");}
            //            if (_GensetStatement.FromDate != DateTime.MinValue ||
            //_GensetStatement.ToDate != DateTime.MinValue)
            //            {
            //                collectionSource
            //                    .Criteria["ModelCriteria"] = CriteriaOperator.Parse(
            //                    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //                    + _GensetStatement.Customer.No +
            //                    "' And [EntryDate] Between(#" + _GensetStatement.FromDate.
            //                    ToShortDateString() + "#, #" + _GensetStatement.ToDate.
            //                    ToShortDateString() + "#)");
            //            }
            //            else if (_GensetStatement.Period > 0 && _GensetStatement.Week > 0)
            //            {
            //                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //                Parse(
            //                "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //                + _GensetStatement.Customer.No +
            //                "' And [Period] = " + _GensetStatement.Period + " And [Week] = " + _GensetStatement.Week);
            //            }
            //            else
            //            {
            //                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //                Parse(
            //                "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //                + _GensetStatement.Customer.No + "'");
            //            }
            #endregion
            #region New Algorithms
            if (_GensetStatement.Customer == null)
            {
                throw new
                    ApplicationException("Customer not specified");
            }
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            // [Customer.No] = 'TC00001' And [Entry Date] >= #2015-06-01# And [Entry Date] < #2015-06-16#
            DateTime todate = _GensetStatement.ToDate.AddDays(1);
            CriteriaOperator criteria = CriteriaOperator.Parse("[Status] = 'Invoiced' And [Customer.No] = ? And [EntryDate] >= ? And [EntryDate] < ?",
                _GensetStatement.Customer.No, _GensetStatement.FromDate, new DateTime(todate.Year,todate.Month,todate.Day));
            XPCollection<GensetEntry> filtered = new XPCollection<GensetEntry>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("BillSeq", DevExpress.Xpo.DB.SortingDirection.Ascending));
            for (int i = 0; i < filtered.Count; i++)
            {
                object obj = filtered[i];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            // StanfilcoTrip_UnpaidTrips_Selector
            string viewId = "GensetEntry_ListView_Unpaid";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(GensetEntry), viewId);
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
        private void GetUnpaidGenset_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (GensetEntry item in e.PopupWindow.View.SelectedObjects) {
                GensetEntry trip = _GensetStatement.Session.GetObjectByKey<
                GensetEntry>(item.Oid);
                GensetCharge genset = new GensetCharge(_GensetStatement.Session)
                ;
                genset.GensetStatementID = _GensetStatement;
                genset.GensetEntryId = trip;
                genset.SourceType = trip.SourceType;
                genset.SourceNo = trip.SourceNo;
                genset.SourceID = trip.Oid;
                genset.Seq = trip.BillSeq;
                genset.Date = trip.EntryDate;
                genset.TruckNo = trip.TruckNo;
                genset.TrailerNo = trip.TrailerNo;
                genset.GensetNo = trip.GensetNo;
                genset.Driver = trip.Driver;
                genset.TripNo = trip.TripNo;
                if (trip.TripID.GetType() == typeof(StanfilcoTrip)) {
                    genset.Tariff = ((StanfilcoTrip)trip.TripID).Tariff;
                    genset.Origin = ((StanfilcoTrip)trip.TripID).Origin;
                    genset.Destination = ((StanfilcoTrip)trip.TripID).
                    Destination;
                }
                if (trip.TripID.GetType() == typeof(DolefilTrip)) {
                    genset.Tariff = ((DolefilTrip)trip.TripID).Tariff;
                    genset.Origin = ((DolefilTrip)trip.TripID).Origin;
                    genset.Destination = ((DolefilTrip)trip.TripID).Destination;
                }
                if (trip.TripID.GetType() == typeof(OtherTrip)) {
                    genset.Origin = ((OtherTrip)trip.TripID).Origin;
                    genset.Destination = ((OtherTrip)trip.TripID).Destination;
                }
                genset.RegHrs = trip.RegularHrs.Value;
                genset.ColdRoom = trip.ColdRoomHrs.Value;
                genset.Other = trip.OtherHrs.Value;
                genset.TotalHrs = trip.TotalHrs;
                genset.RatePerHr = trip.RatePerHr;
                genset.Billing = trip.Total;
                genset.VATAmount = trip.VATAmount;
                genset.GrossBilling = trip.GrossBilling;
                genset.WHTAmount = trip.WHTAmount;
                genset.NetBilling = trip.NetBilling;
                genset.Adjust = trip.OpenAmount == 0 ? trip.GrossBilling : trip.
                OpenAmount;
                genset.Terms = trip.Terms != null ? trip.Terms : null;
                genset.OpenAmount = trip.OpenAmount == 0 ? trip.GrossBilling : 
                trip.OpenAmount;
                genset.Pay = true;
                genset.Save();
            }
        }
    }
}
