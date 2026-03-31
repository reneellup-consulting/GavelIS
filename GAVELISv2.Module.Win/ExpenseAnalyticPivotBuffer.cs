using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win
{
    //[DefaultClassOptions]
    public class ExpenseAnalyticPivotBuffer : XPObject
    {
        private ExpenseType _Category;
        private SubExpenseType _SubCategory;
        private int _Year;
        private MonthsEnum _Month;
        private PaymentTypeEnum _PaymentType;
        private decimal _Amount;
        private string _BufferId;
        // BufferId
        public string BufferId
        {
            get { return _BufferId; }
            set { SetPropertyValue("BufferId", ref _BufferId, value); }
        }
        // Category
        public ExpenseType Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }
        // SubCategory
        public SubExpenseType SubCategory
        {
            get { return _SubCategory; }
            set { SetPropertyValue("SubCategory", ref _SubCategory, value); }
        }
        // Year
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        // Month
        public MonthsEnum Month
        {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        // PaymentMode
        public PaymentTypeEnum PaymentType
        {
            get { return _PaymentType; }
            set { SetPropertyValue("PaymentType", ref _PaymentType, value); }
        }
        // Amount
        public decimal Amount
        {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
        }
        public ExpenseAnalyticPivotBuffer(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
    }

}
