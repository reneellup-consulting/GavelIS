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
    public class GensetStanfilcoReportParam : ReportParametersObjectBase
    {
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
        public GensetStanfilcoReportParam(Session session) : base(session) { }
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
            StringBuilder sbCrit = new StringBuilder();
            sbCrit.AppendFormat("[ExactEntryDate] >= #{0}# And [ExactEntryDateEnd] <= #{1}# And [Hustling] <> True And [GensetNo] Is Not Null", _Start.ToString("yyyy-MM-dd"), _End.ToString("yyyy-MM-dd"));
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
    }

}
