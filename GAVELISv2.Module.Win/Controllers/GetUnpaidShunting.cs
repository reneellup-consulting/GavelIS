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
    public partial class GetUnpaidShunting : ViewController
    {
        private PopupWindowShowAction getUnpaidShunting;
        private ShuntingStatement _ShuntingStatement;
        public GetUnpaidShunting()
        {
            this.TargetObjectType = typeof(ShuntingStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "ShuntingStatement.GetUnpaidShunting";
            this.getUnpaidShunting = new PopupWindowShowAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.getUnpaidShunting.Caption = "Get Unpaid Shunting";
            this.getUnpaidShunting.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            GetUnpaidShunting_CustomizePopupWindowParams);
            this.getUnpaidShunting.Execute += new 
            PopupWindowShowActionExecuteEventHandler(GetUnpaidShunting_Execute
            );
        }
        private void GetUnpaidShunting_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ShuntingStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as ShuntingStatement;
            #region Old Algorithms
            ////_Receipt.Save();
            ////_Receipt.Session.CommitTransaction();
            //IObjectSpace objectSpace = Application.CreateObjectSpace();
            //String listViewId = Application.FindListViewId(typeof(ShuntingEntry)
            //);
            //CollectionSourceBase collectionSource = Application.
            //CreateCollectionSource(objectSpace, typeof(ShuntingEntry), 
            //listViewId);
            //if (_ShuntingStatement.Customer == null) {throw new 
            //    ApplicationException("Customer not specified");}
            //if (_ShuntingStatement.FromDate != DateTime.MinValue || 
            //_ShuntingStatement.ToDate != DateTime.MinValue) {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '" 
            //    + _ShuntingStatement.Customer.No + 
            //    "' And [EntryDate] Between(#" + _ShuntingStatement.
            //    FromDate.ToShortDateString() + "#, #" + _ShuntingStatement.
            //    ToDate.ToShortDateString() + "#)");
            //}
            //else if (_ShuntingStatement.Period > 0 && _ShuntingStatement.Week > 0)
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _ShuntingStatement.Customer.No +
            //    "' And [Period] = " + _ShuntingStatement.Period + " And [Week] = " + _ShuntingStatement.Week);
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
            //    + _ShuntingStatement.Customer.No + "'");
            //}

            //e.View = Application.CreateListView(listViewId, collectionSource, 
            //true);
            #endregion
            #region New Algorithms
            if (_ShuntingStatement.Customer == null)
            {
                throw new
                    ApplicationException("Customer not specified");
            }
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            // [Customer.No] = 'TC00001' And [Entry Date] >= #2015-06-01# And [Entry Date] < #2015-06-16#
            DateTime todate = _ShuntingStatement.ToDate.AddDays(1);
            CriteriaOperator criteria = CriteriaOperator.Parse("[Status] = 'Invoiced' And [Customer.No] = ? And [EntryDate] >= ? And [EntryDate] < ?",
                _ShuntingStatement.Customer.No, _ShuntingStatement.FromDate, new DateTime(todate.Year, todate.Month, todate.Day));
            XPCollection<ShuntingEntry> filtered = new XPCollection<ShuntingEntry>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("BillSeq", DevExpress.Xpo.DB.SortingDirection.Ascending));
            for (int i = 0; i < filtered.Count; i++)
            {
                object obj = filtered[i];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            // StanfilcoTrip_UnpaidTrips_Selector
            string viewId = "ShuntingEntry_ListView_Unpaid";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ShuntingEntry), viewId);
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
        private void GetUnpaidShunting_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (ShuntingEntry item in e.PopupWindow.View.SelectedObjects) {
                ShuntingEntry trip = _ShuntingStatement.Session.
                GetObjectByKey<ShuntingEntry>(item.Oid);
                ShuntingCharge shunting = new ShuntingCharge(
                _ShuntingStatement.Session);
                shunting.ShuntingStatementID = _ShuntingStatement;
                shunting.ShuntingEntryId = trip;
                shunting.SourceType = trip.SourceType;
                shunting.SourceNo = trip.SourceNo;
                shunting.SourceID = trip.Oid;
                shunting.Seq = trip.BillSeq;
                shunting.Date = trip.EntryDate;
                shunting.TruckNo = trip.TruckNo;
                shunting.TrailerNo = trip.TrailerNo;
                shunting.DTRNo = trip.TripNo;
                shunting.Tariff = ((StanfilcoTrip)trip.TripID).Tariff;
                shunting.ShuntingTo = trip.ShuntingTo!=null?trip.ShuntingTo:null;
                shunting.TotalKms = trip.TotalKms;
                shunting.RatePerKms = trip.RatePerKms;
                shunting.Billing = trip.Total;
                shunting.VATAmount = trip.VATAmount;
                shunting.GrossBilling = trip.GrossBilling;
                shunting.WHTAmount = 0;
                shunting.NetBilling = trip.GrossBilling;
                shunting.Adjust = trip.OpenAmount == 0 ? trip.GrossBilling :
                trip.OpenAmount;
                shunting.Terms = trip.Terms != null ? trip.Terms : null;
                shunting.OpenAmount = trip.OpenAmount == 0 ? trip.GrossBilling :
                trip.OpenAmount;
                shunting.Pay = true;
                shunting.Save();
            }
        }
    }
}
