using System;
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
    public class WorkOrderJobsDetail : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private JobOrder _JobOrderNo;
        private JobOrderDetail _JobOrderDetailNo;
        private Item _ItemNo;
        private string _Description;
        private CostCenter _CostCenter;
        private Employee _Driver;
        private Employee _Mechanic;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        private decimal _BaseCost;
        private decimal _Cost;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("WorkOrder-WorkOrderJobsDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set {
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && _GenJournalID != null) {
                    Driver = ((WorkOrder)_GenJournalID).Driver != null ? ((
                    WorkOrder)_GenJournalID).Driver : null;
                    if (((WorkOrder)_GenJournalID).Fleet != null)
                    {
                    CostCenter = ((WorkOrder)_GenJournalID).Fleet != null ? (((
                    WorkOrder)_GenJournalID).Fleet.CostCenter != null ? ((
                    WorkOrder)_GenJournalID).Fleet.CostCenter : null) : null;

                    }
                    if (((WorkOrder)_GenJournalID).Fleet == null && ((WorkOrder)_GenJournalID).Facility != null)
                    {
                        CostCenter = ((WorkOrder)_GenJournalID).Facility != null ? (((
                        WorkOrder)_GenJournalID).Facility.CostCenter != null ? ((
                        WorkOrder)_GenJournalID).Facility.CostCenter : null) : null;

                    }
                }
            }
        }
        public JobOrder JobOrderNo {
            get { return _JobOrderNo; }
            set { SetPropertyValue("JobOrderNo", ref _JobOrderNo, value); }
        }
        public JobOrderDetail JobOrderDetailNo {
            get { return _JobOrderDetailNo; }
            set {
                SetPropertyValue("JobOrderDetailNo", ref _JobOrderDetailNo, 
                value);
                if (!IsLoading) {
                    ItemNo = JobOrderDetailNo.ItemNo;
                    Description = JobOrderDetailNo.Description;
                    CostCenter = JobOrderDetailNo.CostCenter;
                    Driver = JobOrderDetailNo.Driver;
                    Quantity = JobOrderDetailNo.Quantity;
                    UOM = JobOrderDetailNo.UOM;
                    Factor = JobOrderDetailNo.Factor;
                    BaseUOM = JobOrderDetailNo.BaseUOM;
                    BaseCost = JobOrderDetailNo.BaseCost;
                    Cost = JobOrderDetailNo.Cost;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo {
            get { return _ItemNo; }
            set {
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null) {
                    Description = !string.IsNullOrEmpty(_ItemNo.
                    PurchaseDescription) ? _ItemNo.PurchaseDescription : _ItemNo
                    .Description;
                    UOM = _ItemNo.PurchaseUOM != null ? _ItemNo.PurchaseUOM : 
                    _ItemNo.BaseUOM;
                    BaseUOM = _ItemNo.BaseUOM;
                    BaseCost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0) {foreach (UOMRelation 
                        item in _ItemNo.UOMRelations) {if (item.UOM == _UOM) {
                                Factor = item.Factor;}}}
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
            set { SetPropertyValue("CostCenter", ref _CostCenter, value); }
        }
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        public Employee Mechanic {
            get { return _Mechanic; }
            set { SetPropertyValue("Mechanic", ref _Mechanic, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value);
            if (!IsLoading)
            {
                try
                {
                    //((WorkOrder)_GenJournalID).UpdateTotal(true);
                    ((WorkOrder)_GenJournalID).UpdateTotalIntJobs(true);
                    ((WorkOrder)_GenJournalID).UpdateTotalWithJO(true);
                    ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
                }
                catch (Exception) { }
            }

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
                        //((WorkOrder)_GenJournalID).UpdateTotal(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalIntJobs(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalWithJO(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
                    }
                    catch (Exception) { }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set {
                SetPropertyValue("Factor", ref _Factor, value);
                if (!IsLoading) {
                    try {
                        //((WorkOrder)_GenJournalID).UpdateTotal(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalIntJobs(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalWithJO(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
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
                if (!IsLoading) {
                    _Cost = 0;
                    _Cost = _BaseCost * _Factor;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set {
                SetPropertyValue("Cost", ref _Cost, value);
                if (!IsLoading) {
                    _BaseCost = 0;
                    _BaseCost = _Cost / _Factor;
                }
                if (!IsLoading) {
                    try {
                        //((WorkOrder)_GenJournalID).UpdateTotal(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalIntJobs(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalWithJO(true);
                        ((WorkOrder)_GenJournalID).UpdateTotalParts(true);
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
        public decimal GetFactor() {
            bool found = false;
            decimal res = 1;
            if (_ItemNo.UOMRelations.Count > 0) {
                foreach (UOMRelation item in _ItemNo.UOMRelations) {
                    if (item.UOM == _UOM) {
                        found = true;
                        res = item.Factor;
                        break;
                    }
                }
                if (!found) {_UOM = _ItemNo.BaseUOM;}
            } else {
                _UOM = _ItemNo.BaseUOM;
            }
            _Cost = _BaseCost * res;
            return res;
        }
        [NonPersistent]
        public WorkOrder WorkOrderInfo { get { return (WorkOrder)_GenJournalID; 
            } }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
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
        public WorkOrderJobsDetail(Session session): base(session) {
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
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }
        protected override void OnSaving() {
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
