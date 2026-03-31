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
    public class TripsFuelUsageReporter : XPObject
    {
        private GtpGenerationTypeEnum _ReportType;
        private int _Year;
        private MonthsEnum _Month;
        private DateTime _StartDate;
        private DateTime _EndDate;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Reporter No.")]
        public string UsageReportNo
        {
            get { return string.Format("{0:TFU00000000}", Oid); }
        }
        // ReportType
        [ImmediatePostData]
        [DisplayName("Mode")]
        public GtpGenerationTypeEnum ReportType
        {
            get
            {
                return _ReportType;
            }
            set
            {
                SetPropertyValue("ReportType", ref _ReportType, value);
            }
        }
        // Year
        [DisplayName("Year")]
        public int Year
        {
            get
            {
                return _Year;
            }
            set
            {
                SetPropertyValue("Year", ref _Year, value);
            }
        }
        // Month
        [DisplayName("Month")]
        public MonthsEnum Month
        {
            get
            {
                return _Month;
            }
            set
            {
                SetPropertyValue("Month", ref _Month, value);
            }
        }
        // StartDate
        [DisplayName("From")]
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }
        // EndDate
        [DisplayName("To")]
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }
            set
            {
                SetPropertyValue("EndDate", ref _EndDate, value);
            }
        }
        // Title
        public string Title
        {
            get
            {
                switch (ReportType)
                {
                    case GtpGenerationTypeEnum.Monthly:
                        return string.Format("For the month of {0} {1}", Month, Year);
                    case GtpGenerationTypeEnum.Range:
                        return string.Format("From {0} to {1}", StartDate.ToString("MMM dd"), EndDate.ToString("MMM dd, yyy"));
                    default:
                        return string.Empty;
                }
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

        public TripsFuelUsageReporter(Session session)
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
