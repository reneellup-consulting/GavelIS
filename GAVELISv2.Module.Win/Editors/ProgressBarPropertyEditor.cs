using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using System;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;

namespace GAVELISv2.Module.Win.Editors
{
    [PropertyEditor(typeof(decimal), "ProgressBarPropertyEditor", false)]
    public class ProgressBarPropertyEditor : DXPropertyEditor
    {
        public ProgressBarPropertyEditor(Type objectType, IModelMemberViewItem 
        model): base(objectType, model) {
            this.ControlBindingProperty = "Value";
        }
        protected override object CreateControlCore() { return new ProgressBarControl(); }
        protected override void SetupRepositoryItem(RepositoryItem item)
        {
            base.SetupRepositoryItem(item);
            RepositoryItemProgressBar bar = item as RepositoryItemProgressBar;
            if (bar != null)
            {
                bar.Minimum = 0;
                bar.Maximum = 100;
                bar.ShowTitle = true;
                bar.PercentView = true;

                //bar.LookAndFeel.UseDefaultLookAndFeel = false;
                //bar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
                //bar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;

                //bar.StartColor = Color.Red;
                //bar.EndColor = Color.LightPink;

                bar.CustomDisplayText += bar_CustomDisplayText;
            }
            //((RepositoryItemSpinEdit)item).Mask.EditMask = "00000000.00";
            //((RepositoryItemSpinEdit)item).Mask.UseMaskAsDisplayFormat = true;
        }


        protected override RepositoryItem CreateRepositoryItem()
        {
            return new RepositoryItemProgressBar();
        }
        private void bar_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            RepositoryItemProgressBar bar = sender as RepositoryItemProgressBar;
            //if (bar != null)
            //{
            //    bar.StartColor = ((int)e.Value < 95 ? Color.Red : Color.Green);
            //    bar.EndColor = ((int)e.Value < 95 ? Color.LightPink : Color.LightGreen);
            //}
            if (bar != null)
            {
                e.DisplayText = ((int)e.Value * bar.Maximum / 100).ToString();
            }
        }
    }
}
