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
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class CreditMemoDetail : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private Warehouse _Warehouse;
        private string _Reason;
        private decimal _Returning;
        private decimal _Returned;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private decimal _BaseQTY;
        private UnitOfMeasure _BaseUOM;
        private decimal _BasePrice;
        private decimal _Price;
        private decimal _LineDiscount;
        private SalesTaxCode _Tax;
        private decimal _Total;
        private InvoiceDetail _InvoiceDetailID;
        [NonPersistent]
        public CreditMemo CreditMemoInfo { get { return (CreditMemo)
                _GenJournalID; } }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-CreditMemoDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }
        [Custom("AllowEdit", "False")]
        public Item ItemNo {
            get { return _ItemNo; }
            set {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null) {
                    Warehouse = _ItemNo.WarehouseLocation;
                    Description = !string.IsNullOrEmpty(_ItemNo.SalesDescription
                    ) ? _ItemNo.SalesDescription : _ItemNo.Description;
                    Tax = _ItemNo.TaxCode != null ? _ItemNo.TaxCode : null;
                    UOM = _ItemNo.SellUOM != null ? _ItemNo.SellUOM : _ItemNo.
                    BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0) {foreach (UOMRelation 
                        item in _ItemNo.UOMRelations) {if (item.UOM == _UOM) {
                                Factor = item.Factor;}}}
                    Price = _ItemNo.SalesPrice;
                    BasePrice = _ItemNo.SalesPrice / _Factor;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Warehouse Warehouse {
            get { return _Warehouse; }
            set { SetPropertyValue("Warehouse", ref _Warehouse, value); }
        }
        public string Reason {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Returning {
            get { return _Returning; }
            set { SetPropertyValue("Returning", ref _Returning, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Returned {
            get { return _Returned; }
            set { SetPropertyValue("Returned", ref _Returned, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading) {
                    try {
                        ((CreditMemo)_GenJournalID).UpdateTotalTax(true);
                        ((CreditMemo)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set {
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null) {Factor = GetFactor();}
            }
        }
        [Custom("AllowEdit", "False")]
        public decimal Factor {
            get { return _Factor; }
            set {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading) {
                    try {
                        ((CreditMemo)_GenJournalID).UpdateTotalTax(true);
                        ((CreditMemo)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [PersistentAlias("Quantity * Factor")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseQTY {
            get {
                object tempObject = EvaluateAlias("BaseQTY");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set { SetPropertyValue("BaseUOM", ref _BaseUOM, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BasePrice {
            get { return _BasePrice; }
            set {
                SetPropertyValue("BasePrice", ref _BasePrice, value);
                if (!IsLoading) {
                    _Price = 0;
                    _Price = _BasePrice * _Factor;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Price {
            get { return _Price; }
            set {
                SetPropertyValue("Price", ref _Price, value);
                if (!IsLoading) {
                    _BasePrice = 0;
                    _BasePrice = _Price / _Factor;
                }
                if (!IsLoading) {
                    try {
                        ((CreditMemo)_GenJournalID).UpdateTotalTax(true);
                        ((CreditMemo)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal LineDiscount {
            get { return _LineDiscount; }
            set {
                SetPropertyValue("LineDiscount", ref _LineDiscount, value);
                if (!IsLoading) {
                    try {
                        ((CreditMemo)_GenJournalID).UpdateTotalTax(true);
                        ((CreditMemo)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public SalesTaxCode Tax {
            get { return _Tax; }
            set {
                SetPropertyValue("Tax", ref _Tax, value);
                if (!IsLoading) {
                    try {
                        ((CreditMemo)_GenJournalID).UpdateTotalTax(true);
                        ((CreditMemo)_GenJournalID).UpdateGrossTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [PersistentAlias("(Quantity * Price) - LineDiscount")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public InvoiceDetail InvoiceDetailID {
            get { return _InvoiceDetailID; }
            set { SetPropertyValue("InvoiceDetailID", ref _InvoiceDetailID, 
                value); }
        }
        public decimal GetFactor() {
            bool found = false;
            decimal res = 1;
            if (_ItemNo.UOMRelations.Count > 0) {
                var dBaseUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.BaseUOM2).FirstOrDefault();
                foreach (UOMRelation item in _ItemNo.UOMRelations) {
                    if (item.UOM == _UOM) {
                        found = true;
                        res = item.Factor;
                        break;
                    }
                }
                if (!found) { _UOM = dBaseUOM.UOM; }
            } else {
                _UOM = _ItemNo.BaseUOM;
            }
            //_Price = _BasePrice * res;
            return res;
        }
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
        [Aggregated,
        Association("CreditMemoDetail-CreditMemoDetailTrackingLines")]
        public XPCollection<CreditMemoDetailTrackingLine> 
        CreditMemoDetailTrackingLines { get { return GetCollection<
                CreditMemoDetailTrackingLine>("CreditMemoDetailTrackingLines"); 
            } }
        public CreditMemoDetail(Session session): base(session) {
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
            RowID = Guid.NewGuid();
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }
        private int deletedOid;
        protected override void OnDeleting()
        {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
            }
            base.OnDeleting();
        }
        protected override void OnSaving() {
            if (IsDeleted)
            {
                IncomeAndExpense02 incExp = null;
                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = {1}", deletedOid, this.Oid)));
                if (incExp != null)
                {
                    incExp.Delete();
                }
            }
            else
            {
                if (this.GenJournalID != null)
                {
                    this.GenJournalID.IsIncExpNeedUpdate = true;
                    this.GenJournalID.Save();
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
