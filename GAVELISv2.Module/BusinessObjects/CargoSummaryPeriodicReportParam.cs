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
    public class CargoSummaryPeriodicReportParam : ReportParametersObjectBase
    {
        private DateTime _FromDate;
        private DateTime _ToDate;
        public CargoSummaryPeriodicReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[TripID.EntryDate] Between(#{0}#, #{1}#)", _FromDate.ToShortDateString(), _ToDate.AddDays(1).ToShortDateString());
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
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
    }

}
