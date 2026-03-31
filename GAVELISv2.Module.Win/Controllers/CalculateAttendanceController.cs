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
    public partial class CalculateAttendanceController : ViewController
    {
        private SimpleAction calculateAttendanceAction;
        private AttendanceCalculator _AttendanceCalculator;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public CalculateAttendanceController()
        {
            this.TargetObjectType = typeof(AttendanceCalculator);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "CalculateAttendanceActionId";
            this.calculateAttendanceAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.calculateAttendanceAction.Caption = "Calculate";
            this.calculateAttendanceAction.Execute += new SimpleActionExecuteEventHandler(calculateAttendanceAction_Execute);
        }

        void calculateAttendanceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _AttendanceCalculator = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as AttendanceCalculator;
            XPCollection<Employee> emps = new XPCollection<Employee>(((ObjectSpace)ObjectSpace).Session);
            //var empsList = emps.Where(o => o.StaffPayroll == true && o.EnrollNumber== "10028").OrderBy(o => o.Name);
            var empsList = emps.Where(o => o.StaffPayroll == true && o.Inactive == false).OrderBy(o => o.Name);
            if (empsList.Count() == 0)
            {
                XtraMessageBox.Show("There are no employees for staff payroll to process.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Calculating...", empsList.Count(),
                        "Employees processed {0} of {1} ");
            _FrmProgress.CancelClick += FrmProgressCancelClick;
            _BgWorker = new System.ComponentModel.BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = true
            };
            _BgWorker.RunWorkerCompleted += BgWorkerRunWorkerCompleted;
            _BgWorker.ProgressChanged += BgWorkerProgressChanged;
            _BgWorker.DoWork += BgWorkerDoWork;
            _BgWorker.RunWorkerAsync(empsList);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IOrderedEnumerable<Employee> trans = (IOrderedEnumerable<Employee>)e.Argument;
            XPCollection<DeviceAttendanceLog> devAtts = new XPCollection<DeviceAttendanceLog>(session);
            try
            {
                foreach (Employee item in trans)
                {
                    index++;
                    _message = string.Format("Processing employee {0} succesfull.",
                    item.Name);
                    _BgWorker.ReportProgress(1, _message);

                    Employee emp = session.GetObjectByKey<Employee>(item.Oid);
                    AttendanceCalculator att = session.GetObjectByKey<AttendanceCalculator>(_AttendanceCalculator.Oid);
                    #region Algorithms here
                    DateTime startDate = _AttendanceCalculator.TimeRangeFrom;
                    DateTime endDate = _AttendanceCalculator.TimeRangeTo.AddDays(1);
                    do
                    {
                        //XPCollection<TimeTable> ttbls = new XPCollection<TimeTable>(session);
                        //if (ttbls.Count() == 0)
                        //{
                        //    throw new ApplicationException("There are no shift timetable setup");
                        //}
                        ArrayList chiid = new ArrayList();
                        ArrayList choid = new ArrayList();
                        foreach (ShiftEmployee ttbl in item.ShiftEmployees)
                        {
                            TimeTable ottbl = session.GetObjectByKey<TimeTable>(ttbl.Shift.Oid);
                            CheckInAndOut02 chkIO = null;
                            string strId; // "10028 2016-05-24 Morning";
                            if (ottbl.NextDay) strId = string.Format("{0} {1} {2}", emp.EnrollNumber, startDate.Date.AddDays(1).ToString("MM-dd-yyyy"),
                                ottbl.TimeTableName);
                            else strId = string.Format("{0} {1} {2}", emp.EnrollNumber, startDate.Date.ToString("MM-dd-yyyy"),
          ottbl.TimeTableName);
                            chkIO = session.FindObject<CheckInAndOut02>(BinaryOperator.Parse("[LineID]=?", strId));
                            if (chkIO == null)
                            {
                                chkIO = ReflectionHelper.CreateObject<CheckInAndOut02>(session);
                            }
                            chkIO.LineID = strId;
                            chkIO.AttCalcId = att;
                            chkIO.EnrolledNo = emp.EnrollNumber;
                            chkIO.EmployeeId = emp;
                            chkIO.AutoAssigned = false;
                            if (ottbl.NextDay) chkIO.Date = startDate.Date.AddDays(1); else chkIO.Date = startDate.Date;
                            chkIO.Day = chkIO.Date.DayOfWeek;
                            chkIO.TimeTable = ottbl;
                            chkIO.OnDuty = ottbl.OnDutyTime;
                            chkIO.OffDuty = ottbl.OffDutyTime;
                            chkIO.ClockIn = new DateTime(1753, 1, 1, 0, 0, 0);
                            chkIO.ClockOut = new DateTime(1753, 1, 1, 0, 0, 0);
                            chkIO.RefID = 0;
                            chkIO.RefID02 = 0;
                            // ....
                            chkIO.ActualLate = new TimeSpan(0, 0, 0, 0);
                            chkIO.Late = new TimeSpan(0, 0, 0, 0);
                            chkIO.ActualEarly = new TimeSpan(0, 0, 0, 0);
                            chkIO.Early = new TimeSpan(0, 0, 0, 0);
                            chkIO.Absent = false;
                            chkIO.AbsentCount = 0m;
                            chkIO.AbsentHours = new TimeSpan(0, 0, 0, 0);
                            chkIO.OtHours = new TimeSpan(0, 0, 0, 0);
                            chkIO.ValidOtHours = new TimeSpan(0, 0, 0, 0);
                            chkIO.ActualHours = new TimeSpan(0, 0, 0, 0);
                            chkIO.ValidWorkHours = new TimeSpan(0, 0, 0, 0);
                            chkIO.Remarks = string.Empty;
                            //chkIO.OtStatus = OtStatusEnum.None;
                            chkIO.RestDay = new TimeSpan(0, 0, 0, 0);
                            chkIO.Holiday = new TimeSpan(0, 0, 0, 0);
                            chkIO.Night = new TimeSpan(0, 0, 0, 0);

                            chkIO.Flexible = emp.Flexible;
                            // ....
                            //var devAttList = devAtts.Where(o => o.EnrolledNo == item.EnrollNumber && o.LogTime.Date == startDate.Date).OrderBy(o => o.InOutMode);
                            var devAttList = devAtts.Where(o => o.EnrolledNo == item.EnrollNumber && o.LogTime.Date == chkIO.Date).OrderBy(o => o.InOutMode);
                            foreach (DeviceAttendanceLog datl in devAttList)
                            {
                                if (datl.Oid==1341)
                                {
                                    Console.WriteLine("Trap");
                                }
                                //if (ottbl.NextDay)
                                //{
                                //    ottbl.BeginningIn = ottbl.BeginningIn.AddDays(1);
                                //    ottbl.BeginningOut = ottbl.BeginningOut.AddDays(1);
                                //    ottbl.EndingIn = ottbl.EndingIn.AddDays(1);
                                //    ottbl.EndingOut = ottbl.EndingOut.AddDays(1);
                                //}
                                // Search for CheckIn
                                if (datl.InOutMode == InOutModeEnum.CheckIn)
                                {
                                    // Between BeginningIn & EndingIn range
                                    if (datl.LogTime.TimeOfDay >= ottbl.BeginningIn.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.EndingIn.TimeOfDay)
                                    {
                                        // ClockIn
                                        if (chiid.Contains(datl.Oid))
                                        {
                                            continue;
                                        }
                                        chkIO.ClockIn = datl.LogTime;
                                        chkIO.RefID = datl.Oid;
                                        chiid.Add(datl.Oid);
                                    }
                                }
                                // Search for BreakOut
                                if (datl.InOutMode == InOutModeEnum.BreakOut)
                                {
                                    // Between BeginningOut & EndingOut range
                                    if (datl.LogTime.TimeOfDay >= ottbl.BeginningOut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.EndingOut.TimeOfDay)
                                    {
                                        // ClockOut
                                        if (choid.Contains(datl.Oid))
                                        {
                                            continue;
                                        }
                                        chkIO.ClockOut = datl.LogTime;
                                        chkIO.RefID02 = datl.Oid;
                                        choid.Add(datl.Oid);
                                    }
                                }
                                // Search for BreakIn
                                if (datl.InOutMode == InOutModeEnum.BreakIn)
                                {
                                    // Between BeginningIn & EndingIn
                                    if (datl.LogTime.TimeOfDay >= ottbl.BeginningIn.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.EndingIn.TimeOfDay)
                                    {
                                        // ClockIn
                                        if (chiid.Contains(datl.Oid))
                                        {
                                            continue;
                                        }
                                        chkIO.ClockIn = datl.LogTime;
                                        chkIO.RefID = datl.Oid;
                                        chiid.Add(datl.Oid);
                                    }
                                }
                                // Search for CheckOut
                                if (datl.InOutMode == InOutModeEnum.CheckOut)
                                {
                                    // Between BeginningOut & EndingOut range
                                    //bool res1 = datl.LogTime.TimeOfDay >= ottbl.BeginningOut.TimeOfDay;
                                    //bool res2 = datl.LogTime.TimeOfDay <= ottbl.EndingOut.TimeOfDay;
                                    if (datl.LogTime.TimeOfDay >= ottbl.BeginningOut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.EndingOut.TimeOfDay)
                                    {
                                        // ClockOut
                                        if (choid.Contains(datl.Oid))
                                        {
                                            continue;
                                        }
                                        chkIO.ClockOut = datl.LogTime;
                                        chkIO.RefID02 = datl.Oid;
                                        choid.Add(datl.Oid);
                                    }
                                }
                                // Search for OvertimeIn
                                if (datl.InOutMode == InOutModeEnum.OvertimeIn)
                                {
                                    // Between BeginningIn & EndingIn
                                    if (datl.LogTime.TimeOfDay >= ottbl.BeginningIn.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.EndingIn.TimeOfDay)
                                    {
                                        // ClockIn
                                        if (chiid.Contains(datl.Oid))
                                        {
                                            continue;
                                        }
                                        chkIO.ClockIn = datl.LogTime;
                                        chkIO.RefID = datl.Oid;
                                        chiid.Add(datl.Oid);
                                    }
                                }
                                // Search for OvertimeOut
                                if (datl.InOutMode == InOutModeEnum.OvertimeOut)
                                {
                                    // Between BeginningOut & EndingOut range
                                    if (datl.LogTime.TimeOfDay >= ottbl.BeginningOut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.EndingOut.TimeOfDay)
                                    {
                                        // ClockOut
                                        if (choid.Contains(datl.Oid))
                                        {
                                            continue;
                                        }
                                        chkIO.ClockOut = datl.LogTime;
                                        chkIO.RefID02 = datl.Oid;
                                        choid.Add(datl.Oid);
                                    }
                                }
                            }

                            Holiday hol = session.FindObject<Holiday>(BinaryOperator.Parse("[Date]=?", startDate.Date.ToShortDateString()));
                            if (startDate.DayOfWeek == emp.RestDay)
                            {
                                chkIO.Normal = 0m;
                                chkIO.NormalHours = new TimeSpan(0, 0, 0);
                                chkIO.RealTime = chkIO.Normal;
                            }
                            else if (startDate.DayOfWeek != emp.RestDay)
                            {
                                chkIO.Normal = ottbl.CountAsWorkday;
                                chkIO.NormalHours = ottbl.OffDutyTime - ottbl.OnDutyTime;
                                if (chkIO.NormalHours < TimeSpan.Zero)
                                {
                                    chkIO.NormalHours = ottbl.OffDutyTime.AddDays(1) - ottbl.OnDutyTime;
                                }
                                chkIO.RealTime = chkIO.Normal;
                            }
                            #region Process Shift Class
                            // if employee.shifting != timetable.shifting
                            // Normal = 0
                            // Realtime = 0
                            // NormalHours = 0
                            if (item.Shifting != ottbl.ShiftClass)
                            {
                                chkIO.Normal = 0m;
                                chkIO.RealTime = 0m;
                                chkIO.NormalHours = new TimeSpan(0, 0, 0);
                            }
                            #endregion
                            #region Proces Late and Early
                            // Late
                            if (emp.Flexible != true && chkIO.Normal > 0m && chkIO.ClockIn != new DateTime(1753, 1, 1, 0, 0, 0))
                            {
                                chkIO.ActualLate = new TimeSpan(chkIO.ClockIn.Hour, chkIO.ClockIn.Minute, 0) - new TimeSpan(chkIO.OnDuty.Hour, chkIO.OnDuty.Minute, 0);
                                if (chkIO.ActualLate <= new TimeSpan())
                                {
                                    chkIO.ActualLate = new TimeSpan();
                                }
                            }
                            else
                            {
                                chkIO.ActualLate = new TimeSpan();
                            }
                            if (chkIO.ActualLate != new TimeSpan())
                            {
                                if (emp.Flexible != true && chkIO.ActualLate > new TimeSpan(0, ottbl.LateTimeMins, 0))
                                {
                                    chkIO.Late = chkIO.ActualLate;
                                }
                                else
                                {
                                    chkIO.Late = new TimeSpan();
                                }
                            }

                            // Early Out
                            if (emp.Flexible != true && chkIO.Normal > 0m && chkIO.ClockOut != new DateTime(1753, 1, 1, 0, 0, 0))
                            {
                                chkIO.ActualEarly = new TimeSpan(chkIO.OffDuty.Hour, chkIO.OffDuty.Minute, 0) - new TimeSpan(chkIO.ClockOut.Hour, chkIO.ClockOut.Minute, 0);
                                if (chkIO.ActualEarly <= new TimeSpan())
                                {
                                    chkIO.ActualEarly = new TimeSpan();
                                    chkIO.Early = new TimeSpan();
                                }
                            }
                            else
                            {
                                chkIO.ActualEarly = new TimeSpan();
                                chkIO.Early = new TimeSpan();
                            }
                            if (emp.Flexible != true && chkIO.ActualEarly != new TimeSpan())
                            {
                                // ....
                                if (chkIO.ActualEarly >= new TimeSpan(0, ottbl.LeaveEarlyTimeMins, 0))
                                {
                                    chkIO.Early = chkIO.ActualEarly;
                                }
                                else
                                {
                                    chkIO.Early = new TimeSpan();
                                }
                            }
                            #endregion
                            #region Actual Hours
                            // Actual Hours
                            // If ClockIn != Empty && ClockOut == Empty
                            if (chkIO.ClockIn != new DateTime(1753, 1, 1, 0, 0, 0) && chkIO.ClockOut == new DateTime(1753, 1, 1, 0, 0, 0))
                            {
                                chkIO.ActualHours = new TimeSpan(chkIO.OffDuty.Hour, chkIO.OffDuty.Minute, chkIO.OffDuty.Second) - new TimeSpan(chkIO.ClockIn.Hour, chkIO.ClockIn.Minute, chkIO.ClockIn.Second);
                            }
                            // If ClockIn == Empty && ClockOut != Empty
                            else if (chkIO.ClockIn == new DateTime(1753, 1, 1, 0, 0, 0) && chkIO.ClockOut != new DateTime(1753, 1, 1, 0, 0, 0))
                            {
                                chkIO.ActualHours = new TimeSpan(chkIO.ClockOut.Hour, chkIO.ClockOut.Minute, chkIO.ClockOut.Second) - new TimeSpan(chkIO.OnDuty.Hour, chkIO.OnDuty.Minute, chkIO.OnDuty.Second);
                            }
                            // If ClockIn != Empty && ClockOut != Empty
                            else if (chkIO.ClockIn != new DateTime(1753, 1, 1, 0, 0, 0) && chkIO.ClockOut != new DateTime(1753, 1, 1, 0, 0, 0))
                            {
                                chkIO.ActualHours = new TimeSpan(chkIO.ClockOut.Hour, chkIO.ClockOut.Minute, chkIO.ClockOut.Second) - new TimeSpan(chkIO.ClockIn.Hour, chkIO.ClockIn.Minute, chkIO.ClockIn.Second);
                            }
                            else
                            {
                                chkIO.ActualHours = new TimeSpan();
                            }
                            #endregion
                            #region Process Absent
                            if (chkIO.Normal != 0m && chkIO.ActualHours == new TimeSpan(0, 0, 0, 0))
                            {
                                if (hol != null)
                                {
                                    if (hol.HolidayType == HolidayTypeEnum.Special)
                                    {
                                        chkIO.Absent = true;
                                        chkIO.AbsentCount = ottbl.CountAsWorkday;
                                        chkIO.AbsentHours = chkIO.NormalHours;
                                    }
                                    else
                                    {
                                        chkIO.Absent = false;
                                        chkIO.AbsentCount = 0m;
                                        chkIO.AbsentHours = new TimeSpan(0, 0, 0, 0);
                                    }
                                }
                                else
                                {
                                    chkIO.Absent = true;
                                    chkIO.AbsentCount = ottbl.CountAsWorkday;
                                    chkIO.AbsentHours = chkIO.NormalHours;
                                }
                            }
                            else if (chkIO.Normal != 0m && chkIO.ActualHours == new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.Absent = false;
                                chkIO.AbsentCount = ottbl.CountAsWorkday;
                                chkIO.AbsentHours = chkIO.NormalHours;
                            }
                            else if (chkIO.Normal != 0m && chkIO.ActualHours > new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.Absent = false;
                                chkIO.AbsentCount = 0m;
                                chkIO.AbsentHours = new TimeSpan(0, 0, 0, 0);
                                // OT Hours = ActualHours - NormalHours
                                TimeSpan tmpOth = chkIO.ActualHours - chkIO.NormalHours;
                                if (ottbl.NoOvertime != true && tmpOth > chkIO.CompanyInfo.MinOvertime)
                                {
                                    chkIO.OtHours = tmpOth;
                                    // Default overtime-in time not implemented
                                    chkIO.ValidOtHours = chkIO.OtHours;
                                }
                                else
                                {
                                    chkIO.OtHours = new TimeSpan(0, 0, 0, 0);
                                    chkIO.ValidOtHours = chkIO.OtHours;
                                }
                            }
                            else if (hol != null)
                            {
                                chkIO.Absent = false;
                                chkIO.AbsentCount = 0m;
                                chkIO.AbsentHours = new TimeSpan(0, 0, 0, 0);
                                //chkIO.ValidWorkHours = chkIO.NormalHours;
                            }
                            #endregion

                            // Valid Work Hours
                            // Late Only
                            if (chkIO.Late != new TimeSpan(0, 0, 0, 0) && chkIO.Early == new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.ValidWorkHours = chkIO.NormalHours - chkIO.Late;
                            }
                            // Early Only
                            if (chkIO.Late == new TimeSpan(0, 0, 0, 0) && chkIO.Early != new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.ValidWorkHours = chkIO.NormalHours - chkIO.Early;
                            }
                            // Both Late and Early
                            if (chkIO.Late != new TimeSpan(0, 0, 0, 0) && chkIO.Early != new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.ValidWorkHours = (chkIO.NormalHours - chkIO.Late) - chkIO.Early;
                            }
                            // Both None
                            if (chkIO.Late == new TimeSpan(0, 0, 0, 0) && chkIO.Early == new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.ValidWorkHours = chkIO.NormalHours;
                            }
                            // But Absent
                            if (chkIO.Absent)
                            {
                                chkIO.ValidWorkHours = new TimeSpan(0, 0, 0, 0);
                            }
                            // Remarks
                            if (hol == null && chkIO.ValidWorkHours < chkIO.NormalHours)
                            {
                                if (chkIO.Late > new TimeSpan(0, 0, 0, 0) && chkIO.Early == new TimeSpan(0, 0, 0, 0))
                                {
                                    chkIO.Remarks = "Late";
                                    chkIO.OtStatus = OtStatusEnum.None;
                                }
                                else if (chkIO.Late == new TimeSpan(0, 0, 0, 0) && chkIO.Early > new TimeSpan(0, 0, 0, 0))
                                {
                                    chkIO.Remarks = "Early";
                                    chkIO.OtStatus = OtStatusEnum.None;
                                }
                                else if (chkIO.Late > new TimeSpan(0, 0, 0, 0) && chkIO.Early > new TimeSpan(0, 0, 0, 0))
                                {
                                    chkIO.Remarks = "Late & Early";
                                    chkIO.OtStatus = OtStatusEnum.None;
                                }
                            }
                            if (hol == null && chkIO.ValidWorkHours == chkIO.NormalHours)
                            {
                                if (chkIO.ActualHours >= chkIO.ValidWorkHours)
                                {
                                    chkIO.Remarks = "Full";
                                    if (!new[] { "Pending", "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)))
                                    {
                                        chkIO.OtStatus = OtStatusEnum.None;
                                    }
                                }
                                if (chkIO.ActualHours < chkIO.ValidWorkHours)
                                {
                                    chkIO.Remarks = "Partial";
                                    chkIO.OtStatus = OtStatusEnum.None;
                                }
                            }
                            if (hol == null && chkIO.ValidWorkHours == new TimeSpan(0, 0, 0, 0))
                            {
                                if (chkIO.Absent)
                                {
                                    chkIO.Remarks = "Absent";
                                    chkIO.OtStatus = OtStatusEnum.None;
                                }
                                if (emp.RestDay == chkIO.Day)
                                {
                                    if (chkIO.ActualHours != new TimeSpan(0, 0, 0, 0))
                                    {
                                        chkIO.Remarks = "Rest Day OT";
                                        if (!new[] { "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)))
                                        {
                                            chkIO.OtStatus = OtStatusEnum.Pending;
                                        }
                                        chkIO.RestDay = chkIO.ActualHours;
                                    }
                                    else
                                    {
                                        chkIO.Remarks = "Rest Day";
                                        chkIO.OtStatus = OtStatusEnum.None;
                                        chkIO.RestDay = new TimeSpan(0, 0, 0, 0);
                                    }
                                }
                            }
                            if (ottbl.NoOvertime != true && hol == null && chkIO.ActualHours > chkIO.NormalHours && chkIO.ValidOtHours != new TimeSpan(0, 0, 0, 0))
                            {
                                chkIO.Remarks = "Overtime";
                                if (!new[] { "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)))
                                {
                                    chkIO.OtStatus = OtStatusEnum.Pending;
                                }
                            }
                            if (hol != null)
                            {
                                if (chkIO.ActualHours > new TimeSpan(0, 0, 0, 0))
                                {
                                    chkIO.Holiday = chkIO.ValidWorkHours + chkIO.ValidOtHours;
                                    switch (hol.HolidayType)
                                    {
                                        case HolidayTypeEnum.Special:
                                            chkIO.Remarks = chkIO.ValidOtHours > new TimeSpan(0, 0, 0, 0) ? "Holiday SPOT" : "Holiday SP";
                                            chkIO.OtStatus = chkIO.ValidOtHours > new TimeSpan(0, 0, 0, 0) ? chkIO.OtStatus : OtStatusEnum.None;
                                            break;
                                        case HolidayTypeEnum.Regular:
                                            chkIO.Remarks = "Holiday RGOT";
                                            chkIO.OtStatus = new[] { "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)) != true ? OtStatusEnum.Pending : chkIO.OtStatus;
                                            break;
                                        case HolidayTypeEnum.None:
                                            break;
                                        default:
                                            break;
                                    }
                                    if (chkIO.ValidOtHours > new TimeSpan(0, 0, 0, 0) && !new[] { "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)))
                                    {
                                        //chkIO.OtStatus = OtStatusEnum.Pending;
                                        switch (hol.HolidayType)
                                        {
                                            case HolidayTypeEnum.Special:
                                                chkIO.Remarks = "Holiday SPOT";
                                                chkIO.OtStatus = new[] { "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)) != true ? OtStatusEnum.Pending : chkIO.OtStatus;
                                                break;
                                            case HolidayTypeEnum.Regular:
                                                chkIO.Remarks = "Holiday RGOT";
                                                chkIO.OtStatus = new[] { "Approved", "Disapproved" }.Any(o => chkIO.OtStatus.ToString().Contains(o)) != true ? OtStatusEnum.Pending : chkIO.OtStatus;
                                                break;
                                            case HolidayTypeEnum.None:
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    if (hol.HolidayType == HolidayTypeEnum.Special)
                                    {
                                        chkIO.Remarks = "HSP Absent";
                                        if (item.Shifting != ottbl.ShiftClass)
                                        {
                                            chkIO.Remarks = "Full";
                                        }
                                    }
                                    else
                                    {
                                        chkIO.Remarks = "Holiday RG";
                                    }
                                    chkIO.OtStatus = OtStatusEnum.None;
                                    chkIO.Holiday = new TimeSpan(0, 0, 0, 0);
                                }
                            }
                            // Night Diff
                            if (chkIO.LineID == "10076 09-21-2016 Night Shift (AM)")
                            {

                            }
                            chkIO.Night = new TimeSpan(0, 0, 0, 0);
                            //if (item.Shifting == ShiftClassEnum.Dayshift)
                            //{
                            //    // Start Time
                            //    if (chkIO.Absent != true && chkIO.ActualHours != new TimeSpan(0, 0, 0, 0) && ottbl.DefaultMode == InOutModeEnum.CheckOut && chkIO.Remarks != "Rest Day" && chkIO.ClockOut.TimeOfDay >= chkIO.CompanyInfo.NightStartTime.TimeOfDay) // && !new[] { "Rest Day", "Holiday" }.Any(o => chkIO.Remarks.Contains(o)) && chkIO.ClockOut.TimeOfDay >= chkIO.CompanyInfo.NightStartTime.TimeOfDay)
                            //    {
                            //        chkIO.Night = chkIO.ClockOut.TimeOfDay - chkIO.CompanyInfo.NightStartTime.TimeOfDay;
                            //    }
                            //    // End Time
                            //    if (chkIO.Absent != true && chkIO.ActualHours != new TimeSpan(0, 0, 0, 0) && ottbl.DefaultMode == InOutModeEnum.CheckIn && chkIO.Remarks != "Rest Day" && chkIO.CompanyInfo.NightEndTime.TimeOfDay >= chkIO.ClockIn.TimeOfDay) // && !new[] { "Rest Day", "Holiday" }.Any(o => chkIO.Remarks.Contains(o)) && chkIO.CompanyInfo.NightEndTime.TimeOfDay >= chkIO.ClockIn.TimeOfDay)
                            //    {
                            //        chkIO.Night = chkIO.CompanyInfo.NightEndTime.TimeOfDay - chkIO.ClockIn.TimeOfDay;
                            //    }
                            //}
                            //if (item.Shifting == ShiftClassEnum.Nightshift)
                            //{
                            //    // Start Time
                            //    if (chkIO.Absent != true && chkIO.ActualHours != new TimeSpan(0, 0, 0, 0) && ottbl.DefaultMode == InOutModeEnum.CheckOut && chkIO.Remarks != "Rest Day" && chkIO.ClockIn.TimeOfDay >= chkIO.CompanyInfo.NightStartTime.TimeOfDay) // && !new[] { "Rest Day", "Holiday" }.Any(o => chkIO.Remarks.Contains(o)) && chkIO.ClockOut.TimeOfDay >= chkIO.CompanyInfo.NightStartTime.TimeOfDay)
                            //    {
                            //        chkIO.Night = chkIO.ClockOut.TimeOfDay - chkIO.CompanyInfo.NightStartTime.TimeOfDay;
                            //    }
                            //    // End Time
                            //    if (chkIO.Absent != true && chkIO.ActualHours != new TimeSpan(0, 0, 0, 0) && ottbl.DefaultMode == InOutModeEnum.CheckIn && chkIO.Remarks != "Rest Day" && chkIO.CompanyInfo.NightEndTime.TimeOfDay >= chkIO.ClockOut.TimeOfDay) // && !new[] { "Rest Day", "Holiday" }.Any(o => chkIO.Remarks.Contains(o)) && chkIO.CompanyInfo.NightEndTime.TimeOfDay >= chkIO.ClockIn.TimeOfDay)
                            //    {
                            //        chkIO.Night = chkIO.CompanyInfo.NightEndTime.TimeOfDay - chkIO.ClockIn.TimeOfDay;
                            //    }
                            //}
                            TimeSpan nightHrs = ottbl.NightEndTime - ottbl.NightStartTime;
                            if (nightHrs > TimeSpan.Zero && chkIO.ActualHours != new TimeSpan(0, 0, 0, 0))
                            {
                                TimeSpan nFirst = chkIO.ClockIn.TimeOfDay - ottbl.NightStartTime.TimeOfDay;
                                if (nFirst < TimeSpan.Zero)
                                {
                                    nFirst = ottbl.NightStartTime.TimeOfDay;
                                }
                                else
                                {
                                    nFirst = chkIO.ClockIn.TimeOfDay;
                                }
                                TimeSpan nSecond = ottbl.NightEndTime.TimeOfDay - chkIO.ClockOut.TimeOfDay;
                                if (nSecond == ottbl.NightEndTime.TimeOfDay || nSecond < TimeSpan.Zero)
                                {
                                    nSecond = ottbl.NightEndTime.TimeOfDay;
                                }
                                else if (nSecond > ottbl.NightEndTime.TimeOfDay)
                                {
                                    nSecond = chkIO.ClockOut.TimeOfDay;
                                }
                                else
                                {
                                    nSecond = chkIO.ClockOut.TimeOfDay;
                                }
                                chkIO.Night = nSecond - nFirst;
                                if (chkIO.Night < TimeSpan.Zero)
                                {
                                    chkIO.Night = TimeSpan.Zero;
                                }
                            }
                            if (chkIO.Remarks == "Holiday RG")
                            {
                                chkIO.Night = new TimeSpan();
                            }
                            #region Line Pay Calculation
                            if (emp.No == "E00077")
                            {
                                Console.WriteLine("Trap");
                            }
                            chkIO.BasicHrs = 0m;
                            chkIO.BasicAmt = 0m;
                            chkIO.AbsentHrs = 0m;
                            chkIO.AbsentAmt = 0m;
                            chkIO.LateHrs = 0m;
                            chkIO.LateAmt = 0m;
                            chkIO.UndertimeHrs = 0m;
                            chkIO.UndertimeAmt = 0m;
                            chkIO.RestdayOtHrs = 0m;
                            chkIO.RestdayOtAmt = 0m;
                            chkIO.OvertimeHrs = 0m;
                            chkIO.OvertimeAmt = 0m;
                            chkIO.NightDiffHrs = 0m;
                            chkIO.NightDiffAmt = 0m;
                            chkIO.HolidayHrs = 0m;
                            chkIO.HolidayOTHrs = 0m;
                            chkIO.HolidayOTAmt = 0m;
                            chkIO.HolidayType = HolidayTypeEnum.None;
                            chkIO.HolidayAmt = 0m;
                            chkIO.HolidayHrs2 = 0m;
                            chkIO.HolidayAmt2 = 0m;
                            chkIO.HolidayOTHrs2 = 0m;
                            chkIO.HolidayOTAmt2 = 0m;
                            EmployeePayTypeEnum payType = emp.PayType;
                            decimal basic = emp.Basic;
                            decimal hourRate = 0m;
                            switch (payType)
                            {
                                case EmployeePayTypeEnum.Hourly:
                                    break;
                                case EmployeePayTypeEnum.Daily:
                                    // Basic Pay
                                    chkIO.BasicHrs = (decimal)chkIO.NormalHours.TotalHours;
                                    if (basic != 0m && chkIO.BasicHrs != 0)
                                    {
                                        hourRate = (basic * ottbl.CountAsWorkday) / chkIO.BasicHrs;
                                    }
                                    chkIO.BasicAmt = chkIO.BasicHrs * hourRate;
                                    // Absent
                                    if (chkIO.Absent)
                                    {
                                        chkIO.AbsentHrs = (decimal)chkIO.NormalHours.TotalHours;
                                        chkIO.AbsentAmt = chkIO.AbsentHrs * hourRate;
                                    }
                                    // Late
                                    chkIO.LateHrs = (decimal)chkIO.Late.TotalHours;
                                    chkIO.LateAmt = hourRate * chkIO.LateHrs;
                                    // Undertime
                                    chkIO.UndertimeHrs = (decimal)chkIO.Early.TotalHours;
                                    chkIO.UndertimeAmt = hourRate * chkIO.UndertimeHrs;
                                    // Restday OT
                                    if (chkIO.Remarks == "Rest Day OT" && chkIO.OtStatus == OtStatusEnum.Approved)
                                    {
                                        TimeSpan restHrs = ottbl.OffDutyTime - ottbl.OnDutyTime;
                                        chkIO.RestdayOtHrs = (decimal)chkIO.ActualHours.TotalHours;
                                        hourRate = (basic * chkIO.CompanyInfo.RestDayRate / 100) * ottbl.CountAsWorkday / (decimal)restHrs.TotalHours;
                                        //hourRate = (basic / (decimal)restHrs.TotalHours) * chkIO.CompanyInfo.RestDayRate / 100;
                                        chkIO.RestdayOtAmt = chkIO.RestdayOtHrs * hourRate;
                                    }
                                    // Overtime
                                    if (chkIO.Remarks == "Overtime" && chkIO.OtStatus == OtStatusEnum.Approved)
                                    {
                                        chkIO.OvertimeHrs = (decimal)chkIO.ValidOtHours.TotalHours;
                                        hourRate = (basic * chkIO.CompanyInfo.OtRate / 100) * ottbl.CountAsWorkday / (decimal)chkIO.NormalHours.TotalHours;
                                        chkIO.OvertimeAmt = chkIO.OvertimeHrs * hourRate;
                                    }
                                    // Night Diff
                                    if (chkIO.Night != new TimeSpan(0, 0, 0, 0))
                                    {
                                        chkIO.NightDiffHrs = (decimal)Math.Abs(chkIO.Night.TotalHours);
                                        hourRate = (basic * chkIO.CompanyInfo.NightDiffRate / 100) * ottbl.CountAsWorkday / ottbl.CountAsHours;
                                        chkIO.NightDiffAmt = chkIO.NightDiffHrs * hourRate;
                                    }
                                    // Holiday
                                    if (hol != null)
                                    {
                                        chkIO.HolidayType = hol.HolidayType;
                                        if ((decimal)chkIO.NormalHours.TotalHours > 0m)
                                        {
                                            switch (hol.HolidayType)
                                            {
                                                case HolidayTypeEnum.Special:
                                                    chkIO.HolidayHrs2 = (decimal)chkIO.ValidWorkHours.TotalHours;
                                                    hourRate = (basic * hol.Rate / 100) * ottbl.CountAsWorkday / (decimal)chkIO.NormalHours.TotalHours;
                                                    chkIO.HolidayAmt2 = chkIO.HolidayHrs2 * hourRate;
                                                    if (chkIO.ValidOtHours != new TimeSpan(0, 0, 0, 0))
                                                    {
                                                        chkIO.HolidayOTHrs2 = (decimal)chkIO.ValidOtHours.TotalHours;
                                                        hourRate = (basic * hol.ExcessRate / 100) * ottbl.CountAsWorkday / (decimal)chkIO.NormalHours.TotalHours;
                                                        chkIO.HolidayOTAmt2 = chkIO.HolidayOTHrs2 * hourRate;
                                                    }
                                                    break;
                                                case HolidayTypeEnum.Regular:
                                                    if (chkIO.Remarks != "Holiday RG")
                                                    {
                                                        chkIO.HolidayHrs = (decimal)chkIO.ValidWorkHours.TotalHours;
                                                        hourRate = (basic * hol.Rate / 100) * ottbl.CountAsWorkday / (decimal)chkIO.NormalHours.TotalHours;
                                                        chkIO.HolidayAmt = chkIO.HolidayHrs * hourRate;
                                                        if (chkIO.ValidOtHours != new TimeSpan(0, 0, 0, 0))
                                                        {
                                                            chkIO.HolidayOTHrs = (decimal)chkIO.ValidOtHours.TotalHours;
                                                            hourRate = (basic * hol.ExcessRate / 100) * ottbl.CountAsWorkday / (decimal)chkIO.NormalHours.TotalHours;
                                                            chkIO.HolidayOTAmt = chkIO.HolidayOTHrs * hourRate;
                                                        }
                                                    }
                                                    break;
                                                case HolidayTypeEnum.None:
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                    break;
                                case EmployeePayTypeEnum.Monthly:
                                    break;
                                default:
                                    break;
                            }
                            #endregion
                            chkIO.Save();
                        }
                        startDate = startDate.AddDays(1);
                    } while (startDate.Date != endDate.Date);
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
                if (index == Convert.ToInt32(trans.Count().ToString()))
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
                    "Attendance calculation is cancelled.", "Cancelled",
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
                    "Attendance calculation has been successfull.");
                    //ObjectSpace.ReloadObject(_AttendanceCalculator);
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
