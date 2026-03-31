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
    public class CheckVoucher : GenJournalHeader {
        private string _CheckNo;
        private PaymentTypeEnum _PaymentMode;
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private CheckStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Contact _PayToOrder;
        private bool _ToPrint = true;
        private bool _Printed;
        private decimal _Adjusted;
        private decimal _OpenAmount;
        //[Custom("AllowEdit", "False")]
        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue("CheckNo", ref _CheckNo, value); }
        }
        public PaymentTypeEnum PaymentMode {
            get { return _PaymentMode; }
            set { SetPropertyValue("PaymentMode", ref _PaymentMode, value); }
        }
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        [Custom("AllowEdit", "False")]
        public CheckStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != CheckStatusEnum.Current) {
                        Approved = true;
                        foreach (GenJournalDetail item in GenJournalDetails) {
                            item.Approved = true;}
                    } else {
                        Approved = false;
                        foreach (GenJournalDetail item in GenJournalDetails) {
                            item.Approved = false;}
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
        [Action(Caption="Approve Selected", ConfirmationMessage="Are you sure you want to do this?", AutoCommit=true)]
        public void MarkSelectedAsApproved() {
            if (_Status != CheckStatusEnum.Voided && _Status == CheckStatusEnum.Current)
            {
                Status = CheckStatusEnum.Approved;
            }
        }
        
        [Custom("AllowEdit", "False")]
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Contact PayToOrder {
            get { return _PayToOrder; }
            set { SetPropertyValue("PayToOrder", ref _PayToOrder, value); }
        }
        public bool ToPrint {
            get { return _ToPrint; }
            set { SetPropertyValue("ToPrint", ref _ToPrint, value); }
        }
        public bool Printed {
            get { return _Printed; }
            set { SetPropertyValue("Printed", ref _Printed, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set { SetPropertyValue("Adjusted", ref _Adjusted, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }
        #region Check Amount Details Computation
        [Persistent("CheckAmount")]
        private decimal? _CheckAmount;
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("_CheckAmount")]
        public decimal? CheckAmount {
            get {
                try {
                    if (!IsLoading && !IsSaving && _CheckAmount == null) {
                        UpdateCheckAmount(false);}
                } catch (Exception) {}
                return _CheckAmount;
            }
        }
        public void UpdateCheckAmount(bool forceChangeEvent) {
            decimal? oldCheckAmount = _CheckAmount;
            decimal tempTotal = 0m;
            foreach (GenJournalDetail detail in GenJournalDetails) {if (detail.
                IsCheckAmount) {tempTotal += detail.CreditAmount;}}
            _CheckAmount = tempTotal;
            if (forceChangeEvent) {OnChanged("CheckAmount", CheckAmount, 
                _CheckAmount);}
            ;
        }
        #endregion
        public CheckVoucher(Session session): base(session) {
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "CR"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "CV"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "CR"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
        }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            //this.IsIncExpNeedUpdate = true;
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            base.TriggerObjectChanged(args);
        }

        protected override void OnSaving()
        {
            //this.AutoRegisterIncomeExpenseVer();
            base.OnSaving();
        }
        protected override void OnSaved()
        {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            base.OnSaved();
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Check Voucher."
                );} }
    }
}
