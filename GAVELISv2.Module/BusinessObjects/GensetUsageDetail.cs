using System;
using System.Linq;
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
    [NavigationItem(false)]
    public class GensetUsageDetail : XPObject {
        private Guid _RowID;
        private GensetEntry _GensetEntryID;
        private GensetUsageTypeEnum _Type;
        private DateTime _PlugIn;
        private DateTime _PlugOff;
        private TimeSpan _PluggedTime;
        private decimal _RegHrs;
        private decimal _Total;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("GensetEntry-GensetUsageDetails")]
        public GensetEntry GensetEntryID {
            get { return _GensetEntryID; }
            set {
                GensetEntry oldDocumentId = _GensetEntryID;
                SetPropertyValue("GensetEntryID", ref _GensetEntryID, value);
                if (!IsLoading && !IsSaving && oldDocumentId != _GensetEntryID)
                {
                    oldDocumentId = oldDocumentId ?? _GensetEntryID;
                    try
                    {
                        _GensetEntryID.UpdateRegularHrs(true);
                        _GensetEntryID.UpdateColdRoomHrs(true);
                        _GensetEntryID.UpdateOtherHrs(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        public GensetUsageTypeEnum Type {
            get { return _Type; }
            set {
                SetPropertyValue("Type", ref _Type, value);
                if (!IsLoading) {
                    var lastOrDefault = _GensetEntryID.GensetUsageDetails.Where(o=>o.PlugOff != DateTime.MinValue).LastOrDefault();
                    if (lastOrDefault!=null)
                    {
                        PlugIn = lastOrDefault.PlugOff;
                        PlugOff = lastOrDefault.PlugOff.AddHours(1);
                    }
                    else
                    {
                        PlugIn = _GensetEntryID.EntryDate;
                        PlugOff = _GensetEntryID.EntryDate;
                    }
                    try {
                        _GensetEntryID.UpdateRegularHrs(true);
                        _GensetEntryID.UpdateColdRoomHrs(true);
                        _GensetEntryID.UpdateOtherHrs(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime PlugIn {
            get { return _PlugIn; }
            set {
                SetPropertyValue("PlugIn", ref _PlugIn, value);
                if (!IsLoading) {
                    try {
                        RegHrs = (Decimal)(_PlugOff - _PlugIn).TotalHours;
                        //_GensetEntryID.UpdateRegularHrs(true);
                        //_GensetEntryID.UpdateColdRoomHrs(true);
                        //_GensetEntryID.UpdateOtherHrs(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime PlugOff {
            get { return _PlugOff; }
            set {
                SetPropertyValue("PlugOff", ref _PlugOff, value);
                if (!IsLoading) {
                    try {
                        RegHrs = (Decimal)(_PlugOff - _PlugIn).TotalHours;
                        //_GensetEntryID.UpdateRegularHrs(true);
                        //_GensetEntryID.UpdateColdRoomHrs(true);
                        //_GensetEntryID.UpdateOtherHrs(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [DisplayName("Plugged")]
        [PersistentAlias("PlugOff - PlugIn")]
        public TimeSpan PluggedTime
        {
            get
            {
                var tempObject = EvaluateAlias("PluggedTime");
                if (tempObject != null)
                {
                    //RegHrs = (Decimal)((TimeSpan)tempObject).TotalHours;
                    return (TimeSpan)tempObject;
                }
                else
                {
                    return TimeSpan.Zero;
                }
            }
        }
        
        [Custom("DisplayFormat", "n")]
        public decimal RegHrs {
            get { return _RegHrs; }
            set {
                SetPropertyValue("RegHrs", ref _RegHrs, value);
                if (!IsLoading) {
                    try {
                        _GensetEntryID.UpdateRegularHrs(true);
                        _GensetEntryID.UpdateColdRoomHrs(true);
                        _GensetEntryID.UpdateOtherHrs(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [PersistentAlias("PlugOff - PlugIn")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) {
                    switch (Type) {
                        case GensetUsageTypeEnum.Regular:
                            return _RegHrs;
                        case GensetUsageTypeEnum.Coldroom:
                            if (_RegHrs > 0) {return _RegHrs;} 
                            else {
                                TimeSpan tsTot = (TimeSpan)tempObject;
                                double retf = (double)tsTot.Minutes / 100;
                                double ret = ((double)tsTot.Days * 24) + (double)tsTot.Hours + retf;
                                return (decimal)ret; 
                            }
                        case GensetUsageTypeEnum.Others:
                            if (_RegHrs > 0) {return _RegHrs;} 
                            else {
                                TimeSpan tsTot = (TimeSpan)tempObject;
                                double retf = (double)tsTot.Minutes / 100;
                                double ret = ((double)tsTot.Days * 24) + (double)tsTot.Hours + retf;
                                return (decimal)ret;
                            }
                        default:
                            return (decimal)((TimeSpan)tempObject).TotalHours;
                    }
                } else {
                    return 0;
                }
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
        public GensetUsageDetail(Session session): base(session) {
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            RowID = Guid.NewGuid();
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
