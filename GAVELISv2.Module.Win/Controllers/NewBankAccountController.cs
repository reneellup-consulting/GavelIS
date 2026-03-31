using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class NewBankAccountController : ViewController
    {
        private SimpleAction newBankAccount;
        private NewObjectViewController newController;
        private DetailView newDetailView;
        public NewBankAccountController()
        {
            TargetObjectType = typeof(Account);
            //TargetViewId = "Account_ListView";
            newBankAccount = new SimpleAction(this, "New Bank Account",
            PredefinedCategory.Edit);
            newBankAccount.ToolTip = "Create a new Bank Account";
            newBankAccount.SelectionDependencyType = SelectionDependencyType.
            RequireSingleObject;
            newBankAccount.ImageName = "BO_Contact";
            newBankAccount.Execute += newBankAccount_Execute;
        }

        private void newBankAccount_Execute(object sender,
SimpleActionExecuteEventArgs e)
        {
            Frame workFrame = Application.CreateFrame(TemplateContext.
            ApplicationWindow);
            workFrame.SetView(Application
                .CreateDetailView(Application.CreateObjectSpace(),
                "Bank2_DetailView", true));
            newController = workFrame.GetController<NewObjectViewController>();
            if (newController != null)
            {
                ChoiceActionItem newObjectItem = FindNewObjectItem(typeof(
                Account));
                if (newObjectItem != null)
                {
                    newController.NewObjectAction.Executed +=
                    NewObjectAction_Executed;
                    newController.NewObjectAction.DoExecute(newObjectItem);
                    newController.NewObjectAction.Executed -=
                    NewObjectAction_Executed;
                    e.ShowViewParameters.TargetWindow = TargetWindow.Default;
                    e.ShowViewParameters.CreatedView = newDetailView;
                    //Cancel the default processing for this navigation item.
                    return;
                }
            }
        }
        private ChoiceActionItem FindNewObjectItem(Type type)
        {
            foreach (ChoiceActionItem item in newController.NewObjectAction.
            Items) { if (item.Data == type) { return item; } }
            return null;
        }
        private void NewObjectAction_Executed(object sender, ActionBaseEventArgs
        e)
        {
            newDetailView = e.ShowViewParameters.CreatedView as DetailView;
            //Cancel showing the default View by the NewObjectAction
            e.ShowViewParameters.CreatedView = null;
        }

    }
}
