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
    public partial class GetReceiptFuelCreditsAndPayments : ViewController
    {
        private SimpleAction getReceiptFuelCreditsAndPayments;
        private FuelStatementOfAccount _fuelStatement;
        public GetReceiptFuelCreditsAndPayments()
        {
            this.TargetObjectType = typeof(FuelStatementOfAccount);
            this.TargetViewType = ViewType.DetailView;
            string actionID =
            "GetReceiptFuelCreditsAndPayments";
            this.getReceiptFuelCreditsAndPayments = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.getReceiptFuelCreditsAndPayments.Caption = "Get Credits and Payments";
            this.getReceiptFuelCreditsAndPayments.Execute += new SimpleActionExecuteEventHandler(getReceiptFuelCreditsAndPayments_Execute);
        }

        void getReceiptFuelCreditsAndPayments_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _fuelStatement = ((DevExpress.ExpressApp.DetailView)this.
            View).CurrentObject as FuelStatementOfAccount;
            for (int i = _fuelStatement.FuelStatementPayments.Count - 1; i >= 0; i--)
            {
                if (!_fuelStatement.FuelStatementPayments[i].Select)
                {
                    _fuelStatement.FuelStatementPayments[i].Delete();
                }
            }
            _fuelStatement.Save();

            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;

            #region Populate Payments Grid

            ICollection paymentReceived;
            DevExpress.Xpo.Metadata.XPClassInfo paymentReceivedClass;
            paymentReceivedClass = _fuelStatement.Session.GetClassInfo(typeof(ReceivePayment));
            criteria = CriteriaOperator.Parse("[Approved] = True And [ReceiveFrom.No] = '" + _fuelStatement.Customer.No + "' And Not [Status] In ('Current')");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("SourceNo", DevExpress.Xpo.DB.SortingDirection.Ascending));
            paymentReceived = _fuelStatement.Session.GetObjects(paymentReceivedClass, criteria, sortProps, 0, false, true);

            foreach (ReceivePayment item in paymentReceived)
            {
                if (item.CheckAmount != item.Adjusted)
                {
                    FuelStatementPayment fuelCreditExist = _fuelStatement.Session.FindObject<FuelStatementPayment>(BinaryOperator.Parse("[SourceID]=?", item.Oid));
                    if (fuelCreditExist == null)
                    {
                        FuelStatementPayment fuelCredit = new FuelStatementPayment(
                            _fuelStatement.Session);
                        fuelCredit.FuelStatementOfAccountID = _fuelStatement;
                        fuelCredit.SourceType = item.SourceType;
                        fuelCredit.SourceNo = item.SourceNo;
                        fuelCredit.SourceID = item.Oid;
                        fuelCredit.Date = item.EntryDate;
                        fuelCredit.Transaction = item.Memo;
                        fuelCredit.Payment = item.CheckAmount;
                        fuelCredit.AdjustNow = item.OpenAmount == 0 ? item.
                        CheckAmount : item.OpenAmount;
                        fuelCredit.OpenAmount = item.OpenAmount == 0 ? item.
                        CheckAmount : item.OpenAmount;
                        fuelCredit.Save();
                    }
                }
            }
            #endregion
        }
    }
}
