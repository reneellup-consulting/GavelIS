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
    public class ShuntingEntryDetail : XPObject {
        private Guid _RowID;
        private ShuntingEntry _ShuntingEntryID;
        private ShuntingTypeEnum _Type;
        private TripLocation _ShuntingTo;
        private decimal _NoOfBoxes;
        private decimal _PreOdoRead;
        private decimal _PostOdoRead;
        private decimal _KmsPerOdo;
        private decimal _Kms;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("ShuntingEntry-ShuntingEntryDetails")]
        public ShuntingEntry ShuntingEntryID {
            get { return _ShuntingEntryID; }
            set {
                ShuntingEntry oldDocumentId = _ShuntingEntryID;
                SetPropertyValue("ShuntingEntryID", ref _ShuntingEntryID, 
                value);
                if (!IsLoading && !IsSaving && oldDocumentId != _ShuntingEntryID)
                {
                    oldDocumentId = oldDocumentId ?? _ShuntingEntryID;
                    try
                    {
                        oldDocumentId.UpdateKMRunMnl(true);
                        oldDocumentId.UpdateAdditional(true);
                        oldDocumentId.UpdateExcess(true);
                        oldDocumentId.UpdateNoOfBoxes(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        public ShuntingTypeEnum Type {
            get { return _Type; }
            set {
                SetPropertyValue("Type", ref _Type, value);
                if (!IsLoading) {
                    try {
                        _ShuntingEntryID.UpdateKMRunMnl(true);
                        _ShuntingEntryID.UpdateAdditional(true);
                        _ShuntingEntryID.UpdateExcess(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        public TripLocation ShuntingTo {
            get { return _ShuntingTo; }
            set { SetPropertyValue("ShuntingTo", ref _ShuntingTo, value); }
        }
        public decimal NoOfBoxes
        {
            get { return _NoOfBoxes; }
            set { SetPropertyValue("NoOfBoxes", ref _NoOfBoxes, value);
            if (!IsLoading)
            {
                try
                {
                    _ShuntingEntryID.UpdateNoOfBoxes(true);
                }
                catch (Exception)
                {
                }
            }
            }
        }
        
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PreOdoRead {
            get { return _PreOdoRead; }
            set {
                SetPropertyValue("PreOdoRead", ref _PreOdoRead, value);
                if (!IsLoading) {
                    try {
                        _ShuntingEntryID.UpdateKMRunMnl(true);
                        _ShuntingEntryID.UpdateAdditional(true);
                        _ShuntingEntryID.UpdateExcess(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PostOdoRead {
            get { return _PostOdoRead; }
            set {
                SetPropertyValue("PostOdoRead", ref _PostOdoRead, value);
                if (!IsLoading) {
                    try {
                        _ShuntingEntryID.UpdateKMRunMnl(true);
                        _ShuntingEntryID.UpdateAdditional(true);
                        _ShuntingEntryID.UpdateExcess(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [PersistentAlias("PostOdoRead - PreOdoRead")]
        [Custom("DisplayFormat", "n")]
        public decimal KmsPerOdo {
            get {
                decimal oldValue = _Kms;
                decimal newValue = 0;
                object tempObject = EvaluateAlias("KmsPerOdo");
                if (tempObject != null) {
                    if ((decimal)tempObject != 0) {newValue = (decimal)
                        tempObject != 0 ? (decimal)tempObject : 0;}
                    if (newValue != 0 && newValue != oldValue) {
                        Kms = newValue;
                        //OnChanged("Kms",oldValue,_Kms);
                        //try
                        //{
                        //    _ShuntingEntryID.UpdateKMRunMnl(true);
                        //    _ShuntingEntryID.UpdateAdditional(true);
                        //    _ShuntingEntryID.UpdateExcess(true);
                        //}
                        //catch (Exception) { }
                    }
                    return (decimal)tempObject > 0 ? (decimal)tempObject : 0;
                } else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Kms {
            get { return _Kms; }
            set {
                SetPropertyValue("Kms", ref _Kms, value);
                if (!IsLoading) {
                    try {
                        _ShuntingEntryID.UpdateKMRunMnl(true);
                        _ShuntingEntryID.UpdateAdditional(true);
                        _ShuntingEntryID.UpdateExcess(true);
                    } catch (Exception) {
                    }
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
        public ShuntingEntryDetail(Session session): base(session) {
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
            //Session.OptimisticLockingReadBehavior = 
            //OptimisticLockingReadBehavior.ReloadObject;
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
