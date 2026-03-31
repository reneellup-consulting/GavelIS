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
    public class StanfilcoTripsPaymentDetailsParam : ReportParametersObjectBase
    {
        private int _Year = DateTime.Now.Year;
        private int _Period;
        private int _Week;
        // Year
        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get { return _Year; }
            set
            {
                if (_Year == value)
                    return;
                _Year = value;
            }
        }
        // Period
        [Custom("DisplayFormat", "d")]
        public int Period
        {
            get { return _Period; }
            set
            {
                if (_Period == value)
                    return;
                _Period = value;
            }
        }
        // Week
        [Custom("DisplayFormat", "d")]
        public int Week
        {
            get { return _Week; }
            set
            {
                if (_Week == value)
                    return;
                _Week = value;
            }
        }
        public string Title
        {
            get
            {
                return string.Format("YEAR: {0} | PERIOD: {1} | WEEK: {2}", _Year, _Period, _Week);
            }
        }
        public StanfilcoTripsPaymentDetailsParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (_Year == 0 || _Period == 0 || _Week == 0)
            {
                throw new UserFriendlyException("The Year, Period and Week cannot be zero or empty!");
            }
            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[Period] = {0} And [Week] = {1} And [Year] = {2}", _Period, _Week, _Year);
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
    }

}
