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
    public partial class RevolvingAttachedToController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private RevolvingPart _RevPart;
        private RevolvingPartDetail _Obj;
        private PopupWindowShowAction revolvingAttachedToAction;
        public RevolvingAttachedToController()
        {
            this.TargetObjectType = typeof(RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = "RevolvingPart.AttachedTo";
            this.revolvingAttachedToAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.revolvingAttachedToAction.Caption = "Attach To";
            this.revolvingAttachedToAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(revolvingAttachedToAction_CustomizePopupWindowParams);
            this.revolvingAttachedToAction.Execute += new PopupWindowShowActionExecuteEventHandler(revolvingAttachedToAction_Execute);
        }
        void revolvingAttachedToAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void revolvingAttachedToAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            if (thisRev.LastActivityDate != DateTime.MinValue)
            {
                _Obj.ActivityDate = thisRev.LastActivityDate;
            }
            else
            {
                _Obj.ActivityDate = DateTime.Now;
            }
            _Obj.ActivityType = RevolvingPartActivityTypeEnum.Attach;
            if (thisRev.LastActivityType == RevolvingPartActivityTypeEnum.Attach)
            {
                throw new UserFriendlyException("Cannot continue because the part is currently attached to a unit.");
            }
            _Obj.Status = RevolvingPartsStatusEnum.Attached;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartDetail_AttachTo", true, _Obj);
        }
    }
}
