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
    public class DailyTotalPurchasesReportParam : ReportParametersObjectBase
    {
        public DailyTotalPurchasesReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            if (_Start == DateTime.MinValue || _End == DateTime.MinValue)
            {
                throw new UserFriendlyException("Invalid starting date or ending date value");
            }
            if (_Start > _End)
            {
                throw new UserFriendlyException("Starting date cannot be ahead of ending date");
            }
            StringBuilder sbCrit = new StringBuilder();
            DateTime oStartDate = new DateTime(_Start.Year, _Start.Month, _Start.Day, 0, 0, 0);
            DateTime oEndDate = new DateTime(_End.Year, _End.Month, _End.Day, 23, 59, 59);
            sbCrit.AppendFormat("[EntryDate] >= #{0}# And [EntryDate] <= #{1}#", oStartDate.ToString("yyyy-MM-dd"), oEndDate.ToString("yyyy-MM-dd"));
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
        private DateTime _Start;
        private DateTime _End;

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
