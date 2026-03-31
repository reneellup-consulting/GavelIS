using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;

namespace GAVELISv2.Module.Win.Editors
{
    [PropertyEditor(typeof(string), "MonospaceStringPropertyEditor", false)]
    public class MonospaceStringPropertyEditor : DXPropertyEditor
    {
        public MonospaceStringPropertyEditor(Type objectType, 
        IModelMemberViewItem model): base(objectType, model) {
            this.ControlBindingProperty = "EditValue";
        }
        protected override object CreateControlCore() { return new MemoEdit(); }
        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            ((RepositoryItemMemoEdit)item).Appearance.Font = new System.Drawing.Font("Courier New", 9f);
        }
        protected override RepositoryItem CreateRepositoryItem() { return new 
            RepositoryItemMemoEdit(); }
    }
}
