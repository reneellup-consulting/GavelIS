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
    public class EmployeeFingerprint : XPObject {
        private Employee _EmployeedId;
        private int _FingerIndex;
        private string _TmpData;
        private int _TmpLenght;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("EmployeeFingerprintTemplates")]
        public Employee EmployeedId {
            get { return _EmployeedId; }
            set
            {
                SetPropertyValue<Employee>("EmployeedId", ref _EmployeedId, value);
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public int FingerIndex {
            get { return _FingerIndex; }
            set
            {
                SetPropertyValue<int>("FingerIndex", ref _FingerIndex, value);
            }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Size(SizeAttribute.Unlimited)]
        public string TmpData {
            get { return _TmpData; }
            set
            {
                SetPropertyValue<string>("TmpData", ref _TmpData, value);
            }
        }

        public int TmpLenght {
            get { return _TmpLenght; }
            set
            {
                SetPropertyValue<int>("TmpLenght", ref _TmpLenght, value);
            }
        }

        public EmployeeFingerprint(Session session)
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
