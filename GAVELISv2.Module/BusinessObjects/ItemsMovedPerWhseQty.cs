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
    public class ItemsMovedPerWhseQty : XPObject
    {
        private Guid _RowID;
        private ItemsMovementGroupDetail _MovementDetId;
        private BusinessObjects.Warehouse _Warehouse;
        private int _Activity;
        private decimal _CurrQty;
        private bool _ToCount;
        private bool _Counted;
        private decimal _CountQty;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("ItemsMovedPerWhseQty-Lines")]
        [Custom("AllowEdit", "False")]
        public ItemsMovementGroupDetail MovementDetId
        {
            get { return _MovementDetId; }
            set { SetPropertyValue("MovementDetId", ref _MovementDetId, value); }
        }
        [Custom("AllowEdit", "False")]
        public Warehouse Warehouse
        {
            get { return _Warehouse; }
            set { SetPropertyValue("Warehouse", ref _Warehouse, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Activity
        {
            get { return _Activity; }
            set { SetPropertyValue("Activity", ref _Activity, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal CurrQty
        {
            get { return _CurrQty; }
            set { SetPropertyValue("CurrQty", ref _CurrQty, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal CountQty
        {
            get { return _CountQty; }
            set { SetPropertyValue("CountQty", ref _CountQty, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool ToCount
        {
            get { return _ToCount; }
            set { SetPropertyValue("ToCount", ref _ToCount, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Counted
        {
            get { return _Counted; }
            set { SetPropertyValue("Counted", ref _Counted, value); }
        }
        [Action(AutoCommit = true, Caption = "Mark/Unmark for Counting")]
        public void MarkAsInclude()
        {
            ToCount = !_ToCount;
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

        public ItemsMovedPerWhseQty(Session session)
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
