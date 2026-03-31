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
    public class DebitMemo : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private DebitMemoStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Vendor _Vendor;
        private string _VendorAddress;
        private string _BillToAddress;
        private Receipt _ReceiptNo;
        private decimal _Adjusted;
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
        public DebitMemoStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != DebitMemoStatusEnum.Current) {Approved = true
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
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor {
            get { return _Vendor; }
            set {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null) {VendorAddress = _Vendor.
                    FullAddress;}
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string VendorAddress {
            get { return _VendorAddress; }
            set { SetPropertyValue("VendorAddress", ref _VendorAddress, value); 
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string BillToAddress {
            get { return _BillToAddress; }
            set { SetPropertyValue("BillToAddress", ref _BillToAddress, value); 
            }
        }
        public Receipt ReceiptNo {
            get { return _ReceiptNo; }
            set { SetPropertyValue("ReceiptNo", ref _ReceiptNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set { SetPropertyValue("Adjusted", ref _Adjusted, value); }
        }
        [NonPersistent]
        public Company CompanyInfo { get { return Company.GetInstance(Session); 
            } }
        #region Calculated Details
        [Persistent("Total")]
        private decimal? _Total;
        [PersistentAlias("_Total")]
        [Custom("DisplayFormat", "n")]
        public decimal? Total {
            get {
                try {
                    if (!IsLoading && !IsSaving && _Total == null) {UpdateTotal(
                        false);}
                } catch (Exception) {}
                return _Total;
            }
        }
        public void UpdateTotal(bool forceChangeEvent) {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (DebitMemoDetail detail in DebitMemoDetails) {tempTotal += 
                detail.Total;}
            _Total = tempTotal;
            if (forceChangeEvent) {OnChanged("Total", Total, _Total);}
            ;
        }
        #endregion
        public DebitMemo(Session session): base(session) {
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
            "Code", "DM"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "VR"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "DM"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            BillToAddress = company.FullShipAddress;
            Memo = "Vendor Return";
        }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
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
                "The system prohibits the deletion of already approved Debit Memo transactions."
                );} }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() { _Total = null; }
    }
}
