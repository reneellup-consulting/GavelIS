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
    public class TurnaroundTravelTimeReportParam : ReportParametersObjectBase
    {
        public TurnaroundTravelTimeReportParam(Session session) : base(session) { }
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
            // [RequisitionInfo.EntryDate] >= #2018-04-01# And [RequisitionInfo.EntryDate] < #2018-05-01#
            StanfilcoTrip sta = new StanfilcoTrip(Session);
            //sta.Driver.No
            //sta.TruckNo.No
            StringBuilder sbCrit = new StringBuilder();
            if (_Driver != null && _Unit != null)
            {
                sbCrit.AppendFormat("[StartDT] >= #{0}# And [StartDT] <= #{1}# And [Driver.No] == '{2}' And [TruckNo.No] == '{3}'"
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), _Driver.No, _Unit.No);
            }
            else if (_Driver != null && _Unit == null)
            {
                sbCrit.AppendFormat("[StartDT] >= #{0}# And [StartDT] <= #{1}# And [Driver.No] == '{2}'"
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), _Driver.No);
            }
            else if (_Driver == null && _Unit != null)
            {
                sbCrit.AppendFormat("[StartDT] >= #{0}# And [StartDT] <= #{1}# And [TruckNo.No] == '{2}'"
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), _Unit.No);
            }
            else
            {
                sbCrit.AppendFormat("[StartDT] >= #{0}# And [StartDT] <= #{1}#"
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
        }
        private int _Year = DateTime.Now.Year;
        private Employee _Driver;
        private FATruck _Unit;

        public Employee Driver
        {
            get { return _Driver; }
            set
            {
                if (_Driver == value)
                    return;
                _Driver = value;
            }
        }
        public FATruck Unit
        {
            get { return _Unit; }
            set
            {
                if (_Unit == value)
                    return;
                _Unit = value;
            }
        }
        [ImmediatePostData]
        public GtpGenerationTypeEnum ReportType { get; set; }
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
                switch (ReportType)
                {
                    case GtpGenerationTypeEnum.Monthly:
                        return string.Format("For the month of {0} {1}", Month, Year);
                    case GtpGenerationTypeEnum.Range:
                        return string.Format("From {0} to {1}", StartDate.ToString("MMM dd"), EndDate.ToString("MMM dd, yyy"));
                    default:
                        return string.Empty;
                }
            }
        }
    }

}
