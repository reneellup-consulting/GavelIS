using System;
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
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class ReceivePayment : GenJournalHeader, ISetIncomeExpense {
        private Account _BankCashAccount;
        private PaymentTypeEnum _PaymentMode;
        //private DateTime _CheckDate;
        private string _CheckNo;
        private string _BankBranch;
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private PaymentStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private Contact _ReceiveFrom;
        private Account _GetFromAccount;
        private ExpenseType _IncomeType;
        private SubExpenseType _SubIncomeType;
        private decimal _CheckAmount;
        private decimal _Adjusted;
        private decimal _OpenAmount;
        private bool _Deposited = false;
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account BankCashAccount {
            get { return _BankCashAccount; }
            set { SetPropertyValue("BankCashAccount", ref _BankCashAccount,
                value); }
        }

        public PaymentTypeEnum PaymentMode {
            get { return _PaymentMode; }
            set { SetPropertyValue("PaymentMode", ref _PaymentMode, value); }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        //public DateTime CheckDate {
        //    get { return _CheckDate; }
        //    set { SetPropertyValue("CheckDate", ref _CheckDate, value); }
        //}
        [RuleRequiredField("", DefaultContexts.Save)]
        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue("CheckNo", ref _CheckNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Bank Name/Branch")]
        public string BankBranch {
            get { return _BankBranch; }
            set { SetPropertyValue("BankBranch", ref _BankBranch, value); }
        }

        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        [Size(500)]
        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }

        public PaymentStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != PaymentStatusEnum.Current)
                    {
                        Approved = true;
                    } else
                    {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Contact ReceiveFrom {
            get { return _ReceiveFrom; }
            set { SetPropertyValue("ReceiveFrom", ref _ReceiveFrom, value); }
        }

        public Account GetFromAccount {
            get { return _GetFromAccount; }
            set { SetPropertyValue("GetFromAccount", ref _GetFromAccount, value)
                ; }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType IncomeType {
            get { return _IncomeType; }
            set
            {
                SetPropertyValue("IncomeType", ref _IncomeType, value);
                if (!IsLoading)
                {
                    SubIncomeType = null;
                }
            }
        }

        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubIncomeType {
            get { return _SubIncomeType; }
            set { SetPropertyValue("SubIncomeType", ref _SubIncomeType, value)
                ; }
        }

        private decimal _Withheld;
        [Custom("DisplayFormat", "n")]
        public decimal Withheld {
            get { return _Withheld; }
            set { SetPropertyValue<decimal>("Withheld", ref _Withheld, value); }
        }

        private Account _OutputTaxAcct;
        public Account OutputTaxAcct {
            get { return _OutputTaxAcct; }
            set { SetPropertyValue<Account>("OutputTaxAcct", ref _OutputTaxAcct, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CheckAmount {
            get { return _CheckAmount; }
            set { SetPropertyValue("CheckAmount", ref _CheckAmount, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Adjusted {
            get { return _Adjusted; }
            set { SetPropertyValue("Adjusted", ref _Adjusted, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal OpenAmount {
            get { return _OpenAmount; }
            set { SetPropertyValue("OpenAmount", ref _OpenAmount, value); }
        }

        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public bool Deposited {
            get { return _Deposited; }
            set { SetPropertyValue("Deposited", ref _Deposited, value); }
        }

        public ReceivePayment(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "CR"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PR"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "CR"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
        }

        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Payments."
                );
            }
        }
    }
}
