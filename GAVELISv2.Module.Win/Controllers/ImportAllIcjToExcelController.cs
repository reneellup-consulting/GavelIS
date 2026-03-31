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
using System.Runtime.InteropServices;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ImportAllIcjToExcelController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Microsoft.Office.Interop.Excel.Application _Excel;
        private string _FileName;
        private SaveFileDialog _SaveFileDialog;
        private Microsoft.Office.Interop.Excel.Workbook _WorKbooK;
        private SimpleAction ImportAllIcjToExcelAction;
        private InventoryControlJournal _Icjs;
        private ProgressForm _FrmProgress;
        private System.ComponentModel.BackgroundWorker _BgWorker;

        public ImportAllIcjToExcelController()
        {
            this.TargetObjectType = typeof(InventoryControlJournal);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ImportAllIcjToExcelActionId";
            this.ImportAllIcjToExcelAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.ImportAllIcjToExcelAction.Caption = "Export all to Excel";
            this.ImportAllIcjToExcelAction.Execute += new SimpleActionExecuteEventHandler(ImportAllIcjToExcelAction_Execute);
        }

        void ImportAllIcjToExcelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _Icjs = this.View.CurrentObject as InventoryControlJournal;
            if (_Icjs == null)
            {
                throw new UserFriendlyException("Plase select one item");
            }
            XPCollection<InventoryControlJournal> icjs = new XPCollection<InventoryControlJournal>(_Icjs.Session);
            IEnumerable<InventoryControlJournal> data = icjs.Select(o=>o);
            if (data != null && data.Count() > 0)
            {
                if (XtraMessageBox.Show(string.Format("There are {0} items(s) retreived. Do you want to continue?", data.Count()), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }
            }
            _SaveFileDialog = new SaveFileDialog();
            _SaveFileDialog.Filter = "Excel files (*.xlsx) | *.xlsx | All files (*.*) | *.*";
            _SaveFileDialog.FilterIndex = 1;
            _SaveFileDialog.RestoreDirectory = true;
            if (_SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                _FileName = _SaveFileDialog.FileName;
            }
            else
            {
                return;
            }

            _FrmProgress = new ProgressForm("Exporting...", data.Count(),
                        "Items processed {0} of {1} ");
            _FrmProgress.CancelClick += new EventHandler(_FrmProgress_CancelClick);
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BgWorker_RunWorkerCompleted);
            _BgWorker.ProgressChanged += new ProgressChangedEventHandler(_BgWorker_ProgressChanged);
            _BgWorker.DoWork += new DoWorkEventHandler(_BgWorker_DoWork);
            _BgWorker.RunWorkerAsync(data);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        void _BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UnitOfWork session = CreateUpdatingSession();
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT = null;
            Microsoft.Office.Interop.Excel.Range celLrangE;
            int index = 0;
            IEnumerable<InventoryControlJournal> lines = (IEnumerable<InventoryControlJournal>)e.Argument;
            try
            {
                _Excel = new Microsoft.Office.Interop.Excel.Application();
                _Excel.Visible = false;
                _Excel.DisplayAlerts = false;
                _WorKbooK = _Excel.Workbooks.Add(Type.Missing);

                worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)_WorKbooK.ActiveSheet;
                worKsheeT.Name = "ICJ_LINES";

                // ENTRY DATE (1,1)
                worKsheeT.Cells[1, 1] = "ENTRY DATE";
                worKsheeT.Cells[1, 1].Font.Size = 11;
                worKsheeT.Cells[1, 1].Font.Bold = true;

                // SEQUENCE (1,2)
                worKsheeT.Cells[1, 2] = "SEQUENCE";
                worKsheeT.Cells[1, 2].Font.Size = 11;
                worKsheeT.Cells[1, 2].Font.Bold = true;

                // ITEM (1,3)
                worKsheeT.Cells[1, 3] = "ITEM";
                worKsheeT.Cells[1, 3].Font.Size = 11;
                worKsheeT.Cells[1, 3].Font.Bold = true;

                // WAREHOUSE (1,4)
                worKsheeT.Cells[1, 4] = "WAREHOUSE";
                worKsheeT.Cells[1, 4].Font.Size = 11;
                worKsheeT.Cells[1, 4].Font.Bold = true;

                // INQTY (1,5)
                worKsheeT.Cells[1, 5] = "INQTY";
                worKsheeT.Cells[1, 5].Font.Size = 11;
                worKsheeT.Cells[1, 5].Font.Bold = true;

                // OTQTY (1,6)
                worKsheeT.Cells[1, 6] = "OTQTY";
                worKsheeT.Cells[1, 6].Font.Size = 11;
                worKsheeT.Cells[1, 6].Font.Bold = true;

                // QTY (1,7)
                worKsheeT.Cells[1, 7] = "QTY";
                worKsheeT.Cells[1, 7].Font.Size = 11;
                worKsheeT.Cells[1, 7].Font.Bold = true;

                // RQTY (1,8)
                worKsheeT.Cells[1, 8] = "RQTY";
                worKsheeT.Cells[1, 8].Font.Size = 11;
                worKsheeT.Cells[1, 8].Font.Bold = true;

                // RQTY/WHSE (1,9)
                worKsheeT.Cells[1, 9] = "RQTY/WHSE";
                worKsheeT.Cells[1, 9].Font.Size = 11;
                worKsheeT.Cells[1, 9].Font.Bold = true;

                // ST (1,10)
                worKsheeT.Cells[1, 10] = "ST";
                worKsheeT.Cells[1, 10].Font.Size = 11;
                worKsheeT.Cells[1, 10].Font.Bold = true;

                // SOURCE (1,11)
                worKsheeT.Cells[1, 11] = "SOURCE";
                worKsheeT.Cells[1, 11].Font.Size = 11;
                worKsheeT.Cells[1, 11].Font.Bold = true;

                // CREATEDBY (1,12)
                worKsheeT.Cells[1, 12] = "CREATEDBY";
                worKsheeT.Cells[1, 12].Font.Size = 11;
                worKsheeT.Cells[1, 12].Font.Bold = true;

                // CREATEDON (1,13)
                worKsheeT.Cells[1, 13] = "CREATEDON";
                worKsheeT.Cells[1, 13].Font.Size = 11;
                worKsheeT.Cells[1, 13].Font.Bold = true;

                int ln = 1;
                foreach (var item in lines) {
                    index++;
                    _message = string.Format("Exporting line {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);
                    ln++;

                    if (item.ItemNo == null || item.Warehouse == null || item.GenJournalID == null)
                    {
                        ln--;
                        continue;
                    }
                    #region Algorithms

                    // ENTRY DATE (1,1)
                    worKsheeT.Cells[ln, 1] = item.Date.ToShortDateString();
                    worKsheeT.Cells[ln, 1].Font.Size = 11;
                    worKsheeT.Cells[ln, 1].Font.Bold = false;

                    // SEQUENCE (1,2)
                    worKsheeT.Cells[ln, 2] = "SQ-" + item.Sequence;
                    worKsheeT.Cells[ln, 2].Font.Size = 11;
                    worKsheeT.Cells[ln, 2].Font.Bold = false;

                    // ITEM (1,3)

                    Item itm = session.GetObjectByKey<Item>(item.ItemNo.Oid);

                    worKsheeT.Cells[ln, 3] = itm.No;
                    worKsheeT.Cells[ln, 3].Font.Size = 11;
                    worKsheeT.Cells[ln, 3].Font.Bold = false;

                    // WAREHOUSE (1,4)
                    worKsheeT.Cells[ln, 4] = item.Warehouse.Code;
                    worKsheeT.Cells[ln, 4].Font.Size = 11;
                    worKsheeT.Cells[ln, 4].Font.Bold = false;

                    // INQTY (1,5)
                    worKsheeT.Cells[ln, 5] = item.InQTY;
                    worKsheeT.Cells[ln, 5].Font.Size = 11;
                    worKsheeT.Cells[ln, 5].Font.Bold = false;

                    // OTQTY (1,6)
                    worKsheeT.Cells[ln, 6] = item.OutQty;
                    worKsheeT.Cells[ln, 6].Font.Size = 11;
                    worKsheeT.Cells[ln, 6].Font.Bold = false;

                    // QTY (1,7)
                    worKsheeT.Cells[ln, 7] = item.Qty;
                    worKsheeT.Cells[ln, 7].Font.Size = 11;
                    worKsheeT.Cells[ln, 7].Font.Bold = false;

                    // RQTY (1,8)
                    worKsheeT.Cells[ln, 8] = item.RunningQtyAll;
                    worKsheeT.Cells[ln, 8].Font.Size = 11;
                    worKsheeT.Cells[ln, 8].Font.Bold = false;

                    // RQTY/WHSE (1,9)
                    worKsheeT.Cells[ln, 9] = item.RunningQtyWhse;
                    worKsheeT.Cells[ln, 9].Font.Size = 11;
                    worKsheeT.Cells[ln, 9].Font.Bold = false;

                    // ST (1,10)
                    worKsheeT.Cells[ln, 10] = item.SourceTypeCode;
                    worKsheeT.Cells[ln, 10].Font.Size = 11;
                    worKsheeT.Cells[ln, 10].Font.Bold = false;

                    // SOURCE (1,11)
                    worKsheeT.Cells[ln, 11] = item.GenJournalID.SourceNo;
                    worKsheeT.Cells[ln, 11].Font.Size = 11;
                    worKsheeT.Cells[ln, 11].Font.Bold = false;

                    // CREATEDBY (1,12)
                    worKsheeT.Cells[ln, 12] = item.CreatedBy;
                    worKsheeT.Cells[ln, 12].Font.Size = 11;
                    worKsheeT.Cells[ln, 12].Font.Bold = false;

                    // CREATEDON (1,13)
                    worKsheeT.Cells[ln, 13] = item.CreatedOn;
                    worKsheeT.Cells[ln, 13].Font.Size = 11;
                    worKsheeT.Cells[ln, 13].Font.Bold = false;

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        _WorKbooK.Close();
                        _Excel.Quit();
                        e.Cancel = true;
                        break;
                    }
                }
            }
            finally
            {
                if (index == lines.Count())
                {

                    if (!string.IsNullOrEmpty(_FileName))
                    {
                        _WorKbooK.SaveAs(_FileName);
                    }

                    _WorKbooK.Close(true, Type.Missing, Type.Missing);
                    _Excel.Quit();

                    Marshal.ReleaseComObject(worKsheeT);
                    Marshal.ReleaseComObject(_WorKbooK);
                    Marshal.ReleaseComObject(_Excel);
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
        void _BgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }

        void _BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Exporting inventory control data is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    _WorKbooK.Close();
                    _Excel.Quit();
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Exporting inventory control data has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
                    ObjectSpace.Refresh();
                }
            }
        }

        void _FrmProgress_CancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
