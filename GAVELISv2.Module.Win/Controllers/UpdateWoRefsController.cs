using System;
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
    public partial class UpdateWoRefsController : ViewController
    {
        private WorkOrder workOrder;
        private SimpleAction updateWoRefsAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public UpdateWoRefsController()
        {
            this.TargetObjectType = typeof(WorkOrder);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("Update WO Ref(s)", this.GetType().Name);
            this.updateWoRefsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.updateWoRefsAction.Caption = "Update Reference(s)";
            this.updateWoRefsAction.TargetObjectsCriteria = "[CurrentUser.Roles][[Name] = 'Administrator']";
            this.updateWoRefsAction.Execute += new
            SimpleActionExecuteEventHandler(updateWoRefsAction_Execute);
            this.updateWoRefsAction.Executed += new EventHandler<
            ActionBaseEventArgs>(updateWoRefsAction_Executed);
            this.updateWoRefsAction.ConfirmationMessage =
            "Do you really want to update Work Orders references?";
            UpdateActionState(false);
        }
        private void updateWoRefsAction_Execute(object sender,
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
        private void updateWoRefsAction_Executed(object sender,
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
                    _message = string.Format("Updating entry {0} succesfull.",
                    woobj.SourceNo);
                    _BgWorker.ReportProgress(1, _message);

                    StringBuilder sb = new StringBuilder(!string.IsNullOrEmpty(woobj.ReferenceNo) ? woobj.ReferenceNo + "," : string.Empty);
                    foreach (var detail in woobj.WorkOrderItemDetails)
                    {
                        if (detail.RequisitionNo != null && !sb.ToString().Contains(detail.RequisitionNo.SourceNo))
                        {
                            sb.AppendFormat("{0},", detail.RequisitionNo.SourceNo);
                        }
                        //if (detail.CostCenter != null && detail.CostCenter.FixedAsset != null)
                        //{
                        //    detail.Fleet = detail.CostCenter.FixedAsset;
                        //}
                        //else
                        //{
                        //    detail.Fleet = woobj.Fleet ?? null;
                        //}
                    }
                    if (sb.Length > 0 && sb[sb.Length - 1].ToString() == ",")
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    woobj.ReferenceNo = sb.ToString();
                    woobj.Save();
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
                    "Forwarding entries operation has been cancelled", "Cancelled",
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
                    " has been successfully updated");
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
            updateWoRefsAction.
                Enabled.SetItemValue("Forwarding entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
