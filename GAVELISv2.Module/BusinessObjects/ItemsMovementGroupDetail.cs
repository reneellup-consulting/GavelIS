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
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "HeaderId,ItemNo")]
    public class ItemsMovementGroupDetail : XPObject, IWithLineNumber
    {
        private Guid _RowID;
        private ItemsMovementGroup _HeaderId;
        private Item _ItemNo;
        private int _NoOfInstance;
        private int _Rcpt;
        private int _Invc;
        private int _Word;
        private int _Ecs;
        private int _Cm;
        private int _Dm;
        private int _Fpr;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("ItemsMovementGroup-Details")]
        public ItemsMovementGroup HeaderId
        {
            get { return _HeaderId; }
            set { SetPropertyValue("HeaderId", ref _HeaderId, value); }
        }
        [Custom("AllowEdit", "False")]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }

        [Custom("AllowEdit", "False")]
        [DisplayName("NOI")]
        public int NoOfInstance
        {
            get { return _NoOfInstance; }
            set { SetPropertyValue("NoOfInstance", ref _NoOfInstance, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("RCPT")]
        public int Rcpt
        {
            get { return _Rcpt; }
            set { SetPropertyValue("Rcpt", ref _Rcpt, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("INVC")]
        public int Invc
        {
            get { return _Invc; }
            set { SetPropertyValue("Invc", ref _Invc, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("WORD")]
        public int Word
        {
            get { return _Word; }
            set { SetPropertyValue("Word", ref _Word, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("ECS")]
        public int Ecs
        {
            get { return _Ecs; }
            set { SetPropertyValue("Ecs", ref _Ecs, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("CRM")]
        public int Cm
        {
            get { return _Cm; }
            set { SetPropertyValue("Cm", ref _Cm, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("DBM")]
        public int Dm
        {
            get { return _Dm; }
            set { SetPropertyValue("Dm", ref _Dm, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("FPR")]
        public int Fpr
        {
            get { return _Fpr; }
            set { SetPropertyValue("Fpr", ref _Fpr, value); }
        }

        [Aggregated,
        Association("ItemsMovedPerWhseQty-Lines")]
        public XPCollection<ItemsMovedPerWhseQty> ItemsMovedPerWhseQtyLines
        {
            get
            {
                return
                    GetCollection<ItemsMovedPerWhseQty>("ItemsMovedPerWhseQtyLines");
            }
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

        public ItemsMovementGroupDetail(Session session)
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
