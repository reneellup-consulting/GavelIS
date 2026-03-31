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
    public partial class AddDetailsForReclassController : ViewController
    {
        private PopupWindowShowAction addDetails;
        private IncExpenseReclass _incExpenseReclass;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public AddDetailsForReclassController()
        {
            this.TargetObjectType = typeof(IncExpenseReclass);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "IncExpenseReclass.AddDetails";
            this.addDetails = new PopupWindowShowAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.addDetails.Caption = "Add Details";
            this.addDetails.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            addDetails_CustomizePopupWindowParams);
            this.addDetails.Execute += new
            PopupWindowShowActionExecuteEventHandler(AddTrips_Execute
            );
            this.addDetails.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(addDetails_ExecuteCompleted);
        }

        void addDetails_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }

        private void addDetails_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _incExpenseReclass = ((DevExpress.ExpressApp.DetailView)this.View
            ).CurrentObject as IncExpenseReclass;
            this.ObjectSpace.CommitChanges();
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = "IncomeAndExpense02_SelReclass";
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(IncomeAndExpense02), listViewId)
            ;
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
        private void AddTrips_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            int count = e.PopupWindow.View.SelectedObjects.Count;

            _FrmProgress = new ProgressForm("Adding...", count,
                        "Adding detail {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(e.PopupWindow.View.SelectedObjects);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;

            IncExpenseReclass iercls = session.GetObjectByKey<IncExpenseReclass>(
                _incExpenseReclass.Oid);

            try
            {
                foreach (IncomeAndExpense02 item in trans)
                {
                    index++;

                    #region Algorithms here...

                    if (iercls.IncExpenseReclassDetails.Where(o => o.LineId.Oid == item.Oid).Count() != 0)
                    {
                        _message = string.Format("Adding detail {0} succesfull.", index);
                        _BgWorker.ReportProgress(1, _message);
                        continue;
                    }
                    IncExpenseReclassDetail ierdet = ReflectionHelper.CreateObject<IncExpenseReclassDetail>(session);
                    ierdet.HeaderId = iercls;
                    ierdet.EntryDate = item.EntryDate;
                    ierdet.LineId = session.GetObjectByKey<IncomeAndExpense02>(item.Oid);
                    ierdet.Payee = ierdet.LineId.Payee;
                    ierdet.Description1 = item.Description1;
                    ierdet.Description2 = item.Description2;
                    ierdet.Income = item.Income;
                    ierdet.Expense = item.Expense;
                    ierdet.Category = ierdet.LineId.Category;
                    ierdet.SubCategory = item.SubCategory != null ? ierdet.LineId.SubCategory : null;
                    ierdet.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    _message = string.Format("Adding detail {0} succesfull.", index);
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
                    "Adding details is cancelled.", "Cancelled",
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
                    XtraMessageBox.Show(
                    "Adding details has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
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
