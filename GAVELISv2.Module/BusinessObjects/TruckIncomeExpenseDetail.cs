using System;
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
    public class TruckIncomeExpenseDetail : XPObject
    {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private TruckIncomeExpense _MainId;
        [Custom("AllowEdit", "False")]
        [Association("TruckIncomeExpenseDetails")]
        public TruckIncomeExpense MainId
        {
            get { return _MainId; }
            set { SetPropertyValue("MainId", ref _MainId, value); }
        }
        private FixedAsset _Unit;
        private int _Year;
        private MonthsEnum _Month;
        private decimal _Income;
        private decimal _Expense01;
        private decimal _Percent01;
        private decimal _Expense02;
        private decimal _Percent02;
        private decimal _Expense03;
        private decimal _Percent03;
        private decimal _Expense04;
        private decimal _Percent04;
        private decimal _Expense05;
        private decimal _Percent05;
        private decimal _Expense06;
        private decimal _Percent06;
        private decimal _Expense07;
        private decimal _Percent07;
        private decimal _Expense08;
        private decimal _Percent08;
        private decimal _Expense09;
        private decimal _Percent09;
        private decimal _Expense10;
        private decimal _Percent10;
        private decimal _Expense11;
        private decimal _Percent11;
        private decimal _Expense12;
        private decimal _Percent12;
        private decimal _Expense13;
        private decimal _Percent13;
        private decimal _Expense14;
        private decimal _Percent14;
        private decimal _Expense15;
        private decimal _Percent15;
        private decimal _Expense16;
        private decimal _Percent16;
        private decimal _Expense17;
        private decimal _Percent17;
        private decimal _OtherExpenses;
        private decimal _PercentOExp;
        private decimal _TotalExpenses;
        private decimal _TotalExpPercent;
        private decimal _NetIncomeLoss;
        private decimal _NetPercent;
        [Custom("AllowEdit", "False")]
        [DisplayName("UNIT")]
        public FixedAsset Unit
        {
            get { return _Unit; }
            set { SetPropertyValue("Unit", ref _Unit, value); }
        }
        [DisplayName("YEAR")]
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        [DisplayName("MONTH")]
        [Custom("AllowEdit", "False")]
        public MonthsEnum Month
        {
            get { return _Month; }
            set { SetPropertyValue("Month", ref _Month, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("INCOME")]
        public decimal Income
        {
            get { return _Income; }
            set { SetPropertyValue("Income", ref _Income, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense01
        {
            get { return _Expense01; }
            set { SetPropertyValue("Expense01", ref _Expense01, value); }
        }
        [NonPersistent]
        [DisplayName("01(%)")]
        public decimal Percent01
        {
            get {
                if (_Income > 0 && _Expense01 > 0)
                {
                    _Percent01 = (_Expense01 / _Income) * 100;
                }
                else
                {
                    _Percent01 = 0m;
                }
                return _Percent01; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense02
        {
            get { return _Expense02; }
            set { SetPropertyValue("Expense02", ref _Expense02, value); }
        }
        [NonPersistent]
        [DisplayName("02(%)")]
        public decimal Percent02
        {
            get {
                if (_Income > 0 && _Expense02 > 0)
                {
                    _Percent02 = (_Expense02 / _Income) * 100;
                }
                else
                {
                    _Percent02 = 0m;
                }
                return _Percent02; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense03
        {
            get { return _Expense03; }
            set { SetPropertyValue("Expense03", ref _Expense03, value); }
        }
        [NonPersistent]
        [DisplayName("03(%)")]
        public decimal Percent03
        {
            get {
                if (_Income > 0 && _Expense03 > 0)
                {
                    _Percent03 = (_Expense03 / _Income) * 100;
                }
                else
                {
                    _Percent03 = 0m;
                }
                return _Percent03; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense04
        {
            get { return _Expense04; }
            set { SetPropertyValue("Expense04", ref _Expense04, value); }
        }
        [NonPersistent]
        [DisplayName("04(%)")]
        public decimal Percent04
        {
            get {
                if (_Income > 0 && _Expense04 > 0)
                {
                    _Percent04 = (_Expense04 / _Income) * 100;
                }
                else
                {
                    _Percent04 = 0m;
                }
                return _Percent04; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense05
        {
            get { return _Expense05; }
            set { SetPropertyValue("Expense05", ref _Expense05, value); }
        }
        [NonPersistent]
        [DisplayName("05(%)")]
        public decimal Percent05
        {
            get {
                if (_Income > 0 && _Expense05 > 0)
                {
                    _Percent05 = (_Expense05 / _Income) * 100;
                }
                else
                {
                    _Percent05 = 0m;
                }
                return _Percent05; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense06
        {
            get { return _Expense06; }
            set { SetPropertyValue("Expense06", ref _Expense06, value); }
        }
        [NonPersistent]
        [DisplayName("06(%)")]
        public decimal Percent06
        {
            get {
                if (_Income > 0 && _Expense06 > 0)
                {
                    _Percent06 = (_Expense06 / _Income) * 100;
                }
                else
                {
                    _Percent06 = 0m;
                }
                return _Percent06; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense07
        {
            get { return _Expense07; }
            set { SetPropertyValue("Expense07", ref _Expense07, value); }
        }
        [NonPersistent]
        [DisplayName("07(%)")]
        public decimal Percent07
        {
            get {
                if (_Income > 0 && _Expense07 > 0)
                {
                    _Percent07 = (_Expense07 / _Income) * 100;
                }
                else
                {
                    _Percent07 = 0m;
                }
                return _Percent07; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense08
        {
            get { return _Expense08; }
            set { SetPropertyValue("Expense08", ref _Expense08, value); }
        }
        [NonPersistent]
        [DisplayName("08(%)")]
        public decimal Percent08
        {
            get {
                if (_Income > 0 && _Expense08 > 0)
                {
                    _Percent08 = (_Expense08 / _Income) * 100;
                }
                else
                {
                    _Percent08 = 0m;
                }
                return _Percent08; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense09
        {
            get { return _Expense09; }
            set { SetPropertyValue("Expense09", ref _Expense09, value); }
        }
        [NonPersistent]
        [DisplayName("09(%)")]
        public decimal Percent09
        {
            get {
                if (_Income > 0 && _Expense09 > 0)
                {
                    _Percent09 = (_Expense09 / _Income) * 100;
                }
                else
                {
                    _Percent09 = 0m;
                }
                return _Percent09; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense10
        {
            get { return _Expense10; }
            set { SetPropertyValue("Expense10", ref _Expense10, value); }
        }
        [NonPersistent]
        [DisplayName("10(%)")]
        public decimal Percent10
        {
            get {
                if (_Income > 0 && _Expense10 > 0)
                {
                    _Percent10 = (_Expense10 / _Income) * 100;
                }
                else
                {
                    _Percent10 = 0m;
                }
                return _Percent10; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense11
        {
            get { return _Expense11; }
            set { SetPropertyValue("Expense11", ref _Expense11, value); }
        }
        [NonPersistent]
        [DisplayName("11(%)")]
        public decimal Percent11
        {
            get {
                if (_Income > 0 && _Expense11 > 0)
                {
                    _Percent11 = (_Expense11 / _Income) * 100;
                }
                else
                {
                    _Percent11 = 0m;
                }
                return _Percent11; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense12
        {
            get { return _Expense12; }
            set { SetPropertyValue("Expense12", ref _Expense12, value); }
        }
        [NonPersistent]
        [DisplayName("12(%)")]
        public decimal Percent12
        {
            get {
                if (_Income > 0 && _Expense12 > 0)
                {
                    _Percent12 = (_Expense12 / _Income) * 100;
                }
                else
                {
                    _Percent12 = 0m;
                }
                return _Percent12; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense13
        {
            get { return _Expense13; }
            set { SetPropertyValue("Expense13", ref _Expense13, value); }
        }
        [NonPersistent]
        [DisplayName("13(%)")]
        public decimal Percent13
        {
            get {
                if (_Income > 0 && _Expense13 > 0)
                {
                    _Percent13 = (_Expense13 / _Income) * 100;
                }
                else
                {
                    _Percent13 = 0m;
                }
                return _Percent13; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense14
        {
            get { return _Expense14; }
            set { SetPropertyValue("Expense14", ref _Expense14, value); }
        }
        [NonPersistent]
        [DisplayName("14(%)")]
        public decimal Percent14
        {
            get {
                if (_Income > 0 && _Expense14 > 0)
                {
                    _Percent14 = (_Expense14 / _Income) * 100;
                }
                else
                {
                    _Percent14 = 0m;
                }
                return _Percent14; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense15
        {
            get { return _Expense15; }
            set { SetPropertyValue("Expense15", ref _Expense15, value); }
        }
        [NonPersistent]
        [DisplayName("15(%)")]
        public decimal Percent15
        {
            get {
                if (_Income > 0 && _Expense15 > 0)
                {
                    _Percent15 = (_Expense15 / _Income) * 100;
                }
                else
                {
                    _Percent15 = 0m;
                }
                return _Percent15; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense16
        {
            get { return _Expense16; }
            set { SetPropertyValue("Expense16", ref _Expense16, value); }
        }
        [NonPersistent]
        [DisplayName("16(%)")]
        public decimal Percent16
        {
            get {
                if (_Income > 0 && _Expense16 > 0)
                {
                    _Percent16 = (_Expense16 / _Income) * 100;
                }
                else
                {
                    _Percent16 = 0m;
                }
                return _Percent16; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Expense17
        {
            get { return _Expense17; }
            set { SetPropertyValue("Expense17", ref _Expense17, value); }
        }
        [NonPersistent]
        [DisplayName("17(%)")]
        public decimal Percent17
        {
            get {
                if (_Income > 0 && _Expense17 > 0)
                {
                    _Percent17 = (_Expense17 / _Income) * 100;
                }
                else
                {
                    _Percent17 = 0m;
                }
                return _Percent17; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("OTHER EXPENSES")]
        public decimal OtherExpenses
        {
            get { return _OtherExpenses; }
            set { SetPropertyValue("OtherExpenses", ref _OtherExpenses, value); }
        }
        [NonPersistent]
        [DisplayName("OT(%)")]
        public decimal PercentOExp
        {
            get {
                if (_Income > 0 && _OtherExpenses > 0)
                {
                    _PercentOExp = (_OtherExpenses / _Income) * 100;
                }
                else
                {
                    _PercentOExp = 0m;
                }
                return _PercentOExp; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("TOTAL EXPENSES")]
        public decimal TotalExpenses
        {
            get { return _TotalExpenses; }
            set { SetPropertyValue("TotalExpenses", ref _TotalExpenses, value); }
        }
        [NonPersistent]
        [DisplayName("TOTX(%)")]
        public decimal TotalExpPercent
        {
            get {
                if (_TotalExpenses > 0 && _NetIncomeLoss > 0)
                {
                    _TotalExpPercent = (_TotalExpenses / (_TotalExpenses + _NetIncomeLoss)) * 100;
                }
                else
                {
                    _TotalExpPercent = 0m;
                }
                return _TotalExpPercent; }
        }
        
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        [DisplayName("NET INCOME LOSS")]
        public decimal NetIncomeLoss
        {
            get { return _NetIncomeLoss; }
            set { SetPropertyValue("NetIncomeLoss", ref _NetIncomeLoss, value); }
        }
        [NonPersistent]
        [DisplayName("NET(%)")]
        public decimal NetPercent
        {
            get {
                if (_TotalExpenses > 0 && _NetIncomeLoss > 0)
                {
                    _NetPercent = (_NetIncomeLoss / (_TotalExpenses + _NetIncomeLoss)) * 100;
                }
                else
                {
                    _NetPercent = 0m;
                }
                return _NetPercent; }
        }
        
        private string _Seq;
        [Custom("AllowEdit", "False")]
        public string Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
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
        public TruckIncomeExpenseDetail(Session session)
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
            RowID = Guid.NewGuid();
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
