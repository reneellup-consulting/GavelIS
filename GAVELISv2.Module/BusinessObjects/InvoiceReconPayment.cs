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
    public class InvoiceReconPayment : XPObject {
        private InvoiceReconciliation _InvoiceReconID;
        private bool _Select;
        private SourceType _SourceType;
        private string _SourceNo;
        private int _SourceID;
        private DateTime _Date;
        private string _Transaction;
        private decimal _Payment;
        private decimal _AdjustNow;
        private decimal _OpenAmount;
        private decimal _TaxPayment;
        [Association("InvoiceRecon-Payments")]
        public InvoiceReconciliation InvoiceReconID {
            get { return _InvoiceReconID; }
            set { SetPropertyValue("InvoiceReconID", ref _InvoiceReconID, value)
                ; }
        }
        public bool Select {
            get { return _Select; }
            set {
                SetPropertyValue("Select", ref _Select, value);
                if (!IsLoading) {
                    try {
                        _InvoiceReconID.UpdateSelectedPayments(true);
                        _InvoiceReconID.UpdateTaxPayments(true);
                        _InvoiceReconID.UpdateWithheld(true);
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
        public decimal Payment {
            get { return _Payment; }
            set { SetPropertyValue("Payment", ref _Payment, value); }
        }
        private decimal _Withheld;
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Withheld
        {
            get { return _Withheld; }
            set { SetPropertyValue<decimal>("Withheld", ref _Withheld, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal AdjustNow {
            get { return _AdjustNow; }
            set {
                SetPropertyValue("AdjustNow", ref _AdjustNow, value);
                if (!IsLoading) {
                    try {
                        _InvoiceReconID.UpdateSelectedPayments(true);
                    } catch (Exception) {}
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TaxPayment {
            get { return _TaxPayment; }
            set { SetPropertyValue("TaxPayment", ref _TaxPayment, value); }
        }
        public InvoiceReconPayment(Session session): base(session) {
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
