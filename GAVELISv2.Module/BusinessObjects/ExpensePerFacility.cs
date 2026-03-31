using System;
using System.IO;
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

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class ExpensePerFacility : XPObject
    {
        private int _Year;
        private Facility _Facility;
        private string _Description;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public Facility Facility
        {
            get { return _Facility; }
            set { SetPropertyValue("Facility", ref _Facility, value);
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value); }
        }
        [Aggregated,
        Association("ExpensePerFacilityDetails")]
        public XPCollection<ExpensePerFacilityDetail> ExpensePerFacilityDetails
        {
            get { return GetCollection<ExpensePerFacilityDetail>("ExpensePerFacilityDetails"); }
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
        [Action(Caption = "Print All")]
        public void Print()
        {
            this.Session.CommitTransaction();
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\YearlyExpensesPerFacility.repx";
            rep.LoadLayout(path);
            rep.ObjectSpace = ObjectSpace.FindObjectSpaceByObject(Session);
            XPCollection<ExpensePerFacility> data = new XPCollection<ExpensePerFacility>(Session);
            //data.Criteria = BinaryOperator.Parse("[Oid] = ?", this.Oid);
            //int count = data.Count;
            rep.DataSource = data;
            rep.FilterString = string.Format("[Oid] = {0}", this.Oid);
            rep.ShowPreview();
        }
        public ExpensePerFacility(Session session)
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
    }

}
