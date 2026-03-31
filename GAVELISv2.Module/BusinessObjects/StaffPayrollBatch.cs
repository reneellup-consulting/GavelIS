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
    public class StaffPayrollBatch : GenJournalHeader {
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
        [RuleRequiredField("", DefaultContexts.Save)]
        public PayrollBatchType BatchType {
            get { return _BatchType; }
            set {
                SetPropertyValue("BatchType", ref _BatchType, value);
                if (!IsLoading && _BatchType != null) {PayrollDate = EntryDate;}
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PayrollDate {
            get { return _PayrollDate; }
            set { SetPropertyValue("PayrollDate", ref _PayrollDate, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PeriodStart {
            get { return _PeriodStart; }
            set {
                SetPropertyValue("PeriodStart", ref _PeriodStart, value);
                if (!IsLoading && _PeriodStart != DateTime.MinValue) {if (
                    _BatchType != null) {PeriodEnd = _PeriodStart.AddDays(
                        _BatchType.DaysCovered);}}
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PeriodEnd {
            get { return _PeriodEnd; }
            set { SetPropertyValue("PeriodEnd", ref _PeriodEnd, value); }
        }
        public PayrollBatchStatusEnum Status {
            get { return _Status; }
            set {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading) {
                    if (_Status != PayrollBatchStatusEnum.Current) {Approved = 
                        true;} else {
                        Approved = false;
                    }
                }
                if (!IsLoading && SecuritySystem.CurrentUser != null) {
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

        [Association]
        public XPCollection<AttendanceRecord> AttendanceRecords
        {
            get { return GetCollection<AttendanceRecord>("AttendanceRecords"); }
        }

        public StaffPayrollBatch(Session session): base(session) {
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
            "Code", "PRLS"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PRL"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new
            BinaryOperator("Code", "PRLS"));
            if (source != null) {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ? source.
                GetNewNo() : null;
                source.Save();
                session.CommitChanges();
            }
        }
        protected override void OnDeleting() { if (Approved) {throw new 
                UserFriendlyException(
                "The system prohibits the deletion of already approved Payroll Batch."
                );} }

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
    }
}
