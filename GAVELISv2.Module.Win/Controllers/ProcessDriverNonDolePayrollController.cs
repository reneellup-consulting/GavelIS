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

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class ProcessDriverNonDolePayrollController : ViewController
    {
        private SimpleAction processDriverPayroll;
        private DriverPayrollBatch2 _DriverPayrollBatch;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;

        public ProcessDriverNonDolePayrollController()
        {
            this.TargetObjectType = typeof(DriverPayrollBatch2);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "DriverPayrollBatch2.ProcessDriverPayroll";
            this.processDriverPayroll = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.processDriverPayroll.TargetObjectsCriteria = "[Status] = 'Current'";
            this.processDriverPayroll.Caption = "Process Trips";
            this.processDriverPayroll.Execute += new
            SimpleActionExecuteEventHandler(
            processDriverPayroll_Execute);
        }

        private void processDriverPayroll_Execute(object sender,
        SimpleActionExecuteEventArgs e)
        {
            _DriverPayrollBatch = ((DevExpress.ExpressApp.DetailView)this.View).CurrentObject as DriverPayrollBatch2;

            ObjectSpace.CommitChanges();

            IList<DriverRegistry> included;
            string perioEndStr = string.Format("[Date] <= #{0}#", _DriverPayrollBatch.PeriodEnd.AddDays(1).ToString("yyyy-MM-dd"));
            if (!string.IsNullOrEmpty(_DriverPayrollBatch.BatchType.RegistryFilter))
            {
                included = ObjectSpace.GetObjects<DriverRegistry>(CriteriaOperator.Parse(string.Format("{0} And Not [Status] In ('Paid') And {1}", _DriverPayrollBatch.BatchType.RegistryFilter, perioEndStr)));
            }
            else
            {
                included = ObjectSpace.GetObjects<DriverRegistry>(CriteriaOperator.Parse(string.Format("Not [Status] In ('Paid') And {0}", perioEndStr)));
            }
            _FrmProgress = new ProgressForm("Processing data...", included.Count,
            "Data processed {0} of {1} ");
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
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IList<DriverRegistry> _included = (IList<DriverRegistry>)e.Argument;
            try
            {
                DriverPayrollBatch2 dpb = session.GetObjectByKey<DriverPayrollBatch2>(_DriverPayrollBatch.Oid);
                foreach (var item in _included)
                {
                    index++;
                    _message = string.Format("Processing {0} succesfull.",
                    item.Oid);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    // Calculate PartialPay Start
                    DriverRegistry odr = session.GetObjectByKey<DriverRegistry>(item.Oid);
                    StringBuilder sb = new StringBuilder();
                    bool hasError = false;
                    sb.AppendFormat("Problems found in Driver Registry ID#{0}. ", odr.Oid);
                    if (odr.Tariff == null)
                    {
                        hasError = true;
                        sb.Append("Tariff is not specified and ");
                    }
                    if (odr.Driver.DriverClassification == null)
                    {
                        hasError = true;
                        sb.AppendFormat("Driver {0} Driver Classification is not specified     ", odr.Driver.Name);
                    }
                    if (dpb.BatchType.TaggedFuelRequired && string.IsNullOrEmpty(odr.TripID.TaggedFuelReceipts))
                    {
                        hasError = true;
                        sb.AppendFormat("The trip {0} has no tagged fuel receipts     ", odr.TripNo);
                    }
                    if (hasError)
                    {
                        sb.Remove(sb.Length - 5, 5);
                        sb.Append(".");
                        throw new ApplicationException(sb.ToString());
                    }

                    // Start: To make the parent trip tariff to be the default
                    string tariffCode = string.Empty;
                    if (odr.TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        tariffCode = ((StanfilcoTrip)odr.TripID).Tariff.Code;
                    }
                    if (odr.TripID.GetType() == typeof(DolefilTrip))
                    {
                        tariffCode = ((DolefilTrip)odr.TripID).Tariff.Code;
                    }
                    if (odr.TripID.GetType() == typeof(OtherTrip))
                    {
                        tariffCode = ((OtherTrip)odr.TripID).Tariff.Code;
                    }
                    // End: To make the parent trip tariff to be the default

                    TariffDriversClassifier trfclass = session.FindObject<TariffDriversClassifier>(CriteriaOperator.Parse(string.Format("[TariffID.Code] = '{0}' And [DriverClass.Code] = '{1}'", tariffCode, odr.Driver.DriverClassification.Code)));
                    if (trfclass == null)
                    {
                        throw new ApplicationException(string.Format("Tariff driver classifiers has not been set up for Tariff #{0}. Please check", tariffCode));
                    }
                    odr.CppBasic = trfclass.BaseShare * (trfclass.ShareRate / 100);
                    odr.CppAdlMiscExp = trfclass.BaseShare - odr.CppBasic;

                    if (odr.TripID.GetType() == typeof(StanfilcoTrip))
                    {
                        odr.CppMiscExp = ((StanfilcoTrip)odr.TripID).Allowance;
                    }
                    if (odr.TripID.GetType() == typeof(DolefilTrip))
                    {
                        odr.CppMiscExp = ((DolefilTrip)odr.TripID).Allowance.Value;
                    }
                    if (odr.TripID.GetType() == typeof(OtherTrip))
                    {
                        odr.CppMiscExp = ((OtherTrip)odr.TripID).Allowance;
                    }
                    if (odr.CppMiscExp == 0)
                    {
                        odr.CppMiscExp = trfclass.TariffID.Allowance;
                    }
                    ICollection kds;
                    SortingCollection sorts = new SortingCollection(null);
                    DevExpress.Xpo.Metadata.XPClassInfo kdsClassInfo = session.GetClassInfo(typeof(KDEntry)); ;
                    kds = session.GetObjects(kdsClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + odr.TripID.SourceNo + "'"), sorts, 0, false, true);
                    decimal k = 0m;
                    foreach (KDEntry kd in kds)
                    {
                        k = k + trfclass.KDShare;
                    }
                    odr.CppKDs = k;
                    ICollection shunts;
                    DevExpress.Xpo.Metadata.XPClassInfo shuntingClassInfo = session.GetClassInfo(typeof(ShuntingEntry));
                    shunts = session.GetObjects(shuntingClassInfo, CriteriaOperator.Parse("[TripID.SourceNo] = '" + odr.TripID.SourceNo + "'"), sorts, 0, false, true);
                    decimal s = 0m;
                    foreach (ShuntingEntry sh in shunts)
                    {
                        s = s + trfclass.ShuntingShare;
                    }
                    odr.CppShunting = s;
                    odr.Status = DriverRegistryStatusEnum.Processed;
                    odr.PayrollBatchID = dpb;
                    odr.Save();

                    // Calculate PartialPay End

                    DriverPayroll2 dpr = dpb.DriverPayrolls2.Where(o => o.Employee == odr.Driver).FirstOrDefault();
                    if (dpr == null)
                    {
                        dpr = ReflectionHelper.CreateObject<DriverPayroll2>(session);
                        dpr.PayrollBatchID = dpb;
                        dpr.Include = true;
                    }

                    dpr.Employee = odr.Driver;

                    #region Trip processing here...

                    DriverPayrollTripLine2 dptl2 = dpr.DriverPayrollTripLines.Where(o => o.DriverRegistryId == odr).FirstOrDefault();
                    if (dptl2 == null)
                    {
                        dptl2 = ReflectionHelper.CreateObject<DriverPayrollTripLine2>(session);
                        dptl2.DriverPayrollID = dpr;
                        dptl2.DriverRegistryId = odr;
                    }

                    dptl2.TripDate = odr.Date;
                    dptl2.DocumentNo = odr.ReferenceNo;
                    dptl2.Driver = odr.Driver;
                    dptl2.Commission = odr.TripCommission;
                    dptl2.KDs = odr.Kds ?? 0;
                    dptl2.Shunting = odr.Shunting ?? 0;
                    dptl2.Include = true;
                    dptl2.Save();

                    #endregion
                    
                    #region Incentives here...
                    //if (arc.EmployeeID.EmpIncentives.Count > 0)
                    //{
                    //    foreach (var inc in arc.EmployeeID.EmpIncentives)
                    //    {
                    //        if (inc.OnlyInBatch != null && inc.OnlyInBatch != dpb.BatchType)
                    //        {
                    //            continue;
                    //        }
                    //        string lid = string.Format("0{0}-{1}", spr.AttRecId.Oid, inc.Oid);
                    //        StaffPayrollAdjustment spa = session.FindObject<StaffPayrollAdjustment>(CriteriaOperator.Parse("[LineID]=?", lid));
                    //        if (spa == null)
                    //        {
                    //            spa = ReflectionHelper.CreateObject<StaffPayrollAdjustment>(session);
                    //            spa.LineID = lid;
                    //        }
                    //        spa.StaffPayrollID = spr;
                    //        spa.Employee = arc.EmployeeID;
                    //        spa.AdjustmentType = inc.AdjustmentType;
                    //        spa.Explanation = !string.IsNullOrEmpty(inc.Explanation) ? inc.Explanation : inc.AdjustmentType.Description;
                    //        spa.AutoGenerated = true;
                    //        if (inc.DivPerDay)
                    //        {
                    //            int days = WorkingDays.GetWorkingDays(arc.BatchID.TimeRangeFrom, arc.BatchID.TimeRangeTo, new List<DayOfWeek> { arc.EmployeeID.RestDay });
                    //            decimal pd = inc.Amount / days;
                    //            if (AlterValue(spa.Amount))
                    //            {
                    //                spa.Amount = pd * arc.NoOfDays != null ? arc.NoOfDays.Value : 0;
                    //            }
                    //        }
                    //        else
                    //        {
                    //            if (AlterValue(spa.Amount))
                    //            {
                    //                spa.Amount = inc.Amount;
                    //            }
                    //        }
                    //        spa.Save();
                    //    }
                    //}
                    #endregion

                    #region Premiums here...
                    //if (dpb.BatchType.IncludePremiums)
                    //{
                    //    foreach (var prms in arc.EmployeeID.EmployeePremiums)
                    //    {
                    //        string lid = string.Format("1{0}-{1}", spr.AttRecId.Oid, prms.Oid);
                    //        StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                    //        if (spd == null)
                    //        {
                    //            spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                    //            spd.LineID = lid;
                    //            spd.DedId = prms.Oid;
                    //        }
                    //        spd.StaffPayrollID = spr;
                    //        spd.Employee = arc.EmployeeID;
                    //        spd.DeductionType = DeductionType.Premium;
                    //        spd.DeductionName = prms.PremiumCode.Description;
                    //        if (dpb.PeriodStart.Month == 1)
                    //            spd.Month = MonthsEnum.January;
                    //        if (dpb.PeriodStart.Month == 2)
                    //            spd.Month = MonthsEnum.February;
                    //        if (dpb.PeriodStart.Month == 3)
                    //            spd.Month = MonthsEnum.March;
                    //        if (dpb.PeriodStart.Month == 4)
                    //            spd.Month = MonthsEnum.April;
                    //        if (dpb.PeriodStart.Month == 5)
                    //            spd.Month = MonthsEnum.May;
                    //        if (dpb.PeriodStart.Month == 6)
                    //            spd.Month = MonthsEnum.June;
                    //        if (dpb.PeriodStart.Month == 7)
                    //            spd.Month = MonthsEnum.July;
                    //        if (dpb.PeriodStart.Month == 8)
                    //            spd.Month = MonthsEnum.August;
                    //        if (dpb.PeriodStart.Month == 9)
                    //            spd.Month = MonthsEnum.September;
                    //        if (dpb.PeriodStart.Month == 10)
                    //            spd.Month = MonthsEnum.October;
                    //        if (dpb.PeriodStart.Month == 11)
                    //            spd.Month = MonthsEnum.November;
                    //        if (dpb.PeriodStart.Month == 12)
                    //            spd.Month = MonthsEnum.December;
                    //        spd.Caption = string.Format("{0}|{1}", prms.PremiumCode.Description, spd.MonthStr);
                    //        if (AlterValue(spd.Amount))
                    //        {
                    //            spd.Amount = prms.Amount;
                    //        }
                    //        spd.Save();
                    //    }
                    //}
                    #endregion

                    #region EveryPayroll here...

                    //foreach (var lns in arc.EmployeeID.EmpLoans)
                    //{
                    //    if (!lns.LoanCode.EveryPayrollDeduction)
                    //    {
                    //        continue;
                    //    }
                    //    if (lns.Paid)
                    //    {
                    //        continue;
                    //    }
                    //    string lid = string.Format("2{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                    //    StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                    //    if (spd == null)
                    //    {
                    //        spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                    //        spd.LineID = lid;
                    //        spd.DedId = lns.Oid;
                    //    }
                    //    spd.StaffPayrollID = spr;
                    //    spd.Employee = arc.EmployeeID;
                    //    spd.DeductionType = DeductionType.Loan;
                    //    spd.DeductionName = lns.LoanCode.Description;
                    //    if (dpb.PeriodStart.Month == 1)
                    //        spd.Month = MonthsEnum.January;
                    //    if (dpb.PeriodStart.Month == 2)
                    //        spd.Month = MonthsEnum.February;
                    //    if (dpb.PeriodStart.Month == 3)
                    //        spd.Month = MonthsEnum.March;
                    //    if (dpb.PeriodStart.Month == 4)
                    //        spd.Month = MonthsEnum.April;
                    //    if (dpb.PeriodStart.Month == 5)
                    //        spd.Month = MonthsEnum.May;
                    //    if (dpb.PeriodStart.Month == 6)
                    //        spd.Month = MonthsEnum.June;
                    //    if (dpb.PeriodStart.Month == 7)
                    //        spd.Month = MonthsEnum.July;
                    //    if (dpb.PeriodStart.Month == 8)
                    //        spd.Month = MonthsEnum.August;
                    //    if (dpb.PeriodStart.Month == 9)
                    //        spd.Month = MonthsEnum.September;
                    //    if (dpb.PeriodStart.Month == 10)
                    //        spd.Month = MonthsEnum.October;
                    //    if (dpb.PeriodStart.Month == 11)
                    //        spd.Month = MonthsEnum.November;
                    //    if (dpb.PeriodStart.Month == 12)
                    //        spd.Month = MonthsEnum.December;
                    //    spd.Caption = string.Format("{0}|{1}", lns.LoanCode.Code, lns.RefNo);
                    //    if (lns.LoanBalance < lns.Amortization)
                    //    {
                    //        if (AlterValue(spd.Amount))
                    //        {
                    //            spd.Amount = lns.LoanBalance;
                    //        }
                    //        //spd.Amount = lns.LoanBalance;
                    //    }
                    //    else
                    //    {
                    //        if (AlterValue(spd.Amount))
                    //        {
                    //            spd.Amount = lns.Amortization;
                    //        }
                    //        //spd.Amount = lns.Amortization;
                    //    }
                    //    spd.RefNo = lns.RefNo;
                    //    spd.Balance = lns.LoanBalance;
                    //    spd.Save();
                    //}
                    #endregion

                    #region Loans here...
                    //if (dpb.BatchType.IncludeLoans)
                    //{
                    //    foreach (var lns in arc.EmployeeID.EmpLoans)
                    //    {
                    //        if (lns.LoanCode.EveryPayrollDeduction)
                    //        {
                    //            continue;
                    //        }
                    //        if (lns.Paid)
                    //        {
                    //            continue;
                    //        }
                    //        string lid = string.Format("2{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                    //        StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                    //        if (spd == null)
                    //        {
                    //            spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                    //            spd.LineID = lid;
                    //            spd.DedId = lns.Oid;
                    //        }
                    //        spd.StaffPayrollID = spr;
                    //        spd.Employee = arc.EmployeeID;
                    //        spd.DeductionType = DeductionType.Loan;
                    //        spd.DeductionName = lns.LoanCode.Description;
                    //        if (dpb.PeriodStart.Month == 1)
                    //            spd.Month = MonthsEnum.January;
                    //        if (dpb.PeriodStart.Month == 2)
                    //            spd.Month = MonthsEnum.February;
                    //        if (dpb.PeriodStart.Month == 3)
                    //            spd.Month = MonthsEnum.March;
                    //        if (dpb.PeriodStart.Month == 4)
                    //            spd.Month = MonthsEnum.April;
                    //        if (dpb.PeriodStart.Month == 5)
                    //            spd.Month = MonthsEnum.May;
                    //        if (dpb.PeriodStart.Month == 6)
                    //            spd.Month = MonthsEnum.June;
                    //        if (dpb.PeriodStart.Month == 7)
                    //            spd.Month = MonthsEnum.July;
                    //        if (dpb.PeriodStart.Month == 8)
                    //            spd.Month = MonthsEnum.August;
                    //        if (dpb.PeriodStart.Month == 9)
                    //            spd.Month = MonthsEnum.September;
                    //        if (dpb.PeriodStart.Month == 10)
                    //            spd.Month = MonthsEnum.October;
                    //        if (dpb.PeriodStart.Month == 11)
                    //            spd.Month = MonthsEnum.November;
                    //        if (dpb.PeriodStart.Month == 12)
                    //            spd.Month = MonthsEnum.December;
                    //        spd.Caption = string.Format("{0}|{1}", lns.LoanCode.Code, lns.RefNo);
                    //        if (lns.LoanBalance < lns.Amortization)
                    //        {
                    //            if (AlterValue(spd.Amount))
                    //            {
                    //                spd.Amount = lns.LoanBalance;
                    //            }
                    //            //spd.Amount = lns.LoanBalance;
                    //        }
                    //        else
                    //        {
                    //            if (AlterValue(spd.Amount))
                    //            {
                    //                spd.Amount = lns.Amortization;
                    //            }
                    //            //spd.Amount = lns.Amortization;
                    //        }
                    //        spd.RefNo = lns.RefNo;
                    //        spd.Balance = lns.LoanBalance;
                    //        spd.Save();
                    //    }
                    //}
                    #endregion

                    #region Taxes here...
                    //if (dpb.BatchType.IncludeTax)
                    //{
                    //    foreach (var lns in arc.EmployeeID.EmpTaxs)
                    //    {
                    //        if (lns.Paid)
                    //        {
                    //            continue;
                    //        }
                    //        string lid = string.Format("3{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                    //        StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                    //        if (spd == null)
                    //        {
                    //            spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                    //            spd.LineID = lid;
                    //            spd.DedId = lns.Oid;
                    //        }
                    //        spd.StaffPayrollID = spr;
                    //        spd.Employee = arc.EmployeeID;
                    //        spd.DeductionType = DeductionType.Tax;
                    //        spd.DeductionName = lns.TaxCode.Description;
                    //        if (dpb.PeriodStart.Month == 1)
                    //            spd.Month = MonthsEnum.January;
                    //        if (dpb.PeriodStart.Month == 2)
                    //            spd.Month = MonthsEnum.February;
                    //        if (dpb.PeriodStart.Month == 3)
                    //            spd.Month = MonthsEnum.March;
                    //        if (dpb.PeriodStart.Month == 4)
                    //            spd.Month = MonthsEnum.April;
                    //        if (dpb.PeriodStart.Month == 5)
                    //            spd.Month = MonthsEnum.May;
                    //        if (dpb.PeriodStart.Month == 6)
                    //            spd.Month = MonthsEnum.June;
                    //        if (dpb.PeriodStart.Month == 7)
                    //            spd.Month = MonthsEnum.July;
                    //        if (dpb.PeriodStart.Month == 8)
                    //            spd.Month = MonthsEnum.August;
                    //        if (dpb.PeriodStart.Month == 9)
                    //            spd.Month = MonthsEnum.September;
                    //        if (dpb.PeriodStart.Month == 10)
                    //            spd.Month = MonthsEnum.October;
                    //        if (dpb.PeriodStart.Month == 11)
                    //            spd.Month = MonthsEnum.November;
                    //        if (dpb.PeriodStart.Month == 12)
                    //            spd.Month = MonthsEnum.December;
                    //        spd.Caption = string.Format("{0}|{1}", lns.TaxCode.Description, spd.MonthStr);
                    //        if (lns.TaxBalance < lns.Deduction)
                    //        {
                    //            if (AlterValue(spd.Amount))
                    //            {
                    //                spd.Amount = lns.TaxBalance;
                    //            }
                    //            //spd.Amount = lns.TaxBalance;
                    //        }
                    //        else
                    //        {
                    //            if (AlterValue(spd.Amount))
                    //            {
                    //                spd.Amount = lns.Deduction;
                    //            }
                    //            //spd.Amount = lns.Deduction;
                    //        }
                    //        spd.RefNo = lns.Year.ToString();
                    //        spd.Balance = lns.TaxBalance;
                    //        spd.Save();
                    //    }
                    //}
                    #endregion

                    #region Other Deduction

                    //if (dpb.BatchType.IncludeOtherDed)
                    //{
                    //    foreach (var lns in arc.EmployeeID.EmpOtherDeds)
                    //    {
                    //        if (lns.Paid)
                    //        {
                    //            continue;
                    //        }
                    //        string lid = string.Format("4{0}-{1}", spr.AttRecId.Oid, lns.Oid);
                    //        StaffPayrollDeduction spd = session.FindObject<StaffPayrollDeduction>(CriteriaOperator.Parse("[LineID]=?", lid));
                    //        if (spd == null)
                    //        {
                    //            spd = ReflectionHelper.CreateObject<StaffPayrollDeduction>(session);
                    //            spd.LineID = lid;
                    //            spd.DedId = lns.Oid;
                    //        }
                    //        spd.StaffPayrollID = spr;
                    //        spd.Employee = arc.EmployeeID;
                    //        spd.DeductionType = DeductionType.Other;
                    //        if (!string.IsNullOrEmpty(lns.Explanation))
                    //        {
                    //            spd.DeductionName = lns.DedCode.Code + " | " + lns.Explanation;
                    //            spd.Caption = string.Format("{0}|{1}|{2}", lns.DedCode.Description, lns.EntryDate.ToShortDateString(), lns.RefNo);
                    //            //spd.Caption = string.Format("{0}|{1}|{2}-{3}", lns.DedCode.Code, lns.EntryDate.ToShortDateString(), lns.RefNo, lns.Explanation);
                    //        }
                    //        else
                    //        {
                    //            spd.DeductionName = lns.DedCode.Description;
                    //            spd.Caption = string.Format("{0}|{1}|{2}", lns.DedCode.Description, lns.EntryDate.ToShortDateString(), lns.RefNo);
                    //        }
                    //        if (dpb.PeriodStart.Month == 1)
                    //            spd.Month = MonthsEnum.January;
                    //        if (dpb.PeriodStart.Month == 2)
                    //            spd.Month = MonthsEnum.February;
                    //        if (dpb.PeriodStart.Month == 3)
                    //            spd.Month = MonthsEnum.March;
                    //        if (dpb.PeriodStart.Month == 4)
                    //            spd.Month = MonthsEnum.April;
                    //        if (dpb.PeriodStart.Month == 5)
                    //            spd.Month = MonthsEnum.May;
                    //        if (dpb.PeriodStart.Month == 6)
                    //            spd.Month = MonthsEnum.June;
                    //        if (dpb.PeriodStart.Month == 7)
                    //            spd.Month = MonthsEnum.July;
                    //        if (dpb.PeriodStart.Month == 8)
                    //            spd.Month = MonthsEnum.August;
                    //        if (dpb.PeriodStart.Month == 9)
                    //            spd.Month = MonthsEnum.September;
                    //        if (dpb.PeriodStart.Month == 10)
                    //            spd.Month = MonthsEnum.October;
                    //        if (dpb.PeriodStart.Month == 11)
                    //            spd.Month = MonthsEnum.November;
                    //        if (dpb.PeriodStart.Month == 12)
                    //            spd.Month = MonthsEnum.December;
                    //        if (lns.Balance < lns.Deduction)
                    //        {
                    //            if (AlterValue(spd.Amount))
                    //            {
                    //                spd.Amount = lns.Balance;
                    //            }
                    //            //spd.Amount = lns.Balance;
                    //        }
                    //        else
                    //        {
                    //            if (AlterValue(spd.Amount))
                    //            {
                    //                spd.Amount = lns.Deduction;
                    //            }
                    //            //spd.Amount = lns.Deduction;
                    //        }
                    //        spd.RefNo = lns.RefNo;
                    //        spd.Balance = lns.Balance;
                    //        spd.Save();
                    //    }
                    //}
                    #endregion

                    dpb.Processed = true;
                    dpb.Save();
                    CommitUpdatingSession(session);

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
                if (index == _included.Count)
                {
                    CommitUpdatingSession(session);
                }
                session.Dispose();
            }
        }

        private bool AlterValue(decimal p)
        {
            if (p == 0)
            {
                return true;
            }
            else
            {
                return false;
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
                    "Generation of payroll data is cancelled.", "Cancelled",
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
                    "Payroll data has been successfully generated.");
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
