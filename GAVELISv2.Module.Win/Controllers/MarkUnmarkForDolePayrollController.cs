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
    public partial class MarkUnmarkForDolePayrollController : ViewController
    {
        private SimpleAction markUnmarkForDolePayrollAction;
        private DriverRegistry _DriverRegistry;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public MarkUnmarkForDolePayrollController()
        {
            this.TargetObjectType = typeof(DriverRegistry);
            this.TargetViewType = ViewType.ListView;
            string actionID = "MarkUnmarkForDolePayrollActionId";
            this.markUnmarkForDolePayrollAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.markUnmarkForDolePayrollAction.TargetObjectsCriteria = "[TripID.SourceType.Code] = 'DF'";
            this.markUnmarkForDolePayrollAction.Caption = "Un/Mark DF Payroll";
            this.markUnmarkForDolePayrollAction.Execute += new SimpleActionExecuteEventHandler(MarkUnmarkForDolePayrollAction_Execute);
        }

        void MarkUnmarkForDolePayrollAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IList selectedObjects = this.View.SelectedObjects;
            if (selectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no lines selected.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Marking/Unmarking...", selectedObjects.Count,
                        "Line marked {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(selectedObjects);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                foreach (DriverRegistry item in trans)
                {
                    index++;
                    _message = string.Format("Marking source {0} succesfull.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    DriverRegistry objectByKey = session.GetObjectByKey<DriverRegistry>(item.Oid);
                    if (objectByKey.Status == DriverRegistryStatusEnum.Current)
                    {
                        if (objectByKey.DolePayroll != true)
                        {
                            objectByKey.DolePayroll = true;
                        }
                        else
                        {
                            objectByKey.DolePayroll = false;
                        }
                    }
                    objectByKey.Save();
                    #endregion

                    CommitUpdatingSession(session);

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }
            }
            finally
            {
                if (index == trans.Count)
                {
                    e.Result = index;
                }
                session.Dispose();
            }
        }
        private UnitOfWork CreateUpdatingSession()
        {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session)
        {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session)
        {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                    , new SessionEventArgs(session));
            }
        }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session)
        {
            if
                (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                    SessionEventArgs(session));
            }
        }
        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Marking is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Marking has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
