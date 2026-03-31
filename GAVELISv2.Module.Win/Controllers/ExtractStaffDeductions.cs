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
    public partial class ExtractStaffDeductions : ViewController
    {
        private SimpleAction extractStaffDeductions;
        private StaffPayrollBatch _StaffPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ExtractStaffDeductions()
        {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StaffPayrollBatch.ExtractStaffDeductions";
            this.extractStaffDeductions = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.extractStaffDeductions.Caption = "Extract Deductions";
            this.extractStaffDeductions.Execute += new
            SimpleActionExecuteEventHandler(
            ExtractStaffDeductions_Execute);
        }
        private void ExtractStaffDeductions_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _StaffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as StaffPayrollBatch;

            try
            {
                for (int i = _StaffPayrollBatch.PayrollDeductions.Count - 1;
                i >= 0; i--)
                {
                    _StaffPayrollBatch.PayrollDeductions[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();

            var distinctStaff = _StaffPayrollBatch.CalculatedAttendance2.Select(o => o.EmployeeId).Distinct();

            if (distinctStaff.Count() == 0)
            {
                throw new UserFriendlyException("There are no employees found");
            }

            _FrmProgress = new ProgressForm("Generating data...", distinctStaff.Count(),
            "Employees processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(distinctStaff);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<Employee> _employees = (IEnumerable<Employee>)e.Argument;
            StaffPayrollBatch thisIS = session.GetObjectByKey<StaffPayrollBatch>(
            _StaffPayrollBatch.Oid);
            try
            {
                foreach (Employee item in _employees)
                {
                    if (item.No == "E00310")
                    {

                    }
                    index++;
                    Employee emp = session.GetObjectByKey<Employee>(item.Oid);
                    _message = string.Format("Processing {0} successful.",
                    item.No);
                    _BgWorker.ReportProgress(1, _message);

                    if (_StaffPayrollBatch.BatchType.IncludeLoans)
                    {
                        foreach (EmpLoan el in item.EmpLoans)
                        {
                            if (!el.Paid)
                            {
                                PayrollDeduction pd = ReflectionHelper.CreateObject<PayrollDeduction>(thisIS.Session);
                                pd.PayrollBatchID = thisIS;
                                pd.Employee = emp;
                                if (thisIS.PeriodStart.Month == 1)
                                    pd.Month = MonthsEnum.January;
                                if (thisIS.PeriodStart.Month == 2)
                                    pd.Month = MonthsEnum.February;
                                if (thisIS.PeriodStart.Month == 3)
                                    pd.Month = MonthsEnum.March;
                                if (thisIS.PeriodStart.Month == 4)
                                    pd.Month = MonthsEnum.April;
                                if (thisIS.PeriodStart.Month == 5)
                                    pd.Month = MonthsEnum.May;
                                if (thisIS.PeriodStart.Month == 6)
                                    pd.Month = MonthsEnum.June;
                                if (thisIS.PeriodStart.Month == 7)
                                    pd.Month = MonthsEnum.July;
                                if (thisIS.PeriodStart.Month == 8)
                                    pd.Month = MonthsEnum.August;
                                if (thisIS.PeriodStart.Month == 9)
                                    pd.Month = MonthsEnum.September;
                                if (thisIS.PeriodStart.Month == 10)
                                    pd.Month = MonthsEnum.October;
                                if (thisIS.PeriodStart.Month == 11)
                                    pd.Month = MonthsEnum.November;
                                if (thisIS.PeriodStart.Month == 12)
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
                    if (_StaffPayrollBatch.BatchType.IncludePremiums)
                    {
                        foreach (EmployeePremium ep in item.EmployeePremiums)
                        {
                            PayrollDeduction pd = ReflectionHelper.CreateObject<PayrollDeduction>(thisIS.Session);
                            pd.PayrollBatchID = thisIS;
                            pd.Employee = emp;
                            if (thisIS.PeriodStart.Month == 1)
                                pd.Month = MonthsEnum.January;
                            if (thisIS.PeriodStart.Month == 2)
                                pd.Month = MonthsEnum.February;
                            if (thisIS.PeriodStart.Month == 3)
                                pd.Month = MonthsEnum.March;
                            if (thisIS.PeriodStart.Month == 4)
                                pd.Month = MonthsEnum.April;
                            if (thisIS.PeriodStart.Month == 5)
                                pd.Month = MonthsEnum.May;
                            if (thisIS.PeriodStart.Month == 6)
                                pd.Month = MonthsEnum.June;
                            if (thisIS.PeriodStart.Month == 7)
                                pd.Month = MonthsEnum.July;
                            if (thisIS.PeriodStart.Month == 8)
                                pd.Month = MonthsEnum.August;
                            if (thisIS.PeriodStart.Month == 9)
                                pd.Month = MonthsEnum.September;
                            if (thisIS.PeriodStart.Month == 10)
                                pd.Month = MonthsEnum.October;
                            if (thisIS.PeriodStart.Month == 11)
                                pd.Month = MonthsEnum.November;
                            if (thisIS.PeriodStart.Month == 12)
                                pd.Month = MonthsEnum.December;
                            pd.DeductionType = DeductionType.Premium;
                            pd.DeductionName = ep.PremiumCode.Description;
                            pd.Amount = ep.Amount;
                            pd.Save();
                        }
                    }
                    if (_StaffPayrollBatch.BatchType.IncludeTax)
                    {
                        foreach (EmpTax et in item.EmpTaxs)
                        {
                            if (!et.Paid)
                            {
                                PayrollDeduction pd = ReflectionHelper.CreateObject<PayrollDeduction>(thisIS.Session);
                                pd.PayrollBatchID = thisIS;
                                pd.Employee = emp;
                                if (thisIS.PeriodStart.Month == 1)
                                    pd.Month = MonthsEnum.January;
                                if (thisIS.PeriodStart.Month == 2)
                                    pd.Month = MonthsEnum.February;
                                if (thisIS.PeriodStart.Month == 3)
                                    pd.Month = MonthsEnum.March;
                                if (thisIS.PeriodStart.Month == 4)
                                    pd.Month = MonthsEnum.April;
                                if (thisIS.PeriodStart.Month == 5)
                                    pd.Month = MonthsEnum.May;
                                if (thisIS.PeriodStart.Month == 6)
                                    pd.Month = MonthsEnum.June;
                                if (thisIS.PeriodStart.Month == 7)
                                    pd.Month = MonthsEnum.July;
                                if (thisIS.PeriodStart.Month == 8)
                                    pd.Month = MonthsEnum.August;
                                if (thisIS.PeriodStart.Month == 9)
                                    pd.Month = MonthsEnum.September;
                                if (thisIS.PeriodStart.Month == 10)
                                    pd.Month = MonthsEnum.October;
                                if (thisIS.PeriodStart.Month == 11)
                                    pd.Month = MonthsEnum.November;
                                if (thisIS.PeriodStart.Month == 12)
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
                if (index == _employees.Count())
                {
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
                    "Extraction of payroll deductions data is cancelled.", "Cancelled",
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
