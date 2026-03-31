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
    public class Invoice : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private InvoiceTypeEnum _InvoiceType;
        private InvoiceStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Customer _Customer;
        private string _CustomerAddress;
        private string _ShipToAddress;
        private Terms _Terms;
        private ShipVia _ShipVia;
        private DateTime _DueDate;
        private DateTime _DiscountDate;
        private decimal _DiscountRate;
        private string _PONumber;
        private SalesOrder _SONumber;
        private decimal _OpenAmount;
        private decimal _RemainingTax;
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

        public InvoiceTypeEnum InvoiceType {
            get { return _InvoiceType; }
            set
            {
                SetPropertyValue("InvoiceType", ref _InvoiceType, value);
                if (!IsLoading)
                {
                    if (_InvoiceType == InvoiceTypeEnum.Cash)
                    {
                        Terms = (Company.
                        GetInstance(Session)).DefaultCashTerm != null ? (Company
                        .GetInstance(Session)).DefaultCashTerm : null;
                    } else
                    {
                        if (_Customer != null)
                        {
                            Terms = _Customer.Terms != null
                             ? _Customer.Terms : null;
                        }
                    }
                }
            }
        }

        public InvoiceStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != InvoiceStatusEnum.Current)
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

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("Customer-IHReceivables")]
        public Customer Customer {
            get { return _Customer; }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
                if (!IsLoading && _Customer != null)
                {
                    CustomerAddress = _Customer.FullAddress;
                    ShipToAddress = _Customer.FullShipAddress;
                    this.ShipVia = _Customer.ShipVia != null ? _Customer.ShipVia
                     : null;
                    if (_InvoiceType == InvoiceTypeEnum.Cash)
                    {
                        Terms = (Company.
                        GetInstance(Session)).DefaultCashTerm != null ? (Company
                        .GetInstance(Session)).DefaultCashTerm : null;
                    } else
                    {
                        Terms = _Customer.Terms != null ? _Customer.Terms : null
                        ;
                    }
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string CustomerAddress {
            get { return _CustomerAddress; }
            set { SetPropertyValue("CustomerAddress", ref _CustomerAddress,
                value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string ShipToAddress {
            get { return _ShipToAddress; }
            set { SetPropertyValue("ShipToAddress", ref _ShipToAddress, value); }
        }

        //public Terms Terms {
        //    get { return _Terms; }
        //    set {
        //        SetPropertyValue("Terms", ref _Terms, value);
        //        if (!IsLoading && _Terms != null) {
        //            DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0, 0, 
        //            0));
        //            DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
        //            EarlyDaysToPay, 0, 0, 0));
        //            DiscountRate = _Terms.EarlyDiscount;
        //        }
        //    }
        //}
        public Terms Terms {
            get { return _Terms; }
            set
            {
                SetPropertyValue("Terms", ref _Terms, value);
                if (!IsLoading && _Terms != null)
                {
                    if (_Terms.DaysToPay > 0)
                    {
                        DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0
                        , 0, 0));
                        DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
                        EarlyDaysToPay, 0, 0, 0));
                        DiscountRate = _Terms.EarlyDiscount;
                    } else
                    {
                        DueDate = DateTime.MinValue;
                    }
                }
                if (!IsLoading && _Terms == null)
                {
                    DueDate = DateTime.MinValue;
                }
            }
        }

        public ShipVia ShipVia {
            get { return _ShipVia; }
            set { SetPropertyValue("ShipVia", ref _ShipVia, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DueDate {
            get { return _DueDate; }
            set { SetPropertyValue("DueDate", ref _DueDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DiscountDate {
            get { return _DiscountDate; }
            set { SetPropertyValue("DiscountDate", ref _DiscountDate, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal DiscountRate {
            get { return _DiscountRate; }
            set { SetPropertyValue("DiscountRate", ref _DiscountRate, value); }
        }

        public string PONumber {
            get { return _PONumber; }
            set { SetPropertyValue("PONumber", ref _PONumber, value); }
        }

        [Custom("AllowEdit", "False")]
        public SalesOrder SONumber {
            get { return _SONumber; }
            set { SetPropertyValue("SONumber", ref _SONumber, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get
            { //return _OpenAmount==0?_GrossTotal==null?0:_GrossTotal.Value:_OpenAmount; 
                return _OpenAmount;
            }
            set
            {
                SetPropertyValue("OpenAmount", ref _OpenAmount, value);
                if (!IsLoading)
                {
                    _Customer.UpdateBalance(true);
                    ARRegistry arr =
                    Session.FindObject<ARRegistry>(CriteriaOperator
                    .Parse("[GenJournalID.SourceNo] = '" + SourceNo + "'"));
                    if (arr != null)
                    {
                        arr.AmtPaid = GrossTotal.Value - _OpenAmount;
                        arr.Save();
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal RemainingTax {
            get { return _RemainingTax; }
            set { SetPropertyValue("RemainingTax", ref _RemainingTax, value); }
        }

        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public bool RequireTenderPayment
        {
            get { return _RequireTenderPayment; }
            set { SetPropertyValue("RequireTenderPayment", ref _RequireTenderPayment, value); }
        }

        #region Calculated Details

        [Persistent("Net")]
        private decimal? _Net;
        [Persistent("TotalTax")]
        private decimal? _TotalTax;
        [Persistent("GrossTotal")]
        private decimal? _GrossTotal;
        [Persistent("CashPayment")]
        private decimal? _CashPayment;
        [PersistentAlias("_Net")]
        [Custom("DisplayFormat", "n")]
        public decimal? Net {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Net == null)
                    {
                        UpdateNet(
                        false);
                    }
                } catch (Exception)
                {
                }
                return _Net;
            }
        }

        [PersistentAlias("_TotalTax")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalTax {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalTax == null)
                    {
                        UpdateTotalTax(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalTax;
            }
        }

        [PersistentAlias("_GrossTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal? GrossTotal {
            get
            {
                try
                {
                    //if (!IsLoading)
                    //{
                    //    _Customer.UpdateBalance(true);
                    //}
                    if (!IsLoading && !IsSaving && _GrossTotal == null)
                    {
                        UpdateGrossTotal(false);
                    }
                } catch (Exception)
                {
                }
                return _GrossTotal;
            }
        }

        [PersistentAlias("GrossTotal / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfGrossSales
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfGrossSales");
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

        [PersistentAlias("_CashPayment")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Cash/Check Payments")]
        public decimal? CashPayment
        {
            get
            {
                try
                {
                    //if (!IsLoading)
                    //{
                    //    _Customer.UpdateBalance(true);
                    //}
                    if (!IsLoading && !IsSaving && _CashPayment == null)
                    {
                        UpdateCashPayments(false);
                    }
                }
                catch (Exception)
                {
                }
                return _CashPayment;
            }
        }
        [PersistentAlias("GrossTotal / 1.12m")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfTaxableSales
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfTaxableSales");
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

        [PersistentAlias("AmtOfTaxableSales * (12m/100m)")]
        [Custom("DisplayFormat", "n")]
        public decimal AmtOfOutputTax
        {
            get
            {
                object tempObject = EvaluateAlias("AmtOfOutputTax");
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
        public void UpdateNet(bool forceChangeEvent) {
            decimal? oldNet = _Net;
            decimal tempTotal = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
            {
                if (detail.Tax !=
                null && detail.Tax.Taxable)
                {
                    tempTotal += detail.Total / (1 + (
                    detail.Tax.Rate / 100));
                }
            }
            _Net = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Net", Net, _Net);
            }
            ;
        }

        public void UpdateTotalTax(bool forceChangeEvent) {
            decimal? oldTotalTax = _TotalTax;
            decimal tempTotal = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
            {
                if (detail.Tax !=
                null && detail.Tax.Taxable)
                {
                    tempTotal += detail.Total - (detail
                    .Total / (1 + (detail.Tax.Rate / 100)));
                }
            }
            _TotalTax = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalTax", TotalTax, _TotalTax);
            }
            ;
        }

        public void UpdateGrossTotal(bool forceChangeEvent) {
            decimal? oldGrossTotal = _GrossTotal;
            decimal tempTotal = 0m;
            foreach (InvoiceDetail detail in InvoiceDetails)
            {
                tempTotal +=
                detail.Total;
            }
            _GrossTotal = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("GrossTotal", GrossTotal,
                _GrossTotal);
            }
            ;
        }

        public void UpdateCashPayments(bool forceChangeEvent)
        {
            decimal? oldCashPayment = _CashPayment;
            decimal tempTotal = 0m;
            foreach (PaymentsApplied detail in PaymentsApplied)
            {
                tempTotal +=
                detail.Amount;
            }
            _CashPayment = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("CashPayment", CashPayment,
                _CashPayment);
            }
            ;
        }
        public decimal CreditableAmount {
            get { return _Customer.CreditableAmount.Value; }
        }

        #endregion

        [NonPersistent]
        public Company CompanyInfo {
            get { return Company.GetInstance(Session); }
        }

        #region Aging

        //private decimal _AmtPaid;
        private int _DaysOt;
        private decimal _ZT30Days;
        private decimal _T3T60Days;
        private decimal _T6T90Days;
        private decimal _GRT90Days;

        [PersistentAlias("GrossTotal - OpenAmount")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Paid")]
        //[NonPersistent]
        public decimal AmtPaid {
            get
            {
                object tempObject = EvaluateAlias("AmtPaid");
                if (tempObject != null)
                {
                    if (_OpenAmount == 0 && _InvoiceType != InvoiceTypeEnum.Cash)
                    {
                        return 0;
                    } else if (_OpenAmount == 0 && _InvoiceType == InvoiceTypeEnum.Cash)
                    {
                        return _GrossTotal.Value;
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

        [PersistentAlias("GrossTotal - AmtPaid")]
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
        private bool _RequireTenderPayment = true;
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

        public Invoice(Session session)
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
            "Code", "IN"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "IN"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "IN"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Memo = "Invoice #" + SourceNo;
        }

        protected override void OnSaving()
        {
            //this.AutoRegisterIncomeExpenseVer();
            this.UpdateGrossTotal(false);
            this.UpdateCashPayments(false);
            //if (_RequireTenderPayment && _InvoiceType == InvoiceTypeEnum.Cash && _GrossTotal.Value != _CashPayment.Value)
            //{
            //    throw new UserFriendlyException("Cash/Check Payments must be equal to Gross Total.");
            //}
            base.OnSaving();
        }
        protected override void OnSaved()
        {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }
        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Invoice transactions."
                );
            }
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _TotalTax = null;
            _GrossTotal = null;
            _CashPayment = null;
        }

        protected override void TriggerObjectChanged(ObjectChangeEventArgs args) {
            //this.IsIncExpNeedUpdate = true;
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            base.TriggerObjectChanged(args);
            if (args.PropertyName == "EntryDate")
            {
                if (!IsLoading && _Terms != null)
                {
                    DueDate = EntryDate.Add(new TimeSpan(_Terms.DaysToPay, 0, 0,
                    0));
                    DiscountDate = EntryDate.Add(new TimeSpan(_Terms.
                    EarlyDaysToPay, 0, 0, 0));
                    DiscountRate = _Terms.EarlyDiscount;
                }
            }
        }
    }
}
