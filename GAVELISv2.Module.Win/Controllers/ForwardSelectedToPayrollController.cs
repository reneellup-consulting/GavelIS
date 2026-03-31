using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
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
    public partial class ForwardSelectedToPayrollController : ViewController
    {
        private EmployeeChargeSlip chargeSlip;
        private SimpleAction forwardToPayrollAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ForwardSelectedToPayrollController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("ForwardSelectedToPayroll", this.GetType().Name);
            this.forwardToPayrollAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.forwardToPayrollAction.Caption = "Forward Selected To Payroll";
            this.forwardToPayrollAction.Execute += new
            SimpleActionExecuteEventHandler(ForwardToPayrollAction_Execute);
            this.forwardToPayrollAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ForwardToPayrollAction_Executed);
            this.forwardToPayrollAction.ConfirmationMessage =
            "Do you really want to forward the selected Employees Charge Slips?";
            UpdateActionState(false);
        }
        private void ForwardToPayrollAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            IList selected = this.View.SelectedObjects;
            //if (chargeSlip.Status != EmployeeChargeSlipStatusEnum.Approved)
            //{
            //    throw new ApplicationException("Cannot forward unapproved charge slip");
            //}

            var count = selected.Count;
            _FrmProgress = new ProgressForm("Forwarding entries...", count,
            "Forwarding entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(selected);
            _FrmProgress.ShowDialog();
        }
        private void ForwardToPayrollAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //ObjectSpace.ReloadObject(invoice);
            //ObjectSpace.Refresh();
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList selected = (IList)e.Argument;
            //EmployeeChargeSlip _chargeSlip = (EmployeeChargeSlip)e.Argument;
            //EmployeeChargeSlip thisChargeSlip = session.GetObjectByKey<EmployeeChargeSlip>(_chargeSlip.Oid);

            EmployeeChargeSlip eobj = null;
            try
            {
                foreach (var sel in selected)
                {
                    index++;
                    eobj = session.GetObjectByKey<EmployeeChargeSlip>((sel as EmployeeChargeSlip).Oid);
                    _message = string.Format("Forwarding entry {0} succesfull.",
                    eobj.SourceNo);
                    _BgWorker.ReportProgress(1, _message);

                    InventoryControlJournal _icj;
                    TempAccountCollection accounts = new TempAccountCollection();
                    TempAccount tmpAccount;
                    decimal amount = 0;
                    //int partial = 0;

                    decimal creditAmount = 0;
                    decimal debitAmount = 0;

                    #region Items
                    foreach (EmployeeChargeSlipItemDetail ecsid in eobj.EmployeeChargeSlipItemDetails)
                    {
                        if (ecsid.Quantity > ecsid.CurrentQtyBase && chargeSlip.CompanyInfoHead.AllowInsufficientCurrQty != true)
                        {
                            throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + ecsid.ItemNo.No + "!");
                        }
                        _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                        >(session);
                        _icj.GenJournalID = eobj;
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
                            string msg = string.Format("Must specify an income account in {0} Item Card from source #{1}.", ecsid.ItemNo.No, eobj.SourceNo);
                            throw new
                                ApplicationException(msg);
                        }
                        if (ecsid.ItemNo.ItemType != ItemTypeEnum.ServiceItem)
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
                            // Credit Inventory Asset
                            if (ecsid.ItemNo.InventoryAccount == null)
                            {
                                string msg = string.Format("Must specify an inventory account in {0} Item Card from source #{1}.", ecsid.ItemNo.No, eobj.SourceNo);
                                throw new
                                    ApplicationException(msg);
                            }
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
                            // Debit Cost of Goods Sold
                            if (ecsid.ItemNo.COGSAccount == null)
                            {
                                string msg = string.Format("Must specify an COGS account in {0} Item Card from source #{1}.", ecsid.ItemNo.No, eobj.SourceNo);
                                throw new
                                    ApplicationException(msg);
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
                        }

                        //if (ecsid.ItemNo.GetType() == typeof(ServiceItem))
                        //{
                        //    // Debit Cost of Goods Sold
                        //    if (ecsid.ItemNo.COGSAccount == null)
                        //    {
                        //        string msg = string.Format("Must specify an COGS account in {0} Item Card from source #{1}.", ecsid.ItemNo.No, eobj.SourceNo);
                        //        throw new
                        //            ApplicationException(msg);
                        //    }
                        //    inds = null;
                        //    inds = accounts.Find("Account", ecsid.ItemNo.COGSAccount);
                        //    if (inds != null && inds.Length > 0)
                        //    {
                        //        tmpAccount = accounts[inds[0]];
                        //        //tmpAccount.CreditAmount += (ecsid.ItemNo.Cost * ecsid.
                        //        //Factor) * ecsid.Quantity;
                        //        tmpAccount.CreditAmount += ecsid.Price * ecsid.Quantity;
                        //    }
                        //    else
                        //    {
                        //        tmpAccount = new TempAccount();
                        //        tmpAccount.Account = ecsid.ItemNo.COGSAccount;
                        //        //tmpAccount.CreditAmount += (ecsid.ItemNo.Cost * ecsid.
                        //        //Factor) * ecsid.Quantity;
                        //        tmpAccount.CreditAmount += ecsid.Price * ecsid.Quantity;
                        //        accounts.Add(tmpAccount);
                        //    }
                        //}
                        
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                        ecsid.Save();
                    }
                    #endregion

                    #region Expenses
                    foreach (EmployeeChargeSlipExpenseDetail ecscd in eobj.EmployeeChargeSlipExpenseDetails)
                    {
                        amount += ecscd.Amount;
                        int[] inds = null;
                        if (ecscd.Expense == null)
                        {
                            string msg = string.Format("Must specify an expense account in every line of #{0}.", eobj.SourceNo);
                            throw new
                                ApplicationException(msg);
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
                        ecscd.Save();
                    }

                    #endregion

                    #region Advances to Employees
                    if (eobj.AdvancesAccount == null)
                    {
                        string msg = string.Format("Must specify advances account for #{0}.", eobj.SourceNo);
                        throw new ApplicationException(msg);
                    }

                    GenJournalDetail _gjdeAdvances = ReflectionHelper.CreateObject<
                                GenJournalDetail>(session);
                    _gjdeAdvances.GenJournalID = eobj;
                    _gjdeAdvances.GenJournalID.Approved = true;
                    _gjdeAdvances.Account = eobj.AdvancesAccount;
                    _gjdeAdvances.DebitAmount = Math.Abs(eobj.GrossTotal);
                    _gjdeAdvances.Description = "Advances to Employees " +
                    eobj.ReferenceNo;
                    _gjdeAdvances.SubAccountNo = eobj.Employee;
                    _gjdeAdvances.SubAccountType = eobj.Employee.ContactType;
                    _gjdeAdvances.Approved = true;
                    debitAmount = debitAmount + _gjdeAdvances.DebitAmount;
                    _gjdeAdvances.Save();
                    #endregion

                    #region Accounts
                    foreach (TempAccount item in accounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = eobj;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Advances to Employees " +
                        eobj.ReferenceNo;
                        _gjd.SubAccountNo = eobj.Employee;
                        _gjd.SubAccountType = eobj.Employee.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                    }
                    #endregion

                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                        ApplicationException("Accounting entries not balance for #" + eobj.SourceNo);
                    }

                    #region Forward to Payroll
                    EmpOtherDed _eod = ReflectionHelper.CreateObject<
                        EmpOtherDed>(session);
                    _eod.Employee = eobj.Employee;
                    _eod.DedCode = eobj.DeductionCode;
                    _eod.EntryDate = eobj.EntryDate;
                    _eod.Explanation = eobj.Memo;
                    _eod.ChargeSlipRef = eobj;
                    _eod.RefNo = eobj.ReferenceNo;
                    _eod.Amount = eobj.GrossTotal;
                    _eod.Save();
                    #endregion

                    eobj.PayrollDeductionRef = _eod;
                    eobj.Status = EmployeeChargeSlipStatusEnum.ForwardedToPayroll;
                    eobj.Save();

                    CommitUpdatingSession(session);

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
                if (index == selected.Count)
                {
                    e.Result = index;
                }
                session.Dispose();
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
                    "Forwarding entries operation has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully forwarded");
                    //ObjectSpace.ReloadObject(chargeSlip);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e)
        {
            _BgWorker.CancelAsync();
        }
        private void UpdateActionState(bool inProgress)
        {
            forwardToPayrollAction.
                Enabled.SetItemValue("Forwarding entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
