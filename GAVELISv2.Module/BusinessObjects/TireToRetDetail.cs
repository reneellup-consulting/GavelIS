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
    public class TireToRetDetail : XPObject {
        private Guid _RowID;
        [Custom("AllowEdit", "False")]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue<Guid>("RowID", ref _RowID, value); }
        }

        private TireToRetreader _DocNo;
        private Tire _TireNo;
        private string _Description;
        private string _SerialBrandingNo;
        private TreadStatus _PreferredType;
        private bool _Regrooved = false;
        private string _Remarks;
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("TireToRetDetails")]
        public TireToRetreader DocNo {
            get { return _DocNo; }
            set { SetPropertyValue<TireToRetreader>("DocNo", ref _DocNo, value); }
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

        public TreadStatus PreferredType {
            get { return _PreferredType; }
            set { SetPropertyValue<TreadStatus>("PreferredType", ref _PreferredType, value); }
        }

        public bool Regrooved {
            get { return _Regrooved; }
            set { SetPropertyValue<bool>("Regrooved", ref _Regrooved, value); }
        }

        // Findings/Recommendations
        [Size(500)]
        public string Remarks {
            get { return _Remarks; }
            set { SetPropertyValue<string>("Remarks", ref _Remarks, value); }
        }

        private ReceiptDetail _ReceiptDetailId;
        [Custom("AllowEdit", "False")]
        public ReceiptDetail ReceiptDetailId {
            get { return _ReceiptDetailId; }
            set { SetPropertyValue<ReceiptDetail>("ReceiptDetailId", ref _ReceiptDetailId, value); }
        }

        [NonPersistent]
        [DisplayName("On Receipt")]
        public bool Received {
            get { return _ReceiptDetailId != null ? true : false; }
        }

        private bool _Completed = false;
        [Custom("AllowEdit", "False")]
        public bool Completed {
            get { return _Completed; }
            set { SetPropertyValue<bool>("Completed", ref _Completed, value); }
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

        public TireToRetDetail(Session session)
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
