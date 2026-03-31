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
    public class Vendor : Contact {

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

        private Terms _Terms;
        private ShipVia _ShipVia;
        private Account _Account;
        private WHTGroupCode _WHTGroupCode;
        private string _TIN01;
        private string _TIN02;
        private string _TIN03;
        private string _TIN04;
        private decimal _CreditLimit;
        private bool _Retreader = false;
        private bool _VatTaxable = false;
        //[RuleRequiredField("", DefaultContexts.Save)]
        public PartsOrigin Origin
        {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }

        public ShipVia ShipVia {
            get { return _ShipVia; }
            set { SetPropertyValue("ShipVia", ref _ShipVia, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account Account {
            get { return _Account; }
            set { SetPropertyValue("Account", ref _Account, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public WHTGroupCode WHTGroupCode {
            get { return _WHTGroupCode; }
            set { SetPropertyValue("WHTGroupCode", ref _WHTGroupCode, value); }
        }

        public string TIN01 {
            get { return _TIN01; }
            set { SetPropertyValue<string>("TIN01", ref _TIN01, value); }
        }

        public string TIN02 {
            get { return _TIN02; }
            set { SetPropertyValue<string>("TIN02", ref _TIN02, value); }
        }

        public string TIN03 {
            get { return _TIN03; }
            set { SetPropertyValue<string>("TIN03", ref _TIN03, value); }
        }

        public string TIN04 {
            get { return _TIN04; }
            set { SetPropertyValue<string>("TIN04", ref _TIN04, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CreditLimit {
            get { return _CreditLimit; }
            set { SetPropertyValue("CreditLimit", ref _CreditLimit, value); }
        }

        public bool Retreader {
            get { return _Retreader; }
            set { SetPropertyValue<bool>("Retreader", ref _Retreader, value); }
        }
        [DisplayName("VAT Taxable")]
        public bool VatTaxable
        {
            get { return _VatTaxable; }
            set { SetPropertyValue("VatTaxable", ref _VatTaxable, value); }
        }
        
        [Association("Vendor-APRegistry")]
        public XPCollection<APRegistry> APRegistrys {
            get { return GetCollection
                <APRegistry>("APRegistrys"); }
        }

        [Association("Customer-IHPayables")]
        public XPCollection<Receipt> IHPayables {
            get { return GetCollection
                <Receipt>("IHPayables"); }
        }

        [Association("Customer-FuelPayables")]
        public XPCollection<ReceiptFuel> FuelPayables {
            get { return GetCollection
                <ReceiptFuel>("FuelPayables"); }
        }

        [Association("Customer-JobsPayables")]
        public XPCollection<JobOrder> JobsPayables {
            get { return GetCollection
                <JobOrder>("JobsPayables"); }
        }

        [Persistent("Balance")]
        private decimal? _Balance = null;
        private bool _IsFuelStation;
        private bool _UpdateOnline;
        private PartsOrigin _Origin;
        [PersistentAlias("_Balance")]
        [Custom("DisplayFormat", "n")]
        public decimal? Balance {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Balance == null)
                    {
                        UpdateBalance(false);
                    }
                } catch (Exception)
                {
                }
                return _Balance;
            }
        }

        public void UpdateBalance(bool forceChangeEvent) {
            decimal? oldBalance = _Balance;
            decimal tempTotal = 0m;
            //foreach (APRegistry detail in APRegistrys) {tempTotal += detail.
            //    AmtRmn;}
            //_Balance = tempTotal;
            //if (forceChangeEvent) {OnChanged("Balance", Balance, _Balance);}
            //;
            foreach (Receipt detail in IHPayables)
            {
                tempTotal += detail.AmtRmn;
            }
            foreach (ReceiptFuel detail in FuelPayables)
            {
                tempTotal += detail.AmtRmn;
            }
            foreach (JobOrder detail in JobsPayables)
            {
                tempTotal += detail.AmtRmn;
            }

            _Balance = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Balance", Balance, _Balance);
            }
            ;
        }

        public bool OverLimit() {
            if (_CreditLimit != 0)
            {
                if (_Balance > _CreditLimit)
                {
                    return true;
                }
            }
            return false;
        }
        #region Gavel Online v01
        // IsFuelStation
        public bool IsFuelStation
        {
            get { return _IsFuelStation; }
            set { SetPropertyValue("IsFuelStation", ref _IsFuelStation, value);
            if (!IsLoading && !IsSaving)
            {
                UpdateOnline = true;
            }
            }
        }
        // UpdateOnline
        [Custom("AllowEdit", "False")]
        public bool UpdateOnline
        {
            get { return _UpdateOnline; }
            set { SetPropertyValue("UpdateOnline", ref _UpdateOnline, value); }
        }

        public override string No
        {
            get
            {
                return base.No;
            }
            set
            {
                base.No = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }
        #endregion
        public Vendor(Session session)
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
            ContactType = ContactTypeEnum.Vendor;
        }
    }
}
