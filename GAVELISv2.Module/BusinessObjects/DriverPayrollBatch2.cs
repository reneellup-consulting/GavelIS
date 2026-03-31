using System;
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
using DevExpress.Xpo.DB;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class DriverPayrollBatch2 : GenJournalHeader
    {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private PayrollBatchType _BatchType;
        private DateTime _PayrollDate;
        private DateTime _PeriodStart;
        private DateTime _PeriodEnd;
        private PayrollBatchStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        public string ReferenceNo
        {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
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
        [RuleRequiredField("", DefaultContexts.Save)]
        public PayrollBatchType BatchType
        {
            get { return _BatchType; }
            set
            {
                SetPropertyValue("BatchType", ref _BatchType, value);
                if (!IsLoading && _BatchType != null) { PayrollDate = EntryDate; }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PayrollDate
        {
            get { return _PayrollDate; }
            set { SetPropertyValue("PayrollDate", ref _PayrollDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PeriodStart
        {
            get { return _PeriodStart; }
            set
            {
                SetPropertyValue("PeriodStart", ref _PeriodStart, value);
                if (!IsLoading && _PeriodStart != DateTime.MinValue)
                {
                    if (
                        _BatchType != null)
                    {
                        PeriodEnd = _PeriodStart.AddDays(
                            _BatchType.DaysCovered);
                    }
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PeriodEnd
        {
            get { return _PeriodEnd; }
            set { SetPropertyValue("PeriodEnd", ref _PeriodEnd, value); }
        }
        public PayrollBatchStatusEnum Status
        {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != PayrollBatchStatusEnum.Current)
                    {
                        Approved =
                            true;
                    }
                    else
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
        public string StatusBy
        {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }
        public DateTime StatusDate
        {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        // Batch Total
        #region Batch Total

        [Persistent("BatchTotal")]
        private decimal? _BatchTotal;
        [DisplayName("Batch Total")]
        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias("_BatchTotal")]
        public decimal? BatchTotal
        {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _BatchTotal == null)
                    {
                        UpdateBatchTotal(false);
                    }
                }
                catch (Exception)
                {
                }
                return _BatchTotal;
            }
        }

        public void UpdateBatchTotal(bool forceChangeEvent)
        {
            decimal? oldTotal = _BatchTotal;
            decimal tempTotal = 0m;
            foreach (DriverPayroll2 detail in DriverPayrolls2)
            {
                tempTotal += detail.NetPay;
            }
            _BatchTotal = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("BatchTotal", BatchTotal,
                    _BatchTotal);
            }
            ;
        }

        //select PayrollBatchID, sum(PayValue + AdjustmentsAmt) - sum(DeductionsAmt) as BatchTotal from DriverPayroll2 where PayrollBatchID=439193 group by PayrollBatchID
        public decimal BatchTotal2 {
            get {
                decimal x = 0;
                string qry = string.Format("select PayrollBatchID, sum(PayValue + AdjustmentsAmt) - sum(DeductionsAmt) " + 
                    "as BatchTotal from DriverPayroll2 where PayrollBatchID={0} group by PayrollBatchID", this.Oid);
                SelectedData data = Session.ExecuteQuery(qry);
                bool notNull = data != null;
                decimal cnt1 = data.ResultSet[0].Rows.Count();
                if (data != null && data.ResultSet[0].Rows.Count() != 0)
                {
                    var sel = data.ResultSet[0].Rows[0];
                    if (sel.Values[1] != null)
                    {
                        x = Convert.ToDecimal(sel.Values[1].ToString());
                    }
                }
                return x; }
        }

        #endregion

        // DriverPayroll2

        public DriverPayrollBatch2(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            SourceType = Session.FindObject<SourceType>(new BinaryOperator(
            "Code", "PRLD"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PRL"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "PRLD"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
        }

        protected override void OnDeleting()
        {
            if (Approved)
            {
                throw new
                    UserFriendlyException(
                    "The system prohibits the deletion of already approved Payroll Batch."
                    );
            }
        }

        private bool _Processed = false;
        [Custom("AllowEdit", "False")]
        public bool Processed
        {
            get { return _Processed; }
            set
            {
                SetPropertyValue("Processed", ref _Processed, value);
            }
        }
        [Action(AutoCommit = true, Caption = "Unmark as Processed")]
        public void UnmarkAsProcessed()
        {
            Processed = false;
        }
        protected override void OnLoaded()
        {
            Reset();
            base.OnLoaded();
        }
        private void Reset()
        {
            _BatchTotal = null;
        }
    }

}
