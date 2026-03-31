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
    public partial class AddChangeTruckDole : ViewController {
        private IObjectSpace _ObjectSpace;
        private DolefilTrip _Trip;
        private TruckRegistry _Obj;
        private PopupWindowShowAction addChangeTruck;
        public AddChangeTruckDole() {
            this.TargetObjectType = typeof(DolefilTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeTruck", this.GetType()
            .Name);
            this.addChangeTruck = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeTruck.Caption = "Add/Change Truck";
            this.addChangeTruck.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeTruckDole_CustomizePopupWindowParams);
            this.addChangeTruck.Execute += new 
            PopupWindowShowActionExecuteEventHandler(AddChangeTruck_Execute);
        }
        private void AddChangeTruckDole_CustomizePopupWindowParams(object sender
        , CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as DolefilTrip;
            ObjectSpace.CommitChanges();
            DolefilTrip thisTrip = _ObjectSpace.GetObjectByKey<DolefilTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<TruckRegistry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.DocumentNo;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "AddChangeTruck_Detail", true, _Obj);
        }
        private void AddChangeTruck_Execute(object sender, 
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
