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
namespace GAVELISv2.Module.BusinessObjects {
    public enum BatteryStatusEnum
    {
        Available,
        Attached,
        Dettached,
        Scrap
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("BatteryName")]
    public class Battery : XPObject {
        private Guid _RowID;
        private string _BatteryNo;
        private string _Description;
        private Item _BatteryItem;
        private Vendor _PurchaseFrom;
        private DateTime _PurchaseDate;
        private string _SerialNo;
        private string _InvoiceNo;
        private decimal _Cost;
        private Requisition _ReqId;
        private RequisitionWorksheet _ReqWorksheetId;
        private DateTime _DateScrapped;
        private DateTime _DateSold;
        private GenJournalHeader _SoldRef;
        private DateTime _DateDeclared;
        private FixedAsset _InitialUnit;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("ID")]
        public string BatteryId
        {
            get { return string.Format("{0:BT00000000}", Oid); }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Name")]
        public string BatteryName
        {
            get
            {
                string unitcode = string.Empty;
                if (_InitialUnit != null)
                {
                    unitcode = _InitialUnit.No.Replace("-", "").Trim().Replace(" ", "");
                    return string.Format("{0}({1})", BatteryNo, unitcode);
                }
                else
                {
                    return _BatteryNo;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("No.")]
        public string BatteryNo
        {
            get { return _BatteryNo; }
            set { SetPropertyValue("BatteryNo", ref _BatteryNo, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Item")]
        public Item BatteryItem
        {
            get { return _BatteryItem; }
            set { SetPropertyValue("BatteryItem", ref _BatteryItem, value);
            if (!IsLoading && !IsSaving && _BatteryItem != null)
            {
                Description = _BatteryItem.Description;
            }
            }
        }
        public Vendor PurchaseFrom
        {
            get { return _PurchaseFrom; }
            set { SetPropertyValue("PurchaseFrom", ref _PurchaseFrom, value); }
        }
        public DateTime PurchaseDate
        {
            get { return _PurchaseDate; }
            set { SetPropertyValue("PurchaseDate", ref _PurchaseDate, value); }
        }
        public string SerialNo
        {
            get { return _SerialNo; }
            set { SetPropertyValue("SerialNo", ref _SerialNo, value); }
        }
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
        }
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        [Custom("AllowEdit", "False")]
        public Requisition ReqId
        {
            get { return _ReqId; }
            set { SetPropertyValue("ReqId", ref _ReqId, value); }
        }
        [Custom("AllowEdit", "False")]
        public RequisitionWorksheet ReqWorksheetId
        {
            get { return _ReqWorksheetId; }
            set { SetPropertyValue("ReqWorksheetId", ref _ReqWorksheetId, value); }
        }
        [NonPersistent]
        [DisplayName("Date Disposed")]
        [Custom("AllowEdit", "False")]
        public DateTime DateScrapped
        {
            get
            {
                BatteryDettachReason findObject = Session.FindObject<BatteryDettachReason>(BinaryOperator.Parse("[Code]=?", "DISPOSAL"));
                if (findObject != null)
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.Reason == findObject).LastOrDefault();
                    if (data != null)
                    {
                        return data.ActivityDate;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public DateTime DateSold
        {
            get {
                InvoiceDetail invDet = Session.FindObject<InvoiceDetail>(BinaryOperator.Parse("[BatterySoldId]=?", this));
                if (invDet != null)
                {
                    return invDet.GenJournalID.EntryDate;
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public GenJournalHeader SoldRef
        {
            get {
                InvoiceDetail invDet = Session.FindObject<InvoiceDetail>(BinaryOperator.Parse("[BatterySoldId]=?",this));
                if (invDet != null)
                {
                    return invDet.GenJournalID;
                }
                else
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public GenJournalHeader DisposalDoc
        {
            get
            {
                BatteryDettachReason findObject = Session.FindObject<BatteryDettachReason>(BinaryOperator.Parse("[Code]=?", "DISPOSAL"));
                if (findObject != null)
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0 && o.Reason == findObject).LastOrDefault();
                    if (data != null)
                    {
                        return data.AdjustmentDoc;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
        }
        public DateTime DateDeclared
        {
            get { return _DateDeclared; }
            set { SetPropertyValue("DateDeclared", ref _DateDeclared, value); }
        }
        public FixedAsset InitialUnit
        {
            get { return _InitialUnit; }
            set { SetPropertyValue("InitialUnit", ref _InitialUnit, value); }
        }
        
        [NonPersistent]
        [DisplayName("Last Unit")]
        public FixedAsset LastUnitAttachedTo
        {
            get
            {
                try
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.Unit ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        [NonPersistent]
        public BatteryCondition Condition
        {
            get
            {
                try
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.Condition ?? null;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        [NonPersistent]
        [Size(500)]
        public string Remarks
        {
            get
            {
                try
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.Remarks;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                catch (Exception)
                {
                    return string.Empty;
                }
            }
        }

        [NonPersistent]
        public DateTime LastActivityDate
        {
            get
            {
                try
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.ActivityDate;
                    }
                    else
                    {
                        return DateTime.MinValue;
                    }
                }
                catch (Exception)
                {
                    return DateTime.MinValue;
                }
            }
        }
        [NonPersistent]
        public BatteryActivityTypeEnum LastActivityType
        {
            get
            {
                try
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data.ActivityType;
                    }
                    else
                    {
                        return BatteryActivityTypeEnum.None;
                    }
                }
                catch (Exception)
                {
                    return BatteryActivityTypeEnum.None;
                }
            }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public BatteryServiceDetail LastDetail
        {
            get
            {
                try
                {
                    var data = this.BatteryServiceDetails.OrderBy(o => o.Oid).Where(o => o.Oid > 0).LastOrDefault();
                    if (data != null)
                    {
                        return data;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        [Aggregated,
        Association("BatteryServiceDetails")]
        public XPCollection<BatteryServiceDetail> BatteryServiceDetails
        {
            get
            {
                return
                    GetCollection<BatteryServiceDetail>("BatteryServiceDetails");
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

        public Battery(Session session): base(session) {
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
            RowID = Guid.NewGuid();
            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }

            #endregion
        }

        protected override void OnSaving()
        {
            base.OnSaving();
   

            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
            }

            #endregion

        }
    }
}
