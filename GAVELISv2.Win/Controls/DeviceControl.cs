using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.Xpo;
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors;
using GAVELISv2.Module.Editors;
using GAVELISv2.Module.BusinessObjects;
using GAVELISv2.Module.Win;

namespace GAVELISv2.Win.Controls
{
    public partial class DeviceControl : DevExpress.XtraEditors.XtraUserControl, IXpoSessionAwareControl {
        public zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();
        private BiometricDevice _BioDev;
        private Session _Session;
        private StringBuilder _StringBuilder = new StringBuilder();
        public DeviceControl() {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                ClearFields();
                btnConnect.Enabled = false;
                timer1.Start();
                timer2.Start();

                this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);

                UpdateDataSource();
            }
        }

        private void UpdateDataSource() {
            _Session = ((IXpoSessionAwareControl)this).Session;
            Guard.ArgumentNotNull(_Session, "session");
            LoadDeviceNames(_Session);
        }

        private void LoadDeviceNames(Session session) {
            using (XPCollection<BiometricDevice> cols = new XPCollection<BiometricDevice>(session))
            {
                if (cols != null && cols.Count > 0)
                {
                    ComboBoxItemCollection col1 = cboDevice.Properties.Items;
                    col1.BeginUpdate();
                    try
                    {
                        foreach (var item in cols)
                        {
                            col1.Add(item.DeviceName);
                        }
                    } finally
                    {
                        col1.EndUpdate();
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            dtDeviceDateTime.EditValue = dtDeviceDateTime.DateTime.AddSeconds(1);
        }

        Session IXpoSessionAwareControl.Session { get; set; }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e) {
            _BioDev = null;
            _BioDev = _Session.FindObject<BiometricDevice>(BinaryOperator.Parse("[DeviceName]=?", cboDevice.SelectedItem));
            ClearFields();
            if (_BioDev != null)
            {
                txtIPAddress.Text = _BioDev.IpAddress;
                //spinMachineNo.EditValue = bioDev.MachineNo;
                txtSerialNo.Text = _BioDev.SerialNo;
                txtPort.Text = !string.IsNullOrEmpty(_BioDev.Port.ToString()) ? _BioDev.Port.ToString() : "4370";
                dtDeviceDateTime.EditValue = DateTime.Now;
                btnConnect.Enabled = true;
            } else
            {
                cboDevice.Enabled = false;
                throw new ApplicationException("Cannot retreive details for the chosen device");
            }
        }

        private void ClearFields() {
            txtIPAddress.Text = string.Empty;
            spinMachineNo.EditValue = 1;
            txtSerialNo.Text = string.Empty;
            txtPort.Text = "4370";
            spinUserCount.EditValue = 0;
            spinAdminCount.EditValue = 0;
            spinFPCount.EditValue = 0;
            spinLogCount.EditValue = 0;
            dtDeviceDateTime.EditValue = DateTime.Now;
        }

        private void btnConnect_Click(object sender, EventArgs e) {
            //System.Windows.Forms.Application.DoEvents();
            ConnectDisconnectDevice();
        }

        private bool bIsConnected = false;//the boolean value identifies whether the device is connected
        private int iMachineNumber = 1;//the serial number of the device.After connecting the device ,this value will be changed.

        private void ConnectDisconnectDevice() {
            if (string.IsNullOrEmpty(txtIPAddress.Text) || string.IsNullOrEmpty(txtPort.Text))
            {
                XtraMessageBox.Show("IP Address and Port cannot be empty");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (btnConnect.Text == "Disconnect")
            {
                //timer2.Stop();
                axCZKEM1.Disconnect();
                bIsConnected = false;
                btnConnect.Text = "Connect";
                Cursor = Cursors.Default;
                cboDevice.Enabled = true;
                txtIPAddress.Enabled = true;
                txtPort.Enabled = true;
                spinMachineNo.Enabled = true;
                txtSerialNo.Enabled = true;
                btnReadDevice.Enabled = false;
                btnSyncTime.Enabled = false;
                btnGetAllUsers.Enabled = false;
                btnGetAttLog.Enabled = false;
                btnRestartDevice.Enabled = false;
                btnInitialDevice.Enabled = false;
                btnPowerOffDevice.Enabled = false;
                btnClearAdminPrivilege.Enabled = false;
                btnUploadUsers.Enabled = false;
                //btnGetAttLogsUsb.Enabled = false;
                return;
            }
            bIsConnected = axCZKEM1.Connect_Net(txtIPAddress.Text, Convert.ToInt32(txtPort.Text));
            if (bIsConnected == true)
            {
                //timer2.Start();
                this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                this.axCZKEM1.OnConnected -= new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                this.axCZKEM1.OnDisConnected -= new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                {
                    this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                    this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                    this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                    this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                    this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                }
                btnConnect.Text = "Disconnect";
                btnConnect.Refresh();
                iMachineNumber = 1;//In fact,when you are using the tcp/ip communication,this parameter will be ignored,that is any integer will all right.Here we use 1.
                //axCZKEM1.RegEvent(iMachineNumber, 65535);//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                cboDevice.Enabled = false;
                txtIPAddress.Enabled = false;
                txtPort.Enabled = false;
                spinMachineNo.Enabled = false;
                txtSerialNo.Enabled = false;
                btnReadDevice.Enabled = true;
                btnSyncTime.Enabled = true;
                btnGetAllUsers.Enabled = true;
                btnGetAttLog.Enabled = true;
                btnRestartDevice.Enabled = true;
                btnInitialDevice.Enabled = true;
                btnPowerOffDevice.Enabled = true;
                btnClearAdminPrivilege.Enabled = true;
                btnUploadUsers.Enabled = true;
                //btnGetAttLogsUsb.Enabled = true;
                spinUserCount.EditValue = 0;
                spinFPCount.EditValue = 0;
                spinAdminCount.EditValue = 0;
                spinLogCount.EditValue = 0;
                dtDeviceDateTime.EditValue = DateTime.Now;
                // Read Machine No and Serial No
                ReadMachineAndSerial();
                // Auto update Biometric Device record
                if (_BioDev != null)
                {
                    _BioDev.IpAddress = txtIPAddress.Text;
                    _BioDev.Port = Convert.ToInt32(txtPort.Text);
                    _BioDev.MachineNo = 1;
                    _BioDev.SerialNo = txtSerialNo.Text;
                    _BioDev.Save();
                    _Session.CommitTransaction();
                }
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Unable to connect the device,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void axCZKEM1_OnVerify2(int UserID) {
            if (UserID != -1)
            {
                _StringBuilder.AppendLine(" User is veified sucessfully.");
                memoEdit1.Text = _StringBuilder.ToString();
            } else
            {
                _StringBuilder.AppendLine(" User is not veified sucessfully.");
                memoEdit1.Text = _StringBuilder.ToString();
            }
        }

        private void axCZKEM1_OnConnected2() {
            _StringBuilder.AppendLine(" Machine connected event is triggered sucessfully.");
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void axCZKEM1_OnAttTransactionEx2(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode) {
            _StringBuilder.AppendFormat(" OnAttTrasactionEx Has been Triggered,Verified OK onDate :Enrollnumber{0}{1}", EnrollNumber, DateTime.Now);
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void axCZKEM1_OnAttTransaction2(int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second) {
            _StringBuilder.AppendFormat(" OnAttTrasactionEx Has been Triggered,Verified OK onDate :Enrollnumber{0}{1}", EnrollNumber, DateTime.Now);
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void axCZKEM1_OnDisConnected2() {
            _StringBuilder.AppendLine(" Machine is disconnected");
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void ReadMachineAndSerial() {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;

            string sdwSerialNumber = "";

            Cursor = Cursors.WaitCursor;
            if (axCZKEM1.GetSerialNumber(iMachineNumber, out sdwSerialNumber))
            {
                txtSerialNo.Text = sdwSerialNumber;
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void btnReadDevice_Click(object sender, EventArgs e) {
            //System.Windows.Forms.Application.DoEvents();
            ReadDeviceStatus();
        }

        private void ReadDeviceStatus() {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;
            int iValue = 0;

            Cursor = Cursors.WaitCursor;
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device

            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                spinLogCount.EditValue = iValue;
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 2, ref iValue))
            {
                spinUserCount.EditValue = iValue;
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 3, ref iValue))
            {
                spinFPCount.EditValue = iValue;
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 1, ref iValue))
            {
                spinAdminCount.EditValue = iValue;
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            // Get Device Time
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;

            if (axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))
            {
                // txtGetDeviceTime.Text = idwYear.ToString() + "-" + idwMonth.ToString() + "-" + idwDay.ToString() + " " + idwHour.ToString() + ":" + idwMinute.ToString() + ":" + idwSecond.ToString();
                dtDeviceDateTime.EditValue = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond);
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device

            // Auto update Biometric Device record
            if (_BioDev != null)
            {
                _BioDev.UserCount = Convert.ToInt32(spinUserCount.Value);
                _BioDev.FpCount = Convert.ToInt32(spinFPCount.Value);
                _BioDev.AdminCount = Convert.ToInt32(spinAdminCount.Value);
                _BioDev.LogCount = Convert.ToInt32(spinLogCount.Value);
                _BioDev.Save();
                _Session.CommitTransaction();
            }

            Cursor = Cursors.Default;
        }

        private void btnSyncTime_Click(object sender, EventArgs e) {
            //System.Windows.Forms.Application.DoEvents();
            SyncDateTime();
        }

        private void SyncDateTime() {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (axCZKEM1.SetDeviceTime(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                XtraMessageBox.Show("Successfully set the time of the machine and the terminal to sync PC!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                int idwYear = 0;
                int idwMonth = 0;
                int idwDay = 0;
                int idwHour = 0;
                int idwMinute = 0;
                int idwSecond = 0;
                if (axCZKEM1.GetDeviceTime(iMachineNumber, ref idwYear, ref idwMonth, ref idwDay, ref idwHour, ref idwMinute, ref idwSecond))//show the time
                {
                    dtDeviceDateTime.EditValue = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond);
                }
            } else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void axCZKEM1_OnVerify(int UserID)
        {
            if (UserID != -1)
            {
                _StringBuilder.AppendLine(" User is veified sucessfully.");
                memoEdit1.Text = _StringBuilder.ToString();
            }
            else
            {
                _StringBuilder.AppendLine(" User is not veified sucessfully.");
                memoEdit1.Text = _StringBuilder.ToString();
            }
        }

        private void axCZKEM1_OnConnected()
        {
            _StringBuilder.AppendLine(" Machine connected event is triggered sucessfully.");
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void axCZKEM1_OnAttTransactionEx(string EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second, int WorkCode)
        {
            _StringBuilder.AppendFormat(" OnAttTrasactionEx Has been Triggered,Verified OK onDate :Enrollnumber{0}{1}", EnrollNumber, DateTime.Now);
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void axCZKEM1_OnAttTransaction(int EnrollNumber, int IsInValid, int AttState, int VerifyMethod, int Year, int Month, int Day, int Hour, int Minute, int Second)
        {
            _StringBuilder.AppendFormat(" OnAttTrasactionEx Has been Triggered,Verified OK onDate :Enrollnumber{0}{1}", EnrollNumber, DateTime.Now);
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void axCZKEM1_OnDisConnected()
        {
            _StringBuilder.AppendLine(" Machine is disconnected");
            memoEdit1.Text = _StringBuilder.ToString();
        }

        private void timer2_Tick(object sender, EventArgs e) {
            //System.Windows.Forms.Application.DoEvents();
            //if (bIsConnected == true)
            //{
            //    //this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
            //    //this.axCZKEM1.OnConnected -= new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected);
            //    //this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction);
            //    //this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
            //    //this.axCZKEM1.OnDisConnected -= new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected);
            //    //if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            //    //{
            //    //    this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
            //    //    this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx);
            //    //    this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction);
            //    //    this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected);
            //    //    this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected);
            //    //}
            //    this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
            //    this.axCZKEM1.OnConnected -= new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
            //    this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
            //    this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
            //    this.axCZKEM1.OnDisConnected -= new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
            //    if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
            //    {
            //        this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
            //        this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
            //        this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
            //        this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
            //        this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
            //    }

            //}
        }

        private void btnGetAllUsers_Click(object sender, EventArgs e) {
            //System.Windows.Forms.Application.DoEvents();
            //GetAllDeviceUsers();
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first!", "Error");
                return;
            }
            else
            {
                this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                this.axCZKEM1.OnConnected -= new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                this.axCZKEM1.OnDisConnected -= new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                {
                    this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                    this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                    this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                    this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                    this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                }

            }

            int idwErrorCode = 0;
            int iValue = 0;

            string sdwEnrollNumber = "";
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;

            int idwFingerIndex;
            string sTmpData = "";
            int iTmpLength = 0;
            int iFlag = 0;

            ProgressForm frmProgress = new ProgressForm();
            frmProgress.Show(this.ParentForm);
            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 2, ref iValue))
            {
                frmProgress.ChangeRecordCount(iValue);
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            frmProgress.Text = "Retreiving details...";
            frmProgress.Refresh();

            axCZKEM1.EnableDevice(iMachineNumber, false);
            Cursor = Cursors.WaitCursor;

            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
            axCZKEM1.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory
            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
            {
                Employee emp = _Session.FindObject<Employee>(BinaryOperator.Parse("[EnrollNumber]=?", sdwEnrollNumber));
                if (emp!=null)
                {
                    for (idwFingerIndex = 0; idwFingerIndex < 10; idwFingerIndex++)
                    {
                        if (axCZKEM1.GetUserTmpExStr(iMachineNumber, sdwEnrollNumber, idwFingerIndex, out iFlag, out sTmpData, out iTmpLength))//get the corresponding templates string and length from the memory
                        {
                            EmployeeFingerprint fp = _Session.FindObject<EmployeeFingerprint>(BinaryOperator.Parse("[EmployeedId.EnrollNumber]=? And [FingerIndex]=?", sdwEnrollNumber, idwFingerIndex));
                            if (fp==null)
                            {
                                fp = new EmployeeFingerprint(_Session);
                                fp.EmployeedId = emp;
                                fp.FingerIndex = idwFingerIndex;
                            }
                            fp.TmpData = sTmpData;
                            fp.TmpLenght = iTmpLength;
                            fp.Save();
                        }
                    }
                    emp.Save();
                    _Session.CommitTransaction();
                }
                else
                {
                    axCZKEM1.EnableDevice(iMachineNumber, true);
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show(string.Format("User with Enroll #{0} from the device cannot be found in the system.", sdwEnrollNumber));
                    return;
                }
                frmProgress.DoProgress();
                frmProgress.Refresh();
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);
            Cursor = Cursors.Default;
            frmProgress.Close();
        }

        private void GetAllDeviceUsers() {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }

            string sdwEnrollNumber = "";
            string sName = "";
            string sPassword = "";
            int iPrivilege = 0;
            bool bEnabled = false;

            //int idwFingerIndex;
            //string sTmpData = "";
            //int iTmpLength = 0;
            //int iFlag = 0;

            //lvDownload.Items.Clear();
            gridControl1.DataSource = null;
            gridControl1.ForceInitialize();
            //lvDownload.BeginUpdate();
            axCZKEM1.EnableDevice(iMachineNumber, false);
            Cursor = Cursors.WaitCursor;

            axCZKEM1.ReadAllUserID(iMachineNumber);//read all the user information to the memory
            axCZKEM1.ReadAllTemplate(iMachineNumber);//read all the users' fingerprint templates to the memory
            while (axCZKEM1.SSR_GetAllUserInfo(iMachineNumber, out sdwEnrollNumber, out sName, out sPassword, out iPrivilege, out bEnabled))//get all the users' information from the memory
            {
                AllUserInfoFromDevice dUser = _Session.FindObject<AllUserInfoFromDevice>(BinaryOperator.Parse("[EnrolledNo]=?", sdwEnrollNumber));
                if (dUser == null)
                {
                    dUser = new AllUserInfoFromDevice(_Session);
                }
                dUser.EnrolledNo = sdwEnrollNumber;
                dUser.EnrolledName = sName;
                dUser.Privilege = iPrivilege;
                dUser.Enabled = bEnabled;
                dUser.Save();
            }
            _Session.CommitTransaction();
            gridControl1.DataSource = new XPCollection<AllUserInfoFromDevice>(_Session);
            gridControl1.ForceInitialize();
            //lvDownload.EndUpdate();
            axCZKEM1.EnableDevice(iMachineNumber, true);
            Cursor = Cursors.Default;
        }

        private void btnGetAttLog_Click(object sender, EventArgs e) {
            //System.Windows.Forms.Application.DoEvents();
            GetAttendanceLogs();
        }

        private int GetAttendanceLogCount() {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return 0;
            }
            else
            {
                this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                this.axCZKEM1.OnConnected -= new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                this.axCZKEM1.OnDisConnected -= new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                {
                    this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                    this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                    this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                    this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                    this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                }

            }
            int idwErrorCode = 0;
            int iValue = 0;

            Cursor = Cursors.WaitCursor;
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device

            if (axCZKEM1.GetDeviceStatus(iMachineNumber, 6, ref iValue)) //Here we use the function "GetDeviceStatus" to get the record's count.The parameter "Status" is 6.
            {
                axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
                Cursor = Cursors.Default;
                return iValue;
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                return 0;
            }
        }

        private void GetAttendanceLogs() {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }

            string sdwEnrollNumber = "";
            int idwTMachineNumber = 0;
            int idwEMachineNumber = 0;
            int idwVerifyMode = 0;
            int idwInOutMode = 0;
            int idwYear = 0;
            int idwMonth = 0;
            int idwDay = 0;
            int idwHour = 0;
            int idwMinute = 0;
            int idwSecond = 0;
            int idwWorkcode = 0;

            int idwErrorCode = 0;
            int iGLCount = 0;
            int iIndex = 0;

            Cursor = Cursors.WaitCursor;
            ProgressForm frmProgress = new ProgressForm();
            int index = 0;
            frmProgress.Show(this.ParentForm);
            int logCount = GetAttendanceLogCount();
            frmProgress.ChangeRecordCount(logCount);
            frmProgress.Text = "Retreiving details...";
            frmProgress.Refresh();
            gridControl2.DataSource = null;
            gridControl2.ForceInitialize();
            axCZKEM1.EnableDevice(iMachineNumber, false);//disable the device
            //if (axCZKEM1.ReadGeneralLogData(iMachineNumber))
            if (axCZKEM1.ReadGeneralLogData(iMachineNumber))//read all the attendance records to the memory
            {
                // while (axCZKEM1.SSR_GetGeneralLogData
                while (axCZKEM1.SSR_GetGeneralLogData(iMachineNumber, out sdwEnrollNumber, out idwVerifyMode,
                out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, ref idwWorkcode))//get records from the memory
                {
                    index++;
                    DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
                    // [Enrolled No] = '10277' And [In Out Mode] = 'Check-In' And [Log Time] = #2018-08-30#
                    string tmpId = string.Format("{0} {1} {2}", sdwEnrollNumber, idwInOutMode, (new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond)).ToString("d MM yyy HH:mm:ss"));
                    DateTime logt = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond);
                    //DeviceAttendanceLog atLog = _Session.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse("[LogId]=?", tmpId));
                    InOutModeEnum inout = idwInOutMode == 0 ? InOutModeEnum.CheckIn : idwInOutMode == 1 ? InOutModeEnum.CheckOut : idwInOutMode == 2 ? InOutModeEnum.BreakOut : idwInOutMode == 3 ? InOutModeEnum.BreakIn : idwInOutMode == 4 ? InOutModeEnum.OvertimeIn : idwInOutMode == 5 ? InOutModeEnum.OvertimeOut : 0;
                    string crit = string.Format("[EnrolledNo] = '{0}' And [InOutMode] = '{1}' And [LogTime] = #{2}#", sdwEnrollNumber, GAVELISv2.Module.Win.EnumExtensions.GetDisplayName(inout), logt);
                    DeviceAttendanceLog atLog = _Session.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse(crit));
                    if (atLog == null)
                    {
                        atLog = new DeviceAttendanceLog(_Session);
                    }
                    atLog.EnrolledNo = sdwEnrollNumber;
                    Employee emp = _Session.FindObject<Employee>(BinaryOperator.Parse("[EnrollNumber]=?", sdwEnrollNumber));
                    if (emp != null)
                    {
                        atLog.EmployeeName = emp.Name;
                    }
                    else
                    {
                        atLog.EmployeeName = "No matching enrolled number";
                    }
                    atLog.VerifyMode = idwVerifyMode==0?VerifyModeEnum.Password:idwVerifyMode==1?VerifyModeEnum.Fingerprint:0;
                    atLog.InOutMode = idwInOutMode==0?InOutModeEnum.CheckIn:idwInOutMode==1?InOutModeEnum.CheckOut:idwInOutMode==2?InOutModeEnum.BreakOut:idwInOutMode==3?InOutModeEnum.BreakIn:idwInOutMode==4?InOutModeEnum.OvertimeIn:idwInOutMode==5?InOutModeEnum.OvertimeOut:0;
                    atLog.LogTime = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond);
                    atLog.DwYear = idwYear;
                    atLog.DwMonth = idwMonth;
                    atLog.DwDay = idwDay;
                    atLog.DwHour = idwHour;
                    atLog.DwMinute = idwMinute;
                    atLog.DwSecond = idwSecond;
                    atLog.DwWorkCode = idwWorkcode;
                    atLog.LogId = tmpId;
                    atLog.SortLogTime = string.Format(myDTFI.UniversalSortableDateTimePattern, atLog.LogTime);
                    atLog.Save();
                    _Session.CommitTransaction();
                    frmProgress.DoProgress();
                    frmProgress.Refresh();
                    //System.Threading.Thread.Sleep(300);
                }
            } else
            {
                Cursor = Cursors.Default;
                axCZKEM1.GetLastError(ref idwErrorCode);
                if (idwErrorCode != 0)
                {
                    frmProgress.Close();
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show("Reading data from terminal failed,ErrorCode: " + idwErrorCode.ToString(), "Error");
                } else
                {
                    frmProgress.Close();
                    Cursor = Cursors.Default;
                    XtraMessageBox.Show("No data from terminal returns!");
                }
            }
            axCZKEM1.EnableDevice(iMachineNumber, true);//enable the device
            Cursor = Cursors.Default;
            gridControl2.DataSource = new XPCollection<DeviceAttendanceLog>(_Session);
            gridControl2.ForceInitialize();
            frmProgress.Close();
        }

        private void btnRestartDevice_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (axCZKEM1.RestartDevice(iMachineNumber) == true)
            {
                XtraMessageBox.Show("The device will restart!", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void btnInitialDevice_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (axCZKEM1.ClearKeeperData(iMachineNumber))
            {
                XtraMessageBox.Show("Device was initialized", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void btnPowerOffDevice_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (axCZKEM1.PowerOffDevice(iMachineNumber))
            {
                XtraMessageBox.Show("PowerOffDevice", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void btnClearAdminPrivilege_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first");
                return;
            }
            int idwErrorCode = 0;

            Cursor = Cursors.WaitCursor;
            if (axCZKEM1.ClearAdministrators(iMachineNumber))
            {
                axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                MessageBox.Show("Successfully clear administrator privilege from teiminal!", "Success");
            }
            else
            {
                axCZKEM1.GetLastError(ref idwErrorCode);
                Cursor = Cursors.Default;
                XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
            }
            Cursor = Cursors.Default;
        }

        private void btnUploadUsers_Click(object sender, EventArgs e)
        {
            if (bIsConnected == false)
            {
                XtraMessageBox.Show("Please connect the device first!", "Error");
                return;
            }
            else
            {
                this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                this.axCZKEM1.OnConnected -= new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                this.axCZKEM1.OnAttTransaction -= new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                this.axCZKEM1.OnDisConnected -= new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                if (axCZKEM1.RegEvent(1, 65535))//Here you can register the realtime events that you want to be triggered(the parameters 65535 means registering all)
                {
                    this.axCZKEM1.OnVerify += new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify2);
                    this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(axCZKEM1_OnAttTransactionEx2);
                    this.axCZKEM1.OnAttTransaction += new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler(axCZKEM1_OnAttTransaction2);
                    this.axCZKEM1.OnConnected += new zkemkeeper._IZKEMEvents_OnConnectedEventHandler(axCZKEM1_OnConnected2);
                    this.axCZKEM1.OnDisConnected += new zkemkeeper._IZKEMEvents_OnDisConnectedEventHandler(axCZKEM1_OnDisConnected2);
                }

            }

            XPCollection<Employee> employees = new XPCollection<Employee>(_Session);
            var list = employees.Where(o => !string.IsNullOrEmpty(o.EnrollNumber));
            if (list != null && list.Count() == 0)
            {
                XtraMessageBox.Show("There is no data to upload!", "Error");
                return;
            }
            int idwErrorCode = 0;

            string sdwEnrollNumber = "";
            string sName = "";
            int idwFingerIndex = 0;
            string sTmpData = "";
            int iPrivilege = 0;
            string sPassword = "";
            string sEnabled = "";
            bool bEnabled = false;
            int iFlag = 1;

            int iUpdateFlag = 1;

            Cursor = Cursors.WaitCursor;
            ProgressForm frmProgress = new ProgressForm();
            frmProgress.Show(this.ParentForm);
            frmProgress.ChangeRecordCount(list.Count());
            frmProgress.Text = "Uploading profiles...";
            frmProgress.Refresh();
            axCZKEM1.EnableDevice(iMachineNumber, false);
            if (axCZKEM1.BeginBatchUpdate(iMachineNumber, iUpdateFlag))//create memory space for batching data
            {
                //string sLastEnrollNumber = "";//the former enrollnumber you have upload(define original value as 0)
                foreach (var item in list)
                {
                    sdwEnrollNumber = item.EnrollNumber;
                    sName = item.Name;
                    iPrivilege = (int)item.Privilege;
                    if (item.DeleteOnDevices)
                    {
                        sPassword = string.Empty;
                    }
                    else
                    {
                        sPassword = item.Password;
                    }
                    bEnabled = item.Enabled;
                    iFlag = item.Flag?1:0;
                    if (axCZKEM1.SSR_SetUserInfo(iMachineNumber, sdwEnrollNumber, sName, sPassword, iPrivilege, bEnabled))//upload user information to the memory
                    {
                        foreach (EmployeeFingerprint fp in item.EmployeeFingerprintTemplates)
                        {
                            axCZKEM1.SetUserTmpExStr(iMachineNumber, sdwEnrollNumber, fp.FingerIndex, iFlag, fp.TmpData);
                        }
                        if (item.EmployeeFingerprintTemplates.Count>0 && axCZKEM1.EnableUser(iMachineNumber, Convert.ToInt32(sdwEnrollNumber), iMachineNumber, 12, bEnabled))
                        {
                            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
                        }
                    }
                    else
                    {
                        axCZKEM1.GetLastError(ref idwErrorCode);
                        XtraMessageBox.Show("Operation failed,ErrorCode=" + idwErrorCode.ToString(), "Error");
                        Cursor = Cursors.Default;
                        axCZKEM1.EnableDevice(iMachineNumber, true);
                        frmProgress.Close();
                        return;
                    }
                    if (item.DeleteOnDevices)
                    {
                        if (axCZKEM1.SSR_DeleteEnrollData(iMachineNumber, sdwEnrollNumber, 12))
                        {
                        }
                    }

                    frmProgress.DoProgress();
                    frmProgress.Refresh();
                }
            }
            axCZKEM1.BatchUpdate(iMachineNumber);//upload all the information in the memory
            axCZKEM1.RefreshData(iMachineNumber);//the data in the device should be refreshed
            Cursor = Cursors.Default;
            axCZKEM1.EnableDevice(iMachineNumber, true);
            frmProgress.Close();
            XtraMessageBox.Show(string.Format("Successfully upload fingerprint templates in batches , total:{0}", list.Count()), "Success");
        }

        private void btnGetAttLogsUsb_Click(object sender, EventArgs e)
        {
            UDisk udisk = new UDisk();

            byte[] byDataBuf = null;
            int iLength;//length of the bytes to get from the data

            string sPIN2 = "";
            string sVerified = "";
            string sTime_second = "";
            string sDeviceID = "";
            string sStatus = "";
            string sWorkcode = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.OpenOrCreate, FileAccess.Read);
                byDataBuf = File.ReadAllBytes(openFileDialog1.FileName);
                iLength = Convert.ToInt32(stream.Length);

                int idwYear = 0;
                int idwMonth = 0;
                int idwDay = 0;
                int idwHour = 0;
                int idwMinute = 0;
                int idwSecond = 0;

                Cursor = Cursors.WaitCursor;
                ProgressForm frmProgress = new ProgressForm();
                frmProgress.Show(this.ParentForm);
                frmProgress.ChangeRecordCount(iLength);
                frmProgress.Text = "Importing deta...";
                frmProgress.Refresh();
                gridControl2.DataSource = null;
                gridControl2.ForceInitialize();
                int iStartIndex = 0;
                int iOneLogLength;//the length of one line of attendence log
                for (int i = iStartIndex; i < iLength - 2; i++)//modify by darcy on Dec.4 2009
                {
                    if (byDataBuf[i] == 13 && byDataBuf[i + 1] == 10)
                    {
                        iOneLogLength = (i + 1) + 1 - iStartIndex;
                        byte[] bySSRAttLog = new byte[iOneLogLength];
                        Array.Copy(byDataBuf, iStartIndex, bySSRAttLog, 0, iOneLogLength);

                        udisk.GetAttLogFromDat(bySSRAttLog, iOneLogLength, out sPIN2, out sTime_second, out sDeviceID, out sStatus, out sVerified, out sWorkcode);

                        string[] split1 = sTime_second.Split();
                        // Date
                        string strDate = split1[0];
                        string[] splitDate = strDate.Split('-');
                        idwYear = Convert.ToInt16(splitDate[0]);
                        idwMonth = Convert.ToInt16(splitDate[1]);
                        idwDay = Convert.ToInt16(splitDate[2]);
                        // Time
                        string strTime = split1[1];
                        string[] splitTime = strTime.Split(':');
                        idwHour = Convert.ToInt16(splitTime[0]);
                        idwMinute = Convert.ToInt16(splitTime[1]);
                        idwSecond = Convert.ToInt16(splitTime[2]);

                        DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
                        string tmpId = string.Format("{0} {1} {2}", sPIN2, sStatus, (new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond)).ToString("d MM yyy HH:mm:ss"));
                        DateTime logt = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond);
                        InOutModeEnum inout = sStatus == "0" ? InOutModeEnum.CheckIn : sStatus == "1" ? InOutModeEnum.CheckOut : sStatus == "2" ? InOutModeEnum.BreakOut : sStatus == "3" ? InOutModeEnum.BreakIn : sStatus == "4" ? InOutModeEnum.OvertimeIn : sStatus == "5" ? InOutModeEnum.OvertimeOut : 0;
                        string crit = string.Format("[EnrolledNo] = '{0}' And [InOutMode] = '{1}' And [LogTime] = #{2}#", sPIN2, GAVELISv2.Module.Win.EnumExtensions.GetDisplayName(inout), logt);
                        DeviceAttendanceLog atLog = _Session.FindObject<DeviceAttendanceLog>(BinaryOperator.Parse(crit));
                        if (atLog == null)
                        {
                            atLog = new DeviceAttendanceLog(_Session);
                        }
                        atLog.EnrolledNo = sPIN2;
                        Employee emp = _Session.FindObject<Employee>(BinaryOperator.Parse("[EnrollNumber]=?", sPIN2));
                        if (emp != null)
                        {
                            atLog.EmployeeName = emp.Name;
                        }
                        else
                        {
                            atLog.EmployeeName = "No matching enrolled number";
                        }

                        //atLog.VerifyMode = Convert.ToInt16(sVerified);
                        atLog.VerifyMode = Convert.ToInt16(sVerified) == 0 ? VerifyModeEnum.Password : Convert.ToInt16(sVerified) == 1 ? VerifyModeEnum.Fingerprint : 0;
                        //atLog.InOutMode = Convert.ToInt16(sStatus);
                        atLog.InOutMode = Convert.ToInt16(sStatus) == 0 ? InOutModeEnum.CheckIn : Convert.ToInt16(sStatus) == 1 ? InOutModeEnum.CheckOut : Convert.ToInt16(sStatus) == 2 ? InOutModeEnum.BreakOut : Convert.ToInt16(sStatus) == 3 ? InOutModeEnum.BreakIn : Convert.ToInt16(sStatus) == 4 ? InOutModeEnum.OvertimeIn : Convert.ToInt16(sStatus) == 5 ? InOutModeEnum.OvertimeOut : 0;
                        atLog.LogTime = new DateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond);
                        atLog.DwYear = idwYear;
                        atLog.DwMonth = idwMonth;
                        atLog.DwDay = idwDay;
                        atLog.DwHour = idwHour;
                        atLog.DwMinute = idwMinute;
                        atLog.DwSecond = idwSecond;
                        atLog.DwWorkCode = Convert.ToInt16(sWorkcode);
                        atLog.LogId = tmpId;
                        atLog.SortLogTime = atLog.LogTime.ToString(myDTFI.UniversalSortableDateTimePattern);// string.Format(myDTFI.UniversalSortableDateTimePattern, atLog.LogTime);
                        atLog.Save();
                        _Session.CommitTransaction();

                        bySSRAttLog = null;
                        iStartIndex += iOneLogLength;
                        iOneLogLength = 0;
                    }

                    frmProgress.DoProgress();
                    frmProgress.Refresh();
                }
                stream.Close();
                Cursor = Cursors.Default;
                gridControl2.DataSource = new XPCollection<DeviceAttendanceLog>(_Session);
                gridControl2.ForceInitialize();
                frmProgress.Close();
            }
        }

    }
}
