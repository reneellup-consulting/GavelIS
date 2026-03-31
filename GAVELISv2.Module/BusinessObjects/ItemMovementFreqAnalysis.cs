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
    public enum FrequencyFilterEnum
    {
        All,
        Yearly,
        Quarterly,
        Monthly,
        Weekly,
        Date
    }

    public enum QuarterEnum
    {
        None,
        [System.ComponentModel.Description("1st QRT")]
        [DisplayName("1st QRT")]
        Qrt1,
        [System.ComponentModel.Description("2nd QRT")]
        [DisplayName("2nd QRT")]
        Qrt2,
        [System.ComponentModel.Description("3rd QRT")]
        [DisplayName("3rd QRT")]
        Qrt3,
        [System.ComponentModel.Description("4th QRT")]
        [DisplayName("4th QRT")]
        Qrt4
    }

    public enum WeekNumberEnum
    {
        None,
        First,
        Second,
        Third,
        Fourth,
        Fifth
    }

    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class ItemMovementFreqAnalysis : XPObject
    {
        private FrequencyFilterEnum _FrequencyFilter;
        private int _Year = DateTime.Now.Year;
        private QuarterEnum _Quarter; //= (QuarterEnum)(DateTime.Now.Month - 1) / 3 + 1;
        private MonthsEnum _Month;
        private WeekNumberEnum _Week;
        private DateTime _AsOfDate = DateTime.Now;
        private int _Top = 100;
        private BusinessObjects.Warehouse _Warehouse;

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Report No.")]
        public string AnalysisNo
        {
            get { return string.Format("{0:IMFA00000000}", Oid); }
        }

        [ImmediatePostData]
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
        public DateTime AsOfDate
        {
            get
            {
                return _AsOfDate;
            }
            set
            {
                SetPropertyValue("AsOfDate", ref _AsOfDate, value);
            }
        }

        public int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                SetPropertyValue("Top", ref _Top, value);
            }
        }

        public Warehouse Warehouse
        {
            get
            {
                return _Warehouse;
            }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
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
                        tmp = string.Format("For All Time");
                        break;
                    case FrequencyFilterEnum.Yearly:
                        tmp = string.Format("For the Year {0}", Year);
                        break;
                    case FrequencyFilterEnum.Quarterly:
                        tmp = string.Format("For the {0} of {1}", Quarter.Description(), Year);
                        break;
                    case FrequencyFilterEnum.Monthly:
                        tmp = string.Format("For {0} {1}", Month, Year);
                        break;
                    case FrequencyFilterEnum.Weekly:
                        tmp = string.Format("{0} Week of {1} {2}", Week, Month, Year);
                        break;
                    case FrequencyFilterEnum.Date:
                        tmp = string.Format("As of {0}", AsOfDate.ToLongDateString());
                        break;
                    default:
                        break;
                }

                if (_Warehouse != null)
                {
                    tmp = string.Format("{0} in WHSE# {1}", tmp, _Warehouse.Code);
                }

                return tmp;
            }
        }

        public int GetWeekOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        [Aggregated,
        Association("ItemMovementFreqAnalysis-Details")]
        public XPCollection<ItemMovementFreqAnalDetail> ItemMovementFreqAnalysisDetails
        {
            get
            {
                return
                    GetCollection<ItemMovementFreqAnalDetail>("ItemMovementFreqAnalysisDetails");
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
        public ItemMovementFreqAnalysis(Session session)
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
