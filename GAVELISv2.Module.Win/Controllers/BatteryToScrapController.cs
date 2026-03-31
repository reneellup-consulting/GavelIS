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
    public partial class BatteryToScrapController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private Battery _Battery;
        private BatteryServiceDetail _Obj;
        private PopupWindowShowAction batteryToScrapAction;
        public BatteryToScrapController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = "Battery.ToScrap";
            this.batteryToScrapAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.batteryToScrapAction.Caption = "For Disposal";
            this.batteryToScrapAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(batteryToScrapAction_CustomizePopupWindowParams);
            this.batteryToScrapAction.Execute += new PopupWindowShowActionExecuteEventHandler(batteryToScrapAction_Execute);
        }

        void batteryToScrapAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            Item battItem = _ObjectSpace2.FindObject<Item>(BinaryOperator.Parse("[Description] = ?", "Scrapped Battery"));
            if (battItem == null)
            {
                throw new UserFriendlyException("Battery item does not exist.");
            }
            PhysicalAdjustment phys;
            if (_Obj.AdjustmentDoc == null)
            {
                phys = _ObjectSpace2.CreateObject<PhysicalAdjustment>();
                phys.EntryDate = DateTime.Now;
                phys.WarehouseLocation = battItem.WarehouseLocation;
            }
            else
            {
                phys = _ObjectSpace2.GetObject<PhysicalAdjustment>(
                _Obj.AdjustmentDoc);
            }
            PhysicalAdjustmentDetail createObject = _ObjectSpace2.CreateObject<PhysicalAdjustmentDetail>();
            phys.PhysicalAdjustmentDetails.Add(createObject);
            createObject.ItemNo = battItem;
            createObject.ActualQtyStock = 1m;
            createObject.BatteryRef = _Obj.BatteryNo.BatteryName;
            createObject.Warehouse = battItem.WarehouseLocation;
            createObject.Save();
            StringBuilder sb = new StringBuilder();
            if (phys.PhysicalAdjustmentDetails.Count > 0)
            {
                sb.Append("Scrapping battery #s ");
            }
            foreach (var item in phys.PhysicalAdjustmentDetails)
            {
                sb.AppendFormat("{0}, ", item.BatteryRef);
            }
            sb.Remove(sb.Length - 2, 2);
            phys.Memo = sb.ToString();
            phys.Save();
            _ObjectSpace2.CommitChanges();
            _Obj.AdjustmentDoc = _ObjectSpace.GetObject(phys);
            _Obj.Save();
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
            if (_Obj.OpenAdjustmentDoc)
            {
                DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            phys, true);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = viewWO;
            }
        }

        void batteryToScrapAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            if (thisBatt.LastDetail != null && thisBatt.LastDetail.Reason != null && thisBatt.LastDetail.Reason.Code == "DISPOSAL")
            {
                throw new UserFriendlyException("Cannot continue because this battery has already been disposed.");
            }
            if (thisBatt.LastUnitAttachedTo != null)
            {
                _Obj = _ObjectSpace.CreateObject<BatteryServiceDetail>();
                _Obj.BatteryNo = thisBatt;
                _Obj.EntryDate = DateTime.Now;
                _Obj.ActivityDate = thisBatt.LastActivityDate;
                _Obj.ActivityType = BatteryActivityTypeEnum.Dettach;
                _Obj.Unit = thisBatt.LastUnitAttachedTo;
                _Obj.Status = BatteryStatusEnum.Dettached;
                _Obj.Reason = _ObjectSpace.FindObject<BatteryDettachReason>(BinaryOperator.Parse("[Code]=?", "DISPOSAL"));
                _Obj.Save();
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this battery has never been in service.");
            }
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatteryServiceDetail_ToScrap", true, _Obj);
        }
    }
}
