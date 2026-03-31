using System;
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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class VoidBankTransaction : ViewController {
        private GenJournalHeader genJournalHeader;
        private SimpleAction voidBankTransaction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public VoidBankTransaction() {
            this.TargetObjectType = typeof(GenJournalHeader);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.VoidTransaction", this.GetType(
            ).Name);
            this.voidBankTransaction = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.voidBankTransaction.Caption = "Void Transaction";
            this.voidBankTransaction.Execute += new 
            SimpleActionExecuteEventHandler(VoidBankTransaction_Execute);
            this.voidBankTransaction.Executed += new EventHandler<
            ActionBaseEventArgs>(VoidBankTransaction_Executed);
            this.voidBankTransaction.ConfirmationMessage = 
            "Do you really want to void this transaction?";
            UpdateActionState(false);
        }
        private void VoidBankTransaction_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            genJournalHeader = this.View.CurrentObject as GenJournalHeader;
            // Make sure that it is a bank transaction
            if (genJournalHeader.SourceType.Code != "CR") {throw new 
                ApplicationException("This is not a bank transaction");} else {
                bool x = false;
                if (genJournalHeader.OperationType.Code == "PY") {x = true;}
                if (genJournalHeader.OperationType.Code == "PR") {x = true;}
                if (!x) {throw new ApplicationException(
                    "This is not a voidable bank transaction");}
            }
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(genJournalHeader);
        }
        private void VoidBankTransaction_Executed(object sender, 
        ActionBaseEventArgs e) {
            //ObjectSpace.ReloadObject(genJournalHeader);
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
        //private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            decimal creditAmount = 0;
            decimal debitAmount = 0;
            UnitOfWork session = CreateUpdatingSession();
            GenJournalHeader _gjHeader = (GenJournalHeader)e.Argument;
            GenJournalHeader thisTrans = session.GetObjectByKey<GenJournalHeader
            >(_gjHeader.Oid);
            try {
                // Create Journal Entry
                JournalEntry journalEntry = ReflectionHelper.CreateObject<
                JournalEntry>(thisTrans.Session);
                journalEntry.EntryDate = DateTime.Now;
                journalEntry.ReferenceNo = thisTrans.SourceNo;
                journalEntry.Memo = "Voided " + thisTrans.Description;
                journalEntry.Status = JournalEntryStatusEnum.Approved;
                journalEntry.Save();
                foreach (GenJournalDetail item in thisTrans.GenJournalDetails) {
                    GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                    GenJournalDetail>(session);
                    _gjde.GenJournalID = journalEntry;
                    _gjde.GenJournalID.Approved = true;
                    _gjde.Account = item.Account;
                    _gjde.DebitAmount = item.CreditAmount;
                    debitAmount = debitAmount + _gjde.DebitAmount;
                    _gjde.CreditAmount = item.DebitAmount;
                    creditAmount = creditAmount + _gjde.CreditAmount;
                    _gjde.Description = "Voided " + thisTrans.Description;
                    _gjde.SubAccountNo = item.SubAccountNo;
                    _gjde.SubAccountType = item.SubAccountType;
                    _gjde.ExpenseType = item.ExpenseType;
                    _gjde.SubExpenseType = item.SubExpenseType;
                    _gjde.Approved = true;
                    _gjde.Save();
                }
                if (thisTrans.SourceType.Code == "CR") {
                    if (thisTrans.OperationType.Code == "PY") {((CheckPayment)
                        thisTrans).Status = CheckStatusEnum.Voided;}
                    if (thisTrans.OperationType.Code == "PR") {((ReceivePayment)
                        thisTrans).Status = PaymentStatusEnum.Voided;}
                }
                thisTrans.Save();
            } finally {
                e.Result = index;
                if (Math.Round(creditAmount, 2) != Math.Round(debitAmount, 2))
                {
                    throw new
                        ApplicationException("Accounting entries not balance");
                }

                CommitUpdatingSession(session);
            }
        }
        //private void BgWorkerProgressChanged(object sender, 
        //ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
        //        DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            //_FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Voiding bank transaction operation has been cancelled", 
                "Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Bank transaction has been successfully voided");
                    ObjectSpace.ReloadObject(genJournalHeader);
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { voidBankTransaction.
            Enabled.SetItemValue("Releasing Check", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
