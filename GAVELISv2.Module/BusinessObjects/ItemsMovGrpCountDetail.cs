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
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "HeaderId, ItemNo, Warehouse")]
    public class ItemsMovGrpCountDetail : XPObject
    {
        private Guid _RowID;
        private ItemsMovementGroup _HeaderId;
        private Item _ItemNo;
        private BusinessObjects.Warehouse _Warehouse;
        private decimal _CurrQty;
        private UnitOfMeasure _StockUnit;
        private decimal _ActualQty;
        private decimal _AddNew;
        private DateTime _DateCounted;
        private PhysicalAdjustment _AdjustmentDoc;
        private int _Activity;
        private string _Remarks;

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("ItemsMovGrpCountDetails")]
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
        [DisplayName("Stock UOM")]
        public UnitOfMeasure StockUnit
        {
            get { return _StockUnit; }
            set { SetPropertyValue("StockUnit", ref _StockUnit, value); }
        }
        public decimal ActualQty
        {
            get { return _ActualQty; }
            set { SetPropertyValue("ActualQty", ref _ActualQty, value); }
        }
        [PersistentAlias("ActualQty - CurrQty")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Add|Min")]
        public decimal AddNew
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("AddNew");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }
                }
                catch (Exception)
                {
                    return 0;
                }

            }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Date Uploaded")]
        public DateTime DateCounted
        {
            get { return _DateCounted; }
            set { SetPropertyValue("DateCounted", ref _DateCounted, value); }
        }
        [Custom("AllowEdit", "False")]
        public PhysicalAdjustment AdjustmentDoc
        {
            get { return _AdjustmentDoc; }
            set { SetPropertyValue("AdjustmentDoc", ref _AdjustmentDoc, value); }
        }
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
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

        public ItemsMovGrpCountDetail(Session session)
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
