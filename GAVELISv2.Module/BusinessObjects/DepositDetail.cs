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
    public class DepositDetail : XPObject {
        private Guid _RowID;
        private GenJournalHeader _DepositID;
        private bool _Select = false;
        private SourceType _SourceType;
        private string _SourceNo;
        private int _SourceID;
        private DateTime _Date;
        private string _CheckNo;
        private string _ReferenceNo;
        private Contact _Name;
        private PaymentTypeEnum _Mode;
        private string _Memo;
        private decimal _Amount;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-DepositDetails")]
        public GenJournalHeader DepositID {
            get { return _DepositID; }
            set { SetPropertyValue("DepositID", ref _DepositID, value); }
        }
        [ImmediatePostData]
        public bool Select {
            get { return _Select; }
            set { SetPropertyValue("Select", ref _Select, value);
            if (!IsLoading)
            {
                try
                {
                    ((Deposit)_DepositID).UpdateCount(true);
                    ((Deposit)_DepositID).UpdateTotalDeposit(true);
                }
                catch (Exception) { }
            }
            }
        }
        [Custom("AllowEdit", "False")]
        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }
        [Custom("AllowEdit", "False")]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public int SourceID {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue("CheckNo", ref _CheckNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public Contact Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }
        [Custom("AllowEdit", "False")]
        public PaymentTypeEnum Mode {
            get { return _Mode; }
            set { SetPropertyValue("Mode", ref _Mode, value); }
        }
        [Size(1000)]
        [Custom("AllowEdit", "False")]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
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
        public DepositDetail(Session session): base(session) {
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
