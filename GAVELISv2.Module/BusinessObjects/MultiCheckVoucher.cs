using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
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
namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class MultiCheckVoucher : BaseObject
    {
        private string _CheckNos;
        private string _CheckVoucherNo;
        private DateTime _EntryDate = DateTime.Now;
        private Contact _Payee;
        private string _Memo;
        private string _Comments;
        private MultiCheckStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        [Persistent("TotalAmount")]
        private decimal? _TotalAmount;
        [Persistent("ExpenseAmount")]
        private decimal? _ExpenseAmount;
        private SourceType _SourceType;
        private OperationType _OperationType;
        private bool _Reopened = false;
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        //[Custom("AllowEdit", "False")]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string CheckVoucherNo
        {
            get { return _CheckVoucherNo; }
            set
            {
                SetPropertyValue("CheckVoucherNo", ref _CheckVoucherNo, value)
                    ;
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Contact Payee
        {
            get { return _Payee; }
            set { SetPropertyValue("Payee", ref _Payee, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo
        {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        [Size(500)]
        public string Comments
        {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }

        [Custom("AllowEdit", "False")]
        public MultiCheckStatusEnum Status
        {
            get { return _Status; }
            set
            {
                if (!IsLoading && value == MultiCheckStatusEnum.Approved)
                {
                    if (_Printed != true && !_Reopened)
                    {
                        throw new UserFriendlyException("Check Voucher is not yet printed");
                    }
                    if (_Reopened)
                    {
                        throw new UserFriendlyException("Reopened Check Voucher cannot be approved. Close it instead.");
                    }
                    //if (_TotalCheckAmt.Value != _TotalPayDetailsAmt.Value && !_Reopened)
                    //{
                    //    throw new UserFriendlyException("Check Voucher Total Amount is not equal to the Total Check Amount");
                    //}
                }
                SetPropertyValue("Status", ref _Status, value);
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

        [Custom("AllowEdit", "False")]
        public string StatusBy
        {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        [Custom("AllowEdit", "False")]
        public DateTime StatusDate
        {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        private string _cachedState;
        [Persistent]
        [Custom("AllowEdit", "False")]
        public string State
        {
            get
            {
                if (string.IsNullOrEmpty(_cachedState))
                    UpdateStateCache();
                return _cachedState;
            }
        }

        public void UpdateStateCache()
        {
            switch (_Status)
            {
                case MultiCheckStatusEnum.Current:
                    _cachedState = "CURRENT";
                    break;
                case MultiCheckStatusEnum.Approved:
                    _cachedState = "APPROVED";
                    break;
                case MultiCheckStatusEnum.Released:
                    _cachedState = ComputeReleasedState();
                    break;
                default:
                    _cachedState = string.Empty;
                    break;
            }
        }

        private string ComputeReleasedState()
        {
            if (MCheckVoucherDetails.Count == 0 || MCheckVoucherDetails[0].BankAccount == null)
                return "RELEASED";

            decimal reconciledAmt = 0;
            bool isCancelled = false;

            // Use separate UnitOfWork to avoid reentrancy
            using (UnitOfWork uow = new UnitOfWork(Session.DataLayer))
            {
                string query = string.Format("select * from CheckPayment where BankCashAccount='{0}' and CheckNo='{1}'", this.MCheckVoucherDetails[0].BankAccount.Oid, this.MCheckVoucherDetails[0].CheckNo);

                var results = uow.ExecuteQuery(query);
        
                foreach (var row in results.ResultSet[0].Rows)
                {
                    int paymentId = (int)row.Values[0];
                    CheckPayment payment = uow.GetObjectByKey<CheckPayment>(paymentId);

                    if (payment.Status == CheckStatusEnum.Voided)
                    {
                        isCancelled = true;
                    }
                    else if (payment.CRAdjusted > 0)
                    {
                        reconciledAmt += payment.CRAdjusted;
                    }
                }
            }

            if (isCancelled) return "CANCELLED";
            if (reconciledAmt > 0)
            {
                if (TotalCheckAmt == reconciledAmt) return "RECONCILED";
                if (TotalCheckAmt > reconciledAmt) return "PARTIALLY RECONCILED";
            }
            return "RELEASED";
        }
        //[Persistent]
        //[Custom("AllowEdit", "False")]
        //public string State
        //{
        //    get {
        //        switch (_Status)
        //        {
        //            case MultiCheckStatusEnum.Current:
        //                return "CURRENT";
        //            case MultiCheckStatusEnum.Approved:
        //                return "APPROVED";
        //            case MultiCheckStatusEnum.Released:
        //                decimal amt = 0;
        //                if (this.MCheckVoucherDetails.Count > 0 && this.MCheckVoucherDetails[0].BankAccount != null)
        //                {
        //                    var results = Session.ExecuteQuery(string.Format("select * from CheckPayment where BankCashAccount='{0}' and CheckNo='{1}'", this.MCheckVoucherDetails[0].BankAccount.Oid, this.MCheckVoucherDetails[0].CheckNo));
        //                    foreach (var item in results.ResultSet[0].Rows)
        //                    {
        //                        // TODO: Create new ObjectSpace from Session
        //                        // then instead below:
        //                        // CheckPayment cpmmt = NewObjectSpace.GetObjectByKey<CheckPayment>(item.Values[0]);
        //                        CheckPayment cpmmt = Session.GetObjectByKey<CheckPayment>(item.Values[0]);
        //                        if (cpmmt.Status == CheckStatusEnum.Voided)
        //                        {
        //                            return "CANCELLED";
        //                        }
        //                        if (cpmmt.CRAdjusted > 0)
        //                        {
        //                            amt += cpmmt.CRAdjusted;
        //                        }
        //                    }
        //                    if (amt > 0 && this.TotalCheckAmt == amt)
        //                    {
        //                        return "RECONCILED";
        //                    }
        //                    if (amt > 0 && this.TotalCheckAmt > 0 && this.TotalCheckAmt > amt)
        //                    {
        //                        return "PARTIALLY RECONCILED";
        //                    }
        //                }
        //                return "RELEASED";
        //            default:
        //                break;
        //        }
        //        return string.Empty; }
        //}
        [PersistentAlias("_TotalAmount")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalAmount
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalAmount == null)
                    {
                        UpdateTotalAmount(false);
                    }
                }
                catch (Exception)
                {
                }
                return _TotalAmount;
            }
        }
        [NonPersistent]
        public string CheckNos
        {
            get {
                StringBuilder sb = new StringBuilder();
                if (MCheckVoucherDetails != null && MCheckVoucherDetails.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in MCheckVoucherDetails)
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
        private string _BankAccts;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string BankAccts
        {
            get
            {
                StringBuilder sba = new StringBuilder();
                if (MCheckVoucherDetails != null && MCheckVoucherDetails.Count > 0)
                {
                    List<string> strsba = new List<string>();
                    foreach (var item in MCheckVoucherDetails)
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
        
        private string _CheckDates;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string CheckDates
        {
            get
            {
                StringBuilder sbd = new StringBuilder();
                if (MCheckVoucherDetails != null && MCheckVoucherDetails.Count > 0)
                {
                    List<string> strsbd = new List<string>();
                    foreach (var item in MCheckVoucherDetails)
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
        
        public void UpdateTotalAmount(bool forceChangeEvent) {
            decimal? oldTotalAmount = _TotalAmount;
            decimal tempTotal = 0m;
            decimal tempTotal2 = 0m;
            foreach (MCheckVoucherDetail detail in MCheckVoucherDetails)
            {
                tempTotal += detail.Amount;
            }
            foreach (MCheckEffectiveDetails detail in MCheckEffectiveDetails)
            {
                tempTotal2 += !detail.Expense ? detail.Amount : 0;
            }
            _TotalAmount = tempTotal + tempTotal2;
            if (forceChangeEvent)
            {
                OnChanged("TotalAmount", TotalAmount,
                _TotalAmount);
            }
            ;
        }

        [PersistentAlias("_ExpenseAmount")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("Total Expense Amount")]
        public decimal? ExpenseAmount {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _ExpenseAmount == null)
                    {
                        UpdateExpenseAmount(false);
                    }
                } catch (Exception)
                {
                }
                return _ExpenseAmount;
            }
        }

        public SourceType SourceType {
            get { return _SourceType; }
            set { SetPropertyValue<SourceType>("SourceType", ref _SourceType, value); }
        }

        public OperationType OperationType {
            get { return _OperationType; }
            set { SetPropertyValue<OperationType>("OperationType", ref _OperationType, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Reopened
        {
            get { return _Reopened; }
            set { SetPropertyValue("Reopened", ref _Reopened, value); }
        }
        
        public void UpdateExpenseAmount(bool forceChangeEvent) {
            decimal? oldExpenseAmount = _ExpenseAmount;
            decimal tempTotal = 0m;
            foreach (MCheckEffectiveDetails detail in MCheckEffectiveDetails)
            {
                tempTotal += detail.Expense ? detail.Amount : 0;
            }
            _ExpenseAmount = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("ExpenseAmount", ExpenseAmount,
                _ExpenseAmount);
            }
            ;
        }


        #region Multi-Check Voucher Printout

        [Persistent("TotalCheckAmt")]
        private decimal? _TotalCheckAmt;
        [PersistentAlias("_TotalCheckAmt")]
        public decimal? TotalCheckAmt {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalCheckAmt == null)
                    {
                        UpdateTotalCheckAmt(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalCheckAmt;
            }
        }

        public void UpdateTotalCheckAmt(bool forceChangeEvent) {
            decimal? oldTotalCheckAmt = _TotalCheckAmt;
            decimal tempTotal = 0m;
            foreach (MCheckVoucherDetail detail in MCheckVoucherDetails)
            {
                tempTotal += detail.Amount;
            }
            _TotalCheckAmt = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalCheckAmt", TotalCheckAmt, _TotalCheckAmt);
            }
            ;
        }

        [Persistent("TotalNonExpenseAmt")]
        private decimal? _TotalNonExpenseAmt;
        [PersistentAlias("_TotalNonExpenseAmt")]
        public decimal? TotalNonExpenseAmt {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalNonExpenseAmt == null)
                    {
                        UpdateTotalNonExpenseAmt(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalNonExpenseAmt;
            }
        }

        public void UpdateTotalNonExpenseAmt(bool forceChangeEvent) {
            decimal? oldTotalNonExpenseAmt = _TotalNonExpenseAmt;
            decimal tempTotal = 0m;
            foreach (MCheckEffectiveDetails detail in MCheckEffectiveDetails)
            {
                tempTotal += detail.Expense != true ? detail.Amount : 0;
            }
            _TotalNonExpenseAmt = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalNonExpenseAmt", TotalNonExpenseAmt, _TotalNonExpenseAmt);
            }
            ;
        }

        [Persistent("TotalPayDetailsAmt")]
        private decimal? _TotalPayDetailsAmt;
        [PersistentAlias("_TotalPayDetailsAmt")]
        public decimal? TotalPayDetailsAmt {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalPayDetailsAmt == null)
                    {
                        UpdateTotalPayDetailsAmt(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalPayDetailsAmt;
            }
        }

        public void UpdateTotalPayDetailsAmt(bool forceChangeEvent) {
            decimal? oldTotalPayDetailsAmt = _TotalPayDetailsAmt;
            decimal tempTotal = 0m;
            foreach (MultiCheckVouchPayDetail detail in MultiCheckVouchPayDetails)
            {
                tempTotal += detail.Amount;
            }
            _TotalPayDetailsAmt = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalPayDetailsAmt", TotalPayDetailsAmt, _TotalPayDetailsAmt);
            }
            ;
        }

        [Custom("AllowEdit", "False")]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string CheckTotalToWord {
            get
            {
                //return NumToWord.NumWords(Convert.ToDouble(TotalCheckAmt.Value));
                return NumToWord.NumberToCurrencyText(TotalCheckAmt.Value, MidpointRounding.AwayFromZero);
            }
        }

        private void GeneratePayDetails() {
            try
            {
                for (int i = this.MultiCheckVouchPayDetails.Count - 1; i >= 0; i--)
                {
                    this.MultiCheckVouchPayDetails[i].Delete();
                }
            } catch (Exception)
            {
            }
            IObjectSpace objs = ObjectSpace.FindObjectSpaceByObject(Session);
            // Memo ***********         xxx.xx <- TotalAmount
            MultiCheckVouchPayDetail mcpd = objs.CreateObject<MultiCheckVouchPayDetail>();
            mcpd.MCheckVoucherID = this;
            mcpd.Particulars = this.Memo;
            mcpd.Amount = this.TotalAmount.Value;
            mcpd.Save();
            foreach (MCheckEffectiveDetails item in this.MCheckEffectiveDetails)
            {
                if (item.Amount == this.TotalCheckAmt)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(item.Account.MultiCVPayDetCaption))
                {
                    throw new ApplicationException("A payment detail caption in one of the effective expense account has not been set");
                }
                MultiCheckVouchPayDetail mcpd1 = objs.CreateObject<MultiCheckVouchPayDetail>();
                mcpd1.MCheckVoucherID = this;
                mcpd1.Particulars = item.Account.MultiCVPayDetCaption;
                if (item.Account != null && item.Account.LessInMultiCV)
                {
                    // If (Account.IsLessInMCV != false)
                    // Account.MCVCaption       xxx.xx
                    mcpd1.Amount = 0 - item.Amount;
                } else
                {
                    // Else
                    // Account.MCVCaption      (xxx.xx)
                    mcpd1.Amount = item.Amount;
                }
                mcpd1.Save();
            }
            this.Save();
            objs.CommitChanges();
            //ObjectSpace.CommitChanges();
        }

        private bool _Printed;
        [Custom("AllowEdit", "False")]
        public bool Printed {
            get { return _Printed; }
            set { SetPropertyValue<bool>("Printed", ref _Printed, value); }
        }

        // [Reopened] = True And [Comments] = 'dd'
        //private bool _Printing = false;
        //[NonPersistent]
        //[Custom("AllowEdit", "False")]
        //public bool Printing {
        //    get { return _Printing; }
        //    set { SetPropertyValue<bool>("Printing", ref _Printing, value); }
        //}
        [Action(Caption = "Re-open Voucher")]
        public void ResetCheckVoucher()
        {
            if (XtraMessageBox.Show("Please make sure that all related transactions \ngenerated from previous release has been reset properly. \nDo you want to proceed?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                Reopened = true;
                Printed = false;
                Status = MultiCheckStatusEnum.Current;
            }
        }
        //[Action(Caption = "Close Voucher")]
        //public void CloseCheckVoucher()
        //{
        //    if (XtraMessageBox.Show("Please make sure that all related transactions \ngenerated from previous release has been reset properly. \nDo you want to proceed?", "Confirm", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
        //    {
        //        Reopened = false;
        //        Status = MultiCheckStatusEnum.Released;
        //    }
        //}
        [Action(Caption = "Print Voucher")]
        public void PrintVoucher() {
            // Generate Pay Details
            GeneratePayDetails();
            if (Status != MultiCheckStatusEnum.Current)
            {
                throw new ApplicationException("Cannot print already approved or released Check Voucher");
            }
            if (_Printed)
            {
                throw new ApplicationException("Voucher already printed");
            }
            //Printed = true;
            //this.Session.CommitTransaction();
            string path = Directory.GetCurrentDirectory() + @"\MultiCheckVoucher.repx";
            XafReport rep = new XafReport();
            IObjectSpace objs = ObjectSpace.FindObjectSpaceByObject(Session);
            rep.LoadLayout(path);
            rep.ObjectSpace = objs;
            XPCollection<MultiCheckVoucher> xpc = new XPCollection<MultiCheckVoucher>(Session) { LoadingEnabled = true
            };
            xpc.Filter = new InOperator(rep.ObjectSpace.GetKeyPropertyName(typeof(MultiCheckVoucher)), new string[] { this.Oid.ToString()
            });
            //xpc.Add(this);
            //System.Collections.Generic.IList<MultiCheckVoucher> rds = rep.ObjectSpace.GetObjects<MultiCheckVoucher>(CriteriaOperator.Parse(string.Format("[Oid] = '{0}'", this.Oid)));
            //XPDataView xpdbv = new XPDataView();
            //string sql = string.Format("select Oid, EntryDate,Payee from MultiCheckVoucher where Oid = '{0}'", this.Oid);
            //DevExpress.Xpo.DB.SelectedData selectedData = this.Session.ExecuteQuery(sql);
            //xpdbv.AddProperty("Oid", typeof(Guid));
            //xpdbv.AddProperty("EntryDate", typeof(DateTime));
            //xpdbv.AddProperty("Payee", typeof(Contact));
            //xpdbv.LoadData(selectedData);
            rep.DataSource = xpc;
            rep.PrintingSystem.StartPrint += new DevExpress.XtraPrinting.PrintDocumentEventHandler(PrintingSystem_StartPrint);
            rep.ShowPreview();
            objs.CommitChanges();
        }

        void PrintingSystem_StartPrint(object sender, DevExpress.XtraPrinting.PrintDocumentEventArgs e) {
            e.PrintDocument.PrinterSettings.Copies = 2;
            e.PrintDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(PrintDocument_EndPrint);
        }

        void PrintDocument_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
            IObjectSpace objs = ObjectSpace.FindObjectSpaceByObject(Session);
            Printed = true;
            objs.CommitChanges();
            //this.Session.CommitTransaction();
        }

        #endregion

        // MCheckVoucherDetail
        [Aggregated,
        Association("MultiCheckVoucher-MCheckVoucherDetails")]
        public XPCollection<MCheckVoucherDetail> MCheckVoucherDetails {
            get { return GetCollection<MCheckVoucherDetail>("MCheckVoucherDetails"
                ); }
        }

        [Aggregated,
        Association("MultiCheckVoucher-MCheckEffectiveDetails")]
        public XPCollection<MCheckEffectiveDetails> MCheckEffectiveDetails {
            get { return GetCollection<MCheckEffectiveDetails>(
                "MCheckEffectiveDetails"); }
        }

        // EffectiveAccounts (CheckPayeeDetail) if Payee==typeof Payee
        [Aggregated,
        Association("MultiCheckVoucher-MCheckPayeeDetails")]
        public XPCollection<MCheckPayeeDetail> MCheckPayeeDetails {
            get { return
                GetCollection<MCheckPayeeDetail>("MCheckPayeeDetails"); }
        }

        [Aggregated,
        Association("MultiCheckVoucher-MultiCheckVouchPayDetails")]
        public XPCollection<MultiCheckVouchPayDetail> MultiCheckVouchPayDetails {
            get { return
                GetCollection<MultiCheckVouchPayDetail>("MultiCheckVouchPayDetails"); }
        }



        private DateTime _FromPeriod;
        private DateTime _ToPeriod;
        private MonthOfTheQuarterEnum _MonthOfTheQuarter;

        public DateTime FromPeriod {
            get { return _FromPeriod; }
            set { SetPropertyValue<DateTime>("FromPeriod", ref _FromPeriod, value); }
        }

        public DateTime ToPeriod {
            get { return _ToPeriod; }
            set { SetPropertyValue<DateTime>("ToPeriod", ref _ToPeriod, value); }
        }

        public MonthOfTheQuarterEnum MonthOfTheQuarter {
            get { return _MonthOfTheQuarter; }
            set { SetPropertyValue<MonthOfTheQuarterEnum>("MonthOfTheQuarter", ref _MonthOfTheQuarter, value); }
        }

        public string FromMonth {
            get { return _FromPeriod.ToString("MM"); }
        }

        public string FromDate {
            get { return _FromPeriod.ToString("dd"); }
        }

        public string FromYear {
            get { return _FromPeriod.ToString("yy"); }
        }

        public string ToMonth {
            get { return _ToPeriod.ToString("MM"); }
        }

        public string ToDate {
            get { return _ToPeriod.ToString("dd"); }
        }

        public string ToYear {
            get { return _ToPeriod.ToString("yy"); }
        }

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        //[System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        //[System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        //[System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        //[System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        public MultiCheckVoucher(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "MCV"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "MCV"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "MCV"));
            if (source != null)
            {
                CheckVoucherNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
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

        protected override void OnSaving()
        {
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
        protected override void OnDeleting() {
            if (_Status !=
            MultiCheckStatusEnum.Current)
            {
                throw new UserFriendlyException(
                "The system prohibits the deletion of already approved or released Check Voucher."
                );
            }
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            _TotalAmount = null;
            _ExpenseAmount = null;
            _TotalCheckAmt = null;
            _TotalNonExpenseAmt = null;
            _TotalPayDetailsAmt = null;
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
