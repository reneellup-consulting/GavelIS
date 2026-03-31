using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using Xpand.ExpressApp.ImportWizard;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class UploadCorrectionWorksheetController : ViewController
    {
        private readonly SimpleAction _updateAction;
        public UploadCorrectionWorksheetController()
        {
            TargetObjectType = typeof(IncomeAndExpense02);
            TargetViewType = ViewType.ListView;
            _updateAction = new SimpleAction(this, "UploadCorrectionWorksheetId", PredefinedCategory.RecordEdit)
            {
                Caption = "Upload Correction File",
            };
            _updateAction.Execute += UpdateAction_Execute;
        }
        protected override void OnActivated()
        {
            if (View.ObjectTypeInfo.IsAbstract)
                _updateAction.Active.SetItemValue(@"test", false);

            base.OnActivated();
        }

        private void UpdateAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();

            ShowUploadForm(objectSpace);
            View.Refresh();
            View.ObjectSpace.Refresh();
        }

        public void ShowUploadForm(IObjectSpace objectSpace)
        {
            var frm = new UploadCorrectionWorksheetForm(objectSpace, View.ObjectTypeInfo, Helper.GetCurrentCollectionSource(this), Application);
            frm.ShowDialog();
        }
    }
}
