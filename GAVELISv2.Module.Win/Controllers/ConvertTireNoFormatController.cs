using System;
using System.Windows.Forms;
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
using DevExpress.Xpo.Metadata;
namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ConvertTireNoFormatController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private Tire _Tire;
        private SimpleAction _ConvertTireNoFormatAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public ConvertTireNoFormatController()
        {
            this.TargetObjectType = typeof(Tire);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ConvertTireNoFormatActionID";
            _ConvertTireNoFormatAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            _ConvertTireNoFormatAction.Caption = "To New Format";
            _ConvertTireNoFormatAction.Execute += new SimpleActionExecuteEventHandler(_ConvertTireNoFormatAction_Execute);
        }
        void _ConvertTireNoFormatAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            XPClassInfo classInfo = ((ObjectSpace)ObjectSpace).Session.GetClassInfo<Tire>();
            CriteriaOperator criteria = CriteriaOperator.Parse("[FirstActivityDate] >= #2019-01-10#");
            SortingCollection sorting = new SortingCollection();
            sorting.Add(new SortProperty("FirstActivityDate", DevExpress.Xpo.DB.SortingDirection.Ascending));
            var data = ((ObjectSpace)ObjectSpace).Session.GetObjects(classInfo, criteria, sorting, 0, false, true);

            if (data == null && data.Count == 0)
            {
                throw new ApplicationException("There are no tires found");
            }
            _FrmProgress = new ProgressForm("Converting entries...", data.Count,
                "Converting entry {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(data);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            ICollection tires = (ICollection)e.Argument;
            try
            {
                foreach (var item in tires)
                {
                    index++;
                    _message = string.Format("Converting entry {0} succesfull.",
                    tires.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    Tire _tire = session.GetObjectByKey<Tire>(((Tire)item).Oid);
                    if (_tire.TireServiceDetails2.Count > 0)
                    {
                        foreach (TireServiceDetail2 tsd in _tire.TireServiceDetails2)
                        {
                            var _detail = session.GetObjectByKey<TireServiceDetail2>(tsd.Oid);
                            if (_detail != null && _detail.BrandingNo.Length == 6 && _detail.BrandingNo.StartsWith("48"))
                            {
                                // Conversion starts here...
                                string s1=_detail.BrandingNo.Substring(2);
                                int n1 = Convert.ToInt32(s1);
                                string yr = _detail.ActivityDate.Year.ToString().Substring(2);
                                string sBranding = string.Format("48{0}{1:D6}", yr, n1);
                                _detail.BrandingNo = sBranding;
                                if (_detail.TaId != null)
                                {
                                    TiremanActivity tma = session.GetObjectByKey<TiremanActivity>(_detail.TaId.Oid);
                                    tma.NewBrandingNo = sBranding;
                                    tma.DettachBranding = sBranding;
                                    tma.Save();
                                }
                                _detail.Save();
                                // Conversion ends here...
                            }
                        }
                    }

                    _tire.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                }
            }
            finally
            {
                if (index == tires.Count)
                {
                    e.Result = index;
                    //CommitUpdatingSession(session);
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
                "Converting entry has been cancelled", "Cancelled",
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
                    " entries are successfully converted.");
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
            _ConvertTireNoFormatAction.
            Enabled.SetItemValue("Converting entries.", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
