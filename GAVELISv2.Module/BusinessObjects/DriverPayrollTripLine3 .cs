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
    public class DriverPayrollTripLine3 : XPObject
    {
        private DriverPayroll3 _DriverPayrollID;
        private bool _Include;
        private Employee _Driver;
        private decimal _Commission;
        private decimal _NoOfTrips;
        private DolefilTrip _TripId;
        private string _OriginDestination;
        private DriverRegistry _DriverRegistryId;
        private HaulCategory _Category;
        private bool _Manual = false;
        private bool _Altered = false;
        private bool _Reprocessing;

        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll3-Trips")]
        public DriverPayroll3 DriverPayrollID
        {
            get { return _DriverPayrollID; }
            set
            {
                DriverPayroll3 oldDriverPayrollID = _DriverPayrollID;
                SetPropertyValue("DriverPayrollID", ref _DriverPayrollID,
                    value);
                if (!IsSaving && !IsLoading && _DriverPayrollID != null)
                {
                    Driver = _DriverPayrollID.Employee ?? null;
                }
                if (!IsLoading && !IsSaving && oldDriverPayrollID != _DriverPayrollID)
                {
                    oldDriverPayrollID = oldDriverPayrollID ?? _DriverPayrollID;
                    oldDriverPayrollID.UpdatePayValue(true);
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public DolefilTrip TripId
        {
            get { return _TripId; }
            set { SetPropertyValue("TripId", ref _TripId, value); }
        }

        [Custom("AllowEdit", "False")]
        public DriverRegistry DriverRegistryId
        {
            get { return _DriverRegistryId; }
            set { SetPropertyValue("DriverRegistryId", ref _DriverRegistryId, value); }
        }
        [Custom("AllowEdit", "False")]
        [NonPersistent]
        public bool Reprocessing
        {
            get { return _Reprocessing; }
            set { SetPropertyValue("Reprocessing", ref _Reprocessing, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Manual
        {
            get { return _Manual; }
            set { SetPropertyValue("Manual", ref _Manual, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Altered
        {
            get { return _Altered; }
            set { SetPropertyValue("Altered", ref _Altered, value); }
        }
        public bool Include
        {
            get { return _Include; }
            set { SetPropertyValue("Include", ref _Include, value); }
        }

        //[Custom("AllowEdit", "False")]
        public Employee Driver
        {
            get { return _Driver; }
            set
            {
                SetPropertyValue("Driver", ref _Driver, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public HaulCategory Category
        {
            get { return _Category; }
            set
            {
                SetPropertyValue("Category", ref _Category, value);
            }
        }
        //[Custom("AllowEdit", "False")]
        [DisplayName("Origin-Destination")]
        public string OriginDestination
        {
            get { return _OriginDestination; }
            set
            {
                SetPropertyValue("OriginDestination", ref _OriginDestination, value);
                if (!IsSaving && !IsLoading && Oid > 0 && !_Reprocessing)
                {
                    Altered = true;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal NoOfTrips
        {
            get { return _NoOfTrips; }
            set
            {
                SetPropertyValue("NoOfTrips", ref _NoOfTrips, value);
                if (!IsLoading && !IsSaving)
                {
                    _DriverPayrollID.UpdatePayValue(true);
                    if (Oid > 0 && !_Reprocessing)
                    {
                        Altered = true;
                    }
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Commission
        {
            get { return _Commission; }
            set
            {
                SetPropertyValue("Commission", ref _Commission, value);
                if (!IsLoading && !IsSaving)
                {
                    _DriverPayrollID.UpdatePayValue(true);
                    if (Oid > 0 && !_Reprocessing)
                    {
                        Altered = true;
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string CommissionString
        {
            get { return _Commission.ToString("n2"); }
        }

        [Custom("AllowEdit", "False")]
        public string NoOfTripsString
        {
            get { return _NoOfTrips.ToString("n2"); }
        }

        [PersistentAlias("Commission")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }
        [Action(AutoCommit = true, Caption = "Include")]
        public void MarkAsInclude()
        {
            Include = true;
        }

        [Action(AutoCommit = true, Caption = "Not Included")]
        public void UnMarkInclude()
        {
            Include = false;
        }
        
        [Association]
        public XPCollection<DolefilTripDetail> TripDetails
        {
            get
            {
                return GetCollection<DolefilTripDetail>(
                  "TripDetails");
            }
        }
        public DriverPayrollTripLine3(Session session)
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
            Include = true;
            Manual = true;
        }

        protected override void OnDeleting()
        {
            for (int i = TripDetails.Count - 1; i >= 0; i--)
            {
                TripDetails[i].Status = DriverRegistryStatusEnum.Current;
                if (TripDetails[i].DriverRegistryId != null)
                {
                    TripDetails[i].DriverRegistryId.PayrollBatchID = null;
                    if (TripDetails[i].DriverRegistryId.Approved)
                    {
                        TripDetails[i].DriverRegistryId.Status = DriverRegistryStatusEnum.Approved;
                    }
                    else
                    {
                        TripDetails[i].DriverRegistryId.Status = DriverRegistryStatusEnum.Current;
                    }
                    TripDetails[i].DriverRegistryId.DolePayroll = false;
                    TripDetails[i].DriverRegistryId = null;
                    //TripDetails[i].Save();
                }
                TripDetails.Remove(TripDetails[i]);
            }
            //_DriverRegistryId.Status = DriverRegistryStatusEnum.Current;
            //_DriverRegistryId.PayrollBatchID = null;
            base.OnDeleting();
        }
    }

}
