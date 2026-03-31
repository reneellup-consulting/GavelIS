using System;
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
    public partial class GenerateTruckIncomeExpenseController : ViewController
    {
        private SimpleAction GenerateTruckIncomeExpenseAction;
        private TruckIncomeExpense _TruckIncomeExpense;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateTruckIncomeExpenseController()
        {
            this.TargetObjectType = typeof(TruckIncomeExpense);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateTruckIncomeExpenseActionId";
            this.GenerateTruckIncomeExpenseAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GenerateTruckIncomeExpenseAction.Caption = "Generate";
            this.GenerateTruckIncomeExpenseAction.Execute += new SimpleActionExecuteEventHandler(GenerateTruckIncomeExpenseAction_Execute);
        }
        void GenerateTruckIncomeExpenseAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _TruckIncomeExpense = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TruckIncomeExpense;
            XPCollection<TruckIncomeExpenseTruck> units = _TruckIncomeExpense.TruckIncomeExpenseTrucks;
            if (units.Count() == 0)
            {
                XtraMessageBox.Show("There are no selected units to process.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            Session session = _TruckIncomeExpense.Session;
            if (_TruckIncomeExpense.TruckIncomeExpenseDetails.Count > 0)
            {
                session.ExecuteNonQuery(string.Format("delete TruckIncomeExpenseDetail where MainId = {0}", _TruckIncomeExpense.Oid));
            }
            _FrmProgress = new ProgressForm("Generating...", units.Count(),
                        "Units processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(units);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            TruckIncomeExpense tie = session.GetObjectByKey<TruckIncomeExpense>(_TruckIncomeExpense.Oid);
            //IOrderedEnumerable<TruckIncomeExpenseTruck> trans = (IOrderedEnumerable<TruckIncomeExpenseTruck>)e.Argument;
            XPCollection<TruckIncomeExpenseTruck> trans = (XPCollection<TruckIncomeExpenseTruck>)e.Argument;
            try
            {
                foreach (var item in trans)
                {
                    index++;
                    _message = string.Format("Processing unit {0} succesfull.",
                    item.Unit.No);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms
                    FixedAsset fa = session.GetObjectByKey<FixedAsset>(item.Unit.Oid);
                    for (int i = 1; i < 13; i++)
                    {
                        TruckIncomeExpenseDetail tied = ReflectionHelper.CreateObject<TruckIncomeExpenseDetail>(session);
                        tied.MainId = tie;
                        tied.Seq = item.Seq;
                        // UNIT.
                        tied.Unit = fa;
                        // YEAR
                        tied.Year = tie.Year;
                        // MONTH
                        tied.Month = (MonthsEnum)i;
                        // Income
                        DevExpress.Xpo.DB.SelectedData executeSproc = session.ExecuteSproc("GetUnitIncome", tied.Year, i, fa.Oid);
                        SelectStatementResultRow[] rows = executeSproc.ResultSet[0].Rows;
                        if (rows != null && rows.Count() > 0)
                        {
                            tied.Income = Convert.ToDecimal(rows[0].Values[0].ToString());
                        }
                        // Expense 01
                        DevExpress.Xpo.DB.SelectedData executeSproc1 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense01.Oid);
                        SelectStatementResultRow[] rows1 = executeSproc1.ResultSet[0].Rows;
                        if (rows1 != null && rows1.Count() > 0)
                        {
                            tied.Expense01 = Convert.ToDecimal(rows1[0].Values[0].ToString());
                        }
                        // Expense 02
                        DevExpress.Xpo.DB.SelectedData executeSproc2 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense02.Oid);
                        SelectStatementResultRow[] rows2 = executeSproc2.ResultSet[0].Rows;
                        if (rows2 != null && rows2.Count() > 0)
                        {
                            tied.Expense02 = Convert.ToDecimal(rows2[0].Values[0].ToString());
                        }
                        // Expense 03
                        DevExpress.Xpo.DB.SelectedData executeSproc3 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense03.Oid);
                        SelectStatementResultRow[] rows3 = executeSproc3.ResultSet[0].Rows;
                        if (rows3 != null && rows3.Count() > 0)
                        {
                            tied.Expense03 = Convert.ToDecimal(rows3[0].Values[0].ToString());
                        }
                        // Expense 04
                        DevExpress.Xpo.DB.SelectedData executeSproc4 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense04.Oid);
                        SelectStatementResultRow[] rows4 = executeSproc4.ResultSet[0].Rows;
                        if (rows4 != null && rows4.Count() > 0)
                        {
                            tied.Expense04 = Convert.ToDecimal(rows4[0].Values[0].ToString());
                        }
                        // Expense 05
                        DevExpress.Xpo.DB.SelectedData executeSproc5 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense05.Oid);
                        SelectStatementResultRow[] rows5 = executeSproc5.ResultSet[0].Rows;
                        if (rows5 != null && rows5.Count() > 0)
                        {
                            tied.Expense05 = Convert.ToDecimal(rows5[0].Values[0].ToString());
                        }
                        // Expense 06
                        DevExpress.Xpo.DB.SelectedData executeSproc6 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense06.Oid);
                        SelectStatementResultRow[] rows6 = executeSproc6.ResultSet[0].Rows;
                        if (rows6 != null && rows6.Count() > 0)
                        {
                            tied.Expense06 = Convert.ToDecimal(rows6[0].Values[0].ToString());
                        }
                        // Expense 07
                        DevExpress.Xpo.DB.SelectedData executeSproc7 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense07.Oid);
                        SelectStatementResultRow[] rows7 = executeSproc7.ResultSet[0].Rows;
                        if (rows7 != null && rows7.Count() > 0)
                        {
                            tied.Expense07 = Convert.ToDecimal(rows7[0].Values[0].ToString());
                        }
                        // Expense 08
                        DevExpress.Xpo.DB.SelectedData executeSproc8 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense08.Oid);
                        SelectStatementResultRow[] rows8 = executeSproc8.ResultSet[0].Rows;
                        if (rows8 != null && rows8.Count() > 0)
                        {
                            tied.Expense08 = Convert.ToDecimal(rows8[0].Values[0].ToString());
                        }
                        // Expense 09
                        DevExpress.Xpo.DB.SelectedData executeSproc9 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense09.Oid);
                        SelectStatementResultRow[] rows9 = executeSproc9.ResultSet[0].Rows;
                        if (rows9 != null && rows9.Count() > 0)
                        {
                            tied.Expense09 = Convert.ToDecimal(rows9[0].Values[0].ToString());
                        }
                        // Expense 10
                        DevExpress.Xpo.DB.SelectedData executeSproc10 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense10.Oid);
                        SelectStatementResultRow[] rows10 = executeSproc10.ResultSet[0].Rows;
                        if (rows10 != null && rows10.Count() > 0)
                        {
                            tied.Expense10 = Convert.ToDecimal(rows10[0].Values[0].ToString());
                        }
                        // Expense 11
                        DevExpress.Xpo.DB.SelectedData executeSproc11 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense11.Oid);
                        SelectStatementResultRow[] rows11 = executeSproc11.ResultSet[0].Rows;
                        if (rows11 != null && rows11.Count() > 0)
                        {
                            tied.Expense11 = Convert.ToDecimal(rows11[0].Values[0].ToString());
                        }
                        // Expense 12
                        DevExpress.Xpo.DB.SelectedData executeSproc12 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense12.Oid);
                        SelectStatementResultRow[] rows12 = executeSproc12.ResultSet[0].Rows;
                        if (rows12 != null && rows12.Count() > 0)
                        {
                            tied.Expense12 = Convert.ToDecimal(rows12[0].Values[0].ToString());
                        }
                        // Expense 13
                        DevExpress.Xpo.DB.SelectedData executeSproc13 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense13.Oid);
                        SelectStatementResultRow[] rows13 = executeSproc13.ResultSet[0].Rows;
                        if (rows13 != null && rows13.Count() > 0)
                        {
                            tied.Expense13 = Convert.ToDecimal(rows13[0].Values[0].ToString());
                        }
                        // Expense 14
                        DevExpress.Xpo.DB.SelectedData executeSproc14 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense14.Oid);
                        SelectStatementResultRow[] rows14 = executeSproc14.ResultSet[0].Rows;
                        if (rows14 != null && rows14.Count() > 0)
                        {
                            tied.Expense14 = Convert.ToDecimal(rows14[0].Values[0].ToString());
                        }
                        // Expense 15
                        DevExpress.Xpo.DB.SelectedData executeSproc15 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense15.Oid);
                        SelectStatementResultRow[] rows15 = executeSproc15.ResultSet[0].Rows;
                        if (rows15 != null && rows15.Count() > 0)
                        {
                            tied.Expense15 = Convert.ToDecimal(rows15[0].Values[0].ToString());
                        }
                        // Expense 16
                        DevExpress.Xpo.DB.SelectedData executeSproc16 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense16.Oid);
                        SelectStatementResultRow[] rows16 = executeSproc16.ResultSet[0].Rows;
                        if (rows16 != null && rows16.Count() > 0)
                        {
                            tied.Expense16 = Convert.ToDecimal(rows16[0].Values[0].ToString());
                        }
                        // Expense 17
                        DevExpress.Xpo.DB.SelectedData executeSproc17 = session.ExecuteSproc("GetUnitExpense", tied.Year, i, fa.Oid, tie.Expense17.Oid);
                        SelectStatementResultRow[] rows17 = executeSproc17.ResultSet[0].Rows;
                        if (rows17 != null && rows17.Count() > 0)
                        {
                            tied.Expense17 = Convert.ToDecimal(rows17[0].Values[0].ToString());
                        }
                        // TOTAL EXPENSES
                        DevExpress.Xpo.DB.SelectedData executeSprocte = session.ExecuteSproc("GetUnitExpenseTotal", tied.Year, i, fa.Oid);
                        SelectStatementResultRow[] rowste = executeSprocte.ResultSet[0].Rows;
                        if (rowste != null && rowste.Count() > 0)
                        {
                            tied.TotalExpenses = Convert.ToDecimal(rowste[0].Values[0].ToString());
                        }
                        // OTHER EXPENSES
                        decimal seventeentot = tied.Expense01 +
                            tied.Expense02 +
                            tied.Expense03 +
                            tied.Expense04 +
                            tied.Expense05 +
                            tied.Expense06 +
                            tied.Expense07 +
                            tied.Expense08 +
                            tied.Expense09 +
                            tied.Expense10 +
                            tied.Expense11 +
                            tied.Expense12 +
                            tied.Expense13 +
                            tied.Expense14 +
                            tied.Expense15 +
                            tied.Expense16 +
                            tied.Expense17;
                        tied.OtherExpenses = tied.TotalExpenses - seventeentot;
                        // NET INCOME/LOSS
                        tied.NetIncomeLoss = tied.Income - tied.TotalExpenses;
                        tied.Save();
                    }
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
                if (index == Convert.ToInt32(trans.Count().ToString()))
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
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
                    "Generate Income vs. Expense is cancelled.", "Cancelled",
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
                    "Income vs. Expense generation has been successfull.");
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
