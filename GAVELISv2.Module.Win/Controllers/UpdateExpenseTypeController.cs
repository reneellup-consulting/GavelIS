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
    public partial class UpdateExpenseTypeController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private BusinessObjectsAlias.UpdateExpenseType _Obj;
        private PopupWindowShowAction updateExpenseType;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public UpdateExpenseTypeController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.ISetIncomeExpense);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.UpdateExpenseType", this.GetType(
            ).Name);
            this.updateExpenseType = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.updateExpenseType.Caption = "Update Expense Type";
            this.updateExpenseType.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            updateExpenseType_CustomizePopupWindowParams);
            this.updateExpenseType.Execute += new
            PopupWindowShowActionExecuteEventHandler(updateExpenseType_Execute);
            this.updateExpenseType.ExecuteCompleted += new EventHandler<ActionBaseEventArgs>(updateExpenseType_ExecuteCompleted);
        }
        void updateExpenseType_ExecuteCompleted(object sender, ActionBaseEventArgs e)
        {
            this.ObjectSpace.CommitChanges();
        }
        private void updateExpenseType_CustomizePopupWindowParams(object sender,
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
            IList selected = null;
            if (this.View.GetType() == typeof(ListView))
            {
                selected = ((DevExpress.ExpressApp.ListView)this.View).
                    SelectedObjects;
            }
            else
            {
                selected.Add(this.View.CurrentObject);
            }

            _FrmProgress = new ProgressForm("Updating...", selected.Count,
                        "Updating expense type of detail {0} of {1} ");
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
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            //ObjectSpace objectSpace = GetUpdatingObjectSpace();
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
                Object oItem = null;
                foreach (var item in trans)
                {
                    index++;

                    #region Algorithms here

                    foreach (XPMemberInfo property in session.GetClassInfo(item).PersistentProperties)
                    {
                        Object _oid = null;
                        if (property.Name == "Oid")
                        {
                            _oid = property.GetValue(item);
                            oItem = session.FindObject(item.GetType(), CriteriaOperator.Parse("[Oid]=?", _oid));
                        }
                        if (property.MemberType == typeof(BusinessObjectsAlias.ExpenseType) && property.Name == "ExpenseType" && oExpType != null)
                        {
                            property.SetValue(oItem, oExpType);
                        }
                        if (property.MemberType == typeof(BusinessObjectsAlias.SubExpenseType) && property.Name == "SubExpenseType" && oSubExpType != null)
                        {
                            property.SetValue(oItem, oSubExpType);
                        }
                    }

                    (oItem as XPCustomObject).Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Updating expense type of detail {0} succesfull.", index);
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

        private ObjectSpace GetUpdatingObjectSpace() {
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
