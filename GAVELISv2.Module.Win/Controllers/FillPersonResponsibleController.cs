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

namespace GAVELISv2.Module.Win.Controllers {
    public partial class FillPersonResponsibleController : ViewController {
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public FillPersonResponsibleController() {
            InitializeComponent();
            RegisterActions(components);
        }

        private void FillPersonResponsibleAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
            if (((DevExpress.ExpressApp.ListView)this.View).SelectedObjects.Count == 0)
            {
                XtraMessageBox.Show("There are no items selected",
                "Attention", System.Windows.Forms.MessageBoxButtons.OK, System.
                Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }
            IList selected = null;
            selected = ((DevExpress.ExpressApp.ListView)this.View).SelectedObjects;
            var count = selected.Count;
            _FrmProgress = new ProgressForm("Updating items...", count,
            "Updating items {0} of {1} ");
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

        #region Progress Action Defaults

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList ccntrs = (IList)e.Argument;
            try
            {
                CostCenter cs = null;
                foreach (CostCenter item in ccntrs)
                {
                    cs = session.GetObjectByKey<CostCenter>(item.Oid);
                    FixedAsset fa = session.FindObject<FixedAsset>(BinaryOperator.Parse(string.Format("[No] = '{0}'",cs.Code)));
                    if (fa != null)
                    {
                        switch (fa.FixedAssetClass)
                        {
                            case FixedAssetClassEnum.LandAndBuilding:
                                break;
                            case FixedAssetClassEnum.Truck:
                                if (fa.GetType() != typeof(FATruck))
                                {
                                    break;
                                }
                                cs.PersonResponsible = ((FATruck)fa).Operator ?? null;
                                break;
                            case FixedAssetClassEnum.Trailer:
                                if (fa.GetType() != typeof(FATrailer))
                                {
                                    break;
                                }
                                cs.PersonResponsible = ((FATrailer)fa).Operator ?? null;
                                break;
                            case FixedAssetClassEnum.GeneratorSet:
                                if (fa.GetType() != typeof(FAGeneratorSet))
                                {
                                    break;
                                }
                                cs.PersonResponsible = ((FAGeneratorSet)fa).Operator ?? null;
                                break;
                            case FixedAssetClassEnum.OtherVehicle:
                                if (fa.GetType() != typeof(FAOtherVehicle))
                                {
                                    break;
                                }
                                cs.PersonResponsible = ((FAOtherVehicle)fa).Operator ?? null;
                                break;
                            case FixedAssetClassEnum.Other:
                                break;
                            default:
                                break;
                        }
                        cs.FixedAsset = fa;
                        cs.Save();
                    }

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    _message = string.Format("Updating entry {0} succesfull.",
                    ccntrs.Count - 1);
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            }
            finally
            {
                if (index == ccntrs.Count)
                {
                    e.Result = index;
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

        private void BgWorkerProgressChanged(object sender,
        ProgressChangedEventArgs e) {
            if (_FrmProgress != null)
            {
                _FrmProgress.
                DoProgress(e.ProgressPercentage);
            }
        }

        private void BgWorkerRunWorkerCompleted(object sender,
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled)
            {
                XtraMessageBox.Show(
                "updating items has been cancelled", "Cancelled",
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
                    XtraMessageBox.Show("All " + e.Result +
                    " has been successfully updated");
                    //ObjectSpace.ReloadObject(invoice);
                    ObjectSpace.Refresh();
                }
            }
        }

        private void FrmProgressCancelClick(object sender, EventArgs e) {
            _BgWorker.CancelAsync();
        }

        private void UpdateActionState(bool inProgress) {
            this.FillPersonResponsibleAction.
            Enabled.SetItemValue("Update items", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;

        #endregion

    }
}
