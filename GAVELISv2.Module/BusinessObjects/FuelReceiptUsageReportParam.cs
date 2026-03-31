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
    public enum TaggedUntaggedEnum
    {
        None,
        Tagged,
        [DisplayName("Un-tagged")]
        Untagged
    }

    public class FuelReceiptUsageReportParam : ReportParametersObjectBase
    {
        private int _Year = DateTime.Now.Year;
        public FuelReceiptUsageReportParam(Session session) : base(session) { }
        public override CriteriaOperator GetCriteria()
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            switch (ReportType)
            {
                case GtpGenerationTypeEnum.Monthly:
                    startDate = new DateTime(Year, (int)Month, 1);
                    endDate = (new DateTime(Year, (int)Month, DateTime.DaysInMonth(Year, (int)Month))).AddDays(1);
                    break;
                case GtpGenerationTypeEnum.Range:
                    startDate = StartDate;
                    endDate = EndDate.AddDays(1);
                    break;
                default:
                    break;
            }
            StringBuilder sbCrit = new StringBuilder();
            // [Entry Date] >= #2019-03-01# And [Entry Date] < #2019-04-01#
            switch (Selection)
            {
                case TaggedUntaggedEnum.None:
                    sbCrit.AppendFormat("[EntryDate] >= #{0}# And [EntryDate] <= #{1}#"
                        , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                    break;
                case TaggedUntaggedEnum.Tagged:
                    sbCrit.AppendFormat("[Tagged] = True And [EntryDate] >= #{0}# And [EntryDate] <= #{1}#"
                        , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                    break;
                case TaggedUntaggedEnum.Untagged:
                    sbCrit.AppendFormat("[Tagged] <> True And [EntryDate] >= #{0}# And [EntryDate] <= #{1}#"
                        , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
                    break;
                default:
                    break;
            }

            if (PerVendor && Vendor !=null)
            {
                sbCrit.AppendFormat(" And [Vendor.No] = '{0}'", Vendor.No);
            }

            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }

        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }

        [ImmediatePostData]
        public TaggedUntaggedEnum Selection { get; set; }
        [ImmediatePostData]
        public GtpGenerationTypeEnum ReportType { get; set; }
        [ImmediatePostData]
        public bool PerVendor { get; set; }
        public Vendor Vendor { get; set; }
        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
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
        public MonthsEnum Month { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Title
        {
            get
            {
                string selStr = "Unspecified";
                switch (Selection)
                {
                    case TaggedUntaggedEnum.None:
                        break;
                    case TaggedUntaggedEnum.Tagged:
                        selStr = "Tagged";
                        break;
                    case TaggedUntaggedEnum.Untagged:
                        selStr = "Un-tagged";
                        break;
                    default:
                        break;
                }

                switch (ReportType)
                {
                    case GtpGenerationTypeEnum.Monthly:
                        return string.Format("{0} Receipts For the month of {1} {2}", selStr, Month, Year);
                    case GtpGenerationTypeEnum.Range:
                        return string.Format("{0} Receipts From {1} to {2}", selStr, StartDate.ToString("MMM dd"), EndDate.ToString("MMM dd, yyy"));
                    default:
                        return string.Empty;
                }
            }
        }
    }

}
