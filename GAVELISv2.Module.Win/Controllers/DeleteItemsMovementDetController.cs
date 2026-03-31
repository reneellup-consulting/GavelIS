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
using DevExpress.Xpo.DB;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class DeleteItemsMovementDetController : ViewController
    {
        private SimpleAction deleteItemsMovementDetAction;
        private ItemMovementFreqAnalysis _ItemMovementFreqAnalysis;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public DeleteItemsMovementDetController()
        {
            this.TargetObjectType = typeof(ItemMovementFreqAnalysis);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "deleteItemsMovementActionId";
            this.deleteItemsMovementDetAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.deleteItemsMovementDetAction.TargetObjectsCriteria = "[ItemMovementFreqAnalysisDetails][]";
            this.deleteItemsMovementDetAction.Caption = "Clear";
            this.deleteItemsMovementDetAction.Execute += new SimpleActionExecuteEventHandler(deleteItemsMovementDetAction_Execute);
        }

        private void deleteItemsMovementDetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ItemMovementFreqAnalysis = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ItemMovementFreqAnalysis;

            ObjectSpace.CommitChanges();

            int count = _ItemMovementFreqAnalysis.ItemMovementFreqAnalysisDetails.Count();

            _FrmProgress = new ProgressForm("Deleting...", count,
                        "Deleting row {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BgWorker_RunWorkerCompleted);
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += new DoWorkEventHandler(_BgWorker_DoWork);
            _BgWorker.RunWorkerAsync(_ItemMovementFreqAnalysis.ItemMovementFreqAnalysisDetails);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        public void _BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<ItemMovementFreqAnalDetail> trans = (XPCollection<ItemMovementFreqAnalDetail>)e.Argument;
            try
            {
                foreach (var item in trans)
                {
                    #region Algorithms here...

                    ItemMovementFreqAnalDetail imfad = session.GetObjectByKey<ItemMovementFreqAnalDetail>(item.Oid);

                    imfad.Delete();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            }
            finally
            {
                if (index == trans.Count) { CommitUpdatingSession(session); }
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

        public void _BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Deletion is cancelled.", "Cancelled",
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
                    "Deletion has been successfull.");
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
