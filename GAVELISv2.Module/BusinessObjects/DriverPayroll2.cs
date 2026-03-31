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
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "PayrollBatchID,Employee")]
    public class DriverPayroll2 : XPObject
    {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private string _DriverClass;
        private string _EmployeeName;
        private bool _Posted;
        private bool _Include;

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll2-Payroll")]
        public GenJournalHeader PayrollBatchID
        {
            get { return _PayrollBatchID; }
            set
            {
                GenJournalHeader oldPayrollBatchID = _PayrollBatchID;
                SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value)
                    ;
                if (!IsLoading && !IsSaving && oldPayrollBatchID != _PayrollBatchID)
                {
                    oldPayrollBatchID = oldPayrollBatchID ?? _PayrollBatchID;
                    ((DriverPayrollBatch2)oldPayrollBatchID).UpdateBatchTotal(true);
                }
            }
        }

        public bool Include
        {
            get { return _Include; }
            set { SetPropertyValue("Include", ref _Include, value); }
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
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        public Employee Employee
        {
            get { return _Employee; }
            set
            {
                SetPropertyValue("Employee", ref _Employee, value);
                if (!IsSaving && !IsLoading && _Employee != null && _Employee.DriverClassification != null)
                {
                    DriverClass = _Employee.DriverClassification.Code;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string DriverClass
        {
            get
            {
                if (string.IsNullOrEmpty(_DriverClass) && _Employee != null && _Employee.DriverClassification != null)
                {
                    return _Employee.DriverClassification.Code;
                }
                else
                {
                    return _DriverClass;
                }
            }
            set { SetPropertyValue("DriverClass", ref _DriverClass, value); }
        }

        [NonPersistent]
        public string EmployeeName
        {
            get
            {
                if (_Employee != null) { _EmployeeName = _Employee.Name; }
                return _EmployeeName;
            }
        }
        [Custom("AllowEdit", "False")]
        public bool Posted
        {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
        }
        #region Trips

        [Aggregated,
        Association("DriverPayroll2-Trips")]
        public XPCollection<DriverPayrollTripLine2> DriverPayrollTripLines
        {
            get
            {
                return GetCollection<DriverPayrollTripLine2>(
                  "DriverPayrollTripLines");
            }
        }

        #endregion
        #region Adjusments

        [Persistent("AdjustmentsAmt")]
        private decimal? _AdjustmentsAmt;
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("_AdjustmentsAmt")]
        public decimal? AdjustmentsAmt
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _AdjustmentsAmt == null)
                    {
                        UpdateAdjustmentsAmt(false);
                        //((DriverPayrollBatch2)_PayrollBatchID).UpdateBatchTotal(true);
                    }
                }
                catch (Exception)
                {
                }
                return _AdjustmentsAmt;
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string AdjustmentsStr
        {
            get
            {
                return _AdjustmentsAmt.Value.ToString("n2");
            }
        }

        public void UpdateAdjustmentsAmt(bool forceChangeEvent)
        {
            decimal? oldTotal = _AdjustmentsAmt;
            decimal tempTotal = 0m;
            foreach (DriverPayrollAdjustment detail in DriverPayrollAdjustments)
            {
                if (detail.Include)
                {
                    tempTotal += detail.Amount;
                }
            }
            _AdjustmentsAmt = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("AdjustmentsAmt", AdjustmentsAmt,
                    _AdjustmentsAmt);
            }
            ;
        }
        [NonPersistent]
        public int AdjustmentsCount
        {
            get
            {
                return DriverPayrollAdjustments.Count;
            }
        }

        [NonPersistent]
        public int DeductionsCount
        {
            get
            {
                return DriverPayrollDeductions.Count;
            }
        }
        [Aggregated,
        Association("DriverPayroll2-Adjustments")]
        public XPCollection<DriverPayrollAdjustment> DriverPayrollAdjustments
        {
            get
            {
                return GetCollection<DriverPayrollAdjustment>(
                    "DriverPayrollAdjustments");
            }
        }

        #endregion

        #region Deduction

        [Persistent("DeductionsAmt")]
        private decimal? _DeductionsAmt;
        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias("_DeductionsAmt")]
        public decimal? DeductionsAmt
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _DeductionsAmt == null)
                    {
                        UpdateDeductionsAmt(false);
                        //((DriverPayrollBatch2)_PayrollBatchID).UpdateBatchTotal(true);
                    }
                }
                catch (Exception)
                {
                }
                return _DeductionsAmt;
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string DeductionsStr
        {
            get
            {
                return _DeductionsAmt.Value.ToString("n2");
            }
        }

        public void UpdateDeductionsAmt(bool forceChangeEvent)
        {
            decimal? oldTotal = _DeductionsAmt;
            decimal tempTotal = 0m;
            foreach (DriverPayrollDeduction detail in DriverPayrollDeductions)
            {
                if (detail.Include)
                {
                    tempTotal += detail.Amount;
                }
            }
            _DeductionsAmt = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("DeductionsAmt", DeductionsAmt,
                    _DeductionsAmt);
            }
            ;
        }

        [Aggregated,
        Association("DriverPayroll2-Deductions")]
        public XPCollection<DriverPayrollDeduction> DriverPayrollDeductions
        {
            get
            {
                return GetCollection<DriverPayrollDeduction>(
                  "DriverPayrollDeductions");
            }
        }

        [Aggregated,
        Association("DriverPayroll2-DeductionsSummary")]
        public XPCollection<DriverDeductionDetails> DeductionsSummary
        {
            get
            {
                return GetCollection<DriverDeductionDetails>(
                  "DeductionsSummary");
            }
        }

        #endregion

        #region Pay Value

        [Persistent("PayValue")]
        private decimal? _PayValue;
        [DisplayName("Total")]
        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias("_PayValue")]
        public decimal? PayValue
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _PayValue == null)
                    {
                        UpdatePayValue(false);
                        //((DriverPayrollBatch2)_PayrollBatchID).UpdateBatchTotal(true);
                    }
                }
                catch (Exception)
                {
                }
                return _PayValue;
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string PayValueStr
        {
            get
            {
                return _PayValue.Value.ToString("n2");
            }
        }

        public void UpdatePayValue(bool forceChangeEvent)
        {
            decimal? oldTotal = _PayValue;
            decimal tempTotal = 0m;
            foreach (DriverPayrollTripLine2 detail in DriverPayrollTripLines)
            {
                if (detail.Include)
                {
                    tempTotal += detail.Total;
                }
            }
            _PayValue = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("PayValue", PayValue,
                    _PayValue);
            }
            ;
        }
        #endregion

        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias(
        "PayValue + AdjustmentsAmt"
        )]
        public decimal GrossPay
        {
            get
            {
                object tempObject = EvaluateAlias("GrossPay");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string GrossPayStr
        {
            get
            {
                return GrossPay.ToString("n2");
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias("GrossPay - DeductionsAmt")]
        public decimal NetPay
        {
            get
            {
                object tempObject = EvaluateAlias("NetPay");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string NetPayStr
        {
            get
            {
                return NetPay.ToString("n2");
            }
        }

        [NonPersistent]
        public DriverPayrollBatch2 BatchInfo
        {
            get
            {
                return (DriverPayrollBatch2)
                    _PayrollBatchID;
            }
        }

        //[Action(Caption = "Remove not Included", ConfirmationMessage = "Are you sure you want to do this?")]
        //public void RemoveNotIncluded()
        //{
        //    foreach (var item in this.DriverPayrollTripLines)
        //    {
        //        if (item.Include != true)
        //        {
        //            this.DriverPayrollTripLines.Remove(item);
        //        }
        //    }
        //}

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

        public DriverPayroll2(Session session)
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
            _AdjustmentsAmt = null;
            _DeductionsAmt = null;
            _PayValue = null;
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
