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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CreateFuelRegFromPO : ViewController
    {
        private PopupWindowShowAction createReceiptFromPO;
        private FuelPumpRegister _Receipt;
        public CreateFuelRegFromPO()
        {
            this.TargetObjectType = typeof(FuelPumpRegister);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "FuelPumpRegister.CreateFuelRegFromPO";
            this.createReceiptFromPO = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.createReceiptFromPO.Caption = "Create from PO";
            this.createReceiptFromPO.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            CreateReceiptFromPO_CustomizePopupWindowParams);
            this.createReceiptFromPO.Execute += new
            PopupWindowShowActionExecuteEventHandler(CreateReceiptFromPO_Execute
            );
        }
        private void CreateReceiptFromPO_CustomizePopupWindowParams(object
        sender, CustomizePopupWindowParamsEventArgs e)
        {
            _Receipt = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FuelPumpRegister;
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
            

            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void CreateReceiptFromPO_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            PurchaseOrder po = _Receipt.Session.GetObjectByKey<PurchaseOrder>(((
            PurchaseOrder)e.PopupWindow.View.SelectedObjects[0]).Oid);
            _Receipt.EntryDate = po.EntryDate;
            _Receipt.PurchaseOrderRef = po;
            _Receipt.FuelRequestRef = po.ReferenceNo;
            //_Receipt.Unit = null; // Get from PODetails ChargeTo
            _Receipt.Vendor = po.Vendor;
            foreach (PurchaseOrderDetail item in ((PurchaseOrder)e.PopupWindow.
            View.SelectedObjects[0]).PurchaseOrderDetails)
            {
                if (item.RemainingQty > 0)
                {
                    PurchaseOrderDetail tmp = _Receipt.Session.GetObjectByKey<
                    PurchaseOrderDetail>(item.Oid);
                    FuelPumpRegisterDetail recpt = new FuelPumpRegisterDetail(_Receipt.Session);
                    recpt.GenJournalID = _Receipt;
                    recpt.ItemNo = tmp.ItemNo;
                    recpt.Description = tmp.Description;
                    recpt.Ordered = tmp.Quantity;
                    recpt.Received = tmp.Received;
                    recpt.Quantity = tmp.RemainingQty;
                    recpt.UOM = tmp.UOM;
                    recpt.Factor = tmp.Factor;
                    recpt.BaseCost = tmp.BaseCost;
                    recpt.PODetailID = tmp;
                    recpt.Save();
                    _Receipt.ChargeTo = tmp.CostCenter ?? null;
                    _Receipt.Unit = tmp.CostCenter != null ? tmp.CostCenter.FixedAsset ?? null : null;
                    _Receipt.Requestor = tmp.RequestedBy ?? null;
                    _Receipt.Purpose = tmp.Remarks;
                }
            }
        }
    }
}
