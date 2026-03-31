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
    public class HaulCategorySetDetail : XPObject
    {
        private HaulCategorySet _SetId;
        private HaulCategorySet _CategoryId;
        private int _SeqNo = 0;
        // SetId
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [Association("HaulCategorySet-SetDetails")]
        public HaulCategorySet SetId
        {
            get { return _SetId; }
            set
            {
                SetPropertyValue("SetId", ref _SetId, value)
                    ;
            }
        }
        // CategoryId
        [Custom("AllowEdit", "False")]
        [RuleRequiredField("", DefaultContexts.Save)]
        [DisplayName("Category")]
        public HaulCategorySet CategoryId
        {
            get { return _CategoryId; }
            set
            {
                SetPropertyValue("CategoryId", ref _CategoryId, value)
                    ;
            }
        }
        // SeqNo
        public int SeqNo
        {
            get { return _SeqNo; }
            set
            {
                SetPropertyValue("SeqNo", ref _SeqNo, value)
                    ;
            }
        }
        public HaulCategorySetDetail(Session session)
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
