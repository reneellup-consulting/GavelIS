using System;
using System.Collections.Generic;
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
    public class JobOrder : GenJournalHeader {
        private string _ReferenceNo;
        private PurchaseOrder _PurchaseOrderNo;
        private Account _DiscountAcct;
        private InvoiceTypeEnum _InvoiceType = InvoiceTypeEnum.None;
        private string _Memo;
        private string _Comments;
        private JobOrderStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Vendor _Vendor;
        private string _VendorAddress;
        private Terms _Terms;
        private string _InvoiceNo;
        private decimal _Adjusted;
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public PurchaseOrder PurchaseOrderNo {
            get { return _PurchaseOrderNo; }
            set { SetPropertyValue<PurchaseOrder>("PurchaseOrderNo", ref _PurchaseOrderNo, value); }
        }

        public Account DiscountAcct {
            get { return _DiscountAcct; }
            set { SetPropertyValue("DiscountAcct", ref _DiscountAcct, value); }
        }
        public InvoiceTypeEnum InvoiceType
        {
            get { return _InvoiceType; }
            set { SetPropertyValue("InvoiceType", ref _InvoiceType, value); }
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

        public JobOrderStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != JobOrderStatusEnum.Current)
                    {
                        Approved = true;
                    } else
                    {
                        Approved = false;
                    }
                }
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
        private List<string> _Refs;
        [Custom("AllowEdit", "False")]
        public List<string> Refs
        {
            get { return _Refs; }
            set { SetPropertyValue<List<string>>("Refs", ref _Refs, value); }
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
        [Association("Customer-JobsPayables")]
        public Vendor Vendor {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null)
                {
                    VendorAddress = _Vendor.FullAddress;
                    Terms = _Vendor.Terms;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string VendorAddress {
            get { return _VendorAddress; }
            set { SetPropertyValue("VendorAddress", ref _VendorAddress, value); }
        }

        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }

        public string InvoiceNo {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set
            {
                SetPropertyValue("Adjusted", ref _Adjusted, value);
                if (!IsLoading)
                {
                    _Vendor.UpdateBalance(true);
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
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        #region Calculated Details

        private decimal _DiscPercent;
        private decimal _Discount;
        public decimal DiscPercent {
            get { return _DiscPercent; }
            set
            {
                SetPropertyValue("DiscPercent", ref _DiscPercent, value);
                //if (!IsLoading) {
                //    decimal? oldTotal = _Total;
                //    decimal tempTotal = 0m;
                //    foreach (JobOrderDetail detail in JobOrderDetails) {
                //        tempTotal += detail.Total;}
                //    _Discount = tempTotal * (_DiscPercent / 100);
                //    OnChanged("Discount");
                //    UpdateTotal(true);
                //}
            }
        }

        [Persistent("Total")]
        private decimal? _Total;
        public decimal Discount {
            get { return _Discount; }
            set
            {
                SetPropertyValue("Discount", ref _Discount, value);
                //if (!IsLoading) {
                //    decimal? oldTotal = _Total;
                //    decimal tempTotal = 0m;
                //    foreach (JobOrderDetail detail in JobOrderDetails) {
                //        tempTotal += detail.Total;}
                //    _DiscPercent = (_Discount / tempTotal) * 100;
                //    OnChanged("DiscPercent");
                //    UpdateTotal(true);
                //}
            }
        }

        [PersistentAlias("_Total")]
        [Custom("DisplayFormat", "n")]
        public decimal? Total {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Total == null)
                    {
                        UpdateTotal(
                        false);
                    }
                } catch (Exception)
                {
                }
                return _Total;
            }
        }

        public void UpdateTotal(bool forceChangeEvent) {
            decimal? oldDiscount = _Discount;
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (JobOrderDetail detail in JobOrderDetails)
            {
                tempTotal +=
                detail.Total;
            }
            _Total = tempTotal - (tempTotal * (_DiscPercent / 100));
            if (!IsLoading)
            {
                _Discount = tempTotal * (_DiscPercent / 100);
                if (oldDiscount != _Discount)
                {
                    OnChanged("Discount");
                }
            }
            if (forceChangeEvent)
            {
                OnChanged("Total", Total, _Total);
            }
            ;
        }

        #endregion

        [PersistentAlias("Total / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfGrossPurch
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfGrossPurch");
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

        [PersistentAlias("Total / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfTaxablePurch
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfTaxablePurch");
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

        [PersistentAlias("Total / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfPurchOfSrvcs
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfPurchOfSrvcs");
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

        [PersistentAlias("AmtOfPurchOfSrvcs * (12m/100m)")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfInputTax
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfInputTax");
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

        #region Aging

        //private decimal _AmtPaid;
        private int _DaysOt;
        private decimal _ZT30Days;
        private decimal _T3T60Days;
        private decimal _T6T90Days;
        private decimal _GRT90Days;

        [PersistentAlias("Total - Adjusted")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Paid")]
        //[NonPersistent]
        public decimal AmtPaid {
            get
            {
                object tempObject = EvaluateAlias("AmtPaid");
                if (tempObject != null)
                {
                    if (_Adjusted == 0)
                    {
                        return 0;
                    } else
                    {
                        return (decimal)tempObject;
                    }
                } else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("Total - AmtPaid")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Unpaid")]
        public decimal AmtRmn {
            get
            {
                object tempObject = EvaluateAlias("AmtRmn");
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
        [NonPersistent]
        public int DaysOt {
            get
            {
                //=IF(A3<1, "",IF(A3>CURRDT,0,(A3-CURRDT)*-1))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _DaysOt = 0;
                    } else
                    {
                        TimeSpan ts = EntryDate - DateTime.Now;
                        _DaysOt = ts.Days * -1;
                    }
                } else
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
        public decimal ZT30Days {
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
                        } else
                        {
                            _ZT30Days = 0;
                        }
                    }
                } else
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
        public decimal T3T60Days {
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=60,J3>30),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _T3T60Days = 0;
                    } else
                    {
                        if (DaysOt <= 60 && DaysOt > 30)
                        {
                            _T3T60Days = AmtRmn;
                        } else
                        {
                            _T3T60Days = 0;
                        }
                    }
                } else
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
        public decimal T6T90Days {
            get
            {
                //=IF(A3>CURRDT,0,IF(AND(J3<=90,J3>60),I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _T6T90Days = 0;
                    } else
                    {
                        if (DaysOt <= 90 && _DaysOt > 60)
                        {
                            _T6T90Days = AmtRmn;
                        } else
                        {
                            _T6T90Days = 0;
                        }
                    }
                } else
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
        public decimal GRT90Days {
            get
            {
                //=IF(A3>CURRDT,0,IF(J3>=90,I3,0))
                if (AmtRmn > 0)
                {
                    if (EntryDate > DateTime.Now)
                    {
                        _GRT90Days = 0;
                    } else
                    {
                        if (DaysOt >= 90)
                        {
                            _GRT90Days = AmtRmn;
                        } else
                        {
                            _GRT90Days = 0;
                        }
                    }
                } else
                {
                    _GRT90Days = 0;
                }
                return _GRT90Days;
            }
        }

        #endregion

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month {
            get
            {
                _Month = EntryDate.Month == 1 ? MonthsEnum.January : EntryDate.Month
                 == 2 ? MonthsEnum.February : EntryDate.Month == 3 ? MonthsEnum.
                March : EntryDate.Month == 4 ? MonthsEnum.April : EntryDate.Month ==
                5 ? MonthsEnum.May : EntryDate.Month == 6 ? MonthsEnum.June :
                EntryDate.Month == 7 ? MonthsEnum.July : EntryDate.Month == 8 ?
                MonthsEnum.August : EntryDate.Month == 9 ? MonthsEnum.September
                 : EntryDate.Month == 10 ? MonthsEnum.October : EntryDate.Month == 11
                 ? MonthsEnum.November : EntryDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string Quarter {
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
        public int Year {
            get
            {
                return EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string MonthSorter {
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

        protected override void TriggerObjectChanged(ObjectChangeEventArgs args) {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
            base.TriggerObjectChanged(args);
            if (!string.IsNullOrEmpty(args.PropertyName))
            {
                if (!IsLoading)
                {
                    if (args.PropertyName == "DiscPercent")
                    {
                        decimal? oldDiscount = _Discount;
                        decimal tempTotal = 0m;
                        foreach (JobOrderDetail detail in JobOrderDetails)
                        {
                            tempTotal += detail.Total;
                        }
                        _Discount = tempTotal * (_DiscPercent / 100);
                        if (oldDiscount != _Discount)
                        {
                            OnChanged("Discount");
                            UpdateTotal(true);
                        }
                    }
                    if (args.PropertyName == "Discount")
                    {
                        decimal? oldDiscPercent = _DiscPercent;
                        decimal tempTotal = 0m;
                        foreach (JobOrderDetail detail in JobOrderDetails)
                        {
                            tempTotal += detail.Total;
                        }
                        _DiscPercent = (_Discount / tempTotal) * 100;
                        if (oldDiscPercent != _DiscPercent)
                        {
                            OnChanged("DiscPercent");
                            UpdateTotal(true);
                        }
                    }
                }
            }
        }

        public JobOrder(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "JO"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "JO"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "JO"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(Session);
            Memo = "Job Services Received";
            DiscountAcct = company.PurchaseDiscountAcct;
        }

        protected override void OnSaving() {
            //this.AutoRegisterIncomeExpenseVer();
            base.OnSaving();
        }

        protected override void OnSaved() {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }

        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Job Order transactions."
                );
            }
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _Total = null;
        }
    }
}
