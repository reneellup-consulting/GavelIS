using System;

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
    public enum MonthOfTheQuarterEnum
    {
        FirstMonth,
        SecondMonth,
        ThirdMonth
    }
    //[NonPersistent]
    public class Form2307ReportParam : ReportParametersObjectBase {
        public Form2307ReportParam(Session session)
            : base(session) {
        }

        public override CriteriaOperator GetCriteria() {
            return null;
        }

        public override SortingCollection GetSorting() {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        private DateTime _FromPeriod;
        private DateTime _ToPeriod;
        private MonthOfTheQuarterEnum _MonthOfTheQuarter;

        public DateTime FromPeriod {
            get { return _FromPeriod; }
            set { SetPropertyValue<DateTime>("FromPeriod", ref _FromPeriod, value); }
        }

        public DateTime ToPeriod {
            get { return _ToPeriod; }
            set { SetPropertyValue<DateTime>("ToPeriod", ref _ToPeriod, value); }
        }

        public MonthOfTheQuarterEnum MonthOfTheQuarter {
            get { return _MonthOfTheQuarter; }
            set { SetPropertyValue<MonthOfTheQuarterEnum>("MonthOfTheQuarter", ref _MonthOfTheQuarter, value); }
        }

        public int FromMonth {
            get { return _FromPeriod.Month; }
        }

        public int FromDate {
            get { return _FromPeriod.DayOfYear; }
        }

        public string FromYear {
            get { return _FromPeriod.ToString("yy"); }
        }

        public int ToMonth {
            get { return _ToPeriod.Month; }
        }

        public int ToDate {
            get { return _ToPeriod.DayOfYear; }
        }

        public string ToYear {
            get { return _ToPeriod.ToString("yy"); }
        }
    }
}
