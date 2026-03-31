using System;
using System.Linq;
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
    public class FATrailer : FixedAsset {
        private string _SeriesNo;
        private FAGeneratorSet _GensetNo;
        private bool _NotOwned;
        private Employee _Operator;
        
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public string SeriesNo {
            get { return _SeriesNo; }
            set { SetPropertyValue("SeriesNo", ref _SeriesNo, value); }
        }

        public FAGeneratorSet GensetNo {
            get { return _GensetNo; }
            set { SetPropertyValue("GensetNo", ref _GensetNo, value); }
        }

        public bool NotOwned {
            get { return _NotOwned; }
            set { SetPropertyValue("NotOwned", ref _NotOwned, value); }
        }

        public Employee Operator {
            get { return _Operator; }
            set { SetPropertyValue<Employee>("Operator", ref _Operator, value); }
        }
        // TraileRegistryTripLogs
        public System.Collections.Generic.IEnumerable<TripOdoRegistry> TraileRegistryTripLogs
        {
            get
            {
                XPCollection<TripOdoRegistry> trips = new XPCollection<TripOdoRegistry>(Session);
                //var collection = GetCollection<
                //    CheckInAndOut02>("ShiftCalculations");
                //collection.Criteria = CriteriaOperator.Parse("[Date] >= ? And [Date] < ?", _TimeRangeFrom.Date, _TimeRangeTo.AddDays(1).Date); // TODO: create criteria here
                //return collection;
                trips.Criteria = CriteriaOperator.Parse("[Trailer] = ?", this);
                if (trips != null && trips.Count() > 0)
                {
                    return trips;
                }
                else
                {
                    return null;
                }
            }
        }
        public FATrailer(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            FixedAssetClass = FixedAssetClassEnum.Trailer;
        }
    }
}
