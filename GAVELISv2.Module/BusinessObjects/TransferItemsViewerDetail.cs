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
    public class TransferItemsViewerDetail : XPObject, IWithLineNumber
    {
        private Guid _RowID;
        private TransferItemsViewer _HeaderId;
        private DateTime _EntryDate;
        private DateTime _DateReceived;
        private TransferOrder _TransferNo;
        private Item _ItemNo;
        private Warehouse _FromWarehouse;
        private Warehouse _ToWarehouse;
        private Requisition _RequisitionNo;
        private Guid _RequestID;
        private BusinessObjects.Vendor _Vendor;
        private PartsOrigin _Origin;
        private BusinessObjects.CostCenter _CostCenter;
        private decimal _Quantity;
        private BusinessObjects.UnitOfMeasure _UnitOfMeasure;
        private BusinessObjects.UnitOfMeasure _PurchaseUom;
        private decimal _Cost;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("TransferItemsViewer-Details")]
        public TransferItemsViewer HeaderId
        {
            get { return _HeaderId; }
            set { SetPropertyValue("HeaderId", ref _HeaderId, value); }
        }

        // EntryDate
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // DateReceived
        [Custom("AllowEdit", "False")]
        public DateTime DateReceived
        {
            get { return _DateReceived; }
            set { SetPropertyValue("DateReceived", ref _DateReceived, value); }
        }
        // TransferNo
        [Custom("AllowEdit", "False")]
        public TransferOrder TransferNo
        {
            get { return _TransferNo; }
            set { SetPropertyValue("TransferNo", ref _TransferNo, value); }
        }
        // ItemNo
        [Custom("AllowEdit", "False")]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        // FromWarehouse
        [Custom("AllowEdit", "False")]
        public Warehouse FromWarehouse
        {
            get { return _FromWarehouse; }
            set { SetPropertyValue("FromWarehouse", ref _FromWarehouse, value); }
        }
        // ToWarehouse
        [Custom("AllowEdit", "False")]
        public Warehouse ToWarehouse
        {
            get { return _ToWarehouse; }
            set { SetPropertyValue("ToWarehouse", ref _ToWarehouse, value); }
        }
        // RequisitionNo
        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo
        {
            get { return _RequisitionNo; }
            set { SetPropertyValue("RequisitionNo", ref _RequisitionNo, value); }
        }
        // RequestID
        [Custom("AllowEdit", "False")]
        public Guid RequestID
        {
            get { return _RequestID; }
            set { SetPropertyValue("RequestID", ref _RequestID, value); }
        }
        // Vendor
        [Custom("AllowEdit", "False")]
        public Vendor Vendor
        {
            get { return _Vendor; }
            set { SetPropertyValue("Vendor", ref _Vendor, value); }
        }
        // Origin
        [Custom("AllowEdit", "False")]
        public PartsOrigin Origin
        {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        // CostCenter
        [Custom("AllowEdit", "False")]
        [DisplayName("Charge To")]
        public CostCenter CostCenter
        {
            get { return _CostCenter; }
            set { SetPropertyValue("CostCenter", ref _CostCenter, value); }
        }
        // Quantity
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Quantity
        {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value); }
        }
        // UnitOfMeasure
        [Custom("AllowEdit", "False")]
        [DisplayName("UOM")]
        public UnitOfMeasure UnitOfMeasure
        {
            get { return _UnitOfMeasure; }
            set { SetPropertyValue("UnitOfMeasure", ref _UnitOfMeasure, value); }
        }
        // PurchaseUom
        [Custom("AllowEdit", "False")]
        [DisplayName("PUOM")]
        public UnitOfMeasure PurchaseUom
        {
            get { return _PurchaseUom; }
            set { SetPropertyValue("PurchaseUom", ref _PurchaseUom, value); }
        }
        // Cost
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        // Total
        [PersistentAlias("Quantity * Cost")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
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

        public TransferItemsViewerDetail(Session session)
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
