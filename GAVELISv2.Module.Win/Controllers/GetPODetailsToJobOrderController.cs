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
    public partial class GetPODetailsToJobOrderController : ViewController
    {
        private PopupWindowShowAction GetPODetailsToJobAction;
        private JobOrder _Jobs;
        private Vendor _FVendor;
        public GetPODetailsToJobOrderController()
        {
            this.TargetObjectType = typeof(JobOrder);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "Receipt.GetPODetailsToJob";
            this.GetPODetailsToJobAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GetPODetailsToJobAction.Caption = "Receive from Multi PO";
            this.GetPODetailsToJobAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(GetPODetailsToJobAction_CustomizePopupWindowParams);
            this.GetPODetailsToJobAction.Execute += new PopupWindowShowActionExecuteEventHandler(GetPODetailsToJobAction_Execute);
        }
        void GetPODetailsToJobAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var selectedObjects = e.PopupWindow.View.SelectedObjects;
            int index = 0;
            StringBuilder sb = new StringBuilder();
            List<string> strRefs = new List<string>();
            if (_Jobs.ReceiptDetails.Count > 0)
            {
                strRefs = _Jobs.Refs ?? new List<string>();
                sb.AppendFormat("{0},", _Jobs.ReferenceNo);
            }
            else
            {
                _Jobs.ReferenceNo = string.Empty;
            }
            foreach (PurchaseOrderDetail item in selectedObjects)
            {
                PurchaseOrderDetail detail = _Jobs.Session.GetObjectByKey<PurchaseOrderDetail>(item.Oid);
                if (_Jobs.ReceiptDetails.Count > 0)
                {
                    var exist = _Jobs.JobOrderDetails.Where(o => o.PODetailID == detail);
                    if (exist.Count() > 0)
                    {
                        throw new ApplicationException(string.Format("Purchase Order Detail ID # {0} already exist", detail.Oid));
                    }
                }
                if (_Jobs.Vendor == null)
                {
                    index++;
                    if (index == 1)
                    {
                        _FVendor = _Jobs.Session.GetObjectByKey<Vendor>(item.PurchaseInfo.Vendor.Oid);
                        _Jobs.Vendor = _FVendor;
                        _Jobs.VendorAddress = detail.PurchaseInfo.VendorAddress;
                        //_Jobs.ShipToAddress = detail.PurchaseInfo.ShipToAddress;
                        _Jobs.Terms = detail.PurchaseInfo.Terms;
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
                    JobOrderDetail recpt = new JobOrderDetail(_Jobs.Session);
                    recpt.GenJournalID = _Jobs;
                    recpt.ItemNo = detail.ItemNo;
                    recpt.Description = detail.Description;
                    recpt.Ordered = detail.Quantity;
                    recpt.Received = detail.Received;
                    recpt.Quantity = detail.RemainingQty;
                    recpt.UOM = detail.UOM;
                    recpt.Factor = detail.Factor;
                    recpt.BaseCost = detail.BaseCost;
                    //recpt.LineDiscPercent = detail.LineDiscPercent;
                    //recpt.LineDiscount = detail.LineDiscount;
                    _Jobs.UpdateTotal(true);
                    recpt.PODetailID = detail;
                    recpt.RequisitionNo = detail.RequisitionNo != null ? detail.RequisitionNo : null;
                    recpt.RequestID = detail.RequestID;
                    recpt.CostCenter = detail.CostCenter != null ? detail.CostCenter : null;
                    recpt.RequestedBy = detail.RequestedBy != null ? detail.RequestedBy : null;
                    recpt.Save();
                }
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            _Jobs.Refs = strRefs;
            _Jobs.ReferenceNo = sb.ToString();
        }

        void GetPODetailsToJobAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _FVendor = null;
            _Jobs = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as JobOrder;
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "PurchaseOrderDetail_ListView_ToJob";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(PurchaseOrderDetail),
            listViewId);
            // [PurchaseInfo.Status] In ('Approved', 'PartiallyReceived') 
            // And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m
            if (_Jobs.Vendor != null)
            {
                collectionSource.Criteria[
                "ModelCriteria"] = CriteriaOperator.Parse(
                "[PurchaseInfo.Status] In ('Approved', 'PartiallyReceived')And [LineApprovalStatus] = 'Released' And [RemainingQty] <> 0.0m And [PurchaseInfo.Vendor.No] = '"
                 + _Jobs.Vendor.No + "'");
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
