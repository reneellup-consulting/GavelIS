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
    public partial class ChangeDateShiftController : ViewController
    {
        private PopupWindowShowAction ChangeDateShiftAction;
        public ChangeDateShiftController()
        {
            this.TargetObjectType = typeof(CheckInAndOut03);
            this.TargetViewType = ViewType.ListView;
            string actionID = "ChangeDateShiftActionId";
            this.ChangeDateShiftAction = new PopupWindowShowAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.ChangeDateShiftAction.Caption = "Change Shift";
            this.ChangeDateShiftAction.CustomizePopupWindowParams += new CustomizePopupWindowParamsEventHandler(ChangeDateShiftAction_CustomizePopupWindowParams);
            this.ChangeDateShiftAction.Execute += new PopupWindowShowActionExecuteEventHandler(ChangeDateShiftAction_Execute);
        }

        void ChangeDateShiftAction_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            foreach (var item in View.SelectedObjects)
            {
                CheckInAndOut03 chk3 = ObjectSpace.GetObject(item) as CheckInAndOut03;
                TimeTable2 ttbl = ObjectSpace.GetObject<TimeTable2>(e.PopupWindow.View.SelectedObjects[0] as TimeTable2);
                if (chk3.Oid < 0)
                {
                    return;
                }
                DateTime Date = chk3.Date;
                CheckInAndOut03 chkIO = chk3;
                chkIO.TimeTable = ttbl;
                chkIO.OnDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ttbl.OnDutyTime.Hour, ttbl.OnDutyTime.Minute, 0);  //ottbl.OnDutyTime;4
                chkIO.HalfDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ttbl.HalfDutyTime.Hour, ttbl.HalfDutyTime.Minute, 0);
                chkIO.OffDuty = new DateTime(chkIO.Date.Year, chkIO.Date.Month, chkIO.Date.Day, ttbl.OffDutyTime.Hour, ttbl.OffDutyTime.Minute, 0); //ott
                chkIO.RealTime = 0m;
                chkIO.ActualHours = TimeSpan.Zero;
                chkIO.Flexible = false;
                // Zero Set
                chkIO.NextDay2 = false;
                chkIO.OvertimeIn1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.OvertimeOut1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                //chkIO.ZeroSetHrs = TimeSpan.Zero;
                chkIO.Ref31 = 0;
                chkIO.Ref32 = 0;

                // First Set
                chkIO.ClockIn1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.BreakOut1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.BreakIn1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.ClockOut1 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.BreakClock1 = false;
                //chkIO.FirstSetHrs = TimeSpan.Zero;
                chkIO.Ref11 = 0;
                chkIO.Ref12 = 0;
                chkIO.Ref13 = 0;
                chkIO.Ref14 = 0;
                // Second Set
                chkIO.NextDay1 = false;
                chkIO.ClockIn2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.BreakOut2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.BreakIn2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.ClockOut2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.BreakClock2 = false;
                //chkIO.SecondSetHrs = TimeSpan.Zero;
                chkIO.Ref21 = 0;
                chkIO.Ref22 = 0;
                chkIO.Ref23 = 0;
                chkIO.Ref24 = 0;
                // Third Set
                chkIO.OvertimeIn2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                chkIO.OvertimeOut2 = new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
                //chkIO.ThirdSetHrs = TimeSpan.Zero;
                chkIO.Ref33 = 0;
                chkIO.Ref34 = 0;
                chkIO.ZeroNightHrs = TimeSpan.Zero;
                //chkIO.ThirdNightHrs = TimeSpan.Zero;
                ArrayList chiid = new ArrayList();
                XPCollection<DeviceAttendanceLog> devAtts = new XPCollection<DeviceAttendanceLog>(((ObjectSpace)this.ObjectSpace).Session);
                var devAttList = devAtts.Where(o => o.EnrolledNo == chkIO.EnrolledNo && o.LogTime.Date == chkIO.Date).OrderBy(o => o.DwYear & o.DwMonth & o.DwDay & o.DwHour & o.DwMinute);
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
                        if (chkIO.OvertimeIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                        {
                            zdt01 = chkIO.OvertimeIn1;
                        }
                        if (chkIO.OvertimeOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
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
                        if (chkIO.ClockIn1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                        {
                            odt01 = chkIO.ClockIn1;
                        }
                        if (chkIO.ClockOut1 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
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
                        if (chkIO.ClockIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                        {
                            sdt01 = chkIO.ClockIn2;
                        }
                        if (chkIO.ClockOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
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
                        if (chkIO.OvertimeIn2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
                        {
                            tdt01 = chkIO.OvertimeIn2;
                        }
                        if (chkIO.OvertimeOut2 != new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0))
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
                    }
                    #endregion
                }
                chkIO.DisapprovedOt = false;
                chkIO.ApprovedOt = false;
                chkIO.UnAltered = true;
                chkIO.References = chiid;
                chkIO.Save();
                //chk3.Save();
                ObjectSpace.CommitChanges();
            }
        }

        void ChangeDateShiftAction_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace objectSpace = Application.CreateObjectSpace();
            String listViewId = Application.FindListViewId(typeof(TimeTable2));
            CollectionSourceBase collectionSource = Application.
            CreateCollectionSource(objectSpace, typeof(TimeTable2), listViewId)
            ;
            e.View = Application.CreateListView(listViewId, collectionSource,
            true);
        }
    }
}
