using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class SmallToolsCheckOutController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private SmallToolsAndEquipment _SmallTool;
        private SmallToolsAndEquipmentDetail _Obj;
        private PopupWindowShowAction CheckOutAction;
        public SmallToolsCheckOutController()
        {
            this.TargetObjectType = typeof(SmallToolsAndEquipment);
            this.TargetViewType = ViewType.Any;
            string actionID = "SmallToolsAndEquipment.CheckOutAction";
            this.CheckOutAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.CheckOutAction.Caption = "Check-Out";
            this.CheckOutAction.TargetObjectsCriteria = "[AvailabilityStatus] In ('NO HISTORY','AVAILABLE')";
            this.CheckOutAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(CheckOutAction_CustomizePopupWindowParams);
            this.CheckOutAction.Execute += new PopupWindowShowActionExecuteEventHandler(CheckOutAction_Execute);
        }
        void CheckOutAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }

        void CheckOutAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
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
            //if (thisTool.AvailabilityStatus != "AVAILABLE")
            //{
            //    throw new UserFriendlyException("Cannot checkout unavailable tool.");
            //}
            if (!new[] { "NO HISTORY", "AVAILABLE" }.Any(o => thisTool.AvailabilityStatus.Contains(o)))
            {
                throw new UserFriendlyException("Cannot checkout unavailable tool.");
            }
            // Reserve code here...
            _Obj.ActivityType = SmallToolsAndEquipmentActivityTypeEnum.Loaned;
            SmallToolsAndEquipmentDetail oVar1 = null;
            if (thisTool.FirstValidReserve != null)
            {
                if (XtraMessageBox.Show("This tool was currently on reserved. \nIf to be checked-out by other than the one reserved, cancel prior reserves first. \nDo you want to proceed?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.No)
                {
                    throw new UserFriendlyException("Tool check-out has been cancelled");
                }
                SmallToolsAndEquipmentDetail oLast = _ObjectSpace.GetObject(thisTool.LastDetail);
                oVar1 = _ObjectSpace.GetObject(thisTool.FirstValidReserve);
                _Obj.Department = oVar1.Department ?? null;
                _Obj.LoanedTo = oVar1.LoanedTo ?? null;
                _Obj.Condition = oLast.Condition ?? null;
                _Obj.WasReserved = true;
                _Obj.ReserveDetail = oVar1;
                oVar1.ReserveDone = true;
                e.View = Application.CreateDetailView(_ObjectSpace,
                "SmallToolsAndEquipmentDetail_CheckoutReserve", true, _Obj);
            }
            else
            {
                oVar1 = _ObjectSpace.GetObject(thisTool.LastDetail);
                _Obj.Department = oVar1.Department ?? null;
                _Obj.LoanedTo = oVar1.LoanedTo ?? null;
                _Obj.Condition = oVar1.Condition ?? null;
                e.View = Application.CreateDetailView(_ObjectSpace,
                "SmallToolsAndEquipmentDetail_Checkout", true, _Obj);
            }
        }
    }
}
