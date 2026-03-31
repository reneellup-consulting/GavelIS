using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using GAVELISv2.Module.BusinessObjects;
using System.Drawing;

namespace GAVELISv2.Module.Win.Editors
{
    [PropertyEditor(typeof(XPBaseCollection), false)]
    public class TireServiceInsConDetailsPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        public TireServiceInsConDetailsPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override object CreateControlCore()
        {
            return new CheckedListBoxControl();
        }
        XPBaseCollection checkedItems;
        XafApplication application;
        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            if (PropertyValue is XPBaseCollection)
            {
                Control.MultiColumn = true;
                Control.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                Control.Items.Clear();
                checkedItems = (XPBaseCollection)PropertyValue;
                XPCollection dataSource = new XPCollection(checkedItems.Session, typeof(TireInspectionCondition));
                IModelClass classInfo = application.Model.BOModel.GetClass(typeof(TireInspectionCondition));
                if (checkedItems.Sorting.Count > 0)
                {
                    dataSource.Sorting = checkedItems.Sorting;
                }
                else if (!String.IsNullOrEmpty(classInfo.DefaultProperty))
                {
                    dataSource.Sorting.Add(new SortProperty(classInfo.DefaultProperty, DevExpress.Xpo.DB.SortingDirection.Ascending));
                }
                Control.DataSource = dataSource;
                Control.DisplayMember = classInfo.DefaultProperty;
                if (!dataSource.DisplayableProperties.Contains(classInfo.DefaultProperty))
                {
                    dataSource.DisplayableProperties += ";" + classInfo.DefaultProperty;
                }
                try
                {
                    foreach (object obj in checkedItems)
                    {
                        Control.SetItemChecked(dataSource.IndexOf((obj as TireServiceInsConDetail).Condition), true);
                    }
                }
                catch (Exception)
                {
                }
                Control.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(control_ItemCheck);
            }
        }
        private int iPass = 0;
        void control_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e) {
            object obj = Control.GetItemValue(e.Index);
            if (iPass != 0)
            {
                iPass = 0;
                return;
            }
            if (obj == null)
            {
                return;
            }
            TireServiceDetail2 tr = null;
            if (CurrentObject.GetType() == typeof(Tire))
            {
                tr = checkedItems.Session.GetObjectByKey<TireServiceDetail2>((CurrentObject as Tire).LastDetail.Oid);
            }
            else
            {
                tr = checkedItems.Session.GetObjectByKey<TireServiceDetail2>((CurrentObject as TireServiceDetail2).Oid);
            }
            TireInspectionCondition tic = obj as TireInspectionCondition;
            var data = tr.TireServiceInsConDetails.Where(o => o.Condition == tic);
            TireServiceInsConDetail ticd = data.LastOrDefault();
            if (ticd == null)
            {
                ticd = new TireServiceInsConDetail(checkedItems.Session);
                ticd.TsdId = tr;
                ticd.Condition = checkedItems.Session.GetObjectByKey<TireInspectionCondition>((obj as TireInspectionCondition).Oid);
            }
            switch (e.State)
            {
                case CheckState.Checked:
                    checkedItems.BaseAdd(ticd);
                    iPass++;
                    break;
                case CheckState.Unchecked:
                    checkedItems.BaseRemove(ticd);
                    ticd.Delete();
                    iPass++;
                    break;
                default:
                    break;
            }
        }
        public new CheckedListBoxControl Control
        {
            get
            {
                return (CheckedListBoxControl)base.Control;
            }
        }

        #region IComplexPropertyEditor Members

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.application = application;
        }

        #endregion
    }
}
