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
    public class PeriodicRequisitionReport : ReportParametersObjectBase
    {
        private int _Year = DateTime.Now.Year;
        private bool _IncludeCancelled;
        public PeriodicRequisitionReport(Session session) : base(session) { }
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
            StringBuilder sbCrit = new StringBuilder();
            if (!_IncludeCancelled)
            {
                sbCrit.AppendFormat("[RequisitionInfo.EntryDate] >= #{0}# And [RequisitionInfo.EntryDate] <= #{1}#  And [Cancelled] <> True And [RequisitionInfo.Status] In ('Approved','Served') And [Status] In ('Active', 'Completed') And "
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            }
            else
            {
                sbCrit.AppendFormat("[RequisitionInfo.EntryDate] >= #{0}# And [RequisitionInfo.EntryDate] <= #{1}#  And [RequisitionInfo.Status] In ('Current', 'Approved', 'Cancelled', 'Served') And "
                , startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            }
            StringBuilder sbType = new StringBuilder("[ItemNo.ItemType] In (");
            if (PdReqReportItemSelections.Count > 0)
            {
                // [Item No.Item Type] In ('Inventory Item', 'Service Item', 'Fuel Item', 'Repair & Maintenance', 'Tire Item')
                foreach (var item in PdReqReportItemSelections)
                {
                    sbType.AppendFormat("'{0}',", EnumExtensions.GetDisplayName(item.TypeOfItem.TypeOfItem));
                }
                sbType.Remove(sbType.Length - 1, 1);
                sbCrit.Append(sbType.ToString() + ")");
            }
            else
            {
                sbCrit.Remove(sbCrit.Length - 5, 5);
            }
            return !string.IsNullOrEmpty(sbCrit.ToString()) ? CriteriaOperator.Parse(sbCrit.ToString()) : CriteriaOperator.Parse("");
        }
        public override SortingCollection GetSorting()
        {
            SortingCollection sorting = new SortingCollection();
            return sorting;
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
        [DisplayName("Include cancelled items")]
        public bool IncludeCancelled
        {
            get { return _IncludeCancelled; }
            set
            {
                if (_IncludeCancelled == value)
                    return;
                _IncludeCancelled = value;
            }
        }
        [Aggregated,Association("PdReqReportItemSelections")]
        public XPCollection<PdReqReportItemSelection> PdReqReportItemSelections
        {
            get
            {
                return
                    GetCollection<PdReqReportItemSelection>("PdReqReportItemSelections");
            }
        }
    }

}
