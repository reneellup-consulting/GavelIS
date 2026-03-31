using System;
using System.Linq;
using System.ComponentModel;
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
using System.Threading.Tasks;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetPODetailsToReceiptFuelController : ViewController
    {
        private PopupWindowShowAction GetPODetailsToReceiptFuelAction;
        private ReceiptFuel _Receipt;
        private Vendor _FVendor;
        public GetPODetailsToReceiptFuelController()
        {
            this.TargetObjectType = typeof(ReceiptFuel);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "ReceiptFuel.GetPODetailsToReceiptFuel";
            this.GetPODetailsToReceiptFuelAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GetPODetailsToReceiptFuelAction.Caption = "Receive from Multi PO";
            this.GetPODetailsToReceiptFuelAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(GetPODetailsToReceiptFuelAction_CustomizePopupWindowParams);
            this.GetPODetailsToReceiptFuelAction.Execute += new PopupWindowShowActionExecuteEventHandler(GetPODetailsToReceiptFuelAction_Execute);
        }

        void GetPODetailsToReceiptFuelAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var selectedObjects = e.PopupWindow.View.SelectedObjects;
            int index = 0;
            StringBuilder sb = new StringBuilder();
            List<string> strRefs = new List<string>();
            if (_Receipt.ReceiptFuelDetails.Count > 0)
            {
                strRefs = _Receipt.Refs ?? new List<string>();
                sb.AppendFormat("{0},", _Receipt.ReferenceNo);
            }
            else
            {
                _Receipt.ReferenceNo = string.Empty;
            }
            var nonCash = _Receipt.ReceiptFuelDetails.Where(o => o.PettyCashID == null).FirstOrDefault();
            var cashs = selectedObjects.Cast<PurchaseOrderDetail>().Where(o => o.PettyCashID != null).FirstOrDefault();
            foreach (PurchaseOrderDetail item in selectedObjects)
            {
                if (nonCash != null && cashs != null)
                {
                    throw new UserFriendlyException("Cannot combine non cash purchases from cash purchases");
                }
                if (cashs != null && item.PettyCashID == null)
                {
                    throw new UserFriendlyException("Cannot combine non cash purchases from cash purchases");
                }
                //if (_Receipt.InvoiceType == InvoiceTypeEnum.Cash && item.PettyCashID == null)
                //{
                //    throw new UserFriendlyException("Cannot combine non cash purchases from cash purchases");
                //}
                //if (cashs != null)
                //{
                //    _Receipt.InvoiceType = InvoiceTypeEnum.Cash;
                //}

                PurchaseOrderDetail detail = _Receipt.Session.GetObjectByKey<PurchaseOrderDetail>(item.Oid);
                if (_Receipt.ReceiptDetails.Count > 0)
                {
                    var exist = _Receipt.ReceiptFuelDetails.Where(o => o.PODetailID2 == detail);
                    if (exist.Count() > 0)
                    {
                        throw new ApplicationException(string.Format("Purchase Order Detail ID # {0} already exist", detail.Oid));
                    }
                }
                if (_Receipt.Vendor == null)
                {
                    index++;
                    if (index == 1)
                    {
                        _FVendor = _Receipt.Session.GetObjectByKey<Vendor>(item.PurchaseInfo.Vendor.Oid);
                        _Receipt.Vendor = _FVendor;
                        _Receipt.VendorAddress = detail.PurchaseInfo.VendorAddress;
                        _Receipt.ShipToAddress = detail.PurchaseInfo.ShipToAddress;
                        _Receipt.Terms = detail.PurchaseInfo.Terms;
                    }
                    else if (_FVendor.No != item.PurchaseInfo.Vendor.No)
                    {
                        throw new ApplicationException("Cannot receive PO details from multiple vendors");
                    }
                }
                // Code for multiple reference no.
                if (!strRefs.Contains(detail.PurchaseInfo.SourceNo))
                {
                    strRefs.Add(detail.PurchaseInfo.SourceNo);
                    sb.AppendFormat("{0},", detail.PurchaseInfo.SourceNo);
                }
                if (detail.RemainingQty > 0)
                {
                    ReceiptFuelDetail recpt = new ReceiptFuelDetail(_Receipt.Session);
                    recpt.GenJournalID = _Receipt;
                    recpt.ItemNo = detail.ItemNo;
                    recpt.Description = detail.Description;
                    recpt.Ordered = detail.Quantity;
                    recpt.Received = detail.Received;
                    recpt.Quantity = detail.RemainingQty;
                    recpt.UOM = detail.UOM;
                    recpt.Factor = detail.Factor;
                    recpt.BaseCost = detail.BaseCost;
                    recpt.LineDiscPercent = detail.LineDiscPercent;
                    recpt.LineDiscount = detail.LineDiscount;
                    recpt.PODetailID2 = detail;
                    recpt.Remarks = detail.Remarks;
                    recpt.RequisitionNo = detail.RequisitionNo != null ? detail.RequisitionNo : null;
                    recpt.CostCenter = detail.CostCenter != null ? detail.CostCenter : null;
                    recpt.Warehouse = detail.StockTo != null ? detail.StockTo : null;
                    recpt.RequestedBy = detail.RequestedBy != null ? detail.RequestedBy : null;
                    recpt.Facility = detail.Facility ?? null;
                    recpt.FacilityHead = detail.FacilityHead ?? null;
                    recpt.Department = detail.Department ?? null;
                    recpt.DepartmentInCharge = detail.DepartmentInCharge ?? null;
                    recpt.PettyCashID = detail.PettyCashID ?? null;
                    if (detail.RequisitionNo != null && detail.RequestID != Guid.Empty)
                    {
                        RequisitionWorksheet rws = _Receipt.Session.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", detail.RequestID));
                        if (rws != null)
                        {
                            recpt.ExpenseType = rws.ExpenseType ?? null;
                            recpt.SubExpenseType = rws.SubExpenseType ?? null;
                        }
                    }
                    recpt.Save();
                }
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            _Receipt.Refs = strRefs;
            _Receipt.ReferenceNo = sb.ToString();
        }

        void GetPODetailsToReceiptFuelAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _FVendor = null;
            _Receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ReceiptFuel;
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PurchaseOrderDetail_ListView_ToFuelReceive";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(PurchaseOrderDetail),
            listViewId);
            // [PurchaseInfo.Status] In ('Approved', 'PartiallyReceived') 
            // And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m
            if (_Receipt.Vendor != null)
            {
                collectionSource.Criteria[
                "ModelCriteria"] = CriteriaOperator.Parse(
                "[PurchaseInfo.Status] In ('Approved', 'PartiallyReceived')And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m And [PurchaseInfo.Vendor.No] = '"
                 + _Receipt.Vendor.No + "'");
            }
            else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[PurchaseInfo.Status] In ('Approved', 'PartiallyReceived')And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m");
            }
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
