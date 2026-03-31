using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Repository;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ParameterizedActionWindowController : WindowController
    {
        private static bool isActive;
        protected override void OnActivated()
        {
            base.OnActivated();
            if (!isActive)
            {
                BarActionItemsFactory.CustomizeActionControl += new EventHandler<CustomizeActionControlEventArgs>(BarActionItemsFactory_CustomizeActionControl);
                isActive = true;
            }
        }
        protected override void OnDeactivated()
        {
            BarActionItemsFactory.CustomizeActionControl -= new EventHandler<CustomizeActionControlEventArgs>(BarActionItemsFactory_CustomizeActionControl);
            isActive = true;
            base.OnDeactivated();
        }
        void BarActionItemsFactory_CustomizeActionControl(object sender, CustomizeActionControlEventArgs e)
        {
            if (e.Action.Id == "ChangeCompTemplateStfActionId")
            {
                BarEditItem barEditItem = (BarEditItem)e.ActionControl.Control;
                barEditItem.Width = 50;
            }
            if (e.Action.Id == "SetPdWkStanfilcoActionId")
            {
                BarEditItem barEditItem = (BarEditItem)e.ActionControl.Control;
                barEditItem.Width = 50;
            }
        }
    }
}
