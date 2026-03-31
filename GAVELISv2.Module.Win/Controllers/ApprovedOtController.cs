using System;
using System.Linq;
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
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ApprovedOtController : ViewController
    {
        private SimpleAction approvedOtAction;
        //private CheckInAndOut03 _Clock;
        public ApprovedOtController()
        {
            this.TargetObjectType = typeof(CheckInAndOut03);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ApprovedOtActionId";
            this.approvedOtAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.approvedOtAction.Caption = "Approve OT";
            this.approvedOtAction.Execute += new SimpleActionExecuteEventHandler(approvedOtAction_Execute);
        }

        void approvedOtAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IObjectSpace _ObjectSpace = this.View.ObjectSpace; //Application.CreateObjectSpace();
            foreach (var item in this.View.SelectedObjects)
            {
                CheckInAndOut03 objectVar = _ObjectSpace.GetObject<CheckInAndOut03>(item as CheckInAndOut03);
                if (!new[] { "Absent", "Overtime", "Holiday OT", "Rest Day OT", "Holiday RGOT", "Holiday DBOT", "Holiday SPOT" }.Any(o => objectVar.Remarks.Contains(o)))
                {
                    throw new ApplicationException("The selected line is not an overtime entry.");
                }
                objectVar.ApprovedOt = true;
                objectVar.DisapprovedOt = false;
                objectVar.Save();
            }
            _ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }
    }
}
