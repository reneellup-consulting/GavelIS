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
    [NavigationItem(false)]
    public class PayBillExistingCharge : XPObject {
        private PayBill _PayBillID;
        private bool _Pay;
        private int _SourceID;
        private SourceType _SourceType;
        private string _SourceNo;
        private DateTime _Date;
        private string _Transaction;
        private decimal _Charges;
        private decimal _Adjust;
        private Terms _Terms;
        private decimal _Discount;
        private decimal _Interest;
        private decimal _Adjusted;
        [Association("PayBill-Charges")]
        public PayBill PayBillID {
            get { return _PayBillID; }
            set { SetPropertyValue("PayBillID", ref _PayBillID, value); }
        }
        public bool Pay {
            get { return _Pay; }
            set {
                SetPropertyValue("Pay", ref _Pay, value);
                if (!IsLoading) {
                    try {
                        _PayBillID.UpdateChargesAdjust(true);
                        _PayBillID.UpdateInterest(true);
                        _PayBillID.UpdateDiscount(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public int SourceID {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }
        [Custom("AllowEdit", "False")]
        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }
        [Custom("AllowEdit", "False")]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("REF#")]
        public string RefNo
        {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
        [Size(500)]
        public string Transaction {
            get { return _Transaction; }
            set { SetPropertyValue("Transaction", ref _Transaction, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Charges {
            get { return _Charges; }
            set { SetPropertyValue("Charges", ref _Charges, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Adjust {
            get { return _Adjust; }
            set {
                SetPropertyValue("Adjust", ref _Adjust, value);
                if (!IsLoading) {
                    try {
                        _PayBillID.UpdateChargesAdjust(true);
                        _PayBillID.UpdateInterest(true);
                        _PayBillID.UpdateDiscount(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Discount {
            get { return _Discount; }
            set {
                SetPropertyValue("Discount", ref _Discount, value);
                if (!IsLoading) {
                    try {
                        _PayBillID.UpdateChargesAdjust(true);
                        _PayBillID.UpdateInterest(true);
                        _PayBillID.UpdateDiscount(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Interest {
            get { return _Interest; }
            set {
                SetPropertyValue("Interest", ref _Interest, value);
                if (!IsLoading) {
                    try {
                        _PayBillID.UpdateChargesAdjust(true);
                        _PayBillID.UpdateInterest(true);
                        _PayBillID.UpdateDiscount(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set { SetPropertyValue("Adjusted", ref _Adjusted, value); }
        }
        [Action(AutoCommit = true, Caption = "Select/Unselect")]
        public void SelectUnselect()
        {
            if (_Pay)
            {
                Pay = false;
            }
            else
            {
                Pay = true;
            }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Credit Applied")]
        public string AppliedByDocNo
        {
            get { return _AppliedByDocNo; }
            set { SetPropertyValue("AppliedByDocNo", ref _AppliedByDocNo, value); }
        }
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal AppliedAmount
        //{
        //    get { return _AppliedAmount; }
        //    set { SetPropertyValue("AppliedAmount", ref _AppliedAmount, value); }
        //}
        //private decimal _Withheld;
        //[Custom("AllowEdit", "False")]
        //[Custom("DisplayFormat", "n")]
        //public decimal Withheld
        //{
        //    get { return _Withheld; }
        //    set { SetPropertyValue<decimal>("Withheld", ref _Withheld, value); }
        //}

        public PayBillExistingCharge(Session session): base(session) {
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

        #region Get Current User

        private SecurityUser _CurrentUser;
        private string _RefNo;
        private string _AppliedByDocNo;
        private decimal _AppliedAmount;
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
