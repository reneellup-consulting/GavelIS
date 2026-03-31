using System;
using System.ComponentModel;

using DevExpress.Xpo;
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
    public class ContactFileAttachment : FileAttachmentBase
    {
        private Contact _ContactId;
        private string _AttachedBy;

        [Association]
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Contact ContactId
        {
            get { return _ContactId; }
            set
            {
                SetPropertyValue("ContactId", ref _ContactId, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime AttachedDate
        {
            get { return _AttachedDate; }
            set
            {
                SetPropertyValue("AttachedDate", ref _AttachedDate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string AttachedBy
        {
            get { return _AttachedBy; }
            set
            {
                SetPropertyValue("AttachedBy", ref _AttachedBy, value);
            }
        }
        public ContactFileAttachment(Session session)
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
            AttachedBy = CurrentUser.UserName;
            AttachedDate = DateTime.Now;
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private DateTime _AttachedDate;
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
