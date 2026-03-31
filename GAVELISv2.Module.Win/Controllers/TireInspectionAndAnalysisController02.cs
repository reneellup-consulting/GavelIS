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
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class TireInspectionAndAnalysisController02 : ViewController
    {
        private SimpleAction unmarkedSelectedForInspectionAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public TireInspectionAndAnalysisController02()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            //this.TargetViewId = "Tire_ListView_Dettached";

            #region Unmarked selected tires
            this.unmarkedSelectedForInspectionAction = new SimpleAction(this, "Tire.UnmarkForInspection",
PredefinedCategory.RecordEdit);
            this.unmarkedSelectedForInspectionAction.Caption = "Unmark Selected";
            this.unmarkedSelectedForInspectionAction.ConfirmationMessage =
            "Do you really want to unmark these tire(s) for inspection?";
            this.unmarkedSelectedForInspectionAction.Execute += new SimpleActionExecuteEventHandler(unmarkedSelectedForInspectionAction_Execute);
            #endregion

            UpdateActionState(false);
        }

        void unmarkedSelectedForInspectionAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no tires selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            IList lst = null;
            lst = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = lst.Count;
            _FrmProgress = new ProgressForm("Unmarking tires...", count,
            "Unmarking tire {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(lst);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e){
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                foreach (Tire item in trans)
                {
                    index++;
                    _message = string.Format("Unmarking tire {0} succesfull.",
                    trans.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    Tire obj = session.GetObjectByKey<Tire>(item.Oid);
                    if (obj.LastActivityType == TireActivityTypeEnum.Dettached && obj.ForInspection)
                    {
                        obj.ForInspection = false;
                    }
                    obj.Save();
                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    //CommitUpdatingSession(session);
                    System.Threading.Thread.Sleep(20);
                }
            }
            finally
            {
                if (index == trans.Count)
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
                    "Unmarking tires has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully unmarked");
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        private void UpdateActionState(bool inProgress)
        {
            this.unmarkedSelectedForInspectionAction.Enabled.SetItemValue("Unmarking tires", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
