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

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("Code")]
    public class DriversCommision : XPObject
    {
        private Guid _RowID;
        private string _Code;
        private TripZone _Zone;
        private Location _FromLocation;
        private Location _ToLocation;
        private decimal _Distance;
        private decimal _Allowance;
        private decimal _Miscellaneous;
        private decimal _Basic;

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        // e.g. STA-AJMR-PNBO
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public TripZone Zone
        {
            get { return _Zone; }
            set { SetPropertyValue("Zone", ref _Zone, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Location FromLocation
        {
            get { return _FromLocation; }
            set { SetPropertyValue("FromLocation", ref _FromLocation, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Location ToLocation
        {
            get { return _ToLocation; }
            set { SetPropertyValue("ToLocation", ref _ToLocation, value); }
        }

        public decimal Distance
        {
            get { return _Distance; }
            set { SetPropertyValue("Distance", ref _Distance, value); }
        }

        public decimal Allowance
        {
            get { return _Allowance; }
            set { SetPropertyValue("Allowance", ref _Allowance, value); }
        }

        public decimal Miscellaneous
        {
            get { return _Miscellaneous; }
            set { SetPropertyValue("Miscellaneous", ref _Miscellaneous, value); }
        }

        public decimal Basic
        {
            get { return _Basic; }
            set { SetPropertyValue("Basic", ref _Basic, value); }
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

        public DriversCommision(Session session)
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
            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }

            #endregion
        }

        protected override void OnSaving()
        {
            base.OnSaving();


            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
            }

            #endregion

        }
    }

}
