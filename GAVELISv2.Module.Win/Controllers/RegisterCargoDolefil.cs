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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class RegisterCargoDolefil : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private DolefilTrip _Trip;
        private CargoRegistry _Obj;
        private PopupWindowShowAction registerCargo;
        public RegisterCargoDolefil()
        {
            this.TargetObjectType = typeof(DolefilTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.RegisterCargo", this.GetType().
            Name);
            this.registerCargo = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.registerCargo.Caption = "Register Cargo";
            this.registerCargo.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(registerCargo_CustomizePopupWindowParams);
            this.registerCargo.Execute += new PopupWindowShowActionExecuteEventHandler(registerCargo_Execute);
        }

        void registerCargo_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void registerCargo_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as DolefilTrip;
            ObjectSpace.CommitChanges();
            DolefilTrip thisTrip = _ObjectSpace.GetObjectByKey<DolefilTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<CargoRegistry>();
            _Obj.TripID = thisTrip;
            _Obj.Tariff = thisTrip.Tariff;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.DocumentNo;
            _Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            _Obj.Driver = thisTrip.Driver != null ? thisTrip.Driver : null;
            _Obj.TrailerNo = thisTrip.TrailerNo != null ? thisTrip.TrailerNo :
            null;
            _Obj.GensetNo = thisTrip.GensetNo != null ? thisTrip.GensetNo : null;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RegisterCargo_Detail", true, _Obj);
        }
    }
}
