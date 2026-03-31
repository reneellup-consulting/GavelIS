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
    public class CreditMemo : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private CreditMemoStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Customer _Customer;
        private string _CustomerAddress;
        private string _ShipToAddress;
        private ShipVia _ShipVia;
        private Invoice _InvoiceNo;
        private decimal _Adjusted;
        private decimal _OpenAmount;
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
        public CreditMemoStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != CreditMemoStatusEnum.Current) {Approved = 
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
        [Association("Customer-CreditMemos")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Customer Customer {
            get { return _Customer; }
            set {
                SetPropertyValue("Customer", ref _Customer, value);
                if (!IsLoading && _Customer != null) {
                    CustomerAddress = _Customer.FullAddress;
                    ShipToAddress = _Customer.FullShipAddress;
                    this.ShipVia = _Customer.ShipVia != null ? _Customer.ShipVia 
                    : null;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string CustomerAddress {
            get { return _CustomerAddress; }
            set { SetPropertyValue("CustomerAddress", ref _CustomerAddress, 
                value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string ShipToAddress {
            get { return _ShipToAddress; }
            set { SetPropertyValue("ShipToAddress", ref _ShipToAddress, value); 
            }
        }
        public ShipVia ShipVia {
            get { return _ShipVia; }
            set { SetPropertyValue("ShipVia", ref _ShipVia, value); }
        }
        public Invoice InvoiceNo {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
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
        #region Calculated Details
        [Persistent("TotalTax")]
        private decimal? _TotalTax;
        [Persistent("GrossTotal")]
        private decimal? _GrossTotal;
        [PersistentAlias("_TotalTax")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalTax {
            get {
                try {
                    if (!IsLoading && !IsSaving && _TotalTax == null) {
                        UpdateTotalTax(false);}
                } catch (Exception) {}
                return _TotalTax;
            }
        }
        [PersistentAlias("_GrossTotal")]
        [Custom("DisplayFormat", "n")]
        public decimal? GrossTotal {
            get {
                try {
                    if (!IsLoading && !IsSaving && _GrossTotal == null) {
                        UpdateGrossTotal(false);}
                } catch (Exception) {}
                return _GrossTotal;
            }
        }
        public void UpdateTotalTax(bool forceChangeEvent) {
            decimal? oldTotalTax = _TotalTax;
            decimal tempTotal = 0m;
            foreach (CreditMemoDetail detail in CreditMemoDetails) {if (detail.
                Tax != null && detail.Tax.Taxable) {
                    tempTotal += detail.Total - (detail
                        .Total / (1 + (detail.Tax.Rate / 100)));
                //tempTotal += detail.Price * 
                //    (detail.Tax.Rate / 100);

            }}
            _TotalTax = tempTotal;
            if (forceChangeEvent) {OnChanged("TotalTax", TotalTax, _TotalTax);}
            ;
        }
        public void UpdateGrossTotal(bool forceChangeEvent) {
            decimal? oldGrossTotal = _GrossTotal;
            decimal tempTotal = 0m;
            foreach (CreditMemoDetail detail in CreditMemoDetails) {
                tempTotal +=
                detail.Total;
                //tempTotal += 
                //detail.Price + (detail.Price * (detail.Tax.Rate / 100));
            }
            _GrossTotal = tempTotal;
            if (forceChangeEvent) {OnChanged("GrossTotal", GrossTotal, 
                _GrossTotal);}
            ;
        }
        #endregion
        public CreditMemo(Session session): base(session) {
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
            "Code", "CM"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "RE"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "CM"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            Memo = "Credit Memo #" + SourceNo;
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
                "The system prohibits the deletion of already approved Credit Memo transactions."
                );} }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _TotalTax = null;
            _GrossTotal = null;
        }
    }
}
