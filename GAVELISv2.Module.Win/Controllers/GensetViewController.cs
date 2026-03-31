using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GensetViewController : ViewController
    {
        public GensetViewController()
        {
            this.TargetObjectType = typeof(GensetEntry);
            this.TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated() {
            base.OnActivated();
            ((ListView)View).CreateCustomCurrentObjectDetailView += new 
            EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(
            GensetViewController_CreateCustomCurrentObjectDetailView);
        }
        protected override void OnDeactivated() {
            ((ListView)View).CreateCustomCurrentObjectDetailView -= new 
            EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(
            GensetViewController_CreateCustomCurrentObjectDetailView);
            base.OnDeactivated();
        }
        private void GensetViewController_CreateCustomCurrentObjectDetailView(
        object sender, CreateCustomCurrentObjectDetailViewEventArgs e) {
            if (e.ListViewCurrentObject != null) {
                GensetEntry obj = (GensetEntry)e.ListViewCurrentObject;
                if (obj != null) {e.DetailViewId = "GensetEntry_Detail";}
            }
        }
    }
}
