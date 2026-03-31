using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class DriverPayrollTripLine : XPObject {
        private DriverPayroll _DriverPayrollID;
        private DateTime _TripDate;
        private string _DocumentNo;
        private Employee _Driver;
        private decimal _Basic;
        private decimal _AdlMiscExp;
        private decimal _MiscExp;
        private decimal _KDs;
        private decimal _Shunting;
        private string _Destination;
        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll-Trips")]
        public DriverPayroll DriverPayrollID {
            get { return _DriverPayrollID; }
            set { SetPropertyValue("DriverPayrollID", ref _DriverPayrollID,
                value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime TripDate {
            get { return _TripDate; }
            set { SetPropertyValue("TripDate", ref _TripDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public string DocumentNo {
            get { return _DocumentNo; }
            set { SetPropertyValue("DocumentNo", ref _DocumentNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Basic {
            get { return _Basic; }
            set { SetPropertyValue("Basic", ref _Basic, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal AdlMiscExp {
            get { return _AdlMiscExp; }
            set { SetPropertyValue("AdlMiscExp", ref _AdlMiscExp, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal MiscExp {
            get { return _MiscExp; }
            set { SetPropertyValue("MiscExp", ref _MiscExp, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal KDs {
            get { return _KDs; }
            set { SetPropertyValue("KDs", ref _KDs, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Shunting {
            get { return _Shunting; }
            set { SetPropertyValue("Shunting", ref _Shunting, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Destination {
            get { return _Destination; }
            set { SetPropertyValue<string>("Destination", ref _Destination, value); }
        }

        public DriverPayrollTripLine(Session session)
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
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                if (SecuritySystem.CurrentUser != null)
                {
                    _CurrentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                }
                return _CurrentUser;
            }
        }

        #endregion

    }
}
