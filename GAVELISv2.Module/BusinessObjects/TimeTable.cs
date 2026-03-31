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
    public enum ShiftClassEnum
    {
        Dayshift,
        Nightshift
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class TimeTable : XPObject
    {
        private string _TimeTableName;
        private ShiftClassEnum _ShiftClass;
        private DateTime _OnDutyTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _OffDutyTime = new DateTime(1753, 1, 1, 0, 0, 0);
        private bool _NextDay = false;
        private int _LateTimeMins;
        private int _LeaveEarlyTimeMins;
        private DateTime _BeginningIn = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _EndingIn = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _BeginningOut = new DateTime(1753, 1, 1, 0, 0, 0);
        private DateTime _EndingOut = new DateTime(1753, 1, 1, 0, 0, 0);
        private decimal _CountAsWorkday;
        private int _CountAsMinute;
        private decimal _CountAsHours;
        private bool _MustCIn = true;
        private bool _MustCOut = true;
        private InOutModeEnum _DefaultMode;
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
        public ShiftClassEnum ShiftClass
        {
            get { return _ShiftClass; }
            set { SetPropertyValue("ShiftClass", ref _ShiftClass, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat","HH:mm")]
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
        public DateTime OffDutyTime
        {
            get { return _OffDutyTime; }
            set { SetPropertyValue("OffDutyTime", ref _OffDutyTime, value); }
        }
        public bool NextDay
        {
            get { return _NextDay; }
            set { SetPropertyValue("NextDay", ref _NextDay, value); }
        }
        
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
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime BeginningIn
        {
            get { return _BeginningIn; }
            set { SetPropertyValue("BeginningIn", ref _BeginningIn, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime EndingIn
        {
            get { return _EndingIn; }
            set { SetPropertyValue("EndingIn", ref _EndingIn, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime BeginningOut
        {
            get { return _BeginningOut; }
            set { SetPropertyValue("BeginningOut", ref _BeginningOut, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime EndingOut
        {
            get { return _EndingOut; }
            set { SetPropertyValue("EndingOut", ref _EndingOut, value); }
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
        
        [DisplayName("Must C/In")]
        public bool MustCIn
        {
            get { return _MustCIn; }
            set { SetPropertyValue("MustCIn", ref _MustCIn, value); }
        }
        [DisplayName("Must C/Out")]
        public bool MustCOut
        {
            get { return _MustCOut; }
            set { SetPropertyValue("MustCOut", ref _MustCOut, value); }
        }
        public InOutModeEnum DefaultMode
        {
            get { return _DefaultMode; }
            set { SetPropertyValue("DefaultMode", ref _DefaultMode, value); }
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
        
        private bool _NoOvertime = false;
        public bool NoOvertime
        {
            get { return _NoOvertime; }
            set { SetPropertyValue("NoOvertime", ref _NoOvertime, value); }
        }
        [Aggregated, Association("EmployeeShift")]
        public XPCollection<ShiftEmployee> EmployeeShift
        {
            get
            {
                return
                    GetCollection<ShiftEmployee>("EmployeeShift");
            }
        }
        public TimeTable(Session session)
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
