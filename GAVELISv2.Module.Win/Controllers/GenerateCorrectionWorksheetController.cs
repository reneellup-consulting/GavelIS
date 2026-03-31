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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateCorrectionWorksheetController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Microsoft.Office.Interop.Excel.Application _Excel;
        private string _FileName;
        private SaveFileDialog _SaveFileDialog;
        private Microsoft.Office.Interop.Excel.Workbook _WorKbooK;
        private PopupWindowShowAction GenerateCorrectionWorksheetAction;
        private IncomeAndExpense02 _IncomeExpense02;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private CorrectionWorksheetParam _Obj;
        private ProgressForm _FrmProgress;
        public GenerateCorrectionWorksheetController()
        {
            this.TargetObjectType = typeof(IncomeAndExpense02);
            this.TargetViewType = ViewType.ListView;
            string actionID = "GenerateCorrectionWorksheetActionId";
            this.GenerateCorrectionWorksheetAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GenerateCorrectionWorksheetAction.Caption = "Generate Correction WS";
            this.GenerateCorrectionWorksheetAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(GenerateCorrectionWorksheetAction_CustomizePopupWindowParams);
            this.GenerateCorrectionWorksheetAction.Execute += new PopupWindowShowActionExecuteEventHandler(GenerateCorrectionWorksheetAction_Execute);
        }

        void GenerateCorrectionWorksheetAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Obj = new CorrectionWorksheetParam();
            _Obj.EnterYear = DateTime.Now.Year;
            //objectSpace.CommitChanges();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CorrectionWorksheetParam_DetailView", true, _Obj);
        }

        void GenerateCorrectionWorksheetAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _IncomeExpense02 = this.View.CurrentObject as IncomeAndExpense02;
            if (_IncomeExpense02 == null)
            {
                throw new UserFriendlyException("No lines exist");
            }
            XPCollection<IncomeAndExpense02> incxs = new XPCollection<IncomeAndExpense02>(_IncomeExpense02.Session);
            IEnumerable<IncomeAndExpense02> data = incxs.Where(o => o.GYear == _Obj.EnterYear).OrderBy(o => o.EntryDate).OrderBy(o => o.Category);
            if (data == null && data.Count() == 0)
            {
                throw new ApplicationException("Data does not exist!");
            }
            else
            {
                if (XtraMessageBox.Show(string.Format("There are {0} line(s) retreived. Do you want to continue?", data.Count()), "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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
            
            _FrmProgress = new ProgressForm("Generating...", data.Count(),
                        "Lines processed {0} of {1} ");
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
            Microsoft.Office.Interop.Excel.Worksheet worKsheeT;
            Microsoft.Office.Interop.Excel.Range celLrangE;  
            int index = 0;
            IEnumerable<IncomeAndExpense02> lines = (IEnumerable<IncomeAndExpense02>)e.Argument;
            try
            {
                _Excel = new Microsoft.Office.Interop.Excel.Application();
                _Excel.Visible = false;
                _Excel.DisplayAlerts = false;
                _WorKbooK = _Excel.Workbooks.Add(Type.Missing);

                worKsheeT = (Microsoft.Office.Interop.Excel.Worksheet)_WorKbooK.ActiveSheet;
                worKsheeT.Name = "Generated";

                worKsheeT.Cells[1, 1] = "GSC GAVEL ENTERPRISES";
                worKsheeT.Cells[1, 1].Font.Size = 15;
                worKsheeT.Cells[1, 1].Font.Bold = true;

                worKsheeT.Cells[2, 1] = "Expense Type Correction Worksheet";
                worKsheeT.Cells[2, 1].Font.Size = 12;
                worKsheeT.Cells[2, 1].Font.Bold = true;

                worKsheeT.Cells[3, 1] = DateTime.Now.ToLongDateString();
                worKsheeT.Cells[3, 1].Font.Size = 11;
                worKsheeT.Cells[3, 1].Font.Bold = true;

                // OID (1,5)
                worKsheeT.Cells[5, 1] = "OID";
                worKsheeT.Cells[5, 1].Font.Size = 11;
                worKsheeT.Cells[5, 1].Font.Bold = true;
                // ENTRY DATE (2,5)
                worKsheeT.Cells[5, 2] = "ENTRY DATE";
                worKsheeT.Cells[5, 2].Font.Size = 11;
                worKsheeT.Cells[5, 2].Font.Bold = true;
                // YEAR (3,5)
                worKsheeT.Cells[5, 3] = "YEAR";
                worKsheeT.Cells[5, 3].Font.Size = 11;
                worKsheeT.Cells[5, 3].Font.Bold = true;
                // SOURCE (4,5)
                worKsheeT.Cells[5, 4] = "SOURCE";
                worKsheeT.Cells[5, 4].Font.Size = 11;
                worKsheeT.Cells[5, 4].Font.Bold = true;
                // PAYEE (5,5)
                worKsheeT.Cells[5, 5] = "PAYEE";
                worKsheeT.Cells[5, 5].Font.Size = 11;
                worKsheeT.Cells[5, 5].Font.Bold = true;
                // DESCRIPTION 1 (6,5)
                worKsheeT.Cells[5, 6] = "DESCRIPTION 1";
                worKsheeT.Cells[5, 6].Font.Size = 11;
                worKsheeT.Cells[5, 6].Font.Bold = true;
                // DESCRIPTION 2 (7,5)
                worKsheeT.Cells[5, 7] = "DESCRIPTION 2";
                worKsheeT.Cells[5, 7].Font.Size = 11;
                worKsheeT.Cells[5, 7].Font.Bold = true;
                // CHARGE TO (8,5)
                worKsheeT.Cells[5, 8] = "CHARGE TO";
                worKsheeT.Cells[5, 8].Font.Size = 11;
                worKsheeT.Cells[5, 8].Font.Bold = true;
                // FLEET (9,5)
                worKsheeT.Cells[5, 9] = "FLEET";
                worKsheeT.Cells[5, 9].Font.Size = 11;
                worKsheeT.Cells[5, 9].Font.Bold = true;
                // CORR? (10,5)
                worKsheeT.Cells[5, 10] = "CORR?";
                worKsheeT.Cells[5, 10].Font.Size = 11;
                worKsheeT.Cells[5, 10].Font.Bold = true;
                // CORR DATE (11,5)
                worKsheeT.Cells[5, 11] = "CORR DATE";
                worKsheeT.Cells[5, 11].Font.Size = 11;
                worKsheeT.Cells[5, 11].Font.Bold = true;
                // CURR. CATEGORY (12,5)
                worKsheeT.Cells[5, 12] = "CURR. CATEGORY";
                worKsheeT.Cells[5, 12].Font.Size = 11;
                worKsheeT.Cells[5, 12].Font.Bold = true;
                // CURR. SUB CAT. (13,5)
                worKsheeT.Cells[5, 13] = "CURR. SUB CAT.";
                worKsheeT.Cells[5, 13].Font.Size = 11;
                worKsheeT.Cells[5, 13].Font.Bold = true;
                // CORR. CATEGORY (14,5)
                worKsheeT.Cells[5, 14] = "CORR. CATEGORY";
                worKsheeT.Cells[5, 14].Font.Size = 11;
                worKsheeT.Cells[5, 14].Font.Bold = true;
                // CORR. SUB CAT. (15,5)
                worKsheeT.Cells[5, 15] = "CORR. SUB CAT.";
                worKsheeT.Cells[5, 15].Font.Size = 11;
                worKsheeT.Cells[5, 15].Font.Bold = true;
                // EXPENSE
                worKsheeT.Cells[5, 16] = "EXPENSE";
                worKsheeT.Cells[5, 16].Font.Size = 11;
                worKsheeT.Cells[5, 16].Font.Bold = true;
                int ln = 5;
                foreach (var item in lines)
                {
                    index++;
                    _message = string.Format("Processing line {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);
                    ln++;
                    #region Algorithms
                    if (item.Category.Income)
                    {
                        ln--;
                        continue;
                    }
                    // OID (1,5)
                    worKsheeT.Cells[ln, 1] = item.LineID;
                    worKsheeT.Cells[ln, 1].Font.Size = 11;
                    worKsheeT.Cells[ln, 1].Font.Bold = false;
                    // ENTRY DATE (2,5)
                    worKsheeT.Cells[ln, 2] = item.EntryDate.ToShortDateString();
                    worKsheeT.Cells[ln, 2].Font.Size = 11;
                    worKsheeT.Cells[ln, 2].Font.Bold = false;
                    // YEAR (3,5)
                    worKsheeT.Cells[ln, 3] = item.GYear.ToString();
                    worKsheeT.Cells[ln, 3].Font.Size = 11;
                    worKsheeT.Cells[ln, 3].Font.Bold = false;
                    // SOURCE (4,5)
                    worKsheeT.Cells[ln, 4] = item.SourceNo;
                    worKsheeT.Cells[ln, 4].Font.Size = 11;
                    worKsheeT.Cells[ln, 4].Font.Bold = false;
                    // PAYEE (5,5)
                    worKsheeT.Cells[ln, 5] = item.PayeeName;
                    worKsheeT.Cells[ln, 5].Font.Size = 11;
                    worKsheeT.Cells[ln, 5].Font.Bold = false;
                    // DESCRIPTION 1 (6,5)
                    worKsheeT.Cells[ln, 6] = item.Description1;
                    worKsheeT.Cells[ln, 6].Font.Size = 11;
                    worKsheeT.Cells[ln, 6].Font.Bold = false;
                    // DESCRIPTION 2 (7,5)
                    worKsheeT.Cells[ln, 7] = item.Description2;
                    worKsheeT.Cells[ln, 7].Font.Size = 11;
                    worKsheeT.Cells[ln, 7].Font.Bold = false;
                    // CHARGE TO (8,5)
                    worKsheeT.Cells[ln, 8] = item.CostCenter != null ? item.CostCenter.Code : string.Empty;
                    worKsheeT.Cells[ln, 8].Font.Size = 11;
                    worKsheeT.Cells[ln, 8].Font.Bold = false;
                    // FLEET (9,5)
                    worKsheeT.Cells[ln, 9] = item.Fleet != null ? item.Fleet.Code : string.Empty;
                    worKsheeT.Cells[ln, 9].Font.Size = 11;
                    worKsheeT.Cells[ln, 9].Font.Bold = false;
                    // CORR? (10,5)
                    worKsheeT.Cells[ln, 10] = item.Corrected;
                    worKsheeT.Cells[ln, 10].Font.Size = 11;
                    worKsheeT.Cells[ln, 10].Font.Bold = false;
                    // CORR DATE (11,5)
                    worKsheeT.Cells[ln, 11] = item.DateCorrected!=DateTime.MinValue?item.DateCorrected.ToShortDateString():string.Empty;
                    worKsheeT.Cells[ln, 11].Font.Size = 11;
                    worKsheeT.Cells[ln, 11].Font.Bold = false;
                    // CURR. CATEGORY (12,5)
                    worKsheeT.Cells[ln, 12] = item.Category != null ? item.Category.Description : string.Empty;
                    worKsheeT.Cells[ln, 12].Font.Size = 11;
                    worKsheeT.Cells[ln, 12].Font.Bold = false;
                    // CURR. SUB CAT. (13,5)
                    worKsheeT.Cells[ln, 13] = item.SubCategory != null ? item.SubCategory.Description : string.Empty;
                    worKsheeT.Cells[ln, 13].Font.Size = 11;
                    worKsheeT.Cells[ln, 13].Font.Bold = false;
                    // EXPENSE
                    worKsheeT.Cells[ln, 16] = item.Expense;
                    worKsheeT.Cells[ln, 16].NumberFormat = "#,##0.00";
                    worKsheeT.Cells[ln, 16].Font.Size = 11;
                    worKsheeT.Cells[ln, 16].Font.Bold = false;
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
                    "Generating correction worsheet is cancelled.", "Cancelled",
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
                    "Correction worsheet generation has been successfull.");
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
