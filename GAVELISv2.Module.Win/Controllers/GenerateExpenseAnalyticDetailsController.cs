using System;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;

namespace GAVELISv2.Module.Win.Controllers
{
    public class ExpenseAnalyticsWorkObject
    {
        public IOrderedEnumerable<ExpenseType> ExpenseTypesList { get; set; }
        public IList<GenJournalHeader> PaymentsList { get; set; }
    }
    public partial class GenerateExpenseAnalyticDetailsController : ViewController
    {
        private ExpenseAnalyticsWorkObject workObjects = new ExpenseAnalyticsWorkObject();
        private SimpleAction generateExpenseAnalyticDetailsAction;
        private ExpensesAnalyticsHeader _ExpensesAnalyticsHeader;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateExpenseAnalyticDetailsController()
        {
            this.TargetObjectType = typeof(ExpensesAnalyticsHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "GenerateExpenseAnalyticDetailsActionId";
            this.generateExpenseAnalyticDetailsAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateExpenseAnalyticDetailsAction.Caption = "Generate";
            this.generateExpenseAnalyticDetailsAction.Execute += new SimpleActionExecuteEventHandler(generateExpenseAnalyticDetailsAction_Execute);
        }

        void generateExpenseAnalyticDetailsAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ExpensesAnalyticsHeader = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as ExpensesAnalyticsHeader;
            ObjectSpace.CommitChanges();

            var myInClause = new string[] { "0000-1", "0000-2", "0000-3" };
            XPCollection<ExpenseType> exp = new XPCollection<ExpenseType>(((ObjectSpace)ObjectSpace).Session);
            IOrderedEnumerable<ExpenseType> orderBy = exp.Where(o => o.NoBuffer != true && o.Income != true && !myInClause.Contains(o.Code)).OrderBy(o => o.Code);
            if (orderBy.Count() == 0)
            {
                XtraMessageBox.Show("There are no expense types encoded in the system.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            // [GYear] = 2020 And [CRPayment] > 0.0m And [CRDeposit] = 0.0m And [OperationType!] = ##XpoObject#GAVELISv2.Module.BusinessObjects.OperationType({05998234-b94d-4502-95bd-82b5fc4d4660})#
            IList<GenJournalHeader> payments = ObjectSpace.GetObjects<GenJournalHeader>(CriteriaOperator.Parse(string.Format("[SourceType.Code] In ('CR', 'FT') And [Voided] = False And [CRPayment] > 0.0m And [CRDeposit] = 0.0m And [GYear] = {0}", _ExpensesAnalyticsHeader.Year)));
            int cnts = payments.Count();
            workObjects.ExpenseTypesList = orderBy;
            workObjects.PaymentsList = payments;
            _FrmProgress = new ProgressForm("Generating...", orderBy.Count() + cnts,
                        "Objects processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(workObjects);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            ExpenseAnalyticsWorkObject trans = (ExpenseAnalyticsWorkObject)e.Argument;
            int transCount = trans.ExpenseTypesList.Count() + trans.PaymentsList.Count();
            ExpensesAnalyticsHeader thisExpensesAnalyticsHeader = session.GetObjectByKey<ExpensesAnalyticsHeader>(_ExpensesAnalyticsHeader.Oid);
            try
            {
                Company companyInfo = Company.GetInstance(session);
                // Process all PettyCash
                var pettys = trans.PaymentsList.Where(o => o.OperationType.Code == "CV");
                foreach (var petty in pettys)
                {
                    index++;
                    _message = string.Format("Generating buffer {0} succesfull.",
                    petty.SourceNo);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms in here..

                    //System.Threading.Thread.Sleep(100);
                    foreach (var ptdet in petty.GenJournalDetails)
                    {
                        if (ptdet.ExpenseType == null || ptdet.CreditAmount > 0 && ptdet.DebitAmount <= 0)
                        {
                            continue;
                        }
                        string tmpBufferId = string.Format("{0}-{1}", thisExpensesAnalyticsHeader.Year, "petty-" + ptdet.RowID );
                        ExpenseAnalyticsBuffer eab;
                        eab = session.FindObject<ExpenseAnalyticsBuffer>(CriteriaOperator.Parse("[BufferId]=?", tmpBufferId));
                        if (eab == null)
                        {
                            eab = ReflectionHelper.CreateObject<ExpenseAnalyticsBuffer>(session);
                            eab.BufferId = tmpBufferId;
                        }
                        eab.Seq = index;
                        eab.Year = thisExpensesAnalyticsHeader.Year;
                        eab.EntryDate = petty.EntryDate;
                        eab.PayTo = session.GetObjectByKey<Contact>((petty as CheckVoucher).PayToOrder.Oid);
                        eab.Memo = petty.CRMemo;
                        eab.Source = session.GetObjectByKey<GenJournalHeader>(petty.Oid);
                        eab.PaymentMode = PaymentTypeEnum.Cash;
                        eab.CheckNo = (petty as CheckVoucher).CRCheckNo;
                        eab.CheckDate = (petty as CheckVoucher).CRCheckDate;
                        eab.ReferenceNo = (petty as CheckVoucher).CRReferenceNo;
                        eab.CheckAmount = (petty as CheckVoucher).CRPayment;
                        eab.LineDate = ptdet.CVLineDate;
                        eab.Payee = session.GetObjectByKey<Contact>(ptdet.SubAccountNo.Oid);
                        eab.Description = ptdet.Description;
                        eab.ExpenseType = session.GetObjectByKey<ExpenseType>(ptdet.ExpenseType.Oid);
                        eab.SubExpenseType = ptdet.SubExpenseType != null ? session.GetObjectByKey<SubExpenseType>(ptdet.SubExpenseType.Oid) : null;
                        eab.ChargeTo = ptdet.CostCenter != null ? session.GetObjectByKey<CostCenter>(ptdet.CostCenter.Oid) : null;
                        eab.Amount = ptdet.DebitAmount;
                        eab.Save();
                    }

                    #endregion
                    
                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }
                session.CommitTransaction();
                // Process all Non-PettyCash
                var nonPettys = trans.PaymentsList.Where(o => o.OperationType.Code == "PY");
                foreach (var nonPetty in nonPettys)
                {
                    index++;
                    _message = string.Format("Generating buffer {0} succesfull.",
                    nonPetty.SourceNo);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms in here..

                    if ((nonPetty as CheckPayment).CRPayment == 0 || (nonPetty as CheckPayment).ExpenseType == null)
                    {
                        continue;
                    }

                    string tmpBufferId = string.Format("{0}-{1}", thisExpensesAnalyticsHeader.Year, "nonpetty-" + (nonPetty as CheckPayment).Oid);
                    ExpenseAnalyticsBuffer eab;
                    eab = session.FindObject<ExpenseAnalyticsBuffer>(CriteriaOperator.Parse("[BufferId]=?", tmpBufferId));
                    if (eab == null)
                    {
                        eab = ReflectionHelper.CreateObject<ExpenseAnalyticsBuffer>(session);
                        eab.BufferId = tmpBufferId;
                    }
                    eab.Seq = index;
                    eab.Year = thisExpensesAnalyticsHeader.Year;
                    eab.EntryDate = nonPetty.EntryDate;
                    eab.PayTo = session.GetObjectByKey<Contact>((nonPetty as CheckPayment).PayToOrder.Oid);
                    eab.Memo = nonPetty.CRMemo;
                    eab.Source = session.GetObjectByKey<GenJournalHeader>(nonPetty.Oid);
                    eab.PaymentMode = (nonPetty as CheckPayment).CRPaymentMode;
                    eab.CheckNo = (nonPetty as CheckPayment).CRCheckNo;
                    eab.CheckDate = (nonPetty as CheckPayment).CheckDate;
                    eab.ReferenceNo = (nonPetty as CheckPayment).CRReferenceNo;
                    eab.CheckAmount = (nonPetty as CheckPayment).CRPayment;
                    eab.LineDate = nonPetty.EntryDate;
                    eab.Payee = session.GetObjectByKey<Contact>((nonPetty as CheckPayment).PayToOrder.Oid);
                    eab.Description = nonPetty.CRMemo;
                    eab.ExpenseType = session.GetObjectByKey<ExpenseType>((nonPetty as CheckPayment).ExpenseType.Oid);
                    eab.SubExpenseType = (nonPetty as CheckPayment).SubExpenseType != null ? session.GetObjectByKey<SubExpenseType>((nonPetty as CheckPayment).SubExpenseType.Oid) : null;
                    //eab.ChargeTo = session.GetObjectByKey<CostCenter>(ptdet.CostCenter.Oid); ;
                    eab.Amount = (nonPetty as CheckPayment).CheckAmount;
                    eab.Save();

                    #endregion

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }
                session.CommitTransaction();
                string cmd = string.Format("delete ExpensesAnalyticsDetails where Year = {0}", thisExpensesAnalyticsHeader.Year);
                session.ExecuteNonQuery(cmd);
                // Process ExpenseTypes
                foreach (var exp in trans.ExpenseTypesList)
                {
                    index++;
                    _message = string.Format("Processing expense Type {0} succesfull.",
                    exp.Description);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms in here..

                    ExpensesAnalyticsDetails ead = ReflectionHelper.CreateObject<ExpensesAnalyticsDetails>(session);
                    ead.ReporterId = thisExpensesAnalyticsHeader;
                    ead.Year = thisExpensesAnalyticsHeader.Year;
                    ead.Category = session.GetObjectByKey<ExpenseType>(exp.Oid);
                    //System.Threading.Thread.Sleep(100);
                    //IList<ExpenseAnalyticsBuffer> buffers = ObjectSpace.GetObjects<ExpenseAnalyticsBuffer>(CriteriaOperator.Parse(string.Format("[ExpenseType.Code] = {0}", exp.Code)));
                    XPCollection<ExpenseAnalyticsBuffer> buffers = new XPCollection<ExpenseAnalyticsBuffer>(session);
                    IOrderedEnumerable<ExpenseAnalyticsBuffer> orderBy = buffers.Where(o => o.ExpenseType.Code==exp.Code && o.Year == thisExpensesAnalyticsHeader.Year).OrderBy(o => o.Seq);
                    // January
                    ead.January = orderBy.Where(o => o.GMonth == MonthsEnum.January).Sum(o => o.Amount);
                    var janData = from order in orderBy
                                  where order.GMonth == MonthsEnum.January
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    //var jant = janData.FirstOrDefault();
                    foreach (var jant in janData)
                    {
                        if (jant != null)
                        {
                            switch (jant.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.JanCheck = jant.amount;
                                    ead.JanCheckPer = (jant.amount / ead.January) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.JanCash = jant.amount;
                                    ead.JanCashPer = (jant.amount / ead.January) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.JanWire = jant.amount;
                                    ead.JanWirePer = (jant.amount / ead.January) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.JanOthers = jant.amount;
                                    ead.JanOthersPer = (jant.amount / ead.January) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // February
                    ead.February = orderBy.Where(o => o.GMonth == MonthsEnum.February).Sum(o => o.Amount);
                    var febData = from order in orderBy
                                  where order.GMonth == MonthsEnum.February
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    //var febt = febData.FirstOrDefault();
                    foreach (var febt in febData)
                    {
                        if (febt != null)
                        {
                            switch (febt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.FebCheck = febt.amount;
                                    ead.FebCheckPer = (febt.amount / ead.February) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.FebCash = febt.amount;
                                    ead.FebCashPer = (febt.amount / ead.February) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.FebWire = febt.amount;
                                    ead.FebWirePer = (febt.amount / ead.February) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.FebOthers = febt.amount;
                                    ead.FebOthersPer = (febt.amount / ead.February) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // March
                    ead.March = orderBy.Where(o => o.GMonth == MonthsEnum.March).Sum(o => o.Amount);
                    var marData = from order in orderBy
                                  where order.GMonth == MonthsEnum.March
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var mart in marData)
                    {
                        if (mart != null)
                        {
                            switch (mart.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.MarCheck = mart.amount;
                                    ead.MarCheckPer = (mart.amount / ead.March) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.MarCash = mart.amount;
                                    ead.MarCashPer = (mart.amount / ead.March) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.MarWire = mart.amount;
                                    ead.MarWirePer = (mart.amount / ead.March) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.MarOthers = mart.amount;
                                    ead.MarOthersPer = (mart.amount / ead.March) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // April
                    ead.April = orderBy.Where(o => o.GMonth == MonthsEnum.April).Sum(o => o.Amount);
                    var aprData = from order in orderBy
                                  where order.GMonth == MonthsEnum.April
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var aprt in aprData)
                    {
                        if (aprt != null)
                        {
                            switch (aprt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.AprCheck = aprt.amount;
                                    ead.AprCheckPer = (aprt.amount / ead.April) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.AprCash = aprt.amount;
                                    ead.AprCashPer = (aprt.amount / ead.April) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.AprWire = aprt.amount;
                                    ead.AprWirePer = (aprt.amount / ead.April) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.AprOthers = aprt.amount;
                                    ead.AprOthersPer = (aprt.amount / ead.April) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // May
                    ead.May = orderBy.Where(o => o.GMonth == MonthsEnum.May).Sum(o => o.Amount);
                    var mayData = from order in orderBy
                                  where order.GMonth == MonthsEnum.May
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var mayt in mayData)
                    {
                        if (mayt != null)
                        {
                            switch (mayt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.MayCheck = mayt.amount;
                                    ead.MayCheckPer = (mayt.amount / ead.May) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.MayCash = mayt.amount;
                                    ead.MayCashPer = (mayt.amount / ead.May) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.MayWire = mayt.amount;
                                    ead.MayWirePer = (mayt.amount / ead.May) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.MayOthers = mayt.amount;
                                    ead.MayOthersPer = (mayt.amount / ead.May) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // June
                    ead.June = orderBy.Where(o => o.GMonth == MonthsEnum.June).Sum(o => o.Amount);
                    var junData = from order in orderBy
                                  where order.GMonth == MonthsEnum.June
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var junt in junData)
                    {
                        if (junt != null)
                        {
                            switch (junt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.JunCheck = junt.amount;
                                    ead.JunCheckPer = (junt.amount / ead.June) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.JunCash = junt.amount;
                                    ead.JunCashPer = (junt.amount / ead.June) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.JunWire = junt.amount;
                                    ead.JunWirePer = (junt.amount / ead.June) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.JunOthers = junt.amount;
                                    ead.JunOthersPer = (junt.amount / ead.June) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // July
                    ead.July = orderBy.Where(o => o.GMonth == MonthsEnum.July).Sum(o => o.Amount);
                    var julData = from order in orderBy
                                  where order.GMonth == MonthsEnum.July
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var jult in julData)
                    {
                        if (jult != null)
                        {
                            switch (jult.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.JulCheck = jult.amount;
                                    ead.JulCheckPer = (jult.amount / ead.July) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.JulCash = jult.amount;
                                    ead.JulCashPer = (jult.amount / ead.July) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.JulWire = jult.amount;
                                    ead.JulWirePer = (jult.amount / ead.July) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.JulOthers = jult.amount;
                                    ead.JulOthersPer = (jult.amount / ead.July) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // August
                    ead.August = orderBy.Where(o => o.GMonth == MonthsEnum.August).Sum(o => o.Amount);
                    var augData = from order in orderBy
                                  where order.GMonth == MonthsEnum.August
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var augt in augData)
                    {
                        if (augt != null)
                        {
                            switch (augt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.AugCheck = augt.amount;
                                    ead.AugCheckPer = (augt.amount / ead.August) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.AugCash = augt.amount;
                                    ead.AugCashPer = (augt.amount / ead.August) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.AugWire = augt.amount;
                                    ead.AugWirePer = (augt.amount / ead.August) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.AugOthers = augt.amount;
                                    ead.AugOthersPer = (augt.amount / ead.August) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // September
                    ead.September = orderBy.Where(o => o.GMonth == MonthsEnum.September).Sum(o => o.Amount);
                    var sepData = from order in orderBy
                                  where order.GMonth == MonthsEnum.September
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var sept in sepData)
                    {
                        if (sept != null)
                        {
                            switch (sept.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.SepCheck = sept.amount;
                                    ead.SepCheckPer = (sept.amount / ead.September) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.SepCash = sept.amount;
                                    ead.SepCashPer = (sept.amount / ead.September) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.SepWire = sept.amount;
                                    ead.SepWirePer = (sept.amount / ead.September) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.SepOthers = sept.amount;
                                    ead.SepOthersPer = (sept.amount / ead.September) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // October
                    ead.October = orderBy.Where(o => o.GMonth == MonthsEnum.October).Sum(o => o.Amount);
                    var octData = from order in orderBy
                                  where order.GMonth == MonthsEnum.October
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var octt in octData)
                    {
                        if (octt != null)
                        {
                            switch (octt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.OctCheck = octt.amount;
                                    ead.OctCheckPer = (octt.amount / ead.October) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.OctCash = octt.amount;
                                    ead.OctCashPer = (octt.amount / ead.October) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.OctWire = octt.amount;
                                    ead.OctWirePer = (octt.amount / ead.October) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.OctOthers = octt.amount;
                                    ead.OctOthersPer = (octt.amount / ead.October) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    
                    // November
                    ead.November = orderBy.Where(o => o.GMonth == MonthsEnum.November).Sum(o => o.Amount);
                    var novData = from order in orderBy
                                  where order.GMonth == MonthsEnum.November
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var novt in novData)
                    {
                        if (novt != null)
                        {
                            switch (novt.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.NovCheck = novt.amount;
                                    ead.NovCheckPer = (novt.amount / ead.November) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.NovCash = novt.amount;
                                    ead.NovCashPer = (novt.amount / ead.November) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.NovWire = novt.amount;
                                    ead.NovWirePer = (novt.amount / ead.November) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.NovOthers = novt.amount;
                                    ead.NovOthersPer = (novt.amount / ead.November) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    // December
                    ead.December = orderBy.Where(o => o.GMonth == MonthsEnum.December).Sum(o => o.Amount);
                    var decData = from order in orderBy
                                  where order.GMonth == MonthsEnum.December
                                  group order by order.PaymentMode into perMode
                                  select new
                                  {
                                      mode = perMode.Key,
                                      amount = perMode.Sum(x => x.Amount)
                                  };

                    foreach (var dect in decData)
                    {
                        if (dect != null)
                        {
                            switch (dect.mode)
                            {
                                case PaymentTypeEnum.Check:
                                    ead.DecCheck = dect.amount;
                                    ead.DecCheckPer = (dect.amount / ead.December) * 100;
                                    break;
                                case PaymentTypeEnum.Cash:
                                    ead.DecCash = dect.amount;
                                    ead.DecCashPer = (dect.amount / ead.December) * 100;
                                    break;
                                case PaymentTypeEnum.WireTransfer:
                                    ead.DecWire = dect.amount;
                                    ead.DecWirePer = (dect.amount / ead.December) * 100;
                                    break;
                                case PaymentTypeEnum.Others:
                                    ead.DecOthers = dect.amount;
                                    ead.DecOthersPer = (dect.amount / ead.December) * 100;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    ead.Save();

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
                if (index == transCount)
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
                    //ObjectSpace.ReloadObject(_IncomeExpenseReporter);
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
