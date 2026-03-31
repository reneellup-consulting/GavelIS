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
    public partial class CreateMemoFromInvoice : ViewController {
        private PopupWindowShowAction createMemoFromInvoice;
        private CreditMemo _CreditMemo;
        public CreateMemoFromInvoice() {
            this.TargetObjectType = typeof(CreditMemo);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "CreditMemo.CreateMemoFromInvoice";
            this.createMemoFromInvoice = new PopupWindowShowAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.createMemoFromInvoice.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateMemoFromInvoice_CustomizePopupWindowParams);
            this.createMemoFromInvoice.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            CreateMemoFromInvoice_Execute);
        }
        private void CreateMemoFromInvoice_CustomizePopupWindowParams(object 
        sender, CustomizePopupWindowParamsEventArgs e) {
            _CreditMemo = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as CreditMemo;
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(Invoice));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(Invoice), listViewId);
            //if (_CreditMemo.Customer != null) {collectionSource.Criteria[
            //    "ModelCriteria"] = CriteriaOperator.Parse(
            //    "[InvoiceType] <> 'Cash' And [Status] In ('Invoiced', 'PartiallyReturned', 'PartiallyPaid', 'Paid') And [Customer.No] = '" 
            //    + _CreditMemo.Customer.No + "'");} else {
            //    collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
            //    Parse(
            //    "[InvoiceType] <> 'Cash' And [Status] In ('Invoiced', 'PartiallyReturned', 'PartiallyPaid', 'Paid')"
            //    );
            //}
            if (_CreditMemo.Customer != null)
            {
                collectionSource.Criteria[
                    "ModelCriteria"] = CriteriaOperator.Parse(
                    "[Status] In ('Invoiced', 'PartiallyReturned', 'PartiallyPaid', 'Paid') And [Customer.No] = '"
                    + _CreditMemo.Customer.No + "'");
            }
            else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse(
                "[Status] In ('Invoiced', 'PartiallyReturned', 'PartiallyPaid', 'Paid')"
                );
            }
            e.View = Application.CreateListView(listViewId, collectionSource, 
            true);
        }
        private void CreateMemoFromInvoice_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            Invoice inv = _CreditMemo.Session.GetObjectByKey<Invoice>(((Invoice)
            e.PopupWindow.View.SelectedObjects[0]).Oid);
            _CreditMemo.InvoiceNo = inv;
            _CreditMemo.ReferenceNo = inv.SourceNo;
            _CreditMemo.Customer = inv.Customer;
            _CreditMemo.CustomerAddress = inv.CustomerAddress;
            _CreditMemo.ShipToAddress = inv.ShipToAddress;
            _CreditMemo.ShipVia = inv.ShipVia;
            foreach (InvoiceDetail item in ((Invoice)e.PopupWindow.View.
            SelectedObjects[0]).InvoiceDetails) {
                if (item.Quantity > 0) {
                    InvoiceDetail tmp = _CreditMemo.Session.GetObjectByKey<
                    InvoiceDetail>(item.Oid);
                    CreditMemoDetail creditMemo = new CreditMemoDetail(
                    _CreditMemo.Session);
                    creditMemo.GenJournalID = _CreditMemo;
                    creditMemo.ItemNo = tmp.ItemNo;
                    creditMemo.Description = tmp.Description;
                    creditMemo.Returning = tmp.Quantity;
                    creditMemo.Returned = tmp.Returned;
                    creditMemo.Quantity = tmp.Returned == 0 ? tmp.Quantity : tmp
                    .Quantity - tmp.Returned;
                    creditMemo.UOM = tmp.UOM;
                    creditMemo.Factor = tmp.Factor;
                    creditMemo.BasePrice = tmp.BasePrice;
                    creditMemo.Price = tmp.Price;
                    creditMemo.Warehouse = tmp.Warehouse;
                    creditMemo.Tax = tmp.Tax;
                    creditMemo.LineDiscount = tmp.LineDiscount;
                    creditMemo.InvoiceDetailID = tmp;
                    creditMemo.Save();
                }
            }
        }
    }
}
