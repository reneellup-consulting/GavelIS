using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;
namespace GAVELISv2.Module.Win.Editors {
    [PropertyEditor(typeof(decimal), "MeterTypePropertyEditor", false)]
    public class MeterTypePropertyEditor : DXPropertyEditor {
        public MeterTypePropertyEditor(Type objectType, IModelMemberViewItem 
        model): base(objectType, model) {
            this.ControlBindingProperty = "Value";
        }
        protected override object CreateControlCore() { return new SpinEdit(); }
        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            ((RepositoryItemSpinEdit)item).Mask.EditMask = "00000000.00";
            ((RepositoryItemSpinEdit)item).Mask.UseMaskAsDisplayFormat = true;
        }
        protected override RepositoryItem CreateRepositoryItem() { return new 
            RepositoryItemSpinEdit(); }
    }
}
