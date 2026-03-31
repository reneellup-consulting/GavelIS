using System;
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
    [NavigationItem(false)]
    [RuleCriteria("", DefaultContexts.Save, "ToDate >= FromDate", "To Date should not be less than From Date")]
    public class EmpShiftSchedule : XPObject
    {
        //private Employee _Employee;
        private DateTime _FromDate;
        private DateTime _ToDate;
        private TimeTable2 _Shift;
        //[Association("Employee-ShiftSchedules")]
        //public Employee Employee
        //{
        //    get { return _Employee; }
        //    set { SetPropertyValue("Employee", ref _Employee, value); }
        //}
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime FromDate
        {
            get { return _FromDate; }
            set { SetPropertyValue("FromDate", ref _FromDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ToDate
        {
            get { return _ToDate; }
            set { SetPropertyValue("ToDate", ref _ToDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public TimeTable2 Shift
        {
            get { return _Shift; }
            set { SetPropertyValue("Shift", ref _Shift, value); }
        }
        [Association("Employee-ShiftSchedules")]
        public XPCollection<Employee> Employees
        {
            get
            {
                return GetCollection<
                    Employee>("Employees");
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

        public EmpShiftSchedule(Session session)
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
    }
}