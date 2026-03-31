using System;
using System.Linq;
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
    public enum TireShiftEnum
    {
        None,
        [DisplayName("Day Shift")]
        Dayshift,
        [DisplayName("Mid-day Shift")]
        MidShift,
        [DisplayName("Night Shift")]
        NightShift,
        [DisplayName("Mid-night Shift")]
        MidNghtShift
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("TrdID")]
    public class TiremanDaily : XPObject {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string TrdID {
            get { return string.Format("{0:TRD00000000}", Oid); }
        }

        private DateTime _EntryDate;
        private TireShiftEnum _Shift;
        private Employee _Tireman;
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue<DateTime>("EntryDate", ref _EntryDate, value); }
        }

        public TireShiftEnum Shift {
            get { return _Shift; }
            set { SetPropertyValue<TireShiftEnum>("Shift", ref _Shift, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Tireman {
            get { return _Tireman; }
            set { SetPropertyValue<Employee>("Tireman", ref _Tireman, value); }
        }

        // Tireman Daily Activities -> Association
        [Aggregated,
        Association("TiremanDailyActivities")]
        public XPCollection<TiremanDailyActivity> TiremanDailyActivities
        {
            get
            {
                return GetCollection<TiremanDailyActivity>(
                    "TiremanDailyActivities");
            }
        }

        [Aggregated,
        Association("TiremanActivtyDetails")]
        public XPCollection<TiremanActivity> TiremanActivtyDetails
        {
            get
            {
                return GetCollection<TiremanActivity>(
                    "TiremanActivtyDetails");
            }
        }

        // ActivityReason
        [Custom("AllowEdit", "False")]
        public TireDettachReason LastActivityReason
        {
            get
            {
                var data = (from rtd in TiremanActivtyDetails
                            select rtd).Where(o => o.Oid > 0).LastOrDefault();
                return data != null ? data.ActivityReason ?? null : null;
            }
        }
        // ToFleet
        [Custom("AllowEdit", "False")]
        public FixedAsset LastToFleet
        {
            get
            {
                var data = (from rtd in TiremanActivtyDetails
                            select rtd).Where(o => o.Oid > 0).LastOrDefault();
                return data != null ? data.ToFleet ?? null : null;
            }
        }
        // ToOdometer
        [Custom("AllowEdit", "False")]
        public decimal LastToOdometer
        {
            get
            {
                var data = (from rtd in TiremanActivtyDetails
                            select rtd).Where(o => o.Oid > 0).LastOrDefault();
                return data != null ? data.ToOdometer : 0m;
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

        public TiremanDaily(Session session)
            : base(session) {
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

            EntryDate = DateTime.Now;
            Shift = TireShiftEnum.Dayshift;

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

        protected override void OnSaving() {
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
