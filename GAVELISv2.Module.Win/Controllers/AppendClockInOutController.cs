using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.SystemModule;
using GAVELISv2.Module.BusinessObjects;
using DevExpress.XtraEditors;
//using System.Windows.Forms;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Views.Grid;

namespace GAVELISv2.Module.Win.Controllers
{
    public partial class AppendClockInOutController : ViewController
    {
        private IObjectSpace _ObjectSpace;
        private CheckInAndOut02 _CheckInOut;
        private PopupWindowShowAction appendClockInOutAction;
        private DeviceAttendanceLog _AttLogCopy;
        private DevExpress.XtraGrid.Columns.GridColumn _FocusColumn;
        public AppendClockInOutController()
        {
            this.TargetObjectType = typeof(CheckInAndOut02);
            this.TargetViewType = ViewType.ListView;
            string actionID = "AppendClockInOutActionID";
            this.appendClockInOutAction = new PopupWindowShowAction(this, actionID, PredefinedCategory.RecordEdit);
            this.appendClockInOutAction.Caption = "Append Clock In/Out";
            this.appendClockInOutAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(appendClockInOutAction_CustomizePopupWindowParams);
            this.appendClockInOutAction.Execute += new PopupWindowShowActionExecuteEventHandler(appendClockInOutAction_Execute);
        }

        void appendClockInOutAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            _ObjectSpace.CommitChanges();

            IObjectSpace _ObjectSpace02 = Application.CreateObjectSpace();
            DeviceAttendanceLog sndr = _ObjectSpace02.GetObjectByKey<DeviceAttendanceLog>(_AttLogCopy.Oid);
            CheckInAndOut02 cho2 = _ObjectSpace02.GetObjectByKey<CheckInAndOut02>(_CheckInOut.Oid);
            if (sndr == null && _FocusColumn != null)
            {
                if (_FocusColumn.FieldName == "ClockIn")
                {
                    cho2.ClockIn = new DateTime(1753, 1, 1, 0, 0, 0);
                    cho2.RefID = 0;
                }
                else if (_FocusColumn.FieldName == "ClockOut")
                {
                    cho2.ClockOut = new DateTime(1753, 1, 1, 0, 0, 0);
                    cho2.RefID02 = 0;
                }
            }
            // Search for CheckIn
            if (sndr!=null && sndr.InOutMode == InOutModeEnum.CheckIn)
            {
                cho2.ClockIn = sndr.LogTime;
                cho2.RefID = sndr.Oid;
                //// Between BeginningIn & EndingIn range
                //if (sndr.LogTime.TimeOfDay >= cho2.TimeTable.BeginningIn.TimeOfDay && sndr.LogTime.TimeOfDay <= cho2.TimeTable.EndingIn.TimeOfDay)
                //{
                //    // ClockIn
                //    cho2.ClockIn = sndr.LogTime;
                //    cho2.RefID = sndr.Oid;
                //}
            }
            //else if (sndr==null)
            //{
            //    cho2.ClockIn = new DateTime(1753, 1, 1, 0, 0, 0);
            //    cho2.RefID = 0;
            //}
            // Search for BreakOut
            if (sndr != null && sndr.InOutMode == InOutModeEnum.BreakOut)
            {
                cho2.ClockOut = sndr.LogTime;
                cho2.RefID02 = sndr.Oid;
                // Between BeginningOut & EndingOut range
                //if (sndr.LogTime.TimeOfDay >= cho2.TimeTable.BeginningOut.TimeOfDay && sndr.LogTime.TimeOfDay <= cho2.TimeTable.EndingOut.TimeOfDay)
                //{
                //    // ClockOut
                //    cho2.ClockOut = sndr.LogTime;
                //    cho2.RefID02 = sndr.Oid;
                //}
            }
            //else if (sndr == null)
            //{
            //    cho2.ClockOut = new DateTime(1753, 1, 1, 0, 0, 0);
            //    cho2.RefID02 = 0;
            //}
            // Search for BreakIn
            if (sndr != null && sndr.InOutMode == InOutModeEnum.BreakIn)
            {
                cho2.ClockIn = sndr.LogTime;
                cho2.RefID = sndr.Oid;
                // Between BeginningIn & EndingIn
                //if (sndr.LogTime.TimeOfDay >= cho2.TimeTable.BeginningIn.TimeOfDay && sndr.LogTime.TimeOfDay <= cho2.TimeTable.EndingIn.TimeOfDay)
                //{
                //    // ClockIn
                //    cho2.ClockIn = sndr.LogTime;
                //    cho2.RefID = sndr.Oid;
                //}
            }
            //else if (sndr == null)
            //{
            //    cho2.ClockIn = new DateTime(1753, 1, 1, 0, 0, 0);
            //    cho2.RefID = 0;
            //}
            // Search for CheckOut
            if (sndr != null && sndr.InOutMode == InOutModeEnum.CheckOut)
            {
                cho2.ClockOut = sndr.LogTime;
                cho2.RefID02 = sndr.Oid;
                // Between BeginningOut & EndingOut range
                //if (sndr.LogTime.TimeOfDay >= cho2.TimeTable.BeginningOut.TimeOfDay && sndr.LogTime.TimeOfDay <= cho2.TimeTable.EndingOut.TimeOfDay)
                //{
                //    // ClockOut
                //    cho2.ClockOut = sndr.LogTime;
                //    cho2.RefID02 = sndr.Oid;
                //}
            }
            //else if (sndr == null)
            //{
            //    cho2.ClockOut = new DateTime(1753, 1, 1, 0, 0, 0);
            //    cho2.RefID02 = 0;
            //}
            // Search for OvertimeIn
            if (sndr != null && sndr.InOutMode == InOutModeEnum.OvertimeIn)
            {
                cho2.ClockIn = sndr.LogTime;
                cho2.RefID = sndr.Oid;
                // Between BeginningIn & EndingIn
                //if (sndr.LogTime.TimeOfDay >= cho2.TimeTable.BeginningIn.TimeOfDay && sndr.LogTime.TimeOfDay <= cho2.TimeTable.EndingIn.TimeOfDay)
                //{
                //    // ClockIn
                //    cho2.ClockIn = sndr.LogTime;
                //    cho2.RefID = sndr.Oid;
                //}
            }
            //else if (sndr == null)
            //{
            //    cho2.ClockIn = new DateTime(1753, 1, 1, 0, 0, 0);
            //    cho2.RefID = 0;
            //}
            // Search for OvertimeOut
            if (sndr != null && sndr.InOutMode == InOutModeEnum.OvertimeOut)
            {
                cho2.ClockOut = sndr.LogTime;
                cho2.RefID02 = sndr.Oid;
                // Between BeginningOut & EndingOut range
                //if (sndr.LogTime.TimeOfDay >= cho2.TimeTable.BeginningOut.TimeOfDay && sndr.LogTime.TimeOfDay <= cho2.TimeTable.EndingOut.TimeOfDay)
                //{
                //    // ClockOut
                //    cho2.ClockOut = sndr.LogTime;
                //    cho2.RefID02 = sndr.Oid;
                //}
            }
            //else if (sndr == null)
            //{
            //    cho2.ClockOut = new DateTime(1753, 1, 1, 0, 0, 0);
            //    cho2.RefID02 = 0;
            //}
            cho2.Normal = cho2.TimeTable.CountAsWorkday;
            cho2.RealTime = cho2.Normal;
            cho2.NormalHours = cho2.TimeTable.OffDutyTime - cho2.TimeTable.OnDutyTime;
            cho2.Save();
            _ObjectSpace02.CommitChanges();

            //_ObjectSpace.CommitChanges();
            //ObjectSpace.CommitChanges();
            //ObjectSpace.ReloadObject(_CheckInOut);
            ObjectSpace.Refresh();
        }
        //private DeviceAttendanceLog thisAttLog = null;
        void appendClockInOutAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            //DevExpress.ExpressApp.View vw = this.View;
            GridListEditor listEditor = ((ListView)View).Editor as GridListEditor;
            GridView gridView = null;
            if (listEditor != null)
            {
                gridView = listEditor.GridView;
            }
            _FocusColumn = gridView.FocusedColumn;
            if (!new[] { "ClockIn", "ClockOut" }.Any(o => _FocusColumn.FieldName.Contains(o)))
            {
                XtraMessageBox.Show("Invalid selection of row column.", "Warning", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
            }
            _ObjectSpace = Application.CreateObjectSpace();
            //_CheckInOut = ((DevExpress.ExpressApp.ListView)this.View).CurrentObject as CheckInAndOut02;
            _CheckInOut = this.View.SelectedObjects[0] as CheckInAndOut02;
            if (_CheckInOut == null)
            {
                throw new ApplicationException("No entry selected");
            }
            DeviceAttendanceLog thisAttLog = null;
            CheckInAndOut02 thisCheckInOut = _ObjectSpace.GetObjectByKey<CheckInAndOut02>(_CheckInOut.Oid);
            if (_FocusColumn.FieldName == "ClockIn" && thisCheckInOut.RefID != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.RefID));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.ClockIn;
            }
            else if (_FocusColumn.FieldName == "ClockOut" && thisCheckInOut.RefID02 != 0)
            {
                thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.RefID02));
                thisAttLog.LogMode = thisAttLog.InOutMode;
                thisAttLog.LogDateTime = thisCheckInOut.ClockOut;
            }
            //else
            //{
            //    if (focusColumn.FieldName == "ClockIn")
            //    {
            //        thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.RefID));
            //        thisAttLog.LogMode = thisAttLog.InOutMode;
            //        thisAttLog.LogDateTime = thisCheckInOut.ClockIn;
            //    }
            //    else if (focusColumn.FieldName == "ClockOut")
            //    {
            //        thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.RefID02));
            //        thisAttLog.LogMode = thisAttLog.InOutMode;
            //        thisAttLog.LogDateTime = thisCheckInOut.ClockOut;
            //    }
            //    else if (thisCheckInOut.RefID02 != 0)
            //    {
            //        thisAttLog = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[Oid]=?", thisCheckInOut.RefID));
            //        thisAttLog.LogMode = thisAttLog.InOutMode;
            //        thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day, thisCheckInOut.OnDuty.Hour, thisCheckInOut.OnDuty.Minute, thisCheckInOut.OnDuty.Second);
            //    }
            //}
            if (thisAttLog == null)
            {
                thisAttLog = ReflectionHelper.CreateObject<DeviceAttendanceLog>(((ObjectSpace)_ObjectSpace).Session);
                if (_FocusColumn.FieldName == "ClockIn")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.CheckIn;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day, thisCheckInOut.OnDuty.Hour, thisCheckInOut.OnDuty.Minute, thisCheckInOut.OnDuty.Second);
                }
                else if (_FocusColumn.FieldName == "ClockOut")
                {
                    thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                    thisAttLog.LogMode = InOutModeEnum.CheckOut;
                    thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day, thisCheckInOut.OffDuty.Hour, thisCheckInOut.OffDuty.Minute, thisCheckInOut.OffDuty.Second);
                }
                else
                {
                    if (thisCheckInOut.TimeTable.DefaultMode == InOutModeEnum.CheckIn)
                    {
                        thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                        thisAttLog.LogMode = InOutModeEnum.CheckIn;
                        thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day, thisCheckInOut.OnDuty.Hour, thisCheckInOut.OnDuty.Minute, thisCheckInOut.OnDuty.Second);
                    }
                    else if (thisCheckInOut.TimeTable.DefaultMode == InOutModeEnum.CheckOut)
                    {
                        thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
                        thisAttLog.LogMode = InOutModeEnum.CheckOut;
                        thisAttLog.LogDateTime = new DateTime(thisCheckInOut.Date.Year, thisCheckInOut.Date.Month, thisCheckInOut.Date.Day, thisCheckInOut.OffDuty.Hour, thisCheckInOut.OffDuty.Minute, thisCheckInOut.OffDuty.Second);
                    }
                }
            }
            
            thisAttLog.EnrolledNo = thisCheckInOut.EnrolledNo;
            thisAttLog.EmployeeName = thisCheckInOut.Employee;
            thisAttLog.VerifyMode = VerifyModeEnum.System;
            _AttLogCopy = thisAttLog;
            e.View = Application.CreateDetailView(_ObjectSpace,
            "AppendClockInOut_DetailView", true, thisAttLog);
            //e.View.Closing += new EventHandler(View_Closing);
        }
    }
}
