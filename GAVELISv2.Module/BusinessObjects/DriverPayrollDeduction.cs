using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class DriverPayrollDeduction : XPObject
    {
        private DriverPayroll2 _DriverPayrollID;
        private Employee _Employee;
        private DeductionType _DeductionType;
        private string _DeductionName;
        private MonthsEnum _Month;
        private decimal _Amount;
        private decimal _Balance;
        private string _LineID;
        private string _RefNo;
        private int _DedId;
        private string _Caption;
        private bool _Include;
        private string _SummaryCaption;

        [Custom("AllowEdit", "False")]
        [Association("DriverPayroll2-Deductions")]
        public DriverPayroll2 DriverPayrollID
        {
            get { return _DriverPayrollID; }
            set
            {
                DriverPayroll2 oldDriverPayrollID = _DriverPayrollID;
                SetPropertyValue("DriverPayrollID", ref _DriverPayrollID, value)
                    ;
                if (!IsLoading && !IsSaving && oldDriverPayrollID != _DriverPayrollID)
                {
                    oldDriverPayrollID = oldDriverPayrollID ?? _DriverPayrollID;
                    oldDriverPayrollID.UpdateDeductionsAmt(true);
                }
            }
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
        [Size(1000)]
        public string RefNo
        {
            get { return _RefNo; }
            set { SetPropertyValue("RefNo", ref _RefNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee Employee
        {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DeductionType DeductionType
        {
            get { return _DeductionType; }
            set
            {
                SetPropertyValue("DeductionType", ref _DeductionType, value);
            }
        }
        [Custom("AllowEdit", "False")]
        [Size(1000)]
        public string DeductionName
        {
            get { return _DeductionName; }
            set
            {
                SetPropertyValue("DeductionName", ref _DeductionName, value);
            }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(1000)]
        public string Caption
        {
            get { return _Caption; }
            set
            {
                SetPropertyValue("Caption", ref _Caption, value);
            }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(1000)]
        public string SummaryCaption
        {
            get { return _SummaryCaption; }
            set
            {
                SetPropertyValue("SummaryCaption", ref _SummaryCaption, value);
            }
        }
        //[Custom("AllowEdit", "False")]
        public MonthsEnum Month
        {
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
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                SetPropertyValue("Amount", ref _Amount, value);
                if (!IsLoading && _DriverPayrollID!=null) { _DriverPayrollID.UpdateDeductionsAmt(true); }
            }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Balance
        {
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

        public bool Include
        {
            get { return _Include; }
            set
            {
                SetPropertyValue("Include", ref _Include, value);
                if (!IsLoading && _DriverPayrollID!=null) { _DriverPayrollID.UpdateDeductionsAmt(true); }
            }
        }
        [Action(AutoCommit = true, Caption = "Include")]
        public void MarkAsInclude()
        {
            Include = true;
        }

        [Action(AutoCommit = true, Caption = "Not Included")]
        public void UnMarkInclude()
        {
            Include = false;
        }

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion
        public DriverPayrollDeduction(Session session)
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

        DriverPayroll2 beforeDelId = null;
        decimal beforeDelAmt = 0m;
        protected override void OnDeleting()
        {
            if (this.Include)
            {
                beforeDelId = _DriverPayrollID;
                beforeDelAmt = _Amount;
            }
            base.OnDeleting();
        }
        protected override void OnSaving()
        {
            base.OnSaving();

            // OnSaving create or modify DriverDeductionDetails
            if (!this.IsDeleted)
            {
                DriverDeductionDetails dddts = _DriverPayrollID.DeductionsSummary.Where(o => o.LineCaption == _SummaryCaption).FirstOrDefault();
                if (dddts == null)
                {
                    dddts = ReflectionHelper.CreateObject<DriverDeductionDetails>(Session);
                    dddts.DriverPayrollId = _DriverPayrollID;
                    dddts.LineCaption = _SummaryCaption;
                    dddts.LineAmount = _Amount;
                    //dddts.RemBalance = BalanceTotal;
                    dddts.Save();
                }
                else
                {
                    var sumOfAmt = _DriverPayrollID.DriverPayrollDeductions.Where(o => o.SummaryCaption == _SummaryCaption && o.Include == true).Sum(o => o.Amount);
                    dddts.LineAmount = sumOfAmt;
                    //dddts.RemBalance += BalanceTotal;
                    dddts.Save();
                }
            }
            else
            {
                DriverDeductionDetails dddts = beforeDelId.DeductionsSummary.Where(o => o.LineCaption == _SummaryCaption).FirstOrDefault();
                if (dddts != null)
                {
                    var sumOfAmt = beforeDelId.DriverPayrollDeductions.Where(o => o.SummaryCaption == _SummaryCaption && o.Include == true).Sum(o => o.Amount);
                    dddts.LineAmount = sumOfAmt;
                    //dddts.RemBalance += BalanceTotal;
                    dddts.Save();
                    if (sumOfAmt==0m)
                    {
                        dddts.Delete();
                    }
                }           
            }

            //DriverDeductionDetails dddts = Session.FindObject<DriverDeductionDetails>(CriteriaOperator.Parse("[DriverPayrollId]=? And [LineCaption]=?",_DriverPayrollID,_SummaryCaption));
            
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
