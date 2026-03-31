using System;
using System.Linq;
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
    public partial class CompleteIncExpenseReclassController : ViewController
    {
        private SimpleAction completeIncExpenseReclass;
        private IncExpenseReclass _IncExpenseReclass;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public CompleteIncExpenseReclassController()
        {
            this.TargetObjectType = typeof(IncExpenseReclass);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "IncExpenseReclass.Complete";
            this.completeIncExpenseReclass = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.completeIncExpenseReclass.TargetObjectsCriteria = "[Status] = 'Approved'";
            this.completeIncExpenseReclass.Caption = "Complete";
            this.completeIncExpenseReclass.Execute += new
            SimpleActionExecuteEventHandler(
            postDriverPayroll_Execute);
        }
        private void postDriverPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _IncExpenseReclass = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as IncExpenseReclass;
            ObjectSpace.CommitChanges();
            if (_IncExpenseReclass.IncExpenseReclassDetails.Count == 0)
            {
                throw new UserFriendlyException("There are no details to complete!");
            }

            _FrmProgress = new ProgressForm("Completing details...", _IncExpenseReclass.IncExpenseReclassDetails.Count,
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
            _BgWorker.RunWorkerAsync(_IncExpenseReclass.IncExpenseReclassDetails);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<IncExpenseReclassDetail> args = (XPCollection<IncExpenseReclassDetail>)e.Argument;
            IncExpenseReclass ierc = session.GetObjectByKey<IncExpenseReclass>(_IncExpenseReclass.Oid);
            try
            {
                foreach (var item in args)
                {
                    index++;
                    _message = string.Format("Completing detail {0} successful.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    if (item.Done)
                    {
                        continue;
                    }

                    IncExpenseReclassDetail oItem = session.GetObjectByKey<IncExpenseReclassDetail>(item.Oid);
                    IncomeAndExpense02 oIncExp2 = session.GetObjectByKey<IncomeAndExpense02>(oItem.LineId.Oid);

                    if (oItem.ToCategory != null)
                    {
                        if (oIncExp2.SubCategory != null)
                        {
                            var exist = oItem.ToCategory.SubExpenseTypes.Where(o => o.Oid == oIncExp2.SubCategory.Oid).FirstOrDefault();
                            if (exist == null)
                            {
                                oIncExp2.SubCategory = null;
                            }
                        }
                        oIncExp2.Corrected = true;
                        oIncExp2.DateCorrected = DateTime.Now;
                        oIncExp2.CorrectExpenseType = oIncExp2.Category;
                        oIncExp2.Category = oItem.ToCategory;
                    }
                    if (oItem.ToSubCategory != null)
                    {
                        oIncExp2.Corrected = true;
                        oIncExp2.DateCorrected = DateTime.Now;
                        oIncExp2.CorrectSubExpenseType = oIncExp2.SubCategory;
                        oIncExp2.SubCategory = oItem.ToSubCategory;
                    }

                    oIncExp2.Save();

                    oItem.Done = true;
                    oItem.Save();

                    session.CommitChanges();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        ierc.Remarks = "Completion was cancelled";
                        ierc.Save();
                        session.CommitChanges();
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }
            }
            finally
            {
                if (index == args.Count)
                {
                    ierc.Status = IncExpReclassStateEnum.Completed;
                    ierc.Remarks = "Has been completed successfully";
                    CommitUpdatingSession(session);
                    e.Result = index;
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
                    "Completing details is cancelled.", "Cancelled",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                    MessageBoxIcon.Exclamation);
                ObjectSpace.Refresh();
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
                    "Details has been successfully completed.");
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
