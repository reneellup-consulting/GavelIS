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
    public class MCheckEffectiveDetails : BaseObject {
        private Guid _RowID;
        private MultiCheckVoucher _MCheckVoucherID;
        private Account _Account;
        private string _Description;
        private bool _Expense;
        private decimal _Amount;
        private decimal _DebitAmount;
        private decimal _CreditAmount;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Association("MultiCheckVoucher-MCheckEffectiveDetails")]
        public MultiCheckVoucher MCheckVoucherID {
            get { return _MCheckVoucherID; }
            set {
                MultiCheckVoucher oldDocumentId = _MCheckVoucherID;
                SetPropertyValue("MCheckVoucherID", ref _MCheckVoucherID, 
                value);
                if (!IsLoading && !IsSaving && oldDocumentId != _MCheckVoucherID)
                {
                    oldDocumentId = oldDocumentId ?? _MCheckVoucherID;
                    try
                    {
                        _MCheckVoucherID.UpdateTotalAmount(true);
                        _MCheckVoucherID.UpdateExpenseAmount(true);
                        _MCheckVoucherID.UpdateTotalNonExpenseAmt(true);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Account Account {
            get { return _Account; }
            set { SetPropertyValue("Account", ref _Account, value); }
        }
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        public bool Expense {
            get { return _Expense; }
            set {
                SetPropertyValue("Expense", ref _Expense, value);
                if (!IsLoading) {
                    try {
                        _MCheckVoucherID.UpdateTotalAmount(true);
                        _MCheckVoucherID.UpdateExpenseAmount(true);
                        _MCheckVoucherID.UpdateTotalNonExpenseAmt(true);
                    } catch (Exception) {}
                }
            }
        }
        public decimal Amount {
            get { return _Amount; }
            set {
                SetPropertyValue("Amount", ref _Amount, value);
                if (!IsLoading) {
                    try {
                        _MCheckVoucherID.UpdateTotalAmount(true);
                        _MCheckVoucherID.UpdateExpenseAmount(true);
                        _MCheckVoucherID.UpdateTotalNonExpenseAmt(true);
                    } catch (Exception) {}
                }
            }
        }
        public decimal DebitAmount {
            get { return _DebitAmount; }
            set { SetPropertyValue("DebitAmount", ref _DebitAmount, value); }
        }
        public decimal CreditAmount {
            get { return _CreditAmount; }
            set { SetPropertyValue("CreditAmount", ref _CreditAmount, value); }
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
        public MCheckEffectiveDetails(Session session): base(session) {
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
