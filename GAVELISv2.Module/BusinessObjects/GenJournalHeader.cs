using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.Collections.Generic;
namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [FriendlyKeyProperty("SourceNo")]
    public class GenJournalHeader : XPObject
    {
        private string _Code;
        private DateTime _EntryDate = DateTime.Now;
        private string _Description;
        private DateTime _PostedDate;
        private SourceType _SourceType;
        private string _SourceNo;
        private OperationType _OperationType;
        private decimal _Amount;
        private Contact _SubAccountNo;
        private ContactTypeEnum _SubAccountType;
        private bool _Approved = false;
        private int _Period;
        private int _Week;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public virtual DateTime EntryDate
        {
            get { return _EntryDate; }
            set
            {
                SetPropertyValue("EntryDate", ref _EntryDate, value);
                if (!IsLoading)
                {
                    EntryDateAccepted = false;
                    CheckDate = _EntryDate;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }

        public DateTime PostedDate
        {
            get { return _PostedDate; }
            set { SetPropertyValue("PostedDate", ref _PostedDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public SourceType SourceType
        {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }

        public string SourceTypeCode
        {
            get
            {
                if (_SourceType != null)
                {
                    return _SourceType.Code;
                }
                return null;
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string SourceNo
        {
            get { return _SourceNo; }
            set
            {
                SetPropertyValue("SourceNo", ref _SourceNo, value);
                if (!IsLoading && _OperationType != null)
                {
                    Description =
                    _OperationType.Description + " " + _SourceNo;
                }
                if (!IsLoading)
                {
                    Code = _SourceNo;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public OperationType OperationType
        {
            get { return _OperationType; }
            set { SetPropertyValue("OperationType", ref _OperationType, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Amount
        {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
        }

        public Contact SubAccountNo
        {
            get { return _SubAccountNo; }
            set
            {
                SetPropertyValue("SubAccountNo", ref _SubAccountNo, value);
                if (!IsLoading && _SubAccountNo != null)
                {
                    SubAccountType =
                    _SubAccountNo.ContactType;
                }
            }
        }

        public ContactTypeEnum SubAccountType
        {
            get { return _SubAccountType; }
            set
            {
                SetPropertyValue("SubAccountType", ref _SubAccountType, value)
                    ;
            }
        }

        public bool Approved
        {
            get { return _Approved; }
            set { SetPropertyValue("Approved", ref _Approved, value); }
        }

        //private bool _ForceDelete = false;
        //public bool ForceDelete {
        //    get { return _ForceDelete; }
        //    set { SetPropertyValue<bool>("ForceDelete", ref _ForceDelete, value); }
        //}

        public int Period
        {
            get { return _Period; }
            set { SetPropertyValue("Period", ref _Period, value); }
        }

        public int Week
        {
            get { return _Week; }
            set { SetPropertyValue("Week", ref _Week, value); }
        }

        public DateTime ExactEntryDate
        {
            get { return new DateTime(_EntryDate.Date.Year, _EntryDate.Date.Month, _EntryDate.Date.Day, 23, 0, 0); }
        }

        public DateTime ExactEntryDateEnd
        {
            get { return new DateTime(_EntryDate.Date.Year, _EntryDate.Date.Month, _EntryDate.Date.Day, 0, 0, 0); }
        }

        public CheckPayment CheckPaymentInfo
        {
            get
            {
                if (this.SourceType.Code == "CR")
                {
                    if (this.Description.StartsWith("Payment Made"))
                    {
                        var dat = Session.GetObjectByKey<CheckPayment>(this.Oid);
                        return dat;
                    }
                }
                return null;
            }
        }
        [NonPersistent]
        public int HKey
        {
            get { return this.Oid; }
        }
        //public ExpenseType ExpenseType
        //{
        //    get
        //    {
        //        if (this.SourceType.Code == "CR")
        //        {
        //            if (this.Description.StartsWith("Payment Made"))
        //            {
        //                var dat = this as CheckPayment;
        //                return dat.ExpenseType;
        //            }
        //        }
        //        return null;
        //    }
        //}
        //public SubExpenseType SubExpenseType
        //{
        //    get
        //    {
        //        if (this.SourceType.Code == "CR")
        //        {
        //            if (this.Description.StartsWith("Payment Made"))
        //            {
        //                return (this as CheckPayment).SubExpenseType;
        //            }
        //        }
        //        return null;
        //    }
        //}
        [Aggregated,
        Association("GenJournalHeader-GenJournalDetails")]
        public XPCollection<GenJournalDetail> GenJournalDetails
        {
            get
            {
                return
                    GetCollection<GenJournalDetail>("GenJournalDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-PhysicalAdjustmentDetails")]
        public XPCollection<PhysicalAdjustmentDetail> PhysicalAdjustmentDetails
        {
            get
            {
                return GetCollection<PhysicalAdjustmentDetail>(
                    "PhysicalAdjustmentDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-InventoryControlJournals")]
        public XPCollection<InventoryControlJournal> InventoryControlJournals
        {
            get
            {
                return GetCollection<InventoryControlJournal>(
                    "InventoryControlJournals");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-PurchaseOrderDetails")]
        public XPCollection<PurchaseOrderDetail> PurchaseOrderDetails
        {
            get
            {
                return GetCollection<PurchaseOrderDetail>("PurchaseOrderDetails"
                    );
            }
        }

        [Aggregated,
        Association("GenJournalHeader-FuelPurchaseOrderDetails")]
        public XPCollection<FuelPurchaseOrderDetail> FuelPurchaseOrderDetails
        {
            get
            {
                return GetCollection<FuelPurchaseOrderDetail>("FuelPurchaseOrderDetails"
                    );
            }
        }

        [Aggregated,
        Association("GenJournalHeader-ReceiptDetails")]
        public XPCollection<ReceiptDetail> ReceiptDetails
        {
            get
            {
                return
                    GetCollection<ReceiptDetail>("ReceiptDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-JobOrderDetails")]
        public XPCollection<JobOrderDetail> JobOrderDetails
        {
            get
            {
                return
                    GetCollection<JobOrderDetail>("JobOrderDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-DebitMemoDetails")]
        public XPCollection<DebitMemoDetail> DebitMemoDetails
        {
            get
            {
                return
                    GetCollection<DebitMemoDetail>("DebitMemoDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-SalesOrderDetails")]
        public XPCollection<SalesOrderDetail> SalesOrderDetails
        {
            get
            {
                return
                    GetCollection<SalesOrderDetail>("SalesOrderDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-InvoiceDetails")]
        public XPCollection<InvoiceDetail> InvoiceDetails
        {
            get
            {
                return
                    GetCollection<InvoiceDetail>("InvoiceDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-TransferOrderDetails")]
        public XPCollection<TransferOrderDetail> TransferOrderDetails
        {
            get
            {
                return GetCollection<TransferOrderDetail>("TransferOrderDetails"
                    );
            }
        }

        [Aggregated,
        Association("GenJournalHeader-CreditMemoDetails")]
        public XPCollection<CreditMemoDetail> CreditMemoDetails
        {
            get
            {
                return
                    GetCollection<CreditMemoDetail>("CreditMemoDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-BillDetails")]
        public XPCollection<BillDetail> BillDetails
        {
            get
            {
                return GetCollection
                    <BillDetail>("BillDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-CheckPaymentDetails")]
        public XPCollection<CheckPaymentDetail> CheckPaymentDetails
        {
            get { return GetCollection<CheckPaymentDetail>("CheckPaymentDetails"); }
        }

        [Aggregated,
        Association("GenJournalHeader-CheckPayeeDetails")]
        public XPCollection<CheckPayeeDetail> CheckPayeeDetails
        {
            get
            {
                return
                    GetCollection<CheckPayeeDetail>("CheckPayeeDetails");
            }
        }

        // Cargo Registry
        [Aggregated,
        Association("GenJournalHeader-CargoRegistrations")]
        public XPCollection<CargoRegistry> CargoRegistrations
        {
            get
            {
                return
                    GetCollection<CargoRegistry>("CargoRegistrations");
            }
        }

        // Driver Registry
        [Aggregated,
        Association("GenJournalHeader-DriverRegistrations")]
        public XPCollection<DriverRegistry> DriverRegistrations
        {
            get
            {
                return
                    GetCollection<DriverRegistry>("DriverRegistrations");
            }
        }

        // Genset Entry
        [Aggregated,
        Association("GenJournalHeader-GensetEntries")]
        public XPCollection<GensetEntry> GensetEntries
        {
            get
            {
                return
                    GetCollection<GensetEntry>("GensetEntries");
            }
        }

        // KD Entry
        [Aggregated,
        Association("GenJournalHeader-KDEntries")]
        public XPCollection<KDEntry> KDEntries
        {
            get
            {
                return GetCollection<
                    KDEntry>("KDEntries");
            }
        }

        // Shunting Entry
        [Aggregated,
        Association("GenJournalHeader-ShuntingEntries")]
        public XPCollection<ShuntingEntry> ShuntingEntries
        {
            get
            {
                return
                    GetCollection<ShuntingEntry>("ShuntingEntries");
            }
        }

        // Truck Registry
        [Aggregated,
        Association("GenJournalHeader-TruckRegistrations")]
        public XPCollection<TruckRegistry> TruckRegistrations
        {
            get
            {
                return
                    GetCollection<TruckRegistry>("TruckRegistrations");
            }
        }

        // PO Fuel Detail
        [Aggregated,
        Association("GenJournalHeader-POrderFuelDetails")]
        public XPCollection<POrderFuelDetail> POrderFuelDetails
        {
            get
            {
                return
                    GetCollection<POrderFuelDetail>("POrderFuelDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-ReceiptFuelDetails")]
        public XPCollection<ReceiptFuelDetail> ReceiptFuelDetails
        {
            get
            {
                return
                    GetCollection<ReceiptFuelDetail>("ReceiptFuelDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-FuelPumpRegisterDetails")]
        public XPCollection<FuelPumpRegisterDetail> FuelPumpRegisterDetails
        {
            get
            {
                return
                    GetCollection<FuelPumpRegisterDetail>("FuelPumpRegisterDetails");
            }
        }
        // Fuel Registry
        [Aggregated,
        Association("GenJournalHeader-FuelRegistrations")]
        public XPCollection<FuelRegister> FuelRegistrations
        {
            get
            {
                return
                    GetCollection<FuelRegister>("FuelRegistrations");
            }
        }

        // Dolefil Trip Detailsss
        [Aggregated,
        Association("GenJournalHeader-DolefilTripDetails")]
        public XPCollection<DolefilTripDetail> DolefilTripDetails
        {
            get
            {
                return
                    GetCollection<DolefilTripDetail>("DolefilTripDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-DepositDetails")]
        public XPCollection<DepositDetail> DepositDetails
        {
            get
            {
                return
                    GetCollection<DepositDetail>("DepositDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-BankReconDeposits")]
        public XPCollection<BankReconDeposit> BankReconDeposits
        {
            get
            {
                return
                    GetCollection<BankReconDeposit>("BankReconDeposits");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-BankReconCheckAndPayments")]
        public XPCollection<BankReconCheckAndPayment> BankReconCheckAndPayments
        {
            get
            {
                return GetCollection<BankReconCheckAndPayment>(
                    "BankReconCheckAndPayments");
            }
        }

        [Aggregated,
        Association("WorkOrder-WorkOrderJobsDetails")]
        public XPCollection<WorkOrderJobsDetail> WorkOrderJobsDetails
        {
            get
            {
                return GetCollection<WorkOrderJobsDetail>("WorkOrderJobsDetails"
                    );
            }
        }

        [Aggregated,
        Association("WorkOrder-WorkOrderItemDetails")]
        public XPCollection<WorkOrderItemDetail> WorkOrderItemDetails
        {
            get
            {
                return GetCollection<WorkOrderItemDetail>("WorkOrderItemDetails"
                    );
            }
        }

        [Aggregated,
        Association("EmployeeChargeSlip-EmployeeChargeSlipItemDetails")]
        public XPCollection<EmployeeChargeSlipItemDetail> EmployeeChargeSlipItemDetails
        {
            get
            {
                return GetCollection<EmployeeChargeSlipItemDetail>("EmployeeChargeSlipItemDetails"
                    );
            }
        }

        [Aggregated,
        Association("EmployeeChargeSlip-EmployeeChargeSlipExpenseDetails")]
        public XPCollection<EmployeeChargeSlipExpenseDetail> EmployeeChargeSlipExpenseDetails
        {
            get
            {
                return GetCollection<EmployeeChargeSlipExpenseDetail>("EmployeeChargeSlipExpenseDetails"
                    );
            }
        }

        [Aggregated,
        Association("GenJournalHeader-PaymentsApplied")]
        public XPCollection<PaymentsApplied> PaymentsApplied
        {
            get
            {
                return GetCollection<PaymentsApplied>("PaymentsApplied"
                    );
            }
        }

        #region Payroll Batch Associations

        // DriverPayroll -> Generated from Trips, shunting and Kds
        [Aggregated,
        Association("DriverPayroll-Trips")]
        public XPCollection<DriverPayrollTrip> DriverPayrollTrips
        {
            get
            {
                return
                    GetCollection<DriverPayrollTrip>("DriverPayrollTrips");
            }
        }

        // Deduction -> Generated from Premium Deductions,Loans Deductions and Taxes
        [Aggregated,
        Association("Payroll-Deductions")]
        public XPCollection<PayrollDeduction> PayrollDeductions
        {
            get
            {
                return
                    GetCollection<PayrollDeduction>("PayrollDeductions");
            }
        }

        // Deduction(Others) -> Generated from Loan Deductions
        [Aggregated,
        Association("Payroll-DeductionOthers")]
        public XPCollection<PayrollDeductionOther> PayrollDeductionOthers
        {
            get
            {
                return GetCollection<PayrollDeductionOther>(
                    "PayrollDeductionOthers");
            }
        }

        // Adjustments -> Entered adjustments
        [Aggregated,
        Association("Payroll-Adjustments")]
        public XPCollection<PayrollAdjustment> PayrollAdjustments
        {
            get
            {
                return
                    GetCollection<PayrollAdjustment>("PayrollAdjustments");
            }
        }

        [Aggregated,
        Association("DriverPayroll-Payroll")]
        public XPCollection<DriverPayroll> DriverPayrolls
        {
            get
            {
                return
                    GetCollection<DriverPayroll>("DriverPayrolls");
            }
        }

        [Aggregated,
        Association("DriverPayroll2-Payroll")]
        public XPCollection<DriverPayroll2> DriverPayrolls2
        {
            get
            {
                return
                    GetCollection<DriverPayroll2>("DriverPayrolls2");
            }
        }

        [Aggregated,
        Association("DriverPayroll3-Payroll")]
        public XPCollection<DriverPayroll3> DriverPayrolls3
        {
            get
            {
                return
                    GetCollection<DriverPayroll3>("DriverPayrolls3");
            }
        }

        [Association("StaffPayroll-InAndOuts")]
        public XPCollection<CheckInAndOut> CheckInAndOuts
        {
            get
            {
                return
                    GetCollection<CheckInAndOut>("CheckInAndOuts");
            }
        }

        [Association("StaffPayroll-CalculatedRecords")]
        public XPCollection<CalculatedRecord> CalculatedRecords
        {
            get
            {
                return
                    GetCollection<CalculatedRecord>("CalculatedRecords");
            }
        }

        [Association("StaffPayroll-OvertimeAndLeaves")]
        public XPCollection<OvertimeAndLeave> OvertimeAndLeaves
        {
            get
            {
                return
                    GetCollection<OvertimeAndLeave>("OvertimeAndLeaves");
            }
        }

        [Aggregated,
        Association("StaffPayroll-Payroll")]
        public XPCollection<StaffPayroll> StaffPayrolls
        {
            get
            {
                return
                    GetCollection<StaffPayroll>("StaffPayrolls");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-RequisitionWorksheetLines")]
        public XPCollection<RequisitionWorksheet> RequisitionWorksheetLines
        {
            get
            {
                return GetCollection<RequisitionWorksheet>(
                    "RequisitionWorksheetLines");
            }
        }

        [Association("CalculatedAttendance")]
        public XPCollection<CheckInAndOut02> CalculatedAttendance
        {
            get
            {
                return GetCollection<CheckInAndOut02>(
                    "CalculatedAttendance");
            }
        }
        [Association("CalculatedAttendance2")]
        public XPCollection<CheckInAndOut03> CalculatedAttendance2
        {
            get
            {
                return GetCollection<CheckInAndOut03>(
                    "CalculatedAttendance2");
            }
        }
        [Aggregated,
        Association("GenJournalHeader-TripCalculationDetails")]
        public XPCollection<TripCalculationDetail> TripCalculationDetails
        {
            get
            {
                return
                    GetCollection<TripCalculationDetail>("TripCalculationDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-TariffTripCalculationDetails")]
        public XPCollection<TariffTripCalculationDetail> TariffTripCalculationDetails
        {
            get
            {
                return
                    GetCollection<TariffTripCalculationDetail>("TariffTripCalculationDetails");
            }
        }

        [Aggregated,
        Association("GenJournalHeader-PayableDocsApplieds")]
        public XPCollection<PayableDocsApplied> PayableDocsApplieds
        {
            get
            {
                return
                    GetCollection<PayableDocsApplied>("PayableDocsApplieds");
            }
        }
        #endregion

        private string taggedFuelReceipts;
        [NonPersistent]
        public string TaggedFuelReceipts
        {
            get
            {
                if (taggedFuelReceipts == null)
                {
                    RefreshTaggedFuelReceipts();
                }
                return taggedFuelReceipts;
            }
        }

        // Call this method when the collection changes
        public void RefreshTaggedFuelReceipts()
        {
            if (FuelRegistrations == null || FuelRegistrations.Count == 0)
            {
                taggedFuelReceipts = string.Empty;
                return;
            }

            var sourceNos = new List<string>();
            foreach (FuelRegister fr in FuelRegistrations)
            {
                if (!string.IsNullOrEmpty(fr.SourceNo))
                    sourceNos.Add(fr.SourceNo);
            }

            taggedFuelReceipts = string.Join(", ", sourceNos.ToArray());

            // Notify that the property has changed
            OnChanged("FuelReferences");
        }

        // Override to refresh when collection changes
        protected override void OnChanged(string propertyName, object oldValue, object newValue)
        {
            base.OnChanged(propertyName, oldValue, newValue);

            // Refresh when FuelRegistrations collection changes
            if (propertyName == "FuelRegistrations")
            {
                RefreshTaggedFuelReceipts();
            }
        }

        private bool _ReSynced = false;

        [Custom("AllowEdit", "False")]
        public bool ReSynced
        {
            get { return _ReSynced; }
            set { SetPropertyValue<bool>("ReSynced", ref _ReSynced, value); }
        }

        #region Display Customer

        private Customer _TripCustomer;
        public Customer TripCustomer
        {
            get
            {
                if (_SourceType.Code == "ST")
                {
                    _TripCustomer = ((StanfilcoTrip)
                    this).Customer;
                }
                if (_SourceType.Code == "DF")
                {
                    _TripCustomer = ((DolefilTrip)
                    this).Customer;
                }
                if (_SourceType.Code == "OT")
                {
                    _TripCustomer = ((OtherTrip)this)
                    .Customer;
                }
                return _TripCustomer;
            }
        }

        public string TripCustomerNo
        {
            get
            {
                if (TripCustomer != null)
                {
                    return TripCustomer.No;
                }
                return string.Empty;
            }
        }

        #endregion

        #region Check Register

        private string _CRCheckNo;
        private Account _CRBankCashAccount;
        private PaymentTypeEnum _CRPaymentMode;
        private string _CRReferenceNo;
        private Contact _CRName;
        private bool _CRCleared;
        private decimal _CRDeposit;
        private decimal _CRPayment;
        private string _CRMemo;
        private decimal _CRAdjusted;
        private string _CBankBranch;
        private DateTime _CRCheckDate;
        private bool _Voided = false;
        private bool _IsVoided;
        private bool _CRDeposited = false;
        private ExpenseType _ExpenseOrIncome;
        private DateTime _CheckDate;
        private bool _PostDated = false;
        public string CRCheckNo
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRCheckNo = ((
                        CheckPayment)this).CheckNo;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRCheckNo = ((
                        CheckVoucher)this).CheckNo;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _CRCheckNo = ((
                        ReceivePayment)this).CheckNo;
                    }
                }
                if (_SourceType.Code == "FT")
                {
                    _CRCheckNo = "Fund Transfer";
                }
                return _CRCheckNo;
            }
        }

        public Account CRBankCashAccount
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (GenJournalDetails.Count > 0)
                    {
                        foreach (GenJournalDetail item in GenJournalDetails)
                        {
                            if (item.Account != null)
                            {
                                if (item.Account.AccountType.Code == "B" || item.Account
                                .AccountType.Code == "C" || item.Account.Name ==
                                "Undeposited Collection")
                                {
                                    _CRBankCashAccount = item
                                    .Account;
                                }
                            }
                        }
                    }
                }
                if (_SourceType.Code == "FT")
                {
                    if (_OperationType.Code == "FT")
                    {
                        _CRBankCashAccount = ((
                        FundTransfer)this).TransferFundsFrom;
                    }
                    if (_OperationType.Code == "FP")
                    {
                        _CRBankCashAccount = ((
                        FundTransfer)this).TransferFundsTo;
                    }
                }
                return _CRBankCashAccount;
            }
        }

        public PaymentTypeEnum CRPaymentMode
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRPaymentMode = ((
                        CheckPayment)this).PaymentMode;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _CRPaymentMode = ((
                        ReceivePayment)this).PaymentMode;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRPaymentMode = ((
                        CheckVoucher)this).PaymentMode;
                    }
                }
                if (_SourceType.Code == "FT")
                {
                    _CRPaymentMode = PaymentTypeEnum.
                    Others;
                }
                return _CRPaymentMode;
            }
        }

        public string CRReferenceNo
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRReferenceNo = ((
                        CheckPayment)this).ReferenceNo;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _CRReferenceNo = ((
                        ReceivePayment)this).ReferenceNo;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRReferenceNo = ((
                        CheckVoucher)this).ReferenceNo;
                    }
                }
                if (_SourceType.Code == "FT")
                {
                    _CRReferenceNo = SourceNo;
                }
                return _CRReferenceNo;
            }
        }

        public Contact CRName
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRName = ((CheckPayment)
                        this).PayToOrder;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _CRName = ((ReceivePayment
                        )this).ReceiveFrom;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRName = ((CheckVoucher)
                        this).PayToOrder;
                    }
                }
                return _CRName;
            }
        }

        public bool CRCleared
        {
            get { return _CRCleared; }
            set { SetPropertyValue("CRCleared", ref _CRCleared, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CRDeposit
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PR")
                    {
                        _CRDeposit = ((
                        ReceivePayment)this).CheckAmount;
                    }
                    //if (_OperationType.Code == "CV")
                    //{ _CRDeposit = ((CheckVoucher)this).CheckAmount.Value; }
                }
                if (_SourceType.Code == "FT")
                {
                    if (_OperationType.Code == "FP")
                    {
                        _CRDeposit = ((FundTransfer)this).AmountToTransfer;
                    }
                }
                return _CRDeposit;
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CRPayment
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRPayment = ((
                        CheckPayment)this).CheckAmount;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRPayment = ((
                        CheckVoucher)this).CheckAmount.Value;
                    }
                }
                if (_SourceType.Code == "FT")
                {
                    if (_OperationType.Code == "FT")
                    {
                        _CRPayment = ((FundTransfer)this).AmountToTransfer;
                    }
                }
                return _CRPayment;
            }
        }

        [Size(1000)]
        public string CRMemo
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRMemo = ((CheckPayment)
                        this).Memo;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRMemo = ((CheckVoucher)
                        this).Memo;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _CRMemo = ((ReceivePayment
                        )this).Memo;
                    }
                }
                if (_SourceType.Code == "FT")
                {
                    _CRMemo = ((FundTransfer)this).
                    Memo;
                }
                return _CRMemo;
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CRAdjusted
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _CRAdjusted = ((
                        CheckPayment)this).Adjusted;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _CRAdjusted = ((
                        ReceivePayment)this).Adjusted;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _CRAdjusted = ((
                        CheckVoucher)this).Adjusted;
                    }
                }
                return _CRAdjusted;
            }
        }

        [DisplayName("Bank/Branch")]
        public string CBankBranch
        {
            get
            {
                if (_OperationType.Code == "PR")
                {
                    _CBankBranch = ((
                    ReceivePayment)this).BankBranch;
                }
                return _CBankBranch;
            }
        }

        public DateTime CRCheckDate
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PR")
                    {
                        _CRCheckDate = ((
                        ReceivePayment)this).CheckDate;
                    }
                }
                return _CRCheckDate;
            }
        }

        [PersistentAlias("Voided")]
        public bool IsVoided
        {
            get
            {
                object tempObject = EvaluateAlias("IsVoided");
                if (tempObject != null)
                {
                    return (bool)tempObject;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Voided
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PY")
                    {
                        _Voided = ((CheckPayment)
                        this).Status == CheckStatusEnum.Voided;
                    }
                    if (_OperationType.Code == "PR")
                    {
                        _Voided = ((ReceivePayment
                        )this).Status == PaymentStatusEnum.Voided;
                    }
                    if (_OperationType.Code == "CV")
                    {
                        _Voided = ((CheckVoucher)
                        this).Status == CheckStatusEnum.Voided;
                    }
                }
                return _Voided;
            }
        }

        public bool CRDeposited
        {
            get
            {
                if (_SourceType.Code == "CR")
                {
                    if (_OperationType.Code == "PR")
                    {
                        _CRDeposited = ((ReceivePayment)this).Deposited;
                    }
                }
                return _CRDeposited;
            }
        }

        public ExpenseType ExpenseOrIncome
        {
            get { return _ExpenseOrIncome; }
            set
            {
                SetPropertyValue("ExpenseOrIncome", ref _ExpenseOrIncome,
                    value);
            }
        }

        public DateTime CheckDate
        {
            get { return _CheckDate; }
            set { SetPropertyValue("CheckDate", ref _CheckDate, value); }
        }

        public bool PostDated
        {
            get { return _PostDated; }
            set { SetPropertyValue("PostDated", ref _PostDated, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool PdcCleared
        {
            get { return _PdcCleared; }
            set { SetPropertyValue("PdcCleared", ref _PdcCleared, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool PdcDue
        {
            get
            {
                if (_PostDated && CheckDate.Date <= DateTime.Now.Date)
                {
                    return true;
                }
                else if (_PostDated && CheckDate.Date > DateTime.Now.Date)
                {
                    return false;
                }
                return false;
            }
        }

        [Action(Caption = "PDC Cleared", ConfirmationMessage = "Do you really want to mark this PDC?", AutoCommit = true)]
        public void ClearPDC()
        {
            PdcCleared = true;
        }

        [Action(Caption = "Unmark as Cleared", ConfirmationMessage = "Do you really want to unmark this this PDC?", AutoCommit = true)]
        public void UnclearPDC()
        {
            PdcCleared = false;
        }
        #endregion

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum GMonth
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
        public string GQuarter
        {
            get
            {
                switch (GMonth)
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
        public int GYear
        {
            get
            {
                return EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string GMonthSorter
        {
            get
            {
                switch (GMonth)
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

        [Custom("AllowEdit", "False")]
        public bool IsSynced
        {
            get { return _IsSynced; }
            set { SetPropertyValue<bool>("IsSynced", ref _IsSynced, value); }
        }

        [Custom("AllowEdit", "False")]
        public string CreatedByAppwriteUserId
        {
            get { return _CreatedByAppwriteUserId; }
            set { SetPropertyValue<string>("CreatedByAppwriteUserId", ref _CreatedByAppwriteUserId, value); }
        }

        [Custom("AllowEdit", "False")]
        public string StatusByAppwriteUserId
        {
            get { return _StatusByAppwriteUserId; }
            set { SetPropertyValue<string>("StatusByAppwriteUserId", ref _StatusByAppwriteUserId, value); }
        }

        #region Income Vs Expenses Ver. 2

        private bool _IsIncomeExpReg = false;
        private bool _IsIncExpNeedUpdate = true;
        private bool _TempSkipOdoReg = false;
        private string _CantRegIncExpReason;
        [Custom("AllowEdit", "False")]
        public bool IsIncomeExpReg
        {
            get { return _IsIncomeExpReg; }
            set { SetPropertyValue<bool>("IsIncomeExpReg", ref _IsIncomeExpReg, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool IsIncExpNeedUpdate
        {
            get { return _IsIncExpNeedUpdate; }
            set { SetPropertyValue<bool>("IsIncExpNeedUpdate", ref _IsIncExpNeedUpdate, value); }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public bool TempSkipOdoReg
        {
            get { return _TempSkipOdoReg; }
            set { SetPropertyValue<bool>("TempSkipOdoReg", ref _TempSkipOdoReg, value); }
        }

        [Custom("AllowEdit", "False")]
        public string CantRegIncExpReason
        {
            get { return _CantRegIncExpReason; }
            set { SetPropertyValue<string>("CantRegIncExpReason", ref _CantRegIncExpReason, value); }
        }

        [NonPersistent]
        public bool IsIncExpExcluded
        {
            get
            {
                if (Description.StartsWith("Payment Made"))
                {
                    return true;
                }
                if (Description.StartsWith("Payment Received"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private bool _ExpenseTypeCorrected = false;
        [Custom("AllowEdit", "False")]
        public bool ExpenseTypeCorrected
        {
            get { return _ExpenseTypeCorrected; }
            set { SetPropertyValue("ExpenseTypeCorrected", ref _ExpenseTypeCorrected, value); }
        }
        [Custom("AllowEdit", "False")]
        public string RevalidatedId
        {
            get { return _RevalidatedId; }
            set { SetPropertyValue("RevalidatedId", ref _RevalidatedId, value); }
        }
        public void AutoRegisterIncomeExpenseVer()
        {
            //shouldReturn = false;
            if (_ExpenseTypeCorrected)
            {
                return;
            }
            if (IsIncExpNeedUpdate)
            {
                // if (this.Oid != -1)
                if (this.Oid != -1)
                {
                    this.CantRegIncExpReason = string.Empty;
                    if (this.SourceType.Code == "BE")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (BillDetail billDetail in this.BillDetails)
                        {
                            BillDetail bdt = this.Session.GetObjectByKey<BillDetail>(billDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, bdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = bdt.Oid.ToString();
                            incExp.PayeeType = (this as Bill).Vendor.ContactType;
                            incExp.Payee = (this as Bill).Vendor;
                            incExp.Description1 = (this as Bill).Memo;
                            incExp.Description2 = bdt.Description;
                            incExp.Category = bdt.ExpenseType;
                            //incExp.SubCategory = "Not Applicable";
                            incExp.CostCenter = bdt.CostCenter ?? null;
                            if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                            {
                                incExp.Fleet = incExp.CostCenter.FixedAsset;
                            }
                            incExp.Expense = bdt.Amount;
                            //incExp.Income = "Not Applicable";
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //CM	Credit Memo	CreditMemo -> CreditMemo.Details.ExpenseType (IH Parts) <- Subtract
                    if (this.SourceType.Code == "CM")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (CreditMemoDetail cmDetail in this.CreditMemoDetails)
                        {
                            CreditMemoDetail cmdt = this.Session.GetObjectByKey<CreditMemoDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as CreditMemo).Customer.ContactType;
                            incExp.Payee = (this as CreditMemo).Customer;
                            incExp.Description1 = (this as CreditMemo).Memo;
                            incExp.Description2 = cmdt.Description;
                            ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                            incExp.Category = defExpType;
                            //incExp.SubCategory = "Not Applicable";
                            //incExp.CostCenter = cmdt.CostCenter ?? null;
                            //incExp.Expense = 0 - cmdt.Total;
                            incExp.Income = 0 - cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //CR	Check Register	If Petty Cash -> PettyCash.Details.ExpenseType
                    //        If Payments Made -> Exclude
                    //        If Payments Receive -> Exclude
                    if (this.SourceType.Code == "CR")
                    {
                        if (this.Description.StartsWith("Payment Made") && this.CheckPaymentInfo != null && this.CheckPaymentInfo.ExpenseType != null && this.CheckPaymentInfo.ExpenseType.CreateForVoucher)
                        {
                            //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                            //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                            int invCnt = 0;
                            IncomeAndExpense02 incExp = null;
                            foreach (GenJournalDetail gjDetail in this.GenJournalDetails)
                            {
                                if (gjDetail.DebitAmount == 0)
                                {
                                    continue;
                                }
                                GenJournalDetail gjdt = this.Session.GetObjectByKey<GenJournalDetail>(gjDetail.Oid);
                                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, gjdt.Oid)));
                                if (incExp == null)
                                {
                                    incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                                }
                                incExp.EntryDate = gjdt.CVLineDate == DateTime.MinValue ? this.EntryDate : gjdt.CVLineDate;
                                incExp.SourceID = this;
                                incExp.SourceType = this.SourceType;
                                incExp.SourceNo = this.SourceNo;
                                incExp.Seq = this.EntryDate.ToUniversalTime();
                                incExp.RefID = gjdt.Oid.ToString();
                                incExp.PayeeType = (this as CheckPayment).PayToOrder.ContactType;
                                incExp.Payee = (this as CheckPayment).PayToOrder;
                                incExp.Description1 = (this as CheckPayment).Memo;
                                incExp.Description2 = gjdt.Description;
                                //ExpenseType defExpType = session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                                incExp.Category = (this as CheckPayment).ExpenseType;
                                incExp.SubCategory = (this as CheckPayment).SubExpenseType;
                                incExp.CostCenter = gjdt.CostCenter ?? null;
                                if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                                {
                                    incExp.Fleet = incExp.CostCenter.FixedAsset;
                                }
                                incExp.Expense = gjdt.DebitAmount;
                                //incExp.Income = "Not Applicable";
                                if (incExp.Category != null)
                                {
                                    incExp.Save();
                                }
                                else
                                {
                                    invCnt++;
                                    incExp.Delete();
                                }
                            }
                            if (invCnt > 0)
                            {
                                this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                                this.IsIncExpNeedUpdate = false;
                            }
                            else
                            {
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                            }
                            this.Save();
                        }
                        if (this.Description.StartsWith("Payment Received"))
                        {
                            return;
                        }
                        if (this.Description.StartsWith("Check and Petty Cash"))
                        {
                            //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                            //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                            int invCnt = 0;
                            IncomeAndExpense02 incExp = null;
                            foreach (GenJournalDetail gjDetail in this.GenJournalDetails)
                            {
                                if (gjDetail.IsCheckAmount)
                                {
                                    continue;
                                }
                                GenJournalDetail gjdt = this.Session.GetObjectByKey<GenJournalDetail>(gjDetail.Oid);
                                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, gjdt.Oid)));
                                if (incExp == null)
                                {
                                    incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                                }
                                incExp.EntryDate = gjdt.CVLineDate == DateTime.MinValue ? this.EntryDate : gjdt.CVLineDate;
                                incExp.SourceID = this;
                                incExp.SourceType = this.SourceType;
                                incExp.SourceNo = this.SourceNo;
                                incExp.Seq = this.EntryDate.ToUniversalTime();
                                incExp.RefID = gjdt.Oid.ToString();
                                incExp.PayeeType = (this as CheckVoucher).PayToOrder.ContactType;
                                incExp.Payee = (this as CheckVoucher).PayToOrder;
                                incExp.Description1 = (this as CheckVoucher).Memo;
                                incExp.Description2 = gjdt.Description;
                                //ExpenseType defExpType = session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                                incExp.Category = gjdt.ExpenseType;
                                incExp.SubCategory = gjdt.SubExpenseType;
                                incExp.CostCenter = gjdt.CostCenter ?? null;
                                if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                                {
                                    incExp.Fleet = incExp.CostCenter.FixedAsset;
                                }
                                incExp.Expense = gjdt.DebitAmount;
                                //incExp.Income = "Not Applicable";
                                if (incExp.Category != null)
                                {
                                    incExp.Save();
                                }
                                else
                                {
                                    invCnt++;
                                    incExp.Delete();
                                }
                            }
                            if (invCnt > 0)
                            {
                                this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                                this.IsIncExpNeedUpdate = false;
                            }
                            else
                            {
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                            }
                            this.Save();
                        }
                    }
                    //DF	Dolefil Trip	DolefilTrip -> NetBilling -> ExpenseType (Trucking)
                    if (this.SourceType.Code == "DF")
                    {
                        IncomeAndExpense02 incExp = null;
                        incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as DolefilTrip).DocumentNo)));
                        if (incExp == null)
                        {
                            incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp.EntryDate = this.EntryDate;
                        incExp.SourceID = this;
                        incExp.SourceType = this.SourceType;
                        incExp.SourceNo = this.SourceNo;
                        incExp.Seq = this.EntryDate.ToUniversalTime();
                        incExp.RefID = (this as DolefilTrip).DocumentNo;
                        incExp.PayeeType = (this as DolefilTrip).Customer.ContactType;
                        incExp.Payee = (this as DolefilTrip).Customer;
                        incExp.Description1 = (this as DolefilTrip).Memo;
                        incExp.Description2 = this.Description;
                        ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0002'"));
                        incExp.Category = defExpType;
                        //incExp.SubCategory = gjdt.SubExpenseType;
                        //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //incExp.Expense = gjdt.DebitAmount;
                        if (((DolefilTrip)incExp.SourceID).TruckNo != null)
                        {
                            incExp.Fleet = ((DolefilTrip)incExp.SourceID).TruckNo;
                        }
                        incExp.Income = (this as DolefilTrip).NetBilling;
                        if (incExp.Category != null)
                        {
                            incExp.Save();
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            incExp.Delete();
                            this.CantRegIncExpReason = "Cannot register because expense type not specified";
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //DM	Debit Memo	DebitMemo -> DebitMemo.Details.ExpenseType (IH Parts) <- Subtract
                    if (this.SourceType.Code == "DM")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (DebitMemoDetail dmDetail in this.DebitMemoDetails)
                        {
                            DebitMemoDetail dmdt = this.Session.GetObjectByKey<DebitMemoDetail>(dmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, dmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = dmdt.Oid.ToString();
                            incExp.PayeeType = (this as DebitMemo).Vendor.ContactType;
                            incExp.Payee = (this as DebitMemo).Vendor;
                            incExp.Description1 = (this as DebitMemo).Memo;
                            incExp.Description2 = dmdt.Description;
                            ExpenseType defExpType = null;
                            if (dmdt.ExpenseType != null)
                            {
                                incExp.Category = dmdt.ExpenseType ?? null;
                                incExp.SubCategory = dmdt.SubExpenseType ?? null;
                            }
                            else
                            {
                                defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0011'"));
                                incExp.Category = defExpType;
                            }
                            //incExp.SubCategory = "Not Applicable";
                            //incExp.CostCenter = cmdt.CostCenter ?? null;
                            incExp.Expense = 0 - dmdt.Total;
                            //incExp.Income = 0 - dmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //ECS	Employee Charge Slip	EmployeeChargeSlip -> ItemChargeDetails.ExpenseType (IH Parts)
                    //        EmployeeChargeSlip -> ExpenseChargeDetails.ExpenseType
                    if (this.SourceType.Code == "ECS")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (EmployeeChargeSlipItemDetail cmDetail in this.EmployeeChargeSlipItemDetails)
                        {
                            EmployeeChargeSlipItemDetail cmdt = this.Session.GetObjectByKey<EmployeeChargeSlipItemDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as EmployeeChargeSlip).Employee.ContactType;
                            incExp.Payee = (this as EmployeeChargeSlip).Employee;
                            incExp.Description1 = (this as EmployeeChargeSlip).Memo;
                            incExp.Description2 = cmdt.Description;
                            ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                            incExp.Category = defExpType;
                            //incExp.SubCategory = "Not Applicable";
                            incExp.CostCenter = cmdt.CostCenter ?? null;
                            if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                            {
                                incExp.Fleet = incExp.CostCenter.FixedAsset;
                            }
                            //incExp.Expense = 0 - cmdt.Total;
                            incExp.Income = cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }

                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        foreach (EmployeeChargeSlipExpenseDetail cmDetail in this.EmployeeChargeSlipExpenseDetails)
                        {
                            EmployeeChargeSlipExpenseDetail cmdt = this.Session.GetObjectByKey<EmployeeChargeSlipExpenseDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as EmployeeChargeSlip).Employee.ContactType;
                            incExp.Payee = (this as EmployeeChargeSlip).Employee;
                            incExp.Description1 = (this as EmployeeChargeSlip).Memo;
                            incExp.Description2 = cmdt.Description;
                            //ExpenseType defExpType = session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                            incExp.Category = cmdt.ExpenseType;
                            incExp.SubCategory = cmdt.SubExpenseType ?? null;
                            incExp.CostCenter = cmdt.CostCenter ?? null;
                            if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                            {
                                incExp.Fleet = incExp.CostCenter.FixedAsset;
                            }
                            incExp.Expense = cmdt.Amount;
                            //incExp.Income = cmdt.Amount;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //GS	Generator Set	GensetEntry -> NetBilling -> ExpenseType (Trucking)
                    if (this.SourceType.Code == "GS")
                    {
                        bool inDollar = ((GensetEntry)this).TripID.GetType()==typeof(StanfilcoTrip) ? (((GensetEntry)this).TripID as StanfilcoTrip).MigratedToDollar : false;
                        //bool skip = ((GensetEntry)this).TripID.GetType() == typeof(StanfilcoTrip) && (((GensetEntry)this).TripID as StanfilcoTrip).MigratedToDollar;
                        IncomeAndExpense02 incExp = null;
                        incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as GensetEntry).TripNo)));
                        if (incExp == null)
                        {
                            incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp.EntryDate = this.EntryDate;
                        incExp.SourceID = this;
                        incExp.SourceType = this.SourceType;
                        incExp.SourceNo = this.SourceNo;
                        incExp.Seq = this.EntryDate.ToUniversalTime();
                        incExp.RefID = (this as GensetEntry).TripNo;
                        incExp.PayeeType = (this as GensetEntry).Customer.ContactType;
                        incExp.Payee = (this as GensetEntry).Customer;
                        incExp.Description1 = (this as GensetEntry).Memo;
                        incExp.Description2 = this.Description;
                        ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0004'"));
                        incExp.Category = defExpType;
                        //incExp.SubCategory = gjdt.SubExpenseType;
                        //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //incExp.Expense = gjdt.DebitAmount;
                        if (((GensetEntry)incExp.SourceID).TruckNo != null)
                        {
                            incExp.Fleet = ((GensetEntry)incExp.SourceID).TruckNo;
                        }
                        if (inDollar)
                        {
                            incExp.Income = (((GensetEntry)this).TripID as StanfilcoTrip).GensetNetBillingLCY;
                        }
                        else
                        {
                            incExp.Income = (this as GensetEntry).NetBilling;
                        }
                        if (incExp.Category != null)
                        {
                            incExp.Save();
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            incExp.Delete();
                            this.CantRegIncExpReason = "Cannot register because expense type not specified";
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                        //if (!skip && this.EntryDate < new DateTime(2018, 2, 11))
                        //{
                        //    IncomeAndExpense02 incExp = null;
                        //    incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as GensetEntry).TripNo)));
                        //    if (incExp == null)
                        //    {
                        //        incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        //    }
                        //    incExp.EntryDate = this.EntryDate;
                        //    incExp.SourceID = this;
                        //    incExp.SourceType = this.SourceType;
                        //    incExp.SourceNo = this.SourceNo;
                        //    incExp.Seq = this.EntryDate.ToUniversalTime();
                        //    incExp.RefID = (this as GensetEntry).TripNo;
                        //    incExp.PayeeType = (this as GensetEntry).Customer.ContactType;
                        //    incExp.Payee = (this as GensetEntry).Customer;
                        //    incExp.Description1 = (this as GensetEntry).Memo;
                        //    incExp.Description2 = this.Description;
                        //    ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0004'"));
                        //    incExp.Category = defExpType;
                        //    //incExp.SubCategory = gjdt.SubExpenseType;
                        //    //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //    //incExp.Expense = gjdt.DebitAmount;
                        //    if (((GensetEntry)incExp.SourceID).TruckNo != null)
                        //    {
                        //        incExp.Fleet = ((GensetEntry)incExp.SourceID).TruckNo;
                        //    }
                        //    incExp.Income = (this as GensetEntry).NetBilling;
                        //    if (incExp.Category != null)
                        //    {
                        //        incExp.Save();
                        //        this.IsIncomeExpReg = true;
                        //        this.IsIncExpNeedUpdate = false;
                        //    }
                        //    else
                        //    {
                        //        incExp.Delete();
                        //        this.CantRegIncExpReason = "Cannot register because expense type not specified";
                        //        this.IsIncExpNeedUpdate = false;
                        //    }
                        //    this.Save();
                        //}
                    }
                    //IN	Invoice	Invoice -> Invoice.Details.ExpenseType (IH Parts)
                    if (this.SourceType.Code == "IN")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (InvoiceDetail cmDetail in this.InvoiceDetails)
                        {
                            InvoiceDetail cmdt = this.Session.GetObjectByKey<InvoiceDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as Invoice).Customer.ContactType;
                            incExp.Payee = (this as Invoice).Customer;
                            incExp.Description1 = (this as Invoice).Memo;
                            incExp.Description2 = cmdt.Description;
                            ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                            incExp.Category = defExpType;
                            //incExp.SubCategory = "Not Applicable";
                            //incExp.CostCenter = cmdt.CostCenter ?? null;
                            //incExp.Expense = 0 - cmdt.Total;
                            incExp.Income = cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //JO	Job Order	JobOrder -> JobOrder.Details.ItemNo.COGSAccount.ExpenseType
                    if (this.SourceType.Code == "JO")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (JobOrderDetail cmDetail in this.JobOrderDetails)
                        {
                            JobOrderDetail cmdt = this.Session.GetObjectByKey<JobOrderDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as JobOrder).Vendor.ContactType;
                            incExp.Payee = (this as JobOrder).Vendor;
                            incExp.Description1 = (this as JobOrder).Memo;
                            incExp.Description2 = cmdt.Description;
                            if (cmdt.ExpenseType != null)
                            {
                                incExp.Category = cmdt.ExpenseType ?? null;
                                incExp.SubCategory = cmdt.SubExpenseType ?? null;
                            }
                            else
                            {
                                //ExpenseType defExpType = session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                                if (cmdt.ItemNo.COGSAccount != null && cmdt.ItemNo.COGSAccount.ExpenseType != null)
                                {
                                    incExp.Category = cmdt.ItemNo.COGSAccount.ExpenseType;
                                }
                                else
                                {
                                    throw new UserFriendlyException(string.Format("The item {0} has no COGS account or Expense Type", cmdt.ItemNo.No));
                                }
                                //incExp.SubCategory = "Not Applicable";
                            }
                            incExp.CostCenter = cmdt.CostCenter ?? null;
                            if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                            {
                                incExp.Fleet = incExp.CostCenter.FixedAsset;
                            }
                            incExp.Expense = cmdt.Total;
                            //incExp.Income = cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //JR	General Journal	JournalEntry -> JournalEntry.ExpenseType
                    //        If ExpenseType is Null then Exclude
                    //        If Description begins with "Voided" then Exclude
                    //        If Description begins with "Adjustment" then Exclude
                    if (this.SourceType.Code == "JR")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (GenJournalDetail gjDetail in this.GenJournalDetails)
                        {
                            if (gjDetail.ExpenseType == null)
                            {
                                this.CantRegIncExpReason = "No ExpenseType/Not Applicable";
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                                this.Save();
                                continue;
                            }
                            if (gjDetail.Description.StartsWith("Voided", true, null))
                            {
                                this.CantRegIncExpReason = "Not Applicable";
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                                this.Save();
                                continue;
                            }
                            if (gjDetail.Description.StartsWith("Adjustment", true, null))
                            {
                                this.CantRegIncExpReason = "Not Applicable";
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                                this.Save();
                                continue;
                            }
                            if (gjDetail.Description.Contains("Adjustment"))
                            {
                                this.CantRegIncExpReason = "Not Applicable";
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                                this.Save();
                                continue;
                            }
                            if (gjDetail.SubAccountNo == null)
                            {
                                this.CantRegIncExpReason = "No Sub Account/Not Applicable";
                                this.IsIncomeExpReg = true;
                                this.IsIncExpNeedUpdate = false;
                                this.Save();
                                continue;
                            }
                            if (gjDetail.CreditAmount > 0)
                            {
                                continue;
                            }
                            GenJournalDetail gjdt = this.Session.GetObjectByKey<GenJournalDetail>(gjDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, gjdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = gjdt.Oid.ToString();
                            incExp.PayeeType = gjdt.SubAccountType;
                            incExp.Payee = gjdt.SubAccountNo;
                            incExp.Description1 = (this as JournalEntry).Memo;
                            incExp.Description2 = gjdt.Description;
                            //ExpenseType defExpType = session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0001'"));
                            incExp.Category = gjdt.ExpenseType;
                            incExp.SubCategory = gjdt.SubExpenseType;
                            incExp.CostCenter = gjdt.CostCenter ?? null;
                            if (!gjdt.ExpenseType.Income)
                            {
                                incExp.Expense = gjdt.DebitAmount;
                            }
                            else
                            {
                                incExp.Income = gjdt.DebitAmount;
                            }
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //KD	Knockdown Box	KDEntry -> NetBilling -> ExpenseType (Trucking)
                    if (this.SourceType.Code == "KD")
                    {
                        IncomeAndExpense02 incExp = null;
                        incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as KDEntry).TripNo)));
                        if (incExp == null)
                        {
                            incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp.EntryDate = this.EntryDate;
                        incExp.SourceID = this;
                        incExp.SourceType = this.SourceType;
                        incExp.SourceNo = this.SourceNo;
                        incExp.Seq = this.EntryDate.ToUniversalTime();
                        incExp.RefID = (this as KDEntry).TripNo;
                        incExp.PayeeType = (this as KDEntry).Customer.ContactType;
                        incExp.Payee = (this as KDEntry).Customer;
                        incExp.Description1 = (this as KDEntry).Memo;
                        incExp.Description2 = this.Description;
                        ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0002'"));
                        incExp.Category = defExpType;
                        //incExp.SubCategory = gjdt.SubExpenseType;
                        //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //incExp.Expense = gjdt.DebitAmount;
                        if (((KDEntry)incExp.SourceID).TruckNo != null)
                        {
                            incExp.Fleet = ((KDEntry)incExp.SourceID).TruckNo;
                        }
                        incExp.Income = (this as KDEntry).NetBilling;
                        if (incExp.Category != null)
                        {
                            incExp.Save();
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            incExp.Delete();
                            this.CantRegIncExpReason = "Cannot register because expense type not specified";
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //OT	Other Trip	DolefilTrip -> GrossBilling -> ExpenseType (Trucking)
                    if (this.SourceType.Code == "OT")
                    {
                        IncomeAndExpense02 incExp = null;
                        incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as OtherTrip).TripNo)));
                        if (incExp == null)
                        {
                            incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp.EntryDate = this.EntryDate;
                        incExp.SourceID = this;
                        incExp.SourceType = this.SourceType;
                        incExp.SourceNo = this.SourceNo;
                        incExp.Seq = this.EntryDate.ToUniversalTime();
                        incExp.RefID = (this as OtherTrip).TripNo;
                        incExp.PayeeType = (this as OtherTrip).Customer.ContactType;
                        incExp.Payee = (this as OtherTrip).Customer;
                        incExp.Description1 = (this as OtherTrip).Memo;
                        incExp.Description2 = this.Description;
                        ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0002'"));
                        incExp.Category = defExpType;
                        //incExp.SubCategory = gjdt.SubExpenseType;
                        //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //incExp.Expense = gjdt.DebitAmount;
                        if (((OtherTrip)incExp.SourceID).TruckNo != null)
                        {
                            incExp.Fleet = ((OtherTrip)incExp.SourceID).TruckNo;
                        }
                        incExp.Income = (this as OtherTrip).GrossBilling;
                        if (incExp.Category != null)
                        {
                            incExp.Save();
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            incExp.Delete();
                            this.CantRegIncExpReason = "Cannot register because expense type not specified";
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //PRLD	Payroll Driver	Pending
                    if (this.SourceType.Code == "PRLD")
                    {
                        this.CantRegIncExpReason = "Pending Implementation";
                        //this.IsIncomeExpReg = true;
                        this.IsIncExpNeedUpdate = false;
                        this.Save();
                        return;
                    }
                    //RC	Receive	Receive -> Receive.Details.ExpenseType (IH Parts)
                    if (this.SourceType.Code == "RC")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (ReceiptDetail cmDetail in this.ReceiptDetails)
                        {
                            ReceiptDetail cmdt = this.Session.GetObjectByKey<ReceiptDetail>(cmDetail.Oid);
                            if (cmdt == null)
                            {
                                continue;
                            }
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as Receipt).Vendor.ContactType;
                            incExp.Payee = (this as Receipt).Vendor;
                            incExp.Description1 = (this as Receipt).Memo;
                            incExp.Description2 = cmdt.Description;
                            ExpenseType defExpType = null;
                            if (((Receipt)cmdt.GenJournalID).TireTransaction)
                            {
                                if (cmdt.ExpenseType != null)
                                {
                                    incExp.Category = cmdt.ExpenseType ?? null;
                                    incExp.SubCategory = cmdt.SubExpenseType ?? null;
                                }
                                else
                                {
                                    defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0360'")); // Tires
                                    incExp.Category = defExpType;
                                    incExp.SubCategory = null;
                                }
                            }
                            else
                            {
                                if (cmdt.ExpenseType != null)
                                {
                                    incExp.Category = cmdt.ExpenseType ?? null;
                                    incExp.SubCategory = cmdt.SubExpenseType ?? null;
                                }
                                else
                                {
                                    defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0011'")); // IH Parts Purchases
                                    incExp.Category = defExpType;
                                    incExp.SubCategory = null;
                                }
                            }
                            //incExp.SubCategory = "Not Applicable";
                            incExp.CostCenter = cmdt.CostCenter ?? null;
                            if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                            {
                                incExp.Fleet = incExp.CostCenter.FixedAsset;
                            }
                            incExp.Facility = cmdt.Facility ?? null;
                            incExp.FacilityHead = cmdt.FacilityHead ?? null;
                            incExp.Department = cmdt.Department ?? null;
                            incExp.DepartmentInCharge = cmdt.DepartmentInCharge ?? null;
                            incExp.Expense = cmdt.Total;
                            //incExp.Income = cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //FPR Fuel Pump Register
                    if (this.SourceType.Code == "FPR")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (FuelPumpRegisterDetail cmDetail in this.FuelPumpRegisterDetails)
                        {
                            FuelPumpRegisterDetail cmdt = this.Session.GetObjectByKey<FuelPumpRegisterDetail>(cmDetail.Oid);
                            if (cmdt == null)
                            {
                                continue;
                            }
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as FuelPumpRegister).Vendor.ContactType;
                            incExp.Payee = (this as FuelPumpRegister).Vendor;
                            incExp.Description1 = (this as FuelPumpRegister).Memo;
                            incExp.Description2 = cmdt.Description;
                            ExpenseType defExpType = null;
                            
                            if (cmdt.ExpenseType != null)
                            {
                                incExp.Category = cmdt.ExpenseType ?? null;
                                incExp.SubCategory = cmdt.SubExpenseType ?? null;
                            }
                            else
                            {
                                defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0006'")); // IH Parts Purchases
                                incExp.Category = defExpType;
                                incExp.SubCategory = null;
                            }
                            incExp.CostCenter = ((FuelPumpRegister)cmdt.GenJournalID).ChargeTo ?? null;
                            incExp.Fleet = ((FuelPumpRegister)cmdt.GenJournalID).Unit ?? null;
                            incExp.Facility = ((FuelPumpRegister)cmdt.GenJournalID).Facility ?? null;
                            incExp.FacilityHead = ((FuelPumpRegister)cmdt.GenJournalID).FacilityHead ?? null;
                            incExp.Department = ((FuelPumpRegister)cmdt.GenJournalID).Department ?? null;
                            incExp.DepartmentInCharge = ((FuelPumpRegister)cmdt.GenJournalID).DepartmentInCharge ?? null;
                            incExp.Expense = cmdt.Total;
                            //incExp.Income = cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //RFL	Receive (Fuel)	ReceiveFuel -> Receive.Details.ExpenseType (Fuel)
                    if (this.SourceType.Code == "RFL")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        foreach (ReceiptFuelDetail cmDetail in this.ReceiptFuelDetails)
                        {
                            ReceiptFuelDetail cmdt = this.Session.GetObjectByKey<ReceiptFuelDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            incExp.PayeeType = (this as ReceiptFuel).Vendor.ContactType;
                            incExp.Payee = (this as ReceiptFuel).Vendor;
                            incExp.Description1 = (this as ReceiptFuel).Memo;
                            incExp.Description2 = cmdt.Description;
                            ExpenseType defExpType = null;
                            if (cmdt.ExpenseType != null)
                            {
                                incExp.Category = cmdt.ExpenseType ?? null;
                                incExp.SubCategory = cmdt.SubExpenseType ?? null;
                            }
                            else
                            {
                                defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0040'")); // Fuel
                                incExp.Category = defExpType;
                            }

                            //incExp.SubCategory = "Not Applicable";
                            //incExp.CostCenter = cmdt.CostCenter ?? null;
                            switch (((ReceiptFuel)incExp.SourceID).TruckOrGenset)
                            {
                                case TruckOrGensetEnum.Truck:
                                    incExp.Fleet = ((ReceiptFuel)incExp.SourceID).TruckNo ?? null;
                                    break;
                                case TruckOrGensetEnum.Genset:
                                    incExp.Fleet = ((ReceiptFuel)incExp.SourceID).GensetNo ?? null;
                                    break;
                                case TruckOrGensetEnum.NotApplicable:
                                    break;
                                case TruckOrGensetEnum.Other:
                                    incExp.Fleet = ((ReceiptFuel)incExp.SourceID).OtherVehicle ?? null;
                                    break;
                                default:
                                    break;
                            }
                            incExp.Expense = cmdt.Total;
                            //incExp.Income = cmdt.Total;
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            }
                            else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //SH	Shunting	ShuntingEntry -> NetBilling -> ExpenseType (Trucking)
                    if (this.SourceType.Code == "SH")
                    {
                        //this.CantRegIncExpReason = "Pending Implementation";
                        ////this.IsIncomeExpReg = true;
                        //this.IsIncExpNeedUpdate = false;
                        //this.Save();
                        //return;
                        //if (!this.Approved)
                        //{
                        //    throw new ApplicationException("Shunting not yet invoiced! Cannot post to Income And Expense 02 registry.");
                        //}
                        IncomeAndExpense02 incExp = null;
                        incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as ShuntingEntry).TripNo)));
                        if (incExp == null)
                        {
                            incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp.EntryDate = this.EntryDate;
                        incExp.SourceID = this;
                        incExp.SourceType = this.SourceType;
                        incExp.SourceNo = this.SourceNo;
                        incExp.Seq = this.EntryDate.ToUniversalTime();
                        incExp.RefID = (this as ShuntingEntry).TripNo;
                        incExp.PayeeType = (this as ShuntingEntry).Customer.ContactType;
                        incExp.Payee = (this as ShuntingEntry).Customer;
                        incExp.Description1 = (this as ShuntingEntry).Memo;
                        incExp.Description2 = this.Description;
                        ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0005'"));
                        incExp.Category = defExpType;
                        //incExp.SubCategory = gjdt.SubExpenseType;
                        //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //incExp.Expense = gjdt.DebitAmount;
                        if (((ShuntingEntry)incExp.SourceID).TruckNo != null)
                        {
                            incExp.Fleet = ((ShuntingEntry)incExp.SourceID).TruckNo;
                        }
                        incExp.Income = (this as ShuntingEntry).GrossBilling;
                        if (incExp.Category != null)
                        {
                            incExp.Save();
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            incExp.Delete();
                            this.CantRegIncExpReason = "Cannot register because expense type not specified";
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                    //ST	Stanfilco	StanfilcoTrip -> NetBilling -> ExpenseType (Trucking)
                    if (this.SourceType.Code == "ST")
                    {
                        IncomeAndExpense02 incExp = null;
                        incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, (this as StanfilcoTrip).DTRNo)));
                        if (incExp == null)
                        {
                            incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp.EntryDate = this.EntryDate;
                        incExp.SourceID = this;
                        incExp.SourceType = this.SourceType;
                        incExp.SourceNo = this.SourceNo;
                        incExp.Seq = this.EntryDate.ToUniversalTime();
                        incExp.RefID = (this as StanfilcoTrip).DTRNo;
                        incExp.PayeeType = (this as StanfilcoTrip).Customer.ContactType;
                        incExp.Payee = (this as StanfilcoTrip).Customer;
                        incExp.Description1 = (this as StanfilcoTrip).Memo;
                        incExp.Description2 = this.Description;
                        ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0002'"));
                        incExp.Category = defExpType;
                        //incExp.SubCategory = gjdt.SubExpenseType;
                        //incExp.CostCenter = gjdt.CostCenter ?? null;
                        //incExp.Expense = gjdt.DebitAmount;
                        if (((StanfilcoTrip)incExp.SourceID).TruckNo != null)
                        {
                            incExp.Fleet = ((StanfilcoTrip)incExp.SourceID).TruckNo;
                        }
                        if ((this as StanfilcoTrip).NewNetBilling > 0)
                        {
                            incExp.Income = (this as StanfilcoTrip).NewNetBilling;
                        }
                        else
                        {
                            incExp.Income = (this as StanfilcoTrip).NetBilling;
                        }
                        if (incExp.Category != null)
                        {
                            incExp.Save();
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        else
                        {
                            incExp.Delete();
                            this.CantRegIncExpReason = "Cannot register because expense type not specified";
                            this.IsIncExpNeedUpdate = false;
                        }

                        // Do the genset migration to dollar
                        StanfilcoTrip oStft = this.Session.GetObjectByKey<StanfilcoTrip>(((StanfilcoTrip)this).Oid);
                        // Delete genset entry IncomeExpense after Feb. 11, 2018
                        if (oStft.EntryDate > new DateTime(2018, 2, 11, 0, 0, 0) && oStft.GensetEntries.Count > 0)
                        {
                            foreach (GensetEntry gsnt in oStft.GensetEntries)
                            {
                                var iexp = this.Session.FindObject<IncomeAndExpense02>(BinaryOperator.Parse("[SourceID.Oid]=?", gsnt.Oid));
                                if (iexp != null)
                                {
                                    iexp.Delete();
                                }
                            }
                        }

                        IncomeAndExpense02 incExp2 = null;
                        incExp2 = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", oStft.Oid, string.Format("GS-{0}", oStft.DTRNo))));
                        if (incExp2 == null)
                        {
                            incExp2 = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                        }
                        incExp2.EntryDate = oStft.EntryDate;
                        incExp2.SourceID = oStft;
                        incExp2.SourceType = oStft.SourceType;
                        incExp2.SourceNo = oStft.SourceNo;
                        incExp2.Seq = oStft.EntryDate.ToUniversalTime();
                        incExp2.RefID = string.Format("GS-{0}", oStft.DTRNo);
                        incExp2.PayeeType = oStft.Customer.ContactType;
                        incExp2.Payee = oStft.Customer;
                        incExp2.Description1 = string.Format("{0} Genset Entry", oStft.SourceNo);
                        incExp2.Description2 = string.Format("{0} Genset Entry", oStft.SourceNo);
                        ExpenseType defExpType2 = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0004'"));
                        incExp2.Category = defExpType2;
                        if (((StanfilcoTrip)incExp2.SourceID).TruckNo != null)
                        {
                            incExp2.Fleet = ((StanfilcoTrip)incExp2.SourceID).TruckNo;
                        }
                        incExp2.Income = oStft.GensetNetBillingLCY;
                        if (incExp2.Category != null)
                        {
                            incExp2.Save();
                        }
                        else
                        {
                            incExp2.Delete();
                        }
                        oStft.MigratedToDollar = true;
                        oStft.Save();
                        this.Save();
                    }
                    //WO	Work Order	WorkOrder -> WorkOrderDetails.ExpenseType (IH Parts)
                    if (this.SourceType.Code == "WO")
                    {
                        //EntryDate, SourceID, SourceType, SourceNo, Seq, RefID, PayeeType, Payee,
                        //Description1, Description2, Category, SubCategory, CostCenter, Expense, Income
                        int invCnt = 0;
                        IncomeAndExpense02 incExp = null;
                        IncomeAndExpense02 incExpOffset = null;
                        foreach (WorkOrderItemDetail cmDetail in this.WorkOrderItemDetails)
                        {
                            WorkOrderItemDetail cmdt = this.Session.GetObjectByKey<WorkOrderItemDetail>(cmDetail.Oid);
                            incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", this.Oid, cmdt.Oid)));
                            if (incExp == null)
                            {
                                incExp = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                            }
                            incExp.EntryDate = this.EntryDate;
                            incExp.SourceID = this;
                            incExp.SourceType = this.SourceType;
                            incExp.SourceNo = this.SourceNo;
                            incExp.Seq = this.EntryDate.ToUniversalTime();
                            incExp.RefID = cmdt.Oid.ToString();
                            Contact defPayee = this.Session.FindObject<Contact>(CriteriaOperator.Parse("[No] = 'AC00001'"));
                            incExp.PayeeType = defPayee.ContactType;
                            incExp.Payee = defPayee;
                            incExp.Description1 = (this as WorkOrder).Memo;
                            incExp.Description2 = cmdt.Description;
                            // --->
                            ReceiptDetail rcptdt = null;
                            if (cmdt.RequestID != Guid.Empty && cmdt.RequisitionNo != null)
                            {
                                RequisitionWorksheet rws = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", cmdt.RequestID));
                                if (rws != null && rws.ReqCarryoutTransactions.Count > 0)
                                {
                                    SourceType sType = this.Session.FindObject<SourceType>(CriteriaOperator.Parse("[Code] = 'PO'"));
                                    var data = rws.ReqCarryoutTransactions.Where(o => o.SourceType == sType).FirstOrDefault();
                                    if (data != null)
                                    {
                                        // Find ReceiptDetail.PODetailID == data.LineNo
                                        PurchaseOrderDetail podt = this.Session.GetObjectByKey<PurchaseOrderDetail>(data.LineNo);
                                        if (podt!=null)
                                        {
                                            rcptdt = this.Session.FindObject<ReceiptDetail>(BinaryOperator.Parse("[PODetailID]=?", podt));
                                        }
                                    }
                                }
                            }
                            ExpenseType defExpType = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0070'"));
                            incExp.Category = cmdt.ExpenseType ?? defExpType;// defExpType;
                            incExp.SubCategory = cmdt.SubExpenseType ?? null;
                            incExp.CostCenter = cmdt.CostCenter ?? null;
                            if (incExp.CostCenter != null && incExp.CostCenter.FixedAsset != null)
                            {
                                incExp.Fleet = incExp.CostCenter.FixedAsset;
                            }
                            incExp.Facility = cmdt.Facility ?? null;
                            incExp.FacilityHead = cmdt.FacilityHead ?? null;
                            incExp.Department = cmdt.Department ?? null;
                            incExp.DepartmentInCharge = cmdt.DepartmentInCharge ?? null;
                            incExp.Expense = cmdt.Total;
                            //incExp.Income = cmdt.Total;
                            if (cmdt.ReceiptDetailID != null)
                            {
                                // Copy incExp to incExpOffset
                                incExpOffset = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}' And [Expense] < 0", this.Oid, cmdt.Oid)));
                                if (incExpOffset == null)
                                {
                                    incExpOffset = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                                }
                                incExpOffset.EntryDate = this.EntryDate;
                                incExpOffset.SourceID = this;
                                incExpOffset.SourceType = this.SourceType;
                                incExpOffset.SourceNo = this.SourceNo;
                                incExpOffset.Seq = this.EntryDate.ToUniversalTime();
                                incExpOffset.RefID = cmdt.Oid.ToString();
                                Contact defPayee2 = this.Session.FindObject<Contact>(CriteriaOperator.Parse("[No] = 'AC00001'"));
                                incExpOffset.PayeeType = defPayee2.ContactType;
                                incExpOffset.Payee = defPayee;
                                incExpOffset.Description1 = string.Format("Offset line#{0} of #{1}", cmdt.ReceiptDetailID.Oid, cmdt.ReceiptDetailID.ReceiptInfo.SourceNo);
                                incExpOffset.Description2 = cmdt.Description;
                                // --->
                                //ReceiptDetail rcptdt = null;
                                if (cmdt.RequestID != Guid.Empty && cmdt.RequisitionNo != null)
                                {
                                    RequisitionWorksheet rws = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", cmdt.RequestID));
                                    //if (rws != null && rws.ReqCarryoutTransactions.Count > 0)
                                    //{
                                    //    SourceType sType = this.Session.FindObject<SourceType>(CriteriaOperator.Parse("[Code] = 'PO'"));
                                    //    var data = rws.ReqCarryoutTransactions.Where(o => o.SourceType == sType).FirstOrDefault();
                                    //    if (data != null)
                                    //    {
                                    //        // Find ReceiptDetail.PODetailID == data.LineNo
                                    //        rcptdt = this.Session.GetObjectByKey<ReceiptDetail>(data.LineNo);
                                    //    }
                                    //}
                                }
                                ExpenseType defExpType2 = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0011'"));
                                incExpOffset.Category = cmdt.ReceiptDetailID.ExpenseType ?? defExpType2;// defExpType;
                                incExpOffset.SubCategory = cmdt.ReceiptDetailID.SubExpenseType ?? null;
                                incExpOffset.CostCenter = cmdt.ReceiptDetailID.CostCenter ?? null;
                                if (incExpOffset.CostCenter != null && incExpOffset.CostCenter.FixedAsset != null)
                                {
                                    incExpOffset.Fleet = incExp.CostCenter.FixedAsset;
                                }
                                incExpOffset.Facility = cmdt.Facility ?? null;
                                incExpOffset.FacilityHead = cmdt.FacilityHead ?? null;
                                incExpOffset.Department = cmdt.Department ?? null;
                                incExpOffset.DepartmentInCharge = cmdt.DepartmentInCharge ?? null;
                                incExpOffset.Expense = 0 - cmdt.Total;
                                if (incExpOffset.Category != null)
                                {
                                    incExpOffset.Save();
                                }
                                else
                                {
                                    incExpOffset.Delete();
                                }
                            }
                            else if (rcptdt != null)
                            {
                                // Copy incExp to incExpOffset
                                incExpOffset = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}' And [Expense] < 0", this.Oid, cmdt.Oid)));
                                if (incExpOffset == null)
                                {
                                    incExpOffset = ReflectionHelper.CreateObject<IncomeAndExpense02>(this.Session);
                                }
                                incExpOffset.EntryDate = this.EntryDate;
                                incExpOffset.SourceID = this;
                                incExpOffset.SourceType = this.SourceType;
                                incExpOffset.SourceNo = this.SourceNo;
                                incExpOffset.Seq = this.EntryDate.ToUniversalTime();
                                incExpOffset.RefID = cmdt.Oid.ToString();
                                Contact defPayee2 = this.Session.FindObject<Contact>(CriteriaOperator.Parse("[No] = 'AC00001'"));
                                incExpOffset.PayeeType = defPayee2.ContactType;
                                incExpOffset.Payee = defPayee;
                                incExpOffset.Description1 = string.Format("Offset line#{0} of #{1}", rcptdt.Oid, rcptdt.ReceiptInfo.SourceNo);
                                incExpOffset.Description2 = cmdt.Description;
                                // --->
                                //ReceiptDetail rcptdt = null;
                                if (cmdt.RequestID != Guid.Empty && cmdt.RequisitionNo != null)
                                {
                                    RequisitionWorksheet rws = this.Session.FindObject<RequisitionWorksheet>(CriteriaOperator.Parse("[RowID] = ?", cmdt.RequestID));
                                    //if (rws != null && rws.ReqCarryoutTransactions.Count > 0)
                                    //{
                                    //    SourceType sType = this.Session.FindObject<SourceType>(CriteriaOperator.Parse("[Code] = 'PO'"));
                                    //    var data = rws.ReqCarryoutTransactions.Where(o => o.SourceType == sType).FirstOrDefault();
                                    //    if (data != null)
                                    //    {
                                    //        // Find ReceiptDetail.PODetailID == data.LineNo
                                    //        rcptdt = this.Session.GetObjectByKey<ReceiptDetail>(data.LineNo);
                                    //    }
                                    //}
                                }
                                ExpenseType defExpType2 = this.Session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0011'"));
                                incExpOffset.Category = rcptdt.ExpenseType ?? defExpType2;// defExpType;
                                incExpOffset.SubCategory = rcptdt.SubExpenseType ?? null;
                                incExpOffset.CostCenter = rcptdt.CostCenter ?? null;
                                if (incExpOffset.CostCenter != null && incExpOffset.CostCenter.FixedAsset != null)
                                {
                                    incExpOffset.Fleet = incExp.CostCenter.FixedAsset;
                                }
                                incExpOffset.Facility = cmdt.Facility ?? null;
                                incExpOffset.FacilityHead = cmdt.FacilityHead ?? null;
                                incExpOffset.Department = cmdt.Department ?? null;
                                incExpOffset.DepartmentInCharge = cmdt.DepartmentInCharge ?? null;
                                incExpOffset.Expense = 0 - cmdt.Total;
                                if (incExpOffset.Category != null)
                                {
                                    incExpOffset.Save();
                                }
                                else
                                {
                                    incExpOffset.Delete();
                                }
                            }
                            if (incExp.Category != null)
                            {
                                incExp.Save();
                            } else
                            {
                                invCnt++;
                                incExp.Delete();
                            }
                        }
                        if (invCnt > 0)
                        {
                            this.CantRegIncExpReason = string.Format("{0} line cannot be registered because expense type not specified", invCnt);
                            this.IsIncExpNeedUpdate = false;
                        } else
                        {
                            this.IsIncomeExpReg = true;
                            this.IsIncExpNeedUpdate = false;
                        }
                        this.Save();
                    }
                } else
                {
                }
            }
            //this.RevalidatedId = string.Format("{0}{1}{2}{3}{4}", DateTime.Now.Year, DateTime.Now.Month.ToString("D2"), DateTime.Now.Day.ToString("D2"), DateTime.Now.Hour.ToString("D2"), DateTime.Now.Minute.ToString("D2"));
            //this.Save();
            this.Session.CommitTransaction();
        }

        //protected override void OnSaved() {
        //    bool shouldReturn;
        //    AutoRegisterIncomeExpenseVer(out shouldReturn);
        //    if (shouldReturn)
        //        return;
        //    base.OnSaved();
        //}

        private int deletedOid;
        protected override void OnDeleting() {
            // --- ADD THIS BLOCK ---
            if (IsSynced)
            {
                throw new UserFriendlyException("This record has already been synced to the external system and cannot be deleted. Please void or cancel the transaction instead.");
            }
            // ----------------------
            deletedOid = this.Oid;
            base.OnDeleting();
            //if (_ForceDelete)
            //{
            //    foreach (object obj in Session.CollectReferencingObjects(this))
            //    {
            //        foreach (XPMemberInfo property in Session.GetClassInfo(obj).PersistentProperties)
            //        {
            //            if (property.MemberType.IsAssignableFrom(this.GetType()))
            //            {
            //                property.SetValue(obj, null);
            //            }
            //        }
            //    }
            //}
        }

        //protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        //{
        //    this.IsIncExpNeedUpdate = true;
        //    base.TriggerObjectChanged(args);
        //}

        #endregion

        #region Trip Details

        // Trip Reference No.
        [DisplayName("TREF#")]
        public string TripReferenceNo
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).DTRNo;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).DocumentNo;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).TripNo;
                    }
                }
                return string.Empty;
            }
        }

        public Employee TripDriver
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).Driver;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).Driver;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).Driver;
                    }
                }
                return null;
            }
        }

        public FATruck TripUnit
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).TruckNo;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).TruckNo;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).TruckNo;
                    }
                }
                return null;
            }
        }

        public TripLocation TripOrigin
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).Origin;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).Origin;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).Origin;
                    }
                }
                return null;
            }
        }

        public TripLocation TripDestination
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).Destination;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).Destination;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).Destination;
                    }
                }
                return null;
            }
        }

        public decimal TripTruckerPay
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).TruckerPay;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).AmountTruck ?? 0;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).TruckerPay;
                    }
                }
                return 0m;
            }
        }

        public decimal TripNetBilling
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        if ((this as StanfilcoTrip).TripCalculationDetails.Count>0)
                        {
                            return (this as StanfilcoTrip).NewNetBilling;
                        }
                        else
                        {
                            return (this as StanfilcoTrip).NetBilling;
                        }
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).NetBilling;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).NetBilling;
                    }
                }
                return 0m;
            }
        }
        public string FuelUsageDescriptionForReceipt
        {
            get
            {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        Tariff tar = (this as StanfilcoTrip).Tariff;
                        return tar != null ? string.Format("{0}|{1}->{2}", tar.Zone.Code, tar.Origin.Code, tar.Destination.Code) : string.Empty;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        Tariff tar = (this as DolefilTrip).Tariff;
                        return tar != null ? string.Format("{0}|{1}->{2}", tar.Zone.Code, tar.Origin.Code, tar.Destination.Code) : string.Empty;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        Tariff tar = (this as OtherTrip).Tariff;
                        return tar != null ? string.Format("{0}|{1}->{2}", tar.Zone.Code, tar.Origin.Code, tar.Destination.Code) : string.Empty;
                    }
                }
                return string.Empty;
            }
        }
        // Trip Customer
        //public Customer TripCustomer
        //{
        //    get
        //    {
        //        if (_SourceType != null && _OperationType != null)
        //        {
        //            if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
        //            {
        //                return (this as StanfilcoTrip).Customer;
        //            }
        //            if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
        //            {
        //                return (this as DolefilTrip).Customer;
        //            }
        //            if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
        //            {
        //                return (this as OtherTrip).Customer;
        //            }
        //        }
        //        return null;
        //    }
        //}

        // Trip State
        [DisplayName("STATUS")]
        public TripStatusEnum TripState
        {
            get {
                if (_SourceType != null && _OperationType != null)
                {
                    if (_SourceType.Code == "ST" && _OperationType.Code == "ST")
                    {
                        return (this as StanfilcoTrip).Status;
                    }
                    if (_SourceType.Code == "DF" && _OperationType.Code == "DF")
                    {
                        return (this as DolefilTrip).Status;
                    }
                    if (_SourceType.Code == "OT" && _OperationType.Code == "OT")
                    {
                        return (this as OtherTrip).Status;
                    }
                }
                return TripStatusEnum.Current; }
        }

        #endregion
        
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

        //[System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        //[System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        #region "Calculated Details"

        [Persistent("CreditBalance")]
        private decimal? _CreditBalance = null;
        [PersistentAlias("_CreditBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal? CreditBalance {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _CreditBalance == null)
                    {
                        UpdateCreditBalance(false);
                    }
                } catch (Exception)
                {
                }
                return _CreditBalance;
            }
        }

        public void UpdateCreditBalance(bool forceChangeEvent) {
            decimal? oldCreditBalance = _CreditBalance;
            decimal tempTotal = 0m;
            foreach (GenJournalDetail detail in GenJournalDetails)
            {
                tempTotal +=
                detail.CreditAmount;
            }
            _CreditBalance = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("CreditBalance", oldCreditBalance,
                _CreditBalance);
            }
            ;
        }

        [Persistent("DebitBalance")]
        private decimal? _DebitBalance = null;
        [PersistentAlias("_DebitBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal? DebitBalance {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _DebitBalance == null)
                    {
                        UpdateDebitBalance(false);
                    }
                } catch (Exception)
                {
                }
                return _DebitBalance;
            }
        }

        public void UpdateDebitBalance(bool forceChangeEvent) {
            decimal? oldDebitBalance = _DebitBalance;
            decimal tempTotal = 0m;
            foreach (GenJournalDetail detail in GenJournalDetails)
            {
                tempTotal +=
                detail.DebitAmount;
            }
            _DebitBalance = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("DebitBalance", oldDebitBalance,
                _DebitBalance);
            }
            ;
        }

        #endregion
        //[Action(AutoCommit=true,Caption="Mark to Validate")]
        //public void MarkToValidateIncExp(){
        //    this.IsIncExpNeedUpdate = true;
        //}
        #region Boolean Validation

        public bool IsBalance {
            get { return DebitBalance == CreditBalance; }
        }

        #endregion

        public GenJournalHeader(Session session)
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

        [Custom("AllowEdit", "False")]
        public bool EntryDateAccepted
        {
            get { return _EntryDateAccepted; }
            set { SetPropertyValue<bool>("EntryDateAccepted", ref _EntryDateAccepted, value); }
        }
        [NonPersistent]
        public Company CompanyInfoHead
        {
            get
            {
                return Company.GetInstance(Session);
            }
        }
        protected override void OnSaving() {
            var admin = CurrentUser.Roles.Where(o=>o.Name=="Administrator").FirstOrDefault();
            if (CompanyInfoHead.VerifyEntryDate && admin == null)
            {
                var t = DateTime.Now.Date - _EntryDate.Date;
                double d = t.TotalDays;
                if (!_EntryDateAccepted && d > 365)
                {
                    string msg = string.Format("Entry date {0} in {1} is more than 365 days late. Did you make a mistake?", _EntryDate.ToShortDateString(), _SourceNo);
                    if (XtraMessageBox.Show(msg, "Confirm Entry Date",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        throw new UserFriendlyException("Entry date is a mistake");
                    }
                    else
                    {
                        EntryDateAccepted = true;
                    }
                }
                var t2 = DateTime.Now.Date - _EntryDate.Date;
                double d2 = t2.TotalDays;
                if (!_EntryDateAccepted && d2 < 0)
                {
                    string msg = string.Format("You have entered {0} advanced entry date in {1}. Did you make a mistake?", _EntryDate.ToShortDateString(), _SourceNo);
                    if (XtraMessageBox.Show("You have entered advanced entry date. Did you make a mistake?", "Confirm Entry Date",
                        System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question)
                        == DialogResult.Yes)
                    {
                        throw new UserFriendlyException("Entry date is a mistake");
                    }
                    else
                    {
                        EntryDateAccepted = true;
                    }
                }
            }
            
            if (IsDeleted)
            {
                IncomeAndExpense02 incExp = null;
                //incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = '{1}'", deletedOid, this.Oid)));
                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0}", this.Oid)));
                if (incExp != null)
                {
                    incExp.Delete();
                }
            }
            //else
            //{
            //    this.IsIncExpNeedUpdate = true;
            //}

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
        private bool _EntryDateAccepted;
        private string _RevalidatedId;
        private bool _PdcCleared = false;
        private bool _IsSynced = false;
        private string _CreatedByAppwriteUserId;
        private string _StatusByAppwriteUserId;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser {
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

        private void Reset() {
            _DebitBalance = null;
            _CreditBalance = null;
        }
    }
}
