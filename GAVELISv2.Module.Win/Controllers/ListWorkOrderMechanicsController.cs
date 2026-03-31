using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
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
    public partial class ListWorkOrderMechanicsController : ViewController
    {
        private WorkOrder workOrder;
        private SimpleAction listWoMechanicsAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ListWorkOrderMechanicsController()
        {
            this.TargetObjectType = typeof(WorkOrder);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("ListWoMechanisAction", this.GetType().Name);
            this.listWoMechanicsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.listWoMechanicsAction.Caption = "List Mechanics";
            //this.listWoMechanicsAction.TargetObjectsCriteria = "[CurrentUser.Roles][[Name] = 'Administrator']";
            this.listWoMechanicsAction.Execute += new
            SimpleActionExecuteEventHandler(listWoMechanicsAction_Execute);
            this.listWoMechanicsAction.Executed += new EventHandler<
            ActionBaseEventArgs>(listWoMechanicsAction_Executed);
            this.listWoMechanicsAction.ConfirmationMessage =
            "Do you really want to list the mechanics of the selected Work Orders?";
            UpdateActionState(false);
        }
        private void listWoMechanicsAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            System.Collections.IList data = View.SelectedObjects;
            var count = data.Count;
            _FrmProgress = new ProgressForm("Updating entries...", count,
            "Updating entries {0} of {1} ");
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
        private void listWoMechanicsAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //ObjectSpace.ReloadObject(invoice);
            //ObjectSpace.Refresh();
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            System.Collections.IList list = (System.Collections.IList)e.Argument;
            try
            {
                foreach (var item in list)
                {
                    WorkOrder woobj = session.GetObjectByKey<WorkOrder>((item as WorkOrder).Oid);
                    index++;
                    _message = string.Format("Listing {0} succesfull.",
                    woobj.SourceNo);
                    _BgWorker.ReportProgress(1, _message);

                    if (woobj.Mechanic != null)
                    {
                        var query = woobj.Mechanics.FirstOrDefault(o => o.Mechanic == woobj.Mechanic);
                        if (query == null)
                        {
                            WorkOrderMechanic wom = ReflectionHelper.CreateObject<WorkOrderMechanic>(session);
                            wom.Mechanic = woobj.Mechanic;
                            woobj.Mechanics.Add(wom);
                            woobj.Save();
                        }
                    }

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                }
            }
            finally
            {
                if (index == list.Count)
                {
                    //CommitUpdatingSession(session);
                }
                session.Dispose();
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
                    "Listing operation has been cancelled", "Cancelled",
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
                    " has been successfully listed");
                    //ObjectSpace.ReloadObject(workOrder);
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
            listWoMechanicsAction.
                Enabled.SetItemValue("Listing", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
