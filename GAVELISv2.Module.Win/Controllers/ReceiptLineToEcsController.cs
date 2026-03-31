using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiptLineToEcsController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.ReceiptDetail _ReceiptLine;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction receiptLineToEcsAction;

        public ReceiptLineToEcsController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.ReceiptDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ReceiptLineToEcs", this.GetType().
            Name);
            this.receiptLineToEcsAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.receiptLineToEcsAction.Caption = "Carry Out to ECS";
            this.receiptLineToEcsAction.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            ReceiptLineToEcs_CustomizePopupWindowParams);
            this.receiptLineToEcsAction.Execute += new
            PopupWindowShowActionExecuteEventHandler(ReceiptLineToEcs_Execute);
        }

        private void ReceiptLineToEcs_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReceiptLine = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.ReceiptDetail;
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.EmployeeChargeSlip;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CarryOutECS_Detail", true, _Obj);
        }
        private void ReceiptLineToEcs_Execute(object sender,
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
            // Init Employee Chargeslip here
            BusinessObjectsAlias.ReceiptDetail rw = _ObjectSpace2.GetObject<BusinessObjectsAlias.ReceiptDetail>(_ReceiptLine);
            ecsObj.EntryDate = rw.ReceiptInfo.EntryDate;
            ecsObj.ReferenceNo = rw.ReceiptInfo.SourceNo;
            StringBuilder sb = new StringBuilder();
            sb.Append("ITEM REQUEST\r\n");
            //sb.AppendFormat("{0}\r\n", rw.Reason);
            ecsObj.Memo = sb.ToString();
            ecsObj.Employee = rw.RequestedBy ?? null;
            ecsObj.TypeOfCharge = BusinessObjectsAlias.TypeOfEmployeeChargesEnum.ItemRequest;
            ecsObj.DeductionCode = _ObjectSpace2.FindObject<BusinessObjectsAlias.OtherDeduction>(CriteriaOperator.Parse("[Code] = '10-006'"));
            ecsObj.AdvancesAccount = _ObjectSpace2.FindObject<BusinessObjectsAlias.Account>(CriteriaOperator.Parse("[No] = '101915'"));

            BusinessObjectsAlias.EmployeeChargeSlipItemDetail ecsitm = _ObjectSpace2.CreateObject<BusinessObjectsAlias.EmployeeChargeSlipItemDetail>();
            ecsitm.GenJournalID = ecsObj;
            ecsitm.ItemNo = rw.ItemNo;
            ecsitm.CostCenter = _ObjectSpace2.FindObject<BusinessObjectsAlias.CostCenter>(CriteriaOperator.Parse("[Code] = ?", rw.RequestedBy != null ? rw.RequestedBy.No : string.Empty)) ?? null;
            ecsitm.Quantity = rw.Quantity;
            ecsitm.UOM = rw.UOM;
            ecsitm.Cost = rw.Cost;
            ecsitm.Price = rw.Cost;
            //ecsitm.RequestID = rw.RowID;
            //ecsitm.RequisitionNo = rw.RequisitionInfo != null ? rw.RequisitionInfo : null;
            ecsitm.Save();

            ////rw.LastCarrySource = ecsObj;
            //rw.Action = BusinessObjectsAlias.RequisitionActionsEnum.EmployeeChargeSlip;
            //rw.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
            rw.Save();

            ecsObj.Save();

            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            ecsObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
