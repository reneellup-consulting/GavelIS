using System;
using System.Text;
using System.Globalization;
using System.Linq;
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
    public enum TireActivityTypeEnum
    {
        Attached,
        Dettached,
        None,
        Disposed
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class TireServiceDetail2 : XPObject {
        private Guid _RowID;
        private Tire _TireNo;
        private DateTime _EntryDate;
        private string _BrandingNo;
        private DateTime _ActivityDate;
        private TireActivityTypeEnum _ActivityType;
        private FixedAsset _Fleet;
        private WheelPosition _WheelPos;
        private decimal _TreadDepth;
        private decimal _Odometer;
        private TireDettachReason _Reason;
        private Vendor _Vendor;
        private string _ReferenceNo;
        private decimal _Cost;
        private string _Remarks;
        private TiremanActivity _TaId;
        private TireToRetreader _TorId;
        private TireForSale _TfsId;
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TireServiceDetails2")]
        public Tire TireNo {
            get { return _TireNo; }
            set
            {
                SetPropertyValue<Tire>("TireNo", ref _TireNo, value);
                if (!IsLoading && !IsSaving && _TireNo != null)
                {
                    BrandingNo = _TireNo.LastBrandingNo;
                    Fleet = _TireNo.LastFleet ?? null;
                    ActivityType = TireActivityTypeEnum.Attached;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue<DateTime>("EntryDate", ref _EntryDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string BrandingNo {
            get { return _BrandingNo; }
            set { SetPropertyValue<string>("BrandingNo", ref _BrandingNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public DateTime ActivityDate {
            get { return _ActivityDate; }
            set { SetPropertyValue<DateTime>("ActivityDate", ref _ActivityDate, value); }
        }

        public TireActivityTypeEnum ActivityType {
            get { return _ActivityType; }
            set { SetPropertyValue<TireActivityTypeEnum>("ActivityType", ref _ActivityType, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("FleetTireServiceDetails")]
        public FixedAsset Fleet {
            get { return _Fleet; }
            set { SetPropertyValue<FixedAsset>("Fleet", ref _Fleet, value); }
        }

        public WheelPosition WheelPos {
            get { return _WheelPos; }
            set { SetPropertyValue<WheelPosition>("WheelPos", ref _WheelPos, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal TreadDepth {
            get { return _TreadDepth; }
            set { SetPropertyValue<decimal>("TreadDepth", ref _TreadDepth, value); }
        }

        [EditorAlias("MeterTypePropertyEditor")]
        public decimal Odometer {
            get { return _Odometer; }
            set { SetPropertyValue<decimal>("Odometer", ref _Odometer, value); }
        }

        [NonPersistent]
        public decimal DettachOdo {
            get
            {
                if (ActivityType == TireActivityTypeEnum.Dettached)
                {
                    return Odometer;
                } else
                {
                    return 0 - Odometer;
                }
            }
        }
        [NonPersistent]
        public decimal lastOdo {
            get
            {
                try
                {
                    var data = _TireNo.TireServiceDetails2.Where(o=>o.Oid < Oid).LastOrDefault();
                    if (data!=null)
                    {
                        return data.Odometer;
                    }
                    else
                    {
                        return 0m;
                    }
                }
                catch (Exception)
                {
                    return 0m;
                }
            }
        }
        [NonPersistent]
        public decimal KmRun {
            get
            {
                var data = _TireNo.TireServiceDetails2.Where(o => o.Oid < Oid).LastOrDefault();
                if (data == null)
                {
                    return 0m;
                }
                decimal odo = Odometer - lastOdo;
                if (odo > 0m)
                {
                    return Odometer - lastOdo;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [NonPersistent]
        public decimal lastCost
        {
            get
            {
                try
                {
                    var data = _TireNo.TireServiceDetails2.Where(o => o.Oid < Oid && o.ActivityType == TireActivityTypeEnum.Dettached).LastOrDefault();
                    if (data != null)
                    {
                        return data.Cost;
                    }
                    else
                    {
                        return TireNo.Cost;
                    }
                }
                catch (Exception)
                {
                    return 0m;
                }
            }
        }
        [PersistentAlias("lastCost/KmRun")]
        [DisplayName("Cost/Km")]
        public decimal CostPerKm
        {
            get
            {
                //decimal res = 0m;
                //try
                //{
                //    var data = from doms in _TireNo.TireServiceDetails2
                //               where doms.BrandingNo == _BrandingNo && doms.Oid < Oid
                //               orderby doms.Oid
                //               select doms;
                //    if (data != null && KmRun != 0m)
                //    {
                //        res = data.LastOrDefault().TireNo.Cost / KmRun;
                //    }
                //    return res;
                //}
                //catch (Exception)
                //{
                //    return res;
                //}
                if (lastCost == 0m || KmRun == 0m || Odometer == KmRun)
                {
                    return 0m;
                }
                object tempObject = EvaluateAlias("CostPerKm");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0m;
                }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public TireDettachReason Reason {
            get { return _Reason; }
            set
            {
                SetPropertyValue<TireDettachReason>("Reason", ref _Reason, value);
                if (!IsLoading && !IsSaving && _Reason != null)
                {
                    Vendor = _Reason.Vendor ?? _Vendor;
                }
            }
        }

        public Vendor Vendor {
            get { return _Vendor; }
            set { SetPropertyValue<Vendor>("Vendor", ref _Vendor, value); }
        }

        public string ReferenceNo {
            get { return _ReferenceNo; }
            set { SetPropertyValue<string>("ReferenceNo", ref _ReferenceNo, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue<decimal>("Cost", ref _Cost, value); }
        }

        [Size(500)]
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        [Association("TireActivityServiceDetails")]
        public TiremanActivity TaId {
            get { return _TaId; }
            set { SetPropertyValue<TiremanActivity>("TaId", ref _TaId, value); }
        }

        [Custom("AllowEdit", "False")]
        public TireToRetreader TorId {
            get { return _TorId; }
            set { SetPropertyValue<TireToRetreader>("TorId", ref _TorId, value); }
        }
        [Custom("AllowEdit", "False")]
        public TireForSale TfsId {
            get { return _TfsId; }
            set { SetPropertyValue<TireForSale>("TfsId", ref _TfsId, value); }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        public double Seq {
            get
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                DateTime dts = DateTime.MinValue;
                //if (_BrandingNo=="4819000366")
                //{

                //}
                if (_Fleet != null && _TireNo != null && _TireNo.TireNo.Length==10 && _TireNo.FirstActivityDate >= DateTime.Parse("2019-01-10") && _TireNo.FirstActivityType == TireActivityTypeEnum.Attached)
                {
                    //string tno = _TireNo.TireNo.Substring(5);
                    //int i1 = _ActivityDate.Year;
                    //int i2 = _ActivityDate.Month;
                    //int i3 = _ActivityDate.Day;
                    //dts = new DateTime(i1, i2, i3);
                    string[] splt1 = _Fleet.No.Split('-');
                    //return dts.ToOADate() + double.Parse(splt1[0] + splt1[1] + tno);
                    if (splt1.Count()>1)
                    {
                        return double.Parse(_TireNo.TireNo + splt1[1]);
                    }
                    else
                    {
                        string v = new string(splt1[0].Where(c => char.IsDigit(c)).ToArray());
                        return double.Parse(_TireNo.TireNo + v);
                    }
                }
                if (_Fleet != null && !string.IsNullOrEmpty(_BrandingNo))
                {
                    if (_Fleet.GetType() == typeof(FATruck))
                    {
                        string[] lstr = _BrandingNo.Split();
                        int i1 = int.Parse(lstr[0].Substring(4, 2));
                        int i2 = int.Parse(lstr[0].Substring(0, 2));
                        int i3 = int.Parse(lstr[0].Substring(2, 2));
                        if (i2 > 12)
                        {
                            i2 = 1;
                        }
                        dts = new DateTime(i1 + 2000, i2, i3);
                        //dts = DateTime.ParseExact(lstr[0], "mmddyy", provider);
                    }

                    if (_Fleet.GetType() == typeof(FATrailer))
                    {
                        string[] lstr = _BrandingNo.Split();
                        int i1 = int.Parse(lstr[0].Substring(6, 2));
                        int i2 = int.Parse(lstr[0].Substring(2, 2));
                        int i3 = int.Parse(lstr[0].Substring(4, 2));
                        if (i2 > 12)
                        {
                            i2 = 1;
                        }
                        dts = new DateTime(i1 + 2000, i2, i3);
                        //dts = DateTime.ParseExact(lstr[0], "mmddyy", provider);
                    }
                }
                string[] str = _BrandingNo != null ? _BrandingNo.Split() : string.Empty.Split();
                if (str.Count() == 3)
                {
                    if (str[2] == string.Empty)
                    {
                        str[2] = "0";

                    }
                    return dts.ToOADate() + double.Parse(str[2]);

                } else if (str.Count() == 2)
                {
                    return dts.ToOADate() + 20;
                } else
                {
                    return 0;
                }
            }
        }

        [Aggregated,
        Association("TireServiceInsConDetails")]
        public XPCollection<TireServiceInsConDetail> TireServiceInsConDetails {
            get { return GetCollection<TireServiceInsConDetail>("TireServiceInsConDetails"); }
        }

        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Last Problems")]
        public string Problems {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in TireServiceInsConDetails)
                {
                    if (item.Condition != null)
                    {
                        sb.AppendFormat("{0};", item.Condition.Code);
                    }
                }
                if (sb.Length != 0)
                {
                    sb.Remove(sb.Length - 1, 1);
                }
                return sb.ToString();
            }
        }

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum GMonth {
            get
            {
                _Month = ActivityDate.Month == 1 ? MonthsEnum.January : ActivityDate.Month
                 == 2 ? MonthsEnum.February : ActivityDate.Month == 3 ? MonthsEnum.
                March : ActivityDate.Month == 4 ? MonthsEnum.April : ActivityDate.Month ==
                5 ? MonthsEnum.May : ActivityDate.Month == 6 ? MonthsEnum.June :
                ActivityDate.Month == 7 ? MonthsEnum.July : ActivityDate.Month == 8 ?
                MonthsEnum.August : ActivityDate.Month == 9 ? MonthsEnum.September
                 : ActivityDate.Month == 10 ? MonthsEnum.October : ActivityDate.Month == 11
                 ? MonthsEnum.November : ActivityDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string GQuarter {
            get
            {
                switch (GMonth)
                {
                    case MonthsEnum.None:
                        break;
                    case MonthsEnum.January:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.February:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.March:
                        _Quarter = "1st QTR";
                        break;
                    case MonthsEnum.April:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.May:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.June:
                        _Quarter = "2nd QTR";
                        break;
                    case MonthsEnum.July:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.August:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.September:
                        _Quarter = "3rd QTR";
                        break;
                    case MonthsEnum.October:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.November:
                        _Quarter = "4th QTR";
                        break;
                    case MonthsEnum.December:
                        _Quarter = "4th QTR";
                        break;
                    default:
                        break;
                }
                return _Quarter;
            }
        }

        [NonPersistent]
        [Custom("DisplayFormat", "d")]
        //[Association("TireMaintenanceProgramDetails")]
        public int GYear {
            get
            {
                return ActivityDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string GMonthSorter {
            get
            {
                switch (GMonth)
                {
                    case MonthsEnum.None:
                        return "00 NONE";
                    case MonthsEnum.January:
                        return "01 JANUARY";
                    case MonthsEnum.February:
                        return "02 FEBRUARY";
                    case MonthsEnum.March:
                        return "03 MARCH";
                    case MonthsEnum.April:
                        return "04 APRIL";
                    case MonthsEnum.May:
                        return "05 MAY";
                    case MonthsEnum.June:
                        return "06 JUNE";
                    case MonthsEnum.July:
                        return "07 JULY";
                    case MonthsEnum.August:
                        return "08 AUGUST";
                    case MonthsEnum.September:
                        return "09 SEPTEMBER";
                    case MonthsEnum.October:
                        return "10 OCTOBER";
                    case MonthsEnum.November:
                        return "11 NOVEMBER";
                    case MonthsEnum.December:
                        return "12 DECEMBER";
                    default:
                        return "00 NONE";
                }
            }
        }

        #endregion


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

        public TireServiceDetail2(Session session)
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
            RowID = Guid.NewGuid();
            EntryDate = DateTime.Now;

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

        #region Get Current User

        private SecurityUser _CurrentUser;
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
