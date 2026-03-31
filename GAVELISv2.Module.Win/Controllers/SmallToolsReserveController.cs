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
    public partial class SmallToolsReserveController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private SmallToolsAndEquipment _SmallTool;
        private SmallToolsAndEquipmentDetail _Obj;
        private PopupWindowShowAction ReserveAction;
        public SmallToolsReserveController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.Any;
            string actionID = "SmallToolsAndEquipment.ReserveAction";
            this.ReserveAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.ReserveAction.TargetObjectsCriteria = "[AvailabilityStatus] In ('CURRENTLY CHECKEDOUT','UNDER REPAIR','LOST')";
            this.ReserveAction.Caption = "Reserve Tool";
            this.ReserveAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(ReserveAction_CustomizePopupWindowParams);
            this.ReserveAction.Execute += new PopupWindowShowActionExecuteEventHandler(ReserveAction_Execute);
        }
        void ReserveAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }
        void ReserveAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                _SmallTool = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as SmallToolsAndEquipment;
            }
            else
            {
                _SmallTool = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as SmallToolsAndEquipment;
            }

            ObjectSpace.CommitChanges();
            SmallToolsAndEquipment thisTool = _ObjectSpace.GetObjectByKey<SmallToolsAndEquipment>(
            _SmallTool.Oid);
            _Obj = _ObjectSpace.CreateObject<SmallToolsAndEquipmentDetail>();
            _Obj.ParentId = thisTool;
            _Obj.EntryDate = DateTime.Now;
            if (thisTool.LastDetail == null)
            {
                throw new UserFriendlyException("Cannot continue because this tool was not initialized.");
            }
            if (thisTool.LastDetail != null && thisTool.LastDetail.ActivityDate != DateTime.MinValue)
            {
                _Obj.ActivityDate = thisTool.LastDetail.ActivityDate;
            }
            else
            {
                _Obj.ActivityDate = DateTime.Now;
            }
            _Obj.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Reserve;
            SmallToolsAndEquipmentDetail oVar = _ObjectSpace.GetObject(thisTool.LastDetail);
            _Obj.Condition = oVar.Condition ?? null;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "SmallToolsAndEquipmentDetail_Reserve", true, _Obj);
        }
    }
}
