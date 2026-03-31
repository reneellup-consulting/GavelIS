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
    public class FuelSoaDetail : XPObject
    {
        private ReceiptFuel _Source;
        private DateTime _EntryDate;
        private TripType _TypeOfTrip;
        private FixedAsset _TruckNo;
        private Employee _Driver;
        private decimal _OdoRead;
        private string _RefNo;
        private string _InvoiceNo;
        private string _DtrNos;
        private bool _DtrsTagged;
        private string _Comments;
        private string _CodeNo;
        private TripLocation _Origin;
        private TripLocation _Destination;
        private decimal _Tad;
        private decimal _TotalQty;
        private decimal _Price;
        private FuelStatementOfAccount _FuelStatementOfAccountID;
        private bool _Pay;
        private int _Seq;
        private decimal _OpenAmount;
        [Association("FuelStatementOfAccount-Details")]
        public FuelStatementOfAccount FuelStatementOfAccountID
        {
            get { return _FuelStatementOfAccountID; }
            set
            {
                FuelStatementOfAccount oldFuelStatementOfAccountID = _FuelStatementOfAccountID;
                SetPropertyValue("FuelStatementOfAccountID", ref 
                _FuelStatementOfAccountID, value);
                if (!IsLoading && !IsSaving && oldFuelStatementOfAccountID != _FuelStatementOfAccountID)
                {
                    oldFuelStatementOfAccountID = oldFuelStatementOfAccountID ?? _FuelStatementOfAccountID;
                    oldFuelStatementOfAccountID.UpdateTotal(true);
                }
            }
        }
        [ImmediatePostData]
        [Custom("AllowEdit", "False")]
        public bool Pay
        {
            get { return _Pay; }
            set
            {
                SetPropertyValue("Pay", ref _Pay, value);
                if (!IsLoading)
                {
                    try
                    {
                        _FuelStatementOfAccountID.UpdateSelectedCharges(true);
                    }
                    catch (Exception) { }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public int Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        // SourceNo
        [Custom("AllowEdit", "False")]
        [DisplayName("Source No")]
        public ReceiptFuel Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }
        // EntryDate
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // TypeOfTrip
        [Custom("AllowEdit", "False")]
        public TripType TypeOfTrip
        {
            get { return _TypeOfTrip; }
            set { SetPropertyValue("TypeOfTrip", ref _TypeOfTrip, value); }
        }
        // TruckNo
        [Custom("AllowEdit", "False")]
        public FixedAsset TruckNo
        {
            get { return _TruckNo; }
            set { SetPropertyValue("TruckNo", ref _TruckNo, value); }
        }
        // Driver
        [Custom("AllowEdit", "False")]
        public Employee Driver
        {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        // OdoRead
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal OdoRead
        {
            get { return _OdoRead; }
            set { SetPropertyValue("OdoRead", ref _OdoRead, value); }
        }
        // RefNo
        [Custom("AllowEdit", "False")]
        public string RefNo
        {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }
        // InvoiceNo
        [Custom("AllowEdit", "False")]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
        }
        // DtrNos
        [Custom("AllowEdit", "False")]
        [Size(1000)]
        public string DtrNos
        {
            get { return _DtrNos; }
            set { SetPropertyValue("DtrNos", ref _DtrNos, value); }
        }
        // DtrsTagged
        [Custom("AllowEdit", "False")]
        public bool DtrsTagged
        {
            get { return _DtrsTagged; }
            set { SetPropertyValue("DtrsTagged", ref _DtrsTagged, value); }
        }
        // Comments
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited)]
        public string Comments
        {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        // CodeNo
        [Custom("AllowEdit", "False")]
        public string CodeNo
        {
            get { return _CodeNo; }
            set { SetPropertyValue("CodeNo", ref _CodeNo, value); }
        }
        // Origin
        [Custom("AllowEdit", "False")]
        public TripLocation Origin
        {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        // Destination
        [Custom("AllowEdit", "False")]
        public TripLocation Destination
        {
            get { return _Destination; }
            set { SetPropertyValue("Destination", ref _Destination, value); }
        }
        // Tad
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Tad
        {
            get { return _Tad; }
            set { SetPropertyValue("Tad", ref _Tad, value); }
        }
        // TotalQty
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalQty
        {
            get { return _TotalQty; }
            set { SetPropertyValue("TotalQty", ref _TotalQty, value); }
        }
        // Price
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Price
        {
            get { return _Price; }
            set { SetPropertyValue("Price", ref _Price, value); }
        }
        // Total
        [PersistentAlias("TotalQty * Price")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount
        {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Adjust
        {
            get { return _Adjust; }
            set
            {
                SetPropertyValue("Adjust", ref _Adjust, value);
                if (!IsLoading)
                {
                    try
                    {
                        _FuelStatementOfAccountID.UpdateSelectedCharges(true);
                        _FuelStatementOfAccountID.UpdateTotal(true);
                    }
                    catch (Exception) { }
                }
            }
        }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        #endregion
        public FuelSoaDetail(Session session)
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
        protected override void OnSaving()
        {
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
        private decimal _Adjust;
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
