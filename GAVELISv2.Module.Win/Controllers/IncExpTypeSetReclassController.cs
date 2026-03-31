using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class IncExpTypeSetReclassController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.UpdateExpenseType _Obj;
        private PopupWindowShowAction incExpTypeSetReclass;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public IncExpTypeSetReclassController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.IncExpenseReclassDetail);
            this.TargetViewType = ViewType.ListView;
            string actionID = string.Format("{0}.IncExpTypeSetReclass", this.GetType(
            ).Name);
            this.incExpTypeSetReclass = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.incExpTypeSetReclass.Caption = "Set Category";
            this.incExpTypeSetReclass.TargetObjectsCriteria = "[IsOpen] = True";
            this.incExpTypeSetReclass.TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueForAll;
            this.incExpTypeSetReclass.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            incExpTypeSetReclass_CustomizePopupWindowParams);
            this.incExpTypeSetReclass.Execute += new
            PopupWindowShowActionExecuteEventHandler(updateExpenseType_Execute);
            this.incExpTypeSetReclass.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(incExpTypeSetReclass_ExecuteCompleted);
        }
        void incExpTypeSetReclass_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }
        private void incExpTypeSetReclass_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _Obj = new BusinessObjectsAlias.UpdateExpenseType();
            e.View = Application.CreateDetailView(_ObjectSpace, _Obj, true);
        }
        private void updateExpenseType_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace2 = Application.CreateObjectSpace();
            IList selected = ((DevExpress.ExpressApp.ListView)this.View).
                    SelectedObjects;

            _FrmProgress = new ProgressForm("Setting...", selected.Count,
                        "Setting expense type of detail {0} of {1} ");
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

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;

            try
            {
                BusinessObjectsAlias.ExpenseType oExpType = null;
                BusinessObjectsAlias.SubExpenseType oSubExpType = null;
                if (_Obj.ToCategory != null)
                {
                    oExpType = session.GetObjectByKey<BusinessObjectsAlias.ExpenseType>(_Obj.ToCategory.Oid);
                }
                if (_Obj.ToSubCategory != null)
                {
                    oSubExpType = session.GetObjectByKey<BusinessObjectsAlias.SubExpenseType>(_Obj.ToSubCategory.Oid);
                }
                foreach (var item in trans)
                {
                    index++;

                    #region Algorithms here

                    BusinessObjectsAlias.IncExpenseReclassDetail ierd = session.GetObjectByKey<BusinessObjectsAlias.IncExpenseReclassDetail>((item as BusinessObjectsAlias.IncExpenseReclassDetail).Oid);
                    ierd.ToCategory = oExpType ?? null;
                    ierd.ToSubCategory = oSubExpType ?? null;
                    ierd.Done = false;
                    ierd.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Setting expense type of detail {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.Count)
                {
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

        private ObjectSpace GetUpdatingObjectSpace()
        {
            return ObjectSpace as ObjectSpace;
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
                    "Updating expense type is cancelled.", "Cancelled",
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
                    "Updating expense type has been successfull.");
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
