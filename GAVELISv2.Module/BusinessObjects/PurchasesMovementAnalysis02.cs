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
    public class PurchasesMovementAnalysis02 : XPObject
    {
        private int _Year = DateTime.Now.Year;
        private MonthsEnum _Month;

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

        public string Title
        {
            get
            {
                return string.Format("For the month of {0} {1}", Month, Year);
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

        [Aggregated,
        Association("PurchMovAna02-RemainingInv")]
        public XPCollection<PurchMovAnaRemInventory> RemainingInv
        {
            get
            {
                return
                    GetCollection<PurchMovAnaRemInventory>("RemainingInv");
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

        public PurchasesMovementAnalysis02(Session session)
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
