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
    public partial class BatteryDettachFromController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Battery _Battery;
        private BatteryServiceDetail _Obj;
        private PopupWindowShowAction batteryDettachFromAction;
        public BatteryDettachFromController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = "Battery.DettachFrom";
            this.batteryDettachFromAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.batteryDettachFromAction.Caption = "Dettach From";
            this.batteryDettachFromAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(batteryDettachFromAction_CustomizePopupWindowParams);
            this.batteryDettachFromAction.Execute += new PopupWindowShowActionExecuteEventHandler(batteryDettachFromAction_Execute);
        }

        void batteryDettachFromAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void batteryDettachFromAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            _Obj = _ObjectSpace.CreateObject<BatteryServiceDetail>();
            _Obj.BatteryNo = thisBatt;
            _Obj.EntryDate = DateTime.Now;
            _Obj.ActivityDate = DateTime.Now;
            _Obj.ActivityType = BatteryActivityTypeEnum.Dettach;
            // Unit => Last Attached Unit.. If attached unit cannot be found and last activity is not attach to then throw an exception
            if (thisBatt.LastUnitAttachedTo != null && thisBatt.LastActivityType == BatteryActivityTypeEnum.Attach)
            {
                _Obj.ActivityDate = thisBatt.LastActivityDate.AddDays(80);
                _Obj.Unit = thisBatt.LastUnitAttachedTo;
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this battery was not attached to a particular unit.");
            }
            _Obj.Status = BatteryStatusEnum.Dettached;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatteryServiceDetail_DettachFrom", true, _Obj);
        }
    }
}
