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
    public class TruckingOperation2Detail : XPObject {
        private TruckingOperation2 _TOID;
        [Custom("AllowEdit", "False")]
        [Association("TruckingOperation2-Details")]
        public TruckingOperation2 TOID {
            get { return _TOID; }
            set { SetPropertyValue("TOID", ref _TOID, value); }
        }

        #region Details
        private int _Seq;
        private string _Month;
        private decimal _Stanfilco;
        private decimal _Dole;
        private decimal _Others;
        //private decimal? _TotalTruckIncome;
        private decimal _IHParts;
        private decimal _TireBattery;
        private decimal _PercentTireBattery;
        private decimal _Fuel;
        private decimal _PercentFuel;
        private decimal _JobOrders;
        private decimal _PercentJobOrders;
        private decimal _DriversAllowance;
        private decimal _PercentAllowance;
        private decimal _PartsServices;
        private decimal _PercentParts;
        //private decimal? _IncomeLoss;
        [Custom("AllowEdit", "False")]
        public int Seq {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Month {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Stanfilco {
            get { return _Stanfilco; }
            set { SetPropertyValue("Stanfilco", ref _Stanfilco, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Dole {
            get { return _Dole; }
            set { SetPropertyValue("Dole", ref _Dole, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Others {
            get { return _Others; }
            set { SetPropertyValue("Others", ref _Others, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal IHParts {
            get { return _IHParts; }
            set { SetPropertyValue("IHParts", ref _IHParts, value); }
        }
        [PersistentAlias("Stanfilco + Dole + Others + IHParts")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalTruckIncome
        {
            get
            {
                object tempObject = EvaluateAlias("TotalTruckIncome");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TireBattery {
            get { return _TireBattery; }
            set { SetPropertyValue("TireBattery", ref _TireBattery, value); }
        }
        [PersistentAlias("(TireBattery /TotalTruckIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal PercentTireBattery {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("PercentTireBattery");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Fuel {
            get { return _Fuel; }
            set { SetPropertyValue("Fuel", ref _Fuel, value); }
        }
        [PersistentAlias("(Fuel /TotalTruckIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal PercentFuel {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("PercentFuel");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal JobOrders {
            get { return _JobOrders; }
            set { SetPropertyValue("JobOrders", ref _JobOrders, value); }
        }
        [PersistentAlias("(JobOrders /TotalTruckIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal PercentJobOrders {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("PercentJobOrders");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal DriversAllowance {
            get { return _DriversAllowance; }
            set { SetPropertyValue("DriversAllowance", ref _DriversAllowance, 
                value); }
        }
        [PersistentAlias("(DriversAllowance /TotalTruckIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal PercentAllowance {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("PercentAllowance");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal PartsServices {
            get { return _PartsServices; }
            set { SetPropertyValue("PartsServices", ref _PartsServices, value); 
            }
        }
        [PersistentAlias("(PartsServices /TotalTruckIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal PercentParts {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("PercentParts");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        [PersistentAlias("TotalTruckIncome - Fuel - TireBattery - JobOrders - DriversAllowance - PartsServices")]
        [Custom("DisplayFormat", "n")]
        public decimal IncomeLoss {
            get
            {
                object tempObject = EvaluateAlias("IncomeLoss");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        public TruckingOperation2Detail(Session session): base(session) {
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
