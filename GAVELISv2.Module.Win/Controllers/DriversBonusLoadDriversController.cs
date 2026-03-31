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
    public partial class DriversBonusLoadDriversController : ViewController
    {
        private SimpleAction driversBonusLoadDriversAction;
        private DriversBonusGeneratorHeader _DriversBonusGeneratorHeader;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public DriversBonusLoadDriversController()
        {
            this.TargetObjectType = typeof(DriversBonusGeneratorHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriversBonusLoadDriversId";
            this.driversBonusLoadDriversAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.driversBonusLoadDriversAction.Caption = "Load Drivers";
            this.driversBonusLoadDriversAction.Execute += new SimpleActionExecuteEventHandler(driversBonusLoadDriversAction_Execute);
        }

        void driversBonusLoadDriversAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _DriversBonusGeneratorHeader = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DriversBonusGeneratorHeader;

            ObjectSpace.CommitChanges();

            // Get the session from the ObjectSpace
            Session t_session = ((ObjectSpace)ObjectSpace).Session;

            string qry = string.Format("SELECT * FROM vActiveDrivers ORDER BY DriverName");
            SelectedData data = t_session.ExecuteQuery(qry);
            int count = data.ResultSet[0].Rows.Count();
            if (data == null || count == 0)
            {
                XtraMessageBox.Show("There are no retrieved data",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Loading...", count,
                    "Drivers loaded {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(data);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            SelectedData trans = (SelectedData)e.Argument;
            DriversBonusGeneratorHeader o_dbgh = session.GetObjectByKey<DriversBonusGeneratorHeader>(_DriversBonusGeneratorHeader.Oid);
            try
            {
                foreach (var item in trans.ResultSet[0].Rows)
                {
                    index++;

                    #region Algorithms here...

                    Guid driverId = Guid.Parse(item.Values[0].ToString());
                    DriversBonusGeneratorDetail o_dbgd = o_dbgh.DriversBonusGeneratorDetails.Where(o => o.Driver.Oid == driverId).FirstOrDefault();
                    if (o_dbgd == null)
                    {
                        o_dbgd = ReflectionHelper.CreateObject<DriversBonusGeneratorDetail>(session);
                        o_dbgd.Driver = session.GetObjectByKey<Employee>(driverId);
                        o_dbgd.DriverStatus = o_dbgd.Driver.Status;
                        o_dbgd.DriverClass = o_dbgd.Driver.DriverClass;
                        o_dbgd.DriverClassification = o_dbgd.Driver.DriverClassification;
                        o_dbgh.DriversBonusGeneratorDetails.Add(o_dbgd);
                        o_dbgh.Save();
                    }

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Loading driver {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.ResultSet[0].Rows.Count())
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
                    "Loading is cancelled.", "Cancelled",
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
                    "Loading has been successfull.");
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
