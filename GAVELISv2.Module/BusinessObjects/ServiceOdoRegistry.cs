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
    public class ServiceOdoRegistry : MeterRegisterBase
    {
        private FixedAsset _Fleet;
        [Association("FleetServiceOdoLogs")]
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
        private WorkOrder _WorkOrderId;
        [System.ComponentModel.DisplayName("Ref. #")]
        [Association("ServiceOdoLogs")]
        [Custom("AllowEdit", "False")]
        public WorkOrder WorkOrderId
        {
            get { return _WorkOrderId; }
            set
            {
                SetPropertyValue("WorkOrderId", ref _WorkOrderId, value);
                if (!IsLoading)
                {
                    RegRefId = _WorkOrderId != null ? _WorkOrderId.Oid : 0;
                }
            }
        }
        public ServiceOdoRegistry(Session session)
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
                BaseType = RegistryTypeEnum.Service;
            }
        }

        protected override void OnDeleting()
        {
            if (_WorkOrderId != null)
            {
                throw new UserFriendlyException("Cannot delete a work order generated odo log");
            }
            base.OnDeleting();
        }
    }

}
