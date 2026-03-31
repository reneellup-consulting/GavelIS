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
    public class CheckReceivingParam : ReportParametersObjectBase
    {
        public CheckReceivingParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
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
            if (_ReceiveFrom != null)
            {
                sbCrit.AppendFormat(" And [ReceiveFrom.No] == '{0}'", _ReceiveFrom.No);
            }
            if (_IncomeType != null)
            {
                sbCrit.AppendFormat(" And [ExpenseType.Code] == '{0}'", _IncomeType.Code);
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
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private DateTime _Start;
        private DateTime _End;
        private Contact _ReceiveFrom;
        private ExpenseType _IncomeType;

        public DateTime Start
        {
            get { return _Start; }
            set
            {
                if (_Start == value)
                    return;
                _Start = value;
            }
        }

        public DateTime End
        {
            get { return _End; }
            set
            {
                if (_End == value)
                    return;
                _End = value;
            }
        }

        public Contact ReceiveFrom
        {
            get { return _ReceiveFrom; }
            set { SetPropertyValue<Contact>("ReceiveFrom", ref _ReceiveFrom, value); }
        }


        public ExpenseType IncomeType
        {
            get { return _IncomeType; }
            set
            {
                SetPropertyValue<ExpenseType>("IncomeType", ref _IncomeType,
                    value);
            }
        }

    }

}
