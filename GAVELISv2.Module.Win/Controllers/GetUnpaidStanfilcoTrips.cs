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
    public partial class GetUnpaidStanfilcoTrips : ViewController
    {
        private PopupWindowShowAction getUnpaidStanfilcoTrips;
        private StanfilcoTripStatement _StanfilcoTripStatement;
        public GetUnpaidStanfilcoTrips()
        {
            this.TargetObjectType = typeof(StanfilcoTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StanfilcoTripStatement.GetUnpaidStanfilcoTrips";
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
            _StanfilcoTripStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as StanfilcoTripStatement;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            //IObjectSpace objectSpace = Application.CreateObjectSpace();
            //String listViewId = Application.FindListViewId(typeof(StanfilcoTrip)
            //);
            //CollectionSourceBase collectionSource = Application.
            //CreateCollectionSource(objectSpace, typeof(StanfilcoTrip), 
            //listViewId);
            if (_StanfilcoTripStatement.Customer == null) {throw new 
                ApplicationException("Customer not specified");
            }
            #region Old Algorithms
            //if (_StanfilcoTripStatement.FromDate != DateTime.MinValue || 
            //_StanfilcoTripStatement.ToDate != DateTime.MinValue) {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _StanfilcoTripStatement.Customer.No + 
            //    "' And [EntryDate] Between(#" + _StanfilcoTripStatement.
            //    FromDate.ToShortDateString() + "#, #" + _StanfilcoTripStatement.
            //    ToDate.ToShortDateString() + "#)");
            //}
            //else if (_StanfilcoTripStatement.Period>0 && _StanfilcoTripStatement.Week>0)
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _StanfilcoTripStatement.Customer.No +
            //    "' And [Period] = " + _StanfilcoTripStatement.Period + " And [Week] = " + _StanfilcoTripStatement.Week);
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _StanfilcoTripStatement.Customer.No + "'");
            //}
            #endregion
            #region New Algorithms
            // [BYear] = 2016 And [Period] = 11 And [Week] = 4 And [Status] = 'Invoiced' And [Trip Customer.No] = 'tret'
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            CriteriaOperator criteria = CriteriaOperator.Parse("[BYear] = ? And [Period] = ? And [Week] = ? And [Status] = 'Invoiced' And [TripCustomer.No] = ?",
                _StanfilcoTripStatement.Year, _StanfilcoTripStatement.Period, _StanfilcoTripStatement.Week, _StanfilcoTripStatement.Customer.No);
            XPCollection<StanfilcoTrip> filtered = new XPCollection<StanfilcoTrip>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("BillSeq", DevExpress.Xpo.DB.SortingDirection.Ascending));
            for (int i = 0; i < filtered.Count; i++)
            {
                object obj = filtered[i];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            // StanfilcoTrip_UnpaidTrips_Selector
            string viewId = "StanfilcoTrip_UnpaidTrips_Selector";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(StanfilcoTrip), viewId);
            if (keysToShow1.Count > 0){
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
                collectionSource1.Criteria["GKey"] = new InOperator("GKey", new ArrayList[] {});
            }
            e.View = Application.CreateListView(viewId, collectionSource1, true);
            #endregion
        }
        private void CreateInvoiceFromSO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (StanfilcoTrip item in e.PopupWindow.View.SelectedObjects) {
                StanfilcoTrip trip = _StanfilcoTripStatement.Session.
                GetObjectByKey<StanfilcoTrip>(item.Oid);
                StanfilcoTripCharge tripCharge = new StanfilcoTripCharge(
                _StanfilcoTripStatement.Session);
                tripCharge.StanfilcoTripStatementID = _StanfilcoTripStatement;
                tripCharge.Seq = trip.BillSeq;
                tripCharge.SourceType = trip.SourceType;
                tripCharge.SourceNo = trip.SourceNo;
                tripCharge.SourceID = trip.Oid;
                tripCharge.Date = trip.EntryDate;
                tripCharge.TruckNo = trip.TruckNo;
                tripCharge.TrailerNo = trip.TrailerNo;
                tripCharge.DTRNo = trip.DTRNo;
                tripCharge.Tariff = trip.Tariff;
                tripCharge.TruckerPay = trip.TruckerPay;
                tripCharge.TariffTruckerPay = trip.TariffTruckerPay;
                tripCharge.RateAdjmt = trip.RateAdjmt;
                tripCharge.TariffFuelSubsidy = trip.TariffFuelSubsidy;
                tripCharge.TrailerRental = trip.TrailerRental;
                tripCharge.Insurance = trip.Insurance;
                tripCharge.Billing = trip.NewBilling;
                tripCharge.TariffBilling = trip.TariffNewBilling;
                tripCharge.VATAmount = trip.NewVatAmount;
                tripCharge.TariffVATAmount = trip.TariffNewVatAmount;
                tripCharge.GrossBilling = trip.NewGrossBilling;
                tripCharge.TariffGrossBilling = trip.TariffNewGrossBilling;
                tripCharge.WHTAmount = trip.NewWhtAmount;
                tripCharge.TariffWHTAmount = trip.TariffNewWhtAmount;
                tripCharge.NetBilling = trip.NewNetBilling;
                tripCharge.TariffNetBilling = trip.TariffNewNetBilling;
                tripCharge.Adjust = trip.OpenAmount == 0 ? trip.NewNetBilling :
                trip.OpenAmount;
                tripCharge.Terms = trip.Terms != null ? trip.Terms : null;
                tripCharge.OpenAmount = trip.OpenAmount == 0 ? trip.NewNetBilling :
                trip.OpenAmount;
                tripCharge.Save();
            }
        }
    }
}
