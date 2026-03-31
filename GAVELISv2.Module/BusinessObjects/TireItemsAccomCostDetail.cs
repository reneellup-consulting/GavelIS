using System;
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
    public enum AccTireItemClassEnum
    {
        None,
        Tire,
        Flap,
        Tube,
        Rim
    }
    public enum TireTypeEnum
    {
        None,
        Tubeless,
        TubeType
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    [RuleCombinationOfPropertiesIsUnique("TireItemsAccomCostDetailRule01", DefaultContexts.Save, "HeaderId,SourceId,SourceDetId")]
    public class TireItemsAccomCostDetail : XPObject
    {
        private TireItemsAccomulatedCost _HeaderId;
        [Custom("AllowEdit", "False")]
        [Association("TireItemsAccomulatedCost-Details")]
        public TireItemsAccomulatedCost HeaderId
        {
            get { return _HeaderId; }
            set { SetPropertyValue("HeaderId", ref _HeaderId, value); }
        }
        // EntryDate
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get { return _EntryDate; }
            set { SetPropertyValue("EntryDate", ref _EntryDate, value); }
        }
        // SourceId
        [Custom("AllowEdit", "False")]
        [DisplayName("Source")]
        public string SourceId
        {
            get { return _SourceId; }
            set { SetPropertyValue("SourceId", ref _SourceId, value); }
        }

        // SourceDetId
        [Custom("AllowEdit", "False")]
        [DisplayName("Line Ref#")]
        public string SourceDetId
        {
            get { return _SourceDetId; }
            set { SetPropertyValue("SourceDetId", ref _SourceDetId, value); }
        }
        // RWS Tire Detail
        [Custom("AllowEdit", "False")]
        [DisplayName("Request Ref")]
        public RwsTireDetail TireReqDetail
        {
            get { return _TireReqDetail; }
            set { SetPropertyValue("TireReqDetail", ref _TireReqDetail, value); }
        }
        // Meter
        [Custom("AllowEdit", "False")]
        [DisplayName("ODO #")]
        public decimal Meter
        {
            get {
                if (_TireReqDetail != null)
                {
                    _Meter = _TireReqDetail.OdoRead;
                }
                return _Meter; }
        }
        // Unit No
        [Custom("AllowEdit", "False")]
        [DisplayName("Unit #")]
        public FixedAsset Unit
        {
            get
            {
                if (_TireReqDetail != null)
                {
                    return _TireReqDetail.TireIssueId.LastFleet ?? null;
                }
                return null;
            }
        }
        [Custom("AllowEdit", "False")]
        public TireServiceDetail2 ReplacedTire
        {
            get { return _ReplacedTire; }
            set { SetPropertyValue("ReplacedTire", ref _ReplacedTire, value); }
        }
        [Custom("AllowEdit", "False")]
        public TireServiceDetail2 NewBranding
        {
            get { return _NewBranding; }
            set { SetPropertyValue("NewBranding", ref _NewBranding, value); }
        }
        // Last Recap
        [Custom("AllowEdit", "False")]
        public TireServiceDetail2 LastRecap
        {
            get { return _LastRecap; }
            set { SetPropertyValue("LastRecap", ref _LastRecap, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("Prev. #")]
        public string PreviousBranding
        {
            get { return _PreviousBranding; }
            set { SetPropertyValue("PreviousBranding", ref _PreviousBranding, value); }
        }
        [Custom("AllowEdit", "False")]
        public string Remarks
        {
            get { return _Remarks; }
            set { SetPropertyValue("Remarks", ref _Remarks, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal LastRecapCost
        {
            get { return _LastRecap!=null?_LastRecap.Cost:0m; }
        }
        [Custom("AllowEdit", "False")]
        public Receipt RecapInvoice
        {
            get {
                if (_LastRecap != null)
                {
                    _RecapInvoice = Session.FindObject<Receipt>(BinaryOperator.Parse("[SourceNo] = ?", _LastRecap.ReferenceNo)) ?? null;
                }
                return _RecapInvoice; }
        }
        // Old Tire
        [Custom("AllowEdit", "False")]
        [DisplayName("OLD TIRE #")]
        public string OldTireNo
        {
            get
            {
                if (_TireReqDetail != null)
                {
                    return _TireReqDetail.RepTireBranding;
                }
                return string.Empty;
            }
        }
        // Old Make
        [Custom("AllowEdit", "False")]
        public TireMake Make
        {
            get
            {
                if (_TireReqDetail != null && _TireReqDetail.OldTireItem != null)
                {
                    return _TireReqDetail.OldTireItem.Make ?? null;
                }
                return null;
            }
        }
        // New Tire
        [Custom("AllowEdit", "False")]
        [DisplayName("Issued Tire")]
        public Tire Issue
        {
            get { return _Issue; }
            set { SetPropertyValue("Issue", ref _Issue, value); }
        }
        [Custom("AllowEdit", "False")]
        [DisplayName("NEW TIRE #")]
        public string NewTireNo
        {
            get
            {
                if (_Issue != null)
                {
                    return _Issue.TireServiceDetails2.FirstOrDefault() != null ? _Issue.TireServiceDetails2.FirstOrDefault().BrandingNo : string.Empty;
                }
                return string.Empty;
            }
        }
        // New Make
        [Custom("AllowEdit", "False")]
        public TireMake NewMake
        {
            get
            {
                if (_Issue != null && _Issue.TireItem != null)
                {
                    return _Issue.TireItem.Make ?? null;
                }
                return null;
            }
        }
        // New Type
        [Custom("AllowEdit", "False")]
        public TireType NewType
        {
            get
            {
                if (_Issue != null && _Issue.TireItem != null)
                {
                    return _Issue.TireItem.Type ?? null;
                }
                return null;
            }
        }
        // New Size
        [Custom("AllowEdit", "False")]
        public TireSize NewSize
        {
            get
            {
                if (_Issue != null && _Issue.TireItem != null)
                {
                    return _Issue.TireItem.Size ?? null;
                }
                return null;
            }
        }
        // Invoice #
        [Custom("AllowEdit", "False")]
        [DisplayName("Invoice #")]
        public string InvoiceNo
        {
            get
            {
                if (_TireReqDetail != null)
                {
                    return _TireReqDetail.InvoiceNo;
                }
                return string.Empty;
            }
        }
        // TireItemClass
        [Custom("AllowEdit", "False")]
        public AccTireItemClassEnum TireItemClass
        {
            get { return _TireItemClass; }
            set { SetPropertyValue("TireItemClass", ref _TireItemClass, value); }
        }
        // TireType
        [Custom("AllowEdit", "False")]
        public TireTypeEnum TireType
        {
            get { return _TireType; }
            set { SetPropertyValue("TireType", ref _TireType, value); }
        }
        // ItemNo
        [Custom("AllowEdit", "False")]
        public Item ItemNo
        {
            get { return _ItemNo; }
            set { SetPropertyValue("ItemNo", ref _ItemNo, value); }
        }
        [Custom("AllowEdit", "False")]
        public TireItem RecapItem
        {
            get {
                if (_ItemNo.GetType() == typeof(TireItem))
                {
                    _RecapItem = _ItemNo as TireItem;
                }
                return _RecapItem; }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string Condition
        {
            get {
                if (_ItemNo != null && _ItemNo.GetType() == typeof(TireItem))
                {
                    switch ((_ItemNo as TireItem).TireItemClass)
                    {
                        case TireItemClassEnum.None:
                            return "Non Applicable";
                        case TireItemClassEnum.BrandNewTire:
                            return "Brandnew";
                        case TireItemClassEnum.SecondHandTire:
                            return "Recap";
                        case TireItemClassEnum.RecappedTire:
                            return "Recap";
                        case TireItemClassEnum.Flap:
                            return "Non Applicable";
                        case TireItemClassEnum.Tube:
                            return "Non Applicable";
                        case TireItemClassEnum.Rim:
                            return "Non Applicable";
                        case TireItemClassEnum.ScrappedTire:
                            return "Non Applicable";
                        case TireItemClassEnum.OriginalTire:
                            return "Non Applicable";
                        default:
                            return "Non Applicable";
                    }
                }
                return "Non Applicable"; }
        }
        [Custom("AllowEdit", "False")]
        public string LineCondition
        {
            get { return _LineCondition; }
            set { SetPropertyValue("LineCondition", ref _LineCondition, value); }
        }
        // Qty
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Qty
        {
            get { return _Qty; }
            set { SetPropertyValue("Qty", ref _Qty, value); }
        }
        // Cost
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal Cost
        {
            get { return _Cost; }
            set { SetPropertyValue("Cost", ref _Cost, value); }
        }
        [DisplayName("Total Cost")]
        [PersistentAlias("Qty * Cost")]
        [Custom("DisplayFormat", "n")]
        public decimal SubTotal
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("SubTotal");
                    if (tempObject != null) { return (decimal)tempObject; }
                    else
                    {
                        return 0;
                    }

                }
                catch (Exception)
                {

                    return 0;
                }
            }
        }
        public TireItemsAccomCostDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here or place it only when the IsLoading property is false:
            // if (!IsLoading){
            //    It is now OK to place your initialization code here.
            // }
            // or as an alternative, move your initialization code into the AfterConstruction method.
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        #region Get Current User

        private SecurityUser _CurrentUser;
        private DateTime _EntryDate;
        private string _SourceId;
        private string _SourceDetId;
        //private int _Year;
        //private string _Quarter;
        //private MonthsEnum _Month;
        private AccTireItemClassEnum _TireItemClass;
        private TireTypeEnum _TireType;
        private Item _ItemNo;
        private decimal _Qty;
        private decimal _Cost;
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

        #region Registry Info

        private MonthsEnum _Month;
        private string _Quarter;
        private int _Year;
        private string _MonthSorter;
        private RwsTireDetail _TireReqDetail;
        private decimal _Meter;
        private Tire _Issue;
        private TireServiceDetail2 _LastRecap;
        private string _LineCondition;
        private Receipt _RecapInvoice;
        private TireItem _RecapItem;
        private TireServiceDetail2 _NewBranding;
        private TireServiceDetail2 _ReplacedTire;
        private string _PreviousBranding;
        private string _Remarks;
        [Custom("DisplayFormat", "n")]
        [NonPersistent]
        public MonthsEnum Month
        {
            get
            {
                _Month = _EntryDate.Month == 1 ? MonthsEnum.January : _EntryDate.Month
                 == 2 ? MonthsEnum.February : _EntryDate.Month == 3 ? MonthsEnum.
                March : _EntryDate.Month == 4 ? MonthsEnum.April : _EntryDate.Month ==
                5 ? MonthsEnum.May : _EntryDate.Month == 6 ? MonthsEnum.June :
                _EntryDate.Month == 7 ? MonthsEnum.July : _EntryDate.Month == 8 ?
                MonthsEnum.August : _EntryDate.Month == 9 ? MonthsEnum.September
                 : _EntryDate.Month == 10 ? MonthsEnum.October : _EntryDate.Month == 11
                 ? MonthsEnum.November : _EntryDate.Month == 12 ? MonthsEnum.
                December : MonthsEnum.None;
                return _Month;
            }
        }

        [NonPersistent]
        public string Quarter
        {
            get
            {
                switch (Month)
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
        public int Year
        {
            get
            {
                return _EntryDate.Year;
                ;
            }
        }

        [NonPersistent]
        public string MonthSorter
        {
            get
            {
                switch (Month)
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

    }

}
