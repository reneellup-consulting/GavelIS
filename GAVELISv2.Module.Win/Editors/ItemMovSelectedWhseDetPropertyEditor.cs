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
    public class ItemMovSelectedWhseDetPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        public ItemMovSelectedWhseDetPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
        protected override object CreateControlCore()
        {
            return new CheckedListBoxControl();
        }
        XPBaseCollection checkedItems;
        XafApplication application;
        public new CheckedListBoxControl Control
        {
            get
            {
                return (CheckedListBoxControl)base.Control;
            }
        }
        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            if (PropertyValue is XPBaseCollection)
            {
                Control.MultiColumn = true;
                Control.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
                Control.Items.Clear();
                checkedItems = (XPBaseCollection)PropertyValue;
                XPCollection dataSource = new XPCollection(checkedItems.Session, typeof(Warehouse));
                IModelClass classInfo = application.Model.BOModel.GetClass(typeof(Warehouse));
                //if (checkedItems.Sorting.Count > 0)
                //{
                //    dataSource.Sorting = checkedItems.Sorting;
                //}
                //else if (!String.IsNullOrEmpty(classInfo.DefaultProperty))
                //{
                //    dataSource.Sorting.Add(new SortProperty(classInfo.DefaultProperty, DevExpress.Xpo.DB.SortingDirection.Ascending));
                //}
                dataSource.Sorting.Add(new SortProperty(classInfo.DefaultProperty, DevExpress.Xpo.DB.SortingDirection.Ascending));
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
                        Control.SetItemChecked(dataSource.IndexOf((obj as ItemsMovSelectedWhseDetail).Whse), true);
                    }
                }
                catch (Exception)
                {
                }
                Control.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(control_ItemCheck);
            }
        }
        private int iPass = 0;
        void control_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
        {
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
            ItemsMovementGroup img = checkedItems.Session.GetObjectByKey<ItemsMovementGroup>((CurrentObject as ItemsMovementGroup).Oid);
            Warehouse whse = obj as Warehouse;
            var data = img.ItemsMovSelectedWhseDetails.Where(o => o.Whse == whse);
            ItemsMovSelectedWhseDetail itmswd = data.LastOrDefault();
            if (itmswd == null)
            {
                itmswd = new ItemsMovSelectedWhseDetail(checkedItems.Session);
                itmswd.TmgId = img;
                itmswd.Whse = checkedItems.Session.GetObjectByKey<Warehouse>((obj as Warehouse).Oid);
            }
            switch (e.State)
            {
                case CheckState.Checked:
                    checkedItems.BaseAdd(itmswd);
                    iPass++;
                    break;
                case CheckState.Unchecked:
                    checkedItems.BaseRemove(itmswd);
                    itmswd.Delete();
                    iPass++;
                    break;
                default:
                    break;
            }
        }
        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            this.application = application;
        }
    }
}
