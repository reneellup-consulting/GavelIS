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
    public class DolefilTripDetail : XPObject {
        private Guid _RowID;
        private GenJournalHeader _TripID;
        private HaulCategory _Category;
        private HaulType _Type;
        private FATrailer _TrailerNo;
        private decimal _Distance;
        private decimal _Allowance;
        private DateTime _Date;
        private DateTime _Start;
        private DateTime _Finish;
        private decimal _StartOdo;
        private decimal _FinishOdo;
        private decimal _DistanceOdo;
        private decimal _Quantity = 1;
        private decimal _Rate;
        private decimal _Amount;
        private decimal _Commision;
        private decimal _ActualCommisionPaid;
        private string _Remarks;
        private DriverRegistryStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private decimal _CommCount;
        private DriverPayrollTripLine3 _Dptl3Id;
        private DriverPayroll3 _Dprl3Id;
        private DriverRegistry _DriverRegistryId;
        private bool _Reconciled = false;
        private bool _Paid = false;
        private decimal _PaidAmount;
        private DolefilTripPaymentsRecon _ReconId;

        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-DolefilTripDetails")]
        public GenJournalHeader TripID {
            get { return _TripID; }
            set {
                SetPropertyValue("TripID", ref _TripID, value);
                if (!IsLoading && _TripID != null) {
                    Date = _TripID.EntryDate;
                    Distance = ((DolefilTrip)_TripID).Distance;
                }
            }
        }
        public string ReferenceNo
        {
            get { return _TripID != null ? ((DolefilTrip)_TripID).ReferenceNo : null; }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public HaulCategory Category {
            get { return _Category; }
            set {
                SetPropertyValue("Category", ref _Category, value);
                if (!IsLoading && _Category != null) {Rate = _Category.Rate;}
                if (!IsLoading) {
                    try {
                        ((DolefilTrip)_TripID).UpdateAmountTruck(true);
                        ((DolefilTrip)_TripID).UpdateTrailerRental(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        public FATrailer TrailerNo {
            get { return _TrailerNo; }
            set { SetPropertyValue("TrailerNo", ref _TrailerNo, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Distance {
            get { return _Distance; }
            set {
                SetPropertyValue("Distance", ref _Distance, value);
                if (!IsLoading) {
                    try {
                        ((DolefilTrip)_TripID).UpdateActualRun(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Allowance {
            get { return _Allowance; }
            set {
                SetPropertyValue("Allowance", ref _Allowance, value);
                if (!IsLoading) {
                    try {
                        ((DolefilTrip)_TripID).UpdateAllowance(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        //[Custom("DisplayFormat", "n")]
        //public decimal Commision
        //{
        //    get { return _Commision; }
        //    set
        //    {
        //        SetPropertyValue("Commision", ref _Commision, value);
        //    }
        //}

        public DriverRegistryStatusEnum Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }

        public string StatusBy
        {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate
        {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        public DriverRegistry DriverRegistryId
        {
            get { return _DriverRegistryId; }
            set { SetPropertyValue("DriverRegistryId", ref _DriverRegistryId, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal ActualCommisionPaid
        {
            get { return _ActualCommisionPaid; }
            set
            {
                SetPropertyValue("ActualCommisionPaid", ref _ActualCommisionPaid, value);
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public HaulType Type {
            get { return _Type; }
            set { SetPropertyValue("Type", ref _Type, value); }
        }
        public DateTime Date {
            get { return _Date; }
            set {
                SetPropertyValue("Date", ref _Date, value);
                if (!IsLoading) {
                    Start = _Date;
                    Finish = _Date;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public DolefilTripPaymentsRecon ReconId
        {
            get { return _ReconId; }
            set
            {
                SetPropertyValue("ReconId", ref _ReconId, value);
            }
        }
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public decimal CommCount
        {
            get
            {
                if (!IsLoading && !IsSaving && _Category != null)
                {
                    switch (_Category.CommPayType)
                    {
                        case CommPayTypeEnum.Regular:
                            if (_TripID != null && _TripID.DolefilTripDetails.Count == 1)
                            {
                                return 1m;
                            }
                            //else if (_TripID != null && _TripID.DolefilTripDetails.Count == 3)
                            //{
                            //    //decimal c = 0m;
                            //    //foreach (var item in _TripID.DolefilTripDetails)
                            //    //{
                            //    //    c = c + item.CommCount;
                            //    //}
                            //    //var result = c - Math.Truncate(c);
                            //    //if (result == 0.5m)
                            //    //{
                            //    //    return 0m;
                            //    //}
                            //    return 0m;
                            //}
                            return 0.5m;
                        case CommPayTypeEnum.Knockdown:
                            if (_TripID != null && _TripID.DolefilTripDetails.Count == 2)
                            {
                                return 0.5m;
                            }
                            return 1m;
                        case CommPayTypeEnum.Fulls:
                            return 1m;
                        case CommPayTypeEnum.Empty:
                            return 1m;
                        case CommPayTypeEnum.Whole:
                            return 1m;
                        default:
                            break;
                    }
                }
                return 0;
            }
        }
        public string CommRoute
        {
            get
            {
                if (!IsLoading && !IsSaving && _Category != null)
                {
                    switch (_Category.CommPayType)
                    {
                        case CommPayTypeEnum.Regular:
                            return _Category.OrigRoute;
                        case CommPayTypeEnum.Knockdown:
                            if (_TripID != null && _TripID.DolefilTripDetails.Count == 2)
                            {
                                foreach (var item in _TripID.DolefilTripDetails)
                                {
                                    if (item.Category.CommPayType != CommPayTypeEnum.Knockdown)
                                    {
                                        return item.Category.OrigRoute;
                                    }
                                }
                            }
                            return _Category.OrigRoute;
                        case CommPayTypeEnum.Fulls:
                            return _Category.OrigRoute;
                        case CommPayTypeEnum.Empty:
                            return _Category.OrigRoute;
                        case CommPayTypeEnum.Whole:
                            return _Category.OrigRoute;
                        default:
                            break;
                    }
                }
                return string.Empty;
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Commission
        {
            get
            {
                if (!IsLoading && !IsSaving && _Category != null)
                {
                    switch (_Category.CommPayType)
                    {
                        case CommPayTypeEnum.Regular:
                            return _Category.Commission * CommCount;
                        case CommPayTypeEnum.Knockdown:
                            if (_TripID != null && _TripID.DolefilTripDetails.Count == 2)
                            {
                                foreach (var item in _TripID.DolefilTripDetails)
                                {
                                    if (item.Category.CommPayType != CommPayTypeEnum.Knockdown)
                                    {
                                        return item.Category.Commission * CommCount;
                                    }
                                }
                            }
                            return _Category.Commission * CommCount;
                        case CommPayTypeEnum.Fulls:
                            return _Category.Commission * CommCount;
                        case CommPayTypeEnum.Empty:
                            return _Category.Commission * CommCount;
                        case CommPayTypeEnum.Whole:
                            return _Category.Commission * CommCount;
                        default:
                            break;
                    }
                }
                return 0;
            }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [Custom("EditMask", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor2")]
        public DateTime Start {
            get { return _Start; }
            set { SetPropertyValue("Start", ref _Start, value); }
        }
        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [Custom("EditMask", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor2")]
        public DateTime Finish {
            get { return _Finish; }
            set { SetPropertyValue("Finish", ref _Finish, value); }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal StartOdo {
            get { return _StartOdo; }
            set { SetPropertyValue("StartOdo", ref _StartOdo, value); }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal FinishOdo {
            get { return _FinishOdo; }
            set { SetPropertyValue("FinishOdo", ref _FinishOdo, value); }
        }
        [PersistentAlias("FinishOdo - StartOdo")]
        public decimal DistanceOdo {
            get {
                object tempObject = EvaluateAlias("DistanceOdo");
                if (tempObject != null) {
                    if ((decimal)tempObject != 0) {Distance = (decimal)
                        tempObject > 0 ? (decimal)tempObject : 0;}
                    return (decimal)tempObject > 0 ? (decimal)tempObject : 0;
                } else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Quantity {
            get { return _Quantity; }
            set {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading) {
                    try {
                        ((DolefilTrip)_TripID).UpdateAmountTruck(true);
                        ((DolefilTrip)_TripID).UpdateTrailerRental(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Rate {
            get { return _Rate; }
            set {
                SetPropertyValue("Rate", ref _Rate, value);
                if (!IsLoading) {
                    try {
                        ((DolefilTrip)_TripID).UpdateAmountTruck(true);
                        ((DolefilTrip)_TripID).UpdateTrailerRental(true);
                    } catch (Exception) {
                    }
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Quantity * Rate")]
        public decimal Amount {
            get {
                object tempObject = EvaluateAlias("Amount");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }

        [Size(500)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }

        //public DriverPayrollTripLine3 Dptl3Id { get; set; }
        [Association]
        public DriverPayrollTripLine3 Dptl3Id
        {
            get { return _Dptl3Id; }
            set { SetPropertyValue("Dptl3Id", ref _Dptl3Id, value); }
        }

        [Association]
        public DriverPayroll3 Dprl3Id
        {
            get { return _Dprl3Id; }
            set { SetPropertyValue("Dprl3Id", ref _Dprl3Id, value); }
        }
        //[Custom("AllowEdit", "False")]
        public bool Reconciled
        {
            get
            {
                if (_ReconId != null)
                {
                    return true;
                }
                return false;
            }
            //set { SetPropertyValue("Reconciled", ref _Reconciled, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Paid
        {
            get { return _Paid; }
            set { SetPropertyValue("Paid", ref _Paid, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal PaidAmount
        {
            get { return _PaidAmount; }
            set { SetPropertyValue("PaidAmount", ref _PaidAmount, value); }
        }
        // Overpayment
        [Custom("DisplayFormat", "n")]
        [DisplayName("Over Payment")]
        [PersistentAlias("Rate - PaidAmount")]
        public decimal Overpayment
        {
            get
            {
                object tempObject = EvaluateAlias("Overpayment");
                if (tempObject != null)
                {
                    if (_PaidAmount != 0)
                    {
                        return (decimal)tempObject < 0 ? Math.Abs((decimal)tempObject) : 0;
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }
            }
        }
        // Underpayment
        [Custom("DisplayFormat", "n")]
        [DisplayName("Under Payment")]
        [PersistentAlias("Rate - PaidAmount")]
        public decimal Underpayment
        {
            get
            {
                object tempObject = EvaluateAlias("Underpayment");
                if (tempObject != null)
                {
                    if (_PaidAmount != 0)
                    {
                        return (decimal)tempObject > 0 ? Math.Abs((decimal)tempObject) : 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        [Size(1000)]
        public string FuelReferences
        {
            get
            {
                if (_TripID != null)
                {
                    return (_TripID as DolefilTrip).FuelReferences;
                }
                return string.Empty;
            }
            //set { SetPropertyValue("Reconciled", ref _Reconciled, value); }
        }

        public string EmpStatus
        {
            get
            {
                string ret = string.Empty;
                if (_TripID != null && _TripID.TripDriver != null)
                {
                    ret = _TripID.TripDriver.Inactive ? "INACTIVE" : "ACTIVE";
                }
                return ret;
            }
        }

        public DateTime ExactEntryDate
        {
            get { return new DateTime(_Date.Year, _Date.Month, _Date.Day, 23, 0, 0); }
        }

        public DateTime ExactEntryDateEnd
        {
            get { return new DateTime(_Date.Year, _Date.Month, _Date.Day, 0, 0, 0); }
        }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        //[System.ComponentModel.Browsable(false)]
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
        public DolefilTripDetail(Session session): base(session) {
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
