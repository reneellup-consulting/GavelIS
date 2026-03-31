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
    public class InventoryControlJournal : XPObject
    {
        private GenJournalHeader _GenJournalID;
        private DateTime _DateIssued;
        private string _Reference;
        private decimal _InQTY; // IN
        private decimal _OutQty; // OUT
        private Warehouse _Warehouse;
        private Item _ItemNo;
        private decimal _Cost; // PRICE
        private decimal _Price;
        private UnitOfMeasure _UOM; // UNIT
        private decimal _Total;
        private Requisition _RequisitionNo;
        private CostCenter _CostCenter;
        private Employee _RequestedBy;
        private string _RowID;
        [NonPersistent]
        public string Reference
        {
            get
            {
                if (_GenJournalID != null)
                {
                    if (_GenJournalID.SourceType.Code == "PO")
                    {
                        _Reference = ((PurchaseOrder)_GenJournalID).Vendor.Name;
                    }
                    if (_GenJournalID.SourceType.Code == "RC")
                    {
                        _Reference = ((Receipt)_GenJournalID).Vendor.Name;
                    }
                    if (_GenJournalID.SourceType.Code == "PI")
                    {
                        _Reference = "Adjustment";
                    }
                    if (_GenJournalID.SourceType.Code == "DM")
                    {
                        _Reference = ((DebitMemo)_GenJournalID).Vendor.Name;
                    }
                    if (_GenJournalID.SourceType.Code == "IN")
                    {
                        _Reference = ((Invoice)_GenJournalID).Customer.Name;
                    }
                    if (_GenJournalID.SourceType.Code == "CM")
                    {
                        _Reference = ((CreditMemo)_GenJournalID).Customer.Name;
                    }
                    if (_GenJournalID.SourceType.Code == "TO")
                    {
                        _Reference = "Transfer";
                    }
                    if (_GenJournalID.SourceType.Code == "WO")
                    {
                        _Reference = "Work Order";
                    }
                }
                return _Reference;
            }
        }

        [NonPersistent]
        public DateTime Date
        {
            get { return _GenJournalID.ExactEntryDate; }
        }

        [NonPersistent]
        public DateTime EntryDate
        {
            get
            {
                if (_GenJournalID != null)
                {
                    return _GenJournalID.EntryDate;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }
        //[NonPersistent]
        //public int PrevOid
        //{
        //    get { return _PrevOid; }
        //    set { SetPropertyValue<int>("PrevOid", ref _PrevOid, value); }
        //}
        //[NonPersistent]
        //public decimal RunningQtyAll
        //{
        //    get
        //    {
        //        //var data = ItemNo.InventoryControlJournals.OrderBy(o => o.Sequence).Where(o => o.Warehouse == Warehouse && o.Sequence < Sequence && o.Oid != Oid).LastOrDefault();
        //        var data = ItemNo.InventoryControlJournals.OrderBy(o => Convert.ToDecimal(o.Sequence)).Where(o => o.SourceTypeCode != "PO" && Convert.ToDecimal(o.Sequence) < Convert.ToDecimal(Sequence) && o.Oid != Oid).LastOrDefault();
        //        if (data != null)
        //        {
        //            //PrevOid = data.Oid;
        //            //Console.WriteLine(data.Oid.ToString());
        //            return data.RunningQtyAll + Qty;
        //        }
        //        else
        //        {
        //            return Qty;
        //        }
        //    }
        //}
        [Persistent("RunningQtyAll")]
        private decimal? _RunningQtyAll = null;
        [PersistentAlias("_RunningQtyAll")]
        public decimal? RunningQtyAll
        {
            get
            {
                if (!IsLoading && !IsSaving && _RunningQtyAll == null)
                    UpdateRunningQtyAll(false);
                return _RunningQtyAll;
            }
        }
        public void UpdateRunningQtyAll(bool forceChangeEvents)
        {
            decimal? oldRunningQtyAll = _RunningQtyAll;
            decimal tempTotal = 0m;
            if (_ItemNo != null)
            {
                var data = ItemNo.InventoryControlJournals.OrderBy(o => Convert.ToDecimal(o.Sequence)).Where(o => o.SourceTypeCode != "PO" && Convert.ToDecimal(o.Sequence) < Convert.ToDecimal(Sequence) && o.Oid != Oid).LastOrDefault();
                if (data != null)
                {
                    tempTotal = data.RunningQtyAll.Value + Qty;
                }
                else
                {
                    tempTotal = Qty;
                }
            }

            _RunningQtyAll = tempTotal;
            if (forceChangeEvents)
                OnChanged("RunningQtyAll", oldRunningQtyAll, _RunningQtyAll);
        }
        [NonPersistent]
        //[Custom("DisplayFormat", "d")]
        public string Sequence
        {
            get
            {
                int srcsq = 0;
                int rlstt = 0;
                if (RequisitionNo != null)
                {
                    rlstt = RequisitionNo.Oid % 1000;
                }
                if (SourceTypeCode == "PO")
                {
                    srcsq = 1;
                }
                else if (SourceTypeCode == "RC")
                {
                    srcsq = 2;
                }
                else if (SourceTypeCode == "DM")
                {
                    srcsq = 3;
                }
                else if (SourceTypeCode == "IN")
                {
                    srcsq = 4;
                }
                else if (SourceTypeCode == "CM")
                {
                    srcsq = 5;
                }
                else if (SourceTypeCode == "TO")
                {
                    srcsq = 6;
                }
                else if (SourceTypeCode == "WO")
                {
                    srcsq = 7;
                }
                else if (SourceTypeCode == "ECS")
                {
                    srcsq = 8;
                }
                else
                {
                    srcsq = 0;
                }
                string seq = EntryDate != DateTime.MinValue ?
                       string.Format("1{0}{1:00}{2:00}{3:000}{4:0}{5:00}{6:00}{7:00}{8:0000000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, rlstt, srcsq, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, Oid != -1 ? Oid : 0)
                       : string.Empty;
                return seq;
            }
        }
        //[NonPersistent]
        //public decimal RunningQtyWhse
        //{
        //    get
        //    {
        //        //var data = ItemNo.InventoryControlJournals.OrderBy(o => o.EntryDate).ThenBy(o => o.CreatedOn).ThenBy(o => o.Oid).Where(o => o.Warehouse == Warehouse && o.EntryDate < EntryDate && o.Oid != Oid).LastOrDefault();
        //        var data = ItemNo.InventoryControlJournals.OrderBy(o => Convert.ToDecimal(o.Sequence)).Where(o => o.SourceTypeCode != "PO" && o.Warehouse == Warehouse && Convert.ToDecimal(o.Sequence) < Convert.ToDecimal(Sequence) && o.Oid != Oid).LastOrDefault();
        //        if (data != null)
        //        {
        //            //PrevOidWhsw = data.Oid;
        //            //Console.WriteLine(data.Oid.ToString());
        //            return data.RunningQtyWhse + Qty;
        //        }
        //        else
        //        {
        //            return Qty;
        //        }
        //    }
        //}
        [Persistent("RunningQtyWhse")]
        private decimal? _RunningQtyWhse = null;
        [PersistentAlias("_RunningQtyWhse")]
        public decimal? RunningQtyWhse
        {
            get
            {
                if (!IsLoading && !IsSaving && _RunningQtyWhse == null)
                    UpdateRunningQtyWhse(false);
                return _RunningQtyWhse;
            }
        }
        public void UpdateRunningQtyWhse(bool forceChangeEvents)
        {
            decimal? oldRunningQtyWhse = _RunningQtyWhse;
            decimal tempTotal = 0m;
            if (_ItemNo != null)
            {
                var data = ItemNo.InventoryControlJournals.OrderBy(o => Convert.ToDecimal(o.Sequence)).Where(o => o.SourceTypeCode != "PO" && o.Warehouse == Warehouse && Convert.ToDecimal(o.Sequence) < Convert.ToDecimal(Sequence) && o.Oid != Oid).LastOrDefault();
                if (data != null)
                {
                    tempTotal = data.RunningQtyWhse.Value + Qty;
                }
                else
                {
                    tempTotal = Qty;
                }
            }
            _RunningQtyWhse = tempTotal;
            if (forceChangeEvents)
                OnChanged("RunningQtyWhse", oldRunningQtyWhse, _RunningQtyWhse);
        }
        public DateTime DateIssued
        {
            get { return _DateIssued; }
            set { SetPropertyValue<DateTime>("DateIssued", ref _DateIssued, value); }
        }

        [Association("GenJournalHeader-InventoryControlJournals")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader GenJournalID
        {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }

        public decimal InQTY
        {
            get { return _InQTY; }
            set
            {
                bool modified = SetPropertyValue("InQTY", ref _InQTY, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    //this.UpdateRunningQtyAll(true);
                    //this.UpdateRunningQtyWhse(true);
                }
                if (!IsLoading && !IsSaving && _ItemNo != null && modified)
                {
                    //_ItemNo.UpdateQtyOnHand(true);
                }
            }
        }

        public decimal OutQty
        {
            get { return _OutQty; }
            set
            {
                bool modified = SetPropertyValue("OutQty", ref _OutQty, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    //this.UpdateRunningQtyAll(true);
                    //this.UpdateRunningQtyWhse(true);
                }
                if (!IsLoading && !IsSaving && _ItemNo != null && modified)
                {
                    //_ItemNo.UpdateQtyOnHand(true);
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Warehouse Warehouse
        {
            get { return _Warehouse; }
            set
            {
                bool modified = SetPropertyValue("Warehouse", ref _Warehouse, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    this.UpdateRunningQtyWhse(true);
                }
            }
        }

        [PersistentAlias("InQTY - OutQty")]
        public decimal Qty
        {
            get
            {
                var tempObject = EvaluateAlias("Qty");
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

        public decimal StockQty
        {
            get
            {
                if (_ItemNo.UOMRelations.Count > 0)
                {
                    UnitOfMeasure base_uom = _ItemNo.UOMRelations.Count > 0 ? _ItemNo.BaseUOM2 : _ItemNo.BaseUOM;
                    UnitOfMeasure stock_uom = _ItemNo.StockUOM != null ? _ItemNo.StockUOM : base_uom;
                    var uom_rel_base = _ItemNo.UOMRelations.Where(o => o.UOM == base_uom).FirstOrDefault();
                    var uom_rel_stock = _ItemNo.UOMRelations.Where(o => o.UOM == stock_uom).FirstOrDefault();
                    var uom_rel = _ItemNo.UOMRelations.Where(o => o.UOM == _UOM).FirstOrDefault();
                    if (uom_rel_base.Factor == uom_rel_stock.Factor)
                    {
                        return Qty;
                    }
                    else
                    {
                        if (_UOM == stock_uom)
                        {
                            return Qty;
                        }
                        else
                        {
                            decimal baseQty = Qty * uom_rel.Factor;
                            return baseQty / uom_rel_stock.Factor;
                        }
                    }
                }
                else
                {
                    return Qty;
                }
            }
        }

        [PersistentAlias("Qty * ItemNo.Cost")]
        [DisplayName("Current Value")]
        public decimal ValueCost
        {
            get
            {
                var tempObject = EvaluateAlias("ValueCost");
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

        public decimal StockValue
        {
            get
            {
                if (_ItemNo.UOMRelations.Count > 0)
                {
                    UnitOfMeasure base_uom = _ItemNo.UOMRelations.Count > 0 ? _ItemNo.BaseUOM2 : _ItemNo.BaseUOM;
                    UnitOfMeasure stock_uom = _ItemNo.StockUOM != null ? _ItemNo.StockUOM : base_uom;
                    var uom_rel_stock = _ItemNo.UOMRelations.Where(o => o.UOM == stock_uom).FirstOrDefault();
                    return StockQty * uom_rel_stock.CostPerBaseUom;
                }
                else
                {
                    return ValueCost;
                }
            }
        }

        [Association("Item-InventoryControlJournals")]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set
            {
                Item oldItem = _ItemNo;
                bool modified = SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && !IsSaving && oldItem != _ItemNo && modified)
                {
                    oldItem = oldItem ?? _ItemNo;
                    oldItem.UpdateQtyOnHand(true);
                }
            }
        }
        [NonPersistent]
        public string CategoryCode
        {
            get
            {
                if (_ItemNo != null && _ItemNo.Category != null)
                {
                    return _ItemNo.Category.Code;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        [NonPersistent]
        public string SourceTypeCode
        {
            get
            {
                if (_GenJournalID != null && _GenJournalID.SourceType != null)
                {
                    return _GenJournalID.SourceType.Code;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Price
        {
            get { return _Price; }
            set { SetPropertyValue<decimal>("Price", ref _Price, value); }
        }

        public UnitOfMeasure UOM
        {
            get { return _UOM; }
            set { SetPropertyValue("UOM", ref _UOM, value); }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                if (_Price > 0)
                {
                    _Total = Math.Abs(Qty) * _Price;
                }
                else
                    _Total = Math.Abs(Qty) * _Cost;
                return _Total;
            }
        }

        public Requisition RequisitionNo
        {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter
        {
            get { return _CostCenter; }
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value); }
        }

        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set { SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        //[RuleUniqueValue("", DefaultContexts.Save)]
        public string RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
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

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month
        {
            get
            {
                _Month = _GenJournalID.EntryDate.Month == 1 ? MonthsEnum.January : _GenJournalID.EntryDate.Month
                 == 2 ? MonthsEnum.February : _GenJournalID.EntryDate.Month == 3 ? MonthsEnum.
                March : _GenJournalID.EntryDate.Month == 4 ? MonthsEnum.April : _GenJournalID.EntryDate.Month ==
                5 ? MonthsEnum.May : _GenJournalID.EntryDate.Month == 6 ? MonthsEnum.June :
                _GenJournalID.EntryDate.Month == 7 ? MonthsEnum.July : _GenJournalID.EntryDate.Month == 8 ?
                MonthsEnum.August : _GenJournalID.EntryDate.Month == 9 ? MonthsEnum.September
                 : _GenJournalID.EntryDate.Month == 10 ? MonthsEnum.October : _GenJournalID.EntryDate.Month == 11
                 ? MonthsEnum.November : _GenJournalID.EntryDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string Quarter
        {
            get
            {
                switch (Month)
                {
                    case MonthsEnum.None:
                        break;
                    case MonthsEnum.January:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.February:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.March:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.April:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.May:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.June:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.July:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.August:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.September:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.October:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.November:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.December:
                        _Quarter = "4th QTR";
                        break;
                    default:
                        break;
                }
                return _Quarter;
            }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get
            {
                return _GenJournalID.EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string MonthSorter
        {
            get
            {
                switch (Month)
                {
                    case MonthsEnum.None:
                        return "00 NONE";
                    case MonthsEnum.January:
                        return "01 JANUARY";
                    case MonthsEnum.February:
                        return "02 FEBRUARY";
                    case MonthsEnum.March:
                        return "03 MARCH";
                    case MonthsEnum.April:
                        return "04 APRIL";
                    case MonthsEnum.May:
                        return "05 MAY";
                    case MonthsEnum.June:
                        return "06 JUNE";
                    case MonthsEnum.July:
                        return "07 JULY";
                    case MonthsEnum.August:
                        return "08 AUGUST";
                    case MonthsEnum.September:
                        return "09 SEPTEMBER";
                    case MonthsEnum.October:
                        return "10 OCTOBER";
                    case MonthsEnum.November:
                        return "11 NOVEMBER";
                    case MonthsEnum.December:
                        return "12 DECEMBER";
                    default:
                        return "00 NONE";
                }
            }
        }

        #endregion


        public InventoryControlJournal(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;

            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
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
                var currentUser = Session.GetObjectByKey<SecurityUser>(Session.
                GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }

            #endregion

        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private int _PrevOid;
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
        //[NonPersistent]
        //public int PrevOidWhsw { get; set; }
        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }
        private void Reset()
        {
            _RunningQtyAll = null;
            _RunningQtyWhse = null;
        }
    }
}
