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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class ChartOfAccountController : ViewController {
        private SimpleAction showAccount;
        private ListViewProcessCurrentObjectController 
        processCurrentObjectController;
        //private DetailView accountDetailView;
        private Account _Account;
        private Frame workFrame;
        public ChartOfAccountController() {
            TargetObjectType = typeof(Account);
            TargetViewType = ViewType.ListView;
            //TargetViewId = "Account_ListView";
            showAccount = new SimpleAction(this, "Open Account", 
            PredefinedCategory.Edit);
            showAccount.ToolTip = "Open the selected account";
            showAccount.SelectionDependencyType = SelectionDependencyType.
            RequireSingleObject;
            showAccount.ImageName = "BO_Contact";
            showAccount.Execute += ShowAccount_Execute;
        }
        void ShowAccount_Execute(object sender, SimpleActionExecuteEventArgs e) 
        {
            workFrame = Application.CreateFrame(TemplateContext.
            ApplicationWindow);
            _Account = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject 
            as Account;
            if (_Account.AccountType.Code == "B") {workFrame.SetView(Application
                .CreateDetailView(Application.CreateObjectSpace(), 
                "Bank2_DetailView", true));} else {
                workFrame.SetView(Application.CreateDetailView(Application.
                CreateObjectSpace(), "Account_DetailView", true));
            }
            e.ShowViewParameters.TargetWindow = TargetWindow.Default;
            //e.ShowViewParameters.CreatedView = accountDetailView;
            ListViewProcessCurrentObjectController.ShowObject(_Account, e.
            ShowViewParameters, this.Application, workFrame, workFrame.View);
            return;
        }
        protected override void OnActivated() {
            base.OnActivated();
            processCurrentObjectController = Frame.GetController<
            ListViewProcessCurrentObjectController>();
            if (processCurrentObjectController != null) {
                processCurrentObjectController.CustomProcessSelectedItem += 
                processCurrentObjectController_CustomProcessSelectedItem;}
        }
        private void processCurrentObjectController_CustomProcessSelectedItem(
        object sender, CustomProcessListViewSelectedItemEventArgs e) {
            e.Handled = true;
            if (View.Id != "Account_LookupListView") {showAccount.
                DoExecute();}
        }
        public SimpleAction DefaultListViewAction {
            get { return showAccount; }
            set { showAccount = value; }
        }
    }
}
