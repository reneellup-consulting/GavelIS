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
    public partial class AddChangeDriverDole : ViewController {
        private IObjectSpace _ObjectSpace;
        private DolefilTrip _Trip;
        private DriverRegistry _Obj;
        private PopupWindowShowAction addChangeDriver;
        public AddChangeDriverDole() {
            this.TargetObjectType = typeof(DolefilTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeDriver", this.GetType(
            ).Name);
            this.addChangeDriver = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeDriver.Caption = "Add/Change Driver";
            this.addChangeDriver.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeDriverDole_CustomizePopupWindowParams);
            this.addChangeDriver.Execute += new 
            PopupWindowShowActionExecuteEventHandler(AddChangeDriverDole_Execute
            );
        }
        private void AddChangeDriverDole_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as DolefilTrip;
            ObjectSpace.CommitChanges();
            DolefilTrip thisTrip = _ObjectSpace.GetObjectByKey<DolefilTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<DriverRegistry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.DocumentNo;
            _Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            _Obj.Driver = thisTrip.TruckNo != null ? thisTrip.TruckNo.Operator 
            != null ? thisTrip.TruckNo.Operator : null : null;
            _Obj.Tariff = thisTrip.Tariff != null ? thisTrip.Tariff : null;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "AddChangeDriver_Detail", true, _Obj);
        }
        private void AddChangeDriverDole_Execute(object sender, 
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
