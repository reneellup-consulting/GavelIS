using System;
using System.Linq;
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

namespace GAVELISv2.Module.BusinessObjects
{
    public enum TireToRetStatusEnum
    {
        Current,
        Processed,
        PartiallyCompleted,
        Completed
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("DocNo")]
    public class TireToRetreader : XPObject {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string DocNo {
            get { return string.Format("{0:TOR000000}", Oid); }
        }
        private string _ReceivingNo;
        private TireCarryOutTypeEnum _CarryOutAction;
        private DateTime _EntryDate = DateTime.Now;
        private Vendor _Vendor;
        private string _Address;
        private CostCenter _CostCenter;
        private TireToRetStatusEnum _Status;
        [RuleRequiredField("", DefaultContexts.Save)]
        [RuleUniqueValue("", DefaultContexts.Save)]
        [EditorAlias("UpperCaseStringWinPropertyEditor")]
        public string ReceivingNo
        {
            get { return _ReceivingNo; }
            set { SetPropertyValue("ReceivingNo", ref _ReceivingNo, value); }
        }
        
        [Custom("AllowEdit", "False")]
        public TireCarryOutTypeEnum CarryOutAction {
            get { return _CarryOutAction; }
            set { SetPropertyValue<TireCarryOutTypeEnum>("CarryOutAction", ref _CarryOutAction, value); }
        }

        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue<DateTime>("EntryDate", ref _EntryDate, value); }
        }

        [RuleRequiredField("", DefaultContexts.Save)]
        public Vendor Vendor {
            get { return _Vendor; }
            set
            {
                SetPropertyValue<Vendor>("Vendor", ref _Vendor, value);
                if (!IsLoading && !IsSaving && _Vendor != null)
                {
                    Address = _Vendor.FullAddress;
                }
            }
        }

        [Custom("AllowEdit", "False")]
        [Size(500)]
        public string Address {
            get { return _Address; }
            set { SetPropertyValue<string>("Address", ref _Address, value); }
        }
        [DisplayName("Charge To")]
        [RuleRequiredField("", DefaultContexts.Save)]
        public CostCenter CostCenter {
            get { return _CostCenter; }
            set { SetPropertyValue<CostCenter>("CostCenter", ref _CostCenter, value); }
        }
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public TireToRetStatusEnum Status {
            get
            {
                try
                {
                    var list = this.TireToRetDetails.Select(o => o);
                    if (list.Where(o => o.Received == true).Count() == 0)
                    {
                        return TireToRetStatusEnum.Current;
                    } else if (list.Where(o => o.Received == true).Count() > 0 && list.Where(o => o.Completed == true).Count() == 0)
                    {
                        return TireToRetStatusEnum.Processed;
                    } else if (list.Where(o => o.Received == true).Count() > 0 && list.Where(o => o.Completed == true).Count() > 0 && list.Where(o => o.Completed == true).Count() != list.Count())
                    {
                        return TireToRetStatusEnum.PartiallyCompleted;
                    } else if (list.Where(o => o.Received == true).Count() > 0 && list.Where(o => o.Completed == true).Count() == list.Count())
                    {
                        return TireToRetStatusEnum.Completed;
                    } else
                    {
                        return TireToRetStatusEnum.Current;
                    }
                } catch (Exception)
                {
                    return TireToRetStatusEnum.Current;
                }
            }
        }

        [Aggregated,
        Association("TireToRetDetails")]
        public XPCollection<TireToRetDetail> TireToRetDetails {
            get { return GetCollection<TireToRetDetail>("TireToRetDetails"
                ); }
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

        public TireToRetreader(Session session)
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
