using System;
using System.Windows.Forms;
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
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class MigrateToGensetIncomeController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private StanfilcoTrip _StanfilcoTrip;
        private SimpleAction _MigrateToGensetIncomeAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public MigrateToGensetIncomeController()
        {
            this.TargetObjectType = typeof(StanfilcoTrip);
            this.TargetViewType = ViewType.ListView;
            string actionID = "MigrateToGensetIncomeActionID";
            _MigrateToGensetIncomeAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            _MigrateToGensetIncomeAction.Caption = "Migrate to Genset Income";
            _MigrateToGensetIncomeAction.Execute += new SimpleActionExecuteEventHandler(_MigrateToGensetIncomeAction_Execute);
        }

        void _MigrateToGensetIncomeAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            XPClassInfo classInfo = ((ObjectSpace)ObjectSpace).Session.GetClassInfo<StanfilcoTrip>();
            CriteriaOperator criteria = CriteriaOperator.Parse("[EntryDate] < #2018-02-11#");
            SortingCollection sorting = new SortingCollection();
            sorting.Add(new SortProperty("EntryDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
            var data = ((ObjectSpace)ObjectSpace).Session.GetObjects(classInfo, criteria, sorting, 0, false, true);

            if (data == null && data.Count == 0)
            {
                throw new ApplicationException("There are no trips found");
            }
            _FrmProgress = new ProgressForm("Migrating income entries...", data.Count,
                "Migrating entry {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(data);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            ICollection stfs = (ICollection)e.Argument;
            try
            {
                foreach (var item in stfs)
                {
                    index++;
                    _message = string.Format("Migrating entry {0} succesfull.",
                    stfs.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    StanfilcoTrip oStft = session.GetObjectByKey<StanfilcoTrip>(((StanfilcoTrip)item).Oid);
                    // Delete genset entry IncomeExpense after Feb. 11, 2018
                    if (oStft.GensetEntries.Count > 0)
                    {
                        foreach (GensetEntry gsnt in oStft.GensetEntries)
                        {
                            var iexp = session.FindObject<IncomeAndExpense02>(BinaryOperator.Parse("[SourceID.Oid]=?", gsnt.Oid));
                            if (iexp != null)
                            {
                                ExpenseType defExpType = session.FindObject<ExpenseType>(CriteriaOperator.Parse("[Code] = '0004'"));
                                iexp.Category = defExpType;
                                iexp.Save();
                            }
                        }
                    }

                    oStft.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                }
            }
            finally
            {
                if (index == stfs.Count)
                {
                    e.Result = index;
                    //CommitUpdatingSession(session);
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
                "Migrating entry has been cancelled", "Cancelled",
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
                    " entries are successfully migrated.");
                    //ObjectSpace.ReloadObject(invoice);
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
            _MigrateToGensetIncomeAction.
            Enabled.SetItemValue("Migrating genset entries.", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
