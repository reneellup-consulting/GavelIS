using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

namespace GAVELISv2.Module {
    public class ShowDetailViewFromListViewController : ViewController<ListView> {
        public const string EnabledKeyShowDetailView = "ShowDetailViewFromListViewController";
        protected override void OnActivated() {
            base.OnActivated();
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            if (controller != null) {
				IModelShowDetailView modelShowDetailView = View.Model as IModelShowDetailView;
				controller.ProcessCurrentObjectAction.Enabled[EnabledKeyShowDetailView] = modelShowDetailView == null ? true : modelShowDetailView.ShowDetailView;
            }
        }
    }
}