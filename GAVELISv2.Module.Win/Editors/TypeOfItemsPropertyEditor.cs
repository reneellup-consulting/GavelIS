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
    public class TypeOfItemsPropertyEditor : WinPropertyEditor, IComplexPropertyEditor
    {
        public TypeOfItemsPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model) { }
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
                XPCollection dataSource = new XPCollection(checkedItems.Session, typeof(ItemType));
                IModelClass classInfo = application.Model.BOModel.GetClass(typeof(ItemType));
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
                        Control.SetItemChecked(dataSource.IndexOf((obj as PdReqReportItemSelection).TypeOfItem), true);
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
            PeriodicRequisitionReport prr = CurrentObject as PeriodicRequisitionReport;
            //if (CurrentObject.GetType() == typeof(PeriodicRequisitionReport))
            //{
            //    tr = checkedItems.Session.GetObjectByKey<TireServiceDetail2>((CurrentObject as Tire).LastDetail.Oid);
            //}
            //else
            //{
            //    tr = checkedItems.Session.GetObjectByKey<TireServiceDetail2>((CurrentObject as TireServiceDetail2).Oid);
            //}
            ItemType toi = obj as ItemType;
            var data = prr.PdReqReportItemSelections.Where(o => o.TypeOfItem == toi);
            PdReqReportItemSelection prid = data.LastOrDefault();
            if (prid == null)
            {
                prid = new PdReqReportItemSelection(checkedItems.Session);
                prid.PerReqId = prr;
                prid.TypeOfItem = checkedItems.Session.GetObjectByKey<ItemType>((obj as ItemType).Oid);
            }
            switch (e.State)
            {
                case CheckState.Checked:
                    checkedItems.BaseAdd(prid);
                    iPass++;
                    break;
                case CheckState.Unchecked:
                    checkedItems.BaseRemove(prid);
                    prid.Delete();
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
