using System;
using System.Linq;
using DevExpress.ExpressApp;
using System.Collections.Generic;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class HideItemWindowController : WindowController
    {
        private ShowNavigationItemController navigationController;
        public HideItemWindowController()
        {
            TargetWindowType = WindowType.Main;
        }
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            navigationController = Frame.GetController<ShowNavigationItemController>();
            if (navigationController != null)
            {
                navigationController.ItemsInitialized += new EventHandler<EventArgs>(HideItemWindowController_ItemsInitialized);
            }
        }
        protected override void OnDeactivated()
        {
            if (navigationController != null)
            {
                navigationController.ItemsInitialized -= new EventHandler<EventArgs>(HideItemWindowController_ItemsInitialized);
                navigationController = null;
            }
            base.OnDeactivated();
        }
        private void HideItemWindowController_ItemsInitialized(object sender, EventArgs e)
        {
            SecurityUser currentUser = SecuritySystem.CurrentUser as SecurityUser;
            if (currentUser != null)
            {
                if (currentUser.Roles.Count > 0)
                {
                    var role1 = (from rol in currentUser.Roles
                                where rol.Name == "IncomeAndExpenseOneViewer"
                                select rol).FirstOrDefault();
                    if (role1==null)
                    {
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "IncomeAndExpense_ListView");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "IncomeVsExpense");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@ea76e879-079d-41dc-bd1e-4892de04214c");
                    }

                    var role2 = (from rol in currentUser.Roles
                                 where rol.Name == "IncomeAndExpenseTwoViewer"
                                select rol).FirstOrDefault();
                    if (role2 == null)
                    {
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "IncomeAndExpense_ListView_Copy");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@b4a47d31-3b5c-4b1c-8b83-68f26f027934");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@c8c962db-cd4f-4460-ade5-2c05575fc70f");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@c3bfa7ac-f74c-4b01-91b5-7895c2285fca");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@9f3c998e-dc9b-482a-b472-331d8f78b9f0");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@3b523a27-585b-4118-a058-41a4779b071a");
                        HideItemByCaption(navigationController.ShowNavigationItemAction.Items, "@ea76e879-079d-41dc-bd1e-4892de04214c_02");
                    }
                }
            }
        }
        private void HideItemByCaption(ChoiceActionItemCollection items, string navigationItemId)
        {
            foreach (ChoiceActionItem item in items)
            {
                if (item.Id == navigationItemId)
                {
                    item.Active["InactiveForUsersRole"] = false;
                    return;
                }
                HideItemByCaption(item.Items, navigationItemId);
            }
        }
    }
}
