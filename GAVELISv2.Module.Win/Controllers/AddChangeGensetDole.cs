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
    public partial class AddChangeGensetDole : ViewController {
        private IObjectSpace _ObjectSpace;
        private DolefilTrip _Trip;
        private GensetEntry _Obj;
        private PopupWindowShowAction addChangeGenset;
        public AddChangeGensetDole() {
            this.TargetObjectType = typeof(DolefilTrip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AddChangeGenset", this.GetType(
            ).Name);
            this.addChangeGenset = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.addChangeGenset.Caption = "Add/Change Genset";
            this.addChangeGenset.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            AddChangeGensetDole_CustomizePopupWindowParams);
            this.addChangeGenset.Execute += new 
            PopupWindowShowActionExecuteEventHandler(AddChangeGensetDole_Execute
            );
        }
        private void AddChangeGensetDole_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _Trip = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as DolefilTrip;
            ObjectSpace.CommitChanges();
            DolefilTrip thisTrip = _ObjectSpace.GetObjectByKey<DolefilTrip>(
            _Trip.Oid);
            _Obj = _ObjectSpace.CreateObject<GensetEntry>();
            _Obj.TripID = thisTrip;
            _Obj.TripNo = thisTrip.SourceNo;
            _Obj.EntryDate = thisTrip.EntryDate;
            //_Obj.ReferenceNo = thisTrip.DocumentNo;
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
        private void AddChangeGensetDole_Execute(object sender, 
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
