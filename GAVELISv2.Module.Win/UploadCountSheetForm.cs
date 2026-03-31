using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win
{
    public partial class UploadCountSheetForm : DevExpress.XtraEditors.XtraForm
    {
        private XafApplication _application;
        private System.ComponentModel.BackgroundWorker _BgWorker1;
        private ProgressForm _FrmProgress;
        public UploadCountSheetForm(IObjectSpace objectSpace, ITypeInfo typeInfo,
        CollectionSourceBase collectionSourceBase, XafApplication application)
        {
            InitializeComponent();
            if (objectSpace == null)
                throw new ArgumentNullException("objectSpace", "ObjectSpace cannot be null");
            //ObjectSpace = objectSpace;
            _application = application;
            ObjectSpace = collectionSourceBase.ObjectSpace;
            CurrentCollectionSource = collectionSourceBase;
        }
        public IObjectSpace ObjectSpace { get; private set; }
        public CollectionSourceBase CurrentCollectionSource { get; private set; }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".xlxs",
                Filter = "Excel Files(.xls)|*.xls|Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm"
            };
            // Display OpenFileDialog by calling ShowDialog method 
            DialogResult result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == DialogResult.OK)
            {
                // Open document 
                string filename = dlg.FileName;
                this.buttonEdit1.Text = filename;
            }
        }
        private Microsoft.Office.Interop.Excel.Workbook wb;
        Microsoft.Office.Interop.Excel.Application app;
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.buttonEdit1.Text))
            {
                throw new UserFriendlyException(
                "File path not specified");
            }
            app = new
                    Microsoft.Office.Interop.Excel.Application();
            wb = app.Workbooks.
            Open(this.buttonEdit1.Text, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing);

            // Iterate every sheet in the workbook
            int count = 0;
            for (int i = 1; i < wb.Worksheets.Count; i++)
            {
                Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)wb.Sheets[i];
                if ((sheet.UsedRange.Rows.Count - 5) > 0)
                {
                    count += sheet.UsedRange.Rows.Count - 5;
                }
            }

            _FrmProgress = new ProgressForm("Processing items...", count,
            "Row {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick1;
            _BgWorker1 = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress
                 = true
            };
            _BgWorker1.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker1.ProgressChanged += BgWorkerProgressChanged1;
            _BgWorker1.DoWork += BgWorkerDoWork1;
            _BgWorker1.RunWorkerAsync(new WorkerArgs(ObjectSpace, wb.Worksheets));
            _FrmProgress.ShowDialog();
        }
        private int iCount = 0;
        private void BgWorkerDoWork1(object sender, DoWorkEventArgs e)
        {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).Session.ObjectLayer);
            var workerArgs = ((WorkerArgs)e.Argument);
            var sheets = workerArgs.Sheets;
            int c = 0;
            for (int i = 1; i < sheets.Count; i++)
            {
                int n = 0;
                Microsoft.Office.Interop.Excel.Worksheet w_sheet = (Microsoft.Office.Interop.Excel.Worksheet)wb.Sheets[i];
                Microsoft.Office.Interop.Excel.Range usedRange = w_sheet.UsedRange;
                //int r = usedRange.Rows.Count - 5;
                foreach (Microsoft.Office.Interop.Excel.Range row in usedRange.Rows)
                {
                    c++;
                    n++;
                    if (n <= 5) continue;
                    if (_BgWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    #region Algorithms here...

                    int rowNumber = row.Row;
                    string[] A4D4 = GetRange("A" + rowNumber + ":G"
                            + rowNumber + "", w_sheet);
                    if (!string.IsNullOrEmpty(A4D4[6]))
                    {
                        string line_id = A4D4[6].ToString();
                        ItemsMovGrpCountDetail o_imgcd = session.GetObjectByKey<ItemsMovGrpCountDetail>(Convert.ToInt32(line_id));
                        if (o_imgcd != null)
                        {
                            o_imgcd.ActualQty = Convert.ToDecimal(A4D4[4].ToString());
                            o_imgcd.DateCounted = DateTime.Now;
                            o_imgcd.Remarks = A4D4[5].ToString();
                            o_imgcd.Save();
                        }
                    }

                    #endregion

                    string message = string.Empty;
                    _BgWorker1.ReportProgress(1, message);
                    Application.DoEvents();
                    //if (n == r)
                    //    session.CommitChanges();
                }
                session.CommitChanges();
            }
        }
        public string[] GetRange(string range, Microsoft.Office.Interop.Excel.
Worksheet excelWorksheet)
        {
            Microsoft.Office.Interop.Excel.Range workingRangeCells =
            excelWorksheet.get_Range(range, Type.Missing);
            //workingRangeCells.Select();
            System.Array array = (System.Array)workingRangeCells.Cells.Value2;
            string[] arrayS = this.ConvertToStringArray(array);
            return arrayS;
        }
        internal string[] ConvertToStringArray(System.Array values)
        {
            string[] theArray = new string[values.Length];
            for (int i = 1; i <= values.Length; i++)
            {
                if (values.GetValue(1, i) == null) theArray[i - 1] = "";
                else
                    theArray[i - 1] = (string)values.GetValue(1, i).ToString();
            }
            return theArray;
        }
        private void BgWorkerProgressChanged1(object sender, ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                //_FrmProgress.ChangeRecordCount(iCount);
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (wb != null)
            {
                wb.Close(false, Type.Missing, Type.Missing);
                app.Quit();
                releaseObject(wb);
                releaseObject(app);
            }
            if (e.Cancelled)
            {
                //ObjectSpace.Rollback();
                XtraMessageBox.Show(
                    "Uploading is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    //ObjectSpace.Rollback();
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Correction file successfully uploaded! Refresh this view to reflect changes.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    //ObjectSpace.Refresh();
                    this.Close();
                }
            }
        }

        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;

            }
            finally
            {
                GC.Collect();
            }
        }
        private void FrmProgressCancelClick1(object sender, EventArgs e)
        {
            _BgWorker1.CancelAsync();
        }
    }
}