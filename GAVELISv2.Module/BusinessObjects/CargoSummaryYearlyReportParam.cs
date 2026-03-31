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
    public class CargoSummaryYearlyReportParam : ReportParametersObjectBase
    {
        private int _Year = DateTime.Now.Year;
        public CargoSummaryYearlyReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[Year] = {0}", _Year);
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse(""); ;
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
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
    }

}
