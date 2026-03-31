using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
    [OptimisticLocking(false)]
    public class TripsIerPerFuelGroup : XPObject
    {
        private Guid _RowID;
        private TripsIncomeExpenseReporter _ReporterID;
        private int _LineID;
        private GenJournalHeader _FuelReceipt;
        [Persistent("TripsIncome")]
        private decimal? _TripsIncome;
        [Persistent("FuelExpense")]
        private decimal? _FuelExpense;
        private decimal _TotalIncomeOrLoss;

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public int LineID
        {
            get { return _LineID; }
            set { SetPropertyValue("LineID", ref _LineID, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader FuelReceipt
        {
            get { return _FuelReceipt; }
            set { SetPropertyValue("FuelReceipt", ref _FuelReceipt, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TripsPerFuelGroups")]
        public TripsIncomeExpenseReporter ReporterID
        {
            get { return _ReporterID; }
            set { SetPropertyValue("ReporterID", ref _ReporterID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("_TripsIncome")]
        public decimal? TripsIncome
        {
            get {
                try
                {
                    if (!IsLoading && !IsSaving && _TripsIncome == null)
                    {
                        UpdateTripsIncome(
                        false);
                    }
                }
                catch (Exception)
                {
                }
                return _TripsIncome; }
        }

        public void UpdateTripsIncome(bool forceChangeEvent)
        {
            decimal? oldTotal = _TripsIncome;
            decimal tempTotal = 0m;
            foreach (TripsIncomeDetail detail in TripsIncomeDetails)
            {
                tempTotal
                += detail.NetBilling;
            }
            _TripsIncome = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TripsIncome", TripsIncome, _TripsIncome);
            }
            ;
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("_FuelExpense")]
        public decimal? FuelExpense
        {
            get {
                try
                {
                    if (!IsLoading && !IsSaving && _FuelExpense == null)
                    {
                        UpdateFuelExpense(
                        false);
                    }
                }
                catch (Exception)
                {
                }
                return _FuelExpense; }
        }

        public void UpdateFuelExpense(bool forceChangeEvent)
        {
            decimal? oldTotal = _FuelExpense;
            decimal tempTotal = 0m;
            foreach (TripsFuelExpenseDetail detail in TripsFuelExpenseDetails)
            {
                tempTotal
                += detail.Total;
            }
            _FuelExpense = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("FuelExpense", FuelExpense, _FuelExpense);
            }
            ;
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Total Income/Expense")]
        [PersistentAlias("TripsIncome - FuelExpense")]
        public decimal TotalIncomeOrLoss
        {
            get
            {
                object tempObject = EvaluateAlias("TotalIncomeOrLoss");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [Association("TripsFuelExpenseDetails"), Aggregated]
        public XPCollection<TripsFuelExpenseDetail> TripsFuelExpenseDetails
        {
            get { return GetCollection<TripsFuelExpenseDetail>("TripsFuelExpenseDetails"); }
        }

        [Association("TripsIncomeDetails"), Aggregated]
        public XPCollection<TripsIncomeDetail> TripsIncomeDetails
        {
            get { return GetCollection<TripsIncomeDetail>("TripsIncomeDetails"); }
        }

        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;

        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }
        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }
        #endregion

        public TripsIerPerFuelGroup(Session session)
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
            RowID = Guid.NewGuid();
            #region Saving Creation
            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }
            #endregion
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            #region Saving Modified
            if (SecuritySystem.CurrentUser != null)
            {
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
            _TripsIncome = null;
            _FuelExpense = null;
        }
    }
}
