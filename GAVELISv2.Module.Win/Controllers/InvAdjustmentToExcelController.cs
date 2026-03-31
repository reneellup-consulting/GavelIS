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
    public partial class InvAdjustmentToExcelController : ViewController
    {
        private SimpleAction invAdjustmentToExcelAction;
        private PhysicalAdjustment _physicalAdjutment;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private SaveFileDialog saveFile;
        private string fileName;
        public InvAdjustmentToExcelController()
        {
            this.TargetObjectType = typeof(PhysicalAdjustment);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "PhysicalAdjustment.InvAdjustmentToExcelAction";
            this.invAdjustmentToExcelAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.invAdjustmentToExcelAction.Caption = "Export Details to Excel";
            this.invAdjustmentToExcelAction.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateStaffPayroll_Execute);
        }
        private void GenerateStaffPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _physicalAdjutment = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as PhysicalAdjustment;

            ObjectSpace.CommitChanges();

            var details = _physicalAdjutment.PhysicalAdjustmentDetails.OrderBy(o => o.Oid).Select(o => o);

            if (details.Count() == 0)
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
            _FrmProgress = new ProgressForm("Exporting entries...", details.Count(),
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
            _BgWorker.RunWorkerAsync(details);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<PhysicalAdjustmentDetail> _items = (IEnumerable<PhysicalAdjustmentDetail>)e.Argument;
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
                PhysicalAdjustment adj = session.GetObjectByKey<PhysicalAdjustment>(_physicalAdjutment.Oid);
                xlWorkSheet.Cells[1, 1] = "Source No";
                xlWorkSheet.Cells[1, 2] = "Entry Date";
                xlWorkSheet.Cells[1, 3] = "Item No";
                xlWorkSheet.Cells[1, 4] = "Description";
                xlWorkSheet.Cells[1, 5] = "Warehouse";
                xlWorkSheet.Cells[1, 6] = "Current Qty";
                xlWorkSheet.Cells[1, 7] = "UOM";
                xlWorkSheet.Cells[1, 8] = "Actual Qty";
                xlWorkSheet.Cells[1, 9] = "Difference";
                foreach (PhysicalAdjustmentDetail item in _items)
                {
                    index++;
                    n++;
                    _message = string.Format("Exporting {0} succesfull.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    xlWorkSheet.Cells[n, 1] = item.GenJournalID.SourceNo;
                    xlWorkSheet.Cells[n, 2] = item.GenJournalID.EntryDate;
                    xlWorkSheet.Cells[n, 3] = item.ItemNo.No;
                    xlWorkSheet.Cells[n, 4] = item.ItemNo.Description;
                    xlWorkSheet.Cells[n, 5] = item.Warehouse.Code;
                    xlWorkSheet.Cells[n, 6] = item.CurrentQtyBase;
                    xlWorkSheet.Cells[n, 7] = item.BaseUOM.Code;
                    xlWorkSheet.Cells[n, 8] = item.ActualQtyBase;
                    xlWorkSheet.Cells[n, 9] = item.DifferenceBase;
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
