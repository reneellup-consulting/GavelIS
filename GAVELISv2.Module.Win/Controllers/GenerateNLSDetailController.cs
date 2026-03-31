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
    public partial class GenerateNLSDetailController : ViewController
    {
        private SimpleAction GenerateNLSDetailAction;
        private NonLocalSupplierReportGenerator _NLSGenerator;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateNLSDetailController()
        {
            this.TargetObjectType = typeof(NonLocalSupplierReportGenerator);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateNLSDetailActionId";
            this.GenerateNLSDetailAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.GenerateNLSDetailAction.Caption = "Generate";
            this.GenerateNLSDetailAction.Execute += new SimpleActionExecuteEventHandler(GenerateNLSDetailAction_Execute);
        }
        void GenerateNLSDetailAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _NLSGenerator = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as NonLocalSupplierReportGenerator;
            try
            {
                for (int i = _NLSGenerator.NLSGenerationDetails.Count - 1;
                i >= 0; i--)
                {
                    _NLSGenerator.NLSGenerationDetails[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }
            ObjectSpace.CommitChanges();
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now; ;
            switch (_NLSGenerator.GtpGenerationTypeEnum)
            {
                case GtpGenerationTypeEnum.Monthly:
                    startDate = new DateTime(_NLSGenerator.Year, (int)_NLSGenerator.Month, 1);
                    endDate = (new DateTime(_NLSGenerator.Year, (int)_NLSGenerator.Month, DateTime.DaysInMonth(_NLSGenerator.Year, (int)_NLSGenerator.Month))).AddDays(1);
                    break;
                case GtpGenerationTypeEnum.Range:
                    startDate = _NLSGenerator.StartDate;
                    endDate = _NLSGenerator.EndDate.AddDays(1);
                    break;
                default:
                    break;
            }
            List<NLSTempDetInfo> query;
            string crit = string.Format("[GenJournalID.EntryDate] >= #{0}# And [GenJournalID.EntryDate] < #{1}# And [Vendor]  Is Not Null And [Origin]  Is Not Null", startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"));
            IList<WorkOrderItemDetail> woitd = ObjectSpace.GetObjects<WorkOrderItemDetail>(CriteriaOperator.Parse(crit));
            if (woitd != null && woitd.Count > 0)
            {
                query = woitd.OrderBy(o => o.ItemNo).ThenBy(o => o.Vendor)
                    .GroupBy(o => new { o.ItemNo, o.Vendor }).Select(l => 
                        new NLSTempDetInfo { ItemNo = l.First().ItemNo, Vendor = l.First().Vendor, 
                            Amount = l.Sum(c => c.Total) }).ToList();
            }
            else
            {
                throw new UserFriendlyException("There are no details found");
            }
            _FrmProgress = new ProgressForm("Generating...", query.Count,
                        "Lines processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(query);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            List<NLSTempDetInfo> _list = (List<NLSTempDetInfo>)e.Argument;
            NonLocalSupplierReportGenerator tie = session.GetObjectByKey<NonLocalSupplierReportGenerator>(_NLSGenerator.Oid);
            try
            {
                foreach (var item in _list)
                {
                    index++;
                    _message = string.Format("Processing line {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms
                    Item itm = session.GetObjectByKey<Item>(item.ItemNo.Oid);
                    Vendor vnd = session.GetObjectByKey<Vendor>(item.Vendor.Oid);
                    NLSGenerationDetail det = ReflectionHelper.CreateObject<NLSGenerationDetail>(session);
                    det.MainId = tie;
                    det.ItemNo = itm;
                    if (tie.Vendor01 == vnd)
                    {
                        det.ForVendor01 = item.Amount;
                    } 
                    else
                    if (tie.Vendor02 == vnd)
                    {
                        det.ForVendor02 = item.Amount;
                    }
                    else
                    if (tie.Vendor03 == vnd)
                    {
                        det.ForVendor03 = item.Amount;
                    }
                    else
                    if (tie.Vendor04 == vnd)
                    {
                        det.ForVendor04 = item.Amount;
                    }
                    else
                    if (tie.Vendor05 == vnd)
                    {
                        det.ForVendor05 = item.Amount;
                    }
                    else
                    if (tie.Vendor06 == vnd)
                    {
                        det.ForVendor06 = item.Amount;
                    }
                    else
                    if (tie.Vendor07 == vnd)
                    {
                        det.ForVendor07 = item.Amount;
                    }
                    else
                    if (tie.Vendor08 == vnd)
                    {
                        det.ForVendor08 = item.Amount;
                    }
                    else
                    if (tie.Vendor09 == vnd)
                    {
                        det.ForVendor09 = item.Amount;
                    }
                    else
                    if (tie.Vendor10 == vnd)
                    {
                        det.ForVendor10 = item.Amount;
                    }
                    else
                    {
                        det.Others = item.Amount;
                    }
                    det.Save();
                    #endregion

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
                if (index == _list.Count)
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
                    "Generate NLS detail is cancelled.", "Cancelled",
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
                    "NLS detail generation has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
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

    public class NLSTempDetInfo
    {
        public Item ItemNo { get; set; }
        public Vendor Vendor { get; set; }
        public decimal Amount { get; set; }
    }
}
