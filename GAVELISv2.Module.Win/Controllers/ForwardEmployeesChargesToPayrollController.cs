using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ForwardEmployeesChargesToPayrollController : ViewController
    {
        private EmployeeChargeSlip chargeSlip;
        private SimpleAction forwardToPayrollAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ForwardEmployeesChargesToPayrollController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("ForwardToPayroll", this.GetType().Name);
            this.forwardToPayrollAction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.forwardToPayrollAction.Caption = "Forward To Payroll";
            this.forwardToPayrollAction.Execute += new 
            SimpleActionExecuteEventHandler(ForwardToPayrollAction_Execute);
            this.forwardToPayrollAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ForwardToPayrollAction_Executed);
            this.forwardToPayrollAction.ConfirmationMessage = 
            "Do you really want to forward this Employees Charge Slip?";
            UpdateActionState(false);
        }
        private void ForwardToPayrollAction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            //invoice = ((DevExpress.ExpressApp.DetailView)this.View).
            //CurrentObject as Invoice;
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.DetailView))
            {
                chargeSlip = ((DevExpress.ExpressApp.DetailView)this.View).
                CurrentObject as EmployeeChargeSlip;
            }
            if (this.View.GetType() == typeof(DevExpress.ExpressApp.ListView))
            {
                chargeSlip = this.View.CurrentObject as EmployeeChargeSlip;
            }
            ObjectSpace.CommitChanges();

            if (chargeSlip.Status!=EmployeeChargeSlipStatusEnum.Approved)
            {
                throw new ApplicationException("Cannot forward unapproved charge slip");
            }

            var count = chargeSlip.EmployeeChargeSlipItemDetails.Count + chargeSlip.EmployeeChargeSlipExpenseDetails.Count;
            _FrmProgress = new ProgressForm("Forwarding entries...", count, 
            "Forwarding entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(chargeSlip);
            _FrmProgress.ShowDialog();
        }
        private void ForwardToPayrollAction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(invoice);
            //ObjectSpace.Refresh();
        }
        private UnitOfWork CreateUpdatingSession() {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) { 
            if (UpdatingSessionCommitted != null) {UpdatingSessionCommitted(this
                , new SessionEventArgs(session));} }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) { if 
            (UpdatingSessionCreated != null) {UpdatingSessionCreated(this, new 
                SessionEventArgs(session));} }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            EmployeeChargeSlip _chargeSlip = (EmployeeChargeSlip)e.Argument;
            EmployeeChargeSlip thisChargeSlip = session.GetObjectByKey<EmployeeChargeSlip>(_chargeSlip.Oid);

            InventoryControlJournal _icj;
            TempAccountCollection accounts = new TempAccountCollection();
            TempAccount tmpAccount;
            decimal amount = 0;
            //int partial = 0;
            bool runAccounts = false;
            try
            {
                if (thisChargeSlip.Released != true)
                {
                    runAccounts = true;
                    #region Items
                    foreach (EmployeeChargeSlipItemDetail ecsid in thisChargeSlip.EmployeeChargeSlipItemDetails)
                    {
                        if (ecsid.Quantity > ecsid.CurrentQtyBase && chargeSlip.CompanyInfoHead.AllowInsufficientCurrQty != true)
                        {
                            throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + ecsid.ItemNo.No + "!");
                        }
                        _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                        >(session);
                        _icj.GenJournalID = thisChargeSlip;
                        //_icj.OutQty = Math.Abs(ecsid.BaseQTY);
                        _icj.Warehouse = ecsid.Warehouse;
                        _icj.ItemNo = ecsid.ItemNo;
                        if (ecsid.ItemNo.UOMRelations.Count > 0)
                        {
                            var dUOM = ecsid.ItemNo.UOMRelations.Where(o => o.UOM == ecsid.UOM).FirstOrDefault();
                            var dBaseUOM = ecsid.ItemNo.UOMRelations.Where(o => o.UOM == ecsid.ItemNo.BaseUOM2).FirstOrDefault();
                            var dStockUOM = ecsid.ItemNo.UOMRelations.Where(o => o.UOM == ecsid.ItemNo.StockUOM).FirstOrDefault();
                            UOMRelation UOMr = session.GetObjectByKey<UOMRelation>(dUOM.Oid);
                            UOMRelation UOMSr = session.GetObjectByKey<UOMRelation>(dStockUOM.Oid);
                            if (dStockUOM.UOM == dUOM.UOM)
                            {
                                _icj.OutQty = ecsid.Quantity;
                            }
                            else
                            {
                                _icj.OutQty = ecsid.BaseQTY / dStockUOM.Factor;
                                //UOMr.CostPerBaseUom = //(item.Quantity * item.Cost) / _icj.InQTY;
                            }
                            UOMr.PricePerBaseUom = ecsid.Price;
                            UOMSr.PricePerBaseUom = (ecsid.Quantity * ecsid.Price) / _icj.OutQty;
                            _icj.UOM = ecsid.ItemNo.StockUOM;
                            _icj.Price = UOMSr.PricePerBaseUom;
                            _icj.Cost = UOMSr.CostPerBaseUom;
                        }
                        else
                        {
                            _icj.OutQty = ecsid.Quantity;
                            _icj.UOM = ecsid.UOM;
                            _icj.Price = ecsid.Price;
                            _icj.Cost = ecsid.Cost;
                            ecsid.ItemNo.SalesPrice = ecsid.Price;
                            ecsid.ItemNo.Cost = ecsid.Cost;
                        }
                        //_icj.Cost = ecsid.ItemNo.Cost;
                        //_icj.Price = ecsid.Price;
                        //_icj.UOM = ecsid.BaseUOM;
                        _icj.RequisitionNo = ecsid.RequisitionNo;
                        _icj.RowID = ecsid.RowID.ToString();
                        _icj.Save();

                        amount += ecsid.Total;
                        int[] inds = null;
                        // Credit Sales Income
                        if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem && ecsid.ItemNo.IncomeAccount == null)
                        {
                            throw new
                                ApplicationException(
                                "Must specify an income account in " + ecsid.ItemNo.No +
                                " Item Card");
                        }
                        else if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem && ecsid.ItemNo.IncomeAccount != null)
                        {
                            inds = accounts.Find("Account", ecsid.ItemNo.IncomeAccount);
                            if (inds != null && inds.Length > 0)
                            {
                                tmpAccount = accounts[inds[0]];
                                tmpAccount.CreditAmount += Math.Abs(ecsid.Total + ecsid.LineDiscount);
                            }
                            else
                            {
                                tmpAccount = new TempAccount();
                                tmpAccount.Account = ecsid.ItemNo.IncomeAccount;
                                tmpAccount.CreditAmount += Math.Abs(ecsid.Total + ecsid.LineDiscount);
                                accounts.Add(tmpAccount);
                            }
                        }
                        
                        // Sales Discount
                        if (ecsid.LineDiscount > 0)
                        {
                            if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem && ecsid.CompanyInfo.SalesDiscountIHAcct == null)
                            {
                                throw
                                    new ApplicationException(
                                    "Must specify a default sales discount for IH accounts in the company information card"
                                    );
                            }
                            else if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem && ecsid.CompanyInfo.SalesDiscountIHAcct != null)
                            {
                                inds = accounts.Find("Account", ecsid.CompanyInfo.
                                    SalesDiscountIHAcct);
                                if (inds != null && inds.Length > 0)
                                {
                                    tmpAccount = accounts[inds[0]];
                                    tmpAccount.DebitAmount += Math.Abs(ecsid.
                                    LineDiscount);
                                }
                                else
                                {
                                    tmpAccount = new TempAccount();
                                    tmpAccount.Account = ecsid.CompanyInfo.
                                    SalesDiscountIHAcct;
                                    tmpAccount.DebitAmount += Math.Abs(ecsid.
                                    LineDiscount);
                                    accounts.Add(tmpAccount);
                                }
                            }
                        }

                        // Credit Inventory Asset
                        if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem && ecsid.ItemNo.InventoryAccount == null)
                        {
                            throw new
                                ApplicationException(
                                "Must specify an Inventory account in " + ecsid.ItemNo.No
                                + " Item Card");
                        }
                        else if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem && ecsid.ItemNo.InventoryAccount != null)
                        {
                            inds = null;
                            inds = accounts.Find("Account", ecsid.ItemNo.InventoryAccount
                            );
                            if (inds != null && inds.Length > 0)
                            {
                                tmpAccount = accounts[inds[0]];
                                tmpAccount.CreditAmount += (ecsid.Price * ecsid.
                                Factor) * ecsid.Quantity;
                            }
                            else
                            {
                                tmpAccount = new TempAccount();
                                tmpAccount.Account = ecsid.ItemNo.InventoryAccount;
                                tmpAccount.CreditAmount += (ecsid.Price * ecsid.
                                Factor) * ecsid.Quantity;
                                accounts.Add(tmpAccount);
                            }
                        }
                        
                        // Debit Cost of Goods Sold
                        if (ecsid.ItemNo.COGSAccount == null)
                        {
                            throw new
                                ApplicationException("Must specify a COGS account in " +
                                ecsid.ItemNo.No + " Item Card");
                        }
                        inds = null;
                        inds = accounts.Find("Account", ecsid.ItemNo.COGSAccount);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            if (ecsid.ItemNo.ItemType == ItemTypeEnum.ServiceItem)
                            {
                                tmpAccount.CreditAmount += (ecsid.Price * ecsid.
                                        Factor) * ecsid.Quantity;
                            }
                            else
                            {
                                tmpAccount.DebitAmount += (ecsid.Price * ecsid.
                                        Factor) * ecsid.Quantity;
                            }
                        }
                        else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = ecsid.ItemNo.COGSAccount;
                            if (ecsid.ItemNo.ItemType == ItemTypeEnum.ServiceItem)
                            {
                                tmpAccount.CreditAmount += (ecsid.Price * ecsid.
                                    Factor) * ecsid.Quantity;
                            }
                            else
                            {
                                tmpAccount.DebitAmount += (ecsid.Price * ecsid.
                                    Factor) * ecsid.Quantity;
                            }
                            
                            accounts.Add(tmpAccount);
                        }
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                        ecsid.Save();
                        _message = string.Format("Forwarding entry {0} succesfull.",
                        thisChargeSlip.InvoiceDetails.Count - 1);
                        System.Threading.Thread.Sleep(20);
                        _BgWorker.ReportProgress(1, _message);
                        index++;

                    }
                    #endregion

                    foreach (EmployeeChargeSlipExpenseDetail ecscd in thisChargeSlip.EmployeeChargeSlipExpenseDetails)
                    {
                        #region Expenses
                        amount += ecscd.Amount;
                        int[] inds = null;
                        if (ecscd.Expense == null)
                        {
                            throw new
                                ApplicationException(
                                "Must specify an expense account in every line");
                        }

                        inds = accounts.Find("Account", ecscd.Expense);
                        if (inds != null && inds.Length > 0)
                        {
                            tmpAccount = accounts[inds[0]];
                            tmpAccount.CreditAmount += Math.Abs(ecscd.Amount);
                        }
                        else
                        {
                            tmpAccount = new TempAccount();
                            tmpAccount.Account = ecscd.Expense;
                            tmpAccount.CreditAmount += Math.Abs(ecscd.Amount);
                            accounts.Add(tmpAccount);
                        }
                        #endregion
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                        ecscd.Save();
                        _message = string.Format("Forwarding entry {0} succesfull.",
                        thisChargeSlip.InvoiceDetails.Count - 1);
                        System.Threading.Thread.Sleep(20);
                        _BgWorker.ReportProgress(1, _message);
                        index++;
                    }
                }

                thisChargeSlip.Released = true;
                thisChargeSlip.Save();

            }
            finally
            {
                if (runAccounts && index == thisChargeSlip.EmployeeChargeSlipItemDetails.Count + thisChargeSlip.EmployeeChargeSlipExpenseDetails.Count)
                {
                    #region Advances to Employees
                    if (thisChargeSlip.AdvancesAccount == null)
                    {
                        throw new ApplicationException("Must specify advances account for this  employees charge slip.");
                    }

                    GenJournalDetail _gjdeAdvances = ReflectionHelper.CreateObject <
                                GenJournalDetail>(session);
                    _gjdeAdvances.GenJournalID = thisChargeSlip;
                    _gjdeAdvances.GenJournalID.Approved = true;
                    _gjdeAdvances.Account = thisChargeSlip.AdvancesAccount;
                    _gjdeAdvances.DebitAmount = Math.Abs(thisChargeSlip.GrossTotal);
                    _gjdeAdvances.Description = "Advances to Employees " +
                    thisChargeSlip.ReferenceNo;
                    _gjdeAdvances.SubAccountNo = thisChargeSlip.Employee;
                    _gjdeAdvances.SubAccountType = thisChargeSlip.Employee.ContactType;
                    _gjdeAdvances.Approved = true;
                    debitAmount = debitAmount + _gjdeAdvances.DebitAmount;
                    _gjdeAdvances.Save();
                    #endregion
                    #region Accounts
                    foreach (TempAccount item in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = thisChargeSlip;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Advances to Employees " +
                        thisChargeSlip.ReferenceNo;
                        _gjd.SubAccountNo = thisChargeSlip.Employee;
                        _gjd.SubAccountType = thisChargeSlip.Employee.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }

                    #endregion

                    e.Result = index;
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                        ApplicationException("Accounting entries not balance");
                    }

                    #region Forward to Payroll
                    EmpOtherDed _eod = ReflectionHelper.CreateObject<
                        EmpOtherDed>(session);
                    _eod.Employee = thisChargeSlip.Employee;
                    _eod.DedCode = thisChargeSlip.DeductionCode;
                    _eod.EntryDate = thisChargeSlip.EntryDate;
                    _eod.Explanation = thisChargeSlip.Memo;
                    _eod.ChargeSlipRef = thisChargeSlip;
                    _eod.RefNo = thisChargeSlip.ReferenceNo;
                    _eod.Amount = thisChargeSlip.GrossTotal;
                    _eod.Save();
                    #endregion
                    thisChargeSlip.PayrollDeductionRef = _eod;
                    thisChargeSlip.Status = EmployeeChargeSlipStatusEnum.ForwardedToPayroll;
                    CommitUpdatingSession(session);
                }
                else if (!runAccounts)
                {
                    int cnt = thisChargeSlip.EmployeeChargeSlipItemDetails.Count + thisChargeSlip.EmployeeChargeSlipExpenseDetails.Count;
                    _BgWorker.ReportProgress(cnt, _message);


                    #region Forward to Payroll
                    EmpOtherDed _eod = ReflectionHelper.CreateObject<
                        EmpOtherDed>(session);
                    _eod.Employee = thisChargeSlip.Employee;
                    _eod.DedCode = thisChargeSlip.DeductionCode;
                    _eod.EntryDate = thisChargeSlip.EntryDate;
                    _eod.Explanation = thisChargeSlip.Memo;
                    _eod.ChargeSlipRef = thisChargeSlip;
                    _eod.RefNo = thisChargeSlip.ReferenceNo;
                    _eod.Amount = thisChargeSlip.GrossTotal;
                    _eod.Save();
                    #endregion
                    thisChargeSlip.PayrollDeductionRef = _eod;
                    thisChargeSlip.Status = EmployeeChargeSlipStatusEnum.ForwardedToPayroll;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void BgWorkerProgressChanged(object sender, 
        ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
                DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Forwarding entries operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("All " + e.Result + 
                    " has been successfully forwarded");
                    ObjectSpace.ReloadObject(chargeSlip);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { forwardToPayrollAction.
            Enabled.SetItemValue("Forwarding entries", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
