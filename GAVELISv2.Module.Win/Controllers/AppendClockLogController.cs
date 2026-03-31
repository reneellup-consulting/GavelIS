using System;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class AppendClockLogController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private CheckInAndOut03 _CheckInOut;
        private PopupWindowShowAction appendClockLogAction;
        private DeviceAttendanceLog _AttLogCopy;
        private int _LineId;
        private DevExpress.XtraGrid.Columns.GridColumn _FocusColumn;
        public AppendClockLogController()
        {
            this.TargetObjectType = typeof(CheckInAndOut03);
            this.TargetViewType = ViewType.ListView;
            string actionID = "AppendClockLogActionID";
            this.appendClockLogAction = new PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit);
            this.appendClockLogAction.Caption = "Append Clock In/Out";
            this.appendClockLogAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(appendClockLogAction_CustomizePopupWindowParams);
            this.appendClockLogAction.Execute += new PopupWindowShowActionExecuteEventHandler(appendClockLogAction_Execute);
        }
        private void appendClockLogAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            GridView gridView = null;
            if (listEditor != null)
            {
                gridView = listEditor.GridView;
            }
            _FocusColumn = gridView.FocusedColumn;
            if (!new[] { "ClockIn1", "BreakOut1", "BreakIn1", "ClockOut1", "ClockIn2", "BreakOut2", "BreakIn2", "ClockOut2",
            "OvertimeIn1","OvertimeOut1","OvertimeIn2","OvertimeOut2"}.Any(o => _FocusColumn.FieldName.Contains(o)))
            {
                throw new UserFriendlyException("Invalid selection of row column.");
            }
            _ObjectSpace = Application.CreateObjectSpace();
            _CheckInOut = this.View.SelectedObjects[0] as CheckInAndOut03;
            if (_CheckInOut == null)
            {
                throw new ApplicationException("No entry selected");
            }
            DeviceAttendanceLog thisAttLog = null;
            CheckInAndOut03 thisCheckInOut = _ObjectSpace.GetObjectByKey<CheckInAndOut03>(_CheckInOut.Oid);
            _LineId = thisCheckInOut.Oid;
            //"ClockIn1", 
            if (_FocusColumn.FieldName == "ClockIn1" && thisCheckInOut.Ref11 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref11));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.ClockIn1;
            }
            //"BreakOut1", 
            else if (_FocusColumn.FieldName == "BreakOut1" && thisCheckInOut.Ref12 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref12));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.BreakOut1;
            }
            //"BreakIn1", 
            else if (_FocusColumn.FieldName == "BreakIn1" && thisCheckInOut.Ref13 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref13));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.BreakIn1;
            }
            //"ClockOut1", 
            else if (_FocusColumn.FieldName == "ClockOut1" && thisCheckInOut.Ref14 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref14));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.ClockOut1;
            }
            //"ClockIn2", 
            else if (_FocusColumn.FieldName == "ClockIn2" && thisCheckInOut.Ref21 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref21));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.ClockIn2;
            }
            //"BreakOut2", 
            else if (_FocusColumn.FieldName == "BreakOut2" && thisCheckInOut.Ref22 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref22));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.BreakOut2;
            }
            //"BreakIn2", 
            else if (_FocusColumn.FieldName == "BreakIn2" && thisCheckInOut.Ref23 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref23));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.BreakIn2;
            }
            //"ClockOut2",
            else if (_FocusColumn.FieldName == "ClockOut2" && thisCheckInOut.Ref24 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref24));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.ClockOut2;
            }
            //"OvertimeIn1",
            else if (_FocusColumn.FieldName == "OvertimeIn1" && thisCheckInOut.Ref31 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref31));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.OvertimeIn1;
            }
            //"OvertimeOut1",
            else if (_FocusColumn.FieldName == "OvertimeOut1" && thisCheckInOut.Ref32 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref32));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.OvertimeOut1;
            }
            //"OvertimeIn2",
            else if (_FocusColumn.FieldName == "OvertimeIn2" && thisCheckInOut.Ref33 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref33));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.OvertimeIn2;
            }
            //"OvertimeOut2"
            else if (_FocusColumn.FieldName == "OvertimeOut2" && thisCheckInOut.Ref34 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.Ref34));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.OvertimeOut2;
            }
            if (thisAttLog == null)
            {
                thisAttLog = ReflectionHelper.CreateObject<DeviceAttendanceLog>(((ObjectSpace)_ObjectSpace).Session);
                //"ClockIn1", 
                if (_FocusColumn.FieldName == "ClockIn1")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.CheckIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.FirstSetCut.AddHours(2).Hour, thisCheckInOut.TimeTable.FirstSetCut.AddHours(2).Minute,
                        thisCheckInOut.TimeTable.FirstSetCut.AddHours(2).Second);
                }
                //"BreakOut1", 
                else if (_FocusColumn.FieldName == "BreakOut1")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.BreakOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.FirstSetCut.AddHours(4).Hour, thisCheckInOut.TimeTable.FirstSetCut.AddHours(4).Minute,
                        thisCheckInOut.TimeTable.FirstSetCut.AddHours(4).Second);
                }
                //"BreakIn1", 
                else if (_FocusColumn.FieldName == "BreakIn1")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.BreakIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.FirstSetCut.AddHours(4.5).Hour, thisCheckInOut.TimeTable.FirstSetCut.AddHours(4.5).Minute,
                        thisCheckInOut.TimeTable.FirstSetCut.AddHours(4.5).Second);
                }
                //"ClockOut1", 
                else if (_FocusColumn.FieldName == "ClockOut1")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.CheckOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.SecondSetCut.Hour, thisCheckInOut.TimeTable.SecondSetCut.Minute, thisCheckInOut.TimeTable.SecondSetCut.Second);
                }
                //"ClockIn2", 
                else if (_FocusColumn.FieldName == "ClockIn2")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.CheckIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.SecondSetCut.AddHours(1).Hour, thisCheckInOut.TimeTable.SecondSetCut.AddHours(1).Minute,
                        thisCheckInOut.TimeTable.SecondSetCut.AddHours(1).Second);
                }
                //"BreakOut2", 
                else if (_FocusColumn.FieldName == "BreakOut2")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.BreakOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.SecondSetCut.AddHours(3).Hour, thisCheckInOut.TimeTable.SecondSetCut.AddHours(3).Minute,
                        thisCheckInOut.TimeTable.SecondSetCut.AddHours(3).Second);
                }
                //"BreakIn2", 
                else if (_FocusColumn.FieldName == "BreakIn2")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.BreakIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.SecondSetCut.AddHours(3.5).Hour, thisCheckInOut.TimeTable.SecondSetCut.AddHours(3.5).Minute,
                        thisCheckInOut.TimeTable.SecondSetCut.AddHours(3.5).Second);
                }
                //"ClockOut2",
                else if (_FocusColumn.FieldName == "ClockOut2")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.CheckOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, thisCheckInOut.TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute,
                        thisCheckInOut.TimeTable.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                }
                //"OvertimeIn1",
                else if (_FocusColumn.FieldName == "OvertimeIn1")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.OvertimeIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.ZeroSetCut.Hour, thisCheckInOut.TimeTable.ZeroSetCut.Minute, thisCheckInOut.TimeTable.ZeroSetCut.Second);
                }
                //"OvertimeOut1",
                else if (_FocusColumn.FieldName == "OvertimeOut1")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.OvertimeOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.FirstSetCut.Hour, thisCheckInOut.TimeTable.FirstSetCut.Minute, thisCheckInOut.TimeTable.FirstSetCut.Second);
                }
                //"OvertimeIn2",
                else if (_FocusColumn.FieldName == "OvertimeIn2")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.OvertimeIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.ThirdSetCut.Hour, thisCheckInOut.TimeTable.ThirdSetCut.Minute, thisCheckInOut.TimeTable.ThirdSetCut.Second);
                }
                //"OvertimeOut2"
                else if (_FocusColumn.FieldName == "OvertimeOut2")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.OvertimeOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day,
                        thisCheckInOut.TimeTable.LastCut.Hour, thisCheckInOut.TimeTable.LastCut.Minute, thisCheckInOut.TimeTable.LastCut.Second);
                }
            }
            thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
            thisAttLog.EmployeeName = thisCheckInOut.Employee;
            thisAttLog.VerifyMode = VerifyModeEnum.System;
            _AttLogCopy = thisAttLog;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "AppendClockInOut_DetailView", true, thisAttLog);
        }
        private void appendClockLogAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();
            IObjectSpace _ObjectSpace02 = Application.CreateObjectSpace();
            CheckInAndOut03 cho2 = this.View.SelectedObjects[0] as CheckInAndOut03;
            CheckInAndOut03 chkIO = _ObjectSpace02.GetObjectByKey<CheckInAndOut03>(cho2.Oid);
            chkIO.RealTime = 0m;
            //chkIO.NormalHours = TimeSpan.Zero;
            //chkIO.ActualLate = TimeSpan.Zero;
            //chkIO.Late = TimeSpan.Zero;
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
            ////chkIO.LateAmt = 0;
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
            //XPCollection<DeviceAttendanceLog> devAtts = new XPCollection<DeviceAttendanceLog>(((ObjectSpace)_ObjectSpace02).Session);
            //var devAttList = devAtts.Where(o => o.EnrolledNo == chkIO.EnrolledNo && o.LogTime.Date == chkIO.Date).OrderBy(o => o.DwYear & o.DwMonth & o.DwDay & o.DwHour & o.DwMinute);
            //[Enrolled No] = '10277' And [Log Time] >= #2018-04-26# And [Log Time] < #2018-04-27#
            string crit = string.Format("[EnrolledNo] = {0} And [LogTime] >= #{1}# And [LogTime] < #{2}#", chkIO.EnrolledNo, chkIO.Date.ToString("yyy-MM-dd"), chkIO.Date.AddDays(1).ToString("yyy-MM-dd"));
            var devAttList = _ObjectSpace02.GetObjects<DeviceAttendanceLog>(CriteriaOperator.Parse(crit));
            TimeTable2 ottbl = chkIO.TimeTable;
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
                            chkIO.ClockIn1 = datl.LogTime;
                            chkIO.Ref11 = datl.Oid;
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
                    DateTime zdt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                    DateTime zdt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
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
                    DateTime zndstart = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ZeroSetCut.Hour, ottbl.ZeroSetCut.Minute, ottbl.ZeroSetCut.Second);
                    DateTime zndend = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.NightEndTime.Hour, ottbl.NightEndTime.Minute, ottbl.NightEndTime.Second);

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
                    DateTime odt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.Hour, ottbl.SecondSetCut.Minute, ottbl.SecondSetCut.Second);
                    DateTime odt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.Hour, ottbl.SecondSetCut.Minute, ottbl.SecondSetCut.Second);
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
                    DateTime sdt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
                    DateTime sdt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.SecondSetCut.AddHours(1).Hour, ottbl.SecondSetCut.AddHours(1).Minute, ottbl.SecondSetCut.AddHours(1).Second);
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
                    DateTime tdt01 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
                    DateTime tdt02 = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Hour, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Minute, ottbl.ThirdSetCut.Subtract(new TimeSpan(1, 0, 0)).Second);
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
                    DateTime tndstart = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.NightStartTime.Hour, ottbl.NightStartTime.Minute, ottbl.NightStartTime.Second);
                    DateTime tndend = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ottbl.LastCut.Hour, ottbl.LastCut.Minute, ottbl.LastCut.Second);

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
            //                || chkIO.ActualLate > TimeSpan.FromHours(24d) || chkIO.ActualEarly > TimeSpan.FromHours(24d))
            //{
            //    chkIO.Invalid = true;
            //}
            //else
            //{
            //    chkIO.Invalid = false;
            //}
            chkIO.References = chiid;
            chkIO.Save();
            _ObjectSpace02.CommitChanges();
            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();
        }
    }
}
