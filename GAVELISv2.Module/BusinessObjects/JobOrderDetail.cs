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
    public class JobOrderDetail : XPObject, ISetIncomeExpense {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private CostCenter _CostCenter;
        private Employee _Driver;
        private string _VendorMechanic;
        private Employee _OwnMechanic;
        private decimal _Quantity = 1;
        private decimal _Ordered;
        private decimal _Received;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        private decimal _BaseCost;
        private decimal _Cost;
        private PurchaseOrderDetail _PODetailID;
        private Guid _RequestID;
        private Requisition _RequisitionNo;
        private Employee _RequestedBy;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("GenJournalHeader-JobOrderDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set {
                GenJournalHeader oldGenJournalID = _GenJournalID;
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && oldGenJournalID != _GenJournalID)
                {
                    oldGenJournalID = oldGenJournalID ?? _GenJournalID;
                    ((JobOrder)oldGenJournalID).UpdateTotal(true);
                }
            }
        }

        [NonPersistent]
        public JobOrder JobOrderInfo {
            get { return (JobOrder)_GenJournalID; }
        }

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        [Association("Item-JobHistory")]
        public Item ItemNo {
            get { return _ItemNo; }
            set
            {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    Description = !string.IsNullOrEmpty(_ItemNo.
                    PurchaseDescription) ? _ItemNo.PurchaseDescription : _ItemNo
                    .Description;
                    UOM = _ItemNo.PurchaseUOM != null ? _ItemNo.PurchaseUOM :
                    _ItemNo.BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    BaseCost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        foreach (UOMRelation
                        item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _UOM)
                            {
                                Factor = item.Factor;
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
        [DisplayName("Charge To")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set
            {
                SetPropertyValue("CostCenter", ref _CostCenter, value);
                if (!IsLoading && _CostCenter != null)
                {
                    RequestedBy = _CostCenter.PersonResponsible ?? null;
                    Driver = _CostCenter.PersonResponsible ?? null;
                }
            }
        }

        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }

        public string VendorMechanic {
            get { return _VendorMechanic; }
            set
            {
                if (_VendorMechanic == value)
                    return;
                _VendorMechanic = value;
            }
        }

        public Employee OwnMechanic {
            get { return _OwnMechanic; }
            set
            {
                if (_OwnMechanic == value)
                    return;
                _OwnMechanic = value;
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((JobOrder)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Ordered {
            get { return _Ordered; }
            set { SetPropertyValue<decimal>("Ordered", ref _Ordered, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Received {
            get { return _Received; }
            set { SetPropertyValue<decimal>("Received", ref _Received, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set
            {
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null)
                {
                    Factor = GetFactor();
                }
                if (!IsLoading)
                {
                    try
                    {
                        ((JobOrder)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set
            {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((JobOrder)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
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
            get
            {
                object tempObject = EvaluateAlias("BaseQTY");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseCost {
            get { return _BaseCost; }
            set
            {
                SetPropertyValue("BaseCost", ref _BaseCost, value);
                if (!IsLoading)
                {
                    _Cost = 0;
                    _Cost = _BaseCost * _Factor;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public PurchaseOrderDetail PODetailID {
            get { return _PODetailID; }
            set { SetPropertyValue<PurchaseOrderDetail>("PODetailID", ref _PODetailID, value); }
        }

        [Custom("AllowEdit", "False")]
        public Guid RequestID {
            get { return _RequestID; }
            set { SetPropertyValue<Guid>("RequestID", ref _RequestID, value); }
        }

        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }

        public Employee RequestedBy {
            get { return _RequestedBy; }
            set { SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value); }
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
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set
            {
                SetPropertyValue("Cost", ref _Cost, value);
                if (!IsLoading)
                {
                    _BaseCost = 0;
                    _BaseCost = _Cost / _Factor;
                }
                if (!IsLoading)
                {
                    try
                    {
                        ((JobOrder)_GenJournalID).UpdateTotal(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [PersistentAlias("Quantity * Cost")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        public decimal GetFactor() {
            bool found = false;
            decimal res = 1;
            if (_ItemNo.UOMRelations.Count > 0)
            {
                foreach (UOMRelation item in _ItemNo.UOMRelations)
                {
                    if (item.UOM == _UOM)
                    {
                        found = true;
                        res = item.Factor;
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = _ItemNo.BaseUOM;
                }
            } else
            {
                _UOM = _ItemNo.BaseUOM;
            }
            _Cost = _BaseCost * res;
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

        public JobOrderDetail(Session session)
            : base(session) {
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
            //Session.OptimisticLockingReadBehavior = 
            //OptimisticLockingReadBehavior.ReloadObject;
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
        private void RegenerateCarryoutTrans()
        {
            if (_RequisitionNo != null && _RequestID != Guid.Empty)
            {
                //UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
                //var rws = session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _PODetailID.RequestID));
                if (deletedId != null)
                {
                    this.RegenerateCarryoutTrans(deletedId, this, itemId, requisitionId, true);
                }
                else
                {
                    this.RegenerateCarryoutTrans(this.GenJournalID, this, _ItemNo, _RequisitionNo);
                }
                //rws.Save();
                //session.CommitChanges();
            }
        }

        public void RegenerateCarryoutTrans(GenJournalHeader genjo, XPObject obj, Item item, Requisition req, bool delete = false)
        {
            #region Start Processing

            RequisitionWorksheet req01 = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _RequestID));
            if (req01 == null)
            {
                throw new ApplicationException("There is no valid Requisition Worksheet identified for this transaction line");
            }
            CriteriaOperator criteria = null;
            if (obj != null && delete)
            {
                var data = (from rct in req01.ReqCarryoutTransactions
                            where rct.SourceType.Code == genjo.SourceType.Code && rct.LineNo == obj.Oid
                            select rct).FirstOrDefault();
                if (data != null)
                {
                    data.Delete();
                }
            }
            else if (obj != null && genjo != null && !delete)
            {
                GenJournalHeader gjh = this.Session.GetObjectByKey<GenJournalHeader>(genjo.Oid);
                criteria = CriteriaOperator.Parse(string.Format("[SourceType.Code] = '{0}' And [LineNo] = {1}", genjo.SourceType.Code, obj.Oid));
                ReqCarryoutTransaction rwct = this.Session.FindObject<ReqCarryoutTransaction>(criteria);
                if (rwct == null)
                {
                    rwct = ReflectionHelper.CreateObject<ReqCarryoutTransaction>(this.Session);
                }
                rwct.ReqWorksheetId = req01;
                rwct.TransactionId = gjh;
                rwct.SourceType = gjh.SourceType;
                rwct.LineNo = obj.Oid;
                if (gjh.SourceType.Code == "PO")
                {
                    rwct.Quantity = (obj as PurchaseOrderDetail).Quantity;

                }
                if (gjh.SourceType.Code == "RC")
                {
                    rwct.Quantity = (obj as ReceiptDetail).Quantity;

                }
                if (gjh.SourceType.Code == "TO")
                {
                    rwct.Quantity = (obj as TransferOrderDetail).Quantity;

                }
                if (gjh.SourceType.Code == "SO")
                {
                    rwct.Quantity = (obj as SalesOrderDetail).Quantity;

                }
                if (gjh.SourceType.Code == "JO")
                {
                    rwct.Quantity = (obj as JobOrderDetail).Quantity;

                }
                if (gjh.SourceType.Code == "WO")
                {
                    rwct.Quantity = (obj as WorkOrderItemDetail).Quantity;

                }
                rwct.Save();
            }
            req01.Save();
            this.Session.CommitTransaction();
            //this.Session.Reload(req01);
            #endregion
        }

        private GenJournalHeader deletedId;
        private Item itemId;
        private Requisition requisitionId;
        private int deletedOid;
        protected override void OnDeleting() {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
                deletedId = this.GenJournalID;
            }
            itemId = this.ItemNo;
            requisitionId = this.RequisitionNo ?? null;
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
            } else
            {
                if (this.GenJournalID != null)
                {
                    this.GenJournalID.IsIncExpNeedUpdate = true;
                    this.GenJournalID.Save();
                }
            }
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

        public bool skipAuto = false;
        protected override void OnSaved()
        {
            base.OnSaved();
            if (!skipAuto)
            {
                this.RegenerateCarryoutTrans();
            }
        }

        protected override void OnDeleted()
        {
            base.OnDeleted();
            this.RegenerateCarryoutTrans();
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
