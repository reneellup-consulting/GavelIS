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
    public partial class RevolvingDettachFromController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private RevolvingPart _RevPart;
        private RevolvingPartDetail _Obj;
        private PopupWindowShowAction revolvingDettachFromAction;
        public RevolvingDettachFromController()
        {
            this.TargetObjectType = typeof(RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = "RevolvingPart.DettachFrom";
            this.revolvingDettachFromAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.revolvingDettachFromAction.Caption = "Dettach From";
            this.revolvingDettachFromAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(revolvingDettachFromAction_CustomizePopupWindowParams);
            this.revolvingDettachFromAction.Execute += new PopupWindowShowActionExecuteEventHandler(revolvingDettachFromAction_Execute);
        }
        void revolvingDettachFromAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void revolvingDettachFromAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            if (thisRev.LastUnitAttachedTo != null && thisRev.LastActivityType == RevolvingPartActivityTypeEnum.Attach)
            {
                _Obj.ActivityDate = thisRev.LastActivityDate.AddDays(80);
                _Obj.Unit = thisRev.LastUnitAttachedTo;
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this part was not attached to a particular unit.");
            }
            _Obj.Status = RevolvingPartsStatusEnum.Dettached;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartDetail_DettachFrom", true, _Obj);
        }
    }
}
