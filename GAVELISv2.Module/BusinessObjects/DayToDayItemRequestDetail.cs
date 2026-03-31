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
    public enum MovementTypeEnum
    {
        None,
        Issued,
        Receipt
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class DayToDayItemRequestDetail : XPObject
    {
        private DayToDayItemRequestHeader _ParentID;
        private GenJournalHeader _Source;
        private InventoryControlJournal _IcjID;
        private DateTime _EntryDate;
        private decimal _SourceQty;
        private decimal _IcjQty;
        private MovementTypeEnum _MovementType;
        private BusinessObjects.Item _Item;
        private decimal _SourceCost;
        private UnitOfMeasure _SourceUOM;
        private decimal _SourcePrice;
        private Warehouse _Whse;
        private decimal _IcjCost;
        private UnitOfMeasure _ICjUOM;
        private decimal _IcjPrice;
        private Requisition _Request;
        private Employee _RequestedBy;
        private DateTime _DateIssued;
        private CostCenter _IssuedTo;
        private decimal _AbsQty;
        [Custom("AllowEdit", "False")]
        [Association("DayToDayItemRequestDetails")]
        public DayToDayItemRequestHeader ParentID
        {
            get { return _ParentID; }
            set { SetPropertyValue("ParentID", ref _ParentID, value); }
        }
        // IcjID
        [Custom("AllowEdit", "False")]
        public InventoryControlJournal IcjID
        {
            get { return _IcjID; }
            set { SetPropertyValue("IcjID", ref _IcjID, value); }
        }
        // EntryDate
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // Source
        [Custom("AllowEdit", "False")]
        public GenJournalHeader Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }
        // SourceQty
        [Custom("AllowEdit", "False")]
        public decimal SourceQty
        {
            get { return _SourceQty; }
            set { SetPropertyValue("SourceQty", ref _SourceQty, value); }
        }
        // IcjQty
        [Custom("AllowEdit", "False")]
        public decimal IcjQty
        {
            get { return _IcjQty; }
            set { SetPropertyValue("IcjQty", ref _IcjQty, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AbsQty
        {
            get { return _AbsQty; }
            set { SetPropertyValue("AbsQty", ref _AbsQty, value); }
        }
        // ICJ Cost Total (AbsQty * IcjCost)
        [PersistentAlias("AbsQty * IcjCost")]
        [Custom("DisplayFormat", "n")]
        public decimal IcjCostTotal
        {
            get
            {
                object tempObject = EvaluateAlias("IcjCostTotal");
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
        // ICJ Price Total (AbsQty * IcjPrice)
        [PersistentAlias("AbsQty * IcjPrice")]
        [Custom("DisplayFormat", "n")]
        public decimal IcjPriceTotal
        {
            get
            {
                object tempObject = EvaluateAlias("IcjPriceTotal");
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
        // SRC Cost Total (SourceQty * SourceCost)
        [PersistentAlias("SourceQty * SourceCost")]
        [Custom("DisplayFormat", "n")]
        public decimal SrcCostTotal
        {
            get
            {
                object tempObject = EvaluateAlias("SrcCostTotal");
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
        // SRC Price Total (SourceQty * SourcePrice)
        [PersistentAlias("SourceQty * SourcePrice")]
        [Custom("DisplayFormat", "n")]
        public decimal SrcPriceTotal
        {
            get
            {
                object tempObject = EvaluateAlias("SrcPriceTotal");
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
        // MovementType
        [Custom("AllowEdit", "False")]
        public MovementTypeEnum MovementType
        {
            get { return _MovementType; }
            set { SetPropertyValue("MovementType", ref _MovementType, value); }
        }
        // Item
        [Custom("AllowEdit", "False")]
        public Item Item
        {
            get { return _Item; }
            set { SetPropertyValue("Item", ref _Item, value); }
        }
        // SourceCost
        [Custom("AllowEdit", "False")]
        public decimal SourceCost
        {
            get { return _SourceCost; }
            set { SetPropertyValue("SourceCost", ref _SourceCost, value); }
        }
        // SourceUOM
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure SourceUOM
        {
            get { return _SourceUOM; }
            set { SetPropertyValue("SourceUOM", ref _SourceUOM, value); }
        }
        // SourcePrice
        [Custom("AllowEdit", "False")]
        public decimal SourcePrice
        {
            get { return _SourcePrice; }
            set { SetPropertyValue("SourcePrice", ref _SourcePrice, value); }
        }
        // Whse
        [Custom("AllowEdit", "False")]
        public Warehouse Whse
        {
            get { return _Whse; }
            set { SetPropertyValue("Whse", ref _Whse, value); }
        }
        // IcjCost
        [Custom("AllowEdit", "False")]
        public decimal IcjCost
        {
            get { return _IcjCost; }
            set { SetPropertyValue("IcjCost", ref _IcjCost, value); }
        }
        // ICjUOM
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure ICjUOM
        {
            get { return _ICjUOM; }
            set { SetPropertyValue("ICjUOM", ref _ICjUOM, value); }
        }
        // IcjPrice
        [Custom("AllowEdit", "False")]
        public decimal IcjPrice
        {
            get { return _IcjPrice; }
            set { SetPropertyValue("IcjPrice", ref _IcjPrice, value); }
        }
        // Request
        [Custom("AllowEdit", "False")]
        public Requisition Request
        {
            get { return _Request; }
            set { SetPropertyValue("Request", ref _Request, value); }
        }
        // RequestedBy
        [Custom("AllowEdit", "False")]
        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set { SetPropertyValue("RequestedBy", ref _RequestedBy, value); }
        }
        // DateIssued
        [Custom("AllowEdit", "False")]
        public DateTime DateIssued
        {
            get { return _DateIssued; }
            set { SetPropertyValue("DateIssued", ref _DateIssued, value); }
        }
        public DateTime ExactDateIssued
        {
            get { return new DateTime(_DateIssued.Date.Year, _DateIssued.Date.Month, _DateIssued.Date.Day, 23, 0, 0); }
        }
        // IssuedTo
        [Custom("AllowEdit", "False")]
        public CostCenter IssuedTo
        {
            get { return _IssuedTo; }
            set { SetPropertyValue("IssuedTo", ref _IssuedTo, value); }
        }

        public string IssuedToCaption
        {
            get { return string.Format("ISSUE TO: {0}", _IssuedTo.Code); }
        }
        public DayToDayItemRequestDetail(Session session)
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
