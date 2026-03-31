using System;
using System.Linq;
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
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers {
    public partial class TireForSaleController : ViewController {
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private TireForSale _TireForSale;
        private SimpleAction _ReleaseTireForSaleAction;
        public TireForSaleController() {
            this.TargetObjectType = typeof(TireForSale);
            this.TargetViewType = ViewType.DetailView;
            string actionId = "TireForSale.Release";
            _ReleaseTireForSaleAction = new SimpleAction(this, actionId, PredefinedCategory.RecordEdit);
            _ReleaseTireForSaleAction.Caption = "Release All";
            _ReleaseTireForSaleAction.ConfirmationMessage = "Do you really want to release this tires for sale?";
            _ReleaseTireForSaleAction.Execute += new SimpleActionExecuteEventHandler(_ReleaseTireForSaleAction_Execute);
            _ReleaseTireForSaleAction.Executed += new EventHandler<ActionBaseEventArgs>(_ReleaseTireForSaleAction_Executed);
            UpdateActionState(false);
        }

        void _ReleaseTireForSaleAction_Executed(object sender, ActionBaseEventArgs e)
        {
            IObjectSpace objs = Application.CreateObjectSpace();
            Receipt objRcpt = objs.GetObjectByKey<Receipt>(_TireForSale.TireReceiptDoc.Oid);
            DetailView view = Application.CreateDetailView(objs, objRcpt,
            true);
            e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
            e.ShowViewParameters.CreatedView = view;
        }

        void _ReleaseTireForSaleAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            _TireForSale = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TireForSale;
            if (_TireForSale.Status == TireForSaleStatusEnum.Released)
            {
                XtraMessageBox.Show("This Tire for Sale document was already released",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            ObjectSpace.CommitChanges();
            if (_TireForSale.TireForSaleDetails.Count == 0)
            {
                XtraMessageBox.Show("There are no tires to release for sale",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            var count = _TireForSale.TireForSaleDetails.Count;
            _FrmProgress = new ProgressForm("Releasing tire...", count,
            "Release tires {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_TireForSale);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            TireForSale args = e.Argument as TireForSale;
            TireForSale sesTfs = session.GetObjectByKey<TireForSale>(args.Oid);
            try
            {
                Receipt paObj;
                Receipt argsPa = sesTfs.TireReceiptDoc;
                if (argsPa == null)
                {
                    paObj = ReflectionHelper.CreateObject<Receipt>(session);
                    paObj.EntryDate = sesTfs.EntryDate;
                    paObj.ReferenceNo = sesTfs.DocNo;
                    paObj.Vendor = session.FindObject<Vendor>(
                    new BinaryOperator("No", "V01403"))??null;
                } else
                {
                    paObj = session.GetObjectByKey<Receipt>(argsPa.Oid);
                }
                sesTfs.TireReceiptDoc = paObj;
                foreach (TireForSaleDet item in sesTfs.TireForSaleDetails)
                {
                    index++;
                    _message = string.Format("Releasing tire {0} succesfull.",
                    sesTfs.TireForSaleDetails.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    TireForSaleDet sesTfsd = session.GetObjectByKey<TireForSaleDet>(item.Oid);
                    ReceiptDetail rcpt = ReflectionHelper.CreateObject<ReceiptDetail>(session);
                    if (sesTfsd.TireNo.TireItem.Size.ScrappedItem == null)
                    {
                        throw new ApplicationException(string.Format("Scrapped Item not specified in TireSize profile for {0}", sesTfsd.TireNo.TireItem.No));
                    }
                    rcpt.ItemNo = sesTfsd.TireNo.TireItem.Size.ScrappedItem;
                    if (sesTfs.ToWarehouse == null)
                    {
                        throw new ApplicationException("To Warehouse not specified");
                    }
                    rcpt.Warehouse = sesTfs.ToWarehouse;
                    rcpt.Quantity = 1;
                    rcpt.TfsDetailId = sesTfsd;
                    ReceiptDetailTrackingLine rcptDetTl = ReflectionHelper.CreateObject<ReceiptDetailTrackingLine>(session);
                    rcptDetTl.SerialNo = sesTfsd.SerialBrandingNo;
                    rcpt.ReceiptDetailTrackingLines.BaseAdd(rcptDetTl);
                    paObj.ReceiptDetails.BaseAdd(rcpt);
                    // Mark detail as released
                    sesTfsd.Released = true;
                    sesTfsd.Save();
                    
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
            } finally
            {
                if (index == sesTfs.TireForSaleDetails.Count)
                {
                    e.Result = index;
                    //sesTfs.Status = TireForSaleStatusEnum.Released;
                    sesTfs.Save();
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }

        private UnitOfWork CreateUpdatingSession() {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }

        private void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }

        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) {
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                , new SessionEventArgs(session));
            }
        }

        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) {
            if
            (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                SessionEventArgs(session));
            }
        }

        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e) {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                DoProgress(e.ProgressPercentage);
            }
        }

        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                "Releasing tire operation has been cancelled", "Cancelled",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
            } else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);
                } else
                {
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully released");
                    ObjectSpace.ReloadObject(_TireForSale);
                    ObjectSpace.Refresh();
                }
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        private void UpdateActionState(bool inProgress) {
            _ReleaseTireForSaleAction.
            Enabled.SetItemValue("Releasing tires", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
