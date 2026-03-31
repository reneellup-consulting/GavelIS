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
    public class UnitTypeFuelAllocation : XPObject
    {
        private TruckUnitType _UnitType;
        private BusinessObjects.Tariff _Tariff;
        private decimal _DistOneWay;
        private decimal _DistRoundTrip;
        // UnitType
        //[Association("UnitTypeFuelAllocations")]
        public TruckUnitType UnitType
        {
            get { return _UnitType; }
            set { SetPropertyValue("UnitType", ref _UnitType, value); }
        }
        // Tariff
        public Tariff Tariff
        {
            get { return _Tariff; }
            set { SetPropertyValue("Tariff", ref _Tariff, value); }
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
        public UnitTypeFuelAllocation(Session session)
            : base(session)
        {
            
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
    }

}
