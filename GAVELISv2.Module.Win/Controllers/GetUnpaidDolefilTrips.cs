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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class GetUnpaidDolefilTrips : ViewController {
        private PopupWindowShowAction getUnpaidStanfilcoTrips;
        private DolefilTripStatement _DolefilTripStatement;
        public GetUnpaidDolefilTrips() {
            this.TargetObjectType = typeof(DolefilTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DolefilTripStatement.GetUnpaidDolefilTrips";
            this.getUnpaidStanfilcoTrips = new PopupWindowShowAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.getUnpaidStanfilcoTrips.Caption = "Get Unpaid Trips";
            this.getUnpaidStanfilcoTrips.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateInvoiceFromSO_CustomizePopupWindowParams);
            this.getUnpaidStanfilcoTrips.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CreateInvoiceFromSO_Execute
            );
        }
        private void CreateInvoiceFromSO_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _DolefilTripStatement = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DolefilTripStatement;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(DolefilTrip));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(DolefilTrip), listViewId)
            ;
            if (_DolefilTripStatement.Customer == null) {throw new 
                ApplicationException("Customer not specified");}
            //if (_DolefilTripStatement.FromDate != DateTime.MinValue || 
            //_DolefilTripStatement.ToDate != DateTime.MinValue) {collectionSource
            //    .Criteria["ModelCriteria"] = CriteriaOperator.Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _DolefilTripStatement.Customer.No + 
            //    "' And [EntryDate] Between(#" + _DolefilTripStatement.FromDate.
            //    ToShortDateString() + "#, #" + _DolefilTripStatement.ToDate.
            //    ToShortDateString() + "#)");
            //}
            //else if (_DolefilTripStatement.Period > 0 && _DolefilTripStatement.Week > 0)
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _DolefilTripStatement.Customer.No +
            //    "' And [Period] = " + _DolefilTripStatement.Period + " And [Week] = " + _DolefilTripStatement.Week);
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _DolefilTripStatement.Customer.No + "'");
            //}

            if (_DolefilTripStatement.FromDate != DateTime.MinValue ||
            _DolefilTripStatement.ToDate != DateTime.MinValue)
            {
                collectionSource
                    .Criteria["ModelCriteria"] = CriteriaOperator.Parse(
                    "[Status] In ('Invoiced', 'PartiallyPaid') And [EntryDate] Between(#" + _DolefilTripStatement.FromDate.
                    ToShortDateString() + "#, #" + _DolefilTripStatement.ToDate.
                    ToShortDateString() + "#)");
            }
            else if (_DolefilTripStatement.Period > 0 && _DolefilTripStatement.Week > 0)
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse(
                "[Status] In ('Invoiced', 'PartiallyPaid') And [Period] = " + _DolefilTripStatement.Period + " And [Week] = " + _DolefilTripStatement.Week);
            }
            else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse(
                "[Status] In ('Invoiced', 'PartiallyPaid')");
            }
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void CreateInvoiceFromSO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (DolefilTrip item in e.PopupWindow.View.SelectedObjects) {
                DolefilTrip trip = _DolefilTripStatement.Session.GetObjectByKey<
                DolefilTrip>(item.Oid);
                DolefilTripCharge tripCharge = new DolefilTripCharge(
                _DolefilTripStatement.Session);
                tripCharge.DolefilTripStatementID = _DolefilTripStatement;
                tripCharge.SourceType = trip.SourceType;
                tripCharge.SourceNo = trip.SourceNo;
                tripCharge.SourceID = trip.Oid;
                tripCharge.Date = trip.EntryDate;
                tripCharge.TruckNo = trip.TruckNo;
                tripCharge.TrailerNo = trip.TrailerNo;
                tripCharge.DocumentNo = trip.DocumentNo;
                tripCharge.Tariff = trip.Tariff;
                tripCharge.AmountTruck = trip.AmountTruck.Value;
                tripCharge.AmountTrailer = trip.TrailerRental.Value;
                tripCharge.Billing = trip.Billing;
                tripCharge.VATAmount = trip.VATAmount;
                tripCharge.GrossBilling = trip.GrossBilling;
                tripCharge.WHTAmount = trip.WHTAmount;
                tripCharge.NetBilling = trip.NetBilling;
                tripCharge.Adjust = trip.OpenAmount == 0 ? trip.NetBilling : 
                trip.OpenAmount;
                tripCharge.Terms = trip.Terms != null ? trip.Terms : null;
                tripCharge.OpenAmount = trip.OpenAmount == 0 ? trip.NetBilling : 
                trip.OpenAmount;
                tripCharge.Save();
            }
        }
    }
}
