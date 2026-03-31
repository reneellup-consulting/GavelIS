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
    public partial class SmallToolsTransferController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private SmallToolsAndEquipment _SmallTool;
        private SmallToolsAndEquipmentDetail _Obj;
        private SmallToolsAndEquipmentDetail _Obj2;
        private PopupWindowShowAction TransferAction;
        public SmallToolsTransferController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.Any;
            string actionID = "SmallToolsAndEquipment.TransferAction";
            this.TransferAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.TransferAction.TargetObjectsCriteria = "[AvailabilityStatus] = 'CURRENTLY CHECKEDOUT'";
            this.TransferAction.Caption = "Transfer";
            this.TransferAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(TransferAction_CustomizePopupWindowParams);
            this.TransferAction.Execute += new PopupWindowShowActionExecuteEventHandler(TransferAction_Execute);
        }
        void TransferAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _Obj.ActivityDate = _Obj2.ActivityDate;
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void TransferAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            //_Obj.EntryDate = DateTime.Now;
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
            if (thisTool.LastDetail != null && !new[] { "Loaned" }.Any(o => thisTool.LastDetail.ActivityType.ToString().Contains(o)))
            {
                throw new UserFriendlyException("Cannot transfer tool that is not currently checked-out.");
            }
            _Obj.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Returned;
            SmallToolsAndEquipmentDetail oVar = _ObjectSpace.GetObject(thisTool.LastDetail);
            _Obj.Department = oVar.Department ?? null;
            _Obj.LoanedTo = oVar.LoanedTo ?? null;
            _Obj.Condition = oVar.Condition ?? null;
            //_Obj.Save();
            // Checkout
            _Obj2 = _ObjectSpace.CreateObject<SmallToolsAndEquipmentDetail>();
            _Obj2.ParentId = thisTool;
            _Obj2.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Loaned;
            _Obj2.TransferNotice = string.Format("Transfer from {0} by {1}",_Obj.Department.Code,_Obj.LoanedTo.Name);
            e.View = Application.CreateDetailView(_ObjectSpace,
            "SmallToolsAndEquipmentDetail_Transfer", true, _Obj2);
        }
    }
}
