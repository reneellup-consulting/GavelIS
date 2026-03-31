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
using DevExpress.Xpo.DB;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [OptimisticLocking(false)]
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class Item : BaseObject {
        private const string defaultDisplayFormat = "{No}->{Description}";
        private ItemTypeEnum _ItemType;
        private ItemClassEnum _ItemClass;
        private string _No;
        private string _Code;
        private string _OldCode;
        private string _Description;
        private ItemCategory _Category;
        private bool _Blocked = false;
        private UnitOfMeasure _BaseUOM;
        private string _UPC;
        private Account _InventoryAccount;
        private UnitOfMeasure _StockUOM;
        private string _PurchaseDescription;
        private decimal _Cost;
        private Account _COGSAccount;
        private UnitOfMeasure _PurchaseUOM;
        private Vendor _PreferredVendor;
        private string _SalesDescription;
        private decimal _SalesPrice;
        private SalesTaxCode _TaxCode;
        private Account _IncomeAccount;
        private UnitOfMeasure _SellUOM;
        private Warehouse _WarehouseLocation;
        private bool _RequireSerial;
        private bool _RequireExpiryDate;
        private decimal _SafetyStock = 0;
        private decimal _ReOrderPoint = 0;
        public ItemTypeEnum ItemType {
            get { return _ItemType; }
            set { SetPropertyValue("ItemType", ref _ItemType, value); }
        }

        public ItemClassEnum ItemClass {
            get { return _ItemClass; }
            set { SetPropertyValue("ItemClass", ref _ItemClass, value); }
        }
        public AccTireItemClassEnum TirePartType
        {
            get { return _TirePartType; }
            set { SetPropertyValue("TirePartType", ref _TirePartType, value); }
        }
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public virtual string No {
            get { return _No; }
            set {
                SetPropertyValue("No", ref _No, value);
                if (!IsLoading) {Code = _No;}
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string OldCode {
            get { return _OldCode; }
            set { SetPropertyValue("OldCode", ref _OldCode, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        //[RuleUniqueValue("", DefaultContexts.Save)]
        public virtual string Description {
            get { return _Description; }
            set {
                SetPropertyValue("Description", ref _Description, value);
                if (!IsLoading) {
                    PurchaseDescription = _Description;
                    SalesDescription = _Description;
                }
            }
        }

        public ItemCategory Category {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }

        public bool Blocked {
            get { return _Blocked; }
            set { SetPropertyValue("Blocked", ref _Blocked, value); }
        }

        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set {
                SetPropertyValue("BaseUOM", ref _BaseUOM, value);
                if (!IsLoading && _BaseUOM != null && UOMRelations.Count == 0) {
                    StockUOM = _BaseUOM;
                    PurchaseUOM = _BaseUOM;
                    SellUOM = _BaseUOM;
                }
                else if (!IsLoading && _BaseUOM != null && UOMRelations.Count > 0)
                {
                    // Unit of Measure Relations Problems
                }
            }
        }
        private UnitOfMeasure _BaseUOM2;

        [NonPersistent]
        public UnitOfMeasure BaseUOM2
        {
            get {
                if (UOMRelations.Count > 0)
                {
                    var data = UOMRelations.OrderBy(o => o.Factor).FirstOrDefault();
                    if (data != null)
                    {
                        _BaseUOM2 = data.UOM;
                    }
                    else
                    {
                        _BaseUOM2 = _BaseUOM;
                    }
                }
                return _BaseUOM2; }
        }

        [NonCloneable]
        [Aggregated,
        Association("Item-UOMRelations")]
        public XPCollection<UOMRelation> UOMRelations { get { return 
                GetCollection<UOMRelation>("UOMRelations"); } }

        public string UPC {
            get { return _UPC; }
            set { SetPropertyValue("UPC", ref _UPC, value); }
        }

        public Account InventoryAccount {
            get { return _InventoryAccount; }
            set { SetPropertyValue("InventoryAccount", ref _InventoryAccount, 
                value); }
        }

        public UnitOfMeasure StockUOM {
            get { return _StockUOM; }
            set { SetPropertyValue("StockUOM", ref _StockUOM, value); }
        }

        public string PurchaseDescription {
            get { return _PurchaseDescription; }
            set { SetPropertyValue("PurchaseDescription", ref 
                _PurchaseDescription, value); }
        }

        [Custom("DisplayFormat", "n")]
        public virtual decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }

        [Custom("DisplayFormat", "n")]
        //[Custom("AllowEdit", "False")]
        public virtual decimal RelCost
        {
            get
            {
                if (UOMRelations.Count > 0 && _PurchaseUOM != null)
                {
                    var data = UOMRelations.Where(o => o.UOM == _PurchaseUOM).FirstOrDefault();
                    _RelCost = data != null ? data.CostPerBaseUom : 0m;
                }
                return _RelCost;
            }
        }

        public Account COGSAccount {
            get { return _COGSAccount; }
            set { SetPropertyValue("COGSAccount", ref _COGSAccount, value); }
        }

        public UnitOfMeasure PurchaseUOM {
            get { return _PurchaseUOM; }
            set { SetPropertyValue("PurchaseUOM", ref _PurchaseUOM, value); }
        }

        public Vendor PreferredVendor {
            get { return _PreferredVendor; }
            set { SetPropertyValue("PreferredVendor", ref _PreferredVendor, 
                value); }
        }

        public string SalesDescription {
            get { return _SalesDescription; }
            set { SetPropertyValue("SalesDescription", ref _SalesDescription, 
                value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal SalesPrice {
            get { return _SalesPrice; }
            set { SetPropertyValue("SalesPrice", ref _SalesPrice, value); }
        }

        [Custom("AllowEdit", "False")]
        public decimal RelSalesPrice
        {
            get
            {
                if (UOMRelations.Count > 0 && _SellUOM != null)
                {
                    var data = UOMRelations.Where(o => o.UOM == _SellUOM).FirstOrDefault();
                    _RelSalesPrice = data != null ? data.PricePerBaseUom : 0m;
                }
                return _RelSalesPrice;
            }
        }

        public SalesTaxCode TaxCode {
            get { return _TaxCode; }
            set { SetPropertyValue("TaxCode", ref _TaxCode, value); }
        }

        public Account IncomeAccount {
            get { return _IncomeAccount; }
            set { SetPropertyValue("IncomeAccount", ref _IncomeAccount, value); 
            }
        }

        public UnitOfMeasure SellUOM {
            get { return _SellUOM; }
            set { SetPropertyValue("SellUOM", ref _SellUOM, value); }
        }

        public Warehouse WarehouseLocation {
            get { return _WarehouseLocation; }
            set { SetPropertyValue("WarehouseLocation", ref _WarehouseLocation, 
                value); }
        }

        public bool RequireSerial {
            get { return _RequireSerial; }
            set { SetPropertyValue("RequireSerial", ref _RequireSerial, value); 
            }
        }

        public bool RequireExpiryDate {
            get { return _RequireExpiryDate; }
            set { SetPropertyValue("RequireExpiryDate", ref _RequireExpiryDate, 
                value); }
        }

        public decimal SafetyStock {
            get { return _SafetyStock; }
            set { SetPropertyValue("SafetyStock", ref _SafetyStock, value); }
        }

        public decimal ReOrderPoint {
            get { return _ReOrderPoint; }
            set { SetPropertyValue("ReOrderPoint", ref _ReOrderPoint, value); }
        }

        [Association("Item-InventoryControlJournals")]
        public XPCollection<InventoryControlJournal> InventoryControlJournals { 
            get { return GetCollection<InventoryControlJournal>(
                "InventoryControlJournals"); } }
        [Association("Item-JobHistory")]
        public XPCollection<JobOrderDetail> JobHistory
        {
            get
            {
                return GetCollection<JobOrderDetail>(
                    "JobHistory");
            }
        }
        #region Calculated Details
        [Persistent("QtyOnHand")]
        private decimal? _QtyOnHand = null;
        [PersistentAlias("_QtyOnHand")]
        [Custom("DisplayFormat", "n")]
        public decimal? QtyOnHand {
            get {
                try {
                    if (!IsLoading && !IsSaving && _QtyOnHand == null) {
                        UpdateQtyOnHand(false);}
                } catch(Exception) {
                }
                return _QtyOnHand;
            }
        }

        public void UpdateQtyOnHand(bool forceChangeEvent) {
            decimal? oldQtyOnHand = _QtyOnHand;
            decimal tempTotal = 0m;
            //foreach (InventoryControlJournal detail in InventoryControlJournals) 
            //{if (detail.GenJournalID.SourceType.Code != "PO") {tempTotal += 
            //        detail.Qty;}}
            //var data = from invj in InventoryControlJournals
            //           where invj.GenJournalID.SourceType.Code != "PO"
            //           select invj;
            //tempTotal = data.Select(c => c.Qty).Sum();
            try
            {
                //var data = this.InventoryControlJournals.OrderBy(o => o.Sequence).Where(o => o.GenJournalID.SourceType.Code != "PO").LastOrDefault();
                //if (data != null)
                //{
                //    tempTotal = data.RunningQtyAll.Value;
                //}
                //var data = from invj in InventoryControlJournals
                //           where invj.GenJournalID.SourceType.Code != "PO"
                //           select invj;
                //tempTotal = data.Select(c => c.Qty).Sum();
                foreach (var item in InventoryControlJournals)
                {
                    if (item.GenJournalID!=null && item.GenJournalID.SourceType.Code != "PO")
                    {
                        tempTotal += item.Qty;
                    }
                }
                _QtyOnHand = tempTotal;
                if (forceChangeEvent)
                {
                    OnChanged("QtyOnHand", QtyOnHand, _QtyOnHand)
                        ;
                }
                ;
            }
            catch (Exception)
            {
                _QtyOnHand = tempTotal;
            }
        }

        #endregion

        #region Public Methods
        //[Action(AutoCommit = true, Caption = "Revalidate On Hand")]
        //public void RevalidateOnHand()
        //{
        //    this.ResetOnHand = true;
        //    this.ResetOnHand = false;
        //}
        public void ResetOnHand()
        {
            _QtyOnHand = null;
        }
        public decimal GetWarehouseQtyBase(Warehouse warehouse) {
            decimal qty = 0;
            //foreach (InventoryControlJournal item in InventoryControlJournals) {
            //    if (warehouse == item.Warehouse && item.GenJournalID.SourceType.
            //    Code != "PO") {qty += item.Qty;}}
            var data = from invj in InventoryControlJournals
                       where invj.Warehouse == warehouse && invj.GenJournalID.SourceType.Code != "PO"
                       select invj;
            qty = data.Select(c => c.Qty).Sum();
            return qty;
        }

        public decimal GetWarehouseQtyBase(Warehouse warehouse, DateTime entryDate)
        {
            //foreach (InventoryControlJournal item in InventoryControlJournals) {
            //    if (warehouse == item.Warehouse && item.GenJournalID.SourceType.
            //    Code != "PO") {qty += item.Qty;}}
            DateTime edate = new DateTime(entryDate.Date.Year, entryDate.Date.Month, entryDate.Date.Day, entryDate.Date.Hour, entryDate.Date.Minute, entryDate.Date.Second);
            var data = InventoryControlJournals.OrderBy(o => o.Sequence).Where(o => o.GenJournalID.SourceType.Code != "PO" && o.Warehouse == warehouse && o.EntryDate < edate).Select(o => o);
            if (data != null)
            {
                return data.Select(o => o.Qty).Sum();
            }
            else
            {
                return 0m;
            }
        }

        public decimal GetWarehouseQtyBaseSimplified(Warehouse warehouse, DateTime entryDate)
        {
            string qry = string.Format("select ItemID, WhseId, Uomid, sum(Qty) as Quantity from vInvItemWhseQty " +
                "where ItemID='{0}' and Whseid='{1}' " +
                "and EntryDate < '{2}' " +
                "group by ItemID, WhseId, Uomid", this.Oid, warehouse.Oid, entryDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            SelectedData iiwq = Session.ExecuteQuery(qry);

            if (iiwq != null && iiwq.ResultSet[0].Rows.Count() > 0)
            {
                decimal sum = 0m;
                for (int i = 0; i < iiwq.ResultSet[0].Rows.Count(); i++)
                {
                    SelectStatementResultRow row = iiwq.ResultSet[0].Rows[i];
                    Guid u_id = Guid.Parse(row.Values[2].ToString());
                    UnitOfMeasure r_uom = Session.GetObjectByKey<UnitOfMeasure>(u_id);
                    UnitOfMeasure base_uom = UOMRelations.Count > 0 ? this._BaseUOM2 : this._BaseUOM;
                    if (base_uom == r_uom)
                    {
                        sum += Convert.ToDecimal(row.Values[3].ToString());
                    }
                    else
                    {
                        if (UOMRelations.Count > 0)
                        {
                            var uom_rel = UOMRelations.Where(o => o.UOM == r_uom).FirstOrDefault();
                            if (uom_rel != null)
                            {
                                sum += Convert.ToDecimal(row.Values[3].ToString()) * uom_rel.Factor;
                            }
                            else
                            {
                                throw new UserFriendlyException(string.Format("UOM relationship not found in item #{0}", _No));
                            }
                        }
                        else
                        {
                            sum += Convert.ToDecimal(row.Values[3].ToString());
                        }
                    }
                }

                return sum;
            }

            //if (whse != null && whse.ResultSet[0].Rows.Count() > 0)
            //{
            //    SelectStatementResultRow row = whse.ResultSet[0].Rows[0];
            //    //Guid uomid = Guid.Parse(row.Values[2].ToString());
            //    //UnitOfMeasure uom = Session.GetObjectByKey<UnitOfMeasure>(uomid);
            //    //if (this._StockUOM !=null && uom == this._StockUOM)
            //    //{
            //    //    return Convert.ToDecimal(row.Values[3].ToString());
            //    //}
            //    //else if (this._StockUOM !=null && uom != this._StockUOM)
            //    //{
            //    //    var dStockUOM = UOMRelations.Where(o => o.UOM == uom).FirstOrDefault();
            //    //}
            //    //var dStockUOM = UOMRelations.Where(o => o.UOM == uom).FirstOrDefault();
            //    //if (dStockUOM != null)
            //    //{
                    
            //    //    return data.Select(o => o.Qty).Sum() * dStockUOM.Factor;
            //    //}
            //    //else
            //    //{
            //    //    return data.Select(o => o.Qty).Sum();
            //    //}
            //    return Convert.ToDecimal(row.Values[3].ToString());
            //}

            return 0m;
        }

        public decimal GetWarehouseQtyBaseCorrected(Warehouse warehouse, int id, DateTime entryDate, string sourceTypeCode, Requisition requisition = null)
        {
            int srcsq = 0;
            int rlstt = 0;
            if (requisition != null)
            {
                rlstt = requisition.Oid % 1000;
            }
            if (sourceTypeCode == "PO")
            {
                srcsq = 1;
            }
            else if (sourceTypeCode == "RC")
            {
                srcsq = 2;
            }
            else if (sourceTypeCode == "DM")
            {
                srcsq = 3;
            }
            else if (sourceTypeCode == "IN")
            {
                srcsq = 4;
            }
            else if (sourceTypeCode == "CM")
            {
                srcsq = 5;
            }
            else if (sourceTypeCode == "TO")
            {
                srcsq = 6;
            }
            else if (sourceTypeCode == "WO")
            {
                srcsq = 7;
            }
            else if (sourceTypeCode == "ECS")
            {
                srcsq = 8;
            }
            else
            {
                srcsq = 0;
            }
            string seq = entryDate != DateTime.MinValue ?
                       string.Format("1{0}{1:00}{2:00}{3:000}{4:0}{5:00}{6:00}{7:00}{8:0000000000}", entryDate.Year, entryDate.Month,
                       entryDate.Day, rlstt, srcsq, entryDate.Hour, entryDate.Minute, entryDate.Second, id != -1 ? id : 0)
                       : string.Empty;
            var data = InventoryControlJournals.OrderBy(o => o.Sequence).Where(o => o.GenJournalID != null && o.GenJournalID.SourceType.Code != "PO" && o.Warehouse == warehouse && Convert.ToDecimal(o.Sequence) < Convert.ToDecimal(seq)).Select(o => o);
            if (data != null)
            {
                var dStockUOM = UOMRelations.Where(o => o.UOM == _StockUOM).FirstOrDefault();
                if (dStockUOM != null)
                {
                    return data.Select(o => o.Qty).Sum() * dStockUOM.Factor;
                }
                else
                {
                    return data.Select(o => o.Qty).Sum();
                }
            }
            else
            {
                return 0m;
            }
        }
        public decimal GetOverallQtyBase() {
            decimal qty = 0;
            //foreach (InventoryControlJournal item in InventoryControlJournals) {
            //    if (item.GenJournalID.SourceType.Code != "PO") {qty += item.Qty;
            //    }}
            var data = from invj in InventoryControlJournals
                       where invj.GenJournalID.SourceType.Code != "PO"
                       select invj;
            qty = data.Select(c => c.Qty).Sum();
            return qty;
        }

        public decimal GetOverallQtyBase(DateTime entryDate)
        {
            DateTime edate = new DateTime(entryDate.Date.Year, entryDate.Date.Month, entryDate.Date.Day, entryDate.Date.Hour, entryDate.Date.Minute, entryDate.Date.Second);
            var data = Session.ExecuteSproc("GetOverallQtyBase", this.Oid.ToString(), edate.ToString());
            if (data != null)
            {
                return data.ResultSet[0].Rows.Count() > 0 ? Convert.ToDecimal(data.ResultSet[0].Rows[0].Values[0]) : 0m;
            }
            else
            {
                return 0m;
            }
            //var data = InventoryControlJournals.OrderBy(o => o.Sequence).Where(o => o.GenJournalID.SourceType.Code != "PO" && o.EntryDate < edate).Select(o => o);
            //if (data != null)
            //{
            //    return data.Select(o => o.Qty).Sum();
            //}
            //else
            //{
            //    return 0m;
            //}
        }
        public decimal GetCost(CostingMethodEnum costing) {
            decimal res = 0;
            //switch (costing)
            //{
            //    case CostingMethodEnum.Standard:
            //        res=_Cost;
            //        break;
            //    case CostingMethodEnum.Average:
            //        if (InventoryControlJournals.Count>0)
            //        {
            //            decimal inQty = 0;
            //            decimal totalCost = 0;
            //            foreach (InventoryControlJournal item in InventoryControlJournals)
            //            {
            //                inQty+=item.Qty;
            //            }
            //        }
            //        break;
            //    case CostingMethodEnum.FIFO:
            //        break;
            //    case CostingMethodEnum.LIFO:
            //        break;
            //    default:
            //        break;
            //}
            return res;
        }

        public decimal GetInvoiceCost(string rowID)
        {
            if (InventoryControlJournals.Count > 0)
            {
                //foreach (InventoryControlJournal item in 
                //InventoryControlJournals) {if (item.RowID == rowID) {return item
                //        .Cost;}}
                var data = (from invj in InventoryControlJournals
                            where invj.RowID == rowID
                            select invj).FirstOrDefault();
                if (data != null)
                {
                    var data1 = (from invj in InventoryControlJournals
                                 where invj.EntryDate < data.EntryDate && invj.SourceTypeCode == "RC"
                                 select invj).LastOrDefault();
                    if (data1 != null)
                    {
                        _Cost = data1.Cost;
                    }
                }
                return _Cost;
            }
            else
            {
                return _Cost;
            }
        }

        public decimal GetInvoicePrice(string rowID)
        {
            if (InventoryControlJournals.Count > 0)
            {
                //foreach (InventoryControlJournal item in
                //InventoryControlJournals)
                //{
                //    if (item.RowID == rowID)
                //    {
                //        return item
                //            .Price;
                //    }
                //}
                var data = (from invj in InventoryControlJournals
                            where invj.RowID == rowID
                            select invj).FirstOrDefault();
                if (data != null)
                {
                    _SalesPrice = data.Price;
                }
                return _SalesPrice;
            }
            else
            {
                return _SalesPrice;
            }
        }

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

        #region Display String
        public string DisplayName { get { return ObjectFormatter.Format(
                defaultDisplayFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); } }

        #endregion

        public Item(Session session): base(session) {
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;

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
            if (UOMRelations.Count > 0)
            {
                //if (_BaseUOM != null)
                //{
                //    var data = UOMRelations.Where(o => o.UOM == _BaseUOM).FirstOrDefault();
                //    if (data == null)
                //    {
                //        throw new UserFriendlyException("Base UOM does not exist in the list of relationships!");
                //    }
                //}
                if (_PurchaseUOM != null)
                {
                    var data = UOMRelations.Where(o => o.UOM == _PurchaseUOM).FirstOrDefault();
                    if (data == null)
                    {
                        throw new UserFriendlyException(string.Format("Purchase UOM does not exist in the list of relationships of item {0}!", this._No));
                    }
                }
                if (_SellUOM != null)
                {
                    var data = UOMRelations.Where(o => o.UOM == _SellUOM).FirstOrDefault();
                    if (data == null)
                    {
                        throw new UserFriendlyException(string.Format("Purchase UOM does not exist in the list of relationships of item {0}!", this._No));
                    }
                }
                if (_StockUOM != null)
                {
                    var data = UOMRelations.Where(o => o.UOM == _StockUOM).FirstOrDefault();
                    if (data == null)
                    {
                        throw new UserFriendlyException(string.Format("Purchase UOM does not exist in the list of relationships of item {0}!", this._No));
                    }
                }
            }
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

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _QtyOnHand = null; 
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private AccTireItemClassEnum _TirePartType;
        private bool _ResetOnHand;
        private decimal _RelCost;
        private decimal _RelSalesPrice;
        //private AccTireItemClassEnum _TireItemType;
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
