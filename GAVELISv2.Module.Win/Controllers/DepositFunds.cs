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
    public partial class DepositFunds : ViewController {
        private Deposit deposit;
        private SimpleAction depositFunds;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        //private ProgressForm _FrmProgress;
        public DepositFunds() {
            this.TargetObjectType = typeof(Deposit);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.DepositFunds", this.GetType().
            Name);
            this.depositFunds = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.depositFunds.Caption = "Deposit Funds";
            this.depositFunds.Execute += new SimpleActionExecuteEventHandler(
            DepositFunds_Execute);
            this.depositFunds.Executed += new EventHandler<ActionBaseEventArgs>(
            DepositFunds_Executed);
            this.depositFunds.ConfirmationMessage = 
            "Do you really want to deposit the selected entries?";
            UpdateActionState(false);
        }
        private void DepositFunds_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            deposit = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as Deposit;
            if (deposit.Count == 0) {throw new ApplicationException(
                "There is nothing to deposit");}
            if (deposit.TotalDeposit == 0) {throw new ApplicationException(
                "There is nothing to deposit");}
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(deposit);
        }
        private void DepositFunds_Executed(object sender, ActionBaseEventArgs e) 
        {
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
            UnitOfWork session = CreateUpdatingSession();
            Deposit _deposit = (Deposit)e.Argument;
            Deposit thisDeposit = session.GetObjectByKey<Deposit>(_deposit.Oid);
            ReceivePayment receivePayment = null;
            try {
                foreach (DepositDetail item in thisDeposit.DepositDetails) {
                    receivePayment = session.GetObjectByKey<ReceivePayment>(item
                    .SourceID);
                    receivePayment.Deposited = true;
                    receivePayment.Save();
                }
                // Debit deposit to account
                GenJournalDetail _gjde = ReflectionHelper.CreateObject<
                GenJournalDetail>(session);
                _gjde.GenJournalID = thisDeposit;
                _gjde.GenJournalID.Approved = true;
                _gjde.Account = thisDeposit.DepositToAccount;
                _gjde.DebitAmount = thisDeposit.TotalDeposit.Value;
                _gjde.Description = "Deposited " + thisDeposit.SourceNo;
                _gjde.Approved = true;
                _gjde.Save();
                // Credit funds from account
                GenJournalDetail _gjde1 = ReflectionHelper.CreateObject<
                GenJournalDetail>(session);
                _gjde1.GenJournalID = thisDeposit;
                _gjde1.GenJournalID.Approved = true;
                _gjde1.Account = thisDeposit.AccountToDeposit;
                _gjde1.CreditAmount = thisDeposit.TotalDeposit.Value;
                _gjde1.Description = "Deposited " + thisDeposit.SourceNo;
                _gjde1.Approved = true;
                _gjde1.Save();
                thisDeposit.Status = DepositStatusEnum.Deposited;
            } finally {
                e.Result = index;
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
                "Deposit funds operation has been cancelled", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show("Funds has been successfully deposited")
                    ;
                    ObjectSpace.ReloadObject(deposit);
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress) { depositFunds.Enabled.
            SetItemValue("Depositing funds", !inProgress); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
