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
    public partial class GenerateInitialCashSalesController : ViewController
    {
        private SimpleAction generateInitialCashSales;
        private CashSale _CashSales;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateInitialCashSalesController()
        {
            this.TargetObjectType = typeof(CashSale);
            this.TargetViewType = ViewType.ListView;
            string actionID = "GenerateInitialCashSales";
            this.generateInitialCashSales = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.generateInitialCashSales.Caption =
            "Generate Initial Cash Sales";
            this.generateInitialCashSales.Execute += new 
            SimpleActionExecuteEventHandler(
            generateInitialCashSales_Execute);
        }

        private void generateInitialCashSales_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            IList<CashSale> cashSales = ((DevExpress.ExpressApp.ListView)this.View).Items as IList<CashSale>;
            try {
                for (int i = cashSales.Count - 1;
                i >= 0; i--)
                {
                    cashSales[i].Delete(
                        );
                }
            } catch(Exception) {
            }

            ObjectSpace.CommitChanges();
            UnitOfWork session = CreateUpdatingSession();
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            ICollection invoices;
            DevExpress.Xpo.Metadata.XPClassInfo invoicesClass = null;
            invoicesClass = session.GetClassInfo(typeof(Invoice)
                );
            criteria = CriteriaOperator.Parse("[InvoiceType] = 'Cash' And [Status] In ('Paid', 'PartiallyPaid')");
            DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
            sortProps = new SortingCollection(null);
            patcher = new DevExpress.Xpo.Generators.
            CollectionCriteriaPatcher(false, session.TypesManager);
            invoices = session.GetObjects(invoicesClass, criteria,
            sortProps, 0, false, true);
            if (invoices.Count==0)
            {
                throw new UserFriendlyException("There are no invoices to process");
            }
            _FrmProgress = new ProgressForm("Generating data...", invoices.Count, 
            "Invoices processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(invoices);
            _FrmProgress.ShowDialog();

        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) 
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            ICollection _invoices = (ICollection)e.Argument;
            CashSale _cs;
            int aCount = _invoices.Count;
            try {
                foreach (Invoice item in _invoices)
                {
                    Invoice thisInvoice = session.GetObjectByKey<Invoice>(item.Oid);
                    _cs = ReflectionHelper.CreateObject<CashSale
                            >(session);
                    //private GenJournalHeader _GenJournalID;
                    _cs.GenJournalID = thisInvoice;
                    //private DateTime _Date;
                    _cs.Date = thisInvoice.ExactEntryDate;
                    //private Customer _Customer;
                    _cs.Customer = thisInvoice.Customer;
                    //private string _SourceDesc;
                    _cs.SourceDesc = thisInvoice.SourceType.Description;
                    //private string _SourceNo;
                    _cs.SourceNo = thisInvoice.SourceNo;
                    //private string _CINo;
                    _cs.CINo = thisInvoice.ReferenceNo;
                    //private decimal _CashAmount;
                    //private string _BankName;
                    //private string _CheckNo;
                    //private decimal _CheckAmount;
                    //private decimal _Total;
                    _cs.Save();

                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
            }
            finally
            {
                if (index == aCount)
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
                    "Generation of initial cash sales data is cancelled.", "Cancelled",
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
                    "Generation of initial cash sales data has been successfully generated.");
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
}
