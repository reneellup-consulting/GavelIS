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
using DevExpress.Xpo.DB;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class GenerateThirteenthDays : ViewController
    {
        private SimpleAction generateThirteenthDaysAction;
        private ThirteenthGeneratorHeader _ThirteenthGeneratorHeader;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateThirteenthDays()
        {
            this.TargetObjectType = typeof(ThirteenthGeneratorHeader);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "ThirteenthGeneratorHeader.GenerateThirteenthDays";
            this.generateThirteenthDaysAction = new SimpleAction(this,
            actionID, PredefinedCategory.RecordEdit);
            this.generateThirteenthDaysAction.Caption = "Generate";
            this.generateThirteenthDaysAction.Execute += new SimpleActionExecuteEventHandler(generateThirteenthDaysAction_Execute);
        }

        void generateThirteenthDaysAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _ThirteenthGeneratorHeader = ((DevExpress.ExpressApp.DetailView)this.View).
                    CurrentObject as ThirteenthGeneratorHeader;

            try
            {
                _ThirteenthGeneratorHeader.DecLastYr = 0;
                _ThirteenthGeneratorHeader.JanThisYr = 0;
                _ThirteenthGeneratorHeader.FebThisYr = 0;
                _ThirteenthGeneratorHeader.MarThisYr = 0;
                _ThirteenthGeneratorHeader.AprThisYr = 0;
                _ThirteenthGeneratorHeader.MayThisYr = 0;
                _ThirteenthGeneratorHeader.JunThisYr = 0;
                _ThirteenthGeneratorHeader.JulThisYr = 0;
                _ThirteenthGeneratorHeader.AugThisYr = 0;
                _ThirteenthGeneratorHeader.SepThisYr = 0;
                _ThirteenthGeneratorHeader.OctThisYr = 0;
                _ThirteenthGeneratorHeader.NovThisYr = 0;
                _ThirteenthGeneratorHeader.DecThisYr = 0;
                _ThirteenthGeneratorHeader.TotalWithoutSat = 0;
                
                for (int i = _ThirteenthGeneratorHeader.ThirteenthGeneratorDetails.Count - 1; i >= 0; i--)
                {
                    _ThirteenthGeneratorHeader.ThirteenthGeneratorDetails[i].Delete();
                }
            }
            catch (Exception)
            {
            }

            ObjectSpace.CommitChanges();

            CriteriaOperator criteria = CriteriaOperator.Parse("[Date] >= ? AND [Date] <= ?", _ThirteenthGeneratorHeader.StartDate, _ThirteenthGeneratorHeader.EndDate);
            XPCollection<CheckInAndOut03> collection = new XPCollection<CheckInAndOut03>(((ObjectSpace)ObjectSpace).Session, criteria);

            if (collection.Count == 0)
            {
                throw new UserFriendlyException("There are no entries found");
            }

            _FrmProgress = new ProgressForm("Generating data...", collection.Count,
            "Entries processed {0} of {1} ");

            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(collection);
            _FrmProgress.ShowDialog();
        }

        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e) {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            ThirteenthGeneratorHeader tgh = session.GetObjectByKey<ThirteenthGeneratorHeader>(
            _ThirteenthGeneratorHeader.Oid);
            XPCollection<CheckInAndOut03> entries = (XPCollection<CheckInAndOut03>)e.Argument;

            try
            {
                int lastYr = tgh.StartDate.Year;
                int currYr = tgh.EndDate.Year;

                DateTime startDate = tgh.StartDate;
                DateTime endDate = tgh.EndDate;

                SelectedData result = session.ExecuteSproc("GetWorkDaysFor13thMonth",
                new OperandValue(startDate),
                new OperandValue(endDate));

                SelectStatementResultRow[] rows = result.ResultSet[0].Rows;

                int numMonths = Math.Min(rows.Length, 13); // Ensure you don't go out of bounds

                for (int i = 0; i < numMonths; i++)
                {
                    int value = Convert.ToInt32(rows[i].Values[1]);

                    switch (i)
                    {
                        case 0:
                            tgh.DecLastYr = value;
                            break;
                        case 1:
                            tgh.JanThisYr = value;
                            break;
                        case 2:
                            tgh.FebThisYr = value;
                            break;
                        case 3:
                            tgh.MarThisYr = value;
                            break;
                        case 4:
                            tgh.AprThisYr = value;
                            break;
                        case 5:
                            tgh.MayThisYr = value;
                            break;
                        case 6:
                            tgh.JunThisYr = value;
                            break;
                        case 7:
                            tgh.JulThisYr = value;
                            break;
                        case 8:
                            tgh.AugThisYr = value;
                            break;
                        case 9:
                            tgh.SepThisYr = value;
                            break;
                        case 10:
                            tgh.OctThisYr = value;
                            break;
                        case 11:
                            tgh.NovThisYr = value;
                            break;
                        case 12:
                            tgh.DecThisYr = value;
                            break;
                    }
                }

                #region Computation of days without Saturday

                SelectedData result2 = session.ExecuteSproc("GetWorkDaysFor13thMonth_NoSat",
                new OperandValue(startDate),
                new OperandValue(endDate));

                SelectStatementResultRow[] rows2 = result2.ResultSet[0].Rows;

                int numMonths2 = Math.Min(rows2.Length, 13); // Ensure you don't go out of bounds

                tgh.TotalWithoutSat = 0;

                for (int i = 0; i < numMonths2; i++)
                {
                    int value2 = Convert.ToInt32(rows2[i].Values[1]);

                    tgh.TotalWithoutSat = tgh.TotalWithoutSat + value2;
                }

                #endregion

                List<TempThirteenDaysHolder> employeeDataList = new List<TempThirteenDaysHolder>();
                foreach (CheckInAndOut03 item in entries)
                {
                    index++;

                    #region Algorithms here

                    decimal dec1 = 0, jan = 0, feb = 0, mar = 0, apr = 0, may = 0, jun = 0, jul = 0, aug = 0, sep = 0, oct = 0, nov = 0, dec2 = 0;
                    decimal val = 0;
                    decimal sat = 0;
                    decimal bsc = 0m;
                    if (!new[] { "Rest Day", "Absent", "Halfday" }.Any(o => item.Remarks.Contains(o)))
                    {
                        val = 1;
                    }
                    else if (item.Remarks == "Halfday")
                    {
                        val = 0.5m;
                    }

                    // TODO Implementation: Check if item.Date falls on a Saturday
                    if (item.Date.DayOfWeek == DayOfWeek.Saturday)
                    {
                        if (!new[] { "Rest Day", "Absent", "Halfday" }.Any(o => item.Remarks.Contains(o)))
                        {
                            sat = 1;
                        }
                        else if (item.Remarks == "Halfday")
                        {
                            sat = 0.5m;
                        }
                    }

                    if (item.AttRecId != null)
                    {
                        bsc = item.AttRecId.Basic;
                    }
                    switch (item.Date.Month)
                    {
                        case 1:
                            jan = val;
                            break;
                        case 2:
                            feb = val;
                            break;
                        case 3:
                            mar = val;
                            break;
                        case 4:
                            apr = val;
                            break;
                        case 5:
                            may = val;
                            break;
                        case 6:
                            jun = val;
                            break;
                        case 7:
                            jul = val;
                            break;
                        case 8:
                            aug = val;
                            break;
                        case 9:
                            sep = val;
                            break;
                        case 10:
                            oct = val;
                            break;
                        case 11:
                            nov = val;
                            break;
                        case 12:
                            if (item.Date.Year == lastYr)
                            {
                                dec1 = val;
                            }
                            else
                            {
                                dec2 = val;
                            }
                            break;
                        default:
                            break;
                    }
                    
                    var existingEmployee = employeeDataList.FirstOrDefault(emp => emp.EmployeeId == item.EmployeeId.Oid);

                    if (existingEmployee != null)
                    {
                        existingEmployee.Dea += dec1;
                        existingEmployee.Jan += jan;
                        existingEmployee.Feb += feb;
                        existingEmployee.Mar += mar;
                        existingEmployee.Apr += apr;
                        existingEmployee.May += may;
                        existingEmployee.Jun += jun;
                        existingEmployee.Jul += jul;
                        existingEmployee.Aug += aug;
                        existingEmployee.Sep += sep;
                        existingEmployee.Oct += oct;
                        existingEmployee.Nov += nov;
                        existingEmployee.Deb += dec2;
                        existingEmployee.Basic = bsc;
                        existingEmployee.Sat += sat;
                    }
                    else
                    {
                        employeeDataList.Add(new TempThirteenDaysHolder
                        {
                            EmployeeId = item.EmployeeId.Oid,
                            EmployeeName = item.Employee,
                            EnrolledNo = item.EmployeeId.EnrollNumber,
                            Dea = dec1,
                            Jan = jan,
                            Feb = feb,
                            Mar = mar,
                            Apr = apr,
                            May = may,
                            Jun = jun,
                            Jul = jul,
                            Aug = aug,
                            Sep = sep,
                            Oct = oct,
                            Nov = nov,
                            Deb = dec2,
                            Sat = sat,
                            Basic = bsc
                        });
                    }

                    #endregion

                    _message = string.Format("Generating entry {0} succesfull.",
                    index);
                    _BgWorker.ReportProgress(1, _message);

                    if (_BgWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        session.Dispose();
                        break;
                    }
                }

                foreach (var item in employeeDataList)
                {
                    var detail = ReflectionHelper.CreateObject<ThirteenthGeneratorDetail>(session);
                    //private ThirteenthGeneratorHeader _ParentID;
                    detail.ParentID = tgh;
                    //private BusinessObjects.Employee _Employee;
                    detail.Employee = session.GetObjectByKey<Employee>(item.EmployeeId);
                    detail.DecLastYr = item.Dea;
                    detail.JanThisYr = item.Jan;
                    detail.FebThisYr = item.Feb;
                    detail.MarThisYr = item.Mar;
                    detail.AprThisYr = item.Apr;
                    detail.MayThisYr = item.May;
                    detail.JunThisYr = item.Jun;
                    detail.JulThisYr = item.Jul;
                    detail.AugThisYr = item.Aug;
                    detail.SepThisYr = item.Sep;
                    detail.OctThisYr = item.Oct;
                    detail.NovThisYr = item.Nov;
                    detail.DecThisYr = item.Deb;

                    // --- IMPLEMENTATION START ---
                    // Logic: If tgh.TotalWithoutSat - detail.Total > 0, set Absences to result, else 0.

                    // Assuming 'TotalWithoutSat' and 'Total' are compatible numeric types (decimal or double)
                    var absenceDifference = tgh.TotalWithoutSat - detail.Total;

                    // Using a Ternary operator for cleaner syntax
                    detail.Absences = absenceDifference > 0 ? absenceDifference : 0;
                    // --- IMPLEMENTATION END ---

                    detail.SatDuties = item.Sat;
                    detail.Basic = item.Basic;
                    detail.ThirteenthMonthPay = detail.AttendanceBasedPay;
                    detail.Save();
                }
            }
            finally
            {
                if (index == entries.Count)
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

    public class TempThirteenDaysHolder
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EnrolledNo { get; set; }
        public decimal Dea { get; set; }
        public decimal Jan { get; set; }
        public decimal Feb { get; set; }
        public decimal Mar { get; set; }
        public decimal Apr { get; set; }
        public decimal May { get; set; }
        public decimal Jun { get; set; }
        public decimal Jul { get; set; }
        public decimal Aug { get; set; }
        public decimal Sep { get; set; }
        public decimal Oct { get; set; }
        public decimal Nov { get; set; }
        public decimal Deb { get; set; }
        public decimal Basic { get; set; }
        public decimal Sat { get; set; }
    }
}
