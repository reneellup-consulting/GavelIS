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
    public partial class AddChangeTruckOther : ViewController {
        private IObjectSpace _ObjectSpace;
        private OtherTrip _Trip;
        private TruckRegistry _Obj;
        private PopupWindowShowAction addChangeTruck;
        public AddChangeTruckOther() {
            this.TargetObjectType = typeof(OtherTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeTruck", this.GetType()
            .Name);
            this.addChangeTruck = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeTruck.Caption = "Add/Change Truck";
            this.addChangeTruck.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeTruckOther_CustomizePopupWindowParams);
            this.addChangeTruck.Execute += new 
            PopupWindowShowActionExecuteEventHandler(AddChangeTruckOther_Execute
            );
        }
        private void AddChangeTruckOther_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as OtherTrip;
            ObjectSpace.CommitChanges();
            OtherTrip thisTrip = _ObjectSpace.GetObjectByKey<OtherTrip>(_Trip.
            Oid);
            _Obj = _ObjectSpace.CreateObject<TruckRegistry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.TripNo;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "AddChangeTruck_Detail", true, _Obj);
        }
        private void AddChangeTruckOther_Execute(object sender, 
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
