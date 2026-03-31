using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Reports;
using GAVELISv2.Module.BusinessObjects;
using System.IO;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TireInspectionPrintController : ViewController
    {
        private SimpleAction printTireInsWSAction;
        public TireInspectionPrintController()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            //this.TargetViewId = "Tire_ListView_Dettached";

            #region Print Tire Inspection Worksheet
            this.printTireInsWSAction = new SimpleAction(this, "Tire.PrintTireInspectionWS",
            PredefinedCategory.RecordEdit);
            this.printTireInsWSAction.Caption = "Print Ispection Worksheet";
            this.printTireInsWSAction.Execute += new SimpleActionExecuteEventHandler(printTireInsWSAction_Execute);
            #endregion
        }

        void printTireInsWSAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            XafReport rep = new XafReport();
            string path = Directory.GetCurrentDirectory() + @"\TireInspectionWorksheet.repx";
            IObjectSpace objs = View.ObjectSpace;
            rep.LoadLayout(path);
            rep.ObjectSpace = objs;
            XPCollection<Tire> xpc = new XPCollection<Tire>(((ObjectSpace)objs).Session)
            {
                LoadingEnabled = false
            };
            rep.DataSource = xpc;
            rep.ShowPreview();
        }
    }
}
