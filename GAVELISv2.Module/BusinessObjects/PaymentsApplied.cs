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
    public class PaymentsApplied : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private PaymentTenderedTypeEnum _PaymentTenderedType;
        private decimal _Amount;
        private CreditMemo _Memo;
        private string _PayeeName;
        private string _BankName;
        private string _CheckNo;
        private Customer _Customer;
        private DateTime _CheckDate;
        private decimal _CashAmount;
        private decimal _CheckAmount;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("GenJournalHeader-PaymentsApplied")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set {
                GenJournalHeader oldGenJournalID = _GenJournalID;
                SetPropertyValue<GenJournalHeader>("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && !IsSaving && oldGenJournalID != _GenJournalID)
                {
                    oldGenJournalID = oldGenJournalID ?? _GenJournalID;
                    ((Invoice)oldGenJournalID).UpdateCashPayments(true);
                }
            }
        }

        [ImmediatePostData]
        public PaymentTenderedTypeEnum PaymentTenderedType {
            get { return _PaymentTenderedType; }
            set { SetPropertyValue<PaymentTenderedTypeEnum>("PaymentTenderedType", ref _PaymentTenderedType, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue<decimal>("Amount", ref _Amount, value);
                if (!IsLoading && !IsSaving)
                {
                    ((Invoice)_GenJournalID).UpdateCashPayments(true);
                }
            }
        }

        public CreditMemo Memo {
            get { return _Memo; }
            set { SetPropertyValue<CreditMemo>("Memo", ref _Memo, value);
            if (!IsLoading && !IsSaving)
            {
                Amount = _Memo.GrossTotal.Value;
            }
            }
        }

        public string PayeeName {
            get { return _PayeeName; }
            set { SetPropertyValue<string>("PayeeName", ref _PayeeName, value); }
        }

        public string BankName {
            get { return _BankName; }
            set { SetPropertyValue<string>("BankName", ref _BankName, value); }
        }

        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue<string>("CheckNo", ref _CheckNo, value); }
        }

        public DateTime CheckDate {
            get { return _CheckDate; }
            set { SetPropertyValue<DateTime>("CheckDate", ref _CheckDate, value); }
        }
        [NonPersistent]
        public decimal CashAmount {
            get
            {
                if (_PaymentTenderedType == PaymentTenderedTypeEnum.Cash)
                {
                    _CashAmount = _Amount;
                }
                else
                {
                    _CashAmount = 0;
                }
                return _CashAmount; }
        }
        [NonPersistent]
        public decimal CheckAmount {
            get {
                if (_PaymentTenderedType == PaymentTenderedTypeEnum.Check)
                {
                    _CheckAmount = _Amount;
                }
                else
                {
                    _CheckAmount = 0;
                }
                return _CheckAmount; }
        }

        [NonPersistent]
        public Invoice InvoiceInfo {
            get { return (Invoice)
                _GenJournalID; }
        }

        [NonPersistent]
        public Customer Customer {
            get { return (_GenJournalID as Invoice).Customer ?? null; }
        }

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
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

        public PaymentsApplied(Session session)
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
            RowID = Guid.NewGuid();

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
