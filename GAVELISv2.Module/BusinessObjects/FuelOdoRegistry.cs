using System;
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
    public class FuelOdoRegistry : MeterRegisterBase
    {
        private FixedAsset _Fleet;
        [Association("FleetFuelOdoLogs")]
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
        private ReceiptFuel _ReceiptId;
        private FuelPumpRegister _FuelPumpRegId;
        [System.ComponentModel.DisplayName("Ref#")]
        [Association("ReceiptFuelOdoLogs")]
        [Custom("AllowEdit", "False")]
        public ReceiptFuel ReceiptId
        {
            get { return _ReceiptId; }
            set
            {
                SetPropertyValue("ReceiptId", ref _ReceiptId, value);
                if (!IsLoading)
                {
                    RegRefId = _ReceiptId != null ? _ReceiptId.Oid : 0;
                }
            }
        }
        [System.ComponentModel.DisplayName("FPR#")]
        [Association("FuelpumpOdoLogs")]
        [Custom("AllowEdit", "False")]
        public FuelPumpRegister FuelPumpRegId
        {
            get { return _FuelPumpRegId; }
            set
            {
                SetPropertyValue("FuelPumpRegId", ref _FuelPumpRegId, value);
                if (!IsLoading)
                {
                    RegRefId = _FuelPumpRegId != null ? _FuelPumpRegId.Oid : 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [DevExpress.Xpo.DisplayName("Reg. #")]
        [NonPersistent]
        public GenJournalHeader RegistryId
        {
            get {
                if (_ReceiptId != null)
                {
                    return _ReceiptId;
                }
                if (_FuelPumpRegId != null)
                {
                    return _FuelPumpRegId;
                }
                return null; }
            
        }
        public FuelOdoRegistry(Session session)
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
                base.BaseType = RegistryTypeEnum.Fuel;
            }
        }
        protected override void OnDeleting()
        {
            if (_ReceiptId != null)
            {
                throw new UserFriendlyException("Cannot delete a fuel receipt generated odo log");
            }
            base.OnDeleting();
        }
    }

}
