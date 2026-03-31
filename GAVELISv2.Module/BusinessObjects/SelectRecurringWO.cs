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

namespace GAVELISv2.Module.BusinessObjects
{
    public partial class SelectRecurringWO : ViewController
    {
        private PopupWindowShowAction selectRecurringWO;
        private WorkOrder _WorkOrder;
        public SelectRecurringWO()
        {
            this.TargetObjectType = typeof(WorkOrder);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "SelectRecurringWO";
            this.selectRecurringWO = new PopupWindowShowAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.selectRecurringWO.Caption = "Select Recurring";
            this.selectRecurringWO.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            SelectRecurringWO_CustomizePopupWindowParams);
            this.selectRecurringWO.Execute += new 
            PopupWindowShowActionExecuteEventHandler(SelectRecurringWO_Execute
            );
        }
        private void SelectRecurringWO_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _WorkOrder = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as WorkOrder;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(WorkOrder)
            );
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(WorkOrder), 
            listViewId);
            //if (_WorkOrder.Customer == null) {throw new 
            //    ApplicationException("Customer not specified");}
            //if (_WorkOrder.FromDate != DateTime.MinValue || 
            //_WorkOrder.ToDate != DateTime.MinValue) {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _WorkOrder.Customer.No + 
            //    "' And [EntryDate] Between(#" + _WorkOrder.
            //    FromDate.ToShortDateString() + "#, #" + _WorkOrder.
            //    ToDate.ToShortDateString() + "#)");
            //}
            //else if (_WorkOrder.Period>0 && _WorkOrder.Week>0)
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _WorkOrder.Customer.No +
            //    "' And [Period] = " + _WorkOrder.Period + " And [Week] = " + _WorkOrder.Week);
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _WorkOrder.Customer.No + "'");
            //}
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void SelectRecurringWO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (StanfilcoTrip item in e.PopupWindow.View.SelectedObjects) {
                //StanfilcoTrip trip = _WorkOrder.Session.
                //GetObjectByKey<StanfilcoTrip>(item.Oid);
                //StanfilcoTripCharge tripCharge = new StanfilcoTripCharge(
                //_WorkOrder.Session);
                //tripCharge.StanfilcoTripStatementID = _WorkOrder;
                //tripCharge.SourceType = trip.SourceType;
                //tripCharge.SourceNo = trip.SourceNo;
                //tripCharge.SourceID = trip.Oid;
                //tripCharge.Date = trip.EntryDate;
                //tripCharge.TruckNo = trip.TruckNo;
                //tripCharge.TrailerNo = trip.TrailerNo;
                //tripCharge.DTRNo = trip.DTRNo;
                //tripCharge.Tariff = trip.Tariff;
                //tripCharge.TruckerPay = trip.TruckerPay;
                //tripCharge.RateAdjmt = trip.RateAdjmt;
                //tripCharge.TrailerRental = trip.TrailerRental;
                //tripCharge.Insurance = trip.Insurance;
                //tripCharge.Billing = trip.Billing;
                //tripCharge.VATAmount = trip.VATAmount;
                //tripCharge.GrossBilling = trip.GrossBilling;
                //tripCharge.WHTAmount = trip.WHTAmount;
                //tripCharge.NetBilling = trip.NetBilling;
                //tripCharge.Adjust = trip.OpenAmount == 0 ? trip.NetBilling :
                //trip.OpenAmount;
                //tripCharge.Terms = trip.Terms != null ? trip.Terms : null;
                //tripCharge.OpenAmount = trip.OpenAmount == 0 ? trip.NetBilling :
                //trip.OpenAmount;
                //tripCharge.Save();
            }
        }
    }
}
