using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Reports;
using System.IO;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class ThirteenthGeneratorHeader : XPObject
    {
        private int _Year;
        private DateTime _StartDate;
        private DateTime _EndDate;
        private int _DecLastYr;
        private int _JanThisYr;
        private int _FebThisYr;
        private int _MarThisYr;
        private int _AprThisYr;
        private int _MayThisYr;
        private int _JunThisYr;
        private int _JulThisYr;
        private int _AugThisYr;
        private int _SepThisYr;
        private int _OctThisYr;
        private int _NovThisYr;
        private int _DecThisYr;

        private int _TotalWithoutSat = 0;

        [RuleUniqueValue("", DefaultContexts.Save)]
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleRange("",DefaultContexts.Save,2020, 2200)]
        [Custom("DisplayFormat", "d")]
        [Custom("EditMask", "d")]
        public int Year
        {
            get { return _Year; }
            set { SetPropertyValue("Year", ref _Year, value);
                if (!IsLoading && !IsSaving && _Year > 2019) {
                    StartDate = new DateTime(_Year - 1, 12, 9);
                    EndDate = new DateTime(_Year, 12, 8);
                }
            }
        }

        [Custom("AllowEdit", "False")]
        public DateTime StartDate
        {
            get { return _StartDate; }
            set
            {
                SetPropertyValue("StartDate", ref _StartDate, value);
            }
        }

        [Custom("AllowEdit", "False")]
        public DateTime EndDate
        {
            get { return _EndDate; }
            set { SetPropertyValue("EndDate", ref _EndDate, value); }
        }

        [DisplayName("Dec Last Yr")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int DecLastYr
        {
            get { return _DecLastYr; }
            set
            {
                SetPropertyValue("DecLastYr", ref _DecLastYr, value); 
            }
        }
        [DisplayName("January")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int JanThisYr
        {
            get { return _JanThisYr; }
            set
            {
                SetPropertyValue("JanThisYr", ref _JanThisYr, value);
            }
        }
        [DisplayName("February")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int FebThisYr
        {
            get { return _FebThisYr; }
            set
            {
                SetPropertyValue("FebThisYr", ref _FebThisYr, value);
            }
        }
        [DisplayName("March")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int MarThisYr
        {
            get { return _MarThisYr; }
            set
            {
                SetPropertyValue("MarThisYr", ref _MarThisYr, value);
            }
        }
        [DisplayName("April")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int AprThisYr
        {
            get { return _AprThisYr; }
            set
            {
                SetPropertyValue("AprThisYr", ref _AprThisYr, value);
            }
        }
        [DisplayName("May")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int MayThisYr
        {
            get { return _MayThisYr; }
            set
            {
                SetPropertyValue("MayThisYr", ref _MayThisYr, value);
            }
        }
        [DisplayName("June")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int JunThisYr
        {
            get { return _JunThisYr; }
            set
            {
                SetPropertyValue("JunThisYr", ref _JunThisYr, value);
            }
        }
        [DisplayName("July")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int JulThisYr
        {
            get { return _JulThisYr; }
            set
            {
                SetPropertyValue("JulThisYr", ref _JulThisYr, value);
            }
        }
        [DisplayName("August")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int AugThisYr
        {
            get { return _AugThisYr; }
            set
            {
                SetPropertyValue("AugThisYr", ref _AugThisYr, value);
            }
        }
        [DisplayName("September")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int SepThisYr
        {
            get { return _SepThisYr; }
            set
            {
                SetPropertyValue("SepThisYr", ref _SepThisYr, value);
            }
        }
        [DisplayName("October")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int OctThisYr
        {
            get { return _OctThisYr; }
            set
            {
                SetPropertyValue("OctThisYr", ref _OctThisYr, value);
            }
        }
        [DisplayName("November")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int NovThisYr
        {
            get { return _NovThisYr; }
            set
            {
                SetPropertyValue("NovThisYr", ref _NovThisYr, value);
            }
        }
        [DisplayName("December")]
        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int DecThisYr
        {
            get { return _DecThisYr; }
            set
            {
                SetPropertyValue("DecThisYr", ref _DecThisYr, value);
            }
        }
        [Association("ThirteenthGeneratorDetails"), Aggregated]
        public XPCollection<ThirteenthGeneratorDetail> ThirteenthGeneratorDetails
        {
            get
            {
                return GetCollection<ThirteenthGeneratorDetail>
                    ("ThirteenthGeneratorDetails");
            }
        }
        [PersistentAlias("DecLastYr + JanThisYr + FebThisYr + MarThisYr + AprThisYr + MayThisYr + JunThisYr + JulThisYr + AugThisYr + SepThisYr + OctThisYr + NovThisYr + DecThisYr")]
        [Custom("DisplayFormat", "d")]
        public int Total
        {
            get
            {
                object tempObject = EvaluateAlias("Total");
                if (tempObject != null)
                {
                    return (int)tempObject;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Custom("DisplayFormat", "d")]
        [Custom("AllowEdit", "False")]
        public int TotalWithoutSat
        {
            get { return _TotalWithoutSat; }
            set
            {
                SetPropertyValue("TotalWithoutSat", ref _TotalWithoutSat, value);
            }
        }
        
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        [System.ComponentModel.Browsable(false)]
        public string CreatedBy
        {
            get { return createdBy; }
            set { SetPropertyValue("CreatedBy", ref createdBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime CreatedOn
        {
            get { return createdOn; }
            set { SetPropertyValue("CreatedOn", ref createdOn, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public string ModifiedBy
        {
            get { return modifiedBy; }
            set { SetPropertyValue("ModifiedBy", ref modifiedBy, value); }
        }

        [System.ComponentModel.Browsable(false)]
        public DateTime ModifiedOn
        {
            get { return modifiedOn; }
            set { SetPropertyValue("ModifiedOn", ref modifiedOn, value); }
        }

        #endregion

        public ThirteenthGeneratorHeader(Session session)
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
            Year = DateTime.Now.Year;
            StartDate = new DateTime(DateTime.Now.Year - 1, 12, 9);
            EndDate = new DateTime(DateTime.Now.Year, 12, 8);
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

        protected override void OnSaving()
        {
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
