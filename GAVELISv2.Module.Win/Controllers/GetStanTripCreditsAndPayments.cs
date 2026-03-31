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
    public partial class GetStanTripCreditsAndPayments : ViewController
    {
        private SimpleAction getStanTripCreditsAndPayments;
        private StanfilcoTripStatement _stanfilcoTripStatement;
        public GetStanTripCreditsAndPayments()
        {
            this.TargetObjectType = typeof(StanfilcoTripStatement);
            this.TargetViewType = ViewType.DetailView;
            string actionID =
            "StanfilcoTripStatement.GetStanTripCreditsAndPayments";
            this.getStanTripCreditsAndPayments = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getStanTripCreditsAndPayments.Caption="Get Credits and Payments";
            this.getStanTripCreditsAndPayments.Execute += new 
            SimpleActionExecuteEventHandler(GetCustomerChargesAndCredits_Execute
            );
        }
        private void GetCustomerChargesAndCredits_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _stanfilcoTripStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as StanfilcoTripStatement;
            for (int i = _stanfilcoTripStatement.StanfilcoTripPayments.Count - 1; i >= 0; i--) 
            {
                if (!_stanfilcoTripStatement.StanfilcoTripPayments[i].Select)
                {
                    _stanfilcoTripStatement.StanfilcoTripPayments[i].Delete();
                }
            }
            _stanfilcoTripStatement.Save();
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
            paymentReceivedClass = _stanfilcoTripStatement.Session.GetClassInfo(
            typeof(ReceivePayment));
            criteria = CriteriaOperator.Parse(
            "[Approved] = True And [ReceiveFrom.No] = '" + 
            _stanfilcoTripStatement.Customer.No + 
            "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _PayBill.Session.TypesManager);
            paymentReceived = _stanfilcoTripStatement.Session.GetObjects(
            paymentReceivedClass, criteria, sortProps, 0, false, true);
            foreach (ReceivePayment item in paymentReceived) {
                if (item.CheckAmount != item.Adjusted) {
                    // _BioDev = _Session.FindObject<BiometricDevice>(BinaryOperator.Parse("[DeviceName]=?", cboDevice.SelectedItem));
                    StanfilcoTripPayment stanCreditExist = _stanfilcoTripStatement.Session.FindObject<StanfilcoTripPayment>(BinaryOperator.Parse("[SourceID]=?", item.Oid));
                    if (stanCreditExist == null)
                    {
                        StanfilcoTripPayment stanCredit = new StanfilcoTripPayment(
                            _stanfilcoTripStatement.Session);
                        stanCredit.StanfilcoTripStatementID = _stanfilcoTripStatement;
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
            }
            #endregion
            #endregion
        }
    }
}
