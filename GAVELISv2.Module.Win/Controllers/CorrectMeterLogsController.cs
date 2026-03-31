using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Generators;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using GAVELISv2.Module.BusinessObjects;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class CorrectMeterLogsController : ViewController
    {
        private FixedAsset fixedAsset;
        private SimpleAction correcMeterLogAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public CorrectMeterLogsController()
        {
            this.TargetObjectType = typeof(FixedAsset);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "CorrectMeterLogs";
            this.correcMeterLogAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.correcMeterLogAction.Caption = "Correct Meter Logs";
            this.correcMeterLogAction.Execute += new
            SimpleActionExecuteEventHandler(correcMeterLogAction_Execute);
            this.correcMeterLogAction.Executed += new EventHandler<
            ActionBaseEventArgs>(correcMeterLogAction_Executed);
            this.correcMeterLogAction.ConfirmationMessage =
            "Do you really want to correct log entries?";
            UpdateActionState(false);
        }
        private void correcMeterLogAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            fixedAsset = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FixedAsset;
            int iCount = 0;
            ObjectSpace.CommitChanges();
            if (fixedAsset.FuelOdoDescrepancy)
            {
                iCount += fixedAsset.FleetFuelOdoLogs.Count; //+ fixedAsset.FleetServiceOdoLogs.Count + fixedAsset.FleetTripeOdoLogs.Count;
            }
            if (fixedAsset.ServiceOdoDescrepancy)
            {
                iCount += fixedAsset.FleetServiceOdoLogs.Count;
            }
            if (fixedAsset.TripOdoDescrepancy)
            {
                iCount += fixedAsset.FleetTripeOdoLogs.Count;
            }
            if (iCount == 0)
            {
                XtraMessageBox.Show("There are no log entries to correct",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            _FrmProgress = new ProgressForm("Correcting entries...", iCount,
            "Correcting entries {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(fixedAsset);
            _FrmProgress.ShowDialog();
        }
        private void correcMeterLogAction_Executed(object sender,
        ActionBaseEventArgs e)
        {
            //ObjectSpace.ReloadObject(receipt);
            //ObjectSpace.Refresh();
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            //DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            FixedAsset fa = e.Argument as FixedAsset;
            //FixedAsset thisFa = session.GetObjectByKey<FixedAsset>(fa.Oid);
            int iCount = 0;
            if (fa.FuelOdoDescrepancy)
            {
                iCount += fa.FleetFuelOdoLogs.Count; //+ fixedAsset.FleetServiceOdoLogs.Count + fixedAsset.FleetTripeOdoLogs.Count;
            }
            if (fa.ServiceOdoDescrepancy)
            {
                iCount += fa.FleetServiceOdoLogs.Count;
            }
            if (fa.TripOdoDescrepancy)
            {
                iCount += fa.FleetTripeOdoLogs.Count;
            }
            try
            {
                if (fa.FuelOdoDescrepancy)
                {
                    decimal oldRead = 0m;
                    foreach (var item in fa.FleetFuelOdoLogs)
                    {
                        index++;
                        _message = string.Format("Correcting log {0} succesfull.",
                        item.Oid);
                        _BgWorker.ReportProgress(1, _message);
                        FuelOdoRegistry oReg = session.GetObjectByKey<FuelOdoRegistry>(item.Oid);
                        #region Algorithms here
                        oldRead = oReg.Reading;
                        // Change odo device
                        if (oReg.ChangeOdo)
                        {
                            //decimal seq1 = Convert.ToDecimal(oReg.Sequence) + 1;
                            //oReg.Sequence = seq1.ToString();
                            oReg.EntryDate = oReg.EntryDate.AddSeconds(1);
                            oReg.LogType = MeterRegistryTypeEnum.Change;
                            oReg.Reading = 0m;
                            oReg.CallRead();
                            oReg.Difference = 0m;
                            //oReg.Life = oReg.LastReading - oReg.Difference;
                            oReg.Life = oReg.LastLife + oReg.Difference;
                            oReg.ChangeOdo = false;
                            oReg.Save();

                            // create new change entry
                            //decimal seq2 = Convert.ToDecimal(oReg.Sequence) + 2;
                            FuelOdoRegistry newChange = ReflectionHelper.CreateObject<FuelOdoRegistry>(session);
                            newChange.Fleet = oReg.Fleet;
                            newChange.EntryDate = oReg.EntryDate.AddSeconds(1);
                            //newChange.Sequence = seq2.ToString();
                            newChange.LogType = MeterRegistryTypeEnum.Change;
                            newChange.CallRead();
                            newChange.Reading = oldRead;
                            newChange.LastReading = 0m;
                            newChange.Difference = 0m;
                            newChange.Life = oReg.Life;
                            newChange.ChangeOdo = false;
                            newChange.Save();
                        }
                        else
                        {
                            oReg.CallRead();
                            if (oReg.InvalidDiff)
                            {
                                oReg.Difference = oReg.Reading - oReg.LastReading;
                                oReg.Life = oReg.LastReading - oReg.Difference;
                            }
                            //oReg.Life = oReg.LastReading - oReg.Difference;
                            oReg.Save();
                        }
                        //System.Threading.Thread.Sleep(300);
                        session.CommitChanges();
                        #endregion
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                    }
                }
                if (fa.ServiceOdoDescrepancy)
                {
                    decimal oldRead = 0m;
                    foreach (var item in fa.FleetServiceOdoLogs)
                    {
                        index++;
                        _message = string.Format("Correcting log {0} succesfull.",
                        item.Oid);
                        _BgWorker.ReportProgress(1, _message);
                        ServiceOdoRegistry oReg = session.GetObjectByKey<ServiceOdoRegistry>(item.Oid);
                        #region Algorithms here
                        oldRead = oReg.Reading;
                        // Change odo device
                        if (oReg.ChangeOdo)
                        {
                            //decimal seq1 = Convert.ToDecimal(oReg.Sequence) + 1;
                            //oReg.Sequence = seq1.ToString();
                            oReg.EntryDate = oReg.EntryDate.AddSeconds(1);
                            oReg.LogType = MeterRegistryTypeEnum.Change;
                            oReg.Reading = 0m;
                            oReg.CallRead();
                            oReg.Difference = 0m;
                            //oReg.Life = oReg.LastReading - oReg.Difference;
                            oReg.Life = oReg.LastLife + oReg.Difference;
                            oReg.ChangeOdo = false;
                            oReg.Save();

                            // create new change entry
                            //decimal seq2 = Convert.ToDecimal(oReg.Sequence) + 2;
                            ServiceOdoRegistry newChange = ReflectionHelper.CreateObject<ServiceOdoRegistry>(session);
                            newChange.Fleet = oReg.Fleet;
                            newChange.EntryDate = oReg.EntryDate.AddSeconds(1);
                            //newChange.Sequence = seq2.ToString();
                            newChange.LogType = MeterRegistryTypeEnum.Change;
                            newChange.CallRead();
                            newChange.Reading = oldRead;
                            newChange.LastReading = 0m;
                            newChange.Difference = 0m;
                            newChange.Life = oReg.Life;
                            newChange.ChangeOdo = false;
                            newChange.Save();
                        }
                        else
                        {
                            oReg.CallRead();
                            if (oReg.InvalidDiff)
                            {
                                oReg.Difference = oReg.Reading - oReg.LastReading;
                                oReg.Life = oReg.LastReading - oReg.Difference;
                            }
                            //oReg.Life = oReg.LastReading - oReg.Difference;
                            oReg.Save();
                        }
                        //System.Threading.Thread.Sleep(300);
                        session.CommitChanges();
                        #endregion
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                    }
                }
                if (fa.TripOdoDescrepancy)
                {
                    decimal oldRead = 0m;
                    foreach (var item in fa.FleetTripeOdoLogs)
                    {
                        index++;
                        _message = string.Format("Correcting log {0} succesfull.",
                        item.Oid);
                        _BgWorker.ReportProgress(1, _message);
                        TripOdoRegistry oReg = session.GetObjectByKey<TripOdoRegistry>(item.Oid);
                        #region Algorithms here
                        oldRead = oReg.Reading;
                        // Change odo device
                        if (oReg.ChangeOdo)
                        {
                            //decimal seq1 = Convert.ToDecimal(oReg.Sequence) + 1;
                            //oReg.Sequence = seq1.ToString();
                            oReg.EntryDate = oReg.EntryDate.AddSeconds(1);
                            oReg.LogType = MeterRegistryTypeEnum.Change;
                            oReg.Reading = 0m;
                            oReg.CallRead();
                            oReg.Difference = 0m;
                            //oReg.Life = oReg.LastReading - oReg.Difference;
                            oReg.Life = oReg.LastLife + oReg.Difference;
                            oReg.ChangeOdo = false;
                            oReg.Save();

                            // create new change entry
                            //decimal seq2 = Convert.ToDecimal(oReg.Sequence) + 2;
                            TripOdoRegistry newChange = ReflectionHelper.CreateObject<TripOdoRegistry>(session);
                            newChange.Fleet = oReg.Fleet;
                            newChange.EntryDate = oReg.EntryDate.AddSeconds(1);
                            //newChange.Sequence = seq2.ToString();
                            newChange.LogType = MeterRegistryTypeEnum.Change;
                            newChange.CallRead();
                            newChange.Reading = oldRead;
                            newChange.LastReading = 0m;
                            newChange.Difference = 0m;
                            newChange.Life = oReg.Life;
                            newChange.ChangeOdo = false;
                            newChange.Save();
                        }
                        else
                        {
                            oReg.CallRead();
                            if (oReg.InvalidDiff)
                            {
                                oReg.Difference = oReg.Reading - oReg.LastReading;
                                oReg.Life = oReg.LastReading + oReg.Difference;
                            }
                            //oReg.Life = oReg.LastReading - oReg.Difference;
                            oReg.Save();
                        }
                        //System.Threading.Thread.Sleep(600);
                        session.CommitChanges();
                        #endregion
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                    }
                }
            }
            finally
            {
                if (index == iCount)
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
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
                    "Correcting log enrtries has been cancelled", "Cancelled",
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

                    ObjectSpace.ReloadObject(fixedAsset);
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
            correcMeterLogAction.
                Enabled.SetItemValue("Correcting entries", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
