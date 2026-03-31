using System;
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
    public partial class PostDriverPayroll : ViewController
    {
        private SimpleAction postDriverPayrollAction;
        private DriverPayrollBatch _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public PostDriverPayroll()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.DriverPayrollBatch";
            this.postDriverPayrollAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.postDriverPayrollAction.Execute += new
            SimpleActionExecuteEventHandler(
            PostDriverPayrollAction_Execute);
        }
        private void PostDriverPayrollAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch;

            ObjectSpace.CommitChanges();
            if (_DriverPayrollBatch.DriverPayrolls.Count == 0)
            {
                throw new UserFriendlyException("There are no Driver's Payroll data to post");
            }

            _FrmProgress = new ProgressForm("Posting data...", _DriverPayrollBatch.DriverPayrolls.Count,
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
            _BgWorker.RunWorkerAsync(_DriverPayrollBatch);
            _FrmProgress.ShowDialog();

        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            DriverPayrollBatch _payrollBatch = (DriverPayrollBatch)e.Argument;
            DriverPayrollBatch thisIS = session.GetObjectByKey<DriverPayrollBatch>(
            _payrollBatch.Oid);
            int aCount = 0;
            try
            {
                aCount=thisIS.DriverPayrolls.Count;
                foreach (DriverPayroll item in thisIS.DriverPayrolls)
                {
                    foreach (var trip in thisIS.DriverPayrollTrips)
                    {
                        if (trip.Driver == item.Employee){
                            trip.RegID.BasicPaidActual = item.Basic;
                            trip.RegID.Status = DriverRegistryStatusEnum.Paid;
                            trip.Posted = true;
                            trip.Save();
                        }
                    }
                    // Debit 505900 - Salaries and Wages - BatchType.SalariesAndWagesAccount
                    if (item.Basic>0 && thisIS.BatchType.SalariesAndWagesAccount==null)
                        throw new UserFriendlyException("Must provide Salaries and Wages Account in Batch Type.");
                    if (item.Basic>0)
                    {
                        GenJournalDetail _gjde01 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde01.GenJournalID = thisIS;
                        _gjde01.GenJournalID.Approved = true;
                        _gjde01.Account = thisIS.BatchType.SalariesAndWagesAccount;
                        _gjde01.DebitAmount = Math.Abs(item.Basic);
                        debitAmount += _gjde01.DebitAmount;
                        _gjde01.Description = "Salaries and Wages " + thisIS.ReferenceNo;
                        _gjde01.SubAccountNo = item.Employee;
                        _gjde01.SubAccountType = item.Employee.ContactType;
                        _gjde01.Approved = true;
                        _gjde01.Save();
                    }
                    // Debit 506000 - Driver Misc. Expense - BatchType.DriversMiscExpenseAccount
                    if (item.MiscExp > 0 && thisIS.BatchType.DriversMiscExpenseAccount == null)
                        throw new UserFriendlyException("Must provide Driver's Misc. Expense Account in Batch Type.");
                    if (item.MiscExp > 0)
                    {
                        GenJournalDetail _gjde02 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde02.GenJournalID = thisIS;
                        _gjde02.GenJournalID.Approved = true;
                        _gjde02.Account = thisIS.BatchType.DriversMiscExpenseAccount;
                        _gjde02.DebitAmount = Math.Abs(item.MiscExp);
                        debitAmount += _gjde02.DebitAmount;
                        _gjde02.Description = "Driver's Misc. Expense " + thisIS.ReferenceNo;
                        _gjde02.SubAccountNo = item.Employee;
                        _gjde02.SubAccountType = item.Employee.ContactType;
                        _gjde02.Approved = true;
                        _gjde02.Save();
                    }
                        // Debit 506010 - Add'l Misc. Expense - BatchType.AddlMiscExpenseAccount
                    if (item.AdlMiscExp > 0 && thisIS.BatchType.AddlMiscExpenseAccount == null)
                        throw new UserFriendlyException("Must provide Add'l Misc. Expense Account in Batch Type.");
                    if (item.AdlMiscExp > 0)
                    {
                        GenJournalDetail _gjde03 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde03.GenJournalID = thisIS;
                        _gjde03.GenJournalID.Approved = true;
                        _gjde03.Account = thisIS.BatchType.AddlMiscExpenseAccount;
                        _gjde03.DebitAmount = Math.Abs(item.AdlMiscExp);
                        debitAmount += _gjde03.DebitAmount;
                        _gjde03.Description = "Add'l Misc. Expense " + thisIS.ReferenceNo;
                        _gjde03.SubAccountNo = item.Employee;
                        _gjde03.SubAccountType = item.Employee.ContactType;
                        _gjde03.Approved = true;
                        _gjde03.Save();
                    }
                        // Debit 506020 - KD's - BatchType.KDsPayrollAccount
                    if (item.KDs > 0 && thisIS.BatchType.KDsPayrollAccount == null)
                        throw new UserFriendlyException("Must provide KD's Payroll Account in Batch Type.");
                    if (item.KDs > 0)
                    {
                        GenJournalDetail _gjde04 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde04.GenJournalID = thisIS;
                        _gjde04.GenJournalID.Approved = true;
                        _gjde04.Account = thisIS.BatchType.KDsPayrollAccount;
                        _gjde04.DebitAmount = Math.Abs(item.KDs);
                        debitAmount += _gjde04.DebitAmount;
                        _gjde04.Description = "KD's Payroll " + thisIS.ReferenceNo;
                        _gjde04.SubAccountNo = item.Employee;
                        _gjde04.SubAccountType = item.Employee.ContactType;
                        _gjde04.Approved = true;
                        _gjde04.Save();
                    }
                        // Debit 506030 - Shunting - BatchType.ShuntingPayrollAccount
                    if (item.Shunting > 0 && thisIS.BatchType.ShuntingPayrollAccount == null)
                        throw new UserFriendlyException("Must provide Shunting Payroll Account in Batch Type.");
                    if (item.Shunting > 0)
                    {
                        GenJournalDetail _gjde05 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde05.GenJournalID = thisIS;
                        _gjde05.GenJournalID.Approved = true;
                        _gjde05.Account = thisIS.BatchType.ShuntingPayrollAccount;
                        _gjde05.DebitAmount = Math.Abs(item.Shunting);
                        debitAmount += _gjde05.DebitAmount;
                        _gjde05.Description = "Shunting Payroll " + thisIS.ReferenceNo;
                        _gjde05.SubAccountNo = item.Employee;
                        _gjde05.SubAccountType = item.Employee.ContactType;
                        _gjde05.Approved = true;
                        _gjde05.Save();
                    }

                        // For Each Adjustment to Debit : PayAdjustmentType.PayAdjustmentAccount;
                    foreach (var pa in thisIS.PayrollAdjustments)
                    {
                        if (pa.Employee == item.Employee)
                        {
                            GenJournalDetail _gjde06 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                            _gjde06.GenJournalID = thisIS;
                            _gjde06.GenJournalID.Approved = true;
                            _gjde06.Account = pa.AdjustmentType.PayAdjustmentAccount;
                            if (pa.Amount > 0)
                            {
                                _gjde06.DebitAmount = Math.Abs(pa.Amount);
                                debitAmount += _gjde06.DebitAmount;
                            }
                            else
                            {
                                _gjde06.CreditAmount = Math.Abs(pa.Amount);
                                creditAmount += _gjde06.CreditAmount;
                            }
                            _gjde06.Description = string.Format("Payroll Adjustment - {0} {1}", pa.Explanation, thisIS.ReferenceNo);
                            _gjde06.SubAccountNo = item.Employee;
                            _gjde06.SubAccountType = item.Employee.ContactType;
                            _gjde06.Approved = true;
                            _gjde06.Save();
                            pa.Posted = true;
                            pa.Save();
                        }
                    }

                        #region Deductions

                        foreach (PayrollDeduction pd in thisIS.PayrollDeductions)
                        {
                            if (pd.Employee == item.Employee && pd.Amount!=0)
                            {
                                switch (pd.DeductionType)
                                {
                                    case DeductionType.Premium:
                                        // Credit - 200200 SSS Payable
                                        // Credit - 200400 PH Payable
                                        // Credit - 200500 HDMF Payable
                                        // For Each Premium to Credit : Premium.PremiumAccount
                                        Premium prem = thisIS.Session.FindObject<Premium>(new BinaryOperator("Description", pd.DeductionName));
                                        if (prem==null)
                                            throw new UserFriendlyException("Must provide Premium.PremiumAccount");
                                            GenJournalDetail _gjde07 = ReflectionHelper.CreateObject<
                                            GenJournalDetail>(session);
                                            _gjde07.GenJournalID = thisIS;
                                            _gjde07.GenJournalID.Approved = true;
                                            _gjde07.Account = prem.PremiumAccount;
                                            _gjde07.CreditAmount = Math.Abs(pd.Amount);
                                            creditAmount += _gjde07.CreditAmount;
                                            _gjde07.Description = string.Format("Payroll Deduction - {0} {1}", pd.DeductionName, thisIS.ReferenceNo);
                                            _gjde07.SubAccountNo = item.Employee;
                                            _gjde07.SubAccountType = item.Employee.ContactType;
                                            _gjde07.Approved = true;
                                            _gjde07.Save();
                                        pd.Posted = true;
                                        pd.Save();
                                        break;
                                    case DeductionType.Loan:
                                        // Credit - 200200 SSS Payable
                                        // Credit - 200500 HDMF Payable
                                        // For Each Loan to Credit : EmployeeLoan.EmployeeLoanAccount
                                        EmployeeLoan el = thisIS.Session.FindObject<EmployeeLoan>(new BinaryOperator("Description", pd.DeductionName));
                                        if (el == null)
                                            throw new UserFriendlyException("Must provide EmployeeLoan.EmployeeLoanAccount");
                                            GenJournalDetail _gjde08 = ReflectionHelper.CreateObject<
                                            GenJournalDetail>(session);
                                            _gjde08.GenJournalID = thisIS;
                                            _gjde08.GenJournalID.Approved = true;
                                            _gjde08.Account = el.EmployeeLoanAccount;
                                            _gjde08.CreditAmount = Math.Abs(pd.Amount);
                                            creditAmount += _gjde08.CreditAmount;
                                            _gjde08.Description = string.Format("Payroll Deduction - {0} {1}", pd.DeductionName, thisIS.ReferenceNo);
                                            _gjde08.SubAccountNo = item.Employee;
                                            _gjde08.SubAccountType = item.Employee.ContactType;
                                            _gjde08.Approved = true;
                                            _gjde08.Save();
                                        pd.Posted = true;
                                        pd.Save();
                                        // Update Pay Begin
                                    foreach (EmpLoan empLn in item.Employee.EmpLoans)
                                    {
                                        if (empLn.LoanCode.Description == pd.DeductionName)
                                        {
                                            if (empLn.PayBegin == DateTime.MinValue)
                                            {
                                                empLn.PayBegin = thisIS.PayrollDate;
                                                decimal currLoanBal = empLn.LoanBalance;
                                                decimal tmpBalance = currLoanBal - pd.Amount;
                                                if (tmpBalance<0)
                                                {
                                                    throw new UserFriendlyException("Over deduction");
                                                }
                                                else if (tmpBalance == 0)
                                                {
                                                    empLn.PayEnd = thisIS.PayrollDate;
                                                    empLn.Paid = true;
                                                }
                                                empLn.LoanBalance = tmpBalance;
                                                empLn.Save();
                                                pd.Posted = true;
                                                pd.Save();
                                            }
                                            else
                                            {
                                                decimal currLoanBal = empLn.LoanBalance;
                                                decimal tmpBalance = currLoanBal - pd.Amount;
                                                if (tmpBalance < 0)
                                                {
                                                    throw new UserFriendlyException("Over deduction");
                                                }
                                                else if (tmpBalance == 0)
                                                {
                                                    empLn.PayEnd = thisIS.PayrollDate;
                                                    empLn.Paid = true;
                                                }
                                                empLn.LoanBalance = tmpBalance;
                                                empLn.Save();
                                                pd.Posted = true;
                                                pd.Save();
                                            }
                                        }
                                    }
                                        // Update Pay End
                                        // Update Paid
                                        // Update Balance
                                        // Mark as posted

                                        break;
                                    case DeductionType.Tax:
                                        // Credit - 200600 WH Tax
                                        // For Each Tax to Credit : EmployeeTax.EmployeeTaxCodeAccount
                                        EmployeeTax et = thisIS.Session.FindObject<EmployeeTax>(new BinaryOperator("Description", pd.DeductionName));
                                        if (et == null)
                                            throw new UserFriendlyException("Must provide EmployeeTax.EmployeeTaxCodeAccount");
                                            GenJournalDetail _gjde09 = ReflectionHelper.CreateObject<
                                            GenJournalDetail>(session);
                                            _gjde09.GenJournalID = thisIS;
                                            _gjde09.GenJournalID.Approved = true;
                                            _gjde09.Account = et.EmployeeTaxCodeAccount;
                                            _gjde09.CreditAmount = Math.Abs(pd.Amount);
                                            creditAmount += _gjde09.CreditAmount;
                                            _gjde09.Description = string.Format("Payroll Deduction - {0} {1}", pd.DeductionName, thisIS.ReferenceNo);
                                            _gjde09.SubAccountNo = item.Employee;
                                            _gjde09.SubAccountType = item.Employee.ContactType;
                                            _gjde09.Approved = true;
                                            _gjde09.Save();
                                        pd.Posted = true;
                                        pd.Save();
                                        // Update Pay Begin
                                    foreach (EmpTax empTx in item.Employee.EmpTaxs)
                                    {
                                        if (empTx.TaxCode.Description == pd.DeductionName)
                                        {
                                            if (empTx.PayBegin == DateTime.MinValue)
                                            {
                                                empTx.PayBegin = thisIS.PayrollDate;
                                                decimal currTaxBal = empTx.TaxBalance;
                                                decimal tmpBalance = currTaxBal - pd.Amount;
                                                if (tmpBalance<0)
                                                {
                                                    throw new UserFriendlyException("Over deduction");
                                                }
                                                else if (tmpBalance == 0)
                                                {
                                                    empTx.PayEnd = thisIS.PayrollDate;
                                                    empTx.Paid = true;
                                                }
                                                empTx.TaxBalance = tmpBalance;
                                                empTx.Save();
                                                pd.Posted = true;
                                                pd.Save();
                                            }
                                            else
                                            {
                                                decimal currTaxBal = empTx.TaxBalance;
                                                decimal tmpBalance = currTaxBal - pd.Amount;
                                                if (tmpBalance < 0)
                                                {
                                                    throw new UserFriendlyException("Over deduction");
                                                }
                                                else if (tmpBalance == 0)
                                                {
                                                    empTx.PayEnd = thisIS.PayrollDate;
                                                    empTx.Paid = true;
                                                }
                                                empTx.TaxBalance = tmpBalance;
                                                empTx.Save();
                                                pd.Posted = true;
                                                pd.Save();
                                            }
                                        }
                                    }
                                        break;
                                    case DeductionType.Other:
                                        // Credit - 101905 Advances to Employees & Officers - Cash
                                        // Credit - 101910 Advances to Employees & Officers - Item
                                        // Credit - 101911 Higas Genset
                                        // Credit - 101912 Higas Tractor
                                        // Credit - 101913 Tools
                                        // Credit - 101914 Damages
                                        // Credit - 101915 Others
                                        // Credit - 101920 Advances to Employees - Misc. Exp. - BatchType.AdvancesToEmpMiscExpAccount
                                        // Credit - 101925 Advances to Employees - Add'l Misc. Exp - BatchType.AdvancesToEmpAddlMiscExpAccount
                                        // For Each Other Deduction to Credit : OtherDeduction.OtherDeductionAccount
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }
                    #endregion
                    
                    #region Other Deductions
                        foreach (PayrollDeductionOther pdo in thisIS.PayrollDeductionOthers){
                            if (pdo.Employee == item.Employee && pdo.Amount != 0)
                            {
                                switch (pdo.DeductionType)
                                {
                                    case DeductionType.Premium:
                                        break;
                                    case DeductionType.Loan:
                                        break;
                                    case DeductionType.Tax:
                                        break;
                                    case DeductionType.Other:
                                        // Credit - 101905 Advances to Employees & Officers - Cash
                                        // Credit - 101910 Advances to Employees & Officers - Item
                                        // Credit - 101911 Higas Genset
                                        // Credit - 101912 Higas Tractor
                                        // Credit - 101913 Tools
                                        // Credit - 101914 Damages
                                        // Credit - 101915 Others
                                        // Credit - 101920 Advances to Employees - Misc. Exp. - BatchType.AdvancesToEmpMiscExpAccount
                                        // Credit - 101925 Advances to Employees - Add'l Misc. Exp - BatchType.AdvancesToEmpAddlMiscExpAccount
                                        // For Each Other Deduction to Credit : OtherDeduction.OtherDeductionAccount
                                        OtherDeduction od = thisIS.Session.FindObject<OtherDeduction>(new BinaryOperator("DisplayName", pdo.DeductionCode.DisplayName));
                                        if (od == null)
                                            throw new UserFriendlyException("Must provide OtherDeduction.OtherDeductionAccount");
                                            GenJournalDetail _gjde10 = ReflectionHelper.CreateObject<
                                            GenJournalDetail>(session);
                                            _gjde10.GenJournalID = thisIS;
                                            _gjde10.GenJournalID.Approved = true;
                                            _gjde10.Account = od.OtherDeductionAccount;
                                            _gjde10.CreditAmount = Math.Abs(pdo.Amount);
                                            creditAmount += _gjde10.CreditAmount;
                                            _gjde10.Description = string.Format("Payroll Other Deduction - {0} {1}", pdo.Explanation, thisIS.ReferenceNo);
                                            _gjde10.SubAccountNo = item.Employee;
                                            _gjde10.SubAccountType = item.Employee.ContactType;
                                            _gjde10.Approved = true;
                                            _gjde10.Save();
                                            pdo.Posted = true;
                                            pdo.Save();
                                        // Update Pay Begin
                                    foreach (EmpOtherDed empOded in item.Employee.EmpOtherDeds)
                                    {
                                        if (empOded.DedCode.DisplayName == pdo.DeductionCode.DisplayName && empOded.RefNo == pdo.RefNo && empOded.Explanation == pdo.Explanation && empOded.EntryDate.ToShortDateString() == pdo.AdvanceEntryDate.ToShortDateString())
                                        {
                                            if (empOded.PayBegin == DateTime.MinValue)
                                            {
                                                empOded.PayBegin = thisIS.PayrollDate;
                                                decimal currBal = empOded.Balance;
                                                decimal tmpBalance = currBal - pdo.Amount;
                                                if (tmpBalance<0)
                                                {
                                                    throw new UserFriendlyException("Over deduction");
                                                }
                                                else if (tmpBalance == 0)
                                                {
                                                    empOded.PayEnd = thisIS.PayrollDate;
                                                    empOded.Paid = true;
                                                }
                                                empOded.PaidAmount += pdo.Amount;
                                                empOded.Save();
                                                pdo.Posted = true;
                                                pdo.Save();
                                            }
                                            else
                                            {
                                                decimal currBal = empOded.Balance;
                                                decimal tmpBalance = currBal - pdo.Amount;
                                                if (tmpBalance < 0)
                                                {
                                                    throw new UserFriendlyException("Over deduction");
                                                }
                                                else if (tmpBalance == 0)
                                                {
                                                    empOded.PayEnd = thisIS.PayrollDate;
                                                    empOded.Paid = true;
                                                }
                                                empOded.PaidAmount += pdo.Amount;
                                                empOded.Save();
                                                pdo.Posted = true;
                                                pdo.Save();
                                            }
                                        }
                                    }


                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        // Credit - Cash in Bank : PayrollBatchType.CashInBankAccount
                        if (thisIS.BatchType.CashInBankAccount == null)
                            throw new UserFriendlyException("Must provide Cash in Bank Account in Batch Type.");
                        if (item.NetPay > 0)
                        {
                            GenJournalDetail _gjde11 = ReflectionHelper.CreateObject<
                            GenJournalDetail>(session);
                            _gjde11.GenJournalID = thisIS;
                            _gjde11.GenJournalID.Approved = true;
                            _gjde11.Account = thisIS.BatchType.CashInBankAccount;
                            _gjde11.CreditAmount = Math.Abs(item.NetPay);
                            creditAmount += _gjde11.CreditAmount;
                            _gjde11.Description = "Driver Payroll " + thisIS.ReferenceNo;
                            _gjde11.SubAccountNo = item.Employee;
                            _gjde11.SubAccountType = item.Employee.ContactType;
                            _gjde11.Approved = true;
                            _gjde11.Save();
                        }
                        if (thisIS.BatchType.AdvancesToEmpMiscExpAccount == null)
                            throw new UserFriendlyException("Must provide Advances to Employees Misc. Expense Account in Batch Type.");
                        if (item.NetPay > 0)
                        {
                            GenJournalDetail _gjde12 = ReflectionHelper.CreateObject<
                            GenJournalDetail>(session);
                            _gjde12.GenJournalID = thisIS;
                            _gjde12.GenJournalID.Approved = true;
                            _gjde12.Account = thisIS.BatchType.AdvancesToEmpMiscExpAccount;
                            _gjde12.CreditAmount = Math.Abs(item.MiscExpCA);
                            creditAmount += _gjde12.CreditAmount;
                            _gjde12.Description = "Advances to Employees Misc. Expense " + thisIS.ReferenceNo;
                            _gjde12.SubAccountNo = item.Employee;
                            _gjde12.SubAccountType = item.Employee.ContactType;
                            _gjde12.Approved = true;
                            _gjde12.Save();
                        }
                    item.Posted = true;
                    item.Save();
                    #endregion
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            }
            finally
            {
                if (index == aCount)
                {
                    thisIS.Status = PayrollBatchStatusEnum.Posted;
                    thisIS.Save();
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
