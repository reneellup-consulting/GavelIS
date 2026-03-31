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
    public partial class GetCustomerChargesAndCredits : ViewController {
        private SimpleAction getCustomerChargesAndCredits;
        private InvoiceReconciliation _InvoiceReconciliation;
        public GetCustomerChargesAndCredits() {
            this.TargetObjectType = typeof(InvoiceReconciliation);
            this.TargetViewType = ViewType.DetailView;
            string actionID = 
            "InvoiceReconciliation.GetCustomerChargesAndCredits";
            this.getCustomerChargesAndCredits = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getCustomerChargesAndCredits.Execute += new 
            SimpleActionExecuteEventHandler(GetCustomerChargesAndCredits_Execute
            );
        }
        private void GetCustomerChargesAndCredits_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _InvoiceReconciliation = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as InvoiceReconciliation;
            for (int i = _InvoiceReconciliation.Charges.Count - 1; i >= 0; i--) 
            {_InvoiceReconciliation.Charges[i].Delete();}
            for (int i = _InvoiceReconciliation.Payments.Count - 1; i >= 0; i--) 
            {_InvoiceReconciliation.Payments[i].Delete();}
            _InvoiceReconciliation.Save();
            try {
                _InvoiceReconciliation.Charges.Reload();
                _InvoiceReconciliation.Payments.Reload();
            } catch (Exception) {}
            #region Populate Charges Grid
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            #region Invoice - Invoice Entry for Customer
            ICollection invoices;
            DevExpress.Xpo.Metadata.XPClassInfo invoicesClass;
            invoicesClass = _InvoiceReconciliation.Session.GetClassInfo(typeof(
            Invoice));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Customer.No] = '" + _InvoiceReconciliation.
            Customer.No + "' And Not [Status] In ('Current', 'Paid')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            invoices = _InvoiceReconciliation.Session.GetObjects(invoicesClass, 
            criteria, sortProps, 0, false, true);
            foreach (Invoice item in invoices) {
                InvoiceReconCharge invCharge = new InvoiceReconCharge(
                _InvoiceReconciliation.Session);
                invCharge.InvoiceReconID = _InvoiceReconciliation;
                invCharge.SourceType = item.SourceType;
                invCharge.SourceNo = item.SourceNo;
                invCharge.SourceID = item.Oid;
                invCharge.Date = item.EntryDate;
                invCharge.Transaction = item.Memo;
                invCharge.Charges = item.GrossTotal.Value;
                invCharge.Adjust = item.OpenAmount == 0 ? item.GrossTotal.Value 
                : item.OpenAmount;
                invCharge.Tax=item.RemainingTax==0?item.TotalTax.Value:item.RemainingTax;
                invCharge.Terms = item.Terms != null ? item.Terms : null;
                invCharge.OpenAmount = item.OpenAmount == 0 ? item.GrossTotal.
                Value : item.OpenAmount;
                ;
                invCharge.Save();
            }
            #endregion
            #region Payments Made to Customer
            ICollection payments;
            DevExpress.Xpo.Metadata.XPClassInfo paymentsClass;
            paymentsClass = _InvoiceReconciliation.Session.GetClassInfo(typeof(
            CheckPayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [PayToOrder.No] = '" + _InvoiceReconciliation
            .Customer.No + "' And Not [Status] In ('Current', 'Voided')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            payments = _InvoiceReconciliation.Session.GetObjects(paymentsClass, 
            criteria, sortProps, 0, false, true);
            foreach (CheckPayment item in payments) {
                if (item.CheckAmount != item.Adjusted) {
                    InvoiceReconCharge invCharge = new InvoiceReconCharge(
                    _InvoiceReconciliation.Session);
                    invCharge.InvoiceReconID = _InvoiceReconciliation;
                    invCharge.SourceType = item.SourceType;
                    invCharge.OperationType = item.OperationType;
                    invCharge.SourceNo = item.SourceNo;
                    invCharge.SourceID = item.Oid;
                    invCharge.Date = item.EntryDate;
                    invCharge.Transaction = item.Memo;
                    invCharge.Charges = item.CheckAmount;
                    invCharge.Adjust = item.OpenAmount == 0 ? item.CheckAmount : 
                    item.OpenAmount;
                    invCharge.OpenAmount = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    invCharge.Save();
                }
            }

            paymentsClass = _InvoiceReconciliation.Session.GetClassInfo(typeof(
            CheckVoucher));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [PayToOrder.No] = '" + _InvoiceReconciliation
            .Customer.No + "' And Not [Status] In ('Current', 'Voided')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            payments = _InvoiceReconciliation.Session.GetObjects(paymentsClass,
            criteria, sortProps, 0, false, true);
            foreach (CheckVoucher item in payments)
            {
                if (item.CheckAmount != item.Adjusted)
                {
                    InvoiceReconCharge invCharge = new InvoiceReconCharge(
                    _InvoiceReconciliation.Session);
                    invCharge.InvoiceReconID = _InvoiceReconciliation;
                    invCharge.SourceType = item.SourceType;
                    invCharge.OperationType = item.OperationType;
                    invCharge.SourceNo = item.SourceNo;
                    invCharge.SourceID = item.Oid;
                    invCharge.Date = item.EntryDate;
                    invCharge.Transaction = item.Memo;
                    invCharge.Charges = item.CheckAmount.Value;
                    invCharge.Adjust = item.OpenAmount == 0 ? item.CheckAmount.Value :
                    item.OpenAmount;
                    invCharge.OpenAmount = item.OpenAmount == 0 ? item.
                    CheckAmount.Value : item.OpenAmount;
                    invCharge.Save();
                }
            }

            #endregion
            #endregion
            #region Populate Payments Grid
            #region Payment Received - Received Payment from Customer
            ICollection paymentReceived;
            DevExpress.Xpo.Metadata.XPClassInfo paymentReceivedClass;
            paymentReceivedClass = _InvoiceReconciliation.Session.GetClassInfo(
            typeof(ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + 
            _InvoiceReconciliation.Customer.No + 
            "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            paymentReceived = _InvoiceReconciliation.Session.GetObjects(
            paymentReceivedClass, criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in paymentReceived) {
                if (item.CheckAmount != item.Adjusted) {
                    InvoiceReconPayment invCredit = new InvoiceReconPayment(
                    _InvoiceReconciliation.Session);
                    invCredit.InvoiceReconID = _InvoiceReconciliation;
                    invCredit.SourceType = item.SourceType;
                    invCredit.SourceNo = item.SourceNo;
                    invCredit.SourceID = item.Oid;
                    invCredit.Date = item.EntryDate;
                    invCredit.Transaction = item.Memo;
                    invCredit.Payment = item.CheckAmount;
                    invCredit.Withheld = item.Withheld;
                    invCredit.AdjustNow = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    invCredit.OpenAmount = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    invCredit.Save();
                }
            }
            #endregion
            #region Credit Memo - Return of Goods From Customer
            ICollection creditMemos;
            DevExpress.Xpo.Metadata.XPClassInfo creditMemoClass;
            creditMemoClass = _InvoiceReconciliation.Session.GetClassInfo(typeof
            (CreditMemo));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [Customer.No] = '" + _InvoiceReconciliation.
            Customer.No + "' And Not [Status] In ('Current', 'Applied')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            creditMemos = _InvoiceReconciliation.Session.GetObjects(
            creditMemoClass, criteria, sortProps, 0, false, true);
            foreach (CreditMemo item in creditMemos) {
                InvoiceReconPayment invCredit = new InvoiceReconPayment(
                _InvoiceReconciliation.Session);
                invCredit.InvoiceReconID = _InvoiceReconciliation;
                invCredit.SourceType = item.SourceType;
                invCredit.SourceNo = item.SourceNo;
                invCredit.SourceID = item.Oid;
                invCredit.Date = item.EntryDate;
                invCredit.Transaction = item.Memo;
                invCredit.Payment = item.GrossTotal.Value;
                invCredit.TaxPayment=item.TotalTax.Value;
                invCredit.AdjustNow = item.OpenAmount == 0 ? item.GrossTotal.
                Value : item.OpenAmount;
                invCredit.OpenAmount = item.OpenAmount == 0 ? item.GrossTotal.
                Value : item.OpenAmount;
                invCredit.Save();
            }
            #endregion
            #endregion
        }
    }
}
