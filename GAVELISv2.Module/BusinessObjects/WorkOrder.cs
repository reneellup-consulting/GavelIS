using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
//using DevExpress.XtraEditors;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class WorkOrder : GenJournalHeader {
        private string _ReferenceNo;
        private string _Memo;
        private string _Comments;
        private WorkOrderStatusEnum _Status;
        private string _StatusBy;
        private DateTime _StatusDate;
        private string _Problem;
        private Facility _Facility;
        private FixedAsset _Fleet;
        private Employee _Driver;
        private Employee _Mechanic;
        private DateTime _CheckedIn;
        private DateTime _CheckedOut;
        private decimal _LaborCharge;
        private decimal _OtherCharge;
        private decimal _PrevOdo;
        private decimal _CurrOdo;
        private PreventiveMaintenance _PrevMaintenanceID;
        [DisplayName("Reference(s)")]
        [Size(1000)]
        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Size(1000)]
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Memo {
            get { return _Memo; }
            set { SetPropertyValue("Memo", ref _Memo, value); }
        }

        public string Comments {
            get { return _Comments; }
            set { SetPropertyValue("Comments", ref _Comments, value); }
        }
        //[Action(Caption="List Mechanics",AutoCommit=true)]
        //public void ListMechanics() {
        //    if (_Mechanic != null)
        //    {
        //        var query = Mechanics.FirstOrDefault(o => o.Mechanic == _Mechanic);
        //        if (query == null)
        //        {
        //            WorkOrderMechanic wom = ReflectionHelper.CreateObject<WorkOrderMechanic>(Session);
        //            wom.Mechanic = _Mechanic;
        //            Mechanics.Add(wom);
        //        }
        //    }
        //}
        //private string _ListedMechanics;
        [NonPersistent]
        public string ListedMechanics
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (Mechanics != null && Mechanics.Count > 0)
                {
                    List<string> strRefs = new List<string>();
                    foreach (var item in Mechanics)
                    {
                        if (!strRefs.Contains(item.Mechanic.Name))
                        {
                            strRefs.Add(item.Mechanic.Name);
                            sb.AppendFormat("{0}, {1}|", item.Mechanic.LastName, item.Mechanic.FirstName);
                        }
                    }
                }
                if (sb.Length > 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                _ListedMechanics = sb.ToString() != string.Empty ? sb.ToString() : "NONE";
                return _ListedMechanics;
            }
        }
        //[DisplayName("Reference(s)")]
        //public string References
        //{
        //    get {
        //        string strRes = string.Empty;
        //        var reqs = WorkOrderItemDetails.OrderBy(o => o.RequisitionNo);
        //        if (reqs != null)
        //        {
        //            StringBuilder sb = new StringBuilder();
        //            foreach (var item in reqs)
        //            {
        //                if (item.RequisitionNo != null && !sb.ToString().Contains(item.RequisitionNo.SourceNo))
        //                {
        //                    sb.AppendFormat("{0},", item.RequisitionNo.SourceNo);
        //                }
        //            }
        //            strRes = sb.ToString();
        //        }
        //        return strRes;
        //    }
        //}
        public WorkOrderStatusEnum Status {
            get { return _Status; }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading)
                {
                    if (_Status != WorkOrderStatusEnum.Current)
                    {
                        Approved = true
                        ;
                    } else
                    {
                        Approved = false;
                    }
                }

                if (!IsLoading && SecuritySystem.CurrentUser != null)
                {
                    SecurityUser currentUser = Session.GetObjectByKey<
                    SecurityUser>(Session.GetKeyValue(SecuritySystem.CurrentUser
                    ));
                    this.StatusBy = currentUser.UserName;
                    this.StatusDate = DateTime.Now;
                }
                if (!IsLoading)
                {
                    switch (_Status)
                    {
                        case WorkOrderStatusEnum.Current:
                            break;
                        case WorkOrderStatusEnum.CheckedIn:
                            if (this.Oid == -1)
                            {
                                throw new ApplicationException("Please save the Work Order first");
                            }
                            break;
                        case WorkOrderStatusEnum.WaitingForMechanic:
                            break;
                        case WorkOrderStatusEnum.WaitingForParts:
                            break;
                        case WorkOrderStatusEnum.InProgress:
                            break;
                        case WorkOrderStatusEnum.CheckedOut:
                            if (_PrevMaintenanceID!=null)
                            {
                                PrevMaintenanceID.LastRunMeter = CurrOdo;
                                PrevMaintenanceID.LastRunDate = EntryDate;
                                PrevMaintenanceID.StartBaseMeter = CurrOdo;
                                PrevMaintenanceID.StartBaseDate = EntryDate;
                                PrevMaintenanceID.LastOutcome = "Done";
                            }
                            if (_MeterRegistryID!=null)
                            {
                                MeterRegistryID.ServiceCost = Total;
                            }
                            //// Create Fuel Odo Registry
                            //OdometerRegister odo;
                            //odo = ReflectionHelper.CreateObject<OdometerRegister>(this.Session);
                            //odo.Fleet = _Fleet;
                            //odo.MeterType = MeterEntryTypeEnum.Odometer;
                            //if (_Fleet.VehicleOdoRegisters.Count > 1)
                            //{
                            //    odo.LogType = MeterLogTypeEnum.Service;
                            //}
                            //else
                            //{
                            //    throw new UserFriendlyException(
                            //    "An initial odometer log for this vehicle asset has not yet been established"
                            //    );
                            //}
                            //odo.EntryDate = this.EntryDate;
                            //odo.ReportedBy = _Driver!=null?_Driver:null;
                            //odo.Reference = this.ReferenceNo + "/" + this.SourceNo;
                            //odo.Reading = _CurrOdo;
                            //odo.Cost = this.Total;
                            //odo.Save();

                            //// Update PM
                            //PreventiveMaintenance pm = Session.GetObjectByKey<
                            //PreventiveMaintenance>(_PrevMaintenanceID.Oid);
                            //    //Session.FindObject<PreventiveMaintenance>(CriteriaOperator
                            //    //.Parse("[GenJournalID.SourceNo] = '" + SourceNo + "'"));
                            //if (pm != null)
                            //{
                            //    pm.LastOutcome="Completed";
                            //    pm.LastWOrder = this;
                            //    pm.LastRunDate = _CheckedOut;
                            //    pm.LastRunMeter = _CurrOdo;
                            //    pm.Save();
                            //}
                            //#region New Service Odo Logging
                            //if (_Fleet != null)
                            //{
                            //    ServiceOdoRegistry newLog = null;
                            //    //thisReceipt
                            //    if (this.ServiceOdoLogs.Count > 0)
                            //    {
                            //        newLog = this.ServiceOdoLogs.FirstOrDefault();
                            //    }
                            //    else
                            //    {
                            //        newLog = ReflectionHelper.CreateObject<ServiceOdoRegistry>(this.Session);
                            //    }
                            //    newLog.Fleet = _Fleet;
                            //    newLog.WorkOrderId = this;
                            //    newLog.EntryDate = this.EntryDate;
                            //    newLog.LogType = MeterRegistryTypeEnum.Log;
                            //    newLog.Reading = this.CurrOdo;
                            //    newLog.Save();
                            //}
                            //#endregion
                            break;
                        case WorkOrderStatusEnum.Invoiced:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public string StatusBy {
            get { return _StatusBy; }
            set { SetPropertyValue("StatusBy", ref _StatusBy, value); }
        }

        public DateTime StatusDate {
            get { return _StatusDate; }
            set { SetPropertyValue("StatusDate", ref _StatusDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Problem {
            get { return _Problem; }
            set { SetPropertyValue("Problem", ref _Problem, value); }
        }

        public Facility Facility {
            get { return _Facility; }
            set
            {
                SetPropertyValue("Facility", ref _Facility, value);
                //if (!IsLoading && _Facility != null) {
                //    Driver = null;
                //    Fleet = null;
                //}
            }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public FixedAsset Fleet {
            get { return _Fleet; }
            set
            {
                SetPropertyValue("Fleet", ref _Fleet, value);
                if (!IsLoading && _Fleet != null)
                {
                    Facility = null;
                    switch (_Fleet.FixedAssetClass)
                    {
                        case FixedAssetClassEnum.LandAndBuilding:
                            break;
                        case FixedAssetClassEnum.Truck:
                            Driver = ((FATruck)_Fleet).Operator;
                            PopulatePreviousReading();
                            break;
                        case FixedAssetClassEnum.Trailer:
                            Driver = ((FATrailer)_Fleet).Operator;
                            PopulatePreviousReading();
                            break;
                        case FixedAssetClassEnum.GeneratorSet:
                            Driver = ((FAGeneratorSet)_Fleet).Operator;
                            PopulatePreviousReading();
                            break;
                        case FixedAssetClassEnum.OtherVehicle:
                            if (_Fleet.GetType() == typeof(FAOtherVehicle))
                            {
                                Driver = ((FAOtherVehicle)_Fleet).Operator;
                            }
                            else if (_Fleet.GetType() == typeof(FATruck))
                            {
                                Driver = ((FATruck)_Fleet).Operator;
                            }
                            else
                            {
                                Driver = null;
                            }
                            PopulatePreviousReading();
                            break;
                        case FixedAssetClassEnum.Other:
                            Driver = null;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        //[RuleRequiredField("", DefaultContexts.Save)]
        public Employee Driver {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }

        public Employee Mechanic {
            get { return _Mechanic; }
            set
            {
                if (_Mechanic == value)
                    return;
                _Mechanic = value;
            }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime CheckedIn {
            get { return _CheckedIn; }
            set { SetPropertyValue("CheckedIn", ref _CheckedIn, value); }
        }

        [Custom("DisplayFormat", "MM.dd.yyyy HH:mm:ss")]
        [EditorAlias("CustomDateTimeEditor3")]
        public DateTime CheckedOut {
            get { return _CheckedOut; }
            set { SetPropertyValue("CheckedOut", ref _CheckedOut, value); }
        }

        public decimal LaborCharge {
            get { return _LaborCharge; }
            set { SetPropertyValue("LaborCharge", ref _LaborCharge, value); }
        }

        public decimal OtherCharge {
            get { return _OtherCharge; }
            set { SetPropertyValue("OtherCharge", ref _OtherCharge, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        [DisplayName("Previous Meter")]
        [Custom("AllowEdit", "False")]
        public decimal PrevOdo {
            get { return _PrevOdo; }
            set { SetPropertyValue("PrevOdo", ref _PrevOdo, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        [DisplayName("Current Meter")]
        public decimal CurrOdo {
            get { return _CurrOdo; }
            set { SetPropertyValue("CurrOdo", ref _CurrOdo, value); }
        }

        [Custom("AllowEdit", "False")]
        public PreventiveMaintenance PrevMaintenanceID {
            get { return _PrevMaintenanceID; }
            set
            {
                PreventiveMaintenance oldPrevMaintId = _PrevMaintenanceID;
                SetPropertyValue("PrevMaintenanceID", ref _PrevMaintenanceID,
                value);
                if (!IsLoading && _PrevMaintenanceID!=null)
                {
                    if (_PrevMaintenanceID.InProgress())
                    {
                        throw new ApplicationException("Maintenance already in progress");
                    }
                    else
                    {
                        _PrevMaintenanceID.LastOutcome = "In Progress";
                        _PrevMaintenanceID.LastWOrder = this;
                        _PrevMaintenanceID.LastScheduleType = _PrevMaintenanceID.ScheduleType;
                        if (_MeterRegistryID!=null)
                        {
                            MeterRegistryID.PrevMaintenanceID = PrevMaintenanceID;
                        }
                    }
                    if (oldPrevMaintId != null && oldPrevMaintId != _PrevMaintenanceID)
                    {
                        oldPrevMaintId.LastOutcome = "Cancelled";
                        oldPrevMaintId.LastWOrder = this;
                        oldPrevMaintId.LastScheduleType = _PrevMaintenanceID.ScheduleType;
                    }
                }
            }
        }
        
        #region Calculated Details

        [Persistent("TotalWithJO")]
        private decimal? _TotalWithJO;
        [Persistent("TotalIntJobs")]
        private decimal? _TotalIntJobs;
        [Persistent("TotalParts")]
        private decimal? _TotalParts;
        //[Persistent("Total")]
        //private decimal? _Total;
        [PersistentAlias("_TotalWithJO")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalWithJO {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalWithJO == null)
                    {
                        UpdateTotalWithJO(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalWithJO;
            }
        }

        public void UpdateTotalWithJO(bool forceChangeEvent) {
            decimal? oldTotal = _TotalWithJO;
            decimal tempTotal = 0m;
            foreach (WorkOrderJobsDetail detail in WorkOrderJobsDetails)
            {
                tempTotal += detail.JobOrderDetailNo != null ?
                detail.Total :
                0;
            }
            _TotalWithJO = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalWithJO", TotalWithJO,
                _TotalWithJO);
            }
            ;
        }

        [PersistentAlias("_TotalIntJobs")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalIntJobs {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalIntJobs == null)
                    {
                        UpdateTotalIntJobs(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalIntJobs;
            }
        }

        public void UpdateTotalIntJobs(bool forceChangeEvent) {
            decimal? oldTotal = _TotalIntJobs;
            decimal tempTotal = 0m;
            foreach (WorkOrderJobsDetail detail in WorkOrderJobsDetails)
            {
                tempTotal
                += detail.JobOrderDetailNo == null ?
                detail.Total :
                0;
            }
            _TotalIntJobs = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalIntJobs", TotalIntJobs, _TotalIntJobs
                );
            }
            ;
        }

        [PersistentAlias("_TotalParts")]
        [Custom("DisplayFormat", "n")]
        public decimal? TotalParts {
            get
            {
                try
                {
                    if (!IsLoading && !IsSaving && _TotalParts == null)
                    {
                        UpdateTotalParts(false);
                    }
                } catch (Exception)
                {
                }
                return _TotalParts;
            }
        }

        public void UpdateTotalParts(bool forceChangeEvent) {
            decimal? oldTotal = _TotalParts;
            decimal tempTotal = 0m;
            foreach (WorkOrderItemDetail detail in WorkOrderItemDetails)
            {
                tempTotal +=
                detail.Total;
            }
            _TotalParts = tempTotal;
            if (forceChangeEvent)
            {
                OnChanged("TotalParts", TotalParts, _TotalParts);
            }
            ;
        }

        [PersistentAlias("TotalWithJO + TotalIntJobs + TotalParts")]
        [Custom("DisplayFormat", "n")]
        public decimal Total {
            get
            {
                object tempObject = null;
                try
                {
                    tempObject = EvaluateAlias("Total");
                } catch (Exception)
                {
                }
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                } else
                {
                    return 0;
                }
            }
        }

        //[PersistentAlias("_Total")]
        //[Custom("DisplayFormat", "n")]
        //public decimal? Total {
        //    get {
        //        try {
        //            if (!IsLoading && !IsSaving && _Total == null) {UpdateTotal(
        //                false);}
        //        } catch (Exception) {}
        //        return _Total;
        //    }
        //}
        //public void UpdateTotal(bool forceChangeEvent) {
        //    decimal? oldTotal = _Total;
        //    decimal tempTotal = 0m;
        //    foreach (WorkOrderJobsDetail detail in WorkOrderJobsDetails) {
        //        tempTotal += detail.Total;}
        //    _Total = tempTotal;
        //    if (forceChangeEvent) {OnChanged("Total", Total, _Total);}
        //    ;
        //}

        #endregion

        #region Meter Registration

        public override DateTime EntryDate
        {
            get { return base.EntryDate; }
            set
            {
                base.EntryDate = value;
                // Populate Previous Reading
                PopulatePreviousReading();
            }
        }

        private void PopulatePreviousReading()
        {
            if (!IsLoading)
            {
                if (_Fleet != null)
                {
                    PrevOdo = 0m;
                    CurrOdo = 0m;
                    GetLastReading2(_Fleet);
                    //GetLastReading(_Fleet);
                }
                else
                {
                    PrevOdo = 0m;
                    CurrOdo = 0m;
                }
            }
        }
        [Action(Caption = "Get Previous Reading", AutoCommit = true)]
        public void PopulatePreviousReadingAction()
        {
            if (_Fleet != null)
            {
                PrevOdo = 0m;
                CurrOdo = 0m;
                GetLastReading2(_Fleet);
                //GetLastReading(_Fleet);
            }
            else
            {
                PrevOdo = 0m;
                CurrOdo = 0m;
            }
        }
        private void GetLastReading2(FixedAsset asset) {
            FixedAsset selFa = _Fleet != null ? _Fleet : null;
            if (selFa != null)
            {
                if (selFa.GetType() == typeof(FATruck) || selFa.GetType() == typeof(FAOtherVehicle))
                {
                    if (EntryDate != DateTime.MinValue) {
                        string seq = EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, 999999)
                       : string.Empty;
                        decimal toDecimal = Convert.ToDecimal(seq);
                        var data = selFa.FleetFuelOdoLogs.OrderBy(o => o.Sequence).Where(o => o.ReceiptId != null &&  Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevOdo = data.Reading;
                            CurrOdo = data.Reading;
                        }
                    }
                }
                if (selFa.GetType() == typeof(FAGeneratorSet))
                {
                    if (EntryDate != DateTime.MinValue)
                    {
                        string seq = EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, 999999)
                       : string.Empty;
                        decimal toDecimal = Convert.ToDecimal(seq);
                        var data = (selFa as FAGeneratorSet).GensetHoursLogs.OrderBy(o => o.Sequence).Where(o => Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                        if (data != null)
                        {
                            PrevOdo = 0m;
                            CurrOdo = data.LifeHours;
                        }
                    }
                }
                if (selFa.GetType() == typeof(FATrailer))
                {
                    if (EntryDate != DateTime.MinValue)
                    {
                        string seq = EntryDate != DateTime.MinValue ?
                       string.Format("{0}{1:00}{2:00}{3:00}{4:00}{5:00}{6:0000000}", EntryDate.Year, EntryDate.Month,
                       EntryDate.Day, EntryDate.Hour, EntryDate.Minute, EntryDate.Second, 999999)
                       : string.Empty;
                        decimal toDecimal = Convert.ToDecimal(seq);
                        if ((selFa as FATrailer).TraileRegistryTripLogs != null)
                        {
                            var data = (selFa as FATrailer).TraileRegistryTripLogs.OrderBy(o => o.Sequence).Where(o => Convert.ToDecimal(o.Sequence) < toDecimal).LastOrDefault();
                            if (data != null)
                            {
                                PrevOdo = 0m;
                                CurrOdo = data.TrlrMileage;
                            }
                        }
                        else
                        {
                            PrevOdo = 0m;
                            CurrOdo = 0m;
                        }
                    }
                }
            }
        }
        
        private void GetLastReading(FixedAsset asset)
        {
            LastReadings rdngsSrvc = new LastReadings();
            LastReadings rdngsLast = new LastReadings();
            if (_PrevMaintenanceID!=null)
            {
                rdngsSrvc = asset.GetServiceIdLastReadingBeforeDate(EntryDate, _PrevMaintenanceID);
            }
            else
            {
                rdngsSrvc = asset.GetServiceLastReadingBeforeDate(EntryDate);
            }
            rdngsLast = asset.GetLastReadingBeforeDate(EntryDate);
            if (rdngsSrvc != null)
            {
                CurrOdo = rdngsLast != null ? rdngsLast.LastOdoRead : 0m; // ok
                if (_PrevMaintenanceID != null)
                {
                    PrevOdo = rdngsSrvc.LastServiceIdLife;
                }
                else
                {
                    PrevOdo = rdngsSrvc.LastServiceLife;
                }
            }
            else
            {
                CurrOdo = rdngsLast != null ? rdngsLast.LastOdoRead : 0m; // ok
                PrevOdo = 0m;
            }
        }
        private OdometerRegister _MeterRegistryID;
        [Custom("AllowEdit", "False")]
        public OdometerRegister MeterRegistryID {
            get { return _MeterRegistryID; }
            set { SetPropertyValue<OdometerRegister>("MeterRegistryID", ref _MeterRegistryID, value); }
        }
        
        protected override void OnSaving() {
            #region New Service Odo Logging
            if (_Status != WorkOrderStatusEnum.Current)
            {
                if (_Fleet != null)
                {
                    ServiceOdoRegistry newLog = null;
                    //thisReceipt
                    if (this.ServiceOdoLogs.Count > 0)
                    {
                        newLog = this.ServiceOdoLogs.FirstOrDefault();
                    }
                    else
                    {
                        newLog = ReflectionHelper.CreateObject<ServiceOdoRegistry>(this.Session);
                    }
                    newLog.Fleet = _Fleet;
                    newLog.WorkOrderId = this;
                    newLog.EntryDate = this.EntryDate;
                    newLog.LogType = MeterRegistryTypeEnum.Log;
                    newLog.Reading = this.CurrOdo;
                    newLog.Save();
                }
            }
            #endregion

            if (CompanyInfoHead.AllowInsufficientCurrQty != true)
            {
                foreach (var item in this.WorkOrderItemDetails)
                {
                    if (item.Quantity > item.CurrentQtyBase)
                    {
                        throw new UserFriendlyException("Warehouse is not sufficient to fullfil item " + item.ItemNo.No + "!");
                    }
                }
            }

            foreach (var item in this.WorkOrderItemDetails)
            {
                if (item.Quantity > item.CurrentQtyBase)
                {
                    DialogResult dres = XtraMessageBox.Show("Warehouse is not sufficient to fullfil item " + item.ItemNo.No + ". Do you want to continue?", "Insufficient Qty",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (dres == DialogResult.Cancel)
                    {
                        return;
                    }
                }

            }

            base.OnSaving();
        }
        [Action(Caption = "Register Odo", AutoCommit = true)]
        public void RegisterOdo()
        {
            #region New Service Odo Logging
            if (_Status != WorkOrderStatusEnum.Current)
            {
                if (_Fleet != null)
                {
                    ServiceOdoRegistry newLog = null;
                    //thisReceipt
                    if (this.ServiceOdoLogs.Count > 0)
                    {
                        newLog = this.ServiceOdoLogs.FirstOrDefault();
                    }
                    else
                    {
                        newLog = ReflectionHelper.CreateObject<ServiceOdoRegistry>(this.Session);
                    }
                    newLog.Fleet = _Fleet;
                    newLog.WorkOrderId = this;
                    newLog.EntryDate = this.EntryDate;
                    newLog.LogType = MeterRegistryTypeEnum.Log;
                    newLog.Reading = this.CurrOdo;
                    newLog.Save();
                }
            }
            #endregion
        }
        
        private string _Origins;
        private string _ListedMechanics;
        //[Custom("AllowEdit", "False")]
        //[NonPersistent]
        public string Origins
        {
            get {
                var data = WorkOrderItemDetails.Where(o=>o.Origin != null).Select(o=>o);
                if (data != null && data.Count() > 0)
                {
                    List<string> strRefs = new List<string>();
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in data)
                    {
                        if (item.Origin != null)
                        {
                            if (!strRefs.Contains(item.Origin.Name))
                            {
                                strRefs.Add(item.Origin.Name);
                                sb.AppendFormat("{0},", item.Origin.Name);
                            }
                        }
                    }
                    if (sb.Length > 0)
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }
                    _Origins = sb.ToString();
                }
                return _Origins; }
        }
        [Aggregated,
        Association("ServiceOdoLogs")]
        public XPCollection<ServiceOdoRegistry> ServiceOdoLogs
        {
            get
            {
                return GetCollection<ServiceOdoRegistry>("ServiceOdoLogs"
                    );
            }
        }
        #region Multiple Mechanic
        [Aggregated,
        Association("WorkOrder_Mechanics")]
        public XPCollection<WorkOrderMechanic> Mechanics
        {
            get
            {
                return GetCollection<WorkOrderMechanic>("Mechanics"
                    );
            }
        }
        #endregion
        protected override void OnSaved() {
            this.AutoRegisterIncomeExpenseVer();
            //this.Session.CommitTransaction();
            // Auto Correct Start
            //AutoCorrectStart();
            // Auto Correct End

            base.OnSaved();
        }

        #endregion

        public WorkOrder(Session session)
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
            SourceType = Session.FindObject<SourceType>(new BinaryOperator("Code", "WO")
            );
            OperationType = Session.FindObject<OperationType>(new BinaryOperator("Code",
            "WO"));
            UnitOfWork session = new UnitOfWork(this.Session.ObjectLayer);
            SourceType source = session.FindObject<SourceType>(new BinaryOperator("Code"
            , "WO"));
            if (source != null)
            {
                SourceNo = !string.IsNullOrEmpty(source.NumberFormat) ?
                source.GetNewNo() :
                null;
                source.Save();
                session.CommitChanges();
            }
            // Populate ShipToAddress from Company Information
            Company company = Company.GetInstance(session);
        }
        protected override void TriggerObjectChanged(ObjectChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(args.PropertyName) && args.PropertyName != "IsIncExpNeedUpdate" && args.PropertyName != "ModifiedBy" && args.PropertyName != "ModifiedOn")
            {
                this.IsIncExpNeedUpdate = true;
            }
            //this.IsIncExpNeedUpdate = true;
            base.TriggerObjectChanged(args);
        }

        protected override void OnDeleting() {
            if (Approved)
            {
                throw new
                UserFriendlyException(
                "The system prohibits the deletion of already approved Work Order transactions."
                );
            }
            if (MeterRegistryID!=null)
            {
                OdometerRegister oldMeterId = MeterRegistryID;
                MeterRegistryID = null;
                oldMeterId.Delete();
            }
            //AutoCorrectStart();
            //base.OnDeleting();
        }

        protected override void OnLoaded() {
            Reset();
            base.OnLoaded();
        }

        private void Reset() {
            //_Total = null;
            _TotalIntJobs = null;
            _TotalParts = null;
            _TotalWithJO = null;
        }
    }
}
