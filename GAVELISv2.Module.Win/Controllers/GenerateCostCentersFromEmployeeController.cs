using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateCostCentersFromEmployeeController : ViewController {
        private SimpleAction GenCCentersFromEmployeesAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateCostCentersFromEmployeeController() {
            this.TargetObjectType = typeof(CostCenter);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.GenerateCostCentersFromEmployee", this.GetType().
            Name);
            this.GenCCentersFromEmployeesAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GenCCentersFromEmployeesAction.Caption = "Generate From Employees";
            this.GenCCentersFromEmployeesAction.ConfirmationMessage =
            "Before you continue generating Cost Centers from Employees, make sure that you know what you are doing.\r\n" +
            "Do you really want to continue?";
            this.GenCCentersFromEmployeesAction.Execute += new SimpleActionExecuteEventHandler(GenCCentersFromEmployeesAction_Execute);
        }

        void GenCCentersFromEmployeesAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            using (XPCollection<Employee> list = new XPCollection<Employee>(((ObjectSpace)ObjectSpace).Session))
            {
                if (list == null && list.Count == 0)
                {
                    throw new ApplicationException("There are no employees found");
                }
                _FrmProgress = new ProgressForm("Updating items...", list.Count,
                "Generating cost center {0} of {1} ");
                _FrmProgress.CancelClick += FrmProgressCancelClick;
                _BgWorker = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true
                };
                _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
                _BgWorker.ProgressChanged += BgWorkerProgressChanged;
                _BgWorker.DoWork += BgWorkerDoWork;
                _BgWorker.RunWorkerAsync(list);
                _FrmProgress.ShowDialog();
            }
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<Employee> emps = (XPCollection<Employee>)e.Argument;
            try
            {
                foreach (Employee item in emps)
                {
                    index++;
                    _message = string.Format("Generating cost center {0} succesfull.",
                    emps.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    Employee eObj = session.GetObjectByKey<Employee>(item.Oid);
                    CostCenter cctrs = session.FindObject<CostCenter>(CriteriaOperator.Parse("[Code] = ?",item.No));
                    if (cctrs==null)
                    {
                        cctrs = ReflectionHelper.CreateObject<CostCenter>(session);
                    }
                    cctrs.Code = eObj.No;
                    cctrs.Description = eObj.Name;
                    cctrs.PersonResponsible = eObj;
                    cctrs.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    System.Threading.Thread.Sleep(20);
                }               
            }
            finally
            {
                if (index == emps.Count)
                {
                    e.Result = index;
                    //CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }

        private UnitOfWork CreateUpdatingSession() {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }

        private void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }

        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                , new SessionEventArgs(session));
            }
        }

        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) {
            if
            (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                SessionEventArgs(session));
            }
        }

        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e) {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                DoProgress(e.ProgressPercentage);
            }
        }

        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                "Generating Cost Centers has been cancelled", "Cancelled",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
            } else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);
                } else
                {
                    XtraMessageBox.Show("All " + e.Result +
                    " Cost Centers are successfully generated");
                    //ObjectSpace.ReloadObject(invoice);
                    ObjectSpace.Refresh();
                }
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        private void UpdateActionState(bool inProgress) {
            this.GenCCentersFromEmployeesAction.
            Enabled.SetItemValue("Generating Cost Centers", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
