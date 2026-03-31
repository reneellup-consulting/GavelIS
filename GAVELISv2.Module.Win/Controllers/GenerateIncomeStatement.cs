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
    public partial class GenerateIncomeStatement : ViewController
    {
        private SimpleAction generateIncomeStatement;
        private IncomeStatementHeader _IncomeStatement;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateIncomeStatement()
        {
            this.TargetObjectType = typeof(IncomeStatementHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "IncomeStatementHeader.GenerateIncomeStatement";
            this.generateIncomeStatement = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateIncomeStatement.Execute += new
            SimpleActionExecuteEventHandler(GenerateIncomeStatement_Execute);
        }
        private void GenerateIncomeStatement_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _IncomeStatement = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as IncomeStatementHeader;

            try
            {
                for (int i = _IncomeStatement.IncomeStatementDetails.Count - 1;
                i >= 0; i--)
                {
                    _IncomeStatement.IncomeStatementDetails[i].Delete(
                        );
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();

            // Filter Income Statements from Account Types and sort by Sequence
            DevExpress.Data.Filtering.CriteriaOperator criteria;
            DevExpress.Xpo.SortingCollection sortProps;
            DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
            ICollection accountTypes;
            DevExpress.Xpo.Metadata.XPClassInfo accountTypesClass;
            accountTypesClass = _IncomeStatement.Session.GetClassInfo(typeof(
            AccountType));
            //[Balance Or Income] = 'Income statement'
            criteria = CriteriaOperator.Parse("[BalanceOrIncome] = 1");
            sortProps = new SortingCollection(null);
            sortProps.Add(new SortProperty("[StatementSeq]", DevExpress.Xpo.DB.
            SortingDirection.Ascending));
            patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            false, _IncomeStatement.Session.TypesManager);
            accountTypes = _IncomeStatement.Session.GetObjects(accountTypesClass
            , criteria, sortProps, 0, false, true);

            var count = accountTypes.Count;
            _FrmProgress = new ProgressForm("Generating data...", count,
            "Income Statement accounts processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_IncomeStatement);
            _FrmProgress.ShowDialog();

            //=============================== Should End ================================

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
            IncomeStatementHeader _incomeStatement = (IncomeStatementHeader)e.Argument;
            IncomeStatementHeader thisIS = session.GetObjectByKey<IncomeStatementHeader>(
            _incomeStatement.Oid);
            int aCount = 0;

            try
            {
                // Filter Income Statements from Account Types and sort by Sequence
                DevExpress.Data.Filtering.CriteriaOperator criteria;
                DevExpress.Xpo.SortingCollection sortProps;
                DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
                ICollection accountTypes;
                DevExpress.Xpo.Metadata.XPClassInfo accountTypesClass;
                accountTypesClass = thisIS.Session.GetClassInfo(typeof(
                AccountType));
                //[Balance Or Income] = 'Income statement'
                criteria = CriteriaOperator.Parse("[BalanceOrIncome] = 1");
                sortProps = new SortingCollection(null);
                sortProps.Add(new SortProperty("[StatementSeq]", DevExpress.Xpo.DB.
                SortingDirection.Ascending));
                patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
                false, thisIS.Session.TypesManager);
                accountTypes = thisIS.Session.GetObjects(accountTypesClass
                , criteria, sortProps, 0, false, true);
                aCount = accountTypes.Count;
                int iid = 0;
                decimal subTotal0 = 0;
                foreach (AccountType item in accountTypes)
                {
                    _message = "Processing " + item.Description;
                    // Query GL Accounts filtered by Account Type=item sorted by account no
                    ICollection glAccounts;
                    DevExpress.Xpo.Metadata.XPClassInfo glAccountsClass;
                    glAccountsClass = thisIS.Session.GetClassInfo(typeof(
                    Account));
                    criteria = CriteriaOperator.Parse("[AccountType.Code] = '" +
                    item.Code + "'");
                    sortProps = new SortingCollection(null);
                    sortProps.Add(new SortProperty("[No]", DevExpress.Xpo.DB.
                    SortingDirection.Ascending));
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.
                    TypesManager);
                    glAccounts = thisIS.Session.GetObjects(glAccountsClass
                    , criteria, sortProps, 0, false, true);
                    Account headAccount = null;
                    Account prevAccount = null;
                    Account prevAccount1 = null;
                    Account prevAccount2 = null;
                    Account prevAccount3 = null;
                    Account prevAccount4 = null;
                    Account prevAccount5 = null;
                    IncomeStatementDetail headID = null;
                    IncomeStatementDetail prevHeadID = null;
                    IncomeStatementDetail prevHeadID1 = null;
                    IncomeStatementDetail prevHeadID2 = null;
                    IncomeStatementDetail prevHeadID3 = null;
                    IncomeStatementDetail prevHeadID4 = null;
                    IncomeStatementDetail prevHeadID5 = null;
                    decimal postTotal = 0;
                    decimal endTotalTotal0 = 0;
                    decimal endTotalTotal1 = 0;
                    decimal endTotalTotal2 = 0;
                    decimal endTotalTotal3 = 0;
                    decimal endTotalTotal4 = 0;
                    decimal endTotalTotal5 = 0;
                    decimal endTotalTotal6 = 0;
                    int headNo = 0;
                    // Fill Income Statement Detail
                    bool tiradPass = false;
                    foreach (Account account in glAccounts)
                    {
                        if (account.No == "400900")
                        {

                        }
                        iid++;
                        IncomeStatementDetail isDetail = new IncomeStatementDetail(
                        thisIS.Session);
                        isDetail.IncomeStatementHeadID = thisIS;
                        isDetail.ID = iid;
                        if (account.ParentAccount == null)
                        {
                            headAccount = account;
                            headID = isDetail;

                            if (account.AccountType.Description == "Provision for Income Tax")
                            {
                                isDetail.LineType = "Post";
                            }
                            else
                            {
                                isDetail.LineType = "Head";
                                if (account.GeneralType.Description == "Totalling")
                                {
                                    isDetail.LineType = "Total";
                                    isDetail.Amount = endTotalTotal0;
                                    if (account.IsHeadTotal)
                                    {
                                        isDetail.Amount = subTotal0;
                                        //subTotal0=0;
                                    }
                                    else
                                    {
                                        if (account.CashOpe != CashOpeEnum.Less)
                                        {
                                            subTotal0 += isDetail.Amount;
                                        }
                                        else
                                        {
                                            subTotal0 -= isDetail.Amount;
                                        }
                                    }

                                    endTotalTotal0 = 0;

                                }
                            }
                        }
                        if (account.ParentAccount != null)
                        {
                            if (account.ChildAccounts.Count > 0)
                            {
                                isDetail.LineType
                                    = "Begin-Total";
                                postTotal = 0;
                            }
                            else
                            {
                                if (account.GeneralType.Description == "Totalling")
                                {
                                    isDetail.LineType = "End-Total";
                                }
                                else
                                {
                                    isDetail.LineType = "Post";
                                }
                            }
                            if (account.ParentAccount == headAccount)
                            {
  
                                isDetail.ParentID = headID;
                                prevAccount = account;
                                prevHeadID = isDetail;
                                headNo=0;
                                if (account.GeneralType.Description == "Totalling")
                                {
                                    // Where Net Purchases Pass
                                    if (endTotalTotal1 == 0 && tiradPass)
                                    {
                                        isDetail.Amount = endTotalTotal0;
                                        tiradPass=false;
                                    }
                                    else
                                    {
                                        isDetail.Amount = endTotalTotal1;
                                        endTotalTotal0 += isDetail.Amount;
                                    }
                                    endTotalTotal1 = 0;
                                }
                            }
                            if (account.ParentAccount != headAccount)
                            {
                                if (account.ParentAccount == prevAccount)
                                {
                                    isDetail.ParentID = prevHeadID;
                                    prevAccount1 = account;
                                    prevHeadID1 = isDetail;
                                    headNo = 1;
                                    if (account.GeneralType.Description == "Totalling")
                                    {
                                        isDetail.Amount=endTotalTotal2;
                                        endTotalTotal2 = 0;
                                        endTotalTotal1 +=isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount1)
                                {
                                    isDetail.ParentID = prevHeadID1;
                                    prevAccount2 = account;
                                    prevHeadID2 = isDetail;
                                    headNo = 2;
                                    if (account.GeneralType.Description == "Totalling")
                                    {
                                        isDetail.Amount = endTotalTotal3;
                                        endTotalTotal3 = 0;
                                        endTotalTotal2 += isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount2)
                                {
                                    isDetail.ParentID = prevHeadID2;
                                    prevAccount3 = account;
                                    prevHeadID3 = isDetail;
                                    headNo = 3;
                                    if (account.GeneralType.Description == "Totalling")
                                    {
                                        isDetail.Amount = endTotalTotal4;
                                        endTotalTotal4 = 0;
                                        endTotalTotal3 += isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount3)
                                {
                                    isDetail.ParentID = prevHeadID3;
                                    prevAccount4 = account;
                                    prevHeadID4 = isDetail;
                                    headNo = 4;
                                    if (account.GeneralType.Description == "Totalling")
                                    {
                                        isDetail.Amount = endTotalTotal5;
                                        endTotalTotal5 = 0;
                                        endTotalTotal4 += isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount4)
                                {
                                    isDetail.ParentID = prevHeadID4;
                                    prevAccount5 = account;
                                    prevHeadID5 = isDetail;
                                    headNo = 5;
                                    if (account.GeneralType.Description == "Totalling")
                                    {
                                        isDetail.Amount = endTotalTotal6;
                                        endTotalTotal6 = 0;
                                        endTotalTotal5 += isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount5)
                                {
                                    isDetail
                                        .ParentID = prevHeadID5;
                                }
                            }
                        }
                        isDetail.GLAccount = account;
                        isDetail.LabelCaption = account.Name;
                        if (account.ChildAccounts.Count == 0)
                        {
                            ICollection entries;
                            DevExpress.Xpo.Metadata.XPClassInfo entriesClass;
                            entriesClass = thisIS.Session.GetClassInfo(
                            typeof(GenJournalDetail));
                            //DateTime fDate=thisIS.FromDate.Subtract(new TimeSpan(24,0,0));
                            //DateTime tDate = thisIS.ToDate.AddDays(1);
                            //criteria = CriteriaOperator.Parse("[Account.No] = " +
                            //account.No + "And [GenJournalID.EntryDate] Between(#" + 
                            //fDate + "#, #" + tDate + "#)");
                            //IsOutlookIntervalPriorThisYear([Gen Journal ID.Entry Date])
                            // Date range criteria start
                            DateTime fDate;
                            DateTime tDate;
                            string critString=string.Empty;
                            switch (thisIS.DateRange)
                            {
                                case TextDateRangeType.YearToDate:
                                    fDate=new DateTime(DateTime.Today.Year,01,01,01,0,0);
                                    tDate = new DateTime(DateTime.Today.Year,DateTime.Today.Month,DateTime.Today.Day,23,59,59);
                                    critString = "[GenJournalID.EntryDate] >= #"+ fDate +"# And [GenJournalID.EntryDate] <= #"+ tDate +"#";
                                    break;
                                case TextDateRangeType.PreviousYear:
                                    fDate = new DateTime(DateTime.Today.Year-1, 01, 01,0,0,0);
                                    tDate = new DateTime(DateTime.Today.Year - 1, 12, 31,23,59,59);
                                    critString = "[GenJournalID.EntryDate] >= #"+ fDate +"# And [GenJournalID.EntryDate] <= #"+ tDate +"#";
                                    break;
                                case TextDateRangeType.MonthToDate:
                                    fDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01, 01, 0, 0);
                                    tDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 59);
                                    critString = "[GenJournalID.EntryDate] >= #"+ fDate +"# And [GenJournalID.EntryDate] <= #"+ tDate +"#";
                                    break;
                                case TextDateRangeType.PreviousMonth:
                                    fDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, 01, 01, 0, 0);
                                    tDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month - 1, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month - 1), 23, 59, 59);
                                    critString = "[GenJournalID.EntryDate] >= #"+ fDate +"# And [GenJournalID.EntryDate] <= #"+ tDate +"#";
                                    break;
                                case TextDateRangeType.Custom:
                                    fDate = new DateTime(thisIS.FromDate.Year,thisIS.FromDate.Month,thisIS.FromDate.Day,01,0,0);
                                    tDate = new DateTime(thisIS.ToDate.Year,thisIS.ToDate.Month,thisIS.ToDate.Day,23,59,59);;
                                    critString = "[GenJournalID.EntryDate] >= #"+ fDate +"# And [GenJournalID.EntryDate] <= #"+ tDate +"#";
                                    break;
                                default:
                                    break;
                            }
                            // Date range criteria end
                            criteria = CriteriaOperator.Parse("[Account.No] = " +
                            account.No + "And " + critString + "And [Approved] = True");

                            sortProps = new SortingCollection(null);
                            sortProps.Add(new SortProperty("[GenJournalID.EntryDate]", DevExpress.Xpo.DB
                            .SortingDirection.Ascending));
                            patcher = new DevExpress.Xpo.Generators.
                            CollectionCriteriaPatcher(false, thisIS.
                            Session.TypesManager);
                            entries = thisIS.Session.GetObjects(
                            entriesClass, criteria, sortProps, 0, false, true);

                            decimal amt = 0;
                            decimal tmpAmt = 0;
                            foreach (GenJournalDetail entry in entries)
                            {
                                if (entry.GenJournalID.Oid == 1141)
                                {
                                }
                                amt = entry.DebitAmount - entry.CreditAmount;
                                tmpAmt += amt;
                            }
                            if (item.Description == "Sales" || item.Description =="Sales Discount")
                            {
                                isDetail.Amount+=Math.Abs(tmpAmt);
                            }
                            else
                            {
                                isDetail.Amount += tmpAmt;
                            }
                            
                        }
                        isDetail.Less = item.CashOpe == CashOpeEnum.Less ? true :
                        false;
                        if (isDetail.LineType=="End-Total")
                        {
                            postTotal = 0;
                        }
                        if (isDetail.LineType=="Post")
                        {
                            postTotal += isDetail.Amount;
                            if (headNo == 0)
                            {
                                if (account.CashOpe==CashOpeEnum.Less)
                                {
                                    endTotalTotal0 -= Math.Abs(isDetail.Amount);
                                    tiradPass=true;
                                }
                                else { endTotalTotal0 += isDetail.Amount; }
                                
                            }
                            if (headNo==1)
                            {
                                if (account.CashOpe == CashOpeEnum.Less)
                                {
                                    endTotalTotal1 -= Math.Abs(isDetail.Amount);
                                    tiradPass = true;
                                }
                                else { endTotalTotal1 += isDetail.Amount; }
                            }
                            if (headNo == 2)
                            {
                                if (account.CashOpe == CashOpeEnum.Less)
                                {
                                    endTotalTotal2 -= Math.Abs(isDetail.Amount);
                                    tiradPass = true;
                                }
                                else { endTotalTotal2 += isDetail.Amount; }
                            }
                            if (headNo == 3)
                            {
                                if (account.CashOpe == CashOpeEnum.Less)
                                {
                                    endTotalTotal3 -= Math.Abs(isDetail.Amount);
                                    tiradPass = true;
                                }
                                else { endTotalTotal3 += isDetail.Amount; }
                            }
                            if (headNo == 4)
                            {
                                if (account.CashOpe == CashOpeEnum.Less)
                                {
                                    endTotalTotal4 -= Math.Abs(isDetail.Amount);
                                    tiradPass = true;
                                }
                                else { endTotalTotal4 += isDetail.Amount; }
                            }
                            if (headNo == 5)
                            {
                                if (account.CashOpe == CashOpeEnum.Less)
                                {
                                    endTotalTotal5 -= Math.Abs(isDetail.Amount);
                                    tiradPass = true;
                                }
                                else { endTotalTotal5 += isDetail.Amount; }
                            }
                            if (headNo == 6)
                            {
                                if (account.CashOpe == CashOpeEnum.Less)
                                {
                                    endTotalTotal6 -= Math.Abs(isDetail.Amount);
                                    tiradPass = true;
                                }
                                else { endTotalTotal6 += isDetail.Amount; }
                            }
                        }
                        isDetail.Save();
                    }
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
                    "Generation of Income Statement data is cancelled.", "Cancelled",
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
                    "Income Statement data has been successfully generated.");
                    ObjectSpace.ReloadObject(_IncomeStatement);
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
