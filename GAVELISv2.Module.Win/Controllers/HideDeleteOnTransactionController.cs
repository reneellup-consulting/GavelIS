using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class HideDeleteOnTransactionController : ViewController
    {
        public HideDeleteOnTransactionController()
        {
            InitializeComponent();
            RegisterActions(components);
        }
        protected override void OnActivated()
        {

            base.OnActivated();

            //if (View != null && View.ObjectTypeInfo != null && View.ObjectTypeInfo.Type == typeof(GenJournalHeader))
            //{

            //    Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().DeleteAction.Active.SetItemValue("Active", false);
            //}

        }
        protected override void OnDeactivated()
        {
            //if (View is ListView && !View.IsRoot && View.ObjectTypeInfo.Type == typeof(Invoice))
            //{

            //    Frame.GetController<DevExpress.ExpressApp.SystemModule.DeleteObjectsViewController>().DeleteAction.Active.RemoveItem("Active");
            //}
            base.OnDeactivated();
        }
    }
}
