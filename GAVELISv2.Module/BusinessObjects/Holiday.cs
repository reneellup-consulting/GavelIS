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
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class Holiday : BaseObject {
        private const string defaultDisplayFormat = "{Code}->{Description}";
        private string _Code;
        private string _Description;
        private DateTime _Date;
        private HolidayTypeEnum _HolidayType;
        private bool _PaidOnSpecial = true;
        private decimal _Rate = 30;
        private decimal _ExcessRate = 169;
        private decimal _RestDayRate = 195;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        public HolidayTypeEnum HolidayType {
            get { return _HolidayType; }
            set {
                SetPropertyValue("HolidayType", ref _HolidayType, value);
                if (!IsLoading) {
                    switch (_HolidayType) {
                        case HolidayTypeEnum.Special:
                            Rate = 30;
                            ExcessRate = 169;
                            RestDayRate = 150;
                            RestDayOtRate = 195;
                            break;
                        case HolidayTypeEnum.Regular:
                            Rate = 100;
                            ExcessRate = 260;
                            RestDayRate = 260;
                            RestDayOtRate = 338;
                            break;
                        case HolidayTypeEnum.Double:
                            Rate = 230;
                            ExcessRate = 350;
                            RestDayRate = 390;
                            RestDayOtRate = 400;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public bool PaidOnSpecial {
            get { return _PaidOnSpecial; }
            set { SetPropertyValue("PaidOnSpecial", ref _PaidOnSpecial, value); 
            }
        }
        public decimal Rate {
            get { return _Rate; }
            set { SetPropertyValue("Rate", ref _Rate, value); }
        }
        [DisplayName("OT Rate")]
        public decimal ExcessRate {
            get { return _ExcessRate; }
            set { SetPropertyValue("ExcessRate", ref _ExcessRate, value); }
        }
        [DisplayName("RD Rate")]
        public decimal RestDayRate {
            get { return _RestDayRate; }
            set { SetPropertyValue("RestDayRate", ref _RestDayRate, value); }
        }
        [DisplayName("RDOT Rate")]
        public decimal RestDayOtRate
        {
            get { return _RestDayOtRate; }
            set { SetPropertyValue("RestDayOtRate", ref _RestDayOtRate, value); }
        }
        [Association("AttBatchHolidays")]
        public XPCollection<AttendanceCalculator02> AttBatches
        {
            get
            {
                return
                    GetCollection<AttendanceCalculator02>("AttBatches");
            }
        }
        #region Display String
        public string DisplayName { get { return ObjectFormatter.Format(
                defaultDisplayFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); } }
        #endregion
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private decimal _RestDayOtRate;
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
        public Holiday(Session session): base(session) {
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
