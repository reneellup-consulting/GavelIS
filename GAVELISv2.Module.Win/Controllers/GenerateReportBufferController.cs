using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public class ResultLine
    {
        public ResultLine() { }
        //public ExpenseType Category { get; set; }
        public int MonthIndex { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
    }
    public partial class GenerateReportBufferController : ViewController
    {
        private SimpleAction generateReportBufferAction;
        private IncomeExpenseReporter _IncomeExpenseReporter;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateReportBufferController()
        {
            this.TargetObjectType = typeof(IncomeExpenseReporter);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateReportBufferActionId";
            this.generateReportBufferAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateReportBufferAction.Caption = "Generate";
            this.generateReportBufferAction.Execute += new SimpleActionExecuteEventHandler(generateReportBufferAction_Execute);
        }

        void generateReportBufferAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _IncomeExpenseReporter = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as IncomeExpenseReporter;
            ObjectSpace.CommitChanges();
            //CriteriaOperator criteria = CriteriaOperator.Parse("[NoBuffer]!=?", true);
            XPCollection<ExpenseType> exp = new XPCollection<ExpenseType>(((ObjectSpace)ObjectSpace).Session);
            IOrderedEnumerable<ExpenseType> orderBy = exp.Where(o => o.NoBuffer != true).OrderBy(o => o.Code);
            if (orderBy.Count() == 0)
            {
                XtraMessageBox.Show("There are no expense types encoded in the system.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Generating...", orderBy.Count(),
                        "Expense Types processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(orderBy);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IOrderedEnumerable<ExpenseType> trans = (IOrderedEnumerable<ExpenseType>)e.Argument;
            //IOrderedEnumerable<ExpenseType> orderBy = trans.Where(o => o.NoBuffer != true).OrderBy(o => o.Code);
            IncomeExpenseReporter thisInExRep = session.GetObjectByKey<IncomeExpenseReporter>(_IncomeExpenseReporter.Oid);
            //DevExpress.Xpo.Metadata.XPClassInfo classInfo = session.GetClassInfo<IncomeAndExpense02>();
            //SortingCollection sorting = new SortingCollection();
            //sorting.Add(new SortProperty("GMonthSorter", DevExpress.Xpo.DB.SortingDirection.Ascending));
            //sorting.Add(new SortProperty("EntryDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
            //ICollection filtered = session.GetObjects(classInfo, criteria, sorting, 0, false, true);
            //IEnumerable<IncomeAndExpense02> newVariable = filtered.Where(o => o.GYear == thisInExRep.Year);
            try
            {
                Company companyInfo = Company.GetInstance(session);
                string cmd = string.Format("update CashflowSummary set Income=0,Expense=0 where [Year]={0}", thisInExRep.Year);
                session.ExecuteNonQuery(cmd);

                string cmd2 = string.Format("delete IncomeExpenseBuffer where [Year]={0}", thisInExRep.Year);
                session.ExecuteNonQuery(cmd2);

                //CriteriaOperator criteria = CriteriaOperator.Parse("[GYear]=?", thisInExRep.Year);
                //XPCollection<IncomeAndExpense02> filtered = new XPCollection<IncomeAndExpense02>(((ObjectSpace)ObjectSpace).Session, criteria, new SortProperty("GMonthSorter", DevExpress.Xpo.DB.SortingDirection.Ascending), new SortProperty("EntryDate", DevExpress.Xpo.DB.SortingDirection.Ascending));

                // select [Month], sum(Income) as Inc_Total from vIncomeExpense02 where [Year]=2021 group by [Month]
                Dictionary<int, decimal> incomeDict = new Dictionary<int, decimal>();
                string cmd3 = string.Format("select [Month], sum(Income) as Inc_Total from vIncomeExpense02 where [Year]={0} group by [Month]", thisInExRep.Year);
                SelectedData data3 = session.ExecuteQuery(cmd3);

                foreach (var row3 in data3.ResultSet[0].Rows)
                {
                    incomeDict.Add(Convert.ToInt32(row3.Values[0].ToString()), Convert.ToDecimal(row3.Values[1].ToString()));
                }

                //thisInExRep.TotalIncome = incomeDict.Sum(o => o.Value);

                foreach (ExpenseType item in trans)
                {
                    index++;
                    _message = string.Format("Generating buffer {0} succesfull.",
                    item.Description);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms in here..
                    string tmpBufferId = string.Format("{0}-{1}",thisInExRep.Year,item.Code);
                    ExpenseType exp = session.GetObjectByKey<ExpenseType>(item.Oid);

                    IncomeExpenseBuffer inc;
                    inc = session.FindObject<IncomeExpenseBuffer>(CriteriaOperator.Parse("[BufferId]=?", tmpBufferId));
                    if (inc==null)
                    {
                        inc = ReflectionHelper.CreateObject<IncomeExpenseBuffer>(session);
                        inc.BufferId = tmpBufferId;
                    }
                    inc.Seq = index;
                    inc.ReporterId = thisInExRep;
                    inc.Year = thisInExRep.Year;
                    inc.Category = exp;
                    inc.Expense = !exp.Income;
                    //var income = from i in filtered
                    //             group i by i.EntryDate.Month into res
                    //             select new
                    //             {
                    //                 key = res.Key,
                    //                 total = res.Sum(o => o.Income)
                    //             };
                    //var result = from p in filtered where p.Category == item
                    //             group p by p.EntryDate.Month into res
                    //             select new
                    //             {
                    //                 key = res.Key,
                    //                 income = res.Sum(o => o.Income),
                    //                 expense = res.Sum(o => o.Expense)
                    //             };
                    // select [Month], sum(Income) as Inc_Total from vIncomeExpense02 where [Year]=2021 and Category='63DF24BA-D1A2-47B9-A93A-F2BF56A30E4C' group by [Month], Category

                    //Dictionary<int, decimal> ixDict = new Dictionary<int, decimal>();
                    TempIncExpCollection tmpIncExps = new TempIncExpCollection();
                    string cmd4 = string.Format("select [Month], sum(Income) as Inc_Total, sum(Expense) as Exp_Total from vIncomeExpense02 where [Year]={0} and Category='{1}' group by [Month], Category order by [Month]", thisInExRep.Year, item.Oid);
                    SelectedData data4 = session.ExecuteQuery(cmd4);

                    foreach (var row4 in data4.ResultSet[0].Rows)
                    {
                        TempIncExp tmpIncExp = new TempIncExp();
                        tmpIncExp.Key = Convert.ToInt32(row4.Values[0].ToString());
                        tmpIncExp.Income = Convert.ToDecimal(row4.Values[1].ToString());
                        tmpIncExp.Expense = Convert.ToDecimal(row4.Values[2].ToString());
                        tmpIncExps.Add(tmpIncExp);
                    }

                    // if item == companyinfo.TireExpenseType
                    if (item.Code == companyInfo.TireExpenseType.Code)
                    {
                        string cmd5 = string.Format("select HeaderId, [Year], [Month], sum(Cost) as Expense from vTireItemsAccomCostDetail where TireItemClass=1 and [Year]={0} group by HeaderId, [Year], [Month]", thisInExRep.Year);
                        SelectedData data5 = session.ExecuteQuery(cmd5);

                        foreach (var row5 in data5.ResultSet[0].Rows)
                        {
                            TempIncExp tmpIncExp = new TempIncExp();
                            tmpIncExp.Key = Convert.ToInt32(row5.Values[2].ToString());
                            tmpIncExp.Income = 0m;
                            tmpIncExp.Expense = Convert.ToDecimal(row5.Values[3].ToString());
                            tmpIncExps.Add(tmpIncExp);
                        }
                    }
                    //=======

                    //// if item == companyInfo.FlapsExpenseType
                    //if (item.Code == companyInfo.FlapsExpenseType.Code)
                    //{
                    //    string cmd5 = string.Format("select HeaderId, [Year], [Month], sum(Cost * Qty) as Expense from vTireItemsAccomCostDetail where TireItemClass=2 and [Year]={0} group by HeaderId, [Year], [Month]", thisInExRep.Year);
                    //    SelectedData data5 = session.ExecuteQuery(cmd5);

                    //    foreach (var row5 in data5.ResultSet[0].Rows)
                    //    {
                    //        TempIncExp tmpIncExp = new TempIncExp();
                    //        tmpIncExp.Key = Convert.ToInt32(row5.Values[2].ToString());
                    //        tmpIncExp.Income = 0m;
                    //        tmpIncExp.Expense = Convert.ToDecimal(row5.Values[3].ToString());
                    //        tmpIncExps.Add(tmpIncExp);
                    //    }
                    //}
                    ////=======

                    //// if item == companyInfo.TubesExpenseType
                    //if (item.Code == companyInfo.TubesExpenseType.Code)
                    //{
                    //    string cmd5 = string.Format("select HeaderId, [Year], [Month], sum(Cost * Qty) as Expense from vTireItemsAccomCostDetail where TireItemClass=3 and [Year]={0} group by HeaderId, [Year], [Month]", thisInExRep.Year);
                    //    SelectedData data5 = session.ExecuteQuery(cmd5);

                    //    foreach (var row5 in data5.ResultSet[0].Rows)
                    //    {
                    //        TempIncExp tmpIncExp = new TempIncExp();
                    //        tmpIncExp.Key = Convert.ToInt32(row5.Values[2].ToString());
                    //        tmpIncExp.Income = 0m;
                    //        tmpIncExp.Expense = Convert.ToDecimal(row5.Values[3].ToString());
                    //        tmpIncExps.Add(tmpIncExp);

                    //    }
                    //}
                    ////=======

                    // if item == companyInfo.RimExpenseType
                    if (item.Code == companyInfo.RimExpenseType.Code)
                    {
                        string cmd5 = string.Format("select HeaderId, [Year], [Month], sum(Cost * Qty) as Expense from vTireItemsAccomCostDetail where TireItemClass=4 and [Year]={0} group by HeaderId, [Year], [Month]", thisInExRep.Year);
                        SelectedData data5 = session.ExecuteQuery(cmd5);

                        foreach (var row5 in data5.ResultSet[0].Rows)
                        {
                            TempIncExp tmpIncExp = new TempIncExp();
                            tmpIncExp.Key = Convert.ToInt32(row5.Values[2].ToString());
                            tmpIncExp.Income = 0m;
                            tmpIncExp.Expense = Convert.ToDecimal(row5.Values[3].ToString());
                            tmpIncExps.Add(tmpIncExp);
                        }
                    }
                    //=======

                    foreach (var rs in tmpIncExps)
                    {
                        //if (inc.Category.Code == "0360")
                        //{

                        //}
                        if (rs.Key == 1)
                        {
                            inc.January += rs.Income + rs.Expense;
                            var jan = incomeDict.First(o => o.Key == 1);
                            inc.JanuaryPrcnt = jan.Value != 0 ? inc.January / jan.Value * 100 : 0m;
                            //if (inc.JanuaryPrcnt > 100m)
                            //{
                            //    inc.JanuaryPrcnt = 0m;
                            //}
                            if (inc.January!=0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.January, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 2)
                        {
                            inc.February += rs.Income + rs.Expense;
                            var feb = incomeDict.First(o => o.Key == 2);
                            inc.FebruaryPrcnt = feb.Value != 0 ? inc.February / feb.Value * 100 : 0m;
                            ////if (inc.FebruaryPrcnt > 100m)
                            ////{
                            ////    inc.FebruaryPrcnt = 0m;
                            ////}
                            if (inc.February != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.February, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 3)
                        {
                            inc.March += rs.Income + rs.Expense;
                            var mar = incomeDict.First(o => o.Key == 3);
                            inc.MarchPrcnt = mar.Value != 0 ? inc.March / mar.Value * 100 : 0m;
                            //if (inc.MarchPrcnt > 100m)
                            //{
                            //    inc.MarchPrcnt = 0m;
                            //}
                            if (inc.March != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.March, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 4)
                        {
                            inc.April += rs.Income + rs.Expense;
                            var apr = incomeDict.First(o => o.Key == 4);
                            inc.AprilPrcnt = apr.Value != 0 ? inc.April / apr.Value * 100 : 0m;
                            //if (inc.AprilPrcnt > 100m)
                            //{
                            //    inc.AprilPrcnt = 0m;
                            //}
                            if (inc.April != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.April, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 5)
                        {
                            inc.May += rs.Income + rs.Expense;
                            var may = incomeDict.First(o => o.Key == 5);
                            inc.MayPrcnt = may.Value != 0 ? inc.May / may.Value * 100 : 0m;
                            //if (inc.MayPrcnt > 100m)
                            //{
                            //    inc.MayPrcnt = 0m;
                            //}
                            if (inc.May != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.May, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 6)
                        {
                            inc.June += rs.Income + rs.Expense;
                            var jun = incomeDict.First(o => o.Key == 6);
                            inc.JunePrcnt = jun.Value != 0 ? inc.June / jun.Value * 100 : 0m;
                            //if (inc.JunePrcnt > 100m)
                            //{
                            //    inc.JunePrcnt = 0m;
                            //}
                            if (inc.June != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.June, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 7)
                        {
                            inc.July += rs.Income + rs.Expense;
                            var jul = incomeDict.First(o => o.Key == 7);
                            inc.JulyPrcnt = jul.Value != 0 ? inc.July / jul.Value * 100 : 0m;
                            //if (inc.JulyPrcnt > 100m)
                            //{
                            //    inc.JulyPrcnt = 0m;
                            //}
                            if (inc.July != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.July, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 8)
                        {
                            inc.August += rs.Income + rs.Expense;
                            var aug = incomeDict.First(o => o.Key == 8);
                            inc.AugustPrcnt = aug.Value != 0 ? inc.August / aug.Value * 100 : 0m;
                            //if (inc.AugustPrcnt > 100m)
                            //{
                            //    inc.AugustPrcnt = 0m;
                            //}
                            if (inc.August != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.August, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 9)
                        {
                            inc.September += rs.Income + rs.Expense;
                            var sep = incomeDict.First(o => o.Key == 9);
                            inc.SeptemberPrcnt = sep.Value != 0 ? inc.September / sep.Value * 100 : 0m;
                            //if (inc.SeptemberPrcnt > 100m)
                            //{
                            //    inc.SeptemberPrcnt = 0m;
                            //}
                            if (inc.September != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.September, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 10)
                        {
                            inc.October += rs.Income + rs.Expense;
                            var oct = incomeDict.First(o => o.Key == 10);
                            inc.OctobePrcntr = oct.Value != 0 ? inc.October / oct.Value * 100 : 0m;
                            //if (inc.OctobePrcntr > 100m)
                            //{
                            //    inc.OctobePrcntr = 0m;
                            //}
                            if (inc.October != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.October, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 11)
                        {
                            inc.November += rs.Income + rs.Expense;
                            var nov = incomeDict.First(o => o.Key == 11);
                            inc.NovemberPrcnt = nov.Value != 0 ? inc.November / nov.Value * 100 : 0m;
                            //if (inc.NovemberPrcnt > 100m)
                            //{
                            //    inc.NovemberPrcnt = 0m;
                            //}
                            if (inc.November != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.November, inc, rs.Income, rs.Expense);
                            }
                        }
                        if (rs.Key == 12)
                        {
                            inc.December += rs.Income + rs.Expense;
                            var dec = incomeDict.First(o => o.Key == 12);
                            inc.DecemberPrcnt = dec.Value != 0 ? inc.December / dec.Value * 100 : 0m;
                            //if (inc.DecemberPrcnt > 100m)
                            //{
                            //    inc.DecemberPrcnt = 0m;
                            //}
                            if (inc.December != 0)
                            {
                                CreateCashflowSummary(session, thisInExRep, MonthsEnum.December, inc, rs.Income, rs.Expense);
                            }
                        }
                    }
                    decimal totalIncome = incomeDict.Sum(o => o.Value);
                    inc.TotalPercent = totalIncome != 0 ? (inc.Total / totalIncome) * 100 : 0m;
                    inc.Save();
                    session.CommitChanges();
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
                if (index == trans.Count())
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private static void CreateCashflowSummary(UnitOfWork session, IncomeExpenseReporter thisInExRep,MonthsEnum month, IncomeExpenseBuffer inc, decimal income, decimal expense)
        {
            string tmpId = string.Format("{0}-{1}", thisInExRep.Year, month);
            CashflowSummary cfs;
            cfs = session.FindObject<CashflowSummary>(CriteriaOperator.Parse("[BufferId]=?", tmpId));
            if (cfs == null)
            {
                cfs = ReflectionHelper.CreateObject<CashflowSummary>(session);
                cfs.ReporterId = thisInExRep;
                cfs.Year = thisInExRep.Year;
                cfs.BufferId = tmpId;
            }
            switch (month)
            {
                case MonthsEnum.None:
                    break;
                case MonthsEnum.January:
                    cfs.Month = MonthsEnum.January;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.February:
                    cfs.Month = MonthsEnum.February;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.March:
                    cfs.Month = MonthsEnum.March;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.April:
                    cfs.Month = MonthsEnum.April;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.May:
                    cfs.Month = MonthsEnum.May;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.June:
                    cfs.Month = MonthsEnum.June;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.July:
                    cfs.Month = MonthsEnum.July;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.August:
                    cfs.Month = MonthsEnum.August;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.September:
                    cfs.Month = MonthsEnum.September;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.October:
                    cfs.Month = MonthsEnum.October;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.November:
                    cfs.Month = MonthsEnum.November;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                case MonthsEnum.December:
                    cfs.Month = MonthsEnum.December;
                    if (inc.Expense)
                        cfs.Expense += expense;
                    else
                        cfs.Income += income;
                    break;
                default:
                    break;
            }
            cfs.Save();
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
                    "Generation is cancelled.", "Cancelled",
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
                    "Generation has been successfull.");
                    //ObjectSpace.ReloadObject(_IncomeExpenseReporter);
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
