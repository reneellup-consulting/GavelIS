using System;
using System.Linq;
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
    public class ReceiptFuelUsageDetail : XPObject
    {
        private Guid _RowID;
        private ReceiptFuel _HeaderID;
        private GenJournalHeader _TripNo;
        private decimal _FuelQty;

        [NonPersistent]
        public bool MarkToDelete
        {
            get { return _MarkToDelete; }
            set { SetPropertyValue("MarkToDelete", ref _MarkToDelete, value); }
        }
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("ReceiptFuelUsageDetails")]
        public ReceiptFuel HeaderID
        {
            get { return _HeaderID; }
            set { SetPropertyValue("HeaderID", ref _HeaderID, value); }
        }
                
        [Custom("AllowEdit", "False")]
        public GenJournalHeader TripNo
        {
            get { return _TripNo; }
            set { SetPropertyValue("TripNo", ref _TripNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FuelQty
        {
            get { return _FuelQty; }
            set { SetPropertyValue("FuelQty", ref _FuelQty, value); }
        }
        // DocumentNo
        public string DocumentNo
        {
            get {
                if (_TripNo.GetType()==typeof(StanfilcoTrip))
                {
                    return (_TripNo as StanfilcoTrip).DTRNo;
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).DocumentNo;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).TripNo;
                }
                return string.Empty; }
        }
        // Driver
        public Employee Driver
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripNo as StanfilcoTrip).Driver;
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).Driver;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).Driver;
                }
                return null;
            }
        }
        // TruckNo
        public FATruck TruckNo
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripNo as StanfilcoTrip).TruckNo;
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).TruckNo;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).TruckNo;
                }
                return null;
            }
        }
        // Origin
        public TripLocation Origin
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripNo as StanfilcoTrip).Origin;
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).Origin;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).Origin;
                }
                return null;
            }
        }
        // Destination
        public TripLocation Destination
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    return (_TripNo as StanfilcoTrip).Destination;
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).Destination;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).Destination;
                }
                return null;
            }
        }
        // AmountTruck
        public decimal AmountTruck
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    var data= (_TripNo as StanfilcoTrip).TripCalculationDetails.Where(o=>o.Caption=="Trucker Pay").FirstOrDefault();
                    if (data != null)
                    {
                        return data.Value;
                    }
                    else
                    {
                        return 0m;
                    }
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).AmountTruck.Value;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).TruckerPay;
                }
                return 0m;
            }
        }
        // GrossBilling
        public decimal GrossBilling
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    var data = (_TripNo as StanfilcoTrip).TripCalculationDetails.Where(o => o.Caption == "After VAT").FirstOrDefault();
                    if (data != null)
                    {
                        return data.Value;
                    }
                    else
                    {
                        return 0m;
                    }
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).GrossBilling;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).GrossBilling;
                }
                return 0m;
            }
        }

        // WHT
        public decimal Withheld
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    var data = (_TripNo as StanfilcoTrip).TripCalculationDetails.Where(o => o.Caption == "WHT Amount").FirstOrDefault();
                    if (data != null)
                    {
                        return data.Value;
                    }
                    else
                    {
                        return 0m;
                    }
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).WHTAmount;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).WHTAmount;
                }
                return 0m;
            }
        }
        // NetBilling
        public decimal NetBilling
        {
            get
            {
                if (_TripNo.GetType() == typeof(StanfilcoTrip))
                {
                    var data = (_TripNo as StanfilcoTrip).TripCalculationDetails.Where(o => o.Caption == "Net Billing").FirstOrDefault();
                    if (data != null)
                    {
                        return data.Value;
                    }
                    else
                    {
                        return 0m;
                    }
                }
                if (_TripNo.GetType() == typeof(DolefilTrip))
                {
                    return (_TripNo as DolefilTrip).NetBilling;
                }
                if (_TripNo.GetType() == typeof(OtherTrip))
                {
                    return (_TripNo as OtherTrip).NetBilling;
                }
                return 0m;
            }
        }
        [Action(Caption = "Un-tag")]
        public void MarkForProcess()
        {
            FuelRegister freg = Session.FindObject<FuelRegister>(BinaryOperator.Parse("[SourceNo]=?", _HeaderID.SourceNo));
            if (freg !=null)
            {
                freg.Delete();
                this.Delete();
            }
        }
        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private bool _MarkToDelete;
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
        public ReceiptFuelUsageDetail(Session session)
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
    }

}
