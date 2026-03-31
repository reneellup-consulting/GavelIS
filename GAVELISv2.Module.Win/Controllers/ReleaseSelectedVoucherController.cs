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
    public partial class ReleaseSelectedVoucherController : ViewController
    {
        private SimpleAction releaseSelectedVoucherAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ReleaseSelectedVoucherController()
        {
            this.TargetObjectType = typeof(MultiCheckVoucher);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.ReleaseSelectedVoucher", this.GetType().
            Name);
            this.releaseSelectedVoucherAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.releaseSelectedVoucherAction.Caption = "Release Selected";
            this.releaseSelectedVoucherAction.Execute += new
            SimpleActionExecuteEventHandler(ReleaseSelectedVoucher_Execute);
            this.releaseSelectedVoucherAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ReleaseSelectedVoucher_Executed);
            this.releaseSelectedVoucherAction.ConfirmationMessage =
            "Do you really want to release the selected vouchers?";
            UpdateActionState(false);
        }
        private void ReleaseSelectedVoucher_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no vouchers selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList genHeaders = null;
            genHeaders = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = genHeaders.Count;
            _FrmProgress = new ProgressForm("Releasing vouchers...", count,
            "Releasing vouchers {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(genHeaders);
            _FrmProgress.ShowDialog();
        }
        private void ReleaseSelectedVoucher_Executed(object sender,
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
                foreach (MultiCheckVoucher item in trans)
                {
                    index++;

                    #region Algorithms here...

                    decimal creditAmount = 0;
                    decimal debitAmount = 0;
                    MultiCheckVoucher thisVoucher = session.GetObjectByKey<MultiCheckVoucher>(
                    item.Oid);
                    if (thisVoucher.Status == MultiCheckStatusEnum.Released)
                    {
                        throw new ApplicationException(string.Format("Voucher {0} was already released!", thisVoucher.CheckVoucherNo));
                    }
                    if (thisVoucher.Payee == null)
                    {
                        throw new
                            ApplicationException("Must specify a Payee to the check");
                    }
                    switch (thisVoucher.Payee.ContactType)
                    {
                        case ContactTypeEnum.Customer:
                            if (((Customer)thisVoucher.Payee).Account == null)
                            {
                                throw new ApplicationException(
                                "Accounts Receivable account must be specified in the chosen "
                                + thisVoucher.Payee.ContactType + " card");
                            }
                            break;
                        case ContactTypeEnum.Vendor:
                            if (((Vendor)thisVoucher.Payee).Account == null)
                            {
                                throw new ApplicationException(
                                "Accounts Payable account must be specified in the chosen vendor card"
                                );
                            }
                            break;
                        default:
                            break;
                    }

                    #region Expense
                    bool IsExpense = false;
                    // Verify if Expense
                    foreach (MCheckEffectiveDetails mcefd in thisVoucher.
                    MCheckEffectiveDetails) { if (mcefd.Expense) { IsExpense = true; } }
                    // If Expense = true
                    Account tmpExp = null;
                    if (IsExpense)
                    {
                        tmpExp = Company.GetInstance(session).TemporaryExpenseAcct;
                        if (tmpExp == null)
                        {
                            throw new ApplicationException(
                                "Temporary account was not provided in the company setup card"
                                );
                        }
                    }
                    foreach (MCheckVoucherDetail mcvd in thisVoucher.
                    MCheckVoucherDetails)
                    {
                        // Create check for each
                        CheckPayment _chk = ReflectionHelper.CreateObject<
                        CheckPayment>(session);
                        _chk.EntryDate = mcvd.EntryDate;
                        _chk.PostDated = mcvd.PostDated;
                        _chk.CheckDate = mcvd.CheckDate;
                        _chk.BankCashAccount = mcvd.BankAccount;
                        _chk.PaymentMode = PaymentTypeEnum.Check;
                        _chk.CheckNo = mcvd.CheckNo;
                        _chk.ReferenceNo = mcvd.CheckVoucher.CheckVoucherNo;
                        _chk.Memo = mcvd.CheckVoucher.Memo;
                        _chk.Comments = mcvd.CheckVoucher.Comments;
                        _chk.PayToOrder = mcvd.CheckVoucher.Payee;
                        _chk.ExpenseType = mcvd.ExpenseType;
                        _chk.SubExpenseType = mcvd.SubExpenseType != null ? mcvd.SubExpenseType : null;
                        _chk.CheckAmount = mcvd.Amount;
                        _chk.Status = CheckStatusEnum.Approved;
                        _chk.Save();

                        // Create the following entry
                        // If Expense = true Dr. Temporary Expense
                        // If Expense = false Dr. Accounts Payable
                        // Cr. Cash in bank
                        if (IsExpense)
                        {
                            // If Expense = true Dr. Temporary Expense
                            GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                            GenJournalDetail>(session);
                            _gjde1.GenJournalID = _chk;
                            _gjde1.GenJournalID.Approved = true;
                            _gjde1.Account = tmpExp;
                            _gjde1.DebitAmount = Math.Abs(_chk.CheckAmount);
                            debitAmount = debitAmount + _gjde1.DebitAmount;
                            _gjde1.Description = "Payments Made" + " " + mcvd.
                            CheckVoucher.CheckVoucherNo;
                            _gjde1.SubAccountNo = _chk.PayToOrder;
                            _gjde1.SubAccountType = _chk.PayToOrder.ContactType;
                            _gjde1.Approved = true;
                            _gjde1.Save();
                        }
                        else
                        {
                            // If Expense = false Dr. Accounts Payable
                            GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                            GenJournalDetail>(session);
                            _gjde1.GenJournalID = _chk;
                            _gjde1.GenJournalID.Approved = true;
                            switch (_chk.PayToOrder.ContactType)
                            {
                                case ContactTypeEnum.Customer:
                                    _gjde1.Account = ((Customer)_chk.PayToOrder).
                                    Account;
                                    break;
                                case ContactTypeEnum.Vendor:
                                    _gjde1.Account = ((Vendor)_chk.PayToOrder).
                                    Account;
                                    break;
                                //case ContactTypeEnum.Payee:
                                //    break;
                                //case ContactTypeEnum.Employee:
                                //    break;
                                default:
                                    break;
                            }
                            _gjde1.DebitAmount = Math.Abs(_chk.CheckAmount);
                            debitAmount = debitAmount + _gjde1.DebitAmount;
                            _gjde1.Description = "Payments Made" + " " + mcvd.
                            CheckVoucher.CheckVoucherNo;
                            ;
                            _gjde1.SubAccountNo = _chk.PayToOrder;
                            _gjde1.SubAccountType = _chk.PayToOrder.ContactType;
                            _gjde1.Approved = true;
                            _gjde1.Save();
                        }
                        // Credit Cash in Bank
                        GenJournalDetail _gjde2 = ReflectionHelper.CreateObject<
                        GenJournalDetail>(session);
                        _gjde2.GenJournalID = _chk;
                        _gjde2.GenJournalID.Approved = true;
                        _gjde2.Account = _chk.BankCashAccount;
                        _gjde2.CreditAmount = Math.Abs(_chk.CheckAmount);
                        creditAmount = creditAmount + _gjde2.CreditAmount;
                        _gjde2.Description = "Payments Made" + " " + mcvd.
                        CheckVoucher.CheckVoucherNo;
                        _gjde2.SubAccountNo = _chk.PayToOrder;
                        _gjde2.SubAccountType = _chk.PayToOrder.ContactType;
                        _gjde2.ExpenseType = _chk.ExpenseType;
                        _gjde2.SubExpenseType = _chk.SubExpenseType != null ? _chk.SubExpenseType : null;

                        _gjde2.Approved = true;
                        _gjde2.Save();
                    }
                    JournalEntry _je = null;
                    if (thisVoucher.MCheckEffectiveDetails.Count > 0)
                    {
                        _je = ReflectionHelper.CreateObject<JournalEntry>(session);
                        _je.EntryDate = thisVoucher.EntryDate;
                        _je.ReferenceNo = thisVoucher.CheckVoucherNo;
                        _je.Memo = thisVoucher.Memo;
                        _je.Comments = thisVoucher.Comments;
                        _je.Status = JournalEntryStatusEnum.Approved;
                        _je.Save();
                        foreach (MCheckEffectiveDetails mced in
                        thisVoucher.MCheckEffectiveDetails)
                        {
                            if (mced.Expense)
                            {
                                // Dr. Operating Expense
                                GenJournalDetail _gjde1 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde1.GenJournalID = _je;
                                _gjde1.GenJournalID.Approved = true;
                                _gjde1.Account = mced.Account;
                                _gjde1.DebitAmount = Math.Abs(mced.Amount);
                                debitAmount = debitAmount + _gjde1.DebitAmount;
                                _gjde1.Description = mced.Description + (string.IsNullOrEmpty(mced.Description) ? "Adjustment " : " Adjustment ") + mced.
                                MCheckVoucherID.CheckVoucherNo;
                                _gjde1.SubAccountNo = thisVoucher.Payee;
                                _gjde1.SubAccountType = thisVoucher.Payee.
                                ContactType;
                                _gjde1.Approved = true;
                                _gjde1.Save();
                                // Cr. Temp Expense
                                GenJournalDetail _gjde2 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde2.GenJournalID = _je;
                                _gjde2.GenJournalID.Approved = true;
                                _gjde2.Account = tmpExp;
                                _gjde2.CreditAmount = Math.Abs(mced.Amount);
                                creditAmount = creditAmount + _gjde2.CreditAmount;
                                _gjde2.Description = mced.Description + (string.IsNullOrEmpty(mced.Description) ? "Adjustment " : " Adjustment ") + mced.
                                MCheckVoucherID.CheckVoucherNo;
                                _gjde2.SubAccountNo = thisVoucher.Payee;
                                _gjde2.SubAccountType = thisVoucher.Payee.
                                ContactType;
                                _gjde2.Approved = true;
                                _gjde2.Save();
                            }
                            else
                            {
                                // Dr. Account Payable
                                GenJournalDetail _gjde = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde.GenJournalID = _je;
                                _gjde.GenJournalID.Approved = true;
                                switch (thisVoucher.Payee.ContactType)
                                {
                                    case ContactTypeEnum.Customer:
                                        _gjde.Account = ((Customer)
                                        thisVoucher.Payee).Account;
                                        break;
                                    case ContactTypeEnum.Vendor:
                                        _gjde.Account = ((Vendor)
                                        thisVoucher.Payee).Account;
                                        break;
                                    //case ContactTypeEnum.Payee:
                                    //    break;
                                    //case ContactTypeEnum.Employee:
                                    //    break;
                                    default:
                                        break;
                                }
                                _gjde.DebitAmount = Math.Abs(mced.Amount);
                                debitAmount = debitAmount + _gjde.DebitAmount;
                                _gjde.Description = mced.Description + (string.IsNullOrEmpty(mced.Description) ? "Adjustment " : " Adjustment ") + mced.
                                MCheckVoucherID.CheckVoucherNo;
                                _gjde.SubAccountNo = thisVoucher.Payee;
                                _gjde.SubAccountType = thisVoucher.Payee.
                                ContactType;
                                _gjde.Approved = true;
                                _gjde.Save();
                                // Cr. Effective Accounts
                                GenJournalDetail _gjde1 = ReflectionHelper.
                                CreateObject<GenJournalDetail>(session);
                                _gjde1.GenJournalID = _je;
                                _gjde1.GenJournalID.Approved = true;
                                _gjde1.Account = mced.Account;
                                _gjde1.CreditAmount = Math.Abs(mced.Amount);
                                creditAmount = creditAmount + _gjde1.CreditAmount;
                                _gjde1.Description = mced.Description + (string.IsNullOrEmpty(mced.Description) ? "Adjustment " : " Adjustment ") + mced.
                                MCheckVoucherID.CheckVoucherNo;
                                _gjde1.SubAccountNo = thisVoucher.Payee;
                                _gjde1.SubAccountType = thisVoucher.Payee.
                                ContactType;
                                _gjde1.Approved = true;
                                _gjde1.Save();
                            }
                        }
                    }
                    #endregion
                    thisVoucher.Status = MultiCheckStatusEnum.Released;
                    thisVoucher.Save();
                    if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                    {
                        throw new
                            ApplicationException("Accounting entries not balance");
                    }
                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Releasing voucher {0} succesfull.", index);
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
                    "Releasing selected vouchers has been cancelled", "Cancelled",
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
            releaseSelectedVoucherAction.
                Enabled.SetItemValue("Releasing vouchers", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
