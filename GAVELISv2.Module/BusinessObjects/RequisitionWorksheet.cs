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
    public class RequisitionWorksheet : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Item _ItemNo;
        private string _Description;
        private string _PurchaseDescription;
        private string _SalesDescription;
        private decimal _Quantity = 1;
        private UnitOfMeasure _UOM;
        private decimal _Factor = 1;
        private UnitOfMeasure _BaseUOM;
        private decimal _BaseQTY;
        private decimal _BaseCost;
        private decimal _Cost;
        private decimal _Total;
        private RequisitionActionsEnum _Action;
        private RequisitionWSStateEnum _Status;
        private bool _Served = false;
        private decimal _ServedQTY;
        private decimal _OnPO;
        private decimal _OnTO;
        private decimal _OnSO;
        private decimal _OnJo;
        private decimal _OnWO;
        private CostCenter _CostCenter;
        private DateTime _ExpectedDate;
        //private decimal _CurrentQtyBase;
        private string _Reason;
        private bool _CarryOut = false;
        private GenJournalHeader _LastCarrySource;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private Facility _Facility;
        private Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        [NonPersistent]
        public Requisition RequisitionInfo {
            get { return (Requisition)
                _GenJournalID; }
        }
        //[Persistent("RequestSatus")]
        //private string _RequestSatus;
        //[PersistentAlias("_RequestSatus")]
        //public string RequestSatus
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (!IsLoading && !IsSaving && string.IsNullOrEmpty(_RequestSatus))
        //            {
        //                return ((Requisition)
        //                    _GenJournalID).Status.ToString();
        //            }
        //        }
        //        catch (Exception)
        //        {
        //        }
        //        return _RequestSatus;
        //    }
        //}
        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        [Association("GenJournalHeader-RequisitionWorksheetLines")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set
            {
                bool modified = SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    this.UpdateCurrentQtyBase(true);
                }
                if (!IsLoading && _GenJournalID != null)
                {
                    ExpectedDate = ((Requisition)_GenJournalID).ExpectedDate;
                    CostCenter = ((Requisition)_GenJournalID).CostCenter;
                    ExpenseType = ((Requisition)_GenJournalID).ExpenseType ?? null;
                    SubExpenseType = ((Requisition)_GenJournalID).SubExpenseType ?? null;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public Item ItemNo {
            get { return _ItemNo; }
            set
            {
                bool modified = SetPropertyValue("ItemNo", ref _ItemNo, value);
                if (!IsLoading && !IsSaving && modified)
                {
                    this.UpdateCurrentQtyBase(true);
                }
                if (!IsLoading && _ItemNo != null)
                {
                    Description = !string.IsNullOrEmpty(_ItemNo.Description) ?
                    _ItemNo.Description : _ItemNo.Description;
                    SalesDescription = !string.IsNullOrEmpty(_ItemNo.
                    SalesDescription) ? _ItemNo.SalesDescription : _ItemNo.
                    Description;
                    PurchaseDescription = !string.IsNullOrEmpty(_ItemNo.
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

        public string PurchaseDescription {
            get { return _PurchaseDescription; }
            set { SetPropertyValue("PurchaseDescription", ref
                _PurchaseDescription, value); }
        }

        public string SalesDescription {
            get { return _SalesDescription; }
            set { SetPropertyValue("SalesDescription", ref _SalesDescription,
                value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set { SetPropertyValue("Quantity", ref _Quantity, value); }
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
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Factor {
            get { return _Factor; }
            set { SetPropertyValue("Factor", ref _Factor, value); }
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
                    _UOM = dBaseUOM.UOM;
                    Cost = dBaseUOM.CostPerBaseUom;
                }
                //var data = (from rels in _ItemNo.UOMRelations
                //            where rels.UOM == _UOM
                //            select rels).FirstOrDefault<UOMRelation>();
                ////if (!found) {_UOM = _ItemNo.BaseUOM;}
                //if (data != null)
                //{
                //    //_UOM = _ItemNo.BaseUOM;
                //    res = data.Factor;
                //    Cost = data.CostPerBaseUom;
                //}
            } else
            {
                _UOM = _ItemNo.BaseUOM;
                Cost = _ItemNo.Cost;
            }
            //_Cost = _BaseCost * res;
            return res;
        }

        [Custom("AllowEdit", "False")]
        public RequisitionActionsEnum Action {
            get { return _Action; }
            set { SetPropertyValue("Action", ref _Action, value); }
        }

        public RequisitionWSStateEnum Status {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); 
            
            }
        }
        [Action(Caption = "Mark/Unmark as Cancelled", ConfirmationMessage = "Do you really want to do this?", AutoCommit = true)]
        public void MarkUnmarkAsCancelled()
        {
            if (ReqCarryoutTransactions.Count > 0)
            {
                throw new UserFriendlyException("You can no longer alter active item request");
            }
            if (_Cancelled)
            {
                Cancelled = false;
            }
            else
            {
                Cancelled = true;
                Served = false;
            }
        }
        public bool Cancelled
        {
            get { return _Cancelled; }
            set { SetPropertyValue("Cancelled", ref _Cancelled, value); }
        }
        public bool Served {
            get { return _Served; }
            set { SetPropertyValue("Served", ref _Served, value); }
        }
        [Action(Caption = "Mark/Unmark for PO", ConfirmationMessage = "Do you really want to do this?", AutoCommit = true)]
        public void MarkUnmarkForPO()
        {
            if (_ForPO)
            {
                ForPO = false;
            }
            else
            {
                ForPO = true;
            }
        }
        [Custom("AllowEdit", "False")]
        public bool ForPO
        {
            get { return _ForPO; }
            set { SetPropertyValue("ForPO", ref _ForPO, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ServedQTY {
            get { return _ServedQTY; }
            set { SetPropertyValue("ServedQTY", ref _ServedQTY, value); }
        }

        //[Custom("AllowEdit", "False")]
        //public GenJournalHeader LastCarrySource {
        //    get { return _LastCarrySource; }
        //    set { SetPropertyValue("LastCarrySource", ref _LastCarrySource,
        //        value); }
        //}

        [Custom("AllowEdit", "False")]
        public GenJournalHeader LastCarrySource
        {
            get
            {
                if (ReqCarryoutTransactions != null && ReqCarryoutTransactions.Count == 0)
                {
                    return null;
                }
                try
                {
                    var data = ReqCarryoutTransactions.OrderBy(o => o.TransactionId.EntryDate.ToShortDateString()).ThenBy(o => o.Oid).LastOrDefault();
                    if (data != null)
                    {
                        _LastCarrySource = data.TransactionId;
                    }

                }
                catch (Exception)
                {
                }
                return _LastCarrySource ?? null;
            }
        }
        // LastPO
        [Custom("AllowEdit", "False")]
        public GenJournalHeader LastPO
        {
            get
            {
                if (ReqCarryoutTransactions != null && ReqCarryoutTransactions.Count == 0)
                {
                    return null;
                }
                try
                {
                    var data = ReqCarryoutTransactions.OrderBy(o => o.TransactionId.EntryDate).Where(o => o.SourceType.Code == "PO").LastOrDefault();
                    if (data != null)
                    {
                        return data.TransactionId;
                    }

                }
                catch (Exception)
                {
                }
                return null;
            }
        }
        // LastRC
        [Custom("AllowEdit", "False")]
        public GenJournalHeader LastRC
        {
            get
            {
                if (ReqCarryoutTransactions != null && ReqCarryoutTransactions.Count == 0)
                {
                    return null;
                }
                try
                {
                    var data = ReqCarryoutTransactions.OrderBy(o => o.TransactionId.EntryDate).Where(o => o.SourceType.Code == "RC").LastOrDefault();
                    if (data != null)
                    {
                        return data.TransactionId;
                    }

                }
                catch (Exception)
                {
                }
                return null;
            }
        }
        // LastTO
        [Custom("AllowEdit", "False")]
        public GenJournalHeader LastTO
        {
            get
            {
                if (ReqCarryoutTransactions != null && ReqCarryoutTransactions.Count == 0)
                {
                    return null;
                }
                try
                {
                    var data = ReqCarryoutTransactions.OrderBy(o => o.TransactionId.EntryDate).Where(o => o.SourceType.Code == "TO").LastOrDefault();
                    if (data != null)
                    {
                        return data.TransactionId;
                    }

                }
                catch (Exception)
                {
                }
                return null;
            }
        }
        // LastWO
        [Custom("AllowEdit", "False")]
        public GenJournalHeader LastWO
        {
            get
            {
                if (ReqCarryoutTransactions != null && ReqCarryoutTransactions.Count == 0)
                {
                    return null;
                }
                try
                {
                    var data = ReqCarryoutTransactions.OrderBy(o => o.TransactionId.EntryDate).Where(o => o.SourceType.Code == "WO").LastOrDefault();
                    if (data != null)
                    {
                        return data.TransactionId;
                    }

                }
                catch (Exception)
                {
                }
                return null;
            }
        }

        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set
            {
                SetPropertyValue<ExpenseType>("ExpenseType", ref _ExpenseType, value);
                if (!IsLoading)
                {
                    SubExpenseType = null;
                }
            }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get { return _SubExpenseType; }
            set { SetPropertyValue<SubExpenseType>("SubExpenseType", ref _SubExpenseType, value); }
        }
        public Facility Facility
        {
            get { return _Facility; }
            set
            {
                SetPropertyValue("Facility", ref _Facility, value);
            }
        }
        [NonPersistent]
        public bool IsChargeToUnit
        {
            get {
                if (_CostCenter != null)
                {
                    if (_CostCenter.FixedAsset != null)
                    {
                        switch (_CostCenter.FixedAsset.FixedAssetClass)
                        {
                            case FixedAssetClassEnum.LandAndBuilding:
                                return false;
                            case FixedAssetClassEnum.Truck:
                                return true;
                            case FixedAssetClassEnum.Trailer:
                                return true;
                            case FixedAssetClassEnum.GeneratorSet:
                                return false;
                            case FixedAssetClassEnum.OtherVehicle:
                                return false;
                            case FixedAssetClassEnum.Other:
                                return false;
                            default:
                                return false;
                        }
                    }
                }
                return false; }
        }
        [DataSourceProperty("Facility.Departments")]
        public Department Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
                if (!IsLoading && !IsSaving && _Department != null)
                {
                    FacilityHead = _Department.DepartmentHead ?? null;
                    DepartmentInCharge = _Department.InCharge ?? null;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Employee FacilityHead
        {
            get { return _FacilityHead; }
            set { SetPropertyValue("FacilityHead", ref _FacilityHead, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee DepartmentInCharge
        {
            get { return _DepartmentInCharge; }
            set { SetPropertyValue("DepartmentInCharge", ref _DepartmentInCharge, value); }
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal OnPO {
        //    get
        //    {
        //        //if (_LastCarrySource != null && _LastCarrySource.SourceType.Code == "PO")
        //        //{
        //        //    var det = Session.FindObject<PurchaseOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //    _OnPO = det != null ? det.Quantity : 0;
        //        //}
        //        //var det = Session.FindObject<PurchaseOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //_OnPO = det != null ? det.Quantity : 0;

        //        var data = from trans in ReqCarryoutTransactions
        //                   where trans.SourceType.Code == "PO"
        //                   select trans;
        //        if (data != null)
        //        {
        //            return data.Select(c => c.Quantity).Sum();
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //    //set { SetPropertyValue("OnPO", ref _OnPO, value); }
        //}
        [Persistent("OnPO")]
        private decimal? fOnPO = null;
        [PersistentAlias("fOnPO")]
        public decimal? OnPO
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnPO == null)
                    UpdateOnPO(false);
                return fOnPO;
            }
        }
        public void UpdateOnPO(bool forceChangeEvents)
        {
            decimal? oldOnPO = fOnPO;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "PO"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                    _LastPO = data.LastOrDefault().TransactionId;
                }
            }
            catch (Exception)
            {
            }
            fOnPO = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnPO", oldOnPO, fOnPO);
        }

        [Persistent("OnFO")]
        private decimal? fOnFO = null;
        [PersistentAlias("fOnFO")]
        public decimal? OnFO
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnFO == null)
                    UpdateOnFO(false);
                return fOnFO;
            }
        }
        public void UpdateOnFO(bool forceChangeEvents)
        {
            decimal? oldOnFO = fOnFO;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "PF"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                    _LastFO = data.LastOrDefault().TransactionId;
                }
            }
            catch (Exception)
            {
            }
            fOnFO = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnFO", oldOnFO, fOnFO);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal RecQTY {
        //    get
        //    {
        //        var data = from trans in ReqCarryoutTransactions
        //                   where trans.SourceType.Code == "RC"
        //                   select trans;
        //        if (data != null)
        //        {
        //            return data.Select(c => c.Quantity).Sum();
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //}
        [Persistent("RecQTY")]
        private decimal? fRecQTY = null;
        [PersistentAlias("fRecQTY")]
        public decimal? RecQTY
        {
            get
            {
                if (!IsLoading && !IsSaving && fRecQTY == null)
                    UpdateRecQTY(false);
                return fRecQTY;
            }
        }
        public void UpdateRecQTY(bool forceChangeEvents)
        {
            decimal? oldRecQTY = fRecQTY;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "RC"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                    _LastRC = data.LastOrDefault().TransactionId;
                }
            }
            catch (Exception)
            {
            }
            fRecQTY = tempTotal;
            if (forceChangeEvents)
                OnChanged("RecQTY", oldRecQTY, fRecQTY);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal OnTO {
        //    get
        //    {
        //        //if (_LastCarrySource != null && _LastCarrySource.SourceType.Code == "TO")
        //        //{
        //        //    var det = Session.FindObject<TransferOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //    _OnTO = det != null ? det.Quantity : 0;
        //        //}
        //        //var det = Session.FindObject<TransferOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //_OnTO = det != null ? det.Quantity : 0;

        //        var data = from trans in ReqCarryoutTransactions
        //                   where trans.SourceType.Code == "TO"
        //                   select trans;
        //        if (data != null)
        //        {
        //            return data.Select(c => c.Quantity).Sum();
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //    //set { SetPropertyValue("OnTO", ref _OnTO, value); }
        //}
        [Persistent("OnTO")]
        private decimal? fOnTO = null;
        [PersistentAlias("fOnTO")]
        public decimal? OnTO
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnTO == null)
                    UpdateOnTO(false);
                return fOnTO;
            }
        }
        public void UpdateOnTO(bool forceChangeEvents)
        {
            decimal? oldOnTO = fOnTO;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "TO"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                    _LastTO = data.LastOrDefault().TransactionId;
                }
            }
            catch (Exception)
            {
            }
            fOnTO = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnTO", oldOnTO, fOnTO);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal OnSO {
        //    get
        //    {
        //        //if (_LastCarrySource != null && _LastCarrySource.SourceType.Code == "SO")
        //        //{
        //        //    var det = Session.FindObject<SalesOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //    _OnSO = det != null ? det.Quantity : 0;
        //        //}
        //        //var det = Session.FindObject<SalesOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //_OnSO = det != null ? det.Quantity : 0;

        //        var data = from trans in ReqCarryoutTransactions
        //                   where trans.SourceType.Code == "SO"
        //                   select trans;
        //        if (data != null)
        //        {
        //            return data.Select(c => c.Quantity).Sum();
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //    //set { SetPropertyValue("OnSO", ref _OnSO, value); }
        //}
        [Persistent("OnSO")]
        private decimal? fOnSO = null;
        [PersistentAlias("fOnSO")]
        public decimal? OnSO
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnSO == null)
                    UpdateOnSO(false);
                return fOnSO;
            }
        }
        public void UpdateOnSO(bool forceChangeEvents)
        {
            decimal? oldOnSO = fOnSO;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "SO"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                }

            }
            catch (Exception)
            {
            }
            fOnSO = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnSO", oldOnSO, fOnSO);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal OnJO {
        //    get
        //    {
        //        //if (_LastCarrySource != null && _LastCarrySource.SourceType.Code == "SO")
        //        //{
        //        //    var det = Session.FindObject<SalesOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //    _OnSO = det != null ? det.Quantity : 0;
        //        //}
        //        //var det = Session.FindObject<SalesOrderDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //_OnSO = det != null ? det.Quantity : 0;

        //        var data = from trans in ReqCarryoutTransactions
        //                   where trans.SourceType.Code == "JO"
        //                   select trans;
        //        if (data != null)
        //        {
        //            return data.Select(c => c.Quantity).Sum();
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //    //set { SetPropertyValue("OnSO", ref _OnSO, value); }
        //}
        [Persistent("OnJO")]
        private decimal? fOnJO = null;
        [PersistentAlias("fOnJO")]
        public decimal? OnJO
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnJO == null)
                    UpdateOnJO(false);
                return fOnJO;
            }
        }
        public void UpdateOnJO(bool forceChangeEvents)
        {
            decimal? oldOnJO = fOnJO;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "JO"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                }
            }
            catch (Exception)
            {
            }
            fOnJO = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnJO", oldOnJO, fOnJO);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal OnWO {
        //    get
        //    {
        //        //if (_LastCarrySource != null && _LastCarrySource.SourceType.Code == "WO")
        //        //{
        //        //    var det = Session.FindObject<WorkOrderItemDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //    _OnWO = det != null ? det.Quantity : 0;
        //        //}
        //        //var det = Session.FindObject<WorkOrderItemDetail>(CriteriaOperator.Parse("[RequestID]='" + _RowID + "'"));
        //        //_OnWO = det != null ? det.Quantity : 0;

        //        var data = from trans in ReqCarryoutTransactions
        //                   where trans.SourceType.Code == "WO"
        //                   select trans;
        //        if (data != null)
        //        {
        //            return data.Select(c => c.Quantity).Sum();
        //        } else
        //        {
        //            return 0;
        //        }
        //    }
        //    //set { SetPropertyValue("OnWO", ref _OnWO, value); }
        //}
        [Persistent("OnWO")]
        private decimal? fOnWO = null;
        [PersistentAlias("fOnWO")]
        public decimal? OnWO
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnWO == null)
                    UpdateOnWO(false);
                return fOnWO;
            }
        }
        public void UpdateOnWO(bool forceChangeEvents)
        {
            decimal? oldOnWO = fOnWO;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "WO"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                    _LastWO = data.LastOrDefault().TransactionId;
                }

            }
            catch (Exception)
            {
            }
            fOnWO = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnWO", oldOnWO, fOnWO);
        }

        [Persistent("OnECS")]
        private decimal? fOnECS = null;
        [PersistentAlias("fOnECS")]
        public decimal? OnECS
        {
            get
            {
                if (!IsLoading && !IsSaving && fOnECS == null)
                    UpdateOnECS(false);
                return fOnECS;
            }
        }
        public void UpdateOnECS(bool forceChangeEvents)
        {
            decimal? oldOnECS = fOnECS;
            decimal tempTotal = 0m;
            //foreach (Order detail in Orders)
            //    tempTotal += detail.Total;
            try
            {
                var data = from trans in ReqCarryoutTransactions
                           where trans.SourceType.Code == "ECS"
                           select trans;
                if (data != null && data.Count() > 0)
                {
                    tempTotal = data.Select(c => c.Quantity).Sum();
                }

            }
            catch (Exception)
            {
            }
            fOnECS = tempTotal;
            if (forceChangeEvents)
                OnChanged("OnECS", oldOnECS, fOnECS);
        }
        public bool IsNotToday {
            get
            {
                if (_GenJournalID != null && RequisitionInfo.EntryDate.Date < DateTime.Now.Date)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
        [DisplayName("Charge To")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue("CostCenter", ref _CostCenter, value);
            if (!IsLoading && !IsSaving && _CostCenter != null)
            {
                Facility = _CostCenter.Facility ?? null;
                Department = _CostCenter.Department ?? null;
            }
            }
        }

        public DateTime ExpectedDate {
            get { return _ExpectedDate; }
            set { SetPropertyValue("ExpectedDate", ref _ExpectedDate, value); }
        }
        [NonPersistent]
        public int ExpectedYear
        {
            get { return _ExpectedDate.Year; }
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //[NonPersistent]
        //public decimal CurrentQtyBase {
        //    get
        //    {
        //        if (_ItemNo != null)
        //        {
        //            _CurrentQtyBase = _ItemNo.
        //                GetOverallQtyBase(this.GenJournalID != null ? this.GenJournalID.EntryDate : DateTime.Now);
        //        }
        //        return _CurrentQtyBase;
        //    }
        //}

        [Custom("AllowEdit", "False")]
        public decimal FactorQTYBase
        {
            get {
                if (_ItemNo != null && _ItemNo.UOMRelations.Count > 0)
                {
                    UOMRelation stock = _ItemNo.UOMRelations.Where(o => o.UOM == _ItemNo.StockUOM).FirstOrDefault();
                    UOMRelation selec = _ItemNo.UOMRelations.Where(o => o.UOM == _UOM).FirstOrDefault();
                    foreach (UOMRelation
                        item in _ItemNo.UOMRelations)
                    {
                        if (selec != null && stock != null)
                        {
                            decimal dv = selec.Factor / stock.Factor;
                            _FactorQTYBase = _Quantity * dv;
                        }
                        else
                        {
                            _FactorQTYBase = _Quantity;
                        }
                        //if (stock.UOM == _UOM)
                        //{
                        //    _FactorQTYBase = _Quantity;
                        //}
                        //else
                        //{
                        //    _FactorQTYBase = _Quantity / selec.Factor;
                        //}
                    }
                }
                else
                {
                    _FactorQTYBase = _Quantity;
                }
                return _FactorQTYBase; }
        }

        [Persistent("CurrentQtyBase")]
        private decimal? _CurrentQtyBase = null;
        [PersistentAlias("_CurrentQtyBase")]
        public decimal? CurrentQtyBase
        {
            get
            {
                if (!IsLoading && !IsSaving && _CurrentQtyBase == null)
                    UpdateCurrentQtyBase(false);
                return _CurrentQtyBase;
            }
        }
        public void UpdateCurrentQtyBase(bool forceChangeEvents)
        {
            decimal? oldCurrentQtyBase = _CurrentQtyBase;
            decimal tempTotal = 0m;
            if (_ItemNo != null)
            {
                tempTotal = _ItemNo.
                    GetOverallQtyBase(this.GenJournalID != null ? this.GenJournalID.EntryDate : DateTime.Now);
            }
            _CurrentQtyBase = tempTotal;
            if (forceChangeEvents)
                OnChanged("CurrentQtyBase", oldCurrentQtyBase, _CurrentQtyBase);
        }
        [Size(500)]
        public string Reason {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }

        public bool CarryOut {
            get { return _LastCarrySource != null; }
            //set { SetPropertyValue("CarryOut", ref _CarryOut, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Action(Caption = "Mark as Completed", AutoCommit = true, TargetObjectsCriteria = "Not [Status] In ('Open', 'Completed')")]
        public void MarkAsCompleted() {
            if (ReqCarryoutTransactions.Count == 0)
            {
                throw new UserFriendlyException("You can not marked request as completed if there are no transaction was recorded");
            }
            Status = RequisitionWSStateEnum.Completed;
            Served = true;
        }

        #region Unserved Requested Quantity Task

        [Aggregated,
        Association("ReqCarryoutTransactions")]
        public XPCollection<ReqCarryoutTransaction> ReqCarryoutTransactions {
            get { return GetCollection<ReqCarryoutTransaction>(
                "ReqCarryoutTransactions"); }
        }

        #endregion

        #region Tire Concern

        [Aggregated,
        Association("RwsTireDetails")]
        public XPCollection<RwsTireDetail> RwsTireDetails {
            get { return GetCollection<RwsTireDetail>(
                "RwsTireDetails"); }
        }
        [Custom("AllowEdit", "False")]
        public decimal LastRtdOdo {
            get
            {
                var data = (from rtd in RwsTireDetails
                            select rtd).LastOrDefault();
                return data != null ? data.OdoRead : 0m;
            }
        }
        [Custom("AllowEdit", "False")]
        public TireItem LastRtdItem {
            get {
                var data = (from rtd in RwsTireDetails
                            select rtd).LastOrDefault();
                return data != null ? data.OldTireItem ?? null : null;
            }
        }
        [Custom("AllowEdit", "False")]
        public string LastRtdBranding {
            get
            {
                var data = (from rtd in RwsTireDetails
                            select rtd).LastOrDefault();
                return data != null ? data.RepTireBranding : string.Empty;
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


        public RequisitionWorksheet(Session session)
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

        protected override void OnSaving() {
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

        #region Get Current User

        private SecurityUser _CurrentUser;
        private bool _ForPO = false;
        private bool _Cancelled = false;
        private decimal _FactorQTYBase;
        private GenJournalHeader _LastPO;
        private GenJournalHeader _LastFO;
        private GenJournalHeader _LastRC;
        private GenJournalHeader _LastTO;
        private GenJournalHeader _LastWO;
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
        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }
        private void Reset()
        {
            //_CurrentQtyBase = null;
            fOnJO = null;
            fOnPO = null;
            fOnSO = null;
            fOnTO = null;
            fOnWO = null;
            fOnECS = null;
        }
        protected override void OnDeleting()
        {
            if (Status != RequisitionWSStateEnum.Open || ReqCarryoutTransactions.Count > 0)
            {
                throw new UserFriendlyException("Cannot delete already active request");
            }
            base.OnDeleting();
        }
    }
}
