using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
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
    public partial class GeneratePartsPurchUsageController : ViewController
    {
        private PartsPurchasesUsageReporter reporter;
        private SimpleAction generatePartsPurchUsageAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GeneratePartsPurchUsageController()
        {
            this.TargetObjectType = typeof(PartsPurchasesUsageReporter);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.GeneratePartsPurchUsage", this.GetType().
            Name);
            this.generatePartsPurchUsageAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generatePartsPurchUsageAction.Caption = "Generate";
            this.generatePartsPurchUsageAction.Execute += new
            SimpleActionExecuteEventHandler(GeneratePartsPurchUsage_Execute);
            this.generatePartsPurchUsageAction.Executed += new EventHandler<
            ActionBaseEventArgs>(GeneratePartsPurchUsage_Executed);
            UpdateActionState(false);
        }
        private void GeneratePartsPurchUsage_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            reporter = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PartsPurchasesUsageReporter;
            // Delete PartsUsageIcjDetails
            ObjectSpace.CommitChanges();
            if (reporter.PartsUsageDetails.Count>0)
            {
                throw new UserFriendlyException("Cannot generate, details must not exist.");
            }
            XPCollection<Item> items = new XPCollection<Item>(((ObjectSpace)ObjectSpace).Session);
            var filter = new[] { ItemTypeEnum.InventoryItem, ItemTypeEnum.FuelItem, ItemTypeEnum.RepairItem, ItemTypeEnum.TireItem };
            var included = items.Where(o => filter.Contains(o.ItemType));
            var count = included.Count(); // Count of Items to be included
            _FrmProgress = new ProgressForm("Processing items...", count,
            "Processing item {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(included);
            _FrmProgress.ShowDialog();
        }

        private void GeneratePartsPurchUsage_Executed(object sender,
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
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IEnumerable<Item> trans = (IEnumerable<Item>)e.Argument;
            PartsPurchasesUsageReporter oReporter = session.GetObjectByKey<PartsPurchasesUsageReporter>(reporter.Oid);
            try
            {
                DateTime startDate = new DateTime(); // = oReporter.FromDate;
                DateTime endDate = new DateTime(); // = oReporter.ToDate.AddDays(1);

                if (oReporter != null)
                {
                    switch (oReporter.PeriodType)
                    {
                        case ReportPeriodicTypeEnum.Yearly:
                            startDate = new DateTime(oReporter.Year, 1, 1);
                            endDate = new DateTime(oReporter.Year + 1, 1, 1);
                            break;
                        case ReportPeriodicTypeEnum.Monthly:
                            startDate = new DateTime(oReporter.Year, (int)oReporter.Month, 1);
                            endDate = new DateTime(oReporter.Year, (int)oReporter.Month + 1, 1);
                            break;
                        case ReportPeriodicTypeEnum.Range:
                            startDate = oReporter.FromDate;
                            endDate = oReporter.ToDate.AddDays(1);
                            break;
                        default:
                            break;
                    }
                }
                foreach (Item item in trans)
                {
                    index++;

                    //if (item.No == "SPR026020")
                    //{

                    //}
                    #region Algorithms here...
                    DateTime rStartDate = startDate;
                    Item oItm = session.GetObjectByKey<Item>(item.Oid);
                    if (oItm.InventoryControlJournals.Count>0)
                    {
                        //int[] inds = null;
                        do
                        {
                            IEnumerable<InventoryControlJournal> dayIcjs = oItm.InventoryControlJournals.Where(o => o.EntryDate.Date == rStartDate.Date).OrderBy(o => o.Sequence);
                            PartsUsageTempCollection usageCollection = new PartsUsageTempCollection();
                            PartsUsageTemp usage;

                            if (dayIcjs.Count() > 0)
                            {
                                foreach (InventoryControlJournal icj in dayIcjs)
                                {
                                    // Purchases
                                    if (icj.SourceTypeCode == "RC")
                                    {
                                        ReceiptDetail orcdt = session.FindObject<ReceiptDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID.ToString()));
                                        var dat = usageCollection.Where(o => o.SourceType == "RC").FirstOrDefault();
                                        if (dat != null)
                                        {
                                            //dat = usageCollection[inds[0]];
                                            dat.Qty += icj.InQTY;
                                            //usage.Sales += icj.Price;
                                            dat.Cost += orcdt.Total;
                                            dat.Icjs.Add(icj);
                                        }
                                        else
                                        {
                                            usage = new PartsUsageTemp();
                                            usage.SourceType = "RC";
                                            usage.Qty += icj.InQTY;
                                            //usage.Sales += icj.Price;
                                            usage.Cost += orcdt.Total;
                                            usage.Icjs.Add(icj);
                                            usageCollection.Add(usage);
                                        }
                                    }
                                    // Parts Sold to Customers
                                    if (icj.SourceTypeCode == "IN")
                                    {
                                        InvoiceDetail oindt = session.FindObject<InvoiceDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID.ToString()));
                                        var dat = usageCollection.Where(o => o.SourceType == "IN").FirstOrDefault();
                                        if (dat != null)
                                        {
                                            //dat = usageCollection[inds[0]];
                                            dat.Qty += icj.OutQty;
                                            dat.Sales += oindt.Total;
                                            //usage.Cost += icj.Cost;
                                            dat.Icjs.Add(icj);
                                        }
                                        else
                                        {
                                            usage = new PartsUsageTemp();
                                            usage.SourceType = "IN";
                                            usage.Qty += icj.OutQty;
                                            usage.Sales += oindt.Total;
                                            //usage.Cost += icj.Cost;
                                            usage.Icjs.Add(icj);
                                            usageCollection.Add(usage);
                                        }
                                    }
                                    // Parts Used for Repairs
                                    if (icj.SourceTypeCode == "WO")
                                    {
                                        WorkOrderItemDetail oworitmd = session.FindObject<WorkOrderItemDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID.ToString()));
                                        var dat = usageCollection.Where(o=>o.SourceType=="WO").FirstOrDefault();
                                        if (dat != null)
                                        {
                                            //usage = usageCollection[inds[0]];
                                            dat.Qty += icj.OutQty;
                                            //usage.Sales += icj.Price;
                                            dat.Cost += oworitmd.Total;
                                            dat.Icjs.Add(icj);
                                        }
                                        else
                                        {
                                            usage = new PartsUsageTemp();
                                            usage.SourceType = "WO";
                                            usage.Qty += icj.OutQty;
                                            //usage.Sales += icj.Price;
                                            usage.Cost += oworitmd.Total;
                                            usage.Icjs.Add(icj);
                                            usageCollection.Add(usage);
                                        }
                                    }
                                    // Parts Charge to Employees
                                    if (icj.SourceTypeCode == "ECS")
                                    {
                                        EmployeeChargeSlipItemDetail oecsidt = session.FindObject<EmployeeChargeSlipItemDetail>(BinaryOperator.Parse("[RowID]=?", icj.RowID.ToString()));
                                        var dat = usageCollection.Where(o => o.SourceType == "ECS").FirstOrDefault();
                                        if (dat != null)
                                        {
                                            //dat = usageCollection[inds[0]];
                                            dat.Qty += icj.OutQty;
                                            //usage.Sales += oecsidt.Total;
                                            dat.Cost += oecsidt.Total;
                                            dat.Icjs.Add(icj);
                                        }
                                        else
                                        {
                                            usage = new PartsUsageTemp();
                                            usage.SourceType = "ECS";
                                            usage.Qty += icj.OutQty;
                                            //usage.Sales += oecsidt.Total;
                                            usage.Cost += oecsidt.Total;
                                            usage.Icjs.Add(icj);
                                            usageCollection.Add(usage);
                                        }
                                    }
                                }

                                foreach (PartsUsageTemp put in usageCollection)
                                {
                                    PartsPurchasesUsageDetail ppud = ReflectionHelper.CreateObject<PartsPurchasesUsageDetail>(session);
                                    ppud.ReporterID = oReporter;
                                    ppud.EntryDate = rStartDate.Date;
                                    int st = 0;
                                    PartsPurchasesUseCode ppuc = null;
                                    if (put.SourceType == "RC")
                                    {
                                        st = 1;
                                        ppuc = session.FindObject<PartsPurchasesUseCode>(BinaryOperator.Parse("[Code]=?", "PPRCHS"));
                                        var data = put.Icjs.Where(o => o.SourceTypeCode == "RC");
                                        foreach (var icj in data)
                                        {
                                            InventoryControlJournal picj = session.GetObjectByKey<InventoryControlJournal>(icj.Oid);
                                            PartsUsageIcjDetails puid = ReflectionHelper.CreateObject<PartsUsageIcjDetails>(session);
                                            puid.EntryDate = picj.EntryDate;
                                            puid.ReporterID = oReporter;
                                            puid.LineNo = picj.Oid.ToString();
                                            puid.ItemNo = picj.ItemNo;
                                            puid.SeqNo = picj.Sequence;
                                            puid.Qty = Math.Abs(picj.Qty);
                                            puid.Source = picj.GenJournalID;
                                            puid.RequisitionNo = picj.RequisitionNo;
                                            puid.CostPrice = put.Cost / put.Qty;
                                            ppud.PartsUsageIcjDetails.Add(puid);
                                        }
                                    }
                                    else if (put.SourceType == "IN")
                                    {
                                        st = 2;
                                        ppuc = session.FindObject<PartsPurchasesUseCode>(BinaryOperator.Parse("[Code]=?", "PRTSLD"));
                                        var data = put.Icjs.Where(o => o.SourceTypeCode == "IN");
                                        foreach (var icj in data)
                                        {
                                            InventoryControlJournal picj = session.GetObjectByKey<InventoryControlJournal>(icj.Oid);
                                            PartsUsageIcjDetails puid = ReflectionHelper.CreateObject<PartsUsageIcjDetails>(session);
                                            puid.EntryDate = picj.EntryDate;
                                            puid.ReporterID = oReporter;
                                            puid.LineNo = picj.Oid.ToString();
                                            puid.ItemNo = picj.ItemNo;
                                            puid.SeqNo = picj.Sequence;
                                            puid.Qty = Math.Abs(picj.Qty);
                                            puid.Source = picj.GenJournalID;
                                            puid.RequisitionNo = picj.RequisitionNo;
                                            puid.CostPrice = put.Sales / put.Qty;
                                            ppud.PartsUsageIcjDetails.Add(puid);
                                        }
                                    }
                                    else if (put.SourceType == "WO")
                                    {
                                        st = 3;
                                        ppuc = session.FindObject<PartsPurchasesUseCode>(BinaryOperator.Parse("[Code]=?", "RPSMNT"));
                                        var data = put.Icjs.Where(o => o.SourceTypeCode == "WO");
                                        foreach (var icj in data)
                                        {
                                            InventoryControlJournal picj = session.GetObjectByKey<InventoryControlJournal>(icj.Oid);
                                            PartsUsageIcjDetails puid = ReflectionHelper.CreateObject<PartsUsageIcjDetails>(session);
                                            puid.EntryDate = picj.EntryDate;
                                            puid.ReporterID = oReporter;
                                            puid.LineNo = picj.Oid.ToString();
                                            puid.ItemNo = picj.ItemNo;
                                            puid.SeqNo = picj.Sequence;
                                            puid.Qty = Math.Abs(picj.Qty);
                                            puid.Source = picj.GenJournalID;
                                            puid.RequisitionNo = picj.RequisitionNo;
                                            puid.CostPrice = put.Cost / put.Qty;
                                            ppud.PartsUsageIcjDetails.Add(puid);
                                        }
                                    }
                                    else if (put.SourceType == "ECS")
                                    {
                                        st = 4;
                                        ppuc = session.FindObject<PartsPurchasesUseCode>(BinaryOperator.Parse("[Code]=?", "EMCHRG"));
                                        var data = put.Icjs.Where(o => o.SourceTypeCode == "ECS");
                                        foreach (var icj in data)
                                        {
                                            InventoryControlJournal picj = session.GetObjectByKey<InventoryControlJournal>(icj.Oid);
                                            PartsUsageIcjDetails puid = ReflectionHelper.CreateObject<PartsUsageIcjDetails>(session);
                                            puid.EntryDate = picj.EntryDate;
                                            puid.ReporterID = oReporter;
                                            puid.LineNo = picj.Oid.ToString();
                                            puid.ItemNo = picj.ItemNo;
                                            puid.SeqNo = picj.Sequence;
                                            puid.Qty = Math.Abs(picj.Qty);
                                            puid.Source = picj.GenJournalID;
                                            puid.RequisitionNo = picj.RequisitionNo;
                                            puid.CostPrice = put.Cost / put.Qty;
                                            ppud.PartsUsageIcjDetails.Add(puid);
                                        }
                                    }
                                    ppud.LineNo = string.Format("{0}{1}{2}", rStartDate.Date.ToString("yyyMMdd"), st.ToString("D2"), oItm.No);
                                    ppud.ItemNo = oItm;
                                    ppud.UsageCode = ppuc;
                                    ppud.Qty = put.Qty;
                                    ppud.Sales = put.Sales > 0 ? put.Sales / put.Qty : 0;
                                    ppud.Cost = put.Cost > 0 ? put.Cost / put.Qty : 0;
                                    ppud.Save();
                                }
                            }

                            rStartDate = rStartDate.AddDays(1);

                        } while (rStartDate.Date != endDate.Date);
                    }
                    oReporter.Save();
                    CommitUpdatingSession(session);

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }

                    _message = string.Format("Processing {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.Count())
                {
                    e.Result = index;
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
                    "Processing items has been cancelled", "Cancelled",
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
                    " has been successfully processed");

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
            generatePartsPurchUsageAction.
                Enabled.SetItemValue("Processing items", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
