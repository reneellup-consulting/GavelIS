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
    public class DriverPayrollTripLine2 : XPObject
    {
        private DriverPayroll2 _DriverPayrollID;
        private DriverRegistry _DriverRegistryId;
        private bool _Include;
        private DateTime _TripDate;
        private string _DocumentNo;
        private Employee _Driver;
        private decimal _Commission;
        private decimal _KDs;
        private decimal _Shunting;
        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll2-Trips")]
        public DriverPayroll2 DriverPayrollID
        {
            get { return _DriverPayrollID; }
            set
            {
                DriverPayroll2 oldDriverPayrollID = _DriverPayrollID;
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
        public DriverRegistry DriverRegistryId
        {
            get { return _DriverRegistryId; }
            set { SetPropertyValue("DriverRegistryId", ref _DriverRegistryId, value); }
        }
        public bool Include
        {
            get { return _Include; }
            set { SetPropertyValue("Include", ref _Include, value); }
        }
        //[Custom("AllowEdit", "False")]
        public DateTime TripDate
        {
            get { return _TripDate; }
            set { SetPropertyValue("TripDate", ref _TripDate, value); }
        }
        [Custom("AllowEdit", "False")]
        public string DateString
        {
            get { return _TripDate.ToShortDateString(); }
        }
        //[Custom("AllowEdit", "False")]
        public string DocumentNo
        {
            get { return _DocumentNo; }
            set { SetPropertyValue("DocumentNo", ref _DocumentNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public Employee Driver
        {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
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
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string CommissionString
        {
            get { return _Commission.ToString("n2"); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal KDs
        {
            get { return _KDs; }
            set
            {
                SetPropertyValue("KDs", ref _KDs, value);
                if (!IsLoading && !IsSaving)
                {
                    _DriverPayrollID.UpdatePayValue(true);
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string KDsString
        {
            get { return _KDs.ToString("n2"); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Shunting
        {
            get { return _Shunting; }
            set
            {
                SetPropertyValue("Shunting", ref _Shunting, value);
                if (!IsLoading && !IsSaving)
                {
                    _DriverPayrollID.UpdatePayValue(true);
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string ShuntingString
        {
            get { return _Shunting.ToString("n2"); }
        }

        [PersistentAlias("Commission + KDs + Shunting")]
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
        [Action(AutoCommit=true, Caption="Include")]
        public void MarkAsInclude()
        {
            Include = true;
        }

        [Action(AutoCommit = true, Caption = "Not Included")]
        public void UnMarkInclude()
        {
            Include = false;
        }

        public DriverPayrollTripLine2(Session session)
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
        }

        protected override void OnDeleting()
        {
            _DriverRegistryId.Status = DriverRegistryStatusEnum.Current;
            _DriverRegistryId.PayrollBatchID = null;
        }
    }

}
