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
    public partial class GetUnpaidKDs : ViewController {
        private PopupWindowShowAction getUnpaidKD;
        private KDStatement _KDStatement;
        public GetUnpaidKDs() {
            this.TargetObjectType = typeof(KDStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "KDStatement.GetUnpaidKD";
            this.getUnpaidKD = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getUnpaidKD.Caption = "Get Unpaid KD";
            this.getUnpaidKD.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            GetUnpaidKD_CustomizePopupWindowParams);
            this.getUnpaidKD.Execute += new 
            PopupWindowShowActionExecuteEventHandler(GetUnpaidKD_Execute);
        }
        private void GetUnpaidKD_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _KDStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as KDStatement;
            #region Old Algorithms
//            //_Receipt.Save();
//            //_Receipt.Session.CommitTransaction();
//            IObjectSpace objectSpace = Application.CreateObjectSpace();
//            String listViewId = Application.FindListViewId(typeof(KDEntry));
//            CollectionSourceBase collectionSource = Application.
//            CreateCollectionSource(objectSpace, typeof(KDEntry), listViewId);
//            if (_KDStatement.Customer == null)
//            {
//                throw new ApplicationException(
//                    "Customer not specified");
//            }
//            if (_KDStatement.FromDate != DateTime.MinValue ||
//_KDStatement.ToDate != DateTime.MinValue)
//            {
//                collectionSource
//                    .Criteria["ModelCriteria"] = CriteriaOperator.Parse(
//                    "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
//                    + _KDStatement.Customer.No +
//                    "' And [EntryDate] Between(#" + _KDStatement.FromDate.
//                    ToShortDateString() + "#, #" + _KDStatement.ToDate.
//                    ToShortDateString() + "#)");
//            }
//            else if (_KDStatement.Period > 0 && _KDStatement.Week > 0)
//            {
//                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
//                Parse(
//                "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
//                + _KDStatement.Customer.No +
//                "' And [Period] = " + _KDStatement.Period + " And [Week] = " + _KDStatement.Week);
//            }
//            else
//            {
//                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
//                Parse(
//                "[Status] In ('Invoiced', 'PartiallyPaid') And [Customer.No] = '"
//                + _KDStatement.Customer.No + "'");
//            }
            #endregion
            #region New Algorithms
            if (_KDStatement.Customer == null)
            {
                throw new
                    ApplicationException("Customer not specified");
            }
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            // [Customer.No] = 'TC00001' And [Entry Date] >= #2015-06-01# And [Entry Date] < #2015-06-16#
            DateTime todate = _KDStatement.ToDate.AddDays(1);
            CriteriaOperator criteria = CriteriaOperator.Parse("[Status] = 'Invoiced' And [Customer.No] = ? And [EntryDate] >= ? And [EntryDate] < ?",
                _KDStatement.Customer.No, _KDStatement.FromDate, new DateTime(todate.Year, todate.Month, todate.Day));
            XPCollection<KDEntry> filtered = new XPCollection<KDEntry>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("BillSeq", DevExpress.Xpo.DB.SortingDirection.Ascending));
            for (int i = 0; i < filtered.Count; i++)
            {
                object obj = filtered[i];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            // StanfilcoTrip_UnpaidTrips_Selector
            string viewId = "KDEntry_ListView_Unpaid";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(KDEntry), viewId);
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
        private void GetUnpaidKD_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            foreach (KDEntry item in e.PopupWindow.View.SelectedObjects) {
                KDEntry trip = _KDStatement.Session.GetObjectByKey<KDEntry>(item
                .Oid);
                KDCharge kd = new KDCharge(_KDStatement.Session);
                kd.KDStatementID = _KDStatement;
                kd.KDEntryId = trip;
                kd.SourceType = trip.SourceType;
                kd.SourceNo = trip.SourceNo;
                kd.SourceID = trip.Oid;
                kd.Seq = trip.BillSeq;
                kd.Date = trip.EntryDate;
                kd.TruckNo = trip.TruckNo;
                kd.TrailerNo = trip.TrailerNo;
                kd.Driver = trip.Driver;
                kd.DTRNo = trip.TripNo;
                kd.Area = trip.Area;
                kd.KmRun = trip.KmRun;
                kd.KDAmount = trip.KDAmount;
                kd.FuelSubsidy = trip.FuelSubsidy;
                kd.TrailerRental = trip.TrailerRental;
                kd.Billing = trip.Billing;
                kd.VATAmount = trip.VATAmount;
                kd.GrossBilling = trip.GrossBilling;
                kd.WHTAmount = trip.WHTAmount;
                kd.NetBilling = trip.GrossBilling;
                kd.Adjust = trip.OpenAmount == 0 ? trip.GrossBilling : trip.
                OpenAmount;
                kd.Terms = trip.Terms != null ? trip.Terms : null;
                kd.OpenAmount = trip.OpenAmount == 0 ? trip.GrossBilling : trip.
                OpenAmount;
                kd.Pay = true;
                kd.Save();
            }
        }
    }
}
