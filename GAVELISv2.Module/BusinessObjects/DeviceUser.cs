using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.Drawing;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class DeviceUser : XPObject {
        private int _UserID;
        private string _BadgeNo;
        private string _Snn;
        private string _UserName;
        private string _Gender;
        private string _Title;
        private string _Pager;
        private DateTime _BirthDate;
        private DateTime _HireDate;
        private string _Street;
        private string _City;
        private string _State;
        private string _Zip;
        private string _Ophone;
        private string _Fphone;
        private int _VerificationMethod;
        private int _SecurityFlags;
        private int _Att;
        private int _InLate;
        private int _OutEarly;
        private int _OverTime;
        private int _Sep;
        private int _Holiday;
        private string _Minzu;
        private string _Password;
        private int _LunchDuration;
        private string _MverifyPass;
        //private Image _Photo;
        private Image _Notes;
        private int _Privilege;
        private int _InheritDeptSch;
        private int _InheritDeptSchClass;
        private int _AutoSchPlan;
        private int _MinAutoSchInterval;
        private int _RegisterOT;
        private int _InheritDeptRule;
        private int _Emprivilege;
        private string _CardNo;
        [Custom("AllowEdit", "False")]
        public int UserID {
            get { return _UserID; }
            set
            {
                SetPropertyValue<int>("UserID", ref _UserID, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string BadgeNo {
            get { return _BadgeNo; }
            set
            {
                SetPropertyValue<string>("BadgeNo", ref _BadgeNo, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Snn {
            get { return _Snn; }
            set
            {
                SetPropertyValue<string>("Snn", ref _Snn, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string UserName {
            get { return _UserName; }
            set
            {
                SetPropertyValue<string>("UserName", ref _UserName, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Gender {
            get { return _Gender; }
            set
            {
                SetPropertyValue<string>("Gender", ref _Gender, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Title {
            get { return _Title; }
            set
            {
                SetPropertyValue<string>("Title", ref _Title, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Pager {
            get { return _Pager; }
            set
            {
                SetPropertyValue<string>("Pager", ref _Pager, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime BirthDate {
            get { return _BirthDate; }
            set
            {
                SetPropertyValue<DateTime>("BirthDate", ref _BirthDate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime HireDate {
            get { return _HireDate; }
            set
            {
                SetPropertyValue<DateTime>("HireDate", ref _HireDate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Street {
            get { return _Street; }
            set
            {
                SetPropertyValue<string>("Street", ref _Street, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string City {
            get { return _City; }
            set
            {
                SetPropertyValue<string>("City", ref _City, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string State {
            get { return _State; }
            set
            {
                SetPropertyValue<string>("State", ref _State, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Zip {
            get { return _Zip; }
            set
            {
                SetPropertyValue<string>("Zip", ref _Zip, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Ophone {
            get { return _Ophone; }
            set
            {
                SetPropertyValue<string>("Ophone", ref _Ophone, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Fphone {
            get { return _Fphone; }
            set
            {
                SetPropertyValue<string>("Fphone", ref _Fphone, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int VerificationMethod {
            get { return _VerificationMethod; }
            set
            {
                SetPropertyValue<int>("VerificationMethod", ref _VerificationMethod, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int SecurityFlags {
            get { return _SecurityFlags; }
            set
            {
                SetPropertyValue<int>("SecurityFlags", ref _SecurityFlags, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int Att {
            get { return _Att; }
            set
            {
                SetPropertyValue<int>("Att", ref _Att, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int InLate {
            get { return _InLate; }
            set
            {
                SetPropertyValue<int>("InLate", ref _InLate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int OutEarly {
            get { return _OutEarly; }
            set
            {
                SetPropertyValue<int>("OutEarly", ref _OutEarly, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int OverTime {
            get { return _OverTime; }
            set
            {
                SetPropertyValue<int>("OverTime", ref _OverTime, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int Sep {
            get { return _Sep; }
            set
            {
                SetPropertyValue<int>("Sep", ref _Sep, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int Holiday {
            get { return _Holiday; }
            set
            {
                SetPropertyValue<int>("Holiday", ref _Holiday, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Minzu {
            get { return _Minzu; }
            set
            {
                SetPropertyValue<string>("Minzu", ref _Minzu, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string Password {
            get { return _Password; }
            set
            {
                SetPropertyValue<string>("Password", ref _Password, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int LunchDuration {
            get { return _LunchDuration; }
            set
            {
                SetPropertyValue<int>("LunchDuration", ref _LunchDuration, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string MverifyPass {
            get { return _MverifyPass; }
            set
            {
                SetPropertyValue<string>("MverifyPass", ref _MverifyPass, value);
            }
        }
        [Custom("AllowEdit", "False")]
        [Size(SizeAttribute.Unlimited),
                Delayed(true),
                ValueConverter(typeof(ImageValueConverter))]
        public Image Photo
        {
            get { return GetDelayedPropertyValue<Image>("Photo"); }
            set { SetDelayedPropertyValue<Image>("Photo", value); }
        }
        [Custom("AllowEdit", "False")]
        public Image Notes {
            get { return _Notes; }
            set
            {
                SetPropertyValue<Image>("Notes", ref _Notes, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int Privilege {
            get { return _Privilege; }
            set
            {
                SetPropertyValue<int>("Privilege", ref _Privilege, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int InheritDeptSch {
            get { return _InheritDeptSch; }
            set
            {
                SetPropertyValue<int>("InheritDeptSch", ref _InheritDeptSch, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int InheritDeptSchClass {
            get { return _InheritDeptSchClass; }
            set
            {
                SetPropertyValue<int>("InheritDeptSchClass", ref _InheritDeptSchClass, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int AutoSchPlan {
            get { return _AutoSchPlan; }
            set
            {
                SetPropertyValue<int>("AutoSchPlan", ref _AutoSchPlan, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int MinAutoSchInterval {
            get { return _MinAutoSchInterval; }
            set
            {
                SetPropertyValue<int>("MinAutoSchInterval", ref _MinAutoSchInterval, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int RegisterOT {
            get { return _RegisterOT; }
            set
            {
                SetPropertyValue<int>("RegisterOT", ref _RegisterOT, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int InheritDeptRule {
            get { return _InheritDeptRule; }
            set
            {
                SetPropertyValue<int>("InheritDeptRule", ref _InheritDeptRule, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public int Emprivilege {
            get { return _Emprivilege; }
            set
            {
                SetPropertyValue<int>("Emprivilege", ref _Emprivilege, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string CardNo {
            get { return _CardNo; }
            set
            {
                SetPropertyValue<string>("CardNo", ref _CardNo, value);
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
            set
            {
                SetPropertyValue("CreatedBy", ref createdBy, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set
            {
                SetPropertyValue("CreatedOn", ref createdOn, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set
            {
                SetPropertyValue("ModifiedBy", ref modifiedBy, value);
            }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set
            {
                SetPropertyValue("ModifiedOn", ref modifiedOn, value);
            }
        }

        #endregion

        public DeviceUser(Session session)
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
        public SecurityUser CurrentUser {
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
