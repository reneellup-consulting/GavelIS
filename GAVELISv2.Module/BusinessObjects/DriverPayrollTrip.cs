using System;
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
    public class DriverPayrollTrip : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private DateTime _TripDate;
        private GenJournalHeader _TripNo;
        private string _DocumentNo;
        private DriverRegistry _RegID;
        private Employee _Driver;
        private decimal _Basic;
        private decimal _AdlMiscExp;
        private decimal _MiscExp;
        private decimal _KDs;
        private decimal _Shunting;
        private bool _Posted = false;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll-Trips")]
        public GenJournalHeader PayrollBatchID {
            get { return _PayrollBatchID; }
            set { SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value)
                ; }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public DateTime TripDate {
            get { return _TripDate; }
            set { SetPropertyValue("TripDate", ref _TripDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public GenJournalHeader TripNo {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value);
            if (!IsLoading && !IsSaving)
            {
                Customer = _TripNo.TripCustomer != null ? _TripNo.TripCustomer.Name : string.Empty;
            }
            }
        }
        [Custom("AllowEdit", "False")]
        public string Customer
        {
            get { return _CustomerCode; }
            set { SetPropertyValue("CustomerCode", ref _CustomerCode, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string DocumentNo {
            get { return _DocumentNo; }
            set { SetPropertyValue("DocumentNo", ref _DocumentNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public DriverRegistry RegID {
            get { return _RegID; }
            set { SetPropertyValue("RegID", ref _RegID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }

        public decimal Basic {
            get { return _Basic; }
            set { SetPropertyValue("Basic", ref _Basic, value); }
        }

        public decimal AdlMiscExp {
            get { return _AdlMiscExp; }
            set { SetPropertyValue("AdlMiscExp", ref _AdlMiscExp, value); }
        }

        public decimal MiscExp {
            get { return _MiscExp; }
            set { SetPropertyValue("MiscExp", ref _MiscExp, value); }
        }

        public decimal KDs {
            get { return _KDs; }
            set { SetPropertyValue("KDs", ref _KDs, value); }
        }

        public decimal Shunting {
            get { return _Shunting; }
            set { SetPropertyValue("Shunting", ref _Shunting, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool Posted {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
        }

        private string _ShuntingTo;
        [Custom("AllowEdit", "False")]
        //[Size(500)]
        [DisplayName("Distination")]
        public string ShuntingTo {
            get { return _ShuntingTo; }
            set { SetPropertyValue<string>("ShuntingTo", ref _ShuntingTo, value); }
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

        public DriverPayrollTrip(Session session)
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
            RowID = Guid.NewGuid();

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

        protected override void OnDeleting() {
            if (_RegID != null)
            {
                _RegID.Status = DriverRegistryStatusEnum.
                Current;
            } else
            {
                throw new UserFriendlyException(
                "Driver Registry ID was not specified during extraction");
            }
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private string _CustomerCode;
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
