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
    [NavigationItem(false)]
    public class CollectionDetail : BaseObject {
        private Guid _RowID;
        private Collection _DocumentNo;
        private Account _BankAccount;
        private string _CheckNo;
        private string _RefNo;
        private string _BankBranch;
        private DateTime _CheckDate = DateTime.Now;
        private ExpenseType _IncomeType;
        private SubExpenseType _SubIncomeType;
        private Account _OutputTaxAcct;
        private decimal _Withheld;
        private decimal _Amount;
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("Collection-Details")]
        public Collection DocumentNo {
            get { return _DocumentNo; }
            set
            {
                SetPropertyValue("DocumentNo", ref _DocumentNo, value);
                if (!IsLoading && _DocumentNo != null)
                {
                    IncomeType = _DocumentNo.IncomeType != null ? _DocumentNo.IncomeType : null;
                    SubIncomeType = _DocumentNo.SubIncomeType != null ? _DocumentNo.SubIncomeType : null;
                    OutputTaxAcct = _DocumentNo.CompanyInfo.OutputVATAcct ?? null;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account BankAccount {
            get { return _BankAccount; }
            set { SetPropertyValue("BankAccount", ref _BankAccount, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string CheckNo {
            get { return _CheckNo; }
            set { SetPropertyValue("CheckNo", ref _CheckNo, value); }
        }

        public string RefNo {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }

        public bool PostDated
        {
            get { return _PostDated; }
            set { SetPropertyValue("PostDated", ref _PostDated, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Bank Name/Branch")]
        public string BankBranch {
            get { return _BankBranch; }
            set { SetPropertyValue("BankBranch", ref _BankBranch, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime CheckDate {
            get { return _CheckDate; }
            set { SetPropertyValue("CheckDate", ref _CheckDate, value); }
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
            set { SetPropertyValue("SubIncomeType", ref _SubIncomeType, value); }
        }

        public Account OutputTaxAcct {
            get { return _OutputTaxAcct; }
            set { SetPropertyValue<Account>("OutputTaxAcct", ref _OutputTaxAcct, value); }
        }

        public decimal Withheld {
            get { return _Withheld; }
            set { SetPropertyValue<decimal>("Withheld", ref _Withheld, value); }
        }

        [DisplayName("Check Amount")]
        public decimal Amount {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
                if (!IsLoading)
                {
                    try
                    {
                        _DocumentNo.UpdateTotalAmount(true);
                    } catch (Exception)
                    {
                    }
                }
            }
        }

        [PersistentAlias("Withheld + Amount")]
        [Custom("DisplayFormat", "n")]
        public decimal LineAmount {
            get
            {
                object tempObject = EvaluateAlias("LineAmount");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        public CollectionDetail(Session session)
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
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private bool _PostDated;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser {
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
