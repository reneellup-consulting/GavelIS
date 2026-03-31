using System;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.Globalization;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum SortByEnum
    {
        Instance,
        Receipt,
        Invoice,
        WorkOrder,
        EmpChargeSlip,
        CreditMemo,
        DebitMemo,
        FuelRequest
    }

    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("Code")]
    public class ItemsMovementGroup : XPObject
    {
        private string _Code;
        private string _Description;
        private SortByEnum _SortBy;

        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        public SortByEnum SortBy
        {
            get { return _SortBy; }
            set { SetPropertyValue("SortBy", ref _SortBy, value); }
        }

        [Aggregated,
        Association("ItemsMovementGroup-Details")]
        [NonCloneable]
        public XPCollection<ItemsMovementGroupDetail> ItemsMovementGroupDetails
        {
            get
            {
                return
                    GetCollection<ItemsMovementGroupDetail>("ItemsMovementGroupDetails");
            }
        }

        [Aggregated,
        Association("ItemsMovSelectedWhseDetails")]
        public XPCollection<ItemsMovSelectedWhseDetail> ItemsMovSelectedWhseDetails
        {
            get { return GetCollection<ItemsMovSelectedWhseDetail>("ItemsMovSelectedWhseDetails"); }
        }

        [Aggregated,
        Association("ItemsMovGrpCountDetails")]
        [NonCloneable]
        public XPCollection<ItemsMovGrpCountDetail> ItemsMovGrpCountDetails
        {
            get { return GetCollection<ItemsMovGrpCountDetail>("ItemsMovGrpCountDetails"); }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        public string SelectedWarehouses
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                SortingCollection sorting = new SortingCollection(new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));
                ItemsMovSelectedWhseDetails.Sorting = sorting;
                foreach (var item in ItemsMovSelectedWhseDetails)
                {
                    if (item.Whse != null)
                    {
                        sb.AppendFormat("{0};", item.Whse.Code);
                    }
                }
                if (sb.Length != 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
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

        public ItemsMovementGroup(Session session)
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
