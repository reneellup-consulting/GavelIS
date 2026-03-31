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
    [System.ComponentModel.DefaultProperty("Code")]
    public class SourceType : BaseObject {
        private string _Code;
        private string _Description;
        private string _Document;
        private ExpenseType _ExpenseType;
        private SubExpenseType _SubExpenseType;
        private string _NumberFormat;
        private int _Increment = 1;
        private string _LastNumberUsed;
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Document {
            get { return _Document; }
            set { SetPropertyValue("Document", ref _Document, value); }
        }
        public ExpenseType ExpenseType
        {
            get
            {
                return _ExpenseType;
            }
            set
            {
                SetPropertyValue("ExpenseType", ref _ExpenseType, value);
            }
        }
        public SubExpenseType SubExpenseType
        {
            get
            {
                return _SubExpenseType;
            }
            set
            {
                SetPropertyValue("SubExpenseType", ref _SubExpenseType, value);
            }
        }
        
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public string NumberFormat {
            get { return _NumberFormat; }
            set { SetPropertyValue("NumberFormat", ref _NumberFormat, value); }
        }
        public int Increment {
            get { return _Increment; }
            set { SetPropertyValue("Increment", ref _Increment, value); }
        }
        public string LastNumberUsed {
            get { return _LastNumberUsed; }
            set { SetPropertyValue("LastNumberUsed", ref _LastNumberUsed, value)
                ; }
        }
        private Facility _Facility;
        [DisplayName("Default Facility")]
        public Facility Facility
        {
            get { return _Facility; }
            set
            {
                SetPropertyValue("Facility", ref _Facility, value);
            }
        }
        private Department _Department;
        [DisplayName("Default Department")]
        [DataSourceProperty("Facility.Departments")]
        public Department Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }
        #region Action
        public string GetNewNo() {
            string seqNo;
            string incNo;
            int inc = 1;
            if (_Increment != 0) {inc = _Increment;}
            if (!string.IsNullOrEmpty(_LastNumberUsed)) {seqNo = _LastNumberUsed
                ;} else {
                seqNo = _NumberFormat;
            }
            string digits = "0123456789";
            string defaultFormat = "{0:D5}";
            string formatString = string.Empty;
            string num = string.Empty;
            int c = 0;
            int i, x;
            i = x = seqNo.LastIndexOfAny(digits.ToCharArray());
            while (i >= 0 && isDigit(seqNo[i])) {
                num = seqNo[i] + num;
                c++;
                i--;
            }
            int n = int.Parse(num) + inc;
            formatString = defaultFormat.Replace("5", c.ToString());
            incNo = string.Format(formatString, n);
            x = x + 1 - num.Length;
            seqNo = seqNo.Remove(x, num.Length);
            seqNo = seqNo.Insert(x, string.Empty + incNo);
            _LastNumberUsed = seqNo;
            // Update the No Series Line
            //UnitOfWork uow = new UnitOfWork();
            //ObjectSpace os = new ObjectSpace(uow);
            //Session _session = new Session(objectSpace.Session.DataLayer);
            //_session.BeginTransaction();
            //NoSeriesLine nSLine = _session.FindObject<NoSeriesLine>(new
            //BinaryOperator("Oid", nsLineNo));
            //nSLine.LastDateUsed = DateTime.Today;
            //nSLine.LastNoUsed = seqNo;
            ////nSLine.;
            //nSLine.Save();
            //_session.CommitTransaction();
            return seqNo;
        }
        private static bool isDigit(char c) {
            string digits = "0123456789";
            return digits.IndexOf(c) == -1 ? false : true;
        }
        #endregion
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
        public SourceType(Session session): base(session) {
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
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
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
    }
}
