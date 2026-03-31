using System;
using System.ComponentModel;
using System.Collections;
//using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class PostSelectedTransactions : ViewController
    {
        private SimpleAction postSelectedTransactions;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public PostSelectedTransactions()
        {
            this.TargetObjectType = typeof(GenJournalHeader);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.PostSelectedTransactions", this.GetType().
            Name);
            this.postSelectedTransactions = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.postSelectedTransactions.Caption = "Post Transactions";
            this.postSelectedTransactions.Execute += new
            SimpleActionExecuteEventHandler(PostSelectedTransactions_Execute);
            this.postSelectedTransactions.Executed += new EventHandler<
            ActionBaseEventArgs>(PostSelectedTransactions_Executed);
            this.postSelectedTransactions.ConfirmationMessage =
            "Do you really want to post selected transactions?";
            UpdateActionState(false);
        }
        private void PostSelectedTransactions_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no transactions selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList genHeaders = null;
            genHeaders = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = genHeaders.Count;
            _FrmProgress = new ProgressForm("Posting transactions...", count,
            "Posting transactions {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(genHeaders);
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
                GenJournalHeader hd = null;
                foreach (GenJournalHeader item in trans)
                {
                    hd = session.GetObjectByKey<GenJournalHeader>(item.Oid);
                    if (hd.SourceType.Description=="Invoice")
                    {

                    }
                    //hd.Approved = false;
                    //hd.Delete();

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Posting transaction {0} succesfull.",
                    trans.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
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
        private void PostSelectedTransactions_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //throw new NotImplementedException();
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
                    "posting transaction operation has been cancelled", "Cancelled",
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
                    " has been successfully posted");
                    //ObjectSpace.ReloadObject(invoice);
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
            postSelectedTransactions.
                Enabled.SetItemValue("Post transactions", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
