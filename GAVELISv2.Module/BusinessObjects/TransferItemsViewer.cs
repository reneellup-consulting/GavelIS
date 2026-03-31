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
using System.Globalization;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class TransferItemsViewer : XPObject
    {
        private FrequencyFilterEnum _FrequencyFilter;
        private int _Year = DateTime.Now.Year;
        private QuarterEnum _Quarter;
        private MonthsEnum _Month;
        private WeekNumberEnum _Week;
        private DateTime _FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        private DateTime _ToDate = DateTime.Now;
        private Warehouse _FromWarehouse;
        private Warehouse _ToWarehouse;

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Viewer No.")]
        public string ViewerNo
        {
            get { return string.Format("{0:TIVR00000000}", Oid); }
        }

        [ImmediatePostData]
        [DisplayName("Filter By")]
        public FrequencyFilterEnum FrequencyFilter
        {
            get
            {
                return _FrequencyFilter;
            }
            set
            {
                SetPropertyValue("FrequencyFilter", ref _FrequencyFilter, value);
            }
        }

        [ImmediatePostData]
        [Custom("DisplayFormat", "d")]
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

        [ImmediatePostData]
        public QuarterEnum Quarter
        {
            get
            {
                return _Quarter;
            }
            set
            {
                SetPropertyValue("Quarter", ref _Quarter, value);
            }
        }

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

        [ImmediatePostData]
        public WeekNumberEnum Week
        {
            get
            {
                return _Week;
            }
            set
            {
                SetPropertyValue("Week", ref _Week, value);
            }
        }

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

        [ImmediatePostData]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse FromWarehouse
        {
            get
            {
                return _FromWarehouse;
            }
            set
            {
                SetPropertyValue("FromWarehouse", ref _FromWarehouse, value);
            }
        }

        [ImmediatePostData]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse ToWarehouse
        {
            get
            {
                return _ToWarehouse;
            }
            set
            {
                SetPropertyValue("ToWarehouse", ref _ToWarehouse, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public string Description
        {
            get
            {
                string tmp = string.Empty;

                switch (_FrequencyFilter)
                {
                    case FrequencyFilterEnum.All:
                        tmp = string.Format("From {0} to {1} For All Time", FromWarehouse, ToWarehouse);
                        break;
                    case FrequencyFilterEnum.Yearly:
                        tmp = string.Format("From {0} to {1} For the Year {2}", FromWarehouse, ToWarehouse, Year);
                        break;
                    case FrequencyFilterEnum.Quarterly:
                        tmp = string.Format("From {0} to {1} For the {2} of {3}", FromWarehouse, ToWarehouse, Quarter.Description(), Year);
                        break;
                    case FrequencyFilterEnum.Monthly:
                        tmp = string.Format("From {0} to {1} For {2} {3}", FromWarehouse, ToWarehouse, Month, Year);
                        break;
                    case FrequencyFilterEnum.Weekly:
                        tmp = string.Format("From {0} to {1} {0} Week of {2} {3}", FromWarehouse, ToWarehouse, Week, Month, Year);
                        break;
                    case FrequencyFilterEnum.Date:
                        tmp = string.Format("From {0} to {1} Dated {2} to {3}", FromWarehouse, ToWarehouse, FromDate.ToLongDateString(), ToDate.ToLongDateString());
                        break;
                    default:
                        break;
                }
                return tmp.ToUpper();
            }
        }

        public int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        #region Aggregated Collection

        [Aggregated,
        Association("TransferItemsViewer-Details")]
        public XPCollection<TransferItemsViewerDetail> TransferItemsViewerDetails
        {
            get
            {
                return
                    GetCollection<TransferItemsViewerDetail>("TransferItemsViewerDetails");
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


        public TransferItemsViewer(Session session)
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
            Week = (WeekNumberEnum)GetWeekOfMonth(DateTime.Now);
            int iQrt = (DateTime.Now.Month - 1) / 3 + 1;
            Quarter = (QuarterEnum)iQrt;

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
