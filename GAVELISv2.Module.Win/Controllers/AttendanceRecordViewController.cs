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
    public partial class AttendanceRecordViewController : ViewController
    {
        public AttendanceRecordViewController()
        {
            this.TargetObjectType = typeof(AttendanceRecord);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "StaffPayrollBatch_AttendanceRecords_ListView";
            this.ViewControlsCreated += new EventHandler(AttendanceRecordViewController_ViewControlsCreated);
        }
        void AttendanceRecordViewController_ViewControlsCreated(object sender, EventArgs e)
        {
            ListViewProcessCurrentObjectController controller = Frame.GetController<ListViewProcessCurrentObjectController>();
            controller.CustomizeShowViewParameters += new EventHandler<CustomizeShowViewParametersEventArgs>(controller_CustomizeShowViewParameters);
        }
        void controller_CustomizeShowViewParameters(object sender, CustomizeShowViewParametersEventArgs e)
        {
            AttendanceRecord ta = this.View.CurrentObject as AttendanceRecord;
            DetailView view1 = Application.CreateDetailView(ObjectSpace, "AttendanceRecord_DetailView", false, ta);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view1;
        }
    }
}
