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
    public partial class RevolvingPartToMaintenanceController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private RevolvingPart _RevolvingPart;
        private RevolvingPartDetail _Obj;
        private PopupWindowShowAction revolvingPartToScrapAction;
        public RevolvingPartToMaintenanceController()
        {
            this.TargetObjectType = typeof(RevolvingPart);
            this.TargetViewType = ViewType.Any;
            string actionID = "RevolvingPart.ForDisposal";
            this.revolvingPartToScrapAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.revolvingPartToScrapAction.Caption = "To Maintenance";
            this.revolvingPartToScrapAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(revolvingPartToScrapAction_CustomizePopupWindowParams);
            this.revolvingPartToScrapAction.Execute += new PopupWindowShowActionExecuteEventHandler(revolvingPartToScrapAction_Execute);
        }
        void revolvingPartToScrapAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            //Item battItem = _ObjectSpace2.FindObject<Item>(BinaryOperator.Parse("[Description] = ?", "Scrapped RevolvingPart"));
            //if (battItem == null)
            //{
            //    throw new UserFriendlyException("RevolvingPart item does not exist.");
            //}
            if (_Obj.CreateJobOrder)
            {
                // Create Job Order
                JobOrder jobs;
                if (_Obj.JobOrderDoc == null)
                {
                    jobs = _ObjectSpace2.CreateObject<JobOrder>();
                    jobs.EntryDate = _Obj.ActivityDate;
                    jobs.Vendor = _ObjectSpace2.GetObject(_Obj.Location.JobsVendor) ?? null;
                }
                else
                {
                    jobs = _ObjectSpace2.GetObject<JobOrder>(
                    _Obj.JobOrderDoc);
                }
                jobs.Save();
                _ObjectSpace2.CommitChanges();
                _Obj.JobOrderDoc = _ObjectSpace.GetObject(jobs);
                _Obj.Save();
                _ObjectSpace.CommitChanges();
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
                if (_Obj.OpenAdjustmentDoc)
                {
                    DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
                jobs, true);
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    e.ShowViewParameters.CreatedView = viewWO;
                }
            }
            else
            {
                // Create Work Order
                WorkOrder work;
                if (_Obj.WorkOrderDoc == null)
                {
                    work = _ObjectSpace2.CreateObject<WorkOrder>();
                    work.EntryDate = _Obj.ActivityDate;
                }
                else
                {
                    work = _ObjectSpace2.GetObject<WorkOrder>(
                    _Obj.WorkOrderDoc);
                }
                work.Save();
                _ObjectSpace2.CommitChanges();
                _Obj.WorkOrderDoc = _ObjectSpace.GetObject(work);
                _Obj.Save();
                _ObjectSpace.CommitChanges();
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
                if (_Obj.OpenAdjustmentDoc)
                {
                    DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
                work, true);
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    e.ShowViewParameters.CreatedView = viewWO;
                }
            }
        }

        void revolvingPartToScrapAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                _RevolvingPart = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as RevolvingPart;
            }
            else
            {
                _RevolvingPart = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as RevolvingPart;
            }

            ObjectSpace.CommitChanges();
            RevolvingPart thisBatt = _ObjectSpace.GetObjectByKey<RevolvingPart>(
            _RevolvingPart.Oid);
            if (thisBatt.LastDetail != null && thisBatt.LastDetail.Reason != null && thisBatt.LastDetail.Reason.Code == "REPAIR")
            {
                throw new UserFriendlyException("Cannot continue because this revolvingPart has already been disposed.");
            }
            if (thisBatt.LastUnitAttachedTo != null)
            {
                _Obj = _ObjectSpace.CreateObject<RevolvingPartDetail>();
                _Obj.RevolvingPartId = thisBatt;
                _Obj.EntryDate = DateTime.Now;
                _Obj.ActivityDate = thisBatt.LastActivityDate;
                _Obj.ActivityType = RevolvingPartActivityTypeEnum.Dettach;
                _Obj.Unit = thisBatt.LastUnitAttachedTo;
                _Obj.Status = RevolvingPartsStatusEnum.Dettached;
                _Obj.Reason = _ObjectSpace.FindObject<RevolvingPartsActivityReason>(BinaryOperator.Parse("[Code]=?", "REPAIR"));
                _Obj.Save();
            }
            else
            {
                throw new UserFriendlyException("Cannot continue because this revolvingPart has never been in service.");
            }
            e.View = Application.CreateDetailView(_ObjectSpace,
            "RevolvingPartDetail_ToMaintenance", true, _Obj);
        }
    }
}
//"RevolvingPartDetail_ToMaintenance"