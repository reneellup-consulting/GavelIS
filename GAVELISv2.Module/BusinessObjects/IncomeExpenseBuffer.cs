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
    public class IncomeExpenseBuffer : XPObject
    {
        private int _Seq;
        private string _BufferId;
        private IncomeExpenseReporter _ReporterId;
        private int _Year;
        private Contact _Payee;
        private ContactTypeEnum _PayeeType;
        private ExpenseType _Category;
        private bool _Expense;
        private decimal _January;
        private decimal _JanuaryPrcnt;
        private decimal _February;
        private decimal _FebruaryPrcnt;
        private decimal _March;
        private decimal _MarchPrcnt;
        private decimal _April;
        private decimal _AprilPrcnt;
        private decimal _May;
        private decimal _MayPrcnt;
        private decimal _June;
        private decimal _JunePrcnt;
        private decimal _July;
        private decimal _JulyPrcnt;
        private decimal _August;
        private decimal _AugustPrcnt;
        private decimal _September;
        private decimal _SeptemberPrcnt;
        private decimal _October;
        private decimal _OctobePrcntr;
        private decimal _November;
        private decimal _NovemberPrcnt;
        private decimal _December;
        private decimal _DecemberPrcnt;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string BufferId
        {
            get { return _BufferId; }
            set { SetPropertyValue("BufferId", ref _BufferId, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public int Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        [Custom("AllowEdit", "False")]
        [Association("IncomeExpenseBufferLines")]
        public IncomeExpenseReporter ReporterId
        {
            get { return _ReporterId; }
            set { SetPropertyValue("ReporterId", ref _ReporterId, value); }
        }
        [Custom("AllowEdit", "False")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public Contact Payee
        {
            get { return _Payee; }
            set { SetPropertyValue("Payee", ref _Payee, value); }
        }
        [Custom("AllowEdit", "False")]
        public ContactTypeEnum PayeeType
        {
            get { return _PayeeType; }
            set { SetPropertyValue("PayeeType", ref _PayeeType, value); }
        }
        [Custom("AllowEdit", "False")]
        public ExpenseType Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }
        [Custom("AllowEdit", "False")]
        public bool Expense
        {
            get { return _Expense; }
            set { SetPropertyValue("Expense", ref _Expense, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal January
        {
            get { return _January; }
            set { SetPropertyValue("January", ref _January, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Jan %")]
        public decimal JanuaryPrcnt
        {
            get { return _JanuaryPrcnt; }
            set { SetPropertyValue("JanuaryPrcnt", ref _JanuaryPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal February
        {
            get { return _February; }
            set { SetPropertyValue("February", ref _February, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Feb %")]
        public decimal FebruaryPrcnt
        {
            get { return _FebruaryPrcnt; }
            set { SetPropertyValue("FebruaryPrcnt", ref _FebruaryPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal March
        {
            get { return _March; }
            set { SetPropertyValue("March", ref _March, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Mar %")]
        public decimal MarchPrcnt
        {
            get { return _MarchPrcnt; }
            set { SetPropertyValue("MarchPrcnt", ref _MarchPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal April
        {
            get { return _April; }
            set { SetPropertyValue("April", ref _April, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Apr %")]
        public decimal AprilPrcnt
        {
            get { return _AprilPrcnt; }
            set { SetPropertyValue("AprilPrcnt", ref _AprilPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal May
        {
            get { return _May; }
            set { SetPropertyValue("May", ref _May, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("May %")]
        public decimal MayPrcnt
        {
            get { return _MayPrcnt; }
            set { SetPropertyValue("MayPrcnt", ref _MayPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal June
        {
            get { return _June; }
            set { SetPropertyValue("June", ref _June, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Jun %")]
        public decimal JunePrcnt
        {
            get { return _JunePrcnt; }
            set { SetPropertyValue("JunePrcnt", ref _JunePrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal July
        {
            get { return _July; }
            set { SetPropertyValue("July", ref _July, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Jul %")]
        public decimal JulyPrcnt
        {
            get { return _JulyPrcnt; }
            set { SetPropertyValue("JulyPrcnt", ref _JulyPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal August
        {
            get { return _August; }
            set { SetPropertyValue("August", ref _August, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Aug %")]
        public decimal AugustPrcnt
        {
            get { return _AugustPrcnt; }
            set { SetPropertyValue("AugustPrcnt", ref _AugustPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal September
        {
            get { return _September; }
            set { SetPropertyValue("September", ref _September, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Sep %")]
        public decimal SeptemberPrcnt
        {
            get { return _SeptemberPrcnt; }
            set { SetPropertyValue("SeptemberPrcnt", ref _SeptemberPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal October
        {
            get { return _October; }
            set { SetPropertyValue("October", ref _October, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Oct %")]
        public decimal OctobePrcntr
        {
            get { return _OctobePrcntr; }
            set { SetPropertyValue("OctobePrcntr", ref _OctobePrcntr, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal November
        {
            get { return _November; }
            set { SetPropertyValue("November", ref _November, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Nov %")]
        public decimal NovemberPrcnt
        {
            get { return _NovemberPrcnt; }
            set { SetPropertyValue("NovemberPrcnt", ref _NovemberPrcnt, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal December
        {
            get { return _December; }
            set { SetPropertyValue("December", ref _December, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Dec %")]
        public decimal DecemberPrcnt
        {
            get { return _DecemberPrcnt; }
            set { SetPropertyValue("DecemberPrcnt", ref _DecemberPrcnt, value); }
        }
        [PersistentAlias("January + February + March + April + May + June + July + August + September + October + November + December")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        //jan.Value != 0 ? inc.January / jan.Value * 100 : 0m;
        //[PersistentAlias("Total + 0")]
        //[Custom("DisplayFormat", "n")]
        //[DisplayName("Total %")]
        //public decimal TotalPercent
        //{
        //    get
        //    {
        //        object tempObject = EvaluateAlias("TotalPercent");
        //        if (tempObject != null)
        //        {
        //            return ReporterId.TotalIncome != 0 ? ((decimal)tempObject / ReporterId.TotalIncome) * 100 : 0m;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //        //decimal x = ReporterId.TotalIncome != 0 ? (Total / ReporterId.TotalIncome) * 100 : 0m;
        //        //return x;
        //    }
        //}

        [Custom("AllowEdit", "False")]
        [DisplayName("Total %")]
        public decimal TotalPercent
        {
            get { return _TotalPercent; }
            set { SetPropertyValue("TotalPercent", ref _TotalPercent, value); }
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private decimal _TotalPercent;
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

        public IncomeExpenseBuffer(Session session)
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
        }
    }

}
