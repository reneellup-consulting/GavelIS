using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
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
using System.IO;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class FuelPurchaseOrder : GenJournalHeader
    {
        private string _ReferenceNo;
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        private string _Memo;
        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo
        {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        private string _Comments;
        [Size(500)]
        public string Comments
        {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }

        private bool _IsReopened;
        [Custom("AllowEdit", "False")]
        public bool IsReopened
        {
            get { return _IsReopened; }
            set { SetPropertyValue("IsReopened", ref _IsReopened, value); }
        }

        private string _AfterReopenAlterations;
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        [DisplayName("After Re-opening Changes")]
        public string AfterReopenAlterations
        {
            get { return _AfterReopenAlterations; }
            set { SetPropertyValue("AfterReopenAlterations", ref _AfterReopenAlterations, value); }
        }

        private PurchaseOrderStatusEnum _Status;
        public PurchaseOrderStatusEnum Status
        {
            get { return _Status; }
            set
            {
                PurchaseOrderStatusEnum oldStatus = _Status;
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    InventoryControlJournal _icj;
                    if (oldStatus == PurchaseOrderStatusEnum.Approved && value == PurchaseOrderStatusEnum.Current)
                    {
                        IsReopened = true;
                        StringBuilder sb = new StringBuilder(_AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendFormat("{0}*** Reopened by {1}", stamp, user);
                        AfterReopenAlterations = sb.ToString();
                    }
                    if (_Status != PurchaseOrderStatusEnum.Current)
                    {
                        Approved =
                        true;
                    }
                    else
                    {
                        Approved = false;
                        Printed = false;
                    }
                }
                switch (_Status)
                {
                    case PurchaseOrderStatusEnum.Current:
                        Approved = false;
                        Printed = false;
                        ManualPrinted = false;
                        break;
                    case PurchaseOrderStatusEnum.Approved:
                        Approved = true;
                        ApprovedDate = DateTime.Now;
                        break;
                    case PurchaseOrderStatusEnum.PartiallyReceived:
                        break;
                    case PurchaseOrderStatusEnum.Received:
                        break;
                    case PurchaseOrderStatusEnum.Disapproved:
                        Approved = false;
                        DisapprovedDate = DateTime.Now;
                        break;
                    case PurchaseOrderStatusEnum.Pending:
                        Approved = false;
                        PendingDate = DateTime.Now;
                        break;
                    default:
                        break;
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }

        private string _StatusBy;
        public string StatusBy
        {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        private DateTime _StatusDate;
        public DateTime StatusDate
        {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        private Vendor _Vendor;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor
        {
            get { return _Vendor; }
            set
            {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null)
                {
                    VendorAddress = _Vendor.FullAddress;
                    Terms = _Vendor.Terms;
                }
            }
        }

        private string _VendorAddress;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string VendorAddress
        {
            get { return _VendorAddress; }
            set { SetPropertyValue("VendorAddress", ref _VendorAddress, value); }
        }

        private Terms _Terms;
        public Terms Terms
        {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }

        private string _Remarks;
        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        private DateTime _ApprovedDate;
        [Custom("AllowEdit", "False")]
        public DateTime ApprovedDate
        {
            get { return _ApprovedDate; }
            set { SetPropertyValue<DateTime>("ApprovedDate", ref _ApprovedDate, value); }
        }

        private DateTime _DisapprovedDate;
        [Custom("AllowEdit", "False")]
        public DateTime DisapprovedDate
        {
            get { return _DisapprovedDate; }
            set { SetPropertyValue<DateTime>("DisapprovedDate", ref _DisapprovedDate, value); }
        }

        private DateTime _PendingDate;
        [Custom("AllowEdit", "False")]
        public DateTime PendingDate
        {
            get { return _PendingDate; }
            set { SetPropertyValue<DateTime>("PendingDate", ref _PendingDate, value); }
        }

        private bool _ManualPrinted = false;
        [Custom("AllowEdit", "False")]
        public bool ManualPrinted
        {
            get { return _ManualPrinted; }
            set { SetPropertyValue<bool>("ManualPrinted", ref _ManualPrinted, value); }
        }

        private bool _Printed = false;
        [Custom("AllowEdit", "False")]
        public bool Printed
        {
            get { return _Printed; }
            set { SetPropertyValue<bool>("Printed", ref _Printed, value); }
        }

        #region Calculated Details

        [Persistent("Total")]
        private decimal? _Total;
        [Persistent("_Total")]
        public decimal? Total
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _Total == null)
                    {
                        UpdateTotal(false);
                    }
                }
                catch (Exception)
                {
                }
                return _Total;
            }
        }

        public void UpdateTotal(bool forceChangeEvent)
        {
            decimal? oldTotal = _Total;
            decimal tempTotal = 0m;
            foreach (FuelPurchaseOrderDetail detail in FuelPurchaseOrderDetails)
            {
                tempTotal += detail.Total;
            }
            _Total = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("Total", Total, _Total);
            }
            ;
        }

        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }

        private void Reset()
        {
            _Total = null;
        }

        #endregion

        public FuelPurchaseOrder(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "PO"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PO"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "PO"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            Memo = "Purchase Order #" + SourceNo;
        }
    }

}
