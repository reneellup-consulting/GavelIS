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
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers {
    public partial class CarryOutWO : WinDetailViewController {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.RequisitionWorksheet _ReqWs;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        private PopupWindowShowAction carryOutRequest;
        public CarryOutWO() {
            this.TargetObjectType = typeof(BusinessObjectsAlias.
            RequisitionWorksheet);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.CarryOutWO", this.GetType().
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
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.WorkOrder;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace, 
            "CarryOutWO_Detail", true, _Obj);
        }
        private void CarryOutRequest_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.WorkOrder woObj;
            if (_Obj.WOrders == null) {
                woObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                WorkOrder>();
                woObj.EntryDate = DateTime.Now;
            } else {
                woObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.WorkOrder>(
                _Obj.WOrders);
            }
            var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            StringBuilder sb = new StringBuilder(!string.IsNullOrEmpty(woObj.ReferenceNo) ? woObj.ReferenceNo + "," : string.Empty);
            foreach (BusinessObjectsAlias.RequisitionWorksheet item in selwo) {
                #region if receipt has been done
                var rcpts = item.ReqCarryoutTransactions.Where(o => o.SourceType.Code == "RC").LastOrDefault();
                BusinessObjectsAlias.ReceiptDetail rcptline = null;
                BusinessObjectsAlias.Receipt rcpt = null;
                if (rcpts != null)
                {
                    rcpt = _ObjectSpace2.GetObject<BusinessObjectsAlias.Receipt>(rcpts.TransactionId as BusinessObjectsAlias.Receipt);
                    rcptline = _ObjectSpace2.GetObjectByKey<BusinessObjectsAlias.ReceiptDetail>(rcpts.LineNo);
                }
                #endregion
                #region if transfer has been done
                var trnsos = item.ReqCarryoutTransactions.Where(o => o.SourceType.Code == "TO").LastOrDefault();
                BusinessObjectsAlias.TransferOrderDetail trnso = null;
                if (trnsos != null)
                {
                    trnso = _ObjectSpace2.GetObjectByKey<BusinessObjectsAlias.TransferOrderDetail>(trnsos.LineNo);
                }
                #endregion
                BusinessObjectsAlias.RequisitionWorksheet rws = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.RequisitionWorksheet>(item);
                BusinessObjectsAlias.WorkOrderItemDetail woDetail = 
                _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                WorkOrderItemDetail>();
                woDetail.GenJournalID = woObj;
                woDetail.DateIssued = rws.ExpectedDate;
                woDetail.ItemNo = rws.ItemNo;
                woDetail.Description = rws.Description;
                woDetail.Quantity = rws.Quantity;
                woDetail.UOM = rws.UOM;
                woDetail.RequestID = rws.RowID;
                if (trnso == null && rcpt != null)
                {
                    woDetail.Vendor = rcpt != null ? rcpt.Vendor : null;
                    woDetail.Origin = rcpt != null ? rcpt.Vendor != null? rcpt.Vendor.Origin : null : null;
                    woDetail.Price = rcptline != null ? rcptline.Cost : 0m;
                }
                else if (trnso != null && rcpt == null)
                {
                    woDetail.Vendor = trnso != null ? trnso.Vendor : null;
                    woDetail.Origin = trnso != null ? trnso.Vendor!=null ? trnso.Vendor.Origin : null : null;
                }
                else if (trnso != null && rcpt != null)
                {
                    woDetail.Vendor = rcpt != null ? rcpt.Vendor : null;
                    woDetail.Origin = rcpt != null ? rcpt.Vendor != null? rcpt.Vendor.Origin : null: null;
                    woDetail.Price = rcptline != null ? rcptline.Cost : 0m;
                }
                else
                {
                    woDetail.Vendor = null;
                    woDetail.Origin = null;
                }
                woDetail.ExpenseType = rws.ExpenseType ?? null;
                woDetail.SubExpenseType = rws.SubExpenseType ?? null;
                woDetail.RequisitionNo = rws.RequisitionInfo != null ? rws.RequisitionInfo : null;
                woDetail.CostCenter = rws.CostCenter != null ? rws.CostCenter : null;
                woDetail.RequestedBy = rws.RequisitionInfo.RequestedBy != null ? rws.RequisitionInfo.RequestedBy : null;
                woDetail.Facility = rws.Facility ?? null;
                woDetail.FacilityHead = rws.FacilityHead ?? null;
                woDetail.Department = rws.Department ?? null;
                woDetail.DepartmentInCharge = rws.DepartmentInCharge ?? null;
                woDetail.Save();
                woObj.Facility = rws.Facility ?? null;
                //rws.LastCarrySource = woObj;
                rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.
                WorkOrder;
                rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                rws.Save();
                // ...
                BusinessObjectsAlias.FixedAsset flt = null;
                if (rws.CostCenter != null && rws.CostCenter.FixedAsset != null)
                {
                    switch (rws.CostCenter.FixedAsset.FixedAssetClass)
                    {
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.LandAndBuilding:
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Truck:
                            flt = rws.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Trailer:
                            flt = rws.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.GeneratorSet:
                            flt = rws.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.OtherVehicle:
                            flt = rws.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Other:
                            break;
                        default:
                            break;
                    }
                }
                // ...
                //BusinessObjectsAlias.FATruck flt = _ObjectSpace2.FindObject<
                //BusinessObjectsAlias.FATruck>(DevExpress.Data.Filtering.
                //CriteriaOperator.Parse("[No] = '" + rws.CostCenter.Code + "'"));
                if (_Obj.WOrders == null) {
                    woObj.Fleet = flt != null ? flt : null;
                    // Reference(s)
                    if (rws.RequisitionInfo != null && !sb.ToString().Contains(rws.RequisitionInfo.SourceNo))
                    {
                        sb.AppendFormat("{0},", rws.RequisitionInfo.SourceNo);
                    }
                    //woObj.ReferenceNo = rws.RequisitionInfo.SourceNo;
                    woObj.Problem = rws.Reason;
                }
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            woObj.ReferenceNo = sb.ToString();
            DetailView viewWO = Application.CreateDetailView(_ObjectSpace2, 
            woObj, true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = viewWO;
        }
    }
}
