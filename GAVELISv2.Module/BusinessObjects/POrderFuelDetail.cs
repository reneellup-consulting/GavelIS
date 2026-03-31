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
using System.Text;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class POrderFuelDetail : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private FuelItem _ItemNo;
        private string _Description;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private decimal _BaseCost;
        //private decimal _Cost;
        //private decimal _Total;
        private decimal _Received;
        //private decimal _RemainingQty;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("GenJournalHeader-POrderFuelDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set {
                GenJournalHeader oldGenJournalID = _GenJournalID;
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && oldGenJournalID != _GenJournalID)
                {
                    oldGenJournalID = oldGenJournalID ?? _GenJournalID;
                    ((PurchaseOrderFuel)oldGenJournalID).UpdateTotal(true);
                }
            }
        }
        [NonPersistent]
        public PurchaseOrderFuel PurchaseInfo { get { return (PurchaseOrderFuel)
                _GenJournalID; } }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public FuelItem ItemNo {
            get { return _ItemNo; }
            set {
                FuelItem oldItem = _ItemNo;
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    if (RequisitionNo != null)
                    {
                        throw new UserFriendlyException("You can no longer change an item to the line carried out from requisition.");
                    }
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change Item from {1} to {2} by {3}", stamp, oldItem.No, value.No, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    Description = !string.IsNullOrEmpty(_ItemNo.
                    PurchaseDescription) ? _ItemNo.PurchaseDescription : _ItemNo
                    .Description;
                    UOM = _ItemNo.PurchaseUOM != null ? _ItemNo.PurchaseUOM :
                    _ItemNo.BaseUOM;
                    BaseCost = _ItemNo.Cost;
                    Factor = 1;
                    if (_ItemNo.UOMRelations.Count > 0)
                    {
                        var data = _ItemNo.UOMRelations.OrderBy(o => o.Factor).FirstOrDefault();
                        if (data != null)
                        {
                            BaseCost = data.CostPerBaseUom;
                        }
                        foreach (UOMRelation
                        item in _ItemNo.UOMRelations)
                        {
                            if (item.UOM == _UOM)
                            {
                                Factor = item.Factor;
                                //Factor = 1;
                                BaseCost = item.CostPerBaseUom;
                            }
                            //else
                            //{
                            //    Factor = item.Factor;
                            //}
                        }
                    }
                }
            }
        }
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        // Tariff
        public Tariff Tariff
        {
            get { return _Tariff; }
            set {
                Tariff old = _Tariff;
                SetPropertyValue("Tariff", ref _Tariff, value);
                if (!IsLoading && !IsSaving && _Tariff != null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change tariff of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old.Code, ((Tariff)value).Code, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    Origin = _Tariff.Origin;
                    Destination = _Tariff.Destination;
                }
            }
        }
        // CodeNo
        [DataSourceProperty("Tariff.TariffFuelAllocations")]
        public TariffFuelAllocation CodeNo
        {
            get { return _CodeNo; }
            set {
                TariffFuelAllocation old = _CodeNo;
                SetPropertyValue("CodeNo", ref _CodeNo, value);
                if (!IsLoading && !IsSaving && _CodeNo != null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change Code No of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old, ((TariffFuelAllocation)value).UnitType.Code, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    Tad = _CodeNo.DistRoundTrip;
                }
            }
        }
        // TAD
        [DisplayName("TAD")]
        [Custom("DisplayFormat", "n")]
        public decimal Tad
        {
            get { return _Tad; }
            set {
                decimal old = _Tad;
                SetPropertyValue("Tad", ref _Tad, value);
                if (!IsLoading && !IsSaving)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change Tad of {1} from {2:n} to {3:n} by {4}", stamp, ItemNo.No, old, value, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }
        // Origin
        [Custom("AllowEdit", "False")]
        public TripLocation Origin
        {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        // Destination
        [Custom("AllowEdit", "False")]
        public TripLocation Destination
        {
            get { return _Destination; }
            set { SetPropertyValue("Destination", ref _Destination, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set {
                decimal old = _Quantity;
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading && !IsSaving && GenJournalID != null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change quantity of {1} from {2:n} to {3:n} by {4}", stamp, ItemNo.No, old, value, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    ((PurchaseOrderFuel)GenJournalID).UpdateTotal(true);
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM {
            get { return _UOM; }
            set {
                UnitOfMeasure old = _UOM ?? null;
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change UOM of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    Factor = GetFactor();
                }
                if (!IsLoading && !IsSaving && GenJournalID != null)
                {
                    ((PurchaseOrderFuel)GenJournalID).UpdateTotal(true);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
        }
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseCost {
            get { return _BaseCost; }
            set {
                decimal old = _BaseCost;
                SetPropertyValue("BaseCost", ref _BaseCost, value);
                if (!IsLoading && !IsSaving && GenJournalID != null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change cost of {1} from {2:n} to {3:n} by {4}", stamp, ItemNo.No, old, value, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    ((PurchaseOrderFuel)GenJournalID).UpdateTotal(true);
                }
            }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter
        {
            get { return _CostCenter; }
            set
            {
                CostCenter old = _CostCenter ?? null;
                SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value);
                if (!IsLoading && old != null && value == null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change charge to of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : string.Empty, "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
                if (!IsLoading && !IsSaving && _CostCenter != null)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change charge to of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : string.Empty, value.Code, user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    Facility = _CostCenter.Facility ?? null;
                    Department = _CostCenter.Department ?? null;
                }
            }
        }
        public Warehouse StockTo
        {
            get { return _StockTo; }
            set
            {
                Warehouse old = _StockTo ?? null;
                SetPropertyValue<Warehouse>("StockTo", ref _StockTo, value);
                if (!IsLoading)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change StockTo by of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }
        // RequisitionNo
        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo
        {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }
        // RequisitionReq
        [Custom("AllowEdit", "False")]
        public bool RequisitionReq
        {
            get { return _RequisitionReq; }
            set { SetPropertyValue("RequisitionReq", ref _RequisitionReq, value); }
        }
        // PettyCashID
        [Custom("AllowEdit", "False")]
        public GenJournalDetail PettyCashID
        {
            get { return _PettyCashID; }
            set { SetPropertyValue<GenJournalDetail>("PettyCashID", ref _PettyCashID, value); }
        }
        // SkipRwsTrans
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public bool SkipRwsTrans
        {
            get { return _SkipRwsTrans; }
            set { SetPropertyValue<bool>("SkipRwsTrans", ref _SkipRwsTrans, value); }
        }

        [PersistentAlias("BaseCost * Factor")]
        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get {
                object tempObject = EvaluateAlias("Cost");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
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

        // RequestedBy
        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set
            {
                Employee old = _RequestedBy ?? null;
                SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value);
                if (!IsLoading)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change requested by of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Name : "None", value != null ? value.Name : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }
        // Line Approval Status
        [Custom("AllowEdit", "False")]
        public POLineStatusEnum LineApprovalStatus
        {
            get { return _LineApprovalStatus; }
            set { SetPropertyValue<POLineStatusEnum>("LineApprovalStatus", ref _LineApprovalStatus, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool IsSynced
        {
            get { return _IsSynced; }
            set { SetPropertyValue<bool>("IsSynced", ref _IsSynced, value); }
        }
        // Facility
        public Facility Facility
        {
            get { return _Facility; }
            set
            {
                Facility old = _Facility ?? null;
                SetPropertyValue("Facility", ref _Facility, value);
                if (!IsLoading)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change faciliy of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }
        // Department
        [DataSourceProperty("Facility.Departments")]
        public Department Department
        {
            get { return _Department; }
            set
            {
                Department old = _Department ?? null;
                SetPropertyValue("Department", ref _Department, value);
                if (!IsLoading)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change department of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
                if (!IsLoading && !IsSaving && _Department != null)
                {
                    FacilityHead = _Department.DepartmentHead ?? null;
                    DepartmentInCharge = _Department.InCharge ?? null;
                }
            }
        }
        // DepartmentInChange
        [Custom("AllowEdit", "False")]
        public Employee DepartmentInCharge
        {
            get { return _DepartmentInCharge; }
            set
            {
                Employee old = _DepartmentInCharge ?? null;
                SetPropertyValue("DepartmentInCharge", ref _DepartmentInCharge, value);
                if (!IsLoading)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change department in-charge of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Name : "None", value != null ? value.Name : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Employee FacilityHead
        {
            get { return _FacilityHead; }
            set
            {
                Employee old = _FacilityHead ?? null;
                SetPropertyValue("FacilityHead", ref _FacilityHead, value);
                if (!IsLoading)
                {
                    if (PurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change facility head of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Name : "None", value != null ? value.Name : "None", user));
                        PurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }
        // Remarks
        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Received {
            get { return _Received; }
            set { SetPropertyValue("Received", ref _Received, value); }
        }
        [PersistentAlias("Quantity - Received")]
        [Custom("DisplayFormat", "n")]
        public decimal RemainingQty {
            get {
                object tempObject = EvaluateAlias("RemainingQty");
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
            return res;
        }
        #region Fuel Information
        private decimal _OdoReading;
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoReading {
            get { return _OdoReading; }
            set { SetPropertyValue("OdoReading", ref _OdoReading, value); }
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

        public POrderFuelDetail(Session session): base(session) {
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
        protected override void OnSaving() {
            if (this.Oid == -1)
            {
                if (_ItemNo != null && PurchaseInfo.IsReopened)
                {
                    StringBuilder sb = new StringBuilder(PurchaseInfo.AfterReopenAlterations);
                    string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                    string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                    sb.AppendLine();
                    sb.Append(string.Format("{0}*** Added item {1} by {2}", stamp, ItemNo.No, user));
                    PurchaseInfo.AfterReopenAlterations = sb.ToString();
                }
            }
            // --- ADD THIS BLOCK ---
            if (!IsLoading && !IsDeleted && PurchaseInfo != null && PurchaseInfo.IsSynced)
            {
                //PurchaseInfo.ReSynced = true;
                PurchaseInfo.UpdateResync(true);
            }
            // ----------------------
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

        [Custom("AllowEdit", "False")]
        public Guid RequestID
        {
            get { return _RequestID; }
            set { SetPropertyValue("RequestID", ref _RequestID, value); }
        }
        private GenJournalHeader deletedId;
        private Item itemId;
        private Requisition requisitionId;
        private void RegenerateCarryoutTrans()
        {
            if (_RequisitionNo != null && _RequestID != Guid.Empty)
            {
                //UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
                //var rws = session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", _RequestID));
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
                if (gjh.SourceType.Code == "PF")
                {
                    rwct.Quantity = (obj as POrderFuelDetail).Quantity;

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

        #region Get Current User

        private SecurityUser _CurrentUser;
        private BusinessObjects.Tariff _Tariff;
        private TariffFuelAllocation _CodeNo;
        private decimal _Tad;
        private TripLocation _Origin;
        private TripLocation _Destination;
        private decimal _LineDiscPercent;
        private decimal _LineDiscount;
        private Requisition _RequisitionNo;
        private bool _RequisitionReq;
        private GenJournalDetail _PettyCashID;
        private bool _SkipRwsTrans;
        private Employee _RequestedBy;
        private POLineStatusEnum _LineApprovalStatus;
        private CostCenter _ChargeTo;
        private BusinessObjects.Facility _Facility;
        private BusinessObjects.Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        private string _Remarks;
        private Guid _RequestID;
        private BusinessObjects.CostCenter _CostCenter;
        private Warehouse _StockTo;
        private bool _IsSynced = false;
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

        protected override void OnDeleting()
        {
            if (GenJournalID != null && GenJournalID.IsSynced)
            {
                GenJournalID.ReSynced = true;
            }

            // Fix: Populate deletedId for your existing RegenerateCarryoutTrans() logic
            deletedId = this.GenJournalID;

            // --- NEW SYNC QUEUE BLOCK ---
            PoSyncDeletionsQueue syncQueue = new PoSyncDeletionsQueue(Session);
            syncQueue.POType = "Fuel";
            syncQueue.RowType = "Line";
            syncQueue.RowId = this.Oid;
            // ----------------------------

            base.OnDeleting();
        }

        #endregion

    }
}
