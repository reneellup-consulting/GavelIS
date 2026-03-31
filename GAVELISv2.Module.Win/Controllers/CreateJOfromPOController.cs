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
    public partial class CreateJOfromPOController : ViewController
    {
        private PopupWindowShowAction createJOfromPO;
        private JobOrder _JobOrder;
        public CreateJOfromPOController()
        {
            this.TargetObjectType = typeof(JobOrder);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "CreateJOfromPOActionID";
            this.createJOfromPO = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.createJOfromPO.Caption = "Get Details from PO";
            this.createJOfromPO.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateJOfromPO_CustomizePopupWindowParams);
            this.createJOfromPO.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CreateJOfromPO_Execute
            );
        }
        private void CreateJOfromPO_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _JobOrder = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as JobOrder;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PurchaseOrder_ListView_All";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(PurchaseOrder), 
            listViewId);
            if (_JobOrder.Vendor != null)
            {
                collectionSource.Criteria[
                    "ModelCriteria"] = CriteriaOperator.Parse(
                    "[Status] In ('Approved', 'PartiallyReceived') And [Vendor.No] = '"
                    + _JobOrder.Vendor.No + "'");
            }
            else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[Status] In ('Approved', 'PartiallyReceived')");
            }
            //if (_JobOrder.Vendor != null)
            //{
            //    collectionSource.Criteria[
            //        "ModelCriteria"] = CriteriaOperator.Parse(
            //        "[Status] In ('Current' ,'PartiallyReceived') And [Vendor.No] = '"
            //        + _JobOrder.Vendor.No + "'");
            //}
            //else
            //{
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse("[Status] In ('Current' ,'PartiallyReceived')");
            //}
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void CreateJOfromPO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            PurchaseOrder po = _JobOrder.Session.GetObjectByKey<PurchaseOrder>(((
            PurchaseOrder)e.PopupWindow.View.SelectedObjects[0]).Oid);
            _JobOrder.PurchaseOrderNo = po;
            _JobOrder.ReferenceNo = po.SourceNo;
            _JobOrder.Vendor = po.Vendor;
            _JobOrder.VendorAddress = po.VendorAddress;
            _JobOrder.Terms = po.Terms;
            // temporary alteration of Receipt Date = PO Expected Date
            //_Receipt.EntryDate = po.ExpectedDate;
            foreach (PurchaseOrderDetail item in ((PurchaseOrder)e.PopupWindow.
            View.SelectedObjects[0]).PurchaseOrderDetails) {
                if (item.RemainingQty > 0) {
                    PurchaseOrderDetail tmp = _JobOrder.Session.GetObjectByKey<
                    PurchaseOrderDetail>(item.Oid);
                    JobOrderDetail jodt = new JobOrderDetail(_JobOrder.Session);
                    jodt.GenJournalID = _JobOrder;
                    jodt.ItemNo = tmp.ItemNo;
                    jodt.Description = tmp.Description;
                    jodt.Ordered = tmp.Quantity;
                    jodt.Received = tmp.Received;
                    jodt.Quantity = tmp.RemainingQty;
                    jodt.UOM = tmp.UOM;
                    jodt.Factor = tmp.Factor;
                    jodt.BaseCost = tmp.BaseCost;
                    jodt.PODetailID = tmp;
                    jodt.RequestID = tmp.RequestID;
                    jodt.RequisitionNo = tmp.RequisitionNo != null ? tmp.RequisitionNo : null;
                    jodt.CostCenter = tmp.CostCenter != null ? tmp.CostCenter : null;
                    jodt.RequestedBy = tmp.RequestedBy != null ? tmp.RequestedBy : null;
                    jodt.Save();
                }
            }
            //_Receipt.Session.CommitTransaction();
            //ObjectSpace.ReloadObject(_Receipt);
            //ObjectSpace.Refresh();
        }
    }
}
