using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("ScheduleName")]
    public class ShiftTimeTable : XPObject {
        // SheduleName; //.. Morning
        private string _ScheduleName;
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ScheduleName {
            get { return _ScheduleName; }
            set
            {
                SetPropertyValue<string>("ScheduleName", ref _ScheduleName, value);
            }
        }

        // StartTime; //.. 1899-12-30 08:00:00.000
        private DateTime _StartTime;
        public DateTime StartTime {
            get { return _StartTime; }
            set
            {
                SetPropertyValue<DateTime>("StartTime", ref _StartTime, value);
            }
        }

        // EndTime; //.. 1899-12-30 12:00:00.000
        private DateTime _EndTime;
        public DateTime EndTime {
            get { return _EndTime; }
            set
            {
                SetPropertyValue<DateTime>("EndTime", ref _EndTime, value);
            }
        }

        // LateMinutes; //.. 5 mins
        private int _LateMinutes;
        public int LateMinutes {
            get { return _LateMinutes; }
            set
            {
                SetPropertyValue<int>("LateMinutes", ref _LateMinutes, value);
            }
        }

        // EarlyMinutes; //.. 1 min
        private int _EarlyMinutes;
        public int EarlyMinutes {
            get { return _EarlyMinutes; }
            set
            {
                SetPropertyValue<int>("EarlyMinutes", ref _EarlyMinutes, value);
            }
        }

        // CheckIn; //.. 1 Boolean
        private bool _CheckIn = true;
        public bool CheckIn {
            get { return _CheckIn; }
            set
            {
                SetPropertyValue<bool>("CheckIn", ref _CheckIn, value);
            }
        }

        // CheckOut; //.. 1 Boolean
        private bool _CheckOut = true;
        public bool CheckOut {
            get { return _CheckOut; }
            set
            {
                SetPropertyValue<bool>("CheckOut", ref _CheckOut, value);
            }
        }

        // CheckInTime1; //.. 1899-12-30 06:00:00.000
        private DateTime _CheckInTime1;
        public DateTime CheckInTime1 {
            get { return _CheckInTime1; }
            set
            {
                SetPropertyValue<DateTime>("CheckInTime1", ref _CheckInTime1, value);
            }
        }

        // CheckInTime2; //.. 1899-12-30 10:00:00.000
        private DateTime _CheckInTime2;
        public DateTime CheckInTime2 {
            get { return _CheckInTime2; }
            set
            {
                SetPropertyValue<DateTime>("CheckInTime2", ref _CheckInTime2, value);
            }
        }

        // CheckOutTime1; //.. 1899-12-30 10:01:00.000
        private DateTime _CheckOutTime1;
        public DateTime CheckOutTime1 {
            get { return _CheckOutTime1; }
            set
            {
                SetPropertyValue<DateTime>("CheckOutTime1", ref _CheckOutTime1, value);
            }
        }

        // CheckOutTime2; //.. 1899-12-30 12:30:00.000
        private DateTime _CheckOutTime2;
        public DateTime CheckOutTime2 {
            get { return _CheckOutTime2; }
            set
            {
                SetPropertyValue<DateTime>("CheckOutTime2", ref _CheckOutTime2, value);
            }
        }
        private bool _NoOvertime = false;
        public bool NoOvertime
        {
            get { return _NoOvertime; }
            set { SetPropertyValue("NoOvertime", ref _NoOvertime, value); }
        }
        
        // WorkDay; //.. 0.5 day
        private decimal _WorkDay;
        public decimal WorkDay {
            get { return _WorkDay; }
            set
            {
                SetPropertyValue<decimal>("WorkDay", ref _WorkDay, value);
            }
        }

        // WorkMins; //.. 240 mins
        private int _WorkMins;
        public int WorkMins {
            get { return _WorkMins; }
            set
            {
                SetPropertyValue<int>("WorkMins", ref _WorkMins, value);
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
            set
            {
                SetPropertyValue("CreatedBy", ref createdBy, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set
            {
                SetPropertyValue("CreatedOn", ref createdOn, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set
            {
                SetPropertyValue("ModifiedBy", ref modifiedBy, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set
            {
                SetPropertyValue("ModifiedOn", ref modifiedOn, value);
            }
        }

        #endregion

        public ShiftTimeTable(Session session)
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
    }

}
