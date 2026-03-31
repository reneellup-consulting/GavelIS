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
    public partial class UpdateMovGrpDetWhseToCountController : ViewController
    {
        private SimpleAction updateMovGrpDetWhseToCountAction;
        private ItemsMovementGroup _ItemsMovementGroup;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public UpdateMovGrpDetWhseToCountController()
        {
            this.TargetObjectType = typeof(ItemsMovementGroup);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "updateMovGrpDetWhseToCountActionId";
            this.updateMovGrpDetWhseToCountAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.updateMovGrpDetWhseToCountAction.TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueForAll;
            this.updateMovGrpDetWhseToCountAction.Caption = "Update for Counting";
            this.updateMovGrpDetWhseToCountAction.Execute += new SimpleActionExecuteEventHandler(updateMovGrpDetWhseToCountAction_Execute);
        }
        private void updateMovGrpDetWhseToCountAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ItemsMovementGroup = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as ItemsMovementGroup;

            ObjectSpace.CommitChanges();

            if (_ItemsMovementGroup.ItemsMovementGroupDetails.Count == 0)
            {
                XtraMessageBox.Show("There are no rows exist",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            int count = _ItemsMovementGroup.ItemsMovementGroupDetails.Count;

            _FrmProgress = new ProgressForm("Updating...", count,
                        "Processing selected {0} of {1}");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_ItemsMovementGroup.ItemsMovementGroupDetails);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<ItemsMovementGroupDetail> trans = (XPCollection<ItemsMovementGroupDetail>)e.Argument;
            try
            {
                ItemsMovementGroup o_img = session.GetObjectByKey<ItemsMovementGroup>(_ItemsMovementGroup.Oid);
                foreach (ItemsMovementGroupDetail imgd in trans)
                {
                    index++;

                    #region Algorithm here...

                    ItemsMovementGroupDetail o_imgd = session.GetObjectByKey<ItemsMovementGroupDetail>(imgd.Oid);
                    string uqry = string.Format("update ItemsMovedPerWhseQty set ToCount=0 where MovementDetId={0}", imgd.Oid);
                    int res = session.ExecuteNonQuery(uqry);

                    foreach (ItemsMovSelectedWhseDetail imswd in o_img.ItemsMovSelectedWhseDetails)
                    {
                        ItemsMovedPerWhseQty o_impqw = o_imgd.ItemsMovedPerWhseQtyLines.Where(o => o.Warehouse.Code == imswd.Whse.Code).FirstOrDefault();
                        if (o_impqw != null)
                        {
                            o_impqw.ToCount = true;
                            o_impqw.Save();
                        }
                    }

                    o_imgd.Save();
                    o_img.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    _message = string.Format("Processing selected {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
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
                    "Update is cancelled.", "Cancelled",
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
                    "Update has been successfull.");
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
