using System;
using System.ComponentModel;
using System.Collections;
//using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class InitializedOdoLog : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private OdometerRegister _Obj;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public InitializedOdoLog()
        {
            InitializeComponent();
            RegisterActions(components);
        }

        private void InitOdoLog_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no Truck(s) selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList genTrucks = null;
            genTrucks = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = genTrucks.Count;
            _FrmProgress = new ProgressForm("Updating trucks...", count,
            "Updating vendors {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(genTrucks);
            _FrmProgress.ShowDialog();


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
            _ObjectSpace = Application.CreateObjectSpace();
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList trans = (IList)e.Argument;
            try
            {
                FATruck trck = null;
                foreach (FATruck item in trans)
                {
                    trck = session.GetObjectByKey<FATruck>(item.Oid);
            //        FixedAsset thisFA = _ObjectSpace.GetObjectByKey<FixedAsset>(
            //trck.Oid);
                    FixedAsset thisFA = trck as FixedAsset;
                    _Obj = ReflectionHelper.CreateObject<OdometerRegister>(session);
                    _Obj.Fleet = thisFA;
                    if (thisFA.GetType() == typeof(FATruck))
                    {
                        _Obj.ReportedBy = ((FATruck)thisFA).Operator != null ? ((FATruck)thisFA).Operator : null;
                    }
                    _Obj.EntryDate = new DateTime(2014,9,30);
                    _Obj.MeterType = MeterEntryTypeEnum.Odometer;
                    if (thisFA.VehicleOdoRegisters.Count > 1)
                    {
                        _Obj.LogType = MeterLogTypeEnum.Log;
                    }
                    else
                    {
                        _Obj.LogType = MeterLogTypeEnum.Initial;
                    }
                    _Obj.Reading = 0;
                    _Obj.Save();
                   thisFA.Save();
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Updating {0} succesfull.",
                    trans.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            }
            finally
            {
                if (index == trans.Count)
                {
                    e.Result = index;
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }
        private void ClearAllTransactions_Executed(object sender, ActionBaseEventArgs e)
        {
            //throw new NotImplementedException();
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
                    "Updating truck has been cancelled", "Cancelled",
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
                    " has been successfully updated");
                    //ObjectSpace.ReloadObject(invoice);
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
            this.InitOdoLog.
                Enabled.SetItemValue("Updating Truck", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

    }
}
