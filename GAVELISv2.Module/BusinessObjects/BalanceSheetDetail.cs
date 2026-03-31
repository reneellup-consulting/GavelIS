using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.General;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class BalanceSheetDetail : XPObject, ITreeNode {
        #region Identifiers
        private Guid _RowID;
        private BalanceSheetHeader _BalanceSheetHeadID;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("BalanceSheetHeader-BalanceSheetDetails")]
        public BalanceSheetHeader BalanceSheetHeadID {
            get { return _BalanceSheetHeadID; }
            set { SetPropertyValue("BalanceSheetHeadID", ref _BalanceSheetHeadID
                , value); }
        }
        #endregion
        
        #region Details
        private int _ID;
        private BalanceSheetDetail _ParentID;
        private Account _GLAccount;
        private string _LabelCaption;
        private decimal _Amount;
        private bool _Less = false;
        private string _LineType;
        [RuleRequiredField("", DefaultContexts.Save)]
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                SetPropertyValue("ID", ref _ID, value);
            }
        }
        [Association("BSDetail-Child")]
        public BalanceSheetDetail ParentID
        {
            get
            {
                return _ParentID;
            }
            set
            {
                SetPropertyValue("ParentID", ref _ParentID, value);
            }
        }
        public Account GLAccount
        {
            get
            {
                return _GLAccount;
            }
            set
            {
                SetPropertyValue("GLAccount", ref _GLAccount, value);
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string LabelCaption
        {
            get
            {
                return _LabelCaption;
            }
            set
            {
                SetPropertyValue("LabelCaption", ref _LabelCaption, value);
            }
        }
        public decimal Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
            }
        }
        public bool Less
        {
            get
            {
                return _Less;
            }
            set
            {
                SetPropertyValue("Less", ref _Less, value);
            }
        }
        public string LineType
        {
            get
            {
                return _LineType;
            }
            set
            {
                SetPropertyValue("LineType", ref _LineType, value);
            }
        }
        [Association("BSDetail-Child")]
        public XPCollection<BalanceSheetDetail> BalanceSheetDetails
        {
            get
            {
                return GetCollection<BalanceSheetDetail>(
                  "BalanceSheetDetails");
            }
        }
        #region ITreeNode Members
        public System.ComponentModel.IBindingList Children
        {
            get
            {
                return BalanceSheetDetails;
            }
        }
        public string Name { get { return _LabelCaption; } }
        public ITreeNode Parent { get { return _ParentID; } }
        #endregion

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
        public BalanceSheetDetail(Session session): base(session) {
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
