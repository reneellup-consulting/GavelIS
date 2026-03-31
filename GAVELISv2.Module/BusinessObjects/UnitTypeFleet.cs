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
    public class UnitTypeFleet : XPObject
    {
        private TruckUnitType _UnitType;
        private FATruck _Fleet;
        //[Association("UnitTypeFleets")]
        public TruckUnitType UnitType
        {
            get { return _UnitType; }
            set { SetPropertyValue("UnitType", ref _UnitType, value); }
        }
        public FATruck Fleet
        {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }
        public UnitTypeFleet(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }
    }

}
