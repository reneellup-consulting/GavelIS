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
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class TruckUnitType : BaseObject
    {
        private const string defaultDisplayFormat = "{Code}->{Description}";
        private string _Code;
        private string _Description;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string Code
        {
            get { return _Code; }
            set { SetPropertyValue("Code", ref _Code, value);
            if (!IsLoading && !IsSaving)
            {
                UpdateOnline = true;
            }
            }
        }
        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description
        {
            get { return _Description; }
            set { SetPropertyValue("Description", ref _Description, value);
            if (!IsLoading && !IsSaving)
            {
                UpdateOnline = true;
            }
            }
        }
        [Custom("AllowEdit", "False")]
        public bool UpdateOnline
        {
            get { return _UpdateOnline; }
            set { SetPropertyValue("UpdateOnline", ref _UpdateOnline, value); }
        }
        public decimal LtrsPerKm
        {
            get { return _LtrsPerKm; }
            set { SetPropertyValue("LtrsPerKm", ref _LtrsPerKm, value); }
        }
        [Association("UnitTypeFleets")]
        public XPCollection<FATruck> UnitTypeFleets
        {
            get
            {
                return
                    GetCollection<FATruck>("UnitTypeFleets");
            }
        }
        [Association("UnitTypeFuelAllocations")]
        public XPCollection<TariffFuelAllocation> UnitTypeFuelAllocations
        {
            get
            {
                return
                    GetCollection<TariffFuelAllocation>("UnitTypeFuelAllocations");
            }
        }
        #region Records Creation
        private string createdBy;
        private DateTime createdOn;
        private string modifiedBy;
        private DateTime modifiedOn;
        private bool _UpdateOnline;
        private decimal _LtrsPerKm;
        private bool _FmsUpdate;
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
        #region Display String
        public string DisplayName
        {
            get
            {
                return ObjectFormatter.Format(
                    defaultDisplayFormat, this, EmptyEntriesMode.
                    RemoveDelimeterWhenEntryIsEmpty);
            }
        }
        #endregion

        #region Hyper FMS

        // FmsUpdate
        public bool FmsUpdate
        {
            get { return _FmsUpdate; }
            set { SetPropertyValue("FmsUpdate", ref _FmsUpdate, value); }
        }

        #endregion

        public TruckUnitType(Session session)
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
