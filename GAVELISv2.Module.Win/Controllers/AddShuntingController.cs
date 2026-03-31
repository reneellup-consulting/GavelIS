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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class AddShuntingController : ViewController {
        public AddShuntingController() {
            this.TargetObjectType = typeof(ShuntingEntry);
            this.TargetViewType = ViewType.ListView;
        }
        protected override void OnActivated() {
            base.OnActivated();
            ((ListView)View).CreateCustomCurrentObjectDetailView += new 
            EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(
            AddShuntingController_CreateCustomCurrentObjectDetailView);
        }
        protected override void OnDeactivated() {
            ((ListView)View).CreateCustomCurrentObjectDetailView -= new 
            EventHandler<CreateCustomCurrentObjectDetailViewEventArgs>(
            AddShuntingController_CreateCustomCurrentObjectDetailView);
            base.OnDeactivated();
        }
        private void AddShuntingController_CreateCustomCurrentObjectDetailView(
        object sender, CreateCustomCurrentObjectDetailViewEventArgs e) {
            if (e.ListViewCurrentObject != null) {
                ShuntingEntry obj = (ShuntingEntry)e.ListViewCurrentObject;
                if (obj != null) {e.DetailViewId = "AddShunting_Detail";}
            }
        }
    }
}
