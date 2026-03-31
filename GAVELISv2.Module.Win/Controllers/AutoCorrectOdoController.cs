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
    public partial class AutoCorrectOdoController : ViewController {
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private FixedAsset _fixedAsset;

        public AutoCorrectOdoController() {
            InitializeComponent();
            RegisterActions(components);
        }

        private void autoCorrectOdo_Execute(object sender, SimpleActionExecuteEventArgs e) {
            _fixedAsset = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as FixedAsset;

            ObjectSpace.CommitChanges();

            if (_fixedAsset != null)
            {
                _fixedAsset.VehicleOdoRegisters.Sorting.Add(new SortProperty("EntryDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
                if (_fixedAsset.VehicleOdoRegisters.Count == 0)
                {
                    throw new UserFriendlyException("There are no odometer entries found");
                    return;
                }
            }

            _FrmProgress = new ProgressForm("Generating data...", _fixedAsset.VehicleOdoRegisters.Count,
            "Employees processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_fixedAsset.VehicleOdoRegisters);
            _FrmProgress.ShowDialog();
        }

        private void BgWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                "Correction of meter entries is cancelled.", "Cancelled",
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);
            } else
            {
                if (e.Error != null)
                {
                    XtraMessageBox.Show(e.Error.Message,
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);
                } else
                {
                    XtraMessageBox.Show(
                    "Meter entried has been successfully generated.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
                    ObjectSpace.Refresh();
                }
            }
        }

        private void BgWorkerProgressChanged(object sender, ProgressChangedEventArgs e) {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                DoProgress(e.ProgressPercentage);
            }
        }

        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<OdometerRegister> _regEntries = e.Argument as XPCollection<OdometerRegister>;
            FixedAsset thisIS = session.GetObjectByKey<FixedAsset>(
            _fixedAsset.Oid);
            long _SeqNo = thisIS.GetFirstSequenceNo() - 1;
            bool correctNext = false;
            decimal lastReading = 0m;
            decimal lastFuelReading = 0m;
            //DevExpress.Data.CurrencyDataController.DisableThreadingProblemsDetection = true;
            try
            {
                foreach (OdometerRegister item in _regEntries)
                {
                    _SeqNo++;
                    OdometerRegister odr = session.GetObjectByKey<OdometerRegister>(
                    item.Oid);
                    if (correctNext)
                    {
                        LastReadings lstRrd = new LastReadings();
                        lstRrd = thisIS.GetLastReadingBeforeDate(odr.EntryDate);
                        //Difference = _Reading - lstRrd.LastOdoRead;
                        //Life = lstRrd.LastLife + _Difference;

                        LastReadings lstRrdSrv = new LastReadings();
                        LastReadings lstRrdFl = new LastReadings();
                        if (odr.LogType == MeterLogTypeEnum.Service && odr.PrevMaintenanceID != null)
                        {
                            lstRrdSrv = thisIS.GetServiceIdLastReadingBeforeDate(odr.EntryDate, odr.PrevMaintenanceID);
                        } else if (odr.LogType == MeterLogTypeEnum.Service && odr.PrevMaintenanceID == null)
                        {
                            lstRrdSrv = thisIS.GetServiceLastReadingBeforeDate(odr.EntryDate);
                        } else if (odr.LogType == MeterLogTypeEnum.Fuel)
                        {
                            lstRrdFl = thisIS.GetFuelLastReadingBeforeDate(odr.EntryDate);
                        }
                        //lstRrdFl = thisIS.GetFuelLastReadingBeforeDate(odr.EntryDate);
                        //Range = lstRrdFl.LastFuelLife != 0 ? Life - lstRrdFl.LastFuelLife : 0;
                        switch (odr.LogType)
                        {
                            case MeterLogTypeEnum.Initial:
                                break;
                            case MeterLogTypeEnum.Log:
                                odr.Difference = item.Reading - lstRrd.LastOdoRead;
                                odr.Life = lstRrd.LastLife + odr.Difference;
                                odr.Range = 0m;
                                break;
                            case MeterLogTypeEnum.Change:
                                odr.Difference = 0m;
                                odr.Life = lstRrd.LastLife;
                                odr.Range = 0m;
                                break;
                            case MeterLogTypeEnum.Correct:
                                break;
                            case MeterLogTypeEnum.Fuel:
                                odr.Difference = item.Reading - lstRrd.LastOdoRead;
                                odr.Life = lstRrd.LastLife + odr.Difference;
                                odr.Range = lstRrdFl.LastFuelLife != 0 ? odr.Life - lstRrdFl.LastFuelLife : 0;
                                break;
                            case MeterLogTypeEnum.Service:
                                odr.Difference = item.Reading - lstRrd.LastOdoRead;
                                odr.Life = lstRrd.LastLife + odr.Difference;
                                odr.ServiceRange = lstRrdSrv.LastServiceLife != 0 ? odr.Life - lstRrdSrv.LastServiceLife : 0;
                                odr.ServiceIdRange = lstRrdSrv.LastServiceIdLife != 0 ? odr.Life - lstRrdSrv.LastServiceIdLife : 0;
                                break;
                            case MeterLogTypeEnum.None:
                                break;
                            default:
                                break;
                        }
                        //odr.Difference = item.Reading - lastReading;
                        //odr.Range = odr.LogType == MeterLogTypeEnum.Fuel && lastFuelReading != 0 ? item.Reading - lastFuelReading : 0;
                        //odr.Life = lastReading + odr.Difference;
                        odr.Save();
                    }
                    lastReading = odr.Reading;
                    if (odr.LogType == MeterLogTypeEnum.Fuel)
                    {
                        lastFuelReading = odr.Reading;
                    }
                    if (odr.SeqNo != _SeqNo)
                    {
                        correctNext = true;
                        odr.SeqNo = _SeqNo;
                        odr.Corrected = true;
                        odr.Save();
                    }
                    if (odr.Difference < 0)
                    {
                        if (correctNext)
                        {
                            odr.Difference = Math.Abs(odr.Difference);
                            odr.Range = Math.Abs(odr.Range);
                        }
                        else
                        {
                            // throw new UserFriendlyException("Negative Difference from previous reading detected!");
                            XtraMessageBox.Show("Negative Difference from previous reading detected!", "Negative Difference", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, string.Empty);
                    index++;
                }
            }
            finally
            {
                if (index == _regEntries.Count)
                {
                    if (correctNext)
                    {
                        thisIS.SeqNo = _SeqNo;
                    }
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }

        #region View Controller Defaults

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
            if (UpdatingSessionCommitted != null)
            {
                UpdatingSessionCommitted(this
                , new SessionEventArgs(session));
            }
        }

        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) {
            if
            (UpdatingSessionCreated != null)
            {
                UpdatingSessionCreated(this, new
                SessionEventArgs(session));
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

        #endregion

    }
}
