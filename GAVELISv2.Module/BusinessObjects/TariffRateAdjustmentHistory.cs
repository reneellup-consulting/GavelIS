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
    public class TariffRateAdjustmentHistory : BaseObject
    {
        private Tariff _TariffID;
        private DateTime _EntryDate = DateTime.Now;
        private decimal _Distance;
        private decimal _TruckerRate;
        private decimal _FuelSubsidyRate;
        private decimal _TariffDistance;

        // TariffID
        [Association("Tariff-TariffRateAdjustmentHistory")]
        public Tariff TariffID
        {
            get { return _TariffID; }
            set { SetPropertyValue("TariffID", ref _TariffID, value); }
        }
        // EntryDate
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // Distance
        [Custom("DisplayFormat", "n")]
        [DisplayName("Billing Distance")]
        public decimal Distance
        {
            get { return _Distance; }
            set { SetPropertyValue("Distance", ref _Distance, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal TariffDistance
        {
            get { return _TariffDistance; }
            set { SetPropertyValue("TariffDistance", ref _TariffDistance, value); }
        }
        // TruckerRate
        [Custom("DisplayFormat", "n")]
        public decimal TruckerRate
        {
            get { return _TruckerRate; }
            set { SetPropertyValue("TruckerRate", ref _TruckerRate, value); }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Distance * TruckerRate")]
        [DisplayName("Billing Trucker Pay")]
        public decimal TruckerPay
        {
            get
            {
                var tempObject = EvaluateAlias("TruckerPay");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("TariffDistance * TruckerRate")]
        public decimal TariffTruckerPay
        {
            get
            {
                var tempObject = EvaluateAlias("TariffTruckerPay");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        // FuelSubsidyRate
        [Custom("DisplayFormat", "n")]
        public decimal FuelSubsidyRate
        {
            get { return _FuelSubsidyRate; }
            set { SetPropertyValue("FuelSubsidyRate", ref _FuelSubsidyRate, value); }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Distance * FuelSubsidyRate")]
        [DisplayName("Billing Fuel Subsidy")]
        public decimal FuelSubsidy
        {
            get
            {
                var tempObject = EvaluateAlias("FuelSubsidy");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("TariffDistance * FuelSubsidyRate")]
        public decimal TariffFuelSubsidy
        {
            get
            {
                var tempObject = EvaluateAlias("TariffFuelSubsidy");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }

        // IsActive
        [Custom("AllowEdit", "False")]
        public bool IsActive
        {
            get { return this == _TariffID.ActiveTariffRateAdjustment; }
        }

        [Action(AutoCommit = true, Caption = "Activate", ConfirmationMessage="Are you sure you want to activate the selected entry?")]
        public void ActivateAdjustment()
        {
            TariffID.ActiveTariffRateAdjustment = this;
            TariffID.Distance = _Distance;
            TariffID.TariffDistance = _TariffDistance;
            TariffID.TruckerPay = TruckerPay;
            TariffID.RateAdjmt = FuelSubsidy;
            TariffID.TariffTruckerPay = TariffTruckerPay;
            TariffID.TariffFuelSubsidy = TariffFuelSubsidy;
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
        public TariffRateAdjustmentHistory(Session session)
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
    }

}
