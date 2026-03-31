using System;
using System.Linq;
using System.Text;
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

namespace GAVELISv2.Module.BusinessObjects
{
    public enum FuelPumpRegisterStatusEnum
    {
        Current, // newly created entry
        //Invoiced, // for cash sale and charge sale
        Charged // charge to employee or the company
        //Applied // after an employee or the company successfully charge
        //[DisplayName("Partially Paid")]
        //PartiallyPaid, // for charge sale
        //Paid, // for charge sale
        //[DisplayName("Partially Returned")]
        //PartiallyReturned, // returned to excess - if fuel is dirty -> for washing only - create fuel item as Excess
        //Returned // returned to excess - if fuel is dirty -> for washing only - create fuel item as Excess
    }
    public enum FuelPumpRegisterTypeEnum
    {
        [DisplayName("Charge to Company")]
        ChargeToCompany,// charge to fleet expense of the company
        //[DisplayName("Cash Sale")]
        //CashSale, // sold to a customer
        //[DisplayName("Charge Sale")]
        //ChargeSale, // charge to a customer account
        [DisplayName("Charge to Employe")]
        ChargeToEmployee // charge to employee -> auto create charge slip
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class FuelPumpRegister : GenJournalHeader, ISetIncomeExpense
    {
        private Vendor _Vendor;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor
        {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
            }
        }
        private string _Memo;
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo
        {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        private string _Comments;
        [Size(SizeAttribute.Unlimited)]
        public string Comments
        {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        private FuelPumpRegisterTypeEnum _TransactionType;
        public FuelPumpRegisterTypeEnum TransactionType
        {
            get { return _TransactionType; }
            set { SetPropertyValue("TransactionType", ref _TransactionType, value); }
        }
        private FuelPumpRegisterStatusEnum _Status;
        public FuelPumpRegisterStatusEnum Status
        {
            get { return _Status; }
            set { SetPropertyValue("Status", ref _Status, value); }
        }
        private string _StatusBy;
        public string StatusBy
        {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value);
            if (!IsLoading)
            {
                if (_Status != FuelPumpRegisterStatusEnum.Current)
                {
                    Approved = true
                    ;
                }
                else
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
        private DateTime _StatusDate;
        public DateTime StatusDate
        {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        private PurchaseOrder _PurchaseOrderRef;
        [DisplayName("PO No.")]
        [Custom("AllowEdit", "False")]
        public PurchaseOrder PurchaseOrderRef
        {
            get { return _PurchaseOrderRef; }
            set { SetPropertyValue("PurchaseOrderRef", ref _PurchaseOrderRef, value); }
        }
        private string _FuelRequestRef;
        [DisplayName("FRS No.")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string FuelRequestRef
        {
            get { return _FuelRequestRef; }
            set { SetPropertyValue("FuelRequestRef", ref _FuelRequestRef, value); }
        }
        [DisplayName("Invoice No.")]
        private string _InvoiceRef;
        public string InvoiceRef
        {
            get { return _InvoiceRef; }
            set { SetPropertyValue("InvoiceRef", ref _InvoiceRef, value); }
        }
        private string _FuelWithdrawalRef;
        [DisplayName("FWS No.")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string FuelWithdrawalRef
        {
            get { return _FuelWithdrawalRef; }
            set { SetPropertyValue("FuelWithdrawalRef", ref _FuelWithdrawalRef, value); }
        }

        private string _Purpose;
        [Size(SizeAttribute.Unlimited)]
        public string Purpose
        {
            get { return _Purpose; }
            set { SetPropertyValue("Purpose", ref _Purpose, value); }
        }
        private CostCenter _ChargeTo;
        [RuleRequiredField("", DefaultContexts.Save)]
        public CostCenter ChargeTo
        {
            get { return _ChargeTo; }
            set { SetPropertyValue("ChargeTo", ref _ChargeTo, value); }
        }
        private FixedAsset _Unit;
        [RuleRequiredField("", DefaultContexts.Save)]
        public FixedAsset Unit
        {
            get { return _Unit; }
            set { SetPropertyValue("Unit", ref _Unit, value);
            if (!IsLoading)
            {
                if (_Unit != null)
                {
                    Requestor = _Unit.DriverOperator ?? null;
                    GetLastGasUp(_Unit);
                }
                else
                {
                    Requestor = null;
                }
            }
            }
        }
        [Aggregated,
        Association("FuelpumpOdoLogs")]
        public XPCollection<FuelOdoRegistry> ReceiptFuelOdoLogs
        {
            get
            {
                return GetCollection<FuelOdoRegistry>("ReceiptFuelOdoLogs"
                    );
            }
        }
        public override DateTime EntryDate
        {
            get
            {
                return base.EntryDate;
            }
            set
            {
                base.EntryDate = value;
                if (!IsLoading)
                {
                    if (_Unit != null)
                    {
                        GetLastGasUp(_Unit);
                    }
                    else
                    {
                        PrevOdoRead = 0;
                        OdoRead = 0;
                    }
                }
            }
        }
        [Action(Caption = "Get Previous Reading", AutoCommit = true)]
        public void GetPreviousReading()
        {
            if (_Unit != null)
            {
                GetLastGasUp(_Unit);
            }
            else
            {
                OdoRead = 0m;
                PrevOdoRead = 0m;
            }
        }
        // Get Last Gas Up
        #region New Previous Gasup Details
        private void GetLastGasUp(FixedAsset asset)
        {
            if (asset != null)
            {
                //FuelOdoRegistry fuelLog = null;
                if (EntryDate != DateTime.MinValue)
                {
                    string seq = EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, 999999)
                       : string.Empty;
                    decimal toDecimal = Convert.ToDecimal(seq);
                    if (asset.GetType() == typeof(FATruck))
                    {
                        FATruck faTruck = asset as FATruck;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.FuelPumpRegId != this && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            OdoRead = data.Reading;
                            PrevOdoRead = data.Reading;
                        }
                    }
                    else if (asset.GetType() == typeof(FAGeneratorSet))
                    {
                        FAGeneratorSet faTruck = asset as FAGeneratorSet;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.FuelPumpRegId != this && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevOdoRead = data.Reading;
                        }
                    }
                    else if (asset.GetType() == typeof(FAOtherVehicle))
                    {
                        FAOtherVehicle faTruck = asset as FAOtherVehicle;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.FuelPumpRegId != this && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            OdoRead = data.Reading;
                            PrevOdoRead = data.Reading;
                        }
                    }
                }
            }
        }
        #endregion
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoRead
        {
            get { return _OdoRead; }
            set { SetPropertyValue("OdoRead", ref _OdoRead, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevOdoRead
        {
            get { return _PrevOdoRead; }
            set { SetPropertyValue("PrevOdoRead", ref _PrevOdoRead, value); }
        }
        private Employee _Requestor;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Requestor
        {
            get { return _Requestor; }
            set { SetPropertyValue("Requestor", ref _Requestor, value); }
        }
        private Facility _Facility;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Facility Facility
        {
            get { return _Facility; }
            set { SetPropertyValue("Facility", ref _Facility, value);
            if (!IsLoading)
            {
                FacilityHead = null;
                DepartmentInCharge = null;
            }
            }
        }
        private Department _Department;
        [DataSourceProperty("Facility.Departments")]
        public Department Department
        {
            get { return _Department; }
            set { SetPropertyValue("Department", ref _Department, value);
            if (!IsLoading)
            {
                if (_Department != null)
                {
                    FacilityHead = _Department.DepartmentHead ?? null;
                    DepartmentInCharge = _Department.InCharge ?? null;
                }
                else
                {
                    FacilityHead = null;
                    DepartmentInCharge = null;
                }
            }
            }
        }
        private Employee _FacilityHead;
        public Employee FacilityHead
        {
            get { return _FacilityHead; }
            set { SetPropertyValue("FacilityHead", ref _FacilityHead, value); }
        }
        private Employee _DepartmentInCharge;
        public Employee DepartmentInCharge
        {
            get { return _DepartmentInCharge; }
            set { SetPropertyValue("DepartmentInCharge", ref _DepartmentInCharge, value); }
        }
        private ExpenseType _ExpenseType;
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        private SubExpenseType _SubExpenseType;
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get { return _SubExpenseType; }
            set { SetPropertyValue("SubExpenseType", ref _SubExpenseType, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("ECS No.")]
        public EmployeeChargeSlip ChargeSlipRef
        {
            get { return _ChargeSlipRef; }
            set { SetPropertyValue("ChargeSlipRef", ref _ChargeSlipRef, value); }
        }
        [DisplayName("CS #.")]
        public string ChargeSlipNo
        {
            get { return _ChargeSlipNo; }
            set { SetPropertyValue("ChargeSlipNo", ref _ChargeSlipNo, value); }
        }
        //[RuleFromBoolProperty("InvalidCsNo", DefaultContexts.Save, UsedProperties = "ChargeSlipNo")]
        //[NonPersistent]
        //public bool InvalidCsNo
        //{
        //    get { return _TransactionType== FuelPumpRegisterTypeEnum.ChargeToEmployee && !string.IsNullOrEmpty(_ChargeSlipNo); }
        //}
        [Persistent("Total")]
        private decimal? _Total;
        [PersistentAlias("_Total")]
        [Custom("DisplayFormat", "n")]
        public decimal? Total
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Total == null)
                    {
                        UpdateTotal(
                        false);
                    }
                }
                catch (Exception)
                {
                }
                return _Total;
            }
        }
        public void UpdateTotal(bool forceChangeEvent)
        {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (FuelPumpRegisterDetail detail in FuelPumpRegisterDetails)
            {
                tempTotal
                += detail.Total;
            }
            _Total = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Total", Total, _Total);
            }
            ;
        }
        [Persistent("TotalQty")]
        private decimal? _TotalQty;
        [PersistentAlias("_TotalQty")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Total Qty")]
        public decimal? TotalQty
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalQty == null)
                    {
                        UpdateTotalQty(false);
                    }
                }
                catch (Exception)
                {
                }
                return _TotalQty;
            }
        }
        public void UpdateTotalQty(bool forceChangeEvent)
        {
            decimal? oldTotalQty = _TotalQty;
            decimal tempTotal = 0m;
            foreach (FuelPumpRegisterDetail detail in FuelPumpRegisterDetails)
            {
                tempTotal
                += detail.Quantity;
            }
            _TotalQty = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalQty", TotalQty, _TotalQty);
            }
            ;
        }
        [Persistent("Price")]
        private decimal? _Price;
        [PersistentAlias("_Price")]
        [Custom("DisplayFormat", "n")]
        public decimal? Price
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Price == null)
                    {
                        UpdatePrice(
                        false);
                    }
                }
                catch (Exception)
                {
                }
                return _Price;
            }
        }
        public void UpdatePrice(bool forceChangeEvent)
        {
            decimal? oldPrice = _Price;
            decimal tempTotal = 0m;
            foreach (FuelPumpRegisterDetail detail in FuelPumpRegisterDetails)
            {
                tempTotal
                += detail.Cost;
            }
            if (FuelPumpRegisterDetails.Count != 0)
            {
                _Price = tempTotal / FuelPumpRegisterDetails.Count;
            }
            else
            {
                _Price = 0m;
            }

            if (forceChangeEvent)
            {
                OnChanged("Price", Price, _Price);
            }
            ;
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        private decimal _PrevLife;
        private FAOtherVehicle _OtherNo;
        private decimal _OthRead;
        private decimal _OdoRead;
        private decimal _PrevOdoRead;
        private EmployeeChargeSlip _ChargeSlipRef;
        private string _ChargeSlipNo;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month
        {
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
                return EntryDate.Year;
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
        public FuelPumpRegister(Session session)
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
            Vendor = Session.FindObject<Vendor>(new BinaryOperator(
            "No", "V05260")) ?? null;
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "FPR"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "FPR"));
            this.ExpenseType = SourceType != null ? SourceType.ExpenseType ?? null : null;
            this.SubExpenseType = SourceType != null ? SourceType.SubExpenseType ?? null : null;
            this.Facility = SourceType != null ? SourceType.Facility ?? null : null;
            this.Department = SourceType != null ? SourceType.Department ?? null : null;
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "FPR"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ?
                source.GetNewNo() :
                null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            //ShipToAddress = company.FullShipAddress;
            Memo = "Fuel withdrawn from GLC Fuel station";
        }

        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }

        private void Reset()
        {
            _Total = null;
            _TotalQty = null;
            _Price = null;
        }

        protected override void OnSaving()
        {
            #region New Fuel Odo Logging
            FixedAsset thisFA = _Unit != null ? Session.GetObjectByKey<FixedAsset>(_Unit.Oid) : null;
            if (thisFA != null)
            {
                FuelOdoRegistry newLog = null;
                //thisReceipt
                if (ReceiptFuelOdoLogs.Count > 0)
                {
                    newLog = ReceiptFuelOdoLogs.FirstOrDefault();
                }
                else
                {
                    newLog = ReflectionHelper.CreateObject<FuelOdoRegistry>(Session);
                }
                newLog.LastReading = 0m;
                newLog.Difference = 0m;
                newLog.Fleet = thisFA;
                newLog.FuelPumpRegId = this;
                newLog.EntryDate = EntryDate;
                newLog.LogType = MeterRegistryTypeEnum.Log;
                newLog.Reading = _OdoRead;
                newLog.Save();
            }
            #endregion
            base.OnSaving();
        }
        protected override void OnSaved()
        {
            this.AutoRegisterIncomeExpenseVer();
            base.OnSaved();
        }
    }

}
