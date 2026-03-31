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
    public partial class CalculateAttendance2Controller : ViewController
    {
        private SimpleAction calculateAttendanceAction;
        private AttendanceCalculator _AttendanceCalculator;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public CalculateAttendance2Controller()
        {
            this.TargetObjectType = typeof(AttendanceCalculator);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "CalculateAttendanceActionId2";
            this.calculateAttendanceAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.calculateAttendanceAction.Caption = "Calculate 2";
            this.calculateAttendanceAction.Execute += new SimpleActionExecuteEventHandler(calculateAttendanceAction_Execute);
        }
        private void calculateAttendanceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _AttendanceCalculator = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as AttendanceCalculator;
            this.ObjectSpace.CommitChanges();
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
            AttendanceCalculator att = session.GetObjectByKey<AttendanceCalculator>(_AttendanceCalculator.Oid);
            try
            {
                foreach (Employee item in trans)
                {
                    index++;
                    _message = string.Format("Processing employee {0} succesfull.",
                    item.Name);
                    _BgWorker.ReportProgress(1, _message);

                    #region Algorithms here
                    Employee emp = session.GetObjectByKey<Employee>(item.Oid);
                    DateTime startDate = _AttendanceCalculator.TimeRangeFrom;
                    DateTime endDate = _AttendanceCalculator.TimeRangeTo.AddDays(1);
                    do
                    {
                        if (item.Shift == null)
                        {
                            startDate = startDate.AddDays(1);
                            continue;
                        }
                        TimeTable2 ottbl = session.GetObjectByKey<TimeTable2>(item.Shift.Oid);
                        CheckInAndOut03 chkIO = null;
                        string strId; // "10028 2016-05-24 Morning";
                        strId = string.Format("{0} {1} {2}", emp.EnrollNumber, startDate.Date.ToString("MM-dd-yyyy"), ottbl.TimeTableName);
                        chkIO = session.FindObject<CheckInAndOut03>(BinaryOperator.Parse("[LineID]=?", strId));
                        if (chkIO == null)
                        {
                            chkIO = ReflectionHelper.CreateObject<CheckInAndOut03>(session);
                        }
                        chkIO.LineID = strId;
                        chkIO.AttCalcId = att;
                        chkIO.EnrolledNo = emp.EnrollNumber;
                        chkIO.EmployeeId = emp;
                        chkIO.BasicPay = emp.Basic;
                        chkIO.PayType = emp.PayType;
                        chkIO.RestDayOfTheWeek = emp.RestDay;
                        chkIO.Date = startDate.Date;
                        chkIO.Day = chkIO.Date.DayOfWeek;
                        chkIO.TimeTable = ottbl;
                        chkIO.OnDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OnDutyTime.Hour, ottbl.OnDutyTime.Minute, 0);  //ottbl.OnDutyTime;4
                        chkIO.HalfDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.HalfDutyTime.Hour, ottbl.HalfDutyTime.Minute, 0);
                        chkIO.OffDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.OffDutyTime.Hour, ottbl.OffDutyTime.Minute, 0); //ottbl.OffDutyTime;
                        chkIO.Normal = ottbl.CountAsWorkday;
                        chkIO.RealTime = 0m;
                        //chkIO.NormalHours = TimeSpan.Zero;
                        //chkIO.ActualLate = TimeSpan.Zero;
                        ////chkIO.Late = TimeSpan.Zero;
                        //chkIO.ActualEarly = TimeSpan.Zero;
                        //chkIO.Early = TimeSpan.Zero;
                        //chkIO.Absent = false;
                        //chkIO.AbsentCount = 0m;
                        //chkIO.AbsentHours = TimeSpan.Zero;
                        //chkIO.OtHours = TimeSpan.Zero;
                        //chkIO.ValidOtHours = TimeSpan.Zero;
                        chkIO.ActualHours = TimeSpan.Zero;
                        //chkIO.ValidWorkHours = TimeSpan.Zero;
                        //chkIO.Remarks = string.Empty;
                        //chkIO.OtStatus = OtStatusEnum.None;
                        //chkIO.RestDay = TimeSpan.Zero;
                        //chkIO.Holiday = TimeSpan.Zero;
                        //chkIO.Night = TimeSpan.Zero;
                        chkIO.Flexible = false;
                        // Zero Set
                        chkIO.NextDay2 = false;
                        chkIO.OvertimeIn1 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.OvertimeOut1 = new DateTime(1753, 1, 1, 0, 0, 0);
                        //chkIO.ZeroSetHrs = TimeSpan.Zero;
                        chkIO.Ref31 = 0;
                        chkIO.Ref32 = 0;

                        // First Set
                        chkIO.ClockIn1 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.BreakOut1 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.BreakIn1 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.ClockOut1 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.BreakClock1 = false;
                        //chkIO.FirstSetHrs = TimeSpan.Zero;
                        chkIO.Ref11 = 0;
                        chkIO.Ref12 = 0;
                        chkIO.Ref13 = 0;
                        chkIO.Ref14 = 0;
                        // Second Set
                        chkIO.NextDay1 = false;
                        chkIO.ClockIn2 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.BreakOut2 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.BreakIn2 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.ClockOut2 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.BreakClock2 = false;
                        //chkIO.SecondSetHrs = TimeSpan.Zero;
                        chkIO.Ref21 = 0;
                        chkIO.Ref22 = 0;
                        chkIO.Ref23 = 0;
                        chkIO.Ref24 = 0;
                        // Third Set
                        chkIO.OvertimeIn2 = new DateTime(1753, 1, 1, 0, 0, 0);
                        chkIO.OvertimeOut2 = new DateTime(1753, 1, 1, 0, 0, 0);
                        //chkIO.ThirdSetHrs = TimeSpan.Zero;
                        chkIO.Ref33 = 0;
                        chkIO.Ref34 = 0;
                        // Pay Computation
                        //chkIO.BasicHrs = 0;
                        //chkIO.BasicAmt = 0;
                        //chkIO.AbsentHrs = 0;
                        //chkIO.AbsentAmt = 0;
                        //chkIO.LateHrs = 0;
                        //chkIO.LateAmt = 0;
                        //chkIO.UndertimeHrs = 0;
                        //chkIO.UndertimeAmt = 0;
                        //chkIO.RestdayOtHrs = 0;
                        //chkIO.RestdayOtAmt = 0;
                        //chkIO.OvertimeHrs = 0;
                        //chkIO.OvertimeAmt = 0;
                        //chkIO.NightDiffHrs = 0;
                        //chkIO.NightDiffAmt = 0;
                        //chkIO.HolidayHrs = 0;
                        //chkIO.HolidayType = HolidayTypeEnum.None;
                        //chkIO.HolidayAmt = 0;
                        //chkIO.HolidayHrs2 = 0;
                        //chkIO.HolidayAmt2 = 0;
                        //chkIO.HolidayOTHrs2 = 0;
                        //chkIO.HolidayOTAmt2 = 0;
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
                                if (chkIO.OvertimeIn1 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    zdt01 = chkIO.OvertimeIn1;
                                }
                                if (chkIO.OvertimeOut1 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    zdt02 = chkIO.OvertimeOut1;
                                }
                                zts = zdt02 - zdt01;
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
                                if (chkIO.ClockIn1 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    odt01 = chkIO.ClockIn1;
                                }
                                if (chkIO.ClockOut1 != new DateTime(1753, 1, 1, 0, 0, 0))
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
                                TimeSpan vhrs02 = ottbl.OffDutyTime - ottbl.HalfDutyTime;
                                DateTime sdt01 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                                DateTime sdt02 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                                if (chkIO.ClockIn2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    sdt01 = chkIO.ClockIn2;
                                }
                                if (chkIO.ClockOut2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    sdt02 = chkIO.ClockOut2;
                                }
                                sts = sdt02 - sdt01;
                                if (sts > vhrs02)
                                {
                                    sts = vhrs02;
                                }
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
                                DateTime tdt01 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                                DateTime tdt02 = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                                if (chkIO.OvertimeIn2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    tdt01 = chkIO.OvertimeIn2;
                                }
                                if (chkIO.OvertimeOut2 != new DateTime(1753, 1, 1, 0, 0, 0))
                                {
                                    tdt02 = chkIO.OvertimeOut2;
                                }
                                tts = tdt02 - tdt01;
                                //chkIO.ThirdSetHrs = tts;
                                DateTime tndstart = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.NightStartTime.Hour, ottbl.NightStartTime.Minute, ottbl.NightStartTime.Second);
                                DateTime tndend = new DateTime(startDate.Year, startDate.Month, startDate.Day, ottbl.LastCut.Hour, ottbl.LastCut.Minute, ottbl.LastCut.Second);

                                DateTime tnt01;
                                DateTime tnt02;
                                if (tdt01 >= tndstart)
                                {
                                    tnt01 = tdt01;
                                }
                                else
                                {
                                    tnt01 = tndstart;
                                }
                                if (tdt02 <= tndend)
                                {
                                    tnt02 = tdt02;
                                }
                                else
                                {
                                    tnt02 = tndend;
                                }
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
                        chkIO.References = chiid;
                        chkIO.Save();
                        startDate = startDate.AddDays(1);
                    } while (startDate.Date != endDate.Date);
                    //System.Threading.Thread.Sleep(300);
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
