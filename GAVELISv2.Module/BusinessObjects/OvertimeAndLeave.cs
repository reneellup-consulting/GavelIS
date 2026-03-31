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
    public class OvertimeAndLeave : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private string _EmployeeNo;
        private string _Name;
        private string _ReferenceNo;
        private string _StartTime;
        private string _EndTime;
        private string _Department;
        private string _Exception;
        private string _Audited;
        private string _OldAudited;
        private string _TimeLong;
        private string _ValidTime;
        private decimal _NightDiff;
        private DateTime _Date;
        private AttendanceRecordStatusEnum _Status;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("StaffPayroll-OvertimeAndLeaves")] // Not aggregated
        public GenJournalHeader PayrollBatchID {
            get { return _PayrollBatchID; }
            set {
                SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value);
                if (!IsLoading) {Status = _PayrollBatchID != null ? 
                    AttendanceRecordStatusEnum.Processed : 
                    AttendanceRecordStatusEnum.Open;}
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string EmployeeNo {
            get { return _EmployeeNo; }
            set { SetPropertyValue("EmployeeNo", ref _EmployeeNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        [Custom("AllowEdit", "False")]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string StartTime {
            get { return _StartTime; }
            set { SetPropertyValue("StartTime", ref _StartTime, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string EndTime {
            get { return _EndTime; }
            set { SetPropertyValue("EndTime", ref _EndTime, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Department {
            get { return _Department; }
            set { SetPropertyValue("Department", ref _Department, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Exception {
            get { return _Exception; }
            set { SetPropertyValue("Exception", ref _Exception, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Audited {
            get { return _Audited; }
            set { SetPropertyValue("Audited", ref _Audited, value); }
        }
        [Custom("AllowEdit", "False")]
        public string OldAudited {
            get { return _OldAudited; }
            set { SetPropertyValue("OldAudited", ref _OldAudited, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string TimeLong {
            get { return _TimeLong; }
            set { SetPropertyValue("TimeLong", ref _TimeLong, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string ValidTime {
            get { return _ValidTime; }
            set { SetPropertyValue("ValidTime", ref _ValidTime, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal NightDiff {
            get {
                if (!string.IsNullOrEmpty(_StartTime) && !string.IsNullOrEmpty(
                _EndTime)) {
                    DateTime nde = DateTime.Parse("22:00");
                    DateTime ndl = DateTime.Parse("6:00");
                    DateTime mid = DateTime.Parse("23:59:59");
                    DateTime tst = DateTime.Parse(_EndTime);
                    DateTime tsp = DateTime.Parse(_StartTime);
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
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
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
        public OvertimeAndLeave(Session session): base(session) {
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
