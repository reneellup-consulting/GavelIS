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
    public class IncomePerDriverYearlyReportParam : ReportParametersObjectBase
    {
        private Customer _TripCustomer;
        private int _Year = DateTime.Now.Year;
        private bool _AllCustomer = false;
        public IncomePerDriverYearlyReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (!_AllCustomer && _TripCustomer == null)
            {
                throw new UserFriendlyException("Customer not yet provided");
            }
            StringBuilder sbCrit = new StringBuilder();
            if (_AllCustomer)
            {
                sbCrit.AppendFormat("[Year] = {0}", _Year);
            }
            else
            {
                sbCrit.AppendFormat("[TripID.TripCustomer.No] = '{0}' And [Year] = {1}", TripCustomer.No, _Year);
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
        // Year
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
    }

}
