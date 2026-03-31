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
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "HeaderId, DetailId", CustomMessageTemplate = "Was already added to this or to an existing reconciliation transaction!")]
    public class DtpReconDetail : XPObject
    {
        #region Fields
        private Guid _RowID;
        private DolefilTripPaymentsRecon _HeaderId;
        private DolefilTrip _Trip;
        private HaulCategory _HaulDescription;
        private HaulType _HType;
        private DolefilTripDetail _DetailId;
        private decimal _Qty;
        private decimal _Fuel1;
        private decimal _Fuel2;
        private decimal _Adjustment;
        private string _AdjReason;
        private decimal _AmountPaid;
        private string _Remarks;

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("DolefilTripPaymentsRecon-Trips")]
        [Custom("AllowEdit", "False")]
        public DolefilTripPaymentsRecon HeaderId
        {
            get { return _HeaderId; }
            set {
                DolefilTripPaymentsRecon oldHeaderId = _HeaderId;
                SetPropertyValue("HeaderId", ref _HeaderId, value);
                if (!IsLoading && !IsSaving && oldHeaderId != _HeaderId)
                {
                    oldHeaderId = oldHeaderId ?? _HeaderId;
                    oldHeaderId.UpdateUnderpayment(true);
                    oldHeaderId.UpdateOverpayment(true);
                    oldHeaderId.UpdateUnpaid(true);
                    oldHeaderId.UpdateTotal(true);
                }
            }
        }

        // Trip
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public DolefilTrip Trip
        {
            get { return _Trip; }
            set { SetPropertyValue("Trip", ref _Trip, value); }
        }
        // DetailId
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        //[RuleUniqueValue("", DefaultContexts.Save,CustomMessageTemplate="Was already added to this or to an existing reconciliation transaction!")]
        public DolefilTripDetail DetailId
        {
            get { return _DetailId; }
            set { SetPropertyValue("DetailId", ref _DetailId, value); }
        }
        // DocumentNo
        public string DocumentNo
        {
            get
            {
                if (_Trip != null)
                {
                    return _Trip.DocumentNo;
                }
                return string.Empty;
            }
        }
        public FATruck SeriesNo
        {
            get
            {
                if (_Trip != null && _Trip.TruckNo != null)
                {
                    return _Trip.TruckNo;
                }
                return null;
            }
        }
        // HaulCategory
        //[RuleRequiredField("", DefaultContexts.Save)]
        public HaulCategory HaulDescription
        {
            //get { return _HaulDescription; }
            //set { SetPropertyValue("HaulDescription", ref _HaulDescription, value); }
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Category;
                }
                return null;
            }
        }
        // HaulType
        //[RuleRequiredField("", DefaultContexts.Save)]
        public HaulType HType
        {
            //get { return _HType; }
            //set { SetPropertyValue("HType", ref _HType, value); }
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Type;
                }
                return null;
            }
        }
        // Tariff
        public Tariff Tariff
        {
            get
            {
                if (_Trip != null && _Trip.Tariff != null)
                {
                    return _Trip.Tariff;
                }
                return null;
            }
        }
        // DateDispatch
        public string DateDispatch
        {
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Start.ToShortDateString();
                }
                return "__/__/____";
            }
        }
        // Start
        public string Start
        {
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Start.ToLongTimeString();
                }
                return "00:00:00";
            }
        }
        // End
        public string End
        {
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Finish.ToLongTimeString();
                }
                return "00:00:00";
            }
        }
        // Qty
        [Custom("DisplayFormat", "n")]
        public decimal Qty
        {
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Quantity;
                }
                return 0m;
            }
        }
        // Rate
        [Custom("DisplayFormat", "n")]
        public decimal Rate
        {
            get
            {
                if (DetailId != null)
                {
                    return DetailId.Rate;
                }
                return 0m;
            }
        }
        // Fuel1
        [Custom("DisplayFormat", "n")]
        public decimal Fuel1
        {
            get { return _Fuel1; }
            set
            {
                SetPropertyValue("Fuel1", ref _Fuel1, value);
                if (!IsSaving && !IsLoading && _HeaderId != null)
                {
                    _HeaderId.UpdateUnderpayment(true);
                    _HeaderId.UpdateOverpayment(true);
                    _HeaderId.UpdateUnpaid(true);
                    _HeaderId.UpdateTotal(true);
                }
            }
        }
        // Fuel2
        [Custom("DisplayFormat", "n")]
        public decimal Fuel2
        {
            get { return _Fuel2; }
            set
            {
                SetPropertyValue("Fuel2", ref _Fuel2, value);
                if (!IsSaving && !IsLoading && _HeaderId != null)
                {
                    _HeaderId.UpdateUnderpayment(true);
                    _HeaderId.UpdateOverpayment(true);
                    _HeaderId.UpdateUnpaid(true);
                    _HeaderId.UpdateTotal(true);
                }
            }
        }
        // AdjReason
        [Size(200)]
        public string AdjReason
        {
            get { return _AdjReason; }
            set { SetPropertyValue("AdjReason", ref _AdjReason, value); }
        }
        // Amount
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Rate + Fuel1 + Fuel2")]
        public decimal Amount
        {
            get
            {
                object tempObject = EvaluateAlias("Amount");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        public bool IsValidForAddPayment
        {
            get
            {
                if (DetailId == null)
                {
                    return false;
                }
                DolefilTripDetail dtdt = Session.GetObjectByKey<DolefilTripDetail>(this.DetailId.Oid);
                if (dtdt != null && dtdt.ReconId == null && !dtdt.Paid) {
                    return true;
                }
                return false;
            }
        }
        public bool IsValidForRemove
        {
            get
            {
                if (DetailId == null)
                {
                    return false;
                }
                DolefilTripDetail dtdt = Session.GetObjectByKey<DolefilTripDetail>(this.DetailId.Oid);
                if (dtdt != null && dtdt.ReconId == _HeaderId && dtdt.Paid)
                {
                    return true;
                }
                return false;
            }
        }
        //private int flagAddPayment = 0;
        [Action(AutoCommit = true, Caption = "Add Payment")]
        public void AddPayment()
        {
            DolefilTripDetail dtdt = Session.GetObjectByKey<DolefilTripDetail>(this.DetailId.Oid);
            if (dtdt != null)
            {
                dtdt.ReconId = _HeaderId;
            }
            AmountPaid = Amount;
        }

        [Action(AutoCommit = true, Caption = "Remove Payment")]
        public void RemovePayment()
        {
            DolefilTripDetail dtdt = Session.GetObjectByKey<DolefilTripDetail>(this.DetailId.Oid);
            if (dtdt != null)
            {
                dtdt.ReconId = null;
            }
            AmountPaid = 0m;
            DetailId.Paid = false;
            DetailId.PaidAmount = 0m;
        }

        // Amount Paid
        [Custom("DisplayFormat", "n")]
        public decimal AmountPaid
        {
            get { return _AmountPaid; }
            set
            {
                SetPropertyValue("AmountPaid", ref _AmountPaid, value);
                if (!IsSaving && !IsLoading && _HeaderId != null)
                {
                    _HeaderId.UpdateUnderpayment(true);
                    _HeaderId.UpdateOverpayment(true);
                    _HeaderId.UpdateUnpaid(true);
                    _HeaderId.UpdateTotal(true);
                }
            }
        }
        // Remarks
        // Adjustment
        [Custom("DisplayFormat", "n")]
        [DisplayName("OVR|UNDR")]
        [PersistentAlias("Amount - AmountPaid")]
        public decimal Adjustment
        {
            get
            {
                object tempObject = EvaluateAlias("Adjustment");
                if (tempObject != null) {
                    decimal ret = (decimal)tempObject;
                    if (_AmountPaid == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        if (ret > 0)
                        {
                            return 0-ret;
                        }
                        else
                        {
                            return Math.Abs(ret);
                        }
                        
                    }
                    //return (decimal)tempObject; 
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal Unpaid
        {
            get
            {
                if (_AmountPaid == 0)
                {
                    return Amount;
                }
                return 0;
            }
        }

        // Overpayment
        [Custom("DisplayFormat", "n")]
        [DisplayName("Over Payment")]
        [PersistentAlias("Amount - AmountPaid")]
        public decimal Overpaid
        {
            get
            {
                object tempObject = EvaluateAlias("Overpaid");
                if (tempObject != null)
                {
                    if (_AmountPaid != 0)
                    {
                        return (decimal)tempObject < 0 ? Math.Abs((decimal)tempObject) : 0;
                    }
                    else
                    {
                        return 0;
                    }

                }
                else
                {
                    return 0;
                }
            }
        }
        // Underpayment
        [Custom("DisplayFormat", "n")]
        [DisplayName("Under Payment")]
        [PersistentAlias("Amount - AmountPaid")]
        public decimal Underpaid
        {
            get
            {
                object tempObject = EvaluateAlias("Underpaid");
                if (tempObject != null)
                {
                    if (_AmountPaid != 0)
                    {
                        return (decimal)tempObject > 0 ? Math.Abs((decimal)tempObject) : 0;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        [Size(500)]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        #endregion

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

        public DtpReconDetail(Session session)
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

        protected override void OnDeleting()
        {
            //DetailId.ReconId = null;
            //DetailId.Paid = false;
            //DetailId.PaidAmount = 0m;
            if (_AmountPaid != 0)
            {
                throw new UserFriendlyException(string.Format("Cannot delete detail doc#{0} because it is paid.", DocumentNo));
            }
            //if (DetailId.ReconId != null && DetailId.ReconId == _HeaderId)
            //{
            //    DetailId.ReconId = null;
            //    DetailId.Paid = false;
            //    DetailId.PaidAmount = 0m;
            //}
            base.OnDeleting();
        }

        protected override void OnSaving()
        {
            if (!this.IsDeleted && _DetailId != null)
            {
                DetailId.Paid = _AmountPaid != 0;
                if (DetailId.Paid)
                {
                    DolefilTripDetail dtdt = Session.GetObjectByKey<DolefilTripDetail>(DetailId.Oid);
                    if (dtdt != null)
                    {
                        dtdt.ReconId = _HeaderId;
                    }
                }
                else
                {
                    DolefilTripDetail dtdt = Session.GetObjectByKey<DolefilTripDetail>(DetailId.Oid);
                    if (dtdt != null)
                    {
                        dtdt.ReconId = null;
                    }
                }
                DetailId.PaidAmount = _AmountPaid;
            }
            base.OnSaving();
        }
    }
}
