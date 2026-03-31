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
    public class DriverPayroll : XPObject {
        private Guid _RowID;
        private GenJournalHeader _PayrollBatchID;
        private Employee _Employee;
        private string _EmployeeName;
        private decimal _Basic;
        private decimal _AdlMiscExp;
        private decimal _MiscExp;
        private decimal _KDs;
        private decimal _Shunting;
        private decimal _Adjustments;
        private decimal _Rescue;
        private decimal _Incentives;
        private string _AdjExplanation;
        private string _RescueExplanation;
        private string _InctvExplanation;
        //private decimal _Gross;
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
        //private decimal _SubTotal;
        private decimal _CA;
        private decimal _HiGasGenset;
        private decimal _HiGasTractor;
        private decimal _Tools;
        private decimal _Damages;
        private decimal _Others;
        private decimal _MiscExpCA;
        private bool _Posted = false;
        //private decimal _TotalCA;
        //private decimal _TotalDed;
        //private decimal _NetPay;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll-Payroll")]
        public GenJournalHeader PayrollBatchID {
            get { return _PayrollBatchID; }
            set { SetPropertyValue("PayrollBatchID", ref _PayrollBatchID, value)
                ; }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
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
        public decimal Basic {
            get { return _Basic; }
            set { SetPropertyValue("Basic", ref _Basic, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal AdlMiscExp {
            get { return _AdlMiscExp; }
            set { SetPropertyValue("AdlMiscExp", ref _AdlMiscExp, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal MiscExp {
            get { return _MiscExp; }
            set { SetPropertyValue("MiscExp", ref _MiscExp, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal KDs {
            get { return _KDs; }
            set { SetPropertyValue("KDs", ref _KDs, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Shunting {
            get { return _Shunting; }
            set { SetPropertyValue("Shunting", ref _Shunting, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Adjustments {
            get { return _Adjustments; }
            set { SetPropertyValue("Adjustments", ref _Adjustments, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Rescue {
            get { return _Rescue; }
            set { SetPropertyValue("Rescue", ref _Rescue, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Incentives {
            get { return _Incentives; }
            set { SetPropertyValue("Incentives", ref _Incentives, value); }
        }
        [Custom("AllowEdit", "False")]
        public string AdjExplanation {
            get { return _AdjExplanation; }
            set { SetPropertyValue("AdjExplanation", ref _AdjExplanation, value)
                ; }
        }
        [Custom("AllowEdit", "False")]
        public string RescueExplanation {
            get { return _RescueExplanation; }
            set { SetPropertyValue("RescueExplanation", ref _RescueExplanation, 
                value); }
        }
        [Custom("AllowEdit", "False")]
        public string InctvExplanation {
            get { return _InctvExplanation; }
            set { SetPropertyValue("InctvExplanation", ref _InctvExplanation, 
                value); }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias(
        "Basic + AdlMiscExp + MiscExp + KDs + Shunting + Adjustments + Rescue + Incentives"
        )]
        public decimal Gross {
            get {
                object tempObject = EvaluateAlias("Gross");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal WHTax {
            get { return _WHTax; }
            set { SetPropertyValue("WHTax", ref _WHTax, value); }
        }
        [Custom("DisplayFormat", "n")]
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
        public decimal SSSLoan {
            get { return _SSSLoan; }
            set { SetPropertyValue("SSSLoan", ref _SSSLoan, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal PH {
            get { return _PH; }
            set { SetPropertyValue("PH", ref _PH, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal PagIbig {
            get { return _PagIbig; }
            set { SetPropertyValue("PagIbig", ref _PagIbig, value); }
        }
        [Custom("DisplayFormat", "n")]
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
        [PersistentAlias("WHTax + SSS + SSSLoan + PH + PagIbig + PagIbigLoan")]
        public decimal SubTotal {
            get {
                object tempObject = EvaluateAlias("SubTotal");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        public decimal CA {
            get { return _CA; }
            set { SetPropertyValue("CA", ref _CA, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal HiGasGenset {
            get { return _HiGasGenset; }
            set { SetPropertyValue("HiGasGenset", ref _HiGasGenset, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal HiGasTractor {
            get { return _HiGasTractor; }
            set { SetPropertyValue("HiGasTractor", ref _HiGasTractor, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Tools {
            get { return _Tools; }
            set { SetPropertyValue("Tools", ref _Tools, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Damages {
            get { return _Damages; }
            set { SetPropertyValue("Damages", ref _Damages, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal Others {
            get { return _Others; }
            set { SetPropertyValue("Others", ref _Others, value); }
        }
        [Custom("DisplayFormat", "n")]
        public decimal MiscExpCA {
            get { return _MiscExpCA; }
            set { SetPropertyValue("MiscExpCA", ref _MiscExpCA, value); }
        }
        public bool Posted {
            get { return _Posted; }
            set { SetPropertyValue("Posted", ref _Posted, value); }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias(
        "CA + HiGasGenset + HiGasTractor + Tools + Damages + Others + MiscExpCA"
        )]
        public decimal TotalCA {
            get {
                object tempObject = EvaluateAlias("TotalCA");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("SubTotal + TotalCA")]
        public decimal TotalDed {
            get {
                object tempObject = EvaluateAlias("TotalDed");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [Custom("DisplayFormat", "n")]
        [PersistentAlias("Gross - TotalDed")]
        public decimal NetPay {
            get {
                object tempObject = EvaluateAlias("NetPay");
                if (tempObject != null) {return (decimal)tempObject;} else {
                    return 0;
                }
            }
        }
        [NonPersistent]
        public DriverPayrollBatch BatchInfo { get { return (DriverPayrollBatch)
                _PayrollBatchID; } }
        [Aggregated,
        Association("DriverPayroll-Trips")]
        public XPCollection<DriverPayrollTripLine> DriverPayrollTripLines { get 
            { return GetCollection<DriverPayrollTripLine>(
                "DriverPayrollTripLines"); } }
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
        public DriverPayroll(Session session): base(session) {
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
