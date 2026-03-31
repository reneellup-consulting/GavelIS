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
    public partial class AddChangeGenset : ViewController {
        private IObjectSpace _ObjectSpace;
        private StanfilcoTrip _Trip;
        private GensetEntry _Obj;
        private PopupWindowShowAction addChangeGenset;
        public AddChangeGenset() {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeGenset", this.GetType(
            ).Name);
            this.addChangeGenset = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeGenset.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeGenset_CustomizePopupWindowParams);
            this.addChangeGenset.Execute += new 
            PopupWindowShowActionExecuteEventHandler(AddChangeGenset_Execute);
        }
        private void AddChangeGenset_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as StanfilcoTrip;
            ObjectSpace.CommitChanges();
            StanfilcoTrip thisTrip = _ObjectSpace.GetObjectByKey<StanfilcoTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<GensetEntry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.EntryDate = thisTrip.EntryDate;
            //_Obj.ReferenceNo = thisTrip.DTRNo;
            _Obj.Customer = thisTrip.Customer;
            _Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            _Obj.Driver = thisTrip.Driver != null ? thisTrip.Driver : null;
            _Obj.TrailerNo = thisTrip.TrailerNo != null ? thisTrip.TrailerNo : 
            null;
            _Obj.GensetNo = thisTrip.TrailerNo != null ? thisTrip.TrailerNo.
            GensetNo != null ? thisTrip.TrailerNo.GensetNo : null : null;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "AddChangeGenset_Detail", true, _Obj);
        }
        private void AddChangeGenset_Execute(object sender, 
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
