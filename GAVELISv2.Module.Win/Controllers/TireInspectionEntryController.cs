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

namespace GAVELISv2.Module.Win.Controllers {

    #region Tire Dettached All

    public partial class TireInspectionEntryController : ViewController {
        public TireInspectionEntryController() {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Tire_ListView_Dettached_All";
            this.ViewControlsCreated += new EventHandler(TireInspectionEntryController_ViewControlsCreated);
        }

        void TireInspectionEntryController_ViewControlsCreated(object sender, EventArgs e) {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }

        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e) {
            IObjectSpace objs = Application.CreateObjectSpace();
            Tire tr = this.View.CurrentObject as Tire;
            Tire thisTire = objs.GetObjectByKey<Tire>(tr.Oid);
            DetailView view = Application.CreateDetailView(objs, "Tire_InspectionEntry", true, thisTire);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }
    }

    #endregion

    #region Tire Dettach Recap
    public partial class TireInspectionEntryController_Recap : ViewController
    {
        public TireInspectionEntryController_Recap()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Tire_ListView_Dettached_OnRecapping";
            this.ViewControlsCreated += new EventHandler(TireInspectionEntryController_ViewControlsCreated);
        }

        void TireInspectionEntryController_ViewControlsCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }

        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            IObjectSpace objs = Application.CreateObjectSpace();
            Tire tr = this.View.CurrentObject as Tire;
            Tire thisTire = objs.GetObjectByKey<Tire>(tr.Oid);
            DetailView view = Application.CreateDetailView(objs, "Tire_InspectionEntry", true, thisTire);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }
    }

    #endregion

    #region Tire Dettach Regroove
    public partial class TireInspectionEntryController_Regrooved : ViewController
    {
        public TireInspectionEntryController_Regrooved()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Tire_ListView_Dettached_OnRegrooving";
            this.ViewControlsCreated += new EventHandler(TireInspectionEntryController_ViewControlsCreated);
        }

        void TireInspectionEntryController_ViewControlsCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }

        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            IObjectSpace objs = Application.CreateObjectSpace();
            Tire tr = this.View.CurrentObject as Tire;
            Tire thisTire = objs.GetObjectByKey<Tire>(tr.Oid);
            DetailView view = Application.CreateDetailView(objs, "Tire_InspectionEntry", true, thisTire);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }
    }

    #endregion

    #region Tire Dettach Repair
    public partial class TireInspectionEntryController_Repair : ViewController
    {
        public TireInspectionEntryController_Repair()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Tire_ListView_Dettached_OnRepair";
            this.ViewControlsCreated += new EventHandler(TireInspectionEntryController_ViewControlsCreated);
        }

        void TireInspectionEntryController_ViewControlsCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }

        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            IObjectSpace objs = Application.CreateObjectSpace();
            Tire tr = this.View.CurrentObject as Tire;
            Tire thisTire = objs.GetObjectByKey<Tire>(tr.Oid);
            DetailView view = Application.CreateDetailView(objs, "Tire_InspectionEntry", true, thisTire);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }
    }

    #endregion
}
