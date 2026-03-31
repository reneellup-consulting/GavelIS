using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum ConnectionStatusEnum
    {
        Disconnected,
        Connected
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("DeviceName")]
    public class BiometricDevice : XPObject {
        private string _DeviceName;
        private int _MachineNo = 100;
        private ConnectionStatusEnum _Status;
        private string _IpAddress;
        private int _Port = 4370;
        private string _SerialNo;
        private int _UserCount;
        private int _AdminCount;
        private int _FpCount;
        private int _LogCount;
        private DateTime _CurrentDeviceTime;
        //private DateTime _DeviceTimeOverride;
        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string DeviceName {
            get { return _DeviceName; }
            set { SetPropertyValue<string>("DeviceName", ref _DeviceName, value); }
        }

        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("AllowEdit", "False")]
        public int MachineNo {
            get { return _MachineNo; }
            set { SetPropertyValue<int>("MachineNo", ref _MachineNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public ConnectionStatusEnum Status {
            get { return _Status; }
            set { SetPropertyValue<ConnectionStatusEnum>("Status", ref _Status, value); }
        }

        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("IP Address")]
        public string IpAddress {
            get { return _IpAddress; }
            set { SetPropertyValue<string>("IpAddress", ref _IpAddress, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public int Port {
            get { return _Port; }
            set { SetPropertyValue<int>("Port", ref _Port, value); }
        }

        [Custom("AllowEdit", "False")]
        public string SerialNo {
            get { return _SerialNo; }
            set { SetPropertyValue<string>("SerialNo", ref _SerialNo, value); }
        }

        [Custom("AllowEdit", "False")]
        public int UserCount {
            get { return _UserCount; }
            set { SetPropertyValue<int>("UserCount", ref _UserCount, value); }
        }

        [Custom("AllowEdit", "False")]
        public int AdminCount {
            get { return _AdminCount; }
            set { SetPropertyValue<int>("AdminCount", ref _AdminCount, value); }
        }

        [Custom("AllowEdit", "False")]
        [DisplayName("FP Count")]
        public int FpCount {
            get { return _FpCount; }
            set { SetPropertyValue<int>("FpCount", ref _FpCount, value); }
        }

        [Custom("AllowEdit", "False")]
        public int LogCount {
            get { return _LogCount; }
            set { SetPropertyValue<int>("LogCount", ref _LogCount, value); }
        }
        [Custom("AllowEdit", "False")]
        public DateTime CurrentDeviceTime {
            get { return _CurrentDeviceTime; }
            set { SetPropertyValue<DateTime>("CurrentDeviceTime", ref _CurrentDeviceTime, value); }
        }

        //public DateTime DeviceTimeOverride {
        //    get { return _DeviceTimeOverride; }
        //    set { SetPropertyValue<DateTime>("DeviceTimeOverride", ref _DeviceTimeOverride, value); }
        //}

        #region Records Creation

        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        public BiometricDevice(Session session)
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

            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.CreatedBy = currentUser.UserName;
                this.CreatedOn = DateTime.Now;
            }

            #endregion

        }

        protected override void OnSaving() {
            base.OnSaving();

            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                SecurityUser currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                this.ModifiedBy = currentUser.UserName;
                this.ModifiedOn = DateTime.Now;
            }

            #endregion

        }
    }

}
