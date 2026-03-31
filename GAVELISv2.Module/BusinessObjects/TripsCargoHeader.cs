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
    public class TripsCargoHeader : XPObject
    {
        private DateTime _StartDate;
        private DateTime _EndDate;
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { SetPropertyValue("EndDate", ref _EndDate, value); }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string Title
        {
            get
            {
                // Format the StartDate and EndDate in the desired format
                return "From " + StartDate.ToString("MMM. d, yyyy") + " to " + EndDate.ToString("MMM. d, yyyy");
            }
        }

        [Association("TripsCargoDetails"), Aggregated]
        public XPCollection<TripsCargoDetail> TripsCargoDetails
        {
            get
            {
                return GetCollection<TripsCargoDetail>
                    ("TripsCargoDetails");
            }
        }

        [Association("TripsCargoDetailsUnit"), Aggregated]
        public XPCollection<TripsCargoDetailUnit> TripsCargoDetailsUnit
        {
            get
            {
                return GetCollection<TripsCargoDetailUnit>
                    ("TripsCargoDetailsUnit");
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
        public TripsCargoHeader(Session session)
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
            StartDate = new DateTime(DateTime.Now.Year , DateTime.Now.Month, 1);
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
