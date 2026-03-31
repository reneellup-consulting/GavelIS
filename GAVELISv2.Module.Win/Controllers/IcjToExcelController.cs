using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using DevExpress.Xpo;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using System.Runtime.InteropServices;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class IcjToExcelController : ViewController
    {
        private SimpleAction icjToExcelAction;
        private Item _item;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private SaveFileDialog saveFile;
        private string fileName;
        public IcjToExcelController()
        {
            this.TargetObjectType = typeof(Item);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "Item.IcjToExcelAction";
            this.icjToExcelAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.icjToExcelAction.Caption = "Export ICJ to Excel";
            this.icjToExcelAction.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateStaffPayroll_Execute);
        }
        private void GenerateStaffPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _item = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as Item;

            ObjectSpace.CommitChanges();

            var _icjs = _item.InventoryControlJournals.OrderBy(o => o.Sequence).Select(o => o);

            if (_icjs.Count() == 0)
            {
                throw new UserFriendlyException("There are no entries found");
            }
            saveFile = new SaveFileDialog();
            saveFile.Filter = "Excel Files(.xlsx)|*.xlsx";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFile.FileName;
            }
            else
            {
                return;
            }
            _FrmProgress = new ProgressForm("Exporting entries...", _icjs.Count(),
            "Export entry {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_icjs);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<InventoryControlJournal> _items = (IEnumerable<InventoryControlJournal>)e.Argument;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet = null;
            object misValue = System.Reflection.Missing.Value;
            try
            {
                if (xlApp == null)
                {
                    throw new ApplicationException("Excel is not properly installed!!");
                }
                int n = 1;
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                Item itm = session.GetObjectByKey<Item>(_item.Oid);
                xlWorkSheet.Cells[1, 1] = "Entry Date";
                xlWorkSheet.Cells[1, 2] = "Sequence";
                xlWorkSheet.Cells[1, 3] = "Item";
                xlWorkSheet.Cells[1, 4] = "Warehouse";
                xlWorkSheet.Cells[1, 5] = "In QTY";
                xlWorkSheet.Cells[1, 6] = "Out QTY";
                xlWorkSheet.Cells[1, 7] = "QTY";
                xlWorkSheet.Cells[1, 8] = "RQTY";
                xlWorkSheet.Cells[1, 9] = "RQTY/WHSE";
                xlWorkSheet.Cells[1, 10] = "ST";
                xlWorkSheet.Cells[1, 11] = "Source No";
                xlWorkSheet.Cells[1, 12] = "Created By";
                xlWorkSheet.Cells[1, 13] = "Created On";
                foreach (InventoryControlJournal item in _items)
                {
                    index++;
                    n++;
                    _message = string.Format("Exporting {0} succesfull.",
                    item.Sequence);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    xlWorkSheet.Cells[n, 1] = item.Date.ToShortDateString();
                    xlWorkSheet.Cells[n, 2] = item.Sequence;
                    xlWorkSheet.Cells[n, 3] = itm.No;
                    xlWorkSheet.Cells[n, 4] = item.Warehouse.Code;
                    xlWorkSheet.Cells[n, 5] = item.InQTY;
                    xlWorkSheet.Cells[n, 6] = item.OutQty;
                    xlWorkSheet.Cells[n, 7] = item.Qty;
                    xlWorkSheet.Cells[n, 8] = item.RunningQtyAll;
                    xlWorkSheet.Cells[n, 9] = item.RunningQtyWhse;
                    xlWorkSheet.Cells[n, 10] = item.SourceTypeCode;
                    xlWorkSheet.Cells[n, 11] = item.GenJournalID.SourceNo;
                    xlWorkSheet.Cells[n, 12] = item.CreatedBy;
                    xlWorkSheet.Cells[n, 13] = item.CreatedOn;
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
                if (index == _items.Count())
                {
                    
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        xlWorkBook.SaveAs(fileName);
                    }
                    
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();

                    Marshal.ReleaseComObject(xlWorkSheet);
                    Marshal.ReleaseComObject(xlWorkBook);
                    Marshal.ReleaseComObject(xlApp);
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
                    "Exporting entries is cancelled.", "Cancelled",
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
                    "Entries has been successfully exported.");
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
