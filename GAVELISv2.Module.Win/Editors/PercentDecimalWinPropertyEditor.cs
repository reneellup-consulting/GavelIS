using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;

namespace GAVELISv2.Module.Win.Editors
{
    [PropertyEditor(typeof(decimal), "PercentDecimalWinPropertyEditor", false)]
    public class PercentDecimalWinPropertyEditor : DXPropertyEditor
    {
        public PercentDecimalWinPropertyEditor(Type objectType, IModelMemberViewItem 
        model): base(objectType, model) {
            this.ControlBindingProperty = "Value";
        }
        protected override object CreateControlCore() { return new CalcEdit(); }
        protected override void SetupRepositoryItem(RepositoryItem item) {
            base.SetupRepositoryItem(item);
            ((RepositoryItemCalcEdit)item).Mask.EditMask = "P2";
            ((RepositoryItemCalcEdit)item).Precision = 4;
            ((RepositoryItemCalcEdit)item).Mask.UseMaskAsDisplayFormat = true;
        }
        protected override RepositoryItem CreateRepositoryItem() { return new 
            RepositoryItemCalcEdit(); }
    }
}
