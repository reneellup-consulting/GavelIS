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
    public partial class GenerateCountWorksheetController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Microsoft.Office.Interop.Excel.Application _Excel;
        private string _FileName;
        private SaveFileDialog _SaveFileDialog;
        private Microsoft.Office.Interop.Excel.Workbook _WorKbooK;
        private SimpleAction generateCountWorksheetAction;
        private ItemsMovGrpCountDetail _ItemsMovGrpCountDetail;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateCountWorksheetController()
        {
            this.TargetObjectType = typeof(ItemsMovGrpCountDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = "GenerateCountWorksheetActionId";
            this.generateCountWorksheetAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateCountWorksheetAction.Caption = "Generate Count Sheets";
            this.generateCountWorksheetAction.Execute += new SimpleActionExecuteEventHandler(generateCountWorksheetAction_Execute);
        }
        void generateCountWorksheetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
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
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT;
            Microsoft.Office.Interop.Excel.Sheets sheets;
            //Microsoft.Office.Interop.Excel.Range celLrangE;
            IEnumerable<ItemsMovGrpCountDetail> lines = ((IList)e.Argument).Cast<ItemsMovGrpCountDetail>();
            int index = 0;
            try
            {
                _Excel = new Microsoft.Office.Interop.Excel.Application();
                _Excel.Visible = false;
                _Excel.DisplayAlerts = false;
                _WorKbooK = _Excel.Workbooks.Add(Type.Missing);
                sheets = _WorKbooK.Sheets as Sheets;
                int wc = 0;
                var whses = lines.GroupBy(w => w.Warehouse).Select(o => new 
                    { Whse = o.Key, Count = o.Select(d => d.Oid).Distinct().Count() });
                foreach (var w in whses)
                {
                    wc++;
                    worKsheeT = (Worksheet)sheets.Add(sheets[wc], Type.Missing, Type.Missing, Type.Missing);
                    worKsheeT.Name = w.Whse.Code;

                    worKsheeT.Columns["A:A"].ColumnWidth = 14;
                    worKsheeT.Columns["B:B"].ColumnWidth = 69;
                    worKsheeT.Columns["C:C"].ColumnWidth = 11.5;
                    worKsheeT.Columns["D:D"].ColumnWidth = 11.5;
                    worKsheeT.Columns["E:E"].ColumnWidth = 11.5;
                    worKsheeT.Columns["F:F"].ColumnWidth = 39;
                    worKsheeT.Columns["G:G"].ColumnWidth = 11.5;
                    worKsheeT.Columns["G:G"].Hidden = true;

                    worKsheeT.Cells[1, 1] = "GS GAVEL LOGISTICS CO., INC.";
                    worKsheeT.Cells[1, 1].Font.Size = 15;
                    worKsheeT.Cells[1, 1].Font.Bold = true;

                    worKsheeT.Cells[2, 1] = string.Format("{0} PHYSICAL COUNT SHEET", w.Whse.Code);
                    worKsheeT.Cells[2, 1].Font.Size = 12;
                    worKsheeT.Cells[2, 1].Font.Bold = true;

                    worKsheeT.Cells[3, 1] = DateTime.Now.ToLongDateString();
                    worKsheeT.Cells[3, 1].Font.Size = 11;
                    worKsheeT.Cells[3, 1].Font.Bold = true;

                    // ITEM_NO (5,1)

                    worKsheeT.Cells[5, 1] = "ITEM_NO";
                    worKsheeT.Cells[5, 1].Font.Size = 11;
                    worKsheeT.Cells[5, 1].Font.Bold = true;

                    // DESCRIPTION (5,2)
                    worKsheeT.Cells[5, 2] = "DESCRIPTION";
                    worKsheeT.Cells[5, 2].Font.Size = 11;
                    worKsheeT.Cells[5, 2].Font.Bold = true;

                    // CURR_QTY (5,3)
                    worKsheeT.Cells[5, 3] = "CURR_QTY";
                    worKsheeT.Cells[5, 3].Font.Size = 11;
                    worKsheeT.Cells[5, 3].Font.Bold = true;

                    // STCK_UOM (5,4)
                    worKsheeT.Cells[5, 4] = "STCK_UOM";
                    worKsheeT.Cells[5, 4].Font.Size = 11;
                    worKsheeT.Cells[5, 4].Font.Bold = true;

                    // ACTL_QTY (5,5)
                    worKsheeT.Cells[5, 5] = "ACTL_QTY";
                    worKsheeT.Cells[5, 5].Font.Size = 11;
                    worKsheeT.Cells[5, 5].Font.Bold = true;

                    // REMARKS (5,6)
                    worKsheeT.Cells[5, 6] = "REMARKS";
                    worKsheeT.Cells[5, 6].Font.Size = 11;
                    worKsheeT.Cells[5, 6].Font.Bold = true;

                    // LINE_ID (5,7)
                    worKsheeT.Cells[5, 7] = "LINE_ID";
                    worKsheeT.Cells[5, 7].Font.Size = 11;
                    worKsheeT.Cells[5, 7].Font.Bold = true;

                    var w_lines = lines.Where(o => o.Warehouse.Code == w.Whse.Code).OrderByDescending(o => o.Activity);
                    int ln = 5;
                    foreach (var item in w_lines)
                    {
                        index++;
                        _message = string.Format("Processing row {0} succesfull.",
                        index);
                        _BgWorker.ReportProgress(1, _message);
                        ln++;
                        #region Algorithms

                        // ITEM_NO
                        worKsheeT.Cells[ln, 1] = item.ItemNo.No;
                        worKsheeT.Cells[ln, 1].Font.Size = 11;
                        worKsheeT.Cells[ln, 1].Font.Bold = false;

                        // DESCRIPTION
                        worKsheeT.Cells[ln, 2] = item.ItemNo.Description;
                        worKsheeT.Cells[ln, 2].Font.Size = 11;
                        worKsheeT.Cells[ln, 2].Font.Bold = false;

                        // CURR_QTY
                        worKsheeT.Cells[ln, 3] = item.CurrQty;
                        worKsheeT.Cells[ln, 3].NumberFormat = "#,##0.00";
                        worKsheeT.Cells[ln, 3].Font.Size = 11;
                        worKsheeT.Cells[ln, 3].Font.Bold = false;

                        // STCK_UOM
                        worKsheeT.Cells[ln, 4] = item.StockUnit.Code;
                        worKsheeT.Cells[ln, 4].Font.Size = 11;
                        worKsheeT.Cells[ln, 4].Font.Bold = false;

                        // ACTL_QTY
                        worKsheeT.Cells[ln, 5].NumberFormat = "#,##0.00";
                        worKsheeT.Cells[ln, 5].Font.Size = 11;
                        worKsheeT.Cells[ln, 5].Font.Bold = false;
                        Borders border1 = worKsheeT.Range[worKsheeT.Cells[ln, 5], worKsheeT.Cells[ln, 5]].Borders;
                        border1[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;

                        // REMARKS
                        worKsheeT.Cells[ln, 6].Font.Size = 11;
                        worKsheeT.Cells[ln, 6].Font.Bold = false;
                        Borders border2 = worKsheeT.Range[worKsheeT.Cells[ln, 6], worKsheeT.Cells[ln, 6]].Borders;
                        border2[XlBordersIndex.xlEdgeBottom].LineStyle = XlLineStyle.xlContinuous;

                        // LINE_ID
                        worKsheeT.Cells[ln, 7] = item.Oid;
                        worKsheeT.Cells[ln, 7].Font.Size = 11;
                        worKsheeT.Cells[ln, 7].Font.Bold = false;

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
            }
            finally
            {
                if (index == Convert.ToInt32(lines.Count().ToString()))
                {
                    e.Result = index;
                    _WorKbooK.SaveAs(_FileName);
                    _WorKbooK.Close();
                    _Excel.Quit();
                }
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
                    "Generating worsheet is cancelled.", "Cancelled",
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
                    "Worsheet generation has been successfull.");
                    //ObjectSpace.Refresh();
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
