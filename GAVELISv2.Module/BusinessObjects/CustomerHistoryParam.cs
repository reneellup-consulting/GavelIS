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
    public class CustomerHistoryParam : ReportParametersObjectBase
    {
        public CustomerHistoryParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
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
            sbCrit.AppendFormat("[InvoiceInfo.ExactEntryDate] >= #{0}# And [InvoiceInfo.ExactEntryDateEnd] <= #{1}# And ", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            sbCrit.AppendFormat("[InvoiceInfo.Customer.Code] = '{0}'", _Customer.No);
            // if only invoiced Status
            //if (_InvoicedOnly)
            //{
            //    sbCrit.Append("[Status] <> 'Current' And ");
            //}
            // if is not all customer
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private Customer _Customer;
        private DateTime _Start;
        private DateTime _End;

        public Customer Customer
        {
            get
            {
                return _Customer;
            }
            set
            {
                if (_Customer == value)
                    return;
                _Customer = value;
            }
        }
        public DateTime Start
        {
            get
            {
                return _Start;
            }
            set
            {
                if (_Start == value)
                    return;
                _Start = value;
            }
        }
        public DateTime End
        {
            get
            {
                return _End;
            }
            set
            {
                if (_End == value)
                    return;
                _End = value;
            }
        }
        
    }

}
