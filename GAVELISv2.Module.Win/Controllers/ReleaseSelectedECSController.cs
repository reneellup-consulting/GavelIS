using System;
using System.Linq;
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
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ReleaseSelectedECSController : ViewController
    {
        private EmployeeChargeSlip chargeSlip;
        private SimpleAction releaseSelectedECSAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReleaseSelectedECSController()
        {
            this.TargetObjectType = typeof(EmployeeChargeSlip);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ReleaseSelectedECS", this.GetType().
            Name);
            this.releaseSelectedECSAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.releaseSelectedECSAction.Caption = "Release Selected";
            this.releaseSelectedECSAction.Execute += new
            SimpleActionExecuteEventHandler(ReleaseSelectedECS_Execute);
            this.releaseSelectedECSAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReleaseSelectedECS_Executed);
            this.releaseSelectedECSAction.ConfirmationMessage =
            "Do you really want to release this entries?";
            UpdateActionState(false);
        }
        private void ReleaseSelectedECS_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no entries selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList selected = null;
            selected = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = selected.Count;
            _FrmProgress = new ProgressForm("Releasing entries...", count,
            "Releasing entries {0} of {1} ");
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
        private void ReleaseSelectedECS_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //ObjectSpace.ReloadObject(receipt);
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
            IList trans = (IList)e.Argument;
            try
            {
                foreach (EmployeeChargeSlip item in trans)
                {
                    index++;

                    #region Algorithms here...

                    decimal creditAmount = 0;
                    decimal debitAmount = 0;
                    InventoryControlJournal _icj;
                    TempAccountCollection accounts = new TempAccountCollection();
                    TempAccount tmpAccount;
                    decimal amount = 0;

                    EmployeeChargeSlip thisEcs = session.GetObjectByKey<EmployeeChargeSlip>(item.Oid);

                    int sCount = 0;
                    int iCount = thisEcs.EmployeeChargeSlipItemDetails.Count + thisEcs.EmployeeChargeSlipExpenseDetails.Count;

                    try
                    {
                        #region Items

                        foreach (EmployeeChargeSlipItemDetail ecsid in thisEcs.EmployeeChargeSlipItemDetails)
                        {
                            #region Algorithms here...

                            sCount++;

                            if (ecsid.Quantity > ecsid.CurrentQtyBase && chargeSlip.CompanyInfoHead.AllowInsufficientCurrQty != true)
                            {
                                throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + ecsid.ItemNo.No + "!");
                            }

                            EmployeeChargeSlipItemDetail dets = session.GetObjectByKey<EmployeeChargeSlipItemDetail>(ecsid.Oid);
                            _icj = ReflectionHelper.CreateObject<InventoryControlJournal
                            >(session);
                            _icj.GenJournalID = thisEcs;
                            _icj.Warehouse = dets.Warehouse;
                            _icj.ItemNo = dets.ItemNo;
                            if (dets.ItemNo.UOMRelations.Count > 0)
                            {
                                var dUOM = dets.ItemNo.UOMRelations.Where(o => o.UOM == dets.UOM).FirstOrDefault();
                                var dBaseUOM = dets.ItemNo.UOMRelations.Where(o => o.UOM == dets.ItemNo.BaseUOM2).FirstOrDefault();
                                var dStockUOM = dets.ItemNo.UOMRelations.Where(o => o.UOM == dets.ItemNo.StockUOM).FirstOrDefault();
                                UOMRelation UOMr = session.GetObjectByKey<UOMRelation>(dUOM.Oid);
                                UOMRelation UOMSr = session.GetObjectByKey<UOMRelation>(dStockUOM.Oid);
                                if (dStockUOM.UOM == dUOM.UOM)
                                {
                                    _icj.OutQty = dets.Quantity;
                                }
                                else
                                {
                                    _icj.OutQty = dets.BaseQTY / dStockUOM.Factor;
                                }
                                UOMr.PricePerBaseUom = dets.Price;
                                UOMSr.PricePerBaseUom = (dets.Quantity * dets.Price) / _icj.OutQty;
                                _icj.UOM = dets.ItemNo.StockUOM;
                                _icj.Price = UOMSr.PricePerBaseUom;
                                _icj.Cost = UOMSr.CostPerBaseUom;
                            }
                            else
                            {
                                _icj.OutQty = dets.Quantity;
                                _icj.UOM = dets.UOM;
                                _icj.Price = dets.Price;
                                _icj.Cost = dets.Cost;
                                dets.ItemNo.SalesPrice = dets.Price;
                                dets.ItemNo.Cost = dets.Cost;
                            }
                            _icj.RequisitionNo = dets.RequisitionNo;
                            _icj.RowID = dets.RowID.ToString();
                            _icj.Save();

                            amount += dets.Total;
                            int[] inds = null;
                            if (dets.ItemNo.ItemType != ItemTypeEnum.ServiceItem && dets.ItemNo.IncomeAccount == null)
                            {
                                throw new
                                    ApplicationException(
                                    "Must specify an income account in " + dets.ItemNo.No +
                                    " Item Card");
                            }
                            else if (dets.ItemNo.ItemType != ItemTypeEnum.ServiceItem && dets.ItemNo.IncomeAccount != null)
                            {
                                inds = accounts.Find("Account", dets.ItemNo.IncomeAccount);
                                if (inds != null && inds.Length > 0)
                                {
                                    tmpAccount = accounts[inds[0]];
                                    tmpAccount.CreditAmount += Math.Abs(dets.Total + dets.LineDiscount);
                                }
                                else
                                {
                                    tmpAccount = new TempAccount();
                                    tmpAccount.Account = dets.ItemNo.IncomeAccount;
                                    tmpAccount.CreditAmount += Math.Abs(dets.Total + dets.LineDiscount);
                                    accounts.Add(tmpAccount);
                                }
                            }
                            
                            // Sales Discount
                            if (dets.LineDiscount > 0)
                            {
                                if (dets.ItemNo.ItemType != ItemTypeEnum.ServiceItem && dets.CompanyInfo.SalesDiscountIHAcct == null)
                                {
                                    throw
                                        new ApplicationException(
                                        "Must specify a default sales discount for IH accounts in the company information card"
                                        );
                                }
                                else if (dets.ItemNo.ItemType != ItemTypeEnum.ServiceItem && dets.CompanyInfo.SalesDiscountIHAcct != null)
                                {
                                    inds = accounts.Find("Account", dets.CompanyInfo.
                                        SalesDiscountIHAcct);
                                    if (inds != null && inds.Length > 0)
                                    {
                                        tmpAccount = accounts[inds[0]];
                                        tmpAccount.DebitAmount += Math.Abs(dets.
                                        LineDiscount);
                                    }
                                    else
                                    {
                                        tmpAccount = new TempAccount();
                                        tmpAccount.Account = dets.CompanyInfo.
                                        SalesDiscountIHAcct;
                                        tmpAccount.DebitAmount += Math.Abs(dets.
                                        LineDiscount);
                                        accounts.Add(tmpAccount);
                                    }
                                }
                            }

                            // Credit Inventory Asset
                            if (dets.ItemNo.ItemType != ItemTypeEnum.ServiceItem && dets.ItemNo.InventoryAccount == null)
                            {
                                throw new
                                    ApplicationException(
                                    "Must specify an Inventory account in " + dets.ItemNo.No
                                    + " Item Card");
                            }
                            else if (dets.ItemNo.ItemType != ItemTypeEnum.ServiceItem && dets.ItemNo.InventoryAccount != null)
                            {
                                inds = null;
                                inds = accounts.Find("Account", dets.ItemNo.InventoryAccount
                                );
                                if (inds != null && inds.Length > 0)
                                {
                                    tmpAccount = accounts[inds[0]];
                                    tmpAccount.CreditAmount += (dets.Price * dets.
                                    Factor) * dets.Quantity;
                                }
                                else
                                {
                                    tmpAccount = new TempAccount();
                                    tmpAccount.Account = dets.ItemNo.InventoryAccount;
                                    tmpAccount.CreditAmount += (dets.Price * dets.
                                    Factor) * dets.Quantity;
                                    accounts.Add(tmpAccount);
                                }
                            }
                            
                            // Debit Cost of Goods Sold
                            if (dets.ItemNo.COGSAccount == null)
                            {
                                throw new
                                    ApplicationException("Must specify a COGS account in " +
                                    dets.ItemNo.No + " Item Card");
                            }
                            inds = null;
                            inds = accounts.Find("Account", dets.ItemNo.COGSAccount);
                            if (inds != null && inds.Length > 0)
                            {
                                tmpAccount = accounts[inds[0]];
                                if (dets.ItemNo.ItemType == ItemTypeEnum.ServiceItem)
                                {
                                    tmpAccount.CreditAmount += (dets.Price * dets.
                                            Factor) * dets.Quantity;
                                }
                                else
                                {
                                    tmpAccount.DebitAmount += (dets.Price * dets.
                                            Factor) * dets.Quantity;
                                }
                            }
                            else
                            {
                                tmpAccount = new TempAccount();
                                tmpAccount.Account = dets.ItemNo.COGSAccount;
                                if (dets.ItemNo.ItemType == ItemTypeEnum.ServiceItem)
                                {
                                    tmpAccount.CreditAmount += (dets.Price * dets.
                                            Factor) * dets.Quantity;
                                }
                                else
                                {
                                    tmpAccount.DebitAmount += (dets.Price * dets.
                                            Factor) * dets.Quantity;
                                }
                                accounts.Add(tmpAccount);
                            }

                            #endregion

                            dets.Save();

                        }

                        #endregion

                        #region Expenses

                        foreach (EmployeeChargeSlipExpenseDetail ecscd in thisEcs.EmployeeChargeSlipExpenseDetails)
                        {
                            #region Expenses

                            sCount++;

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
                            
                            ecscd.Save();
                            
                        }

                        thisEcs.Released = true;
                        thisEcs.Save();

                        #endregion
                    }
                    finally
                    {
                        if (iCount == sCount)
                        {
                            #region Advances to Employees

                            if (thisEcs.AdvancesAccount == null)
                            {
                                throw new ApplicationException("Must specify advances account for this  employees charge slip.");
                            }

                            GenJournalDetail _gjdeAdvances = ReflectionHelper.CreateObject<
                                        GenJournalDetail>(session);
                            _gjdeAdvances.GenJournalID = thisEcs;
                            _gjdeAdvances.GenJournalID.Approved = true;
                            _gjdeAdvances.Account = thisEcs.AdvancesAccount;
                            _gjdeAdvances.DebitAmount = Math.Abs(thisEcs.GrossTotal);
                            _gjdeAdvances.Description = "Advances to Employees " +
                            thisEcs.ReferenceNo;
                            _gjdeAdvances.SubAccountNo = thisEcs.Employee;
                            _gjdeAdvances.SubAccountType = thisEcs.Employee.ContactType;
                            _gjdeAdvances.Approved = true;
                            debitAmount = debitAmount + _gjdeAdvances.DebitAmount;
                            _gjdeAdvances.Save();

                            #endregion

                            #region Accounts

                            foreach (TempAccount item1 in accounts)
                            {
                                GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                                GenJournalDetail>(session);
                                _gjd.GenJournalID = thisEcs;
                                _gjd.Account = item1.Account;
                                _gjd.DebitAmount = item1.DebitAmount;
                                _gjd.CreditAmount = item1.CreditAmount;
                                _gjd.Description = "Advances to Employees " +
                                thisEcs.ReferenceNo;
                                _gjd.SubAccountNo = thisEcs.Employee;
                                _gjd.SubAccountType = thisEcs.Employee.ContactType;
                                _gjd.Approved = true;
                                debitAmount = debitAmount + _gjd.DebitAmount;
                                creditAmount = creditAmount + _gjd.CreditAmount;
                                _gjd.Save();
                            }

                            #endregion

                            if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                            {
                                throw new
                                ApplicationException(string.Format("Accounting entries not balance in {0}.", thisEcs.SourceNo));
                            }
                        }
                    }

                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Releasing entry {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.Count)
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
                    "Releasing entries has been cancelled", "Cancelled",
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
                    " has been successfully released");

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
            releaseSelectedECSAction.
                Enabled.SetItemValue("Releasing entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
