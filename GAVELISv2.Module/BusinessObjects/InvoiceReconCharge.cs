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
    public class InvoiceReconCharge : XPObject {
        private InvoiceReconciliation _InvoiceReconID;
        private bool _Pay;
        private SourceType _SourceType;
        private OperationType _OperationType;
        private string _SourceNo;
        private int _SourceID;
        private DateTime _Date;
        private string _Transaction;
        private decimal _Charges;
        private decimal _Adjust;
        private decimal _Tax;
        private Terms _Terms;
        private decimal _Discount;
        private decimal _FinanceCharge;
        private WHTGroupCode _WHTCode;
        private decimal _WHTRate;
        private decimal _WHTAmount;
        //private decimal _Total;
        private decimal _OpenAmount;
        [Association("InvoiceRecon-Charges")]
        public InvoiceReconciliation InvoiceReconID {
            get { return _InvoiceReconID; }
            set { SetPropertyValue("InvoiceReconID", ref _InvoiceReconID, value)
                ; }
        }
        public bool Pay {
            get { return _Pay; }
            set {
                SetPropertyValue("Pay", ref _Pay, value);
                if (!IsLoading) {
                    try {
                        _InvoiceReconID.UpdateSelectedCharges(true);
                        _InvoiceReconID.UpdateDiscountAllowed(true);
                        _InvoiceReconID.UpdateFinCharges(true);
                        _InvoiceReconID.UpdateTaxCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }
        [Custom("AllowEdit", "False")]
        public OperationType OperationType {
            get { return _OperationType; }
            set { SetPropertyValue("OperationType", ref _OperationType, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public int SourceID {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime Date {
            get { return _Date; }
            set { SetPropertyValue("Date", ref _Date, value); }
        }
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
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
                        _InvoiceReconID.UpdateSelectedCharges(true);
                        _InvoiceReconID.UpdateDiscountAllowed(true);
                        _InvoiceReconID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Tax {
            get { return _Tax; }
            set { SetPropertyValue("Tax", ref _Tax, value); }
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
                        _InvoiceReconID.UpdateSelectedCharges(true);
                        _InvoiceReconID.UpdateDiscountAllowed(true);
                        _InvoiceReconID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal FinanceCharge {
            get { return _FinanceCharge; }
            set {
                SetPropertyValue("FinanceCharge", ref _FinanceCharge, value);
                if (!IsLoading) {
                    try {
                        _InvoiceReconID.UpdateSelectedCharges(true);
                        _InvoiceReconID.UpdateDiscountAllowed(true);
                        _InvoiceReconID.UpdateFinCharges(true);
                    } catch (Exception) {}
                }
            }
        }
        public WHTGroupCode WHTCode {
            get { return _WHTCode; }
            set {
                SetPropertyValue("WHTCode", ref _WHTCode, value);
                if (!IsLoading) {
                    if (_WHTCode != null) {WHTRate = _WHTCode.WHTRate;} else {
                        WHTRate = 0;
                    }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTRate {
            get { return _WHTRate; }
            set { SetPropertyValue("WHTRate", ref _WHTRate, value); }
        }
        [PersistentAlias("Adjust * WHTRate/100")]
        [Custom("DisplayFormat", "n")]
        public decimal WHTAmount {
            get {
                object tempObject = EvaluateAlias("WHTAmount");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [PersistentAlias("(Adjust - Discount) + FinanceCharge")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }
        public InvoiceReconCharge(Session session): base(session) {
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
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
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
