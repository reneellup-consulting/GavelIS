using System;
using System.Linq;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class TripOdoRegistry : MeterRegisterBase
    {
        private FixedAsset _Fleet;
        [Association("FleetTripOdoLogs")]
        [Custom("AllowEdit", "False")]
        public FixedAsset Fleet
        {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value);
            if (!IsLoading)
            {
                base.BaseFleet = _Fleet ?? null;
            }
            }
        }
        private TruckRegistry _TruckRegistryId;
        private decimal _TrlrMileage;
        [System.ComponentModel.DisplayName("Ref. #")]
        [Association("TruckRegistryOdoLogs")]
        [Custom("AllowEdit", "False")]
        public TruckRegistry TruckRegistryId
        {
            get { return _TruckRegistryId; }
            set
            {
                SetPropertyValue("TruckRegistryId", ref _TruckRegistryId, value);
                if (!IsLoading)
                {
                    RegRefId = _TruckRegistryId != null ? _TruckRegistryId.Oid : 0;
                }
                //if (!IsLoading && Trailer != null)
                //{
                //    decimal toDecimal = Convert.ToDecimal(Sequence);
                //    TripOdoRegistry data3 = null;
                //    decimal lstMileage = 0m;
                //    if (Trailer.TraileRegistryTripLogs != null)
                //    {
                //        data3 = Trailer.TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => o.Trailer == Trailer && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                //        lstMileage = data3 != null ? data3.TrlrMileage : 0m;
                //        TrlrMileage = lstMileage + Distance;
                //    }
                //    else
                //    {
                //        TrlrMileage = lstMileage + Distance;
                //    }
                //}
            }
        }
        [Action(Caption = "Generate TRLR Mileage", AutoCommit = true)]
        public void GenerateTRLRMileage()
        {
            #region New Trip Odo Logging
            if (Trailer != null)
            {
                decimal toDecimal = Convert.ToDecimal(Sequence);
                var data3 = Trailer.TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => o.Trailer == Trailer && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                decimal lstMileage = data3 != null ? data3.TrlrMileage : 0m;
                TrlrMileage = lstMileage + Distance;
            }
            #endregion
        }

        [NonPersistent]
        public FATrailer Trailer
        {
            get
            {
                if (TruckRegistryId != null && TruckRegistryId.AttachedTrailer != null)
                {
                    return TruckRegistryId.AttachedTrailer;
                }
                return null;
            }
        }
        [NonPersistent]
        public decimal Distance
        {
            get
            {
                if (TruckRegistryId != null)
                {
                    return TruckRegistryId.TariffDistance;
                }
                return 0m;
            }
        }
        [Custom("AllowEdit", "False")]
        [EditorAlias("MeterTypePropertyEditor")]
        [System.ComponentModel.DisplayName("Trailer Mileage")]
        public decimal TrlrMileage
        {
            get { return _TrlrMileage; }
            set { SetPropertyValue("TrlrMileage", ref _TrlrMileage, value); }
        }
        //[NonPersistent]
        //[EditorAlias("MeterTypePropertyEditor")]
        //public decimal TrlrMileage
        //{
        //    get
        //    {
        //        if (Trailer == null)
        //        {
        //            return 0m;
        //        }
        //        decimal toDecimal = Convert.ToDecimal(Sequence);
        //        var data3 = Trailer.TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => o.Trailer == Trailer && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
        //        decimal lstMileage = data3 != null ? data3.TrlrMileage : 0m;
        //        return lstMileage + Distance;
        //    }
        //}

        //private decimal GetLastMileage()
        //{
        //    if (Trailer == null)
        //    {
        //        return 0m;
        //    }
        //    decimal toDecimal = Convert.ToDecimal(Sequence);
        //    switch (BaseType)
        //    {
        //        case RegistryTypeEnum.Fuel:
        //            return 0m;
        //        case RegistryTypeEnum.Service:
        //            return 0m;
        //        case RegistryTypeEnum.Trip:
        //            var data3 = Trailer.TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => o.Trailer == Trailer && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
        //            return data3 != null ? data3.Mileage : 0m;
        //        default:
        //            return 0m;
        //    }
        //}
        //[NonPersistent]
        //[EditorAlias("MeterTypePropertyEditor")]
        //public decimal LastMileage
        //{
        //    get
        //    {
        //        if (Trailer == null)
        //        {
        //            return 0m;
        //        }
        //        decimal toDecimal = Convert.ToDecimal(Sequence);
        //        switch (BaseType)
        //        {
        //            case RegistryTypeEnum.Fuel:
        //                return 0m;
        //            case RegistryTypeEnum.Service:
        //                return 0m;
        //            case RegistryTypeEnum.Trip:
        //                var data3 = Trailer.TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => o.Trailer == Trailer && Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
        //                return data3 != null ? data3.Mileage : 0m;
        //            default:
        //                return 0m;
        //        }
        //    }
        //}
        [NonPersistent]
        public Tariff Tariff
        {
            get
            {
                if (TruckRegistryId != null)
                {
                    return TruckRegistryId.Tariff;
                }
                return null;
            }
        }
        public TripOdoRegistry(Session session)
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
            if (!IsLoading)
            {
                BaseType = RegistryTypeEnum.Trip;
            }
        }
        //protected override void OnDeleting()
        //{
        //    if (_TruckRegistryId != null && !_TruckRegistryId.IsDeleted)
        //    {
        //        throw new UserFriendlyException("Cannot delete a truck register generated odo log");
        //    }
        //    base.OnDeleting();
        //}
    }

}
