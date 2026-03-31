using System;
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
    public partial class GenerateOtherDriverDeductions : ViewController
    {
        private SimpleAction generateOtherDriverDeductions;
        private DriverPayrollBatch _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public GenerateOtherDriverDeductions()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.GenerateOtherDriverDeductions";
            this.generateOtherDriverDeductions = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateOtherDriverDeductions.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateOtherDriverDeductions_Execute);
        }
        private void GenerateOtherDriverDeductions_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch;

            try
            {
                for (int i = _DriverPayrollBatch.PayrollDeductionOthers.Count - 1;
                i >= 0; i--)
                {
                    _DriverPayrollBatch.PayrollDeductionOthers[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();

            string critString = string.Empty;
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
            ICollection driversType;
            DevExpress.Xpo.Metadata.XPClassInfo driversClass;
            driversClass = _DriverPayrollBatch.Session.GetClassInfo(typeof(
            Employee));
            criteria = CriteriaOperator.Parse(_DriverPayrollBatch.BatchType.EmployeeFilter);
            sortProps = new SortingCollection(null);
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, _DriverPayrollBatch.Session.TypesManager);
            driversType = _DriverPayrollBatch.Session.GetObjects(driversClass
            , criteria, sortProps, 0, false, true);

            if (driversType.Count == 0)
            {
                throw new UserFriendlyException("There are no drivers found");
            }

            _FrmProgress = new ProgressForm("Generating data...", driversType.Count,
            "Drivers processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_DriverPayrollBatch);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            DriverPayrollBatch _payrollBatch = (DriverPayrollBatch)e.Argument;
            DriverPayrollBatch thisIS = session.GetObjectByKey<DriverPayrollBatch>(
            _payrollBatch.Oid);
            int aCount = 0;
            
            try
            {
                string critString = string.Empty;
                DevExpress.Data.Filtering.CriteriaOperator criteria;
                DevExpress.Xpo.SortingCollection sortProps;
                DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
                ICollection driversType;
                DevExpress.Xpo.Metadata.XPClassInfo driversClass;
                driversClass = thisIS.Session.GetClassInfo(typeof(
                Employee));
                criteria = CriteriaOperator.Parse(thisIS.BatchType.EmployeeFilter);
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
                false, thisIS.Session.TypesManager);
                driversType = thisIS.Session.GetObjects(driversClass
                , criteria, sortProps, 0, false, true);
                aCount = driversType.Count;

                foreach (Employee item in driversType)
                {
                    if (_DriverPayrollBatch.BatchType.IncludeOtherDed)
                    {
                        foreach (EmpOtherDed ed in item.EmpOtherDeds)
                        {
                            if (!ed.Paid)
                            {
                                PayrollDeductionOther pdo = ReflectionHelper.CreateObject<PayrollDeductionOther>(thisIS.Session);
                                pdo.PayrollBatchID = thisIS;
                                pdo.Employee = item;
                                pdo.DeductionDate = _DriverPayrollBatch.EntryDate;
                                pdo.DeductionType = DeductionType.Other;
                                pdo.DeductionCode = ed.DedCode;
                                pdo.AdvanceEntryDate = ed.EntryDate;
                                pdo.Explanation = ed.Explanation;
                                pdo.RefNo = ed.RefNo;
                                pdo.PrevBalance = ed.Balance;
                                if (ed.Balance < ed.Deduction)
                                {
                                    pdo.Amount = ed.Balance;
                                }
                                else
                                {
                                    pdo.Amount = ed.Deduction;
                                }
                                pdo.Balance = ed.Balance - pdo.Amount;
                                pdo.Save();

                            }
                        }
                    }
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
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
                    "Generation of payroll other deductions data is cancelled.", "Cancelled",
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
                    "Payroll other deductions data has been successfully generated.");
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
