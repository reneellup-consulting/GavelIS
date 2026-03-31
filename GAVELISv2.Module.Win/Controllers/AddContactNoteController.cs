using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class AddContactNoteController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Contact _Contact;
        private ContactNote _Obj;
        private PopupWindowShowAction addContactNote;
        public AddContactNoteController()
        {
            this.TargetObjectType = typeof(Contact);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "Contact.AddNote";
            this.addContactNote = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.addContactNote.Caption = "Add Note";
            this.addContactNote.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            AddNote_CustomizePopupWindowParams);
            this.addContactNote.Execute += new PopupWindowShowActionExecuteEventHandler(
            AddNote_Execute);
        }

        private void AddNote_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Contact = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject
            as Contact;
            ObjectSpace.CommitChanges();
            Contact thisContact = _ObjectSpace.GetObjectByKey<Contact>(
            _Contact.Oid);
            _Obj = _ObjectSpace.CreateObject<ContactNote>();
            thisContact.Notes.Add(_Obj);
            e.View = Application.CreateDetailView(_ObjectSpace, "AddNote_Detail",
            true, _Obj);
        }
        private void AddNote_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(_Trip);
            try
            {
                ObjectSpace.Refresh();
            }
            catch (Exception)
            {
            }
        }
    }
}
