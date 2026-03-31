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

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "HeaderId, LineId")]
    public class IncExpenseReclassDetail : XPObject, IWithLineNumber
    {
        private Guid _RowID;
        private IncExpenseReclass _HeaderId;
        private DateTime _EntryDate;
        private IncomeAndExpense02 _LineId;
        private Contact _Payee;
        private string _Description1;
        private string _Description2;
        private decimal _Income;
        private decimal _Expense;
        private ExpenseType _Category;
        private SubExpenseType _SubCategory;
        private ExpenseType _ToCategory;
        private SubExpenseType _ToSubCategory;
        private bool _Done = false;

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("IncExpenseReclass-Details")]
        [Custom("AllowEdit", "False")]
        public IncExpenseReclass HeaderId
        {
            get { return _HeaderId; }
            set { SetPropertyValue("HeaderId", ref _HeaderId, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        [Custom("AllowEdit", "False")]
        public IncomeAndExpense02 LineId
        {
            get { return _LineId; }
            set { SetPropertyValue("LineId", ref _LineId, value); }
        }

        [Custom("AllowEdit", "False")]
        public Contact Payee
        {
            get { return _Payee; }
            set { SetPropertyValue("Payee", ref _Payee, value); }
        }

        [Custom("AllowEdit", "False")]
        [Size(1000)]
        public string Description1
        {
            get { return _Description1; }
            set { SetPropertyValue("Description1", ref _Description1, value); }
        }

        [Custom("AllowEdit", "False")]
        [Size(1000)]
        public string Description2
        {
            get { return _Description2; }
            set { SetPropertyValue("Description2", ref _Description2, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Income
        {
            get { return _Income; }
            set { SetPropertyValue("Income", ref _Income, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense
        {
            get { return _Expense; }
            set { SetPropertyValue("Expense", ref _Expense, value); }
        }

        [Custom("AllowEdit", "False")]
        public ExpenseType Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }

        [Custom("AllowEdit", "False")]
        public SubExpenseType SubCategory
        {
            get { return _SubCategory; }
            set { SetPropertyValue("SubCategory", ref _SubCategory, value); }
        }

        [Custom("AllowEdit", "False")]
        public ExpenseType ToCategory
        {
            get { return _ToCategory; }
            set { SetPropertyValue("ToCategory", ref _ToCategory, value); }
        }

        [Custom("AllowEdit", "False")]
        [DataSourceProperty("ToCategory.SubExpenseTypes")]
        public SubExpenseType ToSubCategory
        {
            get { return _ToSubCategory; }
            set { SetPropertyValue("ToSubCategory", ref _ToSubCategory, value); }
        }

        [Custom("AllowEdit", "False")]
        public bool Done
        {
            get { return _Done; }
            set { SetPropertyValue("Done", ref _Done, value); }
        }

        public bool IsOpen
        {
            get { return _HeaderId.Status == IncExpReclassStateEnum.Current; }
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

        public IncExpenseReclassDetail(Session session)
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
