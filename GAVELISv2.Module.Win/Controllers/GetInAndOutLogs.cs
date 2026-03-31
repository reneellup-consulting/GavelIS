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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class GetInAndOutLogs : ViewController {
        private SimpleAction getInAndOutLogs;
        private StaffPayrollBatch _StaffPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GetInAndOutLogs() {
            this.TargetObjectType = typeof(StaffPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "StaffPayrollBatch.GetInAndOutLogs";
            this.getInAndOutLogs = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.getInAndOutLogs.Caption = "Get In/Outs";
            this.getInAndOutLogs.Execute += new SimpleActionExecuteEventHandler(
            GetInAndOutLogs_Execute);
        }
        private void GetInAndOutLogs_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _StaffPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
CurrentObject as StaffPayrollBatch;

            try
            {
                for (int i = _StaffPayrollBatch.CheckInAndOuts.Count - 1;
                i >= 0; i--)
                {
                    _StaffPayrollBatch.CheckInAndOuts[i].PayrollBatchID=null;
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();
            ICollection employees;
            DevExpress.Xpo.Metadata.XPClassInfo employeesClass = _StaffPayrollBatch.Session.GetClassInfo(typeof(
            Employee));
            string critString = _StaffPayrollBatch.BatchType.EmployeeFilter != string.Empty ? _StaffPayrollBatch.BatchType.EmployeeFilter + " And [Inactive] = False" : "[Inactive] = False";
            CriteriaOperator criteria = CriteriaOperator.Parse(critString);
            SortingCollection sortProps = new SortingCollection(null);
            employees = _StaffPayrollBatch.Session.GetObjects(employeesClass
            , criteria, sortProps, 0, false, true);

            if (employees.Count == 0)
            {
                throw new UserFriendlyException("There are no emplyees found");
            }

            _FrmProgress = new ProgressForm("Extracting records...", employees.Count,
"Employee records processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_StaffPayrollBatch);
            _FrmProgress.ShowDialog();

        }

        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            StaffPayrollBatch _payrollBatch = (StaffPayrollBatch)e.Argument;
            StaffPayrollBatch thisIS = session.GetObjectByKey<StaffPayrollBatch>(
            _payrollBatch.Oid);
            int aCount = 0;

            try
            {
                // Employees filtration start
                ICollection employees;
                DevExpress.Xpo.Metadata.XPClassInfo employeesClass = thisIS.Session.GetClassInfo(typeof(
                Employee));
                string critString = thisIS.BatchType.EmployeeFilter != string.Empty ? thisIS.BatchType.EmployeeFilter + " And [Inactive] = False" : "[Inactive] = False";
                CriteriaOperator criteria = CriteriaOperator.Parse(critString);
                SortingCollection sortProps = new SortingCollection(null);
                employees = thisIS.Session.GetObjects(employeesClass
                , criteria, sortProps, 0, false, true);
                aCount=employees.Count;
                // Employees filtration end

                // Ins and Outs records filtration start
                ICollection insAndOuts;
                DevExpress.Xpo.Metadata.XPClassInfo insAndOutsClass = thisIS.Session.GetClassInfo(typeof(
                CheckInAndOut));
                DateTime fDate = new DateTime(thisIS.PeriodStart.Year, thisIS.PeriodStart.Month, thisIS.PeriodStart.Day, 01, 0, 0);
                DateTime tDate = new DateTime(thisIS.PeriodEnd.Year, thisIS.PeriodEnd.Month, thisIS.PeriodEnd.Day, 23, 59, 59);
                // Ins and Outs records filtration end

                foreach (Employee emp in employees)
                {
                    string critString1 = "[Date] >= #" + fDate + "# And [Date] <= #" + tDate + "# And [Status] = 'Open' And [Employee.IDNo] = '" + emp.IDNo + "'";
                    CriteriaOperator criteria1 = CriteriaOperator.Parse(critString1);
                    SortingCollection sortProps1 = null;
                    insAndOuts = thisIS.Session.GetObjects(insAndOutsClass
                   , criteria1, sortProps1, 0, false, true);

                    foreach (CheckInAndOut io in insAndOuts)
                    {
                        io.PayrollBatchID=thisIS;
                        io.Save();
                    }
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, string.Empty);
                    index++;

                }

            }
            finally
            {
                if (index == aCount)
                {
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
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
        private void BgWorkerProgressChanged(object sender, 
        ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
                DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Extraction of Staff's Check In/Out's records is cancelled", 
                "Cancelled", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Extraction of Staff's Check In/Out's records has been successfully generated."
                    );
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
