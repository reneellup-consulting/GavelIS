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
    public partial class AddChangeTruck : ViewController {
        private IObjectSpace _ObjectSpace;
        private StanfilcoTrip _Trip;
        private TruckRegistry _Obj;
        private PopupWindowShowAction addChangeTruck;
        public AddChangeTruck() {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeTruck", this.GetType()
            .Name);
            this.addChangeTruck = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeTruck.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeTruck_CustomizePopupWindowParams);
            this.addChangeTruck.Execute += new 
            PopupWindowShowActionExecuteEventHandler(AddChangeTruck_Execute);
        }
        private void AddChangeTruck_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as StanfilcoTrip;
            ObjectSpace.CommitChanges();
            StanfilcoTrip thisTrip = _ObjectSpace.GetObjectByKey<StanfilcoTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<TruckRegistry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.Date = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.DTRNo;
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
