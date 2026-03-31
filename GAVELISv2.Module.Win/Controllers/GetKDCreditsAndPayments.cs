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
    public partial class GetKDCreditsAndPayments : ViewController {
        private SimpleAction getKDCreditsAndPayments;
        private KDStatement _kdStatement;
        public GetKDCreditsAndPayments() {
            this.TargetObjectType = typeof(KDStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "KDStatement.GetOtherTripCreditsAndPayments";
            this.getKDCreditsAndPayments = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getKDCreditsAndPayments.Caption = "Get Credits and Payments";
            this.getKDCreditsAndPayments.Execute += new 
            SimpleActionExecuteEventHandler(
            GetOtherTripCreditsAndPayments_Execute);
        }
        private void GetOtherTripCreditsAndPayments_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _kdStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as KDStatement;
            for (int i = _kdStatement.KDPayments.Count - 1; i >= 0; i--) {
                _kdStatement.KDPayments[i].Delete();}
            _kdStatement.Save();
            //try {
            //    _stanfilcoTripStatement.Charges.Reload();
            //    _stanfilcoTripStatement.Payments.Reload();
            //} catch (Exception) {}
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            #region Populate Payments Grid
            #region Payment Received - Received Payment from Customer
            ICollection paymentReceived;
            DevExpress.Xpo.Metadata.XPClassInfo paymentReceivedClass;
            paymentReceivedClass = _kdStatement.Session.GetClassInfo(typeof(
            ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + _kdStatement.Customer
            .No + "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            paymentReceived = _kdStatement.Session.GetObjects(
            paymentReceivedClass, criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in paymentReceived) {
                if (item.CheckAmount != item.Adjusted) {
                    KDPayment otherCredit = new KDPayment(_kdStatement.Session);
                    otherCredit.KDStatementID = _kdStatement;
                    otherCredit.SourceType = item.SourceType;
                    otherCredit.SourceNo = item.SourceNo;
                    otherCredit.SourceID = item.Oid;
                    otherCredit.Date = item.EntryDate;
                    otherCredit.Transaction = item.Memo;
                    otherCredit.Payment = item.CheckAmount;
                    otherCredit.AdjustNow = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    otherCredit.OpenAmount = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    otherCredit.Save();
                }
            }
            #endregion
            #endregion
        }
    }
}
