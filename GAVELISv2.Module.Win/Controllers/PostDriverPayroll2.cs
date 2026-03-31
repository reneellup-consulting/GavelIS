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
    public partial class PostDriverPayroll2 : ViewController
    {
        private SimpleAction postDriverPayroll;
        private DriverPayrollBatch2 _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public PostDriverPayroll2()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch2.PostDriverPayroll2";
            this.postDriverPayroll = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.postDriverPayroll.TargetObjectsCriteria = "[Status] = 'Released'";
            this.postDriverPayroll.Caption = "Post Payroll";
            this.postDriverPayroll.Execute += new
            SimpleActionExecuteEventHandler(
            postDriverPayroll_Execute);
        }
        private void postDriverPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch2;
            // _StaffPayrollBatch.Status = PayrollBatchStatusEnum.Released
            if (_DriverPayrollBatch.BatchType.SalariesAndWagesAccount == null)
            {
                throw new UserFriendlyException("Must provide Salaries and Wages Account in Batch Type.");
            }
            if (_DriverPayrollBatch.BatchType.CashInBankAccount == null)
            {
                throw new UserFriendlyException("Must provide Net Payroll Payable Account in Batch Type.");
            }
            ObjectSpace.CommitChanges();
            if (_DriverPayrollBatch.DriverPayrolls2.Count == 0)
            {
                throw new UserFriendlyException("There are no Driver Payroll data to post");
            }

            _FrmProgress = new ProgressForm("Posting data...", _DriverPayrollBatch.DriverPayrolls2.Count,
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
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<DriverPayroll2> args = (XPCollection<DriverPayroll2>)e.Argument;
            DriverPayrollBatch2 batch = session.GetObjectByKey<DriverPayrollBatch2>(_DriverPayrollBatch.Oid);
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal netpay = 0m;
            try
            {
                foreach (var item in args)
                {
                    index++;
                    _message = string.Format("Posting {0} successful.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    DriverPayroll2 pr = session.GetObjectByKey<DriverPayroll2>(item.Oid);
                    if (pr.BatchInfo.BatchType.TaggedFuelRequired)
                    {
                        foreach (var dr in pr.DriverPayrollTripLines)
                        {
                            throw new ApplicationException(string.Format("Driver {0} DR Doc# {1} has no tagged fuel receipts", pr.Employee.No, dr.DocumentNo));
                        }
                        
                    }
                    netpay += pr.NetPay;
                    if (pr.GrossPay > 0m)
                    {
                        int[] inds = null;
                        inds = accounts.Find2("Account", batch.BatchType.SalariesAndWagesAccount);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.DebitAmount += pr.GrossPay - pr.AdjustmentsAmt.Value;
                        }
                        else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = batch.BatchType.SalariesAndWagesAccount;
                            tmpAccount.DebitAmount += pr.GrossPay - pr.AdjustmentsAmt.Value;
                            accounts.Add(tmpAccount);
                        }

                        #region Adjustments
                        if (pr.DriverPayrollAdjustments.Count > 0)
                        {
                            foreach (var adj in pr.DriverPayrollAdjustments)
                            {
                                if (adj.Include != true)
                                {
                                    continue;
                                }
                                int[] inds1 = null;
                                Account acc1 = null;
                                if (adj.AdjustmentType != null && adj.AdjustmentType.PayAdjustmentAccount != null)
                                {
                                    inds1 = accounts.Find2("Account", adj.AdjustmentType.PayAdjustmentAccount);
                                    acc1 = adj.AdjustmentType.PayAdjustmentAccount;
                                }
                                else
                                {
                                    inds1 = accounts.Find2("Account", batch.BatchType.SalariesAndWagesAccount);
                                    acc1 = batch.BatchType.SalariesAndWagesAccount;
                                }

                                if (inds1 != null && inds1.Length > 0)
                                {
                                    tmpAccount = accounts[inds1[0]];
                                    tmpAccount.DebitAmount += adj.Amount;
                                    //tmpAccount.CreditAmount += adj.Amount < 0 ? Math.Abs(adj.Amount) : 0;
                                }
                                else
                                {
                                    tmpAccount = new TempAccount();
                                    tmpAccount.Account = acc1;
                                    tmpAccount.DebitAmount += adj.Amount;
                                    //tmpAccount.CreditAmount += adj.Amount < 0 ? Math.Abs(adj.Amount) : 0;
                                    accounts.Add(tmpAccount);
                                }
                            }
                        }
                        #endregion

                        #region Deductions
                        if (pr.DriverPayrollDeductions.Count > 0)
                        {
                            foreach (var ded in pr.DriverPayrollDeductions)
                            {
                                if (ded.Include != true)
                                {
                                    continue;
                                }
                                int[] inds1 = null;
                                Account acc1 = null;
                                switch (ded.DeductionType)
                                {
                                    case DeductionType.Premium:
                                        // PremiumCode
                                        EmployeePremium ep = session.FindObject<EmployeePremium>(CriteriaOperator.Parse("[Oid]=?", ded.DedId));
                                        acc1 = ep.PremiumCode.PremiumAccount;
                                        break;
                                    case DeductionType.Loan:
                                        // LoanCode
                                        EmpLoan el = session.FindObject<EmpLoan>(CriteriaOperator.Parse("[Oid]=?", ded.DedId));
                                        if (el == null)
                                        {
                                            throw new UserFriendlyException(string.Format("Ded ID: {0} not found from Emp: {1}", ded.DedId, ded.Employee.No));
                                        }
                                        acc1 = el.LoanCode.EmployeeLoanAccount;
                                        if (el.PayBegin == DateTime.MinValue)
                                        {
                                            el.PayBegin = batch.PayrollDate;
                                            decimal currLoanBal = el.LoanBalance;
                                            decimal tmpBalance = currLoanBal - ded.Amount;
                                            if (tmpBalance < 0)
                                            {
                                                throw new UserFriendlyException(string.Format("Over deduction on Line ID: {0} - {1}", ded.LineID, ded.Employee.No));
                                            }
                                            else if (tmpBalance == 0)
                                            {
                                                el.PayEnd = batch.PayrollDate;
                                                el.Paid = true;
                                            }
                                            el.LoanBalance = tmpBalance;
                                            el.Save();
                                        }
                                        else
                                        {
                                            decimal currLoanBal = el.LoanBalance;
                                            decimal tmpBalance = currLoanBal - ded.Amount;
                                            if (tmpBalance < 0)
                                            {
                                                throw new UserFriendlyException(string.Format("Over deduction on Line ID: {0} - {1}", ded.LineID, ded.Employee.No));
                                            }
                                            else if (tmpBalance == 0)
                                            {
                                                el.PayEnd = batch.PayrollDate;
                                                el.Paid = true;
                                            }
                                            el.LoanBalance = tmpBalance;
                                            el.Save();
                                        }
                                        break;
                                    case DeductionType.Tax:
                                        // TaxCode
                                        EmpTax et = session.FindObject<EmpTax>(CriteriaOperator.Parse("[Oid]=?", ded.DedId));
                                        if (et == null)
                                        {
                                            throw new UserFriendlyException(string.Format("Ded ID: {0} not found from Emp: {1}", ded.DedId, ded.Employee.No));
                                        }
                                        acc1 = et.TaxCode.EmployeeTaxCodeAccount;
                                        if (et.PayBegin == DateTime.MinValue)
                                        {
                                            et.PayBegin = batch.PayrollDate;
                                            decimal currTaxBal = et.TaxBalance;
                                            decimal tmpBalance = currTaxBal - ded.Amount;
                                            if (tmpBalance < 0)
                                            {
                                                throw new UserFriendlyException(string.Format("Over deduction on Line ID: {0} - {1}", ded.LineID, ded.Employee.No));
                                            }
                                            else if (tmpBalance == 0)
                                            {
                                                et.PayEnd = batch.PayrollDate;
                                                et.Paid = true;
                                            }
                                            et.TaxBalance = tmpBalance;
                                            et.Save();
                                        }
                                        else
                                        {
                                            decimal currTaxBal = et.TaxBalance;
                                            decimal tmpBalance = currTaxBal - ded.Amount;
                                            if (tmpBalance < 0)
                                            {
                                                throw new UserFriendlyException(string.Format("Over deduction on Line ID: {0} - {1}", ded.LineID, ded.Employee.No));
                                            }
                                            else if (tmpBalance == 0)
                                            {
                                                et.PayEnd = batch.PayrollDate;
                                                et.Paid = true;
                                            }
                                            et.TaxBalance = tmpBalance;
                                            et.Save();
                                        }
                                        break;
                                    case DeductionType.Other:
                                        // DedCode
                                        EmpOtherDed eo = session.FindObject<EmpOtherDed>(CriteriaOperator.Parse("[Oid]=?", ded.DedId));
                                        if (eo == null)
                                        {
                                            throw new UserFriendlyException(string.Format("Ded ID: {0} not found from Emp: {1}", ded.DedId, ded.Employee.No));
                                        }
                                        acc1 = eo.DedCode.OtherDeductionAccount;
                                        if (eo.PayBegin == DateTime.MinValue)
                                        {
                                            eo.PayBegin = batch.PayrollDate;
                                            decimal pdamnt = eo.PaidAmount + ded.Amount;
                                            if (pdamnt > eo.Amount)
                                            {
                                                throw new UserFriendlyException(string.Format("Over deduction on Line ID: {0} - {1}", ded.LineID, ded.Employee.No));
                                            }
                                            else if (pdamnt == eo.Amount)
                                            {
                                                eo.PayEnd = batch.PayrollDate;
                                                eo.Paid = true;
                                            }
                                            eo.PaidAmount = pdamnt;
                                            eo.Save();
                                        }
                                        else
                                        {
                                            decimal pdamnt = eo.PaidAmount + ded.Amount;
                                            if (pdamnt > eo.Amount)
                                            {
                                                throw new UserFriendlyException(string.Format("Over deduction on Line ID: {0} - {1}", ded.LineID, ded.Employee.No));
                                            }
                                            else if (pdamnt == eo.Amount)
                                            {
                                                eo.PayEnd = batch.PayrollDate;
                                                eo.Paid = true;
                                            }
                                            eo.PaidAmount = pdamnt;
                                            eo.Save();
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                inds1 = accounts.Find2("Account", acc1);
                                if (inds1 != null && inds1.Length > 0)
                                {
                                    tmpAccount = accounts[inds1[0]];
                                    //tmpAccount.DebitAmount += ded.Amount > 0 ? Math.Abs(ded.Amount) : 0;
                                    tmpAccount.CreditAmount += ded.Amount;
                                }
                                else
                                {
                                    tmpAccount = new TempAccount();
                                    tmpAccount.Account = acc1;
                                    //tmpAccount.DebitAmount += ded.Amount > 0 ? Math.Abs(ded.Amount) : 0;
                                    tmpAccount.CreditAmount += ded.Amount;
                                    accounts.Add(tmpAccount);
                                }
                            }
                        }
                        #endregion
                    }
                    //pr.AttRecId.Posted = true;
                    foreach (var trip in pr.DriverPayrollTripLines)
                    {
                        if (trip.DriverRegistryId == null)
                        {
                            continue;
                        }
                        trip.DriverRegistryId.CommissionPaidActual = trip.Commission;
                        trip.DriverRegistryId.Status = DriverRegistryStatusEnum.Paid;
                        trip.Save();
                    }
                    pr.Posted = true;
                    pr.Save();

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
                if (index == args.Count)
                {
                    foreach (TempAccount acc in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = batch;
                        _gjd.Account = acc.Account;
                        _gjd.DebitAmount = acc.DebitAmount;
                        _gjd.CreditAmount = acc.CreditAmount;
                        _gjd.Description = "Driver Payroll " + string.Format("{0}-{1} '{2}", batch.PeriodStart.ToString("MM/dd"), batch.PeriodEnd.ToString("MM/dd"), batch.PeriodEnd.Year);
                        //_gjd.SubAccountNo = thisReceipt.Vendor;
                        //_gjd.SubAccountType = thisReceipt.Vendor.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }
                    // Net Payroll Payable
                    GenJournalDetail _gjd2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                    _gjd2.GenJournalID = batch;
                    _gjd2.Account = batch.BatchType.CashInBankAccount;
                    _gjd2.CreditAmount = netpay;
                    _gjd2.Description = "Driver Payroll " + string.Format("{0}-{1} '{2}", batch.PeriodStart.ToString("MM/dd"), batch.PeriodEnd.ToString("MM/dd"), batch.PeriodEnd.Year);
                    //_gjd.SubAccountNo = thisReceipt.Vendor;
                    //_gjd.SubAccountType = thisReceipt.Vendor.ContactType;
                    _gjd2.Approved = true;
                    creditAmount = creditAmount + _gjd2.CreditAmount;
                    _gjd2.Save();
                    batch.Status = PayrollBatchStatusEnum.Posted;
                    batch.Save();
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                            ApplicationException("Accounting entries not balance");
                    }
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
                    "Posting of payroll batch data is cancelled.", "Cancelled",
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
                    "Payroll batch data has been successfully posted.");
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
