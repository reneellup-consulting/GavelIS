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
    public class GenJournalDetail : XPObject, ISetIncomeExpense {
        private Guid _RowID;
        private GenJournalHeader _GenJournalID;
        private Account _Account;
        private decimal _DebitAmount;
        private decimal _CreditAmount;
        private string _Description;
        private string _Description2;
        private Account _EffectingAccount;
        private Contact _SubAccountNo;
        private ContactTypeEnum _SubAccountType;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private bool _Approved = false;
        [Association("GenJournalHeader-GenJournalDetails")]
        public GenJournalHeader GenJournalID {
            get { return _GenJournalID; }
            set
            {
                SetPropertyValue("GenJournalID", ref _GenJournalID, value);
                if (!IsLoading && _GenJournalID != null)
                {
                    if (_GenJournalID.OperationType.Code == "CV")
                    {
                        CVLineDate = _GenJournalID.EntryDate;
                        SubAccountNo = ((CheckVoucher)_GenJournalID).PayToOrder
                         != null ? ((CheckVoucher)_GenJournalID).PayToOrder :
                        null;
                    }
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("Account-GenJournalDetails")]
        public Account Account {
            get { return _Account; }
            set
            {
                SetPropertyValue("Account", ref _Account, value);
                if (!IsLoading && _PODetailID == null)
                {
                    if (_Account != null)
                    {
                        ExpenseType = _Account.ExpenseType != null ? _Account.
                        ExpenseType : null;
                        SubExpenseType = _Account.SubExpenseType != null ? _Account.SubExpenseType : null;
                        Description = _Account.Name;
                    } else
                    {
                        ExpenseType = null;
                        SubExpenseType = null;
                        Description = string.Empty;
                    }
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal DebitAmount {
            get { return _DebitAmount; }
            set
            {
                SetPropertyValue("DebitAmount", ref _DebitAmount, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((GenJournalHeader)_GenJournalID).UpdateDebitBalance(
                        true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        public decimal CreditAmount {
            get { return _CreditAmount; }
            set
            {
                SetPropertyValue("CreditAmount", ref _CreditAmount, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((GenJournalHeader)_GenJournalID).UpdateCreditBalance(
                        true);
                        if (_GenJournalID.GetType() == typeof(CheckVoucher))
                        {
                            ((CheckVoucher)_GenJournalID).UpdateCheckAmount(true);
                        }
                    } catch (Exception)
                    {
                    }
                }
            }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Description2 {
            get { return _Description2; }
            set { SetPropertyValue("Description2", ref _Description2, value); }
        }

        public Account EffectingAccount {
            get { return _EffectingAccount; }
            set { SetPropertyValue("EffectingAccount", ref _EffectingAccount,
                value); }
        }

        public Contact SubAccountNo {
            get { return _SubAccountNo; }
            set
            {
                SetPropertyValue("SubAccountNo", ref _SubAccountNo, value);
                if (!IsLoading && _SubAccountNo != null)
                {
                    SubAccountType =
                    _SubAccountNo.ContactType;
                }
            }
        }

        public ContactTypeEnum SubAccountType {
            get { return _SubAccountType; }
            set { SetPropertyValue("SubAccountType", ref _SubAccountType, value)
                ; }
        }

        public ExpenseType ExpenseType {
            get { return _ExpenseType; }
            set
            {
                SetPropertyValue("ExpenseType", ref _ExpenseType, value);
                if (!IsLoading)
                {
                    SubExpenseType = null;
                }
            }
        }

        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubExpenseType {
            get { return _SubExpenseType; }
            set { SetPropertyValue("SubExpenseType", ref _SubExpenseType, value)
                ; }
        }

        public bool Approved {
            get { return _Approved; }
            set
            {
                SetPropertyValue("Approved", ref _Approved, value);
                if (!IsLoading)
                {
                    if (_Approved)
                    {
                        bool IsCV = this.GenJournalID.ClassInfo.ClassType == typeof(CheckVoucher) ? true : false;
                        if (!IsCV && _Account.AccountType.Code == "B" || _Account.AccountType.Code == "C")
                        {
                            if (_ExpenseType == null)
                            {
                                if (_GenJournalID.SourceType.ExpenseType != null)
                                {
                                    _ExpenseType = _GenJournalID.SourceType.ExpenseType;
                                } else
                                {
                                    throw new ApplicationException("Cash and Bank transaction must be provided with an ExpenseType");
                                }
                            }
                            if (_ExpenseType.requireSubExp)
                            {
                                if (_SubExpenseType == null)
                                {
                                    if (_GenJournalID.SourceType.SubExpenseType != null)
                                    {
                                        _SubExpenseType = _GenJournalID.SourceType.SubExpenseType;
                                    } else
                                    {
                                        throw new ApplicationException("Must provide Sub Expenses Type for this type of transaction");
                                    }
                                }
                            }

                            IncomeAndExpense incomeExp = new IncomeAndExpense(this.Session);
                            incomeExp.Account = _Account;
                            incomeExp.EntryDate = _GenJournalID.CheckDate != DateTime.MinValue ? _GenJournalID.CheckDate : _GenJournalID.EntryDate;
                            incomeExp.SourceID = _GenJournalID;
                            incomeExp.SourceType = _GenJournalID.SourceType;
                            incomeExp.Seq = DateTime.Now.ToUniversalTime();
                            incomeExp.SourceNo = _GenJournalID.SourceNo;
                            incomeExp.LineID = _RowID;
                            incomeExp.Payee = _SubAccountNo != null ? _SubAccountNo : null;
                            incomeExp.Category = _ExpenseType;
                            incomeExp.SubCategory = _SubExpenseType != null ? _SubExpenseType : null;
                            incomeExp.Payment = _CreditAmount;
                            incomeExp.Deposit = _DebitAmount;
                            incomeExp.Save();
                        }

                        if (IsCV == true && (_Account.AccountType.GeneralType.Code == "1-07" || _Account.AccountType.GeneralType.Code == "1-09" || _Account.AccountType.GeneralType.Code == "1-10"))
                        {
                            if (_ExpenseType == null)
                            {
                                throw new ApplicationException("This expense transaction must be provided with an ExpenseType");
                            }
                            if (_ExpenseType.requireSubExp)
                            {
                                if (_SubExpenseType == null)
                                {
                                    throw new ApplicationException("Must provide Sub Expenses Type for this type of transaction");
                                }
                            }

                            IncomeAndExpense incomeExp = new IncomeAndExpense(this.Session);
                            incomeExp.Account = _GenJournalID.CRBankCashAccount;
                            incomeExp.EntryDate = _GenJournalID.CheckDate != DateTime.MinValue ? _GenJournalID.CheckDate : _GenJournalID.EntryDate;
                            incomeExp.SourceID = _GenJournalID;
                            incomeExp.SourceType = _GenJournalID.SourceType;
                            incomeExp.Seq = DateTime.Now.ToUniversalTime();
                            incomeExp.SourceNo = _GenJournalID.SourceNo;
                            incomeExp.LineID = _RowID;
                            incomeExp.Payee = _SubAccountNo != null ? _SubAccountNo : null;
                            incomeExp.Description2 = _Description != string.Empty ? _Description : _Description2;
                            incomeExp.Category = _ExpenseType;
                            incomeExp.SubCategory = _SubExpenseType != null ? _SubExpenseType : null;
                            if (_CreditAmount != 0)
                            {
                                throw new ApplicationException("Expenses specified in the Petty Cash Voucher cannot be in the Credit side");
                            }
                            incomeExp.Payment = _DebitAmount;
                            //incomeExp.Deposit = _DebitAmount;
                            incomeExp.Save();
                        }
                    } else
                    {
                        IncomeAndExpense incomeExp = this.Session.FindObject<IncomeAndExpense>(CriteriaOperator.Parse("[LineID] = '" + _RowID + "'"));
                        if (incomeExp != null)
                        {
                            incomeExp.Delete();
                        }
                    }
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        #region Post Value Validation

        [RuleFromBoolProperty("", DefaultContexts.Save,
        "Cannot post value to none posting account")]
        protected bool IsValidValuePosting {
            get
            {
                if (_Account != null && _DebitAmount != 0 || _CreditAmount != 0)
                {
                    if (_Account.ChildAccounts.Count == 0)
                    {
                        return true;
                    } else
                    {
                        return false;
                    }
                } else
                {
                    return true;
                }
            }
        }

        #endregion

        #region Check/Petty Cash Voucher

        private DateTime _CVLineDate;
        private CostCenter _CostCenter;
        private bool _IsCheckAmount = false;
        private string _PettyCashNo;
        [DisplayName("Line Date")]
        public DateTime CVLineDate {
            get { return _CVLineDate; }
            set { SetPropertyValue("CVLineDate", ref _CVLineDate, value); }
        }
        [DisplayName("Charge To")]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue("CostCenter", ref _CostCenter, value); }
        }

        [ImmediatePostData]
        public bool IsCheckAmount {
            get { return _IsCheckAmount; }
            set
            {
                SetPropertyValue("IsCheckAmount", ref _IsCheckAmount, value);
                if (!IsLoading)
                {
                    try
                    {
                        ((CheckVoucher)_GenJournalID).UpdateCheckAmount(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        public string PettyCashNo {
            get { return _PettyCashNo; }
            set { SetPropertyValue<string>("PettyCashNo", ref _PettyCashNo, value); }
        }

        #endregion
        #region From PO to Petty Cash
        [Custom("AllowEdit", "False")]
        public PurchaseOrderDetail PODetailID
        {
            get { return _PODetailID; }
            set { SetPropertyValue<PurchaseOrderDetail>("PODetailID", ref _PODetailID, value); }
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

        public GenJournalDetail(Session session)
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
            //Session.OptimisticLockingReadBehavior = 
            //OptimisticLockingReadBehavior.ReloadObject;
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

        private int deletedOid;
        protected override void OnDeleting() {
            if (_PODetailID != null)
            {
                PODetailID.PettyCashID = null;
                PODetailID = null;
            }
            if (this.GenJournalID != null)
            {
                deletedOid = this.GenJournalID.Oid;
            }
            base.OnDeleting();
        }

        protected override void OnSaving() {
            if (IsDeleted)
            {
                IncomeAndExpense02 incExp = null;
                incExp = this.Session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse(string.Format("[SourceID.Oid] = {0} And [RefID] = {1}", deletedOid, this.Oid)));
                if (incExp != null)
                {
                    incExp.Delete();
                }
            } else
            {
                if (this.GenJournalID!=null)
                {
                    this.GenJournalID.IsIncExpNeedUpdate = true;
                    this.GenJournalID.Save();
                }
            }
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
        private PurchaseOrderDetail _PODetailID;
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
