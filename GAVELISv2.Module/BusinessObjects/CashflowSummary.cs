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
    public class CashflowSummary : XPObject
    {
        private IncomeExpenseReporter _ReporterId;
        private int _Year;
        private MonthsEnum _Month;
        private string _BufferId;
        private decimal _Income;
        private decimal _Expense;
        private decimal _NetIncome;
        [Custom("AllowEdit", "False")]
        [Association("CashflowSummaryLines")]
        public IncomeExpenseReporter ReporterId
        {
            get { return _ReporterId; }
            set { SetPropertyValue("ReporterId", ref _ReporterId, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        [Custom("AllowEdit", "False")]
        public MonthsEnum Month
        {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        [Custom("AllowEdit", "False")]
        public string BufferId
        {
            get { return _BufferId; }
            set { SetPropertyValue("BufferId", ref _BufferId, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public decimal Income
        {
            get { return _Income; }
            set { SetPropertyValue("Income", ref _Income, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal Expense
        {
            get { return _Expense; }
            set { SetPropertyValue("Expense", ref _Expense, value); }
        }
        [PersistentAlias("Income - Expense")]
        [Custom("AllowEdit", "False")]
        public decimal NetIncome
        {
            get
            {
                object tempObject = EvaluateAlias("NetIncome");
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
        
        public CashflowSummary(Session session)
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
