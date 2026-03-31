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
    public partial class LogOdometer : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private FixedAsset _FAsset;
        private OdometerRegister _Obj;
        private PopupWindowShowAction logOdometer;
        public LogOdometer()
        {
            this.TargetObjectType = typeof(FixedAsset);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("LogOdometer");
            this.logOdometer = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.logOdometer.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            LogOdometer_CustomizePopupWindowParams);
            this.logOdometer.Execute += new 
            PopupWindowShowActionExecuteEventHandler(LogOdometer_Execute);
        }
        private void LogOdometer_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _FAsset = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject 
            as FixedAsset;
            ObjectSpace.CommitChanges();
            FixedAsset thisFA = _ObjectSpace.GetObjectByKey<FixedAsset>(
            _FAsset.Oid);
            _Obj = _ObjectSpace.CreateObject<OdometerRegister>();
            _Obj.Fleet = thisFA;
            if (thisFA.GetType()==typeof(FATruck))
            {
                _Obj.ReportedBy = ((FATruck)thisFA).Operator!=null?((FATruck)thisFA).Operator:null;
            }
            _Obj.EntryDate = DateTime.Now;
            if (thisFA.VehicleOdoRegisters.Count>1)
            {
                _Obj.LogType = MeterLogTypeEnum.Log;
            }
            else
            {
                _Obj.LogType = MeterLogTypeEnum.Initial;
            }
            _Obj.MeterType = MeterEntryTypeEnum.Odometer;
            _Obj.PreviousReading = _FAsset.LastReading;
            _Obj.Reading = _FAsset.LastReading;
           e.View = Application.CreateDetailView(_ObjectSpace,
            "LogOdometer_Detail", true, _Obj);
        }
        private void LogOdometer_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(_FAsset);
            ObjectSpace.Refresh();
        }
    }
}
