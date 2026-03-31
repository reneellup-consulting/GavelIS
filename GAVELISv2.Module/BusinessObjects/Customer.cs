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
    public class Customer : Contact {

        #region Ship to Address

        private decimal _Credits;
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

        private Terms _Terms;
        private ShipVia _ShipVia;
        private decimal _CreditLimit;
        private Account _Account;
        private SalesTaxCode _TaxCode;
        private WHTGroupCode _WHTGroupCode;
        private CustomerClass _CustomerClass;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }

        public ShipVia ShipVia {
            get { return _ShipVia; }
            set { SetPropertyValue("ShipVia", ref _ShipVia, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CreditLimit {
            get { return _CreditLimit; }
            set { SetPropertyValue("CreditLimit", ref _CreditLimit, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account Account {
            get { return _Account; }
            set { SetPropertyValue("Account", ref _Account, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public SalesTaxCode TaxCode {
            get { return _TaxCode; }
            set { SetPropertyValue("TaxCode", ref _TaxCode, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public WHTGroupCode WHTGroupCode {
            get { return _WHTGroupCode; }
            set { SetPropertyValue("WHTGroupCode", ref _WHTGroupCode, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public CustomerClass CustomerClass {
            get { return _CustomerClass; }
            set { SetPropertyValue("CustomerClass", ref _CustomerClass, value); }
        }

        [Association("Customer-ARRegistry")]
        public XPCollection<ARRegistry> ARRegistrys {
            get { return GetCollection
                <ARRegistry>("ARRegistrys"); }
        }

        [Association("Customer-IHReceivables")]
        public XPCollection<Invoice> IHReceivables {
            get { return GetCollection
                <Invoice>("IHReceivables"); }
        }

        [Association("Customer-CreditMemos")]
        public XPCollection<CreditMemo> CreditMemos {
            get { return GetCollection
                <CreditMemo>("CreditMemos"); }
        }
        //[NonPersistent]
        public decimal Credits
        {
            get {
                Nullable<decimal> sum = CreditMemos.Where(o=>o.Status!= CreditMemoStatusEnum.Current && o.Status!= CreditMemoStatusEnum.Applied).Sum(o => o.GrossTotal);
                return sum.Value;
            }
        }
        [Persistent("CreditableAmount")]
        private decimal? _CreditableAmount = null;
        [PersistentAlias("_CreditableAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal? CreditableAmount {
            get {
                try
                {
                    //UpdateBalance(false);
                    if (!IsLoading && !IsSaving && _CreditableAmount == null)
                    {
                        UpdateCreditableAmount(false);
                    }
                }
                catch (Exception)
                {
                }
                return _CreditableAmount; }
        }
        public void UpdateCreditableAmount(bool forceChangeEvent)
        {
            decimal? oldCreditableAmount = _CreditableAmount;
            decimal tempTotal = 0m;
            foreach (CreditMemo detail in CreditMemos)
            {
                if (detail.Status!=CreditMemoStatusEnum.Current && detail.Status!=CreditMemoStatusEnum.Applied)
                {
                    tempTotal += detail.GrossTotal.Value;
                }
                
            }
            _CreditableAmount = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("CreditableAmount", CreditableAmount, _CreditableAmount);
            }
            ;
        }
        [Persistent("Balance")]
        private decimal? _Balance = null;
        [PersistentAlias("_Balance")]
        [Custom("DisplayFormat", "n")]
        public decimal? Balance {
            get
            {
                try
                {
                    //UpdateBalance(false);
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
            foreach (Invoice detail in IHReceivables)
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

        #region Trucking

        private Account _TruckingIncomeAcct;
        private Account _TrailerIncomeAcct;
        private Account _GensetIncomeAcct;
        private Account _ShuntingIncomeAcct;
        private Account _KDIncomeAcct;
        public Account TruckingIncomeAcct {
            get { return _TruckingIncomeAcct; }
            set { SetPropertyValue("TruckingIncomeAcct", ref _TruckingIncomeAcct
                , value); }
        }

        public Account TrailerIncomeAcct {
            get { return _TrailerIncomeAcct; }
            set { SetPropertyValue("TrailerIncomeAcct", ref _TrailerIncomeAcct,
                value); }
        }

        public Account GensetIncomeAcct {
            get { return _GensetIncomeAcct; }
            set { SetPropertyValue("GensetIncomeAcct", ref _GensetIncomeAcct,
                value); }
        }

        public Account ShuntingIncomeAcct {
            get { return _ShuntingIncomeAcct; }
            set { SetPropertyValue("ShuntingIncomeAcct", ref _ShuntingIncomeAcct
                , value); }
        }

        public Account KDIncomeAcct {
            get { return _KDIncomeAcct; }
            set { SetPropertyValue("KDIncomeAcct", ref _KDIncomeAcct, value); }
        }

        #endregion

        public Customer(Session session)
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
            ContactType = ContactTypeEnum.Customer;
        }

        private void Reset()
        {
            _Balance = null;
            _CreditableAmount = null;
        }
    }
}
