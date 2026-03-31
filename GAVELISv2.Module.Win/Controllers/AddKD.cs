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
    public partial class AddKD : ViewController {
        private IObjectSpace _ObjectSpace;
        private StanfilcoTrip _Trip;
        private KDEntry _Obj;
        private PopupWindowShowAction addKD;
        public AddKD() {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddKD", this.GetType().Name);
            this.addKD = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addKD.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddKD_CustomizePopupWindowParams);
            this.addKD.Execute += new PopupWindowShowActionExecuteEventHandler(
            AddKD_Execute);
        }
        private void AddKD_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as StanfilcoTrip;
            ObjectSpace.CommitChanges();
            StanfilcoTrip thisTrip = _ObjectSpace.GetObjectByKey<StanfilcoTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<KDEntry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.EntryDate = thisTrip.EntryDate;
            _Obj.ReferenceNo = thisTrip.DTRNo;
            _Obj.Customer = thisTrip.Customer;
            _Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            _Obj.Driver = thisTrip.Driver != null ? thisTrip.Driver : null;
            _Obj.TrailerNo = thisTrip.TrailerNo != null ? thisTrip.TrailerNo : 
            null;
            e.View = Application.CreateDetailView(_ObjectSpace, "AddKD_Detail", 
            true, _Obj);
        }
        private void AddKD_Execute(object sender, 
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
