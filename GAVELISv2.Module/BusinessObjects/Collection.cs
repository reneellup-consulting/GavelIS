using System;
using System.Text;
using System.Linq;
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
using System.Collections.Generic;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class Collection : BaseObject {
        private string _DocumentNo;
        private string _Code;
        private PaymentTypeEnum _PaymentMode;
        private DateTime _EntryDate = DateTime.Now;
        private Contact _ReceiveFrom;
        private Account _GetFromAccount;
        private string _Memo;
        private string _Comments;
        private ExpenseType _IncomeType;
        private SubExpenseType _SubIncomeType;
        private CollectionStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        [Persistent("TotalAmount")]
        private decimal? _TotalAmount;
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string DocumentNo {
            get { return _DocumentNo; }
            set { SetPropertyValue("DocumentNo", ref _DocumentNo, value);
            if (!IsLoading) { Code = _DocumentNo; }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        public PaymentTypeEnum PaymentMode
        {
            get { return _PaymentMode; }
            set { SetPropertyValue("PaymentMode", ref _PaymentMode, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Contact ReceiveFrom {
            get { return _ReceiveFrom; }
            set { SetPropertyValue("ReceiveFrom", ref _ReceiveFrom, value); }
        }
        public Account GetFromAccount
        {
            get { return _GetFromAccount; }
            set
            {
                SetPropertyValue("GetFromAccount", ref _GetFromAccount, value)
                    ;
            }
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
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType IncomeType {
            get { return _IncomeType; }
            set {
                SetPropertyValue("IncomeType", ref _IncomeType, value);
                if (!IsLoading) {SubIncomeType = null;}
            }
        }
        [DataSourceProperty("ExpenseType.SubExpenseTypes")]
        public SubExpenseType SubIncomeType {
            get { return _SubIncomeType; }
            set { SetPropertyValue("SubIncomeType", ref _SubIncomeType, value); 
            }
        }
        private bool _Reopened = false;
        [Custom("AllowEdit", "False")]
        public bool Reopened
        {
            get { return _Reopened; }
            set { SetPropertyValue("Reopened", ref _Reopened, value); }
        }
        [Action(Caption = "Re-open")]
        public void ResetCheckVoucher()
        {
            if (XtraMessageBox.Show("Please make sure that all related transactions \ngenerated from previous release has been reset properly. \nDo you want to proceed?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Reopened = true;
                Status = CollectionStatusEnum.Current;
            }
        }
        [Action(Caption = "Close")]
        public void CloseCheckVoucher()
        {
            if (XtraMessageBox.Show("Please make sure that all related transactions \ngenerated from previous release has been reset properly. \nDo you want to proceed?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Reopened = false;
                Status = CollectionStatusEnum.Received;
            }
        }
        [Custom("AllowEdit", "False")]
        public CollectionStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading && SecuritySystem.CurrentUser != null) {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }
        [PersistentAlias("_TotalAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalAmount {
            get {
                try {
                    if (!IsLoading && !IsSaving && _TotalAmount == null) {
                        UpdateTotalAmount(false);
                    }
                } catch (Exception) {
                }
                return _TotalAmount;
            }
        }
        
        #region Check Detail Information

        [NonPersistent]
        public string CheckNos
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (CollectionDetails != null && CollectionDetails.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in CollectionDetails)
                    {
                        if (!strRefs.Contains(item.CheckNo))
                        {
                            strRefs.Add(item.CheckNo);
                            sb.AppendFormat("{0},", item.CheckNo);
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string BankAccts
        {
            get
            {
                StringBuilder sba = new StringBuilder();
                if (CollectionDetails != null && CollectionDetails.Count > 0)
                {
                    List<string> strsba = new List<string>();
                    foreach (var item in CollectionDetails)
                    {
                        if (!strsba.Contains(item.BankAccount.Name))
                        {
                            strsba.Add(item.BankAccount.Name);
                            sba.AppendFormat("{0},", item.BankAccount.Name);
                        }
                    }
                }
                if (sba.Length > 0)
                {
                    sba.Remove(sba.Length - 1, 1);
                }
                return sba.ToString();
            }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string CheckDates
        {
            get
            {
                StringBuilder sbd = new StringBuilder();
                if (CollectionDetails != null && CollectionDetails.Count > 0)
                {
                    List<string> strsbd = new List<string>();
                    foreach (var item in CollectionDetails)
                    {
                        if (!strsbd.Contains(item.CheckDate.ToString("MM/dd/yyyy")))
                        {
                            strsbd.Add(item.CheckDate.ToString("MM/dd/yyyy"));
                            sbd.AppendFormat("{0},", item.CheckDate.ToString("MM/dd/yyyy"));
                        }
                    }
                }
                if (sbd.Length > 0)
                {
                    sbd.Remove(sbd.Length - 1, 1);
                }
                return sbd.ToString();
            }
        }

        #endregion
        
        public void UpdateTotalAmount(bool forceChangeEvent) {
            decimal? oldTotalAmount = _TotalAmount;
            decimal tempTotal = 0m;
            foreach (CollectionDetail detail in CollectionDetails) {tempTotal += 
                detail.Amount;}
            _TotalAmount = tempTotal;
            if (forceChangeEvent) {OnChanged("TotalAmount", TotalAmount, 
                _TotalAmount);}
            ;
        }
        [Aggregated,
        Association("Collection-Details")]
        public XPCollection<CollectionDetail> CollectionDetails { get { return 
                GetCollection<CollectionDetail>("CollectionDetails"); } }
        public Collection(Session session): base(session) {
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
        }
        protected override void OnDeleting() { if (_Status != 
            CollectionStatusEnum.Current) {throw new UserFriendlyException(
                "The system prohibits the deletion of already received Collection Document."
                );} }
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() { _TotalAmount = null; }

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

        [NonPersistent]
        public Company CompanyInfo
        {
            get { return Company.GetInstance(Session); }
        }

    }
}
