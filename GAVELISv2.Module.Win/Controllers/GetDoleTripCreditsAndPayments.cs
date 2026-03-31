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
    public partial class GetDoleTripCreditsAndPayments : ViewController {
        private SimpleAction getDoleTripCreditsAndPayments;
        private DolefilTripStatement _dolefilTripStatement;
        public GetDoleTripCreditsAndPayments() {
            this.TargetObjectType = typeof(DolefilTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID = 
            "DolefilTripStatement.GetDoleTripCreditsAndPayments";
            this.getDoleTripCreditsAndPayments = new SimpleAction(this, actionID
            , PredefinedCategory.RecordEdit);
            this.getDoleTripCreditsAndPayments.Caption = 
            "Get Credits and Payments";
            this.getDoleTripCreditsAndPayments.Execute += new 
            SimpleActionExecuteEventHandler(GetCustomerChargesAndCredits_Execute
            );
        }
        private void GetCustomerChargesAndCredits_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _dolefilTripStatement = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as DolefilTripStatement;
            for (int i = _dolefilTripStatement.DolefilTripPayments.Count - 1; i 
            >= 0; i--) {_dolefilTripStatement.DolefilTripPayments[i].Delete();}
            _dolefilTripStatement.Save();
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
            paymentReceivedClass = _dolefilTripStatement.Session.GetClassInfo(
            typeof(ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + _dolefilTripStatement
            .Customer.No + "' And Not [Status] In ('Current','Voided')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            paymentReceived = _dolefilTripStatement.Session.GetObjects(
            paymentReceivedClass, criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in paymentReceived) {
                if (item.CheckAmount != item.Adjusted) {
                    DolefilTripPayment stanCredit = new DolefilTripPayment(
                    _dolefilTripStatement.Session);
                    stanCredit.DolefilTripStatementID = _dolefilTripStatement;
                    stanCredit.SourceType = item.SourceType;
                    stanCredit.SourceNo = item.SourceNo;
                    stanCredit.SourceID = item.Oid;
                    stanCredit.Date = item.EntryDate;
                    stanCredit.Transaction = item.Memo;
                    stanCredit.Payment = item.CheckAmount;
                    stanCredit.AdjustNow = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    stanCredit.OpenAmount = item.OpenAmount == 0 ? item.
                    CheckAmount : item.OpenAmount;
                    stanCredit.Save();
                }
            }
            #endregion
            #endregion
        }
    }
}
