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
using DevExpress.XtraEditors;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetUnpaidFuelReceipts : ViewController
    {
        private PopupWindowShowAction getUnpaidFuelReceipts;
        private FuelStatementOfAccount _fuelStatement;

        public GetUnpaidFuelReceipts()
        {
            this.TargetObjectType = typeof(FuelStatementOfAccount);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GetUnpaidFuelReceipts";
            this.getUnpaidFuelReceipts = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.getUnpaidFuelReceipts.Caption = "Get Unpaid Receipts";
            this.getUnpaidFuelReceipts.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(getUnpaidFuelReceipts_CustomizePopupWindowParams);
            this.getUnpaidFuelReceipts.Execute +=new PopupWindowShowActionExecuteEventHandler(getUnpaidFuelReceipts_Execute);
        }

        void getUnpaidFuelReceipts_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            int n = 1;
            foreach (ReceiptFuel item in e.PopupWindow.View.SelectedObjects)
            {
                ReceiptFuel recfl = _fuelStatement.Session.
                GetObjectByKey<ReceiptFuel>(item.Oid);
                FuelSoaDetail fsDetail = new FuelSoaDetail(_fuelStatement.Session);
                fsDetail.FuelStatementOfAccountID = _fuelStatement;
                fsDetail.Pay = true;
                fsDetail.Seq = n++;
                fsDetail.Source = recfl;
                fsDetail.EntryDate = recfl.EntryDate;
                fsDetail.TypeOfTrip = recfl.TripType;
                fsDetail.TruckNo = recfl.TruckNo;
                fsDetail.Driver = recfl.Driver;
                fsDetail.OdoRead = recfl.OdoRead;
                fsDetail.RefNo = recfl.ReferenceNo;
                fsDetail.InvoiceNo = recfl.InvoiceNo;
                fsDetail.DtrsTagged = recfl.Tagged;
                fsDetail.CodeNo = recfl.CodeNo != null?recfl.CodeNo.UnitType.Code:null;
                fsDetail.Origin = recfl.Origin;
                fsDetail.Destination = recfl.Destination;
                fsDetail.Tad = recfl.Tad;
                fsDetail.TotalQty = recfl.TotalQty.Value;
                fsDetail.Price = recfl.Price.Value;
                fsDetail.Adjust = recfl.OpenAmount == 0 ? recfl.Total.Value :
                recfl.OpenAmount;
                fsDetail.OpenAmount = recfl.OpenAmount == 0 ? recfl.Total.Value :
                recfl.OpenAmount;
                fsDetail.DtrNos = recfl.DtrNoForRpt;
                fsDetail.Save();
            }
        }

        void getUnpaidFuelReceipts_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _fuelStatement = ((DevExpress.ExpressApp.DetailView)this.
             View).CurrentObject as FuelStatementOfAccount;

            // Vendor must not be empty
            if (_fuelStatement.Vendor == null)
            {
                throw new
                    ApplicationException("Vendor must be specified");
            }
            // Customer must not be empty
            if (_fuelStatement.Customer == null)
            {
                throw new
                    ApplicationException("Customer must be specified");
            }
            // Validate Period Start and Period End
            if (_fuelStatement.PeriodStart == null)
            {
                throw new ApplicationException("Period Start must be specified");
            }
            if (_fuelStatement.PeriodEnd == null)
            {
                throw new ApplicationException("Period End must be specified");
            }
            if (_fuelStatement.PeriodStart > _fuelStatement.PeriodEnd)
            {
                throw new ApplicationException("Period Start cannot be after Period End");
            }

            #region Algorithms
            ArrayList keysToShow1 = new ArrayList();
            int dCount = 0;
            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[IsInFuelSoaDetail] <> True And [EntryDate] >= #{0}# And [EntryDate] < #{1}# And [Vendor.No] = '{2}' And Not [Status] In ('Current', 'Paid')", _fuelStatement.PeriodStart.ToString("yyyy-MM-dd"), _fuelStatement.PeriodEnd.AddDays(1).ToString("yyyy-MM-dd"), _fuelStatement.Vendor.No);
            //if (_fuelStatement.TripType != null)
            //{
            //    sbCrit.AppendFormat(" And [TripType.Oid] = {0}", _fuelStatement.TripType.Oid);
            //}
            CriteriaOperator criteria = CriteriaOperator.Parse(sbCrit.ToString());
            XPCollection<ReceiptFuel> filtered = new XPCollection<ReceiptFuel>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("SourceNo", DevExpress.Xpo.DB.SortingDirection.Ascending));
            for (int i = 0; i < filtered.Count; i++)
            {
                object obj = filtered[i];
                dCount++;
                keysToShow1.Add(ObjectSpace.GetKeyValue(obj));
            }
            string viewId = "ReceiptFuel_Unpaid_Selector";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(ReceiptFuel), viewId);
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
                throw new UserFriendlyException("No unpaid receipts found for the specified criteria. Please check the period dates, vendor, and trip type.");
            }
            e.View = Application.CreateListView(viewId, collectionSource1, true);
            #endregion
        }
    }
}
