using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    public class DriverDeductionDetails : XPObject
    {
        private Guid _RowID;
        private DriverPayroll2 _DriverPayrollId;
        private string _LineCaption;
        private decimal _LineAmount;
        //private decimal _RemBalance;

        [Custom("AllowEdit", "False")]
        public Guid RowID
        {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [Association("DriverPayroll2-DeductionsSummary")]
        [Custom("AllowEdit", "False")]
        public DriverPayroll2 DriverPayrollId
        {
            get { return _DriverPayrollId; }
            set { SetPropertyValue("DriverPayrollId", ref _DriverPayrollId, value); }
        }
        // Detailed charges attachment
        // Witholding Tax:  Cash Adv.:
        // SSS Cont.:       Higas-Genset:
        // SSS Loan:        Higas-Tractor:
        // Philhealth:      Tools:
        // Pag-ibig Cont:   Damages:
        // Pag-ibig Loan:   Others:
        //                  Cash Adv. for Misc. Exp.:

        // LineCaption
        [Custom("AllowEdit", "False")]
        public string LineCaption
        {
            get { return _LineCaption; }
            set { SetPropertyValue("LineCaption", ref _LineCaption, value); }
        }
        // LineAmount
        [Custom("AllowEdit", "False")]
        public decimal LineAmount
        {
            get { return _LineAmount; }
            set { SetPropertyValue("LineAmount", ref _LineAmount, value); }
        }

        // Remaining Balance
        //[Custom("AllowEdit", "False")]
        //public decimal RemBalance
        //{
        //    get { return _RemBalance; }
        //    set { SetPropertyValue("RemBalance", ref _RemBalance, value); }
        //}

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

        public DriverDeductionDetails(Session session)
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
    }

}
