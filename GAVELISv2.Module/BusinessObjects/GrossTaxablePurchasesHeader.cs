using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Reports;
using System.IO;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum GtpGenerationTypeEnum
    {
        Monthly,
        Range
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class GrossTaxablePurchasesHeader : XPObject
    {
        private Guid _RowID;
        private BusinessObjects.GtpGenerationTypeEnum _GtpGenerationTypeEnum;
        private int _Year;
        private MonthsEnum _Month;
        private DateTime _StartDate;
        private DateTime _EndDate;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [DisplayName("Generation Type")]
        [ImmediatePostData(true)]
        public GtpGenerationTypeEnum GtpGenerationTypeEnum
        {
            get { return _GtpGenerationTypeEnum; }
            set { SetPropertyValue("GtpGenerationTypeEnum", ref _GtpGenerationTypeEnum, value);
            }
        }
        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        public MonthsEnum Month
        {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        #region GTP Generation Type DateRange

        public DateTime StartDate
        {
            get { return _StartDate; }
            set { SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { SetPropertyValue("EndDate", ref _EndDate, value); }
        }

        public string Title
        {
            get {
                switch (_GtpGenerationTypeEnum)
                {
                    case GtpGenerationTypeEnum.Monthly:
                        return string.Format("For the month of {0} {1}", _Month, _Year);
                    case GtpGenerationTypeEnum.Range:
                        return string.Format("From {0} to {1}", _StartDate.ToString("MMM dd"), _EndDate.ToString("MMM dd, yyy"));
                    default:
                        return string.Empty;
                }
                 }
        }

        [Action(Caption = "Print")]
        public void PrintDocument()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\GrossTaxablePurchases.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            XPCollection<GrossTaxablePurchasesHeader> data = new XPCollection<GrossTaxablePurchasesHeader>(Session);
            rep.DataSource = data;
            rep.FilterString = string.Format("[Oid] = {0}", this.Oid);
            rep.ShowPreview();
        }
        #endregion
        [Association("GrossTaxablePurchasesDetails"),Aggregated]
        public XPCollection<GrossTaxablePurchasesDetail> GrossTaxablePurchasesDetails
        {
            get
            {
                return GetCollection<GrossTaxablePurchasesDetail>
                    ("GrossTaxablePurchasesDetails");
            }
        }
        private MonthsEnum GetMonthValueByInt(int month)
        {
            if (month == 1)
            {
                return MonthsEnum.January;
            }
            else if (month == 2)
            {
                return MonthsEnum.February;
            }
            else if (month == 3)
            {
                return MonthsEnum.March;
            }
            else if (month == 4)
            {
                return MonthsEnum.April;
            }
            else if (month == 5)
            {
                return MonthsEnum.May;
            }
            else if (month == 6)
            {
                return MonthsEnum.June;
            }
            else if (month == 7)
            {
                return MonthsEnum.July;
            }
            else if (month == 8)
            {
                return MonthsEnum.August;
            }
            else if (month == 9)
            {
                return MonthsEnum.September;
            }
            else if (month == 10)
            {
                return MonthsEnum.October;
            }
            else if (month == 11)
            {
                return MonthsEnum.November;
            }
            else if (month == 12)
            {
                return MonthsEnum.December;
            }
            else
            {
                return MonthsEnum.None;
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

        public GrossTaxablePurchasesHeader(Session session)
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
            RowID = Guid.NewGuid();
            Year = DateTime.Now.Year;
            Month = GetMonthValueByInt(DateTime.Now.Month);
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
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
