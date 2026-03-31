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
    public partial class RegisterCargoOther : ViewController {
        private IObjectSpace _ObjectSpace;
        private OtherTrip _Trip;
        private CargoRegistry _Obj;
        private PopupWindowShowAction registerCargo;
        public RegisterCargoOther() {
            this.TargetObjectType = typeof(OtherTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.RegisterCargo", this.GetType().
            Name);
            this.registerCargo = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.registerCargo.Caption = "Register Cargo";
            this.registerCargo.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            RegisterCargoOther_CustomizePopupWindowParams);
            this.registerCargo.Execute += new 
            PopupWindowShowActionExecuteEventHandler(RegisterCargoOther_Execute)
            ;
        }
        private void RegisterCargoOther_CustomizePopupWindowParams(object sender
        , CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as OtherTrip;
            ObjectSpace.CommitChanges();
            OtherTrip thisTrip = _ObjectSpace.GetObjectByKey<OtherTrip>(_Trip.
            Oid);
            _Obj = _ObjectSpace.CreateObject<CargoRegistry>();
            _Obj.TripID = thisTrip;
            //_Obj.Tariff=thisTrip.Tariff;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.TripNo;
            _Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            _Obj.Driver = thisTrip.Driver != null ? thisTrip.Driver : null;
            _Obj.TrailerNo = thisTrip.TrailerNo != null ? thisTrip.TrailerNo : 
            null;
            _Obj.GensetNo = thisTrip.GensetNo != null ? thisTrip.GensetNo : thisTrip.TrailerNo != null ? thisTrip.TrailerNo.
            GensetNo != null ? thisTrip.TrailerNo.GensetNo : null : null;
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "RegisterCargo_Detail", true, _Obj);
        }
        private void RegisterCargoOther_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(_Trip);
            ObjectSpace.Refresh();
        }
    }
}
