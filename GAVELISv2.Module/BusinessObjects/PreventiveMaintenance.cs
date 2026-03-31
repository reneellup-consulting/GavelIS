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
    public class PreventiveMaintenance : XPObject {
        private FixedAsset _Fleet;
        private string _Name;
        private string _Description;
        private ScheduleTypeEnum _ScheduleType;
        private ScheduleTypeEnum _LastScheduleType;
        private string _LastOutcome;
        private WorkOrder _LastWOrder;
        private DateTime _LastRunDate;
        private decimal _LastRunMeter;
        private int _EveryNoDays;
        //private DateTime _NextRunDate;
        //private decimal _NextRunMeter;
        private decimal _RunEveryMeter;
        //private DateTime _NotifyDate;
        //private decimal _NotifyMeter;
        private PreventiveMaintStatusEnum _Status;
        [Association("Fleet-PreventiveMaintenance")]
        public FixedAsset Fleet {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Name {
            get { return _Name; }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
                if (!IsLoading)
                {
                    Description = _Name;
                }
            }
        }

        [Size(500)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        public ScheduleTypeEnum ScheduleType {
            get { return _ScheduleType; }
            set { SetPropertyValue("ScheduleType", ref _ScheduleType, value); }
        }

        [Custom("AllowEdit", "False")]
        public ScheduleTypeEnum LastScheduleType {
            get { return _LastScheduleType; }
            set { SetPropertyValue("LastScheduleType", ref _LastScheduleType,
                value); }
        }

        [Custom("AllowEdit", "False")]
        public string LastOutcome {
            get { return _LastOutcome; }
            set { SetPropertyValue("LastOutcome", ref _LastOutcome, value); }
        }

        [Custom("AllowEdit", "False")]
        public WorkOrder LastWOrder {
            get { return _LastWOrder; }
            set { SetPropertyValue("LastWOrder", ref _LastWOrder, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime LastRunDate {
            get { return _LastRunDate; }
            set { SetPropertyValue("LastRunDate", ref _LastRunDate, value); }
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal LastRunMeter {
            get { return _LastRunMeter; }
            set { SetPropertyValue("LastRunMeter", ref _LastRunMeter, value); }
        }

        public int EveryNoDays {
            get { return _EveryNoDays; }
            set { SetPropertyValue<int>("EveryNoDays", ref _EveryNoDays, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime NextRunDate {
            get
            {
                if (ScheduleType == ScheduleTypeEnum.Date || ScheduleType == ScheduleTypeEnum.FirstCome)
                {
                    return StartBaseDate.AddDays(_EveryNoDays);
                }
                return DateTime.MinValue;
            }
            //set { SetPropertyValue("NextRunDate", ref _NextRunDate, value); }
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal NextRunMeter {
            get
            {
                if (ScheduleType == ScheduleTypeEnum.Meter || ScheduleType == ScheduleTypeEnum.FirstCome)
                {
                    return StartBaseMeter + _RunEveryMeter;
                }
                return 0m;
            }
            //set { SetPropertyValue("NextRunMeter", ref _NextRunMeter, value); }
        }

        public PreventiveMaintStatusEnum Status {
            get
            {
                //if (!Due2() && !AlmostDue() && !InProgress())
                //{
                //    return PreventiveMaintStatusEnum.Good;
                //}
                //else 
                if (InProgress())
                {
                    return PreventiveMaintStatusEnum.InProgress;
                }
                else if (Due2())
                {
                    return PreventiveMaintStatusEnum.Due;
                }
                else if (AlmostDue())
                {
                    return PreventiveMaintStatusEnum.AlmostDue;
                }
                else
                {
                    return PreventiveMaintStatusEnum.Good;
                }
            }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal RunEveryMeter {
            get { return _RunEveryMeter; }
            set
            {
                SetPropertyValue("RunEveryMeter", ref _RunEveryMeter, value);
                //if (!IsLoading && _Fleet != null)
                //{
                //    NextRunMeter = _Fleet.
                //    LastLife + _RunEveryMeter;
                //}
            }
        }

        [Custom("AllowEdit", "False")]
        public DateTime NotifyDate {
            get
            {
                if (ScheduleType == ScheduleTypeEnum.Date || ScheduleType == ScheduleTypeEnum.FirstCome)
                {
                    return StartBaseDate.AddDays(_EveryNoDays - 2);
                }
                return DateTime.MinValue;
            }
            //get { return _NotifyDate; }
            //set { SetPropertyValue("NotifyDate", ref _NotifyDate, value); }
        }
        public decimal NotificationPeriod
        {
            get { return _NotificationPeriod; }
            set { SetPropertyValue("NotificationPeriod", ref _NotificationPeriod, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal NotifyMeter {
            get
            {
                if (ScheduleType == ScheduleTypeEnum.Meter || ScheduleType == ScheduleTypeEnum.FirstCome)
                {
                    return (StartBaseMeter + _RunEveryMeter) - _NotificationPeriod;
                }
                return 0m;
            }

            //get { return _NotifyMeter; }
            //set { SetPropertyValue("NotifyMeter", ref _NotifyMeter, value); }
        }

        public bool Due2() {
            switch (_ScheduleType)
            {
                case ScheduleTypeEnum.Meter:
                    if (!InProgress() && _RunEveryMeter > 0 && _StartBaseMeter > 0)
                    {
                        if (_Fleet.Mileage02 >= NextRunMeter)
                        {
                            return true;
                        }
                    }
                    break;
                case ScheduleTypeEnum.Date:
                    if (!InProgress() && NextRunDate != DateTime.MinValue && _StartBaseDate != DateTime.MinValue)
                    {
                        if (DateTime.Now >= NextRunDate)
                        {
                            return true;
                        }
                    }
                    break;
                case ScheduleTypeEnum.FirstCome:
                    if (!InProgress() && NextRunMeter != 0 || NextRunDate != DateTime.MinValue || _StartBaseMeter > 0)
                    {
                        if (NextRunMeter != 0 && _Fleet.Mileage02 >= NextRunMeter && _StartBaseMeter > 0)
                        {
                            return true;
                        }
                        if (NextRunDate != DateTime.MinValue && DateTime.Now >= NextRunDate && _StartBaseDate != DateTime.MinValue)
                        {
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        public bool Due() {
            bool x = false;
            if (_Fleet == null || _LastOutcome == "Cancelled" || _LastOutcome ==
            "Done")
            {
                return false;
            }
            switch (_ScheduleType)
            {
                case ScheduleTypeEnum.Meter:
                    if (NotifyMeter == 0)
                    {
                        if (_Fleet.Mileage02 >= NextRunMeter
                        )
                        {
                            x = true;
                        }
                    } else
                    {
                        if (_Fleet.Mileage02 >= NotifyMeter)
                        {
                            x = true;
                        }
                    }
                    break;
                case ScheduleTypeEnum.Date:
                    if (DateTime.Now >= NextRunDate)
                    {
                        x = true;
                    }
                    break;
                case ScheduleTypeEnum.FirstCome:
                    if (NotifyMeter == 0)
                    {
                        if (_Fleet.Mileage02 >= NextRunMeter
                        )
                        {
                            x = true;
                        }
                    } else
                    {
                        if (_Fleet.Mileage02 >= NotifyMeter)
                        {
                            x = true;
                        }
                    }
                    if (DateTime.Now >= NextRunDate)
                    {
                        x = true;
                    }
                    break;
                default:
                    break;
            }
            return x;
        }

        public bool InProgress() {
            if (_LastOutcome == "In Progress")
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool Done() {
            if (_LastOutcome == "Done")
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool Cancelled() {
            if (_LastOutcome == "Cancelled")
            {
                return true;
            } else
            {
                return false;
            }
        }

        public bool AlmostDue() {
            switch (_ScheduleType)
            {
                case ScheduleTypeEnum.Meter:
                    if (_RunEveryMeter > 0)
                    {
                        if (_Fleet.Mileage02 >= NotifyMeter)
                        {
                            return true;
                        }
                    }
                    break;
                case ScheduleTypeEnum.Date:
                    if (NextRunDate != DateTime.MinValue)
                    {
                        if (DateTime.Now >= NotifyDate)
                        {
                            return true;
                        }
                    }
                    break;
                case ScheduleTypeEnum.FirstCome:
                    if (NotifyMeter != 0 || NotifyDate != DateTime.MinValue)
                    {
                        if (NotifyMeter != 0 && _Fleet.Mileage02 >= NotifyMeter)
                        {
                            return true;
                        }
                        if (NotifyDate != DateTime.MinValue && DateTime.Now >= NotifyDate)
                        {
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        #region After Odo Autocorrect

        private decimal _StartBaseMeter = 0m;
        private DateTime _StartBaseDate = DateTime.MinValue;
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal StartBaseMeter {
            get { return _StartBaseMeter; }
            set { SetPropertyValue<decimal>("StartBaseMeter", ref _StartBaseMeter, value); }
        }

        public DateTime StartBaseDate {
            get { return _StartBaseDate; }
            set { SetPropertyValue<DateTime>("StartBaseDate", ref _StartBaseDate, value); }
        }

        #endregion

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

        public PreventiveMaintenance(Session session)
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

        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _NotificationPeriod = 500m;
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
