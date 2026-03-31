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
    public class IncomePerDriverPeriodicReportParam : ReportParametersObjectBase
    {
        private Customer _TripCustomer;
        //private int _Year = DateTime.Now.Year;
        private bool _AllCustomer = false;
        private DateTime _FromDate;
        private DateTime _ToDate;
        public IncomePerDriverPeriodicReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (!_AllCustomer && _TripCustomer == null)
            {
                throw new UserFriendlyException("Customer not yet provided");
            }
            // [Entry Date] >= #2022-01-01# And [Entry Date] < #2022-02-01#
            StringBuilder sbCrit = new StringBuilder();
            if (_AllCustomer)
            {
                sbCrit.AppendFormat("[TripID.EntryDate] >= #{0}# And [TripID.EntryDate] < #{1}#", _FromDate.ToShortDateString(), _ToDate.AddDays(1).ToShortDateString());
            }
            else
            {
                sbCrit.AppendFormat("[TripID.TripCustomer.No] = '{0}' And [TripID.EntryDate] >= #{1}# And [TripID.EntryDate] < #{2}#", TripCustomer.No, _FromDate.ToShortDateString(), _ToDate.AddDays(1).ToShortDateString());
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
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
        // TripCustomer
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
        // FromDate
        //public bool AllCustomer
        //{
        //    get
        //    {
        //        return _AllCustomer;
        //    }
        //    set
        //    {
        //        if (_AllCustomer == value)
        //            return;
        //        _AllCustomer = value;
        //    }
        //}
        
        // Year
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                if (_FromDate == value)
                    return;
                _FromDate = value;
            }
        }
        // ToDate
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                if (_ToDate == value)
                    return;
                _ToDate = value;
            }
        }
        // Year
        //[Custom("DisplayFormat", "d")]
        //public int Year
        //{
        //    get
        //    {
        //        return _Year;
        //    }
        //    set
        //    {
        //        if (_Year == value)
        //            return;
        //        _Year = value;
        //    }
        //}
    }

}
