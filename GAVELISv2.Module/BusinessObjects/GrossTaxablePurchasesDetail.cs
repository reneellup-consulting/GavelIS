using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class GrossTaxablePurchasesDetail : XPObject
    {
        private GrossTaxablePurchasesHeader _ParentID;
        private string _Tin;
        private string _RegisteredName;
        private string _SuppliersAddress;
        private decimal _AmountOfGrossPurchase;
        private decimal _AmountOfTaxablePurchases;
        private decimal _AmountOfPurchaseOfServices;
        private decimal _AmountOfInputTax;
        private decimal _AmountOfGrossTaxablePurchase;
        // ParentID
        [Custom("AllowEdit", "False")]
        [Association("GrossTaxablePurchasesDetails")]
        public GrossTaxablePurchasesHeader ParentID
        {
            get { return _ParentID; }
            set { SetPropertyValue("ParentID", ref _ParentID, value); }
        }
        [Custom("AllowEdit", "False")]
        public GenJournalHeader Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }
        // Tin
        [Custom("AllowEdit", "False")]
        [DisplayName("TIN")]
        public string Tin
        {
            get { return _Tin; }
            set { SetPropertyValue("Tin", ref _Tin, value); }
        }
        // RegisteredName
        [Custom("AllowEdit", "False")]
        public string RegisteredName
        {
            get { return _RegisteredName; }
            set { SetPropertyValue("RegisteredName", ref _RegisteredName, value); }
        }
        // SuppliersAddress
        [Custom("AllowEdit", "False")]
        [Size(500)]
        public string SuppliersAddress
        {
            get { return _SuppliersAddress; }
            set { SetPropertyValue("SuppliersAddress", ref _SuppliersAddress, value); }
        }
        // AmountOfGrossPurchase
        [Custom("AllowEdit", "False")]
        public decimal AmountOfGrossPurchase
        {
            get { return _AmountOfGrossPurchase; }
            set { SetPropertyValue("AmountOfGrossPurchase", ref _AmountOfGrossPurchase, value); }
        }
        // AmountOfTaxablePurchases
        [Custom("AllowEdit", "False")]
        public decimal AmountOfTaxablePurchases
        {
            get { return _AmountOfTaxablePurchases; }
            set { SetPropertyValue("AmountOfTaxablePurchases", ref _AmountOfTaxablePurchases, value); }
        }
        // AmountOfPurchaseOfServices
        [Custom("AllowEdit", "False")]
        public decimal AmountOfPurchaseOfServices
        {
            get { return _AmountOfPurchaseOfServices; }
            set { SetPropertyValue("AmountOfPurchaseOfServices", ref _AmountOfPurchaseOfServices, value); }
        }
        // AmountOfInputTax
        [Custom("AllowEdit", "False")]
        public decimal AmountOfInputTax
        {
            get { return _AmountOfInputTax; }
            set { SetPropertyValue("AmountOfInputTax", ref _AmountOfInputTax, value); }
        }
        // AmountOfGrossTaxablePurchase
        [Custom("AllowEdit", "False")]
        public decimal AmountOfGrossTaxablePurchase
        {
            get { return _AmountOfGrossTaxablePurchase; }
            set { SetPropertyValue("AmountOfGrossTaxablePurchase", ref _AmountOfGrossTaxablePurchase, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Total
        {
            get { return _Total; }
            set { SetPropertyValue("Total", ref _Total, value); }
        }
        public GrossTaxablePurchasesDetail(Session session)
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
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private GenJournalHeader _Source;
        private decimal _Total;
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
