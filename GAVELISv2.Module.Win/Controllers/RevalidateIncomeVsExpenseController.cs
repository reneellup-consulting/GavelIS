using System;
using System.Linq;
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
    public partial class RevalidateIncomeVsExpenseController : ViewController
    {
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        private SimpleAction _RevalidateIncomeVsExpenseAction;
        public RevalidateIncomeVsExpenseController()
        {
            this.TargetObjectType = typeof(IncomeAndExpense02);
            this.TargetViewType = ViewType.ListView;
            string actionId = "IncomeAndExpense02.Revalidate";
            _RevalidateIncomeVsExpenseAction = new SimpleAction(this, actionId, PredefinedCategory.RecordEdit);
            _RevalidateIncomeVsExpenseAction.Caption = "Revalidate Entries";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Do you really want to revalidate all entries?");
            sb.AppendLine("It will take several minutes to complete.");
            _RevalidateIncomeVsExpenseAction.ConfirmationMessage = sb.ToString();
            _RevalidateIncomeVsExpenseAction.Execute += new SimpleActionExecuteEventHandler(_RevalidateIncomeVsExpenseAction_Execute);
        }

        void _RevalidateIncomeVsExpenseAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            XPCollection<IncomeAndExpense02> incxs = new XPCollection<IncomeAndExpense02>(((ObjectSpace)this.ObjectSpace).Session);
            var count = incxs.Count;
            _FrmProgress = new ProgressForm("Validating entry...", count,
            "Entries validated {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(incxs);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<IncomeAndExpense02> args = e.Argument as XPCollection<IncomeAndExpense02>;
            try
            {
                foreach (IncomeAndExpense02 item in args)
                {
                    index++;
                    _message = string.Format("Revalidating entry {0} succesfull.",
                    args.Count - 1);
                    _BgWorker.ReportProgress(1, _message);

                    #region MyRegion
                    IncomeAndExpense02 inc = session.GetObjectByKey<IncomeAndExpense02>(item.Oid);
                    Console.WriteLine(inc.SourceNo);
                    if (inc != null && inc.SourceID != null)
                    {
                        // If expense type is trucking
                        if (inc.Category!=null && inc.Category.Trucking)
                        {
                            if (inc.SourceType.Code == "ST")
                            {
                                if (((StanfilcoTrip)inc.SourceID).TruckNo!=null)
                                {
                                    inc.Fleet = ((StanfilcoTrip)inc.SourceID).TruckNo;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "SH")
                            {
                                if (((ShuntingEntry)inc.SourceID).TruckNo != null)
                                {
                                    inc.Fleet = ((ShuntingEntry)inc.SourceID).TruckNo;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "KD")
                            {
                                if (((KDEntry)inc.SourceID).TruckNo != null)
                                {
                                    inc.Fleet = ((KDEntry)inc.SourceID).TruckNo;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "OT")
                            {
                                if (((OtherTrip)inc.SourceID).TruckNo != null)
                                {
                                    inc.Fleet = ((OtherTrip)inc.SourceID).TruckNo;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "GS")
                            {
                                if (((GensetEntry)inc.SourceID).TruckNo != null)
                                {
                                    inc.Fleet = ((GensetEntry)inc.SourceID).TruckNo;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "DF")
                            {
                                if (((DolefilTrip)inc.SourceID).TruckNo != null)
                                {
                                    inc.Fleet = ((DolefilTrip)inc.SourceID).TruckNo;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "BE")
                            {
                                if (inc.CostCenter!=null && inc.CostCenter.FixedAsset!=null)
                                {
                                    inc.Fleet = inc.CostCenter.FixedAsset;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "ECS")
                            {
                                if (inc.CostCenter != null && inc.CostCenter.FixedAsset != null)
                                {
                                    inc.Fleet = inc.CostCenter.FixedAsset;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "CR")
                            {
                                if (inc.CostCenter != null && inc.CostCenter.FixedAsset != null)
                                {
                                    inc.Fleet = inc.CostCenter.FixedAsset;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "RFL")
                            {
                                switch (((ReceiptFuel)inc.SourceID).TruckOrGenset)
                                {
                                    case TruckOrGensetEnum.Truck:
                                        inc.Fleet = ((ReceiptFuel)inc.SourceID).TruckNo??null;
                                        break;
                                    case TruckOrGensetEnum.Genset:
                                        inc.Fleet = ((ReceiptFuel)inc.SourceID).GensetNo??null;
                                        break;
                                    case TruckOrGensetEnum.NotApplicable:
                                        break;
                                    case TruckOrGensetEnum.Other:
                                        inc.Fleet = ((ReceiptFuel)inc.SourceID).OtherVehicle ?? null;
                                        break;
                                    default:
                                        break;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "WO")
                            {
                                if (inc.CostCenter != null && inc.CostCenter.FixedAsset != null)
                                {
                                    inc.Fleet = inc.CostCenter.FixedAsset;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "JO")
                            {
                                if (inc.CostCenter != null && inc.CostCenter.FixedAsset != null)
                                {
                                    inc.Fleet = inc.CostCenter.FixedAsset;
                                }
                                inc.Save();
                            }
                            if (inc.SourceType.Code == "RC")
                            {
                                if (inc.CostCenter != null && inc.CostCenter.FixedAsset != null)
                                {
                                    inc.Fleet = inc.CostCenter.FixedAsset;
                                }
                                inc.Save();
                            }
                        }
                        if (!string.IsNullOrEmpty(inc.Description2) && inc.Description2.StartsWith("Check and Petty Cash"))
                        {
                            GenJournalDetail gjdt = session.GetObjectByKey<GenJournalDetail>(inc.RefID);
                            if (gjdt == null)
                            {
                                Console.WriteLine(string.Format("Entry no {0} Gen. Journal Detail #{1} cannot be retrieved.", inc.Oid, inc.RefID));
                                inc.Delete();
                                session.CommitChanges();
                            }
                            else
                            {
                                inc.EntryDate = gjdt.CVLineDate == DateTime.MinValue ? inc.EntryDate : gjdt.CVLineDate;
                                inc.Seq = inc.EntryDate.ToUniversalTime();
                                inc.Save();
                                session.CommitChanges();
                            }
                        }
                        else
                        {
                            inc.EntryDate = inc.SourceID.EntryDate;
                            inc.Seq = inc.EntryDate.ToUniversalTime();
                            inc.Save();
                            session.CommitChanges();
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("Entry no {0} has no source.", inc.Oid));
                        inc.Delete();
                        session.CommitChanges();
                    }
                    //Console.WriteLine(string.Format("Revalidating entry {0} succesfull.", index));
                    //System.Threading.Thread.Sleep(20);
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
                if (index == args.Count)
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
                "Revalidating entries operation has been cancelled", "Cancelled",
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
                    " has been successfully revalidated");
                    //ObjectSpace.ReloadObject(_TireForSale);
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
            _RevalidateIncomeVsExpenseAction.
            Enabled.SetItemValue("Revalidating entries", !inProgress);
        }

        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
