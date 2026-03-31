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
    public class BankReconciliation : GenJournalHeader {
        private Account _AccountToReconcile;
        private decimal _BeginingBalance;
        private decimal _EndingBalance;
        [Persistent("DepositsCount")]
        private int? _DepositsCount;
        [Persistent("DepositsBalance")]
        private decimal? _DepositsBalance;
        [Persistent("ChecksAndPaymentsCount")]
        private int? _ChecksAndPaymentsCount;
        [Persistent("ChecksAndPaymentsBalance")]
        private decimal? _ChecksAndPaymentsBalance;
        private Account _PostDifferenceToAccount;
        private decimal _EndingEndingBalance;
        private decimal _ClearedBalance;
        private decimal _Difference;
        private string _Memo;
        private BankReconStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account AccountToReconcile {
            get { return _AccountToReconcile; }
            set { SetPropertyValue("AccountToReconcile", ref _AccountToReconcile
                , value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal BeginingBalance {
            get { return _BeginingBalance; }
            set { SetPropertyValue("BeginingBalance", ref _BeginingBalance, 
                value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal EndingBalance {
            get { return _EndingBalance; }
            set { SetPropertyValue("EndingBalance", ref _EndingBalance, value); 
            }
        }
        [PersistentAlias("_DepositsCount")]
        public int? DepositsCount {
            get { return _DepositsCount; }
            set { SetPropertyValue("DepositsCount", ref _DepositsCount, value); 
            }
        }
        [PersistentAlias("_DepositsBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal? DepositsBalance {
            get { return _DepositsBalance; }
            set { SetPropertyValue("DepositsBalance", ref _DepositsBalance, 
                value); }
        }
        [PersistentAlias("_ChecksAndPaymentsCount")]
        public int? ChecksAndPaymentsCount {
            get { return _ChecksAndPaymentsCount; }
            set { SetPropertyValue("ChecksAndPaymentsCount", ref 
                _ChecksAndPaymentsCount, value); }
        }
        [PersistentAlias("_ChecksAndPaymentsBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal? ChecksAndPaymentsBalance {
            get { return _ChecksAndPaymentsBalance; }
            set { SetPropertyValue("ChecksAndPaymentsBalance", ref 
                _ChecksAndPaymentsBalance, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account PostDifferenceToAccount {
            get { return _PostDifferenceToAccount; }
            set { SetPropertyValue("PostDifferenceToAccount", ref 
                _PostDifferenceToAccount, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal EndingEndingBalance {
            get { return _EndingEndingBalance; }
            set { SetPropertyValue("EndingEndingBalance", ref 
                _EndingEndingBalance, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal ClearedBalance {
            get { return _ClearedBalance; }
            set { SetPropertyValue("ClearedBalance", ref _ClearedBalance, value)
                ; }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Difference {
            get { return _Difference; }
            set { SetPropertyValue("Difference", ref _Difference, value); }
        }
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        public BankReconStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != BankReconStatusEnum.Current) {Approved = true
                        ;} else {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null) {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        public BankReconciliation(Session session): base(session) {
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "BR"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "RA"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "BR"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved bank reconciliation."
                );} }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            //_Count = null;
            //_TotalDeposit = null;
        }

    }
}
