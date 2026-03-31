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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GetShuntingCreditsAndPayments : ViewController
    {
        private SimpleAction getShuntingCreditsAndPayments;
        private ShuntingStatement _shuntingStatement;
        public GetShuntingCreditsAndPayments()
        {
            this.TargetObjectType = typeof(ShuntingStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID =
            "ShuntingStatement.GetOtherTripCreditsAndPayments";
            this.getShuntingCreditsAndPayments = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getShuntingCreditsAndPayments.Caption="Get Credits and Payments";
            this.getShuntingCreditsAndPayments.Execute += new 
            SimpleActionExecuteEventHandler(GetOtherTripCreditsAndPayments_Execute
            );
        }
        private void GetOtherTripCreditsAndPayments_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _shuntingStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as ShuntingStatement;
            for (int i = _shuntingStatement.ShuntingPayments.Count - 1; i >= 0; i--) 
            {_shuntingStatement.ShuntingPayments[i].Delete();}
            _shuntingStatement.Save();
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
            paymentReceivedClass = _shuntingStatement.Session.GetClassInfo(
            typeof(ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + 
            _shuntingStatement.Customer.No + 
            "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            paymentReceived = _shuntingStatement.Session.GetObjects(
            paymentReceivedClass, criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in paymentReceived) {
                if (item.CheckAmount != item.Adjusted) {
                    ShuntingPayment otherCredit = new ShuntingPayment(
                    _shuntingStatement.Session);
                    otherCredit.ShuntingStatementID = _shuntingStatement;
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
