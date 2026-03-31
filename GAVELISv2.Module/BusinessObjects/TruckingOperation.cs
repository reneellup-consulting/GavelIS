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
    public class TruckingOperation : XPObject {
        private Guid _RowID;
        private FATruck _Fleet;
        private DateTime _FromDate;
        private DateTime _ToDate;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        public FATruck Fleet {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime FromDate {
            get { return _FromDate.Date; }
            set { SetPropertyValue("FromDate", ref _FromDate, value.Date); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ToDate {
            get { return _ToDate; }
            set { SetPropertyValue("ToDate", ref _ToDate, value.Date); }
        }

        #region Operation Type Details Collections
        [Aggregated,
Association("TruckingOperation-Details")]
        public XPCollection<TruckingOperationDetail> Operations
        {
            get
            {
                return GetCollection<TruckingOperationDetail>
                    ("Operations");
            }
        }

        // Trucking Trip Datails
        [Aggregated,
        Association("TruckingOperation-Trips")]
        public XPCollection<TOTrip> TOTrips { get { return GetCollection<TOTrip>
                ("TOTrips"); } }

        // Extract Trailer Income from trip datails
        [Aggregated,
        Association("TruckingOperation-Trailers")]
        public XPCollection<TOTrailer> TOTrailers { get { return GetCollection<
                TOTrailer>("TOTrailers"); } }

        // Get Shunting
        [Aggregated,
        Association("TruckingOperation-Shuntings")]
        public XPCollection<TOShunting> TOShuntings { get { return GetCollection
                <TOShunting>("TOShuntings"); } }

        // Get Genset
        [Aggregated,
        Association("TruckingOperation-Gensets")]
        public XPCollection<TOGenset> TOGensets { get { return GetCollection<
                TOGenset>("TOGensets"); } }

        // Get KD's
        [Aggregated,
        Association("TruckingOperation-KnockDowns")]
        public XPCollection<TOKnockDown> TOKnockDowns { get { return 
                GetCollection<TOKnockDown>("TOKnockDowns"); } }

        // Get Fuel
        [Aggregated,
        Association("TruckingOperation-Fuels")]
        public XPCollection<TOFuel> TOFuels { get { return GetCollection<TOFuel>
                ("TOFuels"); } }

        // Get Spare Parts used from Work Order Details Items Used
        [Aggregated,
        Association("TruckingOperation-SpareParts")]
        public XPCollection<TOSparePart> TOSpareParts { get { return 
                GetCollection<TOSparePart>("TOSpareParts"); } }

        // Get Job Orders
        [Aggregated,
        Association("TruckingOperation-JobOrders")]
        public XPCollection<TOJobOrder> TOJobOrders { get { return GetCollection
                <TOJobOrder>("TOJobOrders"); } }

        // Get Tire attachment expenses
        [Aggregated,
        Association("TruckingOperation-Tires")]
        public XPCollection<TOTire> TOTires { get { return GetCollection<TOTire>
                ("TOTires"); } }

        // Get Batter attachment expenses
        [Aggregated,
        Association("TruckingOperation-Batteries")]
        public XPCollection<TOBattery> TOBatterys { get { return GetCollection<
                TOBattery>("TOBatterys"); } }

        #endregion

        #region Details Calculations
        // Add Income
        // --> Income from trips
        [Persistent("IncomeFromTrips")]
        private decimal? _IncomeFromTrips;
        [PersistentAlias("_IncomeFromTrips")]
        [Custom("DisplayFormat", "n")]
        public decimal? IncomeFromTrips
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _IncomeFromTrips == null)
                    {
                        UpdateIncomeFromTrips(
                            false);
                    }
                }
                catch (Exception) { }
                return _IncomeFromTrips;
            }
        }
        public void UpdateIncomeFromTrips(bool forceChangeEvent)
        {
            decimal? oldTotal = _IncomeFromTrips;
            decimal tempTotal = 0m;
            foreach (TOTrip detail in TOTrips)
            {
                tempTotal +=
                    detail.Amount;
            }
            _IncomeFromTrips = tempTotal;
            if (forceChangeEvent) { OnChanged("IncomeFromTrips", IncomeFromTrips, _IncomeFromTrips); }
            ;
        }

        // --> Income from trailer
        [Persistent("IncomeFromTrailers")]
        private decimal? _IncomeFromTrailers;
        [PersistentAlias("_IncomeFromTrailers")]
        [Custom("DisplayFormat", "n")]
        public decimal? IncomeFromTrailers
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _IncomeFromTrailers == null)
                    {
                        UpdateIncomeFromTrailers(
                            false);
                    }
                }
                catch (Exception) { }
                return _IncomeFromTrailers;
            }
        }
        public void UpdateIncomeFromTrailers(bool forceChangeEvent)
        {
            decimal? oldTotal = _IncomeFromTrailers;
            decimal tempTotal = 0m;
            foreach (TOTrailer detail in TOTrailers)
            {
                tempTotal +=
                    detail.Amount;
            }
            _IncomeFromTrailers = tempTotal;
            if (forceChangeEvent) { OnChanged("IncomeFromTrailers", IncomeFromTrailers, _IncomeFromTrailers); }
            ;
        }

        // --> Income from shunting
        [Persistent("IncomeFromShuntings")]
        private decimal? _IncomeFromShuntings;
        [PersistentAlias("_IncomeFromShuntings")]
        [Custom("DisplayFormat", "n")]
        public decimal? IncomeFromShuntings
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _IncomeFromShuntings == null)
                    {
                        UpdateIncomeFromShuntings(
                            false);
                    }
                }
                catch (Exception) { }
                return _IncomeFromShuntings;
            }
        }
        public void UpdateIncomeFromShuntings(bool forceChangeEvent)
        {
            decimal? oldTotal = _IncomeFromShuntings;
            decimal tempTotal = 0m;
            foreach (TOShunting detail in TOShuntings)
            {
                tempTotal +=
                    detail.Amount;
            }
            _IncomeFromShuntings = tempTotal;
            if (forceChangeEvent) { OnChanged("IncomeFromShuntings", IncomeFromShuntings, _IncomeFromShuntings); }
            ;
        }

        // --> Income from Genset
        [Persistent("IncomeFromGensets")]
        private decimal? _IncomeFromGensets;
        [PersistentAlias("_IncomeFromGensets")]
        [Custom("DisplayFormat", "n")]
        public decimal? IncomeFromGensets
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _IncomeFromGensets == null)
                    {
                        UpdateIncomeFromGensets(
                            false);
                    }
                }
                catch (Exception) { }
                return _IncomeFromGensets;
            }
        }
        public void UpdateIncomeFromGensets(bool forceChangeEvent)
        {
            decimal? oldTotal = _IncomeFromGensets;
            decimal tempTotal = 0m;
            foreach (TOGenset detail in TOGensets)
            {
                tempTotal +=
                    detail.Amount;
            }
            _IncomeFromGensets = tempTotal;
            if (forceChangeEvent) { OnChanged("IncomeFromGensets", IncomeFromGensets, _IncomeFromGensets); }
            ;
        }

        // --> Income from KD's
        [Persistent("IncomeFromKnockDowns")]
        private decimal? _IncomeFromKnockDowns;
        [PersistentAlias("_IncomeFromKnockDowns")]
        [Custom("DisplayFormat", "n")]
        public decimal? IncomeFromKnockDowns
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _IncomeFromKnockDowns == null)
                    {
                        UpdateIncomeFromKnockDowns(
                            false);
                    }
                }
                catch (Exception) { }
                return _IncomeFromKnockDowns;
            }
        }
        public void UpdateIncomeFromKnockDowns(bool forceChangeEvent)
        {
            decimal? oldTotal = _IncomeFromKnockDowns;
            decimal tempTotal = 0m;
            foreach (TOKnockDown detail in TOKnockDowns)
            {
                tempTotal +=
                    detail.Amount;
            }
            _IncomeFromKnockDowns = tempTotal;
            if (forceChangeEvent) { OnChanged("IncomeFromKnockDowns", IncomeFromKnockDowns, _IncomeFromKnockDowns); }
            ;
        }

        [PersistentAlias("IncomeFromTrips + IncomeFromShuntings + IncomeFromTrailers + IncomeFromGensets + IncomeFromKnockDowns")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalIncome
        {
            get
            {
                object tempObject = EvaluateAlias("TotalIncome");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        // Less Expenses
        // --> Expenses from Fuel
        [Persistent("ExpensesFromFuel")]
        private decimal? _ExpensesFromFuel;
        [PersistentAlias("_ExpensesFromFuel")]
        [Custom("DisplayFormat", "n")]
        public decimal? ExpensesFromFuel
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ExpensesFromFuel == null)
                    {
                        UpdateExpensesFromFuel(
                            false);
                    }
                }
                catch (Exception) { }
                return _ExpensesFromFuel;
            }
        }
        public void UpdateExpensesFromFuel(bool forceChangeEvent)
        {
            decimal? oldTotal = _ExpensesFromFuel;
            decimal tempTotal = 0m;
            foreach (TOFuel detail in TOFuels)
            {
                tempTotal +=
                    detail.Amount;
            }
            _ExpensesFromFuel = tempTotal;
            if (forceChangeEvent) { OnChanged("ExpensesFromFuel", ExpensesFromFuel, _ExpensesFromFuel); }
            ;
        }

        // --> Expenses from Spareparts
        [Persistent("ExpensesFromSpareParts")]
        private decimal? _ExpensesFromSpareParts;
        [PersistentAlias("_ExpensesFromSpareParts")]
        [Custom("DisplayFormat", "n")]
        public decimal? ExpensesFromSpareParts
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ExpensesFromSpareParts == null)
                    {
                        UpdateExpensesFromSpareParts(
                            false);
                    }
                }
                catch (Exception) { }
                return _ExpensesFromSpareParts;
            }
        }
        public void UpdateExpensesFromSpareParts(bool forceChangeEvent)
        {
            decimal? oldTotal = _ExpensesFromSpareParts;
            decimal tempTotal = 0m;
            foreach (TOSparePart detail in TOSpareParts)
            {
                tempTotal +=
                    detail.Amount;
            }
            _ExpensesFromSpareParts = tempTotal;
            if (forceChangeEvent) { OnChanged("ExpensesFromSpareParts", ExpensesFromSpareParts, _ExpensesFromSpareParts); }
            ;
        }

        // --> Expenses from JOb Orders
        [Persistent("ExpensesFromJobOrder")]
        private decimal? _ExpensesFromJobOrder;
        [PersistentAlias("_ExpensesFromJobOrder")]
        [Custom("DisplayFormat", "n")]
        public decimal? ExpensesFromJobOrder
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ExpensesFromJobOrder == null)
                    {
                        UpdateExpensesFromJobOrder(
                            false);
                    }
                }
                catch (Exception) { }
                return _ExpensesFromJobOrder;
            }
        }
        public void UpdateExpensesFromJobOrder(bool forceChangeEvent)
        {
            decimal? oldTotal = _ExpensesFromJobOrder;
            decimal tempTotal = 0m;
            foreach (TOJobOrder detail in TOJobOrders)
            {
                tempTotal +=
                    detail.Amount;
            }
            _ExpensesFromJobOrder = tempTotal;
            if (forceChangeEvent) { OnChanged("ExpensesFromJobOrder", ExpensesFromJobOrder, _ExpensesFromJobOrder); }
            ;
        }

        // --> Expenses from Tire
        [Persistent("ExpensesFromTire")]
        private decimal? _ExpensesFromTire;
        [PersistentAlias("_ExpensesFromTire")]
        [Custom("DisplayFormat", "n")]
        public decimal? ExpensesFromTire
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ExpensesFromTire == null)
                    {
                        UpdateExpensesFromTire(
                            false);
                    }
                }
                catch (Exception) { }
                return _ExpensesFromTire;
            }
        }
        public void UpdateExpensesFromTire(bool forceChangeEvent)
        {
            decimal? oldTotal = _ExpensesFromTire;
            decimal tempTotal = 0m;
            foreach (TOTire detail in TOTires)
            {
                tempTotal +=
                    detail.Amount;
            }
            _ExpensesFromTire = tempTotal;
            if (forceChangeEvent) { OnChanged("ExpensesFromTire", ExpensesFromTire, _ExpensesFromTire); }
            ;
        }

        // --> Expenses from Battery
        [Persistent("ExpensesFromBattery")]
        private decimal? _ExpensesFromBattery;
        [PersistentAlias("_ExpensesFromBattery")]
        [Custom("DisplayFormat", "n")]
        public decimal? ExpensesFromBattery
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ExpensesFromBattery == null)
                    {
                        UpdateExpensesFromBattery(
                            false);
                    }
                }
                catch (Exception) { }
                return _ExpensesFromBattery;
            }
        }
        public void UpdateExpensesFromBattery(bool forceChangeEvent)
        {
            decimal? oldTotal = _ExpensesFromBattery;
            decimal tempTotal = 0m;
            foreach (TOBattery detail in TOBatterys)
            {
                tempTotal +=
                    detail.Amount;
            }
            _ExpensesFromBattery = tempTotal;
            if (forceChangeEvent) { OnChanged("ExpensesFromBattery", ExpensesFromBattery, _ExpensesFromBattery); }
            ;
        }

        [PersistentAlias("ExpensesFromFuel + ExpensesFromSpareParts + ExpensesFromJobOrder + ExpensesFromTire + ExpensesFromBattery")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalExpenses
        {
            get
            {
                object tempObject = EvaluateAlias("TotalExpenses");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [PersistentAlias("TotalIncome - TotalExpenses")]
        [Custom("DisplayFormat", "n")]
        public decimal IncomeOrLoss
        {
            get
            {
                object tempObject = EvaluateAlias("IncomeOrLoss");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        // Fuel Percent of Income
        [PersistentAlias("(ExpensesFromFuel /TotalIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal FuelPercentage
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("FuelPercentage");
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
        // Spare Parts Percent of Income
        [PersistentAlias("(ExpensesFromSpareParts / TotalIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal SparePartsPercentage
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("SparePartsPercentage");
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

        // Job Orders Percent of Income
        [PersistentAlias("(ExpensesFromJobOrder / TotalIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal JobOrderPercentage
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("JobOrderPercentage");
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

        // Tire Percent of Income
        [PersistentAlias("(ExpensesFromTire / TotalIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal TiresPercentage
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("TiresPercentage");
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

        // Battery Percent of Income
        [PersistentAlias("(ExpensesFromBattery / TotalIncome) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal BatteryPercentage
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("BatteryPercentage");
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


        #endregion

        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        public TruckingOperation(Session session): base(session) {
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
            RowID = Guid.NewGuid();

            #region Saving Creation
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion

        }

        protected override void OnSaving() {
            base.OnSaving();

            #region Saving Modified
            if (SecuritySystem.CurrentUser != null) {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }
            #endregion

        }

        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }
        private void Reset()
        {
            //_TotalTax = null;
            //_GrossTotal = null;
            _IncomeFromTrips = null;
            _IncomeFromTrailers = null;
            _IncomeFromShuntings = null;
            _IncomeFromGensets = null;
            _IncomeFromKnockDowns = null;
            _ExpensesFromFuel = null;
            _ExpensesFromSpareParts = null;
            _ExpensesFromJobOrder = null;
            _ExpensesFromTire = null;
            _ExpensesFromBattery = null;
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
