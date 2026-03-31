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
    public class TripsFuelExpenseDetail : XPObject
    {
        private Guid _RowID;
        private DateTime _EntryDate;
        private TripsIerPerFuelGroup _DetailID;
        private GenJournalHeader _SourceNo;
        private string _InvoiceNo;
        private Employee _Driver;
        private FixedAsset _UnitNo;
        private decimal _TotalQty;
        private decimal _Total;

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
        [Association("TripsFuelExpenseDetails")]
        public TripsIerPerFuelGroup DetailID
        {
            get { return _DetailID; }
            set
            {
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

        // SourceNo
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader SourceNo
        {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }

        // InvoiceNo
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string InvoiceNo
        {
            get { return _InvoiceNo; }
            set { SetPropertyValue("InvoiceNo", ref _InvoiceNo, value); }
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
        public FixedAsset UnitNo
        {
            get { return _UnitNo; }
            set { SetPropertyValue("UnitNo", ref _UnitNo, value); }
        }

        // Dtrs
        [Custom("AllowEdit", "False")]
        [Size(500)]
        [DisplayName("DTR's")]
        public string Dtrs
        {
            get { return _Dtrs; }
            set { SetPropertyValue("Dtrs", ref _Dtrs, value); }
        }

        // TotalQty
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal TotalQty
        {
            get { return _TotalQty; }
            set { SetPropertyValue("TotalQty", ref _TotalQty, value); }
        }

        // Total
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get { return _Total; }
            set
            {
                SetPropertyValue("Total", ref _Total, value);
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
        private string _Dtrs;

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

        public TripsFuelExpenseDetail(Session session)
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
