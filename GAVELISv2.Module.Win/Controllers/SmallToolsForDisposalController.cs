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
    public partial class SmallToolsForDisposalController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private SmallToolsAndEquipment _SmallTool;
        private SmallToolsAndEquipmentDetail _Obj;
        private PopupWindowShowAction ForDisposalAction;
        public SmallToolsForDisposalController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.Any;
            string actionID = "SmallToolsAndEquipment.ForDisposalAction";
            this.ForDisposalAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.ForDisposalAction.TargetObjectsCriteria = "[AvailabilityStatus] In ('NO HISTORY','AVAILABLE')";
            this.ForDisposalAction.Caption = "For Disposal";
            this.ForDisposalAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(ForDisposalAction_CustomizePopupWindowParams);
            this.ForDisposalAction.Execute += new PopupWindowShowActionExecuteEventHandler(ForDisposalAction_Execute);
        }
        void ForDisposalAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void ForDisposalAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            //if (thisTool.LastDetail != null && thisTool.LastDetail.ActivityDate != DateTime.MinValue)
            //{
            //    _Obj.ActivityDate = thisTool.LastDetail.ActivityDate;
            //}
            //else
            //{
            //    _Obj.ActivityDate = DateTime.Now;
            //}
            if (!new[] { "NO HISTORY", "AVAILABLE" }.Any(o => thisTool.AvailabilityStatus.Contains(o)))
            {
                throw new UserFriendlyException("Cannot diposed unreturned tools.");
            }
            _Obj.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Dispose;
            SmallToolsAndEquipmentDetail oVar = _ObjectSpace.GetObject(thisTool.LastDetail);
            //_Obj.Department = oVar.Department ?? null;
            //_Obj.LoanedTo = oVar.LoanedTo ?? null;
            //_Obj.Condition = oVar.Condition ?? null;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "SmallToolsAndEquipmentDetail_Disposed", true, _Obj);
        }
    }
}
