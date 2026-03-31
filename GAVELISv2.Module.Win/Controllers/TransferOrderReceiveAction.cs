using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
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
    public partial class TransferOrderReceiveAction : ViewController
    {
        private TransferOrder transferOrder;
        private SimpleAction transferOrderReceiveAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public TransferOrderReceiveAction()
        {
            this.TargetObjectType = typeof(TransferOrder);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.Complete", this.GetType().Name);
            this.transferOrderReceiveAction = new SimpleAction(this, actionID
            , PredefinedCategory.RecordEdit);
            this.transferOrderReceiveAction.Caption = "Complete Transfer";
            this.transferOrderReceiveAction.Execute += new
            SimpleActionExecuteEventHandler(
            TransferOrderReceiveAction_Execute);
            this.transferOrderReceiveAction.Executed += new EventHandler<
            ActionBaseEventArgs>(TransferOrderReceiveAction_Executed);
            this.transferOrderReceiveAction.ConfirmationMessage =
            "Do you really want to apply these entries?";
            UpdateActionState(false);
        }
        private void TransferOrderReceiveAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            transferOrder = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TransferOrder;
            ObjectSpace.CommitChanges();
            if (transferOrder.TransferOrderDetails.Count == 0)
            {
                XtraMessageBox.Show("There are no entries to transfer", "Attention"
                , System.Windows.Forms.MessageBoxButtons.OK, System.Windows.
                Forms.MessageBoxIcon.Exclamation);
                return;
            }

            if (transferOrder.CompanyInfo.AllowInsufficientCurrQty != true)
            {
                foreach (var item in transferOrder.TransferOrderDetails)
                {
                    if (item.Quantity > item.CurrentQtyBase)
                    {
                        throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + item.ItemNo.No + "!");
                    }
                }
            }

            foreach (var item in transferOrder.TransferOrderDetails)
            {
                if (item.Quantity > item.CurrentQtyBase)
                {
                    DialogResult dres = XtraMessageBox.Show("Warehouse is not sufficient to fullfil item " + item.ItemNo.No + ". Do you want to continue?", "Insufficient Qty",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dres == DialogResult.Cancel)
                    {
                        return;
                    }
                }

            }

            var count = transferOrder.TransferOrderDetails.Count;
            _FrmProgress = new ProgressForm("Transferring entries...", count,
            "Processing entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(transferOrder);
            _FrmProgress.ShowDialog();
        }
        private void TransferOrderReceiveAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
        }

        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            TransferOrder _transferOrder = (TransferOrder)e.
            Argument;
            InventoryControlJournal _icj1;
            InventoryControlJournal _icj2;
            TransferOrder thisTransferOrder = session.GetObjectByKey<
            TransferOrder>(_transferOrder.Oid);
            try
            {
                foreach (TransferOrderDetail item in thisTransferOrder.TransferOrderDetails)
                {
                    item.skipAuto = true;
                    UOMRelation dBaseUOM = null;
                    UOMRelation dStockUOM = null;
                    UOMRelation dSellUOM = null;
                    // Create out from warehouse icj entry
                    _icj1 = ReflectionHelper.CreateObject<InventoryControlJournal
                    >(session);
                    _icj1.GenJournalID = thisTransferOrder;
                    _icj1.Warehouse = item.Warehouse;
                    _icj1.ItemNo = item.ItemNo;
                    if (item.ItemNo.UOMRelations.Count > 0)
                    {
                        dBaseUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.ItemNo.BaseUOM2).FirstOrDefault();
                        dStockUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.ItemNo.StockUOM).FirstOrDefault();
                        dSellUOM = item.ItemNo.UOMRelations.Where(o => o.UOM == item.UOM).FirstOrDefault();
                        if (dStockUOM.UOM == item.UOM)
                        {
                            _icj1.OutQty = Math.Abs(item.Quantity);
                            _icj1.Cost = dStockUOM.CostPerBaseUom;
                        }
                        else
                        {
                            decimal bQty = item.BaseQTY / dStockUOM.Factor;
                            _icj1.OutQty = Math.Abs(bQty);
                            _icj1.Cost = dStockUOM.CostPerBaseUom;
                        }
                    }
                    else
                    {
                        _icj1.OutQty = Math.Abs(item.Quantity);
                        _icj1.Cost = item.ItemNo.Cost;
                    }
                    _icj1.UOM = item.ItemNo.StockUOM;
                    _icj1.RowID = item.RowID.ToString();
                    _icj1.RequisitionNo = item.RequisitionNo != null ? item.RequisitionNo : null;
                    _icj1.CostCenter = item.CostCenter != null ? item.CostCenter : null;
                    _icj1.RequestedBy = item.RequestedBy != null ? item.RequestedBy : null;
                    _icj1.Save();
                    // Create in to warehouse icj entry
                    _icj2 = ReflectionHelper.CreateObject<InventoryControlJournal>(session);
                    _icj2.GenJournalID = thisTransferOrder;
                    _icj2.InQTY = Math.Abs(_icj1.OutQty);
                    _icj2.Warehouse = thisTransferOrder.ToWarehouse;
                    _icj2.ItemNo = item.ItemNo;
                    _icj2.Cost = _icj1.Cost;
                    _icj2.UOM = item.ItemNo.StockUOM;
                    _icj2.RowID = item.RowID.ToString() + "TIN";
                    _icj2.RequisitionNo = item.RequisitionNo != null ? item.RequisitionNo : null;
                    _icj2.CostCenter = item.CostCenter != null ? item.CostCenter : null;
                    _icj2.RequestedBy = item.RequestedBy != null ? item.RequestedBy : null;
                    _icj2.Save();
                    
                    item.Transferred = item.Quantity;
                    // Serialized Items
                    if (item.ItemNo.RequireSerial)
                    {
                        if (item.TransferOrderDetailTrackingLines.Count !=
                        Math.Abs(item.Quantity))
                        {
                            throw new
                                ApplicationException(
                                "An item requires a serial no. Please specify serial nos according to quantity"
                                );
                        }
                        foreach (TransferOrderDetailTrackingLine iTrack in item.TransferOrderDetailTrackingLines)
                        {
                            ItemTrackingEntry _ite = null;
                            if (item.Quantity > 0)
                            {
                                // Check if serial number exist and available
                                _ite = (ItemTrackingEntry)session.FindObject(
                                typeof(ItemTrackingEntry), CriteriaOperator.
                                Parse("[ItemNo.No] = '" + item.ItemNo.No +
                                "' And [SerialNo] = '" + iTrack.SerialNo +
                                "' And [Warehouse.Code] = '" + item.Warehouse.
                                Code + "' And [Status] = 'Available'"));
                                if (_ite == null)
                                {
                                    throw new
                                        ApplicationException(
                                        "A serial number indicated in an item tracking line does not exist or unavailable"
                                        );
                                } else{
                                    _ite.Warehouse = thisTransferOrder.ToWarehouse;
                                    _ite.Save();
                                }
                            }
                        }
                    }
                    //..
                    item.Save();
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Transferring entry {0} succesfull.",
                    thisTransferOrder.TransferOrderDetails.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;

                }
            }
            finally
            {
                thisTransferOrder.Status = TransferOrderStatusEnum.Completed;
                thisTransferOrder.Save();
                CommitUpdatingSession(session);
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
        private string _message;

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
                    "Transfer operation has been cancelled", "Cancelled",
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
                    " has been successfully transferred");

                    ObjectSpace.ReloadObject(transferOrder);
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
            transferOrderReceiveAction.Enabled.SetItemValue(
            "Tranferring entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
