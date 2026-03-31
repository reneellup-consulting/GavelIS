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
    public partial class GenerateGrossTaxablePurchController : ViewController
    {
        private SimpleAction generateGrossTaxablePurch;
        private GrossTaxablePurchasesHeader _GtpHeader;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateGrossTaxablePurchController()
        {
            this.TargetObjectType = typeof(GrossTaxablePurchasesHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GrossTaxablePurchasesHeader.GenerateGrossTaxablePurch";
            this.generateGrossTaxablePurch = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateGrossTaxablePurch.Caption = "Generate";
            this.generateGrossTaxablePurch.Execute += new
            SimpleActionExecuteEventHandler(
            GenerateDriverPayrollActionAction_Execute);
        }
        List<TaxablePurchaseTmp> taxablePurchTmp = new List<TaxablePurchaseTmp>();
        private void GenerateDriverPayrollActionAction_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _GtpHeader = ((DevExpress.ExpressApp.DetailView)this.View).
    CurrentObject as GrossTaxablePurchasesHeader;

            try
            {
                for (int i = _GtpHeader.GrossTaxablePurchasesDetails.Count - 1;
                i >= 0; i--)
                {
                    _GtpHeader.GrossTaxablePurchasesDetails[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();
            taxablePurchTmp.Clear();
            // [Entry Date] >= #2018-01-01# And [Entry Date] < #2018-02-01#
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now; ;
            switch (_GtpHeader.GtpGenerationTypeEnum)
            {
                case GtpGenerationTypeEnum.Monthly:
                    startDate = new DateTime(_GtpHeader.Year, (int)_GtpHeader.Month, 1);
                    endDate = (new DateTime(_GtpHeader.Year, (int)_GtpHeader.Month, DateTime.DaysInMonth(_GtpHeader.Year, (int)_GtpHeader.Month))).AddDays(1);
                    break;
                case GtpGenerationTypeEnum.Range:
                    startDate = _GtpHeader.StartDate;
                    endDate = _GtpHeader.EndDate.AddDays(1);
                    break;
                default:
                    break;
            }
            // Receipts
            string crit = string.Format("[EntryDate] >= #{0}# And [EntryDate] < #{1}# And Not IsNullOrEmpty([Vendor.TIN001])", startDate.ToString("yyy-MM-dd"), endDate.ToString("yyy-MM-dd"));
            IList<Receipt> rcpts = ObjectSpace.GetObjects<Receipt>(CriteriaOperator.Parse(crit));
            foreach (var item in rcpts)
            {
                if (item.Approved && item.Vendor.VatTaxable && item.InvoiceType == InvoiceTypeEnum.Charge)
                {
                    taxablePurchTmp.Add(new TaxablePurchaseTmp()
                    {
                        Tin = string.Format("{0}-{1}-{2}", item.Vendor.TIN001, item.Vendor.TIN002, item.Vendor.TIN003),
                        Source = item,
                        RegName = item.Vendor.Name,
                        SuppAddress = item.Vendor.FullAddress,
                        AmountOfPurch = item.AmtOfGrossPurch,
                        AmountOfTaxPurch = item.AmtOfTaxablePurch,
                        AmountOfPurchOfServ = item.AmtOfPurchOfSrvcs,
                        AmountOfInputTax = item.AmtOfInputTax,
                        Total = item.Total.Value
                    });
                }
            }
            // ReceiptFuel
            IList<ReceiptFuel> rcptfs = ObjectSpace.GetObjects<ReceiptFuel>(CriteriaOperator.Parse(crit));
            foreach (var item in rcptfs)
            {
                if (item.Vendor.VatTaxable)
                {
                    taxablePurchTmp.Add(new TaxablePurchaseTmp()
                    {
                        Tin = string.Format("{0}-{1}-{2}", item.Vendor.TIN001, item.Vendor.TIN002, item.Vendor.TIN003),
                        Source = item,
                        RegName = item.Vendor.Name,
                        SuppAddress = item.Vendor.FullAddress,
                        AmountOfPurch = item.AmtOfGrossPurch,
                        AmountOfTaxPurch = item.AmtOfTaxablePurch,
                        AmountOfPurchOfServ = item.AmtOfPurchOfSrvcs,
                        AmountOfInputTax = item.AmtOfInputTax,
                        Total = item.Total.Value
                    });
                }
            }
            // JobOrders
            IList<JobOrder> jobOrders = ObjectSpace.GetObjects<JobOrder>(CriteriaOperator.Parse(crit));
            foreach (var item in jobOrders)
            {
                if (item.Approved && item.Vendor.VatTaxable)
                {
                    taxablePurchTmp.Add(new TaxablePurchaseTmp()
                    {
                        Tin = string.Format("{0}-{1}-{2}", item.Vendor.TIN001, item.Vendor.TIN002, item.Vendor.TIN003),
                        Source = item,
                        RegName = item.Vendor.Name,
                        SuppAddress = item.Vendor.FullAddress,
                        AmountOfPurch = item.AmtOfGrossPurch,
                        AmountOfTaxPurch = item.AmtOfTaxablePurch,
                        AmountOfPurchOfServ = item.AmtOfPurchOfSrvcs,
                        AmountOfInputTax = item.AmtOfInputTax,
                        Total = item.Total.Value
                    });
                }
            }

            if (taxablePurchTmp.Count == 0)
            {
                throw new UserFriendlyException("There are no records found");
            }

            _FrmProgress = new ProgressForm("Generating data...", taxablePurchTmp.Count,
            "Records processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(taxablePurchTmp);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            List<TaxablePurchaseTmp> _list = (List<TaxablePurchaseTmp>)e.Argument;
            GrossTaxablePurchasesHeader gross = session.GetObjectByKey<GrossTaxablePurchasesHeader>(_GtpHeader.Oid);
            try
            {
                foreach (TaxablePurchaseTmp item in _list)
                {
                    index++;
                    _message = string.Format("Processing record {0} succesfull.",
                    item.Source.SourceNo);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    GenJournalHeader gjh = session.GetObjectByKey<GenJournalHeader>(item.Source.Oid);
                    GrossTaxablePurchasesDetail grt = ReflectionHelper.CreateObject<GrossTaxablePurchasesDetail>(session);
                    grt.Source = gjh;
                    grt.ParentID = gross;
                    grt.Tin = item.Tin;
                    grt.RegisteredName = item.RegName;
                    grt.SuppliersAddress = item.SuppAddress;
                    grt.AmountOfGrossPurchase = item.AmountOfPurch;
                    grt.AmountOfTaxablePurchases = item.AmountOfTaxPurch;
                    grt.AmountOfPurchaseOfServices = item.AmountOfPurchOfServ;
                    grt.AmountOfInputTax = item.AmountOfInputTax;
                    grt.AmountOfGrossTaxablePurchase = item.Total;
                    grt.Total = item.Total;
                    grt.Save();
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
                    "Generation of Gross Taxable Purchases data is cancelled.", "Cancelled",
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
                    "Gross Taxable Purchases data has been successfully generated.");
                    //ObjectSpace.ReloadObject(_IncomeStatement);
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

    public class TaxablePurchaseTmp
    {
        public GenJournalHeader Source { get; set; }
        public string Tin { get; set; }
        public string RegName { get; set; }
        public string SuppAddress { get; set; }
        public decimal AmountOfPurch { get; set; }
        public decimal AmountOfTaxPurch { get; set; }
        public decimal AmountOfPurchOfServ { get; set; }
        public decimal AmountOfInputTax { get; set; }
        public decimal Total { get; set; }
    }
}
