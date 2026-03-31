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
    public partial class AdjustItemPrices : ViewController
    {
        private SimpleAction adjustItemPricesAction;
        private AdjustItemCostPrices2 _AdjustItemCostPrices2;
        private System.ComponentModel.BackgroundWorker _BgWorker1;
        private System.ComponentModel.BackgroundWorker _BgWorker2;
        private ProgressForm _FrmProgress;

        public AdjustItemPrices()
        {
            this.TargetObjectType = typeof(AdjustItemCostPrices2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "AdjustItemCostPrices2.AdjustItemPrices";
            this.adjustItemPricesAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.adjustItemPricesAction.Execute += new
            SimpleActionExecuteEventHandler(
            AdjustItemPricesAction_Execute);
        }
        private void AdjustItemPricesAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _AdjustItemCostPrices2 = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as AdjustItemCostPrices2;
            ObjectSpace.CommitChanges();
            switch (_AdjustItemCostPrices2.AdjustMode) {
                case AdjustItemCostPricesEnum.FromTemplate:
                    if (string.IsNullOrEmpty(_AdjustItemCostPrices2.
                    TemplateFilePath)) {
                        // Show message: File path not specified
                        throw new UserFriendlyException(
                        "File path not specified");
                    }
                    _FrmProgress = new ProgressForm("Processing items...", 1, 
                    "Items updated {0} of {1} ");
                    _FrmProgress.CancelClick += FrmProgressCancelClick1;
                    _BgWorker1 = new System.ComponentModel.BackgroundWorker { 
                        WorkerSupportsCancellation = true, WorkerReportsProgress 
                        = true
                    };
                    _BgWorker1.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
                    _BgWorker1.ProgressChanged += BgWorkerProgressChanged1;
                    _BgWorker1.DoWork += BgWorkerDoWork1;
                    _BgWorker1.RunWorkerAsync(_AdjustItemCostPrices2);
                    _FrmProgress.ShowDialog();
                    break;
                case AdjustItemCostPricesEnum.InputRate:
                    if (string.IsNullOrEmpty(_AdjustItemCostPrices2.ItemsFilter)
                    ) {
                        //Show message: Please specify a valid items filter
                        throw new UserFriendlyException(
                        "Please specify a valid items filter");
                    }
                    // Parse and retrieve items filtered
                    IList<Item> items = ObjectSpace.GetObjects<Item>(
                    CriteriaOperator.Parse(_AdjustItemCostPrices2.
                    CriterionString));
                    // If exist, continue, if not, notify
                    if (items == null || items.Count == 0) {throw new 
                        UserFriendlyException(
                        "There were no items retrieved based on the specified items filter"
                        );}
                    _FrmProgress = new ProgressForm("Processing items...", items.Count, 
                    "Items updated {0} of {1} ");
                    _FrmProgress.CancelClick += FrmProgressCancelClick2;
                    _BgWorker2 = new System.ComponentModel.BackgroundWorker { 
                        WorkerSupportsCancellation = true, WorkerReportsProgress 
                        = true
                    };
                    _BgWorker2.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
                    _BgWorker2.ProgressChanged += BgWorkerProgressChanged2;
                    _BgWorker2.DoWork += BgWorkerDoWork2;
                    _BgWorker2.RunWorkerAsync(items);
                    _FrmProgress.ShowDialog();
                    break;
                default:
                    break;
            }
        }
        private string _message;
        private int iCount=0;
        private void BgWorkerDoWork1(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            AdjustItemCostPrices2 _adjustItemCostPrices = (AdjustItemCostPrices2)e.Argument;
            AdjustItemCostPrices2 thisObj = session.GetObjectByKey<AdjustItemCostPrices2>(
            _adjustItemCostPrices.Oid);

            Microsoft.Office.Interop.Excel.Application app = new
                    Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.
            Open(thisObj.TemplateFilePath, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing, Type.
            Missing, Type.Missing, Type.Missing, Type.Missing);
            Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.
            Office.Interop.Excel.Worksheet)wb.Sheets["Sheet1"];
            try
            {
                Microsoft.Office.Interop.Excel.Range excelRange = sheet.
                UsedRange;
                iCount = excelRange.Rows.Count;
                foreach (Microsoft.Office.Interop.Excel.Range row in
                excelRange.Rows)
                {
                    int rowNumber = row.Row;
                    if (rowNumber != 1)
                    {
                        //int i = 0;
                        string[] A4D4 = GetRange("A" + rowNumber + ":E"
                        + rowNumber + "", sheet);
                        if (!string.IsNullOrEmpty(A4D4[0]))
                        {
                            Item _itm = session.FindObject<Item>(
                            BinaryOperator.Parse("[No] = '" + A4D4[0] +
                            "'"));
                            _itm.SalesPrice = Convert.ToDecimal(A4D4[4])
                            ;
                            _itm.Save();
                        }
                    }
                    System.Threading.Thread.Sleep(20);
                    _BgWorker1.ReportProgress(1, _message);
                    index++;
                }
            }
            catch (Exception ex)
            {
                if (wb != null) { wb.Close(); }
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                e.Result = index;
                CommitUpdatingSession(session);
                session.Dispose();
                if (wb != null) { wb.Close(); }
            }
        }
        private void BgWorkerDoWork2(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList<Item> _items = e.Argument as IList<Item>;

            try
            {
                if (_items.Count > 0)
                {
                    foreach (Item item in _items)
                    {
                        Item _item = null;
                        _item = session.FindObject<Item>(CriteriaOperator.Parse("[No] = '" + item.No + "'"));
                        _item.SalesPrice = _item.Cost + (_item.Cost * _AdjustItemCostPrices2.MarkupRate / 100);
                        _item.Save();
                        if (_BgWorker2.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                        _message = string.Format("Adjusting price {0} succesfull.",
                            index);
                        System.Threading.Thread.Sleep(20);
                        _BgWorker2.ReportProgress(1, _message);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (index == _items.Count)
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
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

        private void BgWorkerProgressChanged1(object sender,
        ProgressChangedEventArgs e)
        {
            if (_FrmProgress != null)
            {
                _FrmProgress.ChangeRecordCount(iCount);
                _FrmProgress.
                    DoProgress(e.ProgressPercentage);
            }
        }
        private void BgWorkerProgressChanged2(object sender,
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
                    "Adjustment of item prices is cancelled.", "Cancelled",
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
                    "Item prices are successfully adjusted.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick1(object sender, EventArgs e)
        {
            _BgWorker1.CancelAsync();
        }
        private void FrmProgressCancelClick2(object sender, EventArgs e)
        {
            _BgWorker2.CancelAsync();
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
