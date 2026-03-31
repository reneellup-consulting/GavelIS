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
    [NavigationItem(false)]
    public class ReqCarryoutTransaction : XPObject {
        private Guid _RowID;
        private RequisitionWorksheet _ReqWorksheetId;
        private GenJournalHeader _TransactionId;
        private SourceType _SourceType;
        private int _LineNo;
        private decimal _Quantity;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("ReqCarryoutTransactions")]
        public RequisitionWorksheet ReqWorksheetId {
            get { return _ReqWorksheetId; }
            set {
                RequisitionWorksheet oldRequisitionWorksheet = _ReqWorksheetId;
                bool modified = SetPropertyValue<RequisitionWorksheet>("ReqWorksheetId", ref _ReqWorksheetId, value);
                if (!IsLoading && !IsSaving && oldRequisitionWorksheet != _ReqWorksheetId && modified)
                {
                    oldRequisitionWorksheet = oldRequisitionWorksheet ?? _ReqWorksheetId;
                    oldRequisitionWorksheet.UpdateOnJO(true);
                    oldRequisitionWorksheet.UpdateOnPO(true);
                    oldRequisitionWorksheet.UpdateOnFO(true);
                    oldRequisitionWorksheet.UpdateOnSO(true);
                    oldRequisitionWorksheet.UpdateOnTO(true);
                    oldRequisitionWorksheet.UpdateOnWO(true);
                    oldRequisitionWorksheet.UpdateRecQTY(true);
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public GenJournalHeader TransactionId {
            get { return _TransactionId; }
            set { SetPropertyValue<GenJournalHeader>("TransactionId", ref _TransactionId, value); }
        }

        [Custom("AllowEdit", "False")]
        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue<SourceType>("SourceType", ref _SourceType, value); }
        }

        [Custom("AllowEdit", "False")]
        public int LineNo {
            get { return _LineNo; }
            set { SetPropertyValue<int>("LineNo", ref _LineNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal Quantity {
            get { return _Quantity; }
            set {
                bool modified = SetPropertyValue<decimal>("Quantity", ref _Quantity, value);
                if (!IsLoading && !IsSaving && ReqWorksheetId != null && modified)
                {
                    ReqWorksheetId.UpdateOnJO(true);
                    ReqWorksheetId.UpdateOnPO(true);
                    ReqWorksheetId.UpdateOnFO(true);
                    ReqWorksheetId.UpdateOnSO(true);
                    ReqWorksheetId.UpdateOnTO(true);
                    ReqWorksheetId.UpdateOnWO(true);
                    ReqWorksheetId.UpdateRecQTY(true);
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

        public ReqCarryoutTransaction(Session session)
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
