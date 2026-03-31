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
    public class HaulCategorySet : XPObject
    {
        private HaulCategory _ParentId;
        private decimal _SetPoint = 1;
        private string _SetCode;
        private decimal _SetPay;
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Category")]
        public HaulCategory ParentId
        {
            get { return _ParentId; }
            set
            {
                SetPropertyValue("ParentId", ref _ParentId, value)
                    ;
            }
        }
        // SetPoint
        public decimal SetPoint
        {
            get { return _SetPoint; }
            set
            {
                SetPropertyValue("SetPoint", ref _SetPoint, value)
                    ;
            }
        }

        // SetPay
        public decimal SetPay
        {
            get { return _SetPay; }
            set
            {
                SetPropertyValue("SetPay", ref _SetPay, value)
                    ;
            }
        }

        // SetCode
        [NonPersistent()]
        public string SetCode
        {
            get { return _SetCode; }
        }

        [Aggregated,
        Association("HaulCategorySet-SetDetails")]
        public XPCollection<HaulCategorySetDetail> SetDetails
        {
            get
            {
                return GetCollection<HaulCategorySetDetail>(
                  "SetDetails");
            }
        }
        public HaulCategorySet(Session session)
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
