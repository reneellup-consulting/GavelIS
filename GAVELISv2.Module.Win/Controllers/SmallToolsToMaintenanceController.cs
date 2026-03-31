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
    public partial class SmallToolsToMaintenanceController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private SmallToolsAndEquipment _SmallTool;
        private SmallToolsAndEquipmentDetail _Obj;
        private PopupWindowShowAction toolToMaintenanceAction;
        public SmallToolsToMaintenanceController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.Any;
            string actionID = "SmallToolsAndEquipment.ToMaintenance";
            this.toolToMaintenanceAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.toolToMaintenanceAction.TargetObjectsCriteria = "[AvailabilityStatus] In ('NO HISTORY','AVAILABLE')";
            this.toolToMaintenanceAction.Caption = "To Maintenance";
            this.toolToMaintenanceAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(toolToMaintenanceAction_CustomizePopupWindowParams);
            this.toolToMaintenanceAction.Execute += new PopupWindowShowActionExecuteEventHandler(toolToMaintenanceAction_Execute);
        }
        void toolToMaintenanceAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
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
                    //jobs.Vendor = _ObjectSpace2.GetObject(_Obj.Location.JobsVendor) ?? null;
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
                DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
                jobs, true);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = viewWO;
                //if (_Obj.OpenAdjustmentDoc)
                //{
                //    DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
                //jobs, true);
                //    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                //    e.ShowViewParameters.CreatedView = viewWO;
                //}
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
                DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
                work, true);
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.CreatedView = viewWO;
                //if (_Obj.OpenAdjustmentDoc)
                //{
                //    DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
                //work, true);
                //    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                //    e.ShowViewParameters.CreatedView = viewWO;
                //}
            }
        }

        void toolToMaintenanceAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            SmallToolsAndEquipment thisBatt = _ObjectSpace.GetObjectByKey<SmallToolsAndEquipment>(
            _SmallTool.Oid);
            if (!new[] { "NO HISTORY", "AVAILABLE" }.Any(o => thisBatt.AvailabilityStatus.Contains(o)))
            {
                throw new UserFriendlyException("Cannot continue because this tool is currently checked-out");
            }
            _Obj = _ObjectSpace.CreateObject<SmallToolsAndEquipmentDetail>();
            _Obj.ParentId = thisBatt;
            _Obj.EntryDate = DateTime.Now;
            if (thisBatt.LastDetail == null)
            {
                throw new UserFriendlyException("Cannot continue because this tool was not initialized.");
            }
            _Obj.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Sent;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "SmallToolsAndEquipmentDetail_ToMaintenance", true, _Obj);
        }
    }
}
