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
    public partial class GenerateTripsIncomeExpenseController : ViewController
    {
        private TripsIncomeExpenseReporter reporter;
        private SimpleAction generateTripsIncomeExpenseAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateTripsIncomeExpenseController()
        {
            this.TargetObjectType = typeof(TripsIncomeExpenseReporter);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.GenerateTripsIncomeExpense", this.GetType().
            Name);
            this.generateTripsIncomeExpenseAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateTripsIncomeExpenseAction.Caption = "Generate";
            this.generateTripsIncomeExpenseAction.Execute += new
            SimpleActionExecuteEventHandler(GenerateTripsIncomeExpense_Execute);
            this.generateTripsIncomeExpenseAction.Executed += new EventHandler<
            ActionBaseEventArgs>(GenerateTripsIncomeExpense_Executed);
            UpdateActionState(false);
        }
        private void GenerateTripsIncomeExpense_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            reporter = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as TripsIncomeExpenseReporter;
            ObjectSpace.CommitChanges();
            if (reporter.TripsPerFuelGroups.Count > 0)
            {
                throw new UserFriendlyException("Cannot generate, details must not exist.");
            }

            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();

            switch (reporter.PeriodType)
            {
                case ReportPeriodicTypeEnum.Yearly:
                    startDate = new DateTime(reporter.Year, 1, 1);
                    endDate = new DateTime(reporter.Year + 1, 1, 1);
                    break;
                case ReportPeriodicTypeEnum.Monthly:
                    startDate = new DateTime(reporter.Year, (int)reporter.Month, 1);
                    endDate = new DateTime(reporter.Year, (int)reporter.Month + 1, 1);
                    break;
                case ReportPeriodicTypeEnum.Range:
                    startDate = reporter.FromDate;
                    endDate = reporter.ToDate.AddDays(1);
                    break;
                default:
                    break;
            }

            XPCollection<ReceiptFuel> items = new XPCollection<ReceiptFuel>(((ObjectSpace)ObjectSpace).Session);

            IEnumerable<GenJournalHeader> included = null;

            included = items.Where(o => o.TruckOrGenset == TruckOrGensetEnum.Truck && o.EntryDate.Date >= startDate.Date && o.EntryDate.Date <= endDate.AddDays(-1));
            //included = items.Where(o => o.SourceNo == "RF0000058030");

            var count = included.Count(); // Count of Items to be included
            _FrmProgress = new ProgressForm("Processing receipts...", count,
            "Processing receipt {0} of {1} ");
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

        private void GenerateTripsIncomeExpense_Executed(object sender,
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
            IEnumerable<ReceiptFuel> trans = (IEnumerable<ReceiptFuel>)e.Argument;
            TripsIncomeExpenseReporter oReporter = session.GetObjectByKey<TripsIncomeExpenseReporter>(reporter.Oid);
            try
            {
                foreach (ReceiptFuel item in trans)
                {
                    index++;

                    #region Algorithms here...

                    ArrayList frcpts = new ArrayList();
                    ArrayList tripsa = new ArrayList();

                    TripsIerPerFuelGroup tirpg = null;
                    ReceiptFuel rcptf = session.GetObjectByKey<ReceiptFuel>(item.Oid);

                    var srcTypeFilter = new[] { "ST", "DF", "OT" };
                    var vttrips = rcptf.ReceiptFuelUsageDetails.Where(o => srcTypeFilter.Contains(o.TripNo.SourceTypeCode) && o.TripNo.TripCustomerNo == oReporter.TripCustomer.No);

                    if (vttrips.Count() > 0)
                    {
                        tirpg = ReflectionHelper.CreateObject<TripsIerPerFuelGroup>(session);
                        tirpg.ReporterID = oReporter;
                        tirpg.LineID = rcptf.Oid;
                        tirpg.FuelReceipt = rcptf;

                        if (frcpts.Contains(rcptf.SourceNo))
                        {
                            continue;
                        }

                        frcpts.Add(rcptf.SourceNo);

                        TripsFuelExpenseDetail tfesd = session.FindObject<TripsFuelExpenseDetail>(CriteriaOperator.Parse("[SourceNo.SourceNo]=?", rcptf.SourceNo));
                        if (tfesd == null)
                        {
                            tfesd = ReflectionHelper.CreateObject<TripsFuelExpenseDetail>(session);
                            tfesd.DetailID = tirpg;
                        }

                        tfesd.EntryDate = rcptf.EntryDate;
                        tfesd.SourceNo = rcptf;
                        tfesd.InvoiceNo = rcptf.InvoiceNo;
                        tfesd.Driver = rcptf.Driver;
                        tfesd.Dtrs = rcptf.DtrNo;
                        switch (rcptf.TruckOrGenset)
                        {
                            case TruckOrGensetEnum.Truck:
                                tfesd.UnitNo = rcptf.TruckNo;
                                break;
                            case TruckOrGensetEnum.Genset:
                                tfesd.UnitNo = rcptf.GensetNo;
                                break;
                            case TruckOrGensetEnum.NotApplicable:
                                break;
                            case TruckOrGensetEnum.Other:
                                tfesd.UnitNo = rcptf.OtherVehicle;
                                break;
                            default:
                                break;
                        }
                        tfesd.TotalQty = rcptf.TotalQty ?? 0;
                        tfesd.Total = rcptf.Total ?? 0;

                        tfesd.Save();

                        foreach (var rfud in vttrips)
                        {
                            GenJournalHeader trip = session.GetObjectByKey<GenJournalHeader>(rfud.TripNo.Oid);
                            if (trip != null)
                            {
                                if (tripsa.Contains(trip.SourceNo))
                                {
                                    continue;
                                }

                                tripsa.Add(trip.SourceNo);

                                TripsIncomeDetail tidet = session.FindObject<TripsIncomeDetail>(CriteriaOperator.Parse("[SourceNo.SourceNo]=?", trip.SourceNo));
                                if (tidet == null)
                                {
                                    tidet = ReflectionHelper.CreateObject<TripsIncomeDetail>(session);
                                    tidet.DetailID = tirpg;
                                }

                                tidet.EntryDate = trip.EntryDate;
                                tidet.SourceNo = trip;
                                tidet.DocumentNo = trip.TripReferenceNo;
                                tidet.Driver = trip.TripDriver;
                                tidet.UnitNo = trip.TripUnit;
                                tidet.Origin = trip.TripOrigin;
                                tidet.Destination = trip.TripDestination;
                                tidet.TruckerPay = trip.TripTruckerPay;
                                tidet.NetBilling = trip.TripNetBilling;

                                tidet.Save();

                                foreach (var rfl in trip.FuelRegistrations)
                                {
                                    if (frcpts.Contains(rfl.SourceNo))
                                    {
                                        continue;
                                    }

                                    frcpts.Add(rfl.SourceNo);

                                    ReceiptFuel rcptf2 = session.FindObject<ReceiptFuel>(CriteriaOperator.Parse("[SourceNo]=?", rfl.SourceNo));
                                    TripsFuelExpenseDetail tfesd2 = session.FindObject<TripsFuelExpenseDetail>(CriteriaOperator.Parse("[SourceNo.SourceNo]=?", rcptf2.SourceNo));
                                    if (tfesd2 == null)
                                    {
                                        tfesd2 = ReflectionHelper.CreateObject<TripsFuelExpenseDetail>(session);
                                        tfesd2.DetailID = tirpg;
                                    }

                                    tfesd2.EntryDate = rcptf2.EntryDate;
                                    tfesd2.SourceNo = rcptf2;
                                    tfesd2.InvoiceNo = rcptf2.InvoiceNo;
                                    tfesd2.Driver = rcptf2.Driver;
                                    tfesd2.Dtrs = rcptf2.DtrNo;
                                    switch (rcptf2.TruckOrGenset)
                                    {
                                        case TruckOrGensetEnum.Truck:
                                            tfesd2.UnitNo = rcptf2.TruckNo;
                                            break;
                                        case TruckOrGensetEnum.Genset:
                                            tfesd2.UnitNo = rcptf2.GensetNo;
                                            break;
                                        case TruckOrGensetEnum.NotApplicable:
                                            break;
                                        case TruckOrGensetEnum.Other:
                                            tfesd2.UnitNo = rcptf2.OtherVehicle;
                                            break;
                                        default:
                                            break;
                                    }
                                    tfesd2.TotalQty = rcptf2.TotalQty ?? 0;
                                    tfesd2.Total = rcptf2.Total ?? 0;
                                    tfesd2.Save();
                                }
                            }
                        }

                        tirpg.Save();
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
                    "Processing trips has been cancelled", "Cancelled",
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
            generateTripsIncomeExpenseAction.
                Enabled.SetItemValue("Processing trips", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
