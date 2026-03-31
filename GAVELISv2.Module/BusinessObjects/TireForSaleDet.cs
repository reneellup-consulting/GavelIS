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
    [RuleCombinationOfPropertiesIsUnique("",DefaultContexts.Save,"DocNo,TireNo")]
    public class TireForSaleDet : XPObject {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        private TireForSale _DocNo;
        private Tire _TireNo;
        private string _Description;
        private string _SerialBrandingNo;
        private string _Remarks;
        private decimal _Price;
        private bool _Released = false;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TireForSaleDetails")]
        public TireForSale DocNo {
            get { return _DocNo; }
            set { SetPropertyValue<TireForSale>("DocNo", ref _DocNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Tire TireNo {
            get { return _TireNo; }
            set { SetPropertyValue<Tire>("TireNo", ref _TireNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Serial/Branding No.")]
        public string SerialBrandingNo {
            get { return _SerialBrandingNo; }
            set { SetPropertyValue<string>("SerialBrandingNo", ref _SerialBrandingNo, value); }
        }

        // Findings/Recommendations
        [Size(500)]
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        [Custom("DisplayFormat", "n")]
        public decimal Price {
            get { return _Price; }
            set { SetPropertyValue<decimal>("Price", ref _Price, value); }
        }

        public bool Released {
            get { return _Released; }
            set { SetPropertyValue<bool>("Released", ref _Released, value); }
        }

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

        public TireForSaleDet(Session session)
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

        protected override void OnDeleting()
        {
            this.TireNo.CarriedOut = this.TireNo.LastCarriedOut;
            this.TireNo.LastDetail.TfsId = null;
            this.TireNo.LastDetail.Save();
            base.OnDeleting();
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
