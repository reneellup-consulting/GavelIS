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
    public class StaffPayrollDeduction : BaseObject {
        private StaffPayroll _StaffPayrollID;
        private Employee _Employee;
        private DeductionType _DeductionType;
        private string _DeductionName;
        private MonthsEnum _Month;
        private decimal _Amount;
        private decimal _Balance;
        [Custom("AllowEdit", "False")]
        [Association("StaffPayroll-Deductions")]
        public StaffPayroll StaffPayrollID {
            get { return _StaffPayrollID; }
            set { SetPropertyValue("StaffPayrollID", ref _StaffPayrollID, value)
                ; }
        }
        [Custom("AllowEdit", "False")]
        public int DedId
        {
            get { return _DedId; }
            set { SetPropertyValue("DedId", ref _DedId, value); }
        }
        [Custom("AllowEdit", "False")]
        public string LineID
        {
            get { return _LineID; }
            set { SetPropertyValue("LineID", ref _LineID, value); }
        }
        [Custom("AllowEdit", "False")]
        public string RefNo
        {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee Employee {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DeductionType DeductionType {
            get { return _DeductionType; }
            set { SetPropertyValue("DeductionType", ref _DeductionType, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        public string DeductionName {
            get { return _DeductionName; }
            set { SetPropertyValue("DeductionName", ref _DeductionName, value); 
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Caption
        {
            get { return _Caption; }
            set
            {
                SetPropertyValue("Caption", ref _Caption, value);
            }
        }
        //[Custom("AllowEdit", "False")]
        public MonthsEnum Month {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        [Custom("AllowEdit", "False")]
        public string MonthStr
        {
            get
            {
                switch (_Month)
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
        //[Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Amount {
            get { return _Amount; }
            set {
                SetPropertyValue("Amount", ref _Amount, value);
                if (!IsLoading) {_StaffPayrollID.UpdateDeductionsAmt(true);}
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Balance {
            get { return _Balance; }
            set { SetPropertyValue("Balance", ref _Balance, value); }
        }

        [Custom("DisplayFormat", "n")]
        [DisplayName("Rem. Bal. After")]
        [EditorAlias("LabelDecControlEditor")]
        [PersistentAlias(
        "Balance - Amount"
        )]
        public decimal BalanceTotal
        {
            get
            {
                object tempObject = EvaluateAlias("BalanceTotal");
                if (tempObject != null)
                {
                    if (_Balance > 0)
                    {
                        return (decimal)tempObject;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }
        public StaffPayrollDeduction(Session session): base(session) {
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

        #region Get Current User

        private SecurityUser _CurrentUser;
        private string _LineID;
        private string _RefNo;
        private int _DedId;
        private string _Caption;
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
