using System;
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
    public partial class UploadCheckinCheckoutForm : DevExpress.XtraEditors.XtraForm {
        private XafApplication _application;
        private System.ComponentModel.BackgroundWorker _BgWorker1;
        private ProgressForm _FrmProgress;
        public UploadCheckinCheckoutForm(IObjectSpace objectSpace, ITypeInfo typeInfo,
        CollectionSourceBase collectionSourceBase, XafApplication application) {
            InitializeComponent();
            //DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            _application = application;
            if (objectSpace == null)
                throw new ArgumentNullException("objectSpace", "ObjectSpace cannot be null");
            //ObjectSpace = objectSpace;
            ObjectSpace = collectionSourceBase.ObjectSpace;
            CurrentCollectionSource = collectionSourceBase;
            
        }

        public IObjectSpace ObjectSpace { get; private set; }
        public CollectionSourceBase CurrentCollectionSource { get; private set; }
        private void btnCancel_Click(object sender, EventArgs e) {
            this.Close();
        }
        private Microsoft.Office.Interop.Excel.Workbook wb;
        private void btnUpload_Click(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(this.beSourceFile.Text))
            {
                throw new UserFriendlyException(
                "File path not specified");
            }
            Microsoft.Office.Interop.Excel.Application app = new
                    Microsoft.Office.Interop.Excel.Application();
            wb = app.Workbooks.
            Open(this.beSourceFile.Text, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.
            Office.Interop.Excel.Worksheet)wb.Sheets[1];

            _FrmProgress = new ProgressForm("Processing items...", sheet.UsedRange.Rows.Count-1,
            "Row {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick1;
            _BgWorker1 = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true,
                WorkerReportsProgress
                 = true
            };
            _BgWorker1.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker1.ProgressChanged += BgWorkerProgressChanged1;
            _BgWorker1.DoWork += BgWorkerDoWork1;
            _BgWorker1.RunWorkerAsync(new WorkerArgs(ObjectSpace, sheet.UsedRange.Rows, sheet, 1));
            _FrmProgress.ShowDialog();
        }

        private void beSourceFile_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog() { DefaultExt = ".xlxs", Filter = "Excel Files(.xls)|*.xls|Excel Files(.xlsx)|*.xlsx| Excel Files(*.xlsm)|*.xlsm"
            };
            // Display OpenFileDialog by calling ShowDialog method 
            DialogResult result = dlg.ShowDialog();
            // Get the selected file name and display in a TextBox 
            if (result == DialogResult.OK)
            {
                // Open document 
                string filename = dlg.FileName;
                this.beSourceFile.Text = filename;
            }
        }
        private int iCount = 0;
        private void BgWorkerDoWork1(object sender, DoWorkEventArgs e) {
            var workerArgs = ((WorkerArgs)e.Argument);
            var records = workerArgs.Rows;
            int i = 0;

            foreach (Microsoft.Office.Interop.Excel.Range excelRow in records)
            {
                ++i;
                if (i <= 1) continue;
                if (_BgWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                int rowNumber = excelRow.Row;
                string[] A4D4 = GetRange("A" + rowNumber + ":AC"
                        + rowNumber + "", workerArgs.Sheet);
                if (!string.IsNullOrEmpty(A4D4[0]))
                {
                    //string test = A4D4[0].ToString();
                    DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
                    CheckInAndOut cio = workerArgs.ObjectSpace.CreateObject<CheckInAndOut>();
                    cio.Employee = workerArgs.ObjectSpace.FindObject<Employee>(new BinaryOperator("IDNo", A4D4[1].ToString()));
                    cio.EmployeeNo = A4D4[1].ToString();
                    cio.Name = A4D4[3].ToString();
                    cio.AutoAssigned = !string.IsNullOrEmpty(A4D4[4].ToString())?A4D4[4].ToString()=="True"?true:false:false;
                    cio.Date = DateTime.Parse(A4D4[5].ToString());
                    cio.TimeTable = A4D4[6].ToString();
                    cio.OnDuty = A4D4[7].ToString();
                    cio.OffDuty = A4D4[8].ToString();
                    cio.ClockIn = A4D4[9].ToString();
                    cio.ClockOut = A4D4[10].ToString();
                    cio.Normal =!string.IsNullOrEmpty(A4D4[11].ToString())? decimal.Parse(A4D4[11].ToString()):0;
                    cio.RealTime = !string.IsNullOrEmpty(A4D4[12].ToString())? decimal.Parse(A4D4[12].ToString()):0;
                    cio.Late = A4D4[13].ToString();
                    cio.Early = A4D4[14].ToString();
                    cio.Absent = !string.IsNullOrEmpty(A4D4[15].ToString()) ? A4D4[15].ToString() == "True" ? true : false : false;
                    cio.OTTime = A4D4[16].ToString();
                    cio.WorkTime = A4D4[17].ToString();
                    cio.Exception = A4D4[18].ToString();
                    cio.MustCIn = !string.IsNullOrEmpty(A4D4[19].ToString()) ? A4D4[19].ToString() == "True" ? true : false : false;
                    cio.MustCOut = !string.IsNullOrEmpty(A4D4[20].ToString()) ? A4D4[20].ToString() == "True" ? true : false : false;
                    cio.Department = A4D4[21].ToString();
                    cio.NDays = !string.IsNullOrEmpty(A4D4[22].ToString())? decimal.Parse(A4D4[22].ToString()):0;
                    cio.WeekEnd = !string.IsNullOrEmpty(A4D4[23].ToString())? decimal.Parse(A4D4[23].ToString()):0;
                    cio.Holiday = !string.IsNullOrEmpty(A4D4[24].ToString()) ? decimal.Parse(A4D4[24].ToString()):0;
                    cio.ATTTime = A4D4[25].ToString();
                    cio.NDaysOT = A4D4[26].ToString();
                    cio.WeekEndOT = A4D4[27].ToString();
                    cio.HolidayOT = A4D4[18].ToString();
                    CurrentCollectionSource.Add(cio);
                    System.Threading.Thread.Sleep(300);

                }
                string message=string.Empty;

                if (i == records.Count)
                    workerArgs.ObjectSpace.CommitChanges();

                _BgWorker1.ReportProgress(1, message);
                Application.DoEvents();
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
        private void BgWorkerProgressChanged1(object sender, ProgressChangedEventArgs e) {
            if (_FrmProgress != null)
            {
                //_FrmProgress.ChangeRecordCount(iCount);
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }

        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (wb != null) { wb.Close(); }
            if (e.Cancelled)
            {
                ObjectSpace.Rollback();
                XtraMessageBox.Show(
                    "Uploading is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                if (e.Error != null)
                {
                    ObjectSpace.Rollback();
                    XtraMessageBox.Show(e.Error.Message,
                        "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                        Windows.Forms.MessageBoxIcon.Error);
                }
                else
                {
                    XtraMessageBox.Show(
                    "Log file successfully uploaded.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                this.Close();
                }
            }
        }

        private void FrmProgressCancelClick1(object sender, EventArgs e) {
            _BgWorker1.CancelAsync();
        }
    }
}