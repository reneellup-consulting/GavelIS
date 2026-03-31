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
    public partial class RegisterFuel : ViewController {
        private PopupWindowShowAction registerFuel;
        private StanfilcoTrip _StanfilcoTrip;
        public RegisterFuel() {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StanfilcoTrip.RegisterFuel";
            this.registerFuel = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            //this.registerFuel.TargetObjectsCriteria = "NONE";
            this.registerFuel.Caption="Register Fuel";
            this.registerFuel.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            RegisterFuel_CustomizePopupWindowParams);
            this.registerFuel.Execute += new 
            PopupWindowShowActionExecuteEventHandler(RegisterFuel_Execute);
        }
        private void RegisterFuel_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _StanfilcoTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as StanfilcoTrip;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "ReceiptFuel_ListView_All"; //Application.FindListViewId(typeof(ReceiptFuel));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(ReceiptFuel), listViewId)
            ;
            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(
            //"[TripUsed] Is Null && [Status] In ('Received', 'PartiallyPaid', 'Paid')");
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void RegisterFuel_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            ReceiptFuel rcpt = _StanfilcoTrip.Session.GetObjectByKey<ReceiptFuel
            >(((ReceiptFuel)e.PopupWindow.View.SelectedObjects[0]).Oid);
            //_StanfilcoTrip.ReceiptNo = rcpt;
            //_StanfilcoTrip.ReferenceNo = rcpt.SourceNo;
            //_StanfilcoTrip.Vendor = rcpt.Vendor;
            //_StanfilcoTrip.VendorAddress = rcpt.VendorAddress;
            //_StanfilcoTrip.BillToAddress = rcpt.CompanyInfo.FullBillAddress;
            foreach (ReceiptFuelDetail item in ((ReceiptFuel)e.PopupWindow.View.
            SelectedObjects[0]).ReceiptFuelDetails) {
                if (item.Quantity > 0) {
                    ReceiptFuelDetail tmp = _StanfilcoTrip.Session.
                    GetObjectByKey<ReceiptFuelDetail>(item.Oid);
                    FuelRegister fuelRegister = new FuelRegister(_StanfilcoTrip.
                    Session);
                    fuelRegister.TripID = _StanfilcoTrip;
                    fuelRegister.ReferenceNo = _StanfilcoTrip.DTRNo;
                    fuelRegister.SourceID = rcpt.Oid;
                    fuelRegister.SourceType = rcpt.SourceType;
                    fuelRegister.SourceNo = rcpt.SourceNo;
                    fuelRegister.TruckOrGenset = rcpt.TruckOrGenset;
                    fuelRegister.TruckNo = rcpt.TruckNo != null ? rcpt.TruckNo : 
                    null;
                    fuelRegister.GensetNo = rcpt.GensetNo != null ? rcpt.
                    GensetNo : null;
                    fuelRegister.Driver = rcpt.Driver != null ? rcpt.Driver : 
                    null;
                    fuelRegister.Total = tmp.Total;
                    fuelRegister.ReceiptFuelDetailID = tmp;
                    fuelRegister.Save();
                    rcpt.TripUsed = _StanfilcoTrip;
                    ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(_StanfilcoTrip.Session);
                    rfud.TripNo = _StanfilcoTrip;
                    rcpt.ReceiptFuelUsageDetails.Add(rfud);
                    rcpt.Save();
                }
            }
        }
    }
}
