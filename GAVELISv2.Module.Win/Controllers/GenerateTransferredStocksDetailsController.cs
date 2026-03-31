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
    public partial class GenerateTransferredStocksDetailsController : ViewController
    {
        private SimpleAction generateTransferredStocksDetailsAction;
        private TransferItemsViewer _TransferItemsViewer;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateTransferredStocksDetailsController()
        {
            this.TargetObjectType = typeof(TransferItemsViewer);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "generateTransferredStocksDetailsActionId";
            this.generateTransferredStocksDetailsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            //this.generateTransferredStocksDetailsAction.TargetObjectsCriteria = "[TransferItemsViewerDetails][].Count() = 0";
            this.generateTransferredStocksDetailsAction.Caption = "Generate";
            this.generateTransferredStocksDetailsAction.Execute += new SimpleActionExecuteEventHandler(generateTransferredStocksDetailsAction_Execute);
        }
        private void generateTransferredStocksDetailsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _TransferItemsViewer = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TransferItemsViewer;

            ObjectSpace.CommitChanges();

            int count = 0;

            string whseFilter = string.Empty;

            if (_TransferItemsViewer.FromWarehouse != null && _TransferItemsViewer.ToWarehouse != null)
            {
                whseFilter = string.Format("FromWarehouse = '{0}' and ToWarehouse = '{1}'", _TransferItemsViewer.FromWarehouse.Oid, _TransferItemsViewer.ToWarehouse.Oid);
            }

            string filter = string.Empty;

            //select * from vTransferOrderDetailsComp where FromWarehouse='2DF8B7A7-4E18-4129-BDA8-7A615B24F8D2' and ToWarehouse='F900FD5D-1162-4A04-8175-76F6923EAC5C' and DateCompleted between '2021/02/01' and '2021/02/28'

            switch (_TransferItemsViewer.FrequencyFilter)
            {
                case FrequencyFilterEnum.All:
                    filter = string.Format(" where {0}", whseFilter);
                    break;
                case FrequencyFilterEnum.Yearly:
                    filter = string.Format(" where {0} and [Year]={1}", whseFilter, _TransferItemsViewer.Year);
                    break;
                case FrequencyFilterEnum.Quarterly:
                    filter = string.Format(" where {0} and [Year]={1} and Qrt={2}", whseFilter, _TransferItemsViewer.Year, (int)_TransferItemsViewer.Quarter);
                    break;
                case FrequencyFilterEnum.Monthly:
                    filter = string.Format(" where {0} and [Year]={1} and [Month]={2}", whseFilter, _TransferItemsViewer.Year, (int)_TransferItemsViewer.Month);
                    break;
                case FrequencyFilterEnum.Weekly:
                    filter = string.Format(" where {0} and [Year]={1} and [Month]={2} and WeekNo={3}", whseFilter, _TransferItemsViewer.Year, (int)_TransferItemsViewer.Month, (int)_TransferItemsViewer.Week);
                    break;
                case FrequencyFilterEnum.Date:
                    filter = string.Format(" where {0} and DateCompleted between '{1}' and '{2}'", whseFilter, _TransferItemsViewer.FromDate.Date.ToShortDateString(), _TransferItemsViewer.ToDate.Date.ToShortDateString());
                    break;
                default:
                    break;
            }

            string qry = string.Format("select * from vTransferOrderDetailsComp{0}", filter);

            Session t_session = ((ObjectSpace)ObjectSpace).Session;
            SelectedData data = t_session.ExecuteQuery(qry);
            if (data == null || data.ResultSet[0].Rows.Count() == 0)
            {
                throw new UserFriendlyException("There is nothing to generate for the specified filter.");
            }
            count = data.ResultSet[0].Rows.Count();

            _FrmProgress = new ProgressForm("Generating...", count,
                        "Generate row {0} of {1} ");
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            SelectedData trans = (SelectedData)e.Argument;
            TransferItemsViewer oGenerator = session.GetObjectByKey<TransferItemsViewer>(_TransferItemsViewer.Oid);
            try
            {
                string cmd1 = string.Format("delete TransferItemsViewerDetail where HeaderId={0}", oGenerator.Oid);
                session.ExecuteNonQuery(cmd1);

                foreach (var row in trans.ResultSet[0].Rows)
                {
                    index++;

                    #region Algorithm here...

                    if (row.Values[0] != null)
                    {
                        // TransferNo
                        TransferOrder transferNo = session.GetObjectByKey<TransferOrder>(Convert.ToInt32(row.Values[0]));
                        // FromWarehouse
                        Warehouse fromWarehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(row.Values[4].ToString()));
                        // ToWarehouse
                        Warehouse toWarehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(row.Values[5].ToString()));
                        // ItemNo
                        Item itemNo = session.GetObjectByKey<Item>(Guid.Parse(row.Values[7].ToString()));
                        // RequisitionNo
                        Requisition requisitionNo = session.GetObjectByKey<Requisition>(Convert.ToInt32(row.Values[8]));
                        // Vendor
                        Vendor vendor = row.Values[10] != null? session.GetObjectByKey<Vendor>(Guid.Parse(row.Values[10].ToString())) : null;
                        // Origin
                        PartsOrigin origin = row.Values[11] != null ? session.GetObjectByKey<PartsOrigin>(Guid.Parse(row.Values[11].ToString())) : null;
                        // CostCenter
                        CostCenter costCenter = session.GetObjectByKey<CostCenter>(Guid.Parse(row.Values[12].ToString()));
                        // UOM
                        UnitOfMeasure uom = session.GetObjectByKey<UnitOfMeasure>(Guid.Parse(row.Values[14].ToString()));

                        if (new[] { ItemTypeEnum.AssemblyItem.ToString(), ItemTypeEnum.ChargeItem.ToString(), ItemTypeEnum.ProductGroup.ToString(), ItemTypeEnum.ServiceItem.ToString() }.Any(o => itemNo.ItemType.ToString().Contains(o)))
                        {
                            _message = string.Format("Generate row {0} succesfull.", index);
                            _BgWorker.ReportProgress(1, _message);
                            continue;
                        }

                        TransferItemsViewerDetail o_tivd = ReflectionHelper.CreateObject<TransferItemsViewerDetail>(session);
                        o_tivd.HeaderId = oGenerator;
                        // EntryDate
                        o_tivd.EntryDate = Convert.ToDateTime(row.Values[1].ToString());
                        // DateReceived
                        o_tivd.DateReceived = Convert.ToDateTime(row.Values[3].ToString());
                        // TransferNo
                        o_tivd.TransferNo = transferNo;
                        // ItemNo
                        o_tivd.ItemNo = itemNo;
                        // FromWarehouse
                        o_tivd.FromWarehouse = fromWarehouse;
                        // ToWarehouse
                        o_tivd.ToWarehouse = toWarehouse;
                        // RequisitionNo
                        o_tivd.RequisitionNo = requisitionNo;
                        // RequestID
                        o_tivd.RequestID = Guid.Parse(row.Values[9].ToString());
                        // Vendor
                        o_tivd.Vendor = vendor;
                        // Origin
                        o_tivd.Origin = origin;
                        // CostCenter
                        o_tivd.CostCenter = costCenter;
                        // Quantity
                        o_tivd.Quantity = Convert.ToDecimal(row.Values[13].ToString());
                        // UnitOfMeasure
                        o_tivd.UnitOfMeasure = uom;
                        // Cost
                        // Get Cost from the nearest receipt cost from Inventory Control Journal
                        string nc_qry = string.Format("select ItemNo, max(EntryDate) as ReceiptDate, Cost, UOM from vNearestReceiptCost " +
                            "where ItemNo='{0}' and EntryDate <= '{1}' group by ItemNo, Cost, UOM order by ReceiptDate", itemNo.Oid, o_tivd.DateReceived.Date.ToShortDateString());
                        SelectedData nc_data = session.ExecuteQuery(nc_qry);
                        bool costFound = false;
                        int rCount = nc_data.ResultSet[0].Rows.Count();
                        if (rCount > 0)
                        {
                            var sRow = nc_data.ResultSet[0].Rows[rCount - 1];
                            o_tivd.PurchaseUom = session.GetObjectByKey<UnitOfMeasure>(Guid.Parse(sRow.Values[3].ToString()));
                            decimal factor = 0;
                            decimal dFactor = 0;
                            if (itemNo.UOMRelations.Count > 0)
                            {
                                if (uom == o_tivd.PurchaseUom)
                                {
                                    factor = 1;
                                    dFactor = 1;
                                }
                                else
                                {
                                    if (itemNo.BaseUOM2 == uom)
                                    {
                                        factor = 1;
                                    }
                                    else
                                    {
                                        UOMRelation duomr = itemNo.UOMRelations.Where(o => o.UOM == uom).FirstOrDefault();
                                        factor = duomr.Factor;
                                    }

                                    if (itemNo.BaseUOM2 == o_tivd.PurchaseUom)
                                    {
                                        dFactor = 1;
                                    }
                                    else
                                    {
                                        UOMRelation duomr = itemNo.UOMRelations.Where(o => o.UOM == o_tivd.PurchaseUom).FirstOrDefault();
                                        dFactor = duomr.Factor;
                                    }
                                }

                                decimal mult = factor * dFactor;
                                o_tivd.Cost = Convert.ToDecimal(sRow.Values[2].ToString()) * mult;
                                costFound = true;
                            }
                        }
                        if (!costFound)
                        {
                            o_tivd.PurchaseUom = uom;
                            o_tivd.Cost = itemNo.Cost;
                        }
                        o_tivd.Save();
                    }

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    _message = string.Format("Generate row {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.ResultSet[0].Rows.Count())
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
                    "Generation is cancelled.", "Cancelled",
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
                    "Generation has been successfull.");
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
