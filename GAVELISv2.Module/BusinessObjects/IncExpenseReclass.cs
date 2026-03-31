using System;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Reports;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.Security;
using System.Globalization;

namespace GAVELISv2.Module.BusinessObjects
{
    public enum IncExpReclassStateEnum
    {
        Current,
        Approved,
        Completed
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    public class IncExpenseReclass : XPObject
    {
        private IncExpReclassStateEnum _Status;
        private DateTime _EntryDate = DateTime.Now;
        private DateTime _ApprovedDate;
        private string _ApprovedBy;
        private DateTime _CompletedDate;
        private string _CompletedBy;
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        [DisplayName("Viewer No.")]
        public string ViewerNo
        {
            get { return string.Format("{0:TIVR00000000}", Oid); }
        }
        [Custom("AllowEdit", "False")]
        public IncExpReclassStateEnum Status
        {
            get
            {
                return _Status;
            }
            set
            {
                SetPropertyValue("Status", ref _Status, value);
                if (!IsLoading && !IsSaving)
                {
                    switch (_Status)
                    {
                        case IncExpReclassStateEnum.Current:
                            break;
                        case IncExpReclassStateEnum.Approved:
                            ApprovedBy = CurrentUser.UserName;
                            ApprovedDate = DateTime.Now;
                            Remarks = "Has been approved";
                            break;
                        case IncExpReclassStateEnum.Completed:
                            CompletedBy = CurrentUser.UserName;
                            CompletedDate = DateTime.Now;
                            Remarks = "Already completed";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        [Custom("AllowEdit", "False")]
        [Size(100)]
        public string Remarks
        {
            get
            {
                return _Remarks;
            }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime EntryDate
        {
            get
            {
                return _EntryDate;
            }
            set
            {
                SetPropertyValue("EntryDate", ref _EntryDate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime ApprovedDate
        {
            get
            {
                return _ApprovedDate;
            }
            set
            {
                SetPropertyValue("ApprovedDate", ref _ApprovedDate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string ApprovedBy
        {
            get
            {
                return _ApprovedBy;
            }
            set
            {
                SetPropertyValue("ApprovedBy", ref _ApprovedBy, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public DateTime CompletedDate
        {
            get
            {
                return _CompletedDate;
            }
            set
            {
                SetPropertyValue("CompletedDate", ref _CompletedDate, value);
            }
        }
        [Custom("AllowEdit", "False")]
        public string CompletedBy
        {
            get
            {
                return _CompletedBy;
            }
            set
            {
                SetPropertyValue("CompletedBy", ref _CompletedBy, value);
            }
        }

        #region Aggregated Collection

        [Aggregated,
        Association("IncExpenseReclass-Details")]
        public XPCollection<IncExpenseReclassDetail> IncExpenseReclassDetails
        {
            get
            {
                return
                    GetCollection<IncExpenseReclassDetail>("IncExpenseReclassDetails");
            }
        }

        #endregion

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

        public IncExpenseReclass(Session session)
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
        private string _Remarks;
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
