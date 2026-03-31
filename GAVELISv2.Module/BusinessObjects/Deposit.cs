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
    public class Deposit : GenJournalHeader {
        private Account _AccountToDeposit;
        [Persistent("Count")]
        private int? _Count;
        [Persistent("TotalDeposit")]
        private decimal? _TotalDeposit;
        private Account _DepositToAccount;
        private string _Memo;
        private DepositStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account AccountToDeposit {
            get { return _AccountToDeposit; }
            set { SetPropertyValue("AccountToDeposit", ref _AccountToDeposit, 
                value); }
        }
        [PersistentAlias("_Count")]
        public int? Count {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Count == null) {UpdateCount(
                        false);}
                } catch (Exception) {}
                return _Count;
            }
        }
        public void UpdateCount(bool forceChangeEvent) {
            int? oldCount = _Count;
            int tempTotal = 0;
            foreach (DepositDetail detail in DepositDetails) {if (detail.Select) 
                {tempTotal++;}}
            _Count = tempTotal;
            if (forceChangeEvent) {OnChanged("Count", Count, _Count);}
            ;
        }
        [PersistentAlias("_TotalDeposit")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalDeposit {
            get {
                try {
                    if (!IsLoading && !IsSaving && _TotalDeposit == null) {
                        UpdateTotalDeposit(false);}
                } catch (Exception) {}
                return _TotalDeposit;
            }
        }
        public void UpdateTotalDeposit(bool forceChangeEvent) {
            decimal? oldTotalDeposit = _TotalDeposit;
            decimal tempTotal = 0m;
            foreach (DepositDetail detail in DepositDetails) {if (detail.Select) 
                {tempTotal += detail.Amount;}}
            _TotalDeposit = tempTotal;
            if (forceChangeEvent) {OnChanged("TotalDeposit", TotalDeposit, 
                _TotalDeposit);}
            ;
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account DepositToAccount {
            get { return _DepositToAccount; }
            set { SetPropertyValue("DepositToAccount", ref _DepositToAccount, 
                value); }
        }
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        public DepositStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != DepositStatusEnum.Current) {Approved = true;} 
                    else {
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
        public Deposit(Session session): base(session) {
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
            "Code", "DT"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "DT"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "DT"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Account tmp = Session.FindObject<Account>(CriteriaOperator.Parse(
            "Contains([Name], 'Undeposited')"));
            this.AccountToDeposit = tmp != null ? tmp : null;
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Deposits."
                );} }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _Count = null;
            _TotalDeposit = null;
        }
    }
}
