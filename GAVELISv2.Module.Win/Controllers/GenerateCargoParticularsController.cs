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
using DevExpress.Xpo.DB;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateCargoParticularsController : ViewController
    {
        private SimpleAction generateCargoParticularsAction;
        private TripsCargoHeader _TripsCargoHeader;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateCargoParticularsController()
        {
            this.TargetObjectType = typeof(TripsCargoHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "TripsCargoHeader.GenerateCargoParticulars";
            this.generateCargoParticularsAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateCargoParticularsAction.Caption = "Generate";
            this.generateCargoParticularsAction.Execute += new SimpleActionExecuteEventHandler(generateCargoParticularsAction_Execute);
        }

        void generateCargoParticularsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _TripsCargoHeader = ((DevExpress.ExpressApp.DetailView)this.View).
                    CurrentObject as TripsCargoHeader;

            for (int i = _TripsCargoHeader.TripsCargoDetails.Count - 1; i >= 0; i--)
            {
                _TripsCargoHeader.TripsCargoDetails[i].Delete();
            }

            ObjectSpace.CommitChanges();

            Session session = ((ObjectSpace)ObjectSpace).Session;

            DateTime startDate = _TripsCargoHeader.StartDate;
            DateTime endDate = new DateTime(_TripsCargoHeader.EndDate.Year, _TripsCargoHeader.EndDate.Month, _TripsCargoHeader.EndDate.Day, 23, 59, 59); ;

            SelectedData result = session.ExecuteSproc("GetCargoParticulars",
                new OperandValue(startDate),
                new OperandValue(endDate));

            SelectStatementResultRow[] rows = result.ResultSet[0].Rows;

            if (rows.Count() == 0)
            {
                throw new UserFriendlyException("There are no results returned");
            }

            _FrmProgress = new ProgressForm("Generating data...", rows.Count(),
            "Entries processed {0} of {1} ");

            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(rows);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            TripsCargoHeader tch = session.GetObjectByKey<TripsCargoHeader>(
            _TripsCargoHeader.Oid);
            SelectStatementResultRow[] rows = (SelectStatementResultRow[])e.Argument;

            try
            {
                CriteriaOperator criteria = CriteriaOperator.Parse("[Date] >= ? AND [Date] <= ?", tch.StartDate, tch.EndDate);
                XPCollection<CargoRegistry> collection = new XPCollection<CargoRegistry>(session, criteria);

                // Create a Dictionary to store driver IDs as keys
                Dictionary<Guid, string> drivers = new Dictionary<Guid, string>();
                
                for (int i = 0; i < rows.Count(); i++)
                {
                    #region Algorithms here

                    index++;
                    TripsCargoDetail detail = null;
                    if (!drivers.ContainsKey(Guid.Parse(rows[i].Values[0].ToString())))
                    {
                        detail = ReflectionHelper.CreateObject<TripsCargoDetail>(session);
                        detail.ParentID = tch;
                        // Driver
                        detail.Driver = session.GetObjectByKey<Employee>(Guid.Parse(rows[i].Values[0].ToString()));
                        drivers.Add(Guid.Parse(rows[i].Values[0].ToString()), detail.Driver.Name);
                    }
                    else
                    {
                        detail = tch.TripsCargoDetails.Where(o => o.Driver.Oid == Guid.Parse(rows[i].Values[0].ToString())).FirstOrDefault();
                    }
                    
                    IEnumerable<CargoRegistry> regs = null;
                    string cargocat = rows[i].Values[4].ToString();
                    switch (cargocat)
                    {
                        case "PINEAPPLE":
                            regs = collection.Where(o => o.Driver == detail.Driver).Select(o => o).Where(o => o.Particular.Category.Category == "PINEAPPLE").Select(o => o);
                            // PineappleTrips
                            detail.PineappleTrips = Convert.ToInt32(rows[i].Values[3]);
                            // PineappleIncome
                            detail.PineappleIncome = regs.Select(o => o.NetBilling).Sum();
                            break;
                        case "BANANA":
                            regs = collection.Where(o => o.Driver == detail.Driver).Select(o => o).Where(o => o.Particular.Category.Category == "BANANA").Select(o => o);
                            // BananaTrips
                            detail.BananaTrips = Convert.ToInt32(rows[i].Values[3]);
                            // BananaIncome
                            detail.BananaIncome = regs.Select(o => o.NetBilling).Sum();
                            break;
                        case "CEMENT":
                            regs = collection.Where(o => o.Driver == detail.Driver).Select(o => o).Where(o => o.Particular.Category.Category == "CEMENT").Select(o => o);
                            // CementTrips
                            detail.CementTrips = Convert.ToInt32(rows[i].Values[3]);
                            // CementIncome
                            detail.CementIncome = regs.Select(o => o.NetBilling).Sum();
                            break;
                        case "OTHERS":
                            regs = collection.Where(o => o.Driver == detail.Driver).Select(o => o).Where(o => o.Particular.Category.Category == "OTHERS").Select(o => o);
                            // OtherTrips
                            detail.OtherTrips = Convert.ToInt32(rows[i].Values[3]);
                            // OtherIncome
                            detail.OtherIncome = regs.Select(o => o.NetBilling).Sum();
                            break;
                        default:
                            break;
                    }
                    detail.Save();

                    #endregion

                    _message = string.Format("Generating entry {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);

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
                if (index == rows.Count())
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
                    "Generation of cargo particulars is cancelled.", "Cancelled",
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
                    "Cargo particulars has been successfully generated.");
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
