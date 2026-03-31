using System;
using System.Linq;
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
using System.Text;
using System.Collections.Generic;
using System.IO;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class PurchaseOrderFuel : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private PurchaseOrderFuelStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Vendor _Vendor;
        private string _VendorAddress;

        private BusinessObjects.Customer _Customer;
        private string _ShipToAddress;
        private Terms _Terms;
        private ShipVia _ShipVia;
        private DateTime _ExpectedDate;
        [Size(SizeAttribute.Unlimited)]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        public override DateTime EntryDate
        {
            get
            {
                return base.EntryDate;
            }
            set
            {
                base.EntryDate = value;
                if (!IsLoading)
                {
                    if (_TruckNo != null)
                    {
                        FuelUsageClassification = _TruckNo.FuelUsageClassification;
                        GetLastGasUp(_TruckNo);
                    }
                    else if (_GensetNo != null)
                    {
                        GetLastGasUp(_GensetNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        PrevOdoRead = 0;
                        PrevHrsRead = 0;
                        NoOfLtrs = 0;
                        PrevPrice = 0;
                        PrevTotalAmt = 0;
                    }
                }
            }
        }
        private List<string> _Refs;
        [Custom("AllowEdit", "False")]
        public List<string> Refs
        {
            get { return _Refs; }
            set { SetPropertyValue<List<string>>("Refs", ref _Refs, value); }
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
        [Size(500)]
        public string RejectionReason
        {
            get { return _RejectionReason; }
            set { SetPropertyValue("RejectionReason", ref _RejectionReason, value); }
        }
        public PurchaseOrderFuelStatusEnum Status {
            get { return _Status; }
            set {
                PurchaseOrderFuelStatusEnum oldStatus = _Status;
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (oldStatus == PurchaseOrderFuelStatusEnum.Approved && value == PurchaseOrderFuelStatusEnum.Current)
                    {
                        IsReopened = true;
                        StringBuilder sb = new StringBuilder(_AfterReopenAlterations);
                        string stamp = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
                        string user = CurrentUser != null ? CurrentUser.UserName : string.Empty;
                        sb.AppendFormat("{0}*** Reopened by {1}", stamp, user).AppendLine();
                        AfterReopenAlterations = sb.ToString();
                    }
                    if (_Status != PurchaseOrderFuelStatusEnum.Current)
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
                    case PurchaseOrderFuelStatusEnum.Current:
                        Approved = false;
                        Printed = false;
                        ManualPrinted = false;
                        break;
                    case PurchaseOrderFuelStatusEnum.Approved:
                        Approved = true;
                        ApprovedDate = DateTime.Now;
                        break;
                    case PurchaseOrderFuelStatusEnum.PartiallyReceived:
                        break;
                    case PurchaseOrderFuelStatusEnum.Received:
                        break;
                    case PurchaseOrderFuelStatusEnum.Disapproved:
                        Approved = false;
                        DisapprovedDate = DateTime.Now;
                        break;
                    case PurchaseOrderFuelStatusEnum.Pending:
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
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        private BusinessObjects.TripType _TripType;
        public TripType TripType
        {
            get { return _TripType; }
            set { SetPropertyValue("TripType", ref _TripType, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor {
            get { return _Vendor; }
            set {
                SetPropertyValue("Vendor", ref _Vendor, value);
                if (!IsLoading && _Vendor != null) {
                    VendorAddress = _Vendor.FullAddress;
                    Terms = _Vendor.Terms;
                }
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
        public Customer Customer
        {
            get { return _Customer; }
            set
            {
                SetPropertyValue("Customer", ref _Customer, value);
                if (!IsLoading && _Customer != null)
                {
                    Terms = _Customer.Terms !=
                    null ? _Customer.Terms : null;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(500)]
        public string ShipToAddress {
            get { return _ShipToAddress; }
            set { SetPropertyValue("ShipToAddress", ref _ShipToAddress, value); 
            }
        }
        public Terms Terms {
            get { return _Terms; }
            set { SetPropertyValue("Terms", ref _Terms, value); }
        }
        public ShipVia ShipVia {
            get { return _ShipVia; }
            set { SetPropertyValue("ShipVia", ref _ShipVia, value); }
        }
        public DateTime ExpectedDate {
            get { return _ExpectedDate; }
            set { SetPropertyValue("ExpectedDate", ref _ExpectedDate, value); }
        }
        #region Fuel Information
        private TruckOrGensetEnum _TruckOrGenset;
        private FATruck _TruckNo;
        private FAGeneratorSet _GensetNo;
        private Employee _Driver;
        [ImmediatePostData]
        public TruckOrGensetEnum TruckOrGenset {
            get { return _TruckOrGenset; }
            set {
                SetPropertyValue("TruckOrGenset", ref _TruckOrGenset, value);
                if (!IsLoading) {
                    switch (_TruckOrGenset) {
                        case TruckOrGensetEnum.Truck:
                            GensetNo = null;
                            OtherNo = null;
                            OtherVehicle = null;
                            MtrRead = 0;
                            OthRead = 0;
                            break;
                        case TruckOrGensetEnum.Genset:
                            TruckNo = null;
                            OtherNo = null;
                            OtherVehicle = null;
                            OdoRead = 0;
                            OthRead = 0;
                            break;
                        case TruckOrGensetEnum.Other:
                            TruckNo = null;
                            GensetNo = null;
                            OdoRead = 0;
                            MtrRead = 0;
                            break;
                        default:
                            TruckNo = null;
                            GensetNo = null;
                            OtherNo = null;
                            OtherVehicle = null;
                            OdoRead = 0;
                            MtrRead = 0;
                            OthRead = 0;
                            break;
                    }
                    PrevDate = DateTime.MinValue;
                    PrevOdoRead = 0;
                    PrevHrsRead = 0;
                    PrevLife = 0m;
                    NoOfLtrs = 0;
                    PrevPrice = 0;
                    PrevTotalAmt = 0;
                }
            }
        }
        public FATruck TruckNo
        {
            get { return _TruckNo; }
            set
            {
                SetPropertyValue("TruckNo", ref _TruckNo, value);
                if (!IsLoading)
                {
                    if (_TruckNo != null)
                    {
                        FuelUsageClassification = _TruckNo.FuelUsageClassification;
                        Driver = _TruckNo.Operator ?? null;
                        GetLastGasUp(_TruckNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        OdoRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        public FAOtherVehicle OtherNo
        {
            get { return _OtherNo; }
            set
            {
                SetPropertyValue("OtherNo", ref _OtherNo, value);
                if (!IsLoading)
                {
                    if (_OtherNo != null)
                    {
                        FuelUsageClassification = _OtherNo.FuelUsageClassification;
                        Driver = _OtherNo.Operator ?? null;
                        GetLastGasUp(_OtherNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        OthRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoRead
        {
            get { return _OdoRead; }
            set { SetPropertyValue("OdoRead", ref _OdoRead, value); }
        }
        public FAGeneratorSet GensetNo
        {
            get { return _GensetNo; }
            set
            {
                SetPropertyValue("GensetNo", ref _GensetNo, value);
                if (!IsLoading)
                {
                    if (_GensetNo != null)
                    {
                        FuelUsageClassification = _GensetNo.FuelUsageClassification;
                        Driver = _GensetNo.Operator ?? null;
                        GetLastGasUp(_GensetNo);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        MtrRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal MtrRead
        {
            get { return _MtrRead; }
            set { SetPropertyValue("MtrRead", ref _MtrRead, value); }
        }
        public FAOtherVehicle OtherVehicle
        {
            get { return _OtherVehicle; }
            set
            {
                SetPropertyValue("OtherVehicle", ref _OtherVehicle, value);
                if (!IsLoading)
                {
                    if (_OtherVehicle != null)
                    {
                        FuelUsageClassification = _OtherVehicle.FuelUsageClassification;
                        Driver = _OtherVehicle.Operator ?? null;
                        GetLastGasUp(_OtherVehicle);
                    }
                    else
                    {
                        PrevDate = DateTime.MinValue;
                        OthRead = 0m;
                        PrevOdoRead = 0m;
                        PrevLife = 0m;
                        NoOfLtrs = 0m;
                        PrevPrice = 0m;
                        PrevTotalAmt = 0m;
                    }
                }
            }
        }
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OthRead
        {
            get { return _OthRead; }
            set { SetPropertyValue("OthRead", ref _OthRead, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        public FuelUsageClassEnum FuelUsageClassification
        {
            get { return _FuelUsageClassification; }
            set { SetPropertyValue<FuelUsageClassEnum>("FuelUsageClassification", ref _FuelUsageClassification, value); }
        }
        [Action(Caption = "Get Previous Reading", AutoCommit = true)]
        public void GetPreviousReading()
        {
            if (_TruckNo != null)
            {
                FuelUsageClassification = _TruckNo.FuelUsageClassification;
                //Driver = _TruckNo.Operator ?? null;
                GetLastGasUp(_TruckNo);
            }
            else
            {
                PrevDate = DateTime.MinValue;
                OdoRead = 0m;
                PrevOdoRead = 0m;
                PrevLife = 0m;
                NoOfLtrs = 0m;
                PrevPrice = 0m;
                PrevTotalAmt = 0m;
            }
        }
        private void GetLastGasUp(FixedAsset asset)
        {
            // Retreive selected fleet
            FixedAsset selFa = null;
            switch (_TruckOrGenset)
            {
                case TruckOrGensetEnum.Truck:
                    if (_TruckNo != null)
                    {
                        selFa = _TruckNo;
                    }
                    break;
                case TruckOrGensetEnum.Genset:
                    if (_GensetNo != null)
                    {
                        selFa = _GensetNo;
                    }
                    break;
                case TruckOrGensetEnum.NotApplicable:
                    break;
                case TruckOrGensetEnum.Other:
                    if (_OtherVehicle != null)
                    {
                        selFa = _OtherVehicle;
                    }
                    break;
                default:
                    break;
            }
            if (selFa != null)
            {
                //FuelOdoRegistry fuelLog = null;
                if (EntryDate != DateTime.MinValue)
                {
                    string seq = EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, 999999)
                       : string.Empty;
                    decimal toDecimal = Convert.ToDecimal(seq);
                    if (selFa.GetType() == typeof(FATruck))
                    {
                        FATruck faTruck = selFa as FATruck;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevDate = data.EntryDate;
                            OdoRead = data.Reading;
                            PrevOdoRead = data.Reading;
                            PrevLife = data.Life;
                            NoOfLtrs = data.ReceiptId != null ? data.ReceiptId.TotalQty.Value : 0m;
                            PrevPrice = data.ReceiptId != null ? data.ReceiptId.Price.Value : 0m;
                            PrevTotalAmt = data.ReceiptId != null ? data.ReceiptId.Total.Value : 0m;
                        }
                    }
                    else if (selFa.GetType() == typeof(FAGeneratorSet))
                    {
                        FAGeneratorSet faTruck = selFa as FAGeneratorSet;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevDate = data.EntryDate;
                            MtrRead = data.Reading;
                            PrevOdoRead = data.Reading;
                            PrevLife = data.Life;
                            NoOfLtrs = data.ReceiptId != null ? data.ReceiptId.TotalQty.Value : 0m;
                            PrevPrice = data.ReceiptId != null ? data.ReceiptId.Price.Value : 0m;
                            PrevTotalAmt = data.ReceiptId != null ? data.ReceiptId.Total.Value : 0m;
                        }
                    }
                    else if (selFa.GetType() == typeof(FAOtherVehicle))
                    {
                        FAOtherVehicle faTruck = selFa as FAOtherVehicle;
                        var data = faTruck.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevDate = data.EntryDate;
                            OthRead = data.Reading;
                            PrevOdoRead = data.Reading;
                            PrevLife = data.Life;
                            NoOfLtrs = data.ReceiptId != null ? data.ReceiptId.TotalQty.Value : 0m;
                            PrevPrice = data.ReceiptId != null ? data.ReceiptId.Price.Value : 0m;
                            PrevTotalAmt = data.ReceiptId != null ? data.ReceiptId.Total.Value : 0m;
                        }
                    }
                }
            }
        }
        #endregion
        #region Previous
        private DateTime _PrevDate;
        private decimal _PrevOdoRead;
        private decimal _PrevHrsRead;
        private decimal _NoOfLtrs;
        private decimal _Price;
        private decimal _PrevTotalAmt;
        private decimal _OdoRead;
        private decimal _MtrRead;
        private FAOtherVehicle _OtherVehicle;
        private decimal _OthRead;
        private FAOtherVehicle _OtherNo;
        private decimal _PrevLife;
        private FuelUsageClassEnum _FuelUsageClassification;
        private decimal _PrevPrice;
        private bool _IsReopened;
        private string _AfterReopenAlterations;
        private string _Remarks;
        private DateTime _ApprovedDate;
        private DateTime _DisapprovedDate;
        private DateTime _PendingDate;
        private bool _ManualPrinted;
        private bool _Printed;
        
        [Custom("AllowEdit", "False")]
        public DateTime PrevDate {
            get { return _PrevDate; }
            set { SetPropertyValue("PrevDate", ref _PrevDate, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevOdoRead {
            get { return _PrevOdoRead; }
            set { SetPropertyValue("PrevOdoRead", ref _PrevOdoRead, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevHrsRead {
            get { return _PrevHrsRead; }
            set { SetPropertyValue("PrevHrsRead", ref _PrevHrsRead, value); }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal PrevLife
        {
            get { return _PrevLife; }
            set { SetPropertyValue("PrevLife", ref _PrevLife, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal NoOfLtrs {
            get { return _NoOfLtrs; }
            set { SetPropertyValue("NoOfLtrs", ref _NoOfLtrs, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal PrevPrice
        {
            get { return _PrevPrice; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Price {
            get { return _Price; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal PrevTotalAmt {
            get { return _PrevTotalAmt; }
            set { SetPropertyValue("PrevTotalAmt", ref _PrevTotalAmt, value); }
        }
        #endregion

        [Aggregated, Association("PurchaseOrderFuelUsageDetails")]
        public XPCollection<PurchaseOrderFuelUsageDetail> PurchaseOrderFuelUsageDetails
        {
            get
            {
                return GetCollection<PurchaseOrderFuelUsageDetail>("PurchaseOrderFuelUsageDetails"
                    );
            }
        }

        // Create a propert that will display Document Nos tagged in PurchaseOrderFuelUsageDetails
        [NonPersistent]
        [DisplayName("Tagged Trips")] // Optional: for a nicer display name in XAF UI
        public string TaggedDocumentNos
        {
            get
            {
                if (PurchaseOrderFuelUsageDetails == null || !PurchaseOrderFuelUsageDetails.Any())
                {
                    return string.Empty;
                }

                // Assumption: PurchaseOrderFuelUsageDetail has a reference property to an object
                // that contains the document number.
                // Example: 'LinkedFuelUsageDocument' is a property on PurchaseOrderFuelUsageDetail,
                // and this 'LinkedFuelUsageDocument' has a 'SourceNo' property.
                // Please adjust 'detail.LinkedFuelUsageDocument.SourceNo' to match your actual data model.
                // If PurchaseOrderFuelUsageDetail directly contains the document number as a string property (e.g., 'TaggedDocumentNumber'),
                // then it would be 'detail.TaggedDocumentNumber'.

                var documentNumbers = PurchaseOrderFuelUsageDetails
                    // Replace 'LinkedFuelUsageDocument' and 'SourceNo' with your actual property names.
                                        .Where(detail => detail.DocumentNo != null && !string.IsNullOrEmpty(detail.DocumentNo
                                            ))
                                        .Select(detail => detail.DocumentNo)
                                        .Distinct() // Ensures each document number appears only once
                                        .OrderBy(docNo => docNo) // Optional: for sorted order
                                        .ToList(); // Materialize the list for joining

                return string.Join(", ", documentNumbers);
            }
        }

        [Custom("AllowEdit", "False")]
        public bool IsReopened
        {
            get { return _IsReopened; }
            set { SetPropertyValue("Comments", ref _IsReopened, value); }
        }

        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        [DisplayName("After Re-opening Changes")]
        public string AfterReopenAlterations
        {
            get { return _AfterReopenAlterations; }
            set { SetPropertyValue("AfterReopenAlterations", ref _AfterReopenAlterations, value); }
        }

        [Size(SizeAttribute.Unlimited)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime ApprovedDate
        {
            get { return _ApprovedDate; }
            set { SetPropertyValue<DateTime>("ApprovedDate", ref _ApprovedDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime DisapprovedDate
        {
            get { return _DisapprovedDate; }
            set { SetPropertyValue<DateTime>("DisapprovedDate", ref _DisapprovedDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime PendingDate
        {
            get { return _PendingDate; }
            set { SetPropertyValue<DateTime>("PendingDate", ref _PendingDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool ManualPrinted
        {
            get { return _ManualPrinted; }
            set { SetPropertyValue<bool>("ManualPrinted", ref _ManualPrinted, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Printed
        {
            get { return _Printed; }
            set { SetPropertyValue<bool>("Printed", ref _Printed, value); }
        }

        public string ChargeTo
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (POrderFuelDetails != null && POrderFuelDetails.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in POrderFuelDetails)
                    {
                        if (item.CostCenter != null && !strRefs.Contains(item.CostCenter.Code))
                        {
                            strRefs.Add(item.CostCenter.Code);
                            sb.AppendFormat("{0},", item.CostCenter.Code);
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }

        [Action(Caption = "Manual PO")]
        public void ManuallyPrinted()
        {
            if (_Printed || _ManualPrinted)
            {
                throw new ApplicationException("PO already printed");
            }
            Printed = true;
            ManualPrinted = true;
            if (!string.IsNullOrEmpty(Remarks))
            {
                Remarks = "Manually Printed PO#:" + Environment.NewLine + Remarks;
            }
            else
            {
                Remarks = "Manually Printed PO#:";
            }
            this.Session.CommitTransaction();
        }

        // PrintDocument
        [Action(Caption = "Print PO")]
        public void PrintDocument()
        {
            if (!Approved)
            {
                throw new ApplicationException("Cannot print unapproved PO");
            }
            if (_Printed || _ManualPrinted)
            {
                throw new ApplicationException("PO already printed");
            }
            Printed = true;
            ManualPrinted = true;
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\PurchaseOrderFuel.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            rep.DataSource = POrderFuelDetails;
            rep.ShowPreview();
        }

        // ODO/HRMTR Consolidation
        #region ODO/HRMTR Consolidation

        [NonPersistent]
        [DisplayName("Unit")]
        [Custom("AllowEdit", "False")]
        public string ReadingUnit
        {
            get
            {
                switch (TruckOrGenset)
                {
                    case TruckOrGensetEnum.Truck:
                        return string.Format("{0:n} KMS", _OdoRead);
                    case TruckOrGensetEnum.Genset:
                        return string.Format("{0:n} HRS", _MtrRead);
                    case TruckOrGensetEnum.Other:
                        return string.Format("{0:n}", _OthRead);
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion

        #region Calculated Details
        [Persistent("Total")]
        private decimal? _Total;
        private string _RejectionReason;
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
            foreach (POrderFuelDetail detail in POrderFuelDetails)
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
        
        public PurchaseOrderFuel(Session session): base(session) {
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
            "Code", "PF"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PF"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "PF"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
            ShipToAddress = company.FullShipAddress;
            Memo = "Purchase Order(Fuel) #" + SourceNo;
        }
        protected override void OnDeleting()
        {
            if (Approved)
            {
                throw new UserFriendlyException(
                "The system prohibits the deletion of already approved Purchase Order transactions."
                );
            }

            // --- NEW SYNC QUEUE BLOCK ---
            PoSyncDeletionsQueue syncQueue = new PoSyncDeletionsQueue(Session);
            syncQueue.POType = "Fuel";
            syncQueue.RowType = "Parent";
            syncQueue.RowId = this.Oid;
            // ----------------------------

            base.OnDeleting();
        }

        protected override void OnSaving()
        {
            // If the record has been modified and was previously synced, flag for re-sync
            if (!IsLoading && !IsDeleted && IsSynced)
            {
                ReSynced = true;
            }
            base.OnSaving();
        }

        public void UpdateResync(bool value)
        {
            OnChanged("ReSynced", ReSynced, value);
        }
    }
}
