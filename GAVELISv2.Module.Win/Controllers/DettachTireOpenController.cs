using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class DettachTireOpenController : ViewController {
        //private Frame _WorkFrame;
        private SimpleAction openDettachTire;
        private DetailView detailView;
        public DettachTireOpenController() {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            //this.TargetViewId = "Tire_ListView_Dettached";
            openDettachTire = new SimpleAction(this, "OpenDettachTire",
            PredefinedCategory.OpenObject);
            openDettachTire.Caption = "Open Tire";
            openDettachTire.ImageName = "Action_Open";
            openDettachTire.SelectionDependencyType = SelectionDependencyType.
            RequireSingleObject;
            openDettachTire.Execute += new SimpleActionExecuteEventHandler(openDettachTire_Execute);
        }

        void openDettachTire_Execute(object sender, SimpleActionExecuteEventArgs e) {
            IObjectSpace objs = Application.CreateObjectSpace();
            Tire tr = this.View.CurrentObject as Tire;
            Tire thisTire = objs.GetObjectByKey<Tire>(tr.Oid);
            detailView = Application
            .CreateDetailView(objs,
            "Tire_DetailView", true, thisTire);
            //_WorkFrame = Application.CreateFrame(TemplateContext.ApplicationWindow);
            //_WorkFrame.SetView(Application
            //.CreateDetailView(objs,
            //"Tire_DetailView", true, thisTire));
            //ListViewProcessCurrentObjectController controller = _WorkFrame.GetController<ListViewProcessCurrentObjectController>();
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = detailView;
        }
    }
}
