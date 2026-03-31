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
    public partial class GenerateStaffPayroll : ViewController
    {
        private SimpleAction generateStaffPayroll;
        private StaffPayrollBatch _StaffPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public GenerateStaffPayroll()
        {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StaffPayrollBatch.GenerateStaffPayroll";
            this.generateStaffPayroll = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateStaffPayroll.Caption = "Generate Payroll";
            this.generateStaffPayroll.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateStaffPayroll_Execute);
        }
        private void GenerateStaffPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _StaffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as StaffPayrollBatch;

            try
            {
                for (int i = _StaffPayrollBatch.StaffPayrolls.Count - 1;
                i >= 0; i--)
                {
                    _StaffPayrollBatch.StaffPayrolls[i].Delete(
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
            try
            {
                //aCount = thisIS.CalculatedAttendance.Count;
                StaffPayrollBatch spb = session.GetObjectByKey<StaffPayrollBatch>(_StaffPayrollBatch.Oid);
                foreach (Employee item in _employees)
                {
                    index++;
                    _message = string.Format("Processing {0} succesfull.",
                    item.No);
                    _BgWorker.ReportProgress(1, _message);

                    Employee thisIS = session.GetObjectByKey<Employee>(item.Oid);
                    StaffPayroll spr = ReflectionHelper.CreateObject<StaffPayroll>(thisIS.Session);
                    spr.PayrollBatchID = spb;
                    spr.Employee = thisIS;
                    if (item.No == "E00288")
                    {

                    }
                    //spr.BasicHrs = 8 * thisIS.BatchType.MonthlyRegDays;
                    //decimal perHrRate = 0;
                    decimal allowance = 0m;
                    switch (item.PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            spr.RegularPay = thisIS.Basic;
                            break;
                        case EmployeePayTypeEnum.Daily:
                            spr.RegularPay = thisIS.Basic;
                            var calcs = spb.CalculatedAttendance2.Where(o => o.EmployeeId == thisIS);
                            if (calcs != null && calcs.Count() != 0)
                            {
                                // Basic Pay
                                spr.BasicHrs = calcs.Sum(o => o.BasicHrs);
                                spr.BasicAmt = calcs.Sum(o => o.BasicAmt);
                                // Absent
                                spr.AbsentHrs = calcs.Sum(o => o.AbsentHrs);
                                spr.AbsentAmt = calcs.Sum(o => o.AbsentAmt);
                                // Late
                                spr.LateHrs = calcs.Sum(o => o.LateHrs);
                                spr.LateAmt = calcs.Sum(o => o.LateAmt);
                                // Undertime
                                spr.UndertimeHrs = calcs.Sum(o => o.UndertimeHrs);
                                spr.UndertimeAmt = calcs.Sum(o => o.UndertimeAmt);
                                // Rest Day OT
                                spr.DayoffOTHrs = calcs.Sum(o => o.RestdayOtHrs);
                                spr.DayoffOTAmt = calcs.Sum(o => o.RestdayOtAmt);
                                // Overtime
                                spr.OvertimeHrs = calcs.Sum(o => o.OvertimeHrs);
                                spr.OvertimeAmt = calcs.Sum(o => o.OvertimeAmt);
                                // Night Diff
                                spr.NightDiffHrs = calcs.Sum(o => o.NightDiffHrs);
                                spr.NightDiffAmt = calcs.Sum(o => o.NightDiffAmt);
                                // Holiday (REG)
                                spr.HolidayHrs = calcs.Where(o => o.OtStatus == OtStatusEnum.Approved).Sum(o => o.HolidayHrs);
                                spr.HolidayAmt = calcs.Where(o => o.OtStatus == OtStatusEnum.Approved).Sum(o => o.HolidayAmt);
                                // Holiday OT (REG)
                                spr.HolidayOTHrs = calcs.Where(o => o.OtStatus == OtStatusEnum.Approved).Sum(o => o.HolidayOTHrs);
                                spr.HolidayOTAmt = calcs.Where(o => o.OtStatus == OtStatusEnum.Approved).Sum(o => o.HolidayOTAmt);
                                // Holiday (SPC)
                                spr.HolidayHrs2 = calcs.Sum(o => o.HolidayHrs2);
                                spr.HolidayAmt2 = calcs.Sum(o => o.HolidayAmt2);
                                // Holiday OT (SPC)
                                spr.HolidayOTHrs2 = calcs.Where(o => o.OtStatus == OtStatusEnum.Approved).Sum(o => o.HolidayOTHrs2);
                                spr.HolidayOTAmt2 = calcs.Where(o => o.OtStatus == OtStatusEnum.Approved).Sum(o => o.HolidayOTAmt2);
                                // Allowance
                                allowance = calcs.Sum(o => o.Allowance);
                            }
                            break;
                        case EmployeePayTypeEnum.Monthly:
                            spr.RegularPay = thisIS.Basic;
                            spr.BasicAmt = thisIS.Basic / 2;
                            allowance = thisIS.Allowance;
                            break;
                        default:
                            break;
                    }
                    // Create allowance adjustmen line
                    if (allowance > 0m)
                    {
                        StaffPayrollAdjustment spa = ReflectionHelper.CreateObject<StaffPayrollAdjustment>(thisIS.Session);
                        spa.StaffPayrollID = spr;
                        spa.Employee = thisIS;
                        PayAdjustmentType padtype = session.FindObject<PayAdjustmentType>(BinaryOperator.Parse("[Description]='Allowance'"));
                        spa.AdjustmentType = padtype;
                        spa.Explanation = "Allowance";
                        spa.AutoGenerated = true;
                        spa.Amount = allowance;
                        spa.Save();
                    }
                    // Insert incentives here
                    if (thisIS.EmpIncentives.Count > 0)
                    {
                        foreach (var inc in thisIS.EmpIncentives)
                        {
                            StaffPayrollAdjustment spa = ReflectionHelper.CreateObject<StaffPayrollAdjustment>(thisIS.Session);
                            spa.StaffPayrollID = spr;
                            spa.Employee = thisIS;
                            spa.AdjustmentType = inc.AdjustmentType;
                            spa.Explanation = !string.IsNullOrEmpty(inc.Explanation) ? inc.Explanation : inc.AdjustmentType.Description;
                            spa.AutoGenerated = true;
                            spa.Amount = inc.Amount;
                            spa.Save();
                        }
                    }
                    var adjts = spb.PayrollAdjustments.Where(o => o.Employee == thisIS);
                    if (adjts.Count() > 0)
                    {
                        foreach (PayrollAdjustment payadj in adjts)
                        {
                            StaffPayrollAdjustment spa=ReflectionHelper.CreateObject<StaffPayrollAdjustment>(thisIS.Session);
                            spa.StaffPayrollID=spr;
                            spa.Employee = thisIS;
                            spa.AdjustmentType=payadj.AdjustmentType;
                            spa.Explanation = !string.IsNullOrEmpty(payadj.Explanation) ? payadj.Explanation : payadj.AdjustmentType.Description;
                            spa.AutoGenerated = false;
                            spa.Amount=payadj.Amount;
                            spa.Save();
                        }
                    }

                    var deds = spb.PayrollDeductions.Where(o => o.Employee == thisIS);
                    if (deds.Count() > 0)
                    {
                        foreach (PayrollDeduction de in deds)
                        {
                            if (spb.BatchType.IncludeLoans!=true && de.DeductionType== DeductionType.Loan)
                            {
                                continue;
                            }
                            if (spb.BatchType.IncludePremiums != true && de.DeductionType == DeductionType.Premium)
                            {
                                continue;
                            }
                            if (spb.BatchType.IncludeTax != true && de.DeductionType == DeductionType.Tax)
                            {
                                continue;
                            }
                            StaffPayrollDeduction spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(thisIS.Session);
                            spd.StaffPayrollID=spr;
                            spd.Employee = thisIS;
                            spd.DeductionType=de.DeductionType;
                            spd.DeductionName=de.DeductionName;
                            spd.Month=de.Month;
                            spd.Amount=de.Amount;
                            if (de.DeductionType==DeductionType.Tax)
                            {
                                spd.Balance = de.TaxBalance;
                            }
                            else
                            {
                                spd.Balance=de.LoanBalance;
                            }
                            spd.Save();
                        }
                    }
                    var odeds = spb.PayrollDeductionOthers.Where(o => o.Employee == thisIS);
                    if (odeds.Count() > 0)
                    {
                        foreach (PayrollDeductionOther ode in odeds)
                        {
                            if (spb.BatchType.IncludeOtherDed != true && ode.DeductionType == DeductionType.Other && ode.Amount > 0)
                            {
                                continue;
                            }
                            StaffPayrollDeduction spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(thisIS.Session);
                            spd.StaffPayrollID = spr;
                            spd.Employee = thisIS;
                            spd.DeductionType = ode.DeductionType;
                            spd.DeductionName = ode.Explanation;
                            spd.Month = MonthsEnum.None;
                            spd.Amount = ode.Amount;
                            spd.Balance = ode.Balance;
                            spd.Save();
                        }
                    }
                    

                    spr.Save();
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
                    "Generation of payroll data is cancelled.", "Cancelled",
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
                    "Payroll data has been successfully generated.");
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
