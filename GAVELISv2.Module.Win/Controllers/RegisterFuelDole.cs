using System;
using System.Linq;
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
    public partial class RegisterFuelDole : ViewController
    {
        private PopupWindowShowAction registerFuel;
        private DolefilTrip _StanfilcoTrip;
        public RegisterFuelDole()
        {
            this.TargetObjectType = typeof(DolefilTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DolefilTrip.RegisterFuel";
            this.registerFuel = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            //this.registerFuel.TargetObjectsCriteria = "NONE";
            this.registerFuel.Caption="Register Fuel";
            this.registerFuel.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            RegisterFuelDole_CustomizePopupWindowParams);
            this.registerFuel.Execute += new 
            PopupWindowShowActionExecuteEventHandler(RegisterFuelDole_Execute);
        }
        private void RegisterFuelDole_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _StanfilcoTrip = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DolefilTrip;
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
        private void RegisterFuelDole_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            ReceiptFuel rcpt = _StanfilcoTrip.Session.GetObjectByKey<ReceiptFuel
            >(((ReceiptFuel)e.PopupWindow.View.SelectedObjects[0]).Oid);
            //StringBuilder sbr = new StringBuilder(string.Format("{0},", rcpt.DtrNo));
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
                    fuelRegister.ReferenceNo = _StanfilcoTrip.DocumentNo;
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
                    // Populate DTR # in ReceiptFuel module
                    var data = rcpt.ReceiptFuelUsageDetails.Where(o => o.TripNo == _StanfilcoTrip).FirstOrDefault();
                    if (data == null)
                    {
                        ReceiptFuelUsageDetail rfud = ReflectionHelper.CreateObject<ReceiptFuelUsageDetail>(_StanfilcoTrip.Session);
                        rfud.TripNo = _StanfilcoTrip;
                        rcpt.ReceiptFuelUsageDetails.Add(rfud);
                        rcpt.Save();
                        //sbr.AppendFormat("{0},", _StanfilcoTrip.TripReferenceNo);
                    }
                }
            }
            //sbr = TrimEnd(sbr);
            //if (sbr.Length > 0)
            //{
            //    sbr.Remove(sbr.Length - 1, 1);
            //}
        }
        private StringBuilder TrimEnd(StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }
    }
}
