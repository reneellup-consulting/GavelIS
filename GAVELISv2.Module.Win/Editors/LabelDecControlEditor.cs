using System;
using System.Windows.Forms;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraEditors.Repository;

namespace GAVELISv2.Module.Win.Editors
{
    [PropertyEditor(typeof(string), "LabelDecControlEditor", false)]
    public class LabelDecControlEditor : WinPropertyEditor
    {
        public LabelDecControlEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override object CreateControlCore()
        {
            var control = new DevExpress.XtraEditors.LabelControl();
            control.AllowHtmlString = true;
            control.Dock = System.Windows.Forms.DockStyle.Fill;
            control.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            control.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            ControlBindingProperty = "Text";
            return control;
        }
    }
}
