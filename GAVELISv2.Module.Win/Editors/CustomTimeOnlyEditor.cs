using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;

namespace GAVELISv2.Module.Win.Editors
{
    [PropertyEditor(typeof(DateTime), "CustomTimeOnlyEditor", false)]
    public class CustomTimeOnlyEditor : DatePropertyEditor {
        public CustomTimeOnlyEditor(Type objectType, IModelMemberViewItem model)
            : base(objectType, model) {
            
        }
        protected override void SetupRepositoryItem(DevExpress.XtraEditors.Repository.RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            ((RepositoryItemDateTimeEdit)item).VistaDisplayMode = DevExpress.
            Utils.DefaultBoolean.True;
            ((RepositoryItemDateTimeEdit)item).VistaEditTime = DevExpress.Utils.
            DefaultBoolean.True;
            ((RepositoryItemDateTimeEdit)item).QueryPopUp += QueryPopup;
        }
        private void QueryPopup(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;
        }
    }
}
