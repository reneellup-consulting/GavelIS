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
    public class CashSale : XPObject {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private DateTime _Date;
        private Customer _Customer;
        private string _SourceDesc;
        private string _SourceNo;
        private string _CINo;
        private decimal _CashAmount;
        private string _BankName;
        private string _CheckNo;
        private decimal _CheckAmount;
        private decimal _Total;
        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set { SetPropertyValue("GenJournalID", ref _GenJournalID, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime Date {
            get { return _Date; }
            set
            {
                SetPropertyValue("Date", ref _Date, value);
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Customer Customer {
            get { return _Customer; }
            set { SetPropertyValue<Customer>("Customer", ref _Customer, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string SourceDesc {
            get { return _SourceDesc; }
            set { SetPropertyValue<string>("SourceDesc", ref _SourceDesc, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string SourceNo {
            get { return _SourceNo; }
            set { SetPropertyValue<string>("SourceNo", ref _SourceNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string CINo {
            get { return _CINo; }
            set { SetPropertyValue<string>("CINo", ref _CINo, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        public decimal CashAmount {
            get { return _CashAmount; }
            set { SetPropertyValue<decimal>("CashAmount", ref _CashAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string BankName {
            get { return _BankName; }
            set { SetPropertyValue<string>("BankName", ref _BankName, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue<string>("CheckNo", ref _CheckNo, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        public decimal CheckAmount {
            get { return _CheckAmount; }
            set { SetPropertyValue<decimal>("CheckAmount", ref _CheckAmount, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        public decimal Total {
            get { return _Total; }
            set { SetPropertyValue<decimal>("Total", ref _Total, value); }
        }

        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month
        {
            get
            {
                _Month = Date.Month == 1 ? MonthsEnum.January : Date.Month
                 == 2 ? MonthsEnum.February : Date.Month == 3 ? MonthsEnum.
                March : Date.Month == 4 ? MonthsEnum.April : Date.Month ==
                5 ? MonthsEnum.May : Date.Month == 6 ? MonthsEnum.June :
                Date.Month == 7 ? MonthsEnum.July : Date.Month == 8 ?
                MonthsEnum.August : Date.Month == 9 ? MonthsEnum.September
                 : Date.Month == 10 ? MonthsEnum.October : Date.Month == 11
                 ? MonthsEnum.November : Date.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string Quarter
        {
            get
            {
                switch (Month)
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
        public int Year
        {
            get
            {
                return Date.Year;
                ;
            }
        }

        public string MonthSorter {
            get
            {
                switch (_Month)
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

        public CashSale(Session session)
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
