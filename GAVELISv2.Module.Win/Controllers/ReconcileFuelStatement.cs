using System;
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
    public partial class ReconcileFuelStatement : ViewController
    {
        private FuelStatementOfAccount fuelStatement;
        private SimpleAction reconcileFuelStatement;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        public ReconcileFuelStatement()
        {
            this.TargetObjectType = typeof(FuelStatementOfAccount);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ReconcileStatement", this.GetType
            ().Name);
            this.reconcileFuelStatement = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.reconcileFuelStatement.Caption = "Reconcile";
            this.reconcileFuelStatement.Execute += new SimpleActionExecuteEventHandler(reconcileFuelStatement_Execute);
            this.reconcileFuelStatement.Executed += new EventHandler<ActionBaseEventArgs>(reconcileFuelStatement_Executed);
            this.reconcileFuelStatement.ConfirmationMessage =
            "Do you really want to reconcile the selected entries?";
            UpdateActionState(false);
        }

        void reconcileFuelStatement_Executed(object sender, ActionBaseEventArgs e)
        {
            System.Threading.Thread.Sleep(500);
        }

        void reconcileFuelStatement_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            fuelStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FuelStatementOfAccount;
            if (Math.Round(fuelStatement.Balance, 2) != 0)
            {
                throw new
                    ApplicationException(
                    "The balance must be zero in order to pay selected trips");
            }
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BgWorker_RunWorkerCompleted);
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += new DoWorkEventHandler(_BgWorker_DoWork);
            _BgWorker.RunWorkerAsync(fuelStatement);
        }

        void _BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            FuelStatementOfAccount _fuelStatement = (FuelStatementOfAccount)e.
            Argument;
            FuelStatementOfAccount thisStatement = session.GetObjectByKey<
            FuelStatementOfAccount>(_fuelStatement.Oid);

            try
            {
                #region Process Charges

                foreach (FuelSoaDetail item in thisStatement.FuelStatementOfAccountDetails)
                {
                    ReceiptFuel invoice = null;
                    if (item.Adjust > item.OpenAmount)
                    {
                        throw new ApplicationException(
                            "Overpayment");
                    }
                    invoice = session.GetObjectByKey<ReceiptFuel>(item.Source.Oid);
                    invoice.OpenAmount = item.OpenAmount - item.Adjust;
                    if (invoice.OpenAmount == 0)
                    {
                        invoice.Status = ReceiptFuelStatusEnum.Paid;
                    }
                    else
                    {
                        invoice.Status = ReceiptFuelStatusEnum.PartiallyPaid;
                    }
                    invoice.Save();
                }
                
                #endregion

                #region Process Discount

                if (thisStatement.Customer == null)
                {
                    throw new ApplicationException("Must specify a customer");
                }
                if (thisStatement.Customer.Account == null)
                {
                    throw new ApplicationException("Accounts receivable account must be specified in the chosen vendor card");
                }
                if (thisStatement.Customer.Terms == null)
                {
                    throw new ApplicationException("Discount not allowed if terms is not provided");
                }

                // Create Journal Entry
                JournalEntry journalEntry = ReflectionHelper.
                CreateObject<JournalEntry>(thisStatement.
                Session);
                journalEntry.EntryDate = thisStatement.
                EntryDate;
                journalEntry.ReferenceNo = thisStatement.StatementNo;
                journalEntry.Memo =
                "Discount Given to Customer #" +
                thisStatement.Customer.No;
                journalEntry.Status = JournalEntryStatusEnum.
                Approved;
                journalEntry.Save();
                // Credit Accounts Receivable
                GenJournalDetail _gjde = ReflectionHelper.
                CreateObject<GenJournalDetail>(session);
                _gjde.GenJournalID = journalEntry;
                _gjde.GenJournalID.Approved = true;
                _gjde.Account = thisStatement.Customer.
                Account;
                _gjde.CreditAmount = Math.Abs(thisStatement.Discount);
                creditAmount = creditAmount + _gjde.CreditAmount;
                _gjde.Description = "Discount Given";
                _gjde.SubAccountNo = thisStatement.Customer;
                _gjde.SubAccountType = thisStatement.Customer
                .ContactType;
                _gjde.Approved = true;
                _gjde.Save();
                // Debit Discount Given
                GenJournalDetail _gjde2 = ReflectionHelper.
                CreateObject<GenJournalDetail>(session);
                _gjde2.GenJournalID = journalEntry;
                _gjde2.GenJournalID.Approved = true;
                _gjde2.Account = thisStatement.Customer.Terms.DicountGivenAccount;
                _gjde2.DebitAmount = Math.Abs(thisStatement.Discount);
                debitAmount = debitAmount + _gjde2.DebitAmount;
                _gjde2.Description = "Discount Given";
                _gjde2.SubAccountNo = thisStatement.Customer;
                _gjde2.SubAccountType = thisStatement.
                Customer.ContactType;
                _gjde2.Approved = true;
                _gjde2.Save();

                #endregion

                #region Process Adjustments

                TempAccountCollection2 adjustmentAccounts = new TempAccountCollection2();
                TempAccount2 adjustmentAccount;
                int[] inds = null;
                foreach (var item in thisStatement.FuelStatementAdjustments)
                {
                    if (item.Account != null && item.Amount < 0)
                    {
                        inds = null;
                        inds = adjustmentAccounts.Find2("AaUniqueId", string.Format("DR{0}", item.Account.No));
                        if (inds != null && inds.Length > 0)
                        {
                            adjustmentAccount = adjustmentAccounts[inds[0]];
                            adjustmentAccount.DebitAmount += Math.Abs(item.Amount);
                        }
                        else
                        {
                            adjustmentAccount = new TempAccount2();
                            adjustmentAccount.AaUniqueId = string.Format("DR{0}", item.Account.No);
                            adjustmentAccount.Account = item.Account;
                            adjustmentAccount.DebitAmount += Math.Abs(item.Amount); ;
                            adjustmentAccounts.Add(adjustmentAccount);
                        }
                    }
                    if (item.Account != null && item.Amount > 0)
                    {
                        inds = null;
                        inds = adjustmentAccounts.Find2("AaUniqueId", string.Format("CR{0}", item.Account.No));
                        if (inds != null && inds.Length > 0)
                        {
                            adjustmentAccount = adjustmentAccounts[inds[0]];
                            adjustmentAccount.DebitAmount += Math.Abs(item.Amount);
                        }
                        else
                        {
                            adjustmentAccount = new TempAccount2();
                            adjustmentAccount.AaUniqueId = string.Format("CR{0}", item.Account.No);
                            adjustmentAccount.Account = item.Account;
                            adjustmentAccount.CreditAmount += Math.Abs(item.Amount); ;
                            adjustmentAccounts.Add(adjustmentAccount);
                        }
                    }
                }

                if (adjustmentAccounts.Count > 0)
                {
                    JournalEntry jeAdjustments = ReflectionHelper.
                                CreateObject<JournalEntry>(thisStatement.
                                Session);
                    jeAdjustments.EntryDate = thisStatement.
                    EntryDate;
                    jeAdjustments.ReferenceNo = thisStatement.EntryNo;
                    jeAdjustments.Memo =
                    "Fuel Statement Reconciliation #" +
                    thisStatement.EntryNo;
                    foreach (TempAccount2 item in adjustmentAccounts)
                    {
                        GenJournalDetail _gjd = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjd.GenJournalID = jeAdjustments;
                        _gjd.Account = item.Account;
                        _gjd.DebitAmount = item.DebitAmount;
                        _gjd.CreditAmount = item.CreditAmount;
                        _gjd.Description = "Fuel Statement Adjustments";
                        _gjd.SubAccountNo = thisStatement.Customer;
                        _gjd.SubAccountType = thisStatement.Customer.ContactType;
                        _gjd.Approved = true;
                        debitAmount = debitAmount + _gjd.DebitAmount;
                        creditAmount = creditAmount + _gjd.CreditAmount;
                        _gjd.Save();
                        //Console.WriteLine("{0} {1} {2}", item.AaUniqueId, item.DebitAmount, item.CreditAmount);
                    }
                    GenJournalDetail _gjdeAdjustments = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                    _gjdeAdjustments.GenJournalID = jeAdjustments;
                    _gjdeAdjustments.GenJournalID.Approved = true;
                    _gjdeAdjustments.Account = thisStatement.Customer.Account;
                    _gjdeAdjustments.CreditAmount = Math.Abs(debitAmount - creditAmount);
                    creditAmount = creditAmount + _gjde2.CreditAmount;
                    _gjdeAdjustments.Description = "Fuel Statement Adjustments";
                    _gjdeAdjustments.SubAccountNo = thisStatement.Customer;
                    _gjdeAdjustments.SubAccountType = thisStatement.
                    Customer.ContactType;
                    _gjdeAdjustments.Approved = true;
                    _gjdeAdjustments.Save();
                    jeAdjustments.Status = JournalEntryStatusEnum.Approved;
                    jeAdjustments.Save();
                }

                #endregion

                #region Process Payments

                foreach (var item in thisStatement.FuelStatementPayments)
                {
                    if (item.Select)
                    {
                        if (item.SourceType.Code == "CR")
                        {
                            ReceivePayment receivePayment = session.
                            GetObjectByKey<ReceivePayment>(item.SourceID);
                            receivePayment.OpenAmount = item.OpenAmount - item.AdjustNow;
                            receivePayment.Adjusted = receivePayment.CheckAmount - receivePayment.OpenAmount;
                            receivePayment.Save();
                        }
                    }
                }

                #endregion

                thisStatement.Reconciled = true;
            }
            finally
            {
                if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                {
                    throw new
                        ApplicationException("Accounting entries not balance");
                }
                CommitUpdatingSession(session);
            }
        }

        void _BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Reconcile selected invoice operation has been cancelled",
                    "Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Exclamation);
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
                    "Invoices has been successfully reconciled");

                    ObjectSpace.ReloadObject(fuelStatement);
                    ObjectSpace.Refresh();

                }
            }
        }

        private void UpdateActionState(bool inProgress)
        {
            reconcileFuelStatement.Enabled.SetItemValue("Reconciling entries", !inProgress);
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
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
