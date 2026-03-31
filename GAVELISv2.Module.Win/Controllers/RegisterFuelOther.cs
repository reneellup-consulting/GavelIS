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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class RegisterFuelOther : ViewController
    {
        private PopupWindowShowAction registerFuel;
        private OtherTrip _otherTrip;
        public RegisterFuelOther()
        {
            this.TargetObjectType = typeof(OtherTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "OtherTrip.RegisterFuel";
            this.registerFuel = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            //this.registerFuel.TargetObjectsCriteria = "NONE";
            this.registerFuel.Caption="Register Fuel";
            this.registerFuel.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            RegisterFuelOther_CustomizePopupWindowParams);
            this.registerFuel.Execute += new 
            PopupWindowShowActionExecuteEventHandler(RegisterFuelOther_Execute);
        }
        private void RegisterFuelOther_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _otherTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as OtherTrip;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "ReceiptFuel_ListView_All"; //Application.FindListViewId(typeof(ReceiptFuel));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(ReceiptFuel), listViewId)
            ;
            //collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.Parse(
            //"[Status] In ('Received', 'PartiallyPaid', 'Paid')");
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void RegisterFuelOther_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            ReceiptFuel rcpt = _otherTrip.Session.GetObjectByKey<ReceiptFuel
            >(((ReceiptFuel)e.PopupWindow.View.SelectedObjects[0]).Oid);
            //_StanfilcoTrip.ReceiptNo = rcpt;
            //_StanfilcoTrip.ReferenceNo = rcpt.SourceNo;
            //_StanfilcoTrip.Vendor = rcpt.Vendor;
            //_StanfilcoTrip.VendorAddress = rcpt.VendorAddress;
            //_StanfilcoTrip.BillToAddress = rcpt.CompanyInfo.FullBillAddress;
            foreach (ReceiptFuelDetail item in ((ReceiptFuel)e.PopupWindow.View.
            SelectedObjects[0]).ReceiptFuelDetails) {
                if (item.Quantity > 0) {
                    ReceiptFuelDetail tmp = _otherTrip.Session.
                    GetObjectByKey<ReceiptFuelDetail>(item.Oid);
                    FuelRegister fuelRegister = new FuelRegister(_otherTrip.
                    Session);
                    fuelRegister.TripID = _otherTrip;
                    fuelRegister.ReferenceNo = _otherTrip.TripNo;
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
                    rcpt.TripUsed = _otherTrip;
                    ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(_otherTrip.Session);
                    rfud.TripNo = _otherTrip;
                    rcpt.ReceiptFuelUsageDetails.Add(rfud);
                    rcpt.Save();
                }
            }
        }
    }
}
