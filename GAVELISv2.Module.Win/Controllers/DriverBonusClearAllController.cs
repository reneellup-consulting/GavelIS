using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class DriverBonusClearAllController : ViewController
    {
        private SimpleAction driversBonusLoadDriversAction;
        private DriversBonusGeneratorHeader _DriversBonusGeneratorHeader;
        public DriverBonusClearAllController()
        {
            this.TargetObjectType = typeof(DriversBonusGeneratorHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverBonusClearAllId";
            this.driversBonusLoadDriversAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.driversBonusLoadDriversAction.Caption = "Clear All";
            this.driversBonusLoadDriversAction.Execute += new SimpleActionExecuteEventHandler(driversBonusLoadDriversAction_Execute);
        }

        void driversBonusLoadDriversAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _DriversBonusGeneratorHeader = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DriversBonusGeneratorHeader;

            // Remove all existing DriversEarningFtmDetail records
            foreach (var detail in _DriversBonusGeneratorHeader.DriversBonusGeneratorDetails.ToList())
            {
                detail.Delete();
            }

            ObjectSpace.CommitChanges();

            XtraMessageBox.Show(
                    "Clearing has been successfull.");
            ObjectSpace.Refresh();
        }
    }
}
