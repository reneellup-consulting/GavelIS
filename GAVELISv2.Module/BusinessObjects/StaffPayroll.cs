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
    public class StaffPayroll : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private string _EmployeeName;
        private decimal _RegularPay;
        private decimal _BasicHrs;
        private decimal _BasicAmt;
        private decimal _AbsentHrs;
        private decimal _AbsentAmt;
        private decimal _LateHrs;
        private decimal _LateAmt;
        private decimal _UndertimeHrs;
        private decimal _UndertimeAmt;
        private decimal _DayoffOTHrs;
        private decimal _DayoffOTAmt;
        private decimal _OvertimeHrs;
        private decimal _OvertimeAmt;
        private decimal _NightDiffHrs;
        private decimal _NightDiffAmt;
        private decimal _HolidayHrs;
        private decimal _HolidayAmt;
        private decimal _HolidayHrs2;
        private decimal _HolidayAmt2;
        private decimal _HolidayOTHrs;
        private decimal _HolidayOTAmt;
        private decimal _HolidayOTHrs2;
        private decimal _HolidayOTAmt2;
        [Persistent("AdjustmentsAmt")]
        private decimal? _AdjustmentsAmt;
        private decimal _GrossPay;
        [Persistent("DeductionsAmt")]
        private decimal? _DeductionsAmt;
        private decimal _NetPay;
        private decimal _WHTax;
        private decimal _SSS;
        private MonthsEnum _SSSMonth;
        private decimal _SSSLoan;
        private decimal _PH;
        private decimal _PagIbig;
        private decimal _PagIbigLoan;
        private MonthsEnum _SSS2Month;
        private MonthsEnum _PHMonth;
        private MonthsEnum _PagIbigMonth;
        private MonthsEnum _PagIbig2Month;
        private MonthsEnum _WHTaxMonth;
        private decimal _CashAdvance;
        private bool _Posted = false;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private AttendanceRecord _AttRecId;
        [Custom("AllowEdit", "False")]
        public AttendanceRecord AttRecId
        {
            get { return _AttRecId; }
            set { SetPropertyValue("AttRecId", ref _AttRecId, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("StaffPayroll-Payroll")]
        public GenJournalHeader PayrollBatchID {
            get { return _PayrollBatchID; }
            set { SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value)
                ; }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [NonPersistent]
        public string EmployeeName {
            get {
                if (_Employee != null) {_EmployeeName = _Employee.Name;}
                return _EmployeeName;
            }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal RegularPay {
            get { return _RegularPay; }
            set { SetPropertyValue("RegularPay", ref _RegularPay, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal BasicHrs {
            get { return _BasicHrs; }
            set { SetPropertyValue("BasicHrs", ref _BasicHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal BasicAmt {
            get { return _BasicAmt; }
            set { SetPropertyValue("BasicAmt", ref _BasicAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal AbsentHrs {
            get { return _AbsentHrs; }
            set { SetPropertyValue("AbsentHrs", ref _AbsentHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal AbsentAmt {
            get { return _AbsentAmt; }
            set { SetPropertyValue("AbsentAmt", ref _AbsentAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal LateHrs {
            get { return _LateHrs; }
            set { SetPropertyValue("LateHrs", ref _LateHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal LateAmt {
            get { return _LateAmt; }
            set { SetPropertyValue("LateAmt", ref _LateAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal UndertimeHrs {
            get { return _UndertimeHrs; }
            set { SetPropertyValue("UndertimeHrs", ref _UndertimeHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal UndertimeAmt {
            get { return _UndertimeAmt; }
            set { SetPropertyValue("UndertimeAmt", ref _UndertimeAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal DayoffOTHrs {
            get { return _DayoffOTHrs; }
            set { SetPropertyValue("DayoffOTHrs", ref _DayoffOTHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal DayoffOTAmt {
            get { return _DayoffOTAmt; }
            set { SetPropertyValue("DayoffOTAmt", ref _DayoffOTAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal OvertimeHrs {
            get { return _OvertimeHrs; }
            set { SetPropertyValue("OvertimeHrs", ref _OvertimeHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal OvertimeAmt {
            get { return _OvertimeAmt; }
            set { SetPropertyValue("OvertimeAmt", ref _OvertimeAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal NightDiffHrs {
            get { return _NightDiffHrs; }
            set { SetPropertyValue("NightDiffHrs", ref _NightDiffHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal NightDiffAmt {
            get { return _NightDiffAmt; }
            set { SetPropertyValue("NightDiffAmt", ref _NightDiffAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayHrs {
            get { return _HolidayHrs; }
            set { SetPropertyValue("HolidayHrs", ref _HolidayHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayAmt {
            get { return _HolidayAmt; }
            set { SetPropertyValue("HolidayAmt", ref _HolidayAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayHrs2
        {
            get { return _HolidayHrs2; }
            set { SetPropertyValue("HolidayHrs2", ref _HolidayHrs2, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayAmt2
        {
            get { return _HolidayAmt2; }
            set { SetPropertyValue("HolidayAmt2", ref _HolidayAmt2, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTHrs
        {
            get { return _HolidayOTHrs; }
            set { SetPropertyValue("HolidayOTHrs", ref _HolidayOTHrs, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTAmt
        {
            get { return _HolidayOTAmt; }
            set { SetPropertyValue("HolidayOTAmt", ref _HolidayOTAmt, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTHrs2
        {
            get { return _HolidayOTHrs2; }
            set { SetPropertyValue("HolidayOTHrs2", ref _HolidayOTHrs2, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal HolidayOTAmt2
        {
            get { return _HolidayOTAmt2; }
            set { SetPropertyValue("HolidayOTAmt2", ref _HolidayOTAmt2, value); }
        }
        
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("_AdjustmentsAmt")]
        public decimal? AdjustmentsAmt {
            get {
                try {
                    if (!IsLoading && !IsSaving && _AdjustmentsAmt == null) {
                        UpdateAdjustmentsAmt(false);}
                } catch (Exception) {
                }
                return _AdjustmentsAmt;
            }
        }
        
        [EditorAlias("LabelDecControlEditor")]
        public string AdjustmentsStr {
            get { 
                return _AdjustmentsAmt.Value.ToString("n2"); 
            } 
        }

        public void UpdateAdjustmentsAmt(bool forceChangeEvent) {
            decimal? oldTotal = _AdjustmentsAmt;
            decimal tempTotal = 0m;
            foreach (StaffPayrollAdjustment detail in StaffPayrollAdjustments) {
                tempTotal += detail.Amount;}
            _AdjustmentsAmt = tempTotal;
            if (forceChangeEvent) {OnChanged("AdjustmentsAmt", AdjustmentsAmt, 
                _AdjustmentsAmt);}
            ;
        }
        [Custom("AllowEdit", "False")]
        public decimal PayValue
        {
            get { return _PayValue; }
            set { SetPropertyValue("PayValue", ref _PayValue, value); }
        }
        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias(
        "PayValue + AdjustmentsAmt"
        )]
        public decimal GrossPay {
            get {
                object tempObject = EvaluateAlias("GrossPay");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string GrossPayStr
        {
            get
            {
                return GrossPay.ToString("n2");
            }
        }

        [Custom("DisplayFormat", "n")]
        [PersistentAlias(
        "HolidayAmt+ HolidayAmt2 + HolidayOTAmt + HolidayOTAmt2"
        )]
        public decimal Holiday
        {
            get
            {
                object tempObject = EvaluateAlias("Holiday");
                if (tempObject != null) { return (decimal)tempObject; }
                else
                {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias("_DeductionsAmt")]
        public decimal? DeductionsAmt {
            get {
                try {
                    if (!IsLoading && !IsSaving && _DeductionsAmt == null) {
                        UpdateDeductionsAmt(false);}
                } catch (Exception) {
                }
                return _DeductionsAmt;
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string DeductionsStr
        {
            get
            {
                return _DeductionsAmt.Value.ToString("n2");
            }
        }

        public void UpdateDeductionsAmt(bool forceChangeEvent) {
            decimal? oldTotal = _DeductionsAmt;
            decimal tempTotal = 0m;
            foreach (StaffPayrollDeduction detail in StaffPayrollDeductions) {
                tempTotal += detail.Amount;}
            _DeductionsAmt = tempTotal;
            if (forceChangeEvent) {OnChanged("DeductionsAmt", DeductionsAmt, 
                _DeductionsAmt);}
            ;
        }
        //[Custom("DisplayFormat", "n")]
        //[PersistentAlias(
        //"WHTax + SSS + SSSLoan + PH + PagIbig + PagIbigLoan + CashAdvance")]
        //public decimal DeductionsAmt {
        //    get {
        //        object tempObject = EvaluateAlias("DeductionsAmt");
        //        if (tempObject != null) {return (decimal)tempObject;} else {
        //            return 0;
        //        }
        //    }
        //}
        [Custom("DisplayFormat", "n")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias("GrossPay - DeductionsAmt")]
        public decimal NetPay {
            get {
                object tempObject = EvaluateAlias("NetPay");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }

        [EditorAlias("LabelDecControlEditor")]
        public string NetPayStr
        {
            get
            {
                return NetPay.ToString("n2");
            }
        }

        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal WHTax {
            get { return _WHTax; }
            set { SetPropertyValue("WHTax", ref _WHTax, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal SSS {
            get { return _SSS; }
            set { SetPropertyValue("SSS", ref _SSS, value); }
        }
        [Custom("AllowEdit", "False")]
        public MonthsEnum SSSMonth {
            get { return _SSSMonth; }
            set { SetPropertyValue("SSSMonth", ref _SSSMonth, value); }
        }
        [Custom("AllowEdit", "False")]
        public string SSSMonthStr
        {
            get
            {
                switch (_SSSMonth)
                {
                    case MonthsEnum.None:
                        return "";
                    case MonthsEnum.January:
                        return "JAN";
                    case MonthsEnum.February:
                        return "FEB";
                    case MonthsEnum.March:
                        return "MAR";
                    case MonthsEnum.April:
                        return "APR";
                    case MonthsEnum.May:
                        return "MAY";
                    case MonthsEnum.June:
                        return "JUN";
                    case MonthsEnum.July:
                        return "JUL";
                    case MonthsEnum.August:
                        return "AUG";
                    case MonthsEnum.September:
                        return "SEP";
                    case MonthsEnum.October:
                        return "OCT";
                    case MonthsEnum.November:
                        return "NOV";
                    case MonthsEnum.December:
                        return "DEC";
                    default:
                        return "";
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal SSSLoan {
            get { return _SSSLoan; }
            set { SetPropertyValue("SSSLoan", ref _SSSLoan, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal PH {
            get { return _PH; }
            set { SetPropertyValue("PH", ref _PH, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal PagIbig {
            get { return _PagIbig; }
            set { SetPropertyValue("PagIbig", ref _PagIbig, value); }
        }
        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal PagIbigLoan {
            get { return _PagIbigLoan; }
            set { SetPropertyValue("PagIbigLoan", ref _PagIbigLoan, value); }
        }
        [Custom("AllowEdit", "False")]
        public MonthsEnum SSS2Month {
            get { return _SSS2Month; }
            set { SetPropertyValue("SSS2Month", ref _SSS2Month, value); }
        }
        [Custom("AllowEdit", "False")]
        public string SSS2MonthStr
        {
            get
            {
                switch (_SSS2Month)
                {
                    case MonthsEnum.None:
                        return "";
                    case MonthsEnum.January:
                        return "JAN";
                    case MonthsEnum.February:
                        return "FEB";
                    case MonthsEnum.March:
                        return "MAR";
                    case MonthsEnum.April:
                        return "APR";
                    case MonthsEnum.May:
                        return "MAY";
                    case MonthsEnum.June:
                        return "JUN";
                    case MonthsEnum.July:
                        return "JUL";
                    case MonthsEnum.August:
                        return "AUG";
                    case MonthsEnum.September:
                        return "SEP";
                    case MonthsEnum.October:
                        return "OCT";
                    case MonthsEnum.November:
                        return "NOV";
                    case MonthsEnum.December:
                        return "DEC";
                    default:
                        return "";
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public MonthsEnum PHMonth {
            get { return _PHMonth; }
            set { SetPropertyValue("PHMonth", ref _PHMonth, value); }
        }
        [Custom("AllowEdit", "False")]
        public MonthsEnum PagIbigMonth {
            get { return _PagIbigMonth; }
            set { SetPropertyValue("PagIbigMonth", ref _PagIbigMonth, value); }
        }
        [Custom("AllowEdit", "False")]
        public string PagIbigMonthStr
        {
            get
            {
                switch (_PagIbigMonth)
                {
                    case MonthsEnum.None:
                        return "";
                    case MonthsEnum.January:
                        return "JAN";
                    case MonthsEnum.February:
                        return "FEB";
                    case MonthsEnum.March:
                        return "MAR";
                    case MonthsEnum.April:
                        return "APR";
                    case MonthsEnum.May:
                        return "MAY";
                    case MonthsEnum.June:
                        return "JUN";
                    case MonthsEnum.July:
                        return "JUL";
                    case MonthsEnum.August:
                        return "AUG";
                    case MonthsEnum.September:
                        return "SEP";
                    case MonthsEnum.October:
                        return "OCT";
                    case MonthsEnum.November:
                        return "NOV";
                    case MonthsEnum.December:
                        return "DEC";
                    default:
                        return "";
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public string PHMonthStr
        {
            get
            {
                switch (_PHMonth)
                {
                    case MonthsEnum.None:
                        return "";
                    case MonthsEnum.January:
                        return "JAN";
                    case MonthsEnum.February:
                        return "FEB";
                    case MonthsEnum.March:
                        return "MAR";
                    case MonthsEnum.April:
                        return "APR";
                    case MonthsEnum.May:
                        return "MAY";
                    case MonthsEnum.June:
                        return "JUN";
                    case MonthsEnum.July:
                        return "JUL";
                    case MonthsEnum.August:
                        return "AUG";
                    case MonthsEnum.September:
                        return "SEP";
                    case MonthsEnum.October:
                        return "OCT";
                    case MonthsEnum.November:
                        return "NOV";
                    case MonthsEnum.December:
                        return "DEC";
                    default:
                        return "";
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public MonthsEnum PagIbig2Month {
            get { return _PagIbig2Month; }
            set { SetPropertyValue("PagIbig2Month", ref _PagIbig2Month, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        public string PagIbig2MonthStr
        {
            get
            {
                switch (_PagIbig2Month)
                {
                    case MonthsEnum.None:
                        return "";
                    case MonthsEnum.January:
                        return "JAN";
                    case MonthsEnum.February:
                        return "FEB";
                    case MonthsEnum.March:
                        return "MAR";
                    case MonthsEnum.April:
                        return "APR";
                    case MonthsEnum.May:
                        return "MAY";
                    case MonthsEnum.June:
                        return "JUN";
                    case MonthsEnum.July:
                        return "JUL";
                    case MonthsEnum.August:
                        return "AUG";
                    case MonthsEnum.September:
                        return "SEP";
                    case MonthsEnum.October:
                        return "OCT";
                    case MonthsEnum.November:
                        return "NOV";
                    case MonthsEnum.December:
                        return "DEC";
                    default:
                        return "";
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public MonthsEnum WHTaxMonth {
            get { return _WHTaxMonth; }
            set { SetPropertyValue("WHTaxMonth", ref _WHTaxMonth, value); }
        }
        [Custom("AllowEdit", "False")]
        public string WHTaxMonthStr
        {
            get
            {
                switch (_WHTaxMonth)
                {
                    case MonthsEnum.None:
                        return "";
                    case MonthsEnum.January:
                        return "JAN";
                    case MonthsEnum.February:
                        return "FEB";
                    case MonthsEnum.March:
                        return "MAR";
                    case MonthsEnum.April:
                        return "APR";
                    case MonthsEnum.May:
                        return "MAY";
                    case MonthsEnum.June:
                        return "JUN";
                    case MonthsEnum.July:
                        return "JUL";
                    case MonthsEnum.August:
                        return "AUG";
                    case MonthsEnum.September:
                        return "SEP";
                    case MonthsEnum.October:
                        return "OCT";
                    case MonthsEnum.November:
                        return "NOV";
                    case MonthsEnum.December:
                        return "DEC";
                    default:
                        return "";
                }
            }
        }

        [Custom("DisplayFormat", "n")]
        [Custom("AllowEdit", "False")]
        public decimal CashAdvance {
            get { return _CashAdvance; }
            set { SetPropertyValue("CashAdvance", ref _CashAdvance, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Posted {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
        }
        [NonPersistent]
        public StaffPayrollBatch BatchInfo { get { return (StaffPayrollBatch)
                _PayrollBatchID; } }
        [Aggregated,
        Association("StaffPayroll-Adjustments")]
        public XPCollection<StaffPayrollAdjustment> StaffPayrollAdjustments { 
            get { return GetCollection<StaffPayrollAdjustment>(
                "StaffPayrollAdjustments"); } }
        [Aggregated,
        Association("StaffPayroll-Deductions")]
        public XPCollection<StaffPayrollDeduction> StaffPayrollDeductions { get 
            { return GetCollection<StaffPayrollDeduction>(
                "StaffPayrollDeductions"); } }
        [Aggregated,
        Association("StaffPayroll-PayDetails")]
        public XPCollection<StaffPayrollPayDetail> PayDetails
        {
            get
            {
                return GetCollection<StaffPayrollPayDetail>(
                  "PayDetails");
            }
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
        public StaffPayroll(Session session): base(session) {
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
        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }
        private void Reset() {
            _AdjustmentsAmt = null;
            _DeductionsAmt = null;
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _PayValue;
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
