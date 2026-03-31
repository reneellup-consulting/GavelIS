using System;
using System.Linq;
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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class CarryOutTO : WinDetailViewController {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.RequisitionWorksheet _ReqWs;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction carryOutRequest;
        public CarryOutTO() {
            this.TargetObjectType = typeof(BusinessObjectsAlias.
            RequisitionWorksheet);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.CarryOutTO", this.GetType().
            Name);
            this.carryOutRequest = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.SuppressConfirmation = true;
            this.carryOutRequest.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CarryOutRequest_CustomizePopupWindowParams);
            this.carryOutRequest.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CarryOutRequest_Execute);
        }
        private void CarryOutRequest_CustomizePopupWindowParams(object sender, 
        CustomizePopupWindowParamsEventArgs e) {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReqWs = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject 
            as BusinessObjectsAlias.RequisitionWorksheet;
            var selected = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (BusinessObjectsAlias.RequisitionWorksheet item in selected)
            {
                if (item.Cancelled)
                {
                    throw new UserFriendlyException("One of the request selected was marked as cancelled");
                }
                if (item.RequisitionInfo.Status != BusinessObjectsAlias.RequisitionStatusEnum.Approved)
                {
                    throw new UserFriendlyException("One of the request selected was not approved");
                }
            }
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.
            TransferOrder;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "CarryOutTO_Detail", true, _Obj);
        }
        private void CarryOutRequest_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.TransferOrder toObj;
            if (_Obj.TOrders == null) {
                toObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                TransferOrder>();
                toObj.EntryDate = DateTime.Now;
                toObj.FromWarehouse = _Obj.FromWarehouse != null ? _ObjectSpace2
                .GetObject<BusinessObjectsAlias.Warehouse>(_Obj.FromWarehouse) : 
                null;
            } else {
                toObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.
                TransferOrder>(_Obj.TOrders);
            }
            var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            foreach (BusinessObjectsAlias.RequisitionWorksheet item in selwo)
            {
                #region if receipt has been done
                var rcpts = item.ReqCarryoutTransactions.Where(o => o.SourceType.Code == "RC").LastOrDefault();
                BusinessObjectsAlias.Receipt rcpt = null;
                if (rcpts != null)
                {
                    rcpt = _ObjectSpace2.GetObject<BusinessObjectsAlias.Receipt>(rcpts.TransactionId as BusinessObjectsAlias.Receipt);
                }
                #endregion
                BusinessObjectsAlias.RequisitionWorksheet rws = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.RequisitionWorksheet>(item);
                BusinessObjectsAlias.TransferOrderDetail toDetail = 
                _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                TransferOrderDetail>();
                toDetail.GenJournalID = toObj;
                toDetail.ItemNo = rws.ItemNo;
                toDetail.Description = rws.Description;
                toDetail.Quantity = rws.Quantity;
                toDetail.UOM = rws.UOM;
                toDetail.RequestID = rws.RowID;
                toDetail.Vendor = rcpt != null ? rcpt.Vendor : null;
                toDetail.Origin = rcpt != null ? rcpt.Vendor.Origin : null;
                toDetail.ExpenseType = rws.ExpenseType ?? null;
                toDetail.SubExpenseType = rws.SubExpenseType ?? null;
                toDetail.RequisitionNo = rws.RequisitionInfo != null ? rws.RequisitionInfo : null;
                toDetail.CostCenter = rws.CostCenter != null ? rws.CostCenter : null;
                toDetail.RequestedBy = rws.RequisitionInfo.RequestedBy != null ? rws.RequisitionInfo.RequestedBy : null;
                toDetail.Facility = rws.Facility ?? null;
                toDetail.FacilityHead = rws.FacilityHead ?? null;
                toDetail.Department = rws.Department ?? null;
                toDetail.DepartmentInCharge = rws.DepartmentInCharge ?? null;
                toDetail.Save();
                //rws.LastCarrySource = toObj;
                rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.
                TransferOrder;
                rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                rws.Save();
                if (_Obj.TOrders == null) {toObj.ReferenceNo = rws.
                    RequisitionInfo.SourceNo;
                toObj.Memo = rws.Reason;
                }
            }
            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2, 
            toObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
