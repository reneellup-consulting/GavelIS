using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    //[RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, 
    //"EmployeeNo,Date")]
    public class CheckInAndOut : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private string _EmployeeNo;
        private string _Name;
        private bool _AutoAssigned = false;
        private DateTime _Date;
        private string _TimeTable;
        private string _OnDuty;
        private string _OffDuty;
        private string _ClockIn;
        private string _ClockOut;
        private decimal _Normal;
        private decimal _RealTime;
        private string _Late;
        private string _Early;
        private bool _Absent = false;
        private string _OTTime;
        private string _WorkTime;
        private string _Exception;
        private bool _MustCIn = true;
        private bool _MustCOut = true;
        private string _Department;
        private decimal _NDays;
        private decimal _WeekEnd;
        private decimal _Holiday;
        private string _ATTTime;
        private decimal _NightDiff;
        private string _NDaysOT;
        private string _WeekEndOT;
        private string _HolidayOT;
        private AttendanceRecordStatusEnum _Status;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("StaffPayroll-InAndOuts")] // Not aggregated
        public GenJournalHeader PayrollBatchID {
            get { return _PayrollBatchID; }
            set {
                SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value);
                if (!IsLoading) {Status = _PayrollBatchID != null ? 
                    AttendanceRecordStatusEnum.Processed : 
                    AttendanceRecordStatusEnum.Open;}
            }
        }
        //[Custom("AllowEdit", "False")]
        //public string TmpEmployee {
        //    get { return _TmpEmployee; }
        //    set { SetPropertyValue("TmpEmployee", ref _TmpEmployee, value);
        //    if (!IsLoading && !string.IsNullOrEmpty(_TmpEmployee))
        //    {
        //        Employee=this.Session.FindObject<Employee>(BinaryOperator.Parse("[No]='" + _TmpEmployee + "'"));
        //    }
        //    }
        //}
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public string EmployeeNo {
            get { return _EmployeeNo; }
            set { SetPropertyValue("EmployeeNo", ref _EmployeeNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        //[Custom("AllowEdit", "False")]
        public bool AutoAssigned {
            get { return _AutoAssigned; }
            set { SetPropertyValue("AutoAssigned", ref _AutoAssigned, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public string TimeTable {
            get { return _TimeTable; }
            set { SetPropertyValue("TimeTable", ref _TimeTable, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public string OnDuty {
            get { return _OnDuty; }
            set { SetPropertyValue("OnDuty", ref _OnDuty, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public string OffDuty {
            get { return _OffDuty; }
            set { SetPropertyValue("OffDuty", ref _OffDuty, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string ClockIn {
            get { return _ClockIn; }
            set { SetPropertyValue("ClockIn", ref _ClockIn, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string ClockOut {
            get { return _ClockOut; }
            set { SetPropertyValue("ClockOut", ref _ClockOut, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal Normal {
            get { return _Normal; }
            set { SetPropertyValue("Normal", ref _Normal, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal RealTime {
            get { return _RealTime; }
            set { SetPropertyValue("RealTime", ref _RealTime, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string Late {
            get { return _Late; }
            set { SetPropertyValue("Late", ref _Late, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string Early {
            get { return _Early; }
            set { SetPropertyValue("Early", ref _Early, value); }
        }
        //[Custom("AllowEdit", "False")]
        public bool Absent {
            get { return _Absent; }
            set { SetPropertyValue("Absent", ref _Absent, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string OTTime {
            get { return _OTTime; }
            set { SetPropertyValue("OTTime", ref _OTTime, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string WorkTime {
            get { return _WorkTime; }
            set { SetPropertyValue("WorkTime", ref _WorkTime, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string Exception {
            get { return _Exception; }
            set { SetPropertyValue("Exception", ref _Exception, value); }
        }
        //[Custom("AllowEdit", "False")]
        public bool MustCIn {
            get { return _MustCIn; }
            set { SetPropertyValue("MustCIn", ref _MustCIn, value); }
        }
        //[Custom("AllowEdit", "False")]
        public bool MustCOut {
            get { return _MustCOut; }
            set { SetPropertyValue("MustCOut", ref _MustCOut, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string Department {
            get { return _Department; }
            set { SetPropertyValue("Department", ref _Department, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal NDays {
            get { return _NDays; }
            set { SetPropertyValue("NDays", ref _NDays, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal WeekEnd {
            get { return _WeekEnd; }
            set { SetPropertyValue("WeekEnd", ref _WeekEnd, value); }
        }
        //[Custom("AllowEdit", "False")]
        public decimal Holiday {
            get { return _Holiday; }
            set { SetPropertyValue("Holiday", ref _Holiday, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string ATTTime {
            get { return _ATTTime; }
            set { SetPropertyValue("ATTTime", ref _ATTTime, value); }
        }
        //[Custom("DisplayFormat", "n")]
        public decimal NightDiff {
            get {
                if (!string.IsNullOrEmpty(_ClockIn) && !string.IsNullOrEmpty(
                _ClockOut)) {
                    DateTime nde = DateTime.Parse("22:00");
                    DateTime ndl = DateTime.Parse("6:00");
                    DateTime mid = DateTime.Parse("23:59:59");
                    DateTime tst = DateTime.Parse(_ClockOut);
                    DateTime tsp = DateTime.Parse(_ClockIn);
                    TimeSpan tspn = new TimeSpan(tst.Hour, tst.Minute, tst.
                    Second);
                    if (tst <= ndl) {
                        if (tsp >= nde) {
                            TimeSpan diff = mid.Subtract(tsp).Add(new TimeSpan(0
                            , 0, 01));
                            _NightDiff = (decimal)tspn.Add(diff).TotalHours;
                        } else {
                            _NightDiff = (decimal)tspn.Add(new TimeSpan(2, 0, 0)
                            ).TotalHours;
                        }
                    }
                    if (tst >= nde) {
                        if (tsp >= nde) {
                            TimeSpan diff = tsp.Subtract(nde);
                            _NightDiff = (decimal)tspn.Subtract((new TimeSpan(22
                            , 0, 0)) + diff).TotalHours;
                        } else {
                            _NightDiff = (decimal)tspn.Subtract(new TimeSpan(22, 
                            0, 0)).TotalHours;
                        }
                    }
                }
                return _NightDiff;
            }
        }
        //[Custom("AllowEdit", "False")]
        public string NDaysOT {
            get { return _NDaysOT; }
            set { SetPropertyValue("NDaysOT", ref _NDaysOT, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string WeekEndOT {
            get { return _WeekEndOT; }
            set { SetPropertyValue("WeekEndOT", ref _WeekEndOT, value); }
        }
        //[Custom("AllowEdit", "False")]
        public string HolidayOT {
            get { return _HolidayOT; }
            set { SetPropertyValue("HolidayOT", ref _HolidayOT, value); }
        }
        //[Custom("AllowEdit", "False")]
        public AttendanceRecordStatusEnum Status {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); }
        }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        #endregion
        public CheckInAndOut(Session session): base(session) {
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
            RowID = Guid.NewGuid();
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null) {
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
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }
            #endregion
        }
        protected override void OnDeleting() { if (_Status != 
            AttendanceRecordStatusEnum.Open) {throw new UserFriendlyException(
                "The system prohibits the deletion of already processed records."
                );} }

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
