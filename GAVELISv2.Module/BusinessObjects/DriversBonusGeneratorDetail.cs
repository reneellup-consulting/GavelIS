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
    public class DriversBonusGeneratorDetail : XPObject
    {
        private DriversBonusGeneratorHeader _ParentID;
        private BusinessObjects.Employee _Driver;
        private decimal _Bonus;
        private EmploymentStatusEnum _DriverStatus;
        private BusinessObjects.DriverClass _DriverClass;
        private BusinessObjects.DriverClassification _DriverClassification;
        [Custom("AllowEdit", "False")]
        [Association("DriversBonusGeneratorDetails")]
        public DriversBonusGeneratorHeader ParentID
        {
            get { return _ParentID; }
            set { SetPropertyValue("ParentID", ref _ParentID, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        public Employee Driver
        {
            get { return _Driver; }
            set { SetPropertyValue("Driver", ref _Driver, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public EmploymentStatusEnum DriverStatus
        {
            get { return _DriverStatus; }
            set { SetPropertyValue("DriverStatus", ref _DriverStatus, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DriverClass DriverClass
        {
            get { return _DriverClass; }
            set { SetPropertyValue("DriverClass", ref _DriverClass, value); }
        }
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public DriverClassification DriverClassification
        {
            get { return _DriverClassification; }
            set { SetPropertyValue("DriverClassification", ref _DriverClassification, value); }
        }
        public decimal Bonus
        {
            get { return _Bonus; }
            set
            {
                SetPropertyValue("Bonus", ref _Bonus, value);
            }
        }
        public DriversBonusGeneratorDetail(Session session)
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
