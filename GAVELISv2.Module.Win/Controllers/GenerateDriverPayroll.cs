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
    public partial class GenerateDriverPayroll : ViewController
    {
        private SimpleAction generateDriverPayrollAction;
        private DriverPayrollBatch _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public GenerateDriverPayroll()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch.GenerateDriverPayroll";
            this.generateDriverPayrollAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateDriverPayrollAction.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateDriverPayrollActionAction_Execute);
        }
        private void GenerateDriverPayrollActionAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as DriverPayrollBatch;

            try
            {
                for (int i = _DriverPayrollBatch.DriverPayrolls.Count - 1;
                i >= 0; i--)
                {
                    _DriverPayrollBatch.DriverPayrolls[i].Delete(
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
            //criteria = CriteriaOperator.Parse(_DriverPayrollBatch.BatchType.EmployeeFilter + " And [Inactive]=false");
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
                //criteria = CriteriaOperator.Parse(thisIS.BatchType.EmployeeFilter + " And [Inactive]=false");
                criteria = CriteriaOperator.Parse(thisIS.BatchType.EmployeeFilter);
                sortProps = new SortingCollection(null);
                patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
                false, thisIS.Session.TypesManager);
                driversType = thisIS.Session.GetObjects(driversClass
                , criteria, sortProps, 0, false, true);
                aCount = driversType.Count;

                foreach (Employee item in driversType)
                {
                    bool IsHolcim = false;
                    DriverPayroll dp = ReflectionHelper.CreateObject<DriverPayroll>(thisIS.Session);
                    dp.PayrollBatchID = thisIS;
                    dp.Employee = item;
                    foreach (DriverPayrollTrip dt in thisIS.DriverPayrollTrips)
                    {
                        if (item == dt.Driver)
                        {
                            dp.Basic = dp.Basic + dt.Basic;
                            dp.AdlMiscExp = dp.AdlMiscExp + dt.AdlMiscExp;
                            dp.MiscExp = dp.MiscExp + dt.MiscExp;
                            dp.KDs = dp.KDs + dt.KDs;
                            dp.Shunting = dp.Shunting + dt.Shunting;

                            DriverPayrollTripLine dptLine=ReflectionHelper.CreateObject<DriverPayrollTripLine>(thisIS.Session);
                            dptLine.DriverPayrollID=dp;
                            dptLine.TripDate=dt.TripDate;
                            dptLine.DocumentNo=dt.DocumentNo;
                            dptLine.Driver=dt.Driver;
                            dptLine.Basic=dt.Basic;
                            dptLine.AdlMiscExp=dt.AdlMiscExp;
                            dptLine.MiscExp=dt.MiscExp;
                            dptLine.KDs=dt.KDs;
                            dptLine.Shunting=dt.Shunting;
                            dptLine.Destination = dt.ShuntingTo;
                            if (dt.Customer.ToLower().Contains("holcim"))
                            {
                                IsHolcim = true;
                            }
                            dptLine.Save();
                        }
                    }
                    foreach (PayrollAdjustment pad in thisIS.PayrollAdjustments)
                    {
                        if (item == pad.Employee)
                        {
                            if (pad.AdjustmentType.Code == "01-001")
                            {
                                dp.Adjustments = dp.Adjustments + pad.Amount;
                                dp.AdjExplanation=pad.Explanation;
                            }
                            if (pad.AdjustmentType.Code == "01-002")
                            {
                                dp.Rescue = dp.Rescue + pad.Amount;
                                dp.RescueExplanation = pad.Explanation;
                            }
                            if (pad.AdjustmentType.Code == "01-003")
                            {
                                dp.Incentives = dp.Incentives + pad.Amount;
                                dp.InctvExplanation = pad.Explanation;
                            }
                        }
                    }

                    foreach (PayrollDeduction de in thisIS.PayrollDeductions)
                    {
                        if (item == de.Employee)
                        {
                            if (de.DeductionName == "SSS Premium")
                            {
                                dp.SSS = dp.SSS + de.Amount;
                                dp.SSSMonth = de.Month;
                            }

                            if (de.DeductionName == "Pag-Ibig Premium")
                            {
                                dp.PagIbig = dp.PagIbig + de.Amount;
                                dp.PagIbigMonth = de.Month;
                            }
                            if (de.DeductionName == "Philhealth Premium")
                            {
                                dp.PH = dp.PH + de.Amount;
                                dp.PHMonth = de.Month;
                            }
                            if (de.DeductionName == "SSS Loan")
                            {
                                dp.SSSLoan = dp.SSSLoan + de.Amount;
                                dp.SSS2Month = de.Month;
                            }
                            if (de.DeductionName == "Pag-Ibig Loan")
                            {
                                dp.PagIbigLoan = dp.PagIbigLoan + de.Amount;
                                dp.PagIbig2Month = de.Month;
                            }
                            if (de.DeductionName == "Witholding Tax")
                            {
                                dp.WHTax = dp.WHTax + de.Amount;
                                dp.WHTaxMonth = de.Month;
                            }
                        }
                    }

                    foreach (PayrollDeductionOther pdo in thisIS.PayrollDeductionOthers)
                    {
                        if (item == pdo.Employee)
                        {
                            if (pdo.DeductionCode.Code == "10-001")
                            {
                                dp.CA = dp.CA + pdo.Amount;
                            }
                            if (pdo.DeductionCode.Code == "10-002")
                            {
                                dp.HiGasGenset = dp.HiGasGenset + pdo.Amount;
                            }
                            if (pdo.DeductionCode.Code == "10-003")
                            {
                                dp.HiGasTractor = dp.HiGasTractor + pdo.Amount;
                            }
                            if (pdo.DeductionCode.Code == "10-004")
                            {
                                dp.Tools = dp.Tools + pdo.Amount;
                            }
                            if (pdo.DeductionCode.Code == "10-005")
                            {
                                dp.Damages = dp.Damages + pdo.Amount;
                            }
                            if (pdo.DeductionCode.Code == "10-006")
                            {
                                dp.Others = dp.Others + pdo.Amount;
                            }
                            if (pdo.DeductionCode.Code == "10-007")
                            {
                                // Altered to cater Cash Bond
                                dp.Others = dp.Others + pdo.Amount;
                            }
                        }
                    }
                    //if (thisIS.BatchType.IsHolcim)
                    //{
                    //    dp.MiscExpCA = dp.MiscExp;
                    //}
                    if (IsHolcim)
                    {
                        dp.MiscExpCA = dp.MiscExp;
                    }
                    else
                    {
                        dp.MiscExpCA = thisIS.BatchType.CAforMiscExp;
                    }
                    if (thisIS.IncludeNoTrip)
                    {
                        dp.Save();

                    }
                    else
                    {
                        if (dp.AdlMiscExp > 0)
                        {
                            dp.Save();
                        }
                        else
                        {
                            dp.Delete();
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
                    "Generation of payroll data is cancelled.", "Cancelled",
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
                    "Payroll data has been successfully generated.");
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
