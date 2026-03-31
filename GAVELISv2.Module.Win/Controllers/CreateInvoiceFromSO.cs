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
    public partial class CreateInvoiceFromSO : ViewController
    {
        private PopupWindowShowAction createInvoiceFromSO;
        private Invoice _Invoice;
        public CreateInvoiceFromSO()
        {
            this.TargetObjectType = typeof(Invoice);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "Invoice.CreateInvoiceFromSO";
            this.createInvoiceFromSO = new PopupWindowShowAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.createInvoiceFromSO.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateInvoiceFromSO_CustomizePopupWindowParams);
            this.createInvoiceFromSO.Execute += new 
            PopupWindowShowActionExecuteEventHandler(CreateInvoiceFromSO_Execute
            );
        }
        private void CreateInvoiceFromSO_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _Invoice = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Invoice;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(SalesOrder)
            );
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(SalesOrder), 
            listViewId);
            if (_Invoice.Customer != null) {collectionSource.Criteria[
                "ModelCriteria"] = CriteriaOperator.Parse(
                "[Status] In ('Approved', 'PartiallyInvoiced') And [Customer.No] = '" 
                + _Invoice.Customer.No + "'");} else {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse("[Status] In ('Approved', 'PartiallyInvoiced')");
            }
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void CreateInvoiceFromSO_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            SalesOrder so = _Invoice.Session.GetObjectByKey<SalesOrder>(((
            SalesOrder)e.PopupWindow.View.SelectedObjects[0]).Oid);
            _Invoice.SONumber = so;
            _Invoice.ReferenceNo = so.SourceNo;
            _Invoice.Customer = so.Customer;
            _Invoice.CustomerAddress= so.CustomerAddress;
            _Invoice.ShipToAddress = so.ShipToAddress;
            _Invoice.Terms = so.Terms;
            foreach (SalesOrderDetail item in ((SalesOrder)e.PopupWindow.
            View.SelectedObjects[0]).SalesOrderDetails) {
                if (item.RemainingQty > 0) {
                    SalesOrderDetail tmp = _Invoice.Session.GetObjectByKey<
                    SalesOrderDetail>(item.Oid);
                    InvoiceDetail invDet = new InvoiceDetail(_Invoice.Session);
                    invDet.GenJournalID = _Invoice;
                    invDet.ItemNo = tmp.ItemNo;
                    invDet.Description = tmp.Description;
                    invDet.Ordered = tmp.Quantity;
                    invDet.Invoiced = tmp.Invoiced;
                    invDet.Quantity = tmp.RemainingQty;
                    invDet.UOM = tmp.UOM;
                    invDet.Factor = tmp.Factor;
                    invDet.BasePrice = tmp.BasePrice;
                    invDet.SalesOrderDetailID= tmp;
                    invDet.RequisitionNo = tmp.RequisitionNo != null ? tmp.RequisitionNo : null;
                    invDet.CostCenter = tmp.CostCenter != null ? tmp.CostCenter : null;
                    invDet.RequestedBy = tmp.RequestedBy != null ? tmp.RequestedBy : null;
                    invDet.Save();
                    //tmp.InvDetID = invDet;
                }
            }
            //_Receipt.Session.CommitTransaction();
            //ObjectSpace.ReloadObject(_Receipt);
            //ObjectSpace.Refresh();
        }
    }
}
