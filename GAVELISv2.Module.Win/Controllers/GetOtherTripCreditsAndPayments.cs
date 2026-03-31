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
    public partial class GetOtherTripCreditsAndPayments : ViewController
    {
        private SimpleAction getOtherTripCreditsAndPayments;
        private OtherTripStatement _otherTripStatement;
        public GetOtherTripCreditsAndPayments()
        {
            this.TargetObjectType = typeof(OtherTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID =
            "OtherTripStatement.GetOtherTripCreditsAndPayments";
            this.getOtherTripCreditsAndPayments = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getOtherTripCreditsAndPayments.Caption="Get Credits and Payments";
            this.getOtherTripCreditsAndPayments.Execute += new 
            SimpleActionExecuteEventHandler(GetOtherTripCreditsAndPayments_Execute
            );
        }
        private void GetOtherTripCreditsAndPayments_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _otherTripStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as OtherTripStatement;
            for (int i = _otherTripStatement.OtherTripPayments.Count - 1; i >= 0; i--) 
            {_otherTripStatement.OtherTripPayments[i].Delete();}
            _otherTripStatement.Save();
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
            paymentReceivedClass = _otherTripStatement.Session.GetClassInfo(
            typeof(ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + 
            _otherTripStatement.Customer.No + 
            "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            paymentReceived = _otherTripStatement.Session.GetObjects(
            paymentReceivedClass, criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in paymentReceived) {
                if (item.CheckAmount != item.Adjusted) {
                    OtherTripPayment otherCredit = new OtherTripPayment(
                    _otherTripStatement.Session);
                    otherCredit.OtherTripStatementID = _otherTripStatement;
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
