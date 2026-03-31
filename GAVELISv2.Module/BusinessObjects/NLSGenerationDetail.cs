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
    public class NLSGenerationDetail : XPObject
    {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        private NonLocalSupplierReportGenerator _MainId;
        [Custom("AllowEdit", "False")]
        [Association("NLSGenerationDetails")]
        public NonLocalSupplierReportGenerator MainId
        {
            get { return _MainId; }
            set { SetPropertyValue("MainId", ref _MainId, value); }
        }

        private Item _ItemNo;
        private decimal _ForVendor01;
        private decimal _ForVendor02;
        private decimal _ForVendor03;
        private decimal _ForVendor04;
        private decimal _ForVendor05;
        private decimal _ForVendor06;
        private decimal _ForVendor07;
        private decimal _ForVendor08;
        private decimal _ForVendor09;
        private decimal _ForVendor10;
        private decimal _Others;

        // ItemNo
        [Custom("AllowEdit", "False")]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        // ForVendor01
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor01
        {
            get { return _ForVendor01; }
            set { SetPropertyValue("ForVendor01", ref _ForVendor01, value); }
        }
        // ForVendor02
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor02
        {
            get { return _ForVendor02; }
            set { SetPropertyValue("ForVendor02", ref _ForVendor02, value); }
        }
        // ForVendor03
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor03
        {
            get { return _ForVendor03; }
            set { SetPropertyValue("ForVendor03", ref _ForVendor03, value); }
        }
        // ForVendor04
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor04
        {
            get { return _ForVendor04; }
            set { SetPropertyValue("ForVendor04", ref _ForVendor04, value); }
        }
        // ForVendor05
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor05
        {
            get { return _ForVendor05; }
            set { SetPropertyValue("ForVendor05", ref _ForVendor05, value); }
        }
        // ForVendor06
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor06
        {
            get { return _ForVendor06; }
            set { SetPropertyValue("ForVendor06", ref _ForVendor06, value); }
        }
        // ForVendor07
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor07
        {
            get { return _ForVendor07; }
            set { SetPropertyValue("ForVendor07", ref _ForVendor07, value); }
        }
        // ForVendor08
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor08
        {
            get { return _ForVendor08; }
            set { SetPropertyValue("ForVendor08", ref _ForVendor08, value); }
        }
        // ForVendor09
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor09
        {
            get { return _ForVendor09; }
            set { SetPropertyValue("ForVendor09", ref _ForVendor09, value); }
        }
        // ForVendor10
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ForVendor10
        {
            get { return _ForVendor10; }
            set { SetPropertyValue("ForVendor10", ref _ForVendor10, value); }
        }
        // Others
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("OTHERS")]
        public decimal Others
        {
            get { return _Others; }
            set { SetPropertyValue("Others", ref _Others, value); }
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
        public NLSGenerationDetail(Session session)
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
