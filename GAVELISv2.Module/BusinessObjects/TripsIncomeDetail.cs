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
    public class TripsIncomeDetail : XPObject
    {
        private Guid _RowID;
        private DateTime _EntryDate;
        private TripsIerPerFuelGroup _DetailID;
        private string _LineNo;
        private GenJournalHeader _SourceNo;
        private string _DocumentNo;
        private Employee _Driver;
        private FATruck _UnitNo;
        private TripLocation _Origin;
        private TripLocation _Destination;
        private decimal _TruckerPay;
        private decimal _NetBilling;

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TripsIncomeDetails")]
        public TripsIerPerFuelGroup DetailID
        {
            get { return _DetailID; }
            set {
                TripsIerPerFuelGroup oldDetailID = _DetailID;
                SetPropertyValue("DetailID", ref _DetailID, value);
                if (!IsLoading && !IsSaving && oldDetailID != _DetailID)
                {
                    oldDetailID = oldDetailID ?? _DetailID;
                    oldDetailID.UpdateTripsIncome(true);
                    oldDetailID.UpdateFuelExpense(true);
                }
            }
        }

        //[Custom("AllowEdit", "False")]
        //[RuleRequiredField("", DefaultContexts.Save)]
        //public string LineNo
        //{
        //    get { return _LineNo; }
        //    set { SetPropertyValue("LineNo", ref _LineNo, value); }
        //}

        // SourceNo
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader SourceNo
        {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }
        // DocumentNo
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string DocumentNo
        {
            get { return _DocumentNo; }
            set { SetPropertyValue("DocumentNo", ref _DocumentNo, value); }
        }
        // Driver
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver
        {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        // UnitNo
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public FATruck UnitNo
        {
            get { return _UnitNo; }
            set { SetPropertyValue("UnitNo", ref _UnitNo, value); }
        }
        // Origin
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public TripLocation Origin
        {
            get { return _Origin; }
            set { SetPropertyValue("Origin", ref _Origin, value); }
        }
        // Destination
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public TripLocation Destination
        {
            get { return _Destination; }
            set { SetPropertyValue("Destination", ref _Destination, value); }
        }
        // TruckerPay
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TruckerPay
        {
            get { return _TruckerPay; }
            set
            {
                SetPropertyValue("TruckerPay", ref _TruckerPay, value);
                if (!IsSaving && !IsLoading)
                {
                    _DetailID.UpdateTripsIncome(true);
                    _DetailID.UpdateFuelExpense(true);
                }
            }
        }
        // NetBilling
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal NetBilling
        {
            get { return _NetBilling; }
            set
            {
                SetPropertyValue("NetBilling", ref _NetBilling, value);
                if (!IsSaving && !IsLoading)
                {
                    _DetailID.UpdateTripsIncome(true);
                    _DetailID.UpdateFuelExpense(true);
                }
            }
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

        public TripsIncomeDetail(Session session)
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
    }

}
