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
    public class CalculatedRecord : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private string _EmployeeNo;
        private string _Name;
        private string _ReferenceNo;
        private decimal _Normal;
        private decimal _Actual;
        private decimal _Late;
        private decimal _Early;
        private decimal _Absent;
        private decimal _OT;
        private decimal _OUT;
        private decimal _BOUT;
        private decimal _WTime;
        private decimal _Times;
        private decimal _VIn;
        private decimal _VOut;
        private decimal _NIn;
        private decimal _NOut;
        private decimal _AFL;
        private decimal _BLeave;
        private decimal _Sick;
        private decimal _Vacation;
        private decimal _Other;
        private decimal _NDays;
        private decimal _Weekend;
        private decimal _Holiday;
        private decimal _AttTime;
        private decimal _NDaysOT;
        private decimal _WeekendOT;
        private decimal _HolidayOT;
        private AttendanceRecordStatusEnum _Status;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("StaffPayroll-CalculatedRecords")] // Not aggregated
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
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Normal {
            get { return _Normal; }
            set { SetPropertyValue("Normal", ref _Normal, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Actual {
            get { return _Actual; }
            set { SetPropertyValue("Actual", ref _Actual, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Late {
            get { return _Late; }
            set { SetPropertyValue("Late", ref _Late, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Early {
            get { return _Early; }
            set { SetPropertyValue("Early", ref _Early, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Absent {
            get { return _Absent; }
            set { SetPropertyValue("Absent", ref _Absent, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OT {
            get { return _OT; }
            set { SetPropertyValue("OT", ref _OT, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OUT {
            get { return _OUT; }
            set { SetPropertyValue("OUT", ref _OUT, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal BOUT {
            get { return _BOUT; }
            set { SetPropertyValue("BOUT", ref _BOUT, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal WTime {
            get { return _WTime; }
            set { SetPropertyValue("WTime", ref _WTime, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Times {
            get { return _Times; }
            set { SetPropertyValue("Times", ref _Times, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal VIn {
            get { return _VIn; }
            set { SetPropertyValue("VIn", ref _VIn, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal VOut {
            get { return _VOut; }
            set { SetPropertyValue("VOut", ref _VOut, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NIn {
            get { return _NIn; }
            set { SetPropertyValue("NIn", ref _NIn, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NOut {
            get { return _NOut; }
            set { SetPropertyValue("NOut", ref _NOut, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AFL {
            get { return _AFL; }
            set { SetPropertyValue("AFL", ref _AFL, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal BLeave {
            get { return _BLeave; }
            set { SetPropertyValue("BLeave", ref _BLeave, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Sick {
            get { return _Sick; }
            set { SetPropertyValue("Sick", ref _Sick, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Vacation {
            get { return _Vacation; }
            set { SetPropertyValue("Vacation", ref _Vacation, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Other {
            get { return _Other; }
            set { SetPropertyValue("Other", ref _Other, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NDays {
            get { return _NDays; }
            set { SetPropertyValue("NDays", ref _NDays, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Weekend {
            get { return _Weekend; }
            set { SetPropertyValue("Weekend", ref _Weekend, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Holiday {
            get { return _Holiday; }
            set { SetPropertyValue("Holiday", ref _Holiday, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AttTime {
            get { return _AttTime; }
            set { SetPropertyValue("AttTime", ref _AttTime, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NDaysOT {
            get { return _NDaysOT; }
            set { SetPropertyValue("NDaysOT", ref _NDaysOT, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal WeekendOT {
            get { return _WeekendOT; }
            set { SetPropertyValue("WeekendOT", ref _WeekendOT, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal HolidayOT {
            get { return _HolidayOT; }
            set { SetPropertyValue("HolidayOT", ref _HolidayOT, value); }
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
        public CalculatedRecord(Session session): base(session) {
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
