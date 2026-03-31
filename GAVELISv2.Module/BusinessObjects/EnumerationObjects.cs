using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
namespace GAVELISv2.Module.BusinessObjects {

    #region Costing Methods

    public enum CostingMethodEnum {
        Standard,
        [DisplayName("Average Cost")]
        Average,
        [DisplayName("FIFO (first in, first out)")]
        FIFO,
        [DisplayName("LIFO (last in, first out)")]
        LIFO
    }

    #endregion

    #region Debit or Credit

    public enum DebitCreditEnum {
        [DisplayName("*")]
        None,
        Debit,
        Credit
    }

    #endregion

    #region Cash Flow Operator

    public enum CashOpeEnum {
        [DisplayName("*")]
        None,
        Plus,
        Less
    }

    #endregion

    #region Balance or Income

    public enum BalanceOrIncomeType {
        BalanceSheet,
        IncomeStatement,
        None
    }

    #endregion

    #region Balance Section

    public enum BalanceSectionEnum {
        None,
        Asset,
        [DisplayName("Liabilities & Equity")]
        LiabilitiesAndEquity
    }

    #endregion

    #region Contact Type

    public enum ContactTypeEnum {
        Customer,
        Vendor,
        Payee,
        Employee
    }

    #endregion

    #region Employment Status

    public enum EmploymentStatusEnum {
        Trainee,
        Probationary,
        Regular,
        Contract,
        Terminated
    }

    #endregion

    #region Item Types

    public enum ItemTypeEnum {
        [DisplayName("Inventory Item")]
        InventoryItem,
        [DisplayName("Service Item")]
        ServiceItem,
        [DisplayName("Product Group")]
        ProductGroup,
        [DisplayName("Assembly Item")]
        AssemblyItem,
        [DisplayName("Fixed Asset")]
        FixedAsset,
        [DisplayName("Charge Item")]
        ChargeItem,
        [DisplayName("Fuel Item")]
        FuelItem,
        [DisplayName("Repair & Maintenance")]
        RepairItem,
        [DisplayName("Tire Item")]
        TireItem
    }

    #endregion

    #region Item Class

    public enum ItemClassEnum {
        None,
        Mechanical,
        Electrical,
        Chemicals,
        [DisplayName("Cleaning Tools")]
        CleaningTools,
        [DisplayName("Construction Materials")]
        ConstructionMaterials,
        Lubricant,
        Tools,
        Equipment,
        Fuel,
        Paint,
        Cement,
        [DisplayName("Plumbing Materials")]
        PlumbingMaterials
    }

    #endregion

    #region Tire Items Classes

    public enum TireItemClassEnum {
        None = 0,
        [DisplayName("Brand New Tire")]
        BrandNewTire = 10,
        [DisplayName("Secondhand Tire")]
        SecondHandTire = 20,
        [DisplayName("Recapped Tire")]
        RecappedTire = 30,
        Flap = 40,
        Tube = 50,
        Rim = 60,
        [DisplayName("Scrapped Tire")]
        ScrappedTire = 70,
        [DisplayName("Original Tire")]
        OriginalTire = 80
    }

    #endregion
    #region Tire Physical Status
    public enum TirePhysicalStatusEnum
    {
        Available,
        InUse,
        Scrap,
        Sold,
        Lost
    }
    #endregion

    #region Fixed Asset Class

    public enum FixedAssetClassEnum {
        [DisplayName("Land and Building")]
        LandAndBuilding,
        Truck,
        Trailer,
        [DisplayName("Generator Set")]
        GeneratorSet,
        [DisplayName("Other Vehicle")]
        OtherVehicle,
        [DisplayName("Other Fixed Asset")]
        Other
    }

    #endregion

    #region FA Acquisition State

    public enum AcquisitionState {
        Old,
        New
    }

    #endregion

    #region Real Property Classification

    public enum RealPropertClassEnum {
        Residential,
        Commercial,
        Agriculture
    }

    #endregion

    #region Amount or Rate

    public enum AmountOrRateEnum {
        Amount,
        Rate
    }

    #endregion

    #region Journal Entry Status

    public enum JournalEntryStatusEnum {
        Current,
        Approved
    }

    #endregion

    #region Physical Adjustment Status

    public enum PhysicalAdjustmentStatusEnum {
        Current,
        Approved,
        Applied
    }

    #endregion

    #region Serial No Status

    public enum SerialNoStatusEnum {
        Available,
        Sold,
        Removed,
        Reserved,
        Used
    }

    #endregion

    #region Purchase Order Status

    public enum PurchaseOrderStatusEnum {
        Current,
        Approved,
        [DisplayName("Partially Received")]
        PartiallyReceived,
        Received,
        Disapproved,
        Pending
    }

    #endregion

    #region Receipt Status

    public enum ReceiptStatusEnum {
        Current,
        Received,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid,
        [DisplayName("Partially Returned")]
        PartiallyReturned,
        Returned
    }

    #endregion

    #region Job Order Status

    public enum JobOrderStatusEnum {
        Current,
        Confirmed,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region Debit Memo Status

    public enum DebitMemoStatusEnum {
        Current,
        Returned,
        [DisplayName("Partially Applied")]
        PartiallyApplied,
        Applied
    }

    #endregion

    #region Sales Order Status

    public enum SalesOrderStatusEnum {
        Current,
        Approved,
        [DisplayName("Partially Invoiced")]
        PartiallyInvoiced,
        Invoiced
    }

    #endregion

    #region Invoice Status

    public enum InvoiceStatusEnum {
        Current,
        Invoiced,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid,
        [DisplayName("Partially Returned")]
        PartiallyReturned,
        Returned
    }

    #endregion

    #region Invoice Type

    public enum InvoiceTypeEnum {
        [DisplayName("Order Slip")]
        OrderSlip,
        Charge,
        Cash,
        None,
        [DisplayName("Delivery Receipt")]
        DeliveryReceipt
    }

    #endregion

    #region Credit Memo Status

    public enum CreditMemoStatusEnum {
        Current,
        Returned,
        [DisplayName("Partially Applied")]
        PartiallyApplied,
        Applied
    }

    #endregion

    #region Bill Status

    public enum BillStatusEnum {
        Current,
        Approved,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region Check Status

    public enum CheckStatusEnum {
        Current,
        Approved,
        Voided
    }

    #endregion

    #region Multi Check Status

    public enum MultiCheckStatusEnum {
        Current,
        Approved,
        Released
    }

    #endregion

    #region Collection Status

    public enum CollectionStatusEnum {
        Current,
        Received
    }

    #endregion

    #region Payment Status

    public enum PaymentStatusEnum {
        Current,
        Approved,
        Voided
    }

    #endregion

    #region Trip Status

    public enum TripStatusEnum {
        Current,
        Released,
        Invoiced,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region Payroll Batch Status

    public enum PayrollBatchStatusEnum {
        Current,
        Approved,
        Released,
        Posted
    }

    #endregion

    #region Truck Registry Status

    public enum TruckRegistryStatusEnum {
        Current,
        Approved
    }

    #endregion

    #region Driver Registry Status

    public enum DriverRegistryStatusEnum {
        Current,
        Approved,
        Processed,
        Paid
    }

    #endregion

    #region Shunting Status

    public enum ShuntingStatusEnum {
        Current,
        Invoiced,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region Genset Status

    public enum GensetStatusEnum {
        Current,
        Invoiced,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region KD Status

    public enum KDStatusEnum {
        Current,
        Invoiced,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region Purchase Order Fuel Status

    public enum PurchaseOrderFuelStatusEnum {
        Current,
        Approved,
        [DisplayName("Partially Received")]
        PartiallyReceived,
        Received,
        Disapproved,
        Pending
    }

    #endregion

    #region Truck or Genset

    public enum TruckOrGensetEnum {
        Truck,
        Genset,
        NotApplicable,
        Other
    }

    #endregion

    #region Receipt Fuel Status

    public enum ReceiptFuelStatusEnum {
        Current,
        Received,
        [DisplayName("Partially Paid")]
        PartiallyPaid,
        Paid
    }

    #endregion

    #region Work Order Status

    public enum WorkOrderStatusEnum {
        Current,
        [DisplayName("Checked In")]
        CheckedIn,
        [DisplayName("Waiting for Mechanic")]
        WaitingForMechanic,
        [DisplayName("Waiting for Parts")]
        WaitingForParts,
        [DisplayName("In Progress")]
        InProgress,
        [DisplayName("Checked Out")]
        CheckedOut,
        Invoiced
    }

    #endregion

    #region Genset Usage Types

    public enum GensetUsageTypeEnum {
        Regular,
        Coldroom,
        Others
    }

    #endregion

    #region Shunting Type

    public enum ShuntingTypeEnum {
        Shunting,
        Excess,
        Additional
    }

    #endregion

    #region Payment Type

    public enum PaymentTypeEnum {
        Check,
        Cash,
        [DisplayName("Wire Transfer")]
        WireTransfer,
        Others
    }

    #endregion

    #region Deposit State

    public enum DepositStatusEnum {
        Current,
        Approved,
        Deposited
    }

    #endregion

    #region Fund Transfer State

    public enum FundTransferStatusEnum {
        Current,
        Approved,
        Transferred
    }

    #endregion

    #region Bank Reconciliation State

    public enum BankReconStatusEnum {
        Current,
        Approved,
        Reconciled
    }

    #endregion

    #region Fuel Efficiency Entry Type

    public enum FEMEntryTypeEnum {
        [DisplayName("Initial Reading")]
        Initial,
        [DisplayName("Odometer Changed")]
        Change,
        [DisplayName("Gas Up Register")]
        Fuel
    }

    #endregion

    #region Text Date Filter

    public enum TextDateRangeType {
        [DisplayName("Year-To-Date")]
        YearToDate,
        PreviousYear,
        [DisplayName("Month-To-Date")]
        MonthToDate,
        PreviousMonth,
        Custom
    }

    #endregion

    #region Deduction Type

    public enum DeductionType {
        Premium,
        Loan,
        Tax,
        Other
    }

    #endregion

    #region Months

    public enum MonthsEnum {
        [DisplayName("")]
        None,
        January,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    #endregion

    #region Employee Pay Types

    public enum EmployeePayTypeEnum {
        Hourly,
        Daily,
        Monthly
    }

    #endregion

    #region Holiday Type

    public enum HolidayTypeEnum {
        Special,
        Regular,
        None,
        Double
    }

    #endregion

    #region Attendance Records Status

    public enum AttendanceRecordStatusEnum {
        Open,
        Processed,
        Posted
    }

    #endregion

    #region Days Of The Week

    public enum DaysOfTheWeekEnum {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    #endregion

    #region Order Types

    public enum OrderTypeEnum {
        Purchase,
        Sale
    }

    #endregion

    #region Transfer Order Status

    public enum TransferOrderStatusEnum {
        Current,
        Approved,
        Completed
    }

    #endregion

    #region Requisition Status

    public enum RequisitionStatusEnum {
        Current,
        Approved,
        Cancelled,
        Served
    }

    #endregion

    #region Requisition Actions

    public enum RequisitionActionsEnum {
        [DisplayName("Purchase Order")]
        PurchaseOrder,
        [DisplayName("Transfer Order")]
        TransferOrder,
        [DisplayName("Sales Order")]
        SalesOrder,
        [DisplayName("Work Order")]
        WorkOrder,
        [DisplayName("Employee Charge Slip")]
        EmployeeChargeSlip,
        [DisplayName("Fuel Order")]
        FuelOrder
    }

    #endregion

    #region Requisition Worksheet Status

    public enum RequisitionWSStateEnum {
        Open,
        Active,
        Completed
    }

    #endregion

    #region Adjust Item Cost Prices Mode

    public enum AdjustItemCostPricesEnum {
        [DisplayName("From Template")]
        FromTemplate,
        [DisplayName("Input Rate")]
        InputRate
    }

    #endregion

    #region Meter Log Types

    public enum MeterLogTypeEnum {
        Initial,
        Log,
        Change,
        Correct,
        Fuel,
        Service,
        None
    }

    #endregion

    #region Meter Entry Type

    public enum MeterEntryTypeEnum {
        Odometer,
        VFuel,
        GFuel,
        VService,
        GService
    }

    #endregion

    #region Schedule Type

    public enum ScheduleTypeEnum {
        Meter,
        Date,
        [DisplayName("Which is First")]
        FirstCome
    }

    #endregion

    #region Preventive Maintenance Status

    public enum PreventiveMaintStatusEnum
    {
        Due = 0,
        AlmostDue = 1,
        InProgress = 2,
        Good = 3
    }
    #endregion
    #region Trucking Operation Types

    public enum TruckingOperationType {
        [DisplayName("Income From Trips")]
        IncomeFromTrips = 10,
        [DisplayName("Income From Trailers")]
        IncomeFromTrailers = 20,
        [DisplayName("Income From Shuntings")]
        IncomeFromShunting = 30,
        [DisplayName("Income From Gensets")]
        IncomeFromGensets = 40,
        [DisplayName("Income From KDS")]
        IncomeFromKDS = 50,
        [DisplayName("Expenses From Fuel")]
        ExpensesFromFuel = 60,
        [DisplayName("Expenses From Spare Sparts")]
        ExpensesFromSpareParts = 70,
        [DisplayName("Expenses From Job Orders")]
        ExpensesFromJobOrders = 80,
        [DisplayName("Expenses From Tires")]
        ExpensesFromTires = 90,
        [DisplayName("Expenses From Batteries")]
        ExpensesFromBattery = 100
    }

    #endregion

    #region Fuel Usage Classification

    public enum FuelUsageClassEnum {
        None,
        Operation,
        Income
    }

    #endregion

    public enum PaymentTenderedTypeEnum {
        Cash,
        Check,
        Memo
    }

    public enum TypeOfEmployeeChargesEnum {
        ToolsRequest,
        ItemRequest,
        PartsDamage,
        TireDamage,
        FlapDamage,
        TubesDamage,
        RimDamage,
        PartLost,
        TireLost,
        BatteryLost,
        BatteryDamage,
        CargoDamage,
        CargoLoss,
        VulcateCharge,
        FuelChargeTractor,
        AccidentCharge,
        Violation,
        HealthAndServices,
        FuelChargeGenset,
        ItemLost,
        FuelCharge,
        Uniform
    }

    public enum EmployeeChargeSlipStatusEnum {
        Current,
        Approved,
        ForwardedToPayroll
    }

}