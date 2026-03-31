using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.General;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [OptimisticLocking(false)]
    public class IncomeStatementHeader : XPObject {
        private int _Year;
        private int _Month;
        private int _Day;
        private string _Title;
        private string _Period;
        private TextDateRangeType _DateRange;
        private DateTime _FromDate;
        private DateTime _ToDate;
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Title {
            get { return _Title; }
            set { SetPropertyValue("Title", ref _Title, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Period {
            get { return _Period; }
            set { SetPropertyValue("Period", ref _Period, value); }
        }
        [ImmediatePostData]
        public TextDateRangeType DateRange
        {
            get
            {
                return _DateRange;
            }
            set
            {
                SetPropertyValue("DateRange", ref _DateRange, value);
                if (!IsLoading)
                {
                    // Set FromDate and ToDate field
                    _Year = DateTime.Now.Year;
                    _Month = DateTime.Now.Month;
                    _Day = DateTime.Now.Day;
                    switch (_DateRange)
                    {
                        case TextDateRangeType.YearToDate:
                            FromDate=new DateTime(_Year,1,1);
                            ToDate=new DateTime(_Year,_Month,_Day);
                            break;
                        case TextDateRangeType.PreviousYear:
                            FromDate=new DateTime(_Year-1,1,1);
                            ToDate=new DateTime(_Year-1,12,31);
                            break;
                        case TextDateRangeType.MonthToDate:
                            FromDate=new DateTime(_Year,_Month,1);
                            ToDate = new DateTime(_Year,_Month,_Day);
                            break;
                        case TextDateRangeType.PreviousMonth:
                            FromDate=new DateTime(_Year,_Month-1,1);
                            ToDate = new DateTime(_Year, _Month - 1, DateTime.DaysInMonth(_Year, _Month - 1));
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime FromDate {
            get { return _FromDate; }
            set { SetPropertyValue("FromDate", ref _FromDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ToDate {
            get { return _ToDate; }
            set { SetPropertyValue("ToDate", ref _ToDate, value); }
        }

        [Aggregated, Association("IncomeStatementHeader-IncomeStatementDetails")]
        public XPCollection<IncomeStatementDetail> IncomeStatementDetails
        {
            get
            {
                return
                GetCollection<IncomeStatementDetail>("IncomeStatementDetails");
            }
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
        public IncomeStatementHeader(Session session): base(session) {
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
            _Year = DateTime.Now.Year;
            _Month = DateTime.Now.Month;
            _Day = DateTime.Now.Day;

            FromDate = new DateTime(_Year, 1, 1);
            ToDate = new DateTime(_Year, _Month, _Day);

            Title="INCOME STATEMENT";
            Period="For the period...";
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
    }
}
