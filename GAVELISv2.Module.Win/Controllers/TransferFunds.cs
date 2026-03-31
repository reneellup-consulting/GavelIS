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
    public partial class TransferFunds : ViewController {
        private FundTransfer fundTransfer;
        private SimpleAction transferFunds;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public TransferFunds() {
            this.TargetObjectType = typeof(FundTransfer);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.TransferFunds", this.GetType().
            Name);
            this.transferFunds = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.transferFunds.Caption = "Transfer";
            this.transferFunds.Execute += new SimpleActionExecuteEventHandler(
            TransferFunds_Execute);
            this.transferFunds.Executed += new EventHandler<ActionBaseEventArgs>
            (TransferFunds_Executed);
            this.transferFunds.ConfirmationMessage = 
            "Do you really want to do this fund transfer?";
            UpdateActionState(false);
        }
        private void TransferFunds_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            fundTransfer = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FundTransfer;
            if (fundTransfer.AmountToTransfer == 0) {throw new 
                ApplicationException("There is nothing to transfer");}
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(fundTransfer);
        }
        private void TransferFunds_Executed(object sender, ActionBaseEventArgs e
        ) {
            //ObjectSpace.ReloadObject(deposit);
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
            FundTransfer _transfer = (FundTransfer)e.Argument;
            FundTransfer thisTransfer = session.GetObjectByKey<FundTransfer>(
            _transfer.Oid);
            try {
                // Create fund transfer offset instance
                FundTransfer transOff = new FundTransfer(session, session.
                FindObject<OperationType>(new BinaryOperator("Code", "FP")));
                transOff.EntryDate = thisTransfer.EntryDate;
                transOff.SourceNo = thisTransfer.SourceNo + "-OS";
                transOff.TransferFundsFrom = thisTransfer.TransferFundsFrom;
                transOff.TransferFundsTo = thisTransfer.TransferFundsTo;
                transOff.AmountToTransfer = thisTransfer.AmountToTransfer;
                transOff.Memo = "Transfer of Funds from " + thisTransfer.TransferFundsFrom.Name;
                transOff.Status = FundTransferStatusEnum.Transferred;
                transOff.Save();
                // Credit from account
                GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                GenJournalDetail>(session);
                _gjde.GenJournalID = thisTransfer;
                _gjde.GenJournalID.Approved = true;
                _gjde.Account = thisTransfer.TransferFundsFrom;
                _gjde.CreditAmount = thisTransfer.AmountToTransfer;
                creditAmount = creditAmount + _gjde.CreditAmount;
                _gjde.Description = "Fund Transfer " + thisTransfer.SourceNo;
                _gjde.Approved = true;
                _gjde.Save();
                // Debit to account
                GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                GenJournalDetail>(session);
                _gjde1.GenJournalID = thisTransfer;
                _gjde1.GenJournalID.Approved = true;
                _gjde1.Account = thisTransfer.TransferFundsTo;
                _gjde1.DebitAmount = thisTransfer.AmountToTransfer;
                debitAmount = debitAmount + _gjde1.DebitAmount;
                _gjde1.Description = "Fund Transfer " + thisTransfer.SourceNo;
                _gjde1.Approved = true;
                _gjde1.Save();
                thisTransfer.Status = FundTransferStatusEnum.Transferred;
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
                "Fund transfer operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Funds has been successfully transferred");
                    ObjectSpace.ReloadObject(fundTransfer);
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { transferFunds.Enabled.
            SetItemValue("Transferring funds", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
