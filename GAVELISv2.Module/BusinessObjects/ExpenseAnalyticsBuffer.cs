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
    public class ExpenseAnalyticsBuffer : XPObject
    {
        private string _BufferId;
        private int _Seq;
        private int _Year;
        private DateTime _EntryDate;
        private Contact _PayTo;
        private string _Memo;
        private GenJournalHeader _Source;
        private PaymentTypeEnum _PaymentMode;
        private string _CheckNo;
        private DateTime _CheckDate;
        private string _ReferenceNo;
        private decimal _CheckAmount;
        private DateTime _LineDate;
        private Contact _Payee;
        private string _Description;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private CostCenter _ChargeTo;
        private decimal _Amount;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string BufferId
        {
            get { return _BufferId; }
            set { SetPropertyValue("BufferId", ref _BufferId, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        // Year
        [Custom("AllowEdit", "False")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        // EntryDate -> DateTime
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // PayTo -> Contact
        [Custom("AllowEdit", "False")]
        public Contact PayTo
        {
            get { return _PayTo; }
            set { SetPropertyValue("PayTo", ref _PayTo, value); }
        }
        // Memo -> string(1000)
        [Custom("AllowEdit", "False")]
        [Size(1000)]
        public string Memo
        {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }
        // Source > GenJournalHeader
        [Custom("AllowEdit", "False")]
        public GenJournalHeader Source
        {
            get { return _Source; }
            set { SetPropertyValue("Source", ref _Source, value); }
        }
        // PaymentMode -> PaymentMode
        [Custom("AllowEdit", "False")]
        public PaymentTypeEnum PaymentMode
        {
            get { return _PaymentMode; }
            set { SetPropertyValue("PaymentMode", ref _PaymentMode, value); }
        }
        // CheckNo -> string
        [Custom("AllowEdit", "False")]
        public string CheckNo
        {
            get { return _CheckNo; }
            set { SetPropertyValue("CheckNo", ref _CheckNo, value); }
        }
        // CheckDate -> DateTime
        [Custom("AllowEdit", "False")]
        public DateTime CheckDate
        {
            get { return _CheckDate; }
            set { SetPropertyValue("CheckDate", ref _CheckDate, value); }
        }
        // ReferenceNo -> string
        [Custom("AllowEdit", "False")]
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }
        // CheckAmount -> decimal
        [Custom("AllowEdit", "False")]
        public decimal CheckAmount
        {
            get { return _CheckAmount; }
            set { SetPropertyValue("CheckAmount", ref _CheckAmount, value); }
        }
        // LineDate -> DateTime
        [Custom("AllowEdit", "False")]
        public DateTime LineDate
        {
            get { return _LineDate; }
            set { SetPropertyValue("LineDate", ref _LineDate, value); }
        }
        // Payee ->  Contact
        [Custom("AllowEdit", "False")]
        public Contact Payee
        {
            get { return _Payee; }
            set { SetPropertyValue("Payee", ref _Payee, value); }
        }
        // Description -> string
        [Custom("AllowEdit", "False")]
        [Size(500)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        // ExpenseType -> ExpenseType
        [Custom("AllowEdit", "False")]
        public ExpenseType ExpenseType
        {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }
        // SubExpenseType -> SubExpenseType
        [Custom("AllowEdit", "False")]
        public SubExpenseType SubExpenseType
        {
            get { return _SubExpenseType; }
            set { SetPropertyValue("SubExpenseType", ref _SubExpenseType, value); }
        }
        // ChargeTo -> CostCenter
        [Custom("AllowEdit", "False")]
        public CostCenter ChargeTo
        {
            get { return _ChargeTo; }
            set { SetPropertyValue("ChargeTo", ref _ChargeTo, value); }
        }
        // Amount -> decimal
        [Custom("AllowEdit", "False")]
        public decimal Amount
        {
            get { return _Amount; }
            set { SetPropertyValue("Amount", ref _Amount, value); }
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

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
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


        public ExpenseAnalyticsBuffer(Session session)
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
