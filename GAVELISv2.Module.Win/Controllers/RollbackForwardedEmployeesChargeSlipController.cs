using System;
using System.Windows.Forms;
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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class RollbackForwardedEmployeesChargeSlipController : ViewController
    {
        private EmployeeChargeSlip chargeSlip;
        private SimpleAction rollbackForwardedToPayrollAction;
        //private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public RollbackForwardedEmployeesChargeSlipController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("RollbackForwardToPayroll", this.GetType().Name);
            this.rollbackForwardedToPayrollAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.rollbackForwardedToPayrollAction.Caption = "Rollback Forward To Payroll";
            this.rollbackForwardedToPayrollAction.Execute += new 
            SimpleActionExecuteEventHandler(RollbackForwardToPayrollAction_Execute);
            this.rollbackForwardedToPayrollAction.Executed += new EventHandler<
            ActionBaseEventArgs>(RollbackForwardToPayroll_Executed);
            this.rollbackForwardedToPayrollAction.ConfirmationMessage = 
            "Do you really want to rollback this Employees Charge Slip?";
            //UpdateActionState(false);
        }
        private void RollbackForwardToPayrollAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            //invoice = ((DevExpress.ExpressApp.DetailView)this.View).
            //CurrentObject as Invoice;
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.DetailView))
            {
                chargeSlip = ((DevExpress.ExpressApp.DetailView)this.View).
                CurrentObject as EmployeeChargeSlip;
            }
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.ListView))
            {
                chargeSlip = this.View.CurrentObject as EmployeeChargeSlip;
            }
            ObjectSpace.CommitChanges();

            if (chargeSlip.Status!=EmployeeChargeSlipStatusEnum.ForwardedToPayroll)
            {
                throw new ApplicationException("Cannot rollback unforwarded charge slip");
            }

            string critString = string.Empty;
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;

            // Delete Inventory Control Journal

            ICollection icjCols;
            DevExpress.Xpo.Metadata.XPClassInfo icjClass;
            icjClass = chargeSlip.Session.GetClassInfo(typeof(
            InventoryControlJournal));
            criteria = CriteriaOperator.Parse(string.Format("[GenJournalID.SourceNo] ='{0}'", chargeSlip.SourceNo));
            sortProps = new SortingCollection(null);
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, chargeSlip.Session.TypesManager);
            icjCols = chargeSlip.Session.GetObjects(icjClass
            , criteria, sortProps, 0, false, true);
            if (icjCols.Count > 0)
            {
                foreach (InventoryControlJournal icj in icjCols)
                {
                    icj.Delete();
                }
            }
            // Delete Journal Entries
            ICollection jeCols;
            DevExpress.Xpo.Metadata.XPClassInfo jeClass;
            jeClass = chargeSlip.Session.GetClassInfo(typeof(
            GenJournalDetail));
            criteria = CriteriaOperator.Parse(string.Format("[GenJournalID.SourceNo] ='{0}'", chargeSlip.SourceNo));
            sortProps = new SortingCollection(null);
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, chargeSlip.Session.TypesManager);
            jeCols = chargeSlip.Session.GetObjects(jeClass
            , criteria, sortProps, 0, false, true);
            if (jeCols.Count > 0)
            {
                foreach (GenJournalDetail gjd in jeCols)
                {
                    gjd.Delete();
                }
            }
            // Deduction Entries
            EmpOtherDed eod = chargeSlip.PayrollDeductionRef;
            chargeSlip.PayrollDeductionRef = null;
            eod.Delete();
            // Change Status from Forwarded to Current
            chargeSlip.Status = EmployeeChargeSlipStatusEnum.Current;
            chargeSlip.Save();
            chargeSlip.Session.CommitTransaction();
        }

        private void RollbackForwardToPayroll_Executed(object sender,
ActionBaseEventArgs e)
        {
            ObjectSpace.CommitChanges();
            ObjectSpace.ReloadObject(chargeSlip);
            ObjectSpace.Refresh();
        }

    }
}
