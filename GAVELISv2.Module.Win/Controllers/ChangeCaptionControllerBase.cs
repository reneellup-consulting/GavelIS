using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ChangeCaptionControllerBase : ObjectViewController
    {
        public ChangeCaptionControllerBase()
        {
            //InitializeComponent();
            //RegisterActions(components);
            TargetObjectType = typeof(TruckIncomeExpenseDetail);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateCaptions("Expense01");
            UpdateCaptions("Expense02");
            UpdateCaptions("Expense03");
            UpdateCaptions("Expense04");
            UpdateCaptions("Expense05");
            UpdateCaptions("Expense06");
            UpdateCaptions("Expense07");
            UpdateCaptions("Expense08");
            UpdateCaptions("Expense09");
            UpdateCaptions("Expense10");
            UpdateCaptions("Expense11");
            UpdateCaptions("Expense12");
            UpdateCaptions("Expense13");
            UpdateCaptions("Expense14");
            UpdateCaptions("Expense15");
            UpdateCaptions("Expense16");
            UpdateCaptions("Expense17");
            ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
        }
        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense01" && e.NewValue != null)
            {
                UpdateCaptions("Expense01");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense02" && e.NewValue != null)
            {
                UpdateCaptions("Expense02");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense03" && e.NewValue != null)
            {
                UpdateCaptions("Expense03");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense04" && e.NewValue != null)
            {
                UpdateCaptions("Expense04");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense05" && e.NewValue != null)
            {
                UpdateCaptions("Expense05");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense06" && e.NewValue != null)
            {
                UpdateCaptions("Expense06");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense07" && e.NewValue != null)
            {
                UpdateCaptions("Expense07");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense08" && e.NewValue != null)
            {
                UpdateCaptions("Expense08");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense09" && e.NewValue != null)
            {
                UpdateCaptions("Expense09");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense10" && e.NewValue != null)
            {
                UpdateCaptions("Expense10");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense11" && e.NewValue != null)
            {
                UpdateCaptions("Expense11");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense12" && e.NewValue != null)
            {
                UpdateCaptions("Expense12");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense13" && e.NewValue != null)
            {
                UpdateCaptions("Expense13");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense14" && e.NewValue != null)
            {
                UpdateCaptions("Expense14");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense15" && e.NewValue != null)
            {
                UpdateCaptions("Expense15");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense16" && e.NewValue != null)
            {
                UpdateCaptions("Expense16");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "Expense17" && e.NewValue != null)
            {
                UpdateCaptions("Expense17");
            }
        }
        protected void UpdateCaptions(string name)
        {
            TruckIncomeExpenseDetail dt = View.CurrentObject as TruckIncomeExpenseDetail;
            if (name == "Expense01")
            {
                string caption = dt.MainId.ExpenseCaption01;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense01");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense02")
            {
                string caption = dt.MainId.ExpenseCaption02;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense02");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense03")
            {
                string caption = dt.MainId.ExpenseCaption03;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense03");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense04")
            {
                string caption = dt.MainId.ExpenseCaption04;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense04");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense05")
            {
                string caption = dt.MainId.ExpenseCaption05;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense05");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense06")
            {
                string caption = dt.MainId.ExpenseCaption06;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense06");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense07")
            {
                string caption = dt.MainId.ExpenseCaption07;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense07");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense08")
            {
                string caption = dt.MainId.ExpenseCaption08;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense08");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense09")
            {
                string caption = dt.MainId.ExpenseCaption09;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense09");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense10")
            {
                string caption = dt.MainId.ExpenseCaption10;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense10");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense11")
            {
                string caption = dt.MainId.ExpenseCaption11;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense11");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense12")
            {
                string caption = dt.MainId.ExpenseCaption12;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense12");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense13")
            {
                string caption = dt.MainId.ExpenseCaption13;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense13");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense14")
            {
                string caption = dt.MainId.ExpenseCaption14;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense14");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense15")
            {
                string caption = dt.MainId.ExpenseCaption15;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense15");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense16")
            {
                string caption = dt.MainId.ExpenseCaption16;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense16");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "Expense17")
            {
                string caption = dt.MainId.ExpenseCaption17;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("Expense17");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
        }
        protected virtual void SetPropertyEditorCaption(PropertyEditor propertyEditor, string caption)
        {
            propertyEditor.Caption = caption;
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            ObjectSpace.ObjectChanged -= new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
        }
    }
}
