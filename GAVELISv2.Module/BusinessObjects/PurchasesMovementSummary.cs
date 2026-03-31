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
    public class PurchasesMovementSummary : XPObject
    {
        private Guid _RowID;
        private PurchasesMovementAnalysis _HeaderId;

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("PurchasesMovement-Summary")]
        public PurchasesMovementAnalysis HeaderId
        {
            get { return _HeaderId; }
            set { SetPropertyValue("HeaderId", ref _HeaderId, value); }
        }

        // ItemNo
        [Custom("AllowEdit", "False")]
        [DisplayName("ITEM")]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        
        // InvBegYear
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("BEG(QTY)")]
        public decimal InvBegYear
        {
            get { return _InvBegYear; }
            set { SetPropertyValue("InvBegYear", ref _InvBegYear, value); }
        }
        // Purchases
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("PO(QTY)")]
        public decimal Purchases
        {
            get { return _Purchases; }
            set { SetPropertyValue("Purchases", ref _Purchases, value); }
        }
        // UnitOfMeasure
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("UOM")]
        public UnitOfMeasure UnitOfMeasure
        {
            get { return _UnitOfMeasure; }
            set { SetPropertyValue("UnitOfMeasure", ref _UnitOfMeasure, value); }
        }
        // ForTruckingOperations
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("TRUCKING(QTY)")]
        public decimal ForTruckingOperations
        {
            get { return _ForTruckingOperations; }
            set { SetPropertyValue("ForTruckingOperations", ref _ForTruckingOperations, value); }
        }
        // ForOtherBusinesses
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("ADMIN(QTY)")]
        public decimal ForOtherBusinesses
        {
            get { return _ForOtherBusinesses; }
            set { SetPropertyValue("ForOtherBusinesses", ref _ForOtherBusinesses, value); }
        }
        // ChargeToEmployees
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("EMPLOYEES(QTY)")]
        public decimal ChargeToEmployees
        {
            get { return _ChargeToEmployees; }
            set { SetPropertyValue("ChargeToEmployees", ref _ChargeToEmployees, value); }
        }
        // SoldToCustomers
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("SOLD(QTY)")]
        public decimal SoldToCustomers
        {
            get { return _SoldToCustomers; }
            set { SetPropertyValue("SoldToCustomers", ref _SoldToCustomers, value); }
        }
        // UsedByTheOwners
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("OWNERS(QTY)")]
        public decimal UsedByTheOwners
        {
            get { return _UsedByTheOwners; }
            set { SetPropertyValue("UsedByTheOwners", ref _UsedByTheOwners, value); }
        }
        // Others
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("OTHERS(QTY)")]
        public decimal Others
        {
            get { return _Others; }
            set { SetPropertyValue("Others", ref _Others, value); }
        }
        // UnAccounted
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("DESCREPANCY(QTY)")]
        public decimal UnAccounted
        {
            get { return _UnAccounted; }
            set { SetPropertyValue("UnAccounted", ref _UnAccounted, value); }
        }

        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private Item _ItemNo;
        private decimal _InvBegYear;
        private decimal _Purchases;
        private BusinessObjects.UnitOfMeasure _UnitOfMeasure;
        private decimal _ForTruckingOperations;
        private decimal _ForOtherBusinesses;
        private decimal _ChargeToEmployees;
        private decimal _SoldToCustomers;
        private decimal _UsedByTheOwners;
        private decimal _Others;
        private decimal _UnAccounted;

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

        public PurchasesMovementSummary(Session session)
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
