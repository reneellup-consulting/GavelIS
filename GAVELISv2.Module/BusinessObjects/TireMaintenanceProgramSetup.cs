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
    //[System.ComponentModel.DefaultProperty("DisplayName")]
    public class TireMaintenanceProgramSetup : XPObject {
        private MonthsEnum _ProgramMonth;
        private int _ProgramYear;
        private decimal _RecapPercent;
        private decimal _RecapTargetQty;
        private decimal _RecapActualQty;
        private decimal _RecapVariance;
        private decimal _ScrapPercent;
        private decimal _ScrapTargetQty;
        private decimal _ScrapActualQty;
        private decimal _ScrapVariance;
        private decimal _RetFailPercent;
        private decimal _RetFailTargetQty;
        private decimal _RetFailActualQty;
        private decimal _RetFailVariance;
        [DisplayName("Month")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public MonthsEnum ProgramMonth {
            get { return _ProgramMonth; }
            set { SetPropertyValue<MonthsEnum>("ProgramMonth", ref _ProgramMonth, value); }
        }

        [DisplayName("Year")]
        //[Custom("DisplayFormat", "d")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public int ProgramYear {
            get { return _ProgramYear; }
            set { SetPropertyValue<int>("ProgramYear", ref _ProgramYear, value); }
        }

        [DisplayName("Recap %")]
        //[Custom("DisplayFormat", "{0:n}%")]
        public decimal RecapPercent {
            get { return _RecapPercent; }
            set { SetPropertyValue<decimal>("RecapPercent", ref _RecapPercent, value); }
        }

        [DisplayName("Recap(QTY) Tgt.")]
        [Custom("DisplayFormat", "n")]
        public decimal RecapTargetQty {
            get { return _RecapTargetQty; }
            set { SetPropertyValue<decimal>("RecapTargetQty", ref _RecapTargetQty, value); }
        }

        [DisplayName("Recap(QTY) Act.")]
        [Custom("DisplayFormat", "n")]
        public decimal RecapActualQty {
            get { return _RecapActualQty; }
            set { SetPropertyValue<decimal>("RecapActualQty", ref _RecapActualQty, value); }
        }

        [DisplayName("Recap Var.")]
        [Custom("DisplayFormat", "n")]
        public decimal RecapVariance {
            get { return _RecapVariance; }
            set { SetPropertyValue<decimal>("RecapVariance", ref _RecapVariance, value); }
        }

        [DisplayName("Scrap %")]
        //[Custom("DisplayFormat", "{0:n}%")]
        public decimal ScrapPercent {
            get { return _ScrapPercent; }
            set { SetPropertyValue<decimal>("ScrapPercent", ref _ScrapPercent, value); }
        }

        [DisplayName("Srap(QTY) Tgt.")]
        [Custom("DisplayFormat", "n")]
        public decimal ScrapTargetQty {
            get { return _ScrapTargetQty; }
            set { SetPropertyValue<decimal>("ScrapTargetQty", ref _ScrapTargetQty, value); }
        }

        [DisplayName("Scrap(QTY) Act.")]
        [Custom("DisplayFormat", "n")]
        public decimal ScrapActualQty {
            get { return _ScrapActualQty; }
            set { SetPropertyValue<decimal>("ScrapActualQty", ref _ScrapActualQty, value); }
        }

        [DisplayName("Scrap Var.")]
        [Custom("DisplayFormat", "n")]
        public decimal ScrapVariance {
            get { return _ScrapVariance; }
            set { SetPropertyValue<decimal>("ScrapVariance", ref _ScrapVariance, value); }
        }

        [DisplayName("Ret. Fail %")]
        //[Custom("DisplayFormat", "{0:n}%")]
        public decimal RetFailPercent {
            get { return _RetFailPercent; }
            set { SetPropertyValue<decimal>("RetFailPercent", ref _RetFailPercent, value); }
        }

        [DisplayName("Ret. Fail(QTY) Tgt.")]
        [Custom("DisplayFormat", "n")]
        public decimal RetFailTargetQty {
            get { return _RetFailTargetQty; }
            set { SetPropertyValue<decimal>("RetFailTargetQty", ref _RetFailTargetQty, value); }
        }
        [DisplayName("Ret. Fail(QTY) Act.")]
        [Custom("DisplayFormat", "n")]
        public decimal RetFailActualQty {
            get { return _RetFailActualQty; }
            set { SetPropertyValue<decimal>("RetFailActualQty", ref _RetFailActualQty, value); }
        }
        [DisplayName("Ret. Fail Var.")]
        [Custom("DisplayFormat", "n")]
        public decimal RetFailVariance {
            get { return _RetFailVariance; }
            set { SetPropertyValue<decimal>("RetFailVariance", ref _RetFailVariance, value); }
        }

        //[Association("TireMaintenanceProgramDetails")]
        //public XPCollection<TireServiceDetail2> TireMaintenanceProgramDetails
        //{
        //    get
        //    {
        //        return GetCollection<TireServiceDetail2>(
        //            "TireMaintenanceProgramDetails");
        //    }
        //}
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

        public TireMaintenanceProgramSetup(Session session)
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
    }

}
