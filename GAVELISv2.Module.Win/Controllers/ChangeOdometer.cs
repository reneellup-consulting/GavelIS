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
    public partial class ChangeOdometer : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private FixedAsset _FAsset;
        private OdometerRegister _Obj;
        private PopupWindowShowAction changeOdometer;
        public ChangeOdometer()
        {
            this.TargetObjectType = typeof(FixedAsset);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("ChangeOdometer");
            this.changeOdometer = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.changeOdometer.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            ChangeOdometer_CustomizePopupWindowParams);
            this.changeOdometer.Execute += new 
            PopupWindowShowActionExecuteEventHandler(ChangeOdometer_Execute);
        }
        private void ChangeOdometer_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _FAsset = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as FixedAsset;
            ObjectSpace.CommitChanges();
            FixedAsset thisFA = _ObjectSpace.GetObjectByKey<FixedAsset>(
            _FAsset.Oid);
            _Obj = _ObjectSpace.CreateObject<OdometerRegister>();
            _Obj.Fleet = thisFA;
            _Obj.EntryDate = DateTime.Now;
            _Obj.MeterType = MeterEntryTypeEnum.Odometer;
            _Obj.LogType = MeterLogTypeEnum.Change;
            _Obj.PreviousReading = _FAsset.LastReading;
            _Obj.Reading = 0m;
            _Obj.Life = thisFA.Mileage02;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "ChangeOdometer_Detail", true, _Obj);
        }
        private void ChangeOdometer_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_FAsset);
            ObjectSpace.Refresh();
        }
    }
}
