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
    public class FundTransfer : GenJournalHeader {
        private Account _TransferFundsFrom;
        private Account _TransferFundsTo;
        private decimal _AmountToTransfer;
        private string _Memo;
        private FundTransferStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account TransferFundsFrom {
            get { return _TransferFundsFrom; }
            set { SetPropertyValue("TransferFundsFrom", ref _TransferFundsFrom, 
                value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account TransferFundsTo {
            get { return _TransferFundsTo; }
            set { SetPropertyValue("TransferFundsTo", ref _TransferFundsTo, 
                value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal AmountToTransfer {
            get { return _AmountToTransfer; }
            set { SetPropertyValue("AmountToTransfer", ref _AmountToTransfer, 
                value); }
        }
        [Size(1000)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        public FundTransferStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != FundTransferStatusEnum.Current) {Approved = 
                        true;} else {
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
        public FundTransfer(Session session): base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            if (!IsLoading) {
                OperationType = Session.FindObject<OperationType>(new 
                BinaryOperator("Code", "FT"));
                UnitOfWork session2 = new UnitOfWork(this.Session.ObjectLayer);
                SourceType source = session2.FindObject<SourceType>(new 
                BinaryOperator("Code", "FT"));
                if (source != null && OperationType.Code == "FT") {
                    SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? 
                    source.GetNewNo() : null;
                    source.Save();
                    session2.CommitChanges();
                }
            }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public FundTransfer(Session session, OperationType ope): base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            if (!IsLoading) {OperationType = Session.FindObject<OperationType>(
                new BinaryOperator("Code", ope.Code));}
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "FT"));
            //if (_Ope == null) {
            //    OperationType = Session.FindObject<OperationType>(new 
            //    BinaryOperator("Code", "FT"));
            //    UnitOfWork session = new UnitOfWork(this.Session.DataLayer);
            //    SourceType source = session.FindObject<SourceType>(new 
            //    BinaryOperator("Code", "FT"));
            //    if (source != null && OperationType.Code == "FT") {
            //        SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? 
            //        source.GetNewNo() : null;
            //        source.Save();
            //        session.CommitChanges();
            //    }
            //}
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Transfer of Funds."
                );} }
    }
}
