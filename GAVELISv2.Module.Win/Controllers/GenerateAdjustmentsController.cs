using System;
using System.Windows.Forms;
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
using Microsoft.Office.Interop.Excel;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateAdjustmentsController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private SimpleAction generateCountAdjustmentAction;
        private ItemsMovGrpCountDetail _ItemsMovGrpCountDetail;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateAdjustmentsController()
        {
            this.TargetObjectType = typeof(ItemsMovGrpCountDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = "GenerateCountAdjustmentActionId";
            this.generateCountAdjustmentAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateCountAdjustmentAction.Caption = "Generate Adjustments";
            this.generateCountAdjustmentAction.Execute += new SimpleActionExecuteEventHandler(generateCountAdjustmentAction_Execute);
        }
        void generateCountAdjustmentAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            IList selected = this.View.SelectedObjects;
            var count = selected.Count;
            if (count == 0)
            {
                XtraMessageBox.Show("There are no selected rows",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            _FrmProgress = new ProgressForm("Generating...", count,
            "Processing row {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(selected);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            IEnumerable<ItemsMovGrpCountDetail> lines = ((IList)e.Argument).Cast<ItemsMovGrpCountDetail>();
            UnitOfWork session = CreateUpdatingSession();
            int index = 0;
            try
            {
                IOrderedEnumerable<ItemsMovGrpCountDetail> o_lines = lines.OrderBy(o => o.Warehouse.Code);
                Warehouse oldWarehouse = null;
                PhysicalAdjustment phys = null;
                foreach (ItemsMovGrpCountDetail item in o_lines)
                {
                    index++;

                    #region Algorithms here...

                    ItemsMovGrpCountDetail o_item = session.GetObjectByKey<ItemsMovGrpCountDetail>(item.Oid);
                    if (oldWarehouse != o_item.Warehouse)
                    {
                        // Create new Physical Adjustment Document
                        oldWarehouse = o_item.Warehouse;
                        phys = null;
                        phys = ReflectionHelper.CreateObject<PhysicalAdjustment>(session);
                        phys.EntryDate = DateTime.Now;
                        phys.WarehouseLocation = o_item.Warehouse;
                        phys.Memo = string.Format("Count Ref#{0} {1}", o_item.HeaderId.Code, o_item.HeaderId.Description);
                    }

                    PhysicalAdjustmentDetail o_pad = ReflectionHelper.CreateObject<PhysicalAdjustmentDetail>(session);
                    o_pad.GenJournalID = phys;
                    o_pad.ItemNo = o_item.ItemNo;
                    o_pad.ActualQtyStock = o_item.ActualQty;
                    o_pad.Warehouse = o_item.Warehouse;
                    o_pad.Save();
                    phys.Save();

                    o_item.AdjustmentDoc = phys;
                    o_item.Save();

                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Generating adjustments {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == Convert.ToInt32(lines.Count().ToString()))
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
                    "Generating adjustments is cancelled.", "Cancelled",
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
                    "Adjustments generation has been successfull.");

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
