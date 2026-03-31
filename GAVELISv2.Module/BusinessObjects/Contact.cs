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
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class Contact : BaseObject {
        private const string defaultDisplayFormat = "{No}->{Name}";
        private string _No;
        private string _Code;
        private string _Name;
        private string _FirstName;
        private string _MiddleName;
        private string _LastName;
        private string _Initial;
        private string _Phone;
        private string _Phone1;
        private string _Phone2;
        private string _Fax;
        private string _WebAddress;
        private string _Email;
        private ContactTypeEnum _ContactType;
        private Account _NonTradeAccountsPayable;
        private bool _Blocked = false;
        private string _TIN001;
        private string _TIN002;
        private string _TIN003;
        private string _TIN004;

        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public virtual string No {
            get { return _No; }
            set
            {
                SetPropertyValue("No", ref _No, value);
                if (!IsLoading)
                {
                    Code = _No;
                }
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public string Code {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value); }
        }

        //[RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public virtual string Name {
            get { return _Name; }
            set { SetPropertyValue("Name", ref _Name, value); }
        }

        public string FirstName {
            get { return _FirstName; }
            set { SetPropertyValue("FirstName", ref _FirstName, value); }
        }

        public string MiddleName {
            get { return _MiddleName; }
            set { SetPropertyValue("MiddleName", ref _MiddleName, value); }
        }

        public string LastName {
            get { return _LastName; }
            set { SetPropertyValue("LastName", ref _LastName, value); }
        }
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Initial {
            get { return _Initial; }
            set { SetPropertyValue<string>("Initial", ref _Initial, value); }
        }

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        [DisplayName("Land Line")]
        public string Phone {
            get { return _Phone; }
            set { SetPropertyValue("Phone", ref _Phone, value); }
        }

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        [DisplayName("Mobile No. 1")]
        public string Phone1
        {
            get { return _Phone1; }
            set { SetPropertyValue("Phone1", ref _Phone1, value); }
        }

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        [DisplayName("Mobile No. 2")]
        public string Phone2
        {
            get { return _Phone2; }
            set { SetPropertyValue("Phone2", ref _Phone2, value); }
        }

        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        public string Fax {
            get { return _Fax; }
            set { SetPropertyValue("Fax", ref _Fax, value); }
        }

        public string WebAddress {
            get { return _WebAddress; }
            set { SetPropertyValue("WebAddress", ref _WebAddress, value); }
        }

        [DisplayName("E-mail")]
        public string Email {
            get { return _Email; }
            set { SetPropertyValue("Email", ref _Email, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public ContactTypeEnum ContactType {
            get { return _ContactType; }
            set { SetPropertyValue("ContactType", ref _ContactType, value); }
        }

        public Account NonTradeAccountsPayable {
            get { return _NonTradeAccountsPayable; }
            set { SetPropertyValue("NonTradeAccountsPayable", ref
                _NonTradeAccountsPayable, value); }
        }

        public bool Blocked {
            get { return _Blocked; }
            set { SetPropertyValue("Blocked", ref _Blocked, value); }
        }

        public string TIN001 {
            get { return _TIN001 ?? string.Empty; }
            set { SetPropertyValue<string>("TIN001", ref _TIN001, value); }
        }

        public string TIN002 {
            get { return _TIN002 ?? string.Empty; }
            set { SetPropertyValue<string>("TIN002", ref _TIN002, value); }
        }

        public string TIN003 {
            get { return _TIN003 ?? string.Empty; }
            set { SetPropertyValue<string>("TIN003", ref _TIN003, value); }
        }

        public string TIN004 {
            get { return _TIN004 ?? string.Empty; }
            set { SetPropertyValue<string>("TIN004", ref _TIN004, value); }
        }

        #region Address

        private const string defaultAddressFormat =
        "{Address}, {ZipCode} {City}, {Province}, {Country}";

        private string _Address;
        private string _City;
        private string _ZipCode;
        private string _Province;
        private string _Country = "Philippines";
        [Size(500)]
        public string Address {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        public string City {
            get { return _City; }
            set { SetPropertyValue("City", ref _City, value); }
        }

        public string ZipCode {
            get { return _ZipCode; }
            set { SetPropertyValue("ZipCode", ref _ZipCode, value); }
        }

        public string Province {
            get { return _Province; }
            set { SetPropertyValue("Province", ref _Province, value); }
        }

        public string Country {
            get { return _Country; }
            set { SetPropertyValue("Country", ref _Country, value); }
        }

        public string FullAddress {
            get { return ObjectFormatter.Format(
                defaultAddressFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        [Association]
        public XPCollection<ContactNote> Notes
        {
            get
            {
                return GetCollection<ContactNote>("Notes"
                );
            }
        }

        [Association]
        public XPCollection<ContactFileAttachment> Attachments
        {
            get
            {
                return GetCollection<ContactFileAttachment>("Attachments"
                );
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

        #region Display String

        public string DisplayName {
            get { return ObjectFormatter.Format(
                defaultDisplayFormat, this, EmptyEntriesMode.
                RemoveDelimeterWhenEntryIsEmpty); }
        }

        #endregion

        public Contact(Session session)
            : base(session) {
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

            if (SecuritySystem.CurrentUser != null)
            {
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
