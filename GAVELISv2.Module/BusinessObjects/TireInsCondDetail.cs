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
    [RuleCombinationOfPropertiesIsUnique("", DefaultContexts.Save, "TireId,Condition")]
    public class TireInsCondDetail : XPObject {
        private Tire _TireId;
        private TireInspectionCondition _Condition;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TireInspectionConditions")]
        public Tire TireId {
            get { return _TireId; }
            set { SetPropertyValue<Tire>("TireId", ref _TireId, value); }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public TireInspectionCondition Condition {
            get { return _Condition; }
            set { SetPropertyValue<TireInspectionCondition>("Condition", ref _Condition, value); }
        }

        public TireInsCondDetail(Session session)
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
