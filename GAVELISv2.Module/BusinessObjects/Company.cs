using System;
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
    [System.ComponentModel.DisplayName("Company Information")]
    public class Company : BaseObject {
        private string _Name;
        private string _Phone;
        private string _Fax;
        private string _WebAddress;
        private string _Email;
        private CostingMethodEnum _InventoryCostingMethod;
        private Account _TemporaryExpenseAcct;
        private Account _PurchaseDiscountAcct;
        private Account _SalesDiscountIHAcct;
        private Account _UndepositedCollectionAcct;
        private SalesTaxCode _DefaultSalesTax;
        private SalesTaxCode _DefaultNonSalesTax;
        private Terms _DefaultCashTerm;
        private Account _ProvisionForIncomeTaxAcct;
        private Account _OutputVATAcct;
        private Account _InputVATAcct;
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Company Name")]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        public string Phone {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        public string Fax {
            get { return _Fax; }
            set { SetPropertyValue("Fax", ref _Fax, value); }
        }

        public string WebAddress {
            get { return _WebAddress; }
            set { SetPropertyValue("WebAddress", ref _WebAddress, value); }
        }

        [DisplayName("E-mail")]
        public string Email {
            get { return _Email; }
            set { SetPropertyValue("Email", ref _Email, value); }
        }

        public CostingMethodEnum InventoryCostingMethod {
            get { return _InventoryCostingMethod; }
            set { SetPropertyValue("InventoryCostingMethod", ref
                _InventoryCostingMethod, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account TemporaryExpenseAcct {
            get { return _TemporaryExpenseAcct; }
            set { SetPropertyValue("TemporaryExpenseAcct", ref
                _TemporaryExpenseAcct, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account PurchaseDiscountAcct {
            get { return _PurchaseDiscountAcct; }
            set { SetPropertyValue("PurchaseDiscountAcct", ref
                _PurchaseDiscountAcct, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account SalesDiscountIHAcct {
            get { return _SalesDiscountIHAcct; }
            set { SetPropertyValue("SalesDiscountIHAcct", ref
                _SalesDiscountIHAcct, value); }
        }

        public Account UndepositedCollectionAcct {
            get { return _UndepositedCollectionAcct; }
            set { SetPropertyValue("UndepositedCollectionAcct", ref
                _UndepositedCollectionAcct, value); }
        }

        public SalesTaxCode DefaultSalesTax {
            get { return _DefaultSalesTax; }
            set { SetPropertyValue("DefaultSalesTax", ref _DefaultSalesTax,
                value); }
        }

        public SalesTaxCode DefaultNonSalesTax {
            get { return _DefaultNonSalesTax; }
            set { SetPropertyValue("DefaultNonSalesTax", ref _DefaultNonSalesTax
                , value); }
        }

        public Terms DefaultCashTerm {
            get { return _DefaultCashTerm; }
            set { SetPropertyValue("DefaultCashTerm", ref _DefaultCashTerm,
                value); }
        }

        public Account ProvisionForIncomeTaxAcct {
            get { return _ProvisionForIncomeTaxAcct; }
            set { SetPropertyValue("ProvisionForIncomeTaxAcct", ref _ProvisionForIncomeTaxAcct, value); }
        }

        public Account OutputVATAcct {
            get { return _OutputVATAcct; }
            set { SetPropertyValue<Account>("OutputVATAcct", ref _OutputVATAcct, value); }
        }

        public Account InputVATAcct {
            get { return _InputVATAcct; }
            set { SetPropertyValue<Account>("InputVATAcct", ref _InputVATAcct, value); }
        }

        #region Bill to Address

        private const string defaultBillAddressFormat =
        "{BillAddress}, {BillZipCode} {BillCity}, {BillProvince}, {BillCountry}"
        ;

        private string _BillAddress;
        private string _BillCity;
        private string _BillZipCode;
        private string _BillProvince;
        private string _BillCountry = "Philippines";
        [Size(500)]
        public string BillAddress {
            get { return _BillAddress; }
            set { SetPropertyValue("BillAddress", ref _BillAddress, value); }
        }

        public string BillCity {
            get { return _BillCity; }
            set { SetPropertyValue("BillCity", ref _BillCity, value); }
        }

        public string BillZipCode {
            get { return _BillZipCode; }
            set { SetPropertyValue("BillZipCode", ref _BillZipCode, value); }
        }

        public string BillProvince {
            get { return _BillProvince; }
            set { SetPropertyValue("BillProvince", ref _BillProvince, value); }
        }

        public string BillCountry {
            get { return _BillCountry; }
            set { SetPropertyValue("BillCountry", ref _BillCountry, value); }
        }

        public string FullBillAddress {
            get { return ObjectFormatter.Format(
                defaultBillAddressFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        #region Ship to Address

        private const string defaultShipAddressFormat =
        "{ShipAddress}, {ShipZipCode} {ShipCity}, {ShipProvince}, {ShipCountry}"
        ;

        private string _ShipAddress;
        private string _ShipCity;
        private string _ShipZipCode;
        private string _ShipProvince;
        private string _ShipCountry = "Philippines";
        [Size(500)]
        public string ShipAddress {
            get { return _ShipAddress; }
            set { SetPropertyValue("ShipAddress", ref _ShipAddress, value); }
        }

        public string ShipCity {
            get { return _ShipCity; }
            set { SetPropertyValue("ShipCity", ref _ShipCity, value); }
        }

        public string ShipZipCode {
            get { return _ShipZipCode; }
            set { SetPropertyValue("ShipZipCode", ref _ShipZipCode, value); }
        }

        public string ShipProvince {
            get { return _ShipProvince; }
            set { SetPropertyValue("ShipProvince", ref _ShipProvince, value); }
        }

        public string ShipCountry {
            get { return _ShipCountry; }
            set { SetPropertyValue("ShipCountry", ref _ShipCountry, value); }
        }

        public string FullShipAddress {
            get { return ObjectFormatter.Format(
                defaultShipAddressFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        #region Picture

        [Size(SizeAttribute.Unlimited),
        Delayed(true),
        ValueConverter(typeof(ImageValueConverter))]
        [NonCloneable]
        public System.Drawing.Image Logo {
            get { return GetDelayedPropertyValue<System.Drawing.Image>("Logo"); }
            set { SetDelayedPropertyValue<System.Drawing.Image>("Logo", value); }
        }

        #endregion

        #region Attendance Setup

        private DayOfWeek _RestDay = DayOfWeek.Sunday;
        public DayOfWeek RestDay
        {
            get { return _RestDay; }
            set { SetPropertyValue("RestDay", ref _RestDay, value); }
        }
        private decimal _RestDayRate = 130m;
        public decimal RestDayRate
        {
            get { return _RestDayRate; }
            set { SetPropertyValue("RestDayRate", ref _RestDayRate, value); }
        }
        
        private int _LateGracePeriod;
        public int LateGracePeriod
        {
            get { return _LateGracePeriod; }
            set { SetPropertyValue("LateGracePeriod", ref _LateGracePeriod, value); }
        }
        private TimeSpan _MinOvertime;
        public TimeSpan MinOvertime
        {
            get { return _MinOvertime; }
            set { SetPropertyValue("MinOvertime", ref _MinOvertime, value); }
        }
        private decimal _OtRate = 120m;
        public decimal OtRate
        {
            get { return _OtRate; }
            set { SetPropertyValue("OtRate", ref _OtRate, value); }
        }
        
        private DateTime _NightStartTime = new DateTime(1753, 1, 1, 22, 0, 0);
        private DateTime _NightEndTime = new DateTime(1753, 1, 1, 6, 0, 0);
        [Custom("DisplayFormat", "hh:mm tt")]
        [Custom("EditMask", "hh:mm tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime NightStartTime
        {
            get { return _NightStartTime; }
            set { SetPropertyValue("NightStartTime", ref _NightStartTime, value); }
        }
        [Custom("DisplayFormat", "hh:mm tt")]
        [Custom("EditMask", "hh:mm tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime NightEndTime
        {
            get { return _NightEndTime; }
            set { SetPropertyValue("NightEndTime", ref _NightEndTime, value); }
        }

        private decimal _NightDiffRate = 10m;
        public decimal NightDiffRate
        {
            get { return _NightDiffRate; }
            set { SetPropertyValue("NightDiffRate", ref _NightDiffRate, value); }
        }
        
        #endregion

        private bool _VerifyEntryDate;
        public bool VerifyEntryDate
        {
            get { return _VerifyEntryDate; }
            set { SetPropertyValue("VerifyEntryDate", ref _VerifyEntryDate, value); }
        }

        private bool _NoReqNoPo;
        [DisplayName("No Request No PO")]
        public bool NoReqNoPo
        {
            get { return _NoReqNoPo; }
            set { SetPropertyValue("NoReqNoPo", ref _NoReqNoPo, value); }
        }
        [DisplayName("Disable Fuel Receipt from Reg PO")]
        public bool NoFuelRecRegPo
        {
            get { return _NoFuelRecRegPo; }
            set { SetPropertyValue("NoFuelRecRegPo", ref _NoFuelRecRegPo, value); }
        }

        private bool _AllowInsufficientCurrQty = false;
        [DisplayName("Allow Issuficient Stock")]
        public bool AllowInsufficientCurrQty
        {
            get { return _AllowInsufficientCurrQty; }
            set { SetPropertyValue("AllowInsufficientCurrQty", ref _AllowInsufficientCurrQty, value); }
        }

        private ExpenseType _TireExpenseType;
        [DisplayName("Tire Expense Type")]
        public ExpenseType TireExpenseType
        {
            get { return _TireExpenseType; }
            set { SetPropertyValue("TireExpenseType", ref _TireExpenseType, value); }
        }

        private SubExpenseType _BrandnewTireExpenseType;
        [DataSourceProperty("TireExpenseType.SubExpenseTypes")]
        [DisplayName("Brandnew Tire Expense Type")]
        public SubExpenseType BrandnewTireExpenseType
        {
            get { return _BrandnewTireExpenseType; }
            set { SetPropertyValue("BrandnewTireExpenseType", ref _BrandnewTireExpenseType, value); }
        }

        private SubExpenseType _RecappedTireExpenseType;
        [DataSourceProperty("TireExpenseType.SubExpenseTypes")]
        [DisplayName("Recapped Tire Expense Type")]
        public SubExpenseType RecappedTireExpenseType
        {
            get { return _RecappedTireExpenseType; }
            set { SetPropertyValue("RecappedTireExpenseType", ref _RecappedTireExpenseType, value); }
        }

        private SubExpenseType _OthersTireExpenseType;
        [DataSourceProperty("TireExpenseType.SubExpenseTypes")]
        [DisplayName("Others Tire Expense Type")]
        public SubExpenseType OthersTireExpenseType
        {
            get { return _OthersTireExpenseType; }
            set { SetPropertyValue("OthersTireExpenseType", ref _OthersTireExpenseType, value); }
        }

        private ExpenseType _FlapsExpenseType;
        [DisplayName("Flaps Expense Type")]
        public ExpenseType FlapsExpenseType
        {
            get { return _FlapsExpenseType; }
            set { SetPropertyValue("FlapsExpenseType", ref _FlapsExpenseType, value); }
        }

        //private SubExpenseType _AccuTireFlapsExpenseType;
        //[DataSourceProperty("FlapsExpenseType.SubExpenseTypes")]
        //[DisplayName("Accumulated Tire Flaps Expense Type")]
        //public SubExpenseType AccuTireFlapsExpenseType
        //{
        //    get { return _AccuTireFlapsExpenseType; }
        //    set { SetPropertyValue("AccuTireFlapsExpenseType", ref _AccuTireFlapsExpenseType, value); }
        //}

        private ExpenseType _TubesExpenseType;
        [DisplayName("Tubes Expense Type")]
        public ExpenseType TubesExpenseType
        {
            get { return _TubesExpenseType; }
            set { SetPropertyValue("TubesExpenseType", ref _TubesExpenseType, value); }
        }

        private ExpenseType _RimExpenseType;
        [DisplayName("Rim Expense Type")]
        public ExpenseType RimExpenseType
        {
            get { return _RimExpenseType; }
            set { SetPropertyValue("RimExpenseType", ref _RimExpenseType, value); }
        }

        private SubExpenseType _RimSubExpenseType;
        [DataSourceProperty("RimExpenseType.SubExpenseTypes")]
        [DisplayName("Rim Sub Expense Type")]
        public SubExpenseType RimSubExpenseType
        {
            get { return _RimSubExpenseType; }
            set { SetPropertyValue("RimSubExpenseType", ref _RimSubExpenseType, value); }
        }

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private bool _NoFuelRecRegPo;
        
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

        public Company(Session session)
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

        public static Company GetInstance(Session session) {
            //Get the Singleton's instance if it exists 
            Company result = session.FindObject<Company>(null);
            //Create the Singleton's instance 
            if (result == null)
            {
                result = new Company(session);
                result.Save();
            }
            return result;
        }

        //Prevent the Singleton from being deleted 
        protected override void OnDeleting() {
            throw new UserFriendlyException(
            "The system prohibits the deletion of company information.");
        }
    }
}
