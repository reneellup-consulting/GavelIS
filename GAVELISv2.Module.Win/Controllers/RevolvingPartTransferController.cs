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
    public partial class RevolvingPartTransferController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private RevolvingPart _RevPart;
        private RevolvingPartDetail _Obj;
        private RevolvingPartDetail _Obj2;
        private PopupWindowShowAction revolvingPartTransferToAction;
        public RevolvingPartTransferController()
        {
            this.TargetObjectType = typeof(RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = "RevolvingPart.TransferTo";
            this.revolvingPartTransferToAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.revolvingPartTransferToAction.Caption = "Transfer To";
            this.revolvingPartTransferToAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(revolvingPartTransferToAction_CustomizePopupWindowParams);
            this.revolvingPartTransferToAction.Execute += new PopupWindowShowActionExecuteEventHandler(revolvingPartTransferToAction_Execute);
        }
        void revolvingPartTransferToAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
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
        void revolvingPartTransferToAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                _RevPart = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as RevolvingPart;
            }
            else
            {
                _RevPart = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as RevolvingPart;
            }

            ObjectSpace.CommitChanges();
            RevolvingPart thisRev = _ObjectSpace.GetObjectByKey<RevolvingPart>(
            _RevPart.Oid);
            // Dettach From Unit
            // Battery must be currently Attached -> if not throw exception
            _Obj = _ObjectSpace.CreateObject<RevolvingPartDetail>();
            _Obj.RevolvingPartId = thisRev;
            _Obj.EntryDate = DateTime.Now;
            _Obj.ActivityDate = DateTime.Now;
            _Obj.ActivityType = RevolvingPartActivityTypeEnum.Dettach;
            // Unit => Last Attached Unit.. If attached unit cannot be found and last activity is not attach to then throw an exception
            if (thisRev.LastUnitAttachedTo != null && thisRev.LastActivityType == RevolvingPartActivityTypeEnum.Attach)
            {
                //_Obj.ActivityDate = thisBatt.LastActivityDate.AddDays(80);
                _Obj.Unit = thisRev.LastUnitAttachedTo;
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this part was not attached to a particular unit.");
            }
            _Obj.Status = RevolvingPartsStatusEnum.Dettached;
            _Obj.Location = _ObjectSpace.FindObject<Warehouse>(BinaryOperator.Parse("[Code]=?", "99-ATTACHED"));
            _Obj.Reason = _ObjectSpace.FindObject<RevolvingPartsActivityReason>(BinaryOperator.Parse("[Code]=?", "TRANSFER"));
            _Obj.Save();
            // Attach To Unit
            _Obj2 = _ObjectSpace.CreateObject<RevolvingPartDetail>();
            _Obj2.TransferNotice = string.Format("Transfer from {0}", _Obj.Unit.Code);
            _Obj2.RevolvingPartId = thisRev;
            _Obj2.EntryDate = DateTime.Now;
            if (thisRev.LastActivityDate != DateTime.MinValue)
            {
                _Obj2.ActivityDate = thisRev.LastActivityDate;
            }
            else
            {
                _Obj2.ActivityDate = DateTime.Now;
            }
            _Obj2.ActivityType = RevolvingPartActivityTypeEnum.Attach;
            //if (thisBatt.LastActivityType == BatteryActivityTypeEnum.Attach)
            //{
            //    throw new UserFriendlyException("Cannot continue because the battery is currently attached to a unit.");
            //}
            _Obj2.Status = RevolvingPartsStatusEnum.Attached;
            _Obj2.Reason = _Obj.Reason;
            _Obj2.Remarks = string.Format("Transfer from {0}", _Obj.Unit.Code);
            //_Obj2.Save();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartDetail_TransferTo", true, _Obj2);
        }
    }
}
