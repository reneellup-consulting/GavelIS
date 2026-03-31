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
    public class GrossTaxableSalesReportParam : ReportParametersObjectBase
    {
        public GrossTaxableSalesReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            var firstDayOfMonth = new DateTime(Year, (int)FromMonth, 1);
            var lastDayOfMonth = new DateTime(Year, (int)ToMonth, 1).AddMonths(1).AddDays(-1);
            DateTime startDate = firstDayOfMonth;
            DateTime endDate = lastDayOfMonth;
            if (startDate > endDate)
            {
                throw new UserFriendlyException("Ending month cannot be ahead of the starting month");
            }
            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}#  And [InvoiceType] In ('Cash', 'Charge')"
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
        private int _Year = DateTime.Now.Year;
        private MonthsEnum _FromMonth;
        private MonthsEnum _ToMonth;
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        public MonthsEnum FromMonth
        {
            get { return _FromMonth; }
            set { SetPropertyValue("FromMonth", ref _FromMonth, value); }
        }
        public MonthsEnum ToMonth
        {
            get { return _ToMonth; }
            set { SetPropertyValue("ToMonth", ref _ToMonth, value); }
        }
    }
}
