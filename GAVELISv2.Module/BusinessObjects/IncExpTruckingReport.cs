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
using System.ComponentModel;

namespace GAVELISv2.Module.BusinessObjects
{
    public struct vIncExpTruckingReportKey
    {
        // Payee
        [Persistent("Payee"), Browsable(false)]
        public Guid Payee_p { get; set; }
        // Expense
        [Persistent("Expense"), Browsable(false)]
        public decimal Expense_p { get; set; }
        // Income
        [Persistent("Income"), Browsable(false)]
        public decimal Income_p { get; set; }
        // Year
        [Persistent("Year"), Browsable(false)]
        public int Year_p { get; set; }
        // Month
        [Persistent("Month"), Browsable(false)]
        public int Month_p { get; set; }
    }

    [DefaultClassOptions]
    [Persistent("vIncExpTruckingReport"), OptimisticLocking(false)]
    public class IncExpTruckingReport : XPBaseObject
    {
        [Key, Persistent, Browsable(false)]
        public vIncExpTruckingReportKey Key { get; set; }

        public Contact Payee
        {
            get
            {
                Contact ret = null;
                if (Key.Payee_p != null)
                {
                    ret = Session.GetObjectByKey<Contact>(Key.Payee_p);
                }
                return ret ?? null;
            }
        }

        public decimal Expense { get { return Key.Expense_p; } }
        public decimal Income { get { return Key.Income_p; } }

        [Custom("DisplayFormat", "d")]
        public int Year { get { return Key.Year_p; } }
        public int Month { get { return Key.Month_p; } }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal January
        {
            get { return Month == 1 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal February
        {
            get { return Month == 2 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal March
        {
            get { return Month == 3 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal April
        {
            get { return Month == 4 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal May
        {
            get { return Month == 5 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal June
        {
            get { return Month == 6 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal July
        {
            get { return Month == 7 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal August
        {
            get { return Month == 8 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal September
        {
            get { return Month == 9 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal October
        {
            get { return Month == 10 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal November
        {
            get { return Month == 11 ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal December
        {
            get { return Month == 12 ? Expense + Income : 0; }
        }
        public IncExpTruckingReport(Session session)
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
