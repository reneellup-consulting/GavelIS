using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReceiptLineToTOController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.ReceiptDetail _ReceiptLine;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction carryOutRequest;

        public ReceiptLineToTOController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.ReceiptDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.CarryOutTO", this.GetType().
            Name);
            this.carryOutRequest = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            //this.SuppressConfirmation = true;
            this.carryOutRequest.Caption = "Carryout To TO";
            this.carryOutRequest.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            CarryOutRequest_CustomizePopupWindowParams);
            this.carryOutRequest.Execute += new
            PopupWindowShowActionExecuteEventHandler(CarryOutRequest_Execute);
        }

        private void CarryOutRequest_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReceiptLine = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.ReceiptDetail;
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.TransferOrder;
            _Obj.FromWarehouse = _ObjectSpace.GetObject<BusinessObjectsAlias.Warehouse>(_ReceiptLine.Warehouse);
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CarryOutTO_Detail", true, _Obj);
        }

        private void CarryOutRequest_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.TransferOrder toObj;
            if (_Obj.TOrders == null)
            {
                toObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                TransferOrder>();
                toObj.EntryDate = DateTime.Now;
                toObj.FromWarehouse = _Obj.FromWarehouse != null ? _ObjectSpace2
                .GetObject<BusinessObjectsAlias.Warehouse>(_Obj.FromWarehouse) :
                null;
            }
            else
            {
                toObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.
                TransferOrder>(_Obj.TOrders);
            }
            var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (BusinessObjectsAlias.ReceiptDetail item in selwo)
            {
                BusinessObjectsAlias.RequisitionWorksheet rws = null;
                if (item.RequisitionNo != null && item.PODetailID != null && item.PODetailID.RequestID != Guid.Empty)
                {
                    rws = _ObjectSpace2.FindObject<BusinessObjectsAlias.RequisitionWorksheet>(BinaryOperator.Parse("[RowID] = ?", item.PODetailID.RequestID));
                }
                BusinessObjectsAlias.ReceiptDetail rcd = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.ReceiptDetail>(item);
                BusinessObjectsAlias.TransferOrderDetail toDetail =
                _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                TransferOrderDetail>();
                toDetail.GenJournalID = toObj;
                toDetail.ItemNo = rcd.ItemNo;
                toDetail.Description = rcd.Description;
                toDetail.Quantity = rcd.Quantity;
                toDetail.UOM = rcd.UOM;
                toDetail.Vendor = rcd.ReceiptInfo != null ? rcd.ReceiptInfo.Vendor : null;
                toDetail.Origin = rcd.ReceiptInfo != null ? rcd.ReceiptInfo.Vendor.Origin : null;
                toDetail.RequestID = rcd.PODetailID != null ? rcd.PODetailID.RequestID : Guid.Empty;
                toDetail.ExpenseType = rcd.ExpenseType ?? null;
                toDetail.SubExpenseType = rcd.SubExpenseType ?? null;
                toDetail.RequisitionNo = rcd.RequisitionNo != null ? rcd.RequisitionNo : null;
                toDetail.CostCenter = rcd.CostCenter != null ? rcd.CostCenter : null;
                if (rcd.RequisitionNo != null)
                {
                    toDetail.RequestedBy = rcd.RequisitionNo.RequestedBy != null ? rcd.RequisitionNo.RequestedBy : null;
                }
                toDetail.Facility = rcd.Facility ?? null;
                toDetail.FacilityHead = rcd.FacilityHead ?? null;
                toDetail.Department = rcd.Department ?? null;
                toDetail.DepartmentInCharge = rcd.DepartmentInCharge ?? null;
                toDetail.Save();
                if (rws != null)
                {
                    //rws.LastCarrySource = toObj;
                    rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.
                    WorkOrder;
                    rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                    rws.Save();
                }
                if (_Obj.TOrders == null)
                {
                    toObj.ReferenceNo = rcd.RequisitionNo != null ? rcd.
                        RequisitionNo.SourceNo : string.Empty;
                    toObj.Memo = rws != null ? rws.Reason : string.Empty;
                }
            }
            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2,
            toObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
