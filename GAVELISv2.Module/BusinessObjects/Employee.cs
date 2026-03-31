using System;
using System.Linq;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    public enum PrivilegeEnum
    {
        Common = 0,
        Enroller = 1,
        Administrator = 2,
        SuperAdministrator = 3
    }
    public enum CiviStatusTypeEnum
    {
        Single,
        Married,
        Separated,
        Divorced,
        Widowed
    }
    public enum POApproverPermissionEnum
    {
        All = 0,
        General = 1,
        Fuel = 2,
        PurchaserAll = 3,
        PurchaserGeneral = 4,
        PurchaserFuel = 5
    }

    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [OptimisticLocking(false)]
    public class Employee : Contact {
        private string _IDNo;
        private DateTime _DateHired;
        private EmployeeClassCode _EmployeeClassCode;
        private Positions _Position;
        private bool _POApprover = false;
        private string _POPassword;
        private int _intID;
        private string _TIN;
        private string _SSSNo;
        private string _Philhealth;
        private string _PagIbig;
        private DateTime _BirthDate;
        private string _BloodType;
        private CiviStatusTypeEnum _CivilStatus;
        private string _Religion;
        private string _EmergencyContactName;
        private string _EmergencyContactNo;
        private string _EmergencyContactAddress;
        private EmploymentStatusEnum _Status;
        private bool _Inactive = false;
        private DateTime _DateOfInactivity;
        private string _ReasonOfInactivity;
        private DriverClass _DriverClass;
        private DriverClassification _DriverClassification;
        private EmployeePayTypeEnum _PayType;
        private decimal _Basic;
        private DayOfWeek _RestDay;
        private ShiftClassEnum _Shifting;
        private TimeTable2 _Shift;
        public string IDNo {
            get { return _IDNo; }
            set
            {
                SetPropertyValue("IDNo", ref _IDNo, value);
            }
        }

        public DateTime DateHired {
            get { return _DateHired; }
            set
            {
                SetPropertyValue("DateHired", ref _DateHired, value);
            }
        }

        public EmployeeClassCode EmployeeClassCode
        {
            get { return _EmployeeClassCode; }
            set
            {
                SetPropertyValue("EmployeeClassCode", ref _EmployeeClassCode,
                value);
                if (!IsLoading && !IsSaving && _EmployeeClassCode != null)
                {
                    StaffPayroll = _EmployeeClassCode.StaffPayroll;
                }
            }
        }

        public Positions Position {
            get { return _Position; }
            set
            {
                SetPropertyValue("Position", ref _Position, value);
            }
        }

        public bool POApprover {
            get { return _POApprover; }
            set
            {
                SetPropertyValue<bool>("POApprover", ref _POApprover, value);
            }
        }

        public POApproverPermissionEnum POApproverPermission
        {
            get { return _POApproverPermission; }
            set
            {
                SetPropertyValue("POApproverPermission", ref _POApproverPermission, value);
            }
        }

        // is_claimed BIT DEFAULT 0, -- 0 = Available, 1 = Taken
        [Custom("AllowEdit", "False")]
        public bool POApproverKeyClaimed
        {
            get { return _POApproverKeyClaimed; }
            set
            {
                SetPropertyValue<bool>("POApproverKeyClaimed", ref _POApproverKeyClaimed, value);
            }
        }

        // appwrite_user_id NVARCHAR(255) NULL -- To track who took it
        [Size(500)]
        [Custom("AllowEdit", "False")]
        public string AppWriteUserId
        {
            get { return _AppWriteUserId; }
            set
            {
                SetPropertyValue<string>("AppWriteUserId", ref _AppWriteUserId, value);
            }
        }

        // LastUpdated DATETIME DEFAULT GETUTCDATE()
        [Custom("AllowEdit", "False")]
        public string AppWriteLastUpdated
        {
            get { return _AppWriteLastUpdated; }
            set
            {
                SetPropertyValue<string>("AppWriteLastUpdated", ref _AppWriteLastUpdated, value);
            }
        }

        public string POPassword {
            get { return _POPassword; }
            set
            {
                SetPropertyValue<string>("POPassword", ref _POPassword, value);
            }
        }
        public override string No
        {
            get
            {
                return base.No;
            }
            set
            {
                base.No = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }
        public int IntID {
            get { return _intID; }
            set
            {
                SetPropertyValue<int>("IntID", ref _intID, value);
            }
        }

        [Size(SizeAttribute.Unlimited),
        Delayed(true),
        ValueConverter(typeof(ImageValueConverter))]
        public Image Photo {
            get { return GetDelayedPropertyValue<Image>("Photo"); }
            set
            {
                SetDelayedPropertyValue<Image>("Photo", value);
            }
        }

        private string _PhotoURL;
        [Size(SizeAttribute.Unlimited)]
        public string PhotoURL {
            get { return _PhotoURL; }
            set
            {
                SetPropertyValue<string>("PhotoURL", ref _PhotoURL, value);
            }
        }

        [Action(Caption = "Photo to Base64")]
        public void PhotoToBase64() {
            string base64String = null;
            if (Photo == null)
            {
                return;
            }
            using (MemoryStream ms = new MemoryStream())
            {
                Photo.Save(ms, Photo.RawFormat);
                byte[] imageBytes = ms.ToArray();
                base64String = Convert.ToBase64String(imageBytes);
                PhotoURL = string.Format("data:image/png;base64,{0}", base64String);
            }
        }
        [Custom("AllowEdit", "False")]
        public bool UpdateOnline
        {
            get { return _UpdateOnline; }
            set { SetPropertyValue("UpdateOnline", ref _UpdateOnline, value); }
        }
        public string TIN {
            get { return _TIN; }
            set
            {
                SetPropertyValue("TIN", ref _TIN, value);
            }
        }

        public string SSSNo {
            get { return _SSSNo; }
            set
            {
                SetPropertyValue("SSSNo", ref _SSSNo, value);
            }
        }

        public string Philhealth {
            get { return _Philhealth; }
            set
            {
                SetPropertyValue("Philhealth", ref _Philhealth, value);
            }
        }
        [DisplayName("Pag-Ibig")]
        public string PagIbig
        {
            get { return _PagIbig; }
            set
            {
                SetPropertyValue("PagIbig", ref _PagIbig, value);
            }
        }

        public DateTime BirthDate
        {
            get { return _BirthDate; }
            set
            {
                SetPropertyValue("BirthDate", ref _BirthDate, value);
            }
        }
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string BloodType
        {
            get { return _BloodType; }
            set
            {
                SetPropertyValue("BloodType", ref _BloodType, value);
            }
        }

        public CiviStatusTypeEnum CivilStatus
        {
            get { return _CivilStatus; }
            set
            {
                SetPropertyValue("CivilStatus", ref _CivilStatus, value);
            }
        }
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Religion
        {
            get { return _Religion; }
            set
            {
                SetPropertyValue("Religion", ref _Religion, value);
            }
        }
        public string EmergencyContactName
        {
            get { return _EmergencyContactName; }
            set
            {
                SetPropertyValue("EmergencyContactName", ref _EmergencyContactName, value);
            }
        }
        [Custom("EditMask", "(999)000-0000 Ext. 9999")]
        public string EmergencyContactNo
        {
            get { return _EmergencyContactNo; }
            set { SetPropertyValue("EmergencyContactNo", ref _EmergencyContactNo, value); }
        }
        [Size(SizeAttribute.Unlimited)]
        public string EmergencyContactAddress
        {
            get { return _EmergencyContactAddress; }
            set { SetPropertyValue("EmergencyContactAddress", ref _EmergencyContactAddress, value); }
        }
        public EmploymentStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
            }
        }

        public bool Inactive {
            get { return _Inactive; }
            set
            {
                SetPropertyValue("Inactive", ref _Inactive, value);
                if (!IsLoading && !IsSaving)
                {
                    UpdateOnline = true;
                }
            }
        }

        public DateTime DateOfInactivity {
            get { return _DateOfInactivity; }
            set
            {
                SetPropertyValue("DateOfInactivity", ref _DateOfInactivity,
                value);
            }
        }

        public string ReasonOfInactivity {
            get { return _ReasonOfInactivity; }
            set
            {
                SetPropertyValue("ReasonOfInactivity", ref _ReasonOfInactivity
                , value);
            }
        }

        public DriverClass DriverClass {
            get { return _DriverClass; }
            set
            {
                SetPropertyValue("DriverClass", ref _DriverClass, value);
            }
        }

        public DriverClassification DriverClassification {
            get { return _DriverClassification; }
            set
            {
                SetPropertyValue("DriverClassification", ref
                _DriverClassification, value);
            }
        }

        public EmployeePayTypeEnum PayType {
            get { return _PayType; }
            set
            {
                SetPropertyValue("PayType", ref _PayType, value);
            }
        }

        public decimal Basic {
            get { return _Basic; }
            set
            {
                SetPropertyValue("Basic", ref _Basic, value);
            }
        }
        public decimal Allowance
        {
            get { return _Allowance; }
            set
            {
                SetPropertyValue("Allowance", ref _Allowance, value);
            }
        }
        public decimal NightAllowance
        {
            get { return _NightAllowance; }
            set
            {
                SetPropertyValue("NightAllowance", ref _NightAllowance, value);
            }
        }
        public decimal Incentive
        {
            get { return _Incentive; }
            set
            {
                SetPropertyValue("Incentive", ref _Incentive, value);
            }
        }
        public DayOfWeek RestDay {
            get { return _RestDay; }
            set
            {
                SetPropertyValue("RestDay", ref _RestDay, value);
            }
        }
        public ShiftClassEnum Shifting
        {
            get { return _Shifting; }
            set { SetPropertyValue("Shifting", ref _Shifting, value); }
        }
        [Association("AssignedEmployees")]
        public TimeTable2 Shift
        {
            get { return _Shift; }
            set { SetPropertyValue("Shift", ref _Shift, value); }
        }
        
        private bool _Flexible = false;
        public bool Flexible
        {
            get { return _Flexible; }
            set { SetPropertyValue("Flexible", ref _Flexible, value); }
        }

        public bool DoleDriver
        {
            get { return _DoleDriver; }
            set { SetPropertyValue("DoleDriver", ref _DoleDriver, value); }
        }
        //public bool Flexible
        //{
        //    get { return _Flexible; }
        //    set { SetPropertyValue("Flexible", ref _Flexible, value); }
        //}
        [Aggregated,
        Association("Employee-Premiums")]
        [NonCloneable]
        public XPCollection<EmployeePremium> EmployeePremiums {
            get { return
                GetCollection<EmployeePremium>("EmployeePremiums"); }
        }

        [Aggregated,
        Association("Employee-Loans")]
        [NonCloneable]
        public XPCollection<EmpLoan> EmpLoans {
            get { return GetCollection<
                EmpLoan>("EmpLoans"); }
        }
        [Association("Employee-ShiftSchedules")]
        public XPCollection<EmpShiftSchedule> ShiftSchedules
        {
            get
            {
                return GetCollection<
                    EmpShiftSchedule>("ShiftSchedules");
            }
        }
        [Aggregated,
        Association("Employee-Taxes")]
        [NonCloneable]
        public XPCollection<EmpTax> EmpTaxs {
            get { return GetCollection<EmpTax>
                ("EmpTaxs"); }
        }

        [Aggregated,
        Association("Employee-EmpIncentives")]
        [NonCloneable]
        public XPCollection<EmpIncentives> EmpIncentives
        {
            get
            {
                return GetCollection<EmpIncentives>
                    ("EmpIncentives");
            }
        }
        [Aggregated,
        Association("Employee-OtherDeds")]
        [NonCloneable]
        public XPCollection<EmpOtherDed> EmpOtherDeds {
            get { return
                GetCollection<EmpOtherDed>("EmpOtherDeds"); }
        }
        [Aggregated, Association("ShiftEmployees")]
        [NonCloneable]
        public XPCollection<ShiftEmployee> ShiftEmployees
        {
            get
            {
                return
                    GetCollection<ShiftEmployee>("ShiftEmployees");
            }
        }
        [Association]
        [NonCloneable]
        public XPCollection<AttendanceCalculator02> AttendanceBatches
        {
            get
            {
                return GetCollection<AttendanceCalculator02>("AttendanceBatches");
            }
        }
        public bool CanViewConfidential
        {
            get
            {
                if (CurrentUser.Roles.Count > 0)
                {
                    var role1 = (from rol in CurrentUser.Roles
                                 where rol.Name == "Administrator" || rol.Name == "PayrollConfi"
                                 select rol).FirstOrDefault();
                    if (role1 != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }
        #region Biometrics Profile

        private bool _StaffPayroll = false;
        public bool StaffPayroll
        {
            get { return _StaffPayroll; }
            set { SetPropertyValue("StaffPayroll", ref _StaffPayroll, value); }
        }
        private bool _NoLateDeduction = false;
        public bool NoLateDeduction
        {
            get { return _NoLateDeduction; }
            set { SetPropertyValue("NoLateDeduction", ref _NoLateDeduction, value); }
        }
        private bool _NoEarlyDeduction = false;
        public bool NoEarlyDeduction
        {
            get { return _NoEarlyDeduction; }
            set { SetPropertyValue("NoEarlyDeduction", ref _NoEarlyDeduction, value); }
        }
        public bool ManualAttLog
        {
            get { return _ManualAttLog; }
            set { SetPropertyValue("ManualAttLog", ref _ManualAttLog, value); }
        }
        private string _EnrollNumber;
        private PrivilegeEnum _Privilege;
        private string _Password;
        private bool _Enabled = true;
        private bool _Flag = true;
        private bool _DeleteOnDevices = false;
        private decimal _Allowance;
        private bool _ManualAttLog = false;
        private bool _UpdateOnline;
        private decimal _NightAllowance;
        private decimal _Incentive;
        private int _DeviceId;
        private string _DriverName;
        private string _FmsContactNo;
        private bool _FmsUpdate;
        private int _DriverId;
        private bool _DoleDriver;
        private bool _POApproverKeyClaimed=false;
        private string _AppWriteUserId;
        private string _AppWriteLastUpdated;
        private POApproverPermissionEnum _POApproverPermission;
        [RuleUniqueValue("", DefaultContexts.Save, SkipNullOrEmptyValues = true)]
        public string EnrollNumber {
            get { return _EnrollNumber; }
            set
            {
                SetPropertyValue<string>("EnrollNumber", ref _EnrollNumber, value);
            }
        }

        public PrivilegeEnum Privilege {
            get { return _Privilege; }
            set
            {
                SetPropertyValue<PrivilegeEnum>("Privilege", ref _Privilege, value);
            }
        }

        public string Password {
            get { return _Password; }
            set
            {
                SetPropertyValue<string>("Password", ref _Password, value);
            }
        }

        public bool Enabled {
            get { return _Enabled; }
            set
            {
                SetPropertyValue<bool>("Enabled", ref _Enabled, value);
            }
        }

        public bool Flag {
            get { return _Flag; }
            set
            {
                SetPropertyValue<bool>("Flag", ref _Flag, value);
            }
        }

        public bool DeleteOnDevices {
            get { return _DeleteOnDevices; }
            set
            {
                SetPropertyValue<bool>("DeleteOnDevices", ref _DeleteOnDevices, value);
            }
        }

        [Association("EmployeeFingerprintTemplates")]
        [NonCloneable]
        public XPCollection<EmployeeFingerprint> EmployeeFingerprintTemplates {
            get { return
                GetCollection<EmployeeFingerprint>("EmployeeFingerprintTemplates"); }
        }
        [Association("Driver-Trips")]
        [NonCloneable]
        public XPCollection<DriverRegistry> Trips
        {
            get
            {
                return
                    GetCollection<DriverRegistry>("Trips");
            }
        }
        #endregion

        #region Hyper FMS

        // DriverId
        [Custom("AllowEdit", "False")]
        public int DriverId
        {
            get { return _DriverId; }
            set { SetPropertyValue("DriverId", ref _DriverId, value); }
        }
        // DriverName
        [Custom("AllowEdit", "False")]
        public string DriverName
        {
            get { return _DriverName; }
            set { SetPropertyValue("DriverName", ref _DriverName, value); }
        }
        // FmsContactNo
        [Custom("AllowEdit", "False")]
        public string FmsContactNo
        {
            get { return _FmsContactNo; }
            set { SetPropertyValue("FmsContactNo", ref _FmsContactNo, value); }
        }
        // FmsUpdate
        public bool FmsUpdate
        {
            get { return _FmsUpdate; }
            set { SetPropertyValue("FmsUpdate", ref _FmsUpdate, value); }
        }

        #endregion

        public Employee(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            //Session.OptimisticLockingReadBehavior = OptimisticLockingReadBehavior.ReloadObject;
            ContactType = ContactTypeEnum.Employee;
        }
    }
}
