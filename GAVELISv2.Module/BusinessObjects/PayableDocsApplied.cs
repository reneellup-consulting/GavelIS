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
    public class PayableDocsApplied : XPObject
    {
        private Guid _RowID;
        private DateTime _EntryDate;
        private GenJournalHeader _GenJournalID;
        private string _LineID;
        private GenJournalHeader _Source;
        private BusinessObjects.SourceType _SourceType;
        private string _RefNo;
        private decimal _AppliedAmount;

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Date Applied")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set
            {
                SetPropertyValue("EntryDate", ref _EntryDate, value);
            }
        }
        [Association("GenJournalHeader-PayableDocsApplieds")]
        public GenJournalHeader GenJournalID
        {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }

        [Custom("AllowEdit", "False")]
        public string LineID
        {
            get { return _LineID; }
            set { SetPropertyValue("LineID", ref _LineID, value); }
        }

        [Custom("AllowEdit", "False")]
        public SourceType SourceType
        {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }

        [Custom("AllowEdit", "False")]
        public GenJournalHeader Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }

        [Custom("AllowEdit", "False")]
        [DisplayName("REF#")]
        public string RefNo
        {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal AppliedAmount
        {
            get { return _AppliedAmount; }
            set { SetPropertyValue("AppliedAmount", ref _AppliedAmount, value); }
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
        public PayableDocsApplied(Session session)
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
    }

}
