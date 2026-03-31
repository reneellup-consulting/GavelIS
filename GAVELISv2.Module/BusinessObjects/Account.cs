using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.Persistent.Base.General;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [OptimisticLocking(false)]
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class Account : BaseObject, ITreeNode {
        private const string defaultSearchFormat1 =
        "{ParentAccount.SearchName}|{Name}";

        private const string defaultSearchFormat2 = "{Name}";
        private const string defaultDisplayFormat = "{No}::{SearchName}";
        private string _No;
        private string _Code;
        private string _Name;
        private Account _ParentAccount;
        private DebitCreditEnum _PlusSide;
        private AccountType _AccountType;
        private AccountTotalType _CashType;
        private CashOpeEnum _CashOpe;
        private AccountGeneralType _GeneralType;
        private bool _Blocked = false;
        private DebitCreditEnum _SystemPlusSide;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private bool _IsHeadTotal = false;
        private string _TotalSubCaption;
        private string _MultiCVPayDetCaption;
        private bool _LessInMultiCV;
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string No {
            get { return _No; }
            set
            {
                SetPropertyValue("No", ref _No, value);
                if (!IsLoading)
                {
                    Code = _No;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }

        [Association("Account-ChildAccounts")]
        public Account ParentAccount {
            get { return _ParentAccount; }
            set { SetPropertyValue("ParentAccount", ref _ParentAccount, value); }
        }

        public DebitCreditEnum PlusSide {
            get { return _PlusSide; }
            set { SetPropertyValue("PlusSide", ref _PlusSide, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public AccountType AccountType {
            get { return _AccountType; }
            set
            {
                SetPropertyValue("AccountType", ref _AccountType, value);
                if (!IsLoading && _AccountType != null)
                {
                    PlusSide = _AccountType.DebitCredit;
                    SystemPlusSide = _AccountType.DebitCredit;
                    CashType = _AccountType.CashType != null ? _AccountType.
                    CashType : null;
                    CashOpe = _AccountType.CashType != null ? _AccountType.
                    CashOpe : CashOpeEnum.None;
                    GeneralType = _AccountType.GeneralType;
                }
            }
        }

        public AccountTotalType CashType {
            get { return _CashType; }
            set { SetPropertyValue("CashType", ref _CashType, value); }
        }

        public CashOpeEnum CashOpe {
            get { return _CashOpe; }
            set { SetPropertyValue("CashOpe", ref _CashOpe, value); }
        }

        //[Custom("AllowEdit", "False")]
        public AccountGeneralType GeneralType {
            get { return _GeneralType; }
            set { SetPropertyValue("GeneralType", ref _GeneralType, value); }
        }

        public bool Blocked {
            get { return _Blocked; }
            set { SetPropertyValue("Blocked", ref _Blocked, value); }
        }

        [Custom("AllowEdit", "False")]
        public DebitCreditEnum SystemPlusSide {
            get { return _SystemPlusSide; }
            set { SetPropertyValue("SystemPlusSide", ref _SystemPlusSide, value)
                ; }
        }

        public ExpenseType ExpenseType {
            get { return _ExpenseType; }
            set { SetPropertyValue("ExpenseType", ref _ExpenseType, value); }
        }

        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType {
            get { return _SubExpenseType; }
            set { SetPropertyValue("SubExpenseType", ref _SubExpenseType, value)
                ; }
        }

        public bool IsHeadTotal {
            get { return _IsHeadTotal; }
            set { SetPropertyValue("IsHeadTotal", ref _IsHeadTotal, value); }
        }

        public string TotalSubCaption {
            get { return _TotalSubCaption; }
            set { SetPropertyValue("TotalSubCaption", ref _TotalSubCaption,
                value); }
        }

        public string MultiCVPayDetCaption {
            get { return _MultiCVPayDetCaption; }
            set { SetPropertyValue<string>("MultiCVPayDetCaption", ref _MultiCVPayDetCaption, value); }
        }

        public bool LessInMultiCV {
            get { return _LessInMultiCV; }
            set { SetPropertyValue<bool>("LessInMultiCV", ref _LessInMultiCV, value); }
        }

        #region Bank Information

        private string _BankName;
        private string _BankAccountNo;
        private string _Phone;
        private string _LastCheckNo;
        public string BankName {
            get { return _BankName; }
            set { SetPropertyValue("BankName", ref _BankName, value); }
        }

        public string BankAccountNo {
            get { return _BankAccountNo; }
            set { SetPropertyValue("BankAccountNo", ref _BankAccountNo, value); }
        }

        #region Bank Address

        private const string defaultBankAddressFormat =
        "{BankAddress}, {BankZipCode} {BankCity}, {BankProvince}, {BankCountry}"
        ;

        private string _BankAddress;
        private string _BankCity;
        private string _BankZipCode;
        private string _BankProvince;
        private string _BankCountry = "Philippines";
        [Size(500)]
        public string BankAddress {
            get { return _BankAddress; }
            set { SetPropertyValue("BankAddress", ref _BankAddress, value); }
        }

        public string BankCity {
            get { return _BankCity; }
            set { SetPropertyValue("BankCity", ref _BankCity, value); }
        }

        public string BankZipCode {
            get { return _BankZipCode; }
            set { SetPropertyValue("BankZipCode", ref _BankZipCode, value); }
        }

        public string BankProvince {
            get { return _BankProvince; }
            set { SetPropertyValue("BankProvince", ref _BankProvince, value); }
        }

        public string BankCountry {
            get { return _BankCountry; }
            set { SetPropertyValue("BankCountry", ref _BankCountry, value); }
        }

        public string FullBankAddress {
            get { return ObjectFormatter.Format(
                defaultBankAddressFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        public string Phone {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        public string LastCheckNo {
            get { return _LastCheckNo; }
            set { SetPropertyValue("LastCheckNo", ref _LastCheckNo, value); }
        }

        #endregion

        [Association("Account-GenJournalDetails")]
        public XPCollection<GenJournalDetail> GenJournalDetails {
            get { return
                GetCollection<GenJournalDetail>("GenJournalDetails"); }
        }

        #region Balance Calculation

        [Persistent("CreditBalance")]
        private decimal? _CreditBalance = null;
        [PersistentAlias("_CreditBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal? CreditBalance {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _CreditBalance == null)
                    {
                        UpdateCreditBalance(false);
                    }
                } catch (Exception)
                {
                }
                return _CreditBalance;
            }
        }

        public void UpdateCreditBalance(bool forceChangeEvent) {
            decimal? oldCreditBalance = _CreditBalance;
            decimal tempTotal = 0m;
            foreach (GenJournalDetail detail in GenJournalDetails)
            {
                if (detail.
                Approved)
                {
                    tempTotal += detail.CreditAmount;
                }
            }
            _CreditBalance = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("CreditBalance", oldCreditBalance,
                _CreditBalance);
            }
            ;
        }

        [Persistent("DebitBalance")]
        private decimal? _DebitBalance = null;
        [PersistentAlias("_DebitBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal? DebitBalance {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _DebitBalance == null)
                    {
                        UpdateDebitBalance(false);
                    }
                } catch (Exception)
                {
                }
                return _DebitBalance;
            }
        }

        public void UpdateDebitBalance(bool forceChangeEvent) {
            decimal? oldDebitBalance = _DebitBalance;
            decimal tempTotal = 0m;
            foreach (GenJournalDetail detail in GenJournalDetails)
            {
                if (detail.
                Approved)
                {
                    tempTotal += detail.DebitAmount;
                }
            }
            _DebitBalance = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("DebitBalance", oldDebitBalance,
                _DebitBalance);
            }
            ;
        }

        [PersistentAlias("DebitBalance - CreditBalance")]
        [Custom("DisplayFormat", "n")]
        public decimal Balance {
            get
            {
                object tempObject = EvaluateAlias("Balance");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
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

        #region Display String

        [Persistent]
        public string SearchName {
            get
            {
                if (!IsLoading && !IsSaving && _ParentAccount != null)
                {
                    return
                    ObjectFormatter.Format(defaultSearchFormat1, this,
                    EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty);
                } else
                {
                    return ObjectFormatter.Format(defaultSearchFormat2, this,
                    EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty);
                }
            }
        }

        [Persistent]
        public string DisplayName {
            get
            {
                // if parentAccount!=null then display displayformat2
                // else displayformat1
                return ObjectFormatter.Format(defaultDisplayFormat, this,
                EmptyEntriesMode.RemoveDelimeterWhenEntryIsEmpty);
            }
        }

        #endregion

        #region Action

        public string GetCheckNo() {
            string seqNo;
            string incNo;
            int inc = 1;
            if (!string.IsNullOrEmpty(_LastCheckNo))
            {
                seqNo = _LastCheckNo;
            } else
            {
                return string.Empty;
            }
            string digits = "0123456789";
            string defaultFormat = "{0:D5}";
            string formatString = string.Empty;
            string num = string.Empty;
            int c = 0;
            int i, x;
            i = x = seqNo.LastIndexOfAny(digits.ToCharArray());
            while (i >= 0 && isDigit(seqNo[i]))
            {
                num = seqNo[i] + num;
                c++;
                i--;
            }
            long n = long.Parse(num) + inc;
            formatString = defaultFormat.Replace("5", c.ToString());
            incNo = string.Format(formatString, n);
            x = x + 1 - num.Length;
            seqNo = seqNo.Remove(x, num.Length);
            seqNo = seqNo.Insert(x, string.Empty + incNo);
            _LastCheckNo = seqNo;
            // Update the No Series Line
            //UnitOfWork uow = new UnitOfWork();
            //ObjectSpace os = new ObjectSpace(uow);
            //Session _session = new Session(objectSpace.Session.DataLayer);
            //_session.BeginTransaction();
            //NoSeriesLine nSLine = _session.FindObject<NoSeriesLine>(new
            //BinaryOperator("Oid", nsLineNo));
            //nSLine.LastDateUsed = DateTime.Today;
            //nSLine.LastNoUsed = seqNo;
            ////nSLine.;
            //nSLine.Save();
            //_session.CommitTransaction();
            return seqNo;
        }

        private static bool isDigit(char c) {
            string digits = "0123456789";
            return digits.IndexOf(c) == -1 ? false : true;
        }

        #endregion

        public Account(Session session)
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;

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

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _DebitBalance = null;
            _CreditBalance = null;
        }

        [Association("Account-ChildAccounts")]
        public XPCollection<Account> ChildAccounts {
            get { return GetCollection<
                Account>("ChildAccounts"); }
        }

        #region ITreeNode Members

        public System.ComponentModel.IBindingList Children {
            get { return
                ChildAccounts; }
        }

        public ITreeNode Parent {
            get { return _ParentAccount; }
        }

        #endregion

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
