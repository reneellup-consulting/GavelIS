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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateDriverDeductions : ViewController
    {
        private SimpleAction generateDriverDeductionsAction;
        private DriverPayrollBatch _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public GenerateDriverDeductions()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.GenerateDriverDeductions";
            this.generateDriverDeductionsAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateDriverDeductionsAction.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateDriverDeductionsActionAction_Execute);
        }
        private void GenerateDriverDeductionsActionAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch;

            try
            {
                for (int i = _DriverPayrollBatch.PayrollDeductions.Count - 1;
                i >= 0; i--)
                {
                    _DriverPayrollBatch.PayrollDeductions[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();


            var distinctDrivers = _DriverPayrollBatch.DriverPayrollTrips.Select(o => o.Driver).Distinct();

            if (distinctDrivers.Count() == 0)
            {
                throw new UserFriendlyException("There are no drivers found");
            }

            _FrmProgress = new ProgressForm("Generating data...", distinctDrivers.Count(),
            "Drivers processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(distinctDrivers);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<Employee> _drivers = (IEnumerable<Employee>)e.Argument;
            try
            {
                foreach (var item in _drivers)
                {
                    index++;
                    Employee driver = session.GetObjectByKey<Employee>(item.Oid);
                    _message = string.Format("Processing {0} successful.",
                    driver.No);
                    _BgWorker.ReportProgress(1, _message);

                    DriverPayrollBatch batch = session.GetObjectByKey<DriverPayrollBatch>(_DriverPayrollBatch.Oid);
                    if (batch.BatchType.IncludeLoans)
                    {
                        foreach (EmpLoan el in item.EmpLoans)
                        {
                            if (!el.Paid)
                            {
                                PayrollDeduction pd = ReflectionHelper.CreateObject<PayrollDeduction>(session);
                                pd.PayrollBatchID = batch;
                                pd.Employee = driver;
                                if (batch.PeriodStart.Month == 1)
                                    pd.Month = MonthsEnum.January;
                                if (batch.PeriodStart.Month == 2)
                                    pd.Month = MonthsEnum.February;
                                if (batch.PeriodStart.Month == 3)
                                    pd.Month = MonthsEnum.March;
                                if (batch.PeriodStart.Month == 4)
                                    pd.Month = MonthsEnum.April;
                                if (batch.PeriodStart.Month == 5)
                                    pd.Month = MonthsEnum.May;
                                if (batch.PeriodStart.Month == 6)
                                    pd.Month = MonthsEnum.June;
                                if (batch.PeriodStart.Month == 7)
                                    pd.Month = MonthsEnum.July;
                                if (batch.PeriodStart.Month == 8)
                                    pd.Month = MonthsEnum.August;
                                if (batch.PeriodStart.Month == 9)
                                    pd.Month = MonthsEnum.September;
                                if (batch.PeriodStart.Month == 10)
                                    pd.Month = MonthsEnum.October;
                                if (batch.PeriodStart.Month == 11)
                                    pd.Month = MonthsEnum.November;
                                if (batch.PeriodStart.Month == 12)
                                    pd.Month = MonthsEnum.December;
                                pd.DeductionType = DeductionType.Loan;
                                pd.DeductionName = el.LoanCode.Description;
                                if (el.LoanBalance < el.Amortization)
                                {
                                    pd.Amount = el.LoanBalance;
                                }
                                else
                                {
                                    pd.Amount = el.Amortization;
                                }
                                pd.RefNo = el.RefNo;
                                pd.LoanBalance = el.LoanBalance - pd.Amount;
                                pd.Save();

                            }
                        }

                    }
                    if (batch.BatchType.IncludePremiums)
                    {
                        foreach (EmployeePremium ep in item.EmployeePremiums)
                        {
                            PayrollDeduction pd=ReflectionHelper.CreateObject<PayrollDeduction>(session);
                            pd.PayrollBatchID = batch;
                            pd.Employee = driver;
                            if (batch.PeriodStart.Month == 1)
                                pd.Month = MonthsEnum.January;
                            if (batch.PeriodStart.Month == 2)
                                pd.Month = MonthsEnum.February;
                            if (batch.PeriodStart.Month == 3)
                                pd.Month = MonthsEnum.March;
                            if (batch.PeriodStart.Month == 4)
                                pd.Month = MonthsEnum.April;
                            if (batch.PeriodStart.Month == 5)
                                pd.Month = MonthsEnum.May;
                            if (batch.PeriodStart.Month == 6)
                                pd.Month = MonthsEnum.June;
                            if (batch.PeriodStart.Month == 7)
                                pd.Month = MonthsEnum.July;
                            if (batch.PeriodStart.Month == 8)
                                pd.Month = MonthsEnum.August;
                            if (batch.PeriodStart.Month == 9)
                                pd.Month = MonthsEnum.September;
                            if (batch.PeriodStart.Month == 10)
                                pd.Month = MonthsEnum.October;
                            if (batch.PeriodStart.Month == 11)
                                pd.Month = MonthsEnum.November;
                            if (batch.PeriodStart.Month == 12)
                                pd.Month = MonthsEnum.December;
                            pd.DeductionType=DeductionType.Premium;
                            pd.DeductionName=ep.PremiumCode.Description;
                            pd.Amount=ep.Amount;
                            pd.Save();
                        }
                    }
                    if (batch.BatchType.IncludeTax)
                    {
                        foreach (EmpTax et in item.EmpTaxs)
                        {
                            if (!et.Paid)
                            {
                                PayrollDeduction pd = ReflectionHelper.CreateObject<PayrollDeduction>(session);
                                pd.PayrollBatchID = batch;
                                pd.Employee = driver;
                                if (batch.PeriodStart.Month == 1)
                                    pd.Month = MonthsEnum.January;
                                if (batch.PeriodStart.Month == 2)
                                    pd.Month = MonthsEnum.February;
                                if (batch.PeriodStart.Month == 3)
                                    pd.Month = MonthsEnum.March;
                                if (batch.PeriodStart.Month == 4)
                                    pd.Month = MonthsEnum.April;
                                if (batch.PeriodStart.Month == 5)
                                    pd.Month = MonthsEnum.May;
                                if (batch.PeriodStart.Month == 6)
                                    pd.Month = MonthsEnum.June;
                                if (batch.PeriodStart.Month == 7)
                                    pd.Month = MonthsEnum.July;
                                if (batch.PeriodStart.Month == 8)
                                    pd.Month = MonthsEnum.August;
                                if (batch.PeriodStart.Month == 9)
                                    pd.Month = MonthsEnum.September;
                                if (batch.PeriodStart.Month == 10)
                                    pd.Month = MonthsEnum.October;
                                if (batch.PeriodStart.Month == 11)
                                    pd.Month = MonthsEnum.November;
                                if (batch.PeriodStart.Month == 12)
                                    pd.Month = MonthsEnum.December;
                                pd.Year = et.Year;
                                pd.DeductionType = DeductionType.Tax;
                                pd.DeductionName = et.TaxCode.Description;
                                if (et.TaxBalance < et.Deduction)
                                {
                                    pd.Amount = et.TaxBalance;
                                }
                                else
                                {
                                    pd.Amount = et.Deduction;
                                }
                                pd.TaxBalance = et.TaxBalance - pd.Amount;
                                pd.Save();
                            }
                        }
                    }
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
                if (index == _drivers.Count())
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
                    "Generation of payroll deductions data is cancelled.", "Cancelled",
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
                    "Payroll deductions data has been successfully generated.");
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
