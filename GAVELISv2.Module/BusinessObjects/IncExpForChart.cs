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
    public struct vIncExpForChartKey
    {
        [Persistent("Oid"), Browsable(false)]
        public Guid Oid_p { get; set; }
        [Persistent("EntryDate"), Browsable(false)]
        public DateTime EntryDate_p { get; set; }
        [Persistent("SourceID"), Browsable(false)]
        [Custom("DisplayFormat", "d")]
        public int SourceID_p { get; set; }
        [Persistent("SourceType"), Browsable(false)]
        public Guid SourceType_p { get; set; }
        [Persistent("SourceNo"), Browsable(false)]
        public string SourceNo_p { get; set; }
        [Persistent("RefID"), Browsable(false)]
        public string RefID_p { get; set; }
        [Persistent("Payee"), Browsable(false)]
        public Guid Payee_p { get; set; }
        [Persistent("Category"), Browsable(false)]
        public Guid Category_p { get; set; }
        [Persistent("SubCategory"), Browsable(false)]
        public Guid SubCategory_p { get; set; }
        [Persistent("CostCenter"), Browsable(false)]
        public Guid CostCenter_p { get; set; }
        [Persistent("Expense"), Browsable(false)]
        public decimal Expense_p { get; set; }
        [Persistent("Income"), Browsable(false)]
        public decimal Income_p { get; set; }
        [Persistent("Fleet"), Browsable(false)]
        public Guid Fleet_p { get; set; }
        [Persistent("Facility"), Browsable(false)]
        public Guid Facility_p { get; set; }
        [Persistent("Department"), Browsable(false)]
        public Guid Department_p { get; set; }
        [Persistent("FacilityHead"), Browsable(false)]
        public Guid FacilityHead_p { get; set; }
        [Persistent("DepartmentInCharge"), Browsable(false)]
        public Guid DepartmentInCharge_p { get; set; } 
    }
    [DefaultClassOptions]
    [Persistent("vIncExpForChart"), OptimisticLocking(false)]
    public class IncExpForChart : XPBaseObject
    {
        [Key, Persistent, Browsable(false)]
        public vIncExpForTruckingKey Key { get; set; }

        public Guid Oid { get { return Key.Oid_p; } }
        public DateTime EntryDate { get { return Key.EntryDate_p; } }
        [Custom("DisplayFormat", "d")]
        public int SourceID { get { return Key.SourceID_p; } }
        public SourceType SourceType
        {
            get
            {
                SourceType ret = null;
                if (Key.SourceType_p != null)
                {
                    ret = Session.GetObjectByKey<SourceType>(Key.SourceType_p);
                }
                return ret ?? null;
            }
        }
        public string SourceNo { get { return Key.SourceNo_p; } }
        public string RefID { get { return Key.RefID_p; } }
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
        public ExpenseType Category
        {
            get
            {
                ExpenseType ret = null;
                if (Key.Category_p != null)
                {
                    ret = Session.GetObjectByKey<ExpenseType>(Key.Category_p);
                }
                return ret ?? null;
            }
        }
        public ExpenseType SubCategory
        {
            get
            {
                ExpenseType ret = null;
                if (Key.SubCategory_p != null)
                {
                    ret = Session.GetObjectByKey<ExpenseType>(Key.SubCategory_p);
                }
                return ret ?? null;
            }
        }
        public CostCenter CostCenter
        {
            get
            {
                CostCenter ret = null;
                if (Key.CostCenter_p != null)
                {
                    ret = Session.GetObjectByKey<CostCenter>(Key.CostCenter_p);
                }
                return ret ?? null;
            }
        }
        public decimal Expense { get { return Key.Expense_p; } }
        public decimal Income { get { return Key.Income_p; } }
        public FixedAsset Fleet
        {
            get
            {
                FixedAsset ret = null;
                if (Key.Fleet_p != null)
                {
                    ret = Session.GetObjectByKey<FixedAsset>(Key.Fleet_p);
                }
                return ret ?? null;
            }
        }
        public Facility Facility
        {
            get
            {
                Facility ret = null;
                if (Key.Facility_p != null)
                {
                    ret = Session.GetObjectByKey<Facility>(Key.Facility_p);
                }
                return ret ?? null;
            }
        }
        public Department Department
        {
            get
            {
                Department ret = null;
                if (Key.Department_p != null)
                {
                    ret = Session.GetObjectByKey<Department>(Key.Department_p);
                }
                return ret ?? null;
            }
        }
        public Employee FacilityHead
        {
            get
            {
                Employee ret = null;
                if (Key.FacilityHead_p != null)
                {
                    ret = Session.GetObjectByKey<Employee>(Key.FacilityHead_p);
                }
                return ret ?? null;
            }
        }
        public Employee DepartmentInCharge
        {
            get
            {
                Employee ret = null;
                if (Key.DepartmentInCharge_p != null)
                {
                    ret = Session.GetObjectByKey<Employee>(Key.DepartmentInCharge_p);
                }
                return ret ?? null;
            }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum GMonth
        {
            get
            {
                _Month = EntryDate.Month == 1 ? MonthsEnum.January : EntryDate.Month
                 == 2 ? MonthsEnum.February : EntryDate.Month == 3 ? MonthsEnum.
                March : EntryDate.Month == 4 ? MonthsEnum.April : EntryDate.Month ==
                5 ? MonthsEnum.May : EntryDate.Month == 6 ? MonthsEnum.June :
                EntryDate.Month == 7 ? MonthsEnum.July : EntryDate.Month == 8 ?
                MonthsEnum.August : EntryDate.Month == 9 ? MonthsEnum.September
                 : EntryDate.Month == 10 ? MonthsEnum.October : EntryDate.Month == 11
                 ? MonthsEnum.November : EntryDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal January
        {
            get { return GMonth == MonthsEnum.January ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal February
        {
            get { return GMonth == MonthsEnum.February ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal March
        {
            get { return GMonth == MonthsEnum.March ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal April
        {
            get { return GMonth == MonthsEnum.April ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal May
        {
            get { return GMonth == MonthsEnum.May ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal June
        {
            get { return GMonth == MonthsEnum.June ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal July
        {
            get { return GMonth == MonthsEnum.July ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal August
        {
            get { return GMonth == MonthsEnum.August ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal September
        {
            get { return GMonth == MonthsEnum.September ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal October
        {
            get { return GMonth == MonthsEnum.October ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal November
        {
            get { return GMonth == MonthsEnum.November ? Expense + Income : 0; }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "n")]
        public decimal December
        {
            get { return GMonth == MonthsEnum.December ? Expense + Income : 0; }
        }

        [NonPersistent]
        public string GQuarter
        {
            get
            {
                switch (GMonth)
                {
                    case MonthsEnum.None:
                        break;
                    case MonthsEnum.January:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.February:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.March:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.April:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.May:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.June:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.July:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.August:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.September:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.October:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.November:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.December:
                        _Quarter = "4th QTR";
                        break;
                    default:
                        break;
                }
                return _Quarter;
            }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "d")]
        public int GYear
        {
            get
            {
                return EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string GMonthSorter
        {
            get
            {
                switch (GMonth)
                {
                    case MonthsEnum.None:
                        return "00 NONE";
                    case MonthsEnum.January:
                        return "01 JANUARY";
                    case MonthsEnum.February:
                        return "02 FEBRUARY";
                    case MonthsEnum.March:
                        return "03 MARCH";
                    case MonthsEnum.April:
                        return "04 APRIL";
                    case MonthsEnum.May:
                        return "05 MAY";
                    case MonthsEnum.June:
                        return "06 JUNE";
                    case MonthsEnum.July:
                        return "07 JULY";
                    case MonthsEnum.August:
                        return "08 AUGUST";
                    case MonthsEnum.September:
                        return "09 SEPTEMBER";
                    case MonthsEnum.October:
                        return "10 OCTOBER";
                    case MonthsEnum.November:
                        return "11 NOVEMBER";
                    case MonthsEnum.December:
                        return "12 DECEMBER";
                    default:
                        return "00 NONE";
                }
            }
        }

        #endregion

        public IncExpForChart(Session session)
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
