using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
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
    public enum TiremanActivityTypeEnum
    {
        Attach,
        Dettach,
        Replace,
        Transfer
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [RuleCriteria("", DefaultContexts.Save, "[Finished] > [Started]")]
    [System.ComponentModel.DefaultProperty("TiremanActivitID")]
    public class TiremanActivity : XPObject {
        public string TiremanActivitID {
            get { return Oid > 0 ? String.Format("TAID:{0:D6}"
                , Oid) : String.Empty; }
        }

        // TiremanDailyId;
        private TiremanDaily _TmDailyId;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TiremanActivtyDetails")]
        [DisplayName("TRD ID")]
        [Custom("AllowEdit", "False")]
        public TiremanDaily TmDailyId {
            get { return _TmDailyId; }
            set
            {
                SetPropertyValue<TiremanDaily>("TmDailyId", ref _TmDailyId, value);
                if (!IsLoading && !IsSaving && _TmDailyId != null)
                {
                    ActivityReason = _TmDailyId.LastActivityReason ?? null;
                    ToFleet = _TmDailyId.LastToFleet ?? null;
                    ToOdometer = _TmDailyId.LastToOdometer;
                    Reported = _TmDailyId.EntryDate;
                    Started = _TmDailyId.EntryDate;
                    Finished = _TmDailyId.EntryDate;
                }
            }
        }

        // TiremanActivityType
        private TiremanActivityTypeEnum _TiremanActivityType;
        public TiremanActivityTypeEnum TiremanActivityType {
            get { return _TiremanActivityType; }
            set { SetPropertyValue<TiremanActivityTypeEnum>("TiremanActivityType", ref _TiremanActivityType, value); }
        }

        // EntryDate
        //private DateTime _EntryDate;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate {
            get { return _TmDailyId != null ? _TmDailyId.EntryDate : DateTime.MinValue; }
        }

        // *** Attach Activity ***
        // ToFleet;
        [ImmediatePostData]
        public string ReplacedBrandingNo
        {
            get { return _ReplacedBrandingNo; }
            set
            {
                SetPropertyValue<string>("ReplacedBrandingNo", ref _ReplacedBrandingNo, value);
                if (!IsLoading)
                {
                    ReplacedRemarks = "Replaced tire not specified";
                    ReplacedTireDetail = null;
                    if (_ToFleet == null)
                    {
                        throw new UserFriendlyException("Please specify Attach to Fleet first");
                    }
                    if (!string.IsNullOrEmpty(_ReplacedBrandingNo))
                    {
                        var usable = Session.FindObject<TireDettachReason>(BinaryOperator.Parse("[Code] = 'USABLE'"));
                        var data = _ToFleet.FleetTireServiceDetails.Where(o => o.BrandingNo == _ReplacedBrandingNo).LastOrDefault();
                        if (data != null)
                        {
                            ReplacedTireDetail = data ?? null;
                            ReplacedRemarks = ReplacedTireDetail.Remarks;
                        }
                        else
                        {
                            ReplacedRemarks = "Replaced tire information not found";
                        }
                    }
                    else
                    {
                        ReplacedRemarks = "Replaced tire not specified";
                    }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        public TireServiceDetail2 ReplacedTireDetail
        {
            get { return _ReplacedTireDetail; }
            set
            {
                SetPropertyValue<TireServiceDetail2>("ReplacedTireDetail", ref _ReplacedTireDetail, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string ReplacedRemarks
        {
            get
            {
                return _ReplacedRemarks;
            }
                // return !string.IsNullOrEmpty(_ReplacedRemarks)?_ReplacedRemarks:"Replaced tire not specified"; }
            set
            {
                SetPropertyValue<string>("ReplacedRemarks", ref _ReplacedRemarks, value);
            }
        }
        private FixedAsset _ToFleet;
        [ImmediatePostData]
        public FixedAsset ToFleet {
            get { return _ToFleet; }
            set
            {
                SetPropertyValue<FixedAsset>("ToFleet", ref _ToFleet, value);
                if (!IsLoading && !IsSaving && _ToFleet != null)
                {
                    ToOdometer = _ToFleet.LastOdoReading;
                    //NewBrandingNo = InitBranding(_ToFleet, EntryDate);
                }
            }
        }

        // ToOdometer;
        private decimal _ToOdometer;
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal ToOdometer {
            get { return _ToOdometer; }
            set { SetPropertyValue<decimal>("ToOdometer", ref _ToOdometer, value); }
        }

        // ToWheelPos;
        private WheelPosition _ToWheelPos;
        public WheelPosition ToWheelPos {
            get { return _ToWheelPos; }
            set { SetPropertyValue<WheelPosition>("ToWheelPos", ref _ToWheelPos, value); }
        }

        // AttachTire; Branding/Serial
        private Tire _AttachTire;
        [ImmediatePostData]
        public Tire AttachTire {
            get { return _AttachTire; }
            set
            {
                SetPropertyValue<Tire>("AttachTire", ref _AttachTire, value);
                if (!IsLoading && !IsSaving && _AttachTire != null)
                {
                    TireStatus = _AttachTire.Condition;
                    if (string.IsNullOrEmpty(_AttachTire.LastBrandingNo) && _ActivityReason != null && _ActivityReason.FirstBranding)
                    {
                        NewBrandingNo = InitBranding(_ToFleet, EntryDate);
                    }
                    else
                    {
                        NewBrandingNo = _AttachTire.LastBrandingNo;
                    }
                }
            }
        }

        // ActivityDate;
        private DateTime _ActivityDate = DateTime.Now;
        public DateTime ActivityDate {
            get { return _ActivityDate; }
            set { SetPropertyValue<DateTime>("ActivityDate", ref _ActivityDate, value); }
        }


        // *** Dettach Activity ***
        // FromFleet;
        private FixedAsset _FromFleet;
        public FixedAsset FromFleet {
            get { return _FromFleet; }
            set { SetPropertyValue<FixedAsset>("FromFleet", ref _FromFleet, value); }
        }

        // FromOdometer;
        private decimal _FromOdometer;
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal FromOdometer {
            get { return _FromOdometer; }
            set { SetPropertyValue<decimal>("FromOdometer", ref _FromOdometer, value); }
        }

        // DettachTire; Branding/Serial
        private Tire _DettachTire;
        public Tire DettachTire {
            get { return _DettachTire; }
            set
            {
                SetPropertyValue<Tire>("DettachTire", ref _DettachTire, value);
                if (!IsLoading && !IsSaving && _DettachTire != null)
                {
                    TireStatus = _DettachTire.Condition;
                    DettachBranding = _DettachTire.LastBrandingNo;
                    FromFleet = _DettachTire.LastFleet ?? null;
                    FromWheelPos = _DettachTire.LastWheelPos ?? null;
                    if (_FromFleet != null)
                    {
                        FromOdometer = _FromFleet.LastOdoReading;
                    }
                }
            }
        }

        // DettachBranding;
        private string _DettachBranding;
        public string DettachBranding {
            get { return _DettachBranding; }
            set { SetPropertyValue<string>("DettachBranding", ref _DettachBranding, value); }
        }

        // FromWheelPos;
        private WheelPosition _FromWheelPos;
        public WheelPosition FromWheelPos {
            get { return _FromWheelPos; }
            set { SetPropertyValue<WheelPosition>("FromWheelPos", ref _FromWheelPos, value); }
        }

        // Reason;
        private string _Reason;
        [Size(500)]
        [DisplayName("Remarks")]
        public string Reason {
            get { return _Reason; }
            set { SetPropertyValue<string>("Reason", ref _Reason, value); }
        }

        // ActivityReason
        private TireDettachReason _ActivityReason;
        [RuleRequiredField("", DefaultContexts.Save)]
        [ImmediatePostData]
        public TireDettachReason ActivityReason
        {
            get { return _ActivityReason; }
            set
            {
                SetPropertyValue<TireDettachReason>("ActivityReason", ref _ActivityReason, value);
                if (!IsLoading && !IsSaving)
                {
                    _InitNo = string.IsNullOrEmpty(_InitNo) && _ActivityReason != null ? _ActivityReason.LastNo : _InitNo;
                    NewBrandingNo = InitBranding(_ToFleet, EntryDate);
                }
            }
        }

        // *** Replace Activity ***
        // DettachTire; Branding/Serial
        // FromFleet;
        // FromWheelPos;
        // AttachTire; Branding/Serial
        // ActivityDate;
        // Reason;

        // *** Transfer Activity ***
        // DettachTire; Branding/Serial
        // FromFleet;
        // FromWheelPos;
        // ToFleet;
        // ToWheelPos;
        // NewBrandingNo;
        private string _NewBrandingNo;
        public string NewBrandingNo {
            get { return _NewBrandingNo; }
            set { SetPropertyValue<string>("NewBrandingNo", ref _NewBrandingNo, value); }
        }

        // *** Tread Depth ***
        private decimal _TreadDepth;
        public decimal TreadDepth {
            get { return _TreadDepth; }
            set { SetPropertyValue<decimal>("TreadDepth", ref _TreadDepth, value); }
        }
        private string _InitNo = string.Empty;
        private string InitBranding(FixedAsset fleet, DateTime date) {
            string tmp = String.Empty;
            if (_ActivityDate >= DateTime.Parse("2019-01-10") && _ActivityReason != null && _ActivityReason.FirstBranding)
            {
                string n = GetNewNo(_InitNo);
                tmp = string.Format("48{0}{1}", _ActivityReason.BYear, n);
                TireDettachReason tdr = Session.GetObjectByKey<TireDettachReason>(_ActivityReason.Oid);
                tdr.LastNo = n;
                tdr.Save();
            }
            else
            {
                if (fleet != null)
                {
                    if (fleet.GetType() == typeof(FATruck))
                    {
                        string[] stringSeparators = new string[] { "-"
                    };
                        string[] sr = fleet.No.Split(stringSeparators, StringSplitOptions.None);
                        tmp = string.Format("{0}{1}{2} {3}{4} 0", date.Month.ToString("00"), date.Day, date.Year.ToString().Substring(2), sr[0], sr[1]);
                    }
                }
            }
            return tmp;
        }

        private string GetNewNo(string lastNo)
        {
            string seqNo;
            string incNo;
            int inc = 1;
            if (!string.IsNullOrEmpty(lastNo))
            {
                seqNo = lastNo
                    ;
            }
            else
            {
                seqNo = "000000";
            }
            string digits = "0123456789";
            string defaultFormat = "{0:D6}";
            string formatString = string.Empty;
            string num = string.Empty;
            int c = 0;
            int i, x;
            i = x = seqNo.LastIndexOfAny(digits.ToCharArray());
            while (i >= 0 && isDigit(seqNo[i]))
            {
                num = seqNo[i] + num;
                c++;
                i--;
            }
            int n = int.Parse(num) + inc;
            formatString = defaultFormat.Replace("6", c.ToString());
            incNo = string.Format(formatString, n);
            x = x + 1 - num.Length;
            seqNo = seqNo.Remove(x, num.Length);
            seqNo = seqNo.Insert(x, string.Empty + incNo);
            lastNo = seqNo;
            // Update the No Series Line
            //UnitOfWork uow = new UnitOfWork();
            //ObjectSpace os = new ObjectSpace(uow);
            //Session _session = new Session(objectSpace.Session.DataLayer);
            //_session.BeginTransaction();
            //NoSeriesLine nSLine = _session.FindObject<NoSeriesLine>(new
            //BinaryOperator("Oid", nsLineNo));
            //nSLine.LastDateUsed = DateTime.Today;
            //nSLine.LastNoUsed = seqNo;
            ////nSLine.;
            //nSLine.Save();
            //_session.CommitTransaction();
            return seqNo;
        }

        private static bool isDigit(char c)
        {
            string digits = "0123456789";
            return digits.IndexOf(c) == -1 ? false : true;
        }
        // Reason;

        // *** Work Duration ***
        private DateTime _Reported;
        private DateTime _Started;
        private DateTime _Finished;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "hh:mm:ss tt")]
        [Custom("EditMask", "hh:mm:ss tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime Reported {
            get { return _Reported; }
            set { SetPropertyValue<DateTime>("Reported", ref _Reported, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "hh:mm:ss tt")]
        [Custom("EditMask", "hh:mm:ss tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime Started {
            get { return _Started; }
            set { SetPropertyValue<DateTime>("Started", ref _Started, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Custom("DisplayFormat", "hh:mm:ss tt")]
        [Custom("EditMask", "hh:mm:ss tt")]
        [EditorAlias("CustomTimeOnlyEditor")]
        public DateTime Finished {
            get { return _Finished; }
            set { SetPropertyValue<DateTime>("Finished", ref _Finished, value); }
        }

        [PersistentAlias("Finished - Started")]
        public TimeSpan Hours {
            get
            {
                var tempObject = EvaluateAlias("Hours");
                if (tempObject != null)
                {
                    return (TimeSpan)tempObject;
                } else
                {
                    return TimeSpan.Zero;
                }
            }
        }

        // Description;
        private string _Description;
        [Size(1000)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }

        // TireStatus;
        private TireDettachReason _TireStatus;
        [Custom("AllowEdit", "False")]
        public TireDettachReason TireStatus {
            get { return _TireStatus; }
            set { SetPropertyValue<TireDettachReason>("TireStatus", ref _TireStatus, value); }
        }


        #region List View

        private FixedAsset _Fleet;
        private decimal _Odometer;
        private WheelPosition _WheelPos;
        //private TireActivityTypeEnum _ActivityType;
        private Tire _AffectedTire;
        private TireItem _Item;
        private string _BrandingNo;
        //private string _Remarks;
        [NonPersistent]
        public FixedAsset Fleet {
            get
            {
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        _Fleet = _ToFleet ?? null;
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        _Fleet = _FromFleet ?? null;
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                return _Fleet;
            }
        }

        [NonPersistent]
        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Odometer {
            get
            {
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        _Odometer = _ToOdometer;
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        _Odometer = _FromOdometer;
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                return _Odometer;
            }
        }

        [NonPersistent]
        public WheelPosition WheelPos {
            get
            {
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        _WheelPos = _ToWheelPos ?? null;
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        _WheelPos = _FromWheelPos ?? null;
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                return _WheelPos;
            }
        }

        //[NonPersistent]
        //public TireActivityTypeEnum ActivityType
        //{
        //    get { return _ActivityType; }
        //    set { SetPropertyValue<TireActivityTypeEnum>("ActivityType", ref _ActivityType, value); }
        //}
        [NonPersistent]
        public Tire AffectedTire {
            get
            {
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        _AffectedTire = _AttachTire ?? null;
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        _AffectedTire = _DettachTire ?? null;
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                return _AffectedTire;
            }
        }

        [NonPersistent]
        public TireItem Item {
            get
            {
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        if (_AttachTire != null)
                        {
                            _Item = _AttachTire.TireItem ?? null;
                        }
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        if (_DettachTire != null)
                        {
                            _Item = _DettachTire.TireItem ?? null;
                        }
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                return _Item;
            }
        }

        [NonPersistent]
        public string BrandingNo {
            get
            {
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        if (_AttachTire != null)
                        {
                            _BrandingNo = _NewBrandingNo;
                        }
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        if (_DettachTire != null)
                        {
                            _BrandingNo = _DettachBranding;
                        }
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                return _BrandingNo;
            }
        }

        //[NonPersistent]
        //public string Remarks {
        //    get { return _Remarks; }
        //    set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        //}

        #endregion

        [Association("TireActivityServiceDetails")]
        public XPCollection<TireServiceDetail2> TireActivityServiceDetails {
            get { return GetCollection<TireServiceDetail2>(
                "TireActivityServiceDetails"); }
        }
        private bool _Release;
        [Custom("AllowEdit", "False")]
        public bool Release
        {
            get { return _Release; }
            set { SetPropertyValue("Release", ref _Release, value); }
        }
        
        [Action(AutoCommit = true, Caption = "Release Line(s)")]
        public void ReleaseLine(){
            Release = true;
        }
        public TiremanActivity(Session session)
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
        }

        private TiremanActivity deletedTaId = null;
        protected override void OnDeleting() {
            deletedTaId = this;
            base.OnDeleting();
        }

        protected override void OnSaving() {
            TireServiceDetail2 tsd = null;
            if (!IsDeleted)
            {
                if (TireActivityServiceDetails.Count > 0)
                {
                    var data = TireActivityServiceDetails.Where(o => o.TaId == this);
                    if (data != null && data.Count() == 1)
                    {
                        tsd = this.Session.GetObjectByKey<TireServiceDetail2>(data.First().Oid);
                    }
                } else
                {
                    tsd = new TireServiceDetail2(this.Session);
                    tsd.TaId = this;
                }
                tsd.TireNo = AffectedTire;
                tsd.BrandingNo = BrandingNo;
                tsd.ActivityDate = _ActivityDate;
                switch (this.TiremanActivityType)
                {
                    case TiremanActivityTypeEnum.Attach:
                        tsd.ActivityType = TireActivityTypeEnum.Attached;
                        break;
                    case TiremanActivityTypeEnum.Dettach:
                        tsd.ActivityType = TireActivityTypeEnum.Dettached;
                        break;
                    case TiremanActivityTypeEnum.Replace:
                        break;
                    case TiremanActivityTypeEnum.Transfer:
                        break;
                    default:
                        break;
                }
                tsd.Fleet = Fleet ?? null;
                tsd.Odometer = Odometer;
                tsd.WheelPos = WheelPos ?? null;
                tsd.TreadDepth = TreadDepth;
                tsd.Reason = ActivityReason ?? null;
                tsd.Remarks = Reason;
                tsd.Save();
            } else
            {
                if (deletedTaId != null)
                {
                    if (deletedTaId.TireActivityServiceDetails.Count > 0)
                    {
                        for (int i = deletedTaId.TireActivityServiceDetails.Count - 1; i >= 0; i--)
                        {
                            TireServiceDetail2 deltsd = this.Session.GetObjectByKey<TireServiceDetail2>(deletedTaId.TireActivityServiceDetails[i].Oid);
                            deltsd.Delete();
                        }
                    }
                }
            }
            base.OnSaving();
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private string _ReplacedBrandingNo;
        private string _ReplacedRemarks;
        private TireServiceDetail2 _ReplacedTireDetail;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public SecurityUser CurrentUser
        {
            get
            {
                if (SecuritySystem.CurrentUser != null)
                {
                    _CurrentUser = Session.GetObjectByKey<SecurityUser>(
                    Session.GetKeyValue(SecuritySystem.CurrentUser));
                }
                return _CurrentUser;
            }
        }

        #endregion

    }
}
