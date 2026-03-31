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
    [System.ComponentModel.DefaultProperty("ItmTrckId")]
    public class ItemTrackingEntry : XPObject {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string ItmTrckId
        {
            get { return string.Format("{0:ITRK00000000}", Oid); }
        }
        private InventoryControlJournal _IcjID;
        private Item _ItemNo;
        private string _SerialNo;
        private Warehouse _Warehouse;
        private SerialNoStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Contact _DestPerson;
        private SourceType _DestSource;
        private GenJournalHeader _DestSourceNo;
        private string _DestRowID;
        [RuleRequiredField("", DefaultContexts.Save)]
        public InventoryControlJournal IcjID {
            get { return _IcjID; }
            set { SetPropertyValue("IcjID", ref _IcjID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Item ItemNo {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string SerialNo {
            get { return _SerialNo; }
            set { SetPropertyValue("SerialNo", ref _SerialNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse Warehouse {
            get { return _Warehouse; }
            set { SetPropertyValue("Warehouse", ref _Warehouse, value); }
        }

        public SerialNoStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        public Contact DestPerson {
            get { return _DestPerson; }
            set { SetPropertyValue("DestPerson", ref _DestPerson, value); }
        }

        public SourceType DestSource {
            get { return _DestSource; }
            set { SetPropertyValue("DestSource", ref _DestSource, value); }
        }

        public GenJournalHeader DestSourceNo {
            get { return _DestSourceNo; }
            set { SetPropertyValue("DestSourceNo", ref _DestSourceNo, value); }
        }

        public string DestRowID {
            get { return _DestRowID; }
            set { SetPropertyValue("DestRowID", ref _DestRowID, value); }
        }

        #region Tire Issuance

        private CostCenter _IssueToCC;
        private GenJournalHeader _OriginSourceId;
        private Requisition _RequisitionId;
        private RequisitionWorksheet _ReqWorksheetId;
        [Custom("AllowEdit", "False")]
        public CostCenter IssueToCC {
            get { return _IssueToCC; }
            set { SetPropertyValue<CostCenter>("IssueToCC", ref _IssueToCC, value); }
        }

        [Custom("AllowEdit", "False")]
        public GenJournalHeader OriginSourceId {
            get { return _OriginSourceId; }
            set { SetPropertyValue<GenJournalHeader>("OriginSourceId", ref _OriginSourceId, value); }
        }

        [Custom("AllowEdit", "False")]
        public Requisition RequisitionId {
            get { return _RequisitionId; }
            set { SetPropertyValue<Requisition>("RequisitionId", ref _RequisitionId, value); }
        }
        [Custom("AllowEdit", "False")]
        public RequisitionWorksheet ReqWorksheetId {
            get { return _ReqWorksheetId; }
            set { SetPropertyValue<RequisitionWorksheet>("ReqWorksheetId", ref _ReqWorksheetId, value); }
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

        public ItemTrackingEntry(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;

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
