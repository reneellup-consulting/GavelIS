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
    public class TireServiceDetail : XPObject {
        private Guid _RowID;
        private Tire _TireNo;
        private DateTime _EntryDate;
        private Vendor _Vendor;
        private FixedAsset _PrevFleet;
        private string _OldBrandingNo;
        private FixedAsset _CurrFleet;
        private string _NewBrandingNo;
        private DateTime _DateAttached;
        private decimal _AttachOdoReading;
        private decimal _AttachTreadDepth;
        private TireDettachReason _DettachReason;
        private DateTime _DateDettached;
        private decimal _DettachOdoReading;
        private decimal _DettachTreadDepth;
        private decimal _Cost;
        private string _Remarks;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("Tire-ServiceDetails")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Tire TireNo {
            get { return _TireNo; }
            set { SetPropertyValue<Tire>("TireNo", ref _TireNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue<DateTime>("EntryDate", ref _EntryDate, value); }
        }

        public Vendor Vendor {
            get { return _Vendor; }
            set { SetPropertyValue<Vendor>("Vendor", ref _Vendor, value); }
        }

        public FixedAsset PrevFleet {
            get { return _PrevFleet; }
            set { SetPropertyValue<FixedAsset>("PrevFleet", ref _PrevFleet, value); }
        }

        public string OldBrandingNo {
            get { return _OldBrandingNo; }
            set { SetPropertyValue<string>("OldBrandingNo", ref _OldBrandingNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public FixedAsset CurrFleet {
            get { return _CurrFleet; }
            set { SetPropertyValue<FixedAsset>("CurrFleet", ref _CurrFleet, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string NewBrandingNo {
            get { return _NewBrandingNo; }
            set { SetPropertyValue<string>("NewBrandingNo", ref _NewBrandingNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime DateAttached {
            get { return _DateAttached; }
            set { SetPropertyValue<DateTime>("DateAttached", ref _DateAttached, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal AttachOdoReading {
            get { return _AttachOdoReading; }
            set { SetPropertyValue<decimal>("AttachOdoReading", ref _AttachOdoReading, value); }
        }

        public decimal AttachTreadDepth {
            get { return _AttachTreadDepth; }
            set { SetPropertyValue<decimal>("AttachTreadDepth", ref _AttachTreadDepth, value); }
        }

        public TireDettachReason DettachReason {
            get { return _DettachReason; }
            set { SetPropertyValue<TireDettachReason>("DettachReason", ref _DettachReason, value); }
        }

        public DateTime DateDettached {
            get { return _DateDettached; }
            set { SetPropertyValue<DateTime>("DateDettached", ref _DateDettached, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal DettachOdoReading {
            get { return _DettachOdoReading; }
            set { SetPropertyValue<decimal>("DettachOdoReading", ref _DettachOdoReading, value); }
        }

        public decimal DettachTreadDepth {
            get { return _DettachTreadDepth; }
            set { SetPropertyValue<decimal>("DettachTreadDepth", ref _DettachTreadDepth, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue<decimal>("Cost", ref _Cost, value); }
        }
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
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

        public TireServiceDetail(Session session)
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
            EntryDate = DateTime.Now;

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
