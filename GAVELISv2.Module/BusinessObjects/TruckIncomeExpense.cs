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
using System.IO;
namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class TruckIncomeExpense : XPObject
    {
        private int _Year;
        private string _Description;
        private ExpenseType _Expense01;
        private ExpenseType _Expense02;
        private ExpenseType _Expense03;
        private ExpenseType _Expense04;
        private ExpenseType _Expense05;
        private ExpenseType _Expense06;
        private ExpenseType _Expense07;
        private ExpenseType _Expense08;
        private ExpenseType _Expense09;
        private ExpenseType _Expense10;
        private ExpenseType _Expense11;
        private ExpenseType _Expense12;
        private ExpenseType _Expense13;
        private ExpenseType _Expense14;
        private ExpenseType _Expense15;
        private ExpenseType _Expense16;
        private ExpenseType _Expense17;
        private string _ExpenseCaption01;
        private string _ExpenseCaption02;
        private string _ExpenseCaption03;
        private string _ExpenseCaption04;
        private string _ExpenseCaption05;
        private string _ExpenseCaption06;
        private string _ExpenseCaption07;
        private string _ExpenseCaption08;
        private string _ExpenseCaption09;
        private string _ExpenseCaption10;
        private string _ExpenseCaption11;
        private string _ExpenseCaption12;
        private string _ExpenseCaption13;
        private string _ExpenseCaption14;
        private string _ExpenseCaption15;
        private string _ExpenseCaption16;
        private string _ExpenseCaption17;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense01
        {
            get { return _Expense01; }
            set { SetPropertyValue("Expense01", ref _Expense01, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense02
        {
            get { return _Expense02; }
            set { SetPropertyValue("Expense02", ref _Expense02, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense03
        {
            get { return _Expense03; }
            set { SetPropertyValue("Expense03", ref _Expense03, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense04
        {
            get { return _Expense04; }
            set { SetPropertyValue("Expense04", ref _Expense04, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense05
        {
            get { return _Expense05; }
            set { SetPropertyValue("Expense05", ref _Expense05, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense06
        {
            get { return _Expense06; }
            set { SetPropertyValue("Expense06", ref _Expense06, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense07
        {
            get { return _Expense07; }
            set { SetPropertyValue("Expense07", ref _Expense07, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense08
        {
            get { return _Expense08; }
            set { SetPropertyValue("Expense08", ref _Expense08, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense09
        {
            get { return _Expense09; }
            set { SetPropertyValue("Expense09", ref _Expense09, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense10
        {
            get { return _Expense10; }
            set { SetPropertyValue("Expense10", ref _Expense10, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense11
        {
            get { return _Expense11; }
            set { SetPropertyValue("Expense11", ref _Expense11, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense12
        {
            get { return _Expense12; }
            set { SetPropertyValue("Expense12", ref _Expense12, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense13
        {
            get { return _Expense13; }
            set { SetPropertyValue("Expense13", ref _Expense13, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense14
        {
            get { return _Expense14; }
            set { SetPropertyValue("Expense14", ref _Expense14, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense15
        {
            get { return _Expense15; }
            set { SetPropertyValue("Expense15", ref _Expense15, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense16
        {
            get { return _Expense16; }
            set { SetPropertyValue("Expense16", ref _Expense16, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public ExpenseType Expense17
        {
            get { return _Expense17; }
            set { SetPropertyValue("Expense17", ref _Expense17, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption01
        {
            get { return _ExpenseCaption01; }
            set { SetPropertyValue("ExpenseCaption01", ref _ExpenseCaption01, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption02
        {
            get { return _ExpenseCaption02; }
            set { SetPropertyValue("ExpenseCaption02", ref _ExpenseCaption02, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption03
        {
            get { return _ExpenseCaption03; }
            set { SetPropertyValue("ExpenseCaption03", ref _ExpenseCaption03, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption04
        {
            get { return _ExpenseCaption04; }
            set { SetPropertyValue("ExpenseCaption04", ref _ExpenseCaption04, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption05
        {
            get { return _ExpenseCaption05; }
            set { SetPropertyValue("ExpenseCaption05", ref _ExpenseCaption05, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption06
        {
            get { return _ExpenseCaption06; }
            set { SetPropertyValue("ExpenseCaption06", ref _ExpenseCaption06, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption07
        {
            get { return _ExpenseCaption07; }
            set { SetPropertyValue("ExpenseCaption07", ref _ExpenseCaption07, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption08
        {
            get { return _ExpenseCaption08; }
            set { SetPropertyValue("ExpenseCaption08", ref _ExpenseCaption08, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption09
        {
            get { return _ExpenseCaption09; }
            set { SetPropertyValue("ExpenseCaption09", ref _ExpenseCaption09, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption10
        {
            get { return _ExpenseCaption10; }
            set { SetPropertyValue("ExpenseCaption10", ref _ExpenseCaption10, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption11
        {
            get { return _ExpenseCaption11; }
            set { SetPropertyValue("ExpenseCaption11", ref _ExpenseCaption11, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption12
        {
            get { return _ExpenseCaption12; }
            set { SetPropertyValue("ExpenseCaption12", ref _ExpenseCaption12, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption13
        {
            get { return _ExpenseCaption13; }
            set { SetPropertyValue("ExpenseCaption13", ref _ExpenseCaption13, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption14
        {
            get { return _ExpenseCaption14; }
            set { SetPropertyValue("ExpenseCaption14", ref _ExpenseCaption14, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption15
        {
            get { return _ExpenseCaption15; }
            set { SetPropertyValue("ExpenseCaption15", ref _ExpenseCaption15, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption16
        {
            get { return _ExpenseCaption16; }
            set { SetPropertyValue("ExpenseCaption16", ref _ExpenseCaption16, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string ExpenseCaption17
        {
            get { return _ExpenseCaption17; }
            set { SetPropertyValue("ExpenseCaption17", ref _ExpenseCaption17, value); }
        }
        [Action(Caption = "Print All")]
        public void Print(){
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\UnitNetLossReport.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            XPCollection<TruckIncomeExpense> data =  new XPCollection<TruckIncomeExpense>(Session);
            //data.Criteria = BinaryOperator.Parse("[Oid] = ?", this.Oid);
            //int count = data.Count;
            rep.DataSource = data;
            rep.FilterString = string.Format("[Oid] = {0}", this.Oid);
            rep.ShowPreview();
        }
        [Action(Caption = "Print All/%")]
        public void PrintPercentage()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\UnitNetLossReportPercentage.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            XPCollection<TruckIncomeExpense> data = new XPCollection<TruckIncomeExpense>(Session);
            //data.Criteria = BinaryOperator.Parse("[Oid] = ?", this.Oid);
            //int count = data.Count;
            rep.DataSource = data;
            rep.FilterString = string.Format("[Oid] = {0}", this.Oid);
            rep.ShowPreview();
        }
        #region Associations

        [Aggregated,
        Association("TruckIncomeExpenseDetails")]
        public XPCollection<TruckIncomeExpenseDetail> TruckIncomeExpenseDetails
        {
            get { return GetCollection<TruckIncomeExpenseDetail>("TruckIncomeExpenseDetails"); }
        }
        [Aggregated,
        Association("TruckIncomeExpenseTrucks")]
        public XPCollection<TruckIncomeExpenseTruck> TruckIncomeExpenseTrucks
        {
            get { return GetCollection<TruckIncomeExpenseTruck>("TruckIncomeExpenseTrucks"); }
        }
        #endregion
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
        public TruckIncomeExpense(Session session)
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
