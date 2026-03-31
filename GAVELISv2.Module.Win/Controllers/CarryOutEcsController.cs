using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using DevExpress.Data.Filtering;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CarryOutEcsController : WinDetailViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.RequisitionWorksheet _ReqWs;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction carryOutRequest;
        public CarryOutEcsController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.
            RequisitionWorksheet);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.CarryOutEcs", this.GetType().
            Name);
            this.carryOutRequest = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.carryOutRequest.Caption = "Carry Out to ECS";
            this.SuppressConfirmation = true;
            this.carryOutRequest.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            CarryOutRequest_CustomizePopupWindowParams);
            this.carryOutRequest.Execute += new
            PopupWindowShowActionExecuteEventHandler(CarryOutRequest_Execute);
        }
        private void CarryOutRequest_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReqWs = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.RequisitionWorksheet;
            if (_ReqWs.Cancelled)
            {
                throw new UserFriendlyException("Cannot carry out cancelled request");
            }
            if (_ReqWs.RequisitionInfo.Status != BusinessObjectsAlias.RequisitionStatusEnum.Approved)
            {
                throw new UserFriendlyException("One of the request selected was not approved");
            }
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.EmployeeChargeSlip;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CarryOutECS_Detail", true, _Obj);
        }
        private void CarryOutRequest_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.EmployeeChargeSlip ecsObj;
            if (_Obj.ECSlip == null)
            {
                ecsObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                EmployeeChargeSlip>();
                ecsObj.EntryDate = DateTime.Now;
            }
            else
            {
                ecsObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.EmployeeChargeSlip>(
                _Obj.ECSlip);
            }
            StringBuilder sbr = new StringBuilder();
            List<string> strRefs = new List<string>();
            // Init Employee Chargeslip here
            BusinessObjectsAlias.RequisitionWorksheet rw = _ObjectSpace2.GetObject<BusinessObjectsAlias.RequisitionWorksheet>(_ReqWs);
            //ecsObj.EntryDate = rw.RequisitionInfo.EntryDate;
            ecsObj.EntryDate = DateTime.Now;

            strRefs = ecsObj.Refs ?? new List<string>();
            if (!string.IsNullOrEmpty(ecsObj.ReferenceNo))
            {
                sbr.AppendFormat("{0},", ecsObj.ReferenceNo);
            }

            // Code for multiple reference no.
            if (!strRefs.Contains(rw.RequisitionInfo.SourceNo))
            {
                strRefs.Add(rw.RequisitionInfo.SourceNo);
                sbr.AppendFormat("{0},", rw.RequisitionInfo.SourceNo);
            }
            if (sbr.Length > 0)
            {
                sbr.Remove(sbr.Length - 1, 1);
            }
            ecsObj.Refs = strRefs;
            ecsObj.ReferenceNo = sbr.ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("ITEM REQUEST\r\n");
            sb.AppendFormat("{0}\r\n", rw.Reason);
            ecsObj.Memo = sb.ToString();
            ecsObj.Employee = rw.RequisitionInfo.RequestedBy;
            ecsObj.TypeOfCharge = BusinessObjectsAlias.TypeOfEmployeeChargesEnum.FuelChargeTractor;
            ecsObj.DeductionCode = _ObjectSpace2.FindObject<BusinessObjectsAlias.OtherDeduction>(CriteriaOperator.Parse("[Code] = '10-006'"));
            ecsObj.AdvancesAccount = _ObjectSpace2.FindObject<BusinessObjectsAlias.Account>(CriteriaOperator.Parse("[No] = '101915'"));

            BusinessObjectsAlias.EmployeeChargeSlipItemDetail ecsitm = _ObjectSpace2.CreateObject<BusinessObjectsAlias.EmployeeChargeSlipItemDetail>();
            ecsitm.GenJournalID = ecsObj;
            ecsitm.ItemNo = rw.ItemNo;
            ecsitm.CostCenter = _ObjectSpace2.FindObject<BusinessObjectsAlias.CostCenter>(CriteriaOperator.Parse("[Code] = ?", rw.RequisitionInfo.RequestedBy.No)) ?? null;
            ecsitm.Quantity = rw.Quantity;
            ecsitm.UOM = rw.UOM;
            ecsitm.Cost = rw.Cost;
            ecsitm.Price = rw.Cost;
            ecsitm.RequestID = rw.RowID;
            ecsitm.RequisitionNo = rw.RequisitionInfo != null ? rw.RequisitionInfo : null;
            ecsitm.Save();

            //rw.LastCarrySource = ecsObj;
            rw.Action = BusinessObjectsAlias.RequisitionActionsEnum.EmployeeChargeSlip;
            rw.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
            rw.Save();

            ecsObj.Save();

            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            ecsObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
