using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Win.SystemModule;
using BusinessObjectsAlias = GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CorrectFacilityAndDepartmentController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private IObjectSpace _ObjectSpace2;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private BusinessObjectsAlias.CorrectFacilityAndDepartment _Obj;
        private PopupWindowShowAction correctFacilityAndDepartment;
        public CorrectFacilityAndDepartmentController()
        {
            this.TargetObjectType = typeof(BusinessObjectsAlias.IncomeAndExpense02);
            this.TargetViewType = ViewType.Any;
            string actionID = string.Format("{0}.CorrectFacilityAndDepartment", this.GetType().
            Name);
            this.correctFacilityAndDepartment = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.correctFacilityAndDepartment.Caption = "Correct Fac. && Dept.";
            this.correctFacilityAndDepartment.CustomizePopupWindowParams += new
            CustomizePopupWindowParamsEventHandler(
            CorrectFacilityAndDepartment_CustomizePopupWindowParams);
            this.correctFacilityAndDepartment.Execute += new
            PopupWindowShowActionExecuteEventHandler(CorrectFacilityAndDepartment_Execute);
        }
        private void CorrectFacilityAndDepartment_CustomizePopupWindowParams(object sender,
        CustomizePopupWindowParamsEventArgs e)
        {
            _ObjectSpace = Application.CreateObjectSpace();
            _ObjectSpace2 = Application.CreateObjectSpace();
            _Obj = new BusinessObjectsAlias.CorrectFacilityAndDepartment();
            e.View = Application.CreateDetailView(_ObjectSpace,
            "CorrectFacilityAndDepartment_DetailView", true, _Obj);
        }
        private void CorrectFacilityAndDepartment_Execute(object sender,
        PopupWindowShowActionExecuteEventArgs e)
        {
            if (this.View.GetType() == typeof(ListView))
            {
                var selwo = ((DevExpress.ExpressApp.ListView)this.View).
            SelectedObjects;
                _FrmProgress = new ProgressForm("Correcting entry...", selwo.Count,
            "Entries corrected {0} of {1} ");
                _FrmProgress.CancelClick += FrmProgressCancelClick;
                _BgWorker = new System.ComponentModel.BackgroundWorker
                {
                    WorkerSupportsCancellation = true,
                    WorkerReportsProgress = true
                };
                _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
                _BgWorker.ProgressChanged += BgWorkerProgressChanged;
                _BgWorker.DoWork += BgWorkerDoWork;
                _BgWorker.RunWorkerAsync(selwo);
                _FrmProgress.ShowDialog();
            }
            else
            {
                BusinessObjectsAlias.IncomeAndExpense02 incomeExp = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as BusinessObjectsAlias.IncomeAndExpense02;
                BusinessObjectsAlias.IncomeAndExpense02 objectVar = _ObjectSpace2.GetObject(incomeExp);
                objectVar.Facility = _Obj.Facility != null ? _ObjectSpace2.GetObject(_Obj.Facility) : null;
                objectVar.Department = _Obj.Department != null ? _ObjectSpace2.GetObject(_Obj.Department) : null;
                objectVar.FacilityHead = _Obj.FacilityHead != null ? _ObjectSpace2.GetObject(_Obj.FacilityHead) : null;
                objectVar.DepartmentInCharge = _Obj.DepartmentInCharge != null ? _ObjectSpace2.GetObject(_Obj.DepartmentInCharge) : null;
                objectVar.Save();
                _ObjectSpace2.CommitChanges();
                ObjectSpace.Refresh();
            }
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            System.Collections.IList args = e.Argument as System.Collections.IList;
            try
            {
                foreach (BusinessObjectsAlias.IncomeAndExpense02 item in args)
                {
                    index++;
                    _message = string.Format("Correcting entry {0} succesfull.",
                    args.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region MyRegion
                    BusinessObjectsAlias.IncomeAndExpense02 inc = session.GetObjectByKey<BusinessObjectsAlias.IncomeAndExpense02>(item.Oid);
                    //Console.WriteLine(inc.SourceNo);
                    inc.Facility = _Obj.Facility != null ? session.GetObjectByKey<BusinessObjectsAlias.Facility>(_Obj.Facility.Oid) : null;
                    inc.Department = _Obj.Department != null ? session.GetObjectByKey<BusinessObjectsAlias.Department>(_Obj.Department.Oid) : null;
                    inc.FacilityHead = _Obj.FacilityHead != null ? session.GetObjectByKey<BusinessObjectsAlias.Employee>(_Obj.FacilityHead.Oid) : null;
                    inc.DepartmentInCharge = _Obj.DepartmentInCharge != null ? session.GetObjectByKey<BusinessObjectsAlias.Employee>(_Obj.DepartmentInCharge.Oid) : null;
                    inc.Save();
                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
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
                    e.Result = index;
                    CommitUpdatingSession(session);
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
                "Correcting entries operation has been cancelled", "Cancelled",
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
                    " has been successfully corrected");
                    //ObjectSpace.ReloadObject(_TireForSale);
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
            correctFacilityAndDepartment.
            Enabled.SetItemValue("Correcting entries", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
