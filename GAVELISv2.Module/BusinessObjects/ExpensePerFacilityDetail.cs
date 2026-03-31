using System;
using System.Collections;
using System.Collections.Generic;
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
    public class ExpensePerFacilityDetail : XPObject
    {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }
        private ExpensePerFacility _MainId;
        [Custom("AllowEdit", "False")]
        [Association("ExpensePerFacilityDetails")]
        public ExpensePerFacility MainId
        {
            get { return _MainId; }
            set { SetPropertyValue("MainId", ref _MainId, value); }
        }
        private string _Seq;
        [Custom("AllowEdit", "False")]
        public string Seq
        {
            get { return _Seq; }
            set { SetPropertyValue("Seq", ref _Seq, value); }
        }
        
        private Department _Department;
        private Employee _Head;
        private Employee _InCharge;
        private decimal _January;
        private decimal _JanPercent;
        private decimal _February;
        private decimal _FebPercent;
        private decimal _March;
        private decimal _MarPercent;
        private decimal _April;
        private decimal _AprPercent;
        private decimal _May;
        private decimal _MayPercent;
        private decimal _June;
        private decimal _JunPercent;
        private decimal _July;
        private decimal _JulPercent;
        private decimal _August;
        private decimal _AugPercent;
        private decimal _September;
        private decimal _SepPercent;
        private decimal _October;
        private decimal _OctPercent;
        private decimal _November;
        private decimal _NovPercent;
        private decimal _December;
        private decimal _DecPercent;
        private string _JanKeys;
        private string _FebKeys;
        private string _MarKeys;
        private string _AprKeys;
        private string _MayKeys;
        private string _JunKeys;
        private string _JulKeys;
        private string _AugKeys;
        private string _SepKeys;
        private string _OctKeys;
        private string _NovKeys;
        private string _DecKeys;
        [Custom("AllowEdit", "False")]
        public Department Department
        {
            get { return _Department; }
            set { SetPropertyValue("Department", ref _Department, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee Head
        {
            get { return _Head; }
            set { SetPropertyValue("Head", ref _Head, value); }
        }
        [Custom("AllowEdit", "False")]
        public Employee InCharge
        {
            get { return _InCharge; }
            set { SetPropertyValue("InCharge", ref _InCharge, value); }
        }
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal January
        {
            get { return _January; }
            set { SetPropertyValue("January", ref _January, value); }
        }
        [DisplayName("Jan %")]
        [PersistentAlias("(January/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal JanPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("JanPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal February
        {
            get { return _February; }
            set { SetPropertyValue("February", ref _February, value); }
        }
        [DisplayName("Feb %")]
        [PersistentAlias("(February/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal FebPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("FebPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal March
        {
            get { return _March; }
            set { SetPropertyValue("March", ref _March, value); }
        }
        [DisplayName("Mar %")]
        [PersistentAlias("(March/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal MarPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("MarPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal April
        {
            get { return _April; }
            set { SetPropertyValue("April", ref _April, value); }
        }
        [DisplayName("Apr %")]
        [PersistentAlias("(April/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal AprPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("AprPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal May
        {
            get { return _May; }
            set { SetPropertyValue("May", ref _May, value); }
        }
        [DisplayName("May %")]
        [PersistentAlias("(May/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal MayPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("MayPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal June
        {
            get { return _June; }
            set { SetPropertyValue("June", ref _June, value); }
        }
        [DisplayName("Jun %")]
        [PersistentAlias("(June/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal JunPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("JunPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal July
        {
            get { return _July; }
            set { SetPropertyValue("July", ref _July, value); }
        }
        [DisplayName("Jul %")]
        [PersistentAlias("(July/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal JulPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("JulPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal August
        {
            get { return _August; }
            set { SetPropertyValue("August", ref _August, value); }
        }
        [DisplayName("Aug %")]
        [PersistentAlias("(August/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal AugPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("AugPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal September
        {
            get { return _September; }
            set { SetPropertyValue("September", ref _September, value); }
        }
        [DisplayName("Sep %")]
        [PersistentAlias("(September/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal SepPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("SepPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal October
        {
            get { return _October; }
            set { SetPropertyValue("October", ref _October, value); }
        }
        [DisplayName("Oct %")]
        [PersistentAlias("(October/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal OctPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("OctPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal November
        {
            get { return _November; }
            set { SetPropertyValue("November", ref _November, value); }
        }
        [DisplayName("Nov %")]
        [PersistentAlias("(November/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal NovPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("NovPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [Custom("AllowEdit", "False")]
        [Custom("DisplayFormat", "n")]
        public decimal December
        {
            get { return _December; }
            set { SetPropertyValue("December", ref _December, value); }
        }
        [DisplayName("Dec %")]
        [PersistentAlias("(December/Total) * 100")]
        [Custom("DisplayFormat", "n")]
        public decimal DecPercent
        {
            get
            {
                try
                {
                    object tempObject = EvaluateAlias("DecPercent");
                    if (tempObject != null)
                    {
                        return (decimal)tempObject;
                    }
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
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string JanKeys
        {
            get { return _JanKeys; }
            set { SetPropertyValue("JanKeys", ref _JanKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string FebKeys
        {
            get { return _FebKeys; }
            set { SetPropertyValue("FebKeys", ref _FebKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string MarKeys
        {
            get { return _MarKeys; }
            set { SetPropertyValue("MarKeys", ref _MarKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string AprKeys
        {
            get { return _AprKeys; }
            set { SetPropertyValue("AprKeys", ref _AprKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string MayKeys
        {
            get { return _MayKeys; }
            set { SetPropertyValue("MayKeys", ref _MayKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string JunKeys
        {
            get { return _JunKeys; }
            set { SetPropertyValue("JunKeys", ref _JunKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string JulKeys
        {
            get { return _JulKeys; }
            set { SetPropertyValue("JulKeys", ref _JulKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string AugKeys
        {
            get { return _AugKeys; }
            set { SetPropertyValue("AugKeys", ref _AugKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string SepKeys
        {
            get { return _SepKeys; }
            set { SetPropertyValue("SepKeys", ref _SepKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string OctKeys
        {
            get { return _OctKeys; }
            set { SetPropertyValue("OctKeys", ref _OctKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string NovKeys
        {
            get { return _NovKeys; }
            set { SetPropertyValue("NovKeys", ref _NovKeys, value); }
        }
        [System.ComponentModel.Browsable(false)]
        [Size(SizeAttribute.Unlimited)]
        public string DecKeys
        {
            get { return _DecKeys; }
            set { SetPropertyValue("DecKeys", ref _DecKeys, value); }
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
        public ExpensePerFacilityDetail(Session session)
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
            RowID = Guid.NewGuid();
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
