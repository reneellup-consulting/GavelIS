using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.ExpressApp.Reports;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
using System.IO;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class FleetTirePrintController : ViewController
    {
        private IObjectSpace _Objs;
        private XafReport report;
        private PopupWindowShowAction printIncompleteAction;
        public FleetTirePrintController() {
            this.TargetObjectType = typeof(FixedAsset);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Fleet_ListView";

            #region Print Incomplete Tires

            printIncompleteAction = new PopupWindowShowAction(this,"Fleet.PrintIncompleteAction", PredefinedCategory.RecordEdit);
            printIncompleteAction.Caption = "Print Incomplete Tires";
            printIncompleteAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(printIncompleteAction_CustomizePopupWindowParams);
            printIncompleteAction.Execute += new PopupWindowShowActionExecuteEventHandler(printIncompleteAction_Execute);
            #endregion

        }

        void printIncompleteAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _Objs = Application.CreateObjectSpace();
            report = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\FleetwithIncompleteTires.repx";
            report.LoadLayout(path);
            report.ObjectSpace = _Objs;
            XPCollection<FixedAsset> xpc = new XPCollection<FixedAsset>(((ObjectSpace)_Objs).Session) { LoadingEnabled = false };
            report.DataSource = xpc;
            e.View = report.CreateParametersDetailView(Application);
        }

        void printIncompleteAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            //report.CreateDocument();
            report.ShowPreview();
        }

        //void printIncompleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        //{
        //    XafReport rep = new XafReport();
        //    rep.LoadLayout("FleetwithIncompleteTires.repx");
        //    rep.ObjectSpace = View.ObjectSpace;
        //    XPCollection<FixedAsset> xpc = new XPCollection<FixedAsset>(((ObjectSpace)View.ObjectSpace).Session) { LoadingEnabled = false };
        //    rep.DataSource = xpc;
        //    rep.CreateParametersDetailView(Application);
        //    FleetTireReportParam param = rep.ReportParametersObject as FleetTireReportParam;
            
        //    rep.ShowPreview();
        //}
    }
}
