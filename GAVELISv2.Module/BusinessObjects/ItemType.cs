using System;
using System.ComponentModel;

using DevExpress.Xpo;
using DevExpress.Data.Filtering;

using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace GAVELISv2.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DeferredDeletion(false)]
    [CreatableItem(false)]
    [System.ComponentModel.DefaultProperty("DisplayName")]
    public class ItemType : XPObject
    {
        private ItemTypeEnum _TypeOfItem;
        private string _DisplayName;
        public ItemTypeEnum TypeOfItem
        {
            get { return _TypeOfItem; }
            set { SetPropertyValue("TypeOfItem", ref _TypeOfItem, value); }
        }
        public string DisplayName
        {
            get { return _DisplayName; }
            set { SetPropertyValue("DisplayName", ref _DisplayName, value); }
        }
        public ItemType(Session session)
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
