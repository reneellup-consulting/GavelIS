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
    public enum PmaReportypeEnum
    {
        Monthly,
        [DisplayName("As of Date")]
        Asof
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class PurchasesMovementAnalysis : XPObject
    {
        private PmaReportypeEnum _ReportType;
        private int _Year = DateTime.Now.Year;
        private MonthsEnum _Month;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private DateTime _AsOfDate=DateTime.Now;
        private PhysicalAdjustment _InitialAdjustmentDoc;
        private Item _TestItem;
        private Warehouse _TestWarehouse;

        // ReportType
        [ImmediatePostData]
        public PmaReportypeEnum ReportType
        {
            get { return _ReportType; }
            set { SetPropertyValue("ReportType", ref _ReportType, value); }
        }
        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        // Month
        public MonthsEnum Month
        {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        // StartDate
        public DateTime StartDate
        {
            get { return _StartDate; }
            set { SetPropertyValue("StartDate", ref _StartDate, value); }
        }
        // EndDate
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { SetPropertyValue("EndDate", ref _EndDate, value); }
        }
        // AsOfDate
        public DateTime AsOfDate
        {
            get { return _AsOfDate; }
            set { SetPropertyValue("AsOfDate", ref _AsOfDate, value); }
        }
        // Title
        public string Title
        {
            get
            {
                switch (ReportType)
                {
                    case PmaReportypeEnum.Monthly:
                        return string.Format("For the month of {0} {1}", Month, Year);
                    case PmaReportypeEnum.Asof:
                        return string.Format("As of {0}", AsOfDate.ToString("MMM dd, yyy"));
                    default:
                        return string.Empty;
                }
            }
        }
        // Initial Physical Adjustments
        public PhysicalAdjustment InitialAdjustmentDoc
        {
            get { return _InitialAdjustmentDoc; }
            set { SetPropertyValue("InitialAdjustmentDoc", ref _InitialAdjustmentDoc, value); }
        }

        // Test Item
        public Item TestItem
        {
            get { return _TestItem; }
            set { SetPropertyValue("TestItem", ref _TestItem, value); }
        }
        // Test Warehouse
        public Warehouse TestWarehouse
        {
            get { return _TestWarehouse; }
            set { SetPropertyValue("TestWarehouse", ref _TestWarehouse, value); }
        }
        [Aggregated,
        Association("PurchasesMovement-Buffer")]
        public XPCollection<PurchasesMovementBuffer> PurchasesMovementBuffDetails
        {
            get
            {
                return
                    GetCollection<PurchasesMovementBuffer>("PurchasesMovementBuffDetails");
            }
        }

        [Aggregated,
        Association("PurchasesMovement-Summary")]
        public XPCollection<PurchasesMovementSummary> PurchasesMovementBuffSummaries
        {
            get
            {
                return
                    GetCollection<PurchasesMovementSummary>("PurchasesMovementBuffSummaries");
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
        public PurchasesMovementAnalysis(Session session)
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
    }

}
