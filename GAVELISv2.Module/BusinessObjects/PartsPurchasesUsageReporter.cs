using System;
using System.Text;
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
    public enum ReportPeriodicTypeEnum
    {
        Yearly,
        Monthly,
        Range
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class PartsPurchasesUsageReporter : XPObject
    {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Report No.")]
        public string ReportNo
        {
            get { return string.Format("{0:PRT00000000}", Oid); }
        }

        private ReportPeriodicTypeEnum _PeriodType;
        private int _Year = DateTime.Now.Year;
        private MonthsEnum _Month;
        private DateTime _FromDate;
        private DateTime _ToDate;

        [ImmediatePostData]
        public ReportPeriodicTypeEnum PeriodType
        {
            get
            {
                return _PeriodType;
            }
            set
            {
                SetPropertyValue("PeriodType", ref _PeriodType, value);
            }
        }
        // Year
        [ImmediatePostData]
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
        [ImmediatePostData]
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
        // FromDate
        [ImmediatePostData]
        public DateTime FromDate
        {
            get
            {
                return _FromDate;
            }
            set
            {
                SetPropertyValue("FromDate", ref _FromDate, value);
            }
        }
        // ToDate
        [ImmediatePostData]
        public DateTime ToDate
        {
            get
            {
                return _ToDate;
            }
            set
            {
                SetPropertyValue("ToDate", ref _ToDate, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public string Description
        {
            get
            {
                switch (_PeriodType)
                {
                    case ReportPeriodicTypeEnum.Yearly:
                        return string.Format("For the year {0}", Year);
                    case ReportPeriodicTypeEnum.Monthly:
                        return string.Format("For the month of {0} {1}", Month, Year);
                    case ReportPeriodicTypeEnum.Range:
                        return string.Format("From {0} to {1}", FromDate.ToString("MMM dd"), ToDate.ToString("MMM dd, yyy"));
                    default:
                        return string.Empty;
                }
            }
        }
        // Parts Usage Details
        [Association("PartsUsageDetails"), Aggregated]
        public XPCollection<PartsPurchasesUsageDetail> PartsUsageDetails
        {
            get { return GetCollection<PartsPurchasesUsageDetail>("PartsUsageDetails"); }
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

        public PartsPurchasesUsageReporter(Session session)
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

            switch (DateTime.Now.Month)
            {
                case 1:
                    Month = MonthsEnum.January;
                    break;
                case 2:
                    Month = MonthsEnum.February;
                    break;
                case 3:
                    Month = MonthsEnum.March;
                    break;
                case 4:
                    Month = MonthsEnum.April;
                    break;
                case 5:
                    Month = MonthsEnum.May;
                    break;
                case 6:
                    Month = MonthsEnum.June;
                    break;
                case 7:
                    Month = MonthsEnum.July;
                    break;
                case 8:
                    Month = MonthsEnum.August;
                    break;
                case 9:
                    Month = MonthsEnum.September;
                    break;
                case 10:
                    Month = MonthsEnum.October;
                    break;
                case 11:
                    Month = MonthsEnum.November;
                    break;
                case 12:
                    Month = MonthsEnum.December;
                    break;
                default:
                    break;
            }

            ToDate = DateTime.Now;
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

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
