using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
using Excel = Microsoft.Office.Interop.Excel;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class AdjustPrice : ViewController
    {
        private Excel.Application _XlApp;
        private Excel.Range _XlRange;
        private string _DesFile;
        private AdjustItemCostPrices2 adjust;
        private SimpleAction adjustPriceAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private System.ComponentModel.BackgroundWorker _BgWorker2;
        private ProgressForm _FrmProgress;

        public AdjustPrice()
        {
            this.TargetObjectType = typeof(AdjustItemCostPrices2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.AdjustPrice", this.GetType().Name);
            this.adjustPriceAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.adjustPriceAction.Caption = "Adjust Price";
            this.adjustPriceAction.Execute += new
            SimpleActionExecuteEventHandler(AdjustPriceAction_Execute);
            this.adjustPriceAction.Executed += new EventHandler<
            ActionBaseEventArgs>(AdjustPriceAction_Executed);
            this.adjustPriceAction.ConfirmationMessage =
            "Do you really want to adjust the prices of these items?";
            UpdateActionState(false);
        }

        private void AdjustPriceAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            adjust = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as AdjustItemCostPrices2;
            // If Template
            switch (adjust.AdjustMode)
            {
                case AdjustItemCostPricesEnum.FromTemplate:
                    if (!string.IsNullOrEmpty(adjust.TemplateFilePath))
                    {
                        // Check if file exists
                        string curFile = adjust.TemplateFilePath;
                        _DesFile = Path.GetDirectoryName(curFile) + "\\" + Path.
                        GetFileNameWithoutExtension(curFile) + "_001" + Path.GetExtension(curFile);
                        if (!File.Exists(curFile))
                        {
                            XtraMessageBox.Show("The file does not exist.",
                                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                                Windows.Forms.MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {
                            // Copy temporary file into then orig file directory
                            _XlApp = new Excel.Application();
                            int rowCount = 0;
                            int colCount = 0;
                            _XlRange = null;
                            try
                            {
                                File.Copy(curFile, _DesFile, true);
                                Excel.Workbook xlWorkbook = _XlApp.Workbooks.Open(_DesFile);
                                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                                _XlRange = xlWorksheet.UsedRange;

                                rowCount = _XlRange.Rows.Count;
                                colCount = _XlRange.Columns.Count;

                            }
                            catch (Exception)
                            {
                                if (_XlApp != null)
                                {
                                    _XlApp.Workbooks.Close();
                                }
                                File.Delete(_DesFile);
                                throw;
                            }
                            finally
                            {
                                if ((rowCount - 1) == 0)
                                {
                                    if (_XlApp != null)
                                    {
                                        _XlApp.Workbooks.Close();
                                    }
                                    File.Delete(_DesFile);

                                    throw new ApplicationException("There are no rows of adjustment to read");
                                }

                                var count = rowCount - 1;
                                _FrmProgress = new ProgressForm("Rows to adjust...", count,
                                "Adjusting {0} of {1} ");
                                _FrmProgress.CancelClick += FrmProgressCancelClick;
                                _BgWorker = new System.ComponentModel.BackgroundWorker
                                {
                                    WorkerSupportsCancellation = true,
                                    WorkerReportsProgress = true
                                };
                                _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
                                _BgWorker.ProgressChanged += BgWorkerProgressChanged;
                                _BgWorker.DoWork += BgWorkerDoWork;
                                _BgWorker.RunWorkerAsync(_XlRange);
                                _FrmProgress.ShowDialog();

                            }

                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Please provide a valid template file.",
                                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                                Windows.Forms.MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                case AdjustItemCostPricesEnum.InputRate:
                    if (!string.IsNullOrEmpty(adjust.CriterionString))
                    {
                        // check if with criteria items are found
                        IList<Item> items = ObjectSpace.GetObjects<Item>(CriteriaOperator.Parse(adjust.CriterionString));
                        if (items!=null && items.Count>0)
                        {
                            _FrmProgress = new ProgressForm("Items to adjust...", items.Count,
                                "Adjusting {0} of {1} ");
                            _FrmProgress.CancelClick += FrmProgressCancelClick;
                            _BgWorker2 = new System.ComponentModel.BackgroundWorker
                            {
                                WorkerSupportsCancellation = true,
                                WorkerReportsProgress = true
                            };
                            _BgWorker2.RunWorkerCompleted += BgWorker2RunWorkerCompleted;
                            _BgWorker2.ProgressChanged += BgWorkerProgressChanged;
                            _BgWorker2.DoWork += BgWorkerDoWork2;
                            _BgWorker2.RunWorkerAsync(items);
                            _FrmProgress.ShowDialog();
                        }
                        else
                        {
                            XtraMessageBox.Show("There are no items to be adjusted based on the filter specified.",
                                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                                Windows.Forms.MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    else
                    {
                        XtraMessageBox.Show("Please provide a valid items filter.",
                                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                                Windows.Forms.MessageBoxIcon.Exclamation);
                        return;
                    }
                    break;
                default:
                    break;
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
        private string _message;
        private void BgWorkerDoWork2(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList<Item> _items = e.Argument as IList<Item>;
            try
            {
                if (_items.Count>0)
                {
                    foreach (Item item in _items)
                    {
                        Item _item = null;
                        _item = session.FindObject<Item>(CriteriaOperator.Parse("[No] = '" + item.No + "'"));
                        _item.SalesPrice = _item.Cost + (_item.Cost * adjust.MarkupRate/100);
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
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            Excel.Range _xRange = (Excel.Range)e.Argument;
            int irow = _xRange.Rows.Count;
            int icol = _xRange.Columns.Count;
            string sTst = string.Empty;
            string itm = string.Empty;
            decimal prc = 0m;
            try
            {
                if (irow > 0)
                {
                    for (int i = 2; i < irow + 1; i++)
                    {
                        Item _item = null;
                        for (int j = 1; j < icol; j++)
                        {
                            // Item No
                            if (j == 1)
                                itm = _xRange.Cells[i, j].Value2.ToString();
                            // Price
                            if (j == 6)
                                prc = (decimal)_xRange.Cells[i, j].Value2;
                        }
                        _item = session.FindObject<Item>(CriteriaOperator.Parse("[No] = '" + itm + "'"));
                        _item.SalesPrice = prc;
                        _item.Save();

                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                        _message = string.Format("Adjusting price {0} succesfull.",
                            irow - 1);
                        System.Threading.Thread.Sleep(20);
                        _BgWorker.ReportProgress(1, _message);
                        index++;
                    }
                }
            }
            finally
            {
                if (index == (irow - 1))
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void AdjustPriceAction_Executed(object sender, ActionBaseEventArgs e)
        {
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
        private void BgWorker2RunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Adjusting price operation has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully adjusted");
                    ObjectSpace.ReloadObject(adjust);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            if (_XlApp != null)
            {
                _XlApp.Workbooks.Close();
            }
            File.Delete(_DesFile);
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Adjusting price operation has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully adjusted");
                    ObjectSpace.ReloadObject(adjust);
                    ObjectSpace.Refresh();
                }
            }

        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        private void UpdateActionState(bool inProgress)
        {
            adjustPriceAction.
                Enabled.SetItemValue("Adjusting entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
