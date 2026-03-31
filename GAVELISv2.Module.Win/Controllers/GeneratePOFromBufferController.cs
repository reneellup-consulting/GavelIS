using System;
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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GeneratePOFromBufferController : ViewController
    {
        private SimpleAction generatePOFromBuffer;
        private OfrsBuffer _Ofrs;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GeneratePOFromBufferController()
        {
            this.TargetObjectType = typeof(OfrsBuffer);
            this.TargetViewType = ViewType.ListView;
            string actionID = "GeneratePOFromBuffer";
            this.generatePOFromBuffer = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generatePOFromBuffer.Caption =
            "Generate PO";
            this.generatePOFromBuffer.Execute += new
            SimpleActionExecuteEventHandler(
            generatePOFromBufferAction_Execute);
        }
        private void generatePOFromBufferAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no entry(s) selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList ofrss = null;
            ofrss = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            _FrmProgress = new ProgressForm("Generating PO...", ofrss.Count,
            "OFRS entry processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(ofrss);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList entries = (IList)e.Argument;
            try
            {
                foreach (OfrsBuffer item in entries)
                {
                    index++;
                    _message = string.Format("Updating {0} succesfull.",
                            index);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    OfrsBuffer ofrs = session.GetObjectByKey<OfrsBuffer>(item.Oid);
                    PurchaseOrder porder = ReflectionHelper.CreateObject<PurchaseOrder>(session);
                    // Vendor
                    Vendor vndr = session.FindObject<Vendor>(BinaryOperator.Parse("[No]=?", ofrs.FuelStation));
                    porder.Vendor = vndr ?? null;
                    // RerenceNo as FRSNo
                    porder.ReferenceNo = ofrs.FrsNo;
                    // EntryDate
                    porder.EntryDate = DateTime.Now;
                    // FromOFRS
                    porder.IsOnlineFrs = true;
                    // Status
                    porder.Status = PurchaseOrderStatusEnum.Approved;
                    // ApprovedDate
                    porder.ApprovedDate = ofrs.ApprovedDate;
                    porder.Approved = true;
                    // Remarks > 
                    StringBuilder sb = new StringBuilder();

                    sb.Append("Online FRS");
                    sb.AppendLine();
                    sb.Append("-------------------");
                    sb.AppendLine();
                    sb.AppendFormat("Purpose: {0}", ofrs.Purpose);
                    sb.AppendLine();
                    Tariff trf = session.FindObject<Tariff>(BinaryOperator.Parse("[Code]=?", ofrs.Tariff));
                    sb.AppendFormat("Origin and Dest.: {0} to {1}", trf.Origin.Code, trf.Destination.Code);
                    sb.AppendLine();
                    sb.AppendFormat("Remarks: {0}", ofrs.Remarks);
                    sb.AppendLine();
                    sb.AppendFormat("Released by: {0} on {1}", ofrs.ReleasedBy, ofrs.ReleasedDate.ToShortDateString());
                    sb.AppendLine();
                    sb.AppendFormat("Checked by: {0} on {1}", ofrs.CheckedBy, ofrs.CheckedDate.ToShortDateString());
                    sb.AppendLine();
                    sb.AppendFormat("Approved by: {0} on {1}", ofrs.ApprovedBy, ofrs.ApprovedDate.ToShortDateString());
                    sb.AppendLine();
                    porder.Remarks = sb.ToString();

                    // Details
                    PurchaseOrderDetail podtl = ReflectionHelper.CreateObject<PurchaseOrderDetail>(session);
                    podtl.GenJournalID = porder;
                    // ItemNo
                    podtl.ItemNo = session.FindObject<Item>(BinaryOperator.Parse("[No]=?", ofrs.ItemNo));
                    // ChargeTo
                    podtl.CostCenter = session.FindObject<CostCenter>(BinaryOperator.Parse("[Code]=?", ofrs.UnitNo));
                    // RequestedBy
                    podtl.RequestedBy = session.FindObject<Employee>(BinaryOperator.Parse("[No]=?", ofrs.Operator));
                    // LineApprovalStatus
                    podtl.LineApprovalStatus = POLineStatusEnum.Released;
                    // Quantity
                    podtl.Quantity = ofrs.Quantity;
                    // BaseCost
                    podtl.BaseCost = ofrs.Cost;
                    // Remarks
                    podtl.Remarks = ofrs.Purpose;
                    podtl.Save();
                    porder.Save();
                    ofrs.IsPoCreated = true;
                    ofrs.PoCreatedRef = porder;
                    ofrs.Save();
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
                if (index == entries.Count)
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
                    "Generation of PO is cancelled.", "Cancelled",
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
                    "Generation of PO has been successfully generated.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
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
