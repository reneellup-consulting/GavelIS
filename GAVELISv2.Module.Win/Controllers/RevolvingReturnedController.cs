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
    public partial class RevolvingReturnedController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private RevolvingPart _RevPart;
        private RevolvingPartDetail _Obj;
        private PopupWindowShowAction revolvingReturnFromAction;
        public RevolvingReturnedController()
        {
            this.TargetObjectType = typeof(RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = "RevolvingPart.Return";
            this.revolvingReturnFromAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.revolvingReturnFromAction.Caption = "Return";
            this.revolvingReturnFromAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(revolvingReturnFromAction_CustomizePopupWindowParams);
            this.revolvingReturnFromAction.Execute += new PopupWindowShowActionExecuteEventHandler(revolvingReturnFromAction_Execute);
        }
        void revolvingReturnFromAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void revolvingReturnFromAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            _Obj = _ObjectSpace.CreateObject<RevolvingPartDetail>();
            _Obj.RevolvingPartId = thisRev;
            _Obj.EntryDate = DateTime.Now;
            _Obj.ActivityDate = DateTime.Now;
            _Obj.ActivityType = RevolvingPartActivityTypeEnum.Dettach;
            // Unit => Last Attached Unit.. If attached unit cannot be found and last activity is not attach to then throw an exception
            if (thisRev.LastUnitAttachedTo != null && thisRev.LastActivityType == RevolvingPartActivityTypeEnum.Dettach)
            {
                _Obj.ActivityDate = thisRev.LastActivityDate.AddDays(20);
                _Obj.Unit = thisRev.LastUnitAttachedTo;
                _Obj.Reason = _ObjectSpace.FindObject<RevolvingPartsActivityReason>(BinaryOperator.Parse("[Code] = ?", "RETURNED")) ?? null;
                _Obj.Remarks = "Returned from repair";
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this part was not attached to a particular unit.");
            }
            _Obj.Status = RevolvingPartsStatusEnum.Available;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartDetail_Return", true, _Obj);
        }
    }
}
