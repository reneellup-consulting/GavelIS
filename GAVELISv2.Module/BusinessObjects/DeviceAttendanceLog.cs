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
using System.Drawing;
using System.Globalization;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum VerifyModeEnum
    {
        Password = 0,
        Fingerprint = 1,
        System = 2
    }
    public enum InOutModeEnum
    {
        [DisplayName("Check-In")]
        CheckIn = 0,
        [DisplayName("Check-Out")]
        CheckOut = 1,
        [DisplayName("Break-Out")]
        BreakOut = 2,
        [DisplayName("Break-In")]
        BreakIn = 3,
        [DisplayName("Overtime-In")]
        OvertimeIn = 4,
        [DisplayName("Overtime-Out")]
        OvertimeOut = 5
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class DeviceAttendanceLog : XPObject {
        private string _EnrolledNo;
        private string _EmployeeName;
        private VerifyModeEnum _VerifyMode;
        private InOutModeEnum _InOutMode;
        private DateTime _LogTime;
        private int _DwYear;
        private int _DwMonth;
        private int _DwDay;
        private int _DwHour;
        private int _DwMinute;
        private int _DwSecond;
        private int _DwWorkCode;
        private string _LogId;
        private string _SortLogTime;
        [Custom("AllowEdit", "False")]
        [Association]
        public CheckInAndOut03 checkId
        {
            get { return _checkId; }
            set
            {
                SetPropertyValue<CheckInAndOut03>("checkId", ref _checkId, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string EnrolledNo {
            get { return _EnrolledNo; }
            set
            {
                SetPropertyValue<string>("EnrolledNo", ref _EnrolledNo, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string EmployeeName {
            get { return _EmployeeName; }
            set
            {
                SetPropertyValue<string>("EmployeeName", ref _EmployeeName, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public VerifyModeEnum VerifyMode {
            get { return _VerifyMode; }
            set
            {
                SetPropertyValue<VerifyModeEnum>("VerifyMode", ref _VerifyMode, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public InOutModeEnum InOutMode {
            get { return _InOutMode; }
            set
            {
                SetPropertyValue<InOutModeEnum>("InOutMode", ref _InOutMode, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public DateTime LogTime {
            get { return _LogTime; }
            set
            {
                SetPropertyValue<DateTime>("LogTime", ref _LogTime, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwYear {
            get { return _DwYear; }
            set
            {
                SetPropertyValue<int>("DwYear", ref _DwYear, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwMonth {
            get { return _DwMonth; }
            set
            {
                SetPropertyValue<int>("DwMonth", ref _DwMonth, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwDay {
            get { return _DwDay; }
            set
            {
                SetPropertyValue<int>("DwDay", ref _DwDay, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwHour {
            get { return _DwHour; }
            set
            {
                SetPropertyValue<int>("DwHour", ref _DwHour, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwMinute {
            get { return _DwMinute; }
            set
            {
                SetPropertyValue<int>("DwMinute", ref _DwMinute, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwSecond {
            get { return _DwSecond; }
            set
            {
                SetPropertyValue<int>("DwSecond", ref _DwSecond, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int DwWorkCode {
            get { return _DwWorkCode; }
            set
            {
                SetPropertyValue<int>("DwWorkCode", ref _DwWorkCode, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public string LogId {
            get { return _LogId; }
            set
            {
                SetPropertyValue<string>("LogId", ref _LogId, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public string SortLogTime {
            get { return _SortLogTime; }
            set
            {
                SetPropertyValue<string>("SortLogTime", ref _SortLogTime, value);
            }
        }
        //[Custom("AllowEdit", "False")]
        //[Association]
        //public AttendanceRecord AttRecordId
        //{
        //    get { return _AttRecordId; }
        //    set
        //    {
        //        SetPropertyValue<AttendanceRecord>("AttRecordId", ref _AttRecordId, value);
        //    }
        //}
        #region Append Clock In/Out Paramters
        private InOutModeEnum _LogMode;
        private DateTime _LogDateTime;

        [NonPersistent]
        public InOutModeEnum LogMode
        {
            get { return _LogMode; }
            set
            {
                SetPropertyValue("LogMode", ref _LogMode, value);
                if (!IsLoading && !IsSaving)
                {
                    InOutMode = _LogMode;
                }
            }
        }
        [NonPersistent]
        [Custom("DisplayFormat", "MM.dd.yyyy hh:mm:ss")]
        [EditorAlias("CustomDateTimeEditor2")]
        public DateTime LogDateTime
        {
            get { return _LogDateTime; }
            set
            {
                SetPropertyValue("LogDateTime", ref _LogDateTime, value);
                if (!IsLoading && !IsSaving)
                {
                    DwYear = _LogDateTime.Year;
                    DwMonth = _LogDateTime.Month;
                    DwDay = _LogDateTime.Day;
                    DwHour = _LogDateTime.Hour;
                    DwMinute = _LogDateTime.Minute;
                    DwSecond = _LogDateTime.Second;
                    DwWorkCode = 0;
                    DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
                    // 10020 0 7/8/2016 8:41:12 AM
                    LogId = string.Format("{0} {1} {2}", _EnrolledNo, (int)InOutMode, (new DateTime(DwYear, DwMonth, DwDay, DwHour, DwMinute, DwSecond)).ToString("G", myDTFI));
                    LogTime = _LogDateTime;
                    SortLogTime = _LogDateTime.ToString(myDTFI.UniversalSortableDateTimePattern);
                }
            }
        }
        [Action(Caption="Mark to Delete", ConfirmationMessage="Are you sure you want to delete this attendance log?")]
        public void MarkToDelete(){
            this.Delete();
        }
        #endregion

        ////[DisplayName("Check-In")]
        ////CheckIn = 0,
        //[Action(Caption = "Change To Check-In", AutoCommit = true)]
        //public void ChangeToCheckin()
        //{
        //    InOutMode = InOutModeEnum.CheckIn;
        //}
        ////[DisplayName("Check-Out")]
        ////CheckOut = 1,
        //[Action(Caption = "Change To Check-Out", AutoCommit = true)]
        //public void ChangeToCheckOut()
        //{
        //    InOutMode = InOutModeEnum.CheckOut;
        //}
        ////[DisplayName("Break-Out")]
        ////BreakOut = 2,
        //[Action(Caption = "Change To Break-Out", AutoCommit = true)]
        //public void ChangeToBreakOut()
        //{
        //    InOutMode = InOutModeEnum.BreakOut;
        //}
        ////[DisplayName("Break-In")]
        ////BreakIn = 3,
        //[Action(Caption = "Change To Break-In", AutoCommit = true)]
        //public void ChangeToBreakIn()
        //{
        //    InOutMode = InOutModeEnum.BreakIn;
        //}
        ////[DisplayName("Overtime-In")]
        ////OvertimeIn = 4,
        //[Action(Caption = "Change To Overtime-In", AutoCommit = true)]
        //public void ChangeToOvertimeIn()
        //{
        //    InOutMode = InOutModeEnum.OvertimeIn;
        //}
        ////[DisplayName("Overtime-Out")]
        ////OvertimeOut = 5
        //[Action(Caption = "Change To Overtime-Out", AutoCommit = true)]
        //public void ChangeToOvertimeOut()
        //{
        //    InOutMode = InOutModeEnum.OvertimeOut;
        //}
        

        
        //[DisplayName("Employee Name")]
        //[NonPersistent]
        //public Employee EmployeeId {
        //    get
        //    {
        //        Employee emp = this.Session.FindObject<Employee>(BinaryOperator.Parse("[EnrollNumber]=?", _EnrolledNo));
        //        if (emp != null)
        //        {
        //            return emp;
        //        } else
        //        {
        //            return null;
        //        }
        //    }
        //}

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy {
            get { return createdBy; }
            set
            {
                SetPropertyValue("CreatedBy", ref createdBy, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set
            {
                SetPropertyValue("CreatedOn", ref createdOn, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set
            {
                SetPropertyValue("ModifiedBy", ref modifiedBy, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set
            {
                SetPropertyValue("ModifiedOn", ref modifiedOn, value);
            }
        }

        #endregion

        public DeviceAttendanceLog(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.

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

        protected override void OnSaving() {
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
        private AttendanceRecord _AttRecordId;
        private CheckInAndOut03 _checkId;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser {
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

        protected override void OnDeleting()
        {
            base.OnDeleting();
            if (_VerifyMode != VerifyModeEnum.System)
            {
                throw new ApplicationException("The system does not allow deletion of machine verified attendance log.");
            }
            var data = _CurrentUser.Roles.Where(o => o.Name == "Payroll" || o.Name == "Administrator").LastOrDefault();
            if (data == null)
            {
                throw new ApplicationException("You have no permission to delete attendance log.");
            }
        }

    }

}
