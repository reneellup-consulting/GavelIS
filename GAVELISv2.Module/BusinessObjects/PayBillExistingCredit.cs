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
    public class PayBillExistingCredit : XPObject {
        private PayBill _PayBillID;
        private bool _Select;
        private int _SourceID;
        private SourceType _SourceType;
        private OperationType _OperationType;
        private string _SourceNo;
        private DateTime _Date;
        private string _Transaction;
        private decimal _Payment;
        private decimal _AdjustNow;
        private decimal _Wht;
        private decimal _Adjusted;
        [Association("PayBill-Credits")]
        public PayBill PayBillID {
            get { return _PayBillID; }
            set {
                PayBill oldDocumentId = _PayBillID;
                SetPropertyValue("PayBillID", ref _PayBillID, value);
                if (!IsLoading && !IsSaving && _PayBillID != null && oldDocumentId != _PayBillID)
                {
                    oldDocumentId = oldDocumentId ?? _PayBillID;
                    try
                    {
                        _PayBillID.UpdateChargesAdjust(true);
                        _PayBillID.UpdateDiscount(true);
                        _PayBillID.UpdateInterest(true);
                        _PayBillID.UpdatePaymentsAdjust(true);
                        _PayBillID.UpdateWhtTotal(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public bool Select {
            get { return _Select; }
            set
            {
                SetPropertyValue("Select", ref _Select, value);
                if (!IsLoading)
                {
                    try
                    {
                        _PayBillID.UpdatePaymentsAdjust(true);
                    } catch (Exception)
                    {
                    }
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
        public OperationType OperationType {
            get { return _OperationType; }
            set { SetPropertyValue("OperationType", ref _OperationType, value); }
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

        [Custom("DisplayFormat", "n")]
        public decimal AdjustNow {
            get { return _AdjustNow; }
            set
            {
                SetPropertyValue("AdjustNow", ref _AdjustNow, value);
                if (!IsLoading)
                {
                    try
                    {
                        _PayBillID.UpdatePaymentsAdjust(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }
        [DisplayName("WHT")]
        [Custom("DisplayFormat", "n")]
        public decimal Wht
        {
            get { return _Wht; }
            set { SetPropertyValue("Wht", ref _Wht, value);
            if (!IsLoading)
            {
                try
                {
                    _PayBillID.UpdateWhtTotal(true);
                }
                catch (Exception)
                {
                }
            }
            }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set { SetPropertyValue("Adjusted", ref _Adjusted, value); }
        }

        public PayBillExistingCredit(Session session)
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
        }

        //protected override void OnSaving() { throw new UserFriendlyException(
        //    "The system prohibits the saving of Pay Bills information"); }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private string _RefNo;
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
