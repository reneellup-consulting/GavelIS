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
using DevExpress.Xpo.DB;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class PurchMovementGeneratorController : ViewController
    {
        private PurchasesMovementAnalysis reporter;
        private SimpleAction purchMovementGeneratorAction;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public PurchMovementGeneratorController()
        {
            this.TargetObjectType = typeof(PurchasesMovementAnalysis);
            this.TargetViewType = ViewType.DetailView;
            string actionID = string.Format("{0}.PurchMovementGeneratorAction", this.GetType().
            Name);
            this.purchMovementGeneratorAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.purchMovementGeneratorAction.Caption = "Generate";
            this.purchMovementGeneratorAction.Execute += new
            SimpleActionExecuteEventHandler(purchMovementGenerator_Execute);
            this.purchMovementGeneratorAction.Executed += new EventHandler<
            ActionBaseEventArgs>(purchMovementGenerator_Executed);
            UpdateActionState(false);
        }
        private void purchMovementGenerator_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            reporter = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as PurchasesMovementAnalysis;
            // Delete PartsUsageIcjDetails
            ObjectSpace.CommitChanges();
            if (reporter.PurchasesMovementBuffDetails.Count > 0)
            {
                throw new UserFriendlyException("Cannot generate, details must not exist.");
            }
            XPCollection<Item> items = new XPCollection<Item>(((ObjectSpace)ObjectSpace).Session);
            var filter = new[] { ItemTypeEnum.InventoryItem, ItemTypeEnum.FuelItem, ItemTypeEnum.RepairItem, ItemTypeEnum.TireItem };
            
            IEnumerable<Item> included = null;
            if (reporter.TestItem != null)
            {
                included = items.Where(o => filter.Contains(o.ItemType) && o.Oid == reporter.TestItem.Oid);
            }
            else
            {
                included = items.Where(o => filter.Contains(o.ItemType));
            }
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

        private void purchMovementGenerator_Executed(object sender,
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
            PurchasesMovementAnalysis oReporter = session.GetObjectByKey<PurchasesMovementAnalysis>(reporter.Oid);
            try
            {
                DateTime baseDate = DateTime.Now;
                DateTime endDate = DateTime.Now; ;
                switch (oReporter.ReportType)
                {
                    case PmaReportypeEnum.Monthly:
                        baseDate = new DateTime(oReporter.Year, (int)oReporter.Month, 1);
                        endDate = (new DateTime(oReporter.Year, (int)oReporter.Month, DateTime.DaysInMonth(oReporter.Year, (int)oReporter.Month))).AddDays(1);
                        break;
                    case PmaReportypeEnum.Asof:
                        baseDate = oReporter.StartDate;
                        endDate = oReporter.EndDate.AddDays(1);
                        break;
                    default:
                        break;
                }

                foreach (Item item in trans)
                {
                    index++;

                    #region Algorithm here...

                    // Initialize Per Item Summary
                    PurchasesMovementSummary pmovsum = ReflectionHelper.CreateObject<PurchasesMovementSummary>(session);
                    pmovsum.HeaderId = oReporter;
                    pmovsum.ItemNo = session.GetObjectByKey<Item>(item.Oid);


                    DateTime dtYrBefore = new DateTime(oReporter.Year, 1, 1);
                    SelectedData whse = null;
                    if (oReporter.TestWarehouse != null)
                    {
                        string whseguid = oReporter.TestWarehouse.Oid.ToString().ToUpper();
                        whse = session.ExecuteQuery(string.Format("select * from warehouse where Oid='{0}'", whseguid));
                    }
                    else
                    {
                        whse = session.ExecuteQuery("select * from warehouse");
                    }
                    
                    foreach (var dwhse in whse.ResultSet[0].Rows)
                    {
                        if (dwhse.Values[0].ToString() == ("6458D569-6BBD-433D-A4FD-0D0491A64343").ToLower())
                        {
                            
                        }
                        decimal wrqty = 0m;
                        // Get before this year begining inventory
                        string qryStr = string.Format("select * from vInvControlJournals where SourceType not in " +
                         "('DF42778E-F4FD-4D0E-B0EE-77CCEC90566D','6D0E3DCB-1725-4AF5-A0E3-583B7A9472FC')" +
                         " and ItemNo='{0}' and EntryDate <= '{1}' and Warehouse = '{2}' order by EntryDate", item.Oid, dtYrBefore.ToString("yyyy-MM-dd HH:mm:ss.fff"), dwhse.Values[0].ToString());

                        bool piFound = false;
                        string piId = string.Empty;
                        string initId = string.Empty;
                        SelectStatementResultRow dicj=null;
                        var data = session.ExecuteQuery(qryStr);

                        if (data != null && data.ResultSet[0].Rows.Count() > 0)
                        {
                            for (int i = data.ResultSet[0].Rows.Count() - 1; i >= 0; i--)
                            {
                                dicj = data.ResultSet[0].Rows[i];
                                if (dicj != null)
                                {
                                    //string stype = dicj.Values[4].ToString();
                                    if (dicj.Values[4].ToString() == ("A95EBEEA-72D7-434B-95CB-A5C57668E315").ToLower())
                                    {
                                        piFound = true;
                                        piId = dicj.Values[0].ToString();
                                        break;
                                    }
                                    initId = dicj.Values[0].ToString();
                                }
                            }
                            // Get remaining balance
                            string qryBRem = string.Empty;
                            PhysicalAdjustmentDetail pidObject = null;
                            if (piFound)
                            {
                                int gid = Convert.ToInt32(dicj.Values[1].ToString());
                                Guid iit = Guid.Parse(dicj.Values[8].ToString());
                                Guid iwh = Guid.Parse(dicj.Values[11].ToString());
                                var oGenJournal = session.GetObjectByKey<GenJournalHeader>(gid);
                                var oItem = session.GetObjectByKey<Item>(iit);
                                var oWarehouse = session.GetObjectByKey<Warehouse>(iwh);
                                CriteriaOperator crit = CriteriaOperator.Parse("[GenJournalID]=? And [ItemNo]=? And [Warehouse]=?", oGenJournal, oItem, oWarehouse);
                                pidObject = session.FindObject<PhysicalAdjustmentDetail>(crit);

                                // Running anlysis from year to month or date selected
                                // Eentry date filter is between dtYrBefore and endDate
                                if (pidObject != null)
                                {
                                    qryBRem = string.Format("select * from vInvControlJournals where SourceType not in " +
                                         "('DF42778E-F4FD-4D0E-B0EE-77CCEC90566D','6D0E3DCB-1725-4AF5-A0E3-583B7A9472FC')" +
                                         " and ItemNo='{0}' and EntryDate > '{1}' and EntryDate <= '{2}' and Warehouse = '{3}' order by EntryDate", item.Oid, pidObject.GenJournalID.EntryDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), dtYrBefore.ToString("yyyy-MM-dd HH:mm:ss.fff"), dwhse.Values[0].ToString());
                                }
                            }
                            else
                            {
                                qryBRem = string.Format("select * from vInvControlJournals where SourceType not in " +
                                     "('DF42778E-F4FD-4D0E-B0EE-77CCEC90566D','6D0E3DCB-1725-4AF5-A0E3-583B7A9472FC')" +
                                     " and ItemNo='{0}' and EntryDate < '{1}' and Warehouse = '{2}' order by EntryDate", item.Oid, dtYrBefore.ToString("yyyy-MM-dd HH:mm:ss.fff"), dwhse.Values[0].ToString());
                            }

                            decimal rQty = 0m;
                            int r = 0;
                            var remQtyData = session.ExecuteQuery(qryBRem);
                            if (piFound)
                            {
                                if (remQtyData != null &&  remQtyData.ResultSet[0].Rows.Count()>0)
                                {
                                    //r = remQtyData.ResultSet[0].Rows.Count() + 1;
                                    r = remQtyData.ResultSet[0].Rows.Count();
                                }
                                else
                                {
                                    r = 0;
                                }
                                rQty = pidObject.ActualQtyBase;
                            }
                            else
                            {
                                r = remQtyData.ResultSet[0].Rows.Count();
                            }
                            for (int i = 0; i < r; i++)
                            {
                                dicj = remQtyData.ResultSet[0].Rows[i];
                                if (dicj != null)
                                {
                                    //string stype = dicj.Values[4].ToString();
                                    if (dicj.Values[4].ToString() == ("A95EBEEA-72D7-434B-95CB-A5C57668E315").ToLower())
                                    {
                                        int gid = Convert.ToInt32(dicj.Values[1].ToString());
                                        Guid iit = Guid.Parse(dicj.Values[8].ToString());
                                        Guid iwh = Guid.Parse(dicj.Values[11].ToString());
                                        var oGenJournal = session.GetObjectByKey<GenJournalHeader>(gid);
                                        var oItem = session.GetObjectByKey<Item>(iit);
                                        var oWarehouse = session.GetObjectByKey<Warehouse>(iwh);
                                        CriteriaOperator crit = CriteriaOperator.Parse("[GenJournalID]=? And [ItemNo]=? And [Warehouse]=?", oGenJournal, oItem, oWarehouse);
                                        PhysicalAdjustmentDetail pidO = session.FindObject<PhysicalAdjustmentDetail>(crit);
                                        rQty = pidO.ActualQtyBase;
                                    }
                                    else
                                    {
                                        decimal x = Convert.ToDecimal(dicj.Values[6].ToString()) - Convert.ToDecimal(dicj.Values[7].ToString());
                                        rQty += x;
                                    }
                                }
                            }

                            //if (rQty != 0)
                            //{
                            //    // Create initial remaining qty entry here
                            //    PurchasesMovementBuffer opmb = ReflectionHelper.CreateObject<PurchasesMovementBuffer>(session);
                            //    opmb.HeaderId = oReporter;
                            //    opmb.EntryDate = dtYrBefore;
                            //    opmb.QtyValue = rQty;
                            //    opmb.Item = session.GetObjectByKey<Item>(item.Oid);
                            //    opmb.Warehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(dwhse.Values[0].ToString()));
                            //    opmb.Save();
                            //}

                            //// Create initial remaining qty entry here
                            //PurchasesMovementBuffer iopmb = ReflectionHelper.CreateObject<PurchasesMovementBuffer>(session);
                            //iopmb.HeaderId = oReporter;
                            //iopmb.EntryDate = dtYrBefore;
                            //if (rQty<0)
                            //{
                            //    iopmb.QtyValue = 0;
                            //}
                            //else
                            //{
                            //    iopmb.QtyValue = rQty;
                            //}
                            //iopmb.WhseRunQty = iopmb.QtyValue;
                            //wrqty = iopmb.WhseRunQty;
                            //iopmb.Item = session.GetObjectByKey<Item>(item.Oid);
                            //iopmb.Warehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(dwhse.Values[0].ToString()));
                            //if (iopmb.QtyValue != 0)
                            //{
                            //    iopmb.Save();
                            //}

                            string qryYtcm = string.Format("select * from vInvControlJournals where SourceType not in " +
                                         "('DF42778E-F4FD-4D0E-B0EE-77CCEC90566D','6D0E3DCB-1725-4AF5-A0E3-583B7A9472FC','A95EBEEA-72D7-434B-95CB-A5C57668E315')" +
                                         " and ItemNo='{0}' and EntryDate > '{1}' and EntryDate <= '{2}' and Warehouse = '{3}' order by EntryDate",
                                         item.Oid, dtYrBefore.ToString("yyyy-MM-dd HH:mm:ss.fff"), endDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), dwhse.Values[0].ToString());
                            var ytcmData = session.ExecuteQuery(qryYtcm);
                            if (ytcmData.ResultSet[0].Rows.Count()>0)
                            {
                                // Create initial remaining qty entry here
                                PurchasesMovementBuffer iopmb = ReflectionHelper.CreateObject<PurchasesMovementBuffer>(session);
                                iopmb.HeaderId = oReporter;
                                iopmb.EntryDate = dtYrBefore;
                                if (rQty < 0)
                                {
                                    iopmb.QtyValue = 0;
                                }
                                else
                                {
                                    iopmb.QtyValue = rQty;
                                }
                                iopmb.WhseRunQty = iopmb.QtyValue;
                                wrqty = iopmb.WhseRunQty;
                                iopmb.Item = session.GetObjectByKey<Item>(item.Oid);
                                iopmb.Warehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(dwhse.Values[0].ToString()));
                                iopmb.Save();

                                pmovsum.InvBegYear += iopmb.QtyValue;
                                //if (iopmb.QtyValue != 0)
                                //{
                                //    iopmb.Save();
                                //}
                            }
                            UomBufferCollection uomBuffCols = new UomBufferCollection();
                            UomBuffer uomBuff = null;
                            for (int i = 0; i < ytcmData.ResultSet[0].Rows.Count(); i++)
                            {
                                SelectStatementResultRow srow = ytcmData.ResultSet[0].Rows[i];
                                // Initialize remaining quantity entry
                                PurchasesMovementBuffer opmb = ReflectionHelper.CreateObject<PurchasesMovementBuffer>(session);
                                // PurchasesMovementAnalysis _HeaderId;
                                opmb.HeaderId = oReporter;
                                // InventoryControlJournal _IcjID;
                                opmb.IcjID = session.GetObjectByKey<InventoryControlJournal>(Convert.ToInt32(srow.Values[0].ToString()));
                                opmb.SeqID = "SID-" + opmb.IcjID.Sequence;
                                opmb.SeqStr = opmb.IcjID.Sequence;
                                // GenJournalHeader _Source;
                                opmb.Source = session.GetObjectByKey<GenJournalHeader>(Convert.ToInt32(srow.Values[1].ToString()));
                                // DateTime _EntryDate;
                                opmb.EntryDate = DateTime.Parse(srow.Values[2].ToString());
                                // DateTime _PostedDate;
                                if (srow.Values[3] != null)
                                {
                                    opmb.PostedDate = DateTime.Parse(srow.Values[3].ToString());
                                }
                                // BusinessObjects.SourceType _SourceType;
                                opmb.SourceType = session.GetObjectByKey<SourceType>(Guid.Parse(srow.Values[4].ToString()));
                                // BusinessObjects.OperationType _OperationType;
                                opmb.OperationType = session.GetObjectByKey<OperationType>(Guid.Parse(srow.Values[5].ToString()));
                                // decimal _InQty;
                                opmb.InQty = Convert.ToDecimal(srow.Values[6].ToString());
                                // decimal _OutQty;
                                opmb.OutQty = Convert.ToDecimal(srow.Values[7].ToString());
                                // decimal _QtyValue;
                                opmb.QtyValue = Convert.ToDecimal(srow.Values[6].ToString()) - Convert.ToDecimal(srow.Values[7].ToString());
                                opmb.WhseRunQty = wrqty + opmb.QtyValue;
                                wrqty = opmb.WhseRunQty;
                                // BusinessObjects.Item _Item;
                                opmb.Item = session.GetObjectByKey<Item>(Guid.Parse(srow.Values[8].ToString()));
                                //// decimal _Cost;
                                //if (opmb.SourceType.Code=="RC")
                                //{
                                //    opmb.Cost = Convert.ToDecimal(srow.Values[9].ToString());
                                //}
                                //else if (opmb.SourceType.Code == "WO")
                                //{
                                //    opmb.Cost = Convert.ToDecimal(srow.Values[16].ToString());
                                //}
                                // UnitOfMeasure _Uom;
                                opmb.Uom = session.GetObjectByKey<UnitOfMeasure>(Guid.Parse(srow.Values[10].ToString()));

                                // BusinessObjects.Warehouse _Warehouse;
                                opmb.Warehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(srow.Values[11].ToString()));
                                // Guid _IcjRowID;
                                opmb.IcjRowID = srow.Values[12].ToString();
                                // BusinessObjects.Requisition _Requisition;
                                if (srow.Values[13] != null)
                                {
                                    opmb.Requisition = session.GetObjectByKey<Requisition>(Convert.ToInt32(srow.Values[13].ToString()));
                                }
                                // Employee _RequestedBy;
                                if (srow.Values[15] != null)
                                {
                                    opmb.RequestedBy = session.GetObjectByKey<Employee>(Guid.Parse(srow.Values[15].ToString()));
                                }
                                // CostCenter _ChargeTo;
                                if (srow.Values[14] != null)
                                {
                                    opmb.ChargeTo = session.GetObjectByKey<CostCenter>(Guid.Parse(srow.Values[14].ToString()));
                                }
                                string detQryStr = string.Empty;
                                switch (opmb.SourceType.Code)
                                {
                                    case "CM":
                                        opmb.Price = 0 - Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from CreditMemoDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var dcm = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            CreditMemoDetail cmd = session.GetObjectByKey<CreditMemoDetail>(Convert.ToInt32(dcm.Values[0].ToString()));
                                            if (cmd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", cmd.Oid.ToString(), cmd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "DM":
                                        opmb.Cost = 0 - Convert.ToDecimal(srow.Values[9].ToString());
                                        detQryStr = string.Format("select * from DebitMemoDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var ddm = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            DebitMemoDetail dmd = session.GetObjectByKey<DebitMemoDetail>(Convert.ToInt32(ddm.Values[0].ToString()));
                                            if (dmd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", dmd.Oid.ToString(), dmd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                        
                                    case "ECS":
                                        opmb.Price = Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from FuelPumpRegisterDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var decs = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            FuelPumpRegisterDetail fprd = session.GetObjectByKey<FuelPumpRegisterDetail>(Convert.ToInt32(decs.Values[0].ToString()));
                                            if (fprd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", fprd.Oid.ToString(), fprd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "FPR":
                                        opmb.Price = Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from EmployeeChargeSlipItemDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var dfpr = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            EmployeeChargeSlipItemDetail ecsid = session.GetObjectByKey<EmployeeChargeSlipItemDetail>(Convert.ToInt32(dfpr.Values[0].ToString()));
                                            if (ecsid != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", ecsid.Oid.ToString(), ecsid.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "IN":
                                        opmb.Price = Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from InvoiceDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var din = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            InvoiceDetail id = session.GetObjectByKey<InvoiceDetail>(Convert.ToInt32(din.Values[0].ToString()));
                                            if (id != null)
                                            {
                                                // [RefID] = '17408' And [SourceID.SourceNo] = 'ff'
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", id.Oid.ToString(), id.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                        
                                    case "PI":
                                        break;
                                    case "PO":
                                        break;
                                    case "RC":
                                        opmb.Cost = Convert.ToDecimal(srow.Values[9].ToString());
                                        detQryStr = string.Format("select * from ReceiptDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var drc = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            ReceiptDetail rcptd = session.GetObjectByKey<ReceiptDetail>(Convert.ToInt32(drc.Values[0].ToString()));
                                            if (rcptd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", rcptd.Oid.ToString(), rcptd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;

                                                pmovsum.Purchases += opmb.QtyValue;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "RQ":
                                        break;
                                    case "TO":
                                        break;
                                    case "WO":
                                        detQryStr = string.Format("select * from WorkOrderItemDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var dwo = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            WorkOrderItemDetail woitd = session.GetObjectByKey<WorkOrderItemDetail>(Convert.ToInt32(dwo.Values[0].ToString()));
                                            if (woitd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", woitd.Oid.ToString(), woitd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                //opmb.Income = ie.Income;
                                                //opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                // DateTime _IssueDate;
                                if (srow.Values[17] != null)
                                {
                                    opmb.PostedDate = DateTime.Parse(srow.Values[17].ToString());
                                }
                                if (opmb.QtyValue != 0)
                                {
                                    opmb.Save();
                                }
                            }
                        }
                        else
                        {                            
                            string qryYtcm = string.Format("select * from vInvControlJournals where SourceType not in " +
                                         "('DF42778E-F4FD-4D0E-B0EE-77CCEC90566D','6D0E3DCB-1725-4AF5-A0E3-583B7A9472FC','A95EBEEA-72D7-434B-95CB-A5C57668E315')" +
                                         " and ItemNo='{0}' and EntryDate > '{1}' and EntryDate <= '{2}' and Warehouse = '{3}' order by EntryDate",
                                         item.Oid, dtYrBefore.ToString("yyyy-MM-dd HH:mm:ss.fff"), endDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), dwhse.Values[0].ToString());
                            var ytcmData = session.ExecuteQuery(qryYtcm);

                            if (ytcmData.ResultSet[0].Rows.Count()>0)
                            {
                                // Create initial remaining qty entry here
                                PurchasesMovementBuffer iopmb = ReflectionHelper.CreateObject<PurchasesMovementBuffer>(session);
                                iopmb.HeaderId = oReporter;
                                iopmb.EntryDate = dtYrBefore;
                                iopmb.QtyValue = 0;
                                iopmb.WhseRunQty = iopmb.QtyValue;
                                wrqty = iopmb.WhseRunQty;
                                iopmb.Item = session.GetObjectByKey<Item>(item.Oid);
                                iopmb.Warehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(dwhse.Values[0].ToString()));
                                iopmb.Save();
                            }

                            for (int i = 0; i < ytcmData.ResultSet[0].Rows.Count(); i++)
                            {
                                SelectStatementResultRow srow = ytcmData.ResultSet[0].Rows[i];
                                // Initialize remaining quantity entry
                                PurchasesMovementBuffer opmb = ReflectionHelper.CreateObject<PurchasesMovementBuffer>(session);
                                // PurchasesMovementAnalysis _HeaderId;
                                opmb.HeaderId = oReporter;
                                // InventoryControlJournal _IcjID;
                                opmb.IcjID = session.GetObjectByKey<InventoryControlJournal>(Convert.ToInt32(srow.Values[0].ToString()));
                                opmb.SeqID = "SID-" + opmb.IcjID.Sequence;
                                opmb.SeqStr = opmb.IcjID.Sequence;
                                // GenJournalHeader _Source;
                                opmb.Source = session.GetObjectByKey<GenJournalHeader>(Convert.ToInt32(srow.Values[1].ToString()));
                                // DateTime _EntryDate;
                                opmb.EntryDate = DateTime.Parse(srow.Values[2].ToString());
                                // DateTime _PostedDate;
                                if (srow.Values[3] != null)
                                {
                                    opmb.PostedDate = DateTime.Parse(srow.Values[3].ToString());
                                }
                                // BusinessObjects.SourceType _SourceType;
                                opmb.SourceType = session.GetObjectByKey<SourceType>(Guid.Parse(srow.Values[4].ToString()));
                                // BusinessObjects.OperationType _OperationType;
                                opmb.OperationType = session.GetObjectByKey<OperationType>(Guid.Parse(srow.Values[5].ToString()));
                                // decimal _InQty;
                                opmb.InQty = Convert.ToDecimal(srow.Values[6].ToString());
                                // decimal _OutQty;
                                opmb.OutQty = Convert.ToDecimal(srow.Values[7].ToString());
                                // decimal _QtyValue;
                                opmb.QtyValue = Convert.ToDecimal(srow.Values[6].ToString()) - Convert.ToDecimal(srow.Values[7].ToString());
                                opmb.WhseRunQty = wrqty + opmb.QtyValue;
                                wrqty = opmb.WhseRunQty;
                                // BusinessObjects.Item _Item;
                                opmb.Item = session.GetObjectByKey<Item>(Guid.Parse(srow.Values[8].ToString()));
                                //// decimal _Cost;
                                //if (opmb.SourceType.Code=="RC")
                                //{
                                //    opmb.Cost = Convert.ToDecimal(srow.Values[9].ToString());
                                //}
                                //else if (opmb.SourceType.Code == "WO")
                                //{
                                //    opmb.Cost = Convert.ToDecimal(srow.Values[16].ToString());
                                //}
                                // UnitOfMeasure _Uom;
                                opmb.Uom = session.GetObjectByKey<UnitOfMeasure>(Guid.Parse(srow.Values[10].ToString()));
                                // BusinessObjects.Warehouse _Warehouse;
                                opmb.Warehouse = session.GetObjectByKey<Warehouse>(Guid.Parse(srow.Values[11].ToString()));
                                // Guid _IcjRowID;
                                opmb.IcjRowID = srow.Values[12].ToString();
                                // BusinessObjects.Requisition _Requisition;
                                if (srow.Values[13] != null)
                                {
                                    opmb.Requisition = session.GetObjectByKey<Requisition>(Convert.ToInt32(srow.Values[13].ToString()));
                                }
                                // Employee _RequestedBy;
                                if (srow.Values[15] != null)
                                {
                                    opmb.RequestedBy = session.GetObjectByKey<Employee>(Guid.Parse(srow.Values[15].ToString()));
                                }
                                // CostCenter _ChargeTo;
                                if (srow.Values[14] != null)
                                {
                                    opmb.ChargeTo = session.GetObjectByKey<CostCenter>(Guid.Parse(srow.Values[14].ToString()));
                                }
                                string detQryStr = string.Empty;
                                switch (opmb.SourceType.Code)
                                {
                                    case "CM":
                                        opmb.Price = 0 - Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from CreditMemoDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var dcm = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            CreditMemoDetail cmd = session.GetObjectByKey<CreditMemoDetail>(Convert.ToInt32(dcm.Values[0].ToString()));
                                            if (cmd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", cmd.Oid.ToString(), cmd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "DM":
                                        opmb.Cost = 0 - Convert.ToDecimal(srow.Values[9].ToString());
                                        detQryStr = string.Format("select * from DebitMemoDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var ddm = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            DebitMemoDetail dmd = session.GetObjectByKey<DebitMemoDetail>(Convert.ToInt32(ddm.Values[0].ToString()));
                                            if (dmd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", dmd.Oid.ToString(), dmd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }

                                    case "ECS":
                                        opmb.Price = Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from FuelPumpRegisterDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var decs = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            FuelPumpRegisterDetail fprd = session.GetObjectByKey<FuelPumpRegisterDetail>(Convert.ToInt32(decs.Values[0].ToString()));
                                            if (fprd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", fprd.Oid.ToString(), fprd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "FPR":
                                        opmb.Price = Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from EmployeeChargeSlipItemDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var dfpr = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            EmployeeChargeSlipItemDetail ecsid = session.GetObjectByKey<EmployeeChargeSlipItemDetail>(Convert.ToInt32(dfpr.Values[0].ToString()));
                                            if (ecsid != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", ecsid.Oid.ToString(), ecsid.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "IN":
                                        opmb.Price = Convert.ToDecimal(srow.Values[16].ToString());
                                        detQryStr = string.Format("select * from InvoiceDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var din = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            InvoiceDetail id = session.GetObjectByKey<InvoiceDetail>(Convert.ToInt32(din.Values[0].ToString()));
                                            if (id != null)
                                            {
                                                // [RefID] = '17408' And [SourceID.SourceNo] = 'ff'
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", id.Oid.ToString(), id.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }

                                    case "PI":
                                        break;
                                    case "PO":
                                        break;
                                    case "RC":
                                        opmb.Cost = Convert.ToDecimal(srow.Values[9].ToString());
                                        detQryStr = string.Format("select * from ReceiptDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var drc = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            ReceiptDetail rcptd = session.GetObjectByKey<ReceiptDetail>(Convert.ToInt32(drc.Values[0].ToString()));
                                            if (rcptd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", rcptd.Oid.ToString(), rcptd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                opmb.Income = ie.Income;
                                                opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    case "RQ":
                                        break;
                                    case "TO":
                                        break;
                                    case "WO":
                                        detQryStr = string.Format("select * from WorkOrderItemDetail where GenJournalID={0} and ItemNo='{1}'", Convert.ToInt32(srow.Values[1].ToString()), Guid.Parse(srow.Values[8].ToString()));
                                        try
                                        {
                                            var dwo = session.ExecuteQuery(detQryStr).ResultSet[0].Rows[0];
                                            WorkOrderItemDetail woitd = session.GetObjectByKey<WorkOrderItemDetail>(Convert.ToInt32(dwo.Values[0].ToString()));
                                            if (woitd != null)
                                            {
                                                IncomeAndExpense02 ie = session.FindObject<IncomeAndExpense02>(CriteriaOperator.Parse("[RefID] = ? And [SourceID.SourceNo] = ?", woitd.Oid.ToString(), woitd.GenJournalID.SourceNo));
                                                opmb.ExpenseType = ie.Category;
                                                opmb.SubExpenseType = ie.SubCategory;
                                                opmb.IssuedType = ie.PayeeType;
                                                opmb.IssuedTo = ie.Payee;
                                                //opmb.Income = ie.Income;
                                                //opmb.Expense = ie.Expense;
                                            }
                                            break;
                                        }
                                        catch (Exception)
                                        {
                                            break;
                                        }
                                    default:
                                        break;
                                }
                                // DateTime _IssueDate;
                                if (srow.Values[17] != null)
                                {
                                    opmb.PostedDate = DateTime.Parse(srow.Values[17].ToString());
                                }
                                if (opmb.QtyValue != 0)
                                {
                                    opmb.Save();
                                }
                            }
                        }
                    }

                    // Save per item summary
                    pmovsum.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                    CommitUpdatingSession(session);
                    _message = string.Format("Processing {0} succesfull.", index);
                    _BgWorker.ReportProgress(1, _message);
                }
            }
            finally
            {
                if (index == trans.Count())
                {
                    e.Result = index;
                    //CommitUpdatingSession(session);
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
            purchMovementGeneratorAction.
                Enabled.SetItemValue("Processing items", !inProgress);
        }
        public event EventHandler<SessionEventArgs> UpdatingSessionCreated;
        public event EventHandler<SessionEventArgs> UpdatingSessionCommitted;
    }
}
