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
    public class AttendanceCalculator : XPObject
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
        //[NonPersistent]
        //[Custom("AllowEdit", "False")]
        //public DateTime FilterFrom { get { return new DateTime(_TimeRangeFrom.Year, _TimeRangeFrom.Month, _TimeRangeFrom.Day, 0, 0, 0); } }
        //[NonPersistent]
        //[Custom("AllowEdit", "False")]
        //public DateTime FilterTo { get { return new DateTime(_TimeRangeTo.Year, _TimeRangeTo.Month, _TimeRangeTo.Day, 0, 0, 0).AddDays(1); } }

        //public static AttendanceCalculator GetInstance(Session session)
        //{
        //    //Get the Singleton's instance if it exists
        //    AttendanceCalculator result = session.FindObject<AttendanceCalculator>(null);
        //    //Create the Singleton's instance
        //    if (result == null)
        //    {
        //        result = new AttendanceCalculator(session);
        //        result.TimeRangeFrom = DateTime.Now;
        //        result.TimeRangeTo = DateTime.Now.AddDays(15);
        //        result.Save();
        //    }
        //    return result;
        //}
        // Actions:
        // Calculate
        // Generate Payroll Calculations

        // Related Details:
        // Log Exceptions
        // 

        //[Aggregated, Association("ShiftCalculations")]
        //public XPCollection<CheckInAndOut02> ShiftCalculations
        //{
        //    get
        //    {
        //        var collection = GetCollection<
        //            CheckInAndOut02>("ShiftCalculations");
        //        collection.Criteria = CriteriaOperator.Parse("[Date] >= ? And [Date] < ?",_TimeRangeFrom.Date,_TimeRangeTo.AddDays(1).Date); // TODO: create criteria here
        //        return collection;
        //    }
        //}
        [Aggregated, Association("ShiftCalculations2")]
        public XPCollection<CheckInAndOut03> ShiftCalculations2
        {
            get
            {
                //var collection = GetCollection<
                //    CheckInAndOut03>("ShiftCalculations2");
                //collection.Criteria = CriteriaOperator.Parse("[Date] >= ? And [Date] < ?", _TimeRangeFrom.Date, _TimeRangeTo.AddDays(1).Date); // TODO: create criteria here
                //collection.Sorting = new SortingCollection(new SortProperty("Date", DevExpress.Xpo.DB.SortingDirection.Ascending));
                //return collection;
                return GetCollection<CheckInAndOut03>(
                    "ShiftCalculations2");
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
        public AttendanceCalculator(Session session)
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

        //Prevent the Singleton from being deleted 
        //protected override void OnDeleting()
        //{
        //    throw new UserFriendlyException(
        //    "The system prohibits the deletion of Attendance Calculator.");
        //}

    }
}