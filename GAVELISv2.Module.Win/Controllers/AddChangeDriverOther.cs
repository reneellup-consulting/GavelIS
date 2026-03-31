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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class AddChangeDriverOther : ViewController {
        private IObjectSpace _ObjectSpace;
        private OtherTrip _Trip;
        private DriverRegistry _Obj;
        private PopupWindowShowAction addChangeDriver;
        public AddChangeDriverOther() {
            this.TargetObjectType = typeof(OtherTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeDriver", this.GetType(
            ).Name);
            this.addChangeDriver = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeDriver.Caption = "Add/Change Driver";
            this.addChangeDriver.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeDriverOther_CustomizePopupWindowParams);
            this.addChangeDriver.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            AddChangeDriverOther_Execute);
        }
        private void AddChangeDriverOther_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as OtherTrip;
            ObjectSpace.CommitChanges();
            OtherTrip thisTrip = _ObjectSpace.GetObjectByKey<OtherTrip>(_Trip.
            Oid);
            _Obj = _ObjectSpace.CreateObject<DriverRegistry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.TripNo;
            _Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            _Obj.Driver = thisTrip.TruckNo != null ? thisTrip.TruckNo.Operator 
            != null ? thisTrip.TruckNo.Operator : null : null;
            _Obj.Tariff = thisTrip.Tariff != null ? thisTrip.Tariff : null;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "AddChangeDriver_Detail", true, _Obj);
        }
        private void AddChangeDriverOther_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(_Trip);
            try
            {
                ObjectSpace.Refresh();
            }
            catch (Exception)
            {
            }
        }
    }
}
