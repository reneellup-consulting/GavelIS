using System;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Reports;

namespace GAVELISv2.Module.BusinessObjects
{
    //[NonPersistent]
    public class CheckDisbursementParam : ReportParametersObjectBase {
        public CheckDisbursementParam(Session session)
            : base(session) {
        }
        public override CriteriaOperator GetCriteria() {

            //[Status] = 'Approved' And [Pay To Order] = 'V02760->RICHFARM AUTO TRACTOR' And [Bank Cash Account] = '101210::ASSETS|Current Assets|Cash on Hand & in Bank|Cash in Bank|MBTC (Auto Parts)' And [Expense Type] = '0011->IH Parts Purchases'

            if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            {
                throw new UserFriendlyException("Start and End Date cannot be empty");
            }
            if (_Start > _End)
            {
                throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            }

            //[ExactEntryDate] >= #2014-10-01# And [ExactEntryDate] <= #2014-10-30# And [Status] <> 'Current' And [Customer.Code] = 'c003080'

            StringBuilder sbCrit = new StringBuilder();
            // Date Range
            sbCrit.AppendFormat("[Status] = 'Approved' And [ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}#", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            if (_Payee!=null)
            {
                sbCrit.AppendFormat(" And [PayToOrder.No] == '{0}'", _Payee.No);
            }
            if (_BankAccount!=null)
            {
                sbCrit.AppendFormat(" And [BankCashAccount.No] == '{0}'", _BankAccount.No);
            }
            if (_ExpenseType!=null)
            {
                sbCrit.AppendFormat(" And [ExpenseType.Code] == '{0}'", _ExpenseType.Code);
            }
                // if only invoiced Status
                //if (_InvoicedOnly)
                //{
                //    sbCrit.Append("[Status] <> 'Current' And ");
                //}
                // if is not all customer
                return !string.IsNullOrEmpty(sbCrit.ToString()) ? 
                CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.
                Parse("");
        }

        public override SortingCollection GetSorting() {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private DateTime _Start;
        private DateTime _End;
        private Contact _Payee;
        private Account _BankAccount;
        private ExpenseType _ExpenseType;

        public DateTime Start {
            get { return _Start; }
            set
            {
                if (_Start == value)
                    return;
                _Start = value;
            }
        }

        public DateTime End {
            get { return _End; }
            set
            {
                if (_End == value)
                    return;
                _End = value;
            }
        }

        public Contact Payee {
            get { return _Payee; }
            set { SetPropertyValue<Contact>("Payee", ref _Payee, value); }
        }

        public Account BankAccount {
            get { return _BankAccount; }
            set { SetPropertyValue<Account>("BankAccount", ref _BankAccount, 
                value); }
        }

        public ExpenseType ExpenseType {
            get { return _ExpenseType; }
            set { SetPropertyValue<ExpenseType>("ExpenseType", ref _ExpenseType, 
                value); }
        }
    }

}
