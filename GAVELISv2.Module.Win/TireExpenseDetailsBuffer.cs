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
    //[NonPersistent]
    public class TireExpenseDetailsBuffer : XPObject
    {
        //private static int myOid = 0;
        private ExpenseType _Category;
        private SubExpenseType _SubCategory;
        private decimal _Expense;

        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        public GenJournalHeader SourceID
        {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }

        public SourceType SourceType
        {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }

        public string SourceNo
        {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }

        public string RefID
        {
            get { return _RefID; }
            set { SetPropertyValue<string>("RefID", ref _RefID, value); }
        }

        public RwsTireDetail RequestRef
        {
            get { return _RequestRef; }
            set { SetPropertyValue<RwsTireDetail>("RequestRef", ref _RequestRef, value); }
        }

        public ContactTypeEnum PayeeType
        {
            get { return _PayeeType; }
            set { SetPropertyValue<ContactTypeEnum>("PayeeType", ref _PayeeType, value); }
        }

        public Contact Payee
        {
            get { return _Payee; }
            set { SetPropertyValue("Payee", ref _Payee, value); }
        }

        [Size(1000)]
        public string Description1
        {
            get { return _Description1; }
            set { SetPropertyValue<string>("Description1", ref _Description1, value); }
        }

        [Size(1000)]
        public string Description2
        {
            get { return _Description2; }
            set { SetPropertyValue("Description2", ref _Description2, value); }
        }

        public ExpenseType Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }
        public SubExpenseType SubCategory
        {
            get { return _SubCategory; }
            set { SetPropertyValue("SubCategory", ref _SubCategory, value); }
        }

        public CostCenter CostCenter
        {
            get { return _CostCenter; }
            set
            {
                SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value);
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Expense
        {
            get { return _Expense; }
            set { SetPropertyValue("Expense", ref _Expense, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Income
        {
            get { return _Income; }
            set { SetPropertyValue<decimal>("Income", ref _Income, value); }
        }

        public FixedAsset Fleet
        {
            get { return _Fleet; }
            set { SetPropertyValue("Fleet", ref _Fleet, value); }
        }

        public Facility Facility
        {
            get { return _Facility; }
            set
            {
                SetPropertyValue("Facility", ref _Facility, value);
            }
        }

        public Department Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        public Employee FacilityHead
        {
            get { return _FacilityHead; }
            set { SetPropertyValue("FacilityHead", ref _FacilityHead, value); }
        }

        public Employee DepartmentInCharge
        {
            get { return _DepartmentInCharge; }
            set { SetPropertyValue("DepartmentInCharge", ref _DepartmentInCharge, value); }
        }

        [PersistentAlias("Income - Expense")]
        [Custom("DisplayFormat", "n")]
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
        [NonPersistent]
        public string PayeeName
        {
            get { return Payee != null ? Payee.Name: string.Empty; }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        private DateTime _EntryDate;
        private GenJournalHeader _SourceID;
        private BusinessObjects.SourceType _SourceType;
        private string _SourceNo;
        private string _RefID;
        private ContactTypeEnum _PayeeType;
        private Contact _Payee;
        private string _Description1;
        private string _Description2;
        private BusinessObjects.CostCenter _CostCenter;
        private decimal _Income;
        private BusinessObjects.Facility _Facility;
        private BusinessObjects.Department _Department;
        private Employee _FacilityHead;
        private Employee _DepartmentInCharge;
        private FixedAsset _Fleet;
        private RwsTireDetail _RequestRef;
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
        [NonPersistent]
        public RequisitionWorksheet RequestLineInfo
        {
            get
            {
                if (_SourceType == null)
                {
                    return null;
                }
                // RC
                if (_SourceType.Code == "RC")
                {
                    ReceiptDetail rDet = Session.FindObject<ReceiptDetail>(BinaryOperator.Parse("[Oid]=?", _RefID));
                    if (rDet != null && rDet.PODetailID != null)
                    {
                        RequisitionWorksheet rwSheet = Session.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", rDet.PODetailID.RequestID));
                        return rwSheet ?? null;
                    }
                }
                // WO
                if (_SourceType.Code == "WO")
                {
                    WorkOrderItemDetail rDet = Session.FindObject<WorkOrderItemDetail>(BinaryOperator.Parse("[Oid]=?", _RefID));
                    RequisitionWorksheet rwSheet = Session.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", rDet.RequestID));
                    return rwSheet ?? null;
                }
                // JO
                if (_SourceType.Code == "JO")
                {
                    JobOrderDetail rDet = Session.FindObject<JobOrderDetail>(BinaryOperator.Parse("[Oid]=?", _RefID));
                    RequisitionWorksheet rwSheet = Session.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", rDet.RequestID));
                    return rwSheet ?? null;
                }
                // DM
                if (_SourceType.Code == "DM")
                {
                    DebitMemoDetail rDet = Session.FindObject<DebitMemoDetail>(BinaryOperator.Parse("[Oid]=?", _RefID));
                    if (rDet != null && rDet.ReceiptDetailID != null && rDet.ReceiptDetailID.PODetailID != null)
                    {
                        RequisitionWorksheet rwSheet = Session.FindObject<RequisitionWorksheet>(BinaryOperator.Parse("[RowID]=?", rDet.ReceiptDetailID.PODetailID.RequestID));
                        return rwSheet ?? null;
                    }
                }
                return null;
            }
        }
        public TireExpenseDetailsBuffer(Session session)
            : base(session) {
            //myOid++;
            //Oid = myOid;
        }

    }
}
