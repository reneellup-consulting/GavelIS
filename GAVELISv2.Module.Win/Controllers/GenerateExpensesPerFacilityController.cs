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
    public partial class GenerateExpensesPerFacilityController : ViewController
    {
        private SimpleAction GenerateExpensesPerFacilityAction;
        private ExpensePerFacility _ExpensePerFacility;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateExpensesPerFacilityController()
        {
            this.TargetObjectType = typeof(ExpensePerFacility);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateExpensesPerFacilityActionId";
            this.GenerateExpensesPerFacilityAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GenerateExpensesPerFacilityAction.Caption = "Generate";
            this.GenerateExpensesPerFacilityAction.Execute += new SimpleActionExecuteEventHandler(GenerateExpensesPerFacilityAction_Execute);
        }
        void GenerateExpensesPerFacilityAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ExpensePerFacility = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ExpensePerFacility;
            IList<Department> objects = ObjectSpace.GetObjects<Department>(BinaryOperator.Parse("[Facility.Code]=?", _ExpensePerFacility.Facility.Code));
            if (objects.Count == 0)
            {
                XtraMessageBox.Show("There are no departments under the specified facility.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            Session session = _ExpensePerFacility.Session;
            if (_ExpensePerFacility.ExpensePerFacilityDetails.Count > 0)
            {
                session.ExecuteNonQuery(string.Format("delete ExpensePerFacilityDetail where MainId = {0}", _ExpensePerFacility.Oid));
            }
            _FrmProgress = new ProgressForm("Generating...", objects.Count,
                        "Departments processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(objects);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            ExpensePerFacility tie = session.GetObjectByKey<ExpensePerFacility>(_ExpensePerFacility.Oid);
            IList<Department> trans = (IList<Department>)e.Argument;
            try
            {
                foreach (var item in trans)
                {
                    index++;
                    _message = string.Format("Processing department {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms
                    DevExpress.Xpo.Metadata.XPClassInfo classInfo = session.GetClassInfo(typeof(IncomeAndExpense02)); ;
                    CriteriaOperator criteria = CriteriaOperator.Parse("[GYear] = ? And [Department.Code] = ?", tie.Year, item.Code);
                    SortingCollection sorting = new SortingCollection();
                    ICollection objects = session.GetObjects(classInfo, criteria, sorting, 0, false, false);
                    Department oDept = session.GetObjectByKey<Department>(item.Oid);
                    ExpensePerFacilityDetail createObject = ReflectionHelper.CreateObject<ExpensePerFacilityDetail>(session);
                    createObject.MainId = tie;
                    createObject.Department = oDept ?? null;
                    createObject.Head = oDept.DepartmentHead ?? null;
                    createObject.InCharge = oDept.InCharge ?? null;
                    StringBuilder sbJan = new StringBuilder();
                    StringBuilder sbFeb = new StringBuilder();
                    StringBuilder sbMar = new StringBuilder();
                    StringBuilder sbApr = new StringBuilder();
                    StringBuilder sbMay = new StringBuilder();
                    StringBuilder sbJun = new StringBuilder();
                    StringBuilder sbJul = new StringBuilder();
                    StringBuilder sbAug = new StringBuilder();
                    StringBuilder sbSep = new StringBuilder();
                    StringBuilder sbOct = new StringBuilder();
                    StringBuilder sbNov = new StringBuilder();
                    StringBuilder sbDec = new StringBuilder();
                    foreach (IncomeAndExpense02 exp in objects)
                    {
                        IncomeAndExpense02 oExp = session.GetObjectByKey<IncomeAndExpense02>(exp.Oid);
                        switch (exp.GMonth)
                        {
                            case MonthsEnum.None:
                                break;
                            case MonthsEnum.January:
                                createObject.January += oExp.Expense;
                                sbJan.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.February:
                                createObject.February += oExp.Expense;
                                sbFeb.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.March:
                                createObject.March += oExp.Expense;
                                sbMar.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.April:
                                createObject.April += oExp.Expense;
                                sbApr.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.May:
                                createObject.May += oExp.Expense;
                                sbMay.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.June:
                                createObject.June += oExp.Expense;
                                sbJun.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.July:
                                createObject.July += oExp.Expense;
                                sbJul.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.August:
                                createObject.August += oExp.Expense;
                                sbAug.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.September:
                                createObject.September += oExp.Expense;
                                sbSep.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.October:
                                createObject.October += oExp.Expense;
                                sbOct.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.November:
                                createObject.November += oExp.Expense;
                                sbNov.AppendFormat("{0},", oExp.Oid);
                                break;
                            case MonthsEnum.December:
                                createObject.December += oExp.Expense;
                                sbDec.AppendFormat("{0},", oExp.Oid);
                                break;
                            default:
                                break;
                        }
                    }
                    createObject.JanKeys = sbJan.ToString();
                    createObject.FebKeys = sbFeb.ToString();
                    createObject.MarKeys = sbMar.ToString();
                    createObject.AprKeys = sbApr.ToString();
                    createObject.MayKeys = sbMay.ToString();
                    createObject.JunKeys = sbJun.ToString();
                    createObject.JulKeys = sbJul.ToString();
                    createObject.AugKeys = sbAug.ToString();
                    createObject.SepKeys = sbSep.ToString();
                    createObject.OctKeys = sbOct.ToString();
                    createObject.NovKeys = sbNov.ToString();
                    createObject.DecKeys = sbDec.ToString();
                    createObject.Save();
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
                if (index == trans.Count)
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
                    "Generate Expenses per Facility is cancelled.", "Cancelled",
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
                    "Expenses per Facility generation has been successfull.");
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
