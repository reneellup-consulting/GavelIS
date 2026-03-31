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
    public partial class PurchMovAnalysisToExcelController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Microsoft.Office.Interop.Excel.Application _Excel;
        private string _FileName;
        private SaveFileDialog _SaveFileDialog;
        private Microsoft.Office.Interop.Excel.Workbook _WorKbooK;
        private SimpleAction PurchMovAnalysisToExcelAction;
        private PurchasesMovementAnalysis _PmAnalysis;
        private ProgressForm _FrmProgress;
        private System.ComponentModel.BackgroundWorker _BgWorker;

        public PurchMovAnalysisToExcelController()
        {
            this.TargetObjectType = typeof(PurchasesMovementAnalysis);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "PurchMovAnalysisToExcelActionId";
            this.PurchMovAnalysisToExcelAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.PurchMovAnalysisToExcelAction.Caption = "Export to Excel";
            this.PurchMovAnalysisToExcelAction.Execute += new SimpleActionExecuteEventHandler(PurchMovAnalysisToExcelAction_Execute);
        }

        void PurchMovAnalysisToExcelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _PmAnalysis = this.View.CurrentObject as PurchasesMovementAnalysis;
            
            if (_PmAnalysis.PurchasesMovementBuffDetails.Count > 0)
            {
                if (XtraMessageBox.Show(string.Format("There are {0} lines(s) in this record. Do you want to continue?", _PmAnalysis.PurchasesMovementBuffDetails.Count), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

            _FrmProgress = new ProgressForm("Exporting...", _PmAnalysis.PurchasesMovementBuffDetails.Count,
                        "Exported line {0} of {1} ");
            _FrmProgress.CancelClick += new EventHandler(_FrmProgress_CancelClick);
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BgWorker_RunWorkerCompleted);
            _BgWorker.ProgressChanged += new ProgressChangedEventHandler(_BgWorker_ProgressChanged);
            _BgWorker.DoWork += new DoWorkEventHandler(_BgWorker_DoWork);
            _BgWorker.RunWorkerAsync(_PmAnalysis.PurchasesMovementBuffDetails);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        void _BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UnitOfWork session = CreateUpdatingSession();
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT = null;
            Microsoft.Office.Interop.Excel.Range celLrangE;
            int index = 0;
            XPCollection<PurchasesMovementBuffer> lines = (XPCollection<PurchasesMovementBuffer>)e.Argument;
            try
            {
                _Excel = new Microsoft.Office.Interop.Excel.Application();
                _Excel.Visible = false;
                _Excel.DisplayAlerts = false;
                _WorKbooK = _Excel.Workbooks.Add(Type.Missing);

                worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)_WorKbooK.ActiveSheet;
                worKsheeT.Name = "GENERATED_LINES";

                // ENTRY_DATE (1,1)
                worKsheeT.Cells[1, 1] = "ENTRY_DATE";
                worKsheeT.Cells[1, 1].Font.Size = 11;
                worKsheeT.Cells[1, 1].Font.Bold = true;

                // ITEM_NO (1,2)
                worKsheeT.Cells[1, 2] = "ITEM_NO";
                worKsheeT.Cells[1, 2].Font.Size = 11;
                worKsheeT.Cells[1, 2].Font.Bold = true;

                // ITEM_DESCRIPTION (1,3)
                worKsheeT.Cells[1, 3] = "ITEM_DESCRIPTION";
                worKsheeT.Cells[1, 3].Font.Size = 11;
                worKsheeT.Cells[1, 3].Font.Bold = true;

                // SEQ_ID (1,4)
                worKsheeT.Cells[1, 4] = "SEQ_ID";
                worKsheeT.Cells[1, 4].Font.Size = 11;
                worKsheeT.Cells[1, 4].Font.Bold = true;

                // SOURCE_TYPE (1,5)
                worKsheeT.Cells[1, 5] = "SOURCE_TYPE";
                worKsheeT.Cells[1, 5].Font.Size = 11;
                worKsheeT.Cells[1, 5].Font.Bold = true;

                // SOURCE_NO (1,6)
                worKsheeT.Cells[1, 6] = "SOURCE_NO";
                worKsheeT.Cells[1, 6].Font.Size = 11;
                worKsheeT.Cells[1, 6].Font.Bold = true;

                // REQUISITION (1,7)
                worKsheeT.Cells[1, 7] = "REQUISITION";
                worKsheeT.Cells[1, 7].Font.Size = 11;
                worKsheeT.Cells[1, 7].Font.Bold = true;

                // OUT_QTY (1,8)
                worKsheeT.Cells[1, 8] = "OUT_QTY";
                worKsheeT.Cells[1, 8].Font.Size = 11;
                worKsheeT.Cells[1, 8].Font.Bold = true;

                // IN_QTY (1,9)
                worKsheeT.Cells[1, 9] = "IN_QTY";
                worKsheeT.Cells[1, 9].Font.Size = 11;
                worKsheeT.Cells[1, 9].Font.Bold = true;

                // QTY_VAL (1,10)
                worKsheeT.Cells[1, 10] = "QTY_VAL";
                worKsheeT.Cells[1, 10].Font.Size = 11;
                worKsheeT.Cells[1, 10].Font.Bold = true;

                // WHRUN_QTY (1,11)
                worKsheeT.Cells[1, 11] = "WHRUN_QTY";
                worKsheeT.Cells[1, 11].Font.Size = 11;
                worKsheeT.Cells[1, 11].Font.Bold = true;

                // WAREHOUSE (1,12)
                worKsheeT.Cells[1, 12] = "WAREHOUSE";
                worKsheeT.Cells[1, 12].Font.Size = 11;
                worKsheeT.Cells[1, 12].Font.Bold = true;

                // COST (1,13)
                worKsheeT.Cells[1, 13] = "COST";
                worKsheeT.Cells[1, 13].Font.Size = 11;
                worKsheeT.Cells[1, 13].Font.Bold = true;

                // UOM (1,14)
                worKsheeT.Cells[1, 14] = "UOM";
                worKsheeT.Cells[1, 14].Font.Size = 11;
                worKsheeT.Cells[1, 14].Font.Bold = true;

                // PRICE (1,15)
                worKsheeT.Cells[1, 15] = "PRICE";
                worKsheeT.Cells[1, 15].Font.Size = 11;
                worKsheeT.Cells[1, 15].Font.Bold = true;

                // EXPENSE (1,16)
                worKsheeT.Cells[1, 16] = "EXPENSE";
                worKsheeT.Cells[1, 16].Font.Size = 11;
                worKsheeT.Cells[1, 16].Font.Bold = true;

                // SUB_EXPENSE (1,17)
                worKsheeT.Cells[1, 17] = "SUB_EXPENSE";
                worKsheeT.Cells[1, 17].Font.Size = 11;
                worKsheeT.Cells[1, 17].Font.Bold = true;

                // INCOME (1,18)
                worKsheeT.Cells[1, 18] = "INCOME";
                worKsheeT.Cells[1, 18].Font.Size = 11;
                worKsheeT.Cells[1, 18].Font.Bold = true;

                // EXPENSE (1,19)
                worKsheeT.Cells[1, 19] = "EXPENSE";
                worKsheeT.Cells[1, 19].Font.Size = 11;
                worKsheeT.Cells[1, 19].Font.Bold = true;

                int ln = 1;
                bool bld = false;
                foreach (var item in lines)
                {
                    index++;
                    _message = string.Format("Exporting line {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);
                    ln++;

                    if (!string.IsNullOrEmpty(item.SeqID))
                    {
                        bld = false;
                    }
                    else
                    {
                        bld = true;
                    }
                    //if (item.ItemNo == null || item.Warehouse == null || item.GenJournalID == null)
                    //{
                    //    ln--;
                    //    continue;
                    //}
                    #region Algorithms

                    // ENTRY_DATE (1,1)
                    worKsheeT.Cells[ln, 1] = item.EntryDate;
                    worKsheeT.Cells[ln, 1].NumberFormat = "MM/DD/yyyy HH:mm";
                    worKsheeT.Cells[ln, 1].Font.Size = 11;
                    worKsheeT.Cells[ln, 1].Font.Bold = bld;

                    // ITEM_NO (1,2)
                    worKsheeT.Cells[ln, 2] = item.Item.No;
                    worKsheeT.Cells[ln, 2].Font.Size = 11;
                    worKsheeT.Cells[ln, 2].Font.Bold = bld;

                    // ITEM_DESCRIPTION (1,3)
                    worKsheeT.Cells[ln, 3] = item.Item.Description;
                    worKsheeT.Cells[ln, 3].Font.Size = 11;
                    worKsheeT.Cells[ln, 3].Font.Bold = bld;

                    // SEQ_ID (1,4)
                    worKsheeT.Cells[ln, 4] = item.SeqID;
                    worKsheeT.Cells[ln, 4].Font.Size = 11;
                    worKsheeT.Cells[ln, 4].Font.Bold = bld;

                    // SOURCE_TYPE (1,5)
                    worKsheeT.Cells[ln, 5] = item.SourceType != null ? item.SourceType.Code : string.Empty;
                    worKsheeT.Cells[ln, 5].Font.Size = 11;
                    worKsheeT.Cells[ln, 5].Font.Bold = bld;

                    // SOURCE_NO (1,6)
                    worKsheeT.Cells[ln, 6] = item.Source != null ? item.Source.SourceNo : string.Empty;
                    worKsheeT.Cells[ln, 6].Font.Size = 11;
                    worKsheeT.Cells[ln, 6].Font.Bold = bld;

                    // REQUISITION (1,7)
                    worKsheeT.Cells[ln, 7] = item.Requisition != null ? item.Requisition.SourceNo : string.Empty;
                    worKsheeT.Cells[ln, 7].Font.Size = 11;
                    worKsheeT.Cells[ln, 7].Font.Bold = bld;

                    // OUT_QTY (1,8)
                    worKsheeT.Cells[ln, 8] = item.OutQty;
                    worKsheeT.Cells[ln, 8].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 8].Font.Size = 11;
                    worKsheeT.Cells[ln, 8].Font.Bold = bld;

                    // IN_QTY (1,9)
                    worKsheeT.Cells[ln, 9] = item.InQty;
                    worKsheeT.Cells[ln, 9].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 9].Font.Size = 11;
                    worKsheeT.Cells[ln, 9].Font.Bold = bld;

                    // QTY_VAL (1,10)
                    worKsheeT.Cells[ln, 10] = item.QtyValue;
                    worKsheeT.Cells[ln, 10].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 10].Font.Size = 11;
                    worKsheeT.Cells[ln, 10].Font.Bold = bld;

                    // WHRUN_QTY (1,11)
                    worKsheeT.Cells[ln, 11] = item.WhseRunQty;
                    worKsheeT.Cells[ln, 11].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 11].Font.Size = 11;
                    worKsheeT.Cells[ln, 11].Font.Bold = bld;

                    // WAREHOUSE (1,12)
                    worKsheeT.Cells[ln, 12] = item.Warehouse.Code;
                    worKsheeT.Cells[ln, 12].Font.Size = 11;
                    worKsheeT.Cells[ln, 12].Font.Bold = bld;

                    // COST (1,13)
                    worKsheeT.Cells[ln, 13] = item.Cost;
                    worKsheeT.Cells[ln, 13].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 13].Font.Size = 11;
                    worKsheeT.Cells[ln, 13].Font.Bold = bld;

                    // UOM (1,14)
                    worKsheeT.Cells[ln, 14] = item.Uom != null ? item.Uom.Code : string.Empty;
                    worKsheeT.Cells[ln, 14].Font.Size = 11;
                    worKsheeT.Cells[ln, 14].Font.Bold = bld;

                    // PRICE (1,15)
                    worKsheeT.Cells[ln, 15] = item.Price;
                    worKsheeT.Cells[ln, 15].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 15].Font.Size = 11;
                    worKsheeT.Cells[ln, 15].Font.Bold = bld;

                    // EXPENSE (1,16)
                    worKsheeT.Cells[ln, 16] = item.ExpenseType != null ? item.ExpenseType.Description : string.Empty;
                    worKsheeT.Cells[ln, 16].Font.Size = 11;
                    worKsheeT.Cells[ln, 16].Font.Bold = bld;

                    // SUB_EXPENSE (1,17)
                    worKsheeT.Cells[ln, 17] = item.SubExpenseType != null ? item.SubExpenseType.Description : string.Empty;
                    worKsheeT.Cells[ln, 17].Font.Size = 11;
                    worKsheeT.Cells[ln, 17].Font.Bold = bld;

                    // INCOME (1,18)
                    worKsheeT.Cells[ln, 18] = item.Income;
                    worKsheeT.Cells[ln, 18].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 18].Font.Size = 11;
                    worKsheeT.Cells[ln, 18].Font.Bold = bld;

                    // EXPENSE (1,19)
                    worKsheeT.Cells[ln, 19] = item.Expense;
                    worKsheeT.Cells[ln, 19].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 19].Font.Size = 11;
                    worKsheeT.Cells[ln, 19].Font.Bold = bld;

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
                    "Exporting purchase movement generated lines is cancelled.", "Cancelled",
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
                    "Exporting purchase movement generated lines has been successfull.");
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
