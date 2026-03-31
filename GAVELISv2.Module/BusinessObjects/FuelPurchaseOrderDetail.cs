using System;
using System.Linq;
using System.Text;
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
    [NavigationItem(false)]
    public class FuelPurchaseOrderDetail : XPObject
    {
        private GenJournalHeader _GenJournalID;
        [Association("GenJournalHeader-FuelPurchaseOrderDetails")]
        public GenJournalHeader GenJournalID
        {
            get { return _GenJournalID; }
            set
            {
                GenJournalHeader oldGenJournalID = _GenJournalID;
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && oldGenJournalID != _GenJournalID)
                {
                    oldGenJournalID = oldGenJournalID ?? _GenJournalID;
                    ((FuelPurchaseOrder)oldGenJournalID).UpdateTotal(true);
                }
            }
        }

        [NonPersistent]
        public FuelPurchaseOrder FuelPurchaseInfo
        {
            get
            {
                return (FuelPurchaseOrder)
                    _GenJournalID;
            }
        }

        [NonPersistent]
        public Company CompanyInfo
        {
            get { return Company.GetInstance(Session); }
        }

        private Item _ItemNo;
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set
            {
                Item oldItem = _ItemNo;
                SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && _ItemNo != null)
                {
                    if (RequisitionNo != null)
                    {
                        throw new UserFriendlyException("You can no longer change an item to the line carried out from requisition.");
                    }
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change Item from {1} to {2} by {3}", stamp, oldItem.No, value.No, user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
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

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        private decimal _Quantity;
        [Custom("DisplayFormat", "n")]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                decimal old = _Quantity;
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading && !IsSaving && GenJournalID != null)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change quantity of {1} from {2:n} to {3:n} by {4}", stamp, ItemNo.No, old, value, user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    ((FuelPurchaseOrder)GenJournalID).UpdateTotal(true);
                }
            }
        }

        private UnitOfMeasure _UOM;
        [RuleRequiredField("", DefaultContexts.Save)]
        public UnitOfMeasure UOM
        {
            get { return _UOM; }
            set
            {
                UnitOfMeasure old = _UOM ?? null;
                SetPropertyValue("UOM", ref _UOM, value);
                if (!IsLoading && _UOM != null)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change UOM of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    Factor = GetFactor();
                }
                if (!IsLoading && !IsSaving && GenJournalID != null)
                {
                    ((PurchaseOrder)GenJournalID).UpdateTotal(true);
                }
            }
        }

        private decimal _Factor;
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor
        {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
        }

        private decimal _BaseCost;
        [Custom("DisplayFormat", "n")]
        public decimal BaseCost
        {
            get { return _BaseCost; }
            set
            {
                decimal old = _BaseCost;
                SetPropertyValue("BaseCost", ref _BaseCost, value);
                if (!IsLoading && !IsSaving && GenJournalID != null)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change cost of {1} from {2:n} to {3:n} by {4}", stamp, ItemNo.No, old, value, user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    ((FuelPurchaseOrder)GenJournalID).UpdateTotal(true);
                }
            }
        }

        private decimal _LineDiscPercent;
        [Custom("DisplayFormat", "n")]
        public decimal LineDiscPercent
        {
            get { return _LineDiscPercent; }
            set
            {
                SetPropertyValue("LineDiscPercent", ref _LineDiscPercent, value);
                if (!IsLoading && !IsSaving)
                {
                    LineDiscount = (_Quantity * Cost) * (_LineDiscPercent / 100);
                }
            }
        }

        private BusinessObjects.CostCenter _CostCenter;
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
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change charge to of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : string.Empty, "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
                if (!IsLoading && !IsSaving && _CostCenter != null)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change charge to of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : string.Empty, value.Code, user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
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
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change StockTo by of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }

        private Employee _RequestedBy;
        public Employee RequestedBy
        {
            get { return _RequestedBy; }
            set
            {
                Employee old = _RequestedBy ?? null;
                SetPropertyValue<Employee>("RequestedBy", ref _RequestedBy, value);
                if (!IsLoading)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change requested by of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Name : "None", value != null ? value.Name : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }

        private POLineStatusEnum _LineApprovalStatus;
        [Custom("AllowEdit", "False")]
        public POLineStatusEnum LineApprovalStatus
        {
            get { return _LineApprovalStatus; }
            set { SetPropertyValue<POLineStatusEnum>("LineApprovalStatus", ref _LineApprovalStatus, value); }
        }

        private string _Remarks;
        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        private Facility _Facility;
        public Facility Facility
        {
            get { return _Facility; }
            set
            {
                Facility old = _Facility ?? null;
                SetPropertyValue("Facility", ref _Facility, value);
                if (!IsLoading)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change faciliy of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }

        private Department _Department;
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
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change department of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Code : "None", value != null ? value.Code : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
                if (!IsLoading && !IsSaving && _Department != null)
                {
                    FacilityHead = _Department.DepartmentHead ?? null;
                    DepartmentInCharge = _Department.InCharge ?? null;
                }
            }
        }

        private Employee _FacilityHead;
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
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change facility head of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Name : "None", value != null ? value.Name : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }

        private Employee _DepartmentInCharge;
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
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change department in-charge of {1} from {2} to {3} by {4}", stamp, ItemNo.No, old != null ? old.Name : "None", value != null ? value.Name : "None", user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                }
            }
        }

        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        private decimal _LineDiscount;
        [Custom("DisplayFormat", "n")]
        public decimal LineDiscount
        {
            get { return _LineDiscount; }
            set
            {
                decimal old = _LineDiscount;
                SetPropertyValue("LineDiscount", ref _LineDiscount, value);
                if (!IsLoading && !IsSaving)
                {
                    if (FuelPurchaseInfo.IsReopened)
                    {
                        StringBuilder sb = new StringBuilder(FuelPurchaseInfo.AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendLine();
                        sb.Append(string.Format("{0}*** Change discount of {1} from {2:n} to {3:n} by {4}", stamp, ItemNo.No, old, value, user));
                        FuelPurchaseInfo.AfterReopenAlterations = sb.ToString();
                    }
                    try
                    {
                        ((PurchaseOrder)_GenJournalID).UpdateTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [PersistentAlias("BaseCost * 1")]
        [Custom("DisplayFormat", "n")]
        public decimal Cost
        {
            get
            {
                object tempObject = EvaluateAlias("Cost");
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

        [PersistentAlias("(Quantity * Cost) - LineDiscount")]
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

        private decimal _Received;
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Received
        {
            get { return _Received; }
            set { SetPropertyValue("Received", ref _Received, value); }
        }

        private Guid _RequestID;
        [Custom("AllowEdit", "False")]
        public Guid RequestID
        {
            get { return _RequestID; }
            set { SetPropertyValue("RequestID", ref _RequestID, value); }
        }

        private Requisition _RequisitionNo;
        [Custom("AllowEdit", "False")]
        public Requisition RequisitionNo
        {
            get { return _RequisitionNo; }
            set { SetPropertyValue<Requisition>("RequisitionNo", ref _RequisitionNo, value); }
        }

        [PersistentAlias("Quantity - Received")]
        [Custom("DisplayFormat", "n")]
        public decimal RemainingQty
        {
            get
            {
                object tempObject = EvaluateAlias("RemainingQty");
                if (tempObject != null)
                {
                    if ((decimal)tempObject < 0)
                    {
                        return 0;
                    }
                    else
                        return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        private bool _RequisitionReq = false;
        [Custom("AllowEdit", "False")]
        public bool RequisitionReq
        {
            get { return _RequisitionReq; }
            set { SetPropertyValue("RequisitionReq", ref _RequisitionReq, value); }
        }

        [RuleFromBoolProperty("", DefaultContexts.Save, CustomMessageTemplate =
        "Must have a corresponding Requisition.")]
        protected bool IsValidRequisitionNo
        {
            get
            {
                if (this.RequisitionReq && this.CompanyInfo.NoReqNoPo && _RequisitionNo == null)
                {
                    return false;
                }
                else if (this.RequisitionReq && this.CompanyInfo.NoReqNoPo && _RequisitionNo != null)
                {
                    return true;
                }
                else
                {
                    return true;
                }
            }
        }

        private bool _SkipRwsTrans;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public bool SkipRwsTrans
        {
            get { return _SkipRwsTrans; }
            set { SetPropertyValue<bool>("SkipRwsTrans", ref _SkipRwsTrans, value); }
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
                        BaseCost = item.CostPerBaseUom;
                        break;
                    }
                }
                if (!found)
                {
                    _UOM = dBaseUOM.UOM;
                    BaseCost = dBaseUOM.CostPerBaseUom;
                }
            }
            else
            {
                _UOM = _ItemNo.BaseUOM;
                BaseCost = _ItemNo.Cost;
            }
            return res;
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

        public FuelPurchaseOrderDetail(Session session)
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
        private Warehouse _StockTo;
        private GenJournalDetail _PettyCashID;
      
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
