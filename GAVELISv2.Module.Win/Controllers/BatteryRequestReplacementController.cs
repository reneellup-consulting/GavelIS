using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class BatteryRequestReplacementController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private Battery _Battery;
        private BatteryRequest _Obj;
        private PopupWindowShowAction batteryRequestReplacementAction;
        public BatteryRequestReplacementController()
        {
            this.TargetObjectType = typeof(Battery);
            this.TargetViewType = ViewType.Any;
            string actionID = "Battery.RequestReplacement";
            this.batteryRequestReplacementAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.batteryRequestReplacementAction.Caption = "Request New";
            this.batteryRequestReplacementAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(batteryRequestReplacementAction_CustomizePopupWindowParams);
            this.batteryRequestReplacementAction.Execute += new PopupWindowShowActionExecuteEventHandler(batteryRequestReplacementAction_Execute);
        }

        void batteryRequestReplacementAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            Requisition reqObj;
            if (_Obj.RequestDoc == null)
            {
                reqObj = _ObjectSpace2.CreateObject<Requisition>();
                reqObj.EntryDate = DateTime.Now;
            }
            else
            {
                reqObj = _ObjectSpace2.GetObject<Requisition>(
                _Obj.RequestDoc);
            }
            Battery batt2 = _ObjectSpace2.GetObject(_Battery);
            if (_Obj.RequestDoc == null)
            {
                if (_Obj.RequestedBy!=null)
                {
                    Employee requestor = _ObjectSpace2.GetObject(_Obj.RequestedBy);
                    reqObj.RequestedBy = requestor ?? null;
                }
                reqObj.ExpectedDate = _Obj.ExpectedDate;
                if (_Obj.ChargeTo != null)
                {
                    CostCenter center = _ObjectSpace2.GetObject(_Obj.ChargeTo);
                    reqObj.CostCenter = center ?? null;
                }
            }
            RequisitionWorksheet reqws = _ObjectSpace2.CreateObject<RequisitionWorksheet>();
            reqws.ItemNo = _ObjectSpace2.GetObject(_Obj.BatteryItem);
            reqws.Quantity = 1m;
            reqws.ExpectedDate = _Obj.ExpectedDate;
            reqws.Reason = _Obj.Remarks;
            reqObj.RequisitionWorksheetLines.Add(reqws);
            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            reqObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }

        void batteryRequestReplacementAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            if (this.View.GetType() == typeof(ListView))
            {
                _Battery = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as Battery;
            }
            else
            {
                _Battery = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as Battery;
            }

            ObjectSpace.CommitChanges();
            _Obj = new BatteryRequest();
            _Obj.DateDeclared = DateTime.Now;
            _Obj.BatteryToReplace = _ObjectSpace.GetObject(_Battery);
            _Obj.BatteryItem = _Obj.BatteryToReplace.BatteryItem;
            _Obj.ChargeTo = _ObjectSpace.FindObject<CostCenter>(BinaryOperator.Parse("[Code]=?",_Battery.LastUnitAttachedTo.No));
            _Obj.Remarks = string.Format("Replacement for Battery #{0}", _Battery.BatteryName);
            e.View = Application.CreateDetailView(_ObjectSpace,
            "BatteryRequest_DetailView", true, _Obj);
        }
    }
}
