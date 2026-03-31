using System;
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
    public class AttendanceCalculator02 : XPObject
    {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Batch No.")]
        public string AttCalNo
        {
            get { return string.Format("{0:ATT00000000}", Oid); }
        }

        // Time Range From
        private DateTime _TimeRangeFrom;
        //[RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        [DisplayName("From")]
        public DateTime TimeRangeFrom
        {
            get
            {
                return _TimeRangeFrom;
            }
            set
            {
                SetPropertyValue("TimeRangeFrom", ref _TimeRangeFrom, value);
            }
        }

        // Time Range To
        private DateTime _TimeRangeTo;
        //[RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        [DisplayName("To")]
        public DateTime TimeRangeTo
        {
            get
            {
                return _TimeRangeTo;
            }
            set
            {
                SetPropertyValue("TimeRangeTo", ref _TimeRangeTo, value);
            }
        }

        [Association]
        public XPCollection<Employee> EmployeesToInclude
        {
            get { return GetCollection<Employee>("EmployeesToInclude"); }
        }
        [Aggregated,
        Association("EmployeeTimesheets")]
        public XPCollection<AttendanceRecord> TimeSheets
        {
            get
            {
                return
                    GetCollection<AttendanceRecord>("TimeSheets");
            }
        }
        public XPCollection<CheckInAndOut03> Overtime
        {
            get
            {
                var collection = new XPCollection<CheckInAndOut03>(Session);
                collection.Filter = CriteriaOperator.Parse("[AttRecId.BatchID.Oid] = ? And [OtHours] > ?", this.Oid, new TimeSpan(0,0,0));
                return collection;
            }
        }
        public XPCollection<CheckInAndOut03> Absent
        {
            get
            {
                var collection = new XPCollection<CheckInAndOut03>(Session);
                collection.Filter = CriteriaOperator.Parse("[AttRecId.BatchID.Oid] = ? And [Absent] = ?", this.Oid, true);
                return collection;
            }
        }
        public XPCollection<CheckInAndOut03> Halfday
        {
            get
            {
                var collection = new XPCollection<CheckInAndOut03>(Session);
                collection.Filter = CriteriaOperator.Parse("[AttRecId.BatchID.Oid] = ? And [Remarks] = ?", this.Oid, "Halfday");
                return collection;
            }
        }
        public XPCollection<CheckInAndOut03> Undertime
        {
            get
            {
                var collection = new XPCollection<CheckInAndOut03>(Session);
                collection.Filter = CriteriaOperator.Parse("[AttRecId.BatchID.Oid] = ? And [UndertimeHrs] > ?", this.Oid, 0m);
                return collection;
            }
        }
        public XPCollection<CheckInAndOut03> Late
        {
            get
            {
                var collection = new XPCollection<CheckInAndOut03>(Session);
                collection.Filter = CriteriaOperator.Parse("[AttRecId.BatchID.Oid] = ? And [LateHrs] > ?", this.Oid, 0m);
                return collection;
            }
        }

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private SecurityUser _CurrentUser;
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

        // Summary Column Caption 1
        public string Day1Caption01
        {
            get { return _TimeRangeFrom.Day.ToString(); }
        }
        public string Day1Caption02
        {
            get { return _TimeRangeFrom.AddDays(1).Day.ToString(); }
        }
        public string Day1Caption03
        {
            get { return _TimeRangeFrom.AddDays(2).Day.ToString(); }
        }
        public string Day1Caption04
        {
            get { return _TimeRangeFrom.AddDays(3).Day.ToString(); }
        }
        public string Day1Caption05
        {
            get { return _TimeRangeFrom.AddDays(4).Day.ToString(); }
        }
        public string Day1Caption06
        {
            get { return _TimeRangeFrom.AddDays(5).Day.ToString(); }
        }
        public string Day1Caption07
        {
            get { return _TimeRangeFrom.AddDays(6).Day.ToString(); }
        }
        public string Day1Caption08
        {
            get { return _TimeRangeFrom.AddDays(7).Day.ToString(); }
        }
        public string Day1Caption09
        {
            get { return _TimeRangeFrom.AddDays(8).Day.ToString(); }
        }
        public string Day1Caption10
        {
            get { return _TimeRangeFrom.AddDays(9).Day.ToString(); }
        }
        public string Day1Caption11
        {
            get { return _TimeRangeFrom.AddDays(10).Day.ToString(); }
        }
        public string Day1Caption12
        {
            get { return _TimeRangeFrom.AddDays(11).Day.ToString(); }
        }
        public string Day1Caption13
        {
            get { return _TimeRangeFrom.AddDays(12).Day.ToString(); }
        }
        public string Day1Caption14
        {
            get { return _TimeRangeFrom.AddDays(13).Day.ToString(); }
        }
        public string Day1Caption15
        {
            get { return _TimeRangeFrom.AddDays(14).Day.ToString(); }
        }
        public string Day1Caption16
        {
            get { return _TimeRangeFrom.AddDays(15).Day.ToString(); }
        }

        //private XPCollection<Holiday> _Holidays;

        [Association("AttBatchHolidays")]
        public XPCollection<Holiday> Holidays
        {
            get
            {
                return
                    GetCollection<Holiday>("Holidays");
            }
        }

        // Summary Column Caption 2
        public string Day2Caption01
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom).FirstOrDefault() : null;
                if (_TimeRangeFrom.DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption02
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(1)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(1).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption03
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(2)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(2).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption04
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(3)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(3).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption05
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(4)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(4).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption06
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(5)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(5).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption07
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(6)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(6).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption08
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(7)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(7).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption09
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(8)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(8).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption10
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(9)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(9).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption11
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(10)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(10).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption12
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(11)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(11).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption13
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(12)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(12).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption14
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(13)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(13).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption15
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(14)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(14).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public string Day2Caption16
        {
            get
            {
                Holiday firstOrDefault = Holidays.Count > 0 ? Holidays.Where(o => o.Date == _TimeRangeFrom.AddDays(16)).FirstOrDefault() : null;
                if (_TimeRangeFrom.AddDays(15).DayOfWeek == DayOfWeek.Sunday)
                {
                    return "SD";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Regular)
                {
                    return "RG";
                }
                else if (firstOrDefault != null && firstOrDefault.HolidayType == HolidayTypeEnum.Special)
                {
                    return "SP";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public AttendanceCalculator02(Session session)
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
