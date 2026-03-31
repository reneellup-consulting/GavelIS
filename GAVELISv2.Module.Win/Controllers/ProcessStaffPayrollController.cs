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
    public partial class ProcessStaffPayrollController : ViewController
    {
        private SimpleAction processStaffPayroll;
        private StaffPayrollBatch _StaffPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ProcessStaffPayrollController()
        {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StaffPayrollBatch.ProcessStaffPayroll";
            this.processStaffPayroll = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.processStaffPayroll.TargetObjectsCriteria = "[Status] = 'Current'";
            this.processStaffPayroll.Caption = "Process";
            this.processStaffPayroll.Execute += new
            SimpleActionExecuteEventHandler(
            ProcessStaffPayroll_Execute);
        }

        private void ProcessStaffPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _StaffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as StaffPayrollBatch;

            ObjectSpace.CommitChanges();

            if (_StaffPayrollBatch.AttendanceRecords.Count == 0)
            {
                throw new UserFriendlyException("There are no attendance records listed.");
            }

            _FrmProgress = new ProgressForm("Processing data...", _StaffPayrollBatch.AttendanceRecords.Count,
            "Data processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_StaffPayrollBatch.AttendanceRecords);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<AttendanceRecord> _attRecs = (XPCollection<AttendanceRecord>)e.Argument;
            try
            {
                StaffPayrollBatch spb = session.GetObjectByKey<StaffPayrollBatch>(_StaffPayrollBatch.Oid);
                foreach (var item in _attRecs)
                {
                    index++;
                    _message = string.Format("Processing {0} succesfull.",
                    item.EmployeeID.Name);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    AttendanceRecord arc = session.GetObjectByKey<AttendanceRecord>(item.Oid);
                    StaffPayroll spr = spb.StaffPayrolls.Where(o=>o.AttRecId == arc).FirstOrDefault();
                    if (spr == null)
                    {
                        spr = ReflectionHelper.CreateObject<StaffPayroll>(session);
                        spr.PayrollBatchID = spb;
                        spr.AttRecId = arc;
                    }
                    spr.Employee = arc.EmployeeID;
                    spr.RegularPay = arc.Basic;

                    #region Paydetails here...

                    for (int i = spr.PayDetails.Count - 1; i >= 0; i--)
                    {
                        spr.PayDetails[i].Delete();
                    }

                    // Basic No. of Days
                    StaffPayrollPayDetail sppd1 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    sppd1.StaffPayrollId = spr;
                    sppd1.LineCaption = item.BasicCaption;
                    sppd1.LineAmount = item.BasicValue;
                    sppd1.Save();
                    // Less: Late/Undertime
                    StaffPayrollPayDetail sppd2 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    sppd2.StaffPayrollId = spr;
                    sppd2.LineCaption = item.LateUndCaption;
                    sppd2.LineAmount = item.LateUndValue;
                    sppd2.Save();
                    // -------------
                    StaffPayrollPayDetail singleLine1 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    singleLine1.StaffPayrollId = spr;
                    singleLine1.LineCaption = string.Empty;
                    singleLine1.LineAmount = "--------------------";
                    singleLine1.Save();
                    // SUB TOTAL:
                    StaffPayrollPayDetail sppd3 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    sppd3.StaffPayrollId = spr;
                    sppd3.LineCaption = string.Empty;
                    sppd3.LineAmount = (decimal.Parse(sppd1.LineAmount) + decimal.Parse(sppd2.LineAmount)).ToString("n2");
                    sppd3.Save();
                    // Space 1
                    StaffPayrollPayDetail space1 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    space1.StaffPayrollId = spr;
                    space1.LineCaption = string.Empty;
                    space1.LineAmount = string.Empty;
                    space1.Save();
                    // Allowance
                    StaffPayrollPayDetail sppd4 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    sppd4.StaffPayrollId = spr;
                    sppd4.LineCaption = item.AllowanceCaption;
                    sppd4.LineAmount = item.AllowanceValue;
                    sppd4.Save();
                    // Overtime
                    if (!item.OvertimeCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.OvertimeCaption;
                        sppd5.LineAmount = item.OvertimeValue;
                        sppd5.Save();
                    }
                    // Restday Duty
                    if (!item.RestdayDutyCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdayDutyCaption;
                        sppd5.LineAmount = item.RestdayDutyValue;
                        sppd5.Save();
                    }
                    // Restday OT
                    if (!item.RestdayOTCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdayOTCaption;
                        sppd5.LineAmount = item.RestdayOTValue;
                        sppd5.Save();
                    }
                    // Restday SP Duty
                    if (!item.RestdaySpDutyCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdaySpDutyCaption;
                        sppd5.LineAmount = item.RestdayDutySpValue;
                        sppd5.Save();
                    }
                    // Restday RG Duty
                    if (!item.RestdayRgDutyCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdayRgDutyCaption;
                        sppd5.LineAmount = item.RestdayDutyRgValue;
                        sppd5.Save();
                    }
                    // Restday DB Duty
                    if (!item.RestdayDbDutyCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdayDbDutyCaption;
                        sppd5.LineAmount = item.RestdayDutyDbValue;
                        sppd5.Save();
                    }
                    // Restday SPOT
                    if (!item.RestdaySpOtCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdaySpOtCaption;
                        sppd5.LineAmount = item.RestdaySpOtValue;
                        sppd5.Save();
                    }
                    // Restday RGOT
                    if (!item.RestdayRgOtCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdayRgOtCaption;
                        sppd5.LineAmount = item.RestdayRgOtValue;
                        sppd5.Save();
                    }
                    // Restday DBOT
                    if (!item.RestdayDbOtCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RestdayDbOtCaption;
                        sppd5.LineAmount = item.RestdayDbOtValue;
                        sppd5.Save();
                    }
                    // Night Diff.
                    if (!item.NightDiffCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.NightDiffCaption;
                        sppd5.LineAmount = item.NightDiffValue;
                        sppd5.Save();
                    }
                    // Reg. Holiday
                    if (!item.RegHolCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RegHolCaption;
                        sppd5.LineAmount = item.RegHolValue;
                        sppd5.Save();
                    }
                    // Reg. Hol. OT
                    if (!item.RegHolOTCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.RegHolOTCaption;
                        sppd5.LineAmount = item.RegHolOTValue;
                        sppd5.Save();
                    }
                    // Spc. Holiday
                    if (!item.SpcHolCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.SpcHolCaption;
                        sppd5.LineAmount = item.SpcHolValue;
                        sppd5.Save();
                    }
                    // Spc. Hol. OT
                    if (!item.SpcHolOTCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.SpcHolOTCaption;
                        sppd5.LineAmount = item.SpcHolOTValue;
                        sppd5.Save();
                    }
                    // Double Holiday
                    if (!item.DoubHolCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.DoubHolCaption;
                        sppd5.LineAmount = item.DoubleHolValue;
                        sppd5.Save();
                    }
                    // Double Hol. OT
                    if (!item.DoubHolOTCaption.Contains("NONE"))
                    {
                        StaffPayrollPayDetail sppd5 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                        sppd5.StaffPayrollId = spr;
                        sppd5.LineCaption = item.DoubHolOTCaption;
                        sppd5.LineAmount = item.DoubHolOTValue;
                        sppd5.Save();
                    }
                    // ==================
                    StaffPayrollPayDetail equalLine1 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    equalLine1.StaffPayrollId = spr;
                    equalLine1.LineCaption = string.Empty;
                    equalLine1.LineAmount = "==========";
                    equalLine1.Save();
                    // GROSS PAY:
                    StaffPayrollPayDetail sppd6 = ReflectionHelper.CreateObject<StaffPayrollPayDetail>(session);
                    sppd6.StaffPayrollId = spr;
                    sppd6.LineCaption = string.Empty;
                    sppd6.LineAmount = item.SubTotalValue;
                    sppd6.Save();

                    spr.PayValue = decimal.Parse(item.SubTotalValue); 
                    #endregion

                    #region Incentives here...
                    if (arc.EmployeeID.EmpIncentives.Count > 0)
                    {
                        foreach (var inc in arc.EmployeeID.EmpIncentives)
                        {
                            if (inc.OnlyInBatch != null && inc.OnlyInBatch != spb.BatchType)
                            {
                                continue;
                            }
                            string lid = string.Format("0{0}-{1}", spr.AttRecId.Oid, inc.Oid);
                            StaffPayrollAdjustment spa = session.FindObject<StaffPayrollAdjustment>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (spa == null)
                            {
                                spa = ReflectionHelper.CreateObject<StaffPayrollAdjustment>(session);
                                spa.LineID = lid;
                            }
                            spa.StaffPayrollID = spr;
                            spa.Employee = arc.EmployeeID;
                            spa.AdjustmentType = inc.AdjustmentType;
                            spa.Explanation = !string.IsNullOrEmpty(inc.Explanation) ? inc.Explanation : inc.AdjustmentType.Description;
                            spa.AutoGenerated = true;
                            if (inc.DivPerDay)
                            {
                                int days = WorkingDays.GetWorkingDays(arc.BatchID.TimeRangeFrom, arc.BatchID.TimeRangeTo, new List<DayOfWeek> { arc.EmployeeID.RestDay });
                                decimal pd = inc.Amount / days;
                                if (AlterValue(spa.Amount))
                                {
                                    spa.Amount = pd * arc.NoOfDays != null ? arc.NoOfDays.Value : 0;
                                }
                            }
                            else
                            {
                                if (AlterValue(spa.Amount))
                                {
                                    spa.Amount = inc.Amount;
                                }
                            }
                            spa.Save();
                        }
                    }
                    #endregion

                    #region Premiums here...
                    if (spb.BatchType.IncludePremiums)
                    {
                        foreach (var prms in arc.EmployeeID.EmployeePremiums)
                        {
                            string lid = string.Format("1{0}-{1}", spr.AttRecId.Oid, prms.Oid);
                            StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (spd == null)
                            {
                                spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                                spd.LineID = lid;
                                spd.DedId = prms.Oid;
                            }
                            spd.StaffPayrollID = spr;
                            spd.Employee = arc.EmployeeID;
                            spd.DeductionType = DeductionType.Premium;
                            spd.DeductionName = prms.PremiumCode.Description;
                            if (spb.PeriodStart.Month == 1)
                                spd.Month = MonthsEnum.January;
                            if (spb.PeriodStart.Month == 2)
                                spd.Month = MonthsEnum.February;
                            if (spb.PeriodStart.Month == 3)
                                spd.Month = MonthsEnum.March;
                            if (spb.PeriodStart.Month == 4)
                                spd.Month = MonthsEnum.April;
                            if (spb.PeriodStart.Month == 5)
                                spd.Month = MonthsEnum.May;
                            if (spb.PeriodStart.Month == 6)
                                spd.Month = MonthsEnum.June;
                            if (spb.PeriodStart.Month == 7)
                                spd.Month = MonthsEnum.July;
                            if (spb.PeriodStart.Month == 8)
                                spd.Month = MonthsEnum.August;
                            if (spb.PeriodStart.Month == 9)
                                spd.Month = MonthsEnum.September;
                            if (spb.PeriodStart.Month == 10)
                                spd.Month = MonthsEnum.October;
                            if (spb.PeriodStart.Month == 11)
                                spd.Month = MonthsEnum.November;
                            if (spb.PeriodStart.Month == 12)
                                spd.Month = MonthsEnum.December;
                            spd.Caption = string.Format("{0}|{1}", prms.PremiumCode.Description, spd.MonthStr);
                            if (AlterValue(spd.Amount))
                            {
                                spd.Amount = prms.Amount;
                            }
                            spd.Save();
                        }
                    }
                    #endregion

                    #region EveryPayroll here...

                    foreach (var lns in arc.EmployeeID.EmpLoans)
                    {
                        if (!lns.LoanCode.EveryPayrollDeduction)
                        {
                            continue;
                        }
                        if (lns.Paid)
                        {
                            continue;
                        }
                        string lid = string.Format("2{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                        StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                        if (spd == null)
                        {
                            spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                            spd.LineID = lid;
                            spd.DedId = lns.Oid;
                        }
                        spd.StaffPayrollID = spr;
                        spd.Employee = arc.EmployeeID;
                        spd.DeductionType = DeductionType.Loan;
                        spd.DeductionName = lns.LoanCode.Description;
                        if (spb.PeriodStart.Month == 1)
                            spd.Month = MonthsEnum.January;
                        if (spb.PeriodStart.Month == 2)
                            spd.Month = MonthsEnum.February;
                        if (spb.PeriodStart.Month == 3)
                            spd.Month = MonthsEnum.March;
                        if (spb.PeriodStart.Month == 4)
                            spd.Month = MonthsEnum.April;
                        if (spb.PeriodStart.Month == 5)
                            spd.Month = MonthsEnum.May;
                        if (spb.PeriodStart.Month == 6)
                            spd.Month = MonthsEnum.June;
                        if (spb.PeriodStart.Month == 7)
                            spd.Month = MonthsEnum.July;
                        if (spb.PeriodStart.Month == 8)
                            spd.Month = MonthsEnum.August;
                        if (spb.PeriodStart.Month == 9)
                            spd.Month = MonthsEnum.September;
                        if (spb.PeriodStart.Month == 10)
                            spd.Month = MonthsEnum.October;
                        if (spb.PeriodStart.Month == 11)
                            spd.Month = MonthsEnum.November;
                        if (spb.PeriodStart.Month == 12)
                            spd.Month = MonthsEnum.December;
                        spd.Caption = string.Format("{0}|{1}", lns.LoanCode.Code, lns.RefNo);
                        if (lns.LoanBalance < lns.Amortization)
                        {
                            if (AlterValue(spd.Amount))
                            {
                                spd.Amount = lns.LoanBalance;
                            }
                            //spd.Amount = lns.LoanBalance;
                        }
                        else
                        {
                            if (AlterValue(spd.Amount))
                            {
                                spd.Amount = lns.Amortization;
                            }
                            //spd.Amount = lns.Amortization;
                        }
                        spd.RefNo = lns.RefNo;
                        spd.Balance = lns.LoanBalance;
                        spd.Save();
                    }
                    #endregion
                    
                    #region Loans here...
                    if (spb.BatchType.IncludeLoans)
                    {
                        foreach (var lns in arc.EmployeeID.EmpLoans)
                        {
                            if (lns.LoanCode.EveryPayrollDeduction)
                            {
                                continue;
                            }
                            if (lns.Paid)
                            {
                                continue;
                            }
                            string lid = string.Format("2{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                            StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (spd == null)
                            {
                                spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                                spd.LineID = lid;
                                spd.DedId = lns.Oid;
                            }
                            spd.StaffPayrollID = spr;
                            spd.Employee = arc.EmployeeID;
                            spd.DeductionType = DeductionType.Loan;
                            spd.DeductionName = lns.LoanCode.Description;
                            if (spb.PeriodStart.Month == 1)
                                spd.Month = MonthsEnum.January;
                            if (spb.PeriodStart.Month == 2)
                                spd.Month = MonthsEnum.February;
                            if (spb.PeriodStart.Month == 3)
                                spd.Month = MonthsEnum.March;
                            if (spb.PeriodStart.Month == 4)
                                spd.Month = MonthsEnum.April;
                            if (spb.PeriodStart.Month == 5)
                                spd.Month = MonthsEnum.May;
                            if (spb.PeriodStart.Month == 6)
                                spd.Month = MonthsEnum.June;
                            if (spb.PeriodStart.Month == 7)
                                spd.Month = MonthsEnum.July;
                            if (spb.PeriodStart.Month == 8)
                                spd.Month = MonthsEnum.August;
                            if (spb.PeriodStart.Month == 9)
                                spd.Month = MonthsEnum.September;
                            if (spb.PeriodStart.Month == 10)
                                spd.Month = MonthsEnum.October;
                            if (spb.PeriodStart.Month == 11)
                                spd.Month = MonthsEnum.November;
                            if (spb.PeriodStart.Month == 12)
                                spd.Month = MonthsEnum.December;
                            spd.Caption = string.Format("{0}|{1}", lns.LoanCode.Code, lns.RefNo);
                            if (lns.LoanBalance < lns.Amortization)
                            {
                                if (AlterValue(spd.Amount))
                                {
                                    spd.Amount = lns.LoanBalance;
                                }
                                //spd.Amount = lns.LoanBalance;
                            }
                            else
                            {
                                if (AlterValue(spd.Amount))
                                {
                                    spd.Amount = lns.Amortization;
                                }
                                //spd.Amount = lns.Amortization;
                            }
                            spd.RefNo = lns.RefNo;
                            spd.Balance = lns.LoanBalance;
                            spd.Save();
                        }
                    }
                    #endregion

                    #region Taxes here...
                    if (spb.BatchType.IncludeTax)
                    {
                        foreach (var lns in arc.EmployeeID.EmpTaxs)
                        {
                            if (lns.Paid)
                            {
                                continue;
                            }
                            string lid = string.Format("3{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                            StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (spd == null)
                            {
                                spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                                spd.LineID = lid;
                                spd.DedId = lns.Oid;
                            }
                            spd.StaffPayrollID = spr;
                            spd.Employee = arc.EmployeeID;
                            spd.DeductionType = DeductionType.Tax;
                            spd.DeductionName = lns.TaxCode.Description;
                            if (spb.PeriodStart.Month == 1)
                                spd.Month = MonthsEnum.January;
                            if (spb.PeriodStart.Month == 2)
                                spd.Month = MonthsEnum.February;
                            if (spb.PeriodStart.Month == 3)
                                spd.Month = MonthsEnum.March;
                            if (spb.PeriodStart.Month == 4)
                                spd.Month = MonthsEnum.April;
                            if (spb.PeriodStart.Month == 5)
                                spd.Month = MonthsEnum.May;
                            if (spb.PeriodStart.Month == 6)
                                spd.Month = MonthsEnum.June;
                            if (spb.PeriodStart.Month == 7)
                                spd.Month = MonthsEnum.July;
                            if (spb.PeriodStart.Month == 8)
                                spd.Month = MonthsEnum.August;
                            if (spb.PeriodStart.Month == 9)
                                spd.Month = MonthsEnum.September;
                            if (spb.PeriodStart.Month == 10)
                                spd.Month = MonthsEnum.October;
                            if (spb.PeriodStart.Month == 11)
                                spd.Month = MonthsEnum.November;
                            if (spb.PeriodStart.Month == 12)
                                spd.Month = MonthsEnum.December;
                            spd.Caption = string.Format("{0}|{1}", lns.TaxCode.Description, spd.MonthStr);
                            if (lns.TaxBalance < lns.Deduction)
                            {
                                if (AlterValue(spd.Amount))
                                {
                                    spd.Amount = lns.TaxBalance;
                                }
                                //spd.Amount = lns.TaxBalance;
                            }
                            else
                            {
                                if (AlterValue(spd.Amount))
                                {
                                    spd.Amount = lns.Deduction;
                                }
                                //spd.Amount = lns.Deduction;
                            }
                            spd.RefNo = lns.Year.ToString();
                            spd.Balance = lns.TaxBalance;
                            spd.Save();
                        }
                    }
                    #endregion

                    #region Other Deduction

                    if (spb.BatchType.IncludeOtherDed)
                    {
                        foreach (var lns in arc.EmployeeID.EmpOtherDeds)
                        {
                            if (lns.Paid)
                            {
                                continue;
                            }
                            string lid = string.Format("4{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                            StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (spd == null)
                            {
                                spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                                spd.LineID = lid;
                                spd.DedId = lns.Oid;
                            }
                            spd.StaffPayrollID = spr;
                            spd.Employee = arc.EmployeeID;
                            spd.DeductionType = DeductionType.Other;
                            if (!string.IsNullOrEmpty(lns.Explanation))
                            {
                                spd.DeductionName = lns.DedCode.Code + " | " + lns.Explanation;
                                spd.Caption = string.Format("{0}|{1}|{2}", lns.DedCode.Description, lns.EntryDate.ToShortDateString(), lns.RefNo);
                                //spd.Caption = string.Format("{0}|{1}|{2}-{3}", lns.DedCode.Code, lns.EntryDate.ToShortDateString(), lns.RefNo, lns.Explanation);
                            }
                            else
                            {
                                spd.DeductionName = lns.DedCode.Description;
                                spd.Caption = string.Format("{0}|{1}|{2}", lns.DedCode.Description, lns.EntryDate.ToShortDateString(), lns.RefNo);
                            }
                            if (spb.PeriodStart.Month == 1)
                                spd.Month = MonthsEnum.January;
                            if (spb.PeriodStart.Month == 2)
                                spd.Month = MonthsEnum.February;
                            if (spb.PeriodStart.Month == 3)
                                spd.Month = MonthsEnum.March;
                            if (spb.PeriodStart.Month == 4)
                                spd.Month = MonthsEnum.April;
                            if (spb.PeriodStart.Month == 5)
                                spd.Month = MonthsEnum.May;
                            if (spb.PeriodStart.Month == 6)
                                spd.Month = MonthsEnum.June;
                            if (spb.PeriodStart.Month == 7)
                                spd.Month = MonthsEnum.July;
                            if (spb.PeriodStart.Month == 8)
                                spd.Month = MonthsEnum.August;
                            if (spb.PeriodStart.Month == 9)
                                spd.Month = MonthsEnum.September;
                            if (spb.PeriodStart.Month == 10)
                                spd.Month = MonthsEnum.October;
                            if (spb.PeriodStart.Month == 11)
                                spd.Month = MonthsEnum.November;
                            if (spb.PeriodStart.Month == 12)
                                spd.Month = MonthsEnum.December;
                            if (lns.Balance < lns.Deduction)
                            {
                                if (AlterValue(spd.Amount))
                                {
                                    spd.Amount = lns.Balance;
                                }
                                //spd.Amount = lns.Balance;
                            }
                            else
                            {
                                if (AlterValue(spd.Amount))
                                {
                                    spd.Amount = lns.Deduction;
                                }
                                //spd.Amount = lns.Deduction;
                            }
                            spd.RefNo = lns.RefNo;
                            spd.Balance = lns.Balance;
                            spd.Save();
                        }
                    }
                    #endregion

                    spr.Save();
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
                if (index == _attRecs.Count)
                {
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }

        private bool AlterValue(decimal p)
        {
            if (p == 0)
            {
                return true;
            }
            else
            {
                return false;
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
