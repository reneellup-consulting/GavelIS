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
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [NavigationItem(false)]
    public class ThirteenthGeneratorDetail : XPObject
    {
        private ThirteenthGeneratorHeader _ParentID;
        private BusinessObjects.Employee _Employee;
        private decimal _DecLastYr;
        private decimal _JanThisYr;
        private decimal _FebThisYr;
        private decimal _MarThisYr;
        private decimal _AprThisYr;
        private decimal _MayThisYr;
        private decimal _JunThisYr;
        private decimal _JulThisYr;
        private decimal _AugThisYr;
        private decimal _SepThisYr;
        private decimal _OctThisYr;
        private decimal _NovThisYr;
        private decimal _DecThisYr;
        private decimal _Absences;
        private decimal _SatDuties;
        private decimal _Basic;
        private decimal _ThirteenthMonthPay;
        
        [Custom("AllowEdit", "False")]
        [Association("ThirteenthGeneratorDetails")]
        public ThirteenthGeneratorHeader ParentID
        {
            get { return _ParentID; }
            set { SetPropertyValue("ParentID", ref _ParentID, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee Employee
        {
            get { return _Employee; }
            set { SetPropertyValue("Employee", ref _Employee, value); }
        }
        [DisplayName("Dec [0]")]
        [Custom("AllowEdit", "False")]
        public decimal DecLastYr
        {
            get { return _DecLastYr; }
            set
            {
                SetPropertyValue("DecLastYr", ref _DecLastYr, value);
            }
        }
        [DisplayName("Jan [1]")]
        [Custom("AllowEdit", "False")]
        public decimal JanThisYr
        {
            get { return _JanThisYr; }
            set
            {
                SetPropertyValue("JanThisYr", ref _JanThisYr, value);
            }
        }
        [DisplayName("Feb [2]")]
        [Custom("AllowEdit", "False")]
        public decimal FebThisYr
        {
            get { return _FebThisYr; }
            set
            {
                SetPropertyValue("FebThisYr", ref _FebThisYr, value);
            }
        }
        [DisplayName("Mar [3]")]
        [Custom("AllowEdit", "False")]
        public decimal MarThisYr
        {
            get { return _MarThisYr; }
            set
            {
                SetPropertyValue("MarThisYr", ref _MarThisYr, value);
            }
        }
        [DisplayName("Apr [4]")]
        [Custom("AllowEdit", "False")]
        public decimal AprThisYr
        {
            get { return _AprThisYr; }
            set
            {
                SetPropertyValue("AprThisYr", ref _AprThisYr, value);
            }
        }
        [DisplayName("May [5]")]
        [Custom("AllowEdit", "False")]
        public decimal MayThisYr
        {
            get { return _MayThisYr; }
            set
            {
                SetPropertyValue("MayThisYr", ref _MayThisYr, value);
            }
        }
        [DisplayName("Jun [6]")]
        [Custom("AllowEdit", "False")]
        public decimal JunThisYr
        {
            get { return _JunThisYr; }
            set
            {
                SetPropertyValue("JunThisYr", ref _JunThisYr, value);
            }
        }
        [DisplayName("Jul [7]")]
        [Custom("AllowEdit", "False")]
        public decimal JulThisYr
        {
            get { return _JulThisYr; }
            set
            {
                SetPropertyValue("JulThisYr", ref _JulThisYr, value);
            }
        }
        [DisplayName("Aug [8]")]
        [Custom("AllowEdit", "False")]
        public decimal AugThisYr
        {
            get { return _AugThisYr; }
            set
            {
                SetPropertyValue("AugThisYr", ref _AugThisYr, value);
            }
        }
        [DisplayName("Sep [9]")]
        [Custom("AllowEdit", "False")]
        public decimal SepThisYr
        {
            get { return _SepThisYr; }
            set
            {
                SetPropertyValue("SepThisYr", ref _SepThisYr, value);
            }
        }
        [DisplayName("Oct [10]")]
        [Custom("AllowEdit", "False")]
        public decimal OctThisYr
        {
            get { return _OctThisYr; }
            set
            {
                SetPropertyValue("OctThisYr", ref _OctThisYr, value);
            }
        }
        [DisplayName("Nov [11]")]
        [Custom("AllowEdit", "False")]
        public decimal NovThisYr
        {
            get { return _NovThisYr; }
            set
            {
                SetPropertyValue("NovThisYr", ref _NovThisYr, value);
            }
        }
        [DisplayName("Dec [12]")]
        [Custom("AllowEdit", "False")]
        public decimal DecThisYr
        {
            get { return _DecThisYr; }
            set
            {
                SetPropertyValue("DecThisYr", ref _DecThisYr, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public decimal Absences
        {
            get { return _Absences; }
            set
            {
                SetPropertyValue("Absences", ref _Absences, value);
            }
        }

        [DisplayName("SD")]
        [Custom("AllowEdit", "False")]
        public decimal SatDuties
        {
            get { return _SatDuties; }
            set
            {
                SetPropertyValue("SatDuties", ref _SatDuties, value);
            }
        }

        [PersistentAlias("DecLastYr + JanThisYr + FebThisYr + MarThisYr + AprThisYr + MayThisYr + JunThisYr + JulThisYr + AugThisYr + SepThisYr + OctThisYr + NovThisYr + DecThisYr")]
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
        [Custom("AllowEdit", "False")]
        public decimal Basic
        {
            get { return _Basic; }
            set
            {
                SetPropertyValue("Basic", ref _Basic, value);
            }
        }

        [PersistentAlias("Basic * Total")]
        public decimal EarnedThisYear
        {
            get
            {
                object tempObject = EvaluateAlias("EarnedThisYear");
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

        [PersistentAlias("EarnedThisYear / 12")]
        public decimal AttendanceBasedPay
        {
            get
            {
                object tempObject = EvaluateAlias("AttendanceBasedPay");
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

        [PersistentAlias("Basic * 26")]
        public decimal BasicBasedPay
        {
            get
            {
                object tempObject = EvaluateAlias("BasicBasedPay");
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

        public decimal ThirteenthMonthPay
        {
            get { return _ThirteenthMonthPay; }
            set
            {
                SetPropertyValue("ThirteenthMonthPay", ref _ThirteenthMonthPay, value);
            }
        }
        public ThirteenthGeneratorDetail(Session session)
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
