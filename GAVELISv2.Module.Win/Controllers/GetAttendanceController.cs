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
    public partial class GetAttendanceController : ViewController
    {
        private SimpleAction GetAttendanceAction;
        private StaffPayrollBatch _StaffPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public GetAttendanceController()
        {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GetAttendanceActionId";
            this.GetAttendanceAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.GetAttendanceAction.Caption = "Get Attendance";
            this.GetAttendanceAction.Execute += new SimpleActionExecuteEventHandler(GetAttendanceAction_Execute);
        }

        void GetAttendanceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _StaffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
CurrentObject as StaffPayrollBatch;
            try
            {
                for (int i = _StaffPayrollBatch.CalculatedAttendance.Count - 1; i >= 0; i--)
                {
                    _StaffPayrollBatch.CalculatedAttendance.Remove(_StaffPayrollBatch.CalculatedAttendance[i]);
                }
            }
            catch (Exception)
            {
            }
            ObjectSpace.CommitChanges();
            var collection = new XPCollection<CheckInAndOut03>(((ObjectSpace)ObjectSpace).Session)
            {
                Criteria = CriteriaOperator.Parse("[Date] >= ? And [Date] < ?", _StaffPayrollBatch.PeriodStart.Date, _StaffPayrollBatch.PeriodEnd.AddDays(1).Date)
            };
            if (collection.Count == 0)
            {
                throw new ApplicationException("There are no attendance to link on this staff payroll batch.");
            }

            _FrmProgress = new ProgressForm("Linking attendance...", collection.Count,
"Attendance processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(collection);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<CheckInAndOut03> cols = (XPCollection<CheckInAndOut03>)e.Argument;
            StaffPayrollBatch stp = session.GetObjectByKey<StaffPayrollBatch>(_StaffPayrollBatch.Oid);
            try
            {
                foreach (var item in cols)
                {
                    index++;
                    _message = string.Format("Linking attendance {0} succesfull.",
                    cols.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    CheckInAndOut03 ci = session.GetObjectByKey<CheckInAndOut03>(item.Oid);
                    stp.CalculatedAttendance2.Add(ci);
                    #endregion
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
                if (index == cols.Count)
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
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
                    "Attendance linking is cancelled",
                    "Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Exclamation);
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
                    "Attendance has been linked successfully."
                    );
                    //ObjectSpace.ReloadObject(_IncomeStatement);
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
