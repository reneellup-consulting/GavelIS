using System;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class Bill : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private BillStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Vendor _Vendor;
        private Terms _Terms;
        private DateTime _DueDate;
        private decimal _Adjusted;
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        public BillStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != BillStatusEnum.Current) {Approved = true;} 
                    else {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null) {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor {
            get { return _Vendor; }
            set {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null) {Terms = _Vendor.Terms;}
            }
        }
        public Terms Terms {
            get { return _Terms; }
            set {
                SetPropertyValue("Terms", ref _Terms, value);
                if (!IsLoading && _Terms != null) {
                    if (_Terms.DaysToPay > 0) {DueDate = EntryDate.Add(new 
                        TimeSpan(_Terms.DaysToPay, 0, 0, 0));} else {
                        DueDate = DateTime.MinValue;
                    }
                }
                if (!IsLoading && _Terms == null) {DueDate = DateTime.MinValue;}
            }
        }
        public DateTime DueDate {
            get { return _DueDate; }
            set { SetPropertyValue("DueDate", ref _DueDate, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set { SetPropertyValue("Adjusted", ref _Adjusted, value);
            if (!IsLoading)
            {
                APRegistry apr =
                    Session.FindObject<APRegistry>(CriteriaOperator
                    .Parse("[GenJournalID.SourceNo] = '" + SourceNo + "'"));
                if (apr != null)
                {
                    apr.AmtPaid = _Adjusted;
                    apr.Save();
                }
            }
            }
        }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
        #region Calculated Details
        [Persistent("Total")]
        private decimal? _Total;
        [PersistentAlias("_Total")]
        [Custom("DisplayFormat", "n")]
        public decimal? Total {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Total == null) {UpdateTotal(
                        false);}
                } catch (Exception) {}
                return _Total;
            }
        }
        public void UpdateTotal(bool forceChangeEvent) {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (BillDetail detail in BillDetails) {tempTotal += detail.
                Amount;}
            _Total = tempTotal;
            if (forceChangeEvent) {OnChanged("Total", Total, _Total);}
            ;
        }
        #endregion
        #region Aging

        //private decimal _AmtPaid;
        private int _DaysOt;
        private decimal _ZT30Days;
        private decimal _T3T60Days;
        private decimal _T6T90Days;
        private decimal _GRT90Days;

        //[PersistentAlias("Total - Adjusted")]
        [PersistentAlias("Adjusted")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Paid")]
        //[NonPersistent]
        public decimal AmtPaid
        {
            get
            {
                object tempObject = EvaluateAlias("AmtPaid");
                if (tempObject != null)
                {
                    if (_Adjusted == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        return (decimal)tempObject;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        //[PersistentAlias("Total - AmtPaid")]
        [PersistentAlias("Total - Adjusted")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Unpaid")]
        public decimal AmtRmn
        {
            get
            {
                object tempObject = EvaluateAlias("AmtRmn");
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
        [NonPersistent]
        public int DaysOt
        {
            get
            {
                //=IF(A3<1, "",IF(A3>CURRDT,0,(A3-CURRDT)*-1))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _DaysOt = 0;
                    }
                    else
                    {
                        TimeSpan ts = EntryDate - DateTime.Now;
                        _DaysOt = ts.Days * -1;
                    }
                }
                else
                {
                    _DaysOt = 0;
                }
                return _DaysOt;
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName("0-30 Days")]
        public decimal ZT30Days
        {
            get
            {
                //=IF(A3<CURRDT,IF(J3<=30,I3,0),)
                if (AmtRmn > 0)
                {
                    if (EntryDate < DateTime.Now)
                    {
                        if (DaysOt <= 30)
                        {
                            _ZT30Days = AmtRmn;
                        }
                        else
                        {
                            _ZT30Days = 0;
                        }
                    }
                }
                else
                {
                    _ZT30Days = 0;
                }
                return _ZT30Days;
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName("30-60 Days")]
        public decimal T3T60Days
        {
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=60,J3>30),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _T3T60Days = 0;
                    }
                    else
                    {
                        if (DaysOt <= 60 && DaysOt > 30)
                        {
                            _T3T60Days = AmtRmn;
                        }
                        else
                        {
                            _T3T60Days = 0;
                        }
                    }
                }
                else
                {
                    _T3T60Days = 0;
                }
                return _T3T60Days;
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName("60-90 Days")]
        public decimal T6T90Days
        {
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=90,J3>60),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _T6T90Days = 0;
                    }
                    else
                    {
                        if (DaysOt <= 90 && _DaysOt > 60)
                        {
                            _T6T90Days = AmtRmn;
                        }
                        else
                        {
                            _T6T90Days = 0;
                        }
                    }
                }
                else
                {
                    _T6T90Days = 0;
                }
                return _T6T90Days;
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        [DisplayName(">90 Days")]
        public decimal GRT90Days
        {
            get
            {
                //=IF(A3>CURRDT,0,IF(J3>=90,I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _GRT90Days = 0;
                    }
                    else
                    {
                        if (DaysOt >= 90)
                        {
                            _GRT90Days = AmtRmn;
                        }
                        else
                        {
                            _GRT90Days = 0;
                        }
                    }
                }
                else
                {
                    _GRT90Days = 0;
                }
                return _GRT90Days;
            }
        }

        #endregion

        public Bill(Session session): base(session) {
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "BE"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "BE"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "BE"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
        }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            //this.IsIncExpNeedUpdate = true;
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            base.TriggerObjectChanged(args);
        }

        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Bill transactions."
                );} }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() { _Total = null; }

        protected override void OnSaving()
        {
            base.OnSaving();
        }

        protected override void OnSaved()
        {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }
    }
}
