using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class IncomeAndExpense : BaseObject {
        private Account _Account;
        private DateTime _EntryDate;
        private GenJournalHeader _SourceID;
        private SourceType _SourceType;
        private string _SourceNo;
        private Guid _LineID;
        private DateTime _Seq;
        private Contact _Payee;
        private string _Description2;
        private ExpenseType _Category;
        private SubExpenseType _SubCategory;
        private decimal _Payment;
        private decimal _Deposit;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account Account {
            get { return _Account; }
            set { SetPropertyValue("Account", ref _Account, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader SourceID {
            get { return _SourceID; }
            set { SetPropertyValue("SourceID", ref _SourceID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue("SourceType", ref _SourceType, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue("SourceNo", ref _SourceNo, value); }
        }

        public DateTime Seq {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid LineID {
            get { return _LineID; }
            set { SetPropertyValue("LineID", ref _LineID, value); }
        }

        public Contact Payee {
            get { return _Payee; }
            set { SetPropertyValue("Payee", ref _Payee, value); }
        }

        public string Description2 {
            get { return _Description2; }
            set { SetPropertyValue("Description2", ref _Description2, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("Expense-IncomeAndExpenses")]
        public ExpenseType Category {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }

        public SubExpenseType SubCategory {
            get { return _SubCategory; }
            set { SetPropertyValue("SubCategory", ref _SubCategory, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Payment {
            get { return _Payment; }
            set { SetPropertyValue("Payment", ref _Payment, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Deposit {
            get { return _Deposit; }
            set { SetPropertyValue("Deposit", ref _Deposit, value); }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum GMonth {
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
        public decimal January {
            get { return GMonth==MonthsEnum.January?Payment + Deposit:0; }
        }
        [NonPersistent]
        public decimal February {
            get { return GMonth == MonthsEnum.February ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal March {
            get { return GMonth == MonthsEnum.March ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal April {
            get { return GMonth == MonthsEnum.April ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal May {
            get { return GMonth == MonthsEnum.May ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal June {
            get { return GMonth == MonthsEnum.June ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal July {
            get { return GMonth == MonthsEnum.July ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal August {
            get { return GMonth == MonthsEnum.August ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal September {
            get { return GMonth == MonthsEnum.September ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal October {
            get { return GMonth == MonthsEnum.October ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal November {
            get { return GMonth == MonthsEnum.November ? Payment + Deposit : 0; }
        }
        [NonPersistent]
        public decimal December {
            get { return GMonth == MonthsEnum.December ? Payment + Deposit : 0; }
        }

        [NonPersistent]
        public string GQuarter {
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
        public int GYear {
            get
            {
                return EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string GMonthSorter {
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

        public IncomeAndExpense(Session session)
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

        protected override void OnSaving() {
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
