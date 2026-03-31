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
    public class DebitMemoDetail : XPObject, IToMovementCapable, ISetIncomeExpense
    {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private Warehouse _Warehouse;
        private string _Reason;
        private decimal _Returning;
        private decimal _Returned;
        private decimal _Quantity = 1;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        //private decimal _BaseQTY;
        private decimal _BaseCost;
        private UnitOfMeasure _UOM;
        private decimal _Cost;
        //private decimal _Total;
        private ReceiptDetail _ReceiptDetailID;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("GenJournalHeader-DebitMemoDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }
        [NonPersistent]
        public DebitMemo DebitMemoInfo { get { return (DebitMemo)_GenJournalID; 
            } }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo {
            get { return _ItemNo; }
            set {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null) {
                    Warehouse = _ItemNo.WarehouseLocation != null ? _ItemNo.
                    WarehouseLocation : null;
                    Description = !string.IsNullOrEmpty(_ItemNo.
                    PurchaseDescription) ? _ItemNo.PurchaseDescription : _ItemNo
                    .Description;
                    UOM = _ItemNo.PurchaseUOM != null ? _ItemNo.PurchaseUOM : 
                    _ItemNo.BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    BaseCost = _ItemNo.Cost;
                    Cost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        var data = _ItemNo.UOMRelations.OrderBy(o => o.Factor).FirstOrDefault();
                        if (data != null)
                        {
                            BaseCost = data.CostPerBaseUom;
                        }
                        foreach (UOMRelation item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _UOM)
                            {
                                Factor = item.Factor;
                                Cost = item.CostPerBaseUom;
                            }
                        }
                    }
                }
            }
        }
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
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
            set { SetPropertyValue("Quantity", ref _Quantity, value);
            if (!IsLoading)
            {
                try
                {
                    ((DebitMemo)_GenJournalID).UpdateTotal(true);
                }
                catch (Exception) { }
            }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading) {
                    try {
                        ((DebitMemo)_GenJournalID).UpdateTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public UnitOfMeasure BaseUOM {
            get { return _BaseUOM; }
            set { SetPropertyValue("BaseUOM", ref _BaseUOM, value); }
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
        [Custom("DisplayFormat", "n")]
        public decimal BaseCost {
            get { return _BaseCost; }
            set {
                SetPropertyValue("BaseCost", ref _BaseCost, value);
                //if (!IsLoading) {
                //    _Cost = 0;
                //    _Cost = _BaseCost * _Factor;
                //}
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set {
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null) {Factor = GetFactor();}
                if (!IsLoading) {
                    try {
                        ((DebitMemo)_GenJournalID).UpdateTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set {
                SetPropertyValue("Cost", ref _Cost, value);
                //if (!IsLoading) {
                //    _BaseCost = 0;
                //    _BaseCost = _Cost / _Factor;
                //}
                if (!IsLoading) {
                    try {
                        ((DebitMemo)_GenJournalID).UpdateTotal(true);
                    } catch (Exception) {}
                }
            }
        }
        [PersistentAlias("Quantity * Cost")]
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
        public ReceiptDetail ReceiptDetailID {
            get { return _ReceiptDetailID; }
            set { SetPropertyValue("ReceiptDetailID", ref _ReceiptDetailID, 
                value);
            if (!IsSaving && !IsLoading & _ReceiptDetailID != null)
            {
                ExpenseType = _ReceiptDetailID.ExpenseType ?? null;
                SubExpenseType = _ReceiptDetailID.SubExpenseType ?? null;
            }
            }
        }
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get
            {
                return _SubExpenseType;
            }
            set
            {
                SetPropertyValue("SubExpenseType", ref _SubExpenseType, value);
            }
        }
        public decimal GetFactor()
        {
            bool found = false;
            decimal res = 1;
            if (_ItemNo.UOMRelations.Count > 0)
            {
                var dBaseUOM = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.BaseUOM2).FirstOrDefault();
                foreach (UOMRelation item in _ItemNo.UOMRelations)
                {
                    if (item.UOM == _UOM)
                    {
                        found = true;
                        res = item.Factor;
                        Cost = item.CostPerBaseUom;
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = _ItemNo.BaseUOM;
                    Cost = dBaseUOM.CostPerBaseUom;
                }
            }
            else
            {
                _UOM = _ItemNo.BaseUOM;
                Cost = _ItemNo.Cost;
            }
            //_Cost = _BaseCost * res;
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
        Association("DebitMemoDetail-DebitMemoDetailTrackingLines")]
        public XPCollection<DebitMemoDetailTrackingLine> 
        DebitMemoDetailTrackingLines { get { return GetCollection<
                DebitMemoDetailTrackingLine>("DebitMemoDetailTrackingLines"); } 
        }
        public DebitMemoDetail(Session session): base(session) {
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
