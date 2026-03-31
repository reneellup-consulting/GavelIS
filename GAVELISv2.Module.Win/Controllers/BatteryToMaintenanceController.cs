using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class BatteryToMaintenanceController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Battery _Battery;
        private BatteryServiceDetail _Obj;
        //private BatteryServiceDetail _Obj2;
        private PopupWindowShowAction batteryToMaintenanceAction;
        public BatteryToMaintenanceController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = "Battery.ToMaintenance";
            this.batteryToMaintenanceAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.batteryToMaintenanceAction.Caption = "To Maintenance";
            this.batteryToMaintenanceAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(batteryToMaintenanceAction_CustomizePopupWindowParams);
            this.batteryToMaintenanceAction.Execute += new PopupWindowShowActionExecuteEventHandler(batteryToMaintenanceAction_Execute);
        }

        void batteryToMaintenanceAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void batteryToMaintenanceAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                _Battery = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as Battery;
            }
            else
            {
                _Battery = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as Battery;
            }

            ObjectSpace.CommitChanges();
            Battery thisBatt = _ObjectSpace.GetObjectByKey<Battery>(
            _Battery.Oid);
            if (thisBatt.LastUnitAttachedTo != null)
            {
                _Obj = _ObjectSpace.CreateObject<BatteryServiceDetail>();
                _Obj.BatteryNo = thisBatt;
                _Obj.EntryDate = DateTime.Now;
                _Obj.ActivityDate = thisBatt.LastActivityDate;
                _Obj.ActivityType = BatteryActivityTypeEnum.Dettach;
                _Obj.Unit = thisBatt.LastUnitAttachedTo;
                _Obj.Status = BatteryStatusEnum.Dettached;
                _Obj.Reason = _ObjectSpace.FindObject<BatteryDettachReason>(BinaryOperator.Parse("[Code]=?", "MAINTENANCE"));
                _Obj.Save();
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this battery has never been in service.");
            }
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatteryServiceDetail_ToMaintenance", true, _Obj);
        }
    }
}
