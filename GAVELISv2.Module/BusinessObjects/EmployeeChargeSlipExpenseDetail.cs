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
    public class EmployeeChargeSlipExpenseDetail : XPObject, ISetIncomeExpense {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Account _Expense;
        private string _Description;
        private bool _IsPettyCash = false;
        private GenJournalDetail _PettyCashRef;
        private Item _ItemRef;
        private WorkOrder _WorkOrderRef;
        private CostCenter _CostCenter;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private decimal _Amount;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [Association("EmployeeChargeSlip-EmployeeChargeSlipExpenseDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue<GenJournalHeader>("GenJournalID", ref _GenJournalID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account Expense {
            get { return _Expense; }
            set
            {
                SetPropertyValue("Expense", ref _Expense, value);
                if (!IsLoading && _Expense != null)
                {
                    Description = _Expense.Name;
                    ExpenseType = _Expense.ExpenseType != null ? _Expense.
                    ExpenseType : null;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool IsPettyCash {
            get { return _IsPettyCash; }
            set { SetPropertyValue<bool>("IsPettyCash", ref _IsPettyCash, value); }
        }

        [Custom("AllowEdit", "False")]
        public GenJournalDetail PettyCashRef {
            get { return _PettyCashRef; }
            set { SetPropertyValue<GenJournalDetail>("PettyCashRef", ref _PettyCashRef, value); }
        }

        public Item ItemRef {
            get { return _ItemRef; }
            set { SetPropertyValue<Item>("ItemRef", ref _ItemRef, value); }
        }
        [Custom("AllowEdit", "False")]
        public WorkOrder WorkOrderRef {
            get { return _WorkOrderRef; }
            set { SetPropertyValue<WorkOrder>("WorkOrderRef", ref _WorkOrderRef, value); }
        }
        private JobOrder _JobOrderRef;
        [Custom("AllowEdit", "False")]
        public JobOrder JobOrderRef
        {
            get { return _JobOrderRef; }
            set { SetPropertyValue("JobOrderRef", ref _JobOrderRef, value); }
        }
        
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value); }
        }

        public ExpenseType ExpenseType {
            get { return _ExpenseType; }
            set { SetPropertyValue<ExpenseType>("ExpenseType", ref _ExpenseType, value); }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType {
            get { return _SubExpenseType; }
            set { SetPropertyValue<SubExpenseType>("SubExpenseType", ref _SubExpenseType, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set
            {
                SetPropertyValue<decimal>("Amount", ref _Amount, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((EmployeeChargeSlip)_GenJournalID).UpdateTotalChargeOfExpense(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

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

        public EmployeeChargeSlipExpenseDetail(Session session)
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
        private int deletedOid;
        protected override void OnDeleting()
        {
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
            }
            base.OnDeleting();
        }
        protected override void OnSaving() {
            if (IsDeleted)
            {
                IncomeAndExpense02 incExp = null;
                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = {1}", deletedOid, this.Oid)));
                if (incExp != null)
                {
                    incExp.Delete();
                }
            }
            else
            {
                if (this.GenJournalID != null)
                {
                    this.GenJournalID.IsIncExpNeedUpdate = true;
                    this.GenJournalID.Save();
                }
            }
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
