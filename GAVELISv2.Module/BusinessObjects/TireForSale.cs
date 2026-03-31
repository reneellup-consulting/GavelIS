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
    public enum TireForSaleStatusEnum
    {
        Current,
        Released,
        PartialReleased
    }
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("DocNo")]
    public class TireForSale : XPObject {
        [NonPersistent]
        [Custom("AllowEdit", "False")]
        public string DocNo {
            get { return string.Format("{0:TFS000000}", Oid); }
        }

        private PhysicalAdjustment _PhysicalAdjDoc;
        private Receipt _TireReceiptDoc;
        private Warehouse _ToWarehouse;
        private TireCarryOutTypeEnum _CarryOutAction;
        private DateTime _EntryDate = DateTime.Now;
        private TireForSaleStatusEnum _Status;
        public PhysicalAdjustment PhysicalAdjDoc {
            get { return _PhysicalAdjDoc; }
            set { SetPropertyValue<PhysicalAdjustment>("PhysicalAdjDoc", ref _PhysicalAdjDoc, value); }
        }

        public Receipt TireReceiptDoc {
            get { return _TireReceiptDoc; }
            set { SetPropertyValue<Receipt>("TireReceiptDoc", ref _TireReceiptDoc, value); }
        }

        public Warehouse ToWarehouse {
            get { return _ToWarehouse; }
            set { SetPropertyValue<Warehouse>("ToWarehouse", ref _ToWarehouse, value); }
        }

        [Custom("AllowEdit", "False")]
        public TireCarryOutTypeEnum CarryOutAction {
            get { return _CarryOutAction; }
            set { SetPropertyValue<TireCarryOutTypeEnum>("CarryOutAction", ref _CarryOutAction, value); }
        }

        [RuleRequiredField]
        public DateTime EntryDate {
            get { return _EntryDate; }
            set { SetPropertyValue<DateTime>("EntryDate", ref _EntryDate, value); }
        }

        public TireForSaleStatusEnum Status {
            get
            {
                try
                {
                    var list = this.TireForSaleDetails.Where(o => o.Oid > 0 && o.Released);
                    if (list != null && list.Count() != 0 && TireForSaleDetails.Count != list.Count())
                    {
                        return TireForSaleStatusEnum.PartialReleased;
                    } else if (list != null && TireForSaleDetails.Count == list.Count())
                    {
                        return TireForSaleStatusEnum.Released;
                    } else
                    {
                        return TireForSaleStatusEnum.Current;
                    }
                } catch (Exception)
                {
                    return TireForSaleStatusEnum.Current;
                }
            }
            //set { SetPropertyValue<TireForSaleStatusEnum>("Status", ref _Status, value); }
        }

        [Aggregated,
        Association("TireForSaleDetails")]
        public XPCollection<TireForSaleDet> TireForSaleDetails {
            get { return GetCollection<TireForSaleDet>("TireForSaleDetails"
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

        public TireForSale(Session session)
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
