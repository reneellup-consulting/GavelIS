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
    public partial class TiremanActivityViewController : ViewController
    {
        public TiremanActivityViewController()
        {
            this.TargetObjectType = typeof(TiremanActivity);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "TiremanDaily_TiremanActivtyDetails_ListView";
            this.ViewControlsCreated += new EventHandler(TiremanActivityViewController_ViewControlsCreated);
        }

        void TiremanActivityViewController_ViewControlsCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }

        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            TiremanActivity ta = this.View.CurrentObject as TiremanActivity;
            switch (ta.TiremanActivityType)
            {
                case TiremanActivityTypeEnum.Attach:
                    DetailView view1 = Application.CreateDetailView(ObjectSpace, "TiremanActivity_Attach", false, ta);
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    e.ShowViewParameters.CreatedView = view1;
                    break;
                case TiremanActivityTypeEnum.Dettach:
                    DetailView view2 = Application.CreateDetailView(ObjectSpace, "TiremanActivity_Dettach", false, ta);
                    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                    e.ShowViewParameters.CreatedView = view2;
                    break;
                case TiremanActivityTypeEnum.Replace:
                    break;
                case TiremanActivityTypeEnum.Transfer:
                    break;
                default:
                    break;
            }
        }
    }
}
