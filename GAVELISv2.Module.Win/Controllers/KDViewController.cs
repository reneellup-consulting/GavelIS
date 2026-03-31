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
    public partial class KDViewController : ViewController
    {
        public KDViewController()
        {
            this.TargetObjectType = typeof(KDEntry);
            this.TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated() {
            base.OnActivated();
            ((ListView)View).CreateCustomCurrentObjectDetailView += new 
            EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(
            KDViewController_CreateCustomCurrentObjectDetailView);
        }
        protected override void OnDeactivated() {
            ((ListView)View).CreateCustomCurrentObjectDetailView -= new 
            EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(
            KDViewController_CreateCustomCurrentObjectDetailView);
            base.OnDeactivated();
        }
        private void KDViewController_CreateCustomCurrentObjectDetailView(
        object sender, CreateCustomCurrentObjectDetailViewEventArgs e) {
            if (e.ListViewCurrentObject != null) {
                KDEntry obj = (KDEntry)e.ListViewCurrentObject;
                if (obj != null) {e.DetailViewId = "AddKD_Detail";}
            }
        }
    }
}
