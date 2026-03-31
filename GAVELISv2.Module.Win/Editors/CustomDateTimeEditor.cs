using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
namespace GAVELISv2.Module.Win.Editors {
    [PropertyEditor(typeof(DateTime), "CustomDateTimeEditor", false)]
    public class CustomDateTimeEditor : DatePropertyEditor {
        public CustomDateTimeEditor(Type objectType, IModelMemberViewItem info): 
            base(objectType, info) { }
        protected override void SetupRepositoryItem(DevExpress.XtraEditors.
        Repository.RepositoryItem item) {
            base.SetupRepositoryItem(item);
            ((RepositoryItemDateTimeEdit)item).VistaDisplayMode = DevExpress.
            Utils.DefaultBoolean.True;
            ((RepositoryItemDateTimeEdit)item).VistaEditTime = DevExpress.Utils.
            DefaultBoolean.True;
            //((RepositoryItemDateTimeEdit)item).Mask.EditMask = 
            //"MM.dd.yyyy hh:mm:ss";
            //((RepositoryItemDateTimeEdit)item).Mask.EditMask = "g";
            //((RepositoryItemDateTimeEdit)item).Mask.UseMaskAsDisplayFormat = 
            //true;
        }
    }
}
