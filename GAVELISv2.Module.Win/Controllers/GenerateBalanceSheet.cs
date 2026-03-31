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
namespace GAVELISv2.Module.Win.Controllers {
    public partial class GenerateBalanceSheet : ViewController {
        private SimpleAction generateBalanceSheet;
        private BalanceSheetHeader _BalanceSheet;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateBalanceSheet() {
            this.TargetObjectType = typeof(BalanceSheetHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "BalanceSheetHeader.GenerateBalanceSheet()";
            this.generateBalanceSheet = new SimpleAction(this, actionID, 
            PredefinedCategory.RecordEdit);
            this.generateBalanceSheet.Execute += new 
            SimpleActionExecuteEventHandler(GenerateBalanceSheet_Execute);
        }
        private void GenerateBalanceSheet_Execute(object sender, 
        SimpleActionExecuteEventArgs e) {
            _BalanceSheet = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as BalanceSheetHeader;
            try {
                for (int i = _BalanceSheet.BalanceSheetDetails.Count - 1; i >= 0
                ; i--) {_BalanceSheet.BalanceSheetDetails[i].Delete();}
            } catch (Exception) {
            }
            ObjectSpace.CommitChanges();
            // Filter Balance Sheet from Account Types and sort by Sequence
            //DevExpress.Data.Filtering.CriteriaOperator criteria;
            //DevExpress.Xpo.SortingCollection sortProps;
            //DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
            //ICollection accountTypes;
            //DevExpress.Xpo.Metadata.XPClassInfo accountTypesClass;
            //accountTypesClass = _BalanceSheet.Session.GetClassInfo(typeof(
            //AccountType));
            //criteria = CriteriaOperator.Parse("[BalanceOrIncome] = 0");
            //sortProps = new SortingCollection(null);
            //sortProps.Add(new SortProperty("[StatementSeq]", DevExpress.Xpo.DB.
            //SortingDirection.Ascending));
            //patcher = new DevExpress.Xpo.Generators.CollectionCriteriaPatcher(
            //false, _BalanceSheet.Session.TypesManager);
            //accountTypes = _BalanceSheet.Session.GetObjects(accountTypesClass, 
            //criteria, sortProps, 0, false, true);
            var count = 2;
            _FrmProgress = new ProgressForm("Generating data...", count, 
            "Balance Sheet accounts processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker { 
                WorkerSupportsCancellation = true, WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(_BalanceSheet);
            _FrmProgress.ShowDialog();
            //=============================== Should End ================================        }
        }
        private UnitOfWork CreateUpdatingSession() {
            UnitOfWork session = new UnitOfWork(((ObjectSpace)ObjectSpace).
            Session.ObjectLayer);
            OnUpdatingSessionCreated(session);
            return session;
        }
        private void CommitUpdatingSession(UnitOfWork session) {
            session.CommitChanges();
            OnUpdatingSessionCommitted(session);
        }
        protected virtual void OnUpdatingSessionCommitted(UnitOfWork session) { 
            if (UpdatingSessionCommitted != null) {UpdatingSessionCommitted(this
                , new SessionEventArgs(session));} }
        protected virtual void OnUpdatingSessionCreated(UnitOfWork session) { if 
            (UpdatingSessionCreated != null) {UpdatingSessionCreated(this, new 
                SessionEventArgs(session));} }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            decimal netIncome=0;
            UnitOfWork session = CreateUpdatingSession();
            BalanceSheetHeader _balanceSheet = (BalanceSheetHeader)e.Argument;
            BalanceSheetHeader thisIS = session.GetObjectByKey<
            BalanceSheetHeader>(_balanceSheet.Oid);
            foreach (IncomeStatementDetail item in thisIS.IncomeStatement.IncomeStatementDetails)
            {
                if (item.LabelCaption == "NET INCOME AFTER INCOME TAX")
                {
                    netIncome=item.Amount;
                }
            }
            int aCount = 2;
            BalanceSectionEnum bSection;
            try {
                int iid = 0;
                decimal subTotal0 = 0;

                for (int i = 0; i < 2; i++)
                {
                    if (i==0)
                    {
                        bSection=BalanceSectionEnum.Asset;
                    }
                    else
                    {
                        bSection=BalanceSectionEnum.LiabilitiesAndEquity;
                    }

                    _message = "Processing " + bSection.ToString();
                    // Query GL Accounts filtered by Account Type=item sorted by account no
                    DevExpress.Data.Filtering.CriteriaOperator criteria;
                    DevExpress.Xpo.SortingCollection sortProps;
                    DevExpress.Xpo.Generators.CollectionCriteriaPatcher patcher;
                    ICollection glAccounts;
                    DevExpress.Xpo.Metadata.XPClassInfo glAccountsClass;
                    glAccountsClass = thisIS.Session.GetClassInfo(typeof(Account
                    ));
                    criteria = CriteriaOperator.Parse("[AccountType.Section] = '"+ bSection.ToString() +"'");
                    sortProps = new SortingCollection(null);
                    sortProps.Add(new SortProperty("[No]", DevExpress.Xpo.DB.
                    SortingDirection.Ascending));
                    patcher = new DevExpress.Xpo.Generators.
                    CollectionCriteriaPatcher(false, thisIS.Session.TypesManager
                    );
                    glAccounts = thisIS.Session.GetObjects(glAccountsClass,
                    criteria, sortProps, 0, false, true);
                    Account headAccount = null;
                    Account prevAccount = null;
                    Account prevAccount1 = null;
                    Account prevAccount2 = null;
                    Account prevAccount3 = null;
                    Account prevAccount4 = null;
                    Account prevAccount5 = null;
                    BalanceSheetDetail headID = null;
                    BalanceSheetDetail prevHeadID = null;
                    BalanceSheetDetail prevHeadID1 = null;
                    BalanceSheetDetail prevHeadID2 = null;
                    BalanceSheetDetail prevHeadID3 = null;
                    BalanceSheetDetail prevHeadID4 = null;
                    BalanceSheetDetail prevHeadID5 = null;
                    decimal postTotal = 0;
                    decimal endTotalTotal0 = 0;
                    decimal endTotalTotal1 = 0;
                    decimal endTotalTotal2 = 0;
                    decimal endTotalTotal3 = 0;
                    decimal endTotalTotal4 = 0;
                    decimal endTotalTotal5 = 0;
                    decimal endTotalTotal6 = 0;
                    int headNo = 0;
                    // Fill Balance Sheet Detail
                    bool tiradPass = false;
                    foreach (Account account in glAccounts)
                    {
                        if (account.No == "104220")
                        {
                        }
                        iid++;
                        BalanceSheetDetail isDetail = new BalanceSheetDetail(
                        thisIS.Session);
                        isDetail.BalanceSheetHeadID = thisIS;
                        isDetail.ID = iid;
                        if (account.ParentAccount == null)
                        {
                            headAccount = account;
                            headID = isDetail;
                            if (account.AccountType.Description ==
                            "Provision for Income Tax")
                            {
                                isDetail.LineType =
                                "Post";
                            }
                            else
                            {
                                isDetail.LineType = "Head";
                                if (account.GeneralType.Description ==
                                "Totalling")
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
                                        subTotal0 += isDetail.Amount;
                                        //if (account.CashOpe != CashOpeEnum.Less)
                                        //{
                                        //    subTotal0 += isDetail.Amount;
                                        //}
                                        //else
                                        //{
                                        //    subTotal0 -= isDetail.Amount;
                                        //}
                                    }
                                    endTotalTotal0 = 0;
                                }
                            }
                        }
                        if (account.ParentAccount != null)
                        {
                            if (account.ChildAccounts.Count > 0)
                            {
                                isDetail.LineType = "Begin-Total";
                                postTotal = 0;
                            }
                            else
                            {
                                if (account.GeneralType.Description ==
                                "Totalling") { isDetail.LineType = "End-Total"; }
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
                                headNo = 0;
                                if (account.GeneralType.Description ==
                                "Totalling")
                                {
                                    // Where Net Purchases Pass
                                    if (endTotalTotal1 == 0 && tiradPass)
                                    {
                                        isDetail.Amount = endTotalTotal0;
                                        tiradPass = false;
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
                                    if (account.GeneralType.Description ==
                                    "Totalling")
                                    {
                                        isDetail.Amount = endTotalTotal2;
                                        endTotalTotal2 = 0;
                                        endTotalTotal1 += isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount1)
                                {
                                    isDetail.ParentID = prevHeadID1;
                                    prevAccount2 = account;
                                    prevHeadID2 = isDetail;
                                    headNo = 2;
                                    if (account.GeneralType.Description ==
                                    "Totalling")
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
                                    if (account.GeneralType.Description ==
                                    "Totalling")
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
                                    if (account.GeneralType.Description ==
                                    "Totalling")
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
                                    if (account.GeneralType.Description ==
                                    "Totalling")
                                    {
                                        isDetail.Amount = endTotalTotal6;
                                        endTotalTotal6 = 0;
                                        endTotalTotal5 += isDetail.Amount;
                                    }
                                }
                                if (account.ParentAccount == prevAccount5)
                                {
                                    isDetail.ParentID = prevHeadID5;
                                }
                            }
                        }
                        isDetail.GLAccount = account;
                        isDetail.LabelCaption = account.Name;
                        if (account.ChildAccounts.Count == 0)
                        {
                            ICollection entries;
                            DevExpress.Xpo.Metadata.XPClassInfo entriesClass;
                            entriesClass = thisIS.Session.GetClassInfo(typeof(
                            GenJournalDetail));
                            //DateTime fDate=thisIS.FromDate.Subtract(new TimeSpan(24,0,0));
                            //DateTime tDate = thisIS.ToDate.AddDays(1);
                            //criteria = CriteriaOperator.Parse("[Account.No] = " +
                            //account.No + "And [GenJournalID.EntryDate] Between(#" + 
                            //fDate + "#, #" + tDate + "#)");
                            //IsOutlookIntervalPriorThisYear([Gen Journal ID.Entry Date])
                            // Date range criteria start
                            DateTime fDate;
                            DateTime tDate;
                            string critString = string.Empty;
                            switch (thisIS.IncomeStatement.DateRange)
                            {
                                case TextDateRangeType.YearToDate:
                                    fDate = new DateTime(DateTime.Today.Year, 01
                                    , 01, 01, 0, 0);
                                    tDate = new DateTime(DateTime.Today.Year,
                                    DateTime.Today.Month, DateTime.Today.Day, 23
                                    , 59, 59);
                                    critString = "[GenJournalID.EntryDate] >= #"
                                    + fDate +
                                    "# And [GenJournalID.EntryDate] <= #" +
                                    tDate + "#";
                                    break;
                                case TextDateRangeType.PreviousYear:
                                    fDate = new DateTime(DateTime.Today.Year - 1
                                    , 01, 01, 0, 0, 0);
                                    tDate = new DateTime(DateTime.Today.Year - 1
                                    , 12, 31, 23, 59, 59);
                                    critString = "[GenJournalID.EntryDate] >= #"
                                    + fDate +
                                    "# And [GenJournalID.EntryDate] <= #" +
                                    tDate + "#";
                                    break;
                                case TextDateRangeType.MonthToDate:
                                    fDate = new DateTime(DateTime.Today.Year,
                                    DateTime.Today.Month, 01, 01, 0, 0);
                                    tDate = new DateTime(DateTime.Today.Year,
                                    DateTime.Today.Month, DateTime.Today.Day, 23
                                    , 59, 59);
                                    critString = "[GenJournalID.EntryDate] >= #"
                                    + fDate +
                                    "# And [GenJournalID.EntryDate] <= #" +
                                    tDate + "#";
                                    break;
                                case TextDateRangeType.PreviousMonth:
                                    fDate = new DateTime(DateTime.Today.Year,
                                    DateTime.Today.Month - 1, 01, 01, 0, 0);
                                    tDate = new DateTime(DateTime.Today.Year,
                                    DateTime.Today.Month - 1, DateTime.
                                    DaysInMonth(DateTime.Today.Year, DateTime.
                                    Today.Month - 1), 23, 59, 59);
                                    critString = "[GenJournalID.EntryDate] >= #"
                                    + fDate +
                                    "# And [GenJournalID.EntryDate] <= #" +
                                    tDate + "#";
                                    break;
                                case TextDateRangeType.Custom:
                                    fDate = new DateTime(thisIS.IncomeStatement.
                                    FromDate.Year, thisIS.IncomeStatement.
                                    FromDate.Month, thisIS.IncomeStatement.
                                    FromDate.Day, 01, 0, 0);
                                    tDate = new DateTime(thisIS.IncomeStatement.
                                    ToDate.Year, thisIS.IncomeStatement.ToDate.
                                    Month, thisIS.IncomeStatement.ToDate.Day, 23
                                    , 59, 59);
                                    ;
                                    critString = "[GenJournalID.EntryDate] >= #"
                                    + fDate +
                                    "# And [GenJournalID.EntryDate] <= #" +
                                    tDate + "#";
                                    break;
                                default:
                                    break;
                            }
                            // Date range criteria end
                            criteria = CriteriaOperator.Parse("[Account.No] = "
                            + account.No + "And " + critString +
                            "And [Approved] = True");
                            sortProps = new SortingCollection(null);
                            sortProps.Add(new SortProperty(
                            "[GenJournalID.EntryDate]", DevExpress.Xpo.DB.
                            SortingDirection.Ascending));
                            patcher = new DevExpress.Xpo.Generators.
                            CollectionCriteriaPatcher(false, thisIS.Session.
                            TypesManager);
                            entries = thisIS.Session.GetObjects(entriesClass,
                            criteria, sortProps, 0, false, true);
                            decimal amt = 0;
                            decimal tmpAmt = 0;
                            foreach (GenJournalDetail entry in entries)
                            {
                                if (entry.GenJournalID.Oid == 1141) { }
                                amt = entry.DebitAmount - entry.CreditAmount;
                                tmpAmt += amt;
                            }
                            if (account.Name == "Net Income (Loss)")
                            {
                                isDetail.Amount = netIncome;
                            }
                            else
                            {
                                //isDetail.Amount += Math.Abs(tmpAmt);
                                if (bSection!=BalanceSectionEnum.Asset)
                                {
                                    if (tmpAmt>0)
                                    {
                                        isDetail.Amount += (0 - tmpAmt);
                                    }
                                    else
                                    {
                                        isDetail.Amount += Math.Abs(tmpAmt);
                                    }
                                }
                                else
                                {
                                    isDetail.Amount += tmpAmt;
                                }
                                
                            }
                        }
                        //isDetail.Less = item.CashOpe == CashOpeEnum.Less ? true
                        //: false;
                        if (isDetail.LineType == "End-Total") { postTotal = 0; }
                        if (isDetail.LineType == "Post")
                        {
                            postTotal += isDetail.Amount;
                            if (headNo == 0)
                            {
                                endTotalTotal0 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal0 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal0 += isDetail.Amount; }
                            }
                            if (headNo == 1)
                            {
                                endTotalTotal1 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal1 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal1 += isDetail.Amount; }
                            }
                            if (headNo == 2)
                            {
                                endTotalTotal2 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal2 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal2 += isDetail.Amount; }
                            }
                            if (headNo == 3)
                            {
                                endTotalTotal3 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal3 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal3 += isDetail.Amount; }
                            }
                            if (headNo == 4)
                            {
                                endTotalTotal4 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal4 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal4 += isDetail.Amount; }
                            }
                            if (headNo == 5)
                            {
                                endTotalTotal5 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal5 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal5 += isDetail.Amount; }
                            }
                            if (headNo == 6)
                            {
                                endTotalTotal6 += isDetail.Amount;
                                //if (account.CashOpe == CashOpeEnum.Less)
                                //{
                                //    endTotalTotal6 -= isDetail.Amount;
                                //    tiradPass = true;
                                //}
                                //else { endTotalTotal6 += isDetail.Amount; }
                            }
                        }
                        isDetail.Save();
                    }
                    System.Threading.Thread.Sleep(20);
                    _BgWorker.ReportProgress(1, _message);
                    index++;
                }
                    
            } finally {
                if (index == aCount) {CommitUpdatingSession(session);}
                session.Dispose();
            }
        }
        private void BgWorkerProgressChanged(object sender, 
        ProgressChangedEventArgs e) { if (_FrmProgress != null) {_FrmProgress.
                DoProgress(e.ProgressPercentage);} }
        private void BgWorkerRunWorkerCompleted(object sender, 
        RunWorkerCompletedEventArgs e) {
            _FrmProgress.Close();
            if (e.Cancelled) {XtraMessageBox.Show(
                "Generation of Balance Sheet data is cancelled.", "Cancelled", 
                System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.
                MessageBoxIcon.Exclamation);} else {
                if (e.Error != null) {XtraMessageBox.Show(e.Error.Message, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.
                    Windows.Forms.MessageBoxIcon.Error);} else {
                    XtraMessageBox.Show(
                    "Balance Sheet data has been successfully generated.");
                    ObjectSpace.ReloadObject(_BalanceSheet);
                    ObjectSpace.Refresh();
                }
            }
        }
        private void FrmProgressCancelClick(object sender, EventArgs e) { 
            _BgWorker.CancelAsync(); }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
