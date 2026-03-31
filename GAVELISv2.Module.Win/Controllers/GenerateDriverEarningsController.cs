using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateDriverEarningsController : ViewController
    {
        private SimpleAction generateDriverEarningsAction;
        private DriversEarningsFtm _DriversEarningsFtm;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateDriverEarningsController()
        {
            this.TargetObjectType = typeof(DriversEarningsFtm);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateDriverEarningsActionId";
            this.generateDriverEarningsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateDriverEarningsAction.Caption = "Generate";
            this.generateDriverEarningsAction.Execute += new SimpleActionExecuteEventHandler(generateDriverEarningsAction_Execute);
        }

        void generateDriverEarningsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _DriversEarningsFtm = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as DriversEarningsFtm;

            // Remove all existing DriversEarningFtmDetail records
            foreach (var detail in _DriversEarningsFtm.DriversEarningFtmDetails.ToList())
            {
                detail.Delete();
            }

            ObjectSpace.CommitChanges();

            // Get the session from the ObjectSpace
            Session session = ((ObjectSpace)ObjectSpace).Session;

            // Assume your stored procedure is named "GetDriversEarningFTM"
            SelectedData result = session.ExecuteSproc("GetDriversEarningFTM",
                new OperandValue(_DriversEarningsFtm.TargetMonth),
                new OperandValue(_DriversEarningsFtm.TargetYear));

            int cnts = result.ResultSet[0].Rows.Count();

            _FrmProgress = new ProgressForm("Generating...", cnts,
                    "Objects processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(result.ResultSet[0].Rows);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            SelectStatementResultRow[] trans = (SelectStatementResultRow[])e.Argument;
            DriversEarningsFtm thisDriversEarningsFtm = session.GetObjectByKey<DriversEarningsFtm>(_DriversEarningsFtm.Oid);
            try
            {
                foreach (var row in trans)
                {
                    index++;
                    _message = string.Format("Generating details {0} succesfull.",
                    Convert.ToString(row.Values[1]));
                    _BgWorker.ReportProgress(1, _message);

                    var detail = ReflectionHelper.CreateObject<DriversEarningFtmDetail>(session);
                    detail.Employeee = session.GetObjectByKey<Employee>(new Guid(Convert.ToString(row.Values[0])));
                    detail.Adjustments = Convert.ToDecimal(row.Values[2]);
                    detail.Deductions = Convert.ToDecimal(row.Values[3]);
                    detail.TotalPayValue = Convert.ToDecimal(row.Values[4]);
                    detail.GrossPay = Convert.ToDecimal(row.Values[5]);
                    detail.NetPay = Convert.ToDecimal(row.Values[6]);
                    thisDriversEarningsFtm.DriversEarningFtmDetails.Add(detail);

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }
                session.CommitTransaction();
            }
            finally
            {
                if (index == trans.Count())
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
                    "Generation is cancelled.", "Cancelled",
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
                    "Generation has been successfull.");
                    //ObjectSpace.ReloadObject(_IncomeExpenseReporter);
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
