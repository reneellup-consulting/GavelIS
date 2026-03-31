using System;
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using System.Reflection;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module {
    public interface IModelDefaultShowDetailView : IModelNode
    {
        [DefaultValue(true)]
        bool DefaultShowDetailViewFromListView { get; set; }
    }
    public interface IModelShowDetailView : IModelNode
    {
        bool ShowDetailView { get; set; }
    }
    [DomainLogic(typeof(IModelShowDetailView))]
    public static class ModelShowDetailViewLogic
    {
        public static bool Get_ShowDetailView(IModelShowDetailView showDetailView)
        {
            IModelDefaultShowDetailView defaultShowDetailViewFromListView = showDetailView.Parent as IModelDefaultShowDetailView;
            if (defaultShowDetailViewFromListView != null)
            {
                return defaultShowDetailViewFromListView.DefaultShowDetailViewFromListView;
            }
            return true;
        }
    }

    public sealed partial class GAVELISv2Module : ModuleBase {
        public GAVELISv2Module() { InitializeComponent();
        this.RequiredModuleTypes.Add(typeof(DevExpress.ExpressApp.Workflow.WorkflowModule));
        }
        public override void ExtendModelInterfaces(ModelInterfaceExtenders 
        extenders) {
            base.ExtendModelInterfaces(extenders);
            extenders.Add<IModelViews, IModelDefaultShowDetailView>();
            extenders.Add<IModelListView, IModelShowDetailView>();
            extenders.Add<IModelListView, IModelListViewExtender>();
            extenders.Add<IModelColumn, IModelColumnExtender>();
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.CustomProcessShortcut += new EventHandler<CustomProcessShortcutEventArgs>(application_CustomProcessShortcut);
        }

        void application_CustomProcessShortcut(object sender, CustomProcessShortcutEventArgs e)
        {
            if (e.Shortcut.ViewId == "AdjustItemCostPrices_ListView")
            {
                IObjectSpace objectSpace = Application.CreateObjectSpace();
                AdjustItemCostPrices adjustItemCostPrices = new AdjustItemCostPrices();
                adjustItemCostPrices.MarkupRate = 0;
                e.View = Application.CreateDetailView(objectSpace, adjustItemCostPrices, true);
                e.View.AllowEdit["CanEditAdjustItemCostPrices"] = true;
                e.Handled = true;
            }
        }
    }
}
