using System;
using System.Linq;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class IncomeExpenseReporter : XPObject
    {
        private int _Year;
        private decimal _TotalIncome;

        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }

        public static IncomeExpenseReporter GetInstance(Session session)
        {
            //Get the Singleton's instance if it exists
            IncomeExpenseReporter result = session.FindObject<IncomeExpenseReporter>(null);
            //Create the Singleton's instance
            if (result == null)
            {
                result = new IncomeExpenseReporter(session);
                result.Year = DateTime.Now.Year;
                result.Save();
            }
            return result;
        }

        [Aggregated, Association("IncomeExpenseBufferLines")]
        public XPCollection<IncomeExpenseBuffer> IncomeExpenseBufferLines
        {
            get
            {
                var collection = GetCollection<
                    IncomeExpenseBuffer>("IncomeExpenseBufferLines");
                collection.Criteria = CriteriaOperator.Parse("[Year]=?", _Year);
                return collection;
            }
        }
        [Aggregated, Association("CashflowSummaryLines")]
        public XPCollection<CashflowSummary> CashflowSummaryLines
        {
            get
            {
                var collection = GetCollection<
                    CashflowSummary>("CashflowSummaryLines");
                collection.Criteria = CriteriaOperator.Parse("[Year]=?", _Year);
                return collection;
            }
        }

        public decimal NetIncome {
            get
            {
                decimal tIncome = CashflowSummaryLines.Where(o => o.Year == _Year).Sum(o => o.NetIncome);
                return tIncome;
            }
        }

        public decimal PerAfterExpense
        {
            get
            {
                decimal netIncome = CashflowSummaryLines.Where(o => o.Year == _Year).Sum(o => o.NetIncome);
                decimal income = CashflowSummaryLines.Where(o => o.Year == _Year).Sum(o => o.Income);
                //decimal perafterx = (tIncome / CashflowSummaryLines.Where(o => o.Year == _Year).Sum(o => o.Income)) / 100;
                //return (netIncome/income) / 100;
                return (netIncome / income) * 100;
            }
        }

        //[Custom("AllowEdit", "False")]
        //public decimal TotalIncome
        //{
        //    get { return _TotalIncome; }
        //    set { SetPropertyValue("TotalIncome", ref _TotalIncome, value); }
        //}

        //public decimal TotalExpense
        //{
        //    get
        //    {
        //        decimal x = IncomeExpenseBufferLines.Where(o => o.Expense == true).Sum(o => o.Total);
        //        return x;
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
        public IncomeExpenseReporter(Session session)
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

        //Prevent the Singleton from being deleted 
        protected override void OnDeleting()
        {
            throw new UserFriendlyException(
            "The system prohibits the deletion of Income and Expense Reporter.");
        }
    }

}
