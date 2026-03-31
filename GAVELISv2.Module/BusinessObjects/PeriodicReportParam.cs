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
    public class PeriodicReportParam : ReportParametersObjectBase
    {
        public PeriodicReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            //if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            //{
            //    throw new UserFriendlyException("Start and End Date cannot be empty");
            //}
            //if (_Start > _End)
            //{
            //    throw new UserFriendlyException("Start Date cannot be greater than the End Date");
            //}

            if (!_All && _Customer == null)
            {
                throw new UserFriendlyException("Customer not yet provided");
            }
            //[ExactEntryDate] >= #2014-10-01# And [ExactEntryDate] <= #2014-10-30# And [Status] <> 'Current' And [Customer.Code] = 'c003080'

            StringBuilder sbCrit = new StringBuilder();
            // Date Range
            if (_Period == 0 || _Week == 0 || _Year == 0)
            {
                sbCrit.AppendFormat("Not [Status] In ('Current', 'Paid', 'Released') And [Marked] = True And ");

            }
            else
            {
                sbCrit.AppendFormat("Not [Status] In ('Current', 'Paid', 'Released') And [Marked] = True And [Period] = {0} And [Week] = {1} And [BYear] = {2} And ", _Period, _Week, _Year);

            }
            // if only invoiced Status
            //if (_InvoicedOnly)
            //{
            //    sbCrit.Append("[Status] <> 'Current' And ");
            //}
            // if is not all customer
            if (!_All)
            {
                sbCrit.AppendFormat("[Customer.Code] = '{0}'", _Customer.No);
            }
            else
            {
                sbCrit.Remove(sbCrit.Length - 5, 5);
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private bool _All = false;
        private bool _InvoicedOnly = false;
        private Customer _Customer;
        //private DateTime _Start;
        //private DateTime _End;
        private int _Period;
        private int _Week;
        private int _Year = DateTime.Now.Year;
        public bool All
        {
            get
            {
                return _All;
            }
            set
            {
                if (_All == value)
                    return;
                _All = value;
            }
        }
        public bool InvoicedOnly
        {
            get
            {
                return _InvoicedOnly;
            }
            set
            {
                if (_InvoicedOnly == value)
                    return;
                _InvoicedOnly = value;
            }
        }

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
        public int Period
        {
            get
            {
                return _Period;
            }
            set
            {
                if (_Period == value)
                    return;
                _Period = value;
            }
        }
        public int Week
        {
            get
            {
                return _Week;
            }
            set
            {
                if (_Week == value)
                    return;
                _Week = value;
            }
        }

        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get
            {
                return _Year;
            }
            set
            {
                if (_Year == value)
                    return;
                _Year = value;
            }
        }
        //public DateTime Start
        //{
        //    get
        //    {
        //        return _Start;
        //    }
        //    set
        //    {
        //        if (_Start == value)
        //            return;
        //        _Start = value;
        //    }
        //}
        //public DateTime End
        //{
        //    get
        //    {
        //        return _End;
        //    }
        //    set
        //    {
        //        if (_End == value)
        //            return;
        //        _End = value;
        //    }
        //}

    }

}
