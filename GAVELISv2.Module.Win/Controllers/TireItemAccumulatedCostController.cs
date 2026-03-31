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
    public partial class TireItemAccumulatedCostController : ViewController
    {
        private SimpleAction RetrieveCostAction;
        private TireItemsAccomulatedCost _AccumulatedCost;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public TireItemAccumulatedCostController()
        {
            this.TargetObjectType = typeof(TireItemsAccomulatedCost);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "RetrieveCostActionId";
            this.RetrieveCostAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.RetrieveCostAction.Caption = "Retrieve Costs";
            this.RetrieveCostAction.Execute += new SimpleActionExecuteEventHandler(RetrieveCostAction_Execute);
        }

        void RetrieveCostAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _AccumulatedCost = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TireItemsAccomulatedCost;
            try
            {
                for (int i = _AccumulatedCost.Details.Count - 1;
                i >= 0; i--)
                {
                    _AccumulatedCost.Details[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }
            ObjectSpace.CommitChanges();
            // Count each objects line to process
            int cnt = 0;
            // Tire RWS
            // [ReleaseStatus] = 'Released' And [Year] = 2017
            Object[] objArray = new Object[3];
            var trwss = this.ObjectSpace.GetObjects<RwsTireDetail>(BinaryOperator.Parse("[ReleaseStatus] = 'Released' And [Year] = ?", _AccumulatedCost.Year), true);
            if (trwss !=null && trwss.Count > 0)
            {
                cnt = trwss.Count;
                objArray[0] = trwss;
            }
            // Requisition Worksheet
            var rwss = this.ObjectSpace.GetObjects<RequisitionWorksheet>(BinaryOperator.Parse("[ItemNo.TirePartType] In ('Flap', 'Tube', 'Rim') And [IsChargeToUnit] = True And [ExpectedYear] = ?", _AccumulatedCost.Year), true);
            if (rwss != null && rwss.Count > 0)
            {
                cnt += rwss.Count;
                objArray[1] = rwss;
            }
            // Tire Service Detail 2
            //var tsd2 = this.ObjectSpace.GetObjects<TireServiceDetail2>(BinaryOperator.Parse("Not [Reason.Code] In ('1ST BRANDING', 'SOLD') And [ActivityType] = 'Attached' And [GYear] = ?", _AccumulatedCost.Year), true);
            //var tsd2 = this.ObjectSpace.GetObjects<TireServiceDetail2>(BinaryOperator.Parse("Not [Reason.Code] In ('SOLD') And [ActivityType] = 'Attached' And [GYear] = ?", _AccumulatedCost.Year), true);
            var tsd2 = this.ObjectSpace.GetObjects<TireServiceDetail2>(BinaryOperator.Parse("Not [Reason.Code] In ('SOLD') And [ActivityType] In ('Attached','Dettached') And [GYear] = ?", _AccumulatedCost.Year), true);
            if (tsd2 != null && tsd2.Count > 0)
            {
                cnt += tsd2.Count;
                objArray[2] = tsd2;
            }
            _FrmProgress = new ProgressForm("Retrieving...", cnt,
                        "Item processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(objArray);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            int cnt = 0;
            UnitOfWork session = CreateUpdatingSession();
            TireItemsAccomulatedCost acc = session.GetObjectByKey<TireItemsAccomulatedCost>(_AccumulatedCost.Oid);
            Object[] trans = (Object[])e.Argument;
            try
            {
                var trwss = trans[0] as IList<RwsTireDetail>;
                if (trwss != null && trwss.Count > 0)
                {
                    cnt = trwss.Count;
                    foreach (var item in trwss)
                    {
                        index++;
                        _message = string.Format("Processing item {0} succesfull.",
                        index);
                        _BgWorker.ReportProgress(1, _message);

                        #region Algorithms
                        if (item.RwsTireDetId == "RWST00001525")
                        {

                        }
                        if (item.NewTireItem.TireItemClass == TireItemClassEnum.SecondHandTire)
                        {
                            continue;
                        }
                        RwsTireDetail rwstd = session.GetObjectByKey<RwsTireDetail>(item.Oid);
                        var tiac = session.FindObject<TireItemsAccomCostDetail>(BinaryOperator.Parse("[HeaderId] = ? And [SourceId] = ? And [SourceDetId] = ?", acc, item.ReqWorksheetId.GenJournalID.SourceNo, item.Oid));
                        if (tiac == null)
                        {
                            tiac = ReflectionHelper.CreateObject<TireItemsAccomCostDetail>(session);
                            tiac.HeaderId = acc;
                            tiac.SourceId = rwstd.ReqWorksheetId.GenJournalID.SourceNo;
                            tiac.SourceDetId = rwstd.Oid.ToString();
                        }
                        if (tiac != null)
                        {
                            tiac.EntryDate = rwstd.IssueDate;
                            tiac.TireReqDetail = rwstd;
                            tiac.TireItemClass = AccTireItemClassEnum.Tire;
                            if (rwstd.NewTireItem != null && rwstd.NewTireItem.Size != null)
                            {
                                tiac.TireType = rwstd.NewTireItem.Size.TireType;
                            }
                            else
                            {
                                tiac.TireType = TireTypeEnum.None;
                            }
                            tiac.ItemNo = rwstd.NewTireItem;
                            switch (item.NewTireItem.TireItemClass)
                            {
                                case TireItemClassEnum.None:
                                    break;
                                case TireItemClassEnum.BrandNewTire:
                                    tiac.LineCondition = "Brandnew";
                                    break;
                                case TireItemClassEnum.SecondHandTire:
                                    tiac.LineCondition = "Recap";
                                    break;
                                case TireItemClassEnum.RecappedTire:
                                    tiac.LineCondition = "Recap";
                                    break;
                                case TireItemClassEnum.Flap:
                                    break;
                                case TireItemClassEnum.Tube:
                                    break;
                                case TireItemClassEnum.Rim:
                                    break;
                                case TireItemClassEnum.ScrappedTire:
                                    break;
                                case TireItemClassEnum.OriginalTire:
                                    break;
                                default:
                                    break;
                            }
                            tiac.Issue = rwstd.TireIssueId;
                            tiac.Qty = 1m;
                            tiac.Cost = rwstd.Cost;
                            tiac.Save();
                        }
                        #endregion

                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                    }
                }
                var rwss = trans[1] as IList<RequisitionWorksheet>;
                if (rwss != null && rwss.Count > 0)
                {
                    cnt += rwss.Count;
                    foreach (var item in rwss)
                    {
                        index++;
                        _message = string.Format("Processing item {0} succesfull.",
                        index);
                        _BgWorker.ReportProgress(1, _message);

                        #region Algorithms
                        if (item.Oid == 40751)
                        {

                        }
                        RequisitionWorksheet rwsh = session.GetObjectByKey<RequisitionWorksheet>(item.Oid);
                        //var last = rwsh.ReqCarryoutTransactions.LastOrDefault();
                        //if (last == null)
                        //{
                        //    continue;
                        //}
                        var tiac = session.FindObject<TireItemsAccomCostDetail>(BinaryOperator.Parse("[HeaderId] = ? And [SourceId] = ? And [SourceDetId] = ?", acc, rwsh.GenJournalID.SourceNo, rwsh.Oid));
                        if (tiac == null)
                        {
                            tiac = ReflectionHelper.CreateObject<TireItemsAccomCostDetail>(session);
                            tiac.HeaderId = acc;
                            tiac.SourceId = rwsh.GenJournalID.SourceNo;
                            tiac.SourceDetId = rwsh.Oid.ToString();
                        }
                        if (tiac != null)
                        {
                            tiac.EntryDate = rwsh.ExpectedDate;
                            //tiac.Year = rwsh.Year;
                            //tiac.Quarter = rwsh.Quarter;
                            //tiac.Month = rwsh.Month;
                            tiac.TireItemClass = rwsh.ItemNo.TirePartType;
                            tiac.TireType = TireTypeEnum.None;
                            tiac.ItemNo = rwsh.ItemNo;
                            tiac.Qty = rwsh.Quantity;
                            tiac.Cost = rwsh.Cost;
                            tiac.Save();
                        }
                        #endregion
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                    }
                }
                // Tire Service Detail 2
                StringBuilder ids = new StringBuilder();
                var tsd2 = trans[2] as IList<TireServiceDetail2>;
                if (tsd2 != null && tsd2.Count > 0)
                {
                    cnt += tsd2.Count;
                    foreach (var item in tsd2)
                    {
                        index++;
                        _message = string.Format("Processing item {0} succesfull.",
                        index);
                        _BgWorker.ReportProgress(1, _message);

                        #region Algorithms
                        //if (item.Oid == 14515)
                        //{

                        //}

                        if (item.BrandingNo == "4819000009")
                        {

                        }
                        bool lastDetIsScrap = false;
                        //DevExpress.Xpo.Metadata.XPClassInfo classinfo = session.GetClassInfo(typeof(TireServiceDetail2));
                        TireServiceDetail2 tsd = session.GetObjectByKey<TireServiceDetail2>(item.Oid);
                        if (tsd != null && tsd.ActivityType == TireActivityTypeEnum.Dettached)
                        {
                            if (tsd.Reason != null && tsd.Reason.Code == "SCRAP")
                            {
                                continue;
                                //lastDetIsScrap = true;
                            }
                            else if (tsd.Reason != null && tsd.Reason.Code == "FOR INSPECTION")
                            {
                                //lastDetIsScrap = true;
                                continue;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        var attaches = tsd.TireNo.TireServiceDetails2.Where(o => o.ActivityType == TireActivityTypeEnum.Attached).OrderBy(o => o.Oid);
                        if (tsd.TireNo.FirstActivityDate < DateTime.Parse("2019-01-10") && tsd.TireNo.Cost==0m && attaches != null && attaches.Count() == 1 && tsd.TireNo.TireServiceDetails2.OrderBy(o => o.Oid).Last().Reason.Code == "SCRAP")
                        {
                            continue;
                        }
                        //SortingCollection sorts = new SortingCollection(null);
                        ////var lastrecap = session.FindObject<TireServiceDetail2>(BinaryOperator.Parse("[Oid] > ? And [ReferenceNo] <> '' And StartsWith([Remarks], 'Received')", item.Oid));
                        //var lastrecap = session.GetObjects(classinfo,BinaryOperator.Parse("[TireNo.Oid] = ? And [Oid] < ?", item.TireNo.Oid, item.Oid), sorts, 0, false, false);
                        var lastrecap = item.TireNo.TireServiceDetails2.Where(o => o.Oid < item.Oid);
                        //var lastdetail = item.TireNo.TireServiceDetails2.Where(o => o.Oid == item.Oid).LastOrDefault();
                        TireServiceDetail2 lasAttached = null;
                        TireServiceDetail2 selRecap = null;
                        int c = 0;
                        bool valid = true;
                        if (lastrecap != null && lastrecap.Count()!=0)
                        {
                            for (int i = lastrecap.Count() - 1; i >= 0; i--)
                            {
                                if (c == 1)
                                {
                                    break;
                                }
                                if (tsd.ActivityType != TireActivityTypeEnum.Attached && lastrecap.ElementAt(i).GYear != _AccumulatedCost.Year)
                                {
                                    valid = false;
                                    break;
                                }
                                if (!string.IsNullOrEmpty(lastrecap.ElementAt(i).Remarks) && lastrecap.ElementAt(i).Remarks.Contains("Received"))
                                {
                                    // This line must be the culprit
                                    //if (item.BrandingNo == lastrecap.ElementAt(i).BrandingNo)
                                    //{
                                    //    break;
                                    //}
                                    selRecap = lastrecap.ElementAt(i);
                                    break;
                                }
                                else if (!string.IsNullOrEmpty(lastrecap.ElementAt(i).Remarks) && lastrecap.ElementAt(i).Remarks.Contains("TRANSFER"))
                                {
                                    valid = false;
                                    break;
                                }
                                else if (lastrecap.ElementAt(i).Reason.FirstBranding)
                                {
                                    // This line must be the culprit
                                    //if (item.BrandingNo == lastrecap.ElementAt(i).BrandingNo)
                                    //{
                                    //    break;
                                    //}
                                    selRecap = lastrecap.ElementAt(i);
                                    break;
                                }
                                else if (lastDetIsScrap)
                                {
                                    lasAttached = item.TireNo.TireServiceDetails2.Where(o => o.Oid < item.Oid).Where(o => o.ActivityType == TireActivityTypeEnum.Attached).LastOrDefault();
                                    if (lasAttached != null && lasAttached.GYear == _AccumulatedCost.Year)
                                    {
                                        selRecap = lasAttached;
                                    }
                                }
                                else
                                {
                                    valid = false;
                                    break;
                                }
                                c++;
                                if (selRecap!=null && selRecap.Remarks.Contains("ATTACHED"))
                                {
                                    selRecap = null;
                                }
                                if (selRecap != null && selRecap.Remarks.Contains("TRANSFER"))
                                {
                                    valid = false;
                                }
                            }
                        }
                        else
                        {
                            if (tsd.TireNo.TireItemClass== TireItemClassEnum.BrandNewTire)
                            {
                                valid = false;
                            }
                        }
                        //if (selRecap == null)
                        //{
                        //    continue;
                        //}
                        //var objLast = session.GetObjectByKey<TireServiceDetail2>(selRecap.Oid);
                        if (!valid)
                        {
                            continue;
                        }
                        TireServiceDetail2 objLast = null;
                        if (selRecap != null)
                        {
                            objLast = session.GetObjectByKey<TireServiceDetail2>(selRecap.Oid);
                        }
                        else
                        {
                            if (item.TireNo.TireItemClass != TireItemClassEnum.SecondHandTire)
                            {
                                if (!item.Reason.FirstBranding)
                                {
                                    continue;
                                }
                            }
                            if (ids.ToString().Contains(item.Oid.ToString()))
                            {
                                continue;
                            }
                        }

                        if (tsd != null && !ids.ToString().Contains(tsd.Oid.ToString()))
                        {
                            ids.Append(tsd.Oid + ",");
                        }
                        //else if (objLast != null && objLast.Remarks == "INITIAL ATTACHED")
                        //{
                        //    continue;
                        //}
                        else if (tsd != null && ids.ToString().Contains(tsd.Oid.ToString()))
                        {
                            continue;
                        }
                        if (tsd.TireNo.TireItem.Size.TireType == TireTypeEnum.Tubeless)
                        {
                            if (objLast != null)
                            {
                                var receipt = session.FindObject<Receipt>(BinaryOperator.Parse("[SourceNo] = ?", objLast.ReferenceNo)) ?? null;
                                if (receipt==null)
                                {
                                    continue;
                                }
                            }
                        }
                        var tiac = objLast!=null?session.FindObject<TireItemsAccomCostDetail>(BinaryOperator.Parse("[HeaderId] = ? And [SourceId] = ? And [EntryDate] = ?", acc, tsd.TireNo.TireNo, objLast.ActivityDate)):null;
                        if (tiac == null)
                        {
                            tiac = ReflectionHelper.CreateObject<TireItemsAccomCostDetail>(session);
                            tiac.HeaderId = acc;
                            tiac.SourceId = tsd.TireNo.TireNo;
                            tiac.SourceDetId = tsd.Oid.ToString();
                        }
                        if (tiac != null)
                        {
                            tiac.EntryDate = tsd.ActivityDate;
                            //tiac.Year = rwsh.Year;
                            //tiac.Quarter = rwsh.Quarter;
                            //tiac.Month = rwsh.Month;
                            if (tsd.TaId != null)
                            {
                                tiac.ReplacedTire = tsd.TaId.ReplacedTireDetail ?? null;
                            }
                            tiac.NewBranding = tsd;
                            tiac.LastRecap = objLast ?? null;
                            if (objLast != null && objLast.GYear==_AccumulatedCost.Year && objLast.ActivityType!= TireActivityTypeEnum.Dettached) 
                            {
                                if (objLast.Reason != null && objLast.Reason.FirstBranding)
                                {
                                    tiac.EntryDate = objLast.ActivityDate;
                                }
                                if (objLast.Reason != null && objLast.Reason.Code == "SCRAP")
                                {
                                    tiac.EntryDate = objLast.ActivityDate;
                                }
                                tiac.PreviousBranding = objLast.BrandingNo;
                                tiac.Remarks = objLast.Remarks;
                            }
                            else
                            {
                                if (objLast != null && objLast.Reason != null && objLast.Reason.FirstBranding)
                                {
                                    tiac.EntryDate = objLast.ActivityDate;
                                }
                                tiac.PreviousBranding = item.TireNo.SerialNo;
                                tiac.Remarks = "Initial Attach";
                            }
                            tiac.TireItemClass = AccTireItemClassEnum.Tire; // tsd.TireNo.TireItem.TirePartType;
                            if (tsd.TireNo.TireItem != null && tsd.TireNo.TireItem.Size != null)
                            {
                                tiac.TireType = tsd.TireNo.TireItem.Size.TireType;
                            }
                            else
                            {
                                tiac.TireType = TireTypeEnum.None;
                            }
                            tiac.ItemNo = tsd.TireNo.TireItem;
                            tiac.LineCondition = "Recap";
                            if (!string.IsNullOrEmpty(item.Remarks) && item.Remarks.Contains("TRANSFER"))
                            {
                                tiac.LineCondition = "Reused";
                            }
                            if (!string.IsNullOrEmpty(item.Remarks) && item.Remarks.Contains("REJECTED"))
                            {
                                tiac.LineCondition = "Rejected";
                            }
                            if (!string.IsNullOrEmpty(item.Remarks) && item.Remarks.Contains("REUSED"))
                            {
                                tiac.LineCondition = "Reused";
                            }
                            //if (tiac.LineCondition=="Recap" && tiac.RecapInvoice==null)
                            //{
                            //    continue;
                            //}
                            //if (item.Reason!=null && item.Reason.Code == "SCRAP")
                            //{
                            //    tiac.LineCondition = "Scrapped";
                            //}
                            tiac.Qty = 1;
                            tiac.Cost = tiac.LastRecapCost;
                            if (tsd.TireNo != null && tsd.TireNo.TireItemClass == TireItemClassEnum.SecondHandTire)
                            {
                                tiac.Cost = tsd.TireNo.TireItem.Cost;
                                if (tsd.Cost != 0m)
                                {
                                    tiac.Cost = tsd.Cost;
                                }
                            }
                            else if (tsd.TireNo != null && tsd.TireNo.TireItemClass != TireItemClassEnum.SecondHandTire)
                            {
                                if (tiac.LastRecapCost == 0m)
                                {
                                    tiac.Cost = tsd.TireNo.Cost;
                                }
                            }
                            if (selRecap != null && !string.IsNullOrEmpty(selRecap.Remarks) && selRecap.Remarks.Contains("NO CHARGE"))
                            {
                                tiac.Cost = 0m;
                            }
                            if (tiac.Cost == 0m)
                            {
                                tiac.Delete();
                            }
                            else if (tiac.TireType == TireTypeEnum.Tubeless && tiac.LineCondition == "Recap" && tiac.LastRecap == null)
                            {
                                tiac.Delete();
                            }
                            else
                            {
                                tiac.Save();
                            }
                        }
                        #endregion
                        if (_BgWorker.CancellationPending)
                        {
                            e.Cancel = true;
                            session.Dispose();
                            break;
                        }
                    }
                }
                acc.Save();
            }
            finally
            {
                if (index == cnt)
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
                    "Retrieving cost is cancelled.", "Cancelled",
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
                    "Retrieving cost has been successfull.");
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
