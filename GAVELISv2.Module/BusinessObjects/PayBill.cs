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
    public class PayBill : XPObject {
        private DateTime _EntryDate = DateTime.Now;
        private Vendor _Vendor;
        [Persistent("ChargesAdjust")]
        private decimal? _ChargesAdjust;
        [Persistent("Interest")]
        private decimal? _Interest;
        //private decimal _SubTotal;
        [Persistent("Discount")]
        private decimal? _Discount;
        //private decimal _ChargesTotal;
        [Persistent("PaymentsAdjust")]
        private decimal? _PaymentsAdjust;
        //private decimal _NewCheck;
        private decimal _PaymentsTotal;
        //private decimal _Difference;
        [Persistent("WhtTotal")]
        private decimal? _WhtTotal;
        private Account _CashBankAccount;
        private string _CheckNo;
        private bool _PostDated;
        private DateTime _CheckDate = DateTime.Now;

        private decimal _Amount;
        private bool _ToBePrinted = true;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;

        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value);
            if (!IsLoading)
            {
                    CheckDate = _EntryDate;
            }
            }
        }
        [ImmediatePostData]
        public Vendor Vendor {
            get { return _Vendor; }
            set { SetPropertyValue("Vendor", ref _Vendor, value); }
        }
        [DisplayName("SUBTOTAL")]
        [PersistentAlias("ChargesAdjust + Interest")]
        [Custom("DisplayFormat", "n")]
        public decimal SubTotal {
            get {
                object tempObject = EvaluateAlias("SubTotal");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [DisplayName("TOTAL(C)")]
        [PersistentAlias("SubTotal - Discount")]
        [Custom("DisplayFormat", "n")]
        public decimal ChargesTotal {
            get {
                object tempObject = EvaluateAlias("ChargesTotal");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [DisplayName("(+)NewCheck")]
        [PersistentAlias("Amount")]
        [Custom("DisplayFormat", "n")]
        public decimal NewCheck {
            get {
                object tempObject = EvaluateAlias("NewCheck");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [DisplayName("TOTAL(P)")]
        [PersistentAlias("PaymentsAdjust + NewCheck")]
        [Custom("DisplayFormat", "n")]
        public decimal PaymentsTotal {
            get
            {
                object tempObject = EvaluateAlias("PaymentsTotal");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("(ChargesTotal - PaymentsTotal) + WhtTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal Difference {
            get {
                object tempObject = EvaluateAlias("Difference");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        public Account CashBankAccount {
            get { return _CashBankAccount; }
            set { SetPropertyValue("CashBankAccount", ref _CashBankAccount, 
                value);
            if (!IsLoading && _CashBankAccount != null)
            {
                CheckNo =
                    _CashBankAccount.GetCheckNo();
            }
            }
        }
        //[Custom("AllowEdit", "False")]
        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue("CheckNo", ref _CheckNo, value); }
        }
        public bool PostDated
        {
            get { return _PostDated; }
            set { SetPropertyValue("PostDated", ref _PostDated, value); }
        }

        public DateTime CheckDate
        {
            get { return _CheckDate; }
            set { SetPropertyValue("CheckDate", ref _CheckDate, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
        }
        public bool ToBePrinted {
            get { return _ToBePrinted; }
            set { SetPropertyValue("ToBePrinted", ref _ToBePrinted, value); }
        }
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType
        {
            get
            {
                return _SubExpenseType;
            }
            set
            {
                SetPropertyValue("SubExpenseType", ref _SubExpenseType, value);
            }
        }

        [Aggregated,
        Association("PayBill-Charges")]
        public XPCollection<PayBillExistingCharge> Charges { get { return 
                GetCollection<PayBillExistingCharge>("Charges"); } }
        [Aggregated,
        Association("PayBill-Credits")]
        public XPCollection<PayBillExistingCredit> Credits { get { return 
                GetCollection<PayBillExistingCredit>("Credits"); } }
        #region Calculated Details
        [DisplayName("Adjust(C)")]
        [PersistentAlias("_ChargesAdjust")]
        [Custom("DisplayFormat", "n")]
        public decimal? ChargesAdjust {
            get {
                try {
                    if (!IsLoading && !IsSaving && _ChargesAdjust == null) {
                        UpdateChargesAdjust(false);}
                } catch (Exception) {}
                return _ChargesAdjust;
            }
        }
        [DisplayName("(+)Interest")]
        [PersistentAlias("_Interest")]
        [Custom("DisplayFormat", "n")]
        public decimal? Interest {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Interest == null) {
                        UpdateInterest(false);}
                } catch (Exception) {}
                return _Interest;
            }
        }
        [DisplayName("(-)Discount")]
        [PersistentAlias("_Discount")]
        [Custom("DisplayFormat", "n")]
        public decimal? Discount {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Discount == null) {
                        UpdateDiscount(false);}
                } catch (Exception) {}
                return _Discount;
            }
        }
        [DisplayName("Adjust(P)")]
        [PersistentAlias("_PaymentsAdjust")]
        [Custom("DisplayFormat", "n")]
        public decimal? PaymentsAdjust
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _PaymentsAdjust == null)
                    {
                        UpdatePaymentsAdjust(false);
                    }
                }
                catch (Exception) { }
                return _PaymentsAdjust;
            }
        }
        [DisplayName("WHT")]
        [PersistentAlias("_WhtTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal? WhtTotal
        {
            get { 
                try
                {
                    if (!IsLoading && !IsSaving && _WhtTotal == null)
                    {
                        UpdateWhtTotal(false);
                    }
                }
                catch (Exception) { }
                return _WhtTotal; }
        }
        
        public void UpdateChargesAdjust(bool forceChangeEvent) {
            decimal? oldChargesAdjust = _ChargesAdjust;
            decimal tempTotal = 0m;
            foreach (PayBillExistingCharge detail in Charges) {tempTotal += 
                detail.Pay ? detail.Adjust : 0;}
            _ChargesAdjust = tempTotal;
            if (forceChangeEvent) {OnChanged("ChargesAdjust", ChargesAdjust, 
                _ChargesAdjust);}
            ;
        }
        public void UpdateInterest(bool forceChangeEvent) {
            decimal? oldInterest = _Interest;
            decimal tempTotal = 0m;
            foreach (PayBillExistingCharge detail in Charges) {tempTotal += 
                detail.Pay ? detail.Interest : 0;}
            _Interest = tempTotal;
            if (forceChangeEvent) {OnChanged("Interest", Interest, _Interest);}
            ;
        }
        public void UpdateDiscount(bool forceChangeEvent) {
            decimal? oldDiscount = _Discount;
            decimal tempTotal = 0m;
            foreach (PayBillExistingCharge detail in Charges) {tempTotal += 
                detail.Pay ? detail.Discount : 0;}
            _Discount = tempTotal;
            if (forceChangeEvent) {OnChanged("Discount", Discount, _Discount);}
            ;
        }

        public void UpdatePaymentsAdjust(bool forceChangeEvent)
        {
            decimal? oldPaymentsAdjust = _PaymentsAdjust;
            decimal tempTotal = 0m;
            foreach (PayBillExistingCredit detail in Credits)
            {
                tempTotal +=
                    detail.Select ? detail.AdjustNow : 0;
            }
            _PaymentsAdjust = tempTotal;
            if (forceChangeEvent) { OnChanged("PaymentsAdjust", PaymentsAdjust, _PaymentsAdjust); }
            ;
        }
        public void UpdateWhtTotal(bool forceChangeEvent)
        {
            decimal? oldWhtTotal = _WhtTotal;
            decimal tempTotal = 0m;
            foreach (PayBillExistingCredit detail in Credits)
            {
                tempTotal +=
                    detail.Select ? detail.Wht : 0;
            }
            _WhtTotal = tempTotal;
            if (forceChangeEvent) { OnChanged("WhtTotal", WhtTotal, _WhtTotal); }
            ;
        }
        #endregion
        public PayBill(Session session): base(session) {
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
        }
        //protected override void OnSaving() { throw new UserFriendlyException(
        //    "The system prohibits the saving of Pay Bills information"); }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _ChargesAdjust = null;
            _Interest = null;
            _Discount = null;
            _PaymentsAdjust = null;
            _WhtTotal = null;
        }
    }
}
