using System;
using System.ComponentModel;
using System.Collections;
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
    public partial class GetChargesAndCreditsAction : ViewController {
        private SimpleAction getChargesAndCreditsAction;
        private PayBill _PayBill;
        public GetChargesAndCreditsAction() {
            this.TargetObjectType = typeof(PayBill);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "PayBill.GetChargesAndCredits";
            this.getChargesAndCreditsAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getChargesAndCreditsAction.Execute += new 
            SimpleActionExecuteEventHandler(GetChargesAndCreditsAction_Execute);
        }
        private void GetChargesAndCreditsAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _PayBill = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PayBill;
            try {
                for (int i = _PayBill.Charges.Count - 1; i >= 0; i--) {_PayBill.
                    Charges[i].Delete();}
                for (int i = _PayBill.Credits.Count - 1; i >= 0; i--) {_PayBill.
                    Credits[i].Delete();}
                //_PayBill.Charges.Reload();
                //_PayBill.Credits.Reload();
            } catch (Exception) {}
            #region Populate Charges Grid
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
            #region Receipts
            ICollection receipts;
            DevExpress.Xpo.Metadata.XPClassInfo receiptClass;
            receiptClass = _PayBill.Session.GetClassInfo(typeof(Receipt));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Vendor.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current', 'Paid')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            receipts = _PayBill.Session.GetObjects(receiptClass, criteria, 
            sortProps, 0, false, true);
            foreach (Receipt item in receipts) {
                PayBillExistingCharge pbCharge = new PayBillExistingCharge(
                _PayBill.Session);
                pbCharge.PayBillID = _PayBill;
                pbCharge.SourceType = item.SourceType;
                pbCharge.SourceNo = item.SourceNo;
                pbCharge.SourceID = item.Oid;
                pbCharge.RefNo = item.InvoiceNo;
                pbCharge.Date = item.EntryDate;
                pbCharge.Transaction = item.Memo;
                pbCharge.Charges = item.Total.Value;
                pbCharge.Adjust = item.Total.Value - item.Adjusted;
                pbCharge.Terms = item.Terms != null ? item.Terms : null;
                pbCharge.Adjusted = item.Adjusted;
                pbCharge.Save();
            }
            #endregion
            #region Receipts Fuel
            ICollection receiptsFuel;
            DevExpress.Xpo.Metadata.XPClassInfo receiptFuelClass;
            receiptFuelClass = _PayBill.Session.GetClassInfo(typeof(ReceiptFuel)
            );
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Vendor.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current', 'Paid')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            receiptsFuel = _PayBill.Session.GetObjects(receiptFuelClass, 
            criteria, sortProps, 0, false, true);
            foreach (ReceiptFuel item in receiptsFuel) {
                PayBillExistingCharge pbCharge = new PayBillExistingCharge(
                _PayBill.Session);
                pbCharge.PayBillID = _PayBill;
                pbCharge.SourceType = item.SourceType;
                pbCharge.SourceNo = item.SourceNo;
                pbCharge.SourceID = item.Oid;
                pbCharge.RefNo = item.InvoiceNo;
                pbCharge.Date = item.EntryDate;
                pbCharge.Transaction = item.Memo;
                pbCharge.Charges = item.Total.Value;
                pbCharge.Adjust = item.Total.Value - item.Adjusted;
                pbCharge.Terms = item.Terms != null ? item.Terms : null;
                pbCharge.Adjusted = item.Adjusted;
                pbCharge.Save();
            }
            #endregion
            #region Bills
            ICollection bills;
            DevExpress.Xpo.Metadata.XPClassInfo billClass;
            billClass = _PayBill.Session.GetClassInfo(typeof(Bill));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Vendor.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current', 'Paid')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, _PayBill.Session.TypesManager);
            bills = _PayBill.Session.GetObjects(billClass, criteria, sortProps, 
            0, false, true);
            foreach (Bill item in bills) {
                PayBillExistingCharge pbCharge = new PayBillExistingCharge(
                _PayBill.Session);
                pbCharge.PayBillID = _PayBill;
                pbCharge.SourceType = item.SourceType;
                pbCharge.SourceNo = item.SourceNo;
                pbCharge.SourceID = item.Oid;
                pbCharge.RefNo = item.ReferenceNo;
                pbCharge.Date = item.EntryDate;
                pbCharge.Transaction = item.Memo;
                pbCharge.Charges = item.Total.Value;
                pbCharge.Adjust = item.Total.Value - item.Adjusted;
                pbCharge.Terms = item.Terms != null ? item.Terms : null;
                pbCharge.Adjusted = item.Adjusted;
                pbCharge.Save();
            }
            #endregion
            #region Job Orders
            ICollection jobs;
            DevExpress.Xpo.Metadata.XPClassInfo jobClass;
            jobClass = _PayBill.Session.GetClassInfo(typeof(JobOrder));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Vendor.No] = '" + _PayBill.Vendor.No +
            "' And Not [Status] In ('Current', 'Paid')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, _PayBill.Session.TypesManager);
            jobs = _PayBill.Session.GetObjects(jobClass, criteria, sortProps,
            0, false, true);
            foreach (JobOrder item in jobs)
            {
                PayBillExistingCharge pbCharge = new PayBillExistingCharge(
                _PayBill.Session);
                pbCharge.PayBillID = _PayBill;
                pbCharge.SourceType = item.SourceType;
                pbCharge.SourceNo = item.SourceNo;
                pbCharge.SourceID = item.Oid;
                pbCharge.RefNo = item.InvoiceNo;
                pbCharge.Date = item.EntryDate;
                pbCharge.Transaction = item.Memo;
                pbCharge.Charges = item.Total.Value;
                pbCharge.Adjust = item.Total.Value - item.Adjusted;
                pbCharge.Terms = item.Terms != null ? item.Terms : null;
                pbCharge.Adjusted = item.Adjusted;
                pbCharge.Save();
            }
            #endregion
            #region Received Vendor Payments from Returns
            ICollection receivePayments;
            DevExpress.Xpo.Metadata.XPClassInfo receivePaymentClass;
            receivePaymentClass = _PayBill.Session.GetClassInfo(typeof(
            ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            receivePayments = _PayBill.Session.GetObjects(receivePaymentClass, 
            criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in receivePayments) {
                if (item.CheckAmount != item.Adjusted) {
                    PayBillExistingCharge pbCharge = new PayBillExistingCharge(
                    _PayBill.Session);
                    pbCharge.PayBillID = _PayBill;
                    pbCharge.SourceType = item.SourceType;
                    pbCharge.SourceNo = item.SourceNo;
                    pbCharge.SourceID = item.Oid;
                    pbCharge.RefNo = item.CheckNo;
                    pbCharge.Date = item.EntryDate;
                    pbCharge.Transaction = item.Memo;
                    pbCharge.Charges = item.CheckAmount;
                    pbCharge.Adjust = item.CheckAmount - item.Adjusted;
                    pbCharge.Adjusted = item.Adjusted;
                    //pbCharge.Withheld = item.Withheld;
                    pbCharge.Save();
                }
            }
            #endregion
            #endregion
            #region Populate Payments Grid
            #region Payments Made
            ICollection checkPayments;
            DevExpress.Xpo.Metadata.XPClassInfo checkPaymentClass;
            checkPaymentClass = _PayBill.Session.GetClassInfo(typeof(
            CheckPayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [PayToOrder.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current', 'Voided')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            checkPayments = _PayBill.Session.GetObjects(checkPaymentClass, 
            criteria, sortProps, 0, false, true);
            foreach (CheckPayment item in checkPayments) {
                if (item.CheckAmount != item.Adjusted) {
                    PayBillExistingCredit pbCredit = new PayBillExistingCredit(
                    _PayBill.Session);
                    pbCredit.PayBillID = _PayBill;
                    pbCredit.SourceType = item.SourceType;
                    pbCredit.OperationType = item.OperationType;
                    pbCredit.SourceNo = item.SourceNo;
                    pbCredit.RefNo = item.CheckNo;
                    pbCredit.SourceID = item.Oid;
                    pbCredit.Date = item.EntryDate;
                    pbCredit.Transaction = item.Memo;
                    pbCredit.Payment = item.CheckAmount;
                    pbCredit.AdjustNow = item.CheckAmount - item.Adjusted;
                    pbCredit.Adjusted = item.Adjusted;
                    pbCredit.Save();
                }
            }
            checkPaymentClass = _PayBill.Session.GetClassInfo(typeof(
            CheckVoucher));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [PayToOrder.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current', 'Voided')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            checkPayments = _PayBill.Session.GetObjects(checkPaymentClass, 
            criteria, sortProps, 0, false, true);
            foreach (CheckVoucher item in checkPayments) {
                if (item.CheckAmount != item.Adjusted) {
                    PayBillExistingCredit pbCredit = new PayBillExistingCredit(
                    _PayBill.Session);
                    pbCredit.PayBillID = _PayBill;
                    pbCredit.SourceType = item.SourceType;
                    pbCredit.OperationType = item.OperationType;
                    pbCredit.SourceNo = item.SourceNo;
                    pbCredit.SourceID = item.Oid;
                    pbCredit.RefNo = item.CheckNo;
                    pbCredit.Date = item.EntryDate;
                    pbCredit.Transaction = item.Memo;
                    pbCredit.Payment = item.CheckAmount.Value;
                    pbCredit.AdjustNow = item.CheckAmount.Value - item.Adjusted;
                    pbCredit.Adjusted = item.Adjusted;
                    pbCredit.Save();
                }
            }
            #endregion
            #region Vendor Returns
            ICollection debitMemos;
            DevExpress.Xpo.Metadata.XPClassInfo debitMemoClass;
            debitMemoClass = _PayBill.Session.GetClassInfo(typeof(DebitMemo));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Vendor.No] = '" + _PayBill.Vendor.No + 
            "' And Not [Status] In ('Current', 'Applied')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            debitMemos = _PayBill.Session.GetObjects(debitMemoClass, criteria, 
            sortProps, 0, false, true);
            foreach (DebitMemo item in debitMemos) {
                PayBillExistingCredit pbCredit = new PayBillExistingCredit(
                _PayBill.Session);
                pbCredit.PayBillID = _PayBill;
                pbCredit.SourceType = item.SourceType;
                pbCredit.SourceNo = item.SourceNo;
                pbCredit.SourceID = item.Oid;
                pbCredit.RefNo = item.ReferenceNo;
                pbCredit.Date = item.EntryDate;
                pbCredit.Transaction = item.Memo;
                pbCredit.Payment = item.Total.Value;
                pbCredit.AdjustNow = item.Total.Value - item.Adjusted;
                pbCredit.Adjusted = item.Adjusted;
                pbCredit.Save();
            }
            #endregion
            #endregion
        }
    }
}
