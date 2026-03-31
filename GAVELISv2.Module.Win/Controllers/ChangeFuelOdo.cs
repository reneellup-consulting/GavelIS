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
    public partial class ChangeFuelOdo : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private FixedAsset _FAsset;
        private FuelMileageRegister _Obj;
        private PopupWindowShowAction changeFuelOdo;
        public ChangeFuelOdo()
        {
            this.TargetObjectType = typeof(FixedAsset);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("ChangeFuelOdo");
            this.changeFuelOdo = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.changeFuelOdo.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            ChangeFuelOdo_CustomizePopupWindowParams);
            this.changeFuelOdo.Execute += new 
            PopupWindowShowActionExecuteEventHandler(ChangeFuelOdo_Execute);
        }
        private void ChangeFuelOdo_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            //_ObjectSpace = Application.CreateObjectSpace();
            //_FAsset = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            //as StanfilcoTrip;
            //ObjectSpace.CommitChanges();
            //StanfilcoTrip thisTrip = _ObjectSpace.GetObjectByKey<StanfilcoTrip>(
            //_FAsset.Oid);
            //_Obj = _ObjectSpace.CreateObject<DriverRegistry>();
            //_Obj.TripID = thisTrip;
            //_Obj.TripNo = thisTrip.SourceNo;
            //_Obj.Date = thisTrip.EntryDate;
            //_Obj.ReferenceNo = thisTrip.DTRNo;
            //_Obj.TruckNo = thisTrip.TruckNo != null ? thisTrip.TruckNo : null;
            //_Obj.Driver = thisTrip.TruckNo != null ? thisTrip.TruckNo.Operator 
            //!= null ? thisTrip.TruckNo.Operator : null : null;
            //_Obj.Tariff = thisTrip.Tariff != null ? thisTrip.Tariff : null;
            ////objectSpace.CommitChanges();
            //e.View = Application.CreateDetailView(_ObjectSpace, 
            //"AddChangeDriver_Detail", true, _Obj);
        }
        private void ChangeFuelOdo_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_FAsset);
            ObjectSpace.Refresh();
        }
    }
}
