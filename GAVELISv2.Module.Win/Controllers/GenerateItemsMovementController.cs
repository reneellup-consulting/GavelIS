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
    public partial class GenerateItemsMovementController : ViewController
    {
        private SimpleAction generateItemsMovementAction;
        private ItemMovementFreqAnalysis _ItemMovementFreqAnalysis;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateItemsMovementController()
        {
            this.TargetObjectType = typeof(ItemMovementFreqAnalysis);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "generateItemsMovementActionId";
            this.generateItemsMovementAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateItemsMovementAction.TargetObjectsCriteria = "[ItemMovementFreqAnalysisDetails][].Count() = 0";
            this.generateItemsMovementAction.Caption = "Generate";
            this.generateItemsMovementAction.Execute += new SimpleActionExecuteEventHandler(generateItemsMovementAction_Execute);
        }
        private void generateItemsMovementAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ItemMovementFreqAnalysis = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ItemMovementFreqAnalysis;

            ObjectSpace.CommitChanges();

            int count = 0;

            string filter = string.Empty;

            switch (_ItemMovementFreqAnalysis.FrequencyFilter)
            {
                case FrequencyFilterEnum.All:
                    break;
                case FrequencyFilterEnum.Yearly:
                    filter = string.Format("where [Year]={0}", _ItemMovementFreqAnalysis.Year);
                    break;
                case FrequencyFilterEnum.Quarterly:
                    filter = string.Format("where [Year]={0} and Qrt={1}", _ItemMovementFreqAnalysis.Year, (int)_ItemMovementFreqAnalysis.Quarter);
                    break;
                case FrequencyFilterEnum.Monthly:
                    filter = string.Format("where [Year]={0} and [Month]={1}", _ItemMovementFreqAnalysis.Year, (int)_ItemMovementFreqAnalysis.Month);
                    break;
                case FrequencyFilterEnum.Weekly:
                    filter = string.Format("where [Year]={0} and [Month]={1} and WeekNo={2}", _ItemMovementFreqAnalysis.Year, (int)_ItemMovementFreqAnalysis.Month, (int)_ItemMovementFreqAnalysis.Week);
                    break;
                case FrequencyFilterEnum.Date:
                    filter = string.Format("where [Year]={0} and [Month]={1} and [Day]={2}", _ItemMovementFreqAnalysis.AsOfDate.Year, _ItemMovementFreqAnalysis.AsOfDate.Month, _ItemMovementFreqAnalysis.AsOfDate.Day);
                    break;
                default:
                    break;
            }

            string whseFilter = string.Empty;

            if (_ItemMovementFreqAnalysis.Warehouse != null)
            {
                whseFilter = string.Format("WhseId = '{0}'", _ItemMovementFreqAnalysis.Warehouse.Oid);
            }

            string qry = string.Empty;

            if (_ItemMovementFreqAnalysis.Top == 0)
            {
                if (whseFilter != string.Empty)
                {
                    if (filter != string.Empty)
                    {
                        qry = string.Format("select ItemID, count(ItemID) as NoOfInstance, SUM(CASE WHEN ISNULL(SrcTypeId,'')='1FF5D945-40B6-4844-A33D-F0593E0F522A' THEN 1 ELSE 0 END) as RCPT, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='0DEEDCCE-759E-461E-A24B-CB925995C6DE' THEN 1 ELSE 0 END) as INVC, SUM(CASE WHEN ISNULL(SrcTypeId,'')='9117678D-98CD-4EEA-ACA3-FDDC70FB6E70' THEN 1 ELSE 0 END) as WORD, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='044FFFC9-F42F-46B1-AADB-92C8DFB5FA2E' THEN 1 ELSE 0 END) as ECS, SUM(CASE WHEN ISNULL(SrcTypeId,'')='83C4F2C8-5BA0-4D6A-94B6-BA0F02536DB3' THEN 1 ELSE 0 END) as CM, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='F0625190-AD17-4648-88D1-C45C3E744EDA' THEN 1 ELSE 0 END) as DM, SUM(CASE WHEN ISNULL(SrcTypeId,'')='7BD40111-6906-448A-90C0-76CED8ECAADE' THEN 1 ELSE 0 END) as FPR " +
                                    "from vInvMovementAnalysis {0} and {1} group by ItemID order by NoOfInstance DESC", filter, whseFilter);
                    }
                    else
                    {
                        qry = string.Format("select ItemID, count(ItemID) as NoOfInstance, SUM(CASE WHEN ISNULL(SrcTypeId,'')='1FF5D945-40B6-4844-A33D-F0593E0F522A' THEN 1 ELSE 0 END) as RCPT, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='0DEEDCCE-759E-461E-A24B-CB925995C6DE' THEN 1 ELSE 0 END) as INVC, SUM(CASE WHEN ISNULL(SrcTypeId,'')='9117678D-98CD-4EEA-ACA3-FDDC70FB6E70' THEN 1 ELSE 0 END) as WORD, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='044FFFC9-F42F-46B1-AADB-92C8DFB5FA2E' THEN 1 ELSE 0 END) as ECS, SUM(CASE WHEN ISNULL(SrcTypeId,'')='83C4F2C8-5BA0-4D6A-94B6-BA0F02536DB3' THEN 1 ELSE 0 END) as CM, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='F0625190-AD17-4648-88D1-C45C3E744EDA' THEN 1 ELSE 0 END) as DM, SUM(CASE WHEN ISNULL(SrcTypeId,'')='7BD40111-6906-448A-90C0-76CED8ECAADE' THEN 1 ELSE 0 END) as FPR " +
                                    "from vInvMovementAnalysis where {0} group by ItemID order by NoOfInstance DESC", whseFilter);
                    }
                }
                else
                {
                    qry = string.Format("select ItemID, count(ItemID) as NoOfInstance, SUM(CASE WHEN ISNULL(SrcTypeId,'')='1FF5D945-40B6-4844-A33D-F0593E0F522A' THEN 1 ELSE 0 END) as RCPT, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='0DEEDCCE-759E-461E-A24B-CB925995C6DE' THEN 1 ELSE 0 END) as INVC, SUM(CASE WHEN ISNULL(SrcTypeId,'')='9117678D-98CD-4EEA-ACA3-FDDC70FB6E70' THEN 1 ELSE 0 END) as WORD, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='044FFFC9-F42F-46B1-AADB-92C8DFB5FA2E' THEN 1 ELSE 0 END) as ECS, SUM(CASE WHEN ISNULL(SrcTypeId,'')='83C4F2C8-5BA0-4D6A-94B6-BA0F02536DB3' THEN 1 ELSE 0 END) as CM, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='F0625190-AD17-4648-88D1-C45C3E744EDA' THEN 1 ELSE 0 END) as DM, SUM(CASE WHEN ISNULL(SrcTypeId,'')='7BD40111-6906-448A-90C0-76CED8ECAADE' THEN 1 ELSE 0 END) as FPR " +
                                    "from vInvMovementAnalysis {0} group by ItemID order by NoOfInstance DESC", filter);
                } 
            }
            else if (_ItemMovementFreqAnalysis.Top > 0)
            {
                if (whseFilter != string.Empty)
                {
                    if (filter != string.Empty)
                    {
                        qry = string.Format("select top {0} ItemID, count(ItemID) as NoOfInstance, SUM(CASE WHEN ISNULL(SrcTypeId,'')='1FF5D945-40B6-4844-A33D-F0593E0F522A' THEN 1 ELSE 0 END) as RCPT, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='0DEEDCCE-759E-461E-A24B-CB925995C6DE' THEN 1 ELSE 0 END) as INVC, SUM(CASE WHEN ISNULL(SrcTypeId,'')='9117678D-98CD-4EEA-ACA3-FDDC70FB6E70' THEN 1 ELSE 0 END) as WORD, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='044FFFC9-F42F-46B1-AADB-92C8DFB5FA2E' THEN 1 ELSE 0 END) as ECS, SUM(CASE WHEN ISNULL(SrcTypeId,'')='83C4F2C8-5BA0-4D6A-94B6-BA0F02536DB3' THEN 1 ELSE 0 END) as CM, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='F0625190-AD17-4648-88D1-C45C3E744EDA' THEN 1 ELSE 0 END) as DM, SUM(CASE WHEN ISNULL(SrcTypeId,'')='7BD40111-6906-448A-90C0-76CED8ECAADE' THEN 1 ELSE 0 END) as FPR " +
                                    "from vInvMovementAnalysis {1} and {2} group by ItemID order by NoOfInstance DESC", _ItemMovementFreqAnalysis.Top, filter, whseFilter);
                    }
                    else
                    {
                        qry = string.Format("select top {0} ItemID, count(ItemID) as NoOfInstance, SUM(CASE WHEN ISNULL(SrcTypeId,'')='1FF5D945-40B6-4844-A33D-F0593E0F522A' THEN 1 ELSE 0 END) as RCPT, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='0DEEDCCE-759E-461E-A24B-CB925995C6DE' THEN 1 ELSE 0 END) as INVC, SUM(CASE WHEN ISNULL(SrcTypeId,'')='9117678D-98CD-4EEA-ACA3-FDDC70FB6E70' THEN 1 ELSE 0 END) as WORD, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='044FFFC9-F42F-46B1-AADB-92C8DFB5FA2E' THEN 1 ELSE 0 END) as ECS, SUM(CASE WHEN ISNULL(SrcTypeId,'')='83C4F2C8-5BA0-4D6A-94B6-BA0F02536DB3' THEN 1 ELSE 0 END) as CM, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='F0625190-AD17-4648-88D1-C45C3E744EDA' THEN 1 ELSE 0 END) as DM, SUM(CASE WHEN ISNULL(SrcTypeId,'')='7BD40111-6906-448A-90C0-76CED8ECAADE' THEN 1 ELSE 0 END) as FPR " +
                                    "from vInvMovementAnalysis where {1} group by ItemID order by NoOfInstance DESC", _ItemMovementFreqAnalysis.Top, whseFilter);
                    }
                    
                }
                else
                {
                    qry = string.Format("select top {0} ItemID, count(ItemID) as NoOfInstance, SUM(CASE WHEN ISNULL(SrcTypeId,'')='1FF5D945-40B6-4844-A33D-F0593E0F522A' THEN 1 ELSE 0 END) as RCPT, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='0DEEDCCE-759E-461E-A24B-CB925995C6DE' THEN 1 ELSE 0 END) as INVC, SUM(CASE WHEN ISNULL(SrcTypeId,'')='9117678D-98CD-4EEA-ACA3-FDDC70FB6E70' THEN 1 ELSE 0 END) as WORD, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='044FFFC9-F42F-46B1-AADB-92C8DFB5FA2E' THEN 1 ELSE 0 END) as ECS, SUM(CASE WHEN ISNULL(SrcTypeId,'')='83C4F2C8-5BA0-4D6A-94B6-BA0F02536DB3' THEN 1 ELSE 0 END) as CM, " +
                                    "SUM(CASE WHEN ISNULL(SrcTypeId,'')='F0625190-AD17-4648-88D1-C45C3E744EDA' THEN 1 ELSE 0 END) as DM, SUM(CASE WHEN ISNULL(SrcTypeId,'')='7BD40111-6906-448A-90C0-76CED8ECAADE' THEN 1 ELSE 0 END) as FPR " +
                                    "from vInvMovementAnalysis {1} group by ItemID order by NoOfInstance DESC", _ItemMovementFreqAnalysis.Top, filter);
                }
            }

            Session t_session = ((ObjectSpace)ObjectSpace).Session;
            SelectedData data = t_session.ExecuteQuery(qry);
            if (data == null || data.ResultSet[0].Rows.Count() == 0)
            {
                throw new UserFriendlyException("There is nothing to generate for the specified filter.");
            }
            count = data.ResultSet[0].Rows.Count();

            _FrmProgress = new ProgressForm("Generate...", count,
                        "Processing row {0} of {1} ");
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
            SelectedData trans = (SelectedData)e.Argument;
            ItemMovementFreqAnalysis oGenerator = session.GetObjectByKey<ItemMovementFreqAnalysis>(_ItemMovementFreqAnalysis.Oid);
            try
            {
                foreach (var row in trans.ResultSet[0].Rows)
                {
                    index++;

                    #region Algorithm here...

                    if (row.Values[0] != null)
                    {
                        Item o_itm = session.GetObjectByKey<Item>(Guid.Parse(row.Values[0].ToString()));

                        if (new[] { ItemTypeEnum.ChargeItem.GetDisplayName(), ItemTypeEnum.FixedAsset.GetDisplayName(), 
                            ItemTypeEnum.ProductGroup.GetDisplayName(), ItemTypeEnum.ServiceItem.GetDisplayName() }.Any(o => o_itm.ItemType.GetDisplayName().Contains(o)))
                        {
                            _message = string.Format("Processing row {0} succesfull.", index);
                            _BgWorker.ReportProgress(1, _message);

                            continue;
                        }

                        ItemMovementFreqAnalDetail o_imfad = ReflectionHelper.CreateObject<ItemMovementFreqAnalDetail>(session);
                        o_imfad.HeaderId = oGenerator;
                        o_imfad.ItemNo = o_itm;
                        o_imfad.NoOfInstance = Convert.ToInt32(row.Values[1].ToString());
                        o_imfad.Rcpt = Convert.ToInt32(row.Values[2].ToString());
                        o_imfad.Invc = Convert.ToInt32(row.Values[3].ToString());
                        o_imfad.Word = Convert.ToInt32(row.Values[4].ToString());
                        o_imfad.Ecs = Convert.ToInt32(row.Values[5].ToString());
                        o_imfad.Cm = Convert.ToInt32(row.Values[6].ToString());
                        o_imfad.Dm = Convert.ToInt32(row.Values[7].ToString());
                        o_imfad.Fpr = Convert.ToInt32(row.Values[8].ToString());
                        o_imfad.Save();
                    }

                    #endregion
                    
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    _message = string.Format("Processing row {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.ResultSet[0].Rows.Count())
                {
                    e.Result = index;
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
}
