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
    public class TripsCargoDetailUnit : XPObject
    {
        private TripsCargoHeader _ParentID;
        private int _PineappleTrips;
        private FATruck _Unit;
        private decimal _PineappleIncome;
        private int _BananaTrips;
        private decimal _BananaIncome;
        private int _CementTrips;
        private decimal _CementIncome;
        private int _OtherTrips;
        private decimal _OtherIncome;
        [Custom("AllowEdit", "False")]
        [Association("TripsCargoDetailsUnit")]
        public TripsCargoHeader ParentID
        {
            get { return _ParentID; }
            set { SetPropertyValue("ParentID", ref _ParentID, value); }
        }

        public FATruck Unit
        {
            get { return _Unit; }
            set { SetPropertyValue("Unit", ref _Unit, value); }
        }
        [Custom("AllowEdit", "False")]
        public int PineappleTrips
        {
            get { return _PineappleTrips; }
            set
            {
                SetPropertyValue("PineappleTrips", ref _PineappleTrips, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public decimal PineappleIncome
        {
            get { return _PineappleIncome; }
            set
            {
                SetPropertyValue("PineappleIncome", ref _PineappleIncome, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int BananaTrips
        {
            get { return _BananaTrips; }
            set
            {
                SetPropertyValue("BananaTrips", ref _BananaTrips, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public decimal BananaIncome
        {
            get { return _BananaIncome; }
            set
            {
                SetPropertyValue("BananaIncome", ref _BananaIncome, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int CementTrips
        {
            get { return _CementTrips; }
            set
            {
                SetPropertyValue("CementTrips", ref _CementTrips, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public decimal CementIncome
        {
            get { return _CementIncome; }
            set
            {
                SetPropertyValue("CementIncome", ref _CementIncome, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public int OtherTrips
        {
            get { return _OtherTrips; }
            set
            {
                SetPropertyValue("OtherTrips", ref _OtherTrips, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public decimal OtherIncome
        {
            get { return _OtherIncome; }
            set
            {
                SetPropertyValue("OtherIncome", ref _OtherIncome, value);
            }
        }
        [PersistentAlias("PineappleTrips + BananaTrips + CementTrips + OtherTrips")]
        public int TotalTrips
        {
            get
            {
                object tempObject = EvaluateAlias("TotalTrips");
                if (tempObject != null)
                {
                    return (int)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }
        [PersistentAlias("PineappleIncome + BananaIncome + CementIncome + OtherIncome")]
        public decimal TotalIncome
        {
            get
            {
                object tempObject = EvaluateAlias("TotalIncome");
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
        public TripsCargoDetailUnit(Session session)
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
    }

}
