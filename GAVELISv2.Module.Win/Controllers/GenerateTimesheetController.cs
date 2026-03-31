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
    public partial class GenerateTimesheetController : ViewController
    {
        private SimpleAction generateTimesheetAction;
        private AttendanceCalculator02 _AttendanceCalculator02;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public GenerateTimesheetController()
        {
            this.TargetObjectType = typeof(AttendanceCalculator02);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "generateTimesheetActionId";
            this.generateTimesheetAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.generateTimesheetAction.Caption = "Generate";
            this.generateTimesheetAction.Execute += new SimpleActionExecuteEventHandler(generateTimesheetAction_Execute);
        }
        private void generateTimesheetAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _AttendanceCalculator02 = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as AttendanceCalculator02;
            XPCollection<Holiday> holidays = new XPCollection<Holiday>(((ObjectSpace)ObjectSpace).Session);
            var hols = holidays.Where(o => o.Date >= _AttendanceCalculator02.TimeRangeFrom && o.Date < _AttendanceCalculator02.TimeRangeTo);
            if (hols != null && hols.Count() > 0)
            {
                _AttendanceCalculator02.Holidays.AddRange(hols);
                _AttendanceCalculator02.Save();
            }

            this.ObjectSpace.CommitChanges();
            if (_AttendanceCalculator02.EmployeesToInclude.Count == 0)
            {
                XtraMessageBox.Show("There are no employees for staff payroll to process.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Generate...", _AttendanceCalculator02.EmployeesToInclude.Count,
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
            _BgWorker.RunWorkerAsync(_AttendanceCalculator02.EmployeesToInclude);
            _FrmProgress.ShowDialog();
        }
        private string _message;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            XPCollection<Employee> trans = (XPCollection<Employee>)e.Argument;
            XPCollection<DeviceAttendanceLog> devAtts = new XPCollection<DeviceAttendanceLog>(session);
            AttendanceCalculator02 att = session.GetObjectByKey<AttendanceCalculator02>(_AttendanceCalculator02.Oid);
            try
            {
                //XPCollection<Holiday> holidays = new XPCollection<Holiday>(session);
                //var hols = holidays.Where(o => o.Date >= att.TimeRangeFrom && o.Date < att.TimeRangeTo);
                //if (hols != null && hols.Count() > 0)
                //{
                //    att.Holidays.AddRange(hols);
                //    att.Save();
                //}
                foreach (Employee item in trans)
                {
                    index++;
                    _message = string.Format("Processing employee {0} succesfull.",
                    item.Name);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here...

                    Employee emp = session.GetObjectByKey<Employee>(item.Oid);
                    AttendanceRecord attrec = session.FindObject<AttendanceRecord>(BinaryOperator.Parse("[BatchID] = ? And [EmployeeID.No] = ?", att, item.No));
                    if (attrec == null)
                    {
                        attrec = ReflectionHelper.CreateObject<AttendanceRecord>(session);
                        attrec.BatchID = att;
                        attrec.EmployeeID = emp;
                    }
                    attrec.EmploymentStatus = emp.Status;
                    attrec.ClassCode = emp.EmployeeClassCode ?? null;
                    attrec.EmployeePosition = emp.Position ?? null;
                    attrec.Shift = emp.Shift ?? null;
                    attrec.RestDay = emp.RestDay;
                    attrec.PayType = emp.PayType;
                    switch (emp.PayType)
                    {
                        case EmployeePayTypeEnum.Hourly:
                            attrec.Basic = emp.Basic * emp.Shift.CountAsHours;
                            break;
                        case EmployeePayTypeEnum.Daily:
                            attrec.Basic = emp.Basic;
                            break;
                        case EmployeePayTypeEnum.Monthly:
                            attrec.Basic = (emp.Basic / WorkingDays.GetWorkingDays(att.TimeRangeFrom, att.TimeRangeTo, new List<DayOfWeek> { emp.RestDay }));
                            break;
                        default:
                            break;
                    }
                    attrec.Allowance = emp.Allowance;
                    DateTime startDate = att.TimeRangeFrom;
                    DateTime endDate = att.TimeRangeTo.AddDays(1);
                    do
                    {
                        if (item.Shift == null)
                        {
                            startDate = startDate.AddDays(1);
                            continue;
                        }
                        EmpShiftSchedule data = emp.ShiftSchedules.Where(o => (startDate.Ticks >= o.FromDate.Ticks && startDate.Ticks < o.ToDate.AddDays(1).Ticks)).OrderBy(o => o.FromDate).LastOrDefault();
                        TimeTable2 ottbl = session.GetObjectByKey<TimeTable2>(item.Shift.Oid);
                        if (data != null)
                        {
                            ottbl = session.GetObjectByKey<TimeTable2>(data.Shift.Oid);
                        }
                        CheckInAndOut03 chkIO = null;
                        string strId; // "10028 2016-05-24 Morning";
                        strId = string.Format("{0} {1} {2}", emp.EnrollNumber, startDate.Date.ToString("MM-dd-yyyy"), ottbl.TimeTableName);
                        if (strId == "10261 04-11-2019 Dayshift")
                        {

                        }
                        chkIO = session.FindObject<CheckInAndOut03>(BinaryOperator.Parse("[LineID]=?", strId));
                        if (chkIO == null)
                        {
                            chkIO = ReflectionHelper.CreateObject<CheckInAndOut03>(session);
                        }
                        if (!chkIO.UnAltered)
                        {
                            switch (emp.PayType)
                            {
                                case EmployeePayTypeEnum.Hourly:
                                    chkIO.BasicPay = emp.Basic;
                                    break;
                                case EmployeePayTypeEnum.Daily:
                                    chkIO.BasicPay = emp.Basic / emp.Shift.CountAsHours;
                                    break;
                                case EmployeePayTypeEnum.Monthly:
                                    chkIO.BasicPay = (emp.Basic / WorkingDays.GetWorkingDays(att.TimeRangeFrom, att.TimeRangeTo, new List<DayOfWeek> { emp.RestDay })) /
                                        emp.Shift.CountAsHours;
                                    break;
                                default:
                                    break;
                            }
                            //chkIO.BasicPay = emp.Basic;
                            chkIO.PayType = emp.PayType;
                            chkIO.RestDayOfTheWeek = emp.RestDay;
                            chkIO.AllowanceOverride = emp.Allowance;
                            chkIO.NightDiffRate = ottbl.NightDiffRate;
                            chkIO.Save();
                            startDate = startDate.AddDays(1);
                            continue;
                        }
                        chkIO.LineID = strId;
                        chkIO.AttRecId = attrec;
                        chkIO.EnrolledNo = emp.EnrollNumber;
                        chkIO.EmployeeId = emp;
                        switch (emp.PayType)
                        {
                            case EmployeePayTypeEnum.Hourly:
                                chkIO.BasicPay = emp.Basic;
                                break;
                            case EmployeePayTypeEnum.Daily:
                                chkIO.BasicPay = emp.Basic / emp.Shift.CountAsHours;
                                break;
                            case EmployeePayTypeEnum.Monthly:
                                chkIO.BasicPay = (emp.Basic / WorkingDays.GetWorkingDays(att.TimeRangeFrom, att.TimeRangeTo, new List<DayOfWeek> { emp.RestDay })) /
                                    emp.Shift.CountAsHours;
                                break;
                            default:
                                break;
                        }
                        //chkIO.BasicPay = emp.Basic;
                        chkIO.PayType = emp.PayType;
                        chkIO.RestDayOfTheWeek = emp.RestDay;
                        chkIO.AllowanceOverride = emp.Allowance;
                        chkIO.Date = startDate.Date;
                        chkIO.Day = chkIO.Date.DayOfWeek;
                        chkIO.TimeTable = ottbl;
                        chkIO.OnDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OnDutyTime.Hour, ottbl.OnDutyTime.Minute, 0);  //ottbl.OnDutyTime;4
                        chkIO.HalfDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.HalfDutyTime.Hour, ottbl.HalfDutyTime.Minute, 0);
                        chkIO.OffDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OffDutyTime.Hour, ottbl.OffDutyTime.Minute, 0); //ottbl.OffDutyTime;
                        chkIO.Normal = ottbl.CountAsWorkday;
                        chkIO.NightDiffRate = ottbl.NightDiffRate;
                        chkIO.RealTime = 0m;
                        chkIO.ActualHours = TimeSpan.Zero;
                        chkIO.Flexible = false;
                        // Zero Set
                        chkIO.NextDay2 = false;
                        chkIO.OvertimeIn1 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.OvertimeOut1 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        //chkIO.ZeroSetHrs = TimeSpan.Zero;
                        chkIO.Ref31 = 0;
                        chkIO.Ref32 = 0;
                        // First Set
                        chkIO.ClockIn1 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.BreakOut1 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.BreakIn1 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.ClockOut1 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.BreakClock1 = false;
                        //chkIO.FirstSetHrs = TimeSpan.Zero;
                        chkIO.Ref11 = 0;
                        chkIO.Ref12 = 0;
                        chkIO.Ref13 = 0;
                        chkIO.Ref14 = 0;
                        // Second Set
                        chkIO.NextDay1 = false;
                        chkIO.ClockIn2 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.BreakOut2 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.BreakIn2 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.ClockOut2 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.BreakClock2 = false;
                        //chkIO.SecondSetHrs = TimeSpan.Zero;
                        chkIO.Ref21 = 0;
                        chkIO.Ref22 = 0;
                        chkIO.Ref23 = 0;
                        chkIO.Ref24 = 0;
                        // Third Set
                        chkIO.OvertimeIn2 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        chkIO.OvertimeOut2 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, 0, 0, 0);
                        //chkIO.ThirdSetHrs = TimeSpan.Zero;
                        chkIO.Ref33 = 0;
                        chkIO.Ref34 = 0;
                        chkIO.ZeroNightHrs = TimeSpan.Zero;
                        //chkIO.ThirdNightHrs = TimeSpan.Zero;
                        ArrayList chiid = new ArrayList();
                        var devAttList = devAtts.Where(o => o.EnrolledNo == item.EnrollNumber && o.LogTime.Date == chkIO.Date).OrderBy(o => o.DwYear & o.DwMonth & o.DwDay & o.DwHour & o.DwMinute);
                        TimeSpan zts = TimeSpan.Zero;
                        TimeSpan ots = TimeSpan.Zero;
                        TimeSpan sts = TimeSpan.Zero;
                        TimeSpan tts = TimeSpan.Zero;
                        foreach (DeviceAttendanceLog datl in devAttList)
                        {
                            DeviceAttendanceLog dLog = session.GetObjectByKey<DeviceAttendanceLog>(datl.Oid);
                            dLog.checkId = chkIO;
                            dLog.Save();

                            #region Zero Set
                            if (datl.LogTime.TimeOfDay >= ottbl.ZeroSetCut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.SecondSetCut.TimeOfDay)
                            {
                                switch (datl.InOutMode)
                                {
                                    case InOutModeEnum.CheckIn:
                                        //chkIO.ClockIn1 = datl.LogTime;
                                        //chkIO.Ref11 = datl.Oid;
                                        break;
                                    case InOutModeEnum.CheckOut:
                                        // Will not register checkout in this zone
                                        break;
                                    case InOutModeEnum.BreakOut:
                                        // Will not register breakout in this zone
                                        break;
                                    case InOutModeEnum.BreakIn:
                                        // Will not register breakin in this zone
                                        break;
                                    case InOutModeEnum.OvertimeIn:
                                        chkIO.OvertimeIn1 = datl.LogTime;
                                        chkIO.Ref31 = datl.Oid;
                                        break;
                                    case InOutModeEnum.OvertimeOut:
                                        chkIO.OvertimeOut1 = datl.LogTime;
                                        chkIO.Ref32 = datl.Oid;
                                        break;
                                    default:
                                        break;
                                }

                                // Zero Hours

                                DateTime zdt01 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                                DateTime zdt02 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                                //if (chkIO.OvertimeIn1 != new DateTime(1753, 1, 1, 0, 0, 0))
                                //{
                                //    zdt01 = chkIO.OvertimeIn1;
                                //}
                                //if (chkIO.OvertimeOut1 != new DateTime(1753, 1, 1, 0, 0, 0))
                                //{
                                //    zdt02 = chkIO.OvertimeOut1;
                                //}
                                //zts = zdt02 - zdt01;
                                //chkIO.ZeroSetHrs = zts;

                                DateTime zndstart = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                                DateTime zndend = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.NightEndTime.Hour, ottbl.NightEndTime.Minute, ottbl.NightEndTime.Second);

                                DateTime znt01;
                                DateTime znt02;
                                if (zdt01 >= zndstart)
                                {
                                    znt01 = zdt01;
                                }
                                else
                                {
                                    znt01 = zndstart;
                                }
                                if (zdt02 <= zndend)
                                {
                                    znt02 = zdt02;
                                }
                                else
                                {
                                    znt02 = zndend;
                                }
                                chkIO.ZeroNightHrs = znt02 - znt01;
                            }
                            #endregion
                            #region First Set
                            if (datl.LogTime.TimeOfDay >= ottbl.FirstSetCut.TimeOfDay && datl.LogTime.TimeOfDay < ottbl.SecondSetCut.TimeOfDay)
                            {
                                switch (datl.InOutMode)
                                {
                                    case InOutModeEnum.CheckIn:
                                        chkIO.ClockIn1 = datl.LogTime;
                                        chkIO.Ref11 = datl.Oid;
                                        break;
                                    case InOutModeEnum.CheckOut:
                                        chkIO.ClockOut1 = datl.LogTime;
                                        chkIO.Ref14 = datl.Oid;
                                        break;
                                    case InOutModeEnum.BreakOut:
                                        chkIO.BreakOut1 = datl.LogTime;
                                        chkIO.Ref12 = datl.Oid;
                                        break;
                                    case InOutModeEnum.BreakIn:
                                        chkIO.BreakIn1 = datl.LogTime;
                                        chkIO.Ref13 = datl.Oid;
                                        break;
                                    case InOutModeEnum.OvertimeIn:
                                        // Will not register OvertimeIn in this zone
                                        break;
                                    case InOutModeEnum.OvertimeOut:
                                        // Will not register OvertimeOut in this zone
                                        break;
                                    default:
                                        break;
                                }
                                // First Hours
                                TimeSpan vhrs01 = ottbl.BreakOutTime - ottbl.OnDutyTime;
                                DateTime odt01 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.SecondSetCut.Hour, ottbl.SecondSetCut.Minute, ottbl.SecondSetCut.Second);
                                DateTime odt02 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.SecondSetCut.Hour, ottbl.SecondSetCut.Minute, ottbl.SecondSetCut.Second);
                                if (chkIO.ClockIn1 != new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0))
                                {
                                    odt01 = chkIO.ClockIn1;
                                }
                                if (chkIO.ClockOut1 != new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0))
                                {
                                    odt02 = chkIO.ClockOut1;
                                }
                                ots = odt02 - odt01;
                                if (ots > vhrs01)
                                {
                                    ots = vhrs01;
                                }
                                //chkIO.FirstSetHrs = ots;
                            }
                            #endregion
                            #region Second Set
                            if (datl.LogTime.TimeOfDay >= ottbl.SecondSetCut.TimeOfDay && datl.LogTime.TimeOfDay < ottbl.ThirdSetCut.TimeOfDay)
                            {
                                switch (datl.InOutMode)
                                {
                                    case InOutModeEnum.CheckIn:
                                        chkIO.ClockIn2 = datl.LogTime;
                                        chkIO.Ref21 = datl.Oid;
                                        break;
                                    case InOutModeEnum.CheckOut:
                                        chkIO.ClockOut2 = datl.LogTime;
                                        chkIO.Ref24 = datl.Oid;
                                        break;
                                    case InOutModeEnum.BreakOut:
                                        chkIO.BreakOut2 = datl.LogTime;
                                        chkIO.Ref22 = datl.Oid;
                                        break;
                                    case InOutModeEnum.BreakIn:
                                        chkIO.BreakIn2 = datl.LogTime;
                                        chkIO.Ref23 = datl.Oid;
                                        break;
                                    case InOutModeEnum.OvertimeIn:
                                        // Will not register OvertimeIn in this zone
                                        break;
                                    case InOutModeEnum.OvertimeOut:
                                        // Will not register OvertimeOut in this zone
                                        break;
                                    default:
                                        break;
                                }
                                // Second Hours
                                //TimeSpan vhrs02 = ottbl.OffDutyTime - ottbl.HalfDutyTime;
                                //DateTime sdt01 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                                //DateTime sdt02 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                                //if (chkIO.ClockIn2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                //{
                                //    sdt01 = chkIO.ClockIn2;
                                //}
                                //if (chkIO.ClockOut2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                //{
                                //    sdt02 = chkIO.ClockOut2;
                                //}
                                //sts = sdt02 - sdt01;
                                //if (sts > vhrs02)
                                //{
                                //    sts = vhrs02;
                                //}
                                //chkIO.SecondSetHrs = sts;
                            }
                            #endregion
                            #region Third Set
                            if (datl.LogTime.TimeOfDay >= ottbl.SecondSetCut.TimeOfDay && datl.LogTime.TimeOfDay <= ottbl.LastCut.TimeOfDay)
                            //if (datl.LogTime.TimeOfDay <= ottbl.LastCut.TimeOfDay)
                            {
                                switch (datl.InOutMode)
                                {
                                    case InOutModeEnum.CheckIn:
                                        // Will not register OvertimeIn in this zone
                                        break;
                                    case InOutModeEnum.CheckOut:
                                        // chkIO.ClockOut2 = datl.LogTime;
                                        // chkIO.Ref24 = datl.Oid;
                                        break;
                                    case InOutModeEnum.BreakOut:
                                        // Will not register OvertimeIn in this zone
                                        break;
                                    case InOutModeEnum.BreakIn:
                                        // Will not register OvertimeIn in this zone
                                        break;
                                    case InOutModeEnum.OvertimeIn:
                                        chkIO.OvertimeIn2 = datl.LogTime;
                                        chkIO.Ref33 = datl.Oid;
                                        break;
                                    case InOutModeEnum.OvertimeOut:
                                        chkIO.OvertimeOut2 = datl.LogTime;
                                        chkIO.Ref34 = datl.Oid;
                                        break;
                                    default:
                                        break;
                                }
                                // Third Hours
                                //DateTime tdt01 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                                //DateTime tdt02 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                                //if (chkIO.OvertimeIn2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                //{
                                //    tdt01 = chkIO.OvertimeIn2;
                                //}
                                //if (chkIO.OvertimeOut2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                //{
                                //    tdt02 = chkIO.OvertimeOut2;
                                //}
                                //tts = tdt02 - tdt01;
                                //chkIO.ThirdSetHrs = tts;
                                //DateTime tndstart = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.NightStartTime.Hour, ottbl.NightStartTime.Minute, ottbl.NightStartTime.Second);
                                //DateTime tndend = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.LastCut.Hour, ottbl.LastCut.Minute, ottbl.LastCut.Second);

                                //DateTime tnt01;
                                //DateTime tnt02;
                                //if (tdt01 >= tndstart)
                                //{
                                //    tnt01 = tdt01;
                                //}
                                //else
                                //{
                                //    tnt01 = tndstart;
                                //}
                                //if (tdt02 <= tndend)
                                //{
                                //    tnt02 = tdt02;
                                //}
                                //else
                                //{
                                //    tnt02 = tndend;
                                //}
                                //chkIO.ThirdNightHrs = tnt02 - tnt01;
                            }
                            #endregion
                        }
                        //if (zts < TimeSpan.Zero || ots < TimeSpan.Zero || sts < TimeSpan.Zero || tts < TimeSpan.Zero
                        //    || chkIO.ActualLate > TimeSpan.FromHours(24d) || chkIO.ActualEarly > TimeSpan.FromHours(24d))
                        //{
                        //    chkIO.Invalid = true;
                        //}
                        //else
                        //{
                        //    chkIO.Invalid = false;
                        //}
                        chkIO.UnAltered = true;
                        chkIO.References = chiid;
                        chkIO.Save();
                        startDate = startDate.AddDays(1);
                    } while (startDate.Date != endDate.Date);
                    attrec.Save();

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
                if (index == trans.Count)
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
