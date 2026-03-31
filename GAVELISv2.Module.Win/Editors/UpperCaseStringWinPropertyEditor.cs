using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;
namespace GAVELISv2.Module.Win.Editors {
    [PropertyEditor(typeof(string), "UpperCaseStringWinPropertyEditor", false)]
    public class UpperCaseStringWinPropertyEditor : DXPropertyEditor {
        public UpperCaseStringWinPropertyEditor(Type objectType, 
        IModelMemberViewItem model): base(objectType, model) {
            this.ControlBindingProperty = "EditValue";
        }
        protected override object CreateControlCore() { return new TextEdit(); }
        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            ((RepositoryItemTextEdit)item).CharacterCasing = System.Windows.
            Forms.CharacterCasing.Upper;
        }
        protected override RepositoryItem CreateRepositoryItem() { return new 
            RepositoryItemTextEdit(); }
    }
}
