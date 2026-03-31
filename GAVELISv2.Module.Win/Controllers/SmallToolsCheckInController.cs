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
    public partial class SmallToolsCheckInController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private SmallToolsAndEquipment _SmallTool;
        private SmallToolsAndEquipmentDetail _Obj;
        private PopupWindowShowAction CheckInAction;
        public SmallToolsCheckInController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.Any;
            string actionID = "SmallToolsAndEquipment.CheckInAction";
            this.CheckInAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.CheckInAction.Caption = "Return";
            this.CheckInAction.TargetObjectsCriteria = "[AvailabilityStatus] In ('CURRENTLY CHECKEDOUT','UNDER REPAIR','LOST','FOR DISPOSAL')";
            this.CheckInAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(CheckInAction_CustomizePopupWindowParams);
            this.CheckInAction.Execute += new PopupWindowShowActionExecuteEventHandler(CheckInAction_Execute);
        }
        void CheckInAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void CheckInAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            if (thisTool.LastDetail != null && !new[] { "Loaned", "Sent", "Reserve", "Dispose" }.Any(o => thisTool.LastDetail.ActivityType.ToString().Contains(o)))
            {
                throw new UserFriendlyException("Cannot return tool that is not currently checked-out or was sent for repair.");
            }
            _Obj.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Returned;
            SmallToolsAndEquipmentDetail oVar = _ObjectSpace.GetObject(thisTool.LastDetail);
            _Obj.Department = oVar.Department??null;
            _Obj.LoanedTo = oVar.LoanedTo??null;
            _Obj.Condition = oVar.Condition ?? null;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "SmallToolsAndEquipmentDetail_Return", true, _Obj);
        }
    }
}
