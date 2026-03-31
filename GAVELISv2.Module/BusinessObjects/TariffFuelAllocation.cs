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
    //[NavigationItem(false)]
    [System.ComponentModel.DefaultProperty("UnitType")]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save,
    "TariffID, UnitType")]
    public class TariffFuelAllocation : BaseObject {
        private Tariff _TariffID;
        private TruckUnitType _UnitType;
        private decimal _DistOneWay;
        private decimal _DistRoundTrip;
        private decimal _FuelAllocation;

        [Association("Tariff-TariffFuelAllocations")]
        public Tariff TariffID {
            get { return _TariffID; }
            set { SetPropertyValue<Tariff>("TariffID", ref _TariffID, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("UnitTypeFuelAllocations")]
        public TruckUnitType UnitType {
            get { return _UnitType; }
            set { SetPropertyValue<TruckUnitType>("UnitType", ref _UnitType, value); }
        }
        // DistOneWay
        public decimal DistOneWay
        {
            get { return _DistOneWay; }
            set { SetPropertyValue("DistOneWay", ref _DistOneWay, value); }
        }
        // DistRoundTrip
        public decimal DistRoundTrip
        {
            get { return _DistRoundTrip; }
            set { SetPropertyValue("DistRoundTrip", ref _DistRoundTrip, value); }
        }
        public decimal LtrsPerKm
        {
            get { return _LtrsPerKm; }
            set { SetPropertyValue("LtrsPerKm", ref _LtrsPerKm, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal FuelAllocation {
            get { return _FuelAllocation; }
            set { SetPropertyValue<decimal>("FuelAllocation", ref _FuelAllocation, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool UpdateOnline
        {
            get { return _UpdateOnline; }
            set { SetPropertyValue("UpdateOnline", ref _UpdateOnline, value); }
        }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private decimal _LtrsPerKm;
        private bool _UpdateOnline;
        private bool _FmsUpdate;
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

        #region Hyper FMS

        // FmsUpdate
        public bool FmsUpdate
        {
            get { return _FmsUpdate; }
            set { SetPropertyValue("FmsUpdate", ref _FmsUpdate, value); }
        }

        #endregion

        public TariffFuelAllocation(Session session)
            : base(session) {
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
            UpdateOnline = true;
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
