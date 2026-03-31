using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class AttendanceRecord : XPObject
    {
        private Guid _RowID;
        private AttendanceCalculator02 _BatchID;
        private bool _Posted = false;
        private Employee _EmployeeID;
        private EmploymentStatusEnum _EmploymentStatus;
        private EmployeeClassCode _ClassCode;
        private Positions _EmployeePosition;
        private TimeTable2 _Shift;
        private DayOfWeek _RestDay;
        private EmployeePayTypeEnum _PayType;
        private decimal _Basic;
        private decimal _Allowance;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("EmployeeTimesheets")]
        public AttendanceCalculator02 BatchID
        {
            get { return _BatchID; }
            set { SetPropertyValue("BatchID", ref _BatchID, value); }
        }
        [DisplayName("Employee")]
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee EmployeeID
        {
            get { return _EmployeeID; }
            set { SetPropertyValue("EmployeeID", ref _EmployeeID, value);
            if (!IsLoading && !IsSaving && _EmployeeID != null)
            {
                EmploymentStatus = _EmployeeID.Status;
                ClassCode = _EmployeeID.EmployeeClassCode ?? null;
                EmployeePosition = _EmployeeID.Position ?? null;
                RestDay = _EmployeeID.RestDay;
                PayType = _EmployeeID.PayType;
                switch (_EmployeeID.PayType)
                {
                    case EmployeePayTypeEnum.Hourly:
                        Basic = _EmployeeID.Basic * _EmployeeID.Shift.CountAsHours;
                        break;
                    case EmployeePayTypeEnum.Daily:
                        Basic = _EmployeeID.Basic;
                        break;
                    case EmployeePayTypeEnum.Monthly:
                        Basic = (_EmployeeID.Basic / WorkingDays.GetWorkingDays(_BatchID.TimeRangeFrom, _BatchID.TimeRangeTo, new List<DayOfWeek> { _EmployeeID.RestDay }));
                        break;
                    default:
                        break;
                }
                //Basic = _EmployeeID.Basic;
                Allowance = _EmployeeID.Allowance;
            }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool Posted
        {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
        }
        [DisplayName("Status")]
        [Custom("AllowEdit", "False")]
        public EmploymentStatusEnum EmploymentStatus
        {
            get { return _EmploymentStatus; }
            set { SetPropertyValue("EmploymentStatus", ref _EmploymentStatus, value); }
        }
        [Custom("AllowEdit", "False")]
        public EmployeeClassCode ClassCode
        {
            get { return _ClassCode; }
            set { SetPropertyValue("ClassCode", ref _ClassCode, value); }
        }
        [DisplayName("Position")]
        [Custom("AllowEdit", "False")]
        public Positions EmployeePosition
        {
            get { return _EmployeePosition; }
            set { SetPropertyValue("EmployeePosition", ref _EmployeePosition, value); }
        }
        // NightDiffRate
        [DisplayName("ND Rate(%)")]
        [Custom("AllowEdit", "False")]
        public decimal NightDiffRate
        {
            get { return _NightDiffRate; }
            set { SetPropertyValue("NightDiffRate", ref _NightDiffRate, value); }
        }
        // NormalOtRate
        [DisplayName("NOT Rate(%)")]
        [Custom("AllowEdit", "False")]
        public decimal NormalOtRate
        {
            get { return _NormalOtRate; }
            set { SetPropertyValue("NormalOtRate", ref _NormalOtRate, value); }
        }
        // RestdayRate
        [DisplayName("RD Rate(%)")]
        [Custom("AllowEdit", "False")]
        public decimal RestdayRate
        {
            get { return _RestdayRate; }
            set { SetPropertyValue("RestdayRate", ref _RestdayRate, value); }
        }
        // RestdayOtRate
        [DisplayName("RDO Rate(%)")]
        [Custom("AllowEdit", "False")]
        public decimal RestdayOtRate
        {
            get { return _RestdayOtRate; }
            set { SetPropertyValue("RestdayOtRate", ref _RestdayOtRate, value); }
        }
        [Action(Caption = "Restore", AutoCommit=true)]
        public void ValidateLine() {
            if (_Shift != null)
            {
                NightDiffRate = _Shift.NightDiffRate;
                NormalOtRate = _Shift.NormalOtRate;
                RestdayRate = _Shift.RestdayRate;
                RestdayOtRate = _Shift.RestdayOtRate;
            }
            foreach (var item in DailyTimeRecords)
            {
                if (item.TimeTable != null)
                {
                    item.NightDiffRate = item.TimeTable.NightDiffRate;
                    item.NormalOtRate = item.TimeTable.NormalOtRate;
                    item.RestdayRate = item.TimeTable.RestdayRate;
                    item.RestdayOtRate = item.TimeTable.RestdayOtRate;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        public TimeTable2 Shift
        {
            get { return _Shift; }
            set
            {
                SetPropertyValue("Shift", ref _Shift, value);
                if (!IsLoading && !IsSaving)
                {
                    NightDiffRate = _Shift.NightDiffRate;
                    NormalOtRate = _Shift.NormalOtRate;
                    RestdayRate = _Shift.RestdayRate;
                    RestdayOtRate = _Shift.RestdayOtRate;
                }
            }
        }

        //[Custom("AllowEdit", "False")]
        public DayOfWeek RestDay
        {
            get { return _RestDay; }
            set { SetPropertyValue("RestDay", ref _RestDay, value); }
        }
        [Custom("AllowEdit", "False")]
        public EmployeePayTypeEnum PayType
        {
            get { return _PayType; }
            set { SetPropertyValue("PayType", ref _PayType, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal Basic
        {
            get { return _Basic; }
            set { SetPropertyValue("Basic", ref _Basic, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal Allowance
        {
            get { return _Allowance; }
            set { SetPropertyValue("Allowance", ref _Allowance, value); }
        }
        [Association, Aggregated]
        public XPCollection<CheckInAndOut03> DailyTimeRecords
        {
            get { return GetCollection<CheckInAndOut03>("DailyTimeRecords"); }
        }
        // Summary Column Caption 3
        public string Day3Caption01
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday" || data.Halfday)
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption02
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(1)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption03
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(2)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption04
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(3)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption05
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(4)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption06
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(5)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption07
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(6)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption08
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(7)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption09
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(8)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption10
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(9)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption11
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(10)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption12
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(11)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption13
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(12)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption14
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(13)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption15
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(14)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return "E";
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day3Caption16
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(15)).LastOrDefault();
                if (data != null)
                {
                    if (data.AbsentHrs != 0)
                    {
                        return "A";
                    }
                    else if (new[] { "Rest Day", "Rest Day OT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return "RD";
                    }
                    else if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => data.Remarks.Contains(o)) && data.BasicHrs == 0)
                    {
                        return Convert.ToInt16(data.TimeTable.CountAsHours).ToString();
                    }
                    else if (data.Remarks == "Halfday")
                    {
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.FirstHalf.ToString();
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            return data.TimeTable.SecondHalf.ToString();
                        }
                        else
                        {
                            return string.Empty;
                        }
                        //return Convert.ToInt16(data.BasicHrs - (data.LateHrs + data.UndertimeHrs)).ToString();
                    }
                    else
                    {
                        return Convert.ToInt16(data.BasicHrs).ToString();
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        // Summary Column Caption 4
        public string Day4Caption01
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption02
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(1)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption03
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(2)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption04
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(3)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption05
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(4)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption06
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(5)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption07
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(6)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption08
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(7)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption09
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(8)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption10
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(9)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption11
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(10)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption12
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(11)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption13
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(12)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption14
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(13)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption15
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(14)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day4Caption16
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(15)).LastOrDefault();
                if (data != null)
                {
                    if (new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday SPOT" }.Any(o => data.Remarks.Contains(o)) && data.OtStatus == OtStatusEnum.Approved)
                    {
                        TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2));
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        return str;
                        //return (data.OvertimeHrs + data.RestdayOtHrs + data.HolidayOTHrs2).ToString("n2");
                    }
                    else
                    {
                        return "E";
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        // Summary Column Caption 5
        public string Day5Caption01
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption02
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(1)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption03
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(2)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption04
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(3)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption05
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(4)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption06
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(5)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption07
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(6)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption08
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(7)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption09
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(8)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption10
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(9)).LastOrDefault();
                if (data != null)
                {
                    if (data.LineID == "10018 08-18-2018 Dayshift")
                    {
                        
                    }
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption11
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(10)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption12
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(11)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption13
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(12)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption14
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(13)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption15
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(14)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return "E";
                }
            }
        }
        public string Day5Caption16
        {
            get
            {
                var data = DailyTimeRecords.Where(o => o.Date == _BatchID.TimeRangeFrom.AddDays(15)).LastOrDefault();
                if (data != null)
                {
                    if (!new[] { "Absent", "Rest Day", "Holiday RGOT" }.Any(o => o.Contains(data.Remarks)))
                    {
                        TimeSpan ts = new TimeSpan();
                        if (data.FirstSetHrs.TotalHours == 0)
                        {
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = sl + se;
                        }
                        else if (data.SecondSetHrs.TotalHours == 0)
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            ts = fl + fe;
                        }
                        else
                        {
                            TimeSpan fl = data.FirstLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.FirstLate : new TimeSpan();
                            TimeSpan fe = data.FirstEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.FirstEarly : new TimeSpan();
                            TimeSpan sl = data.SecondLate.TotalMinutes >= data.TimeTable.LateTimeMins + 1 ? data.SecondLate : new TimeSpan();
                            TimeSpan se = data.SecondEarly.TotalMinutes >= data.TimeTable.LeaveEarlyTimeMins + 1 ? data.SecondEarly : new TimeSpan();
                            ts = fl + fe + sl + se;
                        }
                        string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                        if (str == "0:0:0")
                        {
                            str = string.Empty;
                        }
                        return str;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        [Persistent("NoOfDays")]
        private decimal? _NoOfDays = null;
        [PersistentAlias("_NoOfDays")]
        public decimal? NoOfDays
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _NoOfDays == null)
                    UpdateNoOfDays(false);
                return _NoOfDays;
            }
        }
        public void UpdateNoOfDays(bool forceChangeEvents)
        {
            decimal? oldNoOfDays = _NoOfDays;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                //if (new[] { "Absent", "Rest Day", "Rest Day OT" }.Any(o => o.Contains(detail.Remarks)) && !detail.Halfday)
                //{
                //    tempTotal += 0m;
                //}
                //else if (detail.Remarks == "Halfday" || detail.Halfday)
                //{
                //    tempTotal += 0.5m;
                //} else
                //{
                //    tempTotal += 1m;
                //}
                if (new[] { "Absent", "Rest Day", "Rest Day OT" }.Any(o => o.Contains(detail.Remarks)))
                {
                    tempTotal += 0m;
                }
                else if (detail.Remarks == "Halfday" || detail.Halfday)
                {
                    tempTotal += 0.5m;
                }
                else
                {
                    tempTotal += 1m;
                }
            }
                //tempTotal += detail.BasicHrs / detail.TimeTable.CountAsHours;
            if (tempTotal > 0m)
            {
                _NoOfDays = tempTotal;
            }
            else
            {
                _NoOfDays = null;
            }
            if (forceChangeEvents)
                OnChanged("NoOfDays", oldNoOfDays, _NoOfDays);
        }

        // AllowanceTot
        [Persistent("AllowanceTot")]
        private decimal? _AllowanceTot = null;
        [PersistentAlias("_AllowanceTot")]
        public decimal? AllowanceTot
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _AllowanceTot == null)
                    UpdateAllowanceTot(false);
                return _AllowanceTot;
            }
        }
        public void UpdateAllowanceTot(bool forceChangeEvents)
        {
            decimal? oldAllowanceTot = _AllowanceTot;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                // if (!new[] { "Absent", "Rest Day", "Holiday RG" }.Any(o => o.Contains(detail.Remarks)))
                if (!new[] { "Absent", "Rest Day" }.Any(o => o.Contains(detail.Remarks)))
                {
                    if (!detail.Halfday)
                    {
                        tempTotal += detail.AllowanceOverride;
                    }
                    else
                    {
                        tempTotal += detail.AllowanceOverride / 2;
                    }
                }
            }
            if (tempTotal > 0m)
            {
                _AllowanceTot = tempTotal;
            }
            else
            {
                _AllowanceTot = null;
            }
            if (forceChangeEvents)
                OnChanged("AllowanceTot", oldAllowanceTot, _AllowanceTot);
        }

        // OtHrs
        [Persistent("OtHrs")]
        private decimal? _OtHrs = null;
        [PersistentAlias("_OtHrs")]
        public decimal? OtHrs
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _OtHrs == null)
                    UpdateOtHrs(false);
                return _OtHrs;
            }
        }
        public void UpdateOtHrs(bool forceChangeEvents)
        {
            decimal? oldOtHrs = _OtHrs;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
                tempTotal += (detail.RestdayOtHrs + detail.OvertimeHrs + detail.HolidayOTHrs + detail.HolidayOTHrs2);
            if (tempTotal > 0m)
            {
                _OtHrs = tempTotal;
            }
            else
            {
                _OtHrs = null;
            }
            if (forceChangeEvents)
                OnChanged("OtHrs", oldOtHrs, _OtHrs);
        }

        public string OtHrs2
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (OtHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        // OT NormalDays
        [Persistent("OtNormal")]
        private decimal? _OtNormal = null;
        [PersistentAlias("_OtNormal")]
        public decimal? OtNormal
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _OtNormal == null)
                    UpdateOtNormal(false);
                return _OtNormal;
            }
        }
        public void UpdateOtNormal(bool forceChangeEvents)
        {
            decimal? oldOtNormal = _OtNormal;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
                tempTotal += detail.OvertimeHrs;
            if (tempTotal > 0m)
            {
                _OtNormal = tempTotal;
            }
            else
            {
                _OtNormal = null;
            }
            if (forceChangeEvents)
                OnChanged("OtNormal", oldOtNormal, _OtNormal);
        }
        [DisplayName("OTN")]
        public string OtNormalHrs
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (OtNormal != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(OtNormal.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        
        // RegHolDutyAmt
        [Persistent("RegHolDutyAmt")]
        private decimal? _RegHolDutyAmt = null;
        [PersistentAlias("_RegHolDutyAmt")]
        public decimal? RegHolDutyAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHolDutyAmt == null)
                    UpdateRegHolDutyAmt(false);
                return _RegHolDutyAmt;
            }
        }
        public void UpdateRegHolDutyAmt(bool forceChangeEvents)
        {
            decimal? oldRegHolDutyAmt = _RegHolDutyAmt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Regular && detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * detail.HolidayId.Rate / 100;
                }
            }
            if (tempTotal > 0m)
            {
                _RegHolDutyAmt = tempTotal;
            }
            else
            {
                _RegHolDutyAmt = null;
            }
            if (forceChangeEvents)
                OnChanged("RegHolDutyAmt", oldRegHolDutyAmt, _RegHolDutyAmt);
        }
        // DoubHolDutyAmt
        [Persistent("DoubHolDutyAmt")]
        private decimal? _DoubHolDutyAmt = null;
        [PersistentAlias("_DoubHolDutyAmt")]
        public decimal? DoubHolDutyAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHolDutyAmt == null)
                    UpdateDoubHolDutyAmt(false);
                return _DoubHolDutyAmt;
            }
        }
        public void UpdateDoubHolDutyAmt(bool forceChangeEvents)
        {
            decimal? oldDoubHolDutyAmt = _DoubHolDutyAmt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Double && detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * detail.HolidayId.Rate / 100;
                }
            }
            if (tempTotal > 0m)
            {
                _DoubHolDutyAmt = tempTotal;
            }
            else
            {
                _DoubHolDutyAmt = null;
            }
            if (forceChangeEvents)
                OnChanged("DoubHolDutyAmt", oldDoubHolDutyAmt, _DoubHolDutyAmt);
        }

        // DBOT
        [Persistent("DoubHolOtHrs")]
        private decimal? _DoubHolOtHrs = null;
        [PersistentAlias("_DoubHolOtHrs")]
        public decimal? DoubHolOtHrs
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _DoubHolOtHrs == null)
                    UpdateDoubHolOtHrs(false);
                return _DoubHolOtHrs;
            }
        }
        public void UpdateDoubHolOtHrs(bool forceChangeEvents)
        {
            decimal? oldDoubHolOtHrs = _DoubHolOtHrs;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Double && detail.OtStatus == OtStatusEnum.Approved && detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += Convert.ToDecimal(detail.ValidOtHours.TotalHours);
                }
            }
            if (tempTotal > 0m)
            {
                _DoubHolOtHrs = tempTotal;
            }
            else
            {
                _DoubHolOtHrs = null;
            }
            if (forceChangeEvents)
                OnChanged("RegDoubHolOtHrs", oldDoubHolOtHrs, _DoubHolOtHrs);
        }

        // DoubHolOt
        [Persistent("DoubHolOt")]
        private decimal? _DoubHolOt = null;
        [PersistentAlias("_DoubHolOt")]
        public decimal? DoubHolOt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _DoubHolOt == null)
                    UpdateDoubHolOt(false);
                return _DoubHolOt;
            }
        }
        public void UpdateDoubHolOt(bool forceChangeEvents)
        {
            decimal? oldDoubHolOt = _DoubHolOt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Double && detail.OtStatus == OtStatusEnum.Approved && detail.Day != detail.RestDayOfTheWeek)
                {
                    // tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * detail.HolidayId.Rate / 100;
                    tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.ValidOtHours.TotalHours)) * detail.HolidayId.ExcessRate / 100;
                }
            }
            if (tempTotal > 0m)
            {
                _DoubHolOt = tempTotal;
            }
            else
            {
                _DoubHolOt = null;
            }
            if (forceChangeEvents)
                OnChanged("DoubHolOt", oldDoubHolOt, _DoubHolOt);
        }

        // RegHolOt
        [Persistent("RegHolOt")]
        private decimal? _RegHolOt = null;
        [DisplayName("RGO AMT")]
        [PersistentAlias("_RegHolOt")]
        public decimal? RegHolOt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHolOt == null)
                    UpdateRegHolOt(false);
                return _RegHolOt;
            }
        }
        public void UpdateRegHolOt(bool forceChangeEvents)
        {
            decimal? oldRegHolOt = _RegHolOt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Regular && detail.OtStatus == OtStatusEnum.Approved && detail.Day != detail.RestDayOfTheWeek)
                {
                    // tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * detail.HolidayId.Rate / 100;
                    tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.ValidOtHours.TotalHours)) * detail.HolidayId.ExcessRate / 100;
                }
            }
            if (tempTotal > 0m)
            {
                _RegHolOt = tempTotal;
            }
            else
            {
                _RegHolOt = null;
            }
            if (forceChangeEvents)
                OnChanged("RegHolOt", oldRegHolOt, _RegHolOt);
        }
        // RGOT
        [Persistent("RegHolOtHrs")]
        private decimal? _RegHolOtHrs = null;
        [PersistentAlias("_RegHolOtHrs")]
        public decimal? RegHolOtHrs
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHolOtHrs == null)
                    UpdateRegHolOtHrs(false);
                return _RegHolOtHrs;
            }
        }
        public void UpdateRegHolOtHrs(bool forceChangeEvents)
        {
            decimal? oldRegHolOtHrs = _RegHolOtHrs;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Regular && detail.OtStatus == OtStatusEnum.Approved && detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += Convert.ToDecimal(detail.ValidOtHours.TotalHours);
                }
            }
            if (tempTotal > 0m)
            {
                _RegHolOtHrs = tempTotal;
            }
            else
            {
                _RegHolOtHrs = null;
            }
            if (forceChangeEvents)
                OnChanged("RegRegHolOtHrs", oldRegHolOtHrs, _RegHolOtHrs);
        }
        [DisplayName("RGO")]
        public string RegHolOtStr
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (RegHolOtHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHolOtHrs.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // SPCHolDuty
        [Persistent("SPCHolDuty")]
        private decimal? _SPCHolDuty = null;
        [PersistentAlias("_SPCHolDuty")]
        public decimal? SPCHolDuty
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _SPCHolDuty == null)
                    UpdateSPCHolDuty(false);
                return _SPCHolDuty;
            }
        }
        public void UpdateSPCHolDuty(bool forceChangeEvents)
        {
            decimal? oldSPCHolDuty = _SPCHolDuty;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += detail.HolidayHrs2;
                }
                //if (detail.HolidayType == HolidayTypeEnum.Regular)
                //{
                //    tempTotal += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                //}
            }
            if (tempTotal > 0m)
            {
                _SPCHolDuty = tempTotal;
            }
            else
            {
                _SPCHolDuty = null;
            }
            if (forceChangeEvents)
                OnChanged("SPCHolDuty", oldSPCHolDuty, _SPCHolDuty);
        }
        [DisplayName("SPD")]
        public string SPCHolDutyHrs
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (SPCHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(SPCHolDuty.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // SPCHolOt
        [Persistent("SPCHolOt")]
        private decimal? _SPCHolOt = null;
        [PersistentAlias("_SPCHolOt")]
        public decimal? SPCHolOt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _SPCHolOt == null)
                    UpdateSPCHolOt(false);
                return _SPCHolOt;
            }
        }
        public void UpdateSPCHolOt(bool forceChangeEvents)
        {
            decimal? oldSPCHolOt = _SPCHolOt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += detail.HolidayOTHrs2;
                }
                //if (detail.HolidayType == HolidayTypeEnum.Regular)
                //{
                //    tempTotal += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                //}
            }
            if (tempTotal > 0m)
            {
                _SPCHolOt = tempTotal;
            }
            else
            {
                _SPCHolOt = null;
            }
            if (forceChangeEvents)
                OnChanged("SPCHolOt", oldSPCHolOt, _SPCHolOt);
        }
        [DisplayName("SPO")]
        public string SPCHolOtHrs
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (SPCHolOt != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(SPCHolOt.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // SPCHolDutyAmt
        [Persistent("SPCHolDutyAmt")]
        private decimal? _SPCHolDutyAmt = null;
        [PersistentAlias("_SPCHolDutyAmt")]
        public decimal? SPCHolDutyAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _SPCHolDutyAmt == null)
                    UpdateSPCHolDutyAmt(false);
                return _SPCHolDutyAmt;
            }
        }
        public void UpdateSPCHolDutyAmt(bool forceChangeEvents)
        {
            decimal? oldSPCHolDutyAmt = _SPCHolDutyAmt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Special && detail.Day != detail.RestDayOfTheWeek)
                {
                    // tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * detail.HolidayId.Rate / 100;
                    tempTotal += ((_Basic / _Shift.CountAsHours) * detail.HolidayHrs2) * detail.HolidayId.Rate / 100;
                }
            }
            if (tempTotal > 0m)
            {
                _SPCHolDutyAmt = tempTotal;
            }
            else
            {
                _SPCHolDutyAmt = null;
            }
            if (forceChangeEvents)
                OnChanged("SPCHolDutyAmt", oldSPCHolDutyAmt, _SPCHolDutyAmt);
        }
        // SPCHolOtAmt
        [Persistent("SPCHolOtAmt")]
        private decimal? _SPCHolOtAmt = null;
        [PersistentAlias("_SPCHolOtAmt")]
        public decimal? SPCHolOtAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _SPCHolOtAmt == null)
                    UpdateSPCHolOtAmt(false);
                return _SPCHolOtAmt;
            }
        }
        public void UpdateSPCHolOtAmt(bool forceChangeEvents)
        {
            decimal? oldSPCHolOtAmt = _SPCHolOtAmt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Special && detail.Day != detail.RestDayOfTheWeek)
                {
                    // tempTotal += ((_Basic / _Shift.CountAsHours) * Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * detail.HolidayId.Rate / 100;
                    tempTotal += ((_Basic / _Shift.CountAsHours) * detail.HolidayOTHrs2) * detail.HolidayId.ExcessRate / 100;
                }
            }
            if (tempTotal > 0m)
            {
                _SPCHolOtAmt = tempTotal;
            }
            else
            {
                _SPCHolOtAmt = null;
            }
            if (forceChangeEvents)
                OnChanged("SPCHolOtAmt", oldSPCHolOtAmt, _SPCHolOtAmt);
        }
        // RegHolDuty
        [Persistent("RegHolDuty")]
        private decimal? _RegHolDuty = null;
        [PersistentAlias("_RegHolDuty")]
        public decimal? RegHolDuty
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHolDuty == null)
                    UpdateRegHolDuty(false);
                return _RegHolDuty;
            }
        }
        public void UpdateRegHolDuty(bool forceChangeEvents)
        {
            decimal? oldRegHolDuty = _RegHolDuty;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Regular && detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                }
            }
            if (tempTotal > 0m)
            {
                _RegHolDuty = tempTotal;
            }
            else
            {
                _RegHolDuty = null;
            }
            if (forceChangeEvents)
                OnChanged("RegHolDuty", oldRegHolDuty, _RegHolDuty);
        }
        [DisplayName("RGD")]
        public string RegHolDutyHrs
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (RegHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHolDuty.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // DoubHolDuty
        [Persistent("DoubHolDuty")]
        private decimal? _DoubHolDuty = null;
        [PersistentAlias("_DoubHolDuty")]
        public decimal? DoubHolDuty
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHolDuty == null)
                    UpdateDoubHolDuty(false);
                return _DoubHolDuty;
            }
        }
        public void UpdateDoubHolDuty(bool forceChangeEvents)
        {
            decimal? oldDoubHolDuty = _DoubHolDuty;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.HolidayType == HolidayTypeEnum.Double && detail.Day != detail.RestDayOfTheWeek)
                {
                    tempTotal += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                }
            }
            if (tempTotal > 0m)
            {
                _DoubHolDuty = tempTotal;
            }
            else
            {
                _DoubHolDuty = null;
            }
            if (forceChangeEvents)
                OnChanged("RegHolDuty", oldDoubHolDuty, _DoubHolDuty);
        }
        // Restday Duty
        [Persistent("RestdayDuty")]
        private decimal? _RestdayDuty = null;
        [PersistentAlias("_RestdayDuty")]
        public decimal? RestdayDuty
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDuty;
            }
        }

        [Persistent("RestdayDutyAmt")]
        private decimal? _RestdayDutyAmt = null;
        [PersistentAlias("_RestdayDutyAmt")]
        public decimal? RestdayDutyAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutyAmt;
            }
        }

        [Persistent("RestdayDutySp")]
        private decimal? _RestdayDutySp = null;
        [PersistentAlias("_RestdayDutySp")]
        public decimal? RestdayDutySp
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutySp;
            }
        }

        [Persistent("RestdayDutyRg")]
        private decimal? _RestdayDutyRg = null;
        [PersistentAlias("_RestdayDutyRg")]
        public decimal? RestdayDutyRg
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutyRg;
            }
        }

        [Persistent("RestdayDutySpAmt")]
        private decimal? _RestdayDutySpAmt = null;
        [PersistentAlias("_RestdayDutySpAmt")]
        public decimal? RestdayDutySpAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutySpAmt;
            }
        }

        [Persistent("RestdayDutyRgAmt")]
        private decimal? _RestdayDutyRgAmt = null;
        [PersistentAlias("_RestdayDutyRgAmt")]
        public decimal? RestdayDutyRgAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutyRgAmt;
            }
        }

        [Persistent("RestdayDutyDb")]
        private decimal? _RestdayDutyDb = null;
        [PersistentAlias("_RestdayDutyDb")]
        public decimal? RestdayDutyDb
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutyDb;
            }
        }

        [Persistent("RestdayDutyDbAmt")]
        private decimal? _RestdayDutyDbAmt = null;
        [PersistentAlias("_RestdayDutyDbAmt")]
        public decimal? RestdayDutyDbAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayDuty == null)
                    UpdateRestdayDuty(false);
                return _RestdayDutyDbAmt;
            }
        }

        public void UpdateRestdayDuty(bool forceChangeEvents)
        {
            decimal? oldRestdayDuty = _RestdayDuty;
            decimal tempTotal = 0m;
            decimal restdayDutyAmt = 0m;
            decimal restdayDutySp = 0m;
            decimal restdayDutySpAmt = 0m;
            decimal restdayDutyRg = 0m;
            decimal restdayDutyRgAmt = 0m;
            decimal restdayDutyDb = 0m;
            decimal restdayDutyDbAmt = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.LineID == "10304 08-19-2018 Dayshift")
                {

                }
                if (detail.Day == detail.RestDayOfTheWeek && detail.HolidayId == null)
                {
                    tempTotal += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                    restdayDutyAmt += (_Basic / detail.TimeTable.CountAsHours *
                                Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) *
                                detail.RestdayRate / 100;
                }
                else if (detail.Day == detail.RestDayOfTheWeek && detail.HolidayId != null)
                {
                    switch (detail.HolidayId.HolidayType)
                    {
                        case HolidayTypeEnum.Special:
                            restdayDutySp += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                            restdayDutySpAmt += (_Basic / detail.TimeTable.CountAsHours * 
                                Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) * 
                                detail.HolidayId.RestDayRate / 100;
                            break;
                        case HolidayTypeEnum.Regular:
                            restdayDutyRg += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                            restdayDutyRgAmt += (_Basic / detail.TimeTable.CountAsHours *
                                Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) *
                                detail.HolidayId.RestDayRate / 100;
                            break;
                        case HolidayTypeEnum.None:
                            break;
                        case HolidayTypeEnum.Double:
                            restdayDutyDb += Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours);
                            restdayDutyDbAmt += (_Basic / detail.TimeTable.CountAsHours *
                                Convert.ToDecimal(detail.FirstSetHrs.TotalHours + detail.SecondSetHrs.TotalHours)) *
                                detail.HolidayId.RestDayRate / 100;
                            break;
                        default:
                            break;
                    }
                }
            }
            // Restday SP
            if (restdayDutySp > 0m)
            {
                _RestdayDutySp = restdayDutySp;
            }
            else
            {
                _RestdayDutySp = null;
            }
            if (restdayDutySpAmt > 0m)
            {
                _RestdayDutySpAmt = restdayDutySpAmt;
            }
            else
            {
                _RestdayDutySpAmt = null;
            }
            // Restday RG
            if (restdayDutyRg > 0m)
            {
                _RestdayDutyRg = restdayDutyRg;
            }
            else
            {
                _RestdayDutyRg = null;
            }
            if (restdayDutyRgAmt > 0m)
            {
                _RestdayDutyRgAmt = restdayDutyRgAmt;
            }
            else
            {
                _RestdayDutyRgAmt = null;
            }
            // Restday DB
            if (restdayDutyDb > 0m)
            {
                _RestdayDutyDb = restdayDutyDb;
            }
            else
            {
                _RestdayDutyDb = null;
            }
            if (restdayDutyDbAmt > 0m)
            {
                _RestdayDutyDbAmt = restdayDutyDbAmt;
            }
            else
            {
                _RestdayDutyDbAmt = null;
            }
            if (tempTotal > 0m)
            {
                _RestdayDuty = tempTotal;
            }
            else
            {
                _RestdayDuty = null;
            }
            if (restdayDutyAmt > 0m)
            {
                _RestdayDutyAmt = restdayDutyAmt;
            }
            else
            {
                _RestdayDutyAmt = null;
            }
            if (forceChangeEvents)
                OnChanged("RestdayDuty", oldRestdayDuty, _RestdayDuty);
        }
        [DisplayName("RDD")]
        public string RestdayDutyHrs
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (RestdayDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDuty.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // Restday OT
        [Persistent("RestdayOt")]
        private decimal? _RestdayOt = null;
        [PersistentAlias("_RestdayOt")]
        public decimal? RestdayOt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOt;
            }
        }

        [Persistent("RestdayOtAmt")]
        private decimal? _RestdayOtAmt = null;
        [PersistentAlias("_RestdayOtAmt")]
        public decimal? RestdayOtAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtAmt;
            }
        }

        [Persistent("RestdayOtSp")]
        private decimal? _RestdayOtSp = null;
        [PersistentAlias("_RestdayOtSp")]
        public decimal? RestdayOtSp
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtSp;
            }
        }

        [Persistent("RestdayOtRg")]
        private decimal? _RestdayOtRg = null;
        [PersistentAlias("_RestdayOtRg")]
        public decimal? RestdayOtRg
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtRg;
            }
        }

        [Persistent("RestdayOtSpAmt")]
        private decimal? _RestdayOtSpAmt = null;
        [PersistentAlias("_RestdayOtSpAmt")]
        public decimal? RestdayOtSpAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtSpAmt;
            }
        }

        [Persistent("RestdayOtRgAmt")]
        private decimal? _RestdayOtRgAmt = null;
        [PersistentAlias("_RestdayOtRgAmt")]
        public decimal? RestdayOtRgAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtRgAmt;
            }
        }

        [Persistent("RestdayOtDb")]
        private decimal? _RestdayOtDb = null;
        [PersistentAlias("_RestdayOtDb")]
        public decimal? RestdayOtDb
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtDb;
            }
        }

        [Persistent("RestdayOtDbAmt")]
        private decimal? _RestdayOtDbAmt = null;
        [PersistentAlias("_RestdayOtDbAmt")]
        public decimal? RestdayOtDbAmt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RestdayOt == null)
                    UpdateRestdayOt(false);
                return _RestdayOtDbAmt;
            }
        }

        public void UpdateRestdayOt(bool forceChangeEvents)
        {
            decimal? oldRestdayOt = _RestdayOt;
            decimal tempTotal = 0m;
            decimal restdayOtAmt = 0m;
            decimal restdayOtSp = 0m;
            decimal restdayOtSpAmt = 0m;
            decimal restdayOtRg = 0m;
            decimal restdayOtRgAmt = 0m;
            decimal restdayOtDb = 0m;
            decimal restdayOtDbAmt = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.LineID == "10304 08-19-2018 Dayshift")
                {

                }
                if (detail.Day == detail.RestDayOfTheWeek && detail.HolidayId == null)
                {
                    tempTotal += detail.RestdayOtHrs2;
                    restdayOtAmt += (_Basic / detail.TimeTable.CountAsHours *
                                detail.RestdayOtHrs2) *
                                detail.RestdayOtRate / 100;
                }
                else if (detail.Day == detail.RestDayOfTheWeek && detail.HolidayId != null)
                {
                    switch (detail.HolidayId.HolidayType)
                    {
                        case HolidayTypeEnum.Special:
                            restdayOtSp += detail.RestdayOtHrs2;
                            restdayOtSpAmt += (_Basic / detail.TimeTable.CountAsHours *
                                detail.RestdayOtHrs2) *
                                detail.HolidayId.RestDayOtRate / 100;
                            break;
                        case HolidayTypeEnum.Regular:
                            restdayOtRg += detail.RestdayOtHrs2;
                            restdayOtRgAmt += (_Basic / detail.TimeTable.CountAsHours *
                                detail.RestdayOtHrs2) *
                                detail.HolidayId.RestDayOtRate / 100;
                            break;
                        case HolidayTypeEnum.None:
                            break;
                        case HolidayTypeEnum.Double:
                            restdayOtDb += detail.RestdayOtHrs2;
                            restdayOtDbAmt += (_Basic / detail.TimeTable.CountAsHours *
                                detail.RestdayOtHrs2) *
                                detail.HolidayId.RestDayOtRate / 100;
                            break;
                        default:
                            break;
                    }
                }
            }
            // Restday SP
            if (restdayOtSp > 0m)
            {
                _RestdayOtSp = restdayOtSp;
            }
            else
            {
                _RestdayOtSp = null;
            }
            if (restdayOtSpAmt > 0m)
            {
                _RestdayOtSpAmt = restdayOtSpAmt;
            }
            else
            {
                _RestdayOtSpAmt = null;
            }
            // Restday RG
            if (restdayOtRg > 0m)
            {
                _RestdayOtRg = restdayOtRg;
            }
            else
            {
                _RestdayOtRg = null;
            }
            if (restdayOtRgAmt > 0m)
            {
                _RestdayOtRgAmt = restdayOtRgAmt;
            }
            else
            {
                _RestdayOtRgAmt = null;
            }
            // Restday DB
            if (restdayOtDb > 0m)
            {
                _RestdayOtDb = restdayOtDb;
            }
            else
            {
                _RestdayOtDb = null;
            }
            if (restdayOtDbAmt > 0m)
            {
                _RestdayOtDbAmt = restdayOtDbAmt;
            }
            else
            {
                _RestdayOtDbAmt = null;
            }
            if (tempTotal > 0m)
            {
                _RestdayOt = tempTotal;
            }
            else
            {
                _RestdayOt = null;
            }
            if (restdayOtAmt > 0m)
            {
                _RestdayOtAmt = restdayOtAmt;
            }
            else
            {
                _RestdayOtAmt = null;
            }
            if (forceChangeEvents)
                OnChanged("RestdayOt", oldRestdayOt, _RestdayOt);
        }
        [DisplayName("RDO")]
        public string RestdayOtHrs
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (RestdayOt != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOt.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // SpHldPrcnt
        [Persistent("SpgHldPrcnt")]
        private decimal? _SpgHldPrcnt = null;
        [PersistentAlias("_SpgHldPrcnt")]
        public decimal? SpgHldPrcnt
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _SpgHldPrcnt == null)
                    UpdateSpgHldPrcnt(false);
                return _SpgHldPrcnt;
            }
        }
        public void UpdateSpgHldPrcnt(bool forceChangeEvents)
        {
            decimal? oldSpgHldPrcnt = _SpgHldPrcnt;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
                if (detail.HolidayHrs2 != 0m)
                {
                    //tempTotal += detail.HolidayId.Rate;
                    tempTotal = detail.HolidayId.Rate;
                }
            if (tempTotal > 0m)
            {
                _SpgHldPrcnt = tempTotal;
            }
            else
            {
                _SpgHldPrcnt = null;
            }
            if (forceChangeEvents)
                OnChanged("SpgHldPrcnt", oldSpgHldPrcnt, _SpgHldPrcnt);
        }

        // RegHldHrs
        [Persistent("RegHldHrs")]
        private decimal? _RegHldHrs = null;
        [PersistentAlias("_RegHldHrs")]
        public decimal? RegHldHrs
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _RegHldHrs == null)
                    UpdateRegHldHrs(false);
                return _RegHldHrs;
            }
        }
        public void UpdateRegHldHrs(bool forceChangeEvents)
        {
            decimal? oldRegHldHrs = _RegHldHrs;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
                tempTotal += detail.HolidayHrs;
            if (tempTotal > 0m)
            {
                _RegHldHrs = tempTotal;
            }
            else
            {
                _RegHldHrs = null;
            }
            if (forceChangeEvents)
                OnChanged("RegHldHrs", oldRegHldHrs, _RegHldHrs);
        }

        public string RegHldHrs2
        {
            get
            {
                //if (RegHldHrs != null && RegHldHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(RegHldHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (RegHldHrs != null && RegHldHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(RegHldHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return RegHldHrs != null ? TimeSpan.FromHours(Convert.ToDouble(RegHldHrs.Value)).ToString(@"hh\:mm\:ss") : string.Empty;
                if (RegHldHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHldHrs.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // LateMins
        [Persistent("LateMins")]
        private decimal? _LateMins = null;
        [PersistentAlias("_LateMins")]
        public decimal? LateMins
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _LateMins == null)
                    UpdateLateMins(false);
                return _LateMins;
            }
        }
        public void UpdateLateMins(bool forceChangeEvents)
        {
            if (_RowID == Guid.Parse("a6154533-3167-468f-be17-c9183cdf1543"))
            {

            }
            decimal? oldLateMins = _LateMins;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords) {
                if (!new[] { "Absent", "Rest Day", "Rest Day OT", "Holiday RGOT", "Holiday SP" }.Any(o => o.Contains(detail.Remarks)))
                {
                    TimeSpan ts = new TimeSpan();
                    if (detail.FirstSetHrs.TotalHours == 0)
                    {
                        TimeSpan sl = !detail.EmployeeId.NoLateDeduction ? detail.SecondLate.TotalMinutes >= detail.TimeTable.LateTimeMins + 1 ? detail.SecondLate : new TimeSpan() : TimeSpan.Zero;
                        TimeSpan se = !detail.EmployeeId.NoEarlyDeduction ? detail.SecondEarly.TotalMinutes >= detail.TimeTable.LeaveEarlyTimeMins + 1 ? detail.SecondEarly : new TimeSpan() : TimeSpan.Zero;
                        ts = sl + se;
                    }
                    else if (detail.SecondSetHrs.TotalHours == 0)
                    {
                        TimeSpan fl = !detail.EmployeeId.NoLateDeduction ? detail.FirstLate.TotalMinutes >= detail.TimeTable.LateTimeMins + 1 ? detail.FirstLate : new TimeSpan() : TimeSpan.Zero;
                        TimeSpan fe = !detail.EmployeeId.NoEarlyDeduction ? detail.FirstEarly.TotalMinutes >= detail.TimeTable.LeaveEarlyTimeMins + 1 ? detail.FirstEarly : new TimeSpan() : TimeSpan.Zero;
                        ts = fl + fe;
                    }
                    else
                    {
                        TimeSpan fl = !detail.EmployeeId.NoLateDeduction ? detail.FirstLate.TotalMinutes >= detail.TimeTable.LateTimeMins + 1 ? detail.FirstLate : new TimeSpan() : TimeSpan.Zero;
                        TimeSpan fe = !detail.EmployeeId.NoEarlyDeduction ? detail.FirstEarly.TotalMinutes >= detail.TimeTable.LeaveEarlyTimeMins + 1 ? detail.FirstEarly : new TimeSpan() : TimeSpan.Zero;
                        TimeSpan sl = !detail.EmployeeId.NoLateDeduction ? detail.SecondLate.TotalMinutes >= detail.TimeTable.LateTimeMins + 1 ? detail.SecondLate : new TimeSpan() : TimeSpan.Zero;
                        TimeSpan se = !detail.EmployeeId.NoEarlyDeduction ? detail.SecondEarly.TotalMinutes >= detail.TimeTable.LeaveEarlyTimeMins + 1 ? detail.SecondEarly : new TimeSpan() : TimeSpan.Zero;
                        ts = fl + fe + sl + se;
                    }
                    tempTotal += Convert.ToDecimal(ts.TotalMinutes); //(detail.LateHrs + detail.UndertimeHrs);
                }
            }
            if (tempTotal > 0m)
            {
                TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(tempTotal));
                _LateMins = Convert.ToDecimal(ts.TotalMinutes);
            }
            else
            {
                _LateMins = null;
            }
            if (forceChangeEvents)
                OnChanged("LateMins", oldLateMins, _LateMins);
        }

        public string LateMins2
        {
            get
            {
                //if (LateMins != null && LateMins > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(LateMins.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (LateMins != null && LateMins < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(LateMins.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return LateMins != null ? TimeSpan.FromMinutes(Convert.ToDouble(LateMins.Value)).ToString( @"hh\:mm\:ss") : string.Empty;
                if (LateMins != null)
                {
                    TimeSpan ts = TimeSpan.FromMinutes(Convert.ToDouble(LateMins.Value));
                    return string.Format("{0}", (((ts.Days * 24) + ts.Hours) * 60) + ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        // NightDiff
        [Persistent("NightHrs")]
        private decimal? _NightHrs = null;
        [PersistentAlias("_NightHrs")]
        public decimal? NightHrs
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _NightHrs == null)
                    UpdateNightHrs(false);
                return _NightHrs;
            }
        }
        public void UpdateNightHrs(bool forceChangeEvents)
        {
            decimal? oldNightHrs = _NightHrs;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
                tempTotal += detail.NightDiffHrs;
            if (tempTotal > 0m)
            {
                _NightHrs = tempTotal;
            }
            else
            {
                _NightHrs = null;
            }
            if (forceChangeEvents)
                OnChanged("NightHrs", oldNightHrs, _NightHrs);
        }
        [DisplayName("ND")]
        public string NightHrs2
        {
            get
            {
                //if (OtHrs != null && OtHrs > 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss");
                //}
                //else if (OtHrs != null && OtHrs < 24m)
                //{
                //    return TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"hh\:mm\:ss");
                //}
                //else
                //{
                //    return string.Empty;
                //}
                //return OtHrs != null ? TimeSpan.FromHours(Convert.ToDouble(OtHrs.Value)).ToString(@"dd\.hh\:mm\:ss") : string.Empty;
                if (NightHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(NightHrs.Value));
                    return string.Format("{0}:{1}", (ts.Days * 24) + ts.Hours, ts.Minutes);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        // NightValue
        [Persistent("NightValue")]
        private decimal? _NightValue = null;
        [PersistentAlias("_NightValue")]
        public decimal? NightValue
        {
            get
            {
                if (!IsLoading && !IsSaving) // && _NightHrs == null)
                    UpdateNightValue(false);
                return _NightValue;
            }
        }
        public void UpdateNightValue(bool forceChangeEvents)
        {
            decimal? oldNightValue = _NightValue;
            decimal tempTotal = 0m;
            foreach (CheckInAndOut03 detail in DailyTimeRecords)
            {
                if (detail.LineID == "10261 04-11-2019 Dayshift")
                {

                }
                decimal perHr = _Basic / detail.TimeTable.CountAsHours;
                // decimal diffRate = detail.TimeTable.NightDiffRate / 100;
                decimal diffRate = detail.NightDiffRate / 100;
                switch (detail.NightDiffMode)
                {
                    case NightDiffModeEnum.Ordinary:
                        tempTotal += (perHr * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.RestDay:
                        tempTotal += ((perHr * detail.RestdayRate / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.Special:
                        tempTotal += ((perHr * (detail.HolidayId.Rate + 100) / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.SpecialRestDay:
                        tempTotal += ((perHr * (detail.HolidayId.RestDayRate) / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.Regular:
                        tempTotal += ((perHr * (detail.HolidayId.Rate + 100) / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.RegularRestDay:
                        tempTotal += ((perHr * (detail.HolidayId.RestDayRate) / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.Double:
                        tempTotal += ((perHr * (detail.HolidayId.Rate + 100) / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    case NightDiffModeEnum.DoubeRestDay:
                        tempTotal += ((perHr * (detail.HolidayId.RestDayRate) / 100) * diffRate) * detail.NightDiffHrs;
                        break;
                    default:
                        break;
                }
            }
            if (tempTotal > 0m)
            {
                _NightValue = tempTotal;
            }
            else
            {
                _NightValue = null;
            }
            if (forceChangeEvents)
                OnChanged("NightValue", oldNightValue, _NightValue);
        }
        #region Pay Computation

        //Basic No. of Days(9)		3,000.00
        [EditorAlias("LabelControlEditor")]
        public string BasicCaption { 
            get {
                if (NoOfDays != null)
                {
                    return string.Format("Basic No. of Days({0})", Convert.ToDouble(NoOfDays.Value));
                }
                else
                {
                    return string.Format("Basic No. of Days(0)");
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string BasicValue { 
            get {
                if (NoOfDays != null)
                {
                    return (NoOfDays.Value * _Basic).ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Less: Late/Und.(358mins)	   00.00
        [EditorAlias("LabelControlEditor")]
        public string LateUndCaption { 
            get {
                if (LateMins != null)
                {
                    return string.Format("Less: Late/Und.({0:N2}mins)", LateMins.Value);
                }
                else
                {
                    return string.Format("Less: Late/Und.(0mins)");
                }
            } 
        }

        [EditorAlias("LabelControlEditor")]
        public string LateUndString
        {
            get
            {
                if (LateMins != null)
                {
                    return string.Format("{0:N2}mins", LateMins.Value);
                }
                else
                {
                    return string.Format("0mins");
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string LateUndValue { 
            get {
                if (LateMins != null)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (0 - (_Basic / _Shift.CountAsHours * (LateMins.Value/60))).ToString("n2");
                        case EmployeePayTypeEnum.Daily:
                            return (0 - (_Basic / _Shift.CountAsHours * (LateMins.Value/60))).ToString("n2");
                        case EmployeePayTypeEnum.Monthly:
                            return (0m).ToString("n2");
                        default:
                            return (0m).ToString("n2");
                    }
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Allowance					  000.00
        [EditorAlias("LabelControlEditor")]
        public string AllowanceCaption { 
            get { 
                return "Allowance"; 
            } 
        }
        [EditorAlias("LabelDecControlEditor")]
        public string AllowanceValue { 
            get {
                if (AllowanceTot != null)
                {
                    return (AllowanceTot.Value).ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2"); 
                }
            } 
        }
        //Overtime(13hrs&25mins*125%)   000.00
        [EditorAlias("LabelControlEditor")]
        public string OvertimeCaption { 
            get {
                if (OtNormal != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(OtNormal.Value));
                    // Normal Day
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Overtime({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Overtime({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Overtime({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Overtime(NONE)"; 
                    }
                }
                else
                {
                    return "Overtime(NONE)"; 
                }
            } 
        }

        [EditorAlias("LabelControlEditor")]
        public string OvertimeString
        {
            get
            {
                if (OtNormal != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(OtNormal.Value));
                    // Normal Day
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string OvertimeValue { 
            get {
                if (OtNormal != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return (((_Basic / _Shift.CountAsHours) * OtNormal.Value) * _NormalOtRate / 100).ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Restday Duty(8hrs*130%)       000.00
        [EditorAlias("LabelControlEditor")]
        public string RestdayDutyCaption { 
            get {
                if (RestdayDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday Duty({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday Duty({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday Duty({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday Duty(NONE)";
                    }
                }
                else
                {
                    return "Restday Duty(NONE)";
                }
            } 
        }
        [EditorAlias("LabelControlEditor")]
        public string RestdayDutyString
        {
            get
            {
                if (RestdayDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string RestdayDutyValue { 
            get {
                if (RestdayDutyAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayDutyAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }

        //Restday SP Duty(8hrs*130%)       000.00
        [EditorAlias("LabelControlEditor")]
        public string RestdaySpDutyCaption
        {
            get
            {
                if (RestdayDutySp != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDutySp.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday SP Duty({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday SP Duty({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday SP Duty({0}mins)", ts.Minutes);
                    }
                    else
                    { 
                        return "Restday SP Duty(NONE)";
                    }
                }
                else
                {
                    return "Restday SP Duty(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string RestdaySpDutyString
        {
            get
            {
                if (RestdayDutySp != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDutySp.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string RestdayDutySpValue
        {
            get
            {
                if (RestdayDutySpAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayDutySpAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }

        //Restday RG Duty(8hrs*130%)       000.00
        [EditorAlias("LabelControlEditor")]
        public string RestdayRgDutyCaption
        {
            get
            {
                if (RestdayDutyRg != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDutyRg.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday RG Duty({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday RG Duty({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday RG Duty({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday RG Duty(NONE)";
                    }
                }
                else
                {
                    return "Restday RG Duty(NONE)";
                }
            }
        }
        [EditorAlias("LabelControlEditor")]
        public string RestdayRgDutyString
        {
            get
            {
                if (RestdayDutyRg != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDutyRg.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string RestdayDutyRgValue
        {
            get
            {
                if (RestdayDutyRgAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayDutyRgAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }

        //Restday DB Duty(8hrs*130%)       000.00
        [EditorAlias("LabelControlEditor")]
        public string RestdayDbDutyCaption
        {
            get
            {
                if (RestdayDutyDb != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDutyDb.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday DB Duty({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday DB Duty({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday DB Duty({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday DB Duty(NONE)";
                    }
                }
                else
                {
                    return "Restday DB Duty(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string RestdayDbDutyString
        {
            get
            {
                if (RestdayDutyDb != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayDutyDb.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string RestdayDutyDbValue
        {
            get
            {
                if (RestdayDutyDbAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayDutyDbAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }


        //Restday OT(2hrs*169%)         000.00
        [EditorAlias("LabelControlEditor")]
        public string RestdayOTCaption { 
            get {
                if (RestdayOt != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOt.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday OT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday OT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday OT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday OT(NONE)";
                    }
                }
                else
                {
                    return "Restday OT(NONE)";
                }
            } 
        }
        [EditorAlias("LabelControlEditor")]
        public string RestdayOTString
        {
            get
            {
                if (RestdayOt != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOt.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string RestdayOTValue { 
            get {
                if (RestdayOtAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return (((_Basic / _Shift.CountAsHours) * RestdayOt.Value) * _RestdayOtRate / 100).ToString("n2");
                    return RestdayOtAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Night Diff.(3:59hrs*10%)       00.00
        [EditorAlias("LabelControlEditor")]
        public string NightDiffCaption { 
            get {
                if (NightHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(NightHrs.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Night Diff.({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Night Diff.({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Night Diff.({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Night Diff.(NONE)";
                    }
                }
                else
                {
                    return "Night Diff.(NONE)";
                }
            } 
        }

        [EditorAlias("LabelControlEditor")]
        public string NightDiffString
        {
            get
            {
                if (NightHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(NightHrs.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string NightDiffValue { 
            get {
                if (NightValue != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return (((_Basic / _Shift.CountAsHours) * NightHrs.Value) * _Shift.NightDiffRate / 100).ToString("n2");
                    return NightValue.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Reg. Holiday(8hrs)    	      000.00
        [EditorAlias("LabelControlEditor")]
        public string RegHolCaption { 
            get {
                if (RegHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHolDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Reg. Holiday({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Reg. Holiday({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Reg. Holiday({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Reg. Holiday(NONE)";
                    }
                }
                else
                {
                    return "Reg. Holiday(NONE)";
                }
            } 
        }
        [EditorAlias("LabelControlEditor")]
        public string RegHolString
        {
            get
            {
                if (RegHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHolDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string RegHolValue { 
            get {
                if (RegHolDutyAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return RegHolDutyAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Reg. Holiday OT(8hrs*260%)    000.00
        [EditorAlias("LabelControlEditor")]
        public string RegHolOTCaption { 
            get {
                if (RegHolOtHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHolOtHrs.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Reg. Holiday OT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Reg. Holiday OT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Reg. Holiday OT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Reg. Holiday OT(NONE)";
                    }
                }
                else
                {
                    return "Reg. Holiday OT(NONE)";
                }
            } 
        }
        [EditorAlias("LabelControlEditor")]
        public string RegHolOTString
        {
            get
            {
                if (RegHolOtHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RegHolOtHrs.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string RegHolOTValue { 
            get {
                if (RegHolOt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return RegHolOt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }

        [EditorAlias("LabelControlEditor")]
        public string DoubHolCaption
        {
            get
            {
                if (DoubHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(DoubHolDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Double Holiday({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Double Holiday({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Double Holiday({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Double Holiday(NONE)";
                    }
                }
                else
                {
                    return "Double Holiday(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string DoubHolString
        {
            get
            {
                if (DoubHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(DoubHolDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string DoubleHolValue
        {
            get
            {
                if (DoubHolDutyAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return DoubHolDutyAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string DoubHolOTCaption
        {
            get
            {
                if (DoubHolOtHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(DoubHolOtHrs.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Double Holiday OT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Double Holiday OT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Double Holiday OT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Double Holiday OT(NONE)";
                    }
                }
                else
                {
                    return "Double Holiday OT(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string DoubHolOTString
        {
            get
            {
                if (DoubHolOtHrs != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(DoubHolOtHrs.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string DoubHolOTValue
        {
            get
            {
                if (DoubHolOt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return DoubHolOt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        //Spc. Holiday(8hrs*30%)        000.00
        [EditorAlias("LabelControlEditor")]
        public string SpcHolCaption { 
            get {
                if (SPCHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(SPCHolDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Spc. Holiday({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Spc. Holiday({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Spc. Holiday({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Spc. Holiday(NONE)";
                    }
                }
                else
                {
                    return "Spc. Holiday(NONE)";
                }
            } 
        }

        [EditorAlias("LabelControlEditor")]
        public string SpcHolString
        {
            get
            {
                if (SPCHolDuty != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(SPCHolDuty.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string SpcHolValue { 
            get {
                if (SPCHolDutyAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return SPCHolDutyAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            } 
        }
        //Spc. Holiday OT(2hrs*169%)    000.00
        [EditorAlias("LabelControlEditor")]
        public string SpcHolOTCaption { 
            get {
                if (SPCHolOt != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(SPCHolOt.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Spc. Holiday OT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Spc. Holiday OT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Spc. Holiday OT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Spc. Holiday OT(NONE)";
                    }
                }
                else
                {
                    return "Spc. Holiday OT(NONE)";
                }
            } 
        }

        [EditorAlias("LabelControlEditor")]
        public string SpcHolOTString
        {
            get
            {
                if (SPCHolOt != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(SPCHolOt.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }
        [EditorAlias("LabelDecControlEditor")]
        public string SpcHolOTValue { 
            get {
                if (SPCHolOtAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    return SPCHolOtAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }

        // Restday OT Pay Start
        // SP Restday
        [EditorAlias("LabelControlEditor")]
        public string RestdaySpOtCaption
        {
            get
            {
                if (RestdayOtSp != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOtSp.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday SPOT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday SPOT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday SPOT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday SPOT(NONE)";
                    }
                }
                else
                {
                    return "Restday SPOT(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string RestdaySpOtString
        {
            get
            {
                if (RestdayOtSp != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOtSp.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string RestdaySpOtValue
        {
            get
            {
                if (RestdayOtSpAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayOtSpAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // RG Restday
        [EditorAlias("LabelControlEditor")]
        public string RestdayRgOtCaption
        {
            get
            {
                if (RestdayOtRg != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOtRg.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday RGOT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday RGOT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday RGOT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday RGOT(NONE)";
                    }
                }
                else
                {
                    return "Restday RGOT(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string RestdayRgOtString
        {
            get
            {
                if (RestdayOtRg != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOtRg.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string RestdayRgOtValue
        {
            get
            {
                if (RestdayOtRgAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayOtRgAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // DB Restday
        [EditorAlias("LabelControlEditor")]
        public string RestdayDbOtCaption
        {
            get
            {
                if (RestdayOtDb != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOtDb.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday DBOT({0}hrs&{1}mins)", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("Restday DBOT({0}hrs)", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("Restday DBOT({0}mins)", ts.Minutes);
                    }
                    else
                    {
                        return "Restday DBOT(NONE)";
                    }
                }
                else
                {
                    return "Restday DBOT(NONE)";
                }
            }
        }

        [EditorAlias("LabelControlEditor")]
        public string RestdayDbOtString
        {
            get
            {
                if (RestdayOtDb != null)
                {
                    TimeSpan ts = TimeSpan.FromHours(Convert.ToDouble(RestdayOtDb.Value));
                    if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}hrs&{1}mins", Math.Truncate(ts.TotalHours), ts.Minutes);
                    }
                    else if (Math.Truncate(ts.TotalHours) > 0 && ts.Minutes == 0)
                    {
                        return string.Format("{0}hrs", Math.Truncate(ts.TotalHours));
                    }
                    else if (Math.Truncate(ts.TotalHours) == 0 && ts.Minutes > 0)
                    {
                        return string.Format("{0}mins", ts.Minutes);
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    return "";
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string RestdayDbOtValue
        {
            get
            {
                if (RestdayOtDbAmt != null)
                {
                    //_Basic / _Shift.CountAsHours
                    //return ((((_Basic / _Shift.CountAsHours) * RestdayDuty.Value) * _RestdayRate / 100) + RestdayDutyHol.Value).ToString("n2");
                    return RestdayOtDbAmt.Value.ToString("n2");
                }
                else
                {
                    return (0m).ToString("n2");
                }
            }
        }
        // Restday OT Pay End

        //SUB-TOTAL          			0,000.00
        [EditorAlias("LabelControlEditor")]
        public string SubTotalCaption { get { return "SUB-TOTAL"; } }
        [EditorAlias("LabelDecControlEditor")]
        public string SubTotalValue { 
            get {
                decimal bas = decimal.Parse(BasicValue);
                decimal lun = decimal.Parse(LateUndValue);
                decimal all = decimal.Parse(AllowanceValue);
                decimal ovr = decimal.Parse(OvertimeValue);
                decimal rst = decimal.Parse(RestdayDutyValue);
                decimal rso = decimal.Parse(RestdayOTValue);
                decimal ntd = decimal.Parse(NightDiffValue);
                decimal rhl = decimal.Parse(RegHolValue);
                decimal rho = decimal.Parse(RegHolOTValue);
                decimal shl = decimal.Parse(SpcHolValue);
                decimal sho = decimal.Parse(SpcHolOTValue);
                decimal rspd = decimal.Parse(RestdayDutySpValue);
                decimal rrgd = decimal.Parse(RestdayDutyRgValue);
                decimal rdbd = decimal.Parse(RestdayDutyDbValue);
                decimal rspo = decimal.Parse(RestdaySpOtValue);
                decimal rrgo = decimal.Parse(RestdayRgOtValue);
                decimal rdbo = decimal.Parse(RestdayDbOtValue);
                decimal dbh = decimal.Parse(DoubleHolValue);
                decimal dbho = decimal.Parse(DoubHolOTValue);

                decimal tot = bas + lun + all + ovr + rst + rso + ntd + rhl + rho + shl + sho + rspd + rrgd + rdbd + rspo + rrgo + rdbo + dbh + dbho;
                return (tot).ToString("n2"); 
            } 
        }

        #endregion

        #region Staffpayroll batch associations

        private StaffPayrollBatch _PayrollBatchId;
        [Association]
        [Custom("AllowEdit", "False")]
        public StaffPayrollBatch PayrollBatchId
        {
            get { return _PayrollBatchId; }
            set
            {
                SetPropertyValue("PayrollBatchId", ref _PayrollBatchId, value);
            }
        }
        #endregion
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        #endregion

        public AttendanceRecord(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            RowID = Guid.NewGuid();
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }
        protected override void OnSaving()
        {
            base.OnSaving();
            #region Saving Modified
            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }
            #endregion
        }
        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _NightDiffRate;
        private decimal _NormalOtRate;
        private decimal _RestdayRate;
        private decimal _RestdayOtRate;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                if (SecuritySystem.CurrentUser != null)
                {
                    _CurrentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                }
                return _CurrentUser;
            }
        }
        #endregion

        private void Reset()
        {
            _NoOfDays = null;
            _AllowanceTot = null;
            _OtHrs = null;
            _OtNormal = null;
            _RestdayDuty = null;
            _SpgHldPrcnt = null;
            _RegHldHrs = null;
            _LateMins = null;
            _NightHrs = null;
            _RestdayOt = null;
            _RegHolDuty = null;
            _RegHolDutyAmt = null;
            _RegHolOt = null;
            _RegHolOtHrs = null;
            _SPCHolDuty = null;
            _SPCHolDutyAmt = null;
            _SPCHolOt = null;
            _SPCHolOtAmt = null;
            _NightHrs = null;
        }
    }
}