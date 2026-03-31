using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TransferOrderToWOController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.TransferOrderDetail _Tod;
        private BusinessObjectsAlias.CarryOutRequest _Obj;
        public TransferOrderToWOController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void TransferOrderToWOAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Tod = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.TransferOrderDetail;
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.WorkOrder;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CarryOutWO_Detail", true, _Obj);
        }

        private void TransferOrderToWOAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            BusinessObjectsAlias.WorkOrder woObj;
            if (_Obj.WOrders == null)
            {
                woObj = _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                WorkOrder>();
                woObj.EntryDate = DateTime.Now;
            }
            else
            {
                woObj = _ObjectSpace2.GetObject<BusinessObjectsAlias.WorkOrder>(
                _Obj.WOrders);
            }
            var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
            StringBuilder sb = new StringBuilder(!string.IsNullOrEmpty(woObj.ReferenceNo) ? woObj.ReferenceNo + "," : string.Empty);
            foreach (BusinessObjectsAlias.TransferOrderDetail item in selwo)
            {
                BusinessObjectsAlias.RequisitionWorksheet rws = null;
                if (item.RequisitionNo != null && item.RequestID != Guid.Empty)
                {
                    rws = _ObjectSpace2.FindObject<BusinessObjectsAlias.RequisitionWorksheet>(BinaryOperator.Parse("[RowID] = ?", item.RequestID));
                }
                BusinessObjectsAlias.TransferOrderDetail tod = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.TransferOrderDetail>(item);
                BusinessObjectsAlias.WorkOrderItemDetail woDetail =
                _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                WorkOrderItemDetail>();
                woDetail.GenJournalID = woObj;
                woDetail.DateIssued = tod.GenJournalID.EntryDate;
                woDetail.ItemNo = tod.ItemNo;
                woDetail.Quantity = tod.Quantity;
                woDetail.Warehouse = tod.Warehouse;
                woDetail.UOM = tod.UOM;
                woDetail.IsFromReceipt = true;
                woDetail.TransOrdDetailID = tod;
                if (rws != null)
                {
                    woDetail.RequestID = rws.RowID;
                    var rcpts = rws.ReqCarryoutTransactions.Where(o => o.SourceType.Code == "RC").LastOrDefault();
                    BusinessObjectsAlias.ReceiptDetail rcptline = null;
                    BusinessObjectsAlias.Receipt rcpt = null;
                    if (rcpts != null)
                    {
                        rcpt = _ObjectSpace2.GetObject<BusinessObjectsAlias.Receipt>(rcpts.TransactionId as BusinessObjectsAlias.Receipt);
                        rcptline = _ObjectSpace2.GetObjectByKey<BusinessObjectsAlias.ReceiptDetail>(rcpts.LineNo);
                        woDetail.Vendor = rcpt != null ? rcpt.Vendor : null;
                        woDetail.Origin = rcpt != null ? rcpt.Vendor.Origin : null;
                        woDetail.Price = rcptline != null ? rcptline.Cost : 0m;
                    }
                    else
                    {
                        woDetail.Vendor = tod != null ? tod.Vendor : null;
                        woDetail.Origin = tod != null ? tod.Vendor != null ? tod.Vendor.Origin : null : null;
                    }
                }
                woDetail.ExpenseType = tod.ExpenseType ?? null;
                woDetail.SubExpenseType = tod.SubExpenseType ?? null;
                woDetail.RequisitionNo = tod.RequisitionNo != null ? tod.RequisitionNo : null;
                woDetail.CostCenter = tod.CostCenter != null ? tod.CostCenter : null;
                woDetail.RequestedBy = tod.RequestedBy != null ? tod.RequestedBy : null;
                woDetail.ExpenseType = tod.ExpenseType ?? null;
                woDetail.SubExpenseType = tod.SubExpenseType ?? null;
                woDetail.Facility = tod.Facility ?? null;
                woDetail.FacilityHead = tod.FacilityHead ?? null;
                woDetail.Department = tod.Department ?? null;
                woDetail.DepartmentInCharge = tod.DepartmentInCharge ?? null;
                woDetail.Save();
                if (rws != null)
                {
                    //rws.LastCarrySource = woObj;
                    rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.
                    WorkOrder;
                    rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                    rws.Save();
                }
                //BusinessObjectsAlias.FATruck flt = _ObjectSpace2.FindObject<
                //BusinessObjectsAlias.FATruck>(DevExpress.Data.Filtering.
                //CriteriaOperator.Parse("[No] = '" + tod.CostCenter.Code + "'"));
                BusinessObjectsAlias.FixedAsset flt = null;
                if (tod.CostCenter != null && tod.CostCenter.FixedAsset != null)
                {
                    switch (tod.CostCenter.FixedAsset.FixedAssetClass)
                    {
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.LandAndBuilding:
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Truck:
                            flt = tod.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Trailer:
                            flt = tod.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.GeneratorSet:
                            flt = tod.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.OtherVehicle:
                            flt = tod.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Other:
                            break;
                        default:
                            break;
                    }
                }
                if (_Obj.WOrders == null)
                {
                    woObj.Fleet = flt != null ? flt : null;
                    //woObj.ReferenceNo = tod.RequisitionNo != null ? tod.RequisitionNo.SourceNo : null;
                    if (tod.RequisitionNo != null && !sb.ToString().Contains(tod.RequisitionNo.SourceNo))
                    {
                        sb.AppendFormat("{0},", tod.RequisitionNo.SourceNo);
                    }
                    //woObj.Problem = rcd.Reason;
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
