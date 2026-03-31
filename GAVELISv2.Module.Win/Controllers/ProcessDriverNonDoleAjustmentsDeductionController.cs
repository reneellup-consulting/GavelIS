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
    public partial class ProcessDriverNonDoleAjustmentsDeductionController : ViewController
    {
        private SimpleAction processDriverNonDoleAjustmentsDeduction;
        private DriverPayrollBatch2 _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ProcessDriverNonDoleAjustmentsDeductionController()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch2.ProcessDriverNonDoleAjustmentsDeduction";
            this.processDriverNonDoleAjustmentsDeduction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.processDriverNonDoleAjustmentsDeduction.TargetObjectsCriteria = "[Status] = 'Current'";
            this.processDriverNonDoleAjustmentsDeduction.Caption = "Process Adjustments/Deduction";
            this.processDriverNonDoleAjustmentsDeduction.Execute += new
            SimpleActionExecuteEventHandler(
            processDriverPayroll_Execute);
        }

        private void processDriverPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as DriverPayrollBatch2;

            ObjectSpace.CommitChanges();

            if (_DriverPayrollBatch.DriverPayrolls2.Count==0)
            {
                throw new ApplicationException("There are no records to process.");
            }

            _FrmProgress = new ProgressForm("Processing data...", _DriverPayrollBatch.DriverPayrolls2.Count,
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
            _BgWorker.RunWorkerAsync(_DriverPayrollBatch.DriverPayrolls2);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList<DriverPayroll2> _included = (IList<DriverPayroll2>)e.Argument;
            try
            {
                DriverPayrollBatch2 dpb = session.GetObjectByKey<DriverPayrollBatch2>(_DriverPayrollBatch.Oid);
                foreach (var item in _included)
                {
                    index++;
                    _message = string.Format("Processing {0} succesfull.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    DriverPayroll2 dpr2 = session.GetObjectByKey<DriverPayroll2>(item.Oid);
                    #region Incentives here...
                    if (dpr2.Employee.EmpIncentives.Count > 0)
                    {
                        foreach (var inc in dpr2.Employee.EmpIncentives)
                        {
                            if (inc.OnlyInBatch != null && inc.OnlyInBatch != dpb.BatchType)
                            {
                                continue;
                            }
                            string lid = string.Format("0{0}-{1}", dpr2.Oid, inc.Oid);
                            DriverPayrollAdjustment dpa = session.FindObject<DriverPayrollAdjustment>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (dpa == null)
                            {
                                dpa = ReflectionHelper.CreateObject<DriverPayrollAdjustment>(session);
                                dpa.LineID = lid;
                                dpa.Include = true;
                                dpa.DriverPayrollID = dpr2;
                                dpa.Amount = inc.Amount;
                            }
                            //dpa.DriverPayrollID = dpr2;
                            dpa.Employee = dpr2.Employee;
                            dpa.AdjustmentType = inc.AdjustmentType;
                            dpa.Explanation = !string.IsNullOrEmpty(inc.Explanation) ? inc.Explanation : inc.AdjustmentType.Description;
                            dpa.AutoGenerated = true;
                            //if (AlterValue(dpa.Amount))
                            //{
                            //    dpa.Amount = inc.Amount;
                            //}
                            dpa.Save();
                        }
                    }
                    #endregion

                    #region Premiums here...
                    if (dpb.BatchType.IncludePremiums)
                    {
                        foreach (var prms in dpr2.Employee.EmployeePremiums)
                        {
                            string lid = string.Format("1{0}-{1}", dpr2.Oid, prms.Oid);
                            DriverPayrollDeduction dpd = session.FindObject<DriverPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (dpd == null)
                            {
                                dpd = ReflectionHelper.CreateObject<DriverPayrollDeduction>(session);
                                dpd.LineID = lid;
                                dpd.DedId = prms.Oid;
                                dpd.Include = true;
                                if (dpb.PeriodStart.Month == 1)
                                    dpd.Month = MonthsEnum.January;
                                if (dpb.PeriodStart.Month == 2)
                                    dpd.Month = MonthsEnum.February;
                                if (dpb.PeriodStart.Month == 3)
                                    dpd.Month = MonthsEnum.March;
                                if (dpb.PeriodStart.Month == 4)
                                    dpd.Month = MonthsEnum.April;
                                if (dpb.PeriodStart.Month == 5)
                                    dpd.Month = MonthsEnum.May;
                                if (dpb.PeriodStart.Month == 6)
                                    dpd.Month = MonthsEnum.June;
                                if (dpb.PeriodStart.Month == 7)
                                    dpd.Month = MonthsEnum.July;
                                if (dpb.PeriodStart.Month == 8)
                                    dpd.Month = MonthsEnum.August;
                                if (dpb.PeriodStart.Month == 9)
                                    dpd.Month = MonthsEnum.September;
                                if (dpb.PeriodStart.Month == 10)
                                    dpd.Month = MonthsEnum.October;
                                if (dpb.PeriodStart.Month == 11)
                                    dpd.Month = MonthsEnum.November;
                                if (dpb.PeriodStart.Month == 12)
                                    dpd.Month = MonthsEnum.December;
                                dpd.DriverPayrollID = dpr2;
                                dpd.Amount = prms.Amount;
                            }
                            dpd.Employee = dpr2.Employee;
                            dpd.DeductionType = DeductionType.Premium;
                            dpd.DeductionName = prms.PremiumCode.Description;
                            
                            dpd.Caption = string.Format("{0}|{1}", prms.PremiumCode.Description, dpd.MonthStr);
                            dpd.SummaryCaption = prms.PremiumCode.Caption;
                            //if (AlterValue(dpd.Amount))
                            //{
                            //    dpd.Amount = prms.Amount;
                            //}
                            dpd.Save();
                        }
                    }
                    #endregion

                    #region EveryPayroll here...

                    foreach (var lns in dpr2.Employee.EmpLoans)
                    {
                        if (!lns.LoanCode.EveryPayrollDeduction)
                        {
                            continue;
                        }
                        if (lns.Paid)
                        {
                            continue;
                        }
                        string lid = string.Format("2{0}-{1}", dpr2.Oid, lns.Oid);
                        DriverPayrollDeduction dpd = session.FindObject<DriverPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                        if (dpd == null)
                        {
                            dpd = ReflectionHelper.CreateObject<DriverPayrollDeduction>(session);
                            dpd.LineID = lid;
                            dpd.DedId = lns.Oid;
                            dpd.Include = true;
                            if (dpb.PeriodStart.Month == 1)
                                dpd.Month = MonthsEnum.January;
                            if (dpb.PeriodStart.Month == 2)
                                dpd.Month = MonthsEnum.February;
                            if (dpb.PeriodStart.Month == 3)
                                dpd.Month = MonthsEnum.March;
                            if (dpb.PeriodStart.Month == 4)
                                dpd.Month = MonthsEnum.April;
                            if (dpb.PeriodStart.Month == 5)
                                dpd.Month = MonthsEnum.May;
                            if (dpb.PeriodStart.Month == 6)
                                dpd.Month = MonthsEnum.June;
                            if (dpb.PeriodStart.Month == 7)
                                dpd.Month = MonthsEnum.July;
                            if (dpb.PeriodStart.Month == 8)
                                dpd.Month = MonthsEnum.August;
                            if (dpb.PeriodStart.Month == 9)
                                dpd.Month = MonthsEnum.September;
                            if (dpb.PeriodStart.Month == 10)
                                dpd.Month = MonthsEnum.October;
                            if (dpb.PeriodStart.Month == 11)
                                dpd.Month = MonthsEnum.November;
                            if (dpb.PeriodStart.Month == 12)
                                dpd.Month = MonthsEnum.December;
                            dpd.DriverPayrollID = dpr2;
                            //dpd.Include = true;
                            if (lns.LoanBalance < lns.Amortization)
                            {
                                if (AlterValue(dpd.Amount))
                                {
                                    dpd.Amount = lns.LoanBalance;
                                }
                                //spd.Amount = lns.LoanBalance;
                            }
                            else
                            {
                                if (AlterValue(dpd.Amount))
                                {
                                    dpd.Amount = lns.Amortization;
                                }
                                //spd.Amount = lns.Amortization;
                            }
                        }
                        dpd.Employee = dpr2.Employee;
                        dpd.DeductionType = DeductionType.Loan;
                        dpd.DeductionName = lns.LoanCode.Description;
                        dpd.Caption = string.Format("{0}|{1}", lns.LoanCode.Code, lns.RefNo);
                        dpd.SummaryCaption = lns.LoanCode.Caption;
                        //if (lns.LoanBalance < lns.Amortization)
                        //{
                        //    if (AlterValue(dpd.Amount))
                        //    {
                        //        dpd.Amount = lns.LoanBalance;
                        //    }
                        //    //spd.Amount = lns.LoanBalance;
                        //}
                        //else
                        //{
                        //    if (AlterValue(dpd.Amount))
                        //    {
                        //        dpd.Amount = lns.Amortization;
                        //    }
                        //    //spd.Amount = lns.Amortization;
                        //}
                        dpd.RefNo = lns.RefNo;
                        dpd.Balance = lns.LoanBalance;
                        dpd.Save();
                    }
                    #endregion

                    #region Loans here...
                    if (dpb.BatchType.IncludeLoans)
                    {
                        foreach (var lns in dpr2.Employee.EmpLoans)
                        {
                            if (lns.LoanCode.EveryPayrollDeduction)
                            {
                                continue;
                            }
                            if (lns.Paid)
                            {
                                continue;
                            }
                            string lid = string.Format("2{0}-{1}", dpr2.Oid, lns.Oid);
                            DriverPayrollDeduction dpd = session.FindObject<DriverPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (dpd == null)
                            {
                                dpd = ReflectionHelper.CreateObject<DriverPayrollDeduction>(session);
                                dpd.LineID = lid;
                                dpd.DedId = lns.Oid;
                                dpd.Include = true;
                                if (dpb.PeriodStart.Month == 1)
                                    dpd.Month = MonthsEnum.January;
                                if (dpb.PeriodStart.Month == 2)
                                    dpd.Month = MonthsEnum.February;
                                if (dpb.PeriodStart.Month == 3)
                                    dpd.Month = MonthsEnum.March;
                                if (dpb.PeriodStart.Month == 4)
                                    dpd.Month = MonthsEnum.April;
                                if (dpb.PeriodStart.Month == 5)
                                    dpd.Month = MonthsEnum.May;
                                if (dpb.PeriodStart.Month == 6)
                                    dpd.Month = MonthsEnum.June;
                                if (dpb.PeriodStart.Month == 7)
                                    dpd.Month = MonthsEnum.July;
                                if (dpb.PeriodStart.Month == 8)
                                    dpd.Month = MonthsEnum.August;
                                if (dpb.PeriodStart.Month == 9)
                                    dpd.Month = MonthsEnum.September;
                                if (dpb.PeriodStart.Month == 10)
                                    dpd.Month = MonthsEnum.October;
                                if (dpb.PeriodStart.Month == 11)
                                    dpd.Month = MonthsEnum.November;
                                if (dpb.PeriodStart.Month == 12)
                                    dpd.Month = MonthsEnum.December;
                                dpd.DriverPayrollID = dpr2;
                                //dpd.Include = true;
                                if (lns.LoanBalance < lns.Amortization)
                                {
                                    if (AlterValue(dpd.Amount))
                                    {
                                        dpd.Amount = lns.LoanBalance;
                                    }
                                    //spd.Amount = lns.LoanBalance;
                                }
                                else
                                {
                                    if (AlterValue(dpd.Amount))
                                    {
                                        dpd.Amount = lns.Amortization;
                                    }
                                    //spd.Amount = lns.Amortization;
                                }
                            }
                            dpd.Employee = dpr2.Employee;
                            dpd.DeductionType = DeductionType.Loan;
                            dpd.DeductionName = lns.LoanCode.Description;
                            dpd.Caption = string.Format("{0}|{1}", lns.LoanCode.Code, lns.RefNo);
                            dpd.SummaryCaption = lns.LoanCode.Caption;
                            //if (lns.LoanBalance < lns.Amortization)
                            //{
                            //    if (AlterValue(dpd.Amount))
                            //    {
                            //        dpd.Amount = lns.LoanBalance;
                            //    }
                            //    //spd.Amount = lns.LoanBalance;
                            //}
                            //else
                            //{
                            //    if (AlterValue(dpd.Amount))
                            //    {
                            //        dpd.Amount = lns.Amortization;
                            //    }
                            //    //spd.Amount = lns.Amortization;
                            //}
                            dpd.RefNo = lns.RefNo;
                            dpd.Balance = lns.LoanBalance;
                            dpd.Save();
                        }
                    }
                    #endregion

                    #region Taxes here...
                    if (dpb.BatchType.IncludeTax)
                    {
                        foreach (var lns in dpr2.Employee.EmpTaxs)
                        {
                            if (lns.Paid)
                            {
                                continue;
                            }
                            string lid = string.Format("3{0}-{1}", dpr2.Oid, lns.Oid);
                            DriverPayrollDeduction dpd = session.FindObject<DriverPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (dpd == null)
                            {
                                dpd = ReflectionHelper.CreateObject<DriverPayrollDeduction>(session);
                                dpd.LineID = lid;
                                dpd.DedId = lns.Oid;
                                dpd.Include = true;
                                if (dpb.PeriodStart.Month == 1)
                                    dpd.Month = MonthsEnum.January;
                                if (dpb.PeriodStart.Month == 2)
                                    dpd.Month = MonthsEnum.February;
                                if (dpb.PeriodStart.Month == 3)
                                    dpd.Month = MonthsEnum.March;
                                if (dpb.PeriodStart.Month == 4)
                                    dpd.Month = MonthsEnum.April;
                                if (dpb.PeriodStart.Month == 5)
                                    dpd.Month = MonthsEnum.May;
                                if (dpb.PeriodStart.Month == 6)
                                    dpd.Month = MonthsEnum.June;
                                if (dpb.PeriodStart.Month == 7)
                                    dpd.Month = MonthsEnum.July;
                                if (dpb.PeriodStart.Month == 8)
                                    dpd.Month = MonthsEnum.August;
                                if (dpb.PeriodStart.Month == 9)
                                    dpd.Month = MonthsEnum.September;
                                if (dpb.PeriodStart.Month == 10)
                                    dpd.Month = MonthsEnum.October;
                                if (dpb.PeriodStart.Month == 11)
                                    dpd.Month = MonthsEnum.November;
                                if (dpb.PeriodStart.Month == 12)
                                    dpd.Month = MonthsEnum.December;
                                dpd.DriverPayrollID = dpr2;
                                if (lns.TaxBalance < lns.Deduction)
                                {
                                    if (AlterValue(dpd.Amount))
                                    {
                                        dpd.Amount = lns.TaxBalance;
                                    }
                                    //spd.Amount = lns.TaxBalance;
                                }
                                else
                                {
                                    if (AlterValue(dpd.Amount))
                                    {
                                        dpd.Amount = lns.Deduction;
                                    }
                                    //spd.Amount = lns.Deduction;
                                }
                            }
                            dpd.Employee = dpr2.Employee;
                            dpd.DeductionType = DeductionType.Tax;
                            dpd.DeductionName = lns.TaxCode.Description;
                            dpd.Caption = string.Format("{0}|{1}", lns.TaxCode.Description, dpd.MonthStr);
                            dpd.SummaryCaption = lns.TaxCode.Caption;
                            //if (lns.TaxBalance < lns.Deduction)
                            //{
                            //    if (AlterValue(dpd.Amount))
                            //    {
                            //        dpd.Amount = lns.TaxBalance;
                            //    }
                            //    //spd.Amount = lns.TaxBalance;
                            //}
                            //else
                            //{
                            //    if (AlterValue(dpd.Amount))
                            //    {
                            //        dpd.Amount = lns.Deduction;
                            //    }
                            //    //spd.Amount = lns.Deduction;
                            //}
                            dpd.RefNo = lns.Year.ToString();
                            dpd.Balance = lns.TaxBalance;
                            dpd.Save();
                        }
                    }
                    #endregion

                    #region Other Deduction

                    if (dpb.BatchType.IncludeOtherDed)
                    {
                        foreach (var lns in dpr2.Employee.EmpOtherDeds)
                        {
                            if (lns.Paid)
                            {
                                continue;
                            }
                            string lid = string.Format("4{0}-{1}", dpr2.Oid, lns.Oid);
                            DriverPayrollDeduction dpd = session.FindObject<DriverPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                            if (dpd == null)
                            {
                                dpd = ReflectionHelper.CreateObject<DriverPayrollDeduction>(session);
                                dpd.LineID = lid;
                                dpd.DedId = lns.Oid;
                                dpd.Include = true;
                                if (dpb.PeriodStart.Month == 1)
                                    dpd.Month = MonthsEnum.January;
                                if (dpb.PeriodStart.Month == 2)
                                    dpd.Month = MonthsEnum.February;
                                if (dpb.PeriodStart.Month == 3)
                                    dpd.Month = MonthsEnum.March;
                                if (dpb.PeriodStart.Month == 4)
                                    dpd.Month = MonthsEnum.April;
                                if (dpb.PeriodStart.Month == 5)
                                    dpd.Month = MonthsEnum.May;
                                if (dpb.PeriodStart.Month == 6)
                                    dpd.Month = MonthsEnum.June;
                                if (dpb.PeriodStart.Month == 7)
                                    dpd.Month = MonthsEnum.July;
                                if (dpb.PeriodStart.Month == 8)
                                    dpd.Month = MonthsEnum.August;
                                if (dpb.PeriodStart.Month == 9)
                                    dpd.Month = MonthsEnum.September;
                                if (dpb.PeriodStart.Month == 10)
                                    dpd.Month = MonthsEnum.October;
                                if (dpb.PeriodStart.Month == 11)
                                    dpd.Month = MonthsEnum.November;
                                if (dpb.PeriodStart.Month == 12)
                                    dpd.Month = MonthsEnum.December;
                                dpd.DriverPayrollID = dpr2;
                                //dpd.Include = true;
                                if (lns.Balance < lns.Deduction)
                                {
                                    if (AlterValue(dpd.Amount))
                                    {
                                        dpd.Amount = lns.Balance;
                                    }
                                    //spd.Amount = lns.Balance;
                                }
                                else
                                {
                                    if (AlterValue(dpd.Amount))
                                    {
                                        dpd.Amount = lns.Deduction;
                                    }
                                    //spd.Amount = lns.Deduction;
                                }
                            }
                            dpd.Employee = dpr2.Employee;
                            dpd.DeductionType = DeductionType.Other;
                            if (!string.IsNullOrEmpty(lns.Explanation))
                            {
                                dpd.DeductionName = lns.DedCode.Code + " | " + lns.Explanation;
                                dpd.Caption = string.Format("{0}|{1}|{2}", lns.DedCode.Description, lns.EntryDate.ToShortDateString(), lns.RefNo);
                                //spd.Caption = string.Format("{0}|{1}|{2}-{3}", lns.DedCode.Code, lns.EntryDate.ToShortDateString(), lns.RefNo, lns.Explanation);
                            }
                            else
                            {
                                dpd.DeductionName = lns.DedCode.Description;
                                dpd.Caption = string.Format("{0}|{1}|{2}", lns.DedCode.Description, lns.EntryDate.ToShortDateString(), lns.RefNo);
                            }
                            
                            //if (lns.Balance < lns.Deduction)
                            //{
                            //    if (AlterValue(dpd.Amount))
                            //    {
                            //        dpd.Amount = lns.Balance;
                            //    }
                            //    //spd.Amount = lns.Balance;
                            //}
                            //else
                            //{
                            //    if (AlterValue(dpd.Amount))
                            //    {
                            //        dpd.Amount = lns.Deduction;
                            //    }
                            //    //spd.Amount = lns.Deduction;
                            //}
                            dpd.SummaryCaption = lns.DedCode.Caption;
                            dpd.RefNo = lns.RefNo;
                            dpd.Balance = lns.Balance;
                            dpd.Save();
                        }
                    }
                    #endregion

                    dpb.Save();
                    CommitUpdatingSession(session);

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
                if (index == _included.Count)
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
                    "Processing of adjustments and deductions is cancelled.", "Cancelled",
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
                    "Adjustments and deductions has been successfully processed.");
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
