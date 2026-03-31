using System;
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
    public enum NightOrDayEnum
    {
        Day,
        Night
    }
    [DefaultClassOptions]
    public class TimeTable2 : BaseObject
    {
        private string _TimeTableName;
        private int _LateTimeMins;
        private int _LeaveEarlyTimeMins;
        private decimal _CountAsWorkday;
        private int _CountAsMinute;
        private decimal _CountAsHours;
        private decimal _BreakHours;
        private int _MinimumOtMins;
        private DateTime _NightStartTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _NightEndTime = new DateTime(1753, 1, 1, 0, 0, 0);
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Timetable Name")]
        public string TimeTableName
        {
            get { return _TimeTableName; }
            set { SetPropertyValue("TimeTableName", ref _TimeTableName, value); }
        }
        #region Clocks
        private DateTime _OnDutyTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _BreakOutTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _HalfDutyTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _OffDutyTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private bool _MustBreakClock1;
        private bool _MustBreakClock2;
        private bool _NextDay1 = false;
        private bool _NextDay2 = false;
        private DateTime _ZeroSetCut = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _FirstSetCut = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _SecondSetCut = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _ThirdSetCut = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _LastCut = new DateTime(1753, 1, 1, 0, 0, 0);
        private decimal _NormalOtRate;
        private decimal _RestdayRate;
        private decimal _RestdayOtRate;
        private decimal _NightDiffRate;
        private NightOrDayEnum _NightOrDay;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime OnDutyTime
        {
            get { return _OnDutyTime; }
            set { SetPropertyValue("OnDutyTime", ref _OnDutyTime, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime BreakOutTime
        {
            get { return _BreakOutTime; }
            set { SetPropertyValue("BreakOutTime", ref _BreakOutTime, value); }
        }
        public double FirstHalf
        {
            get { return (_BreakOutTime - _OnDutyTime).TotalHours; }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime HalfDutyTime
        {
            get { return _HalfDutyTime; }
            set { SetPropertyValue("HalfDutyTime", ref _HalfDutyTime, value); }
        }
        
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime OffDutyTime
        {
            get { return _OffDutyTime; }
            set { SetPropertyValue("OffDutyTime", ref _OffDutyTime, value); }
        }
        public double SecondHalf
        {
            get { return (_OffDutyTime - _HalfDutyTime).TotalHours; }
        }
        public bool MustBreakClock1
        {
            get { return _MustBreakClock1; }
            set { SetPropertyValue("MustBreakClock1", ref _MustBreakClock1, value); }
        }
        public bool MustBreakClock2
        {
            get { return _MustBreakClock2; }
            set { SetPropertyValue("MustBreakClock2", ref _MustBreakClock2, value); }
        }
        
        public bool NextDay1
        {
            get { return _NextDay1; }
            set { SetPropertyValue("NextDay1", ref _NextDay1, value); }
        }
        public bool NextDay2
        {
            get { return _NextDay2; }
            set { SetPropertyValue("NextDay1", ref _NextDay2, value); }
        }
        public NightOrDayEnum NightOrDay
        {
            get { return _NightOrDay; }
            set { SetPropertyValue("NightOrDay", ref _NightOrDay, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime ZeroSetCut
        {
            get { return _ZeroSetCut; }
            set { SetPropertyValue("ZeroSetCut", ref _ZeroSetCut, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime FirstSetCut
        {
            get { return _FirstSetCut; }
            set { SetPropertyValue("FirstSetCut", ref _FirstSetCut, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime SecondSetCut
        {
            get { return _SecondSetCut; }
            set { SetPropertyValue("SecondSetCut", ref _SecondSetCut, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime ThirdSetCut
        {
            get { return _ThirdSetCut; }
            set { SetPropertyValue("ThirdSetCut", ref _ThirdSetCut, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime LastCut
        {
            get { return _LastCut; }
            set { SetPropertyValue("LastCut", ref _LastCut, value); }
        }
        
        #endregion
        [DisplayName("Late Time(Mins)")]
        public int LateTimeMins
        {
            get { return _LateTimeMins; }
            set { SetPropertyValue("LateTimeMins", ref _LateTimeMins, value); }
        }
        [DisplayName("Leave Early Time(Mins)")]
        public int LeaveEarlyTimeMins
        {
            get { return _LeaveEarlyTimeMins; }
            set { SetPropertyValue("LeaveEarlyTimeMins", ref _LeaveEarlyTimeMins, value); }
        }
        [DisplayName("Display as Workday")]
        public decimal CountAsWorkday
        {
            get { return _CountAsWorkday; }
            set { SetPropertyValue("CountAsWorkday", ref _CountAsWorkday, value); }
        }
        [DisplayName("Count as minute(Mins)")]
        public int CountAsMinute
        {
            get { return _CountAsMinute; }
            set { SetPropertyValue("CountAsMinute", ref _CountAsMinute, value); }
        }
        [DisplayName("Count as hours(Hrs)")]
        public decimal CountAsHours
        {
            get { return _CountAsHours; }
            set { SetPropertyValue("CountAsHours", ref _CountAsHours, value); }
        }
        [DisplayName("Break(Hrs)")]
        public decimal BreakHours
        {
            get { return _BreakHours; }
            set { SetPropertyValue("BreakHours", ref _BreakHours, value); }
        }
        [DisplayName("Min OT(Mins)")]
        public int MinimumOtMins
        {
            get { return _MinimumOtMins; }
            set { SetPropertyValue("MinimumOtMins", ref _MinimumOtMins, value); }
        }
        
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime NightStartTime
        {
            get { return _NightStartTime; }
            set { SetPropertyValue("NightStartTime", ref _NightStartTime, value); }
        }
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime NightEndTime
        {
            get { return _NightEndTime; }
            set { SetPropertyValue("NightEndTime", ref _NightEndTime, value); }
        }
        [DisplayName("Night Diff Rate(%)")]
        public decimal NightDiffRate
        {
            get { return _NightDiffRate; }
            set { SetPropertyValue("NightDiffRate", ref _NightDiffRate, value); }
        }
        [DisplayName("Normal OT Rate(%)")]
        public decimal NormalOtRate
        {
            get { return _NormalOtRate; }
            set { SetPropertyValue("NormalOtRate", ref _NormalOtRate, value); }
        }
        [DisplayName("Restday Rate(%)")]
        public decimal RestdayRate
        {
            get { return _RestdayRate; }
            set { SetPropertyValue("RestdayRate", ref _RestdayRate, value); }
        }
        [DisplayName("Restday OT Rate(%)")]
        public decimal RestdayOtRate
        {
            get { return _RestdayOtRate; }
            set { SetPropertyValue("RestdayOtRate", ref _RestdayOtRate, value); }
        }
        [Association("AssignedEmployees")]
        public XPCollection<Employee> AssignedEmployees
        {
            get
            {
                return
                    GetCollection<Employee>("AssignedEmployees");
            }
        }
        public TimeTable2(Session session)
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
        }
    }

}
