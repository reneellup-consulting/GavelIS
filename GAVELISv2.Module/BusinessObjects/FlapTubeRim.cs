using System;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;

namespace GAVELISv2.Module.BusinessObjects {
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class FlapTubeRim : XPObject {
        private Guid _RowID;
        private string _PartNo;
        private string _Description;
        private PartMake _Make;
        private PartType _Type;
        private Vendor _PurchaseFrom;
        private DateTime _PurchaseDate;
        private string _SerialNo;
        private PartSize _Size;
        private string _InvoiceNo;
        private decimal _Cost;

        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public Guid RowID {
            get { return _RowID; }
            set { SetPropertyValue("RowID", ref _RowID, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string PartNo {
            get { return _PartNo; }
            set { SetPropertyValue<string>("PartNo", ref _PartNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public string Description {
            get { return _Description; }
            set { SetPropertyValue<string>("Description", ref _Description, value); }
        }

        public PartMake Make {
            get { return _Make; }
            set { SetPropertyValue<PartMake>("Make", ref _Make, value); }
        }

        public PartType Type {
            get { return _Type; }
            set { SetPropertyValue<PartType>("Type", ref _Type, value); }
        }

        public Vendor PurchaseFrom {
            get { return _PurchaseFrom; }
            set { SetPropertyValue<Vendor>("PurchaseFrom", ref _PurchaseFrom, value); }
        }

        public DateTime PurchaseDate {
            get { return _PurchaseDate; }
            set { SetPropertyValue<DateTime>("PurchaseDate", ref _PurchaseDate, value); }
        }

        public string SerialNo {
            get { return _SerialNo; }
            set { SetPropertyValue<string>("SerialNo", ref _SerialNo, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public PartSize Size {
            get { return _Size; }
            set { SetPropertyValue<PartSize>("Size", ref _Size, value); }
        }

        public string InvoiceNo {
            get { return _InvoiceNo; }
            set { SetPropertyValue<string>("InvoiceNo", ref _InvoiceNo, value); }
        }

        public decimal Cost {
            get { return _Cost; }
            set { SetPropertyValue<decimal>("Cost", ref _Cost, value); }
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

        public FlapTubeRim(Session session)
            : base(session) {
        }

        public override void AfterConstruction() {
            base.AfterConstruction();

            RowID = Guid.NewGuid();

            #region Saving Creation

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                CreatedBy = currentUser.UserName;
                CreatedOn = DateTime.Now;
            }

            #endregion


        }

        protected override void OnSaving() {
            base.OnSaving();

            #region Saving Modified

            if (SecuritySystem.CurrentUser != null)
            {
                var currentUser = Session.GetObjectByKey<SecurityUser>(
                Session.GetKeyValue(SecuritySystem.CurrentUser));
                ModifiedBy = currentUser.UserName;
                ModifiedOn = DateTime.Now;
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
