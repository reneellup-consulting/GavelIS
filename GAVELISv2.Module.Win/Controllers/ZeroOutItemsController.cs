using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ZeroOutItemsController : ViewController
    {
        private PopupWindowShowAction zeroOutItemsAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private IObjectSpace _ObjectSpace;
        private ZeroOutRequest _Request;
        public ZeroOutItemsController()
        {
            this.TargetObjectType = typeof(Item);
            this.TargetViewType = ViewType.ListView;
            this.TargetViewId = "Item_ListView_ZeroOut";
            string actionID = "ZeroOutItems";
            this.zeroOutItemsAction = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.zeroOutItemsAction.Caption = "Create Adjustment";
            this.zeroOutItemsAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(zeroOutItemsAction_CustomizePopupWindowParams);
            this.zeroOutItemsAction.Execute += new PopupWindowShowActionExecuteEventHandler(zeroOutItemsAction_Execute);
        }

        void zeroOutItemsAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            if (_Request.AdjustmentDoc == null)
            {
                throw new ApplicationException("Please select Physical Adjustment transaction");
            }
            IList sels = this.View.SelectedObjects;
            _FrmProgress = new ProgressForm("Processing items...", sels.Count,
                "Processing item {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(sels);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList sels = (IList)e.Argument;
            try
            {
                PhysicalAdjustment oPa = session.GetObjectByKey<PhysicalAdjustment>(_Request.AdjustmentDoc.Oid);
                foreach (Item item in sels)
                {
                    index++;
                    Item oItem = session.GetObjectByKey<Item>(item.Oid);
                    _message = string.Format("Processing item {0} succesfull.",
                    oItem.No);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    XPCollection<Warehouse> whses = new XPCollection<Warehouse>(session);
                    foreach (var whs in whses)
                    {
                        PhysicalAdjustmentDetail pad = ReflectionHelper.CreateObject<PhysicalAdjustmentDetail>(session);
                        oPa.PhysicalAdjustmentDetails.Add(pad);
                        pad.ItemNo = oItem;
                        pad.ActualQtyStock = 0m;
                        pad.Warehouse = whs;
                        pad.Save();
                    }
                    oPa.Save();
                    CommitUpdatingSession(session);

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
                if (index == sels.Count)
                {
                    e.Result = index;
                }
                session.Dispose();
            }
        }

        void zeroOutItemsAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Request = new ZeroOutRequest();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "ZeroOutRequest_DetailView", true, _Request);
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
                "Processing items has been cancelled", "Cancelled",
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
                    " items are successfully added to the document");
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
            this.zeroOutItemsAction.
            Enabled.SetItemValue("Creating physical adjustment", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
