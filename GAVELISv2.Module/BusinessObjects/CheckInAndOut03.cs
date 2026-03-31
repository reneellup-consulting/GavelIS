using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum NightDiffModeEnum
    {
        Ordinary,
        RestDay,
        Special,
        SpecialRestDay,
        Regular,
        RegularRestDay,
        Double,
        DoubeRestDay

    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [OptimisticLocking(false)]
    public class CheckInAndOut03 : XPObject
    {
        private decimal _Allowance;
        private bool _UnAltered = true;
        private DateTime _Scuut;
        private string _LineID;
        private Guid _RowID;
        private AttendanceCalculator _AttCalcId;
        private AttendanceRecord _AttRecId;
        private string _EnrolledNo;
        private Employee _EmployeeId;
        private DateTime _Date;
        private DayOfWeek _Day;
        private TimeTable2 _TimeTable;
        private DateTime _OnDuty;
        private DateTime _HalfDuty;
        private DateTime _OffDuty;
        // ....
        private decimal _Normal;
        private decimal _RealTime;
        private TimeSpan _NormalHours;
        private TimeSpan _ActualLate;
        private TimeSpan _Late;
        private TimeSpan _ActualEarly;
        private TimeSpan _Early;
        private HolidayTypeEnum _HolidayType;
        private Holiday _HolidayId;
        private bool _Absent = false;
        private decimal _AbsentCount;
        private TimeSpan _AbsentHours;
        private TimeSpan _OtHours;
        private TimeSpan _ValidOtHours;
        private TimeSpan _ActualHours;
        private TimeSpan _ValidWorkHours;
        private string _Remarks;
        private OtStatusEnum _OtStatus;
        private TimeSpan _RestDay;
        private TimeSpan _Holiday;
        private TimeSpan _Night;
        private bool _Flexible = false;
        private GenJournalHeader _StaffPayrollId;
        private bool _Posted = false;
        private bool _Invalid = false;
        private int _LogTag;
        [Custom("AllowEdit", "False")]
        public bool UnAltered
        {
            get { return _UnAltered; }
            set { SetPropertyValue("UnAltered", ref _UnAltered, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string LineID
        {
            get { return _LineID; }
            set { SetPropertyValue("LineID", ref _LineID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("ShiftCalculations2")]
        public AttendanceCalculator AttCalcId
        {
            get { return _AttCalcId; }
            set
            {
                SetPropertyValue("AttCalcId", ref _AttCalcId, value);
            }
        }
        [Custom("AllowEdit", "False")]
        [Association]
        public AttendanceRecord AttRecId
        {
            get { return _AttRecId; }
            set
            {
                SetPropertyValue("AttRecId", ref _AttRecId, value);
                AttendanceRecord oldAttRecId = _AttRecId;
                bool modified = SetPropertyValue("AttRecId", ref _AttRecId, value);
                if (!IsLoading && !IsSaving && oldAttRecId != _AttRecId && modified)
                {
                    oldAttRecId = oldAttRecId ?? _AttRecId;
                    oldAttRecId.UpdateNoOfDays(false);
                    oldAttRecId.UpdateOtHrs(false);
                    oldAttRecId.UpdateOtNormal(false);
                    oldAttRecId.UpdateSpgHldPrcnt(false);
                    oldAttRecId.UpdateRegHldHrs(false);
                    oldAttRecId.UpdateLateMins(false);
                    oldAttRecId.UpdateRestdayDuty(false);
                    oldAttRecId.UpdateRestdayOt(false);
                    oldAttRecId.UpdateRegHolDuty(false);
                    oldAttRecId.UpdateRegHolDutyAmt(false);
                    oldAttRecId.UpdateNightHrs(false);
                    oldAttRecId.UpdateRegHolOt(false);
                    oldAttRecId.UpdateRegHolOtHrs(false);
                    oldAttRecId.UpdateSPCHolDuty(false);
                    oldAttRecId.UpdateSPCHolDutyAmt(false);
                    oldAttRecId.UpdateSPCHolOt(false);
                    oldAttRecId.UpdateSPCHolOtAmt(false);
                    oldAttRecId.UpdateNightValue(false);
                    oldAttRecId.UpdateDoubHolDuty(false);
                    oldAttRecId.UpdateDoubHolDutyAmt(false);
                    oldAttRecId.UpdateDoubHolOt(false);
                    //UpdateDoubHolOtHrs
                    oldAttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string EnrolledNo
        {
            get { return _EnrolledNo; }
            set { SetPropertyValue("EnrolledNo", ref _EnrolledNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee EmployeeId
        {
            get { return _EmployeeId; }
            set { SetPropertyValue("EmployeeId", ref _EmployeeId, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Employee { get { return _EmployeeId.Name; } }
        [Custom("AllowEdit", "False")]
        public DateTime Date
        {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
        public DayOfWeek Day
        {
            get { return _Day; }
            set { SetPropertyValue("Day", ref _Day, value); }
        }
        [Custom("AllowEdit", "False")]
        public string DayShort
        {
            get {
                switch (Day)
                {
                    case DayOfWeek.Friday:
                        return "FRI";
                    case DayOfWeek.Monday:
                        return "MON";
                    case DayOfWeek.Saturday:
                        return "SAT";
                    case DayOfWeek.Sunday:
                        return "SUN";
                    case DayOfWeek.Thursday:
                        return "THU";
                    case DayOfWeek.Tuesday:
                        return "TUE";
                    case DayOfWeek.Wednesday:
                        return "WED";
                    default:
                        break;
                }
                return string.Empty; }
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
        [Custom("AllowEdit", "False")]
        public TimeTable2 TimeTable
        {
            get { return _TimeTable; }
            set
            {
                bool modified = SetPropertyValue("TimeTable", ref _TimeTable, value);
                if (!IsLoading && !IsSaving)
                {
                    NightDiffRate = _TimeTable.NightDiffRate;
                    NormalOtRate = _TimeTable.NormalOtRate;
                    RestdayRate = _TimeTable.RestdayRate;
                    RestdayOtRate = _TimeTable.RestdayOtRate;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime OnDuty
        {
            get { return _OnDuty; }
            set
            {
                bool modified = SetPropertyValue("OnDuty", ref _OnDuty, value);
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime HalfDuty
        {
            get { return _HalfDuty; }
            set { bool modified = SetPropertyValue("HalfDuty", ref _HalfDuty, value);
            if (!IsLoading && !IsSaving && AttRecId != null && modified)
            {
                AttRecId.UpdateNoOfDays(false);
                AttRecId.UpdateOtHrs(false);
                AttRecId.UpdateOtNormal(false);
                AttRecId.UpdateSpgHldPrcnt(false);
                AttRecId.UpdateRegHldHrs(false);
                AttRecId.UpdateLateMins(false);
                AttRecId.UpdateRestdayDuty(false);
                AttRecId.UpdateRestdayOt(false);
                AttRecId.UpdateRegHolDuty(false);
                AttRecId.UpdateRegHolDutyAmt(false);
                AttRecId.UpdateNightHrs(false);
                AttRecId.UpdateRegHolOt(false);
                AttRecId.UpdateRegHolOtHrs(false);
                AttRecId.UpdateSPCHolDuty(false);
                AttRecId.UpdateSPCHolDutyAmt(false);
                AttRecId.UpdateSPCHolOt(false);
                AttRecId.UpdateSPCHolOtAmt(false);
                AttRecId.UpdateNightValue(false);
                AttRecId.UpdateDoubHolDuty(false);
                AttRecId.UpdateDoubHolDutyAmt(false);
                AttRecId.UpdateDoubHolOt(false);
                AttRecId.UpdateDoubHolOtHrs(false);
            }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime OffDuty
        {
            get { return _OffDuty; }
            set
            {
                bool modified = SetPropertyValue("OffDuty", ref _OffDuty, value);
            if (!IsLoading && !IsSaving && AttRecId != null && modified)
            {
                AttRecId.UpdateNoOfDays(false);
                AttRecId.UpdateOtHrs(false);
                AttRecId.UpdateOtNormal(false);
                AttRecId.UpdateSpgHldPrcnt(false);
                AttRecId.UpdateRegHldHrs(false);
                AttRecId.UpdateLateMins(false);
                AttRecId.UpdateRestdayDuty(false);
                AttRecId.UpdateRestdayOt(false);
                AttRecId.UpdateRegHolDuty(false);
                AttRecId.UpdateRegHolDutyAmt(false);
                AttRecId.UpdateNightHrs(false);
                AttRecId.UpdateRegHolOt(false);
                AttRecId.UpdateRegHolOtHrs(false);
                AttRecId.UpdateSPCHolDuty(false);
                AttRecId.UpdateSPCHolDutyAmt(false);
                AttRecId.UpdateSPCHolOt(false);
                AttRecId.UpdateSPCHolOtAmt(false);
                AttRecId.UpdateNightValue(false);
                AttRecId.UpdateDoubHolDuty(false);
                AttRecId.UpdateDoubHolDutyAmt(false);
                AttRecId.UpdateDoubHolOt(false);
                AttRecId.UpdateDoubHolOtHrs(false);
            }
            }
        }

        #region Clocks
        // First Set
        private DateTime _ClockIn1;
        private DateTime _BreakOut1;
        private DateTime _BreakIn1;
        private DateTime _ClockOut1;
        private bool _BreakClock1;
        private TimeSpan _FirstSetHrs;
        private int _Ref11;
        private int _Ref12;
        private int _Ref13;
        private int _Ref14;
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("C/In(01)")]
        [ImmediatePostData(true)]
        public DateTime ClockIn1
        {
            get { return _ClockIn1; }
            set
            {
                bool modified = SetPropertyValue("ClockIn1", ref _ClockIn1, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("B/Out(01)")]
        public DateTime BreakOut1
        {
            get { return _BreakOut1; }
            set
            {
                bool modified = SetPropertyValue("BreakOut1", ref _BreakOut1, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("B/In(01)")]
        public DateTime BreakIn1
        {
            get { return _BreakIn1; }
            set
            {
                bool modified = SetPropertyValue("BreakIn1", ref _BreakIn1, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("C/Out(01)")]
        public DateTime ClockOut1
        {
            get { return _ClockOut1; }
            set
            {
                bool modified = SetPropertyValue("ClockOut1", ref _ClockOut1, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool BreakClock1
        {
            get { return _BreakClock1; }
            set
            {
                bool modified = SetPropertyValue("BreakClock1", ref _BreakClock1, value);
            if (!IsLoading && !IsSaving && AttRecId != null && modified)
            {
                AttRecId.UpdateNoOfDays(false);
                AttRecId.UpdateOtHrs(false);
                AttRecId.UpdateOtNormal(false);
                AttRecId.UpdateSpgHldPrcnt(false);
                AttRecId.UpdateRegHldHrs(false);
                AttRecId.UpdateLateMins(false);
                AttRecId.UpdateRestdayDuty(false);
                AttRecId.UpdateRestdayOt(false);
                AttRecId.UpdateRegHolDutyAmt(false);
                AttRecId.UpdateNightHrs(false);
                AttRecId.UpdateRegHolOt(false);
                AttRecId.UpdateRegHolOtHrs(false);
                AttRecId.UpdateSPCHolDuty(false);
                AttRecId.UpdateSPCHolDutyAmt(false);
                AttRecId.UpdateSPCHolOt(false);
                AttRecId.UpdateSPCHolOtAmt(false);
                AttRecId.UpdateNightValue(false);
                AttRecId.UpdateDoubHolDuty(false);
                AttRecId.UpdateDoubHolDutyAmt(false);
                AttRecId.UpdateDoubHolOt(false);
                AttRecId.UpdateDoubHolOtHrs(false);
            }
            }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("1st(Hrs)")]
        public TimeSpan FirstSetHrs
        {
            get
            {
                TimeSpan vhrs01 = TimeTable.BreakOutTime - TimeTable.OnDutyTime;
                DateTime odt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, TimeTable.SecondSetCut.Second);
                DateTime odt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, TimeTable.SecondSetCut.Second);
                if (_ClockIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    odt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn1.Hour, _ClockIn1.Minute, _ClockIn1.Second); //_ClockIn1;
                }
                if (_ClockOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    odt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut1.Hour, _ClockOut1.Minute, _ClockOut1.Second); //_ClockOut1;
                }
                TimeSpan ots = odt02 - odt01;
                if (ots > vhrs01)
                {
                    ots = vhrs01;
                }
                return ots;
            }
        }
        [Custom("AllowEdit", "False")]
        public int Ref11
        {
            get { return _Ref11; }
            set { SetPropertyValue("Ref11", ref _Ref11, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref12
        {
            get { return _Ref12; }
            set { SetPropertyValue("Ref12", ref _Ref12, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref13
        {
            get { return _Ref13; }
            set { SetPropertyValue("Ref13", ref _Ref13, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref14
        {
            get { return _Ref14; }
            set { SetPropertyValue("Ref14", ref _Ref14, value); }
        }

        // Second Set
        private bool _NextDay1;
        private DateTime _ClockIn2;
        private DateTime _BreakOut2;
        private DateTime _BreakIn2;
        private DateTime _ClockOut2;
        private bool _BreakClock2;
        private TimeSpan _SecondSetHrs;
        private int _Ref21;
        private int _Ref22;
        private int _Ref23;
        private int _Ref24;
        [Custom("AllowEdit", "False")]
        public bool NextDay1
        {
            get { return _NextDay1; }
            set
            {
                bool modified = SetPropertyValue("NextDay1", ref _NextDay1, value);
            if (!IsLoading && !IsSaving && AttRecId != null && modified)
            {
                AttRecId.UpdateNoOfDays(false);
                AttRecId.UpdateOtHrs(false);
                AttRecId.UpdateOtNormal(false);
                AttRecId.UpdateSpgHldPrcnt(false);
                AttRecId.UpdateRegHldHrs(false);
                AttRecId.UpdateLateMins(false);
                AttRecId.UpdateRestdayDuty(false);
                AttRecId.UpdateRestdayOt(false);
                AttRecId.UpdateRegHolDuty(false);
                AttRecId.UpdateRegHolDutyAmt(false);
                AttRecId.UpdateNightHrs(false);
                AttRecId.UpdateRegHolOt(false);
                AttRecId.UpdateRegHolOtHrs(false);
                AttRecId.UpdateSPCHolDuty(false);
                AttRecId.UpdateSPCHolDutyAmt(false);
                AttRecId.UpdateSPCHolOt(false);
                AttRecId.UpdateSPCHolOtAmt(false);
                AttRecId.UpdateNightValue(false);
                AttRecId.UpdateDoubHolDuty(false);
                AttRecId.UpdateDoubHolDutyAmt(false);
                AttRecId.UpdateDoubHolOt(false);
                AttRecId.UpdateDoubHolOtHrs(false);
            }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("C/In(02)")]
        public DateTime ClockIn2
        {
            get { return _ClockIn2; }
            set
            {
                bool modified = SetPropertyValue("ClockIn2", ref _ClockIn2, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
                //if (!IsLoading)
                //{
                //    TimeSpan vhrs02 = TimeTable.OffDutyTime - TimeTable.HalfDutyTime;
                //    DateTime sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(1).Hour, TimeTable.SecondSetCut.AddHours(1).Minute, TimeTable.SecondSetCut.AddHours(1).Second);
                //    DateTime sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(1).Hour, TimeTable.SecondSetCut.AddHours(1).Minute, TimeTable.SecondSetCut.AddHours(1).Second);
                //    if (_ClockIn2 != new DateTime(1753, 1, 1, 0, 0, 0))
                //    {
                //        sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn2.Hour, _ClockIn2.Minute, _ClockIn2.Second); //_ClockIn2;
                //    }
                //    if (_ClockOut2 != new DateTime(1753, 1, 1, 0, 0, 0))
                //    {
                //        sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut2.Hour, _ClockOut2.Minute, _ClockOut2.Second); //_ClockOut2;
                //    }
                //    TimeSpan sts = sdt02 - sdt01;
                //    if (_ClockIn2 == new DateTime(1753, 1, 1, 0, 0, 0) && sts > vhrs02)
                //    {
                //        sts = vhrs02;
                //    }
                //    else if (_ClockIn2 != new DateTime(1753, 1, 1, 0, 0, 0) && sts > vhrs02)
                //    {

                //    }
                //}
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("B/Out(02)")]
        public DateTime BreakOut2
        {
            get { return _BreakOut2; }
            set
            {
                bool modified = SetPropertyValue("BreakOut2", ref _BreakOut2, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("B/In(02)")]
        public DateTime BreakIn2
        {
            get { return _BreakIn2; }
            set
            {
                bool modified = SetPropertyValue("BreakIn2", ref _BreakIn2, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("C/Out(02)")]
        public DateTime ClockOut2
        {
            get { return _ClockOut2; }
            set
            {
                bool modified = SetPropertyValue("ClockOut2", ref _ClockOut2, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool BreakClock2
        {
            get { return _BreakClock2; }
            set
            {
                bool modified = SetPropertyValue("BreakClock2", ref _BreakClock2, value);
            if (!IsLoading && !IsSaving && AttRecId != null && modified)
            {
                AttRecId.UpdateNoOfDays(false);
                AttRecId.UpdateOtHrs(false);
                AttRecId.UpdateSpgHldPrcnt(false);
                AttRecId.UpdateRegHldHrs(false);
                AttRecId.UpdateLateMins(false);
            }
            }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("2nd(Hrs)")]
        public TimeSpan SecondSetHrs
        {
            get
            {
                TimeSpan vhrs02 = TimeTable.OffDutyTime - TimeTable.HalfDutyTime;
                DateTime sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(Convert.ToDouble(TimeTable.BreakHours)).Hour, TimeTable.SecondSetCut.AddHours(Convert.ToDouble(TimeTable.BreakHours)).Minute, TimeTable.SecondSetCut.AddHours(Convert.ToDouble(TimeTable.BreakHours)).Second);
                DateTime sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(Convert.ToDouble(TimeTable.BreakHours)).Hour, TimeTable.SecondSetCut.AddHours(Convert.ToDouble(TimeTable.BreakHours)).Minute, TimeTable.SecondSetCut.AddHours(Convert.ToDouble(TimeTable.BreakHours)).Second);
                if (_ClockIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn2.Hour, _ClockIn2.Minute, _ClockIn2.Second); //_ClockIn2;
                }
                if (_ClockOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut2.Hour, _ClockOut2.Minute, _ClockOut2.Second); //_ClockOut2;
                }
                TimeSpan sts = sdt02 - sdt01;
                if (sts > vhrs02)
                {
                    sts = vhrs02;
                }
                return sts;
            }
        }
        [Custom("AllowEdit", "False")]
        public int Ref21
        {
            get { return _Ref21; }
            set { SetPropertyValue("Ref21", ref _Ref21, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref22
        {
            get { return _Ref22; }
            set { SetPropertyValue("Ref22", ref _Ref22, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref23
        {
            get { return _Ref23; }
            set { SetPropertyValue("Ref23", ref _Ref23, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref24
        {
            get { return _Ref24; }
            set { SetPropertyValue("Ref24", ref _Ref24, value); }
        }
        // Zero Set
        private bool _NextDay2;
        private DateTime _OvertimeIn1;
        private DateTime _OvertimeOut1;
        private TimeSpan _ZeroSetHrs;
        private int _Ref31;
        private int _Ref32;
        [Custom("AllowEdit", "False")]
        public bool NextDay2
        {
            get { return _NextDay2; }
            set
            {
                bool modified = SetPropertyValue("NextDay2", ref _NextDay2, value);
            if (!IsLoading && !IsSaving && AttRecId != null && modified)
            {
                AttRecId.UpdateNoOfDays(false);
                AttRecId.UpdateOtHrs(false);
                AttRecId.UpdateSpgHldPrcnt(false);
                AttRecId.UpdateRegHldHrs(false);
                AttRecId.UpdateLateMins(false);
            }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("O/In(00)")]
        public DateTime OvertimeIn1
        {
            get { return _OvertimeIn1; }
            set
            {
                bool modified = SetPropertyValue("OvertimeIn1", ref _OvertimeIn1, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("O/Out(00)")]
        public DateTime OvertimeOut1
        {
            get { return _OvertimeOut1; }
            set
            {
                bool modified = SetPropertyValue("OvertimeOut1", ref _OvertimeOut1, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Zero(Hrs)")]
        public TimeSpan ZeroSetHrs
        {
            get
            {
                DateTime zdt01 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                DateTime zdt02 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                if (_OvertimeIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    zdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn1.Hour, _OvertimeIn1.Minute, _OvertimeIn1.Second); //_OvertimeIn1;
                }
                if (_OvertimeOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    zdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut1.Hour, _OvertimeOut1.Minute, _OvertimeOut1.Second); //_OvertimeOut1;
                }
                return zdt02 - zdt01;
            }
        }
        private TimeSpan _ZeroNightHrs;
        [Custom("AllowEdit", "False")]
        [DisplayName("ZeroN(Hrs)")]
        public TimeSpan ZeroNightHrs
        {
            get { return _ZeroNightHrs; }
            set { SetPropertyValue("ZeroNightHrs", ref _ZeroNightHrs, value); }
        }

        [Custom("AllowEdit", "False")]
        public int Ref31
        {
            get { return _Ref31; }
            set { SetPropertyValue("Ref31", ref _Ref31, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref32
        {
            get { return _Ref32; }
            set { SetPropertyValue("Ref32", ref _Ref32, value); }
        }

        // Third Set
        private DateTime _OvertimeIn2;
        private DateTime _OvertimeOut2;
        private TimeSpan _ThirdSetHrs;
        private int _Ref33;
        private int _Ref34;
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("O/In(03)")]
        public DateTime OvertimeIn2
        {
            get { return _OvertimeIn2; }
            set
            {
                bool modified = SetPropertyValue("OvertimeIn2", ref _OvertimeIn2, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        [DisplayName("O/Out(03)")]
        public DateTime OvertimeOut2
        {
            get { return _OvertimeOut2; }
            set
            {
                bool modified = SetPropertyValue("OvertimeOut2", ref _OvertimeOut2, value);
                if (!IsLoading && !IsSaving)
                {
                    UnAltered = false;
                }
                if (!IsLoading && !IsSaving && AttRecId != null && modified)
                {
                    AttRecId.UpdateNoOfDays(false);
                    AttRecId.UpdateOtHrs(false);
                    AttRecId.UpdateOtNormal(false);
                    AttRecId.UpdateSpgHldPrcnt(false);
                    AttRecId.UpdateRegHldHrs(false);
                    AttRecId.UpdateLateMins(false);
                    AttRecId.UpdateRestdayDuty(false);
                    AttRecId.UpdateRestdayOt(false);
                    AttRecId.UpdateRegHolDuty(false);
                    AttRecId.UpdateRegHolDutyAmt(false);
                    AttRecId.UpdateNightHrs(false);
                    AttRecId.UpdateRegHolOt(false);
                    AttRecId.UpdateRegHolOtHrs(false);
                    AttRecId.UpdateSPCHolDuty(false);
                    AttRecId.UpdateSPCHolDutyAmt(false);
                    AttRecId.UpdateSPCHolOt(false);
                    AttRecId.UpdateSPCHolOtAmt(false);
                    AttRecId.UpdateNightValue(false);
                    AttRecId.UpdateDoubHolDuty(false);
                    AttRecId.UpdateDoubHolDutyAmt(false);
                    AttRecId.UpdateDoubHolOt(false);
                    AttRecId.UpdateDoubHolOtHrs(false);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("3rd(Hrs)")]
        public TimeSpan ThirdSetHrs
        {
            get
            {
                if (LineID == "10147 08-20-2018 Dayshift")
                {

                }
                //DateTime tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                //DateTime tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                DateTime tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Hour, TimeTable.ThirdSetCut.Minute, 0);
                DateTime tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, 0);
                if (TimeTable.LastCut.Hour == 0)
                {
                    tdt02 = new DateTime(Date.AddDays(1).Year, Date.AddDays(1).Month, Date.AddDays(1).Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, 0);
                }
                if (_OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn2.Hour, _OvertimeIn2.Minute, _OvertimeIn2.Second); //_OvertimeIn2;
                }
                if (_OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut2.Hour, _OvertimeOut2.Minute, _OvertimeOut2.Second); //_OvertimeOut2;
                }
                TimeSpan tts = tdt02 - tdt01;
                if (_OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day) || _OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day))
                {
                    return tts;
                }
                else
                {
                    return new TimeSpan(0, 0, 0);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public int Ref33
        {
            get { return _Ref33; }
            set { SetPropertyValue("Ref33", ref _Ref33, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Ref34
        {
            get { return _Ref34; }
            set { SetPropertyValue("Ref34", ref _Ref34, value); }
        }
        private TimeSpan _ThirdNightHrs;
        [Custom("AllowEdit", "False")]
        [DisplayName("ThirdN(Hrs)")]
        public TimeSpan ThirdNightHrs
        {
            get
            {
                DateTime tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                DateTime tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                if (_OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn2.Hour, _OvertimeIn2.Minute, _OvertimeIn2.Second); //_OvertimeIn2;
                }
                if (_OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                {
                    tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut2.Hour, _OvertimeOut2.Minute, _OvertimeOut2.Second); //_OvertimeOut2;
                }
                DateTime tndstart = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.NightStartTime.Hour, TimeTable.NightStartTime.Minute, TimeTable.NightStartTime.Second);
                DateTime tndend = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, TimeTable.LastCut.Second);

                DateTime tnt01;
                DateTime tnt02;
                if (tdt01 >= tndstart)
                {
                    tnt01 = tdt01;
                }
                else
                {
                    tnt01 = tndstart;
                }
                if (tdt02 <= tndend)
                {
                    tnt02 = tdt02;
                }
                else
                {
                    tnt02 = tndend;
                }
                return tnt02 - tnt01;
            }
        }
        #endregion

        [Custom("AllowEdit", "False")]
        public decimal Normal
        {
            get { return _Normal; }
            set { SetPropertyValue("Normal", ref _Normal, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal RealTime
        {
            get { return _RealTime; }
            set { SetPropertyValue("RealTime", ref _RealTime, value); }
        }
        //[Custom("AllowEdit", "False")]
        //public TimeSpan NormalHours
        //{
        //    get { return _NormalHours; }
        //    set { SetPropertyValue("NormalHours", ref _NormalHours, value); }
        [PersistentAlias("OffDuty - OnDuty")]
        [Custom("AllowEdit", "False")]
        public TimeSpan NormalHours
        {
            get
            {
                object tempObject = EvaluateAlias("NormalHours");
                if (tempObject != null)
                {
                    return ((TimeSpan)tempObject).Subtract(TimeSpan.FromHours((double)_TimeTable.BreakHours));
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        //public TimeSpan ActualLate
        //{
        //    get { return _ActualLate; }
        //    set { SetPropertyValue("ActualLate", ref _ActualLate, value); }
        //}
        [PersistentAlias("TimeAdd(FirstLate, SecondLate)")]
        [Custom("AllowEdit", "False")]
        public TimeSpan ActualLate
        {
            get
            {
                object tempObject = EvaluateAlias("ActualLate");
                if (tempObject != null && !((TimeSpan)tempObject > TimeSpan.FromHours(24d)))
                {
                    return (TimeSpan)tempObject;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [PersistentAlias("OnDuty - ClockIn1")]
        [Custom("AllowEdit", "False")]
        public TimeSpan FirstLate
        {
            get
            {
                //if (_LineID == "10196 09-14-2016 Dayshift")
                //{

                //}
                object tempObject = EvaluateAlias("FirstLate");
                if (tempObject != null)
                {
                    //if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => Remarks.Contains(o)))
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    if (TotalHrs != TimeSpan.Zero && FirstSetHrs != TimeSpan.Zero && (TimeSpan)tempObject < TimeSpan.Zero)
                    {
                        return ((TimeSpan)tempObject).Negate();
                    }
                    else if (TotalHrs != TimeSpan.Zero && FirstSetHrs == TimeSpan.Zero && SecondSetHrs != TimeSpan.Zero)
                    {
                        DateTime secondsetcut = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, 0);
                        return secondsetcut - OnDuty;
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        public DateTime Scuut
        {
            get { return new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, 0); }
        }
        [PersistentAlias("FirstSetHrs")]
        [Custom("AllowEdit", "False")]
        public TimeSpan FirstEarly
        {
            get
            {
                object tempObject = EvaluateAlias("FirstEarly");
                if (tempObject != null)
                {
                    //if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => Remarks.Contains(o)))
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    TimeSpan newVariable = Scuut - ClockOut1;
                    //if (TotalHrs != TimeSpan.Zero && FirstSetHrs != TimeSpan.Zero && !(newVariable > TimeSpan.FromHours(8d)))
                    //{
                    //    return newVariable;
                    //}
                    //else
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    if (FirstSetHrs != TimeSpan.Zero && !(newVariable > TimeSpan.FromHours(8d)))
                    {
                        return newVariable;
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [PersistentAlias("HalfDuty - ClockIn2")]
        [Custom("AllowEdit", "False")]
        public TimeSpan SecondLate
        {
            get
            {
                object tempObject = EvaluateAlias("SecondLate");
                if (tempObject != null)
                {
                    //if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => Remarks.Contains(o)))
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    if (SecondSetHrs != TimeSpan.Zero && (TimeSpan)tempObject < TimeSpan.Zero)
                    {

                        return ((TimeSpan)tempObject).Negate();
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [PersistentAlias("OffDuty - ClockOut2")]
        [Custom("AllowEdit", "False")]
        public TimeSpan SecondEarly
        {
            get
            {
                object tempObject = EvaluateAlias("SecondEarly");
                if (tempObject != null)
                {
                    //if (TotalHrs != TimeSpan.Zero && FirstSetHrs != TimeSpan.Zero && !((TimeSpan)tempObject < TimeSpan.Zero))
                    //{
                    //    return (TimeSpan)tempObject;
                    //}
                    //else
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    //if (new[] { "Holiday RG", "Holiday RGOT" }.Any(o => Remarks.Contains(o)))
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    if (SecondSetHrs != TimeSpan.Zero && !((TimeSpan)tempObject < TimeSpan.Zero))
                    {
                        return (TimeSpan)tempObject;
                    }
                    else if (SecondSetHrs == TimeSpan.Zero)
                    {
                        //double dutyHrs2 = (_TimeTable.OffDutyTime - _TimeTable.HalfDutyTime).TotalHours;
                        //return TimeSpan.FromHours(4d);
                        return _TimeTable.OffDutyTime - _TimeTable.HalfDutyTime;
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        //public TimeSpan Late
        //{
        //    get { return _Late; }
        //    set { SetPropertyValue("Late", ref _Late, value); }
        //}
        [PersistentAlias("ActualLate")]
        [Custom("AllowEdit", "False")]
        public TimeSpan Late
        {
            get
            {
                object tempObject = EvaluateAlias("Late");
                if (tempObject != null)
                {
                    //if (new[] { "Holiday SP" }.Any(o => Remarks.Contains(o)))
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    if (TotalHrs != TimeSpan.Zero && (TimeSpan)tempObject >= TimeSpan.FromMinutes(_TimeTable.LateTimeMins))
                    {

                        return (TimeSpan)tempObject;
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        //public TimeSpan ActualEarly
        //{
        //    get { return _ActualEarly; }
        //    set { SetPropertyValue("ActualEarly", ref _ActualEarly, value); }
        //}
        [PersistentAlias("TimeAdd(FirstEarly, SecondEarly)")]
        //[PersistentAlias("ClockOut2 - OffDuty")]
        [Custom("AllowEdit", "False")]
        public TimeSpan ActualEarly
        {
            get
            {
                object tempObject = EvaluateAlias("ActualEarly");
                if (tempObject != null && !((TimeSpan)tempObject > TimeSpan.FromHours(24d)))
                {
                    //if (TotalHrs != TimeSpan.Zero && _RestDayOfTheWeek != Date.DayOfWeek && (TimeSpan)tempObject < TimeSpan.Zero)
                    //{
                    //    return ((TimeSpan)tempObject).Negate();
                    //}
                    //else
                    //{
                    //    return TimeSpan.Zero;
                    //}
                    return (TimeSpan)tempObject;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [PersistentAlias("ActualEarly")]
        [Custom("AllowEdit", "False")]
        public TimeSpan Early
        {
            get
            {
                object tempObject = EvaluateAlias("Early");
                if (tempObject != null)
                {
                    if (TotalHrs != TimeSpan.Zero && (TimeSpan)tempObject >= TimeSpan.FromMinutes(_TimeTable.LeaveEarlyTimeMins))
                    {

                        return (TimeSpan)tempObject;
                    }
                    else
                    {
                        return TimeSpan.Zero;
                    }
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public HolidayTypeEnum HolidayType
        {
            get
            {
                //XPCollection<Holiday> hols = new XPCollection<Holiday>(Session);
                Holiday firstOrDefault = null;
                if (!IsSaving && !IsLoading)
                {
                    firstOrDefault = AttRecId.BatchID.Holidays.Where(o => o.Date == Date).FirstOrDefault();
                }
                if (firstOrDefault != null)
                {
                    return firstOrDefault.HolidayType;
                }
                else
                {
                    return HolidayTypeEnum.None;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public bool Absent
        {
            get
            {
                if (_RestDayOfTheWeek != Date.DayOfWeek && FirstSetHrs == TimeSpan.Zero && SecondSetHrs == TimeSpan.Zero)
                {
                    switch (HolidayType)
                    {
                        case HolidayTypeEnum.Special:
                            return true;
                        case HolidayTypeEnum.Regular:
                            return false;
                        case HolidayTypeEnum.Double:
                            return false;
                        case HolidayTypeEnum.None:
                            return true;
                        default:
                            return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public decimal AbsentCount
        {
            get
            {
                if (Absent)
                {
                    return 1m;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan AbsentHours
        {
            get
            {
                if (Absent)
                {
                    return TimeSpan.FromHours((double)_TimeTable.CountAsHours);
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        private TimeSpan _OnExcess;
        private TimeSpan _OffExcess;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan OnExcess
        {
            get
            {
                if (ValidWorkHours != TimeSpan.Zero && _ClockIn1 < _OnDuty)
                {
                    _OnExcess = (_ClockIn1 - _OnDuty).Negate() > TimeSpan.FromDays(1d) ? TimeSpan.Zero : (_ClockIn1 - _OnDuty).Negate();
                    return _OnExcess;
                }
                else
                {
                    _OnExcess = TimeSpan.Zero;
                    return _OnExcess;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan OffExcess
        {
            get
            {
                if (ValidWorkHours != TimeSpan.Zero && _ClockOut2 > _OffDuty)
                {
                    _OffExcess = (_OffDuty - _ClockOut2).Negate();
                    return _OffExcess;
                }
                else
                {
                    _OffExcess = TimeSpan.Zero;
                    return _OffExcess;
                }
            }
        }
        [PersistentAlias("TimeAdd(ZeroSetHrs, ThirdSetHrs)")]
        [Custom("AllowEdit", "False")]
        public TimeSpan OtHours
        {
            //get {
            //    //TimeSpan times = (TotalHrs - NormalHours) - (OnExcess + OffExcess);
            //    TimeSpan times = (ZeroSetHrs + ThirdSetHrs) - (OnExcess + OffExcess);
            //    return times > TimeSpan.Zero ? times : TimeSpan.Zero;
            //}
            //get
            //{
            //    object tempObject = EvaluateAlias("OtHours");
            //    TimeSpan times;
            //    if (tempObject != null) times = (TimeSpan)tempObject - (OnExcess + OffExcess); else times = TimeSpan.Zero;
            //    return times > TimeSpan.Zero ? times : TimeSpan.Zero;
            //}
            get
            {
                object tempObject = EvaluateAlias("OtHours");
                if (tempObject != null)
                {
                    return (TimeSpan)tempObject;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan ValidOtHours
        {
            get
            {
                if (OtHours.TotalMinutes >= _TimeTable.MinimumOtMins)
                {
                    //if (_OtStatus != OtStatusEnum.Approved || _OtStatus != OtStatusEnum.Disapproved)
                    //{
                    //    _OtStatus = OtStatusEnum.Pending;
                    //}
                    return OtHours;
                }
                else
                {
                    //_OtStatus = OtStatusEnum.None;
                    return TimeSpan.Zero;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan ActualHours
        {
            get { return _ActualHours; }
            set { SetPropertyValue("ActualHours", ref _ActualHours, value); }
        }
        //[PersistentAlias("FirstSetHrs + SecondSetHrs + ThirdSetHrs + ZeroSetHrs")]
        [PersistentAlias("TimeAdd(FirstSetHrs, SecondSetHrs, ThirdSetHrs, ZeroSetHrs)")]
        [Custom("AllowEdit", "False")]
        [DisplayName("Actual Hrs")]
        public TimeSpan TotalHrs
        {
            get
            {
                object tempObject = EvaluateAlias("TotalHrs");
                if (tempObject != null)
                {
                    return (TimeSpan)tempObject;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan ValidWorkHours
        {
            get
            {
                if (TotalHrs > TimeSpan.Zero && TotalHrs <= TimeSpan.FromHours((double)_TimeTable.CountAsHours))
                {
                    return TotalHrs;
                }
                else if (TotalHrs > TimeSpan.Zero && TotalHrs > TimeSpan.FromHours((double)_TimeTable.CountAsHours))
                {
                    return TotalHrs - (_OnExcess + _OffExcess);
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        private bool _ApprovedOt = false;
        [Custom("AllowEdit", "False")]
        public bool ApprovedOt
        {
            get { return _ApprovedOt; }
            set { SetPropertyValue("ApprovedOt", ref _ApprovedOt, value); }
        }
        private bool _DisapprovedOt = false;
        [Custom("AllowEdit", "False")]
        public bool DisapprovedOt
        {
            get { return _DisapprovedOt; }
            set { SetPropertyValue("DisapprovedOt", ref _DisapprovedOt, value); }
        }
        [Custom("AllowEdit", "False")]
        public OtStatusEnum OtStatus
        {
            get
            {
                if (!Absent && _Invalid)
                {
                    return OtStatusEnum.None;
                }
                if (_RestDayOfTheWeek == Date.DayOfWeek && ValidWorkHours == TimeSpan.Zero)
                {
                    return OtStatusEnum.None;
                }
                else if (_RestDayOfTheWeek == Date.DayOfWeek && ValidWorkHours > TimeSpan.Zero)
                {
                    if (_DisapprovedOt)
                    {
                        return OtStatusEnum.Disapproved;
                    }
                    else if (_ApprovedOt)
                    {
                        return OtStatusEnum.Approved;
                    }
                    else
                    {
                        return OtStatusEnum.Pending;
                    }
                }
                if (ValidWorkHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
                {
                    return OtStatusEnum.None;
                }
                if (ValidWorkHours > TimeSpan.Zero && ValidOtHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
                {
                    return OtStatusEnum.None;
                }
                else if (ValidWorkHours > TimeSpan.Zero && ValidOtHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
                {
                    if (_DisapprovedOt)
                    {
                        return OtStatusEnum.Disapproved;
                    }
                    else if (_ApprovedOt)
                    {
                        return OtStatusEnum.Approved;
                    }
                    else
                    {
                        return OtStatusEnum.Pending;
                    }
                }
                if (ValidWorkHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Regular)
                {
                    return OtStatusEnum.None;
                }
                else if (ValidWorkHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Regular)
                {
                    if (_DisapprovedOt)
                    {
                        return OtStatusEnum.Disapproved;
                    }
                    else if (_ApprovedOt)
                    {
                        return OtStatusEnum.Approved;
                    }
                    else
                    {
                        return OtStatusEnum.Pending;
                    }
                }
                if (ValidWorkHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Double)
                {
                    return OtStatusEnum.None;
                }
                else if (ValidWorkHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Double)
                {
                    if (_DisapprovedOt)
                    {
                        return OtStatusEnum.Disapproved;
                    }
                    else if (_ApprovedOt)
                    {
                        return OtStatusEnum.Approved;
                    }
                    else
                    {
                        return OtStatusEnum.Pending;
                    }
                }
                if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && ValidWorkHours >= TimeSpan.FromHours((double)_TimeTable.CountAsHours))
                {
                    return OtStatusEnum.None;
                }
                else if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && !_Invalid && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && Late > TimeSpan.Zero)
                {
                    return OtStatusEnum.None;
                }
                else if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && !_Invalid && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && Early > TimeSpan.Zero)
                {
                    return OtStatusEnum.None;
                }
                else if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && !_Invalid && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && ValidWorkHours < TimeSpan.FromHours((double)_TimeTable.CountAsHours))
                {
                    return OtStatusEnum.None;
                }
                if (OtHours.TotalMinutes >= _TimeTable.MinimumOtMins && HolidayType == HolidayTypeEnum.None)
                {
                    if (_DisapprovedOt)
                    {
                        return OtStatusEnum.Disapproved;
                    }
                    else if (_ApprovedOt)
                    {
                        return OtStatusEnum.Approved;
                    }
                    else
                    {
                        return OtStatusEnum.Pending;
                    }
                }
                if (Absent)
                {
                    return OtStatusEnum.None;
                }
                return OtStatusEnum.None;
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan RestDay
        {
            get
            {
                if (_RestDayOfTheWeek == Date.DayOfWeek && ValidWorkHours > TimeSpan.Zero)
                {
                    return ValidWorkHours;
                }
                return TimeSpan.Zero;
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TimeSpan Holiday
        {
            get
            {
                if (ValidWorkHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
                {
                    return ValidWorkHours;
                }
                else if (ValidWorkHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Regular)
                {
                    return ValidWorkHours;
                }
                return TimeSpan.Zero;
            }
        }
        //[PersistentAlias("TimeAdd(ZeroNightHrs, ThirdNightHrs)")]
        //[Custom("AllowEdit", "False")]
        //public TimeSpan Night
        //{
        //    get
        //    {
        //        object tempObject = EvaluateAlias("Night");
        //        if (tempObject != null && (TimeSpan)tempObject > TimeSpan.Zero)
        //        {
        //            return (TimeSpan)tempObject;
        //        }
        //        else
        //        {
        //            return TimeSpan.Zero;
        //        }
        //    }
        //}

        //public string Night0 {
        //    get {
        //        if (LineID == "10024 08-11-2018 Dayshift")
        //        {

        //        }
        //        string nd = string.Empty;
        //        // 10/10 00:00
        //        DateTime n1Start = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
        //        // 10/10 05:00
        //        DateTime n1End = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.NightEndTime.Hour, _TimeTable.NightEndTime.Minute, 0);
        //        TimeSpan ts1 = n1End - n1Start;
        //        // 10/10 22:00
        //        DateTime n2Start = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.NightStartTime.Hour, _TimeTable.NightStartTime.Minute, 0);
        //        // 10/11 00:00
        //        DateTime n2End = (new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0)).AddDays(1);
        //        TimeSpan ts2 = n2End - n2Start;
        //        if (ZeroSetHrs.TotalHours != 0)
        //        {
        //            DateTime zdt01 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
        //            DateTime zdt02 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
        //            if (_OvertimeIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                zdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn1.Hour, _OvertimeIn1.Minute, _OvertimeIn1.Second); //_OvertimeIn1;
        //            }
        //            if (_OvertimeOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                zdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut1.Hour, _OvertimeOut1.Minute, _OvertimeOut1.Second); //_OvertimeOut1;
        //            }
        //            if ((n1Start <= zdt01 && zdt01 <= n1End) && (n1Start <= zdt02 && zdt02 <= n1End))
        //            {
        //                nd = "0Exact 1ND";
        //            }
        //            else if ((n1Start <= zdt01 && zdt01 <= n1End) && (n1Start <= zdt02 && zdt02 > n1End))
        //            {
        //                nd = "01ND Plus";
        //            }
        //            else if ((n2Start <= zdt01 && zdt01 <= n2End) && (n2Start <= zdt02 && zdt02 <= n2End))
        //            {
        //                nd = "0Exact 2ND";
        //            }
        //            else if ((n2Start <= zdt01 && zdt01 <= n2End) && (n2Start <= zdt02 && zdt02 > n2End))
        //            {
        //                nd = "02ND Plus";
        //            }
        //            //else
        //            //{
        //            //    nd = "0None";
        //            //}
        //        } 
        //        if (FirstSetHrs.TotalHours != 0)
        //        {
        //            DateTime odt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, TimeTable.SecondSetCut.Second);
        //            DateTime odt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, TimeTable.SecondSetCut.Second);
        //            if (_ClockIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                odt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn1.Hour, _ClockIn1.Minute, _ClockIn1.Second); //_ClockIn1;
        //            }
        //            if (_ClockOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                odt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut1.Hour, _ClockOut1.Minute, _ClockOut1.Second); //_ClockOut1;
        //            }
        //            //   10/10 00:00  8:00      8:00        5:00       10/10 00:00   12:00     12:00        5:00              
        //            if ((n1Start <= odt01 && odt01 <= n1End) && (n1Start <= odt02 && odt02 <= n1End))
        //            {
        //                nd = "1Exact 1ND";
        //            }
        //            //        10/10 00:00  8:00      8:00        5:00       10/10 00:00   12:00     12:00        5:00 
        //            else if ((n1Start <= odt01 && odt01 <= n1End) && (n1Start <= odt02 && odt02 > n1End))
        //            {
        //                nd = "11ND Plus";
        //            }
        //            //        10/10 22:00  8:00      8:00  10/11 00:00       10/10 22:00   12:00     12:00        10/11 00:00
        //            else if ((n2Start <= odt01 && odt01 <= n2End) && (n2Start <= odt02 && odt02 <= n2End))
        //            {
        //                nd = "1Exact 2ND";
        //            }
        //            //        10/10 22:00  8:00      8:00  10/11 00:00       10/10 22:00   12:00     12:00        10/11 00:00
        //            else if ((n2Start <= odt01 && odt01 <= n2End) && (n2Start <= odt02 && odt02 > n2End))
        //            {
        //                nd = "12ND Plus";
        //            }
        //            //else
        //            //{
        //            //    nd = "1None";
        //            //}
        //        }
        //        if (SecondSetHrs.TotalHours != 0)
        //        {
        //            DateTime sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(1).Hour, TimeTable.SecondSetCut.AddHours(1).Minute, TimeTable.SecondSetCut.AddHours(1).Second);
        //            DateTime sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(1).Hour, TimeTable.SecondSetCut.AddHours(1).Minute, TimeTable.SecondSetCut.AddHours(1).Second);
        //            if (_ClockIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn2.Hour, _ClockIn2.Minute, _ClockIn2.Second); //_ClockIn2;
        //            }
        //            if (_ClockOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut2.Hour, _ClockOut2.Minute, _ClockOut2.Second); //_ClockOut2;
        //            }
        //            //   10/10 00:00  13:00      13:00        5:00       10/10 00:00   17:00     17:00        5:00 
        //            if ((n1Start <= sdt01 && sdt01 <= n1End) && (n1Start <= sdt02 && sdt02 <= n1End))
        //            {
        //                nd = "2Exact 1ND";
        //            }
        //            //        10/10 00:00  13:00      13:00        5:00       10/10 00:00   17:00     17:00        5:00 
        //            else if ((n1Start <= sdt01 && sdt01 <= n1End) && (n1Start <= sdt02 && sdt02 > n1End))
        //            {
        //                nd = "21ND Plus";
        //            }
        //            //        10/10 22:00  13:00      13:00   10/11 00:00   10/10 22:00   17:00     17:00        10/11 00:00
        //            else if ((n2Start <= sdt01 && sdt01 <= n2End) && (n2Start <= sdt02 && sdt02 <= n2End))
        //            {
        //                nd = "2Exact 2ND";
        //            }
        //            //        10/10 22:00  13:00      13:00   10/11 00:00   10/10 22:00   17:00     17:00        10/11 00:00
        //            else if ((n2Start <= sdt01 && sdt01 <= n2End) && (n2Start <= sdt02 && sdt02 > n2End))
        //            {
        //                nd = "22ND Plus";
        //            }
        //            //else
        //            //{
        //            //    nd = "2None";
        //            //}
        //        }
        //        if (ThirdSetHrs.TotalHours != 0)
        //        {
        //            DateTime tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Hour, TimeTable.ThirdSetCut.Minute, 0);
        //            DateTime tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, 0);
        //            if (TimeTable.LastCut.Hour == 0)
        //            {
        //                tdt02 = new DateTime(Date.AddDays(1).Year, Date.AddDays(1).Month, Date.AddDays(1).Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, 0);
        //            }
        //            if (_OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn2.Hour, _OvertimeIn2.Minute, _OvertimeIn2.Second); //_OvertimeIn2;
        //            }
        //            if (_OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
        //            {
        //                tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut2.Hour, _OvertimeOut2.Minute, _OvertimeOut2.Second); //_OvertimeOut2;
        //            }
        //            if ((n1Start <= tdt01 && tdt01 <= n1End) && (n1Start <= tdt02 && tdt02 <= n1End))
        //            {
        //                nd = "3Exact 1ND";
        //            }
        //            else if ((n1Start <= tdt01 && tdt01 <= n1End) && (n1Start <= tdt02 && tdt02 > n1End))
        //            {
        //                nd = "31ND Plus";
        //            }
        //            else if ((n2Start <= tdt01 && tdt01 <= n2End) && (n2Start <= tdt02 && tdt02 <= n2End))
        //            {
        //                nd = "3Exact 2ND";
        //            }
        //            else if ((n2Start <= tdt01 && tdt01 <= n2End) && (n2Start <= tdt02 && tdt02 > n2End))
        //            {
        //                nd = "32ND Plus";
        //            }
        //            //else
        //            //{
        //            //    nd = "3None";
        //            //}
        //        }
        //        //if (OvertimeIn1 != new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.ZeroSetCut.Hour, _TimeTable.ZeroSetCut.Minute, 0))
        //        //{

        //        //}
        //        return nd; }
        //}
        //public double Normal0
        //{
        //    get
        //    {
        //        DateTimeInterval timeInterval1;
        //        switch (_TimeTable.NightOrDay)
        //        {
        //            case NightOrDayEnum.Day:
        //                timeInterval1 = new DateTimeInterval()
        //                {
        //                    From = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.OnDutyTime.Hour, _TimeTable.OnDutyTime.Minute, 00),
        //                    To = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.OffDutyTime.Hour, _TimeTable.OffDutyTime.Minute, 00)
        //                };
        //                break;
        //            case NightOrDayEnum.Night:
        //                timeInterval1 = new DateTimeInterval()
        //                {
        //                    From = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.OnDutyTime.Hour, _TimeTable.OnDutyTime.Minute, 00),
        //                    To = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.OffDutyTime.Hour, _TimeTable.OffDutyTime.Minute, 00).AddDays(1)
        //                };
        //                break;
        //            default:
        //                break;
        //        }
        //        return 0;
        //    }
        //}
        public double Night0
        {
            get
            {
                if (_LineID == "10147 06-08-2019 Dayshift")
                {

                }
                double dbl = 0;
                var nightTime1 = new DateTimeInterval()
                {
                    From = new DateTime(Date.Year, Date.Month, Date.Day, 00, 00, 00),
                    To = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.NightEndTime.Hour, _TimeTable.NightEndTime.Minute, 00)
                };
                var nightTime2 = new DateTimeInterval()
                {
                    From = new DateTime(Date.Year, Date.Month, Date.Day, _TimeTable.NightStartTime.Hour, _TimeTable.NightStartTime.Minute, 00),
                    To = new DateTime(Date.Year, Date.Month, Date.Day, 00, 00, 00).AddDays(1)
                };
                if (ZeroSetHrs.TotalHours != 0)
                {
                    DateTime zdt01 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                    DateTime zdt02 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                    if (_OvertimeIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        zdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn1.Hour, _OvertimeIn1.Minute, _OvertimeIn1.Second); //_OvertimeIn1;
                    }
                    if (_OvertimeOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        zdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut1.Hour, _OvertimeOut1.Minute, _OvertimeOut1.Second); //_OvertimeOut1;
                    }
                    var shiftTime = new DateTimeInterval()
                    {
                        From = zdt01,
                        To = zdt02
                    };
                    if (shiftTime.From.Value.Hour > shiftTime.To.Value.Hour)
                    {
                        shiftTime.To = shiftTime.To.Value.AddDays(1);
                    }
                    var overlap1 = DateTimeUtils.GetIntervalIntersection(nightTime1, shiftTime);
                    double duration1 = 0;
                    if (overlap1.To != null && overlap1.From != null)
                    {
                        duration1 = (overlap1.To.Value - overlap1.From.Value).TotalHours;
                    }

                    var overlap2 = DateTimeUtils.GetIntervalIntersection(nightTime2, shiftTime);
                    double duration2 = 0;
                    if (overlap2.To != null && overlap2.From != null)
                    {
                        duration2 = (overlap2.To.Value - overlap2.From.Value).TotalHours;
                    }
                    dbl += OtStatus == OtStatusEnum.Approved ? (duration1 + duration2) : 0;
                }
                if (FirstSetHrs.TotalHours != 0)
                {
                    DateTime odt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, TimeTable.SecondSetCut.Second);
                    DateTime odt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.Hour, TimeTable.SecondSetCut.Minute, TimeTable.SecondSetCut.Second);
                    if (_ClockIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        odt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn1.Hour, _ClockIn1.Minute, _ClockIn1.Second); //_ClockIn1;
                    }
                    if (_ClockOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        odt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut1.Hour, _ClockOut1.Minute, _ClockOut1.Second); //_ClockOut1;
                    }
                    var shiftTime = new DateTimeInterval()
                    {
                        From = odt01,
                        To = odt02
                    };
                    if (shiftTime.From.Value.Hour > shiftTime.To.Value.Hour)
                    {
                        shiftTime.To = shiftTime.To.Value.AddDays(1);
                    }
                    var overlap1 = DateTimeUtils.GetIntervalIntersection(nightTime1, shiftTime);
                    double duration1 = 0;
                    if (overlap1.To != null && overlap1.From != null)
                    {
                        duration1 = (overlap1.To.Value - overlap1.From.Value).TotalHours;
                    }

                    var overlap2 = DateTimeUtils.GetIntervalIntersection(nightTime2, shiftTime);
                    double duration2 = 0;
                    if (overlap2.To != null && overlap2.From != null)
                    {
                        duration2 = (overlap2.To.Value - overlap2.From.Value).TotalHours;
                    }
                    dbl += (duration1 + duration2);
                }
                if (SecondSetHrs.TotalHours != 0)
                {
                    DateTime sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(1).Hour, TimeTable.SecondSetCut.AddHours(1).Minute, TimeTable.SecondSetCut.AddHours(1).Second);
                    DateTime sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.SecondSetCut.AddHours(1).Hour, TimeTable.SecondSetCut.AddHours(1).Minute, TimeTable.SecondSetCut.AddHours(1).Second);
                    if (_ClockIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        sdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockIn2.Hour, _ClockIn2.Minute, _ClockIn2.Second); //_ClockIn2;
                    }
                    if (_ClockOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        sdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _ClockOut2.Hour, _ClockOut2.Minute, _ClockOut2.Second); //_ClockOut2;
                    }
                    var shiftTime = new DateTimeInterval()
                    {
                        From = sdt01,
                        To = sdt02
                    };
                    if (shiftTime.From.Value.Hour > shiftTime.To.Value.Hour)
                    {
                        shiftTime.To = shiftTime.To.Value.AddDays(1);
                    }
                    var overlap1 = DateTimeUtils.GetIntervalIntersection(nightTime1, shiftTime);
                    double duration1 = 0;
                    if (overlap1.To != null && overlap1.From != null)
                    {
                        duration1 = (overlap1.To.Value - overlap1.From.Value).TotalHours;
                    }

                    var overlap2 = DateTimeUtils.GetIntervalIntersection(nightTime2, shiftTime);
                    double duration2 = 0;
                    if (overlap2.To != null && overlap2.From != null)
                    {
                        duration2 = (overlap2.To.Value - overlap2.From.Value).TotalHours;
                    }
                    dbl += (duration1 + duration2);
                }
                if (ThirdSetHrs.TotalHours != 0)
                {
                    DateTime tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.ThirdSetCut.Hour, TimeTable.ThirdSetCut.Minute, 0);
                    DateTime tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, 0);
                    if (TimeTable.LastCut.Hour == 0)
                    {
                        tdt02 = new DateTime(Date.AddDays(1).Year, Date.AddDays(1).Month, Date.AddDays(1).Day, TimeTable.LastCut.Hour, TimeTable.LastCut.Minute, 0);
                    }
                    if (_OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        tdt01 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeIn2.Hour, _OvertimeIn2.Minute, _OvertimeIn2.Second); //_OvertimeIn2;
                    }
                    if (_OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        tdt02 = new DateTime(Date.Year, Date.Month, Date.Day, _OvertimeOut2.Hour, _OvertimeOut2.Minute, _OvertimeOut2.Second); //_OvertimeOut2;
                    }
                    var shiftTime = new DateTimeInterval()
                    {
                        From = tdt01,
                        To = tdt02
                    };
                    if (shiftTime.From.Value.Hour > shiftTime.To.Value.Hour)
                    {
                        shiftTime.To = shiftTime.To.Value.AddDays(1);
                    }
                    var overlap1 = DateTimeUtils.GetIntervalIntersection(nightTime1, shiftTime);
                    double duration1 = 0;
                    if (overlap1.To != null && overlap1.From != null)
                    {
                        duration1 = (overlap1.To.Value - overlap1.From.Value).TotalHours;
                    }

                    var overlap2 = DateTimeUtils.GetIntervalIntersection(nightTime2, shiftTime);
                    double duration2 = 0;
                    if (overlap2.To != null && overlap2.From != null)
                    {
                        duration2 = (overlap2.To.Value - overlap2.From.Value).TotalHours;
                    }
                    dbl += OtStatus == OtStatusEnum.Approved ? (duration1 + duration2) : 0;
                }
                //TimeSpan ts = TimeSpan.FromHours(dbl);
                //string str = string.Format("{0}:{1}:{2}", ts.Hours, ts.Minutes, ts.Seconds);
                if (TimeTable.NightDiffRate > 0m)
                {
                    return dbl;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public bool Flexible
        {
            get { return _Flexible; }
            set { SetPropertyValue("Flexible", ref _Flexible, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("CalculatedAttendance2")]
        public GenJournalHeader StaffPayrollId
        {
            get { return _StaffPayrollId; }
            set { SetPropertyValue("StaffPayrollId", ref _StaffPayrollId, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Posted
        {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Invalid
        {
            get
            {
                if (ActualHours < TimeSpan.Zero || OtHours < TimeSpan.Zero || SecondSetHrs < TimeSpan.Zero || ThirdSetHrs < TimeSpan.Zero
                            || ActualLate > TimeSpan.FromHours(24d) || ActualEarly > TimeSpan.FromHours(24d))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public string LogTag
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (!_Invalid)
                {
                    sb.Append(_OvertimeIn1 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_OvertimeOut1 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_ClockIn1 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_BreakOut1 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_BreakIn1 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_ClockOut1 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_ClockIn2 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_BreakOut2 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_BreakIn2 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_ClockOut2 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_OvertimeIn2 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    sb.Append(_OvertimeOut2 == new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0) ? 0 : 1);
                    return sb.ToString();
                }
                else { return "000000000000"; }
            }
        }

        #region Pay Computation
        private decimal _BasicPay;
        private EmployeePayTypeEnum _PayType;
        private DayOfWeek _RestDayOfTheWeek;
        private decimal _BasicHrs;
        private decimal _BasicAmt;
        private decimal _AbsentHrs;
        private decimal _AbsentAmt;
        private decimal _LateHrs;
        private decimal _LateAmt;
        private decimal _UndertimeHrs;
        private decimal _UndertimeAmt;
        private decimal _RestdayOtHrs;
        private decimal _RestdayOtAmt;
        private decimal _OvertimeHrs;
        private decimal _OvertimeAmt;
        private decimal _NightDiffHrs;
        private decimal _NightDiffAmt;
        private decimal _HolidayHrs;
        //private HolidayTypeEnum _HolidayType = HolidayTypeEnum.None;
        private decimal _HolidayAmt;
        private decimal _HolidayHrs2;
        private decimal _HolidayAmt2;
        [Custom("AllowEdit", "False")]
        public decimal BasicPay
        {
            get { return _BasicPay; }
            set { SetPropertyValue("BasicPay", ref _BasicPay, value); }
        }
        [Custom("AllowEdit", "False")]
        public EmployeePayTypeEnum PayType
        {
            get { return _PayType; }
            set { SetPropertyValue("PayType", ref _PayType, value); }
        }
        [Custom("AllowEdit", "False")]
        public DayOfWeek RestDayOfTheWeek
        {
            get { return _RestDayOfTheWeek; }
            set { SetPropertyValue("RestDayOfTheWeek", ref _RestDayOfTheWeek, value); }
        }

        [PersistentAlias("TimeTable.CountAsHours")]
        [Custom("AllowEdit", "False")]
        public decimal BasicHrs
        {
            get
            {
                object tempObject = EvaluateAlias("BasicHrs");
                if (tempObject != null)
                {
                    if (_RestDayOfTheWeek == Date.DayOfWeek)
                    {
                        return 0m;
                    }
                    switch (HolidayType)
                    {
                        case HolidayTypeEnum.Special:
                            return (decimal)tempObject;
                        case HolidayTypeEnum.Regular:
                            return 0m;
                        case HolidayTypeEnum.None:
                            return (decimal)tempObject;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("BasicHrs")]
        [Custom("AllowEdit", "False")]
        public decimal BasicAmt
        {
            get
            {
                object tempObject = EvaluateAlias("BasicAmt");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (decimal)tempObject * _BasicPay;
                        case EmployeePayTypeEnum.Daily:
                            return _BasicPay;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("TimeTable.CountAsHours")]
        [Custom("AllowEdit", "False")]
        public decimal AbsentHrs
        {
            get
            {
                object tempObject = EvaluateAlias("AbsentHrs");
                if (tempObject != null && Absent)
                {
                    if (_RestDayOfTheWeek == Date.DayOfWeek)
                    {
                        return 0m;
                    }
                    switch (HolidayType)
                    {
                        case HolidayTypeEnum.Special:
                            return (decimal)tempObject;
                        case HolidayTypeEnum.Regular:
                            return 0m;
                        case HolidayTypeEnum.None:
                            return (decimal)tempObject;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("BasicHrs")]
        [Custom("AllowEdit", "False")]
        public decimal AbsentAmt
        {
            get
            {
                object tempObject = EvaluateAlias("AbsentAmt");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && Absent)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (decimal)tempObject * _BasicPay;
                        case EmployeePayTypeEnum.Daily:
                            return _BasicPay;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("Late")]
        [Custom("AllowEdit", "False")]
        public decimal LateHrs
        {
            get
            {
                object tempObject = EvaluateAlias("LateHrs");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && !Absent 
                    && !new[] { "Holiday RG", "Holiday RGOT" }.Any(o => Remarks.Contains(o)))
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("LateHrs")]
        [Custom("AllowEdit", "False")]
        public decimal LateAmt
        {
            get
            {
                object tempObject = EvaluateAlias("LateAmt");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && !Absent)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return BasicAmt / _TimeTable.CountAsHours * (decimal)tempObject;
                        case EmployeePayTypeEnum.Daily:
                            return BasicAmt / _TimeTable.CountAsHours * (decimal)tempObject;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("Early")]
        [Custom("AllowEdit", "False")]
        public decimal UndertimeHrs
        {
            get
            {
                object tempObject = EvaluateAlias("UndertimeHrs");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && !Absent
                    && !new[] { "Holiday RG", "Holiday RGOT" }.Any(o => Remarks.Contains(o)))
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("UndertimeHrs")]
        [Custom("AllowEdit", "False")]
        public decimal UndertimeAmt
        {
            get
            {
                object tempObject = EvaluateAlias("UndertimeAmt");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && !Absent)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return BasicAmt / _TimeTable.CountAsHours * (decimal)tempObject;
                        case EmployeePayTypeEnum.Daily:
                            return BasicAmt / _TimeTable.CountAsHours * (decimal)tempObject;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("RestDay")]
        [Custom("AllowEdit", "False")]
        public decimal RestdayOtHrs
        {
            get
            {
                object tempObject = EvaluateAlias("RestdayOtHrs");
                if (tempObject != null && _RestDayOfTheWeek == Date.DayOfWeek && !Absent && OtStatus == OtStatusEnum.Approved)
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("RestdayOtHrs")]
        [Custom("AllowEdit", "False")]
        public decimal RestdayOtAmt
        {
            get
            {
                object tempObject = EvaluateAlias("RestdayOtAmt");
                if (tempObject != null && _RestDayOfTheWeek == Date.DayOfWeek && !Absent && OtStatus == OtStatusEnum.Approved)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * CompanyInfo.RestDayRate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * CompanyInfo.RestDayRate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("ValidOtHours")]
        [Custom("AllowEdit", "False")]
        public decimal OvertimeHrs
        {
            get
            {
                object tempObject = EvaluateAlias("OvertimeHrs");
                //if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && OtStatus == OtStatusEnum.Approved)
                //{
                //    return (decimal)((TimeSpan)tempObject).TotalHours;
                //}
                //else
                //{
                //    return 0m;
                //}
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && OtStatus == OtStatusEnum.Approved)
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("ValidOtHours")]
        [Custom("AllowEdit", "False")]
        public decimal RestdayOtHrs2
        {
            get
            {
                object tempObject = EvaluateAlias("RestdayOtHrs2");
                //if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && OtStatus == OtStatusEnum.Approved)
                //{
                //    return (decimal)((TimeSpan)tempObject).TotalHours;
                //}
                //else
                //{
                //    return 0m;
                //}
                //if (tempObject != null && _RestDayOfTheWeek == Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && OtStatus == OtStatusEnum.Approved)
                if (tempObject != null && _RestDayOfTheWeek == Date.DayOfWeek && OtStatus == OtStatusEnum.Approved)
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("OvertimeHrs")]
        [Custom("AllowEdit", "False")]
        public decimal OvertimeAmt
        {
            get
            {
                object tempObject = EvaluateAlias("OvertimeAmt");
                if (tempObject != null && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && OtStatus == OtStatusEnum.Approved)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * CompanyInfo.OtRate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * CompanyInfo.OtRate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        //[PersistentAlias("Night")]
        [Custom("AllowEdit", "False")]
        public decimal NightDiffHrs
        {
            get
            {
                return Convert.ToDecimal(Night0);
                //object tempObject = EvaluateAlias("NightDiffHrs");
                //if (tempObject != null)
                //{
                //    return (decimal)((TimeSpan)tempObject).TotalHours;
                //}
                //else
                //{
                //    return 0m;
                //}
            }
        }
        [PersistentAlias("NightDiffHrs")]
        [Custom("AllowEdit", "False")]
        public decimal NightDiffAmt
        {
            get
            {
                object tempObject = EvaluateAlias("NightDiffAmt");
                if (tempObject != null)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * CompanyInfo.NightDiffRate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * CompanyInfo.NightDiffRate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Holiday HolidayId
        {
            get
            {
                //XPCollection<Holiday> hols = new XPCollection<Holiday>(Session);
                Holiday firstOrDefault = AttRecId.BatchID.Holidays.Where(o => o.Date == Date).FirstOrDefault();
                if (firstOrDefault != null)
                {
                    return firstOrDefault;
                }
                else
                {
                    return null;
                }
            }
        }
        [PersistentAlias("ValidWorkHours")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayHrs
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayHrs");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Regular && !Absent)
                {
                    if ((TimeSpan)tempObject > NormalHours)
                    {
                        return (decimal)NormalHours.TotalHours;
                    }
                    else
                    {
                        return (decimal)((TimeSpan)tempObject).TotalHours;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        private decimal _HolidayOTHrs;
        [PersistentAlias("ValidOtHours")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTHrs
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayOTHrs");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Regular && !Absent && OtStatus == OtStatusEnum.Approved)
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        private decimal _HolidayOTAmt;
        [PersistentAlias("HolidayOTHrs")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTAmt
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayOTAmt");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Regular && !Absent && OtStatus == OtStatusEnum.Approved)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * HolidayId.ExcessRate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * HolidayId.ExcessRate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }

        //[Custom("AllowEdit", "False")]
        //public HolidayTypeEnum HolidayType
        //{
        //    get { return _HolidayType; }
        //    set { SetPropertyValue("HolidayType", ref _HolidayType, value); }
        //}
        [PersistentAlias("HolidayHrs")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayAmt
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayAmt");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Regular && !Absent)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * HolidayId.Rate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * HolidayId.Rate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("ValidWorkHours")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayHrs2
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayHrs2");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Special && !Absent)
                {
                    if (Late.TotalHours > 0)
                    {
                        //return (decimal)(NormalHours.TotalHours - Late.TotalHours);
                        if ((decimal)((TimeSpan)tempObject).TotalHours > (decimal)NormalHours.TotalHours)
                        {
                            return (decimal)(NormalHours.TotalHours - Late.TotalHours);
                        }
                        else
                        {
                            return (decimal)((TimeSpan)tempObject).TotalHours;
                        }
                    }
                    else
                    {
                        return (decimal)NormalHours.TotalHours;
                    }
                    //if ((TimeSpan)tempObject > NormalHours)
                    //{
                    //    return (decimal)NormalHours.TotalHours;
                    //}
                    //else
                    //{
                    //    if (Late.TotalHours > 0)
                    //    {
                    //        return (decimal)(NormalHours.TotalHours - Late.TotalHours);

                    //    }
                    //    else
                    //    {
                    //        return (decimal)NormalHours.TotalHours;
                    //    }
                    //}
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("HolidayHrs2")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayAmt2
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayAmt2");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Special && !Absent)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * HolidayId.Rate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * HolidayId.Rate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        private decimal _HolidayOTHrs2;
        private decimal _HolidayOTAmt2;
        [PersistentAlias("ValidOtHours")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTHrs2
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayOTHrs2");
                //if (tempObject != null && HolidayType == HolidayTypeEnum.Special && !Absent && OtStatus == OtStatusEnum.Approved)
                if (tempObject != null && HolidayType == HolidayTypeEnum.Special && OtStatus == OtStatusEnum.Approved)
                {
                    return (decimal)((TimeSpan)tempObject).TotalHours;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("HolidayOTHrs2")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTAmt2
        {
            get
            {
                object tempObject = EvaluateAlias("HolidayOTAmt2");
                if (tempObject != null && HolidayType == HolidayTypeEnum.Special && !Absent && OtStatus == OtStatusEnum.Approved)
                {
                    switch (_PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            return (_BasicPay * (decimal)tempObject) * HolidayId.ExcessRate / 100;
                        case EmployeePayTypeEnum.Daily:
                            return (_BasicPay / _TimeTable.CountAsHours * (decimal)tempObject) * HolidayId.ExcessRate / 100;
                        case EmployeePayTypeEnum.Monthly:
                            return 0m;
                        default:
                            return 0m;
                    }
                }
                else
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("BasicAmt - (AbsentAmt + LateAmt + UndertimeAmt) + RestdayOtAmt + OvertimeAmt + NightDiffAmt + HolidayAmt + HolidayAmt2 + HolidayOTAmt + HolidayOTAmt2")]
        [Custom("AllowEdit", "False")]
        public decimal LinePay
        {
            get
            {
                object tempObject = EvaluateAlias("LinePay");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }
        #endregion

        private System.Collections.ArrayList _References;
        [Custom("AllowEdit", "False")]
        public System.Collections.ArrayList References
        {
            get { return _References; }
            set { SetPropertyValue("References", ref _References, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Allowance
        {
            get
            {
                if (Absent || _Invalid || Remarks == "Rest Day")
                {
                    // return _EmployeeId != null ? 0m : 0m;
                    return 0m;
                }
                else
                {
                    // return _EmployeeId != null ? _EmployeeId.Allowance : 0m;
                    return _AllowanceOverride;
                }
            }
        }
        //[Custom("AllowEdit", "False")]
        public decimal AllowanceOverride
        {
            get { return _AllowanceOverride != 0m ? _AllowanceOverride : Allowance; }
            set { SetPropertyValue("AllowanceOverride", ref _AllowanceOverride, value); }
        }
        //[NonPersistent]
        //[Custom("AllowEdit", "False")]
        //public string Remarks
        //{
        //    // "Full", "Rest Day", "Holiday SP", "Holiday RG", "Late", "Early", "Partial", "Absent", 
        //    // "Overtime", "Holiday OT", "Rest Day OT", "Holiday RGOT", "Holiday SPOT"
        //    get
        //    {
        //        if (_LineID == "10226 08-10-2018 Dayshift")
        //        {
                    
        //        }
        //        if (!Absent && _Invalid)
        //        {
        //            return "Invalid";
        //        }
        //        if (_RestDayOfTheWeek == Date.DayOfWeek && ValidWorkHours == TimeSpan.Zero)
        //        {
        //            return "Rest Day";
        //        }
        //        else if (_RestDayOfTheWeek == Date.DayOfWeek && ValidWorkHours > TimeSpan.Zero)
        //        {
        //            return "Rest Day OT";
        //        }
        //        if (ValidWorkHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
        //        {
        //            return "Holiday SP Absent";
        //        }
        //        if (ValidWorkHours > TimeSpan.Zero && ValidOtHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
        //        {
        //            return "Holiday SP";
        //        }
        //        else if (ValidWorkHours > TimeSpan.Zero && ValidOtHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Special)
        //        {
        //            return "Holiday SPOT";
        //        }
        //        if (ValidWorkHours == TimeSpan.Zero && HolidayType == HolidayTypeEnum.Regular)
        //        {
        //            return "Holiday RG";
        //        }
        //        else if (ValidWorkHours > TimeSpan.Zero && HolidayType == HolidayTypeEnum.Regular)
        //        {
        //            return "Holiday RGOT";
        //        }
        //        if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && ValidWorkHours >= TimeSpan.FromHours((double)_TimeTable.CountAsHours))
        //        {
        //            return "Full";
        //        }
        //        else if (SecondSetHrs.TotalHours != 0 && !(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && !_Invalid && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && Late > TimeSpan.Zero)
        //        {
        //            if (_TimeTable.FirstHalf != _TimeTable.SecondHalf)
        //            {
        //                if (_TimeTable.SecondHalf >= Convert.ToDouble(_TimeTable.CountAsHours / 2))
        //                {
        //                    return "1";
        //                }
        //                else if (_TimeTable.SecondHalf < Convert.ToDouble(_TimeTable.CountAsHours / 2) && SecondLate.TotalMinutes == 0 && FirstEarly.TotalMinutes == 0)
        //                {
        //                    return "2";
        //                }
        //                else
        //                {
        //                    return "3";
        //                }
        //            }
        //            else
        //            {
        //                if (FirstSetHrs.TotalHours == 0)
        //                {
        //                    return "Halfday";
        //                }
        //                else if (FirstLate.TotalMinutes > Convert.ToDouble(_TimeTable.LateTimeMins))
        //                {
        //                    return "Late";
        //                }
        //            }
                    
        //            //if (FirstLate >= (_TimeTable.BreakOutTime - _TimeTable.OnDutyTime))
        //            //{
        //            //    return "Halfday";
        //            //}
        //            //else
        //            //{
        //            //    return "Late";
        //            //}
        //        }
        //        else if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && !_Invalid && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && Early > TimeSpan.Zero)
        //        {
        //            if (_TimeTable.FirstHalf != _TimeTable.SecondHalf)
        //            {
        //                if (_TimeTable.FirstHalf >= Convert.ToDouble(_TimeTable.CountAsHours/2))
        //                {
        //                    return "Halfday"; //"4";
        //                }
        //                else if (_TimeTable.FirstHalf < Convert.ToDouble(_TimeTable.CountAsHours / 2) && SecondLate.TotalMinutes == 0 && FirstEarly.TotalMinutes == 0)
        //                {
        //                    return "5";
        //                }
        //                else
        //                {
        //                    return "6";
        //                }
        //            }
        //            else
        //            {
        //                if (SecondSetHrs.TotalHours == 0)
        //                {
        //                    return "Halfday";
        //                }
        //                else if (SecondEarly.TotalMinutes > Convert.ToDouble(_TimeTable.LeaveEarlyTimeMins))
        //                {
        //                    return "Early";
        //                }
        //            }
        //            //if (SecondEarly >= (_TimeTable.OffDutyTime - _TimeTable.HalfDutyTime))
        //            //{
        //            //    return "Halfday";
        //            //}
        //            //else
        //            //{
        //            //    return "Early";
        //            //}
        //        }
        //        else if (!(OtHours.TotalMinutes >= _TimeTable.MinimumOtMins) && !_Invalid && _RestDayOfTheWeek != Date.DayOfWeek && HolidayType == HolidayTypeEnum.None && !Absent && ValidWorkHours < TimeSpan.FromHours((double)_TimeTable.CountAsHours))
        //        {
        //            return "Partial";
        //        }
        //        //if (FirstLate >= (_TimeTable.BreakOutTime - _TimeTable.OnDutyTime))
        //        //{
        //        //    return "Halfday";
        //        //}
        //        //if (SecondEarly >= (_TimeTable.OffDutyTime - _TimeTable.HalfDutyTime))
        //        //{
        //        //    return "Halfday";
        //        //}
        //        if (OtHours.TotalMinutes >= _TimeTable.MinimumOtMins && HolidayType == HolidayTypeEnum.None)
        //        {
        //            return "Overtime";
        //        }
        //        if (Absent)
        //        {
        //            return "Absent";
        //        }
        //        if (FirstEarly.TotalMinutes > _TimeTable.LeaveEarlyTimeMins)
        //        {
        //            return "Early";
        //        }
        //        else if (SecondEarly.TotalMinutes > _TimeTable.LeaveEarlyTimeMins)
        //        {
        //            return "Early";
        //        }
        //        else if (FirstLate.TotalMinutes > _TimeTable.LateTimeMins)
        //        {
        //            return "Late";
        //        }
        //        else if (SecondLate.TotalMinutes > _TimeTable.LateTimeMins)
        //        {
        //            return "Late";
        //        }
        //        else
        //        {
        //            return "Partial";
        //        }
        //        //return string.Empty;
        //    }
        //}
        // Halfday
        public bool Halfday
        {
            get
            {
                if (LineID == "10375 08-16-2018 Dayshift")
                {

                }
                //if (ValidWorkHours.TotalHours >= Convert.ToDouble(_TimeTable.CountAsHours / 2) &&
                //        ValidWorkHours.TotalHours < Convert.ToDouble((_TimeTable.CountAsHours / 2) + Convert.ToDecimal(TimeSpan.FromMinutes(30).TotalHours)))
                //{
                //    return true;
                //}
                if (HolidayType != HolidayTypeEnum.Regular && FirstSetHrs.TotalHours != 0 && SecondSetHrs.TotalHours == 0)
                {
                    return true;
                }
                else if (HolidayType != HolidayTypeEnum.Regular && FirstSetHrs.TotalHours == 0 && SecondSetHrs.TotalHours != 0)
                {
                    return true;
                }
                else
                {
                    return false;

                }
            }
        }
        public NightDiffModeEnum NightDiffMode { 
            get {
                if (LineID == "10304 08-21-2018 Dayshift")
                {

                }
                if (HolidayId == null)
                {
                    if (_RestDayOfTheWeek != Date.DayOfWeek)
                    {
                        // Ordinary Day Night Diff
                        return NightDiffModeEnum.Ordinary;
                    }
                    else if (_RestDayOfTheWeek == Date.DayOfWeek)
                    {
                        // Restday Night Diff
                        return NightDiffModeEnum.RestDay;
                    }
                }
                else if (HolidayId != null && _RestDayOfTheWeek != Date.DayOfWeek)
                {
                    if (HolidayId.HolidayType == HolidayTypeEnum.Special)
                    {
                        // Special Holiday Night Diff
                        return NightDiffModeEnum.Special;
                    }
                    else if (HolidayId.HolidayType == HolidayTypeEnum.Regular)
                    {
                        // Regular Holiday Night Diff
                        return NightDiffModeEnum.Regular;
                    }
                    else if (HolidayId.HolidayType == HolidayTypeEnum.Double)
                    {
                        // Double Holiday Night Diff
                        return NightDiffModeEnum.Double;
                    }
                }
                else if (HolidayId != null && _RestDayOfTheWeek == Date.DayOfWeek)
                {
                    if (HolidayId.HolidayType == HolidayTypeEnum.Special)
                    {
                        // Special Holiday and Restday Night Diff
                        return NightDiffModeEnum.SpecialRestDay;
                    }
                    else if (HolidayId.HolidayType == HolidayTypeEnum.Regular)
                    {
                        // Regular Holiday and Restday Night Diff
                        return NightDiffModeEnum.RegularRestDay;
                    }
                    else if (HolidayId.HolidayType == HolidayTypeEnum.Double)
                    {
                        // Double Holiday and Restday Night Diff
                        return NightDiffModeEnum.DoubeRestDay;
                    }
                }
                return NightDiffModeEnum.Ordinary; 
            } 
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string Remarks
        {
            // "Full", "Rest Day", "Holiday SP", "Holiday RG", "Late", "Early", "Partial", "Absent", 
            // "Overtime", "Holiday OT", "Rest Day OT", "Holiday RGOT", "Holiday SPOT"
            get
            {
                if (LineID == "10304 08-10-2018 Dayshift")
                {

                }
                if (Invalid)
                {
                    return "Invalid";
                }
                if (Absent)
                {
                    return "Absent";
                }
                if (_RestDayOfTheWeek == Date.DayOfWeek)
                {
                    if (ValidWorkHours > TimeSpan.Zero)
                    {
                        return "Rest Day OT";
                    }
                    return "Rest Day";
                }
                if (HolidayType == HolidayTypeEnum.Special)
                {
                    if (ValidWorkHours > TimeSpan.Zero && ValidOtHours == TimeSpan.Zero)
                    {
                        return "Holiday SP";
                    }
                    else if (ValidWorkHours > TimeSpan.Zero && ValidOtHours > TimeSpan.Zero)
                    {
                        return "Holiday SPOT";
                    }
                    return "Holiday SP Absent";
                }
                if (HolidayType == HolidayTypeEnum.Regular)
                {
                    if (ValidWorkHours > TimeSpan.Zero)
                    {
                        return "Holiday RGOT";
                    }
                    return "Holiday RG";
                }
                if (HolidayType == HolidayTypeEnum.Double)
                {
                    if (ValidWorkHours > TimeSpan.Zero)
                    {
                        return "Holiday DBOT";
                    }
                    return "Holiday DB";
                }
                if (OtHours.TotalMinutes >= _TimeTable.MinimumOtMins && HolidayType == HolidayTypeEnum.None)
                {
                    return "Overtime";
                }
                if (ValidWorkHours.TotalHours >= Convert.ToDouble(_TimeTable.CountAsHours / 2) &&
                        ValidWorkHours.TotalHours < Convert.ToDouble((_TimeTable.CountAsHours / 2) + Convert.ToDecimal(TimeSpan.FromMinutes(30).TotalHours)))
                {
                    return "Halfday";
                }
                if (FirstLate.TotalMinutes > Convert.ToDouble(_TimeTable.LateTimeMins))
                {
                    return "Late";
                }
                if (SecondLate.TotalMinutes > Convert.ToDouble(_TimeTable.LateTimeMins))
                {
                    return "Late";
                }
                if (FirstEarly.TotalMinutes > Convert.ToDouble(_TimeTable.LateTimeMins))
                {
                    return "Early";
                }
                if (SecondEarly.TotalMinutes > Convert.ToDouble(_TimeTable.LateTimeMins))
                {
                    return "Early";
                }
                if ((ValidWorkHours - ValidOtHours).TotalHours < Convert.ToDouble(_TimeTable.CountAsHours))
                {
                    return "Partial";
                }
                if ((ValidWorkHours - ValidOtHours).TotalHours >= Convert.ToDouble(_TimeTable.CountAsHours))
                {
                    return "Full";
                }
                return string.Empty;
            }
        }

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

        public DateTime TimeMin
        {
            get
            {
                return new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            }
        }

        public decimal Count
        {
            get
            {
                if (new[] { "Absent", "Rest Day", "Rest Day OT" }.Any(o => o.Contains(Remarks)) && !Halfday)
                {
                    return 0m;
                }
                else if (Remarks == "Halfday" || Halfday)
                {
                    return 0.5m;
                }
                else
                {
                    return 1m;
                }
            }
        }
        [NonPersistent]
        public Company CompanyInfo
        {
            get { return Company.GetInstance(Session); }
        }
        [Action(Caption = "Restore")]
        public void ValidateLine()
        {
            if (Oid < 0)
            {
                return;
            }
            CheckInAndOut03 chkIO = this;
            EmpShiftSchedule data = EmployeeId.ShiftSchedules.Where(o => (Date.Ticks >= o.FromDate.Ticks && Date.Ticks < o.ToDate.AddDays(1).Ticks)).OrderBy(o => o.FromDate).LastOrDefault();
            TimeTable2 ottbl = chkIO.TimeTable;
            if (data != null)
            {
                ottbl = Session.GetObjectByKey<TimeTable2>(data.Shift.Oid);
            }
            chkIO.TimeTable = ottbl;
            chkIO.NightDiffRate = ottbl.NightDiffRate;
            chkIO.NormalOtRate = ottbl.NormalOtRate;
            chkIO.RestdayRate = ottbl.RestdayRate;
            chkIO.RestdayOtRate = ottbl.RestdayOtRate;
            chkIO.RestDayOfTheWeek = this.AttRecId.RestDay;
            switch (EmployeeId.PayType)
            {
                case EmployeePayTypeEnum.Hourly:
                    chkIO.BasicPay = EmployeeId.Basic;
                    break;
                case EmployeePayTypeEnum.Daily:
                    chkIO.BasicPay = EmployeeId.Basic / EmployeeId.Shift.CountAsHours;
                    break;
                case EmployeePayTypeEnum.Monthly:
                    chkIO.BasicPay = (EmployeeId.Basic / WorkingDays.GetWorkingDays(AttRecId.BatchID.TimeRangeFrom, AttRecId.BatchID.TimeRangeTo, new List<DayOfWeek> { EmployeeId.RestDay })) /
                        EmployeeId.Shift.CountAsHours;
                    break;
                default:
                    break;
            }
            //chkIO.BasicPay = this.AttRecId.Basic;
            chkIO.AllowanceOverride = this.AttRecId.Allowance;
            chkIO.OnDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OnDutyTime.Hour, ottbl.OnDutyTime.Minute, 0);  //ottbl.OnDutyTime;4
            chkIO.HalfDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.HalfDutyTime.Hour, ottbl.HalfDutyTime.Minute, 0);
            chkIO.OffDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OffDutyTime.Hour, ottbl.OffDutyTime.Minute, 0); //ottbl.OffDutyTime;
            chkIO.Normal = ottbl.CountAsWorkday;
            chkIO.RealTime = 0m;
            chkIO.ActualHours = TimeSpan.Zero;
            chkIO.Flexible = false;
            // Zero Set
            chkIO.NextDay2 = false;
            chkIO.OvertimeIn1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.OvertimeOut1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            //chkIO.ZeroSetHrs = TimeSpan.Zero;
            chkIO.Ref31 = 0;
            chkIO.Ref32 = 0;

            // First Set
            chkIO.ClockIn1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.BreakOut1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.BreakIn1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.ClockOut1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.BreakClock1 = false;
            //chkIO.FirstSetHrs = TimeSpan.Zero;
            chkIO.Ref11 = 0;
            chkIO.Ref12 = 0;
            chkIO.Ref13 = 0;
            chkIO.Ref14 = 0;
            // Second Set
            chkIO.NextDay1 = false;
            chkIO.ClockIn2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.BreakOut2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.BreakIn2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.ClockOut2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.BreakClock2 = false;
            //chkIO.SecondSetHrs = TimeSpan.Zero;
            chkIO.Ref21 = 0;
            chkIO.Ref22 = 0;
            chkIO.Ref23 = 0;
            chkIO.Ref24 = 0;
            // Third Set
            chkIO.OvertimeIn2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            chkIO.OvertimeOut2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
            //chkIO.ThirdSetHrs = TimeSpan.Zero;
            chkIO.Ref33 = 0;
            chkIO.Ref34 = 0;
            chkIO.ZeroNightHrs = TimeSpan.Zero;
            //chkIO.ThirdNightHrs = TimeSpan.Zero;
            ArrayList chiid = new ArrayList();
            //XPCollection<DeviceAttendanceLog> devAtts = new XPCollection<DeviceAttendanceLog>(Session);
            //var devAttList = devAtts.Where(o => o.EnrolledNo == chkIO.EnrolledNo && o.LogTime.Date == chkIO.Date).OrderBy(o => o.DwYear & o.DwMonth & o.DwDay & o.DwHour & o.DwMinute);
            var devAttList = AttendanceLogs.Where(o => o.EnrolledNo == chkIO.EnrolledNo && o.LogTime.Date == chkIO.Date).OrderBy(o => o.DwYear & o.DwMonth & o.DwDay & o.DwHour & o.DwMinute);
            //EmpShiftSchedule data = EmployeeId.ShiftSchedules.Where(o => (Date.Ticks >= o.FromDate.Ticks && Date.Ticks < o.ToDate.AddDays(1).Ticks)).OrderBy(o => o.FromDate).LastOrDefault();
            //TimeTable2 ottbl = chkIO.TimeTable;
            //if (data != null)
            //{
            //    ottbl = Session.GetObjectByKey<TimeTable2>(data.Shift.Oid);
            //    chkIO.TimeTable = ottbl;
            //}
            //chkIO.OnDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OnDutyTime.Hour, ottbl.OnDutyTime.Minute, 0);  //ottbl.OnDutyTime;4
            //chkIO.HalfDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.HalfDutyTime.Hour, ottbl.HalfDutyTime.Minute, 0);
            //chkIO.OffDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OffDutyTime.Hour, ottbl.OffDutyTime.Minute, 0); //ottbl.OffDutyTime;
            //chkIO.Normal = ottbl.CountAsWorkday;
            TimeSpan zts = TimeSpan.Zero;
            TimeSpan ots = TimeSpan.Zero;
            TimeSpan sts = TimeSpan.Zero;
            TimeSpan tts = TimeSpan.Zero;
            foreach (DeviceAttendanceLog datl in devAttList)
            {
                #region Zero Set
                if (datl.LogTime.TimeOfDay >= ottbl.ZeroSetCut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.SecondSetCut.TimeOfDay)
                {
                    switch (datl.InOutMode)
                    {
                        case InOutModeEnum.CheckIn:
                            chkIO.ClockIn1 = datl.LogTime;
                            chkIO.Ref11 = datl.Oid;
                            break;
                        case InOutModeEnum.CheckOut:
                            // Will not register checkout in this zone
                            break;
                        case InOutModeEnum.BreakOut:
                            // Will not register breakout in this zone
                            break;
                        case InOutModeEnum.BreakIn:
                            // Will not register breakin in this zone
                            break;
                        case InOutModeEnum.OvertimeIn:
                            chkIO.OvertimeIn1 = datl.LogTime;
                            chkIO.Ref31 = datl.Oid;
                            break;
                        case InOutModeEnum.OvertimeOut:
                            chkIO.OvertimeOut1 = datl.LogTime;
                            chkIO.Ref32 = datl.Oid;
                            break;
                        default:
                            break;
                    }
                    // Zero Hours
                    DateTime zdt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                    DateTime zdt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                    if (chkIO.OvertimeIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        zdt01 = chkIO.OvertimeIn1;
                    }
                    if (chkIO.OvertimeOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        zdt02 = chkIO.OvertimeOut1;
                    }
                    zts = zdt02 - zdt01;
                    //chkIO.ZeroSetHrs = zts;
                    DateTime zndstart = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                    DateTime zndend = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.NightEndTime.Hour, ottbl.NightEndTime.Minute, ottbl.NightEndTime.Second);

                    DateTime znt01;
                    DateTime znt02;
                    if (zdt01 >= zndstart)
                    {
                        znt01 = zdt01;
                    }
                    else
                    {
                        znt01 = zndstart;
                    }
                    if (zdt02 <= zndend)
                    {
                        znt02 = zdt02;
                    }
                    else
                    {
                        znt02 = zndend;
                    }
                    chkIO.ZeroNightHrs = znt02 - znt01;
                }
                #endregion
                #region First Set
                if (datl.LogTime.TimeOfDay >= ottbl.FirstSetCut.TimeOfDay && datl.LogTime.TimeOfDay < ottbl.SecondSetCut.TimeOfDay)
                {
                    switch (datl.InOutMode)
                    {
                        case InOutModeEnum.CheckIn:
                            chkIO.ClockIn1 = datl.LogTime;
                            chkIO.Ref11 = datl.Oid;
                            break;
                        case InOutModeEnum.CheckOut:
                            chkIO.ClockOut1 = datl.LogTime;
                            chkIO.Ref14 = datl.Oid;
                            break;
                        case InOutModeEnum.BreakOut:
                            chkIO.BreakOut1 = datl.LogTime;
                            chkIO.Ref12 = datl.Oid;
                            break;
                        case InOutModeEnum.BreakIn:
                            chkIO.BreakIn1 = datl.LogTime;
                            chkIO.Ref13 = datl.Oid;
                            break;
                        case InOutModeEnum.OvertimeIn:
                            // Will not register OvertimeIn in this zone
                            break;
                        case InOutModeEnum.OvertimeOut:
                            // Will not register OvertimeOut in this zone
                            break;
                        default:
                            break;
                    }
                    // First Hours
                    TimeSpan vhrs01 = ottbl.BreakOutTime - ottbl.OnDutyTime;
                    DateTime odt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.Hour, ottbl.SecondSetCut.Minute, ottbl.SecondSetCut.Second);
                    DateTime odt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.Hour, ottbl.SecondSetCut.Minute, ottbl.SecondSetCut.Second);
                    if (chkIO.ClockIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        odt01 = chkIO.ClockIn1;
                    }
                    if (chkIO.ClockOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        odt02 = chkIO.ClockOut1;
                    }
                    ots = odt02 - odt01;
                    if (ots > vhrs01)
                    {
                        ots = vhrs01;
                    }
                    //chkIO.FirstSetHrs = ots;
                }
                #endregion
                #region Second Set
                if (datl.LogTime.TimeOfDay >= ottbl.SecondSetCut.TimeOfDay && datl.LogTime.TimeOfDay < ottbl.ThirdSetCut.TimeOfDay.Add(TimeSpan.FromHours(1)))
                {
                    switch (datl.InOutMode)
                    {
                        case InOutModeEnum.CheckIn:
                            chkIO.ClockIn2 = datl.LogTime;
                            chkIO.Ref21 = datl.Oid;
                            break;
                        case InOutModeEnum.CheckOut:
                            chkIO.ClockOut2 = datl.LogTime;
                            chkIO.Ref24 = datl.Oid;
                            break;
                        case InOutModeEnum.BreakOut:
                            chkIO.BreakOut2 = datl.LogTime;
                            chkIO.Ref22 = datl.Oid;
                            break;
                        case InOutModeEnum.BreakIn:
                            chkIO.BreakIn2 = datl.LogTime;
                            chkIO.Ref23 = datl.Oid;
                            break;
                        case InOutModeEnum.OvertimeIn:
                            // Will not register OvertimeIn in this zone
                            break;
                        case InOutModeEnum.OvertimeOut:
                            // Will not register OvertimeOut in this zone
                            break;
                        default:
                            break;
                    }
                    // Second Hours
                    TimeSpan vhrs02 = ottbl.OffDutyTime - ottbl.HalfDutyTime;
                    DateTime sdt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                    DateTime sdt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                    if (chkIO.ClockIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        sdt01 = chkIO.ClockIn2;
                    }
                    if (chkIO.ClockOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        sdt02 = chkIO.ClockOut2;
                    }
                    sts = sdt02 - sdt01;
                    if (sts > vhrs02)
                    {
                        sts = vhrs02;
                    }
                    //chkIO.SecondSetHrs = sts;
                }
                #endregion
                #region Third Set
                if (datl.LogTime.TimeOfDay >= ottbl.SecondSetCut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.LastCut.TimeOfDay)
                //if (datl.LogTime.TimeOfDay <= ottbl.LastCut.TimeOfDay)
                {
                    switch (datl.InOutMode)
                    {
                        case InOutModeEnum.CheckIn:
                            // Will not register OvertimeIn in this zone
                            break;
                        case InOutModeEnum.CheckOut:
                            // chkIO.ClockOut2 = datl.LogTime;
                            // chkIO.Ref24 = datl.Oid;
                            break;
                        case InOutModeEnum.BreakOut:
                            // Will not register OvertimeIn in this zone
                            break;
                        case InOutModeEnum.BreakIn:
                            // Will not register OvertimeIn in this zone
                            break;
                        case InOutModeEnum.OvertimeIn:
                            chkIO.OvertimeIn2 = datl.LogTime;
                            chkIO.Ref33 = datl.Oid;
                            break;
                        case InOutModeEnum.OvertimeOut:
                            chkIO.OvertimeOut2 = datl.LogTime;
                            chkIO.Ref34 = datl.Oid;
                            break;
                        default:
                            break;
                    }
                    // Third Hours
                    DateTime tdt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                    DateTime tdt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                    if (chkIO.OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        tdt01 = chkIO.OvertimeIn2;
                    }
                    if (chkIO.OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                    {
                        tdt02 = chkIO.OvertimeOut2;
                    }
                    tts = tdt02 - tdt01;
                    //chkIO.ThirdSetHrs = tts;
                    DateTime tndstart = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.NightStartTime.Hour, ottbl.NightStartTime.Minute, ottbl.NightStartTime.Second);
                    DateTime tndend = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.LastCut.Hour, ottbl.LastCut.Minute, ottbl.LastCut.Second);

                    DateTime tnt01;
                    DateTime tnt02;
                    if (tdt01 >= tndstart)
                    {
                        tnt01 = tdt01;
                    }
                    else
                    {
                        tnt01 = tndstart;
                    }
                    if (tdt02 <= tndend)
                    {
                        tnt02 = tdt02;
                    }
                    else
                    {
                        tnt02 = tndend;
                    }
                    //chkIO.ThirdNightHrs = tnt02 - tnt01;
                }
                #endregion
            }
            //if (zts < TimeSpan.Zero || ots < TimeSpan.Zero || sts < TimeSpan.Zero || tts < TimeSpan.Zero
            //                || chkIO.ActualLate > TimeSpan.FromHours(24d) || chkIO.ActualEarly > TimeSpan.FromHours(24d))
            //{
            //    chkIO.Invalid = true;
            //}
            //else
            //{
            //    chkIO.Invalid = false;
            //}
            chkIO.DisapprovedOt = false;
            chkIO.ApprovedOt = false;
            chkIO.UnAltered = true;
            chkIO.References = chiid;
            chkIO.Save();
            //Session.CommitTransaction();
        }
        [Association]
        public XPCollection<DeviceAttendanceLog> AttendanceLogs
        {
            get { return GetCollection<DeviceAttendanceLog>("AttendanceLogs"); }
        }
        public CheckInAndOut03(Session session)
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
            //AttRecId.UpdateNoOfDays(false);
            //AttRecId.UpdateOtHrs(false);
            //AttRecId.UpdateSpgHldPrcnt(false);
            //AttRecId.UpdateRegHldHrs(false);
            //AttRecId.UpdateLateMins(false);
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

        protected override void OnDeleting()
        {
            for (int i = AttendanceLogs.Count - 1; i >= 0; i--)
            {
                AttendanceLogs.Remove(AttendanceLogs[i]);
            }
            //foreach (var item in AttendanceLogs)
            //{
            //    AttendanceLogs.Remove(item);
            //}
            base.OnDeleting();
        }
        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _AllowanceOverride;
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
    }

}
