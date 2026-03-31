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
    public partial class BatteryTransferToController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Battery _Battery;
        private BatteryServiceDetail _Obj;
        private BatteryServiceDetail _Obj2;
        private PopupWindowShowAction batteryTransferToAction;
        public BatteryTransferToController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = "Battery.TransferTo";
            this.batteryTransferToAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.batteryTransferToAction.Caption = "Transfer To";
            this.batteryTransferToAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(batteryTransferToAction_CustomizePopupWindowParams);
            this.batteryTransferToAction.Execute += new PopupWindowShowActionExecuteEventHandler(batteryTransferToAction_Execute);
        }

        void batteryTransferToAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _Obj.ActivityDate = _Obj2.ActivityDate;
            _Obj.Condition = _Obj2.Condition;
            _Obj.RequestedBy = _Obj2.RequestedBy;
            _Obj.Remarks = string.Format("Transfer to {0}", _Obj2.Unit.Code); ;
            _Obj.Save();
            _Obj2.Save();
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void batteryTransferToAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            // Dettach From Unit
            // Battery must be currently Attached -> if not throw exception
            _Obj = _ObjectSpace.CreateObject<BatteryServiceDetail>();
            _Obj.BatteryNo = thisBatt;
            _Obj.EntryDate = DateTime.Now;
            _Obj.ActivityDate = DateTime.Now;
            _Obj.ActivityType = BatteryActivityTypeEnum.Dettach;
            // Unit => Last Attached Unit.. If attached unit cannot be found and last activity is not attach to then throw an exception
            if (thisBatt.LastUnitAttachedTo != null && thisBatt.LastActivityType == BatteryActivityTypeEnum.Attach)
            {
                //_Obj.ActivityDate = thisBatt.LastActivityDate.AddDays(80);
                _Obj.Unit = thisBatt.LastUnitAttachedTo;
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this battery was not attached to a particular unit.");
            }
            _Obj.Status = BatteryStatusEnum.Dettached;
            _Obj.Reason = _ObjectSpace.FindObject<BatteryDettachReason>(BinaryOperator.Parse("[Code]=?", "TRANSFER"));
            _Obj.Save();
            // Attach To Unit
            _Obj2 = _ObjectSpace.CreateObject<BatteryServiceDetail>();
            _Obj2.TransferNotice = string.Format("Transfer from {0}", _Obj.Unit.Code);
            _Obj2.BatteryNo = thisBatt;
            _Obj2.EntryDate = DateTime.Now;
            if (thisBatt.LastActivityDate != DateTime.MinValue)
            {
                _Obj2.ActivityDate = thisBatt.LastActivityDate;
            }
            else
            {
                _Obj2.ActivityDate = DateTime.Now;
            }
            _Obj2.ActivityType = BatteryActivityTypeEnum.Attach;
            //if (thisBatt.LastActivityType == BatteryActivityTypeEnum.Attach)
            //{
            //    throw new UserFriendlyException("Cannot continue because the battery is currently attached to a unit.");
            //}
            _Obj2.Status = BatteryStatusEnum.Attached;
            _Obj2.Reason = _Obj.Reason;
            _Obj2.Remarks = string.Format("Transfer from {0}", _Obj.Unit.Code);
            //_Obj2.Save();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatteryServiceDetail_TransferTo", true, _Obj2);
        }
    }
}
