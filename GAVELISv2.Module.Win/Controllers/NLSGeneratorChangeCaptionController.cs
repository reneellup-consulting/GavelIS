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
    public partial class NLSGeneratorChangeCaptionController : ObjectViewController
    {
        public NLSGeneratorChangeCaptionController()
        {
            TargetObjectType = typeof(NLSGenerationDetail);
            TargetViewType = ViewType.DetailView;
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateCaptions("ForVendor01");
            UpdateCaptions("ForVendor02");
            UpdateCaptions("ForVendor03");
            UpdateCaptions("ForVendor04");
            UpdateCaptions("ForVendor05");
            UpdateCaptions("ForVendor06");
            UpdateCaptions("ForVendor07");
            UpdateCaptions("ForVendor08");
            UpdateCaptions("ForVendor09");
            UpdateCaptions("ForVendor10");
            ObjectSpace.ObjectChanged += new EventHandler<ObjectChangedEventArgs>(ObjectSpace_ObjectChanged);
        }
        void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor01" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor01");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor02" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor02");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor03" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor03");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor04" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor04");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor05" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor05");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor06" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor06");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor07" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor07");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor08" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor08");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor09" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor09");
            }
            if (e.Object == View.CurrentObject && e.PropertyName == "ForVendor10" && e.NewValue != null)
            {
                UpdateCaptions("ForVendor10");
            }
        }
        protected void UpdateCaptions(string name)
        {
            NLSGenerationDetail dt = View.CurrentObject as NLSGenerationDetail;
            if (name == "ForVendor01")
            {
                string caption = dt.MainId.VendorCaption01;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor01");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor02")
            {
                string caption = dt.MainId.VendorCaption02;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor02");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor03")
            {
                string caption = dt.MainId.VendorCaption03;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor03");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor04")
            {
                string caption = dt.MainId.VendorCaption04;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor04");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor05")
            {
                string caption = dt.MainId.VendorCaption05;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor05");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor06")
            {
                string caption = dt.MainId.VendorCaption06;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor06");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor07")
            {
                string caption = dt.MainId.VendorCaption07;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor07");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor08")
            {
                string caption = dt.MainId.VendorCaption08;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor08");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor09")
            {
                string caption = dt.MainId.VendorCaption09;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor09");
                SetPropertyEditorCaption(propertyEditor, caption);
            }
            if (name == "ForVendor10")
            {
                string caption = dt.MainId.VendorCaption10;
                PropertyEditor propertyEditor = (PropertyEditor)View.FindItem("ForVendor10");
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
