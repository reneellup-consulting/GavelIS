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
//using DevExpress.ExpressApp.Demos;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ApplyCreditController : ViewController
    {
        private PayBill payBill;
        private SimpleAction applyCreditAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        public ApplyCreditController()
        {
            this.TargetObjectType = typeof(PayBill);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.ApplyCredit", this.GetType().
            Name);
            this.applyCreditAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.applyCreditAction.Caption = "Apply Credit Selected";
            this.applyCreditAction.Execute += new
            SimpleActionExecuteEventHandler(ApplyCreditAction_Execute);
            this.applyCreditAction.Executed += new EventHandler<
            ActionBaseEventArgs>(ApplyCreditAction_Executed);
            UpdateActionState(false);
        }
        private void ApplyCreditAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            payBill = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PayBill;
            if (Math.Round(payBill.Difference, 2) != 0)
            {
                throw new
                    ApplicationException(
                    "The difference must be zero in order to apply credit");
            }

            payBill.Save();
            ObjectSpace.CommitChanges();
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            //_BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(payBill);
        }
        private void ApplyCreditAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
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
            UnitOfWork session = CreateUpdatingSession();
            PayBill _payBill = (PayBill)e.Argument;
            PayBill thisPayBill = session.GetObjectByKey<PayBill>(_payBill.Oid);
            try
            {

            }
            finally
            {
                CommitUpdatingSession(session);
                session.Dispose();
            }
        }
        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e)
        {
            //_FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                    "Apply credit operation has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("Credit has been successfully applied");
                    ObjectSpace.Refresh();
                }
            }
        }
        //private void FrmProgressCancelClick(object sender, EventArgs e) { 
        //    _BgWorker.CancelAsync(); }
        private void UpdateActionState(bool inProgress)
        {
            applyCreditAction.
                Enabled.SetItemValue("Applying credit", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
