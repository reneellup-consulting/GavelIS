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
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class ExpensesAnalyticsDetails : XPObject
    {
        //private int _Seq;
        //private string _BufferId;
        private ExpensesAnalyticsHeader _ReporterId;
        private int _Year;
        //private Contact _Payee;
        //private ContactTypeEnum _PayeeType;
        private ExpenseType _Category;
        private decimal _January;
        private decimal _JanCash;
        private decimal _JanCheck;
        private decimal _JanWire;
        private decimal _JanOthers;
        private decimal _JanCashPer;
        private decimal _JanCheckPer;
        private decimal _JanWirePer;
        private decimal _JanOthersPer;
        private decimal _February;
        private decimal _FebCash;
        private decimal _FebCheck;
        private decimal _FebWire;
        private decimal _FebOthers;
        private decimal _FebCashPer;
        private decimal _FebCheckPer;
        private decimal _FebWirePer;
        private decimal _FebOthersPer;
        private decimal _March;
        private decimal _MarCash;
        private decimal _MarCheck;
        private decimal _MarWire;
        private decimal _MarOthers;
        private decimal _MarCashPer;
        private decimal _MarCheckPer;
        private decimal _MarWirePer;
        private decimal _MarOthersPer;
        private decimal _April;
        private decimal _AprCash;
        private decimal _AprCheck;
        private decimal _AprWire;
        private decimal _AprOthers;
        private decimal _AprCashPer;
        private decimal _AprCheckPer;
        private decimal _AprWirePer;
        private decimal _AprOthersPer;
        private decimal _May;
        private decimal _MayCash;
        private decimal _MayCheck;
        private decimal _MayWire;
        private decimal _MayOthers;
        private decimal _MayCashPer;
        private decimal _MayCheckPer;
        private decimal _MayWirePer;
        private decimal _MayOthersPer;
        private decimal _June;
        private decimal _JunCash;
        private decimal _JunCheck;
        private decimal _JunWire;
        private decimal _JunOthers;
        private decimal _JunCashPer;
        private decimal _JunCheckPer;
        private decimal _JunWirePer;
        private decimal _JunOthersPer;
        private decimal _July;
        private decimal _JulCash;
        private decimal _JulCheck;
        private decimal _JulWire;
        private decimal _JulOthers;
        private decimal _JulCashPer;
        private decimal _JulCheckPer;
        private decimal _JulWirePer;
        private decimal _JulOthersPer;
        private decimal _August;
        private decimal _AugCash;
        private decimal _AugCheck;
        private decimal _AugWire;
        private decimal _AugOthers;
        private decimal _AugCashPer;
        private decimal _AugCheckPer;
        private decimal _AugWirePer;
        private decimal _AugOthersPer;
        private decimal _September;
        private decimal _SepCash;
        private decimal _SepCheck;
        private decimal _SepWire;
        private decimal _SepOthers;
        private decimal _SepCashPer;
        private decimal _SepCheckPer;
        private decimal _SepWirePer;
        private decimal _SepOthersPer;
        private decimal _October;
        private decimal _OctCash;
        private decimal _OctCheck;
        private decimal _OctWire;
        private decimal _OctOthers;
        private decimal _OctCashPer;
        private decimal _OctCheckPer;
        private decimal _OctWirePer;
        private decimal _OctOthersPer;
        private decimal _November;
        private decimal _NovCash;
        private decimal _NovCheck;
        private decimal _NovWire;
        private decimal _NovOthers;
        private decimal _NovCashPer;
        private decimal _NovCheckPer;
        private decimal _NovWirePer;
        private decimal _NovOthersPer;
        private decimal _December;
        private decimal _DecCash;
        private decimal _DecCheck;
        private decimal _DecWire;
        private decimal _DecOthers;
        private decimal _DecCashPer;
        private decimal _DecCheckPer;
        private decimal _DecWirePer;
        private decimal _DecOthersPer;
        //[Custom("AllowEdit", "False")]
        //[RuleRequiredField("", DefaultContexts.Save)]
        //public string BufferId
        //{
        //    get { return _BufferId; }
        //    set { SetPropertyValue("BufferId", ref _BufferId, value); }
        //}

        //[Custom("AllowEdit", "False")]
        //public int Seq
        //{
        //    get { return _Seq; }
        //    set { SetPropertyValue("Seq", ref _Seq, value); }
        //}
        [Custom("AllowEdit", "False")]
        [Association("ExpensesAnalyticsDetailLines")]
        public ExpensesAnalyticsHeader ReporterId
        {
            get { return _ReporterId; }
            set { SetPropertyValue("ReporterId", ref _ReporterId, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value); }
        }
        //[Custom("AllowEdit", "False")]
        //public Contact Payee
        //{
        //    get { return _Payee; }
        //    set { SetPropertyValue("Payee", ref _Payee, value); }
        //}
        //[Custom("AllowEdit", "False")]
        //public ContactTypeEnum PayeeType
        //{
        //    get { return _PayeeType; }
        //    set { SetPropertyValue("PayeeType", ref _PayeeType, value); }
        //}
        [Custom("AllowEdit", "False")]
        public ExpenseType Category
        {
            get { return _Category; }
            set { SetPropertyValue("Category", ref _Category, value); }
        }
        // January
        [Custom("AllowEdit", "False")]
        public decimal January
        {
            get { return _January; }
            set { SetPropertyValue("January", ref _January, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanCash
        {
            get { return _JanCash; }
            set { SetPropertyValue("JanCash", ref _JanCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanCheck
        {
            get { return _JanCheck; }
            set { SetPropertyValue("JanCheck", ref _JanCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanWire
        {
            get { return _JanWire; }
            set { SetPropertyValue("JanWire", ref _JanWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanOthers
        {
            get { return _JanOthers; }
            set { SetPropertyValue("JanOthers", ref _JanOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanCashPer
        {
            get { return _JanCashPer; }
            set { SetPropertyValue("JanCashPer", ref _JanCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanCheckPer
        {
            get { return _JanCheckPer; }
            set { SetPropertyValue("JanCheckPer", ref _JanCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanWirePer
        {
            get { return _JanWirePer; }
            set { SetPropertyValue("JanWirePer", ref _JanWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JanOthersPer
        {
            get { return _JanOthersPer; }
            set { SetPropertyValue("JanOthersPer", ref _JanOthersPer, value); }
        }

        // February
        [Custom("AllowEdit", "False")]
        public decimal February
        {
            get { return _February; }
            set { SetPropertyValue("February", ref _February, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebCash
        {
            get { return _FebCash; }
            set { SetPropertyValue("FebCash", ref _FebCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebCheck
        {
            get { return _FebCheck; }
            set { SetPropertyValue("FebCheck", ref _FebCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebWire
        {
            get { return _FebWire; }
            set { SetPropertyValue("FebWire", ref _FebWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebOthers
        {
            get { return _FebOthers; }
            set { SetPropertyValue("FebOthers", ref _FebOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebCashPer
        {
            get { return _FebCashPer; }
            set { SetPropertyValue("FebCashPer", ref _FebCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebCheckPer
        {
            get { return _FebCheckPer; }
            set { SetPropertyValue("FebCheckPer", ref _FebCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebWirePer
        {
            get { return _FebWirePer; }
            set { SetPropertyValue("FebWirePer", ref _FebWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal FebOthersPer
        {
            get { return _FebOthersPer; }
            set { SetPropertyValue("FebOthersPer", ref _FebOthersPer, value); }
        }
        // March
        [Custom("AllowEdit", "False")]
        public decimal March
        {
            get { return _March; }
            set { SetPropertyValue("March", ref _March, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarCash
        {
            get { return _MarCash; }
            set { SetPropertyValue("MarCash", ref _MarCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarCheck
        {
            get { return _MarCheck; }
            set { SetPropertyValue("MarCheck", ref _MarCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarWire
        {
            get { return _MarWire; }
            set { SetPropertyValue("MarWire", ref _MarWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarOthers
        {
            get { return _MarOthers; }
            set { SetPropertyValue("MarOthers", ref _MarOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarCashPer
        {
            get { return _MarCashPer; }
            set { SetPropertyValue("MarCashPer", ref _MarCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarCheckPer
        {
            get { return _MarCheckPer; }
            set { SetPropertyValue("MarCheckPer", ref _MarCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarWirePer
        {
            get { return _MarWirePer; }
            set { SetPropertyValue("MarWirePer", ref _MarWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MarOthersPer
        {
            get { return _MarOthersPer; }
            set { SetPropertyValue("MarOthersPer", ref _MarOthersPer, value); }
        }
        // April
        [Custom("AllowEdit", "False")]
        public decimal April
        {
            get { return _April; }
            set { SetPropertyValue("April", ref _April, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprCash
        {
            get { return _AprCash; }
            set { SetPropertyValue("AprCash", ref _AprCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprCheck
        {
            get { return _AprCheck; }
            set { SetPropertyValue("AprCheck", ref _AprCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprWire
        {
            get { return _AprWire; }
            set { SetPropertyValue("AprWire", ref _AprWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprOthers
        {
            get { return _AprOthers; }
            set { SetPropertyValue("AprOthers", ref _AprOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprCashPer
        {
            get { return _AprCashPer; }
            set { SetPropertyValue("AprCashPer", ref _AprCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprCheckPer
        {
            get { return _AprCheckPer; }
            set { SetPropertyValue("AprCheckPer", ref _AprCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprWirePer
        {
            get { return _AprWirePer; }
            set { SetPropertyValue("AprWirePer", ref _AprWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AprOthersPer
        {
            get { return _AprOthersPer; }
            set { SetPropertyValue("AprOthersPer", ref _AprOthersPer, value); }
        }
        // May
        [Custom("AllowEdit", "False")]
        public decimal May
        {
            get { return _May; }
            set { SetPropertyValue("May", ref _May, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayCash
        {
            get { return _MayCash; }
            set { SetPropertyValue("MayCash", ref _MayCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayCheck
        {
            get { return _MayCheck; }
            set { SetPropertyValue("MayCheck", ref _MayCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayWire
        {
            get { return _MayWire; }
            set { SetPropertyValue("MayWire", ref _MayWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayOthers
        {
            get { return _MayOthers; }
            set { SetPropertyValue("MayOthers", ref _MayOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayCashPer
        {
            get { return _MayCashPer; }
            set { SetPropertyValue("MayCashPer", ref _MayCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayCheckPer
        {
            get { return _MayCheckPer; }
            set { SetPropertyValue("MayCheckPer", ref _MayCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayWirePer
        {
            get { return _MayWirePer; }
            set { SetPropertyValue("MayWirePer", ref _MayWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal MayOthersPer
        {
            get { return _MayOthersPer; }
            set { SetPropertyValue("MayOthersPer", ref _MayOthersPer, value); }
        }
        // June
        [Custom("AllowEdit", "False")]
        public decimal June
        {
            get { return _June; }
            set { SetPropertyValue("June", ref _June, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunCash
        {
            get { return _JunCash; }
            set { SetPropertyValue("JunCash", ref _JunCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunCheck
        {
            get { return _JunCheck; }
            set { SetPropertyValue("JunCheck", ref _JunCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunWire
        {
            get { return _JunWire; }
            set { SetPropertyValue("JunWire", ref _JunWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunOthers
        {
            get { return _JunOthers; }
            set { SetPropertyValue("JunOthers", ref _JunOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunCashPer
        {
            get { return _JunCashPer; }
            set { SetPropertyValue("JunCashPer", ref _JunCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunCheckPer
        {
            get { return _JunCheckPer; }
            set { SetPropertyValue("JunCheckPer", ref _JunCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunWirePer
        {
            get { return _JunWirePer; }
            set { SetPropertyValue("JunWirePer", ref _JunWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JunOthersPer
        {
            get { return _JunOthersPer; }
            set { SetPropertyValue("JunOthersPer", ref _JunOthersPer, value); }
        }
        // July
        [Custom("AllowEdit", "False")]
        public decimal July
        {
            get { return _July; }
            set { SetPropertyValue("July", ref _July, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulCash
        {
            get { return _JulCash; }
            set { SetPropertyValue("JulCash", ref _JulCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulCheck
        {
            get { return _JulCheck; }
            set { SetPropertyValue("JulCheck", ref _JulCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulWire
        {
            get { return _JulWire; }
            set { SetPropertyValue("JulWire", ref _JulWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulOthers
        {
            get { return _JulOthers; }
            set { SetPropertyValue("JulOthers", ref _JulOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulCashPer
        {
            get { return _JulCashPer; }
            set { SetPropertyValue("JulCashPer", ref _JulCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulCheckPer
        {
            get { return _JulCheckPer; }
            set { SetPropertyValue("JulCheckPer", ref _JulCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulWirePer
        {
            get { return _JulWirePer; }
            set { SetPropertyValue("JulWirePer", ref _JulWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal JulOthersPer
        {
            get { return _JulOthersPer; }
            set { SetPropertyValue("JulOthersPer", ref _JulOthersPer, value); }
        }
        // August
        [Custom("AllowEdit", "False")]
        public decimal August
        {
            get { return _August; }
            set { SetPropertyValue("August", ref _August, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugCash
        {
            get { return _AugCash; }
            set { SetPropertyValue("AugCash", ref _AugCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugCheck
        {
            get { return _AugCheck; }
            set { SetPropertyValue("AugCheck", ref _AugCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugWire
        {
            get { return _AugWire; }
            set { SetPropertyValue("AugWire", ref _AugWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugOthers
        {
            get { return _AugOthers; }
            set { SetPropertyValue("AugOthers", ref _AugOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugCashPer
        {
            get { return _AugCashPer; }
            set { SetPropertyValue("AugCashPer", ref _AugCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugCheckPer
        {
            get { return _AugCheckPer; }
            set { SetPropertyValue("AugCheckPer", ref _AugCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugWirePer
        {
            get { return _AugWirePer; }
            set { SetPropertyValue("AugWirePer", ref _AugWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal AugOthersPer
        {
            get { return _AugOthersPer; }
            set { SetPropertyValue("AugOthersPer", ref _AugOthersPer, value); }
        }
        // September
        [Custom("AllowEdit", "False")]
        public decimal September
        {
            get { return _September; }
            set { SetPropertyValue("September", ref _September, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepCash
        {
            get { return _SepCash; }
            set { SetPropertyValue("SepCash", ref _SepCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepCheck
        {
            get { return _SepCheck; }
            set { SetPropertyValue("SepCheck", ref _SepCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepWire
        {
            get { return _SepWire; }
            set { SetPropertyValue("SepWire", ref _SepWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepOthers
        {
            get { return _SepOthers; }
            set { SetPropertyValue("SepOthers", ref _SepOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepCashPer
        {
            get { return _SepCashPer; }
            set { SetPropertyValue("SepCashPer", ref _SepCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepCheckPer
        {
            get { return _SepCheckPer; }
            set { SetPropertyValue("SepCheckPer", ref _SepCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepWirePer
        {
            get { return _SepWirePer; }
            set { SetPropertyValue("SepWirePer", ref _SepWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal SepOthersPer
        {
            get { return _SepOthersPer; }
            set { SetPropertyValue("SepOthersPer", ref _SepOthersPer, value); }
        }
        // October
        [Custom("AllowEdit", "False")]
        public decimal October
        {
            get { return _October; }
            set { SetPropertyValue("October", ref _October, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctCash
        {
            get { return _OctCash; }
            set { SetPropertyValue("OctCash", ref _OctCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctCheck
        {
            get { return _OctCheck; }
            set { SetPropertyValue("OctCheck", ref _OctCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctWire
        {
            get { return _OctWire; }
            set { SetPropertyValue("OctWire", ref _OctWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctOthers
        {
            get { return _OctOthers; }
            set { SetPropertyValue("OctOthers", ref _OctOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctCashPer
        {
            get { return _OctCashPer; }
            set { SetPropertyValue("OctCashPer", ref _OctCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctCheckPer
        {
            get { return _OctCheckPer; }
            set { SetPropertyValue("OctCheckPer", ref _OctCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctWirePer
        {
            get { return _OctWirePer; }
            set { SetPropertyValue("OctWirePer", ref _OctWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal OctOthersPer
        {
            get { return _OctOthersPer; }
            set { SetPropertyValue("OctOthersPer", ref _OctOthersPer, value); }
        }
        // November
        [Custom("AllowEdit", "False")]
        public decimal November
        {
            get { return _November; }
            set { SetPropertyValue("November", ref _November, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovCash
        {
            get { return _NovCash; }
            set { SetPropertyValue("NovCash", ref _NovCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovCheck
        {
            get { return _NovCheck; }
            set { SetPropertyValue("NovCheck", ref _NovCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovWire
        {
            get { return _NovWire; }
            set { SetPropertyValue("NovWire", ref _NovWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovOthers
        {
            get { return _NovOthers; }
            set { SetPropertyValue("NovOthers", ref _NovOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovCashPer
        {
            get { return _NovCashPer; }
            set { SetPropertyValue("NovCashPer", ref _NovCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovCheckPer
        {
            get { return _NovCheckPer; }
            set { SetPropertyValue("NovCheckPer", ref _NovCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovWirePer
        {
            get { return _NovWirePer; }
            set { SetPropertyValue("NovWirePer", ref _NovWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal NovOthersPer
        {
            get { return _NovOthersPer; }
            set { SetPropertyValue("NovOthersPer", ref _NovOthersPer, value); }
        }
        // December
        [Custom("AllowEdit", "False")]
        public decimal December
        {
            get { return _December; }
            set { SetPropertyValue("December", ref _December, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecCash
        {
            get { return _DecCash; }
            set { SetPropertyValue("DecCash", ref _DecCash, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecCheck
        {
            get { return _DecCheck; }
            set { SetPropertyValue("DecCheck", ref _DecCheck, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecWire
        {
            get { return _DecWire; }
            set { SetPropertyValue("DecWire", ref _DecWire, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecOthers
        {
            get { return _DecOthers; }
            set { SetPropertyValue("DecOthers", ref _DecOthers, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecCashPer
        {
            get { return _DecCashPer; }
            set { SetPropertyValue("DecCashPer", ref _DecCashPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecCheckPer
        {
            get { return _DecCheckPer; }
            set { SetPropertyValue("DecCheckPer", ref _DecCheckPer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecWirePer
        {
            get { return _DecWirePer; }
            set { SetPropertyValue("DecWirePer", ref _DecWirePer, value); }
        }
        [Custom("AllowEdit", "False")]
        public decimal DecOthersPer
        {
            get { return _DecOthersPer; }
            set { SetPropertyValue("DecOthersPer", ref _DecOthersPer, value); }
        }

        [PersistentAlias("January + February + March + April + May + June + July + August + September + October + November + December")]
        [Custom("DisplayFormat", "n")]
        public decimal Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (decimal)tempObject;
                }
                else
                {
                    return 0;
                }
            }
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
        public ExpensesAnalyticsDetails(Session session)
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

    }

}
