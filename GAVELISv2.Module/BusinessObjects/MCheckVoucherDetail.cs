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
    [NavigationItem(false)]
    //[RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "CheckVoucher, CheckNo")]
    public class MCheckVoucherDetail : BaseObject {
        private Guid _RowID;
        private MultiCheckVoucher _CheckVoucher;
        private Account _BankAccount;
        private string _OldCheckNo;
        private string _CheckNo;
        private DateTime _EntryDate;
        private bool _PostDated;
        private DateTime _CheckDate;
        private decimal _Amount;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("MultiCheckVoucher-MCheckVoucherDetails")]
        public MultiCheckVoucher CheckVoucher {
            get { return _CheckVoucher; }
            set {
                MultiCheckVoucher oldDocumentId = _CheckVoucher;
                SetPropertyValue("CheckVoucher", ref _CheckVoucher, value);
                if (!IsLoading && _CheckVoucher != null) {
                    EntryDate = _CheckVoucher.EntryDate;
                    CheckDate = 
                    _CheckVoucher.EntryDate;}
                if (!IsLoading && !IsSaving && oldDocumentId != _CheckVoucher)
                {
                    oldDocumentId = oldDocumentId ?? _CheckVoucher;
                    try
                    {
                        _CheckVoucher.UpdateTotalAmount(true);
                        _CheckVoucher.UpdateTotalCheckAmt(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Account BankAccount {
            get { return _BankAccount; }
            set {
                SetPropertyValue("BankAccount", ref _BankAccount, value);
                if (!IsLoading && _BankAccount != null) {CheckNo = _BankAccount.
                    GetCheckNo();}
            }
        }

        [Custom("AllowEdit", "False")]
        public string OldCheckNo
        {
            get { return _OldCheckNo; }
            set { SetPropertyValue("OldCheckNo", ref _OldCheckNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string CheckNo {
            get { return _CheckNo; }
            set {
                string oldcno = _CheckNo;
                SetPropertyValue("CheckNo", ref _CheckNo, value);
                if (!IsLoading && !IsSaving)
                {
                    OldCheckNo = oldcno;
                }
            }
        }

        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        public bool PostDated {
            get { return _PostDated; }
            set { SetPropertyValue("PostDated", ref _PostDated, value); }
        }

        public DateTime CheckDate {
            get { return _CheckDate; }
            set { SetPropertyValue("CheckDate", ref _CheckDate, value); }
        }

        public decimal Amount {
            get { return _Amount; }
            set {
                SetPropertyValue("Amount", ref _Amount, value);
                if (!IsLoading) {
                    try {
                        _CheckVoucher.UpdateTotalAmount(true);
                        _CheckVoucher.UpdateTotalCheckAmt(true);
                    } catch(Exception) {
                    }
                }
            }
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

        public MCheckVoucherDetail(Session session): base(session) {
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
            if (SecuritySystem.CurrentUser != null) {
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
            if (SecuritySystem.CurrentUser != null) {
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
