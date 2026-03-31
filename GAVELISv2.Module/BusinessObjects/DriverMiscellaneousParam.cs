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
    public class DriverMiscellaneousParam : ReportParametersObjectBase
    {
        public DriverMiscellaneousParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria() {
            //if (!_AllCustomer && _TripCustomer == null)
            //{
            //    throw new UserFriendlyException("Customer not yet provided");
            //}
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
            if (!_AllCustomer)
            {
                if (_TripCustomer != null)
                {
                    sbCrit.AppendFormat("[TripID.TripCustomer.No] = '{0}' And ", TripCustomer.No);
                }
                else
                {
                    sbCrit.AppendFormat("[TripID.TripCustomer.No] = 'TC00002' And ");
                }
            }
            else
            {
                sbCrit.AppendFormat("[TripID.TripCustomer.No] <> 'TC00002' And ");
            }
            // Date Range
            sbCrit.AppendFormat("[ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}# And ", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));

            if (_Driver!=null)
            {
                sbCrit.AppendFormat("[Driver.Code] = '{0}'", _Driver.No);
            }
            else
            {
                sbCrit.Remove(sbCrit.Length - 5, 5);
            }
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

        private Employee _Driver;
        private DateTime _Start;
        private DateTime _End;
        private Customer _TripCustomer;
        private bool _AllCustomer;
        [DisplayName("Non-Dole Customers")]
        public bool AllCustomer
        {
            get
            {
                return _AllCustomer;
            }
            set
            {
                if (_AllCustomer == value)
                    return;
                _AllCustomer = value;
            }
        }
        public Customer TripCustomer
        {
            get
            {
                return _TripCustomer;
            }
            set
            {
                if (_TripCustomer == value)
                    return;
                _TripCustomer = value;
            }
        }
        public Employee Driver
        {
            get
            {
                return _Driver;
            }
            set
            {
                if (_Driver == value)
                    return;
                _Driver = value;
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
