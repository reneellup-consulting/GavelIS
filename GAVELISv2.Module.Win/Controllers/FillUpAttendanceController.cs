using System;
using System.Globalization;
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
    public partial class FillUpAttendanceController : ViewController
    {
        private SimpleAction fillupAttendanceAction;
        private AttendanceCalculator _AttendanceCalculator;
        private System.ComponentModel.BackgroundWorker _BgWorker;
        private ProgressForm _FrmProgress;
        public FillUpAttendanceController()
        {
            this.TargetObjectType = typeof(AttendanceCalculator);
            this.TargetViewType = ViewType.DetailView;
            string actionID = "FillupAttendanceActionId2";
            this.fillupAttendanceAction = new SimpleAction(this, actionID,
            PredefinedCategory.RecordEdit);
            this.fillupAttendanceAction.Caption = "Fillup Manual";
            this.fillupAttendanceAction.Execute += new SimpleActionExecuteEventHandler(fillupAttendanceAction_Execute);
        }
        private void fillupAttendanceAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            _AttendanceCalculator = ((DevExpress.ExpressApp.DetailView)this.View).
            CurrentObject as AttendanceCalculator;
            this.ObjectSpace.CommitChanges();
            XPCollection<Employee> emps = new XPCollection<Employee>(((ObjectSpace)ObjectSpace).Session);
            //var empsList = emps.Where(o => o.StaffPayroll == true && o.EnrollNumber== "10028").OrderBy(o => o.Name);
            var empsList = emps.Where(o => o.StaffPayroll == true && o.Inactive == false && o.ManualAttLog == true).OrderBy(o => o.Name);
            if (empsList.Count() == 0)
            {
                XtraMessageBox.Show("There are no employees for manual log to process.", "Attention",
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Exclamation);
                return;
            }

            _FrmProgress = new ProgressForm("Filling...", empsList.Count(),
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
        private IObjectSpace _ObjectSpace;
        private void BgWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            int index = 0;
            UnitOfWork session = CreateUpdatingSession();
            IOrderedEnumerable<Employee> trans = (IOrderedEnumerable<Employee>)e.Argument;
            //XPCollection<DeviceAttendanceLog> devAtts = new XPCollection<DeviceAttendanceLog>(session);
            AttendanceCalculator att = session.GetObjectByKey<AttendanceCalculator>(_AttendanceCalculator.Oid);
            _ObjectSpace = Application.CreateObjectSpace();
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
                    TimeTable2 ottbl = session.GetObjectByKey<TimeTable2>(item.Shift.Oid);
                    do
                    {
                        if (item.Shift == null)
                        {
                            startDate = startDate.AddDays(1);
                            continue;
                        }
                        if (item.RestDay == startDate.DayOfWeek)
                        {
                            startDate = startDate.AddDays(1);
                            continue;
                        }
                        DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
                        // CheckIn
                        string LogId1 = string.Format("{0} {1} {2}", item.EnrollNumber, (int)InOutModeEnum.CheckIn, (new DateTime(startDate.Year, startDate.Month, startDate.Day,
                            ottbl.OnDutyTime.Hour, ottbl.OnDutyTime.Minute,
                            ottbl.OnDutyTime.Second)).ToString("G", myDTFI));
                        var l1 = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[LogId]=?", LogId1));
                        if (l1 == null)
                        {
                            DeviceAttendanceLog thisAttLog1 = null;
                            thisAttLog1 = ReflectionHelper.CreateObject<DeviceAttendanceLog>(((ObjectSpace)_ObjectSpace).Session);
                            thisAttLog1.EnrolledNo = item.EnrollNumber;
                            thisAttLog1.EmployeeName = item.Name;
                            thisAttLog1.VerifyMode = VerifyModeEnum.System;
                            thisAttLog1.LogMode = InOutModeEnum.CheckIn;
                            thisAttLog1.LogDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                                ottbl.OnDutyTime.Hour, ottbl.OnDutyTime.Minute,
                                ottbl.OnDutyTime.Second);
                            thisAttLog1.Save();
                        }
                        // CheckOut
                        string LogId2 = string.Format("{0} {1} {2}", item.EnrollNumber, (int)InOutModeEnum.CheckOut, (new DateTime(startDate.Year, startDate.Month, startDate.Day,
                            ottbl.OffDutyTime.Hour, ottbl.OffDutyTime.Minute,
                            ottbl.OffDutyTime.Second)).ToString("G", myDTFI));
                        var l2 = _ObjectSpace.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[LogId]=?", LogId2));
                        if (l2 == null)
                        {
                            DeviceAttendanceLog thisAttLog2 = null;
                            thisAttLog2 = ReflectionHelper.CreateObject<DeviceAttendanceLog>(((ObjectSpace)_ObjectSpace).Session);
                            thisAttLog2.EnrolledNo = item.EnrollNumber;
                            thisAttLog2.EmployeeName = item.Name;
                            thisAttLog2.VerifyMode = VerifyModeEnum.System;
                            thisAttLog2.LogMode = InOutModeEnum.CheckOut;
                            thisAttLog2.LogDateTime = new DateTime(startDate.Year, startDate.Month, startDate.Day,
                                ottbl.OffDutyTime.Hour, ottbl.OffDutyTime.Minute,
                                ottbl.OffDutyTime.Second);
                            thisAttLog2.Save();
                        }
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
                    _ObjectSpace.CommitChanges();
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
                    "Filling up attaendance is cancelled.", "Cancelled",
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
                    "Filling up attaendance has been successfull.");
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
