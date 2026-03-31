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
    public partial class ExtractStaffOtherDeductions : ViewController
    {
        private SimpleAction extractStaffOtherDeductions;
        private StaffPayrollBatch _StaffPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ExtractStaffOtherDeductions()
        {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StaffPayrollBatch.ExtractStaffOtherDeductions";
            this.extractStaffOtherDeductions = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.extractStaffOtherDeductions.Caption = "Extract Other Deductions";
            this.extractStaffOtherDeductions.Execute += new
            SimpleActionExecuteEventHandler(
            ExtractStaffOtherDeductions_Execute);
        }
        private void ExtractStaffOtherDeductions_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _StaffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as StaffPayrollBatch;

            try
            {
                for (int i = _StaffPayrollBatch.PayrollDeductionOthers.Count - 1;
                i >= 0; i--)
                {
                    _StaffPayrollBatch.PayrollDeductionOthers[i].Delete(
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
                    index++;
                    Employee emp = session.GetObjectByKey<Employee>(item.Oid);
                    _message = string.Format("Processing {0} successful.",
                    item.No);
                    _BgWorker.ReportProgress(1, _message);

                    if (_StaffPayrollBatch.BatchType.IncludeOtherDed)
                    {
                        foreach (EmpOtherDed ed in item.EmpOtherDeds)
                        {
                            if (!ed.Paid)
                            {
                                EmpOtherDed edo = session.GetObjectByKey<EmpOtherDed>(ed.Oid);
                                PayrollDeductionOther pdo = ReflectionHelper.CreateObject<PayrollDeductionOther>(thisIS.Session);
                                pdo.PayrollBatchID = thisIS;
                                pdo.Employee = emp;
                                pdo.DeductionDate = _StaffPayrollBatch.PeriodEnd;
                                pdo.DeductionType = DeductionType.Other;
                                pdo.DeductionCode = edo.DedCode;
                                pdo.Explanation = edo.Explanation;
                                pdo.RefNo = edo.RefNo;
                                if (edo.Balance < edo.Deduction)
                                {
                                    pdo.Amount = edo.Balance;
                                }
                                else
                                {
                                    pdo.Amount = edo.Deduction;
                                }
                                pdo.Balance = edo.Balance - pdo.Amount;
                                pdo.Save();

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
                    "Extraction of payroll other deductions data is cancelled.", "Cancelled",
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
                    "Extraction of other deductions data has been successfully generated.");
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
