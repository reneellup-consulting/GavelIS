using System;
using System.Linq;
using System.Collections;
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
    public class DriverRegistry : XPObject {
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private string _TripNo;
        private string _ReferenceNo;
        private DateTime _Date = DateTime.Now;
        private FATruck _TruckNo;
        private Employee _Driver;
        //private DriverClassification _DriverClass;
        private Tariff _Tariff;
        //private decimal _ShareRate;
        //private decimal _BaseShare;
        private decimal _PercentShare;
        //private decimal _TripShare;
        private decimal _PreOdoRead;
        private decimal _PostOdoRead;
        //private decimal _KMRunOdo;
        private decimal _KMRunMnl;
        private string _Location;
        private string _Reason;
        //private decimal _BasicPay;
        //private decimal _Commission;
        private decimal _BasicPaidActual;
        private decimal _CommissionPaidActual;
        private DriverRegistryStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private DriverPayrollBatch2 _PayrollBatchID;
        private DriverPayrollBatch3 _PayrollBatchID3;
        private bool _DolePayroll;
        private bool _Approved;


        //private TariffDriversClassifier _TariffDriverClass;
        //private decimal _Shunting;
        //private decimal _Kds;
        //private decimal _Allowance;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-DriverRegistrations")]
        public GenJournalHeader TripID {
            get { return _TripID; }
            set
            {
                SetPropertyValue("TripID", ref _TripID, value);
                if (!IsLoading && _TripID != null)
                {
                    TripNo = _TripID.SourceNo;
                    Date = _TripID.EntryDate;
                    if (_TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        ReferenceNo
                        = ((StanfilcoTrip)_TripID).DTRNo;
                    }
                    if (_TripID.GetType() == typeof(DolefilTrip))
                    {
                        ReferenceNo =
                        ((DolefilTrip)_TripID).DocumentNo;
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string TripNo {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value); }
        }

        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public FATruck TruckNo {
            get { return _TruckNo; }
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("Driver-Trips")]
        public Employee Driver {
            get { return _Driver; }
            set
            {
                SetPropertyValue("Driver", ref _Driver, value);
                //if (!IsLoading && _Driver != null)
                //{
                //    DriverClass = _Driver.
                //    DriverClassification;
                //}
            }
        }

        public string EmpStatus
        {
            get
            {
                string ret = string.Empty;
                if (_Driver != null)
                {
                    ret = _Driver.Inactive ? "INACTIVE" : "ACTIVE";
                }
                return ret;
            }
        }

        //[Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DriverClassification DriverClass
        {
            get
            {
                if (Driver != null && Driver.DriverClassification != null)
                {
                    return Driver.DriverClassification;
                }
                else
                {
                    return null;
                }
            }
            //set
            //{
            //    SetPropertyValue("DriverClass", ref _DriverClass, value);
            //    if (!IsLoading && _DriverClass != null && _Tariff != null)
            //    {
            //        if (
            //        _Tariff.TariffDriversClassifiers.Count > 0)
            //        {
            //            foreach (
            //            TariffDriversClassifier item in _Tariff.
            //            TariffDriversClassifiers)
            //            {
            //                if (item.DriverClass ==
            //                _DriverClass)
            //                {
            //                    BaseShare = item.BaseShare;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Tariff Tariff {
            get { return _Tariff; }
            set
            {
                SetPropertyValue("Tariff", ref _Tariff, value);
                //if (!IsLoading && DriverClass != null && _Tariff != null)
                //{
                //    if (_Tariff.TariffDriversClassifiers.Count > 0)
                //    {
                //        foreach (TariffDriversClassifier item in _Tariff.
                //        TariffDriversClassifiers)
                //        {
                //            if (item.DriverClass == DriverClass)
                //            {
                //                BaseShare = item.BaseShare;
                //                ShareRate = item.ShareRate;
                //            }
                //        }
                //    }
                //}
            }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public Tariff TripTariff
        {
            get
            {
                Tariff tar = null;
                if (TripID.SourceType != null && TripID.OperationType != null)
                {
                    if (TripID.SourceType.Code == "ST" && TripID.OperationType.Code == "ST")
                    {
                        tar = (TripID as StanfilcoTrip).Tariff;
                    }
                    if (TripID.SourceType.Code == "DF" && TripID.OperationType.Code == "DF")
                    {
                        tar = (TripID as DolefilTrip).Tariff;
                    }
                    if (TripID.SourceType.Code == "OT" && TripID.OperationType.Code == "OT")
                    {
                        tar = (TripID as OtherTrip).Tariff;
                    }
                }
                return tar;
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal ShareRate
        {
            get
            {
                if (DriverClass != null && _Tariff != null)
                {
                    var dat = TripTariff.TariffDriversClassifiers.Where(o => o.DriverClass == DriverClass).FirstOrDefault();
                    return dat != null ? dat.ShareRate : 0m;
                }
                else
                {
                    return 0m;
                }
            }
            //set { SetPropertyValue("ShareRate", ref _ShareRate, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BaseShare {
            get
            {
                if (DriverClass != null && _Tariff != null)
                {
                    var dat = TripTariff.TariffDriversClassifiers.Where(o => o.DriverClass == DriverClass).FirstOrDefault();
                    return dat != null ? dat.BaseShare : 0m;
                }
                else
                {
                    return 0m;
                }
            }
            //set { SetPropertyValue("BaseShare", ref _BaseShare, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal PercentShare {
            get { return _PercentShare; }
            set { SetPropertyValue("PercentShare", ref _PercentShare, value); }
        }

        [PersistentAlias("BaseShare * (PercentShare/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal TripShare {
            get
            {
                object tempObject = EvaluateAlias("TripShare");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PreOdoRead {
            get { return _PreOdoRead; }
            set { SetPropertyValue("PreOdoRead", ref _PreOdoRead, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PostOdoRead {
            get { return _PostOdoRead; }
            set { SetPropertyValue("PostOdoRead", ref _PostOdoRead, value); }
        }

        [PersistentAlias("PostOdoRead - PreOdoRead")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal KMRunOdo {
            get
            {
                object tempObject = EvaluateAlias("KMRunOdo");
                if (tempObject != null)
                {
                    if ((decimal)tempObject != 0)
                    {
                        _KMRunMnl = (decimal)tempObject;
                    }
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal KMRunMnl {
            get { return _KMRunMnl; }
            set { SetPropertyValue("KMRunMnl", ref _KMRunMnl, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public string Location {
            get { return _Location; }
            set { SetPropertyValue("Location", ref _Location, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public string Reason {
            get { return _Reason; }
            set { SetPropertyValue("Reason", ref _Reason, value); }
        }

        [PersistentAlias("TripShare * (ShareRate/100)")]
        [Custom("DisplayFormat", "n")]
        public decimal BasicPay {
            get
            {
                object tempObject = EvaluateAlias("BasicPay");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("TripShare - (TripShare * (ShareRate/100))")]
        [Custom("DisplayFormat", "n")]
        public decimal Commission {
            get
            {
                object tempObject = EvaluateAlias("Commission");
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
        public decimal BasicPaidActual {
            get { return _BasicPaidActual; }
            set { SetPropertyValue("BasicPaidActual", ref _BasicPaidActual,
                value); }
        }

        [Custom("AllowEdit", "False")]
        public bool Approved
        {
            get { return _Approved; }
            set
            {
                SetPropertyValue("Approved", ref _Approved,
                    value);
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CommissionPaidActual {
            get { return _CommissionPaidActual; }
            set { SetPropertyValue("CommissionPaidActual", ref
                _CommissionPaidActual, value); }
        }

        public DriverRegistryStatusEnum Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    switch (_Status)
                    {
                        case DriverRegistryStatusEnum.Current:
                            Approved = false;
                            break;
                        case DriverRegistryStatusEnum.Approved:
                            Approved = true;
                            break;
                        case DriverRegistryStatusEnum.Processed:
                            break;
                        case DriverRegistryStatusEnum.Paid:
                            break;
                        default:
                            break;
                    }
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public DriverPayrollBatch2 PayrollBatchID
        {
            get { return _PayrollBatchID; }
            set { SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value); }
        }
        [Custom("AllowEdit", "False")]
        public DriverPayrollBatch3 PayrollBatchID3
        {
            get { return _PayrollBatchID3; }
            set { SetPropertyValue("PayrollBatchID3", ref _PayrollBatchID3, value); }
        }

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [DisplayName("For DFP")]
        [Custom("AllowEdit", "False")]
        public bool DolePayroll
        {
            get { return _DolePayroll; }
            set { SetPropertyValue("DolePayroll", ref _DolePayroll, value); }
        }
        //[Action(AutoCommit = true, Caption = "Mark as Paid")]
        //public void MarkAsPaid()
        //{
        //    Status = DriverRegistryStatusEnum.Paid;
        //}

        //public TariffDriversClassifier TariffDriverClass {
        //    get {
        //        _TariffDriverClass = this.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse("[TariffID.Code] = '" + Tariff.Code + "' And [DriverClass.Code] = '" + DriverClass.Code + "'"));

        //        return _TariffDriverClass; }
        //}

        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //[NonPersistent]
        //public decimal Shunting {
        //    get
        //    {
        //        TariffDriversClassifier _TariffDriverClass;
        //        decimal tShunt = 0m;
        //        try
        //        {
        //            _TariffDriverClass = this.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse("[TariffID.Code] = '" + Tariff.Code + "' And [DriverClass.Code] = '" + DriverClass.Code + "'"));
        //            ICollection shunts;
        //            SortingCollection sorts = new SortingCollection(null);
        //            DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = Session.GetClassInfo(typeof(ShuntingEntry));
        //            shunts = Session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + TripID.SourceNo + "'"), sorts, 0, false, true);
        //            foreach (ShuntingEntry sh in shunts)
        //            {
        //                if (_TariffDriverClass != null)
        //                {
        //                    tShunt = tShunt + _TariffDriverClass.ShuntingShare;
        //                }
        //            }
        //        } catch (Exception)
        //        {
        //        }
        //        return tShunt;
        //    }
        //}
        [Persistent("Shunting")]
        private decimal? fShunting = null;
        [PersistentAlias("fShunting")]
        public decimal? Shunting
        {
            get
            {
                if (!IsLoading && !IsSaving && fShunting == null)
                    UpdateShunting(false);
                return fShunting;
            }
        }
        public void UpdateShunting(bool forceChangeEvents)
        {
            decimal? oldShunting = fShunting;
            decimal tempTotal = 0m;
            XPCollection<ShuntingEntry> shntentries = null;
            DriverClassification dclass = null;
            if (_TripID != null)
            {
                if (_TripID.GetType() == typeof(StanfilcoTrip))
                {
                    shntentries = (_TripID as StanfilcoTrip).ShuntingEntries;
                    dclass = (_TripID as StanfilcoTrip).Driver.DriverClassification ?? null;
                }
                if (_TripID.GetType() == typeof(DolefilTrip))
                {
                    shntentries = (_TripID as DolefilTrip).ShuntingEntries;
                    dclass = (_TripID as DolefilTrip).Driver.DriverClassification ?? null;
                }
                if (_TripID.GetType() == typeof(OtherTrip))
                {
                    shntentries = (_TripID as OtherTrip).ShuntingEntries;
                    dclass = (_TripID as OtherTrip).Driver.DriverClassification ?? null;
                }
            }
            foreach (ShuntingEntry detail in shntentries)
            {
                TariffDriversClassifier _TariffDriverClass;
                if (Tariff != null && dclass != null)
                {
                    _TariffDriverClass = this.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse("[TariffID.Code] = '" + Tariff.Code + "' And [DriverClass.Code] = '" + dclass.Code + "'"));
                    if (_TariffDriverClass != null)
                    {
                        tempTotal += _TariffDriverClass.ShuntingShare;
                    }
                }
            }
            fShunting = tempTotal;
            if (forceChangeEvents)
                OnChanged("Shunting", oldShunting, fShunting);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //[NonPersistent]
        //public decimal Kds {
        //    get
        //    {
        //        TariffDriversClassifier _TariffDriverClass;
        //        decimal tKds = 0m;
        //        try
        //        {
        //            _TariffDriverClass = this.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse("[TariffID.Code] = '" + Tariff.Code + "' And [DriverClass.Code] = '" + DriverClass.Code + "'"));
        //            ICollection kds;
        //            SortingCollection sorts = new SortingCollection(null);
        //            DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = Session.GetClassInfo(typeof(KDEntry));
        //            ;
        //            kds = Session.GetObjects(kdsClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + TripID.SourceNo + "'"), sorts, 0, false, true);
        //            foreach (KDEntry kd in kds)
        //            {
        //                tKds = tKds + _TariffDriverClass.KDShare;
        //            }
        //        } catch (Exception)
        //        {
        //        }
        //        return tKds;
        //    }
        //}
        [Persistent("Kds")]
        private decimal? fKds = null;
        [PersistentAlias("fKds")]
        public decimal? Kds
        {
            get
            {
                if (!IsLoading && !IsSaving && fKds == null)
                    UpdateKds(false);
                return fKds;
            }
        }
        public void UpdateKds(bool forceChangeEvents)
        {
            decimal? oldKds = fKds;
            decimal tempTotal = 0m;
            XPCollection<KDEntry> kdentries = null;
            DriverClassification dclass = null;
            if (_TripID != null)
            {
                if (_TripID.GetType() == typeof(StanfilcoTrip))
                {
                    kdentries = (_TripID as StanfilcoTrip).KDEntries;
                    dclass = (_TripID as StanfilcoTrip).Driver.DriverClassification ?? null;
                }
                if (_TripID.GetType() == typeof(DolefilTrip))
                {
                    kdentries = (_TripID as DolefilTrip).KDEntries;
                    dclass = (_TripID as DolefilTrip).Driver.DriverClassification ?? null;
                }
                if (_TripID.GetType() == typeof(OtherTrip))
                {
                    kdentries = (_TripID as OtherTrip).KDEntries;
                    dclass = (_TripID as OtherTrip).Driver.DriverClassification ?? null;
                }
            }
            foreach (KDEntry detail in kdentries) {
                TariffDriversClassifier _TariffDriverClass;
                if (Tariff != null && dclass != null)
                {
                    _TariffDriverClass = this.Session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse("[TariffID.Code] = '" + Tariff.Code + "' And [DriverClass.Code] = '" + dclass.Code + "'"));
                    if (_TariffDriverClass != null)
                    {
                        tempTotal += _TariffDriverClass.KDShare;
                    }
                }
            }
            fKds = tempTotal;
            if (forceChangeEvents)
                OnChanged("Kds", oldKds, fKds);
        }
        [Persistent("Allowance")]
        private decimal? fAllowance = null;
        [PersistentAlias("fAllowance")]
        public decimal? Allowance
        {
            get
            {
                if (!IsLoading && !IsSaving && fAllowance == null)
                    UpdateAllowance(false);
                return fAllowance;
            }
        }
        public void UpdateAllowance(bool forceChangeEvents)
        {
            decimal? oldAllowance = fAllowance;
            decimal tempTotal = 0m;
            if (TripID.GetType() == typeof(StanfilcoTrip))
            {
                tempTotal = ((StanfilcoTrip)TripID).Allowance;
            }
            if (TripID.GetType() == typeof(DolefilTrip))
            {
                tempTotal = ((DolefilTrip)TripID).Allowance.Value;
            }
            if (TripID.GetType() == typeof(OtherTrip))
            {
                tempTotal = ((OtherTrip)TripID).Allowance;
            }
            fAllowance = tempTotal;
            if (forceChangeEvents)
                OnChanged("Allowance", oldAllowance, fAllowance);
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //[NonPersistent]
        //public decimal Allowance {
        //    get
        //    {
        //        decimal tAllow = 0m;
        //        try
        //        {
        //            if (TripID.GetType() == typeof(StanfilcoTrip))
        //            {
        //                tAllow = ((StanfilcoTrip)TripID).Allowance;
        //            }
        //            if (TripID.GetType() == typeof(DolefilTrip))
        //            {
        //                tAllow = ((DolefilTrip)TripID).Allowance.Value;
        //            }
        //            if (TripID.GetType() == typeof(OtherTrip))
        //            {
        //                tAllow = ((OtherTrip)TripID).Allowance;
        //            }
        //        } catch (Exception)
        //        {
        //        }
        //        return tAllow;
        //    }
        //}

        //public DateTime ExactEntryDate
        //{
        //    get { return _TripID.EntryDate.Date; }
        //}

        public DateTime ExactEntryDate {
            get { return new DateTime(_TripID.EntryDate.Year, _TripID.EntryDate.Month, _TripID.EntryDate.Day, 23, 0, 0); }
        }

        public DateTime ExactEntryDateEnd {
            get { return new DateTime(_TripID.EntryDate.Year, _TripID.EntryDate.Month, _TripID.EntryDate.Day, 0, 0, 0); }
        }


        #region Calculated Partial Pay

        private decimal _CppBasic;
        private decimal _CppAdlMiscExp;
        private decimal _CppMiscExp;
        private decimal _CppKDs;
        private decimal _CppShunting;
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CppBasic {
            get { return _CppBasic; }
            set { SetPropertyValue<decimal>("CppBasic", ref _CppBasic, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CppAdlMiscExp {
            get { return _CppAdlMiscExp; }
            set { SetPropertyValue<decimal>("CppAdlMiscExp", ref _CppAdlMiscExp, value); }
        }

        [PersistentAlias("CppBasic + CppAdlMiscExp")]
        [Custom("DisplayFormat", "n")]
        public decimal TripCommission
        {
            get
            {
                object tempObject = EvaluateAlias("TripCommission");
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CppMiscExp {
            get { return _CppMiscExp; }
            set { SetPropertyValue<decimal>("CppMiscExp", ref _CppMiscExp, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CppKDs {
            get { return _CppKDs; }
            set { SetPropertyValue<decimal>("CppKDs", ref _CppKDs, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal CppShunting {
            get { return _CppShunting; }
            set { SetPropertyValue<decimal>("CppShunting", ref _CppShunting, value); }
        }

        // [PersistentAlias("CppBasic + CppAdlMiscExp + CppMiscExp + CppKDs + CppShunting")]
        [PersistentAlias("CppBasic + CppAdlMiscExp + CppKDs + CppShunting")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalPartialPay
        {
            get
            {
                object tempObject = EvaluateAlias("TotalPartialPay");
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

        #endregion

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        //[System.ComponentModel.Browsable(false)]
        [Custom("AllowEdit", "False")]
        public string CreatedBy {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        //[System.ComponentModel.Browsable(false)]
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

        public DriverRegistry(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
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

        //private SecurityUser _CurrentUser;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                SecurityUser _CurrentUser = null;
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
            fKds = null;
            fShunting = null;
            fAllowance = null;
        }
    }
}
