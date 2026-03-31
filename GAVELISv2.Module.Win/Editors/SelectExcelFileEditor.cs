using System;
using System.Windows;
//using System.Windows.Controls;
using System.Collections.Generic;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Editors;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace GAVELISv2.Module.Win.Editors {
    [PropertyEditor(typeof(string), false)]
    public class SelectExcelFileEditor : WinPropertyEditor {
        public SelectExcelFileEditor(Type objectType, IModelMemberViewItem 
        model): base(objectType, model) {
            var propertyType = model.ModelMember.Type;
            var validTypes = new List<Type> { typeof(string)
            };
            if (!validTypes.Contains(propertyType)) {throw new Exception(
                "Can't use FolderBrowseEditor with property type " + 
                propertyType.FullName);}
            this.ControlBindingProperty = "Value";
        }
        ButtonEdit txtFilePath;
        protected override object CreateControlCore() {
            txtFilePath = new ButtonEdit();
            txtFilePath.ButtonClick += new DevExpress.XtraEditors.Controls.
            ButtonPressedEventHandler(txtFilePath_ButtonClick);
            txtFilePath.EditValueChanged += new EventHandler(
            txtFilePath_EditValueChanged);
            return txtFilePath;
        }
        void txtFilePath_EditValueChanged(object sender, EventArgs e) {
            try
            {
                PropertyValue = txtFilePath.Text;
                OnControlValueChanged();

            }
            catch (Exception)
            {

            }
        }
        void txtFilePath_ButtonClick(object sender, DevExpress.XtraEditors.
        Controls.ButtonPressedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".xlxs";
            dlg.Filter = 
            "Excel Files(.xls)|*.xls|Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm"
            ;
            // Display OpenFileDialog by calling ShowDialog method 
            DialogResult result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == DialogResult.OK) {
                // Open document 
                string filename = dlg.FileName;
                this.txtFilePath.Text = filename;
            }
        }
        protected override void ReadValueCore() { txtFilePath.Text = Convert.
            ToString(PropertyValue); }
        protected override void Dispose(bool disposing) {
            if (txtFilePath != null) {
                txtFilePath.ButtonClick -= txtFilePath_ButtonClick;
                txtFilePath.EditValueChanged -= txtFilePath_EditValueChanged;
            }
            base.Dispose(disposing);
        }
    }
}
