using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiptFuelTagToTripController : ViewController
    {
        private ReceiptFuel _Receipt;
        private PopupWindowShowAction _TagToTrip;
        public ReceiptFuelTagToTripController()
        {
            this.TargetObjectType = typeof(ReceiptFuel);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.TagToTrip", this.GetType().Name);
            _TagToTrip = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            _TagToTrip.Caption = "Tag to Trip";
            _TagToTrip.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(TagToTrip_CustomizePopupWindowParams);
            _TagToTrip.Execute += new PopupWindowShowActionExecuteEventHandler(TagToTrip_Execute);
        }

        private void TagToTrip_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (GenJournalHeader item in e.PopupWindow.View.SelectedObjects)
            {
                if (item.SourceType.Code == "ST")
                {
                    StanfilcoTrip ost = _Receipt.Session.GetObjectByKey<StanfilcoTrip>(item.Oid);
                    foreach (ReceiptFuelDetail rfdt in _Receipt.ReceiptFuelDetails)
                    {
                        if (rfdt.Quantity > 0)
                        {
                            ReceiptFuelDetail tmp = ost.Session.
                            GetObjectByKey<ReceiptFuelDetail>(rfdt.Oid);
                            FuelRegister fuelRegister = new FuelRegister(ost.
                            Session);
                            fuelRegister.TripID = ost;
                            fuelRegister.ReferenceNo = ost.DTRNo;
                            fuelRegister.SourceID = _Receipt.Oid;
                            fuelRegister.SourceType = _Receipt.SourceType;
                            fuelRegister.SourceNo = _Receipt.SourceNo;
                            fuelRegister.TruckOrGenset = _Receipt.TruckOrGenset;
                            fuelRegister.TruckNo = _Receipt.TruckNo != null ? _Receipt.TruckNo :
                            null;
                            fuelRegister.GensetNo = _Receipt.GensetNo != null ? _Receipt.
                            GensetNo : null;
                            fuelRegister.Driver = _Receipt.Driver != null ? _Receipt.Driver :
                            null;
                            fuelRegister.Total = tmp.Total;
                            fuelRegister.ReceiptFuelDetailID = tmp;
                            fuelRegister.Save();
                            _Receipt.TripUsed = ost;
                            ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(ost.Session);
                            rfud.TripNo = ost;
                            _Receipt.ReceiptFuelUsageDetails.Add(rfud);
                            _Receipt.Save();
                        }
                    }
                    ost.Save();
                }
                else if (item.SourceType.Code == "DF")
                {
                    DolefilTrip odf = _Receipt.Session.GetObjectByKey<DolefilTrip>(item.Oid);
                    foreach (ReceiptFuelDetail rfdt in _Receipt.ReceiptFuelDetails)
                    {
                        if (rfdt.Quantity > 0)
                        {
                            ReceiptFuelDetail tmp = odf.Session.
                            GetObjectByKey<ReceiptFuelDetail>(rfdt.Oid);
                            FuelRegister fuelRegister = new FuelRegister(odf.
                            Session);
                            fuelRegister.TripID = odf;
                            fuelRegister.ReferenceNo = odf.DocumentNo;
                            fuelRegister.SourceID = _Receipt.Oid;
                            fuelRegister.SourceType = _Receipt.SourceType;
                            fuelRegister.SourceNo = _Receipt.SourceNo;
                            fuelRegister.TruckOrGenset = _Receipt.TruckOrGenset;
                            fuelRegister.TruckNo = _Receipt.TruckNo != null ? _Receipt.TruckNo :
                            null;
                            fuelRegister.GensetNo = _Receipt.GensetNo != null ? _Receipt.
                            GensetNo : null;
                            fuelRegister.Driver = _Receipt.Driver != null ? _Receipt.Driver :
                            null;
                            fuelRegister.Total = tmp.Total;
                            fuelRegister.ReceiptFuelDetailID = tmp;
                            fuelRegister.Save();
                            _Receipt.TripUsed = odf;
                            ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(odf.Session);
                            rfud.TripNo = odf;
                            _Receipt.ReceiptFuelUsageDetails.Add(rfud);
                            _Receipt.Save();
                        }
                    }
                    odf.Save();
                }
                else if (item.SourceType.Code == "OT")
                {
                    OtherTrip oot = _Receipt.Session.GetObjectByKey<OtherTrip>(item.Oid);
                    foreach (ReceiptFuelDetail rfdt in _Receipt.ReceiptFuelDetails)
                    {
                        if (rfdt.Quantity > 0)
                        {
                            ReceiptFuelDetail tmp = oot.Session.
                            GetObjectByKey<ReceiptFuelDetail>(rfdt.Oid);
                            FuelRegister fuelRegister = new FuelRegister(oot.
                            Session);
                            fuelRegister.TripID = oot;
                            fuelRegister.ReferenceNo = oot.TripReferenceNo;
                            fuelRegister.SourceID = _Receipt.Oid;
                            fuelRegister.SourceType = _Receipt.SourceType;
                            fuelRegister.SourceNo = _Receipt.SourceNo;
                            fuelRegister.TruckOrGenset = _Receipt.TruckOrGenset;
                            fuelRegister.TruckNo = _Receipt.TruckNo != null ? _Receipt.TruckNo :
                            null;
                            fuelRegister.GensetNo = _Receipt.GensetNo != null ? _Receipt.
                            GensetNo : null;
                            fuelRegister.Driver = _Receipt.Driver != null ? _Receipt.Driver :
                            null;
                            fuelRegister.Total = tmp.Total;
                            fuelRegister.ReceiptFuelDetailID = tmp;
                            fuelRegister.Save();
                            _Receipt.TripUsed = oot;
                            ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(oot.Session);
                            rfud.TripNo = oot;
                            _Receipt.ReceiptFuelUsageDetails.Add(rfud);
                            _Receipt.Save();
                        }
                    }
                    oot.Save();
                }
            }
        }

        private void TagToTrip_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _Receipt = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as ReceiptFuel;

            string viewId = "GenJournalHeader_UseInTrip";
            CollectionSourceBase collectionSource1 = Application.CreateCollectionSource(Application.CreateObjectSpace(), typeof(GenJournalHeader), viewId);
            collectionSource1.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[SourceType.Code] In ('ST', 'DF', 'OT')");
            e.View = Application.CreateListView(viewId, collectionSource1, true);
        }
    }
}
