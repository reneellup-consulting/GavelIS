using System;
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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class CreateReceiptFromPO : ViewController {
        private PopupWindowShowAction createReceiptFromPO;
        private Receipt _Receipt;
        public CreateReceiptFromPO() {
            this.TargetObjectType = typeof(Receipt);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "Receipt.CreateReceiptFromPO";
            this.createReceiptFromPO = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.createReceiptFromPO.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateReceiptFromPO_CustomizePopupWindowParams);
            this.createReceiptFromPO.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CreateReceiptFromPO_Execute
            );
        }
        private void CreateReceiptFromPO_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _Receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Receipt;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PurchaseOrder_ListView_All";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(PurchaseOrder), 
            listViewId);
            if (_Receipt.Vendor != null)
            {
                collectionSource.Criteria[
                    "ModelCriteria"] = CriteriaOperator.Parse(
                    "[Status] In ('Approved', 'PartiallyReceived') And [Vendor.No] = '"
                    + _Receipt.Vendor.No + "'");
            }
            else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[Status] In ('Approved', 'PartiallyReceived')");
            }
            //if (_Receipt.Vendor != null)
            //{
            //    collectionSource.Criteria[
            //        "ModelCriteria"] = CriteriaOperator.Parse(
            //        "[Status] In ('Current', 'PartiallyReceived') And [Vendor.No] = '"
            //        + _Receipt.Vendor.No + "'");
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse("[Status] In ('Current', 'PartiallyReceived')");
            //}

            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void CreateReceiptFromPO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            PurchaseOrder po = _Receipt.Session.GetObjectByKey<PurchaseOrder>(((
            PurchaseOrder)e.PopupWindow.View.SelectedObjects[0]).Oid);
            _Receipt.PurchaseOrderNo = po;
            _Receipt.ReferenceNo = po.SourceNo;
            _Receipt.Vendor = po.Vendor;
            _Receipt.VendorAddress = po.VendorAddress;
            _Receipt.ShipToAddress = po.ShipToAddress;
            _Receipt.Terms = po.Terms;
            // temporary alteration of Receipt Date = PO Expected Date
            //_Receipt.EntryDate = po.ExpectedDate;
            foreach (PurchaseOrderDetail item in ((PurchaseOrder)e.PopupWindow.
            View.SelectedObjects[0]).PurchaseOrderDetails) {
                if (item.RemainingQty > 0) {
                    PurchaseOrderDetail tmp = _Receipt.Session.GetObjectByKey<
                    PurchaseOrderDetail>(item.Oid);
                    ReceiptDetail recpt = new ReceiptDetail(_Receipt.Session);
                    recpt.GenJournalID = _Receipt;
                    recpt.ItemNo = tmp.ItemNo;
                    recpt.Description = tmp.Description;
                    recpt.Ordered = tmp.Quantity;
                    recpt.Received = tmp.Received;
                    recpt.Quantity = tmp.RemainingQty;
                    recpt.UOM = tmp.UOM;
                    recpt.Factor = tmp.Factor;
                    recpt.BaseCost = tmp.BaseCost;
                    recpt.LineDiscPercent = tmp.LineDiscPercent;
                    recpt.LineDiscount = tmp.LineDiscount;
                    recpt.PODetailID = tmp;
                    recpt.RequisitionNo = tmp.RequisitionNo != null ? tmp.RequisitionNo : null;
                    recpt.CostCenter = tmp.CostCenter != null ? tmp.CostCenter : null;
                    recpt.Warehouse = tmp.StockTo != null ? tmp.StockTo : null;
                    recpt.RequestedBy = tmp.RequestedBy != null ? tmp.RequestedBy : null;
                    recpt.Facility = tmp.Facility ?? null;
                    recpt.FacilityHead = tmp.FacilityHead ?? null;
                    recpt.Department = tmp.Department ?? null;
                    recpt.DepartmentInCharge = tmp.DepartmentInCharge ?? null;
                    recpt.Save();
                }
            }
            //_Receipt.Session.CommitTransaction();
            //ObjectSpace.ReloadObject(_Receipt);
            //ObjectSpace.Refresh();
        }
    }
}
