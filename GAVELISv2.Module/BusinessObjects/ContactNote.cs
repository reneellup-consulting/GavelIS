using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Persistent.Base.General;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [DefaultProperty("Text")]
    public class ContactNote : XPObject, INote
    {
        public ContactNote(Session session)
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
            note.Author = CurrentUser.UserName;
        }

        private NoteImpl note = new NoteImpl();
        private Contact _ContactId;

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
        public string Author
        {
            get { return note.Author; }
            set
            {
                note.Author = value;
                OnChanged("Author");
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime DateTime
        {
            get { return note.DateTime; }
            set
            {
                note.DateTime = value;
                OnChanged("DateTime");
            }
        }

        [Size(SizeAttribute.Unlimited), ObjectValidatorIgnoreIssue(typeof(ObjectValidatorLargeNonDelayedMember))]
        public string Text
        {
            get { return note.Text; }
            set
            {
                note.Text = value;
                OnChanged("Text");
            }
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
