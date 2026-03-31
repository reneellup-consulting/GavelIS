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
    public class DriverPayrollBatch : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private PayrollBatchType _BatchType;
        private DateTime _PayrollDate;
        private Customer _Customer;
        private DateTime _PeriodStart;
        private DateTime _PeriodEnd;
        private bool _IncludeNoTrip = true;
        private PayrollBatchStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private bool _AllCustomers = false;
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
                if (!IsLoading && _BatchType != null) {
                    Customer = _BatchType.Customer != null ? _BatchType.Customer 
                    : null;
                    PayrollDate = EntryDate;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime PayrollDate {
            get { return _PayrollDate; }
            set { SetPropertyValue("PayrollDate", ref _PayrollDate, value); }
        }
        //[Custom("AllowEdit", "False")]
        //public bool AllCustomers
        //{
        //    get { return _AllCustomers; }
        //    set { SetPropertyValue("AllCustomers", ref _AllCustomers, value);
        //    if (!IsLoading && !IsSaving)
        //    {
        //        Customer = null;
        //    }
        //    }
        //}
        //[RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public Customer Customer {
            get { return _Customer; }
            set { SetPropertyValue("Customer", ref _Customer, value); }
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
        public bool IncludeNoTrip {
            get { return _IncludeNoTrip; }
            set { SetPropertyValue("IncludeNoTrip", ref _IncludeNoTrip, value); 
            }
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
        #region Payroll Batch Associations
        // DriverPayroll -> Generated from Trips, shunting and Kds
        // Deduction -> Generated from Premium Deductions,Loans Deductions and Taxes
        // Deduction(Others) -> Generated from Loan Deductions
        // Adjustments -> Entered adjustments
        #endregion
        public DriverPayrollBatch(Session session): base(session) {
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
            "Code", "PRLD"));
            OperationType = Session.FindObject<OperationType>(new BinaryOperator
            ("Code", "PRL"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new 
            BinaryOperator("Code", "PRLD"));
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
    }
}
