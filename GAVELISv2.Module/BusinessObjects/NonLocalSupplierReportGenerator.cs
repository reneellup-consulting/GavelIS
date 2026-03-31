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
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class NonLocalSupplierReportGenerator : XPObject
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
            set
            {
                SetPropertyValue("GtpGenerationTypeEnum", ref _GtpGenerationTypeEnum, value);
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
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { SetPropertyValue("EndDate", ref _EndDate, value); }
        }

        public string Title
        {
            get
            {
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
        #region Vendor Settings
        // Vendor01
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor01
        {
            get { return _Vendor01; }
            set
            {
                SetPropertyValue("Vendor01", ref _Vendor01, value);
                if (!IsLoading && !IsSaving)
                {
                    VendorCaption01 = _Vendor01 != null ? _Vendor01.Name.ToUpper() : string.Empty;
                }
            }
        }
        // Vendor02
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor02
        {
            get { return _Vendor02; }
            set { SetPropertyValue("Vendor02", ref _Vendor02, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption02 = _Vendor02 != null ? _Vendor02.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor03
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor03
        {
            get { return _Vendor03; }
            set { SetPropertyValue("Vendor03", ref _Vendor03, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption03 = _Vendor03 != null ? _Vendor03.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor04
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor04
        {
            get { return _Vendor04; }
            set { SetPropertyValue("Vendor04", ref _Vendor04, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption04 = _Vendor04 != null ? _Vendor04.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor05
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor05
        {
            get { return _Vendor05; }
            set { SetPropertyValue("Vendor05", ref _Vendor05, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption05 = _Vendor05 != null ? _Vendor05.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor06
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor06
        {
            get { return _Vendor06; }
            set { SetPropertyValue("Vendor06", ref _Vendor06, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption06 = _Vendor06 != null ? _Vendor06.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor07
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor07
        {
            get { return _Vendor07; }
            set { SetPropertyValue("Vendor07", ref _Vendor07, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption07 = _Vendor07 != null ? _Vendor07.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor08
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor08
        {
            get { return _Vendor08; }
            set { SetPropertyValue("Vendor08", ref _Vendor08, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption08 = _Vendor08 != null ? _Vendor08.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor09
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor09
        {
            get { return _Vendor09; }
            set { SetPropertyValue("Vendor09", ref _Vendor09, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption09 = _Vendor09 != null ? _Vendor09.Name.ToUpper() : string.Empty;
            }
            }
        }
        // Vendor10
        //[RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor10
        {
            get { return _Vendor10; }
            set { SetPropertyValue("Vendor10", ref _Vendor10, value);
            if (!IsLoading && !IsSaving)
            {
                VendorCaption10 = _Vendor10 != null ? _Vendor10.Name.ToUpper() : string.Empty;
            }
            }
        }
        // VendorCaption01
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption01
        {
            get { return _VendorCaption01; }
            set { SetPropertyValue("VendorCaption01", ref _VendorCaption01, value); }
        }
        // VendorCaption02
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption02
        {
            get { return _VendorCaption02; }
            set { SetPropertyValue("VendorCaption02", ref _VendorCaption02, value); }
        }
        // VendorCaption03
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption03
        {
            get { return _VendorCaption03; }
            set { SetPropertyValue("VendorCaption03", ref _VendorCaption03, value); }
        }
        // VendorCaption04
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption04
        {
            get { return _VendorCaption04; }
            set { SetPropertyValue("VendorCaption04", ref _VendorCaption04, value); }
        }
        // VendorCaption05
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption05
        {
            get { return _VendorCaption05; }
            set { SetPropertyValue("VendorCaption05", ref _VendorCaption05, value); }
        }
        // VendorCaption06
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption06
        {
            get { return _VendorCaption06; }
            set { SetPropertyValue("VendorCaption06", ref _VendorCaption06, value); }
        }
        // VendorCaption07
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption07
        {
            get { return _VendorCaption07; }
            set { SetPropertyValue("VendorCaption07", ref _VendorCaption07, value); }
        }
        // VendorCaption08
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption08
        {
            get { return _VendorCaption08; }
            set { SetPropertyValue("VendorCaption08", ref _VendorCaption08, value); }
        }
        // VendorCaption09
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption09
        {
            get { return _VendorCaption09; }
            set { SetPropertyValue("VendorCaption09", ref _VendorCaption09, value); }
        }
        // VendorCaption10
        //[RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string VendorCaption10
        {
            get { return _VendorCaption10; }
            set { SetPropertyValue("VendorCaption10", ref _VendorCaption10, value); }
        }

        [Aggregated,
        Association("NLSGenerationDetails")]
        public XPCollection<NLSGenerationDetail> NLSGenerationDetails
        {
            get { return GetCollection<NLSGenerationDetail>("NLSGenerationDetails"); }
        }
        #endregion
        [Action(Caption = "Print All")]
        public void PrintDocument()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\NLSGeneratedReport.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            XPCollection<NonLocalSupplierReportGenerator> data = new XPCollection<NonLocalSupplierReportGenerator>(Session);
            rep.DataSource = data;
            rep.FilterString = string.Format("[Oid] = {0}", this.Oid);
            rep.ShowPreview();
        }
        [Action(Caption = "Print Two")]
        public void PrintDocumentTwo()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\NLSGeneratedReport2.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            XPCollection<NonLocalSupplierReportGenerator> data = new XPCollection<NonLocalSupplierReportGenerator>(Session);
            rep.DataSource = data;
            rep.FilterString = string.Format("[Oid] = {0}", this.Oid);
            rep.ShowPreview();
        }
        #endregion
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
        public NonLocalSupplierReportGenerator(Session session)
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
        private Vendor _Vendor01;
        private Vendor _Vendor02;
        private Vendor _Vendor03;
        private Vendor _Vendor04;
        private Vendor _Vendor05;
        private Vendor _Vendor06;
        private Vendor _Vendor07;
        private Vendor _Vendor08;
        private Vendor _Vendor09;
        private Vendor _Vendor10;
        private string _VendorCaption01;
        private string _VendorCaption02;
        private string _VendorCaption03;
        private string _VendorCaption04;
        private string _VendorCaption05;
        private string _VendorCaption06;
        private string _VendorCaption07;
        private string _VendorCaption08;
        private string _VendorCaption09;
        private string _VendorCaption10;
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