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
    public partial class CreateMemoFromReceipt : ViewController {
        private PopupWindowShowAction createMemoFromReceipt;
        private DebitMemo _DebitMemo;
        public CreateMemoFromReceipt() {
            this.TargetObjectType = typeof(DebitMemo);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DebitMemo.CreateMemoFromReceipt";
            this.createMemoFromReceipt = new PopupWindowShowAction(this, 
            actionID, PredefinedCategory.RecordEdit);
            this.createMemoFromReceipt.CustomizePopupWindowParams += new 
            CustomizePopupWindowParamsEventHandler(
            CreateMemoFromReceipt_CustomizePopupWindowParams);
            this.createMemoFromReceipt.Execute += new 
            PopupWindowShowActionExecuteEventHandler(
            CreateMemoFromReceipt_Execute);
        }
        private void CreateMemoFromReceipt_CustomizePopupWindowParams(object
        sender, CustomizePopupWindowParamsEventArgs e) {
            _DebitMemo = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DebitMemo;
            //_Receipt.Save();
            //_Receipt.Session.CommitTransaction();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "Receipt_ListView_DM"; //Application.FindListViewId(typeof(Receipt));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(Receipt), listViewId);
            if (_DebitMemo.Vendor != null)
            {
                collectionSource.Criteria[
                "ModelCriteria"] = CriteriaOperator.Parse(
                "[Status] In ('Received', 'PartiallyReturned', 'PartiallyPaid', 'Paid') And [Vendor.No] = '"
                 + _DebitMemo.Vendor.No + "'");
            } else
            {
                collectionSource.Criteria["ModelCriteria"] = CriteriaOperator.
                Parse(
                "[Status] In ('Received', 'PartiallyReturned', 'PartiallyPaid', 'Paid')"
                );
            }
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void CreateMemoFromReceipt_Execute(object sender, 
        PopupWindowShowActionExecuteEventArgs e) {
            Receipt rcpt = _DebitMemo.Session.GetObjectByKey<Receipt>(((Receipt)
            e.PopupWindow.View.SelectedObjects[0]).Oid);
            _DebitMemo.ReceiptNo = rcpt;
            _DebitMemo.ReferenceNo = rcpt.SourceNo;
            _DebitMemo.Vendor = rcpt.Vendor;
            _DebitMemo.VendorAddress = rcpt.VendorAddress;
            _DebitMemo.BillToAddress = rcpt.CompanyInfo.FullBillAddress;
            foreach (ReceiptDetail item in ((Receipt)e.PopupWindow.View.
            SelectedObjects[0]).ReceiptDetails) {
                if (item.Quantity > 0) {
                    ReceiptDetail tmp = _DebitMemo.Session.GetObjectByKey<
                    ReceiptDetail>(item.Oid);
                    DebitMemoDetail debitMemo = new DebitMemoDetail(_DebitMemo.
                    Session);
                    debitMemo.GenJournalID = _DebitMemo;
                    debitMemo.ItemNo = tmp.ItemNo;
                    debitMemo.Description = tmp.Description;
                    debitMemo.Returning = tmp.Quantity;
                    debitMemo.Returned = tmp.Returned;
                    debitMemo.Quantity = tmp.Returned == 0 ? tmp.Quantity : tmp.
                    Quantity - tmp.Returned;
                    debitMemo.UOM = tmp.UOM;
                    // Correct cost when discount in receipt is given
                    if (tmp.LineDiscount > 0m)
                    {
                        decimal ccost = tmp.Total / tmp.Quantity;
                        debitMemo.Cost = ccost;
                    }
                    else
                    {
                        debitMemo.Cost = tmp.Cost;
                    }
                    debitMemo.Factor = tmp.Factor;
                    debitMemo.BaseCost = tmp.BaseCost;
                    debitMemo.Warehouse = tmp.Warehouse;
                    debitMemo.ReceiptDetailID = tmp;
                    debitMemo.Save();
                }
            }
        }
    }
}
