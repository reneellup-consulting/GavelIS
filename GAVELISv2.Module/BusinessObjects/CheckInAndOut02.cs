using System;
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
    public enum OtStatusEnum
    {
        None,
        Pending,
        Approved,
        Disapproved
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class CheckInAndOut02 : XPObject
    {
        private string _LineID;
        private int _RefID;
        private int _RefID02;
        private Guid _RowID;
        private AttendanceCalculator _AttCalcId;
        private string _EnrolledNo;
        private Employee _EmployeeId;
        private bool _AutoAssigned = false;
        private DateTime _Date;
        private DayOfWeek _Day;
        private TimeTable _TimeTable;
        private DateTime _OnDuty;
        private DateTime _OffDuty;
        private DateTime _ClockIn;
        private DateTime _ClockOut;
        private decimal _Normal;
        private decimal _RealTime;
        private TimeSpan _NormalHours;
        private TimeSpan _ActualLate;
        private TimeSpan _Late;
        private TimeSpan _ActualEarly;
        private TimeSpan _Early;
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
        [Custom("AllowEdit", "False")]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string LineID
        {
            get { return _LineID; }
            set { SetPropertyValue("LineID", ref _LineID, value); }
        }
        [Custom("AllowEdit", "False")]
        //[RuleUniqueValue("", DefaultContexts.Save, SkipNullOrEmptyValues = true)]
        public int RefID
        {
            get { return _RefID; }
            set { SetPropertyValue("RefID", ref _RefID, value); }
        }
        [Custom("AllowEdit", "False")]
        //[RuleUniqueValue("", DefaultContexts.Save, SkipNullOrEmptyValues = true)]
        public int RefID02
        {
            get { return _RefID02; }
            set { SetPropertyValue("RefID02", ref _RefID02, value); }
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
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Association("ShiftCalculations")]
        public AttendanceCalculator AttCalcId
        {
            get { return _AttCalcId; }
            set { SetPropertyValue("AttCalcId", ref _AttCalcId, value); }
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
        public bool AutoAssigned
        {
            get { return _AutoAssigned; }
            set { SetPropertyValue("AutoAssigned", ref _AutoAssigned, value); }
        }
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
        public TimeTable TimeTable
        {
            get { return _TimeTable; }
            set { SetPropertyValue("TimeTable", ref _TimeTable, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime OnDuty
        {
            get { return _OnDuty; }
            set { SetPropertyValue("OnDuty", ref _OnDuty, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime OffDuty
        {
            get { return _OffDuty; }
            set { SetPropertyValue("OffDuty", ref _OffDuty, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime ClockIn
        {
            get { return _ClockIn; }
            set { SetPropertyValue("ClockIn", ref _ClockIn, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "HH:mm")]
        [Custom("EditMask", "HH:mm")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime ClockOut
        {
            get { return _ClockOut; }
            set { SetPropertyValue("ClockOut", ref _ClockOut, value); }
        }
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
        [Custom("AllowEdit", "False")]
        public TimeSpan NormalHours
        {
            get { return _NormalHours; }
            set { SetPropertyValue("NormalHours", ref _NormalHours, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan ActualLate
        {
            get { return _ActualLate; }
            set { SetPropertyValue("ActualLate", ref _ActualLate, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public TimeSpan Late
        {
            get { return _Late; }
            set { SetPropertyValue("Late", ref _Late, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan ActualEarly
        {
            get { return _ActualEarly; }
            set { SetPropertyValue("ActualEarly", ref _ActualEarly, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public TimeSpan Early
        {
            get { return _Early; }
            set { SetPropertyValue("Early", ref _Early, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Absent
        {
            get { return _Absent; }
            set { SetPropertyValue("Absent", ref _Absent, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AbsentCount
        {
            get { return _AbsentCount; }
            set { SetPropertyValue("AbsentCount", ref _AbsentCount, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan AbsentHours
        {
            get { return _AbsentHours; }
            set { SetPropertyValue("AbsentHours", ref _AbsentHours, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan OtHours
        {
            get { return _OtHours; }
            set { SetPropertyValue("OtHours", ref _OtHours, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan ValidOtHours
        {
            get { return _ValidOtHours; }
            set { SetPropertyValue("ValidOtHours", ref _ValidOtHours, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan ActualHours
        {
            get { return _ActualHours; }
            set { SetPropertyValue("ActualHours", ref _ActualHours, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan ValidWorkHours
        {
            get { return _ValidWorkHours; }
            set { SetPropertyValue("ValidWorkHours", ref _ValidWorkHours, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        [Custom("AllowEdit", "False")]
        public OtStatusEnum OtStatus
        {
            get { return _OtStatus; }
            set { SetPropertyValue("OtStatus", ref _OtStatus, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan RestDay
        {
            get { return _RestDay; }
            set { SetPropertyValue("RestDay", ref _RestDay, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan Holiday
        {
            get { return _Holiday; }
            set { SetPropertyValue("Holiday", ref _Holiday, value); }
        }
        [Custom("AllowEdit", "False")]
        public TimeSpan Night
        {
            get { return _Night; }
            set { SetPropertyValue("Night", ref _Night, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Flexible
        {
            get { return _Flexible; }
            set { SetPropertyValue("Flexible", ref _Flexible, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("CalculatedAttendance")]
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
        
        #region Pay Computation
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
        private HolidayTypeEnum _HolidayType = HolidayTypeEnum.None;
        private decimal _HolidayAmt;
        private decimal _HolidayHrs2;
        private decimal _HolidayAmt2;
        [Custom("AllowEdit", "False")]
        public decimal BasicHrs
        {
            get { return _BasicHrs; }
            set { SetPropertyValue("BasicHrs", ref _BasicHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal BasicAmt
        {
            get { return _BasicAmt; }
            set { SetPropertyValue("BasicAmt", ref _BasicAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AbsentHrs
        {
            get { return _AbsentHrs; }
            set { SetPropertyValue("AbsentHrs", ref _AbsentHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AbsentAmt
        {
            get { return _AbsentAmt; }
            set { SetPropertyValue("AbsentAmt", ref _AbsentAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal LateHrs
        {
            get { return _LateHrs; }
            set { SetPropertyValue("LateHrs", ref _LateHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal LateAmt
        {
            get { return _LateAmt; }
            set { SetPropertyValue("LateAmt", ref _LateAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal UndertimeHrs
        {
            get { return _UndertimeHrs; }
            set { SetPropertyValue("UndertimeHrs", ref _UndertimeHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal UndertimeAmt
        {
            get { return _UndertimeAmt; }
            set { SetPropertyValue("UndertimeAmt", ref _UndertimeAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal RestdayOtHrs
        {
            get { return _RestdayOtHrs; }
            set { SetPropertyValue("RestdayOtHrs", ref _RestdayOtHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal RestdayOtAmt
        {
            get { return _RestdayOtAmt; }
            set { SetPropertyValue("RestdayOtAmt", ref _RestdayOtAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OvertimeHrs
        {
            get { return _OvertimeHrs; }
            set { SetPropertyValue("OvertimeHrs", ref _OvertimeHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OvertimeAmt
        {
            get { return _OvertimeAmt; }
            set { SetPropertyValue("OvertimeAmt", ref _OvertimeAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NightDiffHrs
        {
            get { return _NightDiffHrs; }
            set { SetPropertyValue("NightDiffHrs", ref _NightDiffHrs, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NightDiffAmt
        {
            get { return _NightDiffAmt; }
            set { SetPropertyValue("NightDiffAmt", ref _NightDiffAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal HolidayHrs
        {
            get { return _HolidayHrs; }
            set { SetPropertyValue("HolidayHrs", ref _HolidayHrs, value); }
        }
        private decimal _HolidayOTHrs;
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTHrs
        {
            get { return _HolidayOTHrs; }
            set { SetPropertyValue("HolidayOTHrs", ref _HolidayOTHrs, value); }
        }
        private decimal _HolidayOTAmt;
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTAmt
        {
            get { return _HolidayOTAmt; }
            set { SetPropertyValue("HolidayOTAmt", ref _HolidayOTAmt, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public HolidayTypeEnum HolidayType
        {
            get { return _HolidayType; }
            set { SetPropertyValue("HolidayType", ref _HolidayType, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal HolidayAmt
        {
            get { return _HolidayAmt; }
            set { SetPropertyValue("HolidayAmt", ref _HolidayAmt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal HolidayHrs2
        {
            get { return _HolidayHrs2; }
            set { SetPropertyValue("HolidayHrs2", ref _HolidayHrs2, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal HolidayAmt2
        {
            get { return _HolidayAmt2; }
            set { SetPropertyValue("HolidayAmt2", ref _HolidayAmt2, value); }
        }
        private decimal _HolidayOTHrs2;
        private decimal _HolidayOTAmt2;
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTHrs2
        {
            get { return _HolidayOTHrs2; }
            set { SetPropertyValue("HolidayOTHrs2", ref _HolidayOTHrs2, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTAmt2
        {
            get { return _HolidayOTAmt2; }
            set { SetPropertyValue("HolidayOTAmt2", ref _HolidayOTAmt2, value); }
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

        [NonPersistent]
        public Company CompanyInfo
        {
            get { return Company.GetInstance(Session); }
        }
        [Action(Caption= "Approve OT", ConfirmationMessage = "Do you really want to approve this overtime?", AutoCommit=true)]
        public void ApproveOT(){
            if (!new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday RGOT", "Holiday SPOT" }.Any(o => _Remarks.Contains(o)))
            {
                throw new ApplicationException("The selected line is not an overtime entry.");
            }
            OtStatus = OtStatusEnum.Approved;
        }
        [Action(Caption = "Disapprove OT", ConfirmationMessage = "Do you really want to disapprove this overtime?", AutoCommit=true)]
        public void DisapproveOT(){
            if (!new[] { "Overtime", "Holiday OT", "Rest Day OT", "Holiday RGOT", "Holiday SPOT" }.Any(o => _Remarks.Contains(o)))
            {
                throw new ApplicationException("The selected line is not an overtime entry.");
            }
            OtStatus = OtStatusEnum.Disapproved;
        }
        public CheckInAndOut02(Session session)
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
