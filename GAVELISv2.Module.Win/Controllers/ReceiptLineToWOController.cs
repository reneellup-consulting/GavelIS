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
    public partial class ReceiptLineToWOController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.ReceiptDetail _ReceiptLine;
        private BusinessObjectsAlias.CarryOutRequest _Obj;

        public ReceiptLineToWOController()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void receiptLineToWOAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ReceiptLine = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject
            as BusinessObjectsAlias.ReceiptDetail;
            _Obj = new BusinessObjectsAlias.CarryOutRequest();
            _Obj.Action = BusinessObjectsAlias.RequisitionActionsEnum.WorkOrder;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CarryOutWO_Detail", true, _Obj);
        }

        private void receiptLineToWOAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
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
            foreach (BusinessObjectsAlias.ReceiptDetail item in selwo)
            {
                BusinessObjectsAlias.RequisitionWorksheet rws = null;
                if (item.RequisitionNo != null && item.PODetailID != null && item.PODetailID.RequestID != Guid.Empty)
                {
                    rws = _ObjectSpace2.FindObject<BusinessObjectsAlias.RequisitionWorksheet>(BinaryOperator.Parse("[RowID] = ?", item.PODetailID.RequestID));
                }
                BusinessObjectsAlias.ReceiptDetail rcd = _ObjectSpace2.
                GetObject<BusinessObjectsAlias.ReceiptDetail>(item);
                BusinessObjectsAlias.WorkOrderItemDetail woDetail =
                _ObjectSpace2.CreateObject<BusinessObjectsAlias.
                WorkOrderItemDetail>();
                woDetail.GenJournalID = woObj;
                woDetail.DateIssued = rcd.GenJournalID.EntryDate;
                woDetail.ItemNo = rcd.ItemNo;
                woDetail.Quantity = rcd.Quantity;
                woDetail.UOM = rcd.UOM;
                woDetail.Price = rcd.Cost;
                woDetail.IsFromReceipt = true;
                woDetail.ReceiptDetailID = rcd;
                if (rws != null)
                {
                    woDetail.RequestID = rws.RowID;
                }
                woDetail.Vendor = rcd != null ? rcd.ReceiptInfo.Vendor : null;
                woDetail.Origin = rcd != null ? rcd.ReceiptInfo.Vendor.Origin : null;
                woDetail.ExpenseType = rcd.ExpenseType ?? null;
                woDetail.SubExpenseType = rcd.SubExpenseType ?? null;
                woDetail.RequisitionNo = rcd.RequisitionNo != null ? rcd.RequisitionNo : null;
                woDetail.CostCenter = rcd.CostCenter != null ? rcd.CostCenter : null;
                woDetail.RequestedBy = rcd.RequestedBy != null ? rcd.RequestedBy : null;
                woDetail.Facility = rcd.Facility ?? null;
                woDetail.FacilityHead = rcd.FacilityHead ?? null;
                woDetail.Department = rcd.Department ?? null;
                woDetail.DepartmentInCharge = rcd.DepartmentInCharge ?? null;
                woDetail.Save();
                if (rws != null)
                {
                    //rws.LastCarrySource = woObj;
                    rws.Action = BusinessObjectsAlias.RequisitionActionsEnum.
                    WorkOrder;
                    rws.Status = BusinessObjectsAlias.RequisitionWSStateEnum.Active;
                    rws.Save();
                }
                //BusinessObjectsAlias.FATruck flt = null;
                //if (rcd.CostCenter != null)
                //{
                //    flt = _ObjectSpace2.FindObject<BusinessObjectsAlias.FATruck>(DevExpress.Data.Filtering.CriteriaOperator.Parse("[No] = '" + rcd.CostCenter.Code + "'"));
                //}
                BusinessObjectsAlias.FixedAsset flt = null;
                if (rcd.CostCenter != null && rcd.CostCenter.FixedAsset != null)
                {
                    switch (rcd.CostCenter.FixedAsset.FixedAssetClass)
                    {
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.LandAndBuilding:
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Truck:
                            flt = rcd.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.Trailer:
                            flt = rcd.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.GeneratorSet:
                            flt = rcd.CostCenter.FixedAsset ?? null;
                            break;
                        case GAVELISv2.Module.BusinessObjects.FixedAssetClassEnum.OtherVehicle:
                            flt = rcd.CostCenter.FixedAsset ?? null;
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
                    //woObj.ReferenceNo = rcd.RequisitionNo != null ? rcd.RequisitionNo.SourceNo : null;
                    if (rcd.RequisitionNo != null && !sb.ToString().Contains(rcd.RequisitionNo.SourceNo))
                    {
                        sb.AppendFormat("{0},", rcd.RequisitionNo.SourceNo);
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
