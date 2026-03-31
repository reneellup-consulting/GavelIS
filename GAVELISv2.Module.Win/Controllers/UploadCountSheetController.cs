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
    public partial class UploadCountSheetController : ViewController
    {
        private readonly SimpleAction _uploadAction;
        public UploadCountSheetController()
        {
            TargetObjectType = typeof(ItemsMovementGroup);
            TargetViewType = ViewType.DetailView;
            _uploadAction = new SimpleAction(this, "UploadCountSheetId", PredefinedCategory.RecordEdit)
            {
                Caption = "Upload Count Sheet",
            };
            _uploadAction.Execute += UploadAction_Execute;
        }
        protected override void OnActivated()
        {
            if (View.ObjectTypeInfo.IsAbstract)
                _uploadAction.Active.SetItemValue(@"test", false);

            base.OnActivated();
        }

        private void UploadAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var objectSpace = Application.CreateObjectSpace();

            ShowUploadForm(objectSpace);
            View.Refresh();
            View.ObjectSpace.Refresh();
        }

        public void ShowUploadForm(IObjectSpace objectSpace)
        {
            var frm = new UploadCountSheetForm(objectSpace, View.ObjectTypeInfo, Helper.GetCurrentCollectionSource(this), Application);
            frm.ShowDialog();
        }
    }
}
