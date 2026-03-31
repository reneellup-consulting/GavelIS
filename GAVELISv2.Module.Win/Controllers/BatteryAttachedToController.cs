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
    public partial class BatteryAttachedToController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Battery _Battery;
        private BatteryServiceDetail _Obj;
        private PopupWindowShowAction batteryAttachedToAction;
        public BatteryAttachedToController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = "Battery.AttachedTo";
            this.batteryAttachedToAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.batteryAttachedToAction.Caption = "Attach To";
            this.batteryAttachedToAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(batteryAttachedToAction_CustomizePopupWindowParams);
            this.batteryAttachedToAction.Execute += new PopupWindowShowActionExecuteEventHandler(batteryAttachedToAction_Execute);
        }

        void batteryAttachedToAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void batteryAttachedToAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView)){
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
            if (thisBatt.LastActivityDate != DateTime.MinValue)
            {
                _Obj.ActivityDate = thisBatt.LastActivityDate;
            }
            else
            {
                _Obj.ActivityDate = DateTime.Now;
            }
            _Obj.ActivityType = BatteryActivityTypeEnum.Attach;
            if (thisBatt.LastActivityType == BatteryActivityTypeEnum.Attach)
            {
                throw new UserFriendlyException("Cannot continue because the battery is currently attached to a unit.");
            }
            _Obj.Status = BatteryStatusEnum.Attached;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatteryServiceDetail_AttachTo", true, _Obj);
        }
    }
}
